using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Reactive.Linq;
using System.Collections.Generic;


namespace Xarcade.Infrastructure.Repository
{
    public class RepositoryPortal
    {

        private readonly MongoClient client = new MongoClient("mongodb+srv://dane:pikentz213@bsg-xarcade-proto-f58v3.mongodb.net/test?retryWrites=true&w=majority");
        private readonly IMongoDatabase database = null;


        public RepositoryPortal()
        {
            database = client.GetDatabase("test");
        }

        /// <summary>
        /// Creates a document and inserts into specified collection
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public bool CreateDocument(string collectionName, BsonDocument doc)
        {
            IMongoCollection<BsonDocument> collection = null;
            var success = false;
            try
            {
                collection = database.GetCollection<BsonDocument>(collectionName);
                collection.InsertOne(doc);
                success = true;
            }catch(Exception)
            {
                success = false;
                //TODO log e
            }

            
            return success;
        } 

        /// <summary>
        /// Reads the entire collection and returns a list
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public List<BsonDocument> ReadCollection(string collectionName)
        {
            FilterDefinition<BsonDocument> filter = null;
            IMongoCollection<BsonDocument> collection = null;
            List<BsonDocument> result = null;
            try
            {
                collection = database.GetCollection<BsonDocument>(collectionName);
                filter = Builders<BsonDocument>.Filter.Empty;
                result = collection.Find(filter).ToList();
            }catch(Exception)
            {
                result = null;
                //TODO log e
            }finally
            {
            }
        
            return result;
        }
        
        /// <summary>
        /// Returns a list of documents in a collection
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<BsonDocument> ReadCollection(string collectionName,  FilterDefinition<MongoDB.Bson.BsonDocument> filter )
        {
            IMongoCollection<BsonDocument> collection = null;
            List<BsonDocument> result = null;

            try
            {
                collection = database.GetCollection<BsonDocument>(collectionName);
                result = collection.Find(filter).ToList();
            }catch(Exception)
            {
                result = null;
                //TODO log e
            }finally
            {

            }

            return result;
        }

        /// <summary>
        /// Gets total number of documents in a collection
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public long GetDocumentCount(string collectionName)
        {
            var filter = Builders<BsonDocument>.Filter.Empty;
            IMongoCollection<BsonDocument> collection = null;
            long count = 0;

            try
            {
                collection = database.GetCollection<BsonDocument>(collectionName);
                var result = collection.Find(filter).ToList();
                count = collection.CountDocumentsAsync(filter).GetAwaiter().GetResult();
            }catch(Exception)
            {
                count = 0;
                //TODO log e
            }finally
            {

            }

            return count;
        }

        /// <summary>
        /// Finds and returns a single document from the database. Returns null if no document is found
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public BsonDocument ReadDocument(string collectionName, FilterDefinition<MongoDB.Bson.BsonDocument> filter)
        {
            try 
            {
                var collection = database.GetCollection<BsonDocument>(collectionName);
                var result = collection.Find(filter);
                if(result.CountDocuments() < 1)
                {
                    //Document does not exist
                    return null;
                }
                else
                {
                    return result.Single();
                }
            }catch(Exception)
            {
                return null;
                //TODO log e
            }
        }

        /// <summary>
        /// Checks if a document with specified parameters exist
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool CheckExist(string collectionName, FilterDefinition<MongoDB.Bson.BsonDocument> filter)
        {
            try
            {
                var collection = database.GetCollection<BsonDocument>(collectionName);
                var result = collection.CountDocumentsAsync(filter).GetAwaiter().GetResult();
                if(result < 1)
                    return false;
                else
                    return true;
            }catch(Exception)
            {
                return false;
                //TODO log exception
            }

        }

        
        /// <summary>
        /// Updates a single field of a single document.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <param name="field"></param>
        /// <param name="newContent"></param>
        /// <returns>Returns true if operation succeeded</returns>
        public bool UpdateDocumentField(string collectionName, FilterDefinition<MongoDB.Bson.BsonDocument> filter, string field, string newContent)
        {
            var success = false;
            try
            {
                var collection = database.GetCollection<BsonDocument>(collectionName);
                var update = Builders<BsonDocument>.Update.Set(field, newContent).CurrentDate("lastModified");
                var result = collection.UpdateOne(filter, update);
                success = true;
            }catch(Exception)
            {
                success = false;
                //TODO log e
            }

            return success;
        }

