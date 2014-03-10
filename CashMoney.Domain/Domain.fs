module Domain

type AccountType = Asset | Liability | Income | Expense | Payable | Receivable | Equity

type AccountTag = { Id:int; Name:string }

type Account =
    { 
        Id      : int
        Name    : string
        Enabled : bool
        Type    : AccountType
        Tags    : int list 
    }

type Direction = In | Out

type Money = 
    | Amount of decimal
    | Fraction of (decimal * int)
    
    member this.Value = 
        match this with 
        | Amount a -> a 
        | Fraction (a,f) ->  (a / decimal f)

type Transaction = 
    { 
        Direction : Direction
        Account   : int option
        Amount    : Money
        Note      : string
        Verified  : bool 
    }
    member this.accountIs a = this.Account.IsSome && this.Account.Value = a.Id

type Journal = 
    { 
        Id            : int
        Date          : System.DateTime
        Description   : string
        Verified      : bool
        Transactions  : Transaction list 
    }
    
//Utility functions
let getAccountJournals js (a:Account) = 
    let involvesAccount a j = j.Transactions |> Seq.exists (fun t -> t.accountIs a)
    js |> Seq.where (involvesAccount a)

let findAccounts accounts filter =
    accounts |> Map.filter filter |> Seq.map (fun x -> x.Value) |> Seq.toList