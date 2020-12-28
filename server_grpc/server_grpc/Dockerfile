#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["server_grpc/server_grpc.csproj", "server_grpc/"]
RUN dotnet restore "server_grpc/server_grpc.csproj"
COPY . .
WORKDIR "/src/server_grpc"
RUN dotnet build "server_grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "server_grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "server_grpc.dll"]