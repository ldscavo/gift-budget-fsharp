[<RequireQualifiedAccess>]
module Login

open Elmish
open Fetch
open Thoth.Json
open Thoth.Fetch
open Feliz

type LoginResult =
    { Token: string }

type LoginState =
    | NotLoaded
    | Loading
    | LoginError
    | Loaded of LoginResult

type State =
    { Username: string
      Password: string
      LoginState: LoginState }

type Event =
    | UsernameChanged of string
    | PasswordChanged of string
    | LoginSubmitted
    | LoginRequestRecieved of Result<LoginResult, FetchError>
    | LoginSucceeded of string
    | LoginErrored of exn

let init () =
    { Username = ""
      Password = ""
      LoginState = NotLoaded }, Cmd.none

type RequestData =
    { Email: string
      Password: string }

let headers =
    [ requestHeaders
        [ Origin "*"
          ContentType "application/json" ] ]

let requestLogin data =
    Fetch.tryPost<RequestData, LoginResult>("https://gifting-budget.herokuapp.com/api/login", data, headers, caseStrategy = CamelCase)

let loginCmd username password =
    Cmd.OfPromise.either requestLogin { Email = username; Password = password } LoginRequestRecieved LoginErrored

let update event state =
    match event with
    | UsernameChanged username ->
        { state with Username = username }, Cmd.none
    
    | PasswordChanged password ->
        { state with Password = password }, Cmd.none

    | LoginSubmitted ->
        { state with LoginState = Loading }, (loginCmd state.Username state.Password)          

    | LoginRequestRecieved result ->
        match result with
        | Ok data -> { state with LoginState = Loaded data }, Cmd.ofMsg (LoginSucceeded data.Token)
        | Error _ -> { state with LoginState = LoginError }, Cmd.none

    | LoginErrored _ ->
        { state with LoginState = LoginError }, Cmd.none

    | LoginSucceeded key ->
        Utils.setSession "apiKey" key
        state, Router.Cmd.navigate ("budgets", 1)

let isLoading = function
    | Loading -> true
    | _ -> false   

let isErrored = function
    | LoginError -> true
    | _ -> false

let render state dispatch =
    Html.div [
        Html.form [
            prop.onSubmit (fun e ->
                e.preventDefault ()
                dispatch LoginSubmitted)
            prop.children [
                Html.div [
                    Html.label [
                        prop.htmlFor "email"
                        prop.text "Email:"
                    ]
                    Html.br []
                    Html.input [
                        prop.id "email"
                        prop.type'.email
                        prop.valueOrDefault state.Username
                        prop.onChange (UsernameChanged >> dispatch)
                    ]
                ]
                Html.div [
                    Html.label [
                        prop.htmlFor "password"
                        prop.text "Password:"
                    ]
                    Html.br []
                    Html.input [
                        prop.id "password"
                        prop.type'.password
                        prop.valueOrDefault state.Password
                        prop.onChange (PasswordChanged >> dispatch)
                    ]
                ]
                Html.div [
                    Html.input [
                        prop.type'.submit
                        prop.disabled (isLoading state.LoginState)
                        prop.value "Log In"
                    ]
                ]
                Html.div [
                    prop.hidden (isErrored state.LoginState |> not)                        
                    prop.text "Invalid username or password"
                ]
            ]
        ]
    ]