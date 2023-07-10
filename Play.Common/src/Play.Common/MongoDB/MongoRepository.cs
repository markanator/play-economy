using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Play.Common;

namespace Play.Common.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;
        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }
        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            // search for all documents in the collection
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }
        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).ToListAsync();
        }
        public async Task<T> GetAsync(Guid id)
        {
            // create a filter to look for the document
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            // fetch the single document matching the filter
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task CreateAsync(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            // insert the item into the collection
            await dbCollection.InsertOneAsync(item);
        }
        public async Task UpdateAsync(T itemToUpdate)
        {
            if (itemToUpdate == null)
            {
                throw new ArgumentNullException(nameof(itemToUpdate));
            }
            // create a filter to look for the document
            FilterDefinition<T> filter = filterBuilder.Eq(existingItem => existingItem.Id, itemToUpdate.Id);
            // replace the document with the item
            await dbCollection.ReplaceOneAsync(filter, itemToUpdate);
        }
        public async Task RemoveAsync(Guid id)
        {
            // create a filter to look for the document
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            // delete the document
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}