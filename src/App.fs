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

let getPageStateFromUrl (url: string list)  =
    match url, Utils.getSession "apiKey" with
    | [ "budgets"; Route.Int id ], Some key ->
        let (state, cmd) = Budget.init key id
        BudgetPage state, Cmd.map BudgetEvent cmd 
    | [ "login" ], _ ->
        LoginPage (Login.init () |> fst), Cmd.none
    | _, _ ->
        LoginPage (Login.init () |> fst), Cmd.none

let init () =
    let url = Router.currentUrl ()
    let perhapsKey = Utils.getSession "apiKey"
    
    let (page, cmd) =
        getPageStateFromUrl url 

    { Url = url
      Page = page
      ApiKey = perhapsKey }, cmd

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
        let (page, cmd) = getPageStateFromUrl url
        { state with
            Url = url
            Page = page }, cmd

    | _, _ -> state, Cmd.none

let isMainColor = IsCustomColor "main-color"

let navbar () =
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

let render (state: State) (dispatch: Event -> unit) : ReactElement =
    let pageContent page  =
        match page with
        | LoginPage login -> Login.render login (LoginEvent >> dispatch)
        | BudgetPage budget -> Budget.render budget (BudgetEvent >> dispatch) 
        
    Bulma.container [
        container.isWidescreen
        prop.children [
            navbar ()
            Bulma.content [
                React.router [
                    router.onUrlChanged (UrlChanged >> dispatch)
                    router.children [pageContent state.Page ]
                ]
            ]
        ]
    ]

Program.mkProgram init update render
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
