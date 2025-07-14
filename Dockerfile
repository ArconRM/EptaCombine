FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common/Common.csproj", "Common/"]
COPY ["EptaCombine/EptaCombine.csproj", "EptaCombine/"]
COPY ["FileConverter/FileConverter.csproj", "FileConverter/"]
RUN dotnet restore "EptaCombine/EptaCombine.csproj"

COPY . .
WORKDIR "/src/EptaCombine"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV TZ=UTC
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ffmpeg \
        tzdata \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "EptaCombine.dll"]
