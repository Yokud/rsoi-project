#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8060

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DS_Lab3.Rental.csproj", "DS_Lab3.Rental/"]
RUN dotnet restore "DS_Lab3.Rental/DS_Lab3.Rental.csproj"
WORKDIR "/src/DS_Lab3.Rental"
COPY . .
RUN dotnet build "DS_Lab3.Rental.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DS_Lab3.Rental.csproj" -c Release -o /app/publish --self-contained false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DS_Lab3.Rental.dll", "--urls=http://0.0.0.0:8060"]