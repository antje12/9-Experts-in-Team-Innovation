docker-compose build --no-cache
docker-compose up -d
 
timeout  /t 10 /nobreak
 
docker exec -it mongo.one.db mongosh --eval "rs.initiate({_id:'dbrs', members: [{_id:0, host: 'mongo.one.db'},{_id:1, host: 'mongo.two.db'},{_id:2, host: 'mongo.three.db'}]})"
cd ./AguardioEIT/DatabasePlugin
dotnet ef database update

timeout  /t 20 /nobreak
curl -X GET http://localhost:8082/HDFS/CreateHiveTables -H "accept: */*"