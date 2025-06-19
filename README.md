# VehicleRental - Plataforma de Gestão de Aluguel de Motocicletas 🏍️

O **VehicleRental** é uma plataforma robusta e moderna para gerenciamento de aluguel de motocicletas. Projetado com foco em escalabilidade e alta disponibilidade, o sistema utiliza uma arquitetura baseada em microsserviços, construída com .NET 8 e tecnologias cloud-native.

---

## ⚙️ Configuração Inicial

### 🗄️ MongoDB

O banco de dados principal utilizado é o **MongoDB**. Configure a seção `MongoDbSettings` no `appsettings.json`:

```json
"MongoDbSettings": {
  "ConnectionString": "mongodb://usuario:senha@host:porta",
  "Database": "VehicleRentalDB"
}
```

### ✉️ Amazon SQS (Mensageria)

A API utiliza **Amazon SQS** para envio de eventos assíncronos:

```json
"AwsSQSSettings": {
  "QueueUrl": "https://sqs.us-east-1.amazonaws.com/123456789012/vehiclerental-events",
  "Key": "SUA_AWS_ACCESS_KEY_ID",
  "Secret": "SUA_AWS_SECRET_ACCESS_KEY"
}
```

### 🖼️ Armazenamento de Imagens

O tipo de armazenamento pode ser definido por meio da chave `imageStorageService`:

```json
"imageStorageService": "mongodb" // ou "s3"
```

#### Configuração para S3:

```json
"AwsS3Settings": {
  "BucketName": "vehiclerental-images",
  "Key": "SUA_AWS_ACCESS_KEY_ID",
  "Secret": "SUA_AWS_SECRET_ACCESS_KEY"
}
```

---

## 🧱 Estrutura do Projeto

```plaintext
VehicleRental/
├── API/
│   ├── VehicleRental.API/            # API REST principal
│   ├── VehicleRental.Application/    # Casos de uso e lógica de aplicação
│   ├── VehicleRental.Domain/         # Regras de negócio
│   └── VehicleRental.Infrastructure/ # Repositórios, serviços externos
├── Microservices/
│   └── VehicleRental.Notifier/       # Microsserviço para notificações (Lambda)
├── Tests/
│   └── VehicleRental.Tests/          # Testes automatizados
```

---

## 🚀 Endpoints da API

### 👤 Entregadores (`/entregadores`)
- `POST /entregadores`: Cadastro com validação de CNPJ, CNH e idade mínima.
- `POST /entregadores/{id}/cnh`: Atualiza a imagem da CNH.

### 🏍️ Motocicletas (`/motos`)
- `POST /motos`: Cadastra nova moto, valida dados e publica evento.
- `GET /motos`: Lista motos disponíveis.
- `PUT /motos/{id}/placa`: Atualiza a placa da motocicleta.

### 🔄 Locação (`/locacao`)
- `POST /locacao`: Registra aluguel com validações e cálculos.
- `GET /locacao/{id}`: Retorna dados da locação.
- `PUT /locacao/{id}/devolucao`: Processa devolução com penalidades se aplicável.

---

## 🧰 Tecnologias Utilizadas

- ASP.NET Core 8
- MongoDB com EntityFrameworkCore
- AutoMapper
- Swagger / OpenAPI
- FluentValidation
- AWS SQS / S3 / Lambda
- Middleware global de exceções
- Arquitetura em camadas (Domain, Application, Infrastructure)

---

## 🧩 Middlewares de Logging e Tratamento de Exceções

A API conta com middlewares personalizados para:

### 🔍 Logging de Requisições

Captura detalhes de cada requisição e resposta (headers, body, status code) e salva logs estruturados, possibilitando rastreabilidade e análise posterior.

### ⚠️ Tratamento de Exceções

Intercepta exceções não tratadas, evita que cheguem ao cliente, e registra os dados relevantes (mensagem, stacktrace, etc.) via `IExceptionLogRepository`.

Esses middlewares garantem:
- Observabilidade
- Segurança na exposição de erros
- Registro centralizado para diagnósticos

---

## 🌐 Microsserviço `VehicleRental.Notifier`

Microsserviço assíncrono implementado como função **AWS Lambda**, responsável por processar eventos da fila e encaminhar para webhooks externos.

### 📥 Evento Suportado: `MotorcycleRegisteredEvent`

```json
{
  "Identifier": "m12345",
  "Plate": "ABC1234",
  "Year": 2022,
  "Model": "Honda CG 160"
}
```

### 📤 Payload Enviado ao Webhook

```json
{
  "Timestamp": "2023-08-15T13:45:30Z",
  "LambdaFunctionName": "VehicleRentalNotifier",
  "RequestId": "c2307dde-2a1f-11e6-a530-3ca82a64ff89",
  "LogGroup": "/aws/lambda/VehicleRentalNotifier",
  "LogStream": "2023/08/15/[$LATEST]...",
  "Message": {
    "Identifier": "m12345",
    "Plate": "ABC1234",
    "Year": 2022,
    "Model": "Honda CG 160"
  },
  "Error": null
}
```

---

## 🔁 Comunicação entre Serviços

- A API publica eventos na fila SQS através da classe `MotorcycleEventPublisher`.
- A função Lambda consome eventos da fila e notifica serviços externos.

---

## ▶️ Executando a Solução

1. Configure o `appsettings.json` com os dados corretos (MongoDB, AWS).
2. Execute o projeto principal:

```bash
dotnet run --project API/VehicleRental.API/VehicleRental.API.csproj
```

3. Acesse o Swagger em:

```
https://localhost:5001/swagger
```

4. Configure os webhooks no `VehicleRental.Notifier`.

---

## 🧪 Testes

Os testes estão organizados em `VehicleRental.Tests/` e cobrem cenários de unidade e integração.

---

## 📬 Suporte & Contribuições

Sinta-se à vontade para abrir issues, enviar sugestões ou contribuir com melhorias na plataforma. 🚀
