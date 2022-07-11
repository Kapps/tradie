import { Modifier as ProtoModifier } from "../../protos/Models/Analyzer/Proto/Modifier_pb";

/**
 * A single affix modifier.
 */
export class Modifier {
  /** Unique ID for the modifer. */
  id: number;
  /** Value-independent hash of the modifier. */
  hash: string;
  /** Human readable text for the modifier. */
  text: string;
  /** Whether a standard modifier or pseudo modifier. */
  kind: number;

  constructor(id: number, hash: string, text: string, kind: number) {
    this.id = id;
    this.hash = hash;
    this.text = text;
    this.kind = kind;
  }

  toProto(): ProtoModifier {
    const proto = new ProtoModifier();
    proto.setId(this.id);
    proto.setHash(this.hash);
    proto.setText(this.text);
    proto.setKind(this.kind);
    return proto;
  }

  static fromProto(proto: ProtoModifier): Modifier {
    return new Modifier(
      proto.getId(),
      proto.getHash(),
      proto.getText(),
      proto.getKind(),
    );
  }
}
