# tictactoe
TicTacToe online game

## Commands

### Frontend
* Build the image
```docker
docker build -t tictactoe-frontend .
```

* Start the container from the image
```docker
docker run -d -p 8080:80 --name tictactoe-frontend-container tictactoe-frontend
```

* Clean-up:
```docker
docker stop tictactoe-frontend-container
docker rm tictactoe-frontend-container
```


* Start webserver directly:
```docker
docker run -d -p 8080:80 -v ${PWD}/frontend/public:/usr/share/nginx/html:ro nginx:alpine
```


dotnet new web -o tictactoe-backend



### Backend

```docker
docker build -t tictactoe-backend ./backend

docker run -d -p 5000:80 --name tictactoe-backend-container tictactoe-backend
```


