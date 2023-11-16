#!/bin/bash

docker-compose build --no-cache
docker-compose up -d

sleep 10

docker exec -it mongo.one.db mongosh --eval "rs.initiate({_id:'dbrs', members: [{_id:0, host: 'mongo.one.db'},{_id:1, host: 'mongo.two.db'},{_id:2, host: 'mongo.three.db'}]})"
cd ./AguardioEIT/DatabasePlugin || exit
dotnet ef database update

sleep 30
curl -v -X GET http://localhost:8082/HDFS/CreateHiveTables -H "accept: */*"