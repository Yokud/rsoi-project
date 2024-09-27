FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8030


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DS_Project.Statistics.csproj", "DS_Project.Statistics/"]
RUN dotnet restore "DS_Project.Statistics/DS_Project.Statistics.csproj"

WORKDIR "/src/DS_Project.Statistics"
COPY . .

RUN dotnet build "./DS_Project.Statistics.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./DS_Project.Statistics.csproj" -c Release -o /app/publish --self-contained false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DS_Project.Statistics.dll", "--urls=http://0.0.0.0:8030"]