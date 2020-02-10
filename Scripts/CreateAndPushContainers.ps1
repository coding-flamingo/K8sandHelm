$acrName="codingflamingo"

az acr login --name $acrName

cd K8sFrontEnd
docker image build -t  k8sfrontend .
docker tag k8sfrontend "$($acrName).azurecr.io/k8sfrontend:v2"
docker push "$($acrName).azurecr.io/k8sfrontend"
cd ..

cd K8sBackEnd
docker image build -t  k8sbackend .
docker tag k8sbackend "$($acrName).azurecr.io/k8sbackend:v1"
docker push "$($acrName).azurecr.io/k8sbackend"
cd ..