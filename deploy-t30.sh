docker-compose build
docker push 192.168.1.151:32000/payplh:1.11.119

#payplh-t30
helm upgrade payplh -f ./chart/values.yaml ./chart --namespace default

#dashboard
kubectl port-forward -n kube-system service/kubernetes-dashboard 10443:443
