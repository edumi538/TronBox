#!/bin/bash

localname="tron-box-staging"
path=".\TronBox.API\bin\Debug\netcoreapp2.2\publish"
ecr="006440196462.dkr.ecr.sa-east-1.amazonaws.com/tron/api-box-staging:latest"

docker build -t $localname $path
docker tag $localname:latest $ecr
docker push $ecr
