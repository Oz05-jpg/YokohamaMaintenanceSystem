FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

COPY . . 
RUN dotnet build

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "bin/Debug/net10.0/YokohamaMaintenanceSystem.dll"]