FROM mcr.microsoft.com/dotnet/sdk:9.0 AS prepare-restore
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset --version 0.3.2
WORKDIR "/src"
COPY . .
RUN dotnet subset restore InternalApi/InternalApi.csproj --root-directory /src --output restore_subset/

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR "/src"
COPY --from=prepare-restore /src/restore_subset .
RUN dotnet restore "InternalApi/InternalApi.csproj"


COPY . .

RUN dotnet publish "InternalApi/InternalApi.csproj" -c Release -o app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
RUN apt-get update && apt-get install -y curl && \
	apt-get clean && \
	rm -rf /var/lib/apt/lists/
WORKDIR /app
COPY --from=build src/app/publish .
LABEL maintainer="Alexey <skoralal4@gmail.com>"
LABEL version="1.0"
LABEL description="Dockerfile for InternalAPI"
EXPOSE 5177/tcp
EXPOSE 5188/tcp
ENTRYPOINT ["dotnet", "InternalApi.dll"]