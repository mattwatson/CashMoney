module App

open System
open System.IO
open System.Linq
open System.Xml.Linq

open Domain
open Persistence
open Kitty

let datapath = @"C:\Users\Matt\Dropbox\Akcounts\Data\"

let accountTags = LoadAccountTags datapath
let accounts = LoadAccounts datapath 
let journals = LoadJournals datapath

let addSp sp1 sp2 = 
    if (sp1.Header = sp2.Header)
    then { Header = (sp1.Header); Spent = (sp1.Spent + sp2.Spent); Paid = (sp1.Paid + sp2.Paid) }
    else failwith ("Cannot merge spentPaid for " + sp1.Header + " with " + sp2.Header)

let mergeSpendPaid acc newSP = 
    let existingSP = acc |> List.tryFind (fun x -> x.Header = newSP.Header)
    match existingSP with 
    | None -> newSP :: acc
    | Some accSp -> acc |> List.map (fun sp -> if sp = accSp then addSp sp newSP else sp)

let mergeKittyRow acc kr = 
    { 
        Date = if acc.Date < kr.Date then acc.Date else kr.Date
        Item = "Total"
        SpentPaids = List.fold mergeSpendPaid acc.SpentPaids kr.SpentPaids
    }

let totalKitty (kjs:KittyRow list) = 
    match kjs with
    | [] -> failwith "Cannot work out the total if there are no rows"
    | kj :: kjs -> kjs |> List.fold mergeKittyRow kj 
    
let fixedHeader = ["Date"; "Item";]
let accountGroups (kj:KittyRow) = kj.SpentPaids |> List.map (fun sp -> sp.Header)
let createHeader kj = String.Join (",", fixedHeader @ accountGroups kj @ accountGroups kj)

let kittyJournals = 
    let kjs = kittyRows accounts journals
    
    let rows = kjs |> Seq.map (fun (ac,kjs) -> ac, totalKitty (Seq.toList kjs))

    let rowStrings = 
        rows
        |> Seq.map (fun (ac,kjs) -> 
            let rowStart = sprintf "%s,%s" ac.Name "Total"
            let startAndPaid = List.fold (fun acc x -> sprintf "%s,%M" acc x.Spent) rowStart kjs.SpentPaids
            List.fold (fun acc x -> sprintf "%s,%M" acc x.Paid) startAndPaid kjs.SpentPaids)
        |> Seq.toList

    let firstKJ = rows |> Seq.nth 0 |> snd
    let header = createHeader firstKJ

    header :: rowStrings



[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    
    kittyJournals
    |> List.iter (fun s -> printfn "%s" s) 

    File.WriteAllLines(datapath + @"kitty\Totals.csv", kittyJournals)
    
    let y = Console.ReadLine()
    0 // return an integer exit code