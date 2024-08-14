#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8070

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DS_Lab3.Cars.csproj", "DS_Lab3.Cars/"]
RUN dotnet restore "DS_Lab3.Cars/DS_Lab3.Cars.csproj"

WORKDIR "/src/DS_Lab3.Cars"
COPY . .

RUN dotnet build "DS_Lab3.Cars.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DS_Lab3.Cars.csproj" -c Release -o /app/publish --self-contained false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DS_Lab3.Cars.dll", "--urls=http://0.0.0.0:8070"]