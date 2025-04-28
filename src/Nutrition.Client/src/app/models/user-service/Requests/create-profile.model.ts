import { Gender } from "../Enums/gender.enum"
import { ActivityLevel } from "../Enums/activity-level.enum"

export interface CreateProfileModel {
  userId: string | null;
  name: string;
  height: number;
  weight: number;
  birthday: Date;
  gender: Gender;
  activityLevel: ActivityLevel;
}
