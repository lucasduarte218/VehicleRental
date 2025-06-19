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
