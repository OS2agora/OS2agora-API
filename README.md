# OS2agora - Hearing portal - API

This repository is the API and backend used in OS2agora.  
The repository is part of the overall solution, which also consists of two frontends.

The frontend for employees is located in [OS2agora-Internal-UI](https://github.com/OS2agora/OS2agora-Internal-UI)  
The frontend for the public is located in [OS2agora-Public-UI](https://github.com/OS2agora/OS2agora-Public-UI)

Technical documentation is located in [OS2agora-docs](https://github.com/OS2agora/OS2agora-docs)

A complete setup for running the entire solution on your local machine is provided in [OS2agora-Infrastructure](https://github.com/OS2agora/OS2agora-Infrastructure)

## Technologies

- ASP.NET 6.0
- [Entity Framework Core 6.0](https://docs.microsoft.com/en-us/ef/core/)
- [MediatR](https://github.com/jbogard/MediatR)
- [AutoMapper](https://automapper.org/)
- [FluentValidation](https://fluentvalidation.net/)
- [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq)
- [Docker](https://www.docker.com/)
- [WSL2](https://docs.microsoft.com/en-us/windows/wsl/install-win10)

## Required Tools

- Visual Studio (or another IDE of your choice)
- .NET 6.0 SDK
- Docker
- GIT
- WSL 2 with Ubuntu (or another Linux distribution of your choise)
- (optional) Docker-compose (is included in Docker for Windows)
- (optional) MySQL Workbench

## Getting started

The solution is seperated into multiple projects, each having a `README.md` describing how each project is used.  
The architecture of the solution is heavily inspired by [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)

### Running the solution locally

1. Install [required tools](#required-tools)
2. Download solution from [Git repository](https://github.com/OS2agora/OS2agora-API)
3. Navigate to the repository root folder and run the following command:

   ```cmd
   dotnet restore ./src/Api/Api.csproj
   ```

4. In Visual Studio, right click on the `Api` project and select `Manage User Secrets` and add relevant secrets
5. Start required external dependencies, by following the relevant guide in [OS2agora-Infrastructure](https://github.com/OS2agora/OS2agora-Infrastructure)
6. In Visual Studio, right click on the `Api` project and select `Set as Startup Project`
7. In Visual Studio, press `f5` to start the solution. After it has started, it should open a browser and navigate to the swagger interface.

### Running the solution locally in Docker

To run the entire OS2Agora solution in Docker, follow the instructions provided in the [OS2agora-Infrastructure](https://github.com/OS2agora/OS2agora-Infrastructure) repository.

### Building the _agora-api_ Docker Image

To run the entire OS2Agora solution, you need to build the _agora-api_ Docker Image from this repository. To build the image, follow these steps:

1. Navigate to the `docker` folder
2. Run the following command ```docker compose -f docker-compose.build.yml build api```

The Dockerfile is located in `/docker/image/app`

## Configurations

Before the configuration is created, any `.env.local` file in `app/src/Api`, `app/src` or the `app` directory are sourced using [DotEnv](https://github.com/bolorundurowb/dotenv.net). These are meant for developer overrides and are thus ignored by git.

The application is configured through appsettings with a few non-standard providers. The following is the order of layering in the IConfiguration

1. appsettings.json is applied
2. Environment variables are applied. This implies normal interpretation of these according to settings, so for instance the environment variable `OAuth2__InternalRedirectUri` would override any value of the `OAuth2:InternalRedirectUri` key from a `.json` file.
3. For each file in the folder `/run/secrets`, the file name is loaded as the variable name and the content of the file is loaded as the value. These are used for secrets in docker and applied overrides similar to regular environment variables.
4. Finally, any options provided via the command line when starting the application are applied.

Mind that this is a chain of overrides so the last on the list is the most significant (will override).

### Configuration of Authentication

The solution supports multiple authentication mechanism via the `IAuthenticationHandler` interface. The handler to use can be configured for both the internal and public frontend via the `Authentication__InternalAuthentication` and `Authentication__PublicAuthentication` settings respectively. The following configurations are currently available:

- `OAuth` (uses an authorization code flow mechanism)
- `NemLogin` (uses OIOSAML3)

### Secrets

Secrets are dealt with in two ways, depending on the environment.

#### Docker

When executing in a local Docker environment, secrets should be stored in files that are located inside the `docker/secrets` directory. Read more about this in the README.md file located there.

#### Local development

When running the application outside docker, for instance in Visual studio or with the `dotnet` command, use the `.env.local` file in the `app` directory. For example, for setting the connection string for mysql, you could have

```.env.local
ConnectionStrings__DefaultConnection=server=localhost;port=3306;Database=agora_database;User=agora_user;Password=agora_password;default command timeout=120
```

## Api projects overview

### API

This layer is .NET6 API project. This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only _Startup.cs_ should reference Infrastructure.

### DAO's

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

### Models

This will contain all models, enums, exceptions, interfaces, types and logic specific to the domain layer.

### Operations

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within DAO's.

## Logs and debugging

### Configuration debugging

When the environment is development, the endpoint `/debug-config` of the API returns the configuration of the API.

## License

This project is licensed with the [MIT license](LICENSE)
