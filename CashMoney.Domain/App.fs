module App

open System
open System.IO
open System.Linq
open System.Xml.Linq

open Domain
open Kitty
open Persistence

let datapath = @"C:\Users\Matt\Dropbox\Akcounts\Data\"

let accountTags = LoadAccountTags datapath
let accounts = LoadAccounts datapath
let journals = LoadJournals datapath

[<EntryPoint>]
let main argv = 
    
    File.WriteAllLines(datapath + @"kitty\Totals.csv",(kittyTotalStrings accounts journals))

    kittySummaryStrings accounts journals 
    |> Seq.iter (fun (ac,contents) -> File.WriteAllLines(datapath + @"kitty\" + ac.Name + ".csv",contents))
    
    0 // return an integer exit code
