using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;
using MealPlanService.Infrastructure.MongoDB;
using MealPlanService.Infrastructure.Projections;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;

namespace MealPlanService.Infrastructure.Repositories
{
    public class MealPlanRepository : BaseRepository<MealPlan>, IMealPlanRepository
    {
        public MealPlanRepository(
            MongoDBContext context)
            : base(context.MealPlans)
        {
        }

        public async Task<(List<MealPlanDTO>?, long)> GetAllAsync(MealPlanType? type, int? page, int? size)
        {
            var filter = type.HasValue
                ? Builders<MealPlan>.Filter.Eq(mp => mp.Type, type.Value)
                : Builders<MealPlan>.Filter.Empty;

            var totalCount = await _collection.CountDocumentsAsync(filter);

            var query = _collection
                .Find(filter)
                .Project(mp => new MealPlanDTO
                {
                    Id = mp.Id.ToString(),
                    Name = mp.Name,
                    Description = mp.Description,
                    Type = mp.Type
                });

            if (page.HasValue && size.HasValue)
            {
                var skip = (page.Value - 1) * size.Value;
                query = query.Skip(skip).Limit(size);
            }

            var mealPlans = await query.ToListAsync();

            return (mealPlans, totalCount);
        }

        public async Task<List<MealPlanDTO>> GetManyByIdsAsync(List<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                return new List<MealPlanDTO>();
            }

            var filter = Builders<MealPlan>.Filter.In(mp => mp.Id, ids);

            return await _collection
                .Find(filter)
                .Project(mp => new MealPlanDTO
                {
                    Id = mp.Id,
                    Name = mp.Name,
                    Description = mp.Description,
                    Type = mp.Type
                })
                .ToListAsync();
        }

    }
}
