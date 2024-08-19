#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8050

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DS_Project.Payment.csproj", "DS_Project.Payment/"]
RUN dotnet restore "DS_Project.Payment/DS_Project.Payment.csproj"
WORKDIR "/src/DS_Project.Payment"
COPY . .
RUN dotnet build "DS_Project.Payment.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DS_Project.Payment.csproj" -c Release -o /app/publish --self-contained false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DS_Project.Payment.dll", "--urls=http://0.0.0.0:8050"]