module MOO.Server.Service

open MOO.Core.State
open MOO.Core.Types
open System
open System.Collections.Concurrent

type ServiceState = {
    // Read by service threads, written by the main thread.
    mutable stardate : DateTime
    mutable updateInterval : TimeSpan
    mutable nextUpdateWallTime : DateTime
    mutable planets : Planet array
    mutable formations : Formation array
    // Produced by service threads, consumed by the main thread.
    newClients : string ConcurrentQueue
    newCommands : Command ConcurrentQueue
}
let serviceState = {
    stardate = new DateTime(2013, 3, 23)
    updateInterval = TimeSpan.FromSeconds(10.0)
    nextUpdateWallTime = DateTime.Now
    planets = [||]
    formations = [||]
    newClients = new (string ConcurrentQueue)()
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
        let! updateInterval = getUpdateInterval
        let! planets = getPlanets
        let! formations = getFormations
        serviceState.stardate <- stardate
        serviceState.updateInterval <- updateInterval
        serviceState.nextUpdateWallTime <- DateTime.Now + updateInterval
        serviceState.planets <- Array.map snd <| Map.toArray planets
        serviceState.formations <- Array.map snd <| Map.toArray formations
    }

type MOOServiceHandler() =
    interface MOO.Service.MOOService.Iface with
        member x.Authenticate name =
            serviceState.newClients.Enqueue(name)
        member x.GetUpdate() =
            Conversion.fromUpdateData {
                stardate = serviceState.stardate
                updateInterval = serviceState.updateInterval
                nextUpdate = serviceState.nextUpdateWallTime - DateTime.Now }
        member x.GetPlanets() =
            Conversion.fromEnumerable <| Array.map Conversion.fromPlanet serviceState.planets
        member x.GetFormations() =
            Conversion.fromEnumerable <| Array.map Conversion.fromFormation serviceState.formations
        member x.IssueCommand command =
            serviceState.newCommands.Enqueue(Conversion.toCommand command)

// Returns a function that stops the service.
let startService =
    let handler = new MOOServiceHandler()
    let processor = new MOO.Service.MOOService.Processor(handler)
    let serverTransport = new Thrift.Transport.TServerSocket(8000)
    let server = new Thrift.Server.TThreadPoolServer(processor, serverTransport)
    Async.Start(async { server.Serve() })
    server.Stop

let dropClient c =
    state {
        printfn "Dropping client %s" c
        do! removeClient c
    }
