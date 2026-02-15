# Etap 1: Budowanie (Warsztat)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiuje plik projektu i pobiera biblioteki
COPY ["CorporateHelpdesk.csproj", "./"]
RUN dotnet restore "CorporateHelpdesk.csproj"

# Kopiuje resztê plików i buduje
COPY . .
RUN dotnet publish "CorporateHelpdesk.csproj" -c Release -o /app/publish

# Etap 2: Uruchamianie
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CorporateHelpdesk.dll"]