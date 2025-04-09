import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { DayResultModel } from '../../models/food-service/Responces/day-result.model';
import { CreateDayResultModel } from '../../models/food-service/Requests/create-day-result.model';
import { UpdateDayResultModel } from '../../models/food-service/Requests/update-day-result.model';

@Injectable({
  providedIn: 'root'
})
export class DayResultService {
  private readonly baseUrl = 'https://localhost/food_service/day_results';

  constructor(private http: HttpClient) { }

  createDayResult(createDayResultDTO: CreateDayResultModel): Observable<DayResultModel> {
    return this.http.post<DayResultModel>(this.baseUrl, createDayResultDTO);
  }

  deleteDayResult(dayResultId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${dayResultId}`);
  }

  getOrCreateDayResult(profileId: string): Observable<DayResultModel> {
    return this.http.get<DayResultModel>(`${this.baseUrl}/by-profile/${profileId}`);
  }

  updateDayResult(updateDayResultDTO: UpdateDayResultModel): Observable<void> {
    return this.http.put<void>(this.baseUrl, updateDayResultDTO);
  }

  getDayResultsByPeriod(profileId: string, startDate: string, endDate: string): Observable<DayResultModel[]> {
    let params = new HttpParams()
      .set('startDate', startDate)
      .set('endDate', endDate);

    return this.http.get<DayResultModel[]>(`${this.baseUrl}/by-period/${profileId}`, { params });
  }
}
