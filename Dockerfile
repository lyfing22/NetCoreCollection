FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/MarketingAsync.Web/MarketingAsync.Web.csproj", "src/MarketingAsync.Web/"]
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