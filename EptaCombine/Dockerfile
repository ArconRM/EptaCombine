FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EptaCombine.sln", "."]
COPY ["EptaCombine/EptaCombine.csproj", "EptaCombine/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Core/Core.csproj", "Core/"]

RUN dotnet restore "EptaCombine/EptaCombine.csproj"

COPY ["EptaCombine/", "EptaCombine/"]
COPY ["Common/", "Common/"]
COPY ["Core/", "Core/"]

RUN dotnet publish "EptaCombine/EptaCombine.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EptaCombine.dll"]
