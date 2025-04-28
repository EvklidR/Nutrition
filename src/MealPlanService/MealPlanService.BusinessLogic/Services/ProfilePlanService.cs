using AutoMapper;
using MealPlanService.BusinessLogic.Exceptions;
using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.BusinessLogic.Models;
using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using MealPlanService.Infrastructure.Services.Interfaces;
using MealPlanService.Infrastructure.Enums;
using MealPlanService.Infrastructure.RabbitMQService;

namespace MealPlanService.BusinessLogic.Services
{
    public class ProfilePlanService
    {
        private readonly IProfileMealPlanRepository _usersMealPlanRepository;
        private readonly IMealPlanRepository _mealPlanRepository;

        private readonly MealPlanService _mealPlanService;

        private readonly IUserService _userService;
        private readonly IBrokerService _brokerService;

        private readonly IMapper _mapper;

        public ProfilePlanService(
            IProfileMealPlanRepository usersMealPlanRepository,
            IMealPlanRepository mealPlanRepository,
            MealPlanService mealPlanService,
            IUserService userService,
            IBrokerService brokerService,
            IMapper mapper)
        {
            _usersMealPlanRepository = usersMealPlanRepository;
            _mealPlanRepository = mealPlanRepository;
            _mealPlanService = mealPlanService;
            _userService = userService;
            _brokerService = brokerService;
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
                    await _brokerService.PublishMessageAsync(profileMealPlanDTO.ProfileId, QueueName.MealPlanChoosen);
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

        public async Task<List<ProfileMealPlanWithDetailsDto>> GetProfilePlansAsync(string userId, string profileId)
        {
            if (!await _userService.CheckProfileBelonging(userId, profileId))
            {
                throw new BadRequest("You don't have access to this profile");
            }

            var profilePlans = await _usersMealPlanRepository.GetAllAsync(profileId);

            if (profilePlans == null || !profilePlans.Any())
            {
                return new List<ProfileMealPlanWithDetailsDto>();
            }

            var mealPlanIds = profilePlans.Select(p => p.MealPlanId).Distinct().ToList();

            var mealPlans = await _mealPlanRepository.GetManyByIdsAsync(mealPlanIds);

            var mealPlanMap = mealPlans.ToDictionary(mp => mp.Id);

            var result = profilePlans.Select(p =>
            {
                var plan = mealPlanMap.GetValueOrDefault(p.MealPlanId);

                return new ProfileMealPlanWithDetailsDto
                {
                    Id = p.Id,
                    ProfileId = p.ProfileId,
                    MealPlanId = p.MealPlanId,
                    IsActive = p.IsActive,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    MealPlanName = plan?.Name ?? string.Empty,
                    MealPlanDescription = plan?.Description ?? string.Empty
                };
            }).ToList();

            return result;
        }

        public async Task DeleteProfilePlansAsync(string profileId)
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

                await _brokerService.PublishMessageAsync(profileId, QueueName.MealPlanRevoked);
            }
            else
            {
                throw new BadRequest("You don't have access to this profile");
            }
        }

        public async Task<DailyNeedsResponse> CalculateDailyNutrientsAsync(RequestForCalculating request)
        {
            var day = await _mealPlanService.GetCurrentDay(request.ProfileId);

            DailyNeedsResponse response = new DailyNeedsResponse();

            response.Calories = day.CaloriePercentage * request.DailyKcal;

            foreach (var nutrient in day.NutrientsOfDay)
            {
                switch (nutrient.CalculationType)
                {
                    case CalculationType.PerKg:
                        SetValueWithPerKgType(response, nutrient, request);
                        break;

                    case CalculationType.Persent:
                        SetValueWithPersentType(response, nutrient);
                        break;

                    case CalculationType.Fixed:
                        SetValueWithFixedType(response, nutrient);
                        break;

                    case CalculationType.Bydefault:
                        SetValueWithDefaultType(response, nutrient);
                        break;
                }
            }

            return response;
        }

        private void SetValueWithPerKgType(DailyNeedsResponse response, NutrientOfDay nutrient, RequestForCalculating request)
        {
            if (nutrient.NutrientType == NutrientType.Protein)
            {
                response.Proteins = (double)nutrient.Value! * request.BodyWeight;
            }
            else if (nutrient.NutrientType == NutrientType.Fat)
            {
                response.Fats = (double)nutrient.Value! * request.BodyWeight;
            }
            else if (nutrient.NutrientType == NutrientType.Carbohydrate)
            {
                response.Carbohydrates = (double)nutrient.Value! * request.BodyWeight;
            }
        }

        private void SetValueWithPersentType(DailyNeedsResponse response, NutrientOfDay nutrient)
        {
            double caloriesFromPercentage = (double)nutrient.Value! * response.Calories;

            if (nutrient.NutrientType == NutrientType.Protein)
            {
                response.Proteins = caloriesFromPercentage / 4;
            }
            else if (nutrient.NutrientType == NutrientType.Fat)
            {
                response.Fats = caloriesFromPercentage / 9;
            }
            else if (nutrient.NutrientType == NutrientType.Carbohydrate)
            {
                response.Carbohydrates = caloriesFromPercentage / 4;
            }
        }

        private void SetValueWithFixedType(DailyNeedsResponse response, NutrientOfDay nutrient)
        {
            if (nutrient.NutrientType == NutrientType.Protein)
            {
                response.Proteins = (double)nutrient.Value!;
            }
            else if (nutrient.NutrientType == NutrientType.Fat)
            {
                response.Fats = (double)nutrient.Value!;
            }
            else if (nutrient.NutrientType == NutrientType.Carbohydrate)
            {
                response.Carbohydrates = (double)nutrient.Value!;
            }
        }

        private void SetValueWithDefaultType(DailyNeedsResponse response, NutrientOfDay nutrient)
        {
            double remainingCalories = response.Calories - (response.Proteins * 4 + response.Fats * 9 + response.Carbohydrates * 4);

            if (remainingCalories > 0)
            {
                if (nutrient.NutrientType == NutrientType.Protein)
                {
                    response.Proteins = remainingCalories / 4;
                }
                else if (nutrient.NutrientType == NutrientType.Fat)
                {
                    response.Fats = remainingCalories / 9;
                }
                else if (nutrient.NutrientType == NutrientType.Carbohydrate)
                {
                    response.Carbohydrates = remainingCalories / 4;
                }
            }
        }
    }
}
