using MealPlanService.Core.Entities;
using MealPlanService.Infrastructure.MongoDB;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using MealPlanService.Infrastructure.Repositories.Models;
using MongoDB.Driver;

namespace MealPlanService.Infrastructure.Repositories
{
    public class ProfileMealPlanRepository : BaseRepository<ProfileMealPlan>, IProfileMealPlanRepository
    {
        private readonly MongoDBContext _context;
        public ProfileMealPlanRepository(MongoDBContext context)
            : base(context.ProfileMealPlans)
        {
            _context = context;
        }

        public async Task<(List<ProfileMealPlan>, long)> GetAllAsync(
            string profileId, 
            PaginationParameters? paginationParameters, 
            PeriodParameters? periodParameters)
        {
            var filter = periodParameters == null ?
                Builders<ProfileMealPlan>.Filter.Empty :
                Builders<ProfileMealPlan>.Filter.Where(profileMealPlan => 
                    profileMealPlan.StartDate <= periodParameters.EndDate &&
                    profileMealPlan.EndDate >= periodParameters.StartDate);

            var totalCount = await _collection.CountDocumentsAsync(filter);

            var query = _collection.Find(filter);

            if (paginationParameters != null)
            {
                query.Skip((paginationParameters.Page - 1) * paginationParameters.PageSize).Limit(paginationParameters.PageSize);
            }

            return (await query.ToListAsync(), totalCount);
        }

        public async Task<ProfileMealPlan?> GetActiveProfilePlan(string profileId)
        {
            return await _collection.Find(ump => (ump.IsActive && ump.ProfileId == profileId)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProfileMealPlan>> GetByMealPlan(string mealPlanId)
        {
            return await _collection.Find(ump => ump.MealPlanId == mealPlanId).ToListAsync();
        }
    }
}
