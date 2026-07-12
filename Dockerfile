FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/HtmlToDocx.csproj", "./"]
COPY ["src/Program.cs", "./"]

# Restore dependencies
RUN dotnet restore "./HtmlToDocx.csproj"

# Build the application
RUN dotnet build "./HtmlToDocx.csproj" -c Release -o /app/build

# Test the build by running the help command
RUN dotnet run --project "./HtmlToDocx.csproj" -- --help

FROM build AS publish
RUN dotnet publish "./HtmlToDocx.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Domyślny port trybu serwera HTTP (nadpisywalny env HTML2DOCX_PORT).
# Odziedziczone ASPNETCORE_HTTP_PORTS=8080 jest ignorowane — bind steruje
# jawny UseUrls() w Program.cs.
ENV HTML2DOCX_PORT=3030
EXPOSE 3030

# Set the entrypoint for the container
ENTRYPOINT ["dotnet", "HtmlToDocx.dll"]