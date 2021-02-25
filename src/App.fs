module App

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fulma

type Model =
  { Count: int }

type Msg =
| Increment
| Decrement

let init () = { Count = 1 }

let update (msg:Msg) (model:Model) =
  match msg with
  | Increment -> { model with Count = model.Count + 1 }
  | Decrement -> { model with Count = model.Count - 1 }

let isMainColor = IsCustomColor "main-color"

let view (model:Model) dispatch =
  Container.container [ Container.IsWideScreen ]
    [ Navbar.navbar [ Navbar.Color isMainColor ]
        [ Navbar.Brand.div [ Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is3) ] ]
            [ Navbar.Item.a
                [ Navbar.Item.Props [ Id "logo-text" ;Href "#" ] ] 
                [ div [ Id "logo" ] []
                  span [ Style [ TextShadow "1px 1px #2c3e50" ] ] [ str "Gift Budget" ] ] ] ]
        
      Content.content []
        [ Button.button [ Button.OnClick (fun _ -> dispatch Increment) ] [ str "+" ]
          div [] [ str (string model.Count) ]
          Button.button [ Button.OnClick (fun _ -> dispatch Decrement) ] [ str "-" ] ] ]

// App
Program.mkSimple init update view
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
|> Program.run
