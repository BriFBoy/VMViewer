FROM mcr.microsoft.com/dotnet/sdk:10.0 AS Builder
WORKDIR /build
COPY . /build/
RUN dotnet publish -c Release -f net10.0

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=Builder /build/bin/Release/net10.0/ /app/
CMD [ "./web" ]
