// source: Models/Indexer/Proto/Criteria.proto
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

goog.exportSymbol('proto.Criteria', null, global);
goog.exportSymbol('proto.Criteria.UnderlyingCase', null, global);
goog.exportSymbol('proto.CriteriaKind', null, global);
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
proto.Criteria = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, proto.Criteria.oneofGroups_);
};
goog.inherits(proto.Criteria, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.Criteria.displayName = 'proto.Criteria';
}

/**
 * Oneof group definitions for this message. Each group defines the field
 * numbers belonging to that group. When of these fields' value is set, all
 * other fields in the group are cleared. During deserialization, if multiple
 * fields are encountered for a group, only the last value seen will be kept.
 * @private {!Array<!Array<number>>}
 * @const
 */
proto.Criteria.oneofGroups_ = [[10,11,12,13,14]];

/**
 * @enum {number}
 */
proto.Criteria.UnderlyingCase = {
  UNDERLYING_NOT_SET: 0,
  MODIFIERHASH: 10,
  LEAGUE: 11,
  CATEGORY: 12,
  SUBCATEGORY: 13,
  ITEMTYPEID: 14
};

/**
 * @return {proto.Criteria.UnderlyingCase}
 */
proto.Criteria.prototype.getUnderlyingCase = function() {
  return /** @type {proto.Criteria.UnderlyingCase} */(jspb.Message.computeOneofCase(this, proto.Criteria.oneofGroups_[0]));
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
proto.Criteria.prototype.toObject = function(opt_includeInstance) {
  return proto.Criteria.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.Criteria} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.Criteria.toObject = function(includeInstance, msg) {
  var f, obj = {
    id: jspb.Message.getFieldWithDefault(msg, 1, ""),
    name: jspb.Message.getFieldWithDefault(msg, 2, ""),
    kind: jspb.Message.getFieldWithDefault(msg, 3, 0),
    modifierhash: jspb.Message.getFieldWithDefault(msg, 10, "0"),
    league: jspb.Message.getFieldWithDefault(msg, 11, ""),
    category: jspb.Message.getFieldWithDefault(msg, 12, ""),
    subcategory: jspb.Message.getFieldWithDefault(msg, 13, ""),
    itemtypeid: jspb.Message.getFieldWithDefault(msg, 14, 0)
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
 * @return {!proto.Criteria}
 */
proto.Criteria.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.Criteria;
  return proto.Criteria.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.Criteria} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.Criteria}
 */
proto.Criteria.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {string} */ (reader.readString());
      msg.setId(value);
      break;
    case 2:
      var value = /** @type {string} */ (reader.readString());
      msg.setName(value);
      break;
    case 3:
      var value = /** @type {!proto.CriteriaKind} */ (reader.readEnum());
      msg.setKind(value);
      break;
    case 10:
      var value = /** @type {string} */ (reader.readInt64String());
      msg.setModifierhash(value);
      break;
    case 11:
      var value = /** @type {string} */ (reader.readString());
      msg.setLeague(value);
      break;
    case 12:
      var value = /** @type {string} */ (reader.readString());
      msg.setCategory(value);
      break;
    case 13:
      var value = /** @type {string} */ (reader.readString());
      msg.setSubcategory(value);
      break;
    case 14:
      var value = /** @type {number} */ (reader.readInt32());
      msg.setItemtypeid(value);
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
proto.Criteria.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.Criteria.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.Criteria} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.Criteria.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getId();
  if (f.length > 0) {
    writer.writeString(
      1,
      f
    );
  }
  f = message.getName();
  if (f.length > 0) {
    writer.writeString(
      2,
      f
    );
  }
  f = message.getKind();
  if (f !== 0.0) {
    writer.writeEnum(
      3,
      f
    );
  }
  f = /** @type {string} */ (jspb.Message.getField(message, 10));
  if (f != null) {
    writer.writeInt64String(
      10,
      f
    );
  }
  f = /** @type {string} */ (jspb.Message.getField(message, 11));
  if (f != null) {
    writer.writeString(
      11,
      f
    );
  }
  f = /** @type {string} */ (jspb.Message.getField(message, 12));
  if (f != null) {
    writer.writeString(
      12,
      f
    );
  }
  f = /** @type {string} */ (jspb.Message.getField(message, 13));
  if (f != null) {
    writer.writeString(
      13,
      f
    );
  }
  f = /** @type {number} */ (jspb.Message.getField(message, 14));
  if (f != null) {
    writer.writeInt32(
      14,
      f
    );
  }
};


