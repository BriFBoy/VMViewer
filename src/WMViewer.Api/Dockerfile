FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /build
COPY . /build/
RUN dotnet publish -c Release -f net10.0

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=builder /build/bin/Release/net10.0/publish/ /app/                                                                                                                                                                                                                
CMD ["dotnet", "VM Viewer.dll"]
