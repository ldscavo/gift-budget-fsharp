module App

open Fable.Core
open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fulma
open Feliz
open Feliz.Router

type Page =
    | LoginPage of Login.State
    | BudgetPage of Budget.State

type State =
    { Url: string list
      Page: Page
      ApiKey: string option }

type Event =
    | UrlChanged of string list
    | LoginEvent of Login.Event
    | BudgetEvent of Budget.Event

let apiKey = Utils.config "API_KEY"

let init () =
    let (loginState, _) = Login.init ()

    { Url = Router.currentUrl ()
      Page = LoginPage loginState
      ApiKey = None }, Cmd.none

let changePage (url: string list) (state: State) : Page =
    match url, Utils.getSession "apiKey" with
    | [ "budgets"; Route.Int id ], Some key -> BudgetPage (Budget.init key id |> fst)
    | [ "login" ], _ -> LoginPage (Login.init () |> fst)
    | _, _ -> LoginPage (Login.init () |> fst)

let update event state =
    match state.Page, event with
    | BudgetPage budgetState, BudgetEvent evnt ->
        let (budget, cmd) = Budget.update evnt budgetState

        { state with
            Page = BudgetPage budget }, Cmd.map BudgetEvent cmd

    | LoginPage loginState, LoginEvent evnt -> 
        let (login, cmd) = Login.update evnt loginState
        
        { state with
            Page = LoginPage login }, Cmd.map LoginEvent cmd

    | _, UrlChanged url ->
        { state with
            Url = url
            Page = changePage url state }, Cmd.none

    | _, _ -> state, Cmd.none

let isMainColor = IsCustomColor "main-color"

let render state dispatch =
    let pageContent page  =
        match page with
        | LoginPage login -> Login.render login (LoginEvent >> dispatch)
        | BudgetPage budget -> Budget.render budget (BudgetEvent >> dispatch) 

    Container.container [ Container.IsWideScreen ]
        [ Navbar.navbar [ Navbar.Color isMainColor ]
            [ Navbar.Brand.div [ Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is3) ] ]
                [ Navbar.Item.a
                    [ Navbar.Item.Props [ Id "logo-text"; Href "#" ] ] 
                    [ div [ Id "logo" ] []
                      span [ Style [ TextShadow "1px 1px #2c3e50" ] ] [ str "Gift Budget" ] ] ] ]        
          Content.content []
            [
                React.router [
                    router.onUrlChanged (UrlChanged >> dispatch)
                    router.children [pageContent state.Page ]
                ]
            ]
        ]

Program.mkProgram init update render
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
