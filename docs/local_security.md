# Local Development Guide
- [Local Development Guide](#local-development-guide)
  - [Securing Mongo](#securing-mongo)
  - [Securing Rabbit](#securing-rabbit)

## Securing Mongo
* Start your mongo database
* Modify the `<root>/mongo-scripts/local-security-setup.js` file to set the mongo user and password you will authenticate with.
* Run the `local-security-setup.js` against whatever you wish to be the "auth database"
  * NOTE: some people use "admin" as the auth database but this can be whatever you wish and does not need to be the "corp-hq" database.
  * Ex: `...$ mongo 127.0.0.1/<auth_db> ./mongo-scripts/local-security-setup.js`
* Optional: Update your `launch.json` to include the `MONGO_CONNECTION` connection string
  * Ex: `"MONGO_CONNECTION": "mongodb://<user>:<pass>@127.0.0.1:27017/<auth_db>"`
* Optional: Update your `Makefile` to include the updated `MONGO_CONNECTION` connection string
  * Ex: `MONGO_CONNECTION=mongodb://mongoUser:mongoPass@127.0.0.1:27017/corp-hq dotnet <...>`
* Modify the `docker-compose.yml` to add the `-auth` flag to the `command` for mongo
* Optional: `docker stop <id>` / `docker rm <id>` any existing mongo container
* Start / restart all the various services with the new connection strings.

## Securing Rabbit
* to be documented