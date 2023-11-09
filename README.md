# 9-Experts-in-Team-Innovation

## Applying PostgreSQL Migrations
In order to use the SQL services in the DatabasePlugin, the tables must be created in the PostgreSQL database.
You can do this by navigating to the directory `/AguardioEIT/DatabasePlugin`

```
cd AguardioEIT/DatabasePlugin
```

Then apply the migrations by running:
```
dotnet ef database updated
```

**_Note:_** To do this, the PostgreSQL service must be running in Docker.