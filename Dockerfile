# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /build

COPY *.sln ./
COPY **/*.csproj ./

RUN dotnet sln list | grep ".csproj" \
    | while read -r line; do \
    mkdir -p $(dirname $line); \
    mv $(basename $line) $(dirname $line); \
    done;

RUN dotnet restore --use-current-runtime

COPY . .

RUN dotnet publish RefreshDiscordBot -c Release --property:OutputPath=/build/publish/ --no-restore --no-self-contained

# Final running container

FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS final

ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

# Add non-root user
RUN set -eux && \
apk add --no-cache su-exec icu-libs icu-data-full tzdata && \
su-exec nobody true && \
addgroup -g 1001 refresh && \
adduser -D -h /refresh -u 1001 -G refresh refresh && \
mkdir -p /refresh/app

COPY --from=build /build/publish/publish /refresh/app

RUN chown -R refresh:refresh /refresh

ENTRYPOINT ["su-exec", "refresh:refresh", "/refresh/app/RefreshDiscordBot"]
