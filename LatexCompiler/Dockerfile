FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EptaCombine.sln", "."]
COPY ["LatexCompiler/LatexCompiler.csproj", "LatexCompiler/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Core/Core.csproj", "Core/"]

RUN dotnet restore "LatexCompiler/LatexCompiler.csproj"

COPY ["LatexCompiler/", "LatexCompiler/"]
COPY ["Common/", "Common/"]
COPY ["Core/", "Core/"]

RUN dotnet publish "LatexCompiler/LatexCompiler.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN apt-get update && apt-get install -y --no-install-recommends \
    texlive-base \
    texlive-latex-base \
    texlive-latex-recommended \
    texlive-latex-extra \
    texlive-fonts-recommended \
    texlive-lang-cyrillic \
    latexmk \
    perl \
    python3-pygments \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

RUN apt-get update && apt-get install -y --no-install-recommends \
    texlive-base \
    texlive-latex-base \
    texlive-latex-recommended \
    texlive-latex-extra \
    texlive-fonts-recommended \
    texlive-lang-cyrillic \
    texlive-science \
    texlive-pictures \
    latexmk \
    python3-pygments \
    && apt-get clean && rm -rf /var/lib/apt/lists/*
    
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LatexCompiler.dll"]
