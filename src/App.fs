module App

open Feliz
open Feliz.Router
open Home
open Join
open Play
open Result
open Types

type Page =
    | Home
    | Join of string
    | Play of string * string
    | Result of string * string
    | NotFound

let parseUrl =
    function
    | [] -> Page.Home
    | [ "game"; gameId ] -> Page.Join gameId
    | [ "game"; gameId; "play"; playerId ] -> Page.Play(gameId, playerId)
    | [ "game"; gameId; "result"; playerId ] -> Page.Result(gameId, playerId)
    | _ -> Page.NotFound

[<ReactComponent>]
let App () =
    let (pageUrl, updateUrl) =
        React.useState (parseUrl (Router.currentUrl ()))

    let (currentGame, setCurrentGame) =
        React.useState<InProgressGame option> None

    let currentPage =
        match pageUrl with
        | Home -> HomePage()
        | Join gameId ->
            JoinPage
                {| gameId = gameId
                   onStarted = fun game -> setCurrentGame (Some game) |}
        | Play (gameId, playerId) ->
            match currentGame with
            | Some game -> PlayPage {| game = game; playerId = playerId |}
            | None -> Html.h1 [ prop.text "Error - game not set" ]
        | Result (gameId, playerId) ->
            ResultPage
                {| gameId = gameId
                   playerId = playerId |}
        | NotFound -> Html.h1 [ prop.text "404" ]

    React.router [ router.onUrlChanged (parseUrl >> updateUrl)
                   router.children currentPage ]

open Browser.Dom

ReactDOM.render (App(), document.getElementById "root")
