#!/bin/bash

echo "-----------"
echo "Welcome to the Corp-HQ quick start script. We will attempt to bring you"
echo "from scratch to a fully working environment."
echo "-----------"
echo ""

# Just make sure the containers are being build fresh
docker-compose -f quickstart-compose.yml down -v 1>/dev/null 2>/dev/null

echo "-----------"
echo "First we start up the dependencies."
echo "-----------"
docker-compose -f quickstart-compose.yml up -d mongodb eve-api rabbitmq

echo "-----------"
echo "Next we seed the database with some settings"
echo "-----------"
sleep 2
docker exec -it mongodb mongo 127.0.0.1:27017/corp-hq /var/scripts/local-seed-data.js

echo "-----------"
echo "Now we can start the proxy, API and UI components. The proxy listens on"
echo "port 80 so you should be able to browse to http://localhost for the site."
echo "-----------"
docker-compose -f quickstart-compose.yml up -d proxy ui api

echo "-----------"
echo "Next we sleep about 30 seconds before starting the background runner."
echo "This is to give the API enough time to finish compiling before we start"
echo "compiling the runner. This is due to both containers sharing resources."
echo "-----------"
sleep 30
docker-compose -f quickstart-compose.yml up -d runner

echo "-----------"
echo "Yay! You're done. When you've finished simply run the below command."
echo "docker-compose -f quickstart-compose.yml down -v"
echo "-----------"