# 9-Experts-in-Team-Innovation

## Overleaf Link
[https://www.overleaf.com/project/654fcf977ff977ae7a506617](Experts in Teams Innovation - Aguardio Group 1)

## Applying PostgreSQL Migrations
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
