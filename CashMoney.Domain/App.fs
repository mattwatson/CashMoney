module App

open System
open System.IO
open System.Linq
open System.Xml.Linq

open CashMoney.Domain
open Persistence
open Kitty

let datapath = @"C:\Users\Matt\Dropbox\Akcounts\Data\"

let accountTags = LoadAccountTags datapath
let accounts = LoadAccounts datapath 
let journals = LoadJournals datapath

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    
    let kitties = kittyJournals accounts journals
    0 // return an integer exit code