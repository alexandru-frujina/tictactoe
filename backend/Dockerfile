# 1. Build stage: compile the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY ./tictactoe-backend/*.csproj ./
RUN dotnet restore

COPY ./tictactoe-backend ./

#RUN dotnet publish -c release -o out
RUN dotnet publish -c Debug -o out

# 2. Runtime stage: copy published files and run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Install vsdbg (for debugging)
RUN apt-get update && \
    apt-get install -y curl unzip && \
    mkdir -p /vsdbg && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

# Copy the published files from the build stage
COPY --from=build /app/out ./

ENTRYPOINT [ "dotnet", "tictactoe-backend.dll" ]