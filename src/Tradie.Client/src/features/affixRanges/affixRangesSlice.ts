import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from '../../app/store';
import { notifyError } from '../notifications/notifications';
import { AffixRange, AffixRangeEntityKind, ModKindCategory } from './affixRange';
import { getAffixRanges } from './affixRangeApi';

export interface AffixRangeState {
  ranges: AffixRange[];
}

const initialState: AffixRangeState = {
  ranges: [],
};

export const loadAffixRanges = createAsyncThunk('affixRanges/loadRanges', async () => {
  const response = await getAffixRanges();
  return response;
});

export const selectAllRanges = (state: RootState) => state.affixRanges.ranges;

export const selectAffixRange = (modHash: string, entityKind: AffixRangeEntityKind, category: ModKindCategory) =>
  (state: RootState) => state.affixRanges.ranges.find(c => c.modHash === modHash && c.entityKind === entityKind && (c.modKindCategory === category || c.modKindCategory === ModKindCategory.Pseudo));

export const affixRangesSlice = createSlice({
  name: 'affixRanges',
  initialState,
  reducers: {

  },
  extraReducers: (builder) => {
    builder.addCase(loadAffixRanges.fulfilled, (state, action) => {
      state.ranges = action.payload;
    });
    builder.addCase(loadAffixRanges.rejected, (state, action) => {
      notifyError('Could not load affix ranges', action.error);
    });
  },
});

export default affixRangesSlice.reducer;
