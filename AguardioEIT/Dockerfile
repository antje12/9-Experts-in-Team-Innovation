FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install dependencies for ODBC
RUN apt-get update && apt-get install -y \
  sasl2-bin \
  libsasl2-modules \
  unixodbc \
  alien \
  && apt-get clean \
  && rm -rf /var/lib/apt/lists/*

# Copy the ODBC RPM, convert it to DEB and install it
COPY ./ClouderaHiveODBC-2.6.11.1011-1.x86_64.rpm /tmp/
RUN alien -i /tmp/ClouderaHiveODBC-2.6.11.1011-1.x86_64.rpm && \
  rm -f /tmp/ClouderaHiveODBC-2.6.11.1011-1.x86_64.rpm

# Copy ODBC configuration files
COPY ./odbc.ini /etc/odbc.ini
COPY ./odbcinst.ini /etc/odbcinst.ini


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
# Copy the JSON file that holds the plugin configuration
COPY ./api/appsettings.json /app/publish/appsettings.json
ENTRYPOINT ["dotnet", "api.dll"]
