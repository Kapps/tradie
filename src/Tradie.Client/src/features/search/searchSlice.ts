import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { AppThunk, RootState } from "../../app/store";
import { CriteriaKind } from "../criteria/criteria";
import { selectCriteria } from "../criteria/criteriaSlice";
import { selectCriteriaGroups } from "../criteriagroups/criteriaGroupsSlice";
import { selectCriteriaValues } from "../criterialist/criteriaValueSlice";
import { Item } from "../item/item";
import { notifyError } from "../notifications/notifications";
import { AffixRange, ModKey, ModKind, SearchGroup, SearchQuery, SortKind, SortOrder } from "./search";
import { search } from "./searchApi";

export interface SearchState {
  results: Item[];
  isLoading: boolean;
}

const initialState: SearchState = {
  results: [],
  isLoading: false,
};

export const selectSearchResults = (state: RootState) => state.search.results;

export const performSearch = createAsyncThunk('search/performSearch', async (_, thunkAPI) => {
  const state = thunkAPI.getState() as RootState;
  const criteriaGroups = selectCriteriaGroups(state);

  const searchGroups = criteriaGroups.map((group) => {
    const criteriaValues = selectCriteriaValues(group.id)(state);
    const criteria = criteriaValues.map((c) => ({ value: c, criteria: selectCriteria(c.id)(state) }));

    return new SearchGroup(
      group.kind,
      criteria
        .filter((c) => c.criteria?.kind === CriteriaKind.MODIFIER)
        .map(
          (c) =>
            new AffixRange(
              new ModKey(c.criteria!.modifier!.hash!, ModKind.Explicit),
              c.value.minValue,
              c.value.maxValue,
            ),
        ),
    );
  });

  const searchQuery = new SearchQuery(searchGroups, new SortOrder(SortKind.Price));

  const response = await search(searchQuery);
  return response.results;
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
  }
});

export const { clearResults } = searchSlice.actions;

export default searchSlice.reducer;
