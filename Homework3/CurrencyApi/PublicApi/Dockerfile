FROM mcr.microsoft.com/dotnet/sdk:9.0 AS prepare-restore
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset --version 0.3.2
WORKDIR "/src"
COPY . .
RUN dotnet subset restore PublicApi/PublicApi.csproj --root-directory /src --output restore_subset/

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR "/src"
COPY --from=prepare-restore /src/restore_subset .
RUN dotnet restore "PublicApi/PublicApi.csproj"


COPY . .

RUN dotnet publish "PublicApi/PublicApi.csproj" -c Release -o app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
RUN apt-get update && apt-get install -y curl && \
	apt-get clean && \
	rm -rf /var/lib/apt/lists/
WORKDIR /app
COPY --from=build src/app/publish .
LABEL maintainer="Alexey <skoralal4@gmail.com>"
LABEL version="1.0"
LABEL description="Dockerfile for PublicAPI"
EXPOSE 8080/tcp
ENTRYPOINT ["dotnet", "PublicApi.dll"]