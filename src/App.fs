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
    | LoginPage
    | BudgetPage

type State =
    { Page: Page
      LoginState: Login.State
      BudgetState: Budget.State }

type Event =
    | LoginEvent of Login.Event
    | BudgetEvent of Budget.Event

let apiKey = config "API_KEY"

let init () =
    let budgetState, cmd = Budget.init apiKey 1
    let (loginState, _) = Login.init ()

    { Page = LoginPage
      LoginState = loginState
      BudgetState = budgetState }, Cmd.map BudgetEvent cmd

let update event state =
    match event with
    | BudgetEvent evnt ->
        let (budget, cmd) = Budget.update evnt state.BudgetState
        { state with
            BudgetState = budget }, Cmd.map BudgetEvent cmd

    | LoginEvent evnt ->
        let (login, cmd) = Login.update evnt state.LoginState
        { state with
            LoginState = login }, Cmd.map LoginEvent cmd

let isMainColor = IsCustomColor "main-color"

let render state (dispatch: Event -> unit) =
  Container.container [ Container.IsWideScreen ]
    [ Navbar.navbar [ Navbar.Color isMainColor ]
        [ Navbar.Brand.div [ Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is3) ] ]
            [ Navbar.Item.a
                [ Navbar.Item.Props [ Id "logo-text"; Href "#" ] ] 
                [ div [ Id "logo" ] []
                  span [ Style [ TextShadow "1px 1px #2c3e50" ] ] [ str "Gift Budget" ] ] ] ]        
      Content.content []
        [ match state.Page with
          | LoginPage -> Login.render state.LoginState (LoginEvent >> dispatch)
          | BudgetPage -> Budget.render state.BudgetState (BudgetEvent >> dispatch) ] ]

// App
Program.mkProgram init update render
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
