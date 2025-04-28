import { ActivityLevel } from "../Enums/activity-level.enum";

export interface ProfileModel {
  id: string;
  userId: string;
  name: string;
  height: number;
  weight: number;
  birthday: Date;
  gender: string;
  activityLevel: ActivityLevel;
  desiredGlassesOfWater: number;
  thereIsMealPlan: boolean;
}
