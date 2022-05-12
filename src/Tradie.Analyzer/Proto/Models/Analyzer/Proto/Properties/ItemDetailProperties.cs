// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Models/Analyzer/Proto/Properties/ItemDetailProperties.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Tradie.Analyzer.Proto {

  /// <summary>Holder for reflection information generated from Models/Analyzer/Proto/Properties/ItemDetailProperties.proto</summary>
  public static partial class ItemDetailPropertiesReflection {

    #region Descriptor
    /// <summary>File descriptor for Models/Analyzer/Proto/Properties/ItemDetailProperties.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ItemDetailPropertiesReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CjtNb2RlbHMvQW5hbHl6ZXIvUHJvdG8vUHJvcGVydGllcy9JdGVtRGV0YWls",
            "UHJvcGVydGllcy5wcm90bxooTW9kZWxzL0FuYWx5emVyL1Byb3RvL1JlcXVp",
            "cmVtZW50cy5wcm90byKhAQoUSXRlbURldGFpbFByb3BlcnRpZXMSDAoEbmFt",
            "ZRgBIAEoCRINCgVmbGFncxgCIAEoDRISCgppbmZsdWVuY2VzGAMgASgNEhEK",
            "CWl0ZW1MZXZlbBgEIAEoDRIQCghpY29uUGF0aBgFIAEoCRIjCgxyZXF1aXJl",
            "bWVudHMYBiABKAsyDS5SZXF1aXJlbWVudHMSDgoGcmFyaXR5GAcgASgNQhiq",
            "AhVUcmFkaWUuQW5hbHl6ZXIuUHJvdG9iBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Tradie.Analyzer.Proto.RequirementsReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Tradie.Analyzer.Proto.ItemDetailProperties), global::Tradie.Analyzer.Proto.ItemDetailProperties.Parser, new[]{ "Name", "Flags", "Influences", "ItemLevel", "IconPath", "Requirements", "Rarity" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class ItemDetailProperties : pb::IMessage<ItemDetailProperties>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ItemDetailProperties> _parser = new pb::MessageParser<ItemDetailProperties>(() => new ItemDetailProperties());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ItemDetailProperties> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Tradie.Analyzer.Proto.ItemDetailPropertiesReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ItemDetailProperties() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ItemDetailProperties(ItemDetailProperties other) : this() {
      name_ = other.name_;
      flags_ = other.flags_;
      influences_ = other.influences_;
      itemLevel_ = other.itemLevel_;
      iconPath_ = other.iconPath_;
      requirements_ = other.requirements_ != null ? other.requirements_.Clone() : null;
      rarity_ = other.rarity_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ItemDetailProperties Clone() {
      return new ItemDetailProperties(this);
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "flags" field.</summary>
    public const int FlagsFieldNumber = 2;
    private uint flags_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint Flags {
      get { return flags_; }
      set {
        flags_ = value;
      }
    }

    /// <summary>Field number for the "influences" field.</summary>
    public const int InfluencesFieldNumber = 3;
    private uint influences_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint Influences {
      get { return influences_; }
      set {
        influences_ = value;
      }
    }

    /// <summary>Field number for the "itemLevel" field.</summary>
    public const int ItemLevelFieldNumber = 4;
    private uint itemLevel_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint ItemLevel {
      get { return itemLevel_; }
      set {
        itemLevel_ = value;
      }
    }

    /// <summary>Field number for the "iconPath" field.</summary>
    public const int IconPathFieldNumber = 5;
    private string iconPath_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string IconPath {
      get { return iconPath_; }
      set {
        iconPath_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "requirements" field.</summary>
    public const int RequirementsFieldNumber = 6;
    private global::Tradie.Analyzer.Proto.Requirements requirements_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Tradie.Analyzer.Proto.Requirements Requirements {
      get { return requirements_; }
      set {
        requirements_ = value;
      }
    }

    /// <summary>Field number for the "rarity" field.</summary>
    public const int RarityFieldNumber = 7;
    private uint rarity_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint Rarity {
      get { return rarity_; }
      set {
        rarity_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ItemDetailProperties);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ItemDetailProperties other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if (Flags != other.Flags) return false;
      if (Influences != other.Influences) return false;
      if (ItemLevel != other.ItemLevel) return false;
      if (IconPath != other.IconPath) return false;
      if (!object.Equals(Requirements, other.Requirements)) return false;
      if (Rarity != other.Rarity) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (Flags != 0) hash ^= Flags.GetHashCode();
      if (Influences != 0) hash ^= Influences.GetHashCode();
      if (ItemLevel != 0) hash ^= ItemLevel.GetHashCode();
      if (IconPath.Length != 0) hash ^= IconPath.GetHashCode();
      if (requirements_ != null) hash ^= Requirements.GetHashCode();
      if (Rarity != 0) hash ^= Rarity.GetHashCode();
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
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (Flags != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Flags);
      }
      if (Influences != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(Influences);
      }
      if (ItemLevel != 0) {
        output.WriteRawTag(32);
        output.WriteUInt32(ItemLevel);
      }
      if (IconPath.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(IconPath);
      }
      if (requirements_ != null) {
        output.WriteRawTag(50);
        output.WriteMessage(Requirements);
      }
      if (Rarity != 0) {
        output.WriteRawTag(56);
        output.WriteUInt32(Rarity);
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
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (Flags != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Flags);
      }
      if (Influences != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(Influences);
      }
      if (ItemLevel != 0) {
        output.WriteRawTag(32);
        output.WriteUInt32(ItemLevel);
      }
      if (IconPath.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(IconPath);
      }
      if (requirements_ != null) {
        output.WriteRawTag(50);
        output.WriteMessage(Requirements);
      }
      if (Rarity != 0) {
        output.WriteRawTag(56);
        output.WriteUInt32(Rarity);
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
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (Flags != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Flags);
      }
      if (Influences != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Influences);
      }
      if (ItemLevel != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ItemLevel);
      }
      if (IconPath.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(IconPath);
      }
      if (requirements_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Requirements);
      }
      if (Rarity != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Rarity);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ItemDetailProperties other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.Flags != 0) {
        Flags = other.Flags;
      }
      if (other.Influences != 0) {
        Influences = other.Influences;
      }
      if (other.ItemLevel != 0) {
        ItemLevel = other.ItemLevel;
      }
      if (other.IconPath.Length != 0) {
        IconPath = other.IconPath;
      }
      if (other.requirements_ != null) {
        if (requirements_ == null) {
          Requirements = new global::Tradie.Analyzer.Proto.Requirements();
        }
        Requirements.MergeFrom(other.Requirements);
      }
      if (other.Rarity != 0) {
        Rarity = other.Rarity;
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
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 16: {
            Flags = input.ReadUInt32();
            break;
          }
          case 24: {
            Influences = input.ReadUInt32();
            break;
          }
          case 32: {
            ItemLevel = input.ReadUInt32();
            break;
          }
          case 42: {
            IconPath = input.ReadString();
            break;
          }
          case 50: {
            if (requirements_ == null) {
              Requirements = new global::Tradie.Analyzer.Proto.Requirements();
            }
            input.ReadMessage(Requirements);
            break;
          }
          case 56: {
            Rarity = input.ReadUInt32();
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
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 16: {
            Flags = input.ReadUInt32();
            break;
          }
          case 24: {
            Influences = input.ReadUInt32();
            break;
          }
          case 32: {
            ItemLevel = input.ReadUInt32();
            break;
          }
          case 42: {
            IconPath = input.ReadString();
            break;
          }
          case 50: {
            if (requirements_ == null) {
              Requirements = new global::Tradie.Analyzer.Proto.Requirements();
            }
            input.ReadMessage(Requirements);
            break;
          }
          case 56: {
            Rarity = input.ReadUInt32();
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
