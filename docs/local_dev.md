# Local Development Guide
- [Local Development Guide](#local-development-guide)
  - [General Notes](#general-notes)
  - [Requirements](#requirements)
  - [Install Dependencies](#install-dependencies)
  - [Running the applications and dependencies](#running-the-applications-and-dependencies)
  - [Getting Help](#getting-help)
    - [Core developers](#core-developers)

## General Notes
* `<root>` will signify the root directory of this solution. This is the directory with "LICENSE" and "app" in it.

## Requirements
* [DotNet Core SDK](https://www.microsoft.com/net/download)
* [Vue.JS](https://vuejs.org) / `vlue-cli` via NPM
* Some kind of editor
  * Most are using [VS Code](https://code.visualstudio.com/download) with the following extensions. This is a "cleaned up" list with items only relivant to this project.
    * christian-kohler.npm-intellisense
    * dariofuzinato.vue-peek
    * donjayamanne.githistory
    * eg2.vscode-npm-script
    * jesschadwick.nuget-reverse-package-search
    * jmrog.vscode-nuget-package-manager
    * mohsen1.prettify-json
    * ms-vscode.csharp
    * octref.vetur
    * yzhang.markdown-all-in-one

## Install Dependencies
* UI Project
  * `cd <root>/app/ui`
  * `npm install`
* API Project
  * `cd <root>/app/api`
  * `dotnet restore`

## Running the applications and dependencies
* Notes
  * For users using linux or MacOS there is a `Makefile.template` that may be useful. The makefile template is used to offer useful things between menbers of the team. Your actual `Makefile` should never be committed to the project simplay because each contributor may have different security setups.
  * Most, if not all, of the non-external dependences should be controllable through the standard `docker-compose` services. The `up` and `down` commands are usually sufficient but feel free to see `docker-compose` for help.
  * For those that can run `make` on their system the `Makefile` has a few handy shortcuts for common actions taken by devs.
* Dependencies
  * MongoDB
    * `docker-compose up -d mongodb`
* UI Project
  * `cd <root>/app/ui`
  * `npm run dev`
* API Project
  * `cd <root>/app/api`
  * `dotnet run`

## Getting Help
One excellent way of getting help getting your environment set up is to ask a question on the [Discord](https://discord.gg/FvTw9pF) chat. Join the "help" channel and post your question!

### Core developers
As cores join and leave the project we will try to keep this list as up to date as possible.

| Developer (GitHub user) | Environment   |
| ----------------------- | ------------- |
| fritogotlayed           | Mac OS        |
| naitp                   | Linux         |