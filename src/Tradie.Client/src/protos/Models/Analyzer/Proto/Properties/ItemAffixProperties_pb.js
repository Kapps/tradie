// source: Models/Analyzer/Proto/Properties/ItemAffixProperties.proto
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

var Models_Analyzer_Proto_ModifierTypes_pb = require('../../../../Models/Analyzer/Proto/ModifierTypes_pb.js');
goog.object.extend(proto, Models_Analyzer_Proto_ModifierTypes_pb);
goog.exportSymbol('proto.Affix', null, global);
goog.exportSymbol('proto.ItemAffixProperties', null, global);
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
proto.Affix = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.Affix, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.Affix.displayName = 'proto.Affix';
}
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
proto.ItemAffixProperties = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, proto.ItemAffixProperties.repeatedFields_, null);
};
goog.inherits(proto.ItemAffixProperties, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.ItemAffixProperties.displayName = 'proto.ItemAffixProperties';
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
proto.Affix.prototype.toObject = function(opt_includeInstance) {
  return proto.Affix.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.Affix} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.Affix.toObject = function(includeInstance, msg) {
  var f, obj = {
    key: (f = msg.getKey()) && Models_Analyzer_Proto_ModifierTypes_pb.ModKey.toObject(includeInstance, f),
    value: jspb.Message.getFloatingPointFieldWithDefault(msg, 2, 0.0)
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
 * @return {!proto.Affix}
 */
proto.Affix.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.Affix;
  return proto.Affix.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.Affix} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.Affix}
 */
proto.Affix.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = new Models_Analyzer_Proto_ModifierTypes_pb.ModKey;
      reader.readMessage(value,Models_Analyzer_Proto_ModifierTypes_pb.ModKey.deserializeBinaryFromReader);
      msg.setKey(value);
      break;
    case 2:
      var value = /** @type {number} */ (reader.readFloat());
      msg.setValue(value);
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
proto.Affix.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.Affix.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.Affix} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.Affix.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getKey();
  if (f != null) {
    writer.writeMessage(
      1,
      f,
      Models_Analyzer_Proto_ModifierTypes_pb.ModKey.serializeBinaryToWriter
    );
  }
  f = message.getValue();
  if (f !== 0.0) {
    writer.writeFloat(
      2,
      f
    );
  }
};


/**
 * optional ModKey key = 1;
 * @return {?proto.ModKey}
 */
proto.Affix.prototype.getKey = function() {
  return /** @type{?proto.ModKey} */ (
    jspb.Message.getWrapperField(this, Models_Analyzer_Proto_ModifierTypes_pb.ModKey, 1));
};


/**
 * @param {?proto.ModKey|undefined} value
 * @return {!proto.Affix} returns this
*/
proto.Affix.prototype.setKey = function(value) {
  return jspb.Message.setWrapperField(this, 1, value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.Affix} returns this
 */
proto.Affix.prototype.clearKey = function() {
  return this.setKey(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.Affix.prototype.hasKey = function() {
  return jspb.Message.getField(this, 1) != null;
};


/**
 * optional float value = 2;
 * @return {number}
 */
proto.Affix.prototype.getValue = function() {
  return /** @type {number} */ (jspb.Message.getFloatingPointFieldWithDefault(this, 2, 0.0));
};


/**
 * @param {number} value
 * @return {!proto.Affix} returns this
 */
proto.Affix.prototype.setValue = function(value) {
  return jspb.Message.setProto3FloatField(this, 2, value);
};



/**
 * List of repeated fields within this message type.
 * @private {!Array<number>}
 * @const
 */
proto.ItemAffixProperties.repeatedFields_ = [1];



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
proto.ItemAffixProperties.prototype.toObject = function(opt_includeInstance) {
  return proto.ItemAffixProperties.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.ItemAffixProperties} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.ItemAffixProperties.toObject = function(includeInstance, msg) {
  var f, obj = {
    affixesList: jspb.Message.toObjectList(msg.getAffixesList(),
    proto.Affix.toObject, includeInstance)
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
 * @return {!proto.ItemAffixProperties}
 */
proto.ItemAffixProperties.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.ItemAffixProperties;
  return proto.ItemAffixProperties.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.ItemAffixProperties} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.ItemAffixProperties}
 */
proto.ItemAffixProperties.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = new proto.Affix;
      reader.readMessage(value,proto.Affix.deserializeBinaryFromReader);
      msg.addAffixes(value);
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
proto.ItemAffixProperties.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.ItemAffixProperties.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.ItemAffixProperties} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.ItemAffixProperties.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getAffixesList();
  if (f.length > 0) {
    writer.writeRepeatedMessage(
      1,
      f,
      proto.Affix.serializeBinaryToWriter
    );
  }
};


/**
 * repeated Affix affixes = 1;
 * @return {!Array<!proto.Affix>}
 */
proto.ItemAffixProperties.prototype.getAffixesList = function() {
  return /** @type{!Array<!proto.Affix>} */ (
    jspb.Message.getRepeatedWrapperField(this, proto.Affix, 1));
};


/**
 * @param {!Array<!proto.Affix>} value
 * @return {!proto.ItemAffixProperties} returns this
*/
proto.ItemAffixProperties.prototype.setAffixesList = function(value) {
  return jspb.Message.setRepeatedWrapperField(this, 1, value);
};


/**
 * @param {!proto.Affix=} opt_value
 * @param {number=} opt_index
 * @return {!proto.Affix}
 */
proto.ItemAffixProperties.prototype.addAffixes = function(opt_value, opt_index) {
  return jspb.Message.addToRepeatedWrapperField(this, 1, opt_value, proto.Affix, opt_index);
};


/**
 * Clears the list making it empty but non-null.
 * @return {!proto.ItemAffixProperties} returns this
 */
proto.ItemAffixProperties.prototype.clearAffixesList = function() {
  return this.setAffixesList([]);
};


goog.object.extend(exports, proto);