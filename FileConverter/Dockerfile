FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EptaCombine.sln", "."]
COPY ["FileConverter/FileConverter.csproj", "FileConverter/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Core/Core.csproj", "Core/"]

RUN dotnet restore "FileConverter/FileConverter.csproj"

COPY ["FileConverter/", "FileConverter/"]
COPY ["Common/", "Common/"]
COPY ["Core/", "Core/"]

RUN dotnet publish "FileConverter/FileConverter.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN apt-get update \
    && apt-get install -y --no-install-recommends ffmpeg \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
COPY --from=pandoc/minimal:2.19.2 /pandoc /usr/bin/pandoc
ENTRYPOINT ["dotnet", "FileConverter.dll"]