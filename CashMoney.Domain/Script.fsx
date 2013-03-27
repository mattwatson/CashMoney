#load "Domain.fs"
open System
open CashMoney.Domain

type Direction = In | Out

type Money = 
    | Amount of decimal
    | Fraction of (decimal * int)

type AccountType = Asset | Liability | Income | Expense | Payable | Receivable | Equity

type Account =
    { Id      : int;    //Id will never change
      Name    : string; //want to support renaming, so either it's mutable or the collection of accounts will allow replacements
      Enabled : bool;
      Type    : AccountType; }

type Transaction = 
    { Direction : Direction;
      Account   : int;
      Amount    : Money;
      Note      : string;
      Verified  : bool }

type Journal = 
    { Date          : DateTime;
      Description   : string;
      Transactions  : Transaction list}

//Maybe when saving down the Journals to files they are saved to the Account of their first transaction.
//Issues: when an account is renamed what happens to it's file? What if the Journal has no transactions?

//Or could just save journals by the year, or by a number per file.

let convertMoney a = 
    match a with 
    | Amount a -> a
    | Fraction (a,f) ->  (a / decimal f)

let bank = { Id = 1; Name = "Bank" ; Enabled = true ; Type = Asset }
let cash = { Id = 2; Name = "Cash" ; Enabled = true ; Type = Asset }
let food = { Id = 3; Name = "Food" ; Enabled = true ; Type = Expense }

let accounts = [ bank; cash; food ]

let tran1 = { Direction = In ; Account = bank.Id ; Amount = Amount 5.35M ; Note = "" ; Verified = false }
let tran2 = { Direction = Out; Account = cash.Id ; Amount = Fraction (5.34M, 3) ; Note = "" ; Verified = false }

let journal1 = {
    Date = DateTime.Today ;
    Description = "Bought some food with Cash" 
    Transactions = 
        [{ Direction = Out ; Account = cash.Id ; Amount = Amount 5.99M ; Note = "" ; Verified = false };
        { Direction = In ; Account = food.Id ; Amount = Amount 5.99M ; Note = "Burger" ; Verified = false }]
}

let balances = 
    journal1.Transactions
    |> Seq.groupBy (fun t -> t.Account)
    |> Seq.map (fun (a, ts) -> a, ts |> Seq.sumBy (fun t -> if t.Direction = In then convertMoney t.Amount else -convertMoney t.Amount))

balances |> Seq.iter (fun (ac, am) -> printfn "%s:\t%M" (accounts |> List.filter (fun f -> f.Id = ac) |> List.head).Name am)
