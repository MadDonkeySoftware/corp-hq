.DEFAULT_GOAL := help

build-api:  ## Builds the API displaying any analytics violations.
	dotnet build --no-incremental ./app/api

build-runner:  ## Builds the API displaying any analytics violations.
	dotnet build --no-incremental ./app/runner

build-ui:  ## Builds the vue web site to deployable assets
	npm run --prefix ./app/ui build

docker-build-api:  ## Runs the docker build for the corp-hq api project
	docker build -t corp-hq-api ./app/api

docker-build-images: docker-build-api docker-build-ui  ## Run the docker build for all the projects

docker-build-ui: build-ui  ## Runs the docker build for the corp-hq ui project
	docker build -t corp-hq-ui ./app/ui

help:  ## Prints this help message.
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

run-api: build-api  ## Builds the API then runs it.
	MONGO_CONNECTION=mongodb://127.0.0.1:27017/corp-hq dotnet run -p ./app/api

run-runner: build-runner  ## Builds the API then runs it.
	MONGO_CONNECTION=mongodb://127.0.0.1:27017/corp-hq dotnet run -p ./app/runner

run-ui:  ## Builds the UI then runs it.
	npm run --prefix ./app/ui dev