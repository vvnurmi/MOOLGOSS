namespace MOO.Types

open System.Runtime.Serialization

type ID = int
type Location =
    | Planet of ID

[<DataContract>]
type Planet = {
    [<DataMember>]
    mutable id : ID
    [<DataMember>]
    mutable player : string option
    [<DataMember>]
    mutable name : string
    [<DataMember>]
    mutable population : int
    [<DataMember>]
    mutable maxPopulation : int
    [<DataMember>]
    mutable orbit : int
}

[<DataContract>]
type Formation = {
    [<DataMember>]
    mutable id : ID
    [<DataMember>]
    mutable player : string
    [<DataMember>]
    mutable location : Location
    [<DataMember>]
    mutable ships : int
}
