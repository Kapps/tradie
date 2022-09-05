import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { AppThunk, RootState } from "../../app/store";
import { CriteriaKind, ModifierKind } from "../criteria/criteria";
import { selectCriteria } from "../criteria/criteriaSlice";
import { selectCriteriaGroups } from "../criteriagroups/criteriaGroupsSlice";
import { selectCriteriaValues } from "../criterialist/criteriaValueSlice";
import { Item } from "../item/item";
import { AnalyzerId, ItemDetailProperties, ItemListingProperties } from "../item/itemProperties";
import { getNameForCurrency } from "../item/price";
import { notify, notifyError } from "../notifications/notifications";
import { SearchRange, ModKey, ModKind, SearchGroup, SearchQuery, SortKind, SortOrder } from "./search";
import { search, SearchResultEntry } from "./searchApi";

export interface SearchState {
  results: SearchResultEntry[];
  isLoading: boolean;
}

const initialState: SearchState = {
  results: [],
  isLoading: false,
};

export const selectSearchResults = (state: RootState) => state.search.results;

export const copyWhisperDetails = createAsyncThunk('search/copyWhisperDetails', async (entry: SearchResultEntry) => {
  console.log('Copying details');
  const item = entry.item;
  const itemDetails = item.findProperty<ItemDetailProperties>(AnalyzerId.ItemDetails);
  const tradeDetails = item.findProperty<ItemListingProperties>(AnalyzerId.TradeAttributes);
  const price = tradeDetails.price;
  const name = entry.lastCharacterName;
  const currency = getNameForCurrency(price.currency);
  const tab = entry.tabName;
  const whisper = `@${name} Hi, I would like to buy your ${itemDetails.name} listed for ${price.amount} ${currency} in Unknown League (stash tab "${tab}"; position: left ${tradeDetails.x}, top ${tradeDetails.y})`;
  await navigator.clipboard.writeText(whisper);
  notify('Whisper copied to clipboard', whisper);
});

export const performSearch = createAsyncThunk('search/performSearch', async (_, thunkAPI) => {
  const state = thunkAPI.getState() as RootState;
  const criteriaGroups = selectCriteriaGroups(state);
  let league = '';

  const searchGroups = criteriaGroups.map((group) => {
    const criteriaValues = selectCriteriaValues(group.id)(state);
    const criteria = criteriaValues.filter(c => c.enabled).map((c) => ({ value: c, criteria: selectCriteria(c.id)(state) }));

    const leagueCriteria = criteria.find(c => c.criteria?.kind === CriteriaKind.LEAGUE);
    if (leagueCriteria) {
      if (league) {
        throw new Error('Multiple leagues selected');
      }
      league = leagueCriteria.criteria!.league!;
    }

    return new SearchGroup(
      group.kind,
      criteria
        .filter((c) => c.criteria?.kind === CriteriaKind.MODIFIER)
        .map(
          (c) =>
            new SearchRange(
              new ModKey(c.criteria!.modifier!.hash!, c.criteria?.modifier!.kind == ModifierKind.Pseudo ? ModKind.Pseudo : ModKind.Total),
              c.value.minValue,
              c.value.maxValue,
            ),
        ),
    );
  });

  if (!league) {
    notifyError('You must select a league to search.');
    return [];
  }

  const searchQuery = new SearchQuery(searchGroups, new SortOrder(SortKind.Price), league);

  const response = await search(searchQuery);
  return response.entries;
});

export const searchSlice = createSlice({
  name: 'search',
  initialState,
  reducers: {
    clearResults: (state) => {
      state.results = [];
    },
  },
  extraReducers: (builder) => {
    builder.addCase(performSearch.pending, (state) => {
      state.isLoading = true;
    });
    builder.addCase(performSearch.fulfilled, (state, action) => {
      state.results = action.payload;
      state.isLoading = false;
    });
    builder.addCase(performSearch.rejected, (state, action) => {
      notifyError('Could not load search results', action.error);
    });

    builder.addCase(copyWhisperDetails.pending, (state) => {
      state.isLoading = true;
    });
    builder.addCase(copyWhisperDetails.fulfilled, (state) => {
      state.isLoading = false;
    });
    builder.addCase(copyWhisperDetails.rejected, (state, action) => {
      notifyError('Could not copy whisper', action.error);
    });
  }
});

export const { clearResults } = searchSlice.actions;

export default searchSlice.reducer;
