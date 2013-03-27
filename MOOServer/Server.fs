module MOO.Server

open MOO.Service
open MOO.State
open MOO.Types
open System
open System.ServiceModel
open System.ServiceModel.Description

type ServiceState = {
    mutable planets : Planet array
}
let serviceState = {
    planets = [||]
}
let updateServiceState =
    state {
        let! planets = getPlanets
        serviceState.planets <- Array.map snd <| Map.toArray planets
    }

[<ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)>]
type MOOService() =
    interface IMOOService with
        member x.GetPlanets() = serviceState.planets

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
