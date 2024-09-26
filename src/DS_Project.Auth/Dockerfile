FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8040


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DS_Project.Auth.csproj", "DS_Project.Auth/"]
RUN dotnet restore "DS_Project.Auth/DS_Project.Auth.csproj"

WORKDIR "/src/DS_Project.Auth"
COPY . .

RUN dotnet build "./DS_Project.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./DS_Project.Auth.csproj" -c Release -o /app/publish --self-contained false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DS_Project.Auth.dll", "--urls=http://0.0.0.0:8040"]