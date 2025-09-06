FROM mcr.microsoft.com/dotnet/sdk:8.0 AS sdk-install

RUN apt-get update && apt-get install -y --no-install-recommends \
    python3  \
    python3-pip \
    nodejs \
    g++ \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EptaCombine.sln", "."]
COPY ["CodeRunner/CodeRunner.csproj", "CodeRunner/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Core/Core.csproj", "Core/"]

RUN dotnet restore "CodeRunner/CodeRunner.csproj"

COPY ["CodeRunner/", "CodeRunner/"]
COPY ["Common/", "Common/"]
COPY ["Core/", "Core/"]

RUN dotnet publish "CodeRunner/CodeRunner.csproj" -c Release -o /app/publish --no-restore

FROM sdk-install AS final

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CodeRunner.dll"]