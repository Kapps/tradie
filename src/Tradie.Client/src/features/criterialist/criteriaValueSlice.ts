import { createSlice } from "@reduxjs/toolkit";
import { RootState } from "../../app/store";

export interface CriteriaValueState {

  selectedValues: CriteriaValue[]
}

export enum CriteriaKind {
  Modifier = 0,
  League = 1
}

export interface Criteria {
  id: string;
  kind: CriteriaKind;
}

const initialState: CriteriaValueState = {
  selectedValues: []
};

export interface CriteriaValue {
  criteriaId: string,
  id: string;
  groupId: string;
  minValue?: number;
  maxValue?: number;
}

export const criteriaValueSlice = createSlice({
  name: 'criteriaValues',
  initialState,
  reducers: {
    addCriteriaValue: (state, action: { payload: CriteriaValue }) => {
      state.selectedValues.push(action.payload);
    },
    removeCriteriaValue: (state, action: { payload: CriteriaValue }) => {
      state.selectedValues = state.selectedValues.filter(
        (value) => value.id !== action.payload.id
      );
    },
    clearCriteriaValues: (state) => {
      state.selectedValues = [];
    },
    updateCriteriaValue: (state, action: { payload: CriteriaValue }) => {
      const index = state.selectedValues.findIndex(
        (value) => value.id === action.payload.id
      );
      console.log('index: ', index);
      console.log(action.payload);
      if (index !== -1) {
        state.selectedValues[index] = action.payload;
      }
    },
  },
});

export const selectCriteria = (groupId: string, criteriaId: number) => (state: RootState) =>
  state.criteriaValues.selectedValues.find(
    (value) => value.groupId === groupId && value.criteriaId === criteriaId.toString()
  );

export const selectCriteriaValues = (groupId: string) => (state: RootState) =>
  state.criteriaValues.selectedValues.filter(
    (value) => value.groupId === groupId
  );

export const selectCriteriaValue = (id: string) => (state: RootState) =>
  state.criteriaValues.selectedValues.find((value) => value.id === id);

export const {
  addCriteriaValue,
  removeCriteriaValue,
  clearCriteriaValues,
  updateCriteriaValue,
} = criteriaValueSlice.actions;

export default criteriaValueSlice.reducer;