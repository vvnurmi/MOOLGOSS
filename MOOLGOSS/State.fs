module MOO.State

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

type StateBuilder() =
    member x.Lift(f, mg) = lift f mg
    member x.Compose(f, g) = compose f g
    member x.Bind(mf, g) = bind mf g
    member x.Combine(mf, mg) = bind mf <| fun _ -> mg
    member x.Return(a) = StateOp <| fun state -> a, state
    member x.ReturnFrom(mf) = StateOp <| fun state -> run mf state
    member x.Zero() = StateOp <| fun state -> (), state
    member x.For(s, f) = adapt2 Seq.iter f s
    member x.Delay(f) = f ()

let state = StateBuilder()
let (~%) a = state.Return a
let (%|>) op opf = state.Bind(op, opf)
let (%>>) opf1 opf2 = state.Compose(opf1, opf2)

let getState f =
    StateOp <| fun state ->
    f state, state
let mapState f =
    StateOp <| fun state ->
    (), f state

type State = {
    nextId : int
}

let stateUnit = {
    nextId = 1
}

let getNewId =
    StateOp <| fun state ->
    state.nextId, { state with nextId = state.nextId + 1 }
