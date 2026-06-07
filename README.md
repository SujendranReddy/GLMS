# Global Logistics Management System (GLMS)

This is an ASP.NET Core system for managing clients, contracts, and service requests.

The system is now split into:

* MVC frontend
* Web API backend
* SQL Server database

The MVC frontend uses `HttpClient` to call the Web API.
The Web API handles the database, business logic, authentication, and file handling.

## Features:

* Client Management
  Create, edit, view, and delete clients.
  Stores name, email, phone no., and region.

* Contract Management
  Contracts are linked to clients.
  Create, edit, view, delete, and filter contracts.
  Upload and download signed agreement PDFs.
  Unique file storage using GUID naming.

* Service Requests
  Linked to contracts.
  USD cost automatically converted to ZAR.
  Workflow validation prevents requests on expired or on-hold contracts.

* Filtering
  Contracts can be filtered by status and date range.

* Web API Backend
  API handles database access and business logic.
  Returns JSON data to the MVC frontend.
  Includes endpoints for clients, contracts, and service requests.

* Authentication
  JWT authentication is used to protect API endpoints.
  The MVC frontend gets a token and sends it with API requests.

* Swagger/OpenAPI
  Swagger is enabled for testing API endpoints.
  JWT can be used in Swagger to test protected endpoints.

* External API Integration
  Uses an exchange rate API to convert USD to ZAR.

* Automated Testing
  xUnit tests are included for:

  * currency conversion
  * file validation
  * workflow rules
  * API integration tests
  * JWT login
  * protected endpoints
  * create, read, and delete API test

* Docker Compose
  Docker Compose runs the full system with:

  * sql-server-db
  * glms-backend-api
  * glms-frontend-web

## Technologies Used:

* ASP.NET Core MVC
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* HttpClient
* JWT Authentication
* Swagger/OpenAPI
* xUnit
* Docker
* Docker Compose
* GitHub Actions
* Bootstrap

## Design Patterns:

* Factory Pattern – used for contract creation
* Observer Pattern – used for logging contract events
* Repository Pattern – separates database access from controllers
* Service Layer Pattern – separates business logic from controllers

## How to Run Normally:

1. Clone the repository
2. Open the solution in Visual Studio
3. Run `GLMS.Api`
4. Run the MVC project
5. Open the MVC frontend in the browser

## How to Run with Docker:

1. Open Docker Desktop
2. Open a terminal in the project root folder
3. Run:

```cmd
docker compose build
docker compose up
```

4. Open the MVC frontend:

```text
http://localhost:8080
```

5. Open Swagger:

```text
http://localhost:8081/swagger
```

## Notes:

Only PDF files are accepted for contract agreements.

Service requests cannot be created for contracts that are:

* Expired
* On Hold

The MVC frontend no longer connects directly to the SQL database.
It calls the Web API instead.

The Web API connects to the SQL Server database.

Docker Compose uses internal networking so the containers can communicate.

## YouTube Link:

