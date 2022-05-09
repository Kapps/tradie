import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from '../../app/store';
import * as criteriaApi from './criteriaApi';
import memoize from 'memoizee';
import { Criteria } from './criteria';


const CACHE_KEY = 'CACHED_CRITERIA';

export interface CriteriaState {
  criteria: Array<Criteria>;
}

const initialState: CriteriaState = {
  criteria: [],
};

export const loadCriteria = createAsyncThunk('criteria/loadCriteria', async () => {
  return criteriaApi.getAllCriteria();
});

export const selectAvailableCriteria = (state: RootState) => state.criteria.criteria;

export const selectCriteria = (id: string) => (state: RootState) => state.criteria.criteria.find(c => c.id === id);

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
