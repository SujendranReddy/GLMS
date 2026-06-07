FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["GLMS.Api/GLMS.Api.csproj", "GLMS.Api/"]
RUN dotnet restore "GLMS.Api/GLMS.Api.csproj"

COPY . .
WORKDIR "/src/GLMS.Api"
RUN dotnet build "GLMS.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GLMS.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "GLMS.Api.dll"]