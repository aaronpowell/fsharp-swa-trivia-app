module Play

open Feliz
open Types
open Thoth.Fetch
open Thoth.Json
open Feliz.Router

let save currentQuestion currentAnswer gameId playerId =
    promise {
        let url = sprintf "api/game/%s/answer" gameId

        let! _ =
            Fetch.post<Answer, _> (
                url,
                { QuestionId = currentQuestion.Id
                  PlayerId = playerId
                  Answer = currentAnswer }
            )

        return ()
    }

[<ReactComponent>]
let PlayPage
    (props: {| game: InProgressGame
               playerId: string |})
    =
    let (currentQuestion, setCurrentQuestion) =
        React.useState (props.game.Questions |> Seq.tryHead)

    let (remainingQuestions, setRemainingQuestions) =
        React.useState (props.game.Questions |> Seq.tail)

    let (currentAnswer, setCurrentAnswer) = React.useState ""

    let (submitAnswer, setSubmitAnswer) = React.useState false

    React.useEffect
        (fun _ ->
            if submitAnswer then
                promise {
                    let url =
                        sprintf "api/game/%s/answer" props.game.Id

                    let! _ =
                        Fetch.post (
                            url,
                            {| questionId = currentQuestion.Value.Id
                               playerId = props.playerId
                               answer = currentAnswer |},
                            caseStrategy = CamelCase
                        )

                    setSubmitAnswer false
                    setCurrentAnswer ""
                    setCurrentQuestion (remainingQuestions |> Seq.tryHead)
                    setRemainingQuestions (remainingQuestions |> Seq.tail)
                }
                |> Promise.start)

    React.useEffect (
        (fun _ ->
            match currentQuestion with
            | None -> Router.navigate ("game", props.game.Id, "result", props.playerId)
            | Some _ -> ()),
        [| box currentQuestion |]
    )

    match currentQuestion with
    | Some currentQuestion ->
        Html.section [ Html.h1 [ prop.text currentQuestion.Question ]
                       Html.ul [ prop.onChange setCurrentAnswer
                                 prop.children (
                                     currentQuestion.Answers
                                     |> Array.map
                                         (fun answer ->
                                             Html.li [ Html.label [ prop.children [| Html.input [ prop.type'.radio
                                                                                                  prop.name
                                                                                                      currentQuestion.Id
                                                                                                  prop.value answer
                                                                                                  prop.isChecked (
                                                                                                      (answer = currentAnswer)
                                                                                                  ) ]
                                                                                     Html.span [ prop.text answer ] |] ] ])
                                     |> Array.toList
                                 ) ]
                       Html.button [ prop.text "Submit Answer"
                                     prop.onClick (fun _ -> setSubmitAnswer true) ] ]
    | None -> Html.section [ Html.h1 [ prop.text "No more questions!" ] ]
