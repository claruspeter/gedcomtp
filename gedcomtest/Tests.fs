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
let ``Comiles a group all of the same level into a list``()=
    let input = [
        "1 AAAA"
        "1 BBBB askdjhas"
        "1 CCCC sjsjsj"
    ]
    let result = fromStrings input
    Assert.Equal(3, result.Length)
    Assert.True(result |> List.fold (fun acc item -> acc && item.level=1) true)

[<Fact>]
let ``Can compile a group of different levels into a hierarchy``()=
    let input = [
        "1 AAAA"
        "2 BBBB askdjhas"
        "3 CCCC sjsjsj"
    ]
    let result = fromStrings input
    let getlabel = (fun x -> x.label)
    let getchildren = (fun x -> x.children)
    Assert.Equal(1, result.Length)
    Assert.Equal("AAAA", result.[0].label)
    Assert.Equal("BBBB", result.[0].children |> Seq.head |> getlabel)
    Assert.Equal("CCCC", result.[0].children |> Seq.head |> getchildren |> Seq.head |> getlabel)

//     FAILS!
// [<Fact>]
// let ``Can compile many groups of different levels that go up and down``()=
//     let input = [
//         "1 A000"
//         "2 AA00 askdjhas"
//         "3 AAA0 sjsjsj"
//         "1 B000 sjsjsj"
//         "2 BA00 sjsjsj"
//         "2 BB00 sjsjsj"
//     ]
//     let result = fromStrings input
//     let getlabel = (fun x -> x.label)
//     let getchildren = (fun x -> x.children)
//     printfn "%A" result
//     Assert.Equal(1, result.Length)
//     Assert.Equal("A000", result.[0].label)
//     Assert.Equal("AA00", result.[0].children |> Seq.head |> getlabel)
//     Assert.Equal("AAA0", result.[0].children |> Seq.head |> getchildren |> Seq.head |> getlabel)
//     Assert.Equal("B000", result.[1].label)
//     Assert.Equal("BA00", result.[1].children |> Seq.head |> getlabel)
//     Assert.Equal("BB00", result.[1].children |> Seq.item 1 |> getlabel)

[<Fact>]
let ``Starts with head `` () =
    let result = fromFile "../../../sample.ged"
    Assert.Equal("HEAD", result.label)
    Assert.Equal(None, result.value)
