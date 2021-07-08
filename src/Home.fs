module Home

open Feliz
open Fetch
open Thoth.Fetch
open Thoth.Json
open Types
open Feliz.Router

[<ReactComponent>]
let HomePage () =
    let (starting, setStarting) = React.useState false

    React.useEffect
        (fun _ ->
            if starting then
                promise {
                    let! game = Fetch.post<unit, Game> ("/api/game", (), caseStrategy = CamelCase)

                    Router.navigate ("game", game.Id)

                    return ()
                }
                |> Promise.start)

    Html.section [ Html.h1 [ prop.text "Let's play a game" ]
                   Html.button [ prop.text "Start a new game"
                                 prop.onClick (fun _ -> setStarting true) ] ]
