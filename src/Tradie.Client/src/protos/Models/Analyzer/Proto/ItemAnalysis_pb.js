// source: Models/Analyzer/Proto/ItemAnalysis.proto
/**
 * @fileoverview
 * @enhanceable
 * @suppress {missingRequire} reports error on implicit type usages.
 * @suppress {messageConventions} JS Compiler reports an error if a variable or
 *     field starts with 'MSG_' and isn't a translatable message.
 * @public
 */
// GENERATED CODE -- DO NOT EDIT!
/* eslint-disable */
// @ts-nocheck

var jspb = require('google-protobuf');
var goog = jspb;
var global = (function() {
  if (this) { return this; }
  if (typeof window !== 'undefined') { return window; }
  if (typeof global !== 'undefined') { return global; }
  if (typeof self !== 'undefined') { return self; }
  return Function('return this')();
}.call(null));

var Models_Analyzer_Proto_Properties_ItemDetailProperties_pb = require('../../../Models/Analyzer/Proto/Properties/ItemDetailProperties_pb.js');
goog.object.extend(proto, Models_Analyzer_Proto_Properties_ItemDetailProperties_pb);
var Models_Analyzer_Proto_Properties_ItemAffixProperties_pb = require('../../../Models/Analyzer/Proto/Properties/ItemAffixProperties_pb.js');
goog.object.extend(proto, Models_Analyzer_Proto_Properties_ItemAffixProperties_pb);
var Models_Analyzer_Proto_Properties_ItemListingProperties_pb = require('../../../Models/Analyzer/Proto/Properties/ItemListingProperties_pb.js');
goog.object.extend(proto, Models_Analyzer_Proto_Properties_ItemListingProperties_pb);
var Models_Analyzer_Proto_Properties_ItemTypeProperties_pb = require('../../../Models/Analyzer/Proto/Properties/ItemTypeProperties_pb.js');
goog.object.extend(proto, Models_Analyzer_Proto_Properties_ItemTypeProperties_pb);
goog.exportSymbol('proto.ItemAnalysis', null, global);
goog.exportSymbol('proto.ItemAnalysis.AnalyzerCase', null, global);
/**
 * Generated by JsPbCodeGenerator.
 * @param {Array=} opt_data Optional initial data array, typically from a
 * server response, or constructed directly in Javascript. The array is used
 * in place and becomes part of the constructed object. It is not cloned.
 * If no data is provided, the constructed object will be empty, but still
 * valid.
 * @extends {jspb.Message}
 * @constructor
 */
proto.ItemAnalysis = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, proto.ItemAnalysis.oneofGroups_);
};
goog.inherits(proto.ItemAnalysis, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.ItemAnalysis.displayName = 'proto.ItemAnalysis';
}

/**
 * Oneof group definitions for this message. Each group defines the field
 * numbers belonging to that group. When of these fields' value is set, all
 * other fields in the group are cleared. During deserialization, if multiple
 * fields are encountered for a group, only the last value seen will be kept.
 * @private {!Array<!Array<number>>}
 * @const
 */
proto.ItemAnalysis.oneofGroups_ = [[2,3,4,5]];

/**
 * @enum {number}
 */
proto.ItemAnalysis.AnalyzerCase = {
  ANALYZER_NOT_SET: 0,
  BASICPROPERTIES: 2,
  AFFIXPROPERTIES: 3,
  TRADEPROPERTIES: 4,
  TYPEPROPERTIES: 5
};

/**
 * @return {proto.ItemAnalysis.AnalyzerCase}
 */
proto.ItemAnalysis.prototype.getAnalyzerCase = function() {
  return /** @type {proto.ItemAnalysis.AnalyzerCase} */(jspb.Message.computeOneofCase(this, proto.ItemAnalysis.oneofGroups_[0]));
};



if (jspb.Message.GENERATE_TO_OBJECT) {
/**
 * Creates an object representation of this proto.
 * Field names that are reserved in JavaScript and will be renamed to pb_name.
 * Optional fields that are not set will be set to undefined.
 * To access a reserved field use, foo.pb_<name>, eg, foo.pb_default.
 * For the list of reserved names please see:
 *     net/proto2/compiler/js/internal/generator.cc#kKeyword.
 * @param {boolean=} opt_includeInstance Deprecated. whether to include the
 *     JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @return {!Object}
 */
proto.ItemAnalysis.prototype.toObject = function(opt_includeInstance) {
  return proto.ItemAnalysis.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.ItemAnalysis} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.ItemAnalysis.toObject = function(includeInstance, msg) {
  var f, obj = {
    analyzerid: jspb.Message.getFieldWithDefault(msg, 1, 0),
    basicproperties: (f = msg.getBasicproperties()) && Models_Analyzer_Proto_Properties_ItemDetailProperties_pb.ItemDetailProperties.toObject(includeInstance, f),
    affixproperties: (f = msg.getAffixproperties()) && Models_Analyzer_Proto_Properties_ItemAffixProperties_pb.ItemAffixProperties.toObject(includeInstance, f),
    tradeproperties: (f = msg.getTradeproperties()) && Models_Analyzer_Proto_Properties_ItemListingProperties_pb.ItemListingProperties.toObject(includeInstance, f),
    typeproperties: (f = msg.getTypeproperties()) && Models_Analyzer_Proto_Properties_ItemTypeProperties_pb.ItemTypeProperties.toObject(includeInstance, f)
  };

  if (includeInstance) {
    obj.$jspbMessageInstance = msg;
  }
  return obj;
};
}


/**
 * Deserializes binary data (in protobuf wire format).
 * @param {jspb.ByteSource} bytes The bytes to deserialize.
 * @return {!proto.ItemAnalysis}
 */
proto.ItemAnalysis.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.ItemAnalysis;
  return proto.ItemAnalysis.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.ItemAnalysis} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.ItemAnalysis}
 */
proto.ItemAnalysis.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {number} */ (reader.readInt32());
      msg.setAnalyzerid(value);
      break;
    case 2:
      var value = new Models_Analyzer_Proto_Properties_ItemDetailProperties_pb.ItemDetailProperties;
      reader.readMessage(value,Models_Analyzer_Proto_Properties_ItemDetailProperties_pb.ItemDetailProperties.deserializeBinaryFromReader);
      msg.setBasicproperties(value);
      break;
    case 3:
      var value = new Models_Analyzer_Proto_Properties_ItemAffixProperties_pb.ItemAffixProperties;
      reader.readMessage(value,Models_Analyzer_Proto_Properties_ItemAffixProperties_pb.ItemAffixProperties.deserializeBinaryFromReader);
      msg.setAffixproperties(value);
      break;
    case 4:
      var value = new Models_Analyzer_Proto_Properties_ItemListingProperties_pb.ItemListingProperties;
      reader.readMessage(value,Models_Analyzer_Proto_Properties_ItemListingProperties_pb.ItemListingProperties.deserializeBinaryFromReader);
      msg.setTradeproperties(value);
      break;
    case 5:
      var value = new Models_Analyzer_Proto_Properties_ItemTypeProperties_pb.ItemTypeProperties;
      reader.readMessage(value,Models_Analyzer_Proto_Properties_ItemTypeProperties_pb.ItemTypeProperties.deserializeBinaryFromReader);
      msg.setTypeproperties(value);
      break;
    default:
      reader.skipField();
      break;
    }
  }
  return msg;
};


/**
 * Serializes the message to binary data (in protobuf wire format).
 * @return {!Uint8Array}
 */
proto.ItemAnalysis.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.ItemAnalysis.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.ItemAnalysis} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.ItemAnalysis.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getAnalyzerid();
  if (f !== 0) {
    writer.writeInt32(
      1,
      f
    );
  }
  f = message.getBasicproperties();
  if (f != null) {
    writer.writeMessage(
      2,
      f,
      Models_Analyzer_Proto_Properties_ItemDetailProperties_pb.ItemDetailProperties.serializeBinaryToWriter
    );
  }
  f = message.getAffixproperties();
  if (f != null) {
    writer.writeMessage(
      3,
      f,
      Models_Analyzer_Proto_Properties_ItemAffixProperties_pb.ItemAffixProperties.serializeBinaryToWriter
    );
  }
  f = message.getTradeproperties();
  if (f != null) {
    writer.writeMessage(
      4,
      f,
      Models_Analyzer_Proto_Properties_ItemListingProperties_pb.ItemListingProperties.serializeBinaryToWriter
    );
  }
  f = message.getTypeproperties();
  if (f != null) {
    writer.writeMessage(
      5,
      f,
      Models_Analyzer_Proto_Properties_ItemTypeProperties_pb.ItemTypeProperties.serializeBinaryToWriter
    );
  }
};


/**
 * optional int32 analyzerId = 1;
 * @return {number}
 */
