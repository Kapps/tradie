import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import memoize from 'memoizee';
import { RootState } from '../../app/store';
import { League, getActiveLeagues } from './leaguesApi';

export interface LeagueState {
  activeLeagues: League[];
}

const initialState: LeagueState = {
  activeLeagues: [],
};

export const loadActiveLeagues = createAsyncThunk('leagues/getActiveLeagues', memoize(async () => {
  const response = await getActiveLeagues();
  return response;
}));

export const selectActiveLeagues = (state: RootState) => state.leagues.activeLeagues;

export const leagueSlice = createSlice({
  name: 'leagues',
  initialState,
  reducers: {

  },
  extraReducers: (builder) => {
    builder.addCase(loadActiveLeagues.fulfilled, (state, action) => {
      state.activeLeagues = action.payload;
    });
    builder.addCase(loadActiveLeagues.rejected, (state, action) => {
      throw new Error('could not load active leagues');
    });
  },
});

export default leagueSlice.reducer;
