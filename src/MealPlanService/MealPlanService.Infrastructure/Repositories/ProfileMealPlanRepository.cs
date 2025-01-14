using MealPlanService.Core.Entities;
using MealPlanService.Infrastructure.MongoDB;
using MongoDB.Driver;

namespace MealPlanService.Infrastructure.Repositories
{
    public class ProfileMealPlanRepository : BaseRepository<ProfileMealPlan>
    {
        private readonly MongoDBContext _context;
        public ProfileMealPlanRepository(MongoDBContext context)
            : base(context.ProfileMealPlans)
        {
            _context = context;
        }

        public async Task<List<ProfileMealPlan>?> GetAllAsync(string profileId)
        {
            return await _collection.Find(ump => ump.ProfileId == profileId).ToListAsync();
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
