FROM mcr.microsoft.com/dotnet/sdk:9.0 AS prepare-restore
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset --version 0.3.2
WORKDIR "/src"
COPY ["InternalApi", "InternalApi/"]
COPY ["Common/", "Common/"]
RUN dotnet subset restore InternalApi/InternalApi.csproj --root-directory /src --output restore_subset/

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR "/src"
COPY --from=prepare-restore /src/restore_subset .
RUN dotnet restore "InternalApi/InternalApi.csproj"


COPY ["InternalApi", "InternalApi/"]
COPY ["Common/", "Common/"]

RUN dotnet publish "InternalApi/InternalApi.csproj" -c Release -o app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
RUN apt-get update && apt-get install -y curl
WORKDIR /app
COPY --from=build src/app/publish .
ENTRYPOINT ["dotnet", "InternalApi.dll"]


LABEL maintainer="Alexey <skoralal4@gmail.com>"
LABEL version="1.0"
LABEL description="Dockerfile for InternalAPI"
EXPOSE 5177/tcp
EXPOSE 5188/tcp