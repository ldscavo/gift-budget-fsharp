module App

(**
 The famous Increment/Decrement ported from Elm.
 You can find more info about Elmish architecture and samples at https://elmish.github.io/
*)

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fulma

// MODEL

type Model = int

type Msg =
| Increment
| Decrement

let init() : Model = 0

// UPDATE

let update (msg:Msg) (model:Model) =
    match msg with
    | Increment -> model + 1
    | Decrement -> model - 1

// VIEW (rendered with React)

let isMainColor = IsCustomColor "main-color"

let view (model:Model) dispatch =
  Container.container [ Container.IsWideScreen ]
      [ Navbar.navbar [ Navbar.Color isMainColor ]
          [ Navbar.Brand.div [ Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is3) ] ]
              [ Navbar.Item.a
                  [ Navbar.Item.Modifiers
                      [ Modifier.TextColor IsWhite
                        Modifier.TextWeight TextWeight.Bold ]
                    Navbar.Item.Props [ Href "#" ] ] 
                  [ div [ Id "logo" ] []
                    span [ Style [ TextShadow "1px 1px #2c3e50" ] ] [ str "Gift Budget" ] ] ] ]
        
        Content.content []
          [ Button.button [ Button.OnClick (fun _ -> dispatch Increment) ] [ str "+" ]
            div [] [ str (string model) ]
            Button.button [ Button.OnClick (fun _ -> dispatch Decrement) ] [ str "-" ] ] ]

// App
Program.mkSimple init update view
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
|> Program.run
