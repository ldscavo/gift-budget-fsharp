module Login

open Fable.React
open Fulma

type LoginState =
  | Entry
  | CheckingLogin
  | Failure of string

type LoginModel =
  { LoginState: LoginState }

type LoginEvents =
  | MakeLoginRequest of Result<string, string>
  | RedirectToBudgetPage

let submitButton model =
  let loginDisabled =
    match model.LoginState with
    | CheckingLogin -> true
    | _ -> false

  Button.button
    [ Button.Color IsPrimary; Button.Disabled loginDisabled ]
    [ str "Log in" ]

let loginView model dispatch =
  Container.container []
    [ Label.label [] [str "Email"]
      Field.div []
        [ Input.email [ Input.DefaultValue "test@example.com" ] ]
      Label.label [] [str "Password"]
      Field.div []
        [ Input.password [] ]
      Field.div [ Field.IsGrouped ]
        [ Control.div [] [ submitButton model ] ] ]