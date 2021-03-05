module Budget

open Fetch
open Thoth.Json
open Model

let loadBudget () =
  async {
    do! Async.Sleep 2000

    return
      { Id = 1L
        Name = "Test Budget"
        Amount = 500M
        Recipients =
          [ { Id = 1L
              Name = "Bekah"
              Amount = 250M
              Items = [] } ] }
  }

let loadBudgetV2 (id, apiKey) =
  promise {
    let! response =
      fetch (sprintf "https://gifting-budget.herokuapp.com/api/budgets/%i/expanded" id)
        [ requestHeaders [ Authorization (sprintf "Bearer %s" apiKey) ] ]   
    
    let! data = response.text ()

    return data |> Decode.fromString BudgetResponse.Decoder
  }