using MongoDB.Bson;
using MongoDB.Driver;

namespace MealPlanService.Infrastructure.Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        public BaseRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            ObjectId objectId;
            try
            {
                objectId = new ObjectId(id);
            }
            catch
            {
                return null;
            }
            return await _collection.Find(Builders<T>.Filter.Eq("_id", objectId)).FirstOrDefaultAsync();
        }

        public virtual async Task CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public virtual async Task UpdateAsync(T updatedEntity)
        {
            var objectId = new ObjectId(typeof(T).GetProperty("Id")!.GetValue(updatedEntity)!.ToString());

            await _collection.ReplaceOneAsync(
                Builders<T>.Filter.Eq("_id", objectId),
                updatedEntity
            );
        }


        public virtual async Task DeleteAsync(string id)
        {
            var objectId = new ObjectId(id);

            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", objectId));
        }
    }
}
