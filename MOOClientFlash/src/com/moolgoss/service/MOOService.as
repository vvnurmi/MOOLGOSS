/**
 * Autogenerated by Thrift Compiler (0.9.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
package com.moolgoss.service {

import org.apache.thrift.Set;
import flash.utils.ByteArray;
import flash.utils.Dictionary;

import org.apache.thrift.*;
import org.apache.thrift.meta_data.*;
import org.apache.thrift.protocol.*;


  public interface MOOService {

    //function onError(Error):void;
    //function onSuccess():void;
    function Authenticate(name:String, onError:Function, onSuccess:Function):void;

    //function onError(Error):void;
    //function onSuccess(UpdateData):void;
    function GetUpdate(onError:Function, onSuccess:Function):void;

    //function onError(Error):void;
    //function onSuccess(Array):void;
    function GetPlanets(onError:Function, onSuccess:Function):void;

    //function onError(Error):void;
    //function onSuccess(Array):void;
    function GetFormations(onError:Function, onSuccess:Function):void;

    //function onError(Error):void;
    //function onSuccess():void;
    function IssueCommand(command:Command, onError:Function, onSuccess:Function):void;

  }

}
