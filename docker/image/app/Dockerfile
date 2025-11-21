#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG vsconfig=Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
expose 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# Install Java 15
# CHECK FOR JAVA HOME
RUN wget -q https://download.java.net/java/GA/jdk15.0.1/51f4f36ad4ef43e39d0dfdbaf6549e32/9/GPL/openjdk-15.0.1_linux-x64_bin.tar.gz && \
    mkdir -p /usr/lib/jvm/open-jdk-15 && \
    tar -xzf ./openjdk-15.0.1_linux-x64_bin.tar.gz -C /usr/lib/jvm/open-jdk-15 --strip-components=1
ARG vsconfig
ARG buildNumber
RUN echo "Setting version -> $buildNumber"
RUN echo "Building dotnet projects with configuration -> $vsconfig"
COPY app/ /app

# must use --disable-parallel when restoring packages to avoid connection issues with nuget server...
RUN dotnet restore "/app/src/Api/Api.csproj" --disable-parallel
WORKDIR /app/src/Api
RUN dotnet build "Api.csproj" -c $vsconfig
RUN dotnet publish "Api.csproj" -c $vsconfig -o /app/publish --version-suffix $buildNumber

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY docker/image/app/entrypoint.sh /app/
RUN chmod 755 /app/entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh"]