proto.ItemAnalysis.prototype.getAnalyzerid = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 1, 0));
};


/**
 * @param {number} value
 * @return {!proto.ItemAnalysis} returns this
 */
proto.ItemAnalysis.prototype.setAnalyzerid = function(value) {
  return jspb.Message.setProto3IntField(this, 1, value);
};


/**
 * optional ItemDetailProperties basicProperties = 2;
 * @return {?proto.ItemDetailProperties}
 */
proto.ItemAnalysis.prototype.getBasicproperties = function() {
  return /** @type{?proto.ItemDetailProperties} */ (
    jspb.Message.getWrapperField(this, Models_Analyzer_Proto_Properties_ItemDetailProperties_pb.ItemDetailProperties, 2));
};


/**
 * @param {?proto.ItemDetailProperties|undefined} value
 * @return {!proto.ItemAnalysis} returns this
*/
proto.ItemAnalysis.prototype.setBasicproperties = function(value) {
  return jspb.Message.setOneofWrapperField(this, 2, proto.ItemAnalysis.oneofGroups_[0], value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.ItemAnalysis} returns this
 */
proto.ItemAnalysis.prototype.clearBasicproperties = function() {
  return this.setBasicproperties(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.ItemAnalysis.prototype.hasBasicproperties = function() {
  return jspb.Message.getField(this, 2) != null;
};


/**
 * optional ItemAffixProperties affixProperties = 3;
 * @return {?proto.ItemAffixProperties}
 */
proto.ItemAnalysis.prototype.getAffixproperties = function() {
  return /** @type{?proto.ItemAffixProperties} */ (
    jspb.Message.getWrapperField(this, Models_Analyzer_Proto_Properties_ItemAffixProperties_pb.ItemAffixProperties, 3));
};


/**
 * @param {?proto.ItemAffixProperties|undefined} value
 * @return {!proto.ItemAnalysis} returns this
*/
proto.ItemAnalysis.prototype.setAffixproperties = function(value) {
  return jspb.Message.setOneofWrapperField(this, 3, proto.ItemAnalysis.oneofGroups_[0], value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.ItemAnalysis} returns this
 */
proto.ItemAnalysis.prototype.clearAffixproperties = function() {
  return this.setAffixproperties(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.ItemAnalysis.prototype.hasAffixproperties = function() {
  return jspb.Message.getField(this, 3) != null;
};


/**
 * optional ItemListingProperties tradeProperties = 4;
 * @return {?proto.ItemListingProperties}
 */
proto.ItemAnalysis.prototype.getTradeproperties = function() {
  return /** @type{?proto.ItemListingProperties} */ (
    jspb.Message.getWrapperField(this, Models_Analyzer_Proto_Properties_ItemListingProperties_pb.ItemListingProperties, 4));
};


/**
 * @param {?proto.ItemListingProperties|undefined} value
 * @return {!proto.ItemAnalysis} returns this
*/
proto.ItemAnalysis.prototype.setTradeproperties = function(value) {
  return jspb.Message.setOneofWrapperField(this, 4, proto.ItemAnalysis.oneofGroups_[0], value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.ItemAnalysis} returns this
 */
proto.ItemAnalysis.prototype.clearTradeproperties = function() {
  return this.setTradeproperties(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.ItemAnalysis.prototype.hasTradeproperties = function() {
  return jspb.Message.getField(this, 4) != null;
};


/**
 * optional ItemTypeProperties typeProperties = 5;
 * @return {?proto.ItemTypeProperties}
 */
proto.ItemAnalysis.prototype.getTypeproperties = function() {
  return /** @type{?proto.ItemTypeProperties} */ (
    jspb.Message.getWrapperField(this, Models_Analyzer_Proto_Properties_ItemTypeProperties_pb.ItemTypeProperties, 5));
};


/**
 * @param {?proto.ItemTypeProperties|undefined} value
 * @return {!proto.ItemAnalysis} returns this
*/
proto.ItemAnalysis.prototype.setTypeproperties = function(value) {
  return jspb.Message.setOneofWrapperField(this, 5, proto.ItemAnalysis.oneofGroups_[0], value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.ItemAnalysis} returns this
 */
proto.ItemAnalysis.prototype.clearTypeproperties = function() {
  return this.setTypeproperties(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.ItemAnalysis.prototype.hasTypeproperties = function() {
  return jspb.Message.getField(this, 5) != null;
};


goog.object.extend(exports, proto);
