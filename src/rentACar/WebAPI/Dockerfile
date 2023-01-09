#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/rentACar/WebAPI/WebAPI.csproj", "src/rentACar/WebAPI/"]
COPY ["src/rentACar/Persistence/Persistence.csproj", "src/rentACar/Persistence/"]
COPY ["src/rentACar/Domain/Domain.csproj", "src/rentACar/Domain/"]
COPY ["src/corePackages/Core.Persistence/Core.Persistence.csproj", "src/corePackages/Core.Persistence/"]
COPY ["src/rentACar/Application/Application.csproj", "src/rentACar/Application/"]
COPY ["src/corePackages/Core.Application/Core.Application.csproj", "src/corePackages/Core.Application/"]
COPY ["src/corePackages/Core.Security/Core.Security.csproj", "src/corePackages/Core.Security/"]
COPY ["src/corePackages/Core.CrossCuttingConcers/Core.CrossCuttingConcerns.csproj", "src/corePackages/Core.CrossCuttingConcers/"]
COPY ["src/corePackages/Core.Mailing/Core.Mailing.csproj", "src/corePackages/Core.Mailing/"]
RUN dotnet restore "src/rentACar/WebAPI/WebAPI.csproj"
COPY . .
WORKDIR "/src/src/rentACar/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]