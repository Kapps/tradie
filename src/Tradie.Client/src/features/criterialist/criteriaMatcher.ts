import { Criteria, CriteriaKind } from "../criteria/criteria";
import { CriteriaValue } from "./criteriaValueSlice";



export const getMatchedCriteria = (criteria: Criteria[], selectedCriteriaValues: CriteriaValue[], searchText: string, count: number) => {
  const selectedCriteria = selectedCriteriaValues.map(c => criteria.find(d => d.id === c.criteriaId)!);
  const selectedLookup = new Set<Criteria>(selectedCriteria);
  const searchLowerified = (searchText || '').toLowerCase();
  const results = new Array<Criteria>();

  for (const crit of criteria) {
    if (crit.kind !== CriteriaKind.LEAGUE && !selectedCriteria.some(c => c.kind === CriteriaKind.LEAGUE)) {
      continue;
    }
    if (crit.kind !== CriteriaKind.MODIFIER && selectedCriteria.some(c => c.kind === crit.kind)) {
      continue;
    }
    if (crit.name.toLowerCase().includes(searchLowerified) || selectedLookup.has(crit)) {
      results.push(crit);
      selectedLookup.delete(crit);
    }
    if (results.length > count) {
      break;
    }
  }

  for (const remaining of selectedLookup) {
    results.push(remaining);
  }

  return results;
};


