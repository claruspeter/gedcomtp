namespace System.Data
open System.IO

module Gedcom =
    type DataNode = {level: int; label: string; value: string option}

    let toNode (text:string) : DataNode =
        {
            level = System.Int32.Parse(text.Substring(0, 1))
            label = text.Substring(2, 4)
            value = if text.Length > 7 then Some (text.Substring(7)) else None
        }

    let fromFile filename =
        File.ReadLines filename
        |> Seq.head
        |> toNode
        
