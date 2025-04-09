import { CalculationType } from '../Enums/calculation-type.enum'
import { NutrientType } from '../Enums/nutrient-type.enum'

export interface NutrientOfDayModel {
  nutrientType: NutrientType;
  calculationType: CalculationType;
  value: number;
}
