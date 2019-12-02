FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 8007

FROM microsoft/dotnet:2.2-sdk AS build
RUN curl -sL https://deb.nodesource.com/setup_8.x | bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY /src .
RUN dotnet restore ./service-components/Umoya
RUN dotnet build ./service-components/Umoya -c Release -o /app

FROM build AS publish
RUN dotnet publish ./service-components/Umoya -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Umoya.dll"]