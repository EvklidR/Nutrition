import { ShortDayResultResponse } from "./short-day-result.model";

export interface DayResultsResponse {
  dayResults: ShortDayResultResponse[]
  totalCount: number
}
