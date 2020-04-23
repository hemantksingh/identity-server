# Identity server


## Running the application using docker

```sh
# Build the docker image
docker build -t hemantksingh/identity-server .

# Run in development mode
docker run -p 5000:5000 -e ASPNETCORE_ENVIRONMENT=Development hemantksingh/identity-server

# Run in production mode
docker run -p 5000:5000 hemantksingh/identity-server

```

Identity server should be accessible at http://localhost:5000 with the discovery document available at http://localhost:5000/.well-known/openid-configuration