module MOO.Server.App

open MOO.Players
open MOO.Server.Service
open MOO.Service
open MOO.State
open MOO.Types
open System

let printWelcome =
    printfn @"
    MOOLGOSS  Copyright (C) 2013  Ville Nurmi
    This program comes with ABSOLUTELY NO WARRANTY.
    This is free software, and you are welcome to redistribute it
    under certain conditions; see LICENSE.txt for details."
    printfn ""
let printPlanet p =
    printfn "%s, population %i/%i, orbit %i" p.name p.population p.maxPopulation p.orbit
let printFormation f =
    printfn "%s's formation with %i ships" f.player f.ships
let printState =
    state {
        let! planets = getPlanets
        printfn "\n%i planets" planets.Count
        Map.iter (fun id p -> printPlanet p) planets
        let! formations = getFormations
        printfn "\n%i formations" formations.Count
        Map.iter (fun id f -> printFormation f) formations
    }
let createPlanetarySystem =
    state {
        do! addPlanet {
            id = 0
            player = None
            name = "Planet 1"
            maxPopulation = 30
            population = 0
            orbit = 1
        }
        do! addPlanet {
            id = 0
            player = None
            name = "Planet 2"
            maxPopulation = 1000
            population = 0
            orbit = 2
        }
        do! addPlanet {
            id = 0
            player = Some "Tellurians"
            name = "Planet 3"
            maxPopulation = 400
            population = 50
            orbit = 3
        }
    }
let carryOutCommand c =
    state {
        match c with
        | MoveFormation(id, location) ->
            let! formation = getFormation id
            do! id |> mapFormation (fun f -> { f with location = location })
    }
let updateFormation f =
    state {
        match f.location with
        | Planet id ->
            let! planet = getPlanet id
            match planet.player with
            | Some p when p = f.player -> ()
            | _ ->
                let bodyCount = 15
                if planet.population <= bodyCount
                then do! id |> mapPlanet (fun p -> { p with player = Some f.player; population = 2 })
                else do! id |> mapPlanet (fun p -> { p with population = planet.population - bodyCount })
    }
let updatePlanet p =
    state {
        match p.population with
        | n when n > p.maxPopulation -> do! p.id |> mapPlanet (fun p -> { p with population = max p.maxPopulation <| p.population - 2 })
        | n when n > 0 -> do! p.id |> mapPlanet (fun p -> { p with population = min p.maxPopulation <| (p.population * 103) / 100 + 1 })
        | _ -> ()
    }
let updateUniverse =
    state {
        let! stardate = getStardate
        let newStardate = stardate + TimeSpan.FromHours(1.0)
        do! setStardate newStardate
        printfn "It's %s" <| newStardate.ToString("yyyy-MM-dd HH:mm")
        do! getCommands %|> adapt2 List.iter carryOutCommand
        do! clearCommands
        let! formations = lift (Map.toList >> List.map snd) getFormations
        do! adapt2 List.iter updateFormation formations
        let! planets = lift (Map.toList >> List.map snd) getPlanets
        do! adapt2 List.iter updatePlanet planets
    }
let initNewPlayers =
    state {
        let! clients = getClients
        let players = clients |> List.map (fun c -> c.player)
        let! newPlayers = adapt2 List.filter (fun p -> lift not (hasPlanets p)) players
        do! adapt2 List.iter initPlayer newPlayers
    }
let init =
    state {
        do! createPlanetarySystem
    }
let rec updateLoop () =
    state {
        do! addClients ()
        do! addCommands ()
        do! updateUniverse
        do! initNewPlayers
        do! printState
        do! updateServiceState ()
        let! updateInterval = getUpdateInterval
        System.Threading.Thread.Sleep(updateInterval)
        return! updateLoop ()
    }
let mainCore =
    state {
        printWelcome
        do! init
        do! updateLoop ()
        do! getClients %|> adapt2 List.iter dropClient
    }
let main =
    runWithService <| fun () -> run mainCore initialState |> ignore
