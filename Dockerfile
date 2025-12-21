# 1) Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out


# 2) Runtime aşaması (Chrome + Selenium için gerekli)
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Chrome bağımlılıkları + Chrome kurulumu
RUN apt-get update && \
    apt-get install -y wget gnupg unzip fonts-liberation libasound2 libatk1.0-0 \
    libc6 libcairo2 libcups2 libdbus-1-3 libexpat1 libgcc1 libc... (devamı aynı)
