namespace System.Data
open System.IO

module Gedcom =
    type DataNode = {level: int; label: string; value: string option; children: DataNode seq}
                    with static member Zero = {level=0; label="NONE"; value=None; children=[||]}
    let toNode (text:string) : DataNode =
        {
            level = System.Int32.Parse(text.Substring(0, 1))
            label = text.Substring(2, 4)
            value = if text.Length > 7 then Some (text.Substring(7)) else None
            children = seq []
        }

    let appendChild parent child = 
        {parent with children = parent.children |> Seq.append [child]}

    let fromStrings (data:string seq) =
        data
        |> Seq.map toNode 
        |> Seq.fold (fun acc item -> acc @ [item] ) []

    let fromFile filename =
        File.ReadLines filename
        |> Seq.head
        |> toNode
        
