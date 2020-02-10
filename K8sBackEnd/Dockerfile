FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["K8sBackEnd.csproj", "."]
RUN dotnet restore "K8sBackEnd.csproj"
COPY . .
RUN dotnet build "K8sBackEnd.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "K8sBackEnd.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "K8sBackEnd.dll"]