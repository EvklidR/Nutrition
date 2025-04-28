import { SortingCriteria } from "../Enums/sorting-criteria.enum";

export interface GetFoodRequestParameters {
  name: string | null,
  page: number | null,
  pageSize: number | null,
  sortAsc: boolean | null, 
  sortingCriteria: SortingCriteria | null
}
