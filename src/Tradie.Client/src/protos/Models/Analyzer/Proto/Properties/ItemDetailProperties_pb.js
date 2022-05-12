// source: Models/Analyzer/Proto/Properties/ItemDetailProperties.proto
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

var Models_Analyzer_Proto_Requirements_pb = require('../../../../Models/Analyzer/Proto/Requirements_pb.js');
goog.object.extend(proto, Models_Analyzer_Proto_Requirements_pb);
goog.exportSymbol('proto.ItemDetailProperties', null, global);
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
proto.ItemDetailProperties = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.ItemDetailProperties, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.ItemDetailProperties.displayName = 'proto.ItemDetailProperties';
}



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
proto.ItemDetailProperties.prototype.toObject = function(opt_includeInstance) {
  return proto.ItemDetailProperties.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.ItemDetailProperties} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.ItemDetailProperties.toObject = function(includeInstance, msg) {
  var f, obj = {
    name: jspb.Message.getFieldWithDefault(msg, 1, ""),
    flags: jspb.Message.getFieldWithDefault(msg, 2, 0),
    influences: jspb.Message.getFieldWithDefault(msg, 3, 0),
    itemlevel: jspb.Message.getFieldWithDefault(msg, 4, 0),
    iconpath: jspb.Message.getFieldWithDefault(msg, 5, ""),
    requirements: (f = msg.getRequirements()) && Models_Analyzer_Proto_Requirements_pb.Requirements.toObject(includeInstance, f)
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
 * @return {!proto.ItemDetailProperties}
 */
proto.ItemDetailProperties.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.ItemDetailProperties;
  return proto.ItemDetailProperties.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.ItemDetailProperties} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.ItemDetailProperties}
 */
proto.ItemDetailProperties.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {string} */ (reader.readString());
      msg.setName(value);
      break;
    case 2:
      var value = /** @type {number} */ (reader.readUint32());
      msg.setFlags(value);
      break;
    case 3:
      var value = /** @type {number} */ (reader.readUint32());
      msg.setInfluences(value);
      break;
    case 4:
      var value = /** @type {number} */ (reader.readUint32());
      msg.setItemlevel(value);
      break;
    case 5:
      var value = /** @type {string} */ (reader.readString());
      msg.setIconpath(value);
      break;
    case 6:
      var value = new Models_Analyzer_Proto_Requirements_pb.Requirements;
      reader.readMessage(value,Models_Analyzer_Proto_Requirements_pb.Requirements.deserializeBinaryFromReader);
      msg.setRequirements(value);
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
proto.ItemDetailProperties.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.ItemDetailProperties.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.ItemDetailProperties} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.ItemDetailProperties.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getName();
  if (f.length > 0) {
    writer.writeString(
      1,
      f
    );
  }
  f = message.getFlags();
  if (f !== 0) {
    writer.writeUint32(
      2,
      f
    );
  }
  f = message.getInfluences();
  if (f !== 0) {
    writer.writeUint32(
      3,
      f
    );
  }
  f = message.getItemlevel();
  if (f !== 0) {
    writer.writeUint32(
      4,
      f
    );
  }
  f = message.getIconpath();
  if (f.length > 0) {
    writer.writeString(
      5,
      f
    );
  }
  f = message.getRequirements();
  if (f != null) {
    writer.writeMessage(
      6,
      f,
      Models_Analyzer_Proto_Requirements_pb.Requirements.serializeBinaryToWriter
    );
  }
};


/**
 * optional string name = 1;
 * @return {string}
 */
proto.ItemDetailProperties.prototype.getName = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 1, ""));
};


/**
 * @param {string} value
 * @return {!proto.ItemDetailProperties} returns this
 */
proto.ItemDetailProperties.prototype.setName = function(value) {
  return jspb.Message.setProto3StringField(this, 1, value);
};


/**
 * optional uint32 flags = 2;
 * @return {number}
 */
proto.ItemDetailProperties.prototype.getFlags = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 2, 0));
};


/**
 * @param {number} value
 * @return {!proto.ItemDetailProperties} returns this
 */
proto.ItemDetailProperties.prototype.setFlags = function(value) {
  return jspb.Message.setProto3IntField(this, 2, value);
};


/**
 * optional uint32 influences = 3;
 * @return {number}
 */
proto.ItemDetailProperties.prototype.getInfluences = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 3, 0));
};


/**
 * @param {number} value
 * @return {!proto.ItemDetailProperties} returns this
 */
proto.ItemDetailProperties.prototype.setInfluences = function(value) {
  return jspb.Message.setProto3IntField(this, 3, value);
};


/**
 * optional uint32 itemLevel = 4;
 * @return {number}
 */
proto.ItemDetailProperties.prototype.getItemlevel = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 4, 0));
};


/**
 * @param {number} value
 * @return {!proto.ItemDetailProperties} returns this
 */
proto.ItemDetailProperties.prototype.setItemlevel = function(value) {
  return jspb.Message.setProto3IntField(this, 4, value);
};


/**
 * optional string iconPath = 5;
 * @return {string}
 */
proto.ItemDetailProperties.prototype.getIconpath = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 5, ""));
};


/**
 * @param {string} value
 * @return {!proto.ItemDetailProperties} returns this
 */
proto.ItemDetailProperties.prototype.setIconpath = function(value) {
  return jspb.Message.setProto3StringField(this, 5, value);
};


/**
 * optional Requirements requirements = 6;
 * @return {?proto.Requirements}
 */
proto.ItemDetailProperties.prototype.getRequirements = function() {
  return /** @type{?proto.Requirements} */ (
    jspb.Message.getWrapperField(this, Models_Analyzer_Proto_Requirements_pb.Requirements, 6));
};


/**
 * @param {?proto.Requirements|undefined} value
 * @return {!proto.ItemDetailProperties} returns this
*/
proto.ItemDetailProperties.prototype.setRequirements = function(value) {
  return jspb.Message.setWrapperField(this, 6, value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.ItemDetailProperties} returns this
 */
proto.ItemDetailProperties.prototype.clearRequirements = function() {
  return this.setRequirements(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.ItemDetailProperties.prototype.hasRequirements = function() {
  return jspb.Message.getField(this, 6) != null;
};


goog.object.extend(exports, proto);
