# 1. Build stage: compile the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY ./tictactoe-backend/*.csproj ./
RUN dotnet restore

COPY ./tictactoe-backend ./

RUN dotnet publish -c release -o out

# 2. Runtime stage: copy published files and run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/out ./

EXPOSE 80

ENTRYPOINT [ "dotnet", "tictactoe-backend.dll" ]