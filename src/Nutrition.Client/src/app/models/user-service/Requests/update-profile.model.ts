import { ActivityLevel } from "../Enums/activity-level.enum";

export interface UpdateProfileModel {
  id: string;
  name: string;
  height: number;
  weight: number;
  activityLevel: ActivityLevel;
  desiredGlassesOfWater: number;
}
