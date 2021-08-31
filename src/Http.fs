[<RequireQualifiedAccess>]
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

let request method endpoint apiKey =
    let url = sprintf "https://gifting-budget.herokuapp.com/api/%s" endpoint
    let requestOpts = requestOptions method apiKey

    fetch url requestOpts
    |> Promise.bind(fun response -> response.text ())

let get = request HttpMethod.GET

let post = request HttpMethod.POST