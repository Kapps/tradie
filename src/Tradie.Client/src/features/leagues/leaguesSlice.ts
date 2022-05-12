import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from '../../app/store';
import { notifyError } from '../notifications/notifications';
import { League } from './league';
import { getActiveLeagues } from './leaguesApi';

export interface LeagueState {
  activeLeagues: League[];
}

const initialState: LeagueState = {
  activeLeagues: [],
};

export const loadActiveLeagues = createAsyncThunk('leagues/getActiveLeagues', async () => {
  const response = await getActiveLeagues();
  return response;
});

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
      notifyError('Could not load active leagues', action.error);
    });
  },
});

export default leagueSlice.reducer;
