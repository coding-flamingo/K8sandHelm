#kubectl create namespace development

helm install firstdeployment1dev .\k8sandhelm 
kubectl get pods -n devcodingflamingo

helm delete firstdeployment1dev