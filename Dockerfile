FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source


COPY *.sln .
COPY GameDesign/*.csproj ./GameDesign/
COPY GameServerDefinitions/*.csproj ./GameServerDefinitions/
COPY GameServerImplementation/*.csproj ./GameServerImplementation/
COPY GameServerImplementation.Tests/*.csproj ./GameServerImplementation.Tests/
COPY WebInterface/*.csproj ./WebInterface/
RUN dotnet restore


COPY GameDesign/. ./GameDesign/
COPY GameServerDefinitions/. ./GameServerDefinitions/
COPY GameServerImplementation/. ./GameServerImplementation/
COPY GameServerImplementation.Tests/. ./GameServerImplementation.Tests/
COPY WebInterface/. ./WebInterface/
WORKDIR /source/WebInterface
RUN dotnet publish -c release -o /app --no-restore



FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT Production
ENTRYPOINT ["dotnet", "WebInterface.dll", "--urls=http://0.0.0.0:5000"]