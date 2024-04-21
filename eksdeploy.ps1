$Region = "us-east-1"
$vmSize = "t3.large"
$clusterName = "test-tls"
eksctl create cluster --name $clusterName--region $Region --nodes 2 --node-type $vmSize --asg-access 

##Setup EKS as current context
aws eks  update-kubeconfig --name $clusterName --region $Region

## For the first time only
helm repo add stable https://charts.helm.sh/stable --force-update
helm repo add jetstack https://charts.jetstack.io --force-update

helm repo update
helm install cert-manager jetstack/cert-manager --namespace cert-manager  --create-namespace --version v1.14.4  --set installCRDs=true

### Create ingress for frontent
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.10.0/deploy/static/provider/aws/deploy.yaml

kubectl get services --all-namespaces
# kubectl get pods -n cert-manager

kubectl create namespace sampleapp


## Deploy the Certificate (only for the first time)
helm install app-cert-deployment ./aws-certificates-helmchart 

## Deploy the app 
helm install sampleapp-deployment ./aws-helmchart-flamingo 

## Update the app
helm upgrade sampleapp-deployment ./aws-helmchart-flamingo 

## Check Pods
kubectl get pods -n sampleapp -w

## Rollback the app
helm rollback sampleapp-deployment 8

## Check Services
kubectl get services -n sampleapp

## Check logs
kubectl logs sampleapp-ui-sampleapp-deployment-sampleapp-helm-chart-69d9fb79d8xxf9  -n sampleapp --previous

## Get all resources in a namesapce powrershell
# kubectl api-resources --verbs=list --namespaced -o name | `%{ kubectl get $_ --show-kind --ignore-not-found -n sampleapp }


## Delete Cluster
# https://docs.aws.amazon.com/eks/latest/userguide/delete-cluster.html