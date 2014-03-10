module Kitty

open System
open System.Linq
open Domain

type SpentPaid = { Header:string; Spent:decimal; Paid:decimal }

type KittyRow = { Date:DateTime; Item:string; SpentPaids:SpentPaid list }

type KittyAccountSummary = { Rows: KittyRow list; Total: KittyRow }

let accGroups accounts = 

    let personAccs = ["Russell"; "Rima"; "Jia"; "Argiro"] |> List.collect (fun x -> findAccounts accounts (fun _ ac -> ac.Name.StartsWith(x)))
    let kittyAccs = findAccounts accounts (fun _ ac -> ac.Name.Contains("Kitty"))
    let nonMattAccs = personAccs @ kittyAccs
    let mattAccs = findAccounts accounts (fun _ ac -> nonMattAccs |> List.forall (fun x -> x <> ac))

    let totalGroup = "Total",kittyAccs
    let mattGroup = "Matt",mattAccs
    let otherGroups = List.map (fun ac -> ac.Name,[ac]) personAccs
    totalGroup :: mattGroup :: otherGroups

let kittyRows (accounts:Map<int,Account>) journals = 

    let CreateSpentPaid ts (header,accs) = 
        let isRelevantTran direction t = t.Direction = direction && List.exists t.accountIs accs 
        let sumTrans direction = Seq.filter (isRelevantTran direction) >> Seq.sumBy (fun t -> t.Amount.Value)
        { 
            Header = header
            Spent = sumTrans In ts
            Paid = sumTrans Out ts 
        }

    let createKittyRow (journal:Journal) = 
        {
            Date = journal.Date
            Item = journal.Description
            SpentPaids = accGroups accounts |> List.map (fun accGroup -> CreateSpentPaid journal.Transactions accGroup)
        }

    let kittyAccs = findAccounts accounts (fun _ ac -> ac.Name.Contains("Kitty"))
    
    kittyAccs
    |> Seq.map (fun ac -> ac, getAccountJournals journals ac) 
    |> Seq.map (fun (ac,js) -> ac, js |> Seq.map createKittyRow)
    |> Seq.toList

let kittyTotal (krs:seq<KittyRow>) = 
    let totalSpent sps = Seq.sumBy(fun sp -> sp.Spent) sps
    let totalPaid sps = Seq.sumBy(fun sp -> sp.Paid) sps

    let totalSpentPaidsByHeader sps = 
        Seq.groupBy (fun sp -> sp.Header) sps
        |> Seq.map (fun (header,sps) -> { Header = header; Spent = totalSpent sps; Paid = totalPaid sps })
        |> Seq.toList

    let minDate = krs |> Seq.map (fun kr -> kr.Date) |> Seq.min
    let spentPaids = totalSpentPaidsByHeader (krs |> Seq.collect (fun kr -> kr.SpentPaids))
    {
        Date = minDate; Item = "Total"; SpentPaids = spentPaids
    }


let kittyTotals accounts journals = 

    let headerStrings = "Date" :: "Item" :: ((accGroups accounts) |> List.map (fun (ac,_) -> ac)) |> Seq.toArray
    let header = String.Join(",", headerStrings)

    let buildOutputString (ac,sps) = 
        let spents = sps |> List.map (fun x -> x.Spent)
        let paids = sps |> List.map (fun x -> x.Paid)
        let values = spents @ paids |> List.toArray |> Array.map (fun x -> Math.Round(x, 5))
        ac.Name + ",Total," + String.Join (",",values)

    let allRows = kittyRows accounts journals
    let totalRows = allRows |> List.map (fun (ac,krs) -> ac, kittyTotal krs)
    let totalRowStrings = totalRows |> List.map (fun (ac,kjs) -> buildOutputString (ac,kjs.SpentPaids))

    header :: totalRowStrings
