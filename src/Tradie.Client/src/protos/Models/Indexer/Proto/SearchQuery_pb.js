// source: Models/Indexer/Proto/SearchQuery.proto
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

var Models_Analyzer_Proto_ModifierTypes_pb = require('../../../Models/Analyzer/Proto/ModifierTypes_pb.js');
goog.object.extend(proto, Models_Analyzer_Proto_ModifierTypes_pb);
goog.exportSymbol('proto.SearchGroup', null, global);
goog.exportSymbol('proto.SearchQuery', null, global);
goog.exportSymbol('proto.SearchRange', null, global);
goog.exportSymbol('proto.SortOrder', null, global);
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
proto.SearchQuery = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, proto.SearchQuery.repeatedFields_, null);
};
goog.inherits(proto.SearchQuery, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.SearchQuery.displayName = 'proto.SearchQuery';
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
proto.SearchGroup = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, proto.SearchGroup.repeatedFields_, null);
};
goog.inherits(proto.SearchGroup, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.SearchGroup.displayName = 'proto.SearchGroup';
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
proto.SortOrder = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.SortOrder, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.SortOrder.displayName = 'proto.SortOrder';
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
proto.SearchRange = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.SearchRange, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.SearchRange.displayName = 'proto.SearchRange';
}

/**
 * List of repeated fields within this message type.
 * @private {!Array<number>}
 * @const
 */
proto.SearchQuery.repeatedFields_ = [1];



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
proto.SearchQuery.prototype.toObject = function(opt_includeInstance) {
  return proto.SearchQuery.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.SearchQuery} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.SearchQuery.toObject = function(includeInstance, msg) {
  var f, obj = {
    groupsList: jspb.Message.toObjectList(msg.getGroupsList(),
    proto.SearchGroup.toObject, includeInstance),
    sort: (f = msg.getSort()) && proto.SortOrder.toObject(includeInstance, f),
    league: jspb.Message.getFieldWithDefault(msg, 3, "")
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
 * @return {!proto.SearchQuery}
 */
proto.SearchQuery.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.SearchQuery;
  return proto.SearchQuery.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.SearchQuery} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.SearchQuery}
 */
proto.SearchQuery.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = new proto.SearchGroup;
      reader.readMessage(value,proto.SearchGroup.deserializeBinaryFromReader);
      msg.addGroups(value);
      break;
    case 2:
      var value = new proto.SortOrder;
      reader.readMessage(value,proto.SortOrder.deserializeBinaryFromReader);
      msg.setSort(value);
      break;
    case 3:
      var value = /** @type {string} */ (reader.readString());
      msg.setLeague(value);
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
proto.SearchQuery.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.SearchQuery.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.SearchQuery} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.SearchQuery.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getGroupsList();
  if (f.length > 0) {
    writer.writeRepeatedMessage(
      1,
      f,
      proto.SearchGroup.serializeBinaryToWriter
    );
  }
  f = message.getSort();
  if (f != null) {
    writer.writeMessage(
      2,
      f,
      proto.SortOrder.serializeBinaryToWriter
    );
  }
  f = message.getLeague();
  if (f.length > 0) {
    writer.writeString(
      3,
      f
    );
  }
};


/**
 * repeated SearchGroup Groups = 1;
 * @return {!Array<!proto.SearchGroup>}
 */
proto.SearchQuery.prototype.getGroupsList = function() {
  return /** @type{!Array<!proto.SearchGroup>} */ (
    jspb.Message.getRepeatedWrapperField(this, proto.SearchGroup, 1));
};


/**
 * @param {!Array<!proto.SearchGroup>} value
 * @return {!proto.SearchQuery} returns this
*/
proto.SearchQuery.prototype.setGroupsList = function(value) {
  return jspb.Message.setRepeatedWrapperField(this, 1, value);
};


/**
 * @param {!proto.SearchGroup=} opt_value
 * @param {number=} opt_index
 * @return {!proto.SearchGroup}
 */
proto.SearchQuery.prototype.addGroups = function(opt_value, opt_index) {
  return jspb.Message.addToRepeatedWrapperField(this, 1, opt_value, proto.SearchGroup, opt_index);
};


/**
 * Clears the list making it empty but non-null.
 * @return {!proto.SearchQuery} returns this
 */
proto.SearchQuery.prototype.clearGroupsList = function() {
  return this.setGroupsList([]);
};


/**
 * optional SortOrder Sort = 2;
 * @return {?proto.SortOrder}
 */
proto.SearchQuery.prototype.getSort = function() {
  return /** @type{?proto.SortOrder} */ (
    jspb.Message.getWrapperField(this, proto.SortOrder, 2));
};


