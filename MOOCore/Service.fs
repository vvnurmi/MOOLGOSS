namespace MOO.Service

open MOO.Types
open System
open System.ServiceModel
open System.ServiceModel.Web

type IMOOCallbackContract =
    [<OperationContract(IsOneWay = true)>]
    abstract member Update : now : DateTime -> unit

[<ServiceContract(CallbackContract = typeof<IMOOCallbackContract>)>]
type IMOOService =
    [<OperationContract>]
    abstract member GetPlanets : unit -> Planet[]

[<CustomEquality; CustomComparison>]
type Client =
    {
        sessionID : string
        channel : IMOOCallbackContract
    }
    override x.Equals(yobj) = 
        match yobj with
        | :? Client as y -> x.sessionID = y.sessionID
        | _ -> false
    override x.GetHashCode() = hash x.sessionID
    interface System.IComparable with
        member x.CompareTo yobj = 
            match yobj with
            | :? Client as y -> compare x.sessionID y.sessionID
            | _ -> invalidArg "yobj" "cannot compare values of different types"
