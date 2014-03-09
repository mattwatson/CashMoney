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

let fixedHeader = ["Date"; "Item";]
let accountGroups (kj:KittyRow) = kj.SpentPaids |> List.map (fun sp -> sp.Header)
let header rows = 
    let firstRow = rows |> Seq.nth 0 |> snd
    String.Join (",", fixedHeader @ accountGroups firstRow @ accountGroups firstRow)

let kittyTotals = 
    let rows = 
        kittyRows accounts journals
        |> Seq.map (fun (ac,krs) -> ac, kittyTotal krs)
    
    //want to pipe rows straight in
    let rowStrings = 
        rows
        |> Seq.map (fun (ac,kjs) -> 
            let rowStart = sprintf "%s,%s" ac.Name "Total"
            let startAndPaid = List.fold (fun acc x -> sprintf "%s,%M" acc x.Spent) rowStart kjs.SpentPaids
            List.fold (fun acc x -> sprintf "%s,%M" acc x.Paid) startAndPaid kjs.SpentPaids)
        |> Seq.toList

    let header = header rows

    header :: rowStrings


[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    
    kittyTotals
    |> List.iter (fun s -> printfn "%s" s) 

    File.WriteAllLines(datapath + @"kitty\Totals.csv", kittyTotals)
    
    let y = Console.ReadLine()
    0 // return an integer exit code