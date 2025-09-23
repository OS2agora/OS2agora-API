# OS2AGORA - Hearing portal

This repository is the API and backend used in the OS2AGORA project.  
The repository is part of the solution, which also consists of two frontends.

The frontend for employees is located in [Github](https://github.com/OS2agora/Agora-Internal-UI)  
The frontend for the public is located in [Github](https://github.com/OS2agora/Agora-Public-UI)

## Technologies

- ASP.NET Core 3.1
- [Entity Framework Core 3.1](https://docs.microsoft.com/en-us/ef/core/)
- [MediatR](https://github.com/jbogard/MediatR)
- [AutoMapper](https://automapper.org/)
- [FluentValidation](https://fluentvalidation.net/)
- [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq)
- [Docker](https://www.docker.com/)
- [WSL2](https://docs.microsoft.com/en-us/windows/wsl/install-win10)

## Getting started

The solution is seperated into projects, which each have a README.md which describes how each project is meant to be used.  
The architecture of the solution is heavily inspired by [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)

### Running the solution locally

1. Install required tools
    * Install Visual Studio (or another IDE of your choice)
    * Install .NET Core 3.1 SDK
    * Install Docker
    * (optional) Install docker-compose (is included in Docker for Windows)
    * Install GIT
    * Install WSL 2 with Ubuntu (or another Linux distribution of your choise)
    * (optional) Install MySQL Workbench
2. Download solution from [Git repository](https://bitbucket.org/novataris/hoeringsportal/src/development/)

3. Navigate to the repository root folder and run the following command:

   ```cmd
   dotnet restore ./src/Api/Api.csproj
   ```

4. Install `AntlrVSIX` extension for Visual Studio -  Used for `NovaSec`
    * Open Visual Studio
    * Click 'Extensions' in top
    * Click 'Manage Extensions'
    * Click 'Online' tab
    * Find and install `AntlrVSIX`
    * Restart Visual Studio
5. In Visual Studio, right click on the `Api` project and select `Manage User Secrets`. You can find the appsettings file under Teams -> NovaCare -> General or the process to it:

6. Add secrets to the files in the `docker/secrets/` folder as described in the README.md found there. The docker-compose secrets mentioned in that file are required to perform the next step.

7. a) To run the Api, all it's external dependencies must be met. There are different permutations, but for development it means running the database locally and mocking everything else.
   In order to start all dependencies for the Api locally, navigate to the `<root-path>/docker` folder and start the services marked as "mock" and "base"
   ```base
   $ docker-compose -f docker-compose.local.yml --env-file environments/local/.env --profile base --profile mock up -d
   ```
   If you want the services running in the background without the console attached for logging, add the `-d` parameter lastly to the command line

7. b) 11. If you want to run Jaeger alongside the required dependencies for improved debugging, run the following command:
    ```bash
    $ docker-compose -f docker-compose.local.yml --env-file environments/local/.env --profile logs up -d
    ```

8. Validate that containers are up and running by executing the following command
   ```bash
   $ docker ps
   ```
9. In Visual Studio, right click on the `Api` project and select `Set as Startup Project`

10. In Visual Studio, press `f5` to start the solution. After it has started, it should open a browser and navigate to the swagger interface.

### Running the solution locally in Docker

In order to quickly spin up the solution, use the docker compose profiles addressed for this. Profiles available are

* _all_: Every container the makes up the solution. Note that this includes containers for the front end projects which require those images have been built locally beforehand (using the docker-compose.development.yml spec)
* _base_: Contains just the database and clamav virus scanner containers. These are necessary for running the application in any configuration.
* _logs_: Includes the Jaeger container for intercepting and presenting opentelemetry metrics from the running Api
* _mock_: Mocks that require external services. These are e-mail and the identity provider (IdP)
* _be_: The container with the Api image
* _fe_: All containers that constitute the front end part of the project. These include the proxy in front of the front-ends, as well as both the internal and the public front end.

Any profile can be addressed directly with docker-compose for pulling, starting, stopping and removing containers. 

For example, starting external dependencies for local Api development would address `mock`, `base`
```bash
$ docker-compose -f docker-compose.local.yml --env-file environments/local/.env --profile mock --profile base up -d
```
Stopping it again would be
```bash
$ docker-compose -f docker-compose.local.yml --env-file environments/local/.env --profile mock --profile base stop
```
And removing the containers entirely (not including volumes) is 
```bash
$ docker-compose -f docker-compose.local.yml --env-file environments/local/.env --profile mock --profile base down
```

Starting the Api part - all docker - to support local frontend development would be 
```bash
$ docker-compose -f docker-compose.local.yml --env-file environments/local/.env --profile mock --profile base --profile be up -d
```

Adding dockerized front ends on top of local Api development can be done by running
```bash
$ docker-compose -f docker-compose.local.yml --env-file environments/local/.env --profile fe up -d
```

For convenience and reference, there are scripts in `<root-path>/docker/scripts` for starting, stopping, removing local containers. 
These are `local_all_start.bat`, `local_all_stop.bat` and `local_all_clear_containers.bat`. 

### Overview of Docker compose files

Docker execution is organized in several docker-compose specification files optimized for readability. There are two categories

#### Solution compose file

These are for running a solution. The configurations are similar, but meant for different purposes.

* _docker-compose.local.yml_: Local development of any part of the solution. This means open port mappings for individual services so they're easily accessible and applying mocks.
* _docker-compose.server.yml_: Running a server environment. Main difference is no mocks, few open ports and reliance on an existing docker network for patching to the solution.

#### Specification files

These files are not meant for running directly, but for DRY specification of the containers used overall.

* _docker-compose.build.yml_: Any image the solution requires to run and that must be build, i.e. not standard images with volumed configuration files but actual images.
* _docker-compose.base.yml_: Base configuration of database and clamav
* _docker-compose.metrics.yml_: Configuration of Jaeger for displaying opentelemetry metrics
* _docker-compose.mocks.yml_: Configuration of e-mail, IdP mock and reverse proxy servers.

### Overview of Docker images

All images the solution can produce _or_ modifies for use are kept in the `<root-path>/docker/image` folder. Each folder has one image.

#### Images that can be built

* _app_: The backend application itself
* _idpmock_: Small application that constitute the mock for the IdP. See [Idp-Mock/Idp-Mock/README.md](IdP-Mock - README)
* _proxy_: The reverse proxy placed in front of the backend and frontend containers

#### Only configuration for standard Docker images

* _balkproxy_: Configuration for a container required for the setup. This does automatic reverse proxying as docker containers come and go.
* _balkproxymock_: A mock of balkproxy so the server environment can be run locally.
* _clamav_: Starting Clamav with custom configuration file.

### Overview of scripts

All scripts are kept in `<root-path>/docker/scripts`. There are two kinds of scripts

#### Local

All local script are minded for development and in turn only available as .bat files.

```cmd
REM Start every container in the solution

local_all_start.bat

REM Stop every container in the solution

local_all_stop.bat

REM Remove every container in the solution

local_all_clear_containers.bat

REM Nuke the current database and recreate it with seeded data

local_recreate_db_with_test_data.bat
```

#### Server

These scripts are for maintaining the server part. They are identical and come in a bash and Cmd variety

This is the Windows variant

```cmd
REM Copy all ASP DataProtection keys used for encrypting the database to a local dir on the host. Parameters are
REM 1. environment: bktest | bkprod
REM 2. target folder: path to any folder relative or absolution where the keys should be placed

server_dump_dataprotection_keys.bat bktest keys

REM Activate mysqldump on the running database container and capture entire database as sql to stdout. Parameters are
REM 1. environment: bktest | bkprod

server_dump_db.bat bktest > dbdump.sql

REM Start the server using the images that are locally available. Parameters are
REM 1. environment: bktest | bkprod

server_start.bat bktest

REM Stop all containers in the solution. Parameters are
REM 1. environment: bktest | bkprod

server_stop.bat bktest

REM Pull newest images and spin up or recreate containers to use them. Parameters are
REM 1. environment: bktest | bkprod

server_update.bat bktest
```

These are the bash scripts for linux or WSL

```bash
# Copy all ASP DataProtection keys used for encrypting the database to a local dir on the host. Parameters are
# 1. environment: bktest | bkprod
# 2. target folder: path to any folder relative or absolution where the keys should be placed

$ ./server_dump_dataprotection_keys.sh bktest keys

# Activate mysqldump on the running database container and capture entire database as sql to stdout. Parameters are
# 1. environment: bktest | bkprod

$ ./server_dump_db.sh bktest > dbdump.sql

# Start the server using the images that are locally available. Parameters are
# 1. environment: bktest | bkprod

$ ./server_start.sh bktest

# Stop all containers in the solution. Parameters are
# 1. environment: bktest | bkprod

$ server_stop.sh bktest

# Pull newest images and spin up or recreate containers to use them. Parameters are
# 1. environment: bktest | bkprod

$ ./server_update.sh bktest
```

## Configuration

Before the configuration is created, any `.env.local` file in `app/src/Api`, `app/src` or the `app` directory are sourced using [DotEnv](https://github.com/bolorundurowb/dotenv.net). These are meant for developer overrides and are thus ignored by git.

The application is configured though appsettings with a few non-standard providers. The following is the order of layering in the IConfiguration

1. appsettings.json is applied
2. If a value was provided for the APPSETTINGS_PROFILE, a second appsettings.${APPSETTINGS_PROFILE}.json is applied
3. Environment variables are applied. This implies normal interpretation of these according to settings, so for instance the environment variable `OAuth2__InternalRedirectUri` would override any value of the `OAuth2:InternalRedirectUri` key from a `.json` file.
4. For each file in the folder `/run/secrets`, the file name is loaded as the variable name and the content of the file is loaded as the value. These are used for secrets in docker and applied overrides similar to regular environment variables.
5. Finally, any options provided via the command line when starting the application are applied.

Mind that this is a chain of overrides so the last on the list is the most significant (will override).

### Strategy for appsettings.${APPSETTINGS_PROFILE}.json files

The point of these files are to provide complete sets of overrides that are targeted at a specific environment.
When running the application in a server environment, one of these sets are chosen via the APPSETTINGS_PROFILE variable.

This leaves the main point of the `appsettings.json` file to provide sane defaults for all keys when they're expected to have no variance or, to provide defaults for the local development environment. Consequently, a targeted appsettings file should override every variable that would vary from local development.

### Secrets

Secrets are dealt with in two ways, depending on the environment. 

#### Docker

When executing in a local Docker environment, secrets should be stored in files that are located inside the `docker/secrets` directory. Read more about this in the README.md file located there.

#### Local development

When running the application outside docker, for instance in Visual studio or with the `dotnet` command, use the `.env.local` file in the `app` directory. For example, for setting the connection string for mysql, you could have

```.env.local
ConnectionStrings__DefaultConnection=server=localhost;port=3306;Database=bk_database;User=bkhpuser;Password=bkhppassword;default command timeout=120
```

## Deployment

### Ballerup Test and Ballerup Prod

Ballerup is currently running the solution in Docker. The infrastructure around pushing code to their serveres are revolved around the following services:

Deployment repository: [GIT-Service](https://git-k8s.itf-services.dk/novataris/hoeringsportalen)  
Docker-image repository: [Harbor](https://dockerregistry-k8s.itf-services.dk/harbor/projects/9/repositories)  
Kubernetes management panel: [Argo](https://argo-k8s.itf-services.dk/applications)

All services requires a registered user in their KeyCloack.  
It also requires that you are a member of the `novataris` group.

When running the solution in Docker:
Secrets are inject using enviornment variables and docker-secrets.
Secrets are stored in files on the server.

When running the solution in Kubernetes:
Secrets are injected using environment variables in the Helm Chart.  
Secrets are stored in Kubernetes, and we do not have access to them.

To change or create new secrets, we need to contact IT-Forsygningen.

#### Deploy procedure

- Push new code to this repository, and either build it manually in [Novataris Bamboo](https://ci.novataris.com) or create a PR to development, and let it happen automatically
- Access [Harbor](https://dockerregistry-k8s.itf-services.dk/harbor/projects/9/repositories) to get the correct tag for the docker image you want to deploy - Note that this is the build-number from [Novataris Bamboo](https://ci.novataris.com)
- Change the correct `tag` in the [GIT-Repository](https://git-k8s.itf-services.dk/novataris/hoeringsportalen) - Remember to do it in the correct folder matching this project - The folder is called `api`
- Push the `tag` changes to master
- Go to [Argo](https://argo-k8s.itf-services.dk/applications)
- Refresh the application you want to deploy changes to
- Sync the application you want to deploy changes to
- Now everything should be deployed!

## Api projects overview

### Models

This will contain all models, enums, exceptions, interfaces, types and logic specific to the domain layer.

### Operations

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within DAO's.

### DAO's

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

### API

This layer is .NET Core 3.1 API project. This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only _Startup.cs_ should reference Infrastructure.

## Logs and debugging

### Elmah

[ElmahCore](https://github.com/ElmahCore/ElmahCore) is configured for the API and provides logging of unhandled 
exceptions and a user interface with more information on the exception. It is found at the endpoint `elmah` of the API.

For exceptions coming from failed requests, the cookie information has been redacted to avoid anyone being able to do a 
session hijacking.

### Configuration debugging

When the environment is development, the endpoint `/debug-config` of the API returns the configuration of the API.

## License

This project is licensed with the [MIT license](LICENSE).
