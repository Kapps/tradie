// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Models/Analyzer/Proto/Requirements.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Tradie.Analyzer.Proto {

  /// <summary>Holder for reflection information generated from Models/Analyzer/Proto/Requirements.proto</summary>
  public static partial class RequirementsReflection {

    #region Descriptor
    /// <summary>File descriptor for Models/Analyzer/Proto/Requirements.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static RequirementsReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CihNb2RlbHMvQW5hbHl6ZXIvUHJvdG8vUmVxdWlyZW1lbnRzLnByb3RvIkQK",
            "DFJlcXVpcmVtZW50cxILCgNkZXgYASABKAUSCwoDc3RyGAIgASgFEgsKA2lu",
            "dBgDIAEoBRINCgVsZXZlbBgEIAEoBUIYqgIVVHJhZGllLkFuYWx5emVyLlBy",
            "b3RvYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Tradie.Analyzer.Proto.Requirements), global::Tradie.Analyzer.Proto.Requirements.Parser, new[]{ "Dex", "Str", "Int", "Level" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Requirements : pb::IMessage<Requirements>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<Requirements> _parser = new pb::MessageParser<Requirements>(() => new Requirements());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<Requirements> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Tradie.Analyzer.Proto.RequirementsReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Requirements() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Requirements(Requirements other) : this() {
      dex_ = other.dex_;
      str_ = other.str_;
      int_ = other.int_;
      level_ = other.level_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Requirements Clone() {
      return new Requirements(this);
    }

    /// <summary>Field number for the "dex" field.</summary>
    public const int DexFieldNumber = 1;
    private int dex_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int Dex {
      get { return dex_; }
      set {
        dex_ = value;
      }
    }

    /// <summary>Field number for the "str" field.</summary>
    public const int StrFieldNumber = 2;
    private int str_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int Str {
      get { return str_; }
      set {
        str_ = value;
      }
    }

    /// <summary>Field number for the "int" field.</summary>
    public const int IntFieldNumber = 3;
    private int int_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int Int {
      get { return int_; }
      set {
        int_ = value;
      }
    }

    /// <summary>Field number for the "level" field.</summary>
    public const int LevelFieldNumber = 4;
    private int level_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int Level {
      get { return level_; }
      set {
        level_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as Requirements);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(Requirements other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Dex != other.Dex) return false;
      if (Str != other.Str) return false;
      if (Int != other.Int) return false;
      if (Level != other.Level) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (Dex != 0) hash ^= Dex.GetHashCode();
      if (Str != 0) hash ^= Str.GetHashCode();
      if (Int != 0) hash ^= Int.GetHashCode();
      if (Level != 0) hash ^= Level.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (Dex != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Dex);
      }
      if (Str != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Str);
      }
      if (Int != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Int);
      }
      if (Level != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(Level);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (Dex != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Dex);
      }
      if (Str != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Str);
      }
      if (Int != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Int);
      }
      if (Level != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(Level);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (Dex != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Dex);
      }
      if (Str != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Str);
      }
      if (Int != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Int);
      }
      if (Level != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Level);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(Requirements other) {
      if (other == null) {
        return;
      }
      if (other.Dex != 0) {
        Dex = other.Dex;
      }
      if (other.Str != 0) {
        Str = other.Str;
      }
      if (other.Int != 0) {
        Int = other.Int;
      }
      if (other.Level != 0) {
        Level = other.Level;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Dex = input.ReadInt32();
            break;
          }
          case 16: {
            Str = input.ReadInt32();
            break;
          }
          case 24: {
            Int = input.ReadInt32();
            break;
          }
          case 32: {
            Level = input.ReadInt32();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            Dex = input.ReadInt32();
            break;
          }
          case 16: {
            Str = input.ReadInt32();
            break;
          }
          case 24: {
            Int = input.ReadInt32();
            break;
          }
          case 32: {
            Level = input.ReadInt32();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code