/**
 * @param {?proto.SortOrder|undefined} value
 * @return {!proto.SearchQuery} returns this
*/
proto.SearchQuery.prototype.setSort = function(value) {
  return jspb.Message.setWrapperField(this, 2, value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.SearchQuery} returns this
 */
proto.SearchQuery.prototype.clearSort = function() {
  return this.setSort(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.SearchQuery.prototype.hasSort = function() {
  return jspb.Message.getField(this, 2) != null;
};


/**
 * optional string League = 3;
 * @return {string}
 */
proto.SearchQuery.prototype.getLeague = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 3, ""));
};


/**
 * @param {string} value
 * @return {!proto.SearchQuery} returns this
 */
proto.SearchQuery.prototype.setLeague = function(value) {
  return jspb.Message.setProto3StringField(this, 3, value);
};



/**
 * List of repeated fields within this message type.
 * @private {!Array<number>}
 * @const
 */
proto.SearchGroup.repeatedFields_ = [2];



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
proto.SearchGroup.prototype.toObject = function(opt_includeInstance) {
  return proto.SearchGroup.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.SearchGroup} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.SearchGroup.toObject = function(includeInstance, msg) {
  var f, obj = {
    groupkind: jspb.Message.getFieldWithDefault(msg, 1, 0),
    rangesList: jspb.Message.toObjectList(msg.getRangesList(),
    proto.SearchRange.toObject, includeInstance)
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
 * @return {!proto.SearchGroup}
 */
proto.SearchGroup.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.SearchGroup;
  return proto.SearchGroup.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.SearchGroup} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.SearchGroup}
 */
proto.SearchGroup.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {number} */ (reader.readInt32());
      msg.setGroupkind(value);
      break;
    case 2:
      var value = new proto.SearchRange;
      reader.readMessage(value,proto.SearchRange.deserializeBinaryFromReader);
      msg.addRanges(value);
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
proto.SearchGroup.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.SearchGroup.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.SearchGroup} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.SearchGroup.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getGroupkind();
  if (f !== 0) {
    writer.writeInt32(
      1,
      f
    );
  }
  f = message.getRangesList();
  if (f.length > 0) {
    writer.writeRepeatedMessage(
      2,
      f,
      proto.SearchRange.serializeBinaryToWriter
    );
  }
};


/**
 * optional int32 GroupKind = 1;
 * @return {number}
 */
proto.SearchGroup.prototype.getGroupkind = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 1, 0));
};


/**
 * @param {number} value
 * @return {!proto.SearchGroup} returns this
 */
proto.SearchGroup.prototype.setGroupkind = function(value) {
  return jspb.Message.setProto3IntField(this, 1, value);
};


/**
 * repeated SearchRange Ranges = 2;
 * @return {!Array<!proto.SearchRange>}
 */
proto.SearchGroup.prototype.getRangesList = function() {
  return /** @type{!Array<!proto.SearchRange>} */ (
    jspb.Message.getRepeatedWrapperField(this, proto.SearchRange, 2));
};


/**
 * @param {!Array<!proto.SearchRange>} value
 * @return {!proto.SearchGroup} returns this
*/
proto.SearchGroup.prototype.setRangesList = function(value) {
  return jspb.Message.setRepeatedWrapperField(this, 2, value);
};


/**
 * @param {!proto.SearchRange=} opt_value
 * @param {number=} opt_index
 * @return {!proto.SearchRange}
 */
proto.SearchGroup.prototype.addRanges = function(opt_value, opt_index) {
  return jspb.Message.addToRepeatedWrapperField(this, 2, opt_value, proto.SearchRange, opt_index);
};


/**
 * Clears the list making it empty but non-null.
 * @return {!proto.SearchGroup} returns this
 */
proto.SearchGroup.prototype.clearRangesList = function() {
  return this.setRangesList([]);
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
proto.SortOrder.prototype.toObject = function(opt_includeInstance) {
  return proto.SortOrder.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.SortOrder} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.SortOrder.toObject = function(includeInstance, msg) {
  var f, obj = {
    sortkind: jspb.Message.getFieldWithDefault(msg, 1, 0),
    modifier: (f = msg.getModifier()) && Models_Analyzer_Proto_ModifierTypes_pb.ModKey.toObject(includeInstance, f)
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
 * @return {!proto.SortOrder}
 */
proto.SortOrder.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.SortOrder;
  return proto.SortOrder.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.SortOrder} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.SortOrder}
 */
proto.SortOrder.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {number} */ (reader.readInt32());
      msg.setSortkind(value);
      break;
    case 2:
      var value = new Models_Analyzer_Proto_ModifierTypes_pb.ModKey;
      reader.readMessage(value,Models_Analyzer_Proto_ModifierTypes_pb.ModKey.deserializeBinaryFromReader);
      msg.setModifier(value);
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
proto.SortOrder.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.SortOrder.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.SortOrder} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.SortOrder.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getSortkind();
  if (f !== 0) {
    writer.writeInt32(
      1,
      f
    );
  }
  f = message.getModifier();
  if (f != null) {
    writer.writeMessage(
      2,
      f,
      Models_Analyzer_Proto_ModifierTypes_pb.ModKey.serializeBinaryToWriter
    );
  }
};


