# Stage 1: Build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS yms

WORKDIR /app
COPY . . 
RUN dotnet build -o /app/publish

# Stage 2: Final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=yms /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "YokohamaMaintenanceSystem.dll"]