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


  public class DateTime implements TBase   {
    private static const STRUCT_DESC:TStruct = new TStruct("DateTime");
    private static const YEAR_FIELD_DESC:TField = new TField("year", TType.I32, 1);
    private static const MONTH_FIELD_DESC:TField = new TField("month", TType.BYTE, 2);
    private static const DAY_FIELD_DESC:TField = new TField("day", TType.BYTE, 3);
    private static const HOUR_FIELD_DESC:TField = new TField("hour", TType.BYTE, 4);
    private static const MINUTE_FIELD_DESC:TField = new TField("minute", TType.BYTE, 5);

    private var _year:int;
    public static const YEAR:int = 1;
    private var _month:int;
    public static const MONTH:int = 2;
    private var _day:int;
    public static const DAY:int = 3;
    private var _hour:int;
    public static const HOUR:int = 4;
    private var _minute:int;
    public static const MINUTE:int = 5;

    private var __isset_year:Boolean = false;
    private var __isset_month:Boolean = false;
    private var __isset_day:Boolean = false;
    private var __isset_hour:Boolean = false;
    private var __isset_minute:Boolean = false;

    public static const metaDataMap:Dictionary = new Dictionary();
    {
      metaDataMap[YEAR] = new FieldMetaData("year", TFieldRequirementType.DEFAULT, 
          new FieldValueMetaData(TType.I32));
      metaDataMap[MONTH] = new FieldMetaData("month", TFieldRequirementType.DEFAULT, 
          new FieldValueMetaData(TType.BYTE));
      metaDataMap[DAY] = new FieldMetaData("day", TFieldRequirementType.DEFAULT, 
          new FieldValueMetaData(TType.BYTE));
      metaDataMap[HOUR] = new FieldMetaData("hour", TFieldRequirementType.DEFAULT, 
          new FieldValueMetaData(TType.BYTE));
      metaDataMap[MINUTE] = new FieldMetaData("minute", TFieldRequirementType.DEFAULT, 
          new FieldValueMetaData(TType.BYTE));
    }
    {
      FieldMetaData.addStructMetaDataMap(DateTime, metaDataMap);
    }

    public function DateTime() {
    }

    public function get year():int {
      return this._year;
    }

    public function set year(year:int):void {
      this._year = year;
      this.__isset_year = true;
    }

    public function unsetYear():void {
      this.__isset_year = false;
    }

    // Returns true if field year is set (has been assigned a value) and false otherwise
    public function isSetYear():Boolean {
      return this.__isset_year;
    }

    public function get month():int {
      return this._month;
    }

    public function set month(month:int):void {
      this._month = month;
      this.__isset_month = true;
    }

    public function unsetMonth():void {
      this.__isset_month = false;
    }

    // Returns true if field month is set (has been assigned a value) and false otherwise
    public function isSetMonth():Boolean {
      return this.__isset_month;
    }

    public function get day():int {
      return this._day;
    }

    public function set day(day:int):void {
      this._day = day;
      this.__isset_day = true;
    }

    public function unsetDay():void {
      this.__isset_day = false;
    }

    // Returns true if field day is set (has been assigned a value) and false otherwise
    public function isSetDay():Boolean {
      return this.__isset_day;
    }

    public function get hour():int {
      return this._hour;
    }

    public function set hour(hour:int):void {
      this._hour = hour;
      this.__isset_hour = true;
    }

    public function unsetHour():void {
      this.__isset_hour = false;
    }

    // Returns true if field hour is set (has been assigned a value) and false otherwise
    public function isSetHour():Boolean {
      return this.__isset_hour;
    }

    public function get minute():int {
      return this._minute;
    }

    public function set minute(minute:int):void {
      this._minute = minute;
      this.__isset_minute = true;
    }

    public function unsetMinute():void {
      this.__isset_minute = false;
    }

    // Returns true if field minute is set (has been assigned a value) and false otherwise
    public function isSetMinute():Boolean {
      return this.__isset_minute;
    }

    public function setFieldValue(fieldID:int, value:*):void {
      switch (fieldID) {
      case YEAR:
        if (value == null) {
          unsetYear();
        } else {
          this.year = value;
        }
        break;

      case MONTH:
        if (value == null) {
          unsetMonth();
        } else {
          this.month = value;
        }
        break;

      case DAY:
        if (value == null) {
          unsetDay();
        } else {
          this.day = value;
        }
        break;

      case HOUR:
        if (value == null) {
          unsetHour();
        } else {
          this.hour = value;
        }
        break;

      case MINUTE:
        if (value == null) {
          unsetMinute();
        } else {
          this.minute = value;
        }
        break;

      default:
        throw new ArgumentError("Field " + fieldID + " doesn't exist!");
      }
    }

    public function getFieldValue(fieldID:int):* {
      switch (fieldID) {
      case YEAR:
        return this.year;
      case MONTH:
        return this.month;
      case DAY:
        return this.day;
      case HOUR:
        return this.hour;
      case MINUTE:
        return this.minute;
      default:
        throw new ArgumentError("Field " + fieldID + " doesn't exist!");
      }
    }

    // Returns true if field corresponding to fieldID is set (has been assigned a value) and false otherwise
    public function isSet(fieldID:int):Boolean {
      switch (fieldID) {
      case YEAR:
        return isSetYear();
      case MONTH:
        return isSetMonth();
      case DAY:
        return isSetDay();
      case HOUR:
        return isSetHour();
      case MINUTE:
        return isSetMinute();
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
          case YEAR:
            if (field.type == TType.I32) {
              this.year = iprot.readI32();
              this.__isset_year = true;
            } else { 
              TProtocolUtil.skip(iprot, field.type);
            }
            break;
          case MONTH:
            if (field.type == TType.BYTE) {
              this.month = iprot.readByte();
              this.__isset_month = true;
            } else { 
              TProtocolUtil.skip(iprot, field.type);
            }
            break;
          case DAY:
            if (field.type == TType.BYTE) {
              this.day = iprot.readByte();
              this.__isset_day = true;
            } else { 
              TProtocolUtil.skip(iprot, field.type);
            }
            break;
          case HOUR:
            if (field.type == TType.BYTE) {
              this.hour = iprot.readByte();
              this.__isset_hour = true;
            } else { 
              TProtocolUtil.skip(iprot, field.type);
            }
            break;
          case MINUTE:
            if (field.type == TType.BYTE) {
              this.minute = iprot.readByte();
              this.__isset_minute = true;
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
      oprot.writeFieldBegin(YEAR_FIELD_DESC);
      oprot.writeI32(this.year);
      oprot.writeFieldEnd();
      oprot.writeFieldBegin(MONTH_FIELD_DESC);
      oprot.writeByte(this.month);
      oprot.writeFieldEnd();
      oprot.writeFieldBegin(DAY_FIELD_DESC);
      oprot.writeByte(this.day);
      oprot.writeFieldEnd();
      oprot.writeFieldBegin(HOUR_FIELD_DESC);
      oprot.writeByte(this.hour);
      oprot.writeFieldEnd();
      oprot.writeFieldBegin(MINUTE_FIELD_DESC);
      oprot.writeByte(this.minute);
      oprot.writeFieldEnd();
      oprot.writeFieldStop();
      oprot.writeStructEnd();
    }

    public function toString():String {
      var ret:String = new String("DateTime(");
      var first:Boolean = true;

      ret += "year:";
      ret += this.year;
      first = false;
      if (!first) ret +=  ", ";
      ret += "month:";
      ret += this.month;
      first = false;
      if (!first) ret +=  ", ";
      ret += "day:";
      ret += this.day;
      first = false;
      if (!first) ret +=  ", ";
      ret += "hour:";
      ret += this.hour;
      first = false;
      if (!first) ret +=  ", ";
      ret += "minute:";
      ret += this.minute;
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
