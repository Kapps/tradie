import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '../../app/store';
import { CriteriaKind } from '../criteria/criteria';
import { SearchGroupKind } from '../search/search';

export interface CriteriaGroup {
  id: string;
  kind: SearchGroupKind;
}

export interface CritieriaGroupsState {
  criteriaGroups: CriteriaGroup[];
}

const initialState: CritieriaGroupsState = {
  criteriaGroups: [
    {
      id: 'default',
      kind: SearchGroupKind.And,
    },
  ],
};

export const selectCriteriaGroup = (id: string) => (state: RootState) =>
  state.criteriaGroups.criteriaGroups.find((group) => group.id === id);

export const selectCriteriaGroups = (state: RootState) => state.criteriaGroups.criteriaGroups;

export const criteriaGroupsSlice = createSlice({
  name: 'criteriaGroups',
  initialState,
  reducers: {
    addCriteriaGroup: (state, action) => {
      state.criteriaGroups.push(action.payload);
    },
    removeCriteriaGroup: (state, action) => {
      state.criteriaGroups = state.criteriaGroups.filter((criteriaGroup) => criteriaGroup.id !== action.payload);
    },
    updateCriteriaGroup: (state, action: PayloadAction<CriteriaGroup>) => {
      const { payload: criteria } = action;
      const index = state.criteriaGroups.findIndex((criteriaGroup) => criteriaGroup.id === criteria.id);
      console.log(criteria);
      state.criteriaGroups[index] = criteria;
    },
    clearCriteriaGroups: (state) => {
      state.criteriaGroups = [...initialState.criteriaGroups];
    },
    /*updateCriteriaValue: (state, action: PayloadAction<UpdateCriteriaValueAction>) => {
      const { groupId, criteriaId, value } = action.payload;
      const criteria = state.criteriaGroups
        .find((c) => c.id === groupId)!
        .criteria.find((c) => c.modifier.id === criteriaId)!;
      console.log(criteria);
      if (criteria) {
        criteria.value = value;
      }
    },*/
  },
});

export const { addCriteriaGroup, removeCriteriaGroup, updateCriteriaGroup, clearCriteriaGroups } =
  criteriaGroupsSlice.actions;

export default criteriaGroupsSlice.reducer;
