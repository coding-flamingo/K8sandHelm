#kubectl create namespace devcodingflamingo
helm delete firstdeployment1dev

helm install firstdeployment1dev .\k8sandhelm 
kubectl get pods -n devcodingflamingo 


#deploy ingress
helm repo add stable https://kubernetes-charts.storage.googleapis.com/
helm repo update

helm install nginx-ingress stable/nginx-ingress --namespace devcodingflamingo  --set controller.replicaCount=2   --set controller.nodeSelector."beta\.kubernetes\.io/os"=linux      --set defaultBackend.nodeSelector."beta\.kubernetes\.io/os"=linux 

helm delete nginx-ingress  --namespace devcodingflamingo
kubectl get pods -n devcodingflamingo
