#r "System.Xml.Linq" //Don't understand why I need this - not even sure if I do.

#load "Domain.fs"
#load "Persistence.fs"

open System
open System.IO
open System.Linq
open System.Xml.Linq

open CashMoney.Domain
open Persistence

//let datapath = @"C:\Users\Matt\Documents\dev\CashMoney\CashMoney\Data\"
let datapath = @"C:\Users\Matt\Dropbox\Akcounts\Data\"

let accountTags = LoadAccountTags <| datapath + @"\accountTags.xml"
let accounts = LoadAccounts <| datapath + @"\accounts.xml"
let journals = LoadJournals <| datapath + @"\journals.xml"
//let templates = LoadTemplates <| datapath + @"\templates.xml"

let getAccountId name = (accounts |> Seq.find (fun a -> a.Value.Name.StartsWith(name))).Value.Id
let isKittyAccount ac = ac.Name.Contains("Kitty")

let russAccount = getAccountId "Russell"
let rimaAccount = getAccountId "Rima"
let jiaAccount = getAccountId "Jia"
let argiroAccount = getAccountId "Argiro"

let kittyAccounts = accounts |> Seq.map (fun ac -> ac.Value) |> Seq.where isKittyAccount |> Seq.map (fun ac -> ac.Id) |> Seq.toList
let mattAccounts = 
    let nonMattAccounts = russAccount :: rimaAccount :: jiaAccount :: argiroAccount :: kittyAccounts
    accounts |> Seq.map (fun x-> x.Key) |> Seq.where (fun a -> nonMattAccounts.Contains(a) = false) |> Seq.toList

let convertMoney a = match a with | Amount a -> a | Fraction (a,f) ->  (a / decimal f)

type SpentPaid = { Spent:decimal; Paid:decimal; }
type KittyRow = { Date:DateTime; Item:string; Total:SpentPaid; Matt:SpentPaid; Russ:SpentPaid; Jia:SpentPaid; Rima:SpentPaid; Argiro:SpentPaid; }

let kittyRow journal = 
    let sumTrans filter ts = ts |> Seq.filter filter |> Seq.sumBy (fun t -> if t.Direction = In then convertMoney t.Amount else -convertMoney t.Amount)
    let CreateSpentPaidFromId id ts = 
        { Spent = sumTrans (fun x -> x.Account.Value = id && x.Direction = In) ts; 
          Paid = 0M - sumTrans (fun x -> x.Account.Value = id && x.Direction = Out) ts }
    let CreateSpentPaidFromIds (ids:int list) ts = 
        { Spent = sumTrans (fun x -> ids.Contains(x.Account.Value) && x.Direction = In) ts; 
          Paid = 0M - sumTrans (fun x -> ids.Contains(x.Account.Value) && x.Direction = Out) ts }
        
    journal.Transactions
    |> fun ts -> 
        { Date = journal.Date
          Item = journal.Description
          Total = CreateSpentPaidFromIds kittyAccounts ts
          Matt = CreateSpentPaidFromIds mattAccounts ts
          Russ = CreateSpentPaidFromId russAccount ts
          Jia = CreateSpentPaidFromId jiaAccount ts
          Rima = CreateSpentPaidFromId rimaAccount ts
          Argiro = CreateSpentPaidFromId argiroAccount ts
        }

let kittyJournals = 
    let hasKittyTransactions ac j = j.Transactions.Any (fun t -> t.Account.IsSome && t.Account.Value = ac)
    kittyAccounts 
    |> Seq.map (fun id -> accounts.Item(id), journals |> Seq.where (hasKittyTransactions id)) 
    |> Seq.map (fun (ac,js) -> ac, js |> Seq.map kittyRow)