        /// <summary>
        /// Deletes a document from the database
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool DeleteDocument(string collectionName, FilterDefinition<MongoDB.Bson.BsonDocument> filter)
        {
            var success = false;
            try
            {
                var collection = database.GetCollection<BsonDocument>(collectionName);
                var result = collection.DeleteOne(filter);
                success = true;
            }catch(Exception)
            {
                success = false;
                //TODO log e
            }
            return success;
        }

        /// <summary>
        /// Generates a Filter using a key value pair and an operator
        /// </summary>
        /// <param name="filter">Key value pair. String 1 is the key, string 2 is the value of said key</param>
        /// <param name="filterOperator">Operator to compare the filter. Use FilterOperator enum</param>
        /// <returns></returns>
        public FilterDefinition<MongoDB.Bson.BsonDocument> CreateFilter(KeyValuePair<string, string> filter, FilterOperator filterOperator)
        {
            FilterDefinition<MongoDB.Bson.BsonDocument> filterObject = null;
            switch(filterOperator)
            {
                case FilterOperator.EQUAL                  : filterObject = Builders<BsonDocument>.Filter.Eq(filter.Key,filter.Value); break;
                case FilterOperator.LESSERTHAN             : filterObject = Builders<BsonDocument>.Filter.Lt(filter.Key,filter.Value); break;
                case FilterOperator.GREATERTHAN            : filterObject = Builders<BsonDocument>.Filter.Gt(filter.Key,filter.Value); break;
                case FilterOperator.LESSERTHAN_OR_EQUALTO  : filterObject = Builders<BsonDocument>.Filter.Lte(filter.Key,filter.Value); break;
                case FilterOperator.GREATERTHAN_OR_EQUALTO : filterObject = Builders<BsonDocument>.Filter.Gte(filter.Key,filter.Value); break;
                default : filterObject = Builders<BsonDocument>.Filter.Eq(filter.Key,filter.Value); break;
            }
            return filterObject;
        }

        public FilterDefinition<MongoDB.Bson.BsonDocument> CreateFilter(KeyValuePair<string, long> filter, FilterOperator filterOperator)
        {
            FilterDefinition<MongoDB.Bson.BsonDocument> filterObject = null;
            switch(filterOperator)
            {
                case FilterOperator.EQUAL                  : filterObject = Builders<BsonDocument>.Filter.Eq(filter.Key,filter.Value); break;
                case FilterOperator.LESSERTHAN             : filterObject = Builders<BsonDocument>.Filter.Lt(filter.Key,filter.Value); break;
                case FilterOperator.GREATERTHAN            : filterObject = Builders<BsonDocument>.Filter.Gt(filter.Key,filter.Value); break;
                case FilterOperator.LESSERTHAN_OR_EQUALTO  : filterObject = Builders<BsonDocument>.Filter.Lte(filter.Key,filter.Value); break;
                case FilterOperator.GREATERTHAN_OR_EQUALTO : filterObject = Builders<BsonDocument>.Filter.Gte(filter.Key,filter.Value); break;
                default : filterObject = Builders<BsonDocument>.Filter.Eq(filter.Key,filter.Value); break;
            }
            return filterObject;
        }
        
        
        /// <summary>
        /// Generates a mongoDB-safe _id
        /// </summary>
        /// <returns></returns>
        public static ObjectId GenerateID()
        {
            return new ObjectId();
        }
    }
}

public enum FilterOperator
{
    EQUAL,
    LESSERTHAN,
    GREATERTHAN,
    LESSERTHAN_OR_EQUALTO,
    GREATERTHAN_OR_EQUALTO
}
