export interface BriefDishModel {
  id: string;
  name: string;
  description: string | null;
  imageUrl: string | null;
  calories: number;
  proteins: number;
  fats: number;
  carbohydrates: number;
  weight: number;
}
