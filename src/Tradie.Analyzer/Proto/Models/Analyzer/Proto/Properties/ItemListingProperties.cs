// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Models/Analyzer/Proto/Properties/ItemListingProperties.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Tradie.Analyzer.Proto {

  /// <summary>Holder for reflection information generated from Models/Analyzer/Proto/Properties/ItemListingProperties.proto</summary>
  public static partial class ItemListingPropertiesReflection {

    #region Descriptor
    /// <summary>File descriptor for Models/Analyzer/Proto/Properties/ItemListingProperties.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ItemListingPropertiesReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CjxNb2RlbHMvQW5hbHl6ZXIvUHJvdG8vUHJvcGVydGllcy9JdGVtTGlzdGlu",
            "Z1Byb3BlcnRpZXMucHJvdG8iQQoJSXRlbVByaWNlEhAKCGN1cnJlbmN5GAEg",
            "ASgNEg4KBmFtb3VudBgCIAEoAhISCgpidXlvdXRLaW5kGAMgASgNIlYKFUl0",
            "ZW1MaXN0aW5nUHJvcGVydGllcxIJCgF4GAEgASgNEgkKAXkYAiABKA0SGQoF",
            "cHJpY2UYAyABKAsyCi5JdGVtUHJpY2USDAoEbm90ZRgEIAEoCUIYqgIVVHJh",
            "ZGllLkFuYWx5emVyLlByb3RvYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Tradie.Analyzer.Proto.ItemPrice), global::Tradie.Analyzer.Proto.ItemPrice.Parser, new[]{ "Currency", "Amount", "BuyoutKind" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Tradie.Analyzer.Proto.ItemListingProperties), global::Tradie.Analyzer.Proto.ItemListingProperties.Parser, new[]{ "X", "Y", "Price", "Note" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class ItemPrice : pb::IMessage<ItemPrice>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ItemPrice> _parser = new pb::MessageParser<ItemPrice>(() => new ItemPrice());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ItemPrice> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Tradie.Analyzer.Proto.ItemListingPropertiesReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ItemPrice() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ItemPrice(ItemPrice other) : this() {
      currency_ = other.currency_;
      amount_ = other.amount_;
      buyoutKind_ = other.buyoutKind_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ItemPrice Clone() {
      return new ItemPrice(this);
    }

    /// <summary>Field number for the "currency" field.</summary>
    public const int CurrencyFieldNumber = 1;
    private uint currency_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint Currency {
      get { return currency_; }
      set {
        currency_ = value;
      }
    }

    /// <summary>Field number for the "amount" field.</summary>
    public const int AmountFieldNumber = 2;
    private float amount_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float Amount {
      get { return amount_; }
      set {
        amount_ = value;
      }
    }

    /// <summary>Field number for the "buyoutKind" field.</summary>
    public const int BuyoutKindFieldNumber = 3;
    private uint buyoutKind_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint BuyoutKind {
      get { return buyoutKind_; }
      set {
        buyoutKind_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ItemPrice);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ItemPrice other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Currency != other.Currency) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Amount, other.Amount)) return false;
      if (BuyoutKind != other.BuyoutKind) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (Currency != 0) hash ^= Currency.GetHashCode();
      if (Amount != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Amount);
      if (BuyoutKind != 0) hash ^= BuyoutKind.GetHashCode();
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
      if (Currency != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Currency);
      }
      if (Amount != 0F) {
        output.WriteRawTag(21);
        output.WriteFloat(Amount);
      }
      if (BuyoutKind != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(BuyoutKind);
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
      if (Currency != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Currency);
      }
      if (Amount != 0F) {
        output.WriteRawTag(21);
        output.WriteFloat(Amount);
      }
      if (BuyoutKind != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(BuyoutKind);
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
      if (Currency != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Currency);
      }
      if (Amount != 0F) {
        size += 1 + 4;
      }
      if (BuyoutKind != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(BuyoutKind);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ItemPrice other) {
      if (other == null) {
        return;
      }
      if (other.Currency != 0) {
        Currency = other.Currency;
      }
      if (other.Amount != 0F) {
        Amount = other.Amount;
      }
      if (other.BuyoutKind != 0) {
        BuyoutKind = other.BuyoutKind;
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
            Currency = input.ReadUInt32();
            break;
          }
          case 21: {
            Amount = input.ReadFloat();
            break;
          }
          case 24: {
            BuyoutKind = input.ReadUInt32();
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
            Currency = input.ReadUInt32();
            break;
          }
          case 21: {
            Amount = input.ReadFloat();
            break;
          }
          case 24: {
            BuyoutKind = input.ReadUInt32();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class ItemListingProperties : pb::IMessage<ItemListingProperties>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ItemListingProperties> _parser = new pb::MessageParser<ItemListingProperties>(() => new ItemListingProperties());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ItemListingProperties> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Tradie.Analyzer.Proto.ItemListingPropertiesReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ItemListingProperties() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ItemListingProperties(ItemListingProperties other) : this() {
      x_ = other.x_;
      y_ = other.y_;
      price_ = other.price_ != null ? other.price_.Clone() : null;
      note_ = other.note_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ItemListingProperties Clone() {
      return new ItemListingProperties(this);
    }

    /// <summary>Field number for the "x" field.</summary>
    public const int XFieldNumber = 1;
    private uint x_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint X {
      get { return x_; }
      set {
        x_ = value;
      }
    }

    /// <summary>Field number for the "y" field.</summary>
    public const int YFieldNumber = 2;
    private uint y_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint Y {
      get { return y_; }
      set {
        y_ = value;
      }
    }

    /// <summary>Field number for the "price" field.</summary>
    public const int PriceFieldNumber = 3;
    private global::Tradie.Analyzer.Proto.ItemPrice price_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Tradie.Analyzer.Proto.ItemPrice Price {
      get { return price_; }
      set {
        price_ = value;
      }
    }

    /// <summary>Field number for the "note" field.</summary>
    public const int NoteFieldNumber = 4;
    private string note_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Note {
      get { return note_; }
      set {
        note_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ItemListingProperties);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ItemListingProperties other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (X != other.X) return false;
      if (Y != other.Y) return false;
      if (!object.Equals(Price, other.Price)) return false;
      if (Note != other.Note) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (X != 0) hash ^= X.GetHashCode();
      if (Y != 0) hash ^= Y.GetHashCode();
      if (price_ != null) hash ^= Price.GetHashCode();
      if (Note.Length != 0) hash ^= Note.GetHashCode();
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
      if (X != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(X);
      }
      if (Y != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Y);
      }
      if (price_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Price);
      }
      if (Note.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(Note);
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
      if (X != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(X);
      }
      if (Y != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Y);
      }
      if (price_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Price);
      }
      if (Note.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(Note);
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
      if (X != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(X);
      }
      if (Y != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Y);
      }
      if (price_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Price);
      }
      if (Note.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Note);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ItemListingProperties other) {
      if (other == null) {
        return;
      }
      if (other.X != 0) {
        X = other.X;
      }
      if (other.Y != 0) {
        Y = other.Y;
      }
      if (other.price_ != null) {
        if (price_ == null) {
          Price = new global::Tradie.Analyzer.Proto.ItemPrice();
        }
        Price.MergeFrom(other.Price);
      }
      if (other.Note.Length != 0) {
        Note = other.Note;
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
            X = input.ReadUInt32();
            break;
          }
          case 16: {
            Y = input.ReadUInt32();
            break;
          }
          case 26: {
            if (price_ == null) {
              Price = new global::Tradie.Analyzer.Proto.ItemPrice();
            }
            input.ReadMessage(Price);
            break;
          }
          case 34: {
            Note = input.ReadString();
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
            X = input.ReadUInt32();
            break;
          }
          case 16: {
            Y = input.ReadUInt32();
            break;
          }
          case 26: {
            if (price_ == null) {
              Price = new global::Tradie.Analyzer.Proto.ItemPrice();
            }
            input.ReadMessage(Price);
            break;
          }
          case 34: {
            Note = input.ReadString();
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
