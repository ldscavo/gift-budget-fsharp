module Model

open Thoth.Json

type User = 
    { Id: int64
      Username: string
      Email: string }

    static member Decoder =
        Decode.object (fun get ->
          { Id = get.Required.Field "id" Decode.int64
            Username = get.Required.Field "username" Decode.string
            Email = get.Required.Field "email" Decode.string })

type Item =
    { Id: int64
      Name: string
      Price: decimal
      Purchased: bool }

    static member Decoder =
        Decode.object (fun get ->
            { Id = get.Required.Field "id" Decode.int64
              Name = get.Required.Field "name" Decode.string
              Price = get.Required.Field "price" Decode.decimal
              Purchased = get.Required.Field "purchased" Decode.bool })

type Recipient =
    { Id: int64
      Name: string
      Amount: decimal
      Items: seq<Item> }

    static member Decoder =    
        Decode.object (fun get ->
            { Id = get.Required.Field "id" Decode.int64
              Name = get.Required.Field "name" Decode.string
              Amount = get.Required.Field "amount" Decode.decimal
              Items =
                 get.Optional.Field "items" (Decode.list Item.Decoder)
                 |> Option.defaultValue [] })

type Budget =
    { Id: int64
      Name: string
      Amount: decimal
      Recipients: seq<Recipient> }
  
    static member Decoder =
        Decode.object (fun get ->
            { Id = get.Required.Field "id" Decode.int64
              Name = get.Required.Field "name" Decode.string
              Amount = get.Required.Field "amount" Decode.decimal
              Recipients =
                  get.Optional.Field "recipients" (Decode.list Recipient.Decoder)
                  |> Option.defaultValue [] })

type BudgetResponse =
    { Data: Budget }

    static member Decoder =
        Decode.object (fun get ->
            { Data = get.Required.Field "data" Budget.Decoder })