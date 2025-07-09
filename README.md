# tictactoe
TicTacToe online game

## Commands

### Frontend

* Build the image

```console
docker build -t tictactoe-frontend .
```

* Start the container from the image

```console
docker run -d -p 8080:80 --name tictactoe-frontend-container tictactoe-frontend
```

* Clean-up:

```console
docker stop tictactoe-frontend-container
docker rm tictactoe-frontend-container
```


* Start webserver directly:

```console
docker run --rm -d -p 8080:80 -v ${PWD}/frontend/public:/usr/share/nginx/html:ro nginx:alpine
```


dotnet new web -o tictactoe-backend



### Backend

* Add the Entity Framework package

```console
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

* Run to initialize the database
```console
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate
dotnet ef database update
```

```console
docker build -t tictactoe-backend ./backend

docker run --rm -d -p 5000:80 --name tictactoe-backend-container tictactoe-backend
```






#### Configure Postgres

* Create network and docker volume for persistent database storage

```console
docker network create pg-network
docker volume create pgdata
```

* Run Postgres

```console
docker run --rm --name my-postgres --network pg-network -e POSTGRES_USER=user -e POSTGRES_PASSWORD=user -e POSTGRES_DB=mydatabase -p 5432:5432 -d -v pgdata:/var/lib/postgresql/data postgres:15-alpine
````

* Run PGAdmin

```console
docker run --rm --network pg-network -p 8081:80 -e PGADMIN_DEFAULT_EMAIL=admin@example.com -e PGADMIN_DEFAULT_PASSWORD=admin dpage/pgadmin4
```


