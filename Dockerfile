FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5003

ENV ASPNETCORE_URLS=http://+:5003

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["PiketWebApi/PiketWebApi.csproj", "PiketWebApi/"]
RUN dotnet restore "PiketWebApi/PiketWebApi.csproj"
COPY . .
WORKDIR "/src/PiketWebApi"
RUN dotnet build "PiketWebApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "PiketWebApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PiketWebApi.dll"]
