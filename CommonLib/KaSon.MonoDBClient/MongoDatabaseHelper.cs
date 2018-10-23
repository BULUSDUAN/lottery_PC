using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace KaSon.MonoDBClient
{
    public class MongoDatabaseHelper
    {
        private static string connectionString = "mongodb://1.0.3.6:27017";
        static IMongoDatabase mDB;
        MongoClient client;
        public MongoDatabaseHelper(string dbName = "elinklog")
        {
            client = new MongoClient(connectionString);
            // var server = client.GetServer();
            mDB = client.GetDatabase(dbName);


        }
        public IList<BsonDocument> MapReduce(string _map, string _reduce, string collName, string fileter, string outputResult, int timeOutSeconds = 60, bool isDel = false)
        {

            IList<BsonDocument> result = new List<BsonDocument>();
            var map = new BsonJavaScript(_map);

            var reduce = new BsonJavaScript(_reduce);
            //const key = this.Date.getFullYear();

            if (isDel && !string.IsNullOrEmpty(outputResult))
            {
                //删除结果集合
                mDB.DropCollection(outputResult);
            }
            var coll = mDB.GetCollection<BsonDocument>(collName);
            //var options = new MapReduceOptions<BsonDocument, BsonDocument>();//what should be TResult?

            //options.OutputOptions = MapReduceOutputOptions.Inline;
            //查询条件
            FilterDefinition<BsonDocument> _filter = null;
            if (!string.IsNullOrEmpty(fileter))
            {
                _filter = fileter;
            }

            //输出结果集合
            var output = MapReduceOutputOptions.Reduce(outputResult, nonAtomic: true);
            if (string.IsNullOrEmpty(outputResult))
            {
                output = MapReduceOutputOptions.Inline;
            }

            MapReduceOptions<BsonDocument, BsonDocument> options = new MapReduceOptions<BsonDocument, BsonDocument>
            {
                Filter = _filter,
                MaxTime = TimeSpan.FromSeconds(timeOutSeconds),
                OutputOptions = output,
                Verbose = true,

            };
            try
            {
                result = coll.MapReduce(map, reduce, options).ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred {ex.Message}");
            }

            return result;


        }


        public IList<BsonDocument> MapReduce(string _map, string _reduce, string collName, int timeOutSeconds = 60)
        {

            return this.MapReduce(_map, _reduce, collName, "", "", timeOutSeconds, false);


        }
        public IList<BsonDocument> MapReduce(string _map, string _reduce, string collName, string filter, int timeOutSeconds = 60)
        {

            return this.MapReduce(_map, _reduce, collName, filter, "", timeOutSeconds, false);


        }

        public IList<BsonDocument> Find(string collName, string filter)
        {

            var coll = mDB.GetCollection<BsonDocument>(collName);

            FilterDefinition<BsonDocument> _f = filter;
            return coll.Find<BsonDocument>(_f).ToList();

        }

        #region 测试部分
        //public static void test()
        //{

        //    var coll = mDB.GetCollection<ProtocolByPub>("publog");



        //    string mapFunction = @"function(){  
        //                            emit(this.cusid, this.price);  
        //                        };";

        //    string reduceFunction = @"function(cusid, prices){  
        //                            var total = 0;  
        //                            total = Array.sum(prices);  
        //                            return { sum: total };  
        //                        };";


        //    FilterDefinition<BsonDocument> filter = "{ x : { $regex : /ABC/ } }";
        //    var options = new MapReduceOptions<BsonDocument, ProtocolByPub>();//what should be TResult?
        //    options.Filter = filter;
        //    options.OutputOptions = MapReduceOutputOptions.Inline;
        //    var cusid_prices_results = coll.MapReduce<ProtocolByPub>(mapFunction, reduceFunction);
        //}
        //this.Date.getFullYear()
        //        private const string MapJs = @"function mapF() {
        //    const key = this.Date.getFullYear();
        //    const valuePerYear = { total: 1};

        //    emit(key, valuePerYear);
        //}; ";

        //        private const string ReduceJS = @"function reduceF(year, values) {
        //    var sum = 0;
        //    values.forEach(function(v) {
        //        sum += v.total;
        //    });
        //    return {total: NumberInt(sum)};
        //}";


        public void test1()
        {
            string MapJs = @"function mapF() {
    const key = this.firstName;
    const ages = this.age;

    emit(key, ages);
}; ";
            string ReduceJS = @"function reduceF(key, ages) {
    var sum = 0;
    ages.forEach(function(v) {
        sum += v.age;
    });
    return NumberInt(sum);
}";

            //string mongoConnectionString = "my-connection-string";

            IMongoCollection<BsonDocument> collection = mDB.GetCollection<BsonDocument>("test2");
            BsonJavaScript map = new BsonJavaScript(MapJs);
            BsonJavaScript reduce = new BsonJavaScript(ReduceJS);
            FilterDefinitionBuilder<BsonDocument> filterBuilder = new FilterDefinitionBuilder<BsonDocument>();
            var f = filterBuilder.Eq("age", 12);
            //     FilterDefinition<BsonDocument> filter = new BsonDocument{
            //  {"_id",12}
            //};
            MapReduceOptions<BsonDocument, BsonDocument> options = new MapReduceOptions<BsonDocument, BsonDocument>
            {
                Filter = f,
                MaxTime = TimeSpan.FromMinutes(1),
                OutputOptions = MapReduceOutputOptions.Reduce("Result", nonAtomic: true),
                Verbose = true
            };
            try
            {
                var result = collection.MapReduce(map, reduce, options).ToList();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred {ex.Message}");
            }
        }
        /// <summary>
        /// 测试通过
        /// </summary>
        public void test2()
        {
            var map = new BsonJavaScript(@"
     function mapF(){
    const key = this.status;
    const valuePerYear = 1;
        emit(key,valuePerYear)
    };");

            var reduce = new BsonJavaScript(@"
     function reduceF(key, values){
      var sum = 0;
    values.forEach(function(v) {
        sum += v;
    });
    return sum;

        
    };");
            //const key = this.Date.getFullYear();
            map = @"function mapF() {
    const key = this.Date.getFullYear();
    const valuePerYear = 1;

    emit(key, valuePerYear);
}; ";

            reduce = @"function reduceF(year, values) {
    var sum = 0;
    values.forEach(v => {
        sum += v;
    });
    return {total: NumberInt(sum)};
}";
            var coll = mDB.GetCollection<BsonDocument>("test2");
            //var options = new MapReduceOptions<BsonDocument, BsonDocument>();//what should be TResult?

            //options.OutputOptions = MapReduceOutputOptions.Inline;
            FilterDefinitionBuilder<BsonDocument> filterBuilder = new FilterDefinitionBuilder<BsonDocument>();
            var f = filterBuilder.Eq("Value", 14);
            MapReduceOptions<BsonDocument, BsonDocument> options = new MapReduceOptions<BsonDocument, BsonDocument>
            {
                Filter = f,
                MaxTime = TimeSpan.FromMinutes(1),
                OutputOptions = MapReduceOutputOptions.Inline,
                Verbose = true,

            };
            try
            {
                var result = coll.MapReduce(map, reduce, options).ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred {ex.Message}");
            }

        }
        public void test3()
        {
            var coll = mDB.GetCollection<BsonDocument>("aaa");
            var list = coll.Find(new BsonDocument("age", "12")).ToList<BsonDocument>();
            foreach (var item in list)
            {
                Console.WriteLine(item["age"]);
            }
        }
    }
    #endregion
}
