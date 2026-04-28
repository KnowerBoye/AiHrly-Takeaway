# AIHRly API

A .NET 9 REST API for managing job applications, built with ASP.NET Core, Entity Framework Core, and PostgreSQL.

## Table of Contents


- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Run with Docker (Recommended)](#run-with-docker-recommended)
  - [Run Locally without Docker](#run-locally-without-docker)
- [Database Setup](#database-setup)
- [Running the Tests](#running-the-tests)
- [Default Team Members](#default-team-members)
- [API Endpoints](#api-endpoints)
- [Assumptions](#assumptions)
- [Future Improvements](#future-improvements)

---


## Getting Started

### Prerequisites

- **.NET 9 SDK** – [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL 16** (if running without Docker)
- **Docker & Docker Compose** (if running with Docker)

### Run with Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repo-url>
   cd aihrly-api
   ```

2. **Create environment file**
   ```bash
   cp example.env .env
   ```
   The default values are:
   ```
   POSTGRES_USER=appuser
   POSTGRES_PASSWORD=dbpassword
   POSTGRES_DB=aihrlydb
   ```

3. **Start the containers**
   ```bash
   docker compose up --build -d
   ```

4. **Access the API**
   - API: http://localhost


5. **Stop the containers**
   ```bash
   docker compose down
   ```

### Run Locally without Docker

1. **Clone the repository**
   ```bash
   git clone <repo-url>
   cd aihrly-api
   ```

2. **Set up PostgreSQL**
   - Create a database named `aihrlydb`
   - Update the connection string in `appsettings.Development.json`:
     ```json
     {
       "ConnectionStrings": {
         "Default": "Host=localhost;Port=5432;Database=aihrlydb;Username=your_user;Password=your_password"
       }
     }
     ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the API**
   ```bash
   dotnet run
   ```

5. **Seed data**
```bash 
dotnet run seed
```
You can't run this in the container. To seed data when deploying using docker uncomment the code block in Program.cs under `// container seeder` this will run the seed script by checking SEED environment variable. Didn't want to keep it active to keep the entry file from being bloated


---

## Database Setup

The project uses **Entity Framework Core migrations**. When the API starts, it automatically runs `db.Database.Migrate()`.


---

## Running the Tests

The test project is located in the `Tests/` folder and uses **xUnit** with an **in-memory database** for isolation.

```bash
# Run all tests
dotnet test Tests/AihrlyApi.Tests.csproj





## Default Team Members

The following team members are automatically seeded once you run migrations

```json
[
    {
        "id": "00000000-0000-0000-0000-000000000001",
        "name": "Noah Boye",
        "email": "noahboye@mail.com",
        "role": "hiring_manager"
    },
    {
        "id": "00000000-0000-0000-0000-000000000002",
        "name": "Jane Doe",
        "email": "janedoe@mail.com",
        "role": "recruiter"
    },
    {
        "id": "00000000-0000-0000-0000-000000000003",
        "name": "John Boye",
        "email": "johnboye@mail.com",
        "role": "hiring_manager"
    }
]
```



## Assumptions

- Given the scope of the project I decided to go with the Minimal API rather than the MVC pattern to keep things simple and straight forward.

- The POST `/api/jobs/:id/applications` endpoint accepts a `coverLetter` field. I assumed it to be a text field instead of maybe a File. If it were to be a file the implementation would be to rather upload the file to cloud storage service and store the resource url rather in `coverLetter`

- On the initial start giving how basic the endpoints looked I decided to go with Data Annotations on my DTo's and a custom endpoint filter for validation. But along the line some endpoints required more advanced validation that Data Annotations can't handle so I hade to introduce FluentValidation given enough time I'd implement all other validations using FluentValidation to improve code maintainability 



## With More Time
- Also for the applicaiton stage transition validation I assumed terminal is not an actual stage and just means cannot be changed

- With more time I'd add XML documentations to the Service classes 
- Keep DTOs clean and move to Record based DTOs instead



## Part 2 
For the Background Job I used Hangfire and since this is not production grade version I used `MemoryStorage` instead to avoid having to spin up an SQLServer DB or anything of that sorts.
I chose Hangfire over the native BackgrounService because it's persist data. 
As the applications scales some of the options will be to move to using a message queue like RabbitMQ etc 
with that I'll spin up a separate container to handle the consumers/workers as a separate .

