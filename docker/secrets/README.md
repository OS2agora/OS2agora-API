# Secrets

The files listed below should be created and filled. Ask a sytem owner of the project for the correct values. 

### Application specific secrets

These secrets are required to run the docker image of the application. 

- `ConnectionStrings__DefaultConnection`
- `JwtSettings__Secret`
- `Sbsip__ClientId`
- `Sbsip__ClientSecret`
- `OAuth2__InternalClientId`
- `OAuth2__PublicClientId`
- `OAuth2__InternalClientSecret`
- `OAuth2__PublicClientSecret`
- `DataScanner__Token`
- `EBoks__ClientId`
- `EBoks__ClientSecret`
- `Email__Password`
- `Email__UserName`

#### Development

For development, only the following files need to be filled. The remaining can be created as empty files

- `ConnectionStrings__DefaultConnection` =
```
server=db;port=3306;Database=bk_database;User=bkhpuser;Password=bkhppassword;default command timeout=120
```

- `db_name` =
```
bk_database
```

- `db_password` =
```
bkhppassword
```

- `db_root_password` =
```
bkhppassword_root
```

- `db_username` =
```
bkhpuser
```

### External system secrets

These secrets are required to run the external systems of the application in docker. 

- `db_name`
- `db_username`
- `db_password`
- `db_root_password`
