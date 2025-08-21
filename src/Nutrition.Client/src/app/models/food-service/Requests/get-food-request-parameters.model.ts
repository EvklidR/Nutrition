import { SortingCriteria } from "../Enums/sorting-criteria.enum";
import { PaginatedParameters } from "./paginated-parameters.model";

export interface GetFoodRequestParameters {
  name: string | null,
  sortAsc: boolean | null,
  paginatedParameters: PaginatedParameters | null,
  sortingCriteria: SortingCriteria | null
}
