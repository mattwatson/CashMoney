module Persistence

open System
open System.Linq
open System.Xml.Linq

open CashMoney.Domain

//When saving journals in the future, just dump them out in date order, and perhaps have one file per year, or one per month.

//XML parsing functions
let attribute (node:XElement, name) = 
    let attr = node.Attribute(XName.Get(name))
    if (attr <> null) then attr.Value else failwith("Could not find attribute called '" + name + "'")    

let intAttribute (node:XElement, name) = 
    let attr = attribute(node, name)
    let ok, result = Int32.TryParse (attr)
    if ok then result else failwith("Unabled to parse int out of '" + attr + "' for attribute called '" + name + "'")

let boolAttribute (node:XElement, name) = 
    let attr = attribute(node, name)
    let ok, result = Boolean.TryParse (attr)
    if ok then result else failwith("Unabled to parse boolean out of '" + attr + "' for attribute called '" + name + "'")

let dateTimeAttribute (node:XElement, name) = 
    let attr = attribute(node, name)
    let ok, result = DateTime.TryParse (attr)
    if ok then result else failwith("Unabled to parse datetime out of '" + attr + "' for attribute called '" + name + "'")

let decimalAttribute (node:XElement, name) = 
    let attr = attribute(node, name)
    let ok, result = Decimal.TryParse (attr)
    if ok then result else failwith("Unabled to parse decimal out of '" + attr + "' for attribute called '" + name + "'")

let parseAccountTag (account :XElement):AccountTag = 
    let id = intAttribute (account, "id")
    let name = account.Value
    { Id = id; Name = name}
    
let parseAccount (account:XElement) =

    let parseAccountType(node) =
        match intAttribute(node, "type") with
        | 1 -> Asset;
        | 2 -> Liability;
        | 3 -> Income;
        | 4 -> Expense;
        | 5 -> Payable;
        | 6 -> Receivable;
        | 7 -> Equity;
        | x -> failwith ("Unknown Account Type specified '" + string x + "'")

    let parseAccountTags(node:XElement) =
        let parseTagId tag = 
            let (ok, result) = Int32.TryParse(tag)
            if ok then result else failwith ("Unabled to parse int from '" + tag + "' for tag'")
        
        node.Element(XName.Get("tags")).Elements()
        |> Seq.map (fun tag -> parseTagId tag.Value)
   
    let id = intAttribute (account, "id")
    let name = attribute (account, "name")
    let accountType = parseAccountType account
    let isEnabled = boolAttribute (account, "isEnabled")
    let tags = parseAccountTags account
    { Id = id; Name = name; Enabled = isEnabled; Type = accountType; Tags = Seq.toList tags}

let parseJournal (journal:XElement) = 
    
    let parseTransaction(node) =
        let direction = 
            match intAttribute(node,"direction") with
            | 1 -> In
            | 2 -> Out
            | x -> failwith ("Unknown Direction specified '" + string x + "'")

        let accountId = 
            let hasAccount = node.Attributes() |> Seq.exists (fun x -> x.Name.LocalName = "account")
            if hasAccount then 
                let attr = attribute(node, "account")
                let ok, result = Int32.TryParse (attr)
                if ok then Some result else None
            else None
        let amount = 
            let amountDecimal = decimalAttribute(node, "amount")
            Amount amountDecimal

        let note = attribute(node,"note")
        let verified = boolAttribute(node,"isVerified")
        {Direction = direction; Account = accountId; Amount = amount; Note = note; Verified = verified}

    let parseTransactions(node:XElement) =    
        node.Element(XName.Get("transactions")).Elements()
        |> Seq.map (fun transaction -> parseTransaction transaction)
   
    let id = intAttribute (journal, "id")
    let date = dateTimeAttribute (journal, "date")
    let description = attribute (journal, "description")
    let verified = boolAttribute (journal, "isVerified")
    let transactions = parseTransactions journal
    { Id = id; Date = date; Description = description; Verified = verified; Transactions = Seq.toList transactions}

let LoadAccountTags (path:string) =
    XDocument.Load(path + @"\accountTags.xml").Root.Elements()
    |> Seq.map parseAccountTag
    |> Seq.map (fun x -> x.Id,x)
    |> Map.ofSeq

let LoadAccounts (path:string) = 
    XDocument.Load(path + @"\accounts.xml").Root.Elements() 
    |> Seq.map parseAccount
    |> Seq.map (fun x -> x.Id, x)
    |> Map.ofSeq

let LoadJournals (path:string) = 
    XDocument.Load(path + @"\journals.xml" ).Root.Elements() 
    |> Seq.map parseJournal
