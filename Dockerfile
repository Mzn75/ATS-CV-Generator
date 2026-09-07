# 1. Base runtime image (includes Puppeteer Linux dependencies)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

RUN apt-get update && apt-get install -y \
    wget gnupg2 apt-transport-https ca-certificates \
    fonts-liberation libappindicator3-1 libasound2 \
    libatk-bridge2.0-0 libatk1.0-0 libcups2 libnss3 \
    libx11-xcb1 libxcomposite1 libxdamage1 libxrandr2 xdg-utils

# 2. Build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ATS_CV_Generator.csproj", "./"]
RUN dotnet restore "./ATS_CV_Generator.csproj"
COPY . .
RUN dotnet build "ATS_CV_Generator.csproj" -c Release -o /app/build

# 3. Publish application
FROM build AS publish
RUN dotnet publish "ATS_CV_Generator.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Final image assembly
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ATS_CV_Generator.dll"]