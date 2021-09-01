[<RequireQualifiedAccess>]
module Login

open Elmish
open Thoth.Json
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
    | LoginRequestRecieved of Result<LoginResult, string>
    | LoginErrored of exn

let init () =
    { Username = ""
      Password = ""
      LoginState = NotLoaded }, Cmd.none

let loginDecoder = Decode.Auto.generateDecoder<LoginResult>()

let requestLogin (username, password) =
    Http.post "login" None
    |> Promise.map (Decode.fromString loginDecoder)

let loginCmd state =
    Cmd.OfPromise.either requestLogin (state.Username, state.Password) LoginRequestRecieved LoginErrored

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
        { state with LoginState = Loading }, (loginCmd state)          

    | LoginRequestRecieved result ->
        result
        |> Result.bind parseResult
        |> function
            | Ok r -> { state with LoginState = Loaded r }, Cmd.none
            | Error msg -> { state with LoginState = LoginError msg }, Cmd.none

    | LoginErrored ex ->
        { state with LoginState = LoginError ex.Message }, Cmd.none

let render state dispatch =
    Html.div [
        // TODO: Fill this in!
    ]