import { SortingCriteria } from "../Enums/sorting-criteria.enum";

export interface GetFoodRequestParameters {
  name: string,
  page: number,
  PageSize: number,
  SortAsc: boolean,
  SortingCriteria: SortingCriteria
}
