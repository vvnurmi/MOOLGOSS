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


  public class Location implements TBase   {
    private static const STRUCT_DESC:TStruct = new TStruct("Location");
    private static const PLANET_FIELD_DESC:TField = new TField("planet", TType.I32, 1);

    private var _planet:int;
    public static const PLANET:int = 1;

    private var __isset_planet:Boolean = false;

    public static const metaDataMap:Dictionary = new Dictionary();
    {
      metaDataMap[PLANET] = new FieldMetaData("planet", TFieldRequirementType.DEFAULT, 
          new FieldValueMetaData(TType.I32));
    }
    {
      FieldMetaData.addStructMetaDataMap(Location, metaDataMap);
    }

    public function Location() {
    }

    public function get planet():int {
      return this._planet;
    }

    public function set planet(planet:int):void {
      this._planet = planet;
      this.__isset_planet = true;
    }

    public function unsetPlanet():void {
      this.__isset_planet = false;
    }

    // Returns true if field planet is set (has been assigned a value) and false otherwise
    public function isSetPlanet():Boolean {
      return this.__isset_planet;
    }

    public function setFieldValue(fieldID:int, value:*):void {
      switch (fieldID) {
      case PLANET:
        if (value == null) {
          unsetPlanet();
        } else {
          this.planet = value;
        }
        break;

      default:
        throw new ArgumentError("Field " + fieldID + " doesn't exist!");
      }
    }

    public function getFieldValue(fieldID:int):* {
      switch (fieldID) {
      case PLANET:
        return this.planet;
      default:
        throw new ArgumentError("Field " + fieldID + " doesn't exist!");
      }
    }

    // Returns true if field corresponding to fieldID is set (has been assigned a value) and false otherwise
    public function isSet(fieldID:int):Boolean {
      switch (fieldID) {
      case PLANET:
        return isSetPlanet();
      default:
        throw new ArgumentError("Field " + fieldID + " doesn't exist!");
      }
    }

    public function read(iprot:TProtocol):void {
      var field:TField;
      iprot.readStructBegin();
      while (true)
      {
        field = iprot.readFieldBegin();
        if (field.type == TType.STOP) { 
          break;
        }
        switch (field.id)
        {
          case PLANET:
            if (field.type == TType.I32) {
              this.planet = iprot.readI32();
              this.__isset_planet = true;
            } else { 
              TProtocolUtil.skip(iprot, field.type);
            }
            break;
          default:
            TProtocolUtil.skip(iprot, field.type);
            break;
        }
        iprot.readFieldEnd();
      }
      iprot.readStructEnd();


      // check for required fields of primitive type, which can't be checked in the validate method
      validate();
    }

    public function write(oprot:TProtocol):void {
      validate();

      oprot.writeStructBegin(STRUCT_DESC);
      oprot.writeFieldBegin(PLANET_FIELD_DESC);
      oprot.writeI32(this.planet);
      oprot.writeFieldEnd();
      oprot.writeFieldStop();
      oprot.writeStructEnd();
    }

    public function toString():String {
      var ret:String = new String("Location(");
      var first:Boolean = true;

      ret += "planet:";
      ret += this.planet;
      first = false;
      ret += ")";
      return ret;
    }

    public function validate():void {
      // check for required fields
      // check that fields of type enum have valid values
    }

  }

}
