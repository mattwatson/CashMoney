#r "System.Xml.Linq" //Don't understand why I need this - not even sure if I do.
#load "Domain.fs"
#load "Persistence.fs"
#load "Kitty.fs"

open System
open System.IO
open System.Linq
open System.Xml.Linq

open Domain
open Kitty
open Persistence

let datapath = @"C:\Users\Matt\Dropbox\Akcounts\Data\"
//let datapath = @"C:\Users\matt__000\Dropbox\Akcounts\Data\"

let accountTags = LoadAccountTags datapath
let accounts = LoadAccounts datapath
let journals = LoadJournals datapath
let kts = kittyTotalStrings accounts journals

let kss = 
    kittySummaryStrings accounts journals 
    |> Seq.iter (fun (ac,contents) -> File.WriteAllLines(datapath + @"kitty\" + ac.Name + ".csv",contents))

