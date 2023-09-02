FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

  RUN apk add --no-cache --upgrade bash

  COPY . /temp
  WORKDIR /temp
  
  RUN dotnet tool restore
  RUN dotnet nuke

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS final

  ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

  RUN apk add --no-cache icu-libs

  RUN addgroup -g 10000 appgroup && \
      adduser -D -u 100000 appuser -s /sbin/nologin -g appgroup

  RUN mkdir /app && \
      chown -R appuser:appgroup /app

  WORKDIR /app

  USER appuser:appgroup

  COPY --from=build /temp/publish /app

  ENTRYPOINT [ "dotnet", "BeanBot.dll" ]