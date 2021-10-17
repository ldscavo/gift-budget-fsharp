module App

open Fable.Core
open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fulma
open Feliz
open Feliz.Router
open Feliz.Bulma

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

let changePage (url: string list) (state: State) =
    match url, Utils.getSession "apiKey" with
    | [ "budgets"; Route.Int id ], Some key ->
        let (state, cmd) = Budget.init key id
        BudgetPage state, Cmd.map BudgetEvent cmd 
    | [ "login" ], _ ->
        LoginPage (Login.init () |> fst), Cmd.none
    | _, _ ->
        LoginPage (Login.init () |> fst), Cmd.none

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
        let (pageState, cmd) = changePage url state
        { state with
            Url = url
            Page = pageState }, cmd

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

let render2 (state: State) (dispatch: Event -> unit) : ReactElement =
    let pageContent page  =
        match page with
        | LoginPage login -> Login.render login (LoginEvent >> dispatch)
        | BudgetPage budget -> Budget.render budget (BudgetEvent >> dispatch) 
        
    Bulma.container [
        container.isWidescreen
        prop.children [
            Bulma.navbar [
                prop.classes ["is-main-color"]
                prop.children [                
                    Bulma.navbarStart.div [
                        Bulma.navbarBrand.div [  
                            size.isSize3
                            prop.children [                                            
                                Bulma.navbarItem.a [
                                    prop.id "logo-text"
                                    prop.children [
                                        Html.div [prop.id "logo"]
                                        Html.span  "Gift Budget"                                        
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
            Bulma.content [
                React.router [
                    router.onUrlChanged (UrlChanged >> dispatch)
                    router.children [pageContent state.Page ]
                ]
            ]
        ]
    ]


Program.mkProgram init update render2
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
