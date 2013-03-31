namespace MOO.Types

open System.Runtime.Serialization

type ID = int

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
