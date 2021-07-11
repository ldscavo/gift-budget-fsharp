module Budget

open Fetch
open Thoth.Json
open Model

let httpGet endpoint apiKey =
  promise {
    let url = sprintf "https://gifting-budget.herokuapp.com/api/%s" endpoint
    let requestOpts =
      [ requestHeaders
          [ Authorization (sprintf "Bearer %s" apiKey)
            Origin "*" ] ]

    return!
      fetch url requestOpts
      |> Promise.bind(fun response -> response.text ())
  }

let requestBudget (id, apiKey) =
  promise {    
    let! data = httpGet (sprintf "budgets/%i/expanded" id) apiKey
    
    return data |> Decode.fromString BudgetResponse.Decoder
  }