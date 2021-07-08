namespace Company.Function

open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Types

module GetGame =
    [<FunctionName("GetGame")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Function, "get", Route = "game/{id}")>] req: HttpRequest)
        ([<CosmosDB("trivia", "game", ConnectionStringSetting = "CosmosConnection", Id = "{id}", PartitionKey = "Game")>] game: Game)
        (log: ILogger)
        =
        match box game with
        | null -> NotFoundResult() :> IActionResult
        | game -> OkObjectResult game :> IActionResult
