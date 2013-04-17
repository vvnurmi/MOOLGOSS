module MOO.Server.Service

open MOO.Service
open MOO.State
open MOO.Types
open System
open System.Collections.Concurrent
open System.ServiceModel
open System.ServiceModel.Description

type ServiceState = {
    // Read by service threads, written by the main thread.
    mutable stardate : DateTime
    mutable planets : Planet array
    mutable formations : Formation array
    // Produced by service threads, consumed by the main thread.
    newClients : Client ConcurrentQueue
    newCommands : Command ConcurrentQueue
}
let serviceState = {
    stardate = new DateTime(2013, 3, 23)
    planets = [||]
    formations = [||]
    newClients = new (Client ConcurrentQueue)()
    newCommands = new (Command ConcurrentQueue)()
}
let rec processQueue<'a> (queue : 'a ConcurrentQueue) f =
    state {
        match queue.TryDequeue() with
        | true, x ->
            do! f x
            return! processQueue queue f
        | false, _ -> ()
    }
let addClients () = processQueue serviceState.newClients addClient
let addCommands () = processQueue serviceState.newCommands addCommand
let updateServiceState () =
    state {
        let! stardate = getStardate
        let! planets = getPlanets
        let! formations = getFormations
        serviceState.stardate <- stardate
        serviceState.planets <- Array.map snd <| Map.toArray planets
        serviceState.formations <- Array.map snd <| Map.toArray formations
    }

[<ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)>]
type MOOService() =
    interface IMOOService with
        member x.Authenticate name =
            let client = {
                player = name
                sessionID = OperationContext.Current.SessionId
            }
            serviceState.newClients.Enqueue(client)
        member x.GetUpdate() =
            serviceState.stardate
        member x.GetPlanets() =
            serviceState.planets
        member x.GetFormations() =
            serviceState.formations
        member x.IssueCommand command =
            serviceState.newCommands.Enqueue(CommandC.toCommand command)

let runWithService f =
    let core baseAddresses bindings httpGetEnabled =
        use host = new ServiceHost(typeof<MOOService>, baseAddresses)
        host.UnknownMessageReceived.Add <| fun args -> printfn "Note: Unknown message received. Skipping it."
        let openSucceeded =
            try
                let addEndpoint binding =
                    host.AddServiceEndpoint(typeof<IMOOService>, binding, "MOOService") |> ignore
                List.iter addEndpoint bindings
                if httpGetEnabled then host.Description.Behaviors.Add <| new ServiceMetadataBehavior(HttpGetEnabled = true)
                host.Open()
                true
            with
            | :? CommunicationException as ex ->
                printfn "An exception occurred: %s" ex.Message
                host.Abort()
                false
        if openSucceeded then
            try
                f ()
            with
            | :? CommunicationException as ex ->
                printfn "An exception occurred: %s" ex.Message
                host.Abort()
            true
        else false

    let httpBaseAddress = new Uri(Uri.UriSchemeHttp + "://localhost:8000/MOO")
    let pipeBaseAddress = new Uri(Uri.UriSchemeNetPipe + "://localhost/MOO")
    let bindings = [ new BasicHttpBinding() :> Channels.Binding; upcast new NetNamedPipeBinding() ]
    if not <| core [| httpBaseAddress; pipeBaseAddress |] bindings true then
        printfn "An exception occurred while creating the HTTP endpoint. Try running the server with administrator rights?"
        core [| pipeBaseAddress |] bindings.Tail false |> ignore

let dropClient (c : Client) =
    state {
        printfn "Dropping client %s" c.player
        do! removeClient c
    }
