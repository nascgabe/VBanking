# VBanking API

## 📌 Descrição
API para cadastro de contas bancárias, transferência de fundos e inativação de contas.

## 🚀 Como Rodar Localmente

### 1️⃣ Requisitos
- [.NET 8](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [pgAdmin](https://www.pgadmin.org/download/)

### 2 Configuração do Banco
1. Crie um banco no **pgAdmin** com o nome `VBankingDB`.
2. Atualize `appsettings.json` com a conexão:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=VBankingDB;Username=postgres;Password=password"
  }
}
```
## Instalando Dependências e Rodando Migration
1. `dotnet restore`
2. `dotnet build`
3. `dotnet ef migrations add InitialCreate`
4. `dotnet ef database update`

### 3 Iniciar API
`dotnet run`

### ▶️ POST `/api/accounts`

Cria uma nova conta.

#### 🧾 Exemplo de Request

```json
{
  "name": "Gabriel",
  "document": "17595876787"
}
```

#### ✅ Response Esperada

```json
"b3b7ee62-96af-44bc-98e2-eaf07edd7390"
```

---

### ▶️ POST `/api/accounts/transfer`

Cria uma transferência entre contas.

#### 🧾 Exemplo de Request

```json
{
  "fromDocument": "17863033705",
  "toDocument": "65432198700",
  "amount": 500
}
```

#### ✅ Response Esperada

```json
Transferência realizada com sucesso.
```

---

### 🔍 GET `/api/accounts/{document}`

Busca uma conta existente.

#### 🧾 Exemplo de Request

```json
{
  "document": 17685676534
}
```

#### ✅ Response Esperada

```json
{
  "id": "2d187bb2-18e4-4d62-a59f-bc8cf18c89d7",
  "name": "Gabriel",
  "document": "17863033705",
  "balance": 1000,
  "createdAt": "2025-04-24T02:14:52.770659Z",
  "isActive": true
}
```

---

### ✏️ PUT `/api/accounts/{document}/deactivate`

Inativa uma conta existente e registra na tabela de auditoria.

#### 🧾 Exemplo de Request

```json
{
  "document": 17685676534
}
```

#### ✅ Response Esperada

```json
{
  status: 204
}
```
