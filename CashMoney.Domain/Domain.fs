namespace CashMoney.Domain

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

type Journal = 
    { 
        Id            : int
        Date          : System.DateTime
        Description   : string
        Verified      : bool
        Transactions  : Transaction list 
    }

    member this.HasTransForAccount ac =
        this.Transactions |> Seq.exists (fun t -> t.Account.IsSome && t.Account.Value = ac)
