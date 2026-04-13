# ─── Stage 1: Build ────────────────────────────────────────────────────────────
# Use the full .NET 8 SDK image so we have all the tools needed to restore
# packages and compile the project.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file first and restore NuGet packages.
# Docker caches each instruction as a layer. By restoring before copying the
# rest of the code, a rebuild only re-downloads packages when the .csproj
# actually changes, not on every code change.
COPY BudgetTracker.csproj ./
RUN dotnet restore BudgetTracker.csproj

# Copy the remaining source code into the container.
COPY . ./

# Publish a Release build into /app/publish.
# --no-restore skips a second restore since we just did it above.
RUN dotnet publish BudgetTracker.csproj \
        --configuration Release \
        --no-restore \
        --output /app/publish


# ─── Stage 2: Runtime ──────────────────────────────────────────────────────────
# Use the smaller ASP.NET runtime image – it does NOT include the SDK,
# which keeps the final image size significantly smaller.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Tell ASP.NET Core to listen on port 8080 over plain HTTP inside the container.
# In production, HTTPS termination is handled by a reverse proxy (e.g. nginx),
# not by the app itself.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Copy only the published output from the build stage – nothing else.
COPY --from=build /app/publish .

# Start the application when the container launches.
ENTRYPOINT ["dotnet", "BudgetTracker.dll"]
