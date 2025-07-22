FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

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

RUN tlmgr update --self && \
    tlmgr install latexmk synctex chktex latexindent \
    titlesec titletoc enumitem caption graphicx amsmath amssymb fancyvrb longtable array cite \
    minted pygments fvextra \
    collection-langcyrillic collection-langgreek collection-fontsrecommended \
    babel-russian inputenc tempora

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LatexCompiler.dll"]
