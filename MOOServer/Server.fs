module MOO.Server.App

open MOO.Server.Service
open MOO.State
open MOO.Types
open System

let printPlanet p =
    printfn "%s, population %i/%i, orbit %i" p.name p.population p.maxPopulation p.orbit
let printState =
    state {
        let! planets = getPlanets
        printfn "\n%i planets" planets.Count
        Map.iter (fun id p -> printPlanet p) planets
    }
let rec uiLoop () =
    state {
        do! updateServiceState
        do! printState
        let input = Console.ReadLine()
        if input <> "q" then
            let! planets = getPlanets
            let n = planets.Count + 1
            let planet = {
                id = 0
                name = sprintf "Planet %i" n
                maxPopulation = 99
                population = 42
                orbit = n
            }
            do! addPlanet planet
            do! uiLoop ()
    }
let main =
    runWithService <| fun () -> run (uiLoop ()) initialState |> ignore
