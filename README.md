VehicleRental - Plataforma de Gestão de Aluguel de Veiculos
O VehicleRental é uma plataforma robusta e moderna para gerenciamento de aluguel de Veiculos. Projetado com foco em escalabilidade e alta disponibilidade, o sistema utiliza uma arquitetura baseada em microsserviços, construída com .NET 8 e tecnologias cloud-native.

🔧 Configuração Inicial
Para executar a API principal do VehicleRental, é necessário configurar corretamente os serviços externos no arquivo appsettings.json. Abaixo estão os detalhes de cada configuração:

🗄️ Banco de Dados - MongoDB
A API utiliza MongoDB como principal fonte de dados. Configure a seção MongoDbSettings:

json
Copy
Edit
"MongoDbSettings": {
  "ConnectionString": "mongodb://usuario:senha@host:porta",
  "Database": "VehicleRentalDB"
}
ConnectionString: String completa de conexão, incluindo usuário e senha.

Database: Nome do banco a ser utilizado pela aplicação.

✉️ Fila de Mensagens - AWS SQS
O sistema utiliza o Amazon SQS para publicação de eventos assíncronos. Adicione as credenciais em AwsSQSSettings:

json
Copy
Edit
"AwsSQSSettings": {
  "QueueUrl": "https://sqs.us-east-1.amazonaws.com/123456789012/vehiclerental-events",
  "Key": "SUA_AWS_ACCESS_KEY_ID",
  "Secret": "SUA_AWS_SECRET_ACCESS_KEY"
}
QueueUrl: Endereço da fila SQS.

Key / Secret: Credenciais da AWS para autenticação.

🖼️ Armazenamento de Imagens
O serviço de imagens pode ser definido por meio da chave imageStorageService, com duas opções:

"mongodb": As imagens são armazenadas diretamente no MongoDB.

"s3": Utiliza o Amazon S3 como repositório de imagens.

Configuração para Amazon S3:
json
Copy
Edit
"AwsS3Settings": {
  "BucketName": "vehiclerental-images",
  "Key": "SUA_AWS_ACCESS_KEY_ID",
  "Secret": "SUA_AWS_SECRET_ACCESS_KEY"
}
🔗 Integração com Microsserviços
A API do VehicleRental interage com o microsserviço VehicleRental.Notifier. Sempre que uma nova motocicleta é registrada, um evento é publicado na fila SQS para processamento posterior de forma assíncrona.

🧱 Estrutura da Solução
plaintext
Copy
Edit
VehicleRental/
├── API/
│   ├── VehicleRental.API/            # API principal (REST)
│   ├── VehicleRental.Application/    # Lógica de aplicação e casos de uso
│   ├── VehicleRental.Domain/         # Entidades e regras de negócio
│   └── VehicleRental.Infrastructure/ # Serviços externos e persistência
├── Microservices/
│   └── VehicleRental.Notifier/       # Microsserviço para notificações (AWS Lambda)
├── Tests/
    └── VehicleRental.Tests/          # Testes automatizados
🚀 VehicleRental API
📌 Descrição
API construída com ASP.NET Core 8, fornece endpoints RESTful para operações de negócio relacionadas ao aluguel de motos.

🔍 Endpoints principais
Entregadores (/entregadores)
POST /entregadores: Cadastra novo entregador, validando CNPJ, CNH e idade mínima (18 anos).

POST /entregadores/{id}/cnh: Atualiza imagem da CNH.

Motocicletas (/motos)
POST /motos: Cadastra nova moto, valida placa/modelo e publica evento.

GET /motos: Lista motos disponíveis com filtro por placa.

PUT /motos/{id}/placa: Altera a placa de uma moto existente.

Locação (/locacao)
POST /locacao: Registra o aluguel, verifica disponibilidade e CNH válida.

GET /locacao/{id}: Consulta locação específica.

PUT /locacao/{id}/devolucao: Registra devolução, calcula taxas e penalidades.

🛠️ Tecnologias Utilizadas
ASP.NET Core 8

MongoDB com EntityFrameworkCore

AutoMapper (mapeamento de objetos)

Swagger (documentação)

Middleware global para tratamento de exceções

FluentValidation para validação de modelos

Retorno padronizado via padrão Result

Métodos de extensão para reutilização de lógica

▶️ Executando a API
Configure o appsettings.json com suas credenciais e dados externos.

Execute o comando:

bash
Copy
Edit
dotnet run --project API/VehicleRental.API/VehicleRental.API.csproj
Acesse a documentação Swagger em:

bash
Copy
Edit
https://localhost:5001/swagger
🛰️ VehicleRental Notifier
📌 Função
Microsserviço assíncrono desenvolvido como uma função AWS Lambda, responsável por processar notificações e eventos relacionados ao sistema principal.

🔄 Funcionalidades
Processamento automático de mensagens da fila SQS

Geração e envio de payloads para webhooks

Logs centralizados no AWS CloudWatch

Extensibilidade para múltiplos tipos de eventos

🌐 Eventos Suportados
MotorcycleRegisteredEvent
Exemplo de payload:

json
Copy
Edit
{
  "Identifier": "m12345",
  "Plate": "ABC1234",
  "Year": 2022,
  "Model": "Honda CG 160"
}
📤 Payload Enviado ao Webhook
json
Copy
Edit
{
  "Timestamp": "2023-08-15T13:45:30Z",
  "LambdaFunctionName": "VehicleRentalNotifier",
  "RequestId": "c2307dde-2a1f-11e6-a530-3ca82a64ff89",
  "LogGroup": "/aws/lambda/VehicleRentalNotifier",
  "LogStream": "2023/08/15/[$LATEST]c763be94956c41e49e8c6f461e8a1b1c",
  "Message": {
    "Identifier": "m12345",
    "Plate": "ABC1234",
    "Year": 2022,
    "Model": "Honda CG 160"
  },
  "Error": null
}
🔁 Comunicação entre Serviços
A API principal utiliza a classe MotorcycleEventPublisher (na camada de infraestrutura) para publicar eventos na fila SQS, que são consumidos pela função Lambda do Notifier.

📦 Tecnologias Utilizadas
VehicleRental API
ASP.NET Core 8.0

MongoDB.EntityFrameworkCore

AutoMapper

Swagger / OpenAPI

VehicleRental Notifier
Amazon.Lambda.Core

Amazon.Lambda.Serialization.SystemTextJson

Amazon.Lambda.SQSEvents

🏁 Executando a Solução Completa
Configure as credenciais AWS para o VehicleRental.Notifier

Ajuste a string de conexão do MongoDB

Inicie a API VehicleRental

Configure os webhooks externos para receber os eventos processados
