FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BrbHabitaciones.Api/BrbHabitaciones.Api.csproj", "BrbHabitaciones.Api/"]
COPY ["BrbHabitaciones.Application/BrbHabitaciones.Application.csproj", "BrbHabitaciones.Application/"]
COPY ["BrbHabitaciones.Domain/BrbHabitaciones.Domain.csproj", "BrbHabitaciones.Domain/"]
COPY ["BrbHabitaciones.Infrastructure/BrbHabitaciones.Infrastructure.csproj", "BrbHabitaciones.Infrastructure/"]
RUN dotnet restore "BrbHabitaciones.Api/BrbHabitaciones.Api.csproj"

COPY . .
WORKDIR "/src/BrbHabitaciones.Api"

# Cache buster — pass --build-arg BUILD_DATE=$(date +%s) from EasyPanel to force recompile
ARG BUILD_DATE=unknown
RUN echo "Build date: $BUILD_DATE"

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BrbHabitaciones.Api.dll"]
