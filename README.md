# F# Trivia Game

This repository contains an F# trivia game that has been built for [Azure Static Web Apps](https://docs.microsoft.com/azure/app-service-static-web/?WT.mc_id=dotnet-33392-aapowell), showing off how you can use [Fable](https://fable.io) for a front end and [F# Azure Functions](https://docs.microsoft.com/azure/functions/?WT.mc_id=dotnet-33392-aapowell) for the backend. Data is then stored in [CosmosDB](https://docs.microsoft.com/azure/cosmos-db/?WT.mc_id=dotnet-33392-aapowell).

You can find a deployed version of the application at [https://black-glacier-08edf5f10.azurestaticapps.net/](https://black-glacier-08edf5f10.azurestaticapps.net/).

## Running the app

This repo is designed to be used in a [VS Code Remote Container](https://code.visualstudio.com/docs/editor/remote-containers?WT.mc_id=dotnet-33392-aapowell), so you'll need VS Code and Docker installed, the devcontainer will take care of the rest of the dependencies.

You'll need to create a [CosmosDB account and a CosmosDB database](https://docs.microsoft.com/azure/cosmos-db/create-cosmosdb-resources-portal/?WT.mc_id=dotnet-33392-aapowell), with a database named `trivia` and a collection named `game`. A sample question dataset can be found at [`api/trivia.json`](api/trivia.json), which has been exported from [Open TriviaDB](https://opentdb.com/).

Once your CosmosDB account is created, you'll need to create a `local.settings.json` file in the `api` folder to put the connection string for CosmosDB:

```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "CosmosConnection": "YOUR_CONNECTION_STRING_HERE"
    }
}
```

_Note: this file is not part of source control, we don't want secrets on GitHub 😉._

Launch the application with the `Launch it all 🚀` VS Code debugger option and navigate to `http://localhost:4280` in your browser.

_Note: If the site isn't accessible, ensure that the `Run emulator` command launched the [Static Web Apps CLI](https://github.com/azure/static-web-apps-cli) properly. Sometimes it fails if Fable takes too long to start, so just restart it._

## Resources

* [Fable + Feliz GitHub repo template](https://github.com/aaronpowell/swa-feliz-template)
* [Fable](https://fable.io)
* [YouTube Live Stream showing it off](https://youtu.be/wBP8k1ZuRmQ)
* [Creating Azure Functions with F#](https://www.aaron-powell.com/posts/2020-01-13-creating-azure-functions-in-fsharp/)
* [More Fable SWA templates](https://www.aaron-powell.com/posts/2021-07-09-creating-static-web-apps-with-fsharp-and-fable/)