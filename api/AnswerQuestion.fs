namespace Company.Function

open System
open System.IO
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open Types

module AnswerQuestion =
    [<FunctionName("AnswerQuestion")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = "game/{gameId}/answer")>] req: HttpRequest)
        ([<CosmosDB("trivia",
                    "game",
                    ConnectionStringSetting = "CosmosConnection",
                    Id = "{gameId}",
                    PartitionKey = "Game")>] game: Game)
        ([<CosmosDB("trivia", "game", ConnectionStringSetting = "CosmosConnection")>] updatedGameCollector: IAsyncCollector<Game>)
        =
        async {
            match box game with
            | null -> return NotFoundResult() :> IActionResult
            | _ ->
                use stream = new StreamReader(req.Body)
                let! reqBody = stream.ReadToEndAsync() |> Async.AwaitTask

                let answer =
                    JsonConvert.DeserializeObject<Answer>(reqBody)

                return!
                    if (not (Array.contains answer.QuestionId game.Questions)
                        || not (Array.contains answer.PlayerId game.Players)) then
                        async {
                            return
                                BadRequestObjectResult("This player can't answer this question for this game")
                                :> IActionResult
                        }
                    else
                        async {
                            let updatedGame =
                                { game with
                                      Answers = game.Answers |> Array.append [| answer |] }

                            do!
                                { game with
                                      Answers = game.Answers |> Array.append [| answer |] }
                                |> updatedGameCollector.AddAsync
                                |> Async.AwaitIAsyncResult
                                |> Async.Ignore

                            return OkObjectResult updatedGame :> IActionResult
                        }


        }
        |> Async.StartAsTask
