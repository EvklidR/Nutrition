import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { format } from 'date-fns';


import { UpdateDayResultModel } from '../../models/food-service/Requests/update-day-result.model';
import { DayResultResponse } from '../../models/food-service/Responses/day-result.model';
import { EatenFoodResponse } from '../../models/food-service/Responses/eaten-food.model';
import { PeriodParameters } from '../../models/request-parameters/period-parameters.model';
import { PaginationParameters } from '../../models/request-parameters/pagination-parameters.model';
import { DayResultsResponse } from '../../models/food-service/Responses/day-results.model';

@Injectable({
  providedIn: 'root'
})
export class DayResultService {
  private readonly baseUrl = 'https://localhost/food_service/DayResults';

  constructor(private http: HttpClient) { }

  deleteDayResult(dayResultId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${dayResultId}`);
  }

  getOrCreateDayResult(profileId: string): Observable<DayResultResponse> {
    return this.http.get<DayResultResponse>(`${this.baseUrl}/get-or-create/${profileId}`);
  }

  updateDayResult(updateDayResultDTO: UpdateDayResultModel): Observable<void> {
    return this.http.put<void>(this.baseUrl, updateDayResultDTO);
  }

  getDayResult(profileId: string, dayResultId: string): Observable<DayResultResponse> {
    return this.http.get<DayResultResponse>(`${this.baseUrl}/${profileId}/${dayResultId}`);
  }

  getDayResults(
    profileId: string,
    periodParameters: PeriodParameters | null,
    paginationParameters: PaginationParameters | null): Observable<DayResultsResponse>
  {
    const params = this.buildParams({ ...periodParameters, ...paginationParameters });
    return this.http.get<DayResultsResponse>(`${this.baseUrl}/${profileId}`, { params });
  }

  getEatenFood(profileId: string, periodParameters: PeriodParameters): Observable<EatenFoodResponse[]> {
    const params = this.buildParams(periodParameters);
    return this.http.get<EatenFoodResponse[]>(`${this.baseUrl}/eaten-food/${profileId}`, { params });
  }

  private buildParams(obj: any): HttpParams {
    let params = new HttpParams();

    for (const key in obj) {
      if (obj[key] !== null && obj[key] !== undefined) {
        let value = obj[key];
        if (value instanceof Date) {
          value = format(value, 'yyyy-MM-dd');
        }
        params = params.set(key, String(value));
      }
    }

    return params;
  }
}
