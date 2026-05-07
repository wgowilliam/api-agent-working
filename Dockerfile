FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AgentWorking.sln .
COPY AgentWorking.API/AgentWorking.API.csproj AgentWorking.API/
COPY AgentWorking.Application/AgentWorking.Application.csproj AgentWorking.Application/
COPY AgentWorking.Domain/AgentWorking.Domain.csproj AgentWorking.Domain/
COPY AgentWorking.Infrastructure/AgentWorking.Infrastructure.csproj AgentWorking.Infrastructure/
COPY AgentWorking.Tests/AgentWorking.Tests.csproj AgentWorking.Tests/

RUN dotnet restore

COPY . .

RUN dotnet test AgentWorking.Tests/AgentWorking.Tests.csproj --no-restore

RUN dotnet publish AgentWorking.API/AgentWorking.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AgentWorking.API.dll"]