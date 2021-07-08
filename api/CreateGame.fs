namespace Company.Function

open System
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Types

module CreateGame =
    let private scramble sqn =
        let rnd = Random()

        let rec scrambleInner sqn =
            /// Removes an element from a sequence.
            let remove n sqn = sqn |> Seq.filter (fun x -> x <> n)

            seq {
                let x =
                    sqn |> Seq.item (rnd.Next(0, sqn |> Seq.length))

                yield x
                let sqn' = remove x sqn

                if not (sqn' |> Seq.isEmpty) then
                    yield! scrambleInner sqn'
            }

        scrambleInner sqn

    [<FunctionName("CreateGame")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = "game")>] req: HttpRequest)
        ([<CosmosDB("trivia",
                    "game",
                    ConnectionStringSetting = "CosmosConnection",
                    SqlQuery = "select q.id from q where q.modelType = 'Question'")>] questions: {| id: string |} seq)
        ([<CosmosDB("trivia", "game", ConnectionStringSetting = "CosmosConnection")>] game: outref<Game>)
        (log: ILogger)
        =
        log.LogInformation("Creating Game")

        let questions =
            questions
            |> scramble
            |> Seq.take 10
            |> Seq.map (fun q -> q.id)

        game <-
            { Id = Guid.NewGuid().ToString().Substring(0, 4)
              Questions = questions |> Seq.toArray
              Players = Array.empty
              Answers = Array.empty
              State = GameState.WaitingForPlayers
              ModelType = "Game" }

        OkObjectResult game
