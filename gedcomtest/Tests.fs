module Tests

open System
open Xunit
open System.Data.Gedcom

[<Fact>]
let ``Can parse level label and value`` () =
    let result = toNode "0 DATA sdjfhgasjfdg"
    Assert.Equal(0, result.level)
    Assert.Equal("DATA", result.label)
    Assert.Equal(Some "sdjfhgasjfdg", result.value)

[<Fact>]
let ``Can parse line without value`` () =
    let result = toNode "0 ABCD"
    Assert.Equal(0, result.level)
    Assert.Equal("ABCD", result.label)
    Assert.Equal(None, result.value)

[<Fact>]
let ``Append node to parent``() =
    let parent = {DataNode.Zero with level=1}
    let child = {DataNode.Zero with level=2}
    let updated = appendChild parent child
    Assert.Equal(updated.children |> Seq.head, child)

[<Fact>]
let ``Can compile a group all of the same level into a list``()=
    let input = [
        "1 AAAA"
        "1 BBBB askdjhas"
        "1 CCCC sjsjsj"
    ]
    let result = fromStrings input
    Assert.Equal(3, input.Length)
    Assert.True(result |> List.fold (fun acc item -> acc && item.level=1) true)

[<Fact>]
let ``Starts with head `` () =
    let result = fromFile "../../../sample.ged"
    Assert.Equal("HEAD", result.label)
    Assert.Equal(None, result.value)
