FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

# Copy the Plugin and dependency DLLs to /app/plugins
RUN mkdir -p /app/plugins
RUN find /app/build -path "*.dll" -exec cp {} /app/plugins \;

WORKDIR "/src/api"
RUN dotnet publish "api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
# Copy the main app and plugins
COPY --from=build /app/publish .
COPY --from=build /app/plugins ./plugins
ENTRYPOINT ["dotnet", "api.dll"]
