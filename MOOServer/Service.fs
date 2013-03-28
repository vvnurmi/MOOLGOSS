module MOO.Server.Service

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

let runWithService f =
    let httpBaseAddress = new Uri(Uri.UriSchemeHttp + "://localhost:8000/MOO")
    let pipeBaseAddress = new Uri(Uri.UriSchemeNetPipe + "://localhost/MOO")
    use host = new ServiceHost(typeof<MOOService>, httpBaseAddress, pipeBaseAddress)
    try
        host.AddServiceEndpoint(typeof<IMOOService>, new WSDualHttpBinding(), "MOOService") |> ignore
        host.AddServiceEndpoint(typeof<IMOOService>, new NetNamedPipeBinding(), "MOOService") |> ignore
        host.Description.Behaviors.Add <| new ServiceMetadataBehavior(HttpGetEnabled = true)
        host.Open()
        f ()
        host.Close()
    with
    | :? CommunicationException as ex ->
        printfn "An exception occurred: %s" ex.Message
        host.Abort()
