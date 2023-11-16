# 9-Experts-in-Team-Innovation

## Overleaf Link
[Experts in Teams Innovation - Aguardio Group 1](https://www.overleaf.com/project/654fcf977ff977ae7a506617)

## Applying PostgreSQL Migrations
The easiest way to apply the migrations is to start docker-compose by using one of the
`startdocker` scripts at the root of the project. They automatically configure both PostgreSQL and Mongodb 
after all the containers are up and running.

If you don't use the `startdocker` scripts, follow these instructions:

To use the SQL services in the DatabasePlugin, the tables must be created in the PostgreSQL database.
You can do this by navigating to the directory `/AguardioEIT/DatabasePlugin`

```
cd AguardioEIT/DatabasePlugin
```

Then apply the migrations by running:
```
dotnet ef database update
```

**_Note:_** To do this, the PostgreSQL service must be running in Docker.
