namespace MOO.Service

open MOO.Types
open System
open System.Runtime.Serialization
open System.ServiceModel
open System.ServiceModel.Web

[<DataContract>]
type UpdateData =
    {
    stardate : DateTime
    updateInterval : TimeSpan
    nextUpdate : TimeSpan
    }
    [<DataMember>]
    member x.Stardate with get() = x.stardate and set (_ : DateTime) = ()
    [<DataMember>]
    member x.UpdateInterval with get() = x.updateInterval and set (_ : TimeSpan) = ()
    [<DataMember>]
    member x.NextUpdate with get() = x.nextUpdate and set (_ : TimeSpan) = ()

[<ServiceContract>]
type IMOOService =
    [<OperationContract>]
    abstract member Authenticate : name : string -> unit
    [<OperationContract>]
    abstract member GetUpdate : unit -> UpdateData
    [<OperationContract>]
    abstract member GetPlanets : unit -> Planet[]
    [<OperationContract>]
    abstract member GetFormations : unit -> Formation[]
    [<OperationContract>]
    abstract member IssueCommand : command : CommandC.Base -> unit

[<CustomEquality; CustomComparison>]
type Client =
    {
        player : string
        sessionID : string
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
