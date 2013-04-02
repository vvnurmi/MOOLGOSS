module MOO.Server.App

open MOO.Players
open MOO.Server.Service
open MOO.Service
open MOO.State
open MOO.Types
open System

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
let updateUniverse =
    state {
        let! stardate = getStardate
        let newStardate = stardate + TimeSpan.FromHours(1.0)
        do! setStardate newStardate
        printfn "It's %s" <| newStardate.ToString("yyyy-MM-dd HH:mm")
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
let rec uiLoop () =
    state {
        do! addClients ()
        do! updateUniverse
        do! initNewPlayers
        do! printState
        do! updateServiceState ()
        do! sendToClients sendUpdate
        let input = Console.ReadLine()
        if input <> "q" then
            do! uiLoop ()
    }
let mainCore =
    state {
        do! init
        do! uiLoop ()
        do! getClients %|> adapt2 List.iter dropClient
    }
let main =
    runWithService <| fun () -> run mainCore initialState |> ignore
