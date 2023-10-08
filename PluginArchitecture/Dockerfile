FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PluginArchitecture/PluginArchitecture.csproj", "PluginArchitecture/"]
RUN dotnet restore "PluginArchitecture/PluginArchitecture.csproj"
COPY . .
WORKDIR "/src/PluginArchitecture"
RUN dotnet build "PluginArchitecture.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PluginArchitecture.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PluginArchitecture.dll"]
