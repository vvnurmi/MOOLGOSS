module MOO.State

open MOO.Service
open MOO.Types

type StateOp<'s, 'a when 's : equality> =
    StateOp of ('s -> 'a * 's)

let run (StateOp f) state = f state
let lift f mg =
    StateOp <| fun state ->
    let gResult, state2 = run mg state
    f gResult, state2
let adapt2 f op a =
    StateOp <| fun state ->
    let mutableState = ref state
    let opAdapted q =
        let rOp, newState = run (op q) !mutableState
        mutableState := newState
        rOp
    f opAdapted a, !mutableState
let adapt3 f op a b =
    StateOp <| fun state ->
    let mutableState = ref state
    let opAdapted q p =
        let rOp, newState = run (op q p), !mutableState
        mutableState := newState
        rOp
    f opAdapted a b, !mutableState
let compose f g = fun a ->
    StateOp <| fun state ->
    let rf, state2 = run (f a) state
    run (g rf) state2
let bind mf g =
    StateOp <| fun state ->
    let rf, state2 = run mf state
    run (g rf) state2
let ret a =
    StateOp <| fun state -> a, state

type StateBuilder() =
    member x.Lift(f, mg) = lift f mg
    member x.Compose(f, g) = compose f g
    member x.Bind(mf, g) = bind mf g
    member x.Combine(mf, mg) = bind mf <| fun _ -> mg
    member x.Return(a) = ret a
    member x.ReturnFrom(mf) = StateOp <| fun state -> run mf state
    member x.Zero() = StateOp <| fun state -> (), state
    member x.For(s, f) = adapt2 Seq.iter f s
    member x.Delay(f) = f ()

let state = StateBuilder()
let (~%) a = ret a
let (%|>) op opf = bind op opf
let (%>>) opf1 opf2 = compose opf1 opf2

let getState f =
    StateOp <| fun state ->
    f state, state
let mapState f =
    StateOp <| fun state ->
    (), f state

type State = {
    nextID : int
    stardate : System.DateTime
    planets : Map<ID, Planet>
    clients : Client list
    formations : Map<ID, Formation>
    commands : Command list
}

let initialState = {
    nextID = 1
    stardate = new System.DateTime(2215, 3, 23)
    planets = Map.empty
    clients = []
    formations = Map.empty
    commands = []
}

let getNewID =
    StateOp <| fun state ->
    state.nextID, { state with nextID = state.nextID + 1 }

let getStardate =
    getState <| fun state -> state.stardate
let setStardate sd =
    mapState <| fun state -> { state with stardate = sd }

let getPlanets =
    getState <| fun state -> state.planets
let setPlanets planets =
    mapState <| fun state -> { state with planets = planets }
let getPlanet id =
    getState <| fun state -> Map.find id state.planets
let setPlanet (p : Planet) =
    mapState <| fun state -> { state with planets = Map.add p.id p state.planets }
let addPlanet p =
    getNewID %|> fun id -> setPlanet { p with id = id }
let mapPlanet f id =
    bind (getPlanet id |> lift f) setPlanet

let getClients =
    getState <| fun state -> state.clients
let addClient (c : Client) =
    printfn "Adding client %s" c.player
    mapState <| fun state -> { state with clients = c :: state.clients |> Set.ofList |> List.ofSeq }
let removeClient c =
    mapState <| fun state -> { state with clients = List.filter (fun x -> x <> c) state.clients }

let getFormations =
    getState <| fun state -> state.formations
let getFormation id =
    getState <| fun state -> Map.find id state.formations
let setFormation f =
    mapState <| fun state -> { state with formations = Map.add f.id f state.formations }
let addFormation f =
    getNewID %|> fun id -> setFormation { f with id = id }
let mapFormation f id =
    bind (getFormation id |> lift f) setFormation

let getCommands =
    getState <| fun state -> state.commands
let addCommand c =
    mapState <| fun state -> { state with commands = c :: state.commands }
let clearCommands =
    mapState <| fun state -> { state with commands = [] }
