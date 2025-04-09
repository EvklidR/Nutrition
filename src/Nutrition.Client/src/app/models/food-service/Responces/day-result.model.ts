import { BriefMealModel } from "./brief-meal.model";

export interface DayResultModel {
  id: string;
  glassesOfWater: number;
  date: Date;
  meals: BriefMealModel[];
}
