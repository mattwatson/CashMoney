#load "Domain.fs"
open System
open CashMoney.Domain

type AccountType = Asset | Liability | Income | Expense | Payable | Receivable | Equity

type Account =
    { Name    : string;
      Enabled : bool;
      Type    : AccountType; }

type Direction = In | Out
    
type Money = 
    | Amount of Decimal
    | Fraction of (Decimal * int)

type Transaction = 
    { Direction : Direction;
      Account   : Account;
      Amount    : Money;
      Note      : string;
      Verified  : bool }

type Journal = 
    { Date          : DateTime;
      Description   : string; }

let account1 = { Name = "Bank" ; Enabled = true ; Type = Asset } 

let tran1 = { Direction = In ; Account = account1 ; Amount = Amount 5.35M ; Note = "" ; Verified = false }
let tran2 = { Direction = Out; Account = account1 ; Amount = Fraction (5.34M, 3) ; Note = "" ; Verified = false }

let journal1 = { Date = DateTime.Today ; Description = "" }