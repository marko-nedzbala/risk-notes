# Portfolio Summary
Built and deployed a Dockerized ASP.NET Core risk-management application with PostgreSQL, Identity authentication, REST APIs, Swagger documentation, file uploads, CSV/Excel export, dashboard analytics, pagination, and cloud deployment on Render.

# RiskNotes
RiskNotes is a full-stack ASP.NET Core MVC application for tracking and managing risk notes. The app includes user authentication, PostgreSQL persistence, REST API endpoints, Swagger documentation, file attachments, CSV/Excel export, dashboard charts, pagination, Docker support, and cloud deployment on Render.

## Features
- ASP.NET Core MVC web application
- Entity Framework Core data access
- PostgreSQL production database
- SQLite-compatible development setup
- ASP.NET Core Identity authentication
- User-specific risk notes
- Create, read, update, and delete risk notes
- File attachment upload and download
- CSV export
- Excel export
- Dashboard with severity chart
- Search, sorting, and pagination
- REST API endpoints
- Swagger API documentation
- Dockerized deployment
- Render cloud deployment

## Tech Stack
- C#
- ASP.NET Core MVC
- Entity Framework Core
- ASP.NET Core Identity
- PostgreSQL
- SQLite
- Docker
- Bootstrap
- Chart.js
- ClosedXML
- Swagger / Swashbuckle
- Render

### Run locally with .NET
```bash
# The commands to run locally .NET
dotnet restore
dotnet ef database update
dotnet run

# Then open your browser to:
http://localhost:5000
```

### Docker
```bash
# Build the image:
docker build -t risknotes .

# Run the container:
docker run -p 8080:8080 risknotes

# Open your browser to:
http://localhost:8080
```

### Docker Compose
```bash
# Run the app with Docker Compose:
docker compose up --build

# Stop the app:
docker compose down

# Remove volumes:
docker compose down -v
```
### Database
The app supports:
- SQLite for local development
- PostgreSQL for Render deployment

For Render, the app reads the PostgreSQL connection from:
- DATABASE_URL

Render Deployment Notes
- The deployed version uses:
    - Docker runtime
    - Render Web Service
    - Render PostgreSQL
    - DATABASE_URL environment variable
    - Manual EF Core migration against Render PostgreSQL
    - UTC timestamps for PostgreSQL compatibility

#### Important production notes:
- Use the Render Internal Database URL for the deployed app.
- Use the Render External Database URL when running migrations locally.
- Use DateTime.UtcNow instead of DateTime.Now for PostgreSQL timestamp compatibility.

### Running Migrations Against Render PostgreSQL

#### On Mac/Linux:
```bash
export DATABASE_URL='postgresql://USER:PASSWORD@HOST:PORT/DATABASE'
dotnet ef database update
```

#### On Windows PowerShell:
```bash
$env:DATABASE_URL="postgresql://USER:PASSWORD@HOST:PORT/DATABASE"
dotnet ef database update
```

### API Endpoints
```
GET     /api/risknotes
GET     /api/risknotes/{id}
POST    /api/risknotes
PUT     /api/risknotes/{id}
DELETE  /api/risknotes/{id}
```


