import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from '../../app/store';
import * as criteriaApi from './criteriaApi';
import memoize from 'memoizee';
import { Criteria } from '../../protos/Models/Indexer/Proto/Criteria_pb';


const CACHE_KEY = 'CACHED_CRITERIA';

export interface CriteriaState {
  criteria: Array<Criteria.AsObject>;
}

const initialState: CriteriaState = {
  criteria: [],
};

export const loadCriteria = createAsyncThunk('criteria/loadCriteria', memoize(async () => {
  const cachedItems = JSON.parse(window.localStorage.getItem(CACHE_KEY) ?? 'null');
  if (cachedItems) {
    if (cachedItems.criteria && cachedItems.timestamp && Date.now() - cachedItems.timestamp < 1000 * 60 * 60 * 24) {
      return cachedItems.criteria;
    }
  }

  const response = await criteriaApi.getAllCriteria();
  const serialized = JSON.stringify({
    criteria: response,
    timestamp: Date.now(),
  });
  window.localStorage.setItem(CACHE_KEY, serialized);

  return response;
}));

export const selectAvailableCriteria = (state: RootState) => state.criteria.criteria;

export const criteriaSlice = createSlice({
  name: 'criteria',
  initialState,
  reducers: {

  },
  extraReducers: (builder) => {
    builder.addCase(loadCriteria.fulfilled, (state, action) => {
      state.criteria = action.payload;
    });
    builder.addCase(loadCriteria.rejected, (state, action) => {
      throw new Error('could not load filter criteria');
    });
  },
});

export default criteriaSlice.reducer;
