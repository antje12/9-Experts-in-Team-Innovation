# 9-Experts-in-Team-Innovation

## Applying PostgreSQL Migrations
The easiest way to apply the migrations is to use start docker compose by using one of the
`startdocker` scripts in the root of the project. They automatically configure both postgresql
mongodb after all the containers are up and running.

If you don't use the `startdocker` scripts, follow these instructions:

In order to use the SQL services in the DatabasePlugin, the tables must be created in the PostgreSQL database.
You can do this by navigating to the directory `/AguardioEIT/DatabasePlugin`

```
cd AguardioEIT/DatabasePlugin
```

Then apply the migrations by running:
```
dotnet ef database update
```

**_Note:_** To do this, the PostgreSQL service must be running in Docker.
