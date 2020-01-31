#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy everything else and build
COPY . ./

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app .
ENV ASPNETCORE_ENVIRONMENT=Staging
CMD cat ./hosts >> /etc/hosts && dotnet TronBox.API.dll