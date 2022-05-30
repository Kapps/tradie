import { configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import criteriaReducer from '../features/criteria/criteriaSlice';
import criteriaGroupsReducer from '../features/criteriagroups/criteriaGroupsSlice';
import criteriaValuesReducer from '../features/criterialist/criteriaValueSlice';
import leaguesReducer from '../features/leagues/leaguesSlice';
import searchReducer from '../features/search/searchSlice';
import itemTypesReducer from '../features/itemTypes/itemTypesSlice';
import modifiersReducer from '../features/modifiers/modifiersSlice';
import affixRangesReducer from '../features/affixRanges/affixRangesSlice';

export const store = configureStore({
  reducer: {
    criteria: criteriaReducer,
    criteriaGroups: criteriaGroupsReducer,
    criteriaValues: criteriaValuesReducer,
    modifiers: modifiersReducer,
    affixRanges: affixRangesReducer,
    leagues: leaguesReducer,
    search: searchReducer,
    itemTypes: itemTypesReducer,
  },
  middleware: (getDefaultMiddleware) => getDefaultMiddleware({
    serializableCheck: false
  }),
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  Action<string>
>;
