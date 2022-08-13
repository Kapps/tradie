import { post } from '../../api/apiClient';
import {
  SearchRequest,
  SearchResponse as SearchResponseProto,
  SearchResultEntry as SearchResultEntryProto,
} from '../../protos/Services/Web/Proto/SearchService_pb';
import { Item } from '../item/item';
import { SearchQuery } from './search';
import * as google_protobuf_timestamp_pb from 'google-protobuf/google/protobuf/timestamp_pb';

/**
 * A single item and its metadata returned from a search.
 */
export class SearchResultEntry {
  /** The actual item. */
  item: Item;
  /** The first time this item was seen. */
  firstSeen: Date;
  /** The character name last used by the player that owns this item. */
  lastCharacterName: string;
  /** The price of this item converted to chaos orbs. */
  chaosEquivalentPrice: number;
  /** The amount of time someone has attempted to whisper for this item. */
  timesRequested: number;
  /** The name of the stash tab the item is stored in. */
  tabName: string;

  constructor(
    item: Item,
    firstSeen: Date,
    lastCharacterName: string,
    chaosEquivalentPrice: number,
    timesRequested: number,
    tabName: string,
  ) {
    this.item = item;
    this.firstSeen = firstSeen;
    this.lastCharacterName = lastCharacterName;
    this.chaosEquivalentPrice = chaosEquivalentPrice;
    this.timesRequested = timesRequested;
    this.tabName = tabName;
  }

  static fromProto(proto: SearchResultEntryProto): SearchResultEntry {
    return new SearchResultEntry(
      Item.fromProto(proto.getItem()!),
      proto.getFirstseen()?.toDate() ?? new Date(),
      proto.getLastcharactername(),
      proto.getChaosequivalentprice(),
      proto.getTimesrequested(),
      proto.getTabname(),
    );
  }

  toProto(): SearchResultEntryProto {
    const proto = new SearchResultEntryProto();
    const ts = new google_protobuf_timestamp_pb.Timestamp();
    proto.setItem(this.item.toProto());
    proto.setFirstseen(google_protobuf_timestamp_pb.Timestamp.fromDate(this.firstSeen));
    proto.setLastcharactername(this.lastCharacterName);
    proto.setChaosequivalentprice(this.chaosEquivalentPrice);
    proto.setTimesrequested(this.timesRequested);
    proto.setTabname(this.tabName);
    return proto;
  }
}

export class SearchResponse {
  entries: SearchResultEntry[];

  constructor(entries: SearchResultEntry[]) {
    this.entries = entries;
  }

  static fromProto(proto: SearchResponseProto): SearchResponse {
    return new SearchResponse(proto.getResultsList().map(SearchResultEntry.fromProto));
  }

  toProto(): SearchResponseProto {
    const proto = new SearchResponseProto();
    proto.setResultsList(this.entries.map((e) => e.toProto()));
    return proto;
  }
}


export const search = async (query: SearchQuery): Promise<SearchResponse> => {
  const request = new SearchRequest();

  const protoQuery = query.toProto();
  request.setQuery(protoQuery);

  const body = await post('search', request);
  const response = SearchResponseProto.deserializeBinary(body);
  return SearchResponse.fromProto(response);
};
