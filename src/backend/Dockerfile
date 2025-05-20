FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY *.sln ./
COPY Api/Api.csproj ./Api/
COPY BL/BL.csproj ./BL/
COPY DAL/DAL.csproj ./DAL/
COPY Tests/Tests.csproj ./Tests/
RUN dotnet restore
COPY . .
WORKDIR /src/Api
RUN dotnet publish -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Api.dll"]
