az login
##add azure container registry to aks
$clustername="devcodingflamingocluster"
$resourceGroupName="AKSTestResourceGroup"
$location="westus2"
$acrName = "codingflamingoacr"
$nodes = 2
$maxNodes = 100

az group create --name $resourceGroupName --location $location
#create ACR if havent create one
az acr create --resource-group $resourceGroupName --name $acrName  --sku Basic
#create AKS cluster
az aks create --resource-group $resourceGroupName  --name $clustername --node-count $nodes  --generate-ssh-keys --enable-cluster-autoscaler --max-count $maxNodes --min-count $nodes --location $location
#attach to ACR
az aks update -n $clustername -g $resourceGroupName --attach-acr $acrName 

#add dev spaces
az aks use-dev-spaces -g $resourceGroupName -n $clustername