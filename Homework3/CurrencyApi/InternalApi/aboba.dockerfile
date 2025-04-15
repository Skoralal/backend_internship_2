FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["InternalApi/InternalApi.csproj", "InternalApi/"]
COPY ["CurrencyApi.sln", "./"]
RUN dotnet restore "InternalApi/InternalApi.csproj"
COPY . .
WORKDIR "/src/InternalApi"
RUN dotnet build "InternalApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InternalApi.csproj" -c Release -o /app/publish /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./InternalApi"] # Adjust if your executable name is different