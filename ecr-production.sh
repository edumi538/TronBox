#!/bin/bash

localname="tron-box-production"
path=".\TronBox.API\bin\Release\netcoreapp2.2\publish"
ecr="006440196462.dkr.ecr.sa-east-1.amazonaws.com/tron/api-box-production:latest"

docker build -t $localname $path
docker tag $localname:latest $ecr
docker push $ecr
