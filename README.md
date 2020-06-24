# Identity server


## Running the application using docker

```sh
# Build the docker image
docker build -t hemantksingh/identity-server .

# Run in development mode
docker run -p 80:5000 -e ASPNETCORE_ENVIRONMENT=Development hemantksingh/identity-server

# Run in production mode
docker run -p 80:5000 hemantksingh/identity-server
```

Identity server should be accessible at http://localhost and the discovery document at http://localhost/.well-known/openid-configuration on the docker host

## Deploying to AKS (Azure kubernetes service)

In order to route external traffic to identity server running within the AKS cluster, we use nginx controller for layer 7 routing. To fulfill ingress to your application, the nginx ingress controller deployment provisions a load balancer in Azure and assigns it a public IP.

```sh
# deploy nginx ingress controller
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-0.32.0/deploy/static/provider/cloud/deploy.yaml

# deploy identity server with ingress rules
kubectl apply -f identity-server.yaml
```

Identity server should be accessible at `http://{publicIp}/identity` with the discovery document available at `http://{publicIp}/identity/.well-known/openid-configuration`

### Running Identity server in K8s

https://medium.com/@christopherlenard/identity-server-and-nginx-ingress-controller-in-kubernetes-7146c22a2466

