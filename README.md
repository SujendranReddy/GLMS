Global Logistics Management System (GLMS)
This is an ASP.NET Core MVC web application developed as amonolithic system to manage clients, contracts, and service requests.

Features:
- Client Management
  Create, edit, view, and delete clients.
  Stores names, email, phone no., and region.
- Contract Mangement
  Contracts linked to clients.
  Upload and download signed agreement PDFs.
  Unique file storage using GUID naming.
- Service Requests
  Linked to contracts.
  USD cost automatically converted to ZAR.
  Workflow validation prevents requests on inactive contracts.
- Filtering
  Contracts can be filtered by status and date range using LINQ
- External API Integration
  Uses an exchange rate API to convert USD to ZAR
- Unit Testing
  xUnit test: currency conversion
              file validation
              workflow rules

Technologies Used:
  ASP.NET Core MVC
  Entity Framework Core
  SQL Server
  xUnit
  Github Actions
  Bootstrap

Design Patterns:
Factory Pattern – used for contract creation
Observer Pattern – used for logging contract events
Service Layer Pattern – separates business logic from controllers

How to Run:
1. Clone the repository
2. Open the solution in Visual Studio
3. Update the database: Update-Database
4. Run the application

Notes:
Only PDF files are accepted for contract agreements
Service requests cannot be created for contracts that are:
Expired
On Hold

Youtube Link: https://youtu.be/TtHoj2qN9iE?si=i545Rv5kD9yIauQJ 
