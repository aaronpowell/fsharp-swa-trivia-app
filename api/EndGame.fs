namespace Company.Function

open System
open System.IO
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open Microsoft.Extensions.Logging
open Types

module EndGame =
    // Define a nullable container to deserialize into.
    [<AllowNullLiteral>]
    type NameContainer() =
        member val Name = "" with get, set

    // For convenience, it's better to have a central place for the literal.
    [<Literal>]
    let Name = "name"

    [<FunctionName("EndGame")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Function, "put", Route = "game/{gameId}/end")>] req: HttpRequest)
        ([<CosmosDB("trivia",
                    "game",
                    ConnectionStringSetting = "CosmosConnection",
                    Id = "{gameId}",
                    PartitionKey = "Game")>] game: Game)
        ([<CosmosDB("trivia", "game", ConnectionStringSetting = "CosmosConnection")>] updatedGame: outref<Game>)
        =
        match box game with
        | null -> NotFoundResult() :> IActionResult
        | _ ->
            match game.State with
            | GameState.InProgress ->
                updatedGame <-
                    { game with
                          State = GameState.Completed }
            | _ -> ()

            NoContentResult() :> IActionResult
