using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public class MongoDbStorage : IResultStorage
    {
        private const string DatabaseName = "ResultIssues";
        private const string CollectionName = "IssueCollection";

        private readonly IMongoCollection<IssueResults> _collection;

        public MongoDbStorage()
        {
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress("mongo")
            };

            var mongo = new MongoClient(settings);
            var db = mongo.GetDatabase(DatabaseName);

            _collection = db.GetCollection<IssueResults>(CollectionName);
        }

        public Task UpdateAsync(IssueResults results, CancellationToken token)
        {
            var options = new UpdateOptions
            {
                BypassDocumentValidation = false
            };

            var updated = Builders<IssueResults>.Update
                .Set(t => t.Issues, results.Issues);

            return _collection.UpdateOneAsync(t => t.Id == results.Id, updated, options, token);
        }

        public async Task<IssueResults> CreateAsync(CancellationToken token)
        {
            var options = new InsertOneOptions
            {
                BypassDocumentValidation = false
            };

            var results = new IssueResults
            {
                Id = Guid.NewGuid(),
                Issues = Array.Empty<Issue>()
            };

            await _collection.InsertOneAsync(results, options, token);

            return results;
        }

        public async Task<IssueResults> GetAsync(Guid guid, CancellationToken token)
        {
            var results = await _collection.FindAsync(i => i.Id == guid, cancellationToken: token);

            var m1 = await results.MoveNextAsync();

            if (!m1)
            {
                return null;
            }

            var list = results.Current.ToList();

            if (list.Count != 1 || await results.MoveNextAsync(token))
            {
                return null;
            }

            return list[0];
        }
    }
}
