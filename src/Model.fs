module Model

type User = 
  { Id: int64
    Username: string
    Email: string }

type Item =
  { Id: int64
    Name: string
    Price: decimal
    Purchased: bool }

type Recipient =
  { Id: int64
    Name: string
    Amount: decimal
    Items: Item[] }

type Budget =
  { Id: int64
    Name: string
    Amount: decimal
    Recipients: Recipient[] }