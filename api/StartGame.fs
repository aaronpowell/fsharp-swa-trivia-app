namespace Company.Function

open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Types

module StartGame =
    [<FunctionName("StartGame")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Function, "put", Route = "game/{gameId}/start")>] req: HttpRequest)
        ([<CosmosDB("trivia",
                    "game",
                    ConnectionStringSetting = "CosmosConnection",
                    Id = "{gameId}",
                    PartitionKey = "Game")>] game: Game)
        ([<CosmosDB("trivia",
                    "game",
                    ConnectionStringSetting = "CosmosConnection",
                    SqlQuery = "select * from q where q.modelType = 'Question'")>] questions: Question seq)
        ([<CosmosDB("trivia", "game", ConnectionStringSetting = "CosmosConnection")>] updatedGame: outref<Game>)
        (log: ILogger)
        =
        match box game with
        | null -> NotFoundResult() :> IActionResult
        | _ ->
            match game.State with
            | GameState.WaitingForPlayers ->
                updatedGame <-
                    { game with
                          State = GameState.InProgress }
            | _ -> ()

            let gameQuestions =
                questions
                |> Seq.filter (fun q -> game.Questions |> Array.contains q.Id)
                |> Seq.map
                    (fun q ->
                        {| Id = q.Id
                           Question = q.Question
                           Answers =
                               q.IncorrectAnswers
                               |> Array.append [| q.CorrectAnswer |]
                               |> Array.sort |})

            OkObjectResult(
                { Id = game.Id
                  Questions = gameQuestions |> Seq.toArray }
            )
            :> IActionResult
