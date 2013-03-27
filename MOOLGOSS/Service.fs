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
