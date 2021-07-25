module Http

open Fetch
open Thoth.Json
open Model

let requestOptions method (apiKey: string option) =
  match apiKey with
  | Some key ->
    [ requestHeaders
        [ Authorization (sprintf "Bearer %s" key)
          Origin "*" ]
      Method method ]
  | None ->
    [ requestHeaders [ Origin "*" ]
      Method method ]

let makeRequest method endpoint apiKey =
  let url = sprintf "https://gifting-budget.herokuapp.com/api/%s" endpoint
  let requestOpts = requestOptions method apiKey

  fetch url requestOpts
  |> Promise.bind(fun response -> response.text ())

let httpGet = makeRequest HttpMethod.GET
let httpPost = makeRequest HttpMethod.POST
 
let requestBudget (id, apiKey) =
  httpGet (sprintf "budgets/%i/expanded" id) apiKey
  |> Promise.map (Decode.fromString BudgetResponse.Decoder)