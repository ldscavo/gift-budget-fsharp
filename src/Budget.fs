module Budget

open Fetch
open Thoth.Json
open Model

let requestBudget (id, apiKey) =
  promise {
    let! response =
      fetch (sprintf "https://gifting-budget.herokuapp.com/api/budgets/%i/expanded" id)
        [ requestHeaders [ Authorization (sprintf "Bearer %s" apiKey) ] ]   
    
    let! data = response.text ()

    return data |> Decode.fromString BudgetResponse.Decoder
  }