module App

open Fable.Core
open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fulma

[<Emit("process.env[$0] ? process.env[$0] : ''")>]
let config (key: string) : string = jsNative

type Page =
    | BudgetPage

type State =
    { Page: Page
      BudgetState: Budget.State }

type Event =
    | BudgetEvent of Budget.Event

let apiKey = config "API_KEY"

let init () =
    let budgetState, cmd = Budget.init apiKey 1
    
    { Page = BudgetPage
      BudgetState = budgetState }, Cmd.map BudgetEvent cmd

let update event state =
    match event with
    | BudgetEvent evnt ->
        let (budget, cmd) = Budget.update evnt state.BudgetState
        { state with
            BudgetState = budget }, Cmd.map BudgetEvent cmd

let isMainColor = IsCustomColor "main-color"

let render state dispatch =
  Container.container [ Container.IsWideScreen ]
    [ Navbar.navbar [ Navbar.Color isMainColor ]
        [ Navbar.Brand.div [ Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is3) ] ]
            [ Navbar.Item.a
                [ Navbar.Item.Props [ Id "logo-text"; Href "#" ] ] 
                [ div [ Id "logo" ] []
                  span [ Style [ TextShadow "1px 1px #2c3e50" ] ] [ str "Gift Budget" ] ] ] ]        
      Content.content []
        [ match state.Page with
          | BudgetPage -> (Budget.render state.BudgetState dispatch) ] ]

// App
Program.mkProgram init update render
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