kittyJournals 
|> Seq.map (fun (ac,kjs) -> ac, kjs |> Seq.sortBy (fun kj -> kj.Date) |> Seq.map (fun kj -> sprintf "%s,%s,%M,%M,%M,%M,%M,%M,%M,%M,%M,%M,%M" (kj.Date.ToShortDateString()) (kj.Item.Replace(',','.')) kj.Total.Spent kj.Matt.Spent kj.Russ.Spent kj.Jia.Spent kj.Rima.Spent kj.Argiro.Spent kj.Matt.Paid kj.Russ.Paid kj.Jia.Paid kj.Rima.Paid kj.Argiro.Paid) |> Seq.toList)
|> Seq.iter (fun (ac,contents) -> File.WriteAllLines(datapath + @"kitty\" + ac.Name + ".csv", List.toArray ("Date,Item,Total,Matt,Russ,Jia,Rima,Argiro,Matt,Russ,Jia,Rima,Argiro" :: contents)))

//
//let kittySheets = 
//    let hasKittyTransactions j = j.Transactions.Any (fun t -> t.Account.IsSome && kittyAccounts.Contains(t.Account.Value))
//    journals
//    |> Seq.where hasKittyTransactions
//    |> Seq.map (fun j -> j, kittyRow j)
//
//let kittyTransactions = 
//    let hasKittyTransactions j = j.Transactions.Any (fun t -> t.Account.IsSome && kittyAccounts.Contains(t.Account.Value))
//    journals 
//    |> Seq.where hasKittyTransactions
//    |> Seq.where (fun j -> j.Transactions.All (fun t -> t.Account.IsSome))
//    |> Seq.groupBy (fun j -> j.Transactions |> Seq.where (fun t -> kittyAccounts.Contains(t.Account.Value)) |> Seq.map (fun t -> t.Account.Value) |> Seq.distinct |> Seq.exactlyOne)
//    |> Seq.map (fun (ac,js) -> ac, js |> Seq.collect(fun j -> j.Transactions))
//    |> Seq.map (fun (acId, ts) -> accounts.Item(acId), ts)
//
//
//type KittyBalance = { Name:string; Total:decimal; MattSpent:decimal; RussSpent:decimal; JiaSpent:decimal; RimaSpent:decimal; ArgiroSpent:decimal; MattPaid:decimal; RussPaid:decimal; JiaPaid:decimal; RimaPaid:decimal; ArgiroPaid:decimal;}
//
//let kittyBalances =
//    kittyTransactions
//    |> Seq.map (fun (ac, ts) -> 
//        { Name=ac.Name
//          Total = sumTrans (fun x -> x.Account.Value = ac.Id && x.Direction = In) ts
//          MattSpent = sumTrans (fun x -> otherAccounts.Contains(x.Account.Value) && x.Direction = In) ts
//          RussSpent = sumTrans (fun x -> x.Account.Value = russAccount && x.Direction = In) ts
//          JiaSpent = sumTrans (fun x -> x.Account.Value = jiaAccount && x.Direction = In) ts
//          RimaSpent = sumTrans (fun x -> x.Account.Value = rimaAccount && x.Direction = In) ts
//          ArgiroSpent = sumTrans (fun x -> x.Account.Value = argiroAccount && x.Direction = In) ts
//          MattPaid = 0M - sumTrans (fun x -> otherAccounts.Contains(x.Account.Value) && x.Direction = Out) ts
//          RussPaid = 0M - sumTrans (fun x -> x.Account.Value = russAccount && x.Direction = Out) ts
//          JiaPaid = 0M - sumTrans (fun x -> x.Account.Value = jiaAccount && x.Direction = Out) ts
//          RimaPaid = 0M - sumTrans (fun x -> x.Account.Value = rimaAccount && x.Direction = Out) ts
//          ArgiroPaid = 0M - sumTrans (fun x -> x.Account.Value = argiroAccount && x.Direction = Out) ts
//        })
//    
//kittyBalances 
//|> Seq.map (fun kb -> sprintf "%s,%M,%M,%M,%M,%M,%M,%M,%M,%M,%M,%M" kb.Name kb.Total kb.MattSpent kb.RussSpent kb.JiaSpent kb.RimaSpent kb.ArgiroSpent kb.MattPaid kb.RussPaid kb.JiaPaid kb.RimaPaid kb.ArgiroPaid)
//|> Seq.toList
//|> fun contents -> File.WriteAllLines(datapath + "kittySum.csv", List.toArray ("Name,T,Matt,Russ,Jia,Rima,Argiro,Matt,Russ,Jia,Rima,Argiro" :: contents))
//
////Next idea - generate kittyBalance per row, and only total them later.
//
//let balances = 
//    transactions
//    |> Seq.groupBy (fun t -> t.Account)
//    |> Seq.where (fun (ac,_) -> ac.IsSome) 
//    |> Seq.map (fun (ac, ts) -> ac, sumTrans (fun x -> true) ts)
//    |> Seq.map (fun (ac, am) -> accounts.Item(ac.Value), am)
//    
//
////let brokenJournals = 
////    let hasKittyTransactions j = j.Transactions.Any (fun t -> t.Account.IsSome && kittyAccounts.Contains(t.Account.Value))
////    journals 
////    |> Seq.where hasKittyTransactions
////    |> Seq.where (fun j -> j.Transactions.All (fun t -> t.Account.IsSome))
////    |> Seq.where (fun j -> j.Transactions |> Seq.map (fun t -> t.Account.Value) |> Seq.where (fun a -> kittyAccounts.Contains(a)) |> Seq.distinct |> Seq.length > 1)
////                              
////brokenJournals |> Seq.length
//
//balances 
//|> Seq.sortBy (fun (ac,am) -> ac.Name)
//|> Seq.iter (fun (ac, am) -> printfn "%s,%A,%b,%M" ac.Name ac.Type ac.Enabled am)
//
