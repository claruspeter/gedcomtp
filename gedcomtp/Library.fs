namespace System.Data
open System.IO

module Gedcom =
    type DataNode = {level: int; label: string; value: string option; children: DataNode seq}
                    with 
                        static member Zero = {level=0; label="NONE"; value=None; children=[||]}
                        override this.ToString() = 
                            let prefix = String.init this.level (fun _ -> " ")
                            let me = sprintf "%s%d %s %s" prefix this.level this.label (if this.value.IsSome then this.value.Value else "-")
                            this.children |> Seq.fold (fun acc item -> acc + "\n" + item.ToString()) me


    let toNode (text:string) : DataNode =
        let parts = text.Split([|' '|], 3)
        try
            {
                level = System.Int32.Parse(parts.[0])
                label = parts.[1]
                value = if parts.Length > 2 then Some (parts.[2]) else None
                children = seq []
            }
        with 
        | exc ->
            failwith ("Unable to parse : " + text + ". Error: " + exc.Message)

    let appendChild parent child = 
        {parent with children = parent.children |> Seq.append [child]}

    let (|Seq|_|) test input =
        if Seq.compareWith Operators.compare input test = 0
            then Some input
            else None

    type Walker = {data: DataNode list; toInsert: DataNode list}

    let rec private walkTree (data: string list) =
        match data with 
        | [] -> failwith "Empty list"
        | [s] -> {data=[toNode s]; toInsert=[]}
        | x::xs ->
            let next = walkTree xs
            let node = toNode x
            let diff = next.data.[0].level - node.level
            let data = 
                match diff with
                | 0 -> [node] @ next.data
                | 1 -> [{node with children = next.data}]
                | n when n < 0 -> [node]
                | _ -> failwith "Skipped a level"
            let insertionAtThisLevel = next.toInsert.Length > 0 && next.toInsert.[0].level = node.level
            let dataWithInsertion = if insertionAtThisLevel then data @ next.toInsert else data
            let toInsert = 
                match insertionAtThisLevel, diff with
                | _, n when n < 0 -> next.data @ next.toInsert
                | true, _ -> []
                | false, _ -> next.toInsert

            {data=dataWithInsertion; toInsert=toInsert}

            
    let fromStrings data = (walkTree data).data

    let fromFile filename =
        let result = 
            File.ReadAllLines filename
            |> Seq.toList
            |> walkTree
        result.data
        
        
