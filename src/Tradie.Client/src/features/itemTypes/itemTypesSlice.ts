import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import memoize from 'memoizee';
import { RootState } from '../../app/store';
import { ItemType } from './itemType';
import { getItemTypes } from './itemTypesApi';

export interface ItemTypeState {
  itemTypes: ItemType[];
}

const initialState: ItemTypeState = {
  itemTypes: [],
};

export const loadItemTypes = createAsyncThunk('itemTypes/getItemTypes', memoize(async () => {
  const response = await getItemTypes();
  return response;
}));

export const selectItemTypes = (state: RootState) => state.itemTypes.itemTypes;

export const selectItemType = (id: number) => (state: RootState) => state.itemTypes.itemTypes.find(c => c.id === id);

export const itemTypesSlice = createSlice({
  name: 'itemTypes',
  initialState,
  reducers: {

  },
  extraReducers: (builder) => {
    builder.addCase(loadItemTypes.fulfilled, (state, action) => {
      state.itemTypes = action.payload;
    });
    builder.addCase(loadItemTypes.rejected, (state, action) => {
      throw new Error('could not load item types');
    });
  },
});

export default itemTypesSlice.reducer;