/**
 * optional string id = 1;
 * @return {string}
 */
proto.Criteria.prototype.getId = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 1, ""));
};


/**
 * @param {string} value
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.setId = function(value) {
  return jspb.Message.setProto3StringField(this, 1, value);
};


/**
 * optional string name = 2;
 * @return {string}
 */
proto.Criteria.prototype.getName = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 2, ""));
};


/**
 * @param {string} value
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.setName = function(value) {
  return jspb.Message.setProto3StringField(this, 2, value);
};


/**
 * optional CriteriaKind kind = 3;
 * @return {!proto.CriteriaKind}
 */
proto.Criteria.prototype.getKind = function() {
  return /** @type {!proto.CriteriaKind} */ (jspb.Message.getFieldWithDefault(this, 3, 0));
};


/**
 * @param {!proto.CriteriaKind} value
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.setKind = function(value) {
  return jspb.Message.setProto3EnumField(this, 3, value);
};


/**
 * optional int64 modifierHash = 10;
 * @return {string}
 */
proto.Criteria.prototype.getModifierhash = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 10, "0"));
};


/**
 * @param {string} value
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.setModifierhash = function(value) {
  return jspb.Message.setOneofField(this, 10, proto.Criteria.oneofGroups_[0], value);
};


/**
 * Clears the field making it undefined.
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.clearModifierhash = function() {
  return jspb.Message.setOneofField(this, 10, proto.Criteria.oneofGroups_[0], undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.Criteria.prototype.hasModifierhash = function() {
  return jspb.Message.getField(this, 10) != null;
};


/**
 * optional string league = 11;
 * @return {string}
 */
proto.Criteria.prototype.getLeague = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 11, ""));
};


/**
 * @param {string} value
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.setLeague = function(value) {
  return jspb.Message.setOneofField(this, 11, proto.Criteria.oneofGroups_[0], value);
};


/**
 * Clears the field making it undefined.
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.clearLeague = function() {
  return jspb.Message.setOneofField(this, 11, proto.Criteria.oneofGroups_[0], undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.Criteria.prototype.hasLeague = function() {
  return jspb.Message.getField(this, 11) != null;
};


/**
 * optional string category = 12;
 * @return {string}
 */
proto.Criteria.prototype.getCategory = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 12, ""));
};


/**
 * @param {string} value
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.setCategory = function(value) {
  return jspb.Message.setOneofField(this, 12, proto.Criteria.oneofGroups_[0], value);
};


/**
 * Clears the field making it undefined.
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.clearCategory = function() {
  return jspb.Message.setOneofField(this, 12, proto.Criteria.oneofGroups_[0], undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.Criteria.prototype.hasCategory = function() {
  return jspb.Message.getField(this, 12) != null;
};


/**
 * optional string subcategory = 13;
 * @return {string}
 */
proto.Criteria.prototype.getSubcategory = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 13, ""));
};


/**
 * @param {string} value
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.setSubcategory = function(value) {
  return jspb.Message.setOneofField(this, 13, proto.Criteria.oneofGroups_[0], value);
};


/**
 * Clears the field making it undefined.
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.clearSubcategory = function() {
  return jspb.Message.setOneofField(this, 13, proto.Criteria.oneofGroups_[0], undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.Criteria.prototype.hasSubcategory = function() {
  return jspb.Message.getField(this, 13) != null;
};


/**
 * optional int32 itemTypeId = 14;
 * @return {number}
 */
proto.Criteria.prototype.getItemtypeid = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 14, 0));
};


/**
 * @param {number} value
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.setItemtypeid = function(value) {
  return jspb.Message.setOneofField(this, 14, proto.Criteria.oneofGroups_[0], value);
};


/**
 * Clears the field making it undefined.
 * @return {!proto.Criteria} returns this
 */
proto.Criteria.prototype.clearItemtypeid = function() {
  return jspb.Message.setOneofField(this, 14, proto.Criteria.oneofGroups_[0], undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.Criteria.prototype.hasItemtypeid = function() {
  return jspb.Message.getField(this, 14) != null;
};


/**
 * @enum {number}
 */
proto.CriteriaKind = {
  UNKNOWN: 0,
  MODIFIER: 1,
  LEAGUE: 2,
  CATEGORY: 3,
  SUBCATEGORY: 4,
  ITEM_TYPE: 5,
  UNIQUE: 6
};

goog.object.extend(exports, proto);
