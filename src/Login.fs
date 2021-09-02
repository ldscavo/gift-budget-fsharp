[<RequireQualifiedAccess>]
module Login

open Fable.Core
open Elmish
open Fetch
open Thoth.Json
open Thoth.Fetch
open Feliz

type LoginData =
    { Token: string }

type LoginError =
    { Error: string }

type LoginResult =
    | LoginSuccess of LoginData
    | LoginFailure of LoginError

type LoginState =
    | NotLoaded
    | Loading
    | LoginError of string
    | Loaded of LoginData

type State =
    { Username: string
      Password: string
      LoginState: LoginState }

type Event =
    | UsernameChanged of string
    | PasswordChanged of string
    | LoginSubmitted
    | LoginRequestRecieved of LoginResult
    | LoginErrored of exn

let init () =
    { Username = ""
      Password = ""
      LoginState = NotLoaded }, Cmd.none

let loginDecoder = Decode.Auto.generateDecoder<LoginResult>()

type RequestData =
    { Email: string
      Password: string }

let headers =
    [ requestHeaders
        [ Origin "*"
          ContentType "application/json" ] ]

let requestLogin data =
    Fetch.post<RequestData, LoginResult>("https://gifting-budget.herokuapp.com/api/login", data, headers, caseStrategy = CamelCase)

let loginCmd username password =
    Cmd.OfPromise.either requestLogin { Email = username; Password = password } LoginRequestRecieved LoginErrored

let parseResult = function
    | LoginSuccess data -> Ok data
    | LoginFailure error -> Error error.Error

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
        | LoginSuccess data ->
            { state with LoginState = Loaded data }, Cmd.none
        | LoginFailure error ->
            { state with LoginState = LoginError error.Error }, Cmd.none

    | LoginErrored ex ->
        { state with LoginState = LoginError ex.Message }, Cmd.none

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
                        prop.value "Log In"
                    ]
                ]
            ]
        ]
    ]