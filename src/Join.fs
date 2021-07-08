module Join

open Feliz
open Thoth.Fetch
open Thoth.Json
open Feliz.Router
open Types

[<ReactComponent>]
let JoinPage
    (props: {| gameId: string
               onStarted: InProgressGame -> unit |})
    =
    let (name, setName) = React.useState ""
    let (joining, setJoining) = React.useState false

    React.useEffect
        (fun _ ->
            if joining then
                promise {
                    let url =
                        sprintf "/api/game/%s/player/%s" props.gameId name

                    let! gameWithPlayer = Fetch.put<_, Game> (url, (), caseStrategy = CamelCase)

                    let! gameInfo =
                        Fetch.put<_, InProgressGame> (
                            sprintf "/api/game/%s/start" props.gameId,
                            caseStrategy = CamelCase
                        )

                    props.onStarted gameInfo

                    Router.navigate ("game", props.gameId, "play", gameWithPlayer.Players |> Array.last)
                }
                |> Promise.start)

    Html.section [ Html.h1 [ prop.text "Join the same" ]
                   Html.div [ Html.label [ prop.text "Enter your name"
                                           prop.htmlFor "name" ]
                              Html.input [ prop.placeholder "Type here"
                                           prop.onChange (fun e -> setName (e))
                                           prop.type' "text"
                                           prop.id "name"
                                           prop.value name ]
                              Html.button [ prop.text "Join the game"
                                            prop.disabled ((name = ""))
                                            prop.onClick (fun _ -> setJoining true) ] ] ]
