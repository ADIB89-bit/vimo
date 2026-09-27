FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all files
COPY . .

# Find csproj and restore/publish
RUN dotnet restore $(ls *.csproj | head -n 1)
RUN dotnet publish $(ls *.csproj | head -n 1) -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "vimo.dll"]
