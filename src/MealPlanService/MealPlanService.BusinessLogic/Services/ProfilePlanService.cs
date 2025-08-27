using AutoMapper;
using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.BusinessLogic.Exceptions;
using MealPlanService.BusinessLogic.Models;
using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;
using MealPlanService.Infrastructure.Enums;
using MealPlanService.Infrastructure.RabbitMQService;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using MealPlanService.Infrastructure.Repositories.Models;
using MealPlanService.Infrastructure.Services.Interfaces;

namespace MealPlanService.BusinessLogic.Services
{
    public class ProfilePlanService
    {
        private readonly IProfileMealPlanRepository _profileMealPlanRepository;
        private readonly IMealPlanRepository _mealPlanRepository;

        private readonly MealPlanService _mealPlanService;

        private readonly IUserService _userService;
        private readonly IBrokerService _brokerService;

        private readonly IMapper _mapper;

        public ProfilePlanService(
            IProfileMealPlanRepository profileMealPlanRepository,
            IMealPlanRepository mealPlanRepository,
            MealPlanService mealPlanService,
            IUserService userService,
            IBrokerService brokerService,
            IMapper mapper)
        {
            _profileMealPlanRepository = profileMealPlanRepository;
            _mealPlanRepository = mealPlanRepository;
            _mealPlanService = mealPlanService;
            _userService = userService;
            _brokerService = brokerService;
            _mapper = mapper;
        }

        public async Task<List<Recommendation>?> GetRecommendations(string profileId)
        {
            var day = await _mealPlanService.GetCurrentDay(profileId);

            var userPlan = await _profileMealPlanRepository.GetActiveProfilePlan(profileId);

            var mealPlan = await _mealPlanRepository.GetByIdAsync(userPlan.MealPlanId);

            var recommendations = day.Recommendations;

            recommendations.AddRange(mealPlan!.Recommendations);

            return recommendations;
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

                var userPlan = await _profileMealPlanRepository.GetActiveProfilePlan(profileMealPlanDTO.ProfileId);

                if (userPlan != null)
                {
                    userPlan.EndDate = DateOnly.FromDateTime(DateTime.Now);
                    userPlan.IsActive = false;

                    await _profileMealPlanRepository.UpdateAsync(userPlan);
                }
                else
                {
                    await _brokerService.PublishMessageAsync(profileMealPlanDTO.ProfileId, QueueName.MealPlanChoosen, exchangeName: null);
                }

                var usersMealPlan = _mapper.Map<ProfileMealPlan>(profileMealPlanDTO);

                await _profileMealPlanRepository.CreateAsync(usersMealPlan);

                return usersMealPlan;
            }
            else
            {
                throw new BadRequest("You don't have access to this profile");
            }
        }

        public async Task<ProfileMealPlanWithDetailsDto?> GetActiveProfilePlanAsync(string userId, string profileId)
        {
            if (!await _userService.CheckProfileBelonging(userId, profileId))
            {
                throw new BadRequest("You don't have access to this profile");
            }

            var profilePlan = await _profileMealPlanRepository.GetActiveProfilePlan(profileId);

            if (profilePlan == null)
            {
                return null;
            }

            var mealPlan = await _mealPlanRepository.GetByIdAsync(profilePlan.MealPlanId);

            if (mealPlan == null)
            {
                throw new NotFound("Meal plan not found");
            }

            return new ProfileMealPlanWithDetailsDto
            {
                Id = profilePlan.Id,
                ProfileId = profilePlan.ProfileId,
                MealPlanId = profilePlan.MealPlanId,
                IsActive = profilePlan.IsActive,
                StartDate = profilePlan.StartDate,
                EndDate = profilePlan.EndDate,
                MealPlanName = mealPlan.Name,
                MealPlanDescription = mealPlan.Description
            };
        }


        public async Task<ProfileMealPlansResponse> GetProfilePlansAsync(
            string userId, 
            string profileId,
            PaginationParameters? paginationParameters,
            PeriodParameters? periodParameters)
        {
            if (!await _userService.CheckProfileBelonging(userId, profileId))
            {
                throw new BadRequest("You don't have access to this profile");
            }

            var (profileMealPlans, totalCount) = await _profileMealPlanRepository.GetAllAsync(profileId, paginationParameters, periodParameters);

            var mealPlanIds = profileMealPlans.Select(p => p.MealPlanId).Distinct().ToList();

            var mealPlans = await _mealPlanRepository.GetManyByIdsAsync(mealPlanIds);

            var mealPlanMap = mealPlans.ToDictionary(mp => mp.Id);

            var plans = profileMealPlans.Select(p =>
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

            var response = new ProfileMealPlansResponse
            {
                ProfileMealPlans = plans,
                TotalCount = totalCount
            };

            return response;
        }

        public async Task DeleteProfilePlansAsync(string profileId)
        {
            var (plans, _) = await _profileMealPlanRepository.GetAllAsync(profileId);

            foreach (var plan in plans)
            {
                await _profileMealPlanRepository.DeleteAsync(plan.Id);
            }
        }

        public async Task CompleteProfilePlanAsync(string userId, string profileId)
        {
            if (await _userService.CheckProfileBelonging(userId, profileId))
            {
                var userPlan = await _profileMealPlanRepository.GetActiveProfilePlan(profileId);

                if (userPlan == null)
                {
                    throw new NotFound("Active plan not found");
                }

                userPlan.EndDate = DateOnly.FromDateTime(DateTime.Now);
                userPlan.IsActive = false;

                await _profileMealPlanRepository.UpdateAsync(userPlan);

                await _brokerService.PublishMessageAsync(profileId, QueueName.MealPlanRevoked, exchangeName: null);
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

            response.Calories = Math.Round(response.Calories, 2);
            response.Carbohydrates = Math.Round(response.Carbohydrates, 2);
            response.Proteins = Math.Round(response.Proteins, 2);
            response.Fats = Math.Round(response.Fats, 2);

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
