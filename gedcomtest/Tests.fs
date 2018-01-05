module Tests

open System
open Xunit
open System.Data.Gedcom

let getlabel x = x.label
let getchildren x = x.children
let print result =
    result |> List.iter (fun x -> printfn "%s" (x.ToString()))


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
    Assert.Equal(1, result.Length)
    Assert.Equal("AAAA", result.[0].label)
    Assert.Equal("BBBB", result.[0].children |> Seq.head |> getlabel)
    Assert.Equal("CCCC", result.[0].children |> Seq.head |> getchildren |> Seq.head |> getlabel)

[<Fact>]
let ``Can compile many groups of different levels that go up and down``()=
    let input = [
        "1 A000"
        "2 AA00 askdjhas"
        "1 B000 sjsjsj"
        "2 BA00 adsasdadasd"
    ]
    let result = fromStrings input
    Assert.Equal(2, result.Length)
    Assert.Equal("A000", result.[0].label)
    Assert.Equal("AA00", result.[0].children |> Seq.head |> getlabel)
    Assert.Equal("B000", result.[1].label)
    Assert.Equal("BA00", result.[1].children |> Seq.head |> getlabel)


[<Fact>]
let ``Starts with head `` () =
    let result = fromFile "../../../sample.ged"
    print result
    Assert.Equal("HEAD", result.[0].label)
    Assert.Equal(None, result.[0].value)
