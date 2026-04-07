# OmniHealth — Sistema de Gestão Hospitalar

> Versão 1.3 | C# · ASP.NET Core 8 · SQLite · Entity Framework Core

## Estrutura do Projeto

```
OmniHealth/
├── OmniHealth.sln
├── OmniHealth.API/               ← Backend (ASP.NET Core Web API)
│   ├── Controllers/              ← Endpoints REST por módulo
│   ├── Models/                   ← Entidades do banco (C#)
│   ├── DTOs/                     ← Objetos de transferência de dados
│   │   ├── Auth/
│   │   ├── Consulta/
│   │   └── Farmacia/
│   ├── Data/                     ← DbContext + Migrations
│   ├── Services/                 ← Lógica de negócio (interfaces + impl)
│   ├── Repositories/             ← Acesso ao banco (interfaces + impl)
│   ├── Middleware/               ← LoggingMiddleware
│   ├── Helpers/                  ← JwtHelper, PasswordHelper
│   ├── wwwroot/                  ← Frontend estático
│   │   ├── index.html
│   │   ├── css/
│   │   ├── js/
│   │   └── pages/
│   │       ├── auth/
│   │       ├── paciente/
│   │       ├── medico/
│   │       ├── funcionario/
│   │       └── farmaceutico/
│   ├── appsettings.json
│   └── Program.cs
└── OmniHealth.Tests/             ← Testes unitários (xUnit + Moq)
    ├── Services/
    ├── Controllers/
    └── Repositories/
```

## Como Rodar

```bash
# Restaurar dependências
dotnet restore

# Aplicar migrations e criar o banco
dotnet ef database update --project OmniHealth.API

# Rodar a API (desenvolvimento)
dotnet run --project OmniHealth.API

# Rodar testes
dotnet test
```

A API estará disponível em `https://localhost:5001` e o Swagger em `https://localhost:5001/swagger`.

## Tecnologias

| Camada     | Tecnologia                        |
|------------|-----------------------------------|
| Backend    | C# / ASP.NET Core 8               |
| ORM        | Entity Framework Core + SQLite    |
| Auth       | JWT Bearer + BCrypt (fator >= 12) |
| Frontend   | HTML + CSS + JavaScript           |
| Testes     | xUnit + Moq + EF InMemory         |
| Docs API   | Swagger / OpenAPI                 |

## Perfis de Acesso

- **Paciente** — auto-cadastro, visualiza consultas, prescrições e exames
- **Médico** — prontuários, diagnósticos, prescrições, exames
- **Funcionário Administrativo** — agendamentos, internações, relatórios
- **Farmacêutico** — estoque por lote, dispensação, alertas de validade







teste gabriel