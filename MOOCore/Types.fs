namespace MOO.Types

open System.Runtime.Serialization

type ID = int
type Location =
    | Planet of ID
type Command =
    | MoveFormation of ID * Location

module CommandC =
    [<DataContract>]
    [<KnownType(typeof<MoveFormation>)>]
    type Base() = class end
    and MoveFormation(id, location) =
        inherit Base()
        [<DataMember>] member val ID = id
        [<DataMember>] member val Location = location
    let fromCommand = function
        | MoveFormation(id, location) -> MoveFormation(id, location) :> Base
    let toCommand (c : Base) =
        match c with
        | :? MoveFormation as c -> Command.MoveFormation(c.ID, c.Location)
        | _ -> failwith "Unexpected command"

[<DataContract>]
type Planet =
    {
    id : ID
    player : string option
    name : string
    population : int
    maxPopulation : int
    orbit : int
    }
    [<DataMember>]
    member x.ID with get() = x.id and set (_ : ID) = ()
    [<DataMember>]
    member x.Player
        with get() =
            match x.player with
            | None -> null
            | Some s -> s
        and set (_ : string) = ()
    [<DataMember>]
    member x.Name with get() = x.name and set (_ : string) = ()
    [<DataMember>]
    member x.Population with get() = x.population and set (_ : int) = ()
    [<DataMember>]
    member x.MaxPopulation with get() = x.maxPopulation and set (_ : int) = ()
    [<DataMember>]
    member x.Orbit with get() = x.orbit and set (_ : int) = ()

[<DataContract>]
type Formation =
    {
    id : ID
    player : string
    location : Location
    ships : int
    }
    [<DataMember>]
    member x.ID with get() = x.id and set (_ : ID) = ()
    [<DataMember>]
    member x.Player with get() = x.player and set (_ : string) = ()
    [<DataMember>]
    member x.Location with get() = x.location and set (_ : Location) = ()
    [<DataMember>]
    member x.Ships with get() = x.ships and set (_ : int) = ()
