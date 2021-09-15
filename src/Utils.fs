module Utils

open Fable.Core

[<Emit("process.env[$0] ? process.env[$0] : ''")>]
let config (key: string) : string = jsNative

[<Emit("sessionStorage.setItem([$0], [$1])")>]
let setSession (key: string) (value: string) : unit = jsNative

[<Emit("sessionStorage.getItem([$0])")>]
let getSession (key: string) : string option = jsNative