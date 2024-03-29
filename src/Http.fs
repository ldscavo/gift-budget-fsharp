[<RequireQualifiedAccess>]
module Http

open Fetch

let requestOptions method (apiKey: string option) =
    match apiKey with
    | Some key ->
        [ requestHeaders
            [ Authorization (sprintf "Bearer %s" key)
              Origin "*"
              ContentType "application/json" ]
          Method method ]
    | None ->
        [ requestHeaders
            [ Origin "*"
              ContentType "application/json" ]
          Method method ]

let get apiKey endpoint =
    let url = sprintf "https://gifting-budget.herokuapp.com/api/%s" endpoint
    let requestOpts = requestOptions HttpMethod.GET apiKey

    fetch url requestOpts
    |> Promise.bind(fun response -> response.text ())
