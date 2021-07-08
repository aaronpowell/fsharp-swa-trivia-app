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

module GetResults =
    // Define a nullable container to deserialize into.
    [<AllowNullLiteral>]
    type NameContainer() =
        member val Name = "" with get, set

    // For convenience, it's better to have a central place for the literal.
    [<Literal>]
    let Name = "name"

    [<FunctionName("GetResults")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Function, "get", Route = "game/{gameId}/player/{playerId}/results")>] req: HttpRequest)
        ([<CosmosDB("trivia",
                    "game",
                    ConnectionStringSetting = "CosmosConnection",
                    Id = "{gameId}",
                    PartitionKey = "Game")>] game: Game)
        ([<CosmosDB("trivia",
                    "game",
                    ConnectionStringSetting = "CosmosConnection",
                    SqlQuery = "select * from q where q.modelType = 'Question'")>] questions: Question seq)
        playerId
        =
        match box game with
        | null -> NotFoundResult() :> IActionResult
        | _ ->
            let gameQuestions =
                questions
                |> Seq.filter (fun q -> game.Questions |> Array.contains q.Id)

            let gameResults =
                { Id = game.Id
                  Answers =
                      game.Answers
                      |> Array.filter (fun a -> a.PlayerId = playerId)
                      |> Array.map
                          (fun a ->
                              let question =
                                  gameQuestions
                                  |> Seq.find (fun q -> q.Id = a.QuestionId)

                              {| Answer = a.Answer
                                 Question = question.Question
                                 Correct = question.CorrectAnswer = a.Answer
                                 CorrectAnswer = question.CorrectAnswer
                                 Answers =
                                     question.IncorrectAnswers
                                     |> Array.append [| question.CorrectAnswer |]
                                     |> Array.sort |}) }

            OkObjectResult gameResults :> IActionResult
