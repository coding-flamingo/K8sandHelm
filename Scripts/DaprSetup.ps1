#1 Optional Download Dapr localy (source https://github.com/dapr/docs/blob/master/getting-started/environment-setup.md)
powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"

#2 add dapr to help repos sorce for 2-5 https://github.com/dapr/docs/blob/master/getting-started/environment-setup.md 
helm repo add dapr https://daprio.azurecr.io/helm/v1/repo
helm repo update
#3 crate dapr namespace
kubectl create namespace dapr-system
#4 instal dapr 
helm install dapr dapr/dapr --namespace dapr-system
#5 check on your pods
kubectl get pods -n dapr-system -w

#Uninstal dapr 
helm uninstall dapr -n dapr-system