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

module AddPlayerToGame =
    [<FunctionName("AddPlayerToGame")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "game/{gameId}/player/{name}")>] req: HttpRequest)
        ([<CosmosDB("trivia",
                    "game",
                    ConnectionStringSetting = "CosmosConnection",
                    SqlQuery = "SELECT * FROM p WHERE p.name = {name} AND p.modelType = 'Player'")>] existingPlayers: Player seq)
        ([<CosmosDB("trivia",
                    "game",
                    ConnectionStringSetting = "CosmosConnection",
                    Id = "{gameId}",
                    PartitionKey = "Game")>] game: Game)
        ([<CosmosDB("trivia", "game", ConnectionStringSetting = "CosmosConnection")>] newPlayer: outref<Player>)
        ([<CosmosDB("trivia", "game", ConnectionStringSetting = "CosmosConnection")>] updateGame: outref<Game>)
        (name: string)
        (log: ILogger)
        =
        let player =
            match existingPlayers |> Seq.toList with
            | [] ->
                let p =
                    { Id = Guid.NewGuid().ToString().Substring(0, 4)
                      ModelType = "Player"
                      Name = name }

                newPlayer <- p
                p
            | [ existingPlayer ] -> existingPlayer
            | _ -> existingPlayers |> Seq.head

        updateGame <-
            { game with
                  Players =
                      game.Players
                      |> Array.append [| player.Id |]
                      |> Array.distinct }

        OkObjectResult updateGame
