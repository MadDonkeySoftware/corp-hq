#!/bin/bash

# Just make sure the containers are being build fresh
docker-compose -f test-compose.yml down -v

echo ""
echo "-----------"
echo "Build the applications before we create docker images for them."
echo "-----------"
dotnet publish --configuration Release ./app/api
dotnet publish --configuration Release ./app/runner
npm install --prefix ./app/ui
npm run --prefix ./app/ui build

echo "-----------"
echo "Build the images we will eventually test."
echo "-----------"
docker-compose -f test-compose.yml build

echo "-----------"
echo "Start the db and dependencies."
echo "-----------"
docker-compose -f test-compose.yml up -d test-mongodb test-rabbitmq test-eve-api

echo "-----------"
echo "Sleep then seed the db with test data."
echo "-----------"
sleep 2
docker exec -it test-mongodb mongo 127.0.0.1:27017/corp-hq /var/scripts/test-seed-data.js

echo "-----------"
echo "Sleeping then starting the tested services."
echo "-----------"
sleep 2
docker-compose -f test-compose.yml up -d test-runner test-api 

echo "-----------"
echo "Sleeping then starting the tests."
echo "-----------"
sleep 2
docker run -it -v $(pwd)/tests/behavior:/home/node/app -w="/home/node/app" --network="test-network" --name test-node node:8 bash -c "npm install; API_URL=http://test-api MONGO_URL=mongodb://test-mongodb:27017/auth npm run cucumber"
EXIT_CODE="$(docker inspect --format='{{.State.ExitCode}}' test-node)"
docker rm test-node

echo "-----------"
echo "Shutdown the services and clean up dangling volumes."
echo "-----------"
if [ "$1" = "debug" ] && [ "$EXIT_CODE" != "0" ]; then
    echo "Not cleaning up containers or volumes since debug mode is on and there was an error. Run 'docker-compose -f test-compose.yml down -v' to clean up containers."
else
    docker-compose -f test-compose.yml down -v
fi

# Helpful commands for debugging.
# docker logs test-runner
# docker exec -it test-mongodb mongo
# docker run -it --rm -v $(pwd)/tests/mock-services/eve-api:/var/mock-scripts -w="/var/mock-scripts" -p 3000:3000 --name test-eve-api node:8 bash -c "npm install; node server.js"
# docker volume rm $(docker volume ls -qf dangling=true)
# docker system prune -f --volumes

echo ""
echo "EXIT_CODE: $EXIT_CODE"
if [ "$EXIT_CODE" = "0" ]; then
    exit 0
else
    exit 1
fi