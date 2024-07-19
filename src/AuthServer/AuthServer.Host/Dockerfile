#FROM net6-gdi:6.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
COPY . /app
WORKDIR /app

ENV TZ Asia/Shanghai
ENV ASPNETCORE_ENVIRONMENT Docker

EXPOSE 80
ENTRYPOINT ["dotnet", "AuthServer.Host.dll"]
