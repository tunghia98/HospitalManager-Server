# HospitalManager-Server

This repository contains the HospitalManager-Server project, which is a web application for managing hospital operations.

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js](https://nodejs.org/) (for the WebUI)
- [Docker](https://www.docker.com/) (optional, for containerization)

### Building the Project

1. Clone the repository:
    ```sh
    git clone https://github.com/tunghia98/HospitalManager-Server
    cd HospitalManager-Server
    ```

2. Restore .NET dependencies:
    ```sh
    dotnet restore
    ```

3. Build the project:
    ```sh
    dotnet 
    ```

### Running the Project

1. Run the backend:
    ```sh
    dotnet run --project EHospital.csproj
    ```

2. Navigate to the WebUI directory and install dependencies:
    ```sh
    cd WebUI
    npm install
    ```

3. Start the frontend:
    ```sh
    npm run dev
    ```

### Running with Docker

1. Build and run the Docker containers:
    ```sh
    docker-compose up --build
    ```

## Project Details

### Backend

- **Framework**: ASP.NET Core
- **Database**: SQL Server (configured in `appsettings.json`)

### Frontend

- **Framework**: React
- **Build Tool**: Vite
- **Styling**: Bootstrap


## License

This project is licensed under the MIT License.