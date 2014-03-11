#r "System.Xml.Linq" //Don't understand why I need this - not even sure if I do.

#load "Domain.fs"
#load "Persistence.fs"
#load "Kitty.fs"

open System
open System.IO
open System.Linq
open System.Xml.Linq

open Domain
open Persistence
open Kitty

let datapath = @"C:\Users\Matt\Dropbox\Akcounts\Data\"
//let datapath = @"C:\Users\matt__000\Dropbox\Akcounts\Data\"

let accountTags = LoadAccountTags datapath
let accounts = LoadAccounts datapath 
let journals = LoadJournals datapath

let kts = kittyTotalStrings accounts journals

let kss = kittySummaryStrings accounts journals
          |> Seq.iter (fun (ac,contents) -> File.WriteAllLines(datapath + @"kitty\" + ac.Name + ".csv", contents))


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



// Broken Journal stuff

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