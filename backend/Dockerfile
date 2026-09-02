FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

ENV DOTNET_NOLOGO=1
ENV NUGET_XMLDOC_MODE=skip

COPY DJI.Market.slnx ./
COPY src/DJI.Core/DJI.Core.csproj                     src/DJI.Core/
COPY src/DJI.Contracts/DJI.Contracts.csproj           src/DJI.Contracts/
COPY src/DJI.Infrastructure/DJI.Infrastructure.csproj src/DJI.Infrastructure/
COPY src/DJI.Bl/DJI.Bl.csproj                         src/DJI.Bl/
COPY src/DJI.Web/DJI.Web.csproj                       src/DJI.Web/
COPY tests/DJI.Tests/DJI.Tests.csproj                 tests/DJI.Tests/

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore DJI.Market.slnx --disable-parallel

COPY . .

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish src/DJI.Web/DJI.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DJI.Web.dll"]