/**
 * optional int32 SortKind = 1;
 * @return {number}
 */
proto.SortOrder.prototype.getSortkind = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 1, 0));
};


/**
 * @param {number} value
 * @return {!proto.SortOrder} returns this
 */
proto.SortOrder.prototype.setSortkind = function(value) {
  return jspb.Message.setProto3IntField(this, 1, value);
};


/**
 * optional ModKey Modifier = 2;
 * @return {?proto.ModKey}
 */
proto.SortOrder.prototype.getModifier = function() {
  return /** @type{?proto.ModKey} */ (
    jspb.Message.getWrapperField(this, Models_Analyzer_Proto_ModifierTypes_pb.ModKey, 2));
};


/**
 * @param {?proto.ModKey|undefined} value
 * @return {!proto.SortOrder} returns this
*/
proto.SortOrder.prototype.setModifier = function(value) {
  return jspb.Message.setWrapperField(this, 2, value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.SortOrder} returns this
 */
proto.SortOrder.prototype.clearModifier = function() {
  return this.setModifier(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.SortOrder.prototype.hasModifier = function() {
  return jspb.Message.getField(this, 2) != null;
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
proto.SearchRange.prototype.toObject = function(opt_includeInstance) {
  return proto.SearchRange.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.SearchRange} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.SearchRange.toObject = function(includeInstance, msg) {
  var f, obj = {
    key: (f = msg.getKey()) && Models_Analyzer_Proto_ModifierTypes_pb.ModKey.toObject(includeInstance, f),
    minvalue: jspb.Message.getFloatingPointFieldWithDefault(msg, 2, 0.0),
    maxvalue: jspb.Message.getFloatingPointFieldWithDefault(msg, 3, 0.0)
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
 * @return {!proto.SearchRange}
 */
proto.SearchRange.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.SearchRange;
  return proto.SearchRange.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.SearchRange} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.SearchRange}
 */
proto.SearchRange.deserializeBinaryFromReader = function(msg, reader) {
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
      msg.setMinvalue(value);
      break;
    case 3:
      var value = /** @type {number} */ (reader.readFloat());
      msg.setMaxvalue(value);
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
proto.SearchRange.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.SearchRange.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.SearchRange} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.SearchRange.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getKey();
  if (f != null) {
    writer.writeMessage(
      1,
      f,
      Models_Analyzer_Proto_ModifierTypes_pb.ModKey.serializeBinaryToWriter
    );
  }
  f = message.getMinvalue();
  if (f !== 0.0) {
    writer.writeFloat(
      2,
      f
    );
  }
  f = message.getMaxvalue();
  if (f !== 0.0) {
    writer.writeFloat(
      3,
      f
    );
  }
};


/**
 * optional ModKey Key = 1;
 * @return {?proto.ModKey}
 */
proto.SearchRange.prototype.getKey = function() {
  return /** @type{?proto.ModKey} */ (
    jspb.Message.getWrapperField(this, Models_Analyzer_Proto_ModifierTypes_pb.ModKey, 1));
};


/**
 * @param {?proto.ModKey|undefined} value
 * @return {!proto.SearchRange} returns this
*/
proto.SearchRange.prototype.setKey = function(value) {
  return jspb.Message.setWrapperField(this, 1, value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.SearchRange} returns this
 */
proto.SearchRange.prototype.clearKey = function() {
  return this.setKey(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.SearchRange.prototype.hasKey = function() {
  return jspb.Message.getField(this, 1) != null;
};


/**
 * optional float MinValue = 2;
 * @return {number}
 */
proto.SearchRange.prototype.getMinvalue = function() {
  return /** @type {number} */ (jspb.Message.getFloatingPointFieldWithDefault(this, 2, 0.0));
};


/**
 * @param {number} value
 * @return {!proto.SearchRange} returns this
 */
proto.SearchRange.prototype.setMinvalue = function(value) {
  return jspb.Message.setProto3FloatField(this, 2, value);
};


/**
 * optional float MaxValue = 3;
 * @return {number}
 */
proto.SearchRange.prototype.getMaxvalue = function() {
  return /** @type {number} */ (jspb.Message.getFloatingPointFieldWithDefault(this, 3, 0.0));
};


/**
 * @param {number} value
 * @return {!proto.SearchRange} returns this
 */
proto.SearchRange.prototype.setMaxvalue = function(value) {
  return jspb.Message.setProto3FloatField(this, 3, value);
};


goog.object.extend(exports, proto);
