import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from '../../app/store';
import { notifyError } from '../notifications/notifications';
import { Modifier } from './modifier';
import { getModifiers } from './modifiersApi';

export interface ModifierState {
  modifiers: Modifier[];
}

const initialState: ModifierState = {
  modifiers: [],
};

export const loadModifiers = createAsyncThunk('modifiers/loadModifiers', async () => {
  const response = await getModifiers();
  return response;
});

export const selectAllModifiers = (state: RootState) => state.modifiers.modifiers;

export const selectModifier = (hash: string) => (state: RootState) => state.modifiers.modifiers.find(c => c.hash === hash);

export const modifiersSlice = createSlice({
  name: 'modifiers',
  initialState,
  reducers: {

  },
  extraReducers: (builder) => {
    builder.addCase(loadModifiers.fulfilled, (state, action) => {
      state.modifiers = action.payload;
    });
    builder.addCase(loadModifiers.rejected, (state, action) => {
      notifyError('Could not load modifiers', action.error);
    });
  },
});

export default modifiersSlice.reducer;
