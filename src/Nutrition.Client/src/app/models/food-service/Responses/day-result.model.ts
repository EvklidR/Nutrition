import { ShortMealResponse } from "./short-meal.model";

export interface DayResultResponse {
  id: string;
  glassesOfWater: number;
  date: Date;
  weight: number;
  meals: ShortMealResponse[];
}
