#load "Domain.fs"
open System
open CashMoney.Domain

type AccountType = Asset | Liability | Income | Expense | Payable | Receivable | Equity

type Account =
    { Name    : string;
      Enabled : bool;
      Type    : AccountType; }

type Money = 
    | Amount of decimal
    | Fraction of (decimal * int)

type Direction = In | Out

type Transaction = 
    { Direction : Direction;
      Account   : Account;
      Amount    : Money;
      Note      : string;
      Verified  : bool }

type Journal = 
    { Date          : DateTime;
      Description   : string;
      Transactions  : Transaction list}

let bank = { Name = "Bank" ; Enabled = true ; Type = Asset }
let cash = { Name = "Cash" ; Enabled = true ; Type = Asset }
let food = { Name = "Food" ; Enabled = true ; Type = Expense }

let tran1 = { Direction = In ; Account = bank ; Amount = Amount 5.35M ; Note = "" ; Verified = false }
let tran2 = { Direction = Out; Account = cash ; Amount = Fraction (5.34M, 3) ; Note = "" ; Verified = false }

let journal1 = { 
    Date = DateTime.Today ;
    Description = "Bought some food with Cash" 
    Transactions = 
        [{ Direction = Out ; Account = cash ; Amount = Amount 5.99M ; Note = "" ; Verified = false };
        { Direction = In ; Account = food ; Amount = Amount 5.99M ; Note = "Burger" ; Verified = false }]
}