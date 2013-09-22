namespace CashMoney.Domain

type AccountType = Asset | Liability | Income | Expense | Payable | Receivable | Equity

type AccountTag = { Id:int; Name:string }

type Account =
    { Id      : int;
      Name    : string;
      Enabled : bool;
      Type    : AccountType;
      Tags    : AccountTag list }

type Class1() = 
    member this.X = "F#"
