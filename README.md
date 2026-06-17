# BeneficentEvent

Sistema de gerenciamento de eventos beneficentes desenvolvido para organizar arrecadações, leilões, bingos, doações e controle financeiro.

O objetivo do projeto é facilitar a gestão de eventos solidários, centralizando participantes, receitas, despesas e relatórios em um único sistema.

---

# Tecnologias utilizadas

# Backend
- C# 
- .NET 10.0.3
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger

# Banco de dados
- SQLite

# Frontend
*(em desenvolvimento)*
- React
- TypeScript

---

# Funcionalidades

# Eventos
- Criar eventos
- Editar eventos
- Fechamento de eventos (balancete)
- Gerenciar participantes

# Doações
- Cadastro de doações
- Controle de itens doados
- Vinculação com benfeitores

# Leilões
- Cadastro de leilões
- Controle de itens
- Registro de lances

# Bingos
- Cadastro de bingos
- Controle de prêmios
- Fechamento de bingos

# Financeiro
- Registro de receitas
- Registro de despesas
- Controle de lucro líquido
- Dashboard financeiro

# Produtos
- Cadastrar
- Editar
- Consultar
- Excluir

# Vendas
- Vender produtos
- Editar
- Consultar
- Excluir 
---

# Dashboard

O sistema possui indicadores como:

- Total de eventos
- Eventos ativos
- Eventos encerrados
- Total arrecadado
- Total de despesas
- Lucro líquido
- Ranking de eventos
- Ranking de benfeitores
- Receita mensal

---

# Arquitetura

O projeto segue uma separação por camadas:

BeneficentEvent
├── Controllers
│
├── Services
│
├── Models
│
├── DTOs
│
├── Data
│
└── Migrations

---

# Como executar

# Pré-requisitos

- .NET 10 SDK instalado
- Clone o projeto: git clone https://github.com/diegosantos451/BeneficetEvent.git
- Vá para a pasta do projeto: cd BeneficetEvent
- Restaure as dependências: dotnet restore
- Crie o banco de dados: dotnet ef database update
- Execute a API: dotnet run
- Após iniciar: http://localhost:5187/swagger
- Teste as funcionabilidades

---

# Próximas melhorias
- Finalizar frontend em React
- Autenticação de usuários
- Controle de permissões
- Relatórios em PDF
- Melhorias no dashboard

---

# Autor

Diego Santos

Projeto desenvolvido para estudo e evolução em desenvolvimento Full Stack.

<img width="1919" height="944" alt="image" src="https://github.com/user-attachments/assets/190cd488-6a98-4cb6-a84b-46b2f93fe88e" />

---

<img width="1919" height="936" alt="image" src="https://github.com/user-attachments/assets/39ace466-afc8-49de-8d87-e10ca3b1c73e" />

