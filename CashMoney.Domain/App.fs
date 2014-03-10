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

[<EntryPoint>]
let main argv = 
    
    File.WriteAllLines(datapath + @"kitty\Totals.csv", (kittyTotals accounts journals))
    
    0 // return an integer exit code