using AutoMapper;
using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.BusinessLogic.Exceptions;
using MealPlanService.BusinessLogic.Models;
using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;
using MealPlanService.Infrastructure.Repositories.Interfaces;

namespace MealPlanService.BusinessLogic.Services
{
    public class MealPlanService
    {
        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly IProfileMealPlanRepository _profileMealPlanRepository;
        private readonly IMapper _mapper;

        public MealPlanService(
            IMealPlanRepository mealPlanRepository,
            IProfileMealPlanRepository profileMealPlanRepository,
            IMapper mapper
)
        {
            _mealPlanRepository = mealPlanRepository;
            _profileMealPlanRepository = profileMealPlanRepository;
            _mapper = mapper;
        }

        public async Task<MealPlansResponse> GetMealPlansAsync(MealPlanType? type, int? page, int? size)
        {
            var (mealPlans, totalCount) = await _mealPlanRepository.GetAllAsync(type, page, size);

            MealPlansResponse mealPlansResponse = new MealPlansResponse 
            { 
                MealPlans = mealPlans,
                TotalCount = totalCount
            };

            return mealPlansResponse;
        }

        public async Task<MealPlan> CreateMealPlanAsync(CreateMealPlanDTO mealPlanDto)
        {
            var mealPlan = _mapper.Map<MealPlan>(mealPlanDto);

            await _mealPlanRepository.CreateAsync(mealPlan);

            return mealPlan;
        }

        public async Task DeleteMealPlanAsync(string mealPlanId)
        {
            var mealPlan = await _mealPlanRepository.GetByIdAsync(mealPlanId);

            if (mealPlan == null)
            {
                throw new NotFound("Meal plan not found");
            }

            await _mealPlanRepository.DeleteAsync(mealPlanId);

            var profilePlans = await _profileMealPlanRepository.GetByMealPlan(mealPlanId);

            foreach (var profilePlan in profilePlans) 
            {
                await _profileMealPlanRepository.DeleteAsync(profilePlan.Id);
            }

            //TODO: send message for broker
        }

        public async Task UpdateMealPlanAsync(MealPlan updatedMealPlan)
        {
            var existingMealPlan = await _mealPlanRepository.GetByIdAsync(updatedMealPlan.Id);

            if (existingMealPlan == null)
            {
                throw new NotFound("Meal plan not found");
            }

            var mealPlan = _mapper.Map<MealPlan>(updatedMealPlan);

            await _mealPlanRepository.UpdateAsync(updatedMealPlan);
        }

        internal async Task<MealPlanDay> GetCurrentDay(string profileId)
        {
            var userPlan = await _profileMealPlanRepository.GetActiveProfilePlan(profileId);

            if (userPlan == null)
            {
                throw new NotFound("User meal plan not found");
            }

            var mealPlan = await _mealPlanRepository.GetByIdAsync(userPlan.MealPlanId);

            if (mealPlan == null)
            {
                throw new NotFound("Meal plan not found");
            }

            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            int daysPassed = currentDate.DayNumber - userPlan.StartDate.DayNumber;
            var numberOfDay = daysPassed % mealPlan.Days.Count + 1;

            var day = mealPlan.Days.FirstOrDefault(d => d.DayNumber == numberOfDay);

            if (day == null)
            {
                throw new NotFound("Meal plan day not found");
            }

            return day;
        }
    }
}
