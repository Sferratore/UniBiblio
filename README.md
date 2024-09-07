# UniBiblio
UniBiblio is a web-based library management system that allows users to manage book reservations, room bookings, and provides administrative functionalities for libraries. This project is built with ASP.NET Core and Entity Framework Core.

## Features
- User and admin dashboard for managing reservations.
- Book reservation and room booking system.
- Monthly statistics reporting for both books and room usage.

## Requirements
- .NET 6 SDK
- MySQL Database
- Visual Studio or other compatible IDE
- MySQL Connector for .NET
  
## Installation
### 1. Clone the Repository
`
git clone https://github.com/yourusername/UniBiblio.git
cd UniBiblio
`
### 2. Configure the Database
Create a MySQL database for the project, and run the SQL scripts provided in the UniBiblio folder:

tabelle_db.sql: Contains the table structure.
viste_db.sql: Creates the views for data visualization.
storedprocedures_db.sql: Creates the stored procedures for managing complex queries.
eventi_db.sql: Contains database events, such as automatic task triggers.
strutture_etl_db.sql: Contains the ETL structure for data processing and reporting.

### 3. Set Up the Configuration
Update the appsettings.json file to include your database connection details:

`
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=unibiblio;User=root;Password=yourpassword;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
`

### 4. Scaffold the Database Context
You can use DB scaffholding to ensure coherence between the code and the DB structures in use (they are already aligned but just in case there is the command):

`
dotnet ef dbcontext scaffold "Server=localhost;Database=unibiblio;User=root;Password=yourpassword;" Pomelo.EntityFrameworkCore.MySql -o Models
`

### 5. Build and Run the Application
To run the application, use the following commands:

`
dotnet build
dotnet run
`

### 6. Access the Application
Once the project is running, access the application at https://localhost:5001.

### 7. Seed the Database (Optional)
You can modify the Program.cs file to add initial seed data for users, books, and rooms. This is useful for testing and initial setup.

## Database Structure
The database structure is split into multiple components:

- Tables: Core entities include Utenti, Libri, Sale_Studio, Prenotazioni_Libri, and Prenotazioni_Sale.
- Views: Predefined queries such as PrenotazioniEffettuateLibriView and PrenotazioniEffettuateSaleView are used for reporting.
- Stored Procedures: Procedures like InsertMonthlyStatistics allow automatic calculation of monthly statistics for books and rooms.
- ETL Processes: strutture_etl_db.sql defines the structure for ETL processes, used to generate monthly reports and aggregate data.

## Technical Overview
- Backend: ASP.NET Core with MVC and Razor Pages for rendering dynamic content.
- Database: MySQL, managed with Entity Framework Core and raw SQL scripts for stored procedures and events.
- Front-end: Uses Bootstrap for responsive design and jQuery for client-side interactions.
- Authentication: Handled via ASP.NET Identity, allowing for user roles (admin, user) and secure login.
