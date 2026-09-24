# =========================================================
# Étape 1 : Build (compilation + publication)
# =========================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copie tout le code source
COPY . .

# Restaure les dépendances NuGet
RUN dotnet restore

# Publie l'application en mode Release
RUN dotnet publish -c Release -o /app/publish --no-restore

# =========================================================
# Étape 2 : Image finale (runtime uniquement, plus légère)
# =========================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Render fournit dynamiquement le port via la variable d'environnement PORT.
# Program.cs doit déjà lire cette variable (voir tutoriel de déploiement).
EXPOSE 8080

# (visible dans le nom de ton fichier .csproj, ex: WerewolfGM.Web.csproj -> WerewolfGM.Web.dll)
ENTRYPOINT ["dotnet", "WerewolfGM.Web.dll"]