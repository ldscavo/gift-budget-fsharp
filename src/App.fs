module App

open Fable.Core
open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fulma
open Model
open Budget

[<Emit("process.env[$0] ? process.env[$0] : ''")>]
let config (key: string) : string = jsNative

type BudgetModel =
  | Budget of Budget
  | Loading
  | Failed

type Model =
  { Budget: BudgetModel }

type Msg =
  | GetBudgetResponse of Result<BudgetResponse, string>
  | Fail of string
  | FailWithError of exn

let apiKey = config "API_KEY"

let requestBudgetCmd id =
  Cmd.OfPromise.either requestBudget (id, apiKey) GetBudgetResponse FailWithError

let init () =
  { Budget = Loading }, (requestBudgetCmd 1)

let update msg model =
  match msg with
  | GetBudgetResponse response ->
    match response with
    | Ok budget -> { model with Budget = Budget budget.Data }, Cmd.none
    | Error err-> model, Cmd.ofMsg (Fail err)

  | Fail message ->
    printfn "oh no! %s" message
    model, Cmd.none

  | FailWithError err -> model, Cmd.ofMsg (Fail err.Message)  


let view model dispatch =
  let isMainColor = IsCustomColor "main-color"

  Container.container [ Container.IsWideScreen ]
    [ Navbar.navbar [ Navbar.Color isMainColor ]
        [ Navbar.Brand.div [ Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is3) ] ]
            [ Navbar.Item.a
                [ Navbar.Item.Props [ Id "logo-text"; Href "#" ] ] 
                [ div [ Id "logo" ] []
                  span [ Style [ TextShadow "1px 1px #2c3e50" ] ] [ str "Gift Budget" ] ] ] ]        
      Content.content []
        [ match model.Budget with
          | Loading -> div [] [ str "LOADING..." ]
          | Failed -> div [] [ str ":(" ]
          | Budget budget -> div [] [ str budget.Name ] ] ]


// App
Program.mkProgram init update view
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
