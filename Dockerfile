# 1) Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out


# 2) Runtime (Chrome + Selenium için)
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Chrome bağımlılıkları + Chrome
RUN apt-get update && apt-get install -y \
    wget \
    gnupg \
    unzip \
    fonts-liberation \
    libasound2 \
    libatk1.0-0 \
    libc6 \
    libcairo2 \
    libcups2 \
    libdbus-1-3 \
    libexpat1 \
    libfontconfig1 \
    libgcc1 \
    libgconf-2-4 \
    libgdk-pixbuf2.0-0 \
    libglib2.0-0 \
    libgtk-3-0 \
    libnspr4 \
    libnss3 \
    libpango-1.0-0 \
    libx11-6 \
    libx11-xcb1 \
    libxcb1 \
    libxcomposite1 \
    libxcursor1 \
    libxdamage1 \
    libxext6 \
    libxfixes3 \
    libxi6 \
    libxrandr2 \
    libxrender1 \
    libxss1 \
    libxtst6 \
    xdg-utils \
    ca-certificates \
    && apt-get clean

# Chrome indir ve kur
RUN wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb \
    && apt install -y ./google-chrome-stable_current_amd64.deb \
    && rm google-chrome-stable_current_amd64.deb

WORKDIR /app
COPY --from=build /app/out .

# Chrome için zorunlu parametreler
ENV SELENIUM_CHROME_ARGS="--no-sandbox --disable-dev-shm-usage --headless"

ENTRYPOINT ["dotnet", "FlightTracker.dll"]
