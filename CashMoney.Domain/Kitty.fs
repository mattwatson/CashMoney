module Kitty

open System
open System.Linq
open Domain

type SpentPaid = { Header:string; Spent:decimal; Paid:decimal }

type KittyRow = { Date:DateTime; Item:string; SpentPaids:SpentPaid list }
                member this.toString showDate item = 
                    let date = if showDate then this.Date.ToString("dd/MM/yyyy") else ""
                    let spents = this.SpentPaids |> List.map (fun x -> x.Spent)
                    let paids = this.SpentPaids |> List.map (fun x -> x.Paid)
                    let values = spents @ paids |> List.toArray |> Array.map (fun x -> Math.Round(x, 5))
                    sprintf "%s,%s,%s" date item (String.Join (",",values))

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

let kittyTotal item (krs:seq<KittyRow>) = 
    let totalSpent sps = Seq.sumBy(fun sp -> sp.Spent) sps
    let totalPaid sps = Seq.sumBy(fun sp -> sp.Paid) sps

    let totalSpentPaidsByHeader sps = 
        Seq.groupBy (fun sp -> sp.Header) sps
        |> Seq.map (fun (header,sps) -> { Header = header; Spent = totalSpent sps; Paid = totalPaid sps })
        |> Seq.toList

    let minDate = krs |> Seq.map (fun kr -> kr.Date) |> Seq.min
    let spentPaids = totalSpentPaidsByHeader (krs |> Seq.collect (fun kr -> kr.SpentPaids))
    {
        Date = minDate; Item = item; SpentPaids = spentPaids
    }

let kittySummaries accounts journals = 

    let kittySummary item krs = 
        { 
            Rows = krs |> Seq.sortBy (fun kr -> kr.Date) |> Seq.toList
            Total = kittyTotal item krs
        }

    kittyRows accounts journals 
    |> Seq.map (fun (ac,krs) -> ac,kittySummary "Total" krs)



let kittyTotalStrings accounts journals = 

    let people = ((accGroups accounts) |> List.map (fun (ac,_) -> ac))
    let headerStrings = "Date" :: "Item" :: (people @ people) |> Seq.toArray
    let header = String.Join(",", headerStrings)

    let totalRowStrings = kittyRows accounts journals
                          |> List.map (fun (ac,krs) -> kittyTotal ac.Name krs)
                          |> List.map (fun kt -> kt.toString true kt.Item)

    header :: totalRowStrings

let kittySummaryStrings accounts journals =

    let kittySummaryString ac ks =
        let people = ((accGroups accounts) |> List.map (fun (ac,_) -> ac))
        let headerStrings = "Date" :: "Item" :: (people @ people) |> Seq.toArray
        let header = String.Join(",", headerStrings)
        let footer = ks.Total.toString false "Total"
        let rows = ks.Rows |> Seq.sortBy (fun kr -> kr.Date) |> Seq.map (fun kr -> kr.toString true kr.Item) |> Seq.toList
        (header :: rows) @ [footer]
    
    kittySummaries accounts journals
    |> Seq.map (fun (ac,ks) -> ac,kittySummaryString ac ks)
