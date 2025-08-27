import { PaginationParameters } from "../../request-parameters/pagination-parameters.model";
import { SortingCriteria } from "../Enums/sorting-criteria.enum";

export interface GetFoodRequestParameters {
  name: string | null,
  sortAsc: boolean | null,
  paginationParameters: PaginationParameters | null,
  sortingCriteria: SortingCriteria | null
}
