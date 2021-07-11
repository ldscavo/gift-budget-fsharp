module Budget

open Fetch
open Thoth.Json
open Model

let requestBudget (id, apiKey) =
  promise {
    let url = sprintf "https://gifting-budget.herokuapp.com/api/budgets/%i/expanded" id
    let requestOpts =
      [ requestHeaders
          [ Authorization (sprintf "Bearer %s" apiKey)
            Origin "*" ] ]

    let! data =
      fetch url requestOpts
      |> Promise.bind(fun response -> response.text ())

    return data |> Decode.fromString BudgetResponse.Decoder
  }