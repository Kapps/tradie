import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import localforage from 'localforage';
import { RootState } from '../../app/store';
import { notifyError } from '../notifications/notifications';
import { League } from './league';
import { getActiveLeagues } from './leaguesApi';


const DEFAULT_LEAGUE_KEY = 'DEFAULT_LEAGUE';

export interface LeagueState {
  activeLeagues: League[];
  defaultLeague?: League;
}

const initialState: LeagueState = {
  activeLeagues: [],
  defaultLeague: undefined
};

export const loadActiveLeagues = createAsyncThunk('leagues/getActiveLeagues', async () => {
  const response = await getActiveLeagues();
  return response;
});

export const loadDefaultLeague = createAsyncThunk('leagues/loadDefaultLeague', async () => {
  const response = await localforage.getItem<League>(DEFAULT_LEAGUE_KEY);
  // TODO: Should use the most popular league if not.
  return response;
});

export const setDefaultLeague = createAsyncThunk('leagues/setDefault', async (leagueId: League) => {
  await localforage.setItem(DEFAULT_LEAGUE_KEY, leagueId);
});

export const selectActiveLeagues = (state: RootState) => state.leagues.activeLeagues;

export const selectDefaultLeague = (state: RootState) => state.leagues.defaultLeague;

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
    builder.addCase(loadDefaultLeague.fulfilled, (state, action) => {
      if (action.payload) {
        state.defaultLeague = action.payload;
      }
    });
    builder.addCase(loadDefaultLeague.rejected, (state, action) => {
      notifyError('Could not load default league', action.error);
    });
  },
});

export default leagueSlice.reducer;
