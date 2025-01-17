using AutoMapper;
using MealPlanService.BusinessLogic.Exceptions;
using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.BusinessLogic.Models;
using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;
using MealPlanService.Infrastructure.Services;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using MealPlanService.Infrastructure.Services.Interfaces;

namespace MealPlanService.BusinessLogic.Services
{
    public class ProfilePlanService
    {
        private readonly IProfileMealPlanRepository _usersMealPlanRepository;
        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly MealPlanService _mealPlanService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ProfilePlanService(
            IProfileMealPlanRepository usersMealPlanRepository,
            IMealPlanRepository mealPlanRepository,
            MealPlanService mealPlanService,
            IUserService userService,
            IMapper mapper)
        {
            _usersMealPlanRepository = usersMealPlanRepository;
            _mealPlanRepository = mealPlanRepository;
            _mealPlanService = mealPlanService;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<List<Recommendation>?> GetRecommendations(string profileId)
        {
            var day = await _mealPlanService.GetCurrentDay(profileId);

            return day.Recommendations.ToList();
        }

        public async Task<ProfileMealPlan> CreateProfilePlanAsync(string userId, ProfileMealPlanDTO profileMealPlanDTO)
        {
            if (await _userService.CheckProfileBelonging(userId, profileMealPlanDTO.ProfileId))
            {
                var plan = await _mealPlanRepository.GetByIdAsync(profileMealPlanDTO.MealPlanId);

                if (plan == null)
                {
                    throw new NotFound("Meal plan not found");
                }

                var userPlan = await _usersMealPlanRepository.GetActiveProfilePlan(profileMealPlanDTO.ProfileId);

                if (userPlan != null)
                {
                    userPlan.EndDate = DateOnly.FromDateTime(DateTime.Now);
                    userPlan.IsActive = false;

                    await _usersMealPlanRepository.UpdateAsync(userPlan);
                }
                else
                {
                    //send message to broker
                }

                var usersMealPlan = _mapper.Map<ProfileMealPlan>(profileMealPlanDTO);

                await _usersMealPlanRepository.CreateAsync(usersMealPlan);

                return usersMealPlan;
            }
            else
            {
                throw new BadRequest("You don't have access to this profile");
            }
        }

        public async Task<List<ProfileMealPlan>?> GetProfilePlansAsync(string userId, string profileId)
        {
            if (await _userService.CheckProfileBelonging(userId, profileId))
            {
                return await _usersMealPlanRepository.GetAllAsync(profileId);
            }
            else
            {
                throw new BadRequest("You don't have access to this profile");
            }
        }

        public async Task DeleteProfilePlansAsync(string profileId) //For rabbitMQ when profile is deleted
        {
            var plans = await _usersMealPlanRepository.GetAllAsync(profileId);

            foreach (var plan in plans)
            {
                await _usersMealPlanRepository.DeleteAsync(plan.Id);
            }
        }

        public async Task CompleteProfilePlanAsync(string userId, string profileId)
        {
            if (await _userService.CheckProfileBelonging(userId, profileId))
            {
                var userPlan = await _usersMealPlanRepository.GetActiveProfilePlan(profileId);

                if (userPlan == null)
                {
                    throw new NotFound("Active plan not found");
                }

                userPlan.EndDate = DateOnly.FromDateTime(DateTime.Now);
                userPlan.IsActive = false;

                await _usersMealPlanRepository.UpdateAsync(userPlan);

                //message to broker
            }
            else
            {
                throw new BadRequest("You don't have access to this profile");
            }
        }

        public async Task<DailyNeedsResponse> CalculateDailyNutrientsAsync(RequestForCalculating requestForCalculating) // for gprc only
        {
            var day = await _mealPlanService.GetCurrentDay(requestForCalculating.ProfileId);

            DailyNeedsResponse response = new DailyNeedsResponse();

            response.Calories = day.CaloriePercentage * requestForCalculating.DailyKcal;

            foreach (var nutrient in day.NutrientsOfDay)
            {
                switch (nutrient.CalculationType)
                {
                    case CalculationType.PerKg:
                        if (nutrient.NutrientType == NutrientType.Protein)
                            response.Proteins = (double)nutrient.Value! * requestForCalculating.BodyWeight;
                        else if (nutrient.NutrientType == NutrientType.Fat)
                            response.Fats = (double)nutrient.Value! * requestForCalculating.BodyWeight;
                        else if (nutrient.NutrientType == NutrientType.Carbohydrate)
                            response.Carbohydrates = (double)nutrient.Value! * requestForCalculating.BodyWeight;
                        break;

                    case CalculationType.Persent:
                        double caloriesFromPercentage = (double)nutrient.Value! * response.Calories;
                        if (nutrient.NutrientType == NutrientType.Protein)
                            response.Proteins = caloriesFromPercentage / 4;
                        else if (nutrient.NutrientType == NutrientType.Fat)
                            response.Fats = caloriesFromPercentage / 9;
                        else if (nutrient.NutrientType == NutrientType.Carbohydrate)
                            response.Carbohydrates = caloriesFromPercentage / 4;
                        break;

                    case CalculationType.Fixed:
                        if (nutrient.NutrientType == NutrientType.Protein)
                            response.Proteins = (double)nutrient.Value!;
                        else if (nutrient.NutrientType == NutrientType.Fat)
                            response.Fats = (double)nutrient.Value!;
                        else if (nutrient.NutrientType == NutrientType.Carbohydrate)
                            response.Carbohydrates = (double)nutrient.Value!;
                        break;

                    case CalculationType.Bydefault:
                        double remainingCalories = response.Calories - (response.Proteins * 4 + response.Fats * 9 + response.Carbohydrates * 4);
                        if (remainingCalories > 0)
                        {
                            if (nutrient.NutrientType == NutrientType.Protein)
                                response.Proteins = remainingCalories / 4;
                            else if (nutrient.NutrientType == NutrientType.Fat)
                                response.Fats = remainingCalories / 9;
                            else if (nutrient.NutrientType == NutrientType.Carbohydrate)
                                response.Carbohydrates = remainingCalories / 4;
                        }
                        break;
                }
            }

            return response;
        }
    }
}
