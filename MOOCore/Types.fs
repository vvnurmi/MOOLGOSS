namespace MOO.Core.Types

open System

type ID = int
type Location =
    | Planet of ID
type Command =
    | MoveFormation of ID * Location
type Planet = {
    id : ID
    player : string option
    name : string
    population : int
    maxPopulation : int
    orbit : int
}
type Formation = {
    id : ID
    player : string
    location : Location
    ships : int
}
type UpdateData = {
    stardate : DateTime
    updateInterval : TimeSpan
    nextUpdate : TimeSpan
}

module Conversion =
    let fromOption = function
        | None -> null
        | Some s -> s
    let fromEnumerable (a : System.Collections.Generic.IEnumerable<'a>) =
        new System.Collections.Generic.List<'a>(a)
    let fromTimeSpan (t : TimeSpan) =
        new MOO.Service.TimeSpan(Milliseconds = (int)t.TotalMilliseconds)
    let fromDateTime (t : DateTime) =
        new MOO.Service.DateTime(
            Year = t.Year,
            Month = (byte)t.Month,
            Day = (byte)t.Day,
            Hour = (byte)t.Hour,
            Minute = (byte)t.Minute)
    let fromLocation = function
        | Planet id -> new MOO.Service.Location(Planet = id)
    let toLocation (loc : MOO.Service.Location) =
        Planet(loc.Planet)
    let fromFormation f =
        new MOO.Service.Formation(
            ID = f.id,
            Player = f.player,
            Location = fromLocation f.location,
            Ships = f.ships)
    let fromPlanet (p : Planet) =
        new MOO.Service.Planet(
            ID = p.id,
            Player = fromOption p.player,
            Name = p.name,
            Population = p.population,
            MaxPopulation = p.maxPopulation,
            Orbit = p.orbit)
    let fromUpdateData ud =
        new MOO.Service.UpdateData(
            Stardate = fromDateTime ud.stardate,
            UpdateInterval = fromTimeSpan ud.updateInterval,
            NextUpdate = fromTimeSpan ud.nextUpdate)
    let (|MoveFormation|_|) (c : MOO.Service.Command) =
        match c.Type with
        | MOO.Service.CommandType.MoveFormation -> Some(c.MoveFormationData.Formation, c.MoveFormationData.Destination)
        | _ -> None
    let toCommand = function
        | MoveFormation(formation, destination) ->
            MoveFormation(formation, toLocation destination)
        | _ -> failwith "Unknown command"
