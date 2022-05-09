import { configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import counterReducer from '../features/counter/counterSlice';
import criteriaReducer from '../features/criteria/criteriaSlice';
import criteriaGroupsReducer from '../features/criteriagroups/criteriaGroupsSlice';
import criteriaValuesReducer from '../features/criterialist/criteriaValueSlice';
import leaguesReducer from '../features/leagues/leaguesSlice';
import searchReducer from '../features/search/searchSlice';

export const store = configureStore({
  reducer: {
    counter: counterReducer,
    criteria: criteriaReducer,
    criteriaGroups: criteriaGroupsReducer,
    criteriaValues: criteriaValuesReducer,
    leagues: leaguesReducer,
    search: searchReducer,
  },
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  Action<string>
>;
