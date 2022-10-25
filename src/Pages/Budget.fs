[<RequireQualifiedAccess>]
module Budget

open Model
open Elmish
open Feliz
open Thoth.Json

type ApiResponse<'a> =
    | Loading
    | Failed of string
    | Loaded of 'a

type State =
    { Budget: ApiResponse<Budget>
      ApiKey: string
      BudgetId: int }

type Event =
    | BudgetLoaded of Result<BudgetResponse, string>
    | FailWithError of exn

let requestBudget (id, apiKey) =
    Http.get (Some apiKey) (sprintf "budgets/%i/expanded" id)
    |> Promise.map (Decode.fromString BudgetResponse.Decoder)

let requestBudgetCmd apiKey id =
  Cmd.OfPromise.either requestBudget (id, apiKey) BudgetLoaded FailWithError

let init apiKey id =
    { Budget = Loading
      ApiKey = apiKey
      BudgetId = id }, (requestBudgetCmd apiKey id)

let update event state =
    match event with
    | BudgetLoaded response ->
        match response with
        | Ok budget ->
            { state with Budget = Loaded budget.Data }, Cmd.none
        | Error err ->
            { state with Budget = Failed err }, Cmd.none

    | FailWithError ex ->
        { state with Budget = Failed ex.Message }, Cmd.none

let renderBudget (budget: Budget) (dispatch: Event -> unit) =
    Html.div [
        Html.h1 budget.Name
    ]

let render state dispatch =
    match state.Budget with
    | Loading -> Html.div "LOADING..."
    | Failed msg -> Html.div (sprintf "Could not load budget: %s" msg)
    | Loaded budget ->
        renderBudget budget dispatch