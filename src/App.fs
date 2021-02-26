module App

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fulma
open Model
open Budget

type Model =
| Loading
| Finished of Budget

type Msg =
| SetBudget of Budget
| AddUser

let init () =
  let loadBudgetCmd dispatch =
    let setBudget = async {
      let! budget = loadBudget ()
      dispatch (SetBudget budget)
    }
    Async.StartImmediate setBudget

  Loading, Cmd.ofSub loadBudgetCmd

let update msg model =
  match msg with
  | SetBudget budget -> Finished budget, Cmd.none
  | AddUser -> model, Cmd.none

let isMainColor = IsCustomColor "main-color"

let view model dispatch =
  Container.container [ Container.IsWideScreen ]
    [ Navbar.navbar [ Navbar.Color isMainColor ]
        [ Navbar.Brand.div [ Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is3) ] ]
            [ Navbar.Item.a
                [ Navbar.Item.Props [ Id "logo-text"; Href "#" ] ] 
                [ div [ Id "logo" ] []
                  span [ Style [ TextShadow "1px 1px #2c3e50" ] ] [ str "Gift Budget" ] ] ] ]
        
      Content.content []
        [ match model with
          | Loading -> div [] [ str "LOADING..." ]
          | Finished budget -> div [] [ str budget.Name ] ] ]

// App
Program.mkProgram init update view
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
