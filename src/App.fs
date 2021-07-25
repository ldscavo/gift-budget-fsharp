module App

open Fable.Core
open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fulma
open Model
open Budget
open Login

[<Emit("process.env[$0] ? process.env[$0] : ''")>]
let config (key: string) : string = jsNative

type Page =
  | LoginPage
  | BudgetPage

type BudgetModel =
  | Budget of Budget
  | LoadingBudget
  | Failed

type Model =
  { Page: Page
    BudgetModel: BudgetModel
    LoginModel: LoginModel }

type Msg =
  | GetBudgetResponse of Result<BudgetResponse, string>
  | Fail of string
  | FailWithError of exn

let apiKey = config "API_KEY"

let requestBudgetCmd id =
  Cmd.OfPromise.either requestBudget (id, Some apiKey) GetBudgetResponse FailWithError

let loginInit =
  { LoginState = Entry }

let init () =
  { Page = LoginPage
    BudgetModel = LoadingBudget
    LoginModel = loginInit }, (requestBudgetCmd 1)

let update msg model =
  match msg with
  | GetBudgetResponse response ->
    match response with
    | Ok budget -> { model with BudgetModel = Budget budget.Data }, Cmd.none
    | Error err-> model, Cmd.ofMsg (Fail err)

  | Fail message ->
    printfn "oh no! %s" message
    model, Cmd.none

  | FailWithError err -> model, Cmd.ofMsg (Fail err.Message)  

let isMainColor = IsCustomColor "main-color"

let budgetView model dispatch =
  match model.BudgetModel with
  | LoadingBudget -> div [] [ str "LOADING..." ]
  | Failed -> div [] [ str ":(" ]
  | Budget budget -> div [] [ str budget.Name ]

let view model dispatch =
  Container.container [ Container.IsWideScreen ]
    [ Navbar.navbar [ Navbar.Color isMainColor ]
        [ Navbar.Brand.div [ Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is3) ] ]
            [ Navbar.Item.a
                [ Navbar.Item.Props [ Id "logo-text"; Href "#" ] ] 
                [ div [ Id "logo" ] []
                  span [ Style [ TextShadow "1px 1px #2c3e50" ] ] [ str "Gift Budget" ] ] ] ]        
      Content.content []
        [ match model.Page with
          | LoginPage -> (loginView model.LoginModel dispatch)
          | BudgetPage -> (budgetView model dispatch) ] ]

// App
Program.mkProgram init update view
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
