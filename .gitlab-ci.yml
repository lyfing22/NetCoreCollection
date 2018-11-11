FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/MarketingAsync.Web/MarketingAsync.Web.csproj", "src/MarketingAsync.Web/"]
COPY ["src/MarketingAsync.Core/MarketingAsync.Core.csproj", "src/MarketingAsync.Core/"]
COPY ["MarketingAsync.RabbitMq/MarketingAsync.RabbitMq.csproj", "MarketingAsync.RabbitMq/"]
COPY ["MarketingAsync.Redis/MarketingAsync.Redis.csproj", "MarketingAsync.Redis/"]
COPY ["MarketingAsync.Mongodb/MarketingAsync.Mongodb.csproj", "MarketingAsync.Mongodb/"]
COPY ["MarketingAsync.Dapper/MarketingAsync.Dapper.csproj", "MarketingAsync.Dapper/"]
COPY ["src/MarketingAsync.Application/MarketingAsync.Application.csproj", "src/MarketingAsync.Application/"]
RUN dotnet restore "src/MarketingAsync.Web/MarketingAsync.Web.csproj"
COPY . .
WORKDIR "/src/src/MarketingAsync.Web"
RUN dotnet build "MarketingAsync.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MarketingAsync.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MarketingAsync.Web.dll"]