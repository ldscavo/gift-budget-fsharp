module Budget

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