using GetOption.Infrastructure.Utils;
using MongoDB.Driver;
using GetOption.Core.Implementations;
using GetOption.Core.Interface;
using GetOption.Core.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetOption.Infrastructure.Implementations
{
    public class MongoRepository<T> : RepositoryBase<T> where T : class, IEntity
    {

        public IMongoDatabase mongoDatabase;
        string tableName = "";

        public MongoRepository(IUnitOfWork _unitOfWork,MongoConnection connection) : this(typeof(T).Name,connection,_unitOfWork)
        {

        }

        public MongoRepository(string _tableName,MongoConnection connection,IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
            if (connection.Database == null)
            {
                throw new Exception("Database connection parameteter error");
            }
            mongoDatabase = connection.Database;
            tableName = _tableName;
        }

        public override Task<List<T>> GetEntitiesAsync(GetOption<T> option = null)
        {
            option = option ?? new GetOption<T>();
            GetOptionToDocumentQuery<T> documentQuery = option.ToDocumentQuery();
            return mongoDatabase.GetCollection<T>(this.tableName).Find(documentQuery.FilterDefinition)
                .Project(documentQuery.ProjectionDefinition)
                .Sort(documentQuery.SortDefinition)
                .Skip(documentQuery.PaginationDefinition.skip)
                .Limit(documentQuery.PaginationDefinition.limit).ToListAsync();
        }

        public override Task<T> GetEntityAsync(string entityId, GetOption<T> option = null)
        {
            option = option ?? new GetOption<T>();
            option.SearchOption = new SearchOption<T>() { Expression = new PropertySearchExpression() { Operator = "=", Property = nameof(IEntity.Id), Value = entityId } };
            GetOptionToDocumentQuery<T> documentQuery = option.ToDocumentQuery();
            return mongoDatabase.GetCollection<T>(this.tableName).Find(documentQuery.FilterDefinition).Limit(1).Project(documentQuery.ProjectionDefinition).SingleOrDefaultAsync();
        }

        protected override Task<CrudResult<string>> PersistAddEntityAsync(T entity)
        {
            TaskCompletionSource<CrudResult<string>> tcs = new TaskCompletionSource<CrudResult<string>>();
            mongoDatabase.GetCollection<T>(this.tableName).InsertOneAsync(entity).ContinueWith(resultTask => {
                if (resultTask.IsFaulted)
                {
                    MongoCommandException mongoException = resultTask.Exception?.InnerException as MongoCommandException;
                    if (mongoException != null && mongoException.CodeName == "OperationNotSupportedInTransaction")
                    {
                        try
                        {
                            mongoDatabase.CreateCollection(tableName);
                            //List<CreateIndexModel<User>> indexModelList = new List<CreateIndexModel<User>>();
                            //var emailIndexOption = new CreateIndexOptions();
                            //emailIndexOption.Unique = true;
                            //var emailIndexKey = Builders<User>.IndexKeys.Ascending(user => user.Email);
                            //var emailIndexModel = new CreateIndexModel<User>(emailIndexKey, emailIndexOption);
                            //indexModelList.Add(emailIndexModel);

                            ////adding examId of UserExams field as Index
                            //var examIdIndexKey = Builders<User>.IndexKeys.Ascending(new StringFieldDefinition<User>($"{nameof(User.UserExams)}.{nameof(UserExam.ExamId)}"));
                            //var examIdIndexModel = new CreateIndexModel<User>(examIdIndexKey);
                            //indexModelList.Add(examIdIndexModel);

                            ////adding testId of UserTests field as Index
                            //var testIdIndexKey = Builders<User>.IndexKeys.Ascending(new StringFieldDefinition<User>($"{nameof(User.UserTests)}.{nameof(UserTest.TestId)}"));
                            //var testIdIndexModel = new CreateIndexModel<User>(testIdIndexKey);
                            //indexModelList.Add(testIdIndexModel);

                            ////adding subscriptionId of Subscriptions field as Index
                            //var subscriptionIdIndexKey = Builders<User>.IndexKeys.Ascending(new StringFieldDefinition<User>($"{nameof(User.Subscriptions)}.{nameof(Subscription.Id)}"));
                            //var subscriptionIdIndexModel = new CreateIndexModel<User>(subscriptionIdIndexKey);
                            //indexModelList.Add(subscriptionIdIndexModel);

                            //adding all the indexes. 
                            //mongoTransaction.Database.GetCollection<User>(this.tableName).Indexes.CreateMany(indexModelList);
                            //mongoTransaction.Database.GetCollection<User>(this.tableName).InsertOne(mongoTransaction.SessionHandle, entity);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }

                    tcs.SetException(resultTask.Exception);

                }
                else
                    tcs.SetResult(CrudResult<string>.Success(entity.Id));

            });
            return tcs.Task;
        }

        protected override Task<CrudResult<bool>> PersistDeleteEntityAsync(T entity)
        {
            return mongoDatabase.GetCollection<T>(this.tableName).DeleteOneAsync(user => user.Id == entity.Id).ContinueWith(resultTask =>
            {
                if (!resultTask.IsFaulted)
                    return CrudResult<bool>.Success(true);
                throw resultTask.Exception;
            });
        }

        protected override Task<CrudResult<bool>> PersistUpdateEntityAsync(T entity)
        {

            return mongoDatabase.GetCollection<T>(this.tableName).ReplaceOneAsync(iEntity => iEntity.Id == entity.Id, entity).ContinueWith(resultTask =>
            {
                if (!resultTask.IsFaulted)
                    return CrudResult<bool>.Success(true);
                throw resultTask.Exception;
            });
        }

     
     

    }
}
