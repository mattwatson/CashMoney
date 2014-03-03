module Kitty

open System
open System.Linq
open CashMoney.Domain

type SpentPaid = 
    { 
        AcName:string
        Spent:decimal
        Paid:decimal
    }

type KittyRow = 
    { 
        Date:DateTime
        Item:string
        SpentPaids:SpentPaid list
    }

let kittyJournals accounts journals = 

    let findAccount accounts name = 
        let accountNameFilter id (account:Account) = account.Name.StartsWith(name)
        let id = accounts |> Map.tryFindKey accountNameFilter 
        let id = match id with | Some x -> x | _ -> failwithf "unable to find account %s" name

        accounts |> Map.find id
    
    let personAcNames = ["Russell"; "Rima"; "Jia"; "Argiro"]
    let personAcs = personAcNames |> List.map (findAccount accounts)

    let isKittyAccount _ ac = ac.Name.Contains("Kitty")
    let kittyAcs = 
        accounts 
        |> Map.filter isKittyAccount 
        |> Seq.map (fun x -> x.Value)
        |> Seq.toList
 
    let mattAcs = 
        let nonMattAccountIds = personAcs @ kittyAcs |> List.map (fun a -> a.Id)
        accounts 
        |> Map.filter (fun _ ac -> not <| nonMattAccountIds.Contains(ac.Id))
        |> Seq.map (fun x -> x.Value)
        |> Seq.toList

    let getJournals ac =
        let hasTransForAccount (j:Journal) (ac:Account) = j.HasTransForAccount ac.Id
        journals |> Seq.where (fun j -> hasTransForAccount j ac)

    let kittyRow journal = 
    
        let CreateSpentPaidFromIds ts (acs:Account list) name = 
            let sumTrans filter = Seq.filter filter >> Seq.sumBy (fun (t:Transaction) -> t.Amount.Value)
            let hasAccount t (ac:Account) = ac.Id = t.Account.Value
            { 
                AcName = name
                Spent = sumTrans (fun t -> acs |> List.exists (hasAccount t) && t.Direction = In) ts
                Paid = sumTrans (fun t -> acs |> List.exists (hasAccount t) && t.Direction = Out) ts 
            }
        
        let accList = ("Total",kittyAcs) :: ("Matt",mattAcs) :: (personAcs |> List.map (fun ac -> ac.Name,[ac]))

        journal.Transactions
        |> fun ts -> 
            { 
                Date = journal.Date
                Item = journal.Description
                SpentPaids = accList |> List.map (fun (name,acs) -> CreateSpentPaidFromIds ts acs name)
            }

    kittyAcs 
    |> Seq.map (fun ac -> ac, getJournals ac) 
    |> Seq.map (fun (ac,js) -> ac, js |> Seq.map kittyRow)
