module MOO.Server

open MOO.Service
open MOO.State
open MOO.Types
open System
open System.ServiceModel
open System.ServiceModel.Description

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
    let httpBaseAddress = new Uri(Uri.UriSchemeHttp + "://localhost:8000/MOO")
    let pipeBaseAddress = new Uri(Uri.UriSchemeNetPipe + "://localhost/MOO")
    use host = new ServiceHost(typeof<MOOService>, httpBaseAddress, pipeBaseAddress)
    try
        host.AddServiceEndpoint(typeof<IMOOService>, new WSDualHttpBinding(), "MOOService") |> ignore
        host.AddServiceEndpoint(typeof<IMOOService>, new NetNamedPipeBinding(), "MOOService") |> ignore
        host.Description.Behaviors.Add <| new ServiceMetadataBehavior(HttpGetEnabled = true)
        host.Open()
        let _, finalState = run (uiLoop ()) initialState
        host.Close()
    with
    | :? CommunicationException as ex ->
        printfn "An exception occurred: %s" ex.Message
        host.Abort()
