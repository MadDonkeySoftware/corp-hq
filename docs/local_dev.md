# Local Development Guide
- [Requirements](#requirements)

## General Notes
* `<root>` will signify the root directory of this solution. This is the directory with "LICENSE" and "app" in it.

## Requirements
* [DotNet Core SDK](https://www.microsoft.com/net/download)
* [Vue.JS](https://vuejs.org) / `vlue-cli` via NPM
* Some kind of editor
  * I am using [VS Code](https://code.visualstudio.com/download) with the following extensions. This is a "cleaned up" list with items only relivant to this project.
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
* Dependencies
  * Note: Most, if not all, of the non-external dependences should be controllable through the standard `docker-compose` services. The `up` and `down` commands are usually sufficient but feel free to see `docker-compose` for help.
  * MongoDB
    * `docker-compose up -d mongodb`
* UI Project
  * `cd <root>/app/ui`
  * `npm run dev`
* API Project
  * `cd <root>/app/api`
  * `dotnet run`