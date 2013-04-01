#load "Domain.fs"
open System
open CashMoney.Domain
open System.Linq
#r "System.Xml.Linq" //Don't understand why I need this - not even sure if I do.
open System.Xml.Linq

//Domain
type AccountType = Asset | Liability | Income | Expense | Payable | Receivable | Equity

type Account =
    { Id      : int;
      Name    : string;
      Enabled : bool;
      Type    : AccountType;
      Tags    : int list }

//XML parsing functions
let attribute (node:XElement, name) = 
    let attr = node.Attribute(XName.Get(name))
    if (attr <> null) then attr.Value else failwith("Could not find attribute called '" + name + "'")    

let intAttribute (node:XElement, name) = 
    let attr = attribute(node, name)
    let (ok, result) = Int32.TryParse (attr)
    if (ok) then result else failwith("Unabled to parse int out of '" + attr + "' for attribute called '" + name + "'")

let boolAttribute (node:XElement, name) = 
    let attr = attribute(node, name)
    let (ok, result) = Boolean.TryParse (attr)
    if (ok) then result else failwith("Unabled to parse boolean out of '" + attr + "' for attribute called '" + name + "'")

let parseAccountType(node) =
    match intAttribute(node, "type") with
    | 1 -> Asset
    | 2 -> Liability
    | 3 -> Income
    | 4 -> Expense
    | 5 -> Payable
    | 6 -> Receivable
    | 7 -> Equity
    | x -> failwith ("Unknown Account Type specified '" + string x + "'")

let parseAccountTags(node:XElement) =
    let elements = node.Element(XName.Get("tags"))
    node.Element(XName.Get("tags")).Elements()
    |> List.ofSeq 
    |> List.map (fun tag -> 
        let (ok, result) = Int32.TryParse(tag.Value)
        if ok then result else failwith ("Unabled to parse int from '" + tag.Value + "' for tag'"))

let parseAccount (account :XElement) =
    let id = intAttribute (account, "id")
    let name = attribute (account, "name")
    let accountType = parseAccountType account
    let isEnabled = boolAttribute (account, "isEnabled")
    let tags = parseAccountTags account
    { Id = id; Name = name; Enabled = isEnabled; Type = accountType; Tags = tags}


let accounts = XDocument.Load(@"C:\Users\Matt\Documents\GitHub\CashMoney\CashMoney\Data\accounts.xml").Root.Elements() 
               |> List.ofSeq 
               |> List.map parseAccount


accounts |> List.sortBy (fun x -> x.Name) |> List.iter (fun a -> printfn "%s (%A) Tags: %A" a.Name a.Type a.Tags)

//type Direction = In | Out
//
//type Money = 
//    | Amount of decimal
//    | Fraction of (decimal * int)

//type Transaction = 
//    { Direction : Direction;
//      Account   : int;
//      Amount    : Money;
//      Note      : string;
//      Verified  : bool }
//
//type Journal = 
//    { Date          : DateTime;
//      Description   : string;
//      Transactions  : Transaction list}

//Maybe when saving down the Journals to files they are saved to the Account of their first transaction.
//Issues: when an account is renamed what happens to it's file? What if the Journal has no transactions?

//Or could just save journals by the year, or by a number per file.

//let convertMoney a = 
//    match a with 
//    | Amount a -> a
//    | Fraction (a,f) ->  (a / decimal f)
//

//let tran1 = { Direction = In ; Account = bank.Id ; Amount = Amount 5.35M ; Note = "" ; Verified = false }
//let tran2 = { Direction = Out; Account = cash.Id ; Amount = Fraction (5.34M, 3) ; Note = "" ; Verified = false }
//
//let journal1 = {
//    Date = DateTime.Today ;
//    Description = "Bought some food with Cash" 
//    Transactions = 
//        [{ Direction = Out ; Account = cash.Id ; Amount = Amount 5.99M ; Note = "" ; Verified = false };
//        { Direction = In ; Account = food.Id ; Amount = Amount 5.99M ; Note = "Burger" ; Verified = false }]
//}

//let balances = 
//    journal1.Transactions
//    |> Seq.groupBy (fun t -> t.Account)
//    |> Seq.map (fun (a, ts) -> a, ts |> Seq.sumBy (fun t -> if t.Direction = In then convertMoney t.Amount else -convertMoney t.Amount))
//
//balances |> Seq.iter (fun (ac, am) -> printfn "%s:\t%M" (accounts |> List.filter (fun f -> f.Id = ac) |> List.head).Name am)
//
