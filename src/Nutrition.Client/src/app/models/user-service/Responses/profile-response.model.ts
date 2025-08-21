import { ActivityLevel } from "../Enums/activity-level.enum";
import { Gender } from "../Enums/gender.enum";

export interface ProfileResponse {
  id: string;
  name: string;
  height: number;
  weight: number;
  birthday: Date;
  gender: Gender;
  activityLevel: ActivityLevel;
  desiredGlassesOfWater: number;
  thereIsMealPlan: boolean;
}
