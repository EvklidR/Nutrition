using UserService.Application.DTOs.Responses.Profile;
using UserService.Contracts.Broker;
using UserService.Contracts.DataAccess.Repositories;
using UserService.Contracts.Exceptions;
using UserService.Domain.Enums;

namespace UserService.Application.UseCases.Queries
{
    public class CalculateDailyNutrientsHandler : IQueryHandler<CalculateDailyNutrientsQuery, DailyNeedsResponse>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMealPlanService _mealPlanService;

        public CalculateDailyNutrientsHandler(IProfileRepository profileRepository, IMealPlanService mealPlanService)
        {
            _profileRepository = profileRepository;
            _mealPlanService = mealPlanService;
        }

        public async Task<DailyNeedsResponse> Handle(
            CalculateDailyNutrientsQuery request,
            CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByIdAsync(request.profileId, cancellationToken);

            if (profile == null)
            {
                throw new NotFound("Profile not found");
            }

            if (request.userId != profile!.UserId)
            {
                throw new Unauthorized("Owner isn't valid");
            }

            var response = new DailyNeedsResponse();

            if (!profile.ThereIsMealPlan)
            {
                response = CalculateDailyMacros(profile);
            }
            else
            {
                var resp = await _mealPlanService.GetDailyNeedsByMealPlanAsync(
                    profile.Id,
                    profile.Weight,
                    CalculateDailyCalories(profile));

                response = new DailyNeedsResponse
                {
                    Calories = resp.Calories,
                    Proteins = resp.Proteins,
                    Fats = resp.Fats,
                    Carbohydrates = resp.Carbohydrates,
                };
            }
            
            response.DesiredGlassesOfWater = profile.DesiredGlassesOfWater;

            return response;
        }

        // calculated using the Harris-Benedict formula
        private double CalculateBMR(Domain.Entities.Profile profile)
        {
            if (profile.Gender == Gender.Male)
            {
                return 88.36 +
                    13.4 * profile.Weight +
                    4.8 * profile.Height -
                    5.7 * CalculateAge(profile.Birthday);
            }
            else
            {
                return 447.6 +
                    9.2 * profile.Weight +
                    3.1 * profile.Height -
                    4.3 * CalculateAge(profile.Birthday);
            }
        }

        private double CalculateDailyCalories(Domain.Entities.Profile profile)
        {
            return Math.Round(CalculateBMR(profile) * GetActivityMultiplier(profile.ActivityLevel), 2);
        }


        private DailyNeedsResponse CalculateDailyMacros(Domain.Entities.Profile profile)
        {
            var calories = CalculateDailyCalories(profile);

            DailyNeedsResponse result = new DailyNeedsResponse
            {
                Calories = Math.Round(calories, 2),
                Proteins = Math.Round(0.3 * calories / 4, 2),
                Fats = Math.Round(0.3 * calories / 9, 2),
                Carbohydrates = Math.Round(0.4 * calories / 4, 2)
            };

            return result;
        }


        private int CalculateAge(DateOnly birthday)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            int age = today.Year - birthday.Year;

            if (today < birthday.AddYears(age))
            {
                age--;
            }

            return age;
        }

        private double GetActivityMultiplier(ActivityLevel activityLevel)
        {
            return activityLevel switch
            {
                ActivityLevel.Sedentary => 1.2,
                ActivityLevel.Low => 1.375,
                ActivityLevel.Medium => 1.55,
                ActivityLevel.High => 1.725,
                ActivityLevel.VeryHigh => 1.9,
                _ => 0
            };
        }

    }
}

