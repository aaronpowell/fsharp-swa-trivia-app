module Types

type Question =
    { Id: string
      ModelType: string
      Category: string
      Question: string
      CorrectAnswer: string
      IncorrectAnswers: string array }

type GameState =
    | WaitingForPlayers = 0
    | InProgress = 1
    | Completed = 2

type Answer =
    { QuestionId: string
      PlayerId: string
      Answer: string }

type Game =
    { Id: string
      Questions: string []
      Players: string []
      Answers: Answer []
      State: GameState
      ModelType: string }

type Player =
    { Id: string
      Name: string
      ModelType: string }

type InProgressGame =
    { Id: string
      Questions: {| Id: string
                    Question: string
                    Answers: string array |} array }

type GameResult =
    { Id: string
      Answers: {| Question: string
                  Answer: string
                  Correct: bool
                  CorrectAnswer: string
                  Answers: string [] |} array }
