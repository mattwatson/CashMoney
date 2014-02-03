#r "System.Xml.Linq" //Don't understand why I need this - not even sure if I do.

#load "Domain.fs"
#load "Persistence.fs"

open System
open System.Linq
open System.Xml.Linq

open CashMoney.Domain
open Persistence

//let datapath = @"C:\Users\Matt\Documents\dev\CashMoney\CashMoney\Data\"
let datapath = @"C:\Users\Matt\Dropbox\Akcounts\Data\"

let accountTags =
    XDocument.Load(datapath + @"\accountTags.xml").Root.Elements()
    |> Seq.map parseAccountTag
    |> Seq.map (fun x -> x.Id,x)
    |> Map.ofSeq

let accounts = 
    XDocument.Load(datapath + @"\accounts.xml").Root.Elements() 
    |> Seq.map parseAccount
    |> Seq.map (fun x -> x.Id, x)
    |> Map.ofSeq

let journals = 
    XDocument.Load(datapath + @"\journals.xml").Root.Elements()
    |> Seq.map parseJournal



//TODO make parsing for templates (probably reusing the journal parsing)

//When saving journals in the future, just dump them out in date order, and perhaps have one file per year, or one per month.


let convertMoney a = 
    match a with 
    | Amount a -> a
    | Fraction (a,f) ->  (a / decimal f)

let balances = 
    journals 
    |> Seq.collect(fun x -> x.Transactions)
    |> Seq.groupBy (fun t -> t.Account)
    |> Seq.map (fun (a, ts) -> a, ts |> Seq.sumBy (fun t -> if t.Direction = In then convertMoney t.Amount else -convertMoney t.Amount))

balances 
|> Seq.where (fun (ac,am) -> ac.IsSome) 
|> Seq.map (fun (ac,am) -> accounts.Item(ac.Value), am)
|> Seq.sortBy (fun (ac,am) -> ac.Name)
|> Seq.iter (fun (ac, am) -> printfn "%s,%A,%b,%M" ac.Name ac.Type ac.Enabled am)
