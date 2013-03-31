module MOO.Server.Service

open MOO.Service
open MOO.State
open MOO.Types
open System
open System.Collections.Concurrent
open System.ServiceModel
open System.ServiceModel.Description

let sendUpdate (c : Client) = c.channel.Update(DateTime.Now)

type ServiceState = {
    // Read by service threads, written by the main thread.
    mutable planets : Planet array
    // Produced by service threads, consumed by the main thread.
    newClients : Client ConcurrentQueue
}
let serviceState = {
    planets = [||]
    newClients = new (Client ConcurrentQueue)()
}
let updateServiceState =
    let rec addClients () =
        state {
            match serviceState.newClients.TryDequeue() with
            | true, c ->
                do! addClient c
                do! addClients ()
            | false, _ -> ()
        }
    state {
        let! planets = getPlanets
        serviceState.planets <- Array.map snd <| Map.toArray planets
        do! addClients ()
    }

[<ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)>]
type MOOService() =
    interface IMOOService with
        member x.Authenticate(name) =
            let client = {
                player = name
                sessionID = OperationContext.Current.SessionId
                channel = OperationContext.Current.GetCallbackChannel<IMOOCallbackContract>()
            }
            serviceState.newClients.Enqueue(client)
            sendUpdate client
        member x.GetPlanets() =
            serviceState.planets

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

let sendToClients f =
    let safely f c =
        try
            f c
            false
        with
        | :? CommunicationException -> true
    state {
        let! clients = getClients
        let badClients = List.filter (safely f) clients
        do! adapt2 List.iter removeClient badClients
    }
let dropClient c =
    state {
        (c.channel :?> System.ServiceModel.IClientChannel).Close()
        do! removeClient c
    }
