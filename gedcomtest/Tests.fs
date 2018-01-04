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
let ``Starts with head `` () =
    let result = fromFile "../../../sample.ged"
    Assert.Equal("HEAD", result.label)
    Assert.Equal(None, result.value)
