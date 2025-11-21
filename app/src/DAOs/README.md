# What goes here?

This project is used for data access only.  
This project has access to the database using EntityFrameworkCore and a MySQL connector.  
This project has access to external services such as ESDH, ClamAV, FileScan, etc.

## General

The Dependency class will be used to register all dependencies DAOs needs configured in order to work.

## EntityFrameworkCore

Configuration is done using FluentValidation in a configuration file for each Entity. 
The applicationDbContext is registered to find all configurations in the assembly and use them. If an Entity does not have any configuration we should still create the file so we can add configuration in the future.  

*If an entity requires dependencies to be injected into the configuration class, it must be registered manually in the DependencyInjection file. 

### Database Migrations

To be able to run migrations, install `dotnet ef` - https://learn.microsoft.com/en-us/ef/core/cli/dotnet

To use `dotnet-ef` for your migrations please add the following flags to your command (values assume you are executing from repository root)

* `--project src/DAOs` (optional if in this folder)
* `--startup-project src/Api`
* `--output-dir Persistence/Migrations`

For example, to add a new migration from the root folder:

 `dotnet ef migrations add "SampleMigration" --project src/DAOs --startup-project src/Api --output-dir Persistence\Migrations`

### Database Update

To use ´dotnet-ef´ for updating your database, please add the following flags to your command (values assume you are executing from repository root)

* `--project src/DAOs` (optional if in this folder)
* `--startup-project src/Api`

To update the database to the latest migrations: 
 
 `dotnet ef database update --project src/DAOs --startup-project src/Api`