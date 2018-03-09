.DEFAULT_GOAL := help

build-ui:  ## Builds the vue web site to deployable assets
	npm run --prefix ./app/ui build

docker-build-api:  ## Runs the docker build for the corp-hq api project
	docker build -t corp-hq-api ./app/api

docker-build-images: docker-build-api docker-build-ui  ## Run the docker build for all the projects

docker-build-ui: build-ui  ## Runs the docker build for the corp-hq ui project
	docker build -t corp-hq-ui ./app/ui

help:  ## Prints this help message.
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'