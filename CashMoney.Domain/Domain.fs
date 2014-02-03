namespace CashMoney.Domain

open System

type AccountType = Asset | Liability | Income | Expense | Payable | Receivable | Equity

type AccountTag = { Id:int; Name:string }

type Account =
    { Id      : int;
      Name    : string;
      Enabled : bool;
      Type    : AccountType;
      Tags    : int list }

type Direction = In | Out

type Money = 
    | Amount of decimal
    | Fraction of (decimal * int)

type Transaction = 
    { Direction : Direction;
      Account   : int option;
      Amount    : Money;
      Note      : string;
      Verified  : bool }

type Journal = 
    { Id            : int;
      Date          : DateTime;
      Description   : string;
      Verified      : bool;
      Transactions  : Transaction list }

type Class1() = 
    member this.X = "F#"
