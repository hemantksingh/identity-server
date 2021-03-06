# Identity server


## Running the application using docker

```sh
# Build the docker image
docker build -t hemantksingh/identity-server .

# Run in development mode
docker run -p 80:5000 -e ASPNETCORE_ENVIRONMENT=Development hemantksingh/identity-server

# Run in production mode
docker run -p 80:5000 hemantksingh/identity-server

# or run identity server behind an nginx reverse proxy
docker-compose up
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

## Deploying to Azure App Service 

You can [deploy to azure app service as a container](https://docs.microsoft.com/en-us/azure/app-service/containers/tutorial-custom-docker-image) by following the steps below

```sh

appPlan=$1
resourceGroup=$2
app=$3
acrRegistry=$4
acrUsername=$5
acrPassword=$6

# create the app service plan
az appservice plan create --name $appPlan --resource-group $resourceGroup --sku S1 --is-linux

# create the webapp
az webapp create --resource-group $resourceGroup --plan $appPlan --name $app --multicontainer-config-type compose --multicontainer-config-file docker-compose.yml

# required only if the docker image is stores in ACR
az webapp config container set --name $app --resource-group $resourceGroup --docker-custom-image-name $acrRegistry.azurecr.io/pmsaas/$app:latest --docker-registry-server-url https://$acrRegistry.azurecr.io --docker-registry-server-user $acrUsername --docker-registry-server-password $acrPassword

# Tell App Service about the port that your contianer uses by using the WEBSITES_PORT app setting. It is required if the docker container runs on a custom port other than 80
az webapp config appsettings set --resource-group $resourceGroup --name $app --settings WEBSITES_PORT=5000 ASPNETCORE_ENVIRONMENT=Development

# Tail logs
az webapp log tail --name $app --resource-group $resourceGroup
```
Further info about Azure app service - https://azure.github.io/AppService

### Running with HTTPS enforced in contianer

Azure app service enforces TLS termination at the app service level, so you may not have to enforce TLS within the container, but [.net core allows hosting containers with https support](https://docs.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-3.1) by

* sharing the host certificate with docker container using volume mounts
* expose container ports 80 & 44 for external use

### Enable CORS

You can either [enable CORS via Azure app service](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-rest-api#enable-cors) or within the application. To enable it within Azure app service via azcli

```sh
az webapp cors add --resource-group myResourceGroup --name <app-name> --allowed-origins 'http://localhost:5000'
```

Don't try to use App Service CORS and your own CORS code together. When used together, App Service CORS takes precedence and your own CORS code has no effect.

### Session Affinity 

Azure app service load balances requests using [IIS Application Request Routing (ARR)](https://www.iis.net/downloads/microsoft/application-request-routing). When a request comes in, ARR slaps a "session affinity cookie" `ARRAffinity` on the response which it uses on subsequent requests to direct that specific users requests back to the same server.  This cookie is enabled by default. If you're not using any [session state](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-3.1#session-state) in your application and want the requests to be evenly load balanced (e.g. round robin load balancing) across machines you can [disable session affinity in azure app service](https://dzone.com/articles/disabling-session-affinity-in-azure-app-service-we) by adding a special response header `Arr-Disable-Session-Affinity` in the application and setting it to true.


