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
sleep 5
docker exec -it mongodb mongo 127.0.0.1:27017/corp-hq /var/scripts/local-seed-data.js

echo "-----------"
echo "Now we can start the API and UI components. They will not be reachable yet"
echo "as we have not turned on the proxy. Patience young grasshopper."
echo "-----------"
docker-compose -f quickstart-compose.yml up -d ui api manager

echo "-----------"
echo "Next we sleep about 30 seconds before starting the background runner."
echo "This is to give the API enough time to finish compiling before we start"
echo "compiling the runner. This is due to both containers sharing resources."
echo "-----------"
docker-compose -f quickstart-compose.yml up -d runner runner2

echo "-----------"
echo "Now we can start the proxy. The proxy listens on port 80 so you should be"
echo "able to browse to http://localhost for the site."
echo "-----------"
docker-compose -f quickstart-compose.yml up -d proxy

echo "-----------"
echo "Yay! You're done. When you're ready to clean up run one of the following."
echo "make quickstart-cleanup"
echo "or"
echo "docker-compose -f quickstart-compose.yml down -v"
echo "-----------"