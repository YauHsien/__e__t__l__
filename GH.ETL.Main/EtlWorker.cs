using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GH.ETL.Main
{
    using CustIdNo = String;
    using SourceData = ISet<Data>;
    using TrasformedData = ISet<Data>;
    using CacheData = ISet<Data>;

    class EtlWorker
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EtlWorker> _logger;

        public EtlWorker(
            IConfiguration configuration,
            ILogger<EtlWorker> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [Obsolete]
        public Action DummyTask => async () => { };

        public Action Task
        {
            get
            {
                var conf = _configuration;
                Action task = async () => {
                    try
                    {
                        var worker = this;

                        _logger.LogInformation("Extracting...");
                        var sourceData = await worker.ExtractFromSource();
                        var transData = await worker.FindDataDiffFrom(sourceData);
                        await worker.CacheData(transData);
                        _logger.LogInformation("{0} records cached.", transData.Count());

                        _logger.LogInformation("Loading...");
                        var cachedData = await worker.GetCachedData();
                        if (cachedData.Count() == 0)
                        {
                            _logger.LogInformation("No record to load.");
                        }
                        else
                        {
                            var remote = new Remote(conf);
                            var sentTime = DateTime.UtcNow;
                            await remote.Update(cachedData, sentTime);

                            await worker.SetUploadTime(sentTime);

                            _logger.LogInformation("{0} records loaded.", cachedData.Count());
                        }

                        _logger.LogInformation("Task complete");
                    }
                    catch (Exception _ex)
                    {
                        _logger.LogError(_ex, "Task failed: {0}", _ex.Message);
                    }
                };
                return task;
            }
        }

        public async Task<SourceData> ExtractFromSource()
        {
            var connStr = _configuration.GetConnectionString("DbConnection");
            var sqlTemp = _configuration.GetSection("Parameter").GetValue<string>("SQL Template");
            var param = new { };

            using (var conn = new SqlConnection(connStr))
            {
                var result = await conn.QueryAsync<Data>(sqlTemp, param);

                return BuildDictionary(result);
            }
        }

        public async Task<TrasformedData> FindDataDiffFrom(SourceData sourceData)
        {
            var connStr = _configuration.GetConnectionString("LocalConnection");
            var sqlTemp = "select CustomerName, IdNo, Birthday from CacheData";
            var param = new { };

            CacheData cacheData = null;
            using (var conn = new SqliteConnection(connStr))
            {
                var result = await conn.QueryAsync<Data>(sqlTemp, param);

                cacheData = BuildDictionary(result);
            }

            return GetDiffData(sourceData, cacheData);
        }

        public async Task CacheData(TrasformedData trasformedData)
        {
            var connStr = _configuration.GetConnectionString("LocalConnection");
            var sqlTemp = "insert into CacheData (CustomerName, IdNo, Birthday) values (@CustomerName, @IdNo, @Birthday)";
            var param = trasformedData;

            using (var conn = new SqliteConnection(connStr))
            {
                await conn.ExecuteAsync(sqlTemp, param);
            }
        }

        public async Task<ISet<Data>> GetCachedData()
        {
            var connStr = _configuration.GetConnectionString("LocalConnection");
            var sqlTemp = "select CustomerName, IdNo, Birthday from CacheData where CacheTime is null";
            var param = new { };

            using (var conn = new SqliteConnection(connStr))
            {
                var result = await conn.QueryAsync<Data>(sqlTemp, param);

                return result.ToHashSet();
            }
        }

        public async Task SetUploadTime(DateTime time)
        {
            var connStr = _configuration.GetConnectionString("LocalConnection");
            var sqlTemp = "update CacheData set CacheTime = @DateTime where CacheTime is null";
            var param = new { DateTime = @time };

            using (var conn = new SqliteConnection(connStr))
            {
                await conn.ExecuteAsync(sqlTemp, param);
            }
        }

        private ISet<Data> BuildDictionary(IEnumerable<Data> data)
        {
            return data.ToList().ToHashSet<Data>();
        }

        private TrasformedData GetDiffData(SourceData sourceData, CacheData cacheData)
        {
            IEqualityComparer<Data> comparer = new DataComparer();
            return sourceData.Except(cacheData, comparer).ToList().ToHashSet();
        }
    }
}
