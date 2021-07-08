module Result

open Feliz
open Thoth.Fetch
open Thoth.Json
open Types

let formatQuestion correct question =
    sprintf "%s %s" (if correct then "✔" else "❌") question

let formatAnswer ca a =
    sprintf "%s%s" (if ca = a then "👉 " else "") a

[<ReactComponent>]
let ResultPage (props: {| gameId: string; playerId: string |}) =
    let (results, setResults) = React.useState<GameResult option> None

    React.useEffect (
        (fun _ ->
            promise {
                let url =
                    sprintf "/api/game/%s/player/%s/results" props.gameId props.playerId

                let! results = Fetch.get<_, GameResult> (url, caseStrategy = CamelCase)

                setResults (Some results)
            }
            |> Promise.start),
        [||]
    )

    match results with
    | None -> Html.section [ Html.h1 [ prop.text "Fetching results..." ] ]
    | Some results ->
        let answers =
            results.Answers
            |> Array.map
                (fun answer ->
                    Html.div [ prop.children (
                                   [| Html.h2 [ prop.text (formatQuestion answer.Correct answer.Question) ]
                                      Html.ul (
                                          answer.Answers
                                          |> Array.map
                                              (fun a -> Html.li [ prop.text (formatAnswer answer.CorrectAnswer a) ])
                                      ) |]

                               ) ])

        Html.section [ prop.children (
                           [| Html.h1 [ prop.text "Your results" ] |]
                           |> Array.append answers
                           |> Array.toList
                           |> List.rev
                       ) ]
