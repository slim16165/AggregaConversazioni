# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["src/AggregaConversazioni.Api/AggregaConversazioni.Api.csproj", "src/AggregaConversazioni.Api/"]
COPY ["src/AggregaConversazioni.Application/AggregaConversazioni.Application.csproj", "src/AggregaConversazioni.Application/"]
COPY ["src/AggregaConversazioni.Domain/AggregaConversazioni.Domain.csproj", "src/AggregaConversazioni.Domain/"]
COPY ["src/AggregaConversazioni.Infrastructure/AggregaConversazioni.Infrastructure.csproj", "src/AggregaConversazioni.Infrastructure/"]

RUN dotnet restore "src/AggregaConversazioni.Api/AggregaConversazioni.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/AggregaConversazioni.Api"
RUN dotnet build "AggregaConversazioni.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AggregaConversazioni.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AggregaConversazioni.Api.dll"]
