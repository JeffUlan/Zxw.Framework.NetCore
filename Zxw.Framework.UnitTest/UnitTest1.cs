using System;
using System.Data;
using System.Threading;
using AspectCore.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zxw.Framework.NetCore.Attributes;
using Zxw.Framework.NetCore.Cache;
using Zxw.Framework.NetCore.CodeGenerator;
using Zxw.Framework.NetCore.DbContextCore;
using Zxw.Framework.NetCore.Extensions;
using Zxw.Framework.NetCore.Helpers;
using Zxw.Framework.NetCore.IoC;
using Zxw.Framework.NetCore.Options;
using Zxw.Framework.UnitTest.TestModels;

namespace Zxw.Framework.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        #region Test methods for Oracle

        [TestMethod]
        public void TestGetDataTableForOracle()
        {
            BuildServiceForOracle();
            var dbContext = AspectCoreContainer.Resolve<IDbContextCore>();
            var dt1 = dbContext.GetCurrentDatabaseAllTables();
            Assert.IsNotNull(dt1);
            foreach (DataRow row in dt1.Rows)
            {
                var dt2 = dbContext.GetTableColumns(row["TableName"].ToString());
                Assert.IsNotNull(dt2);
            }
        }

        [TestMethod]
        public void TestGetDataTableListForOracle()
        {
            BuildServiceForOracle();
            var dbContext = AspectCoreContainer.Resolve<IDbContextCore>();
            var tables = dbContext.GetCurrentDatabaseTableList();
            Assert.IsNotNull(tables);
        }

        [TestMethod]
        public void TestGenerateEntitiesForOracle()
        {
            BuildServiceForOracle();
            CodeGenerator.GenerateAllCodesFromDatabase(true);
        }

        #endregion

        #region Test methods for PostgreSQL

        [TestMethod]
        public void TestGetDataTableForPostgreSql()
        {
            BuildServiceForPostgreSql();
            var factory = AspectCoreContainer.Resolve<DbContextFactory>();
            var test = new TestController(factory);
            test.Run();
            var dbContext = AspectCoreContainer.Resolve<IDbContextCore>();
            var dt1 = dbContext.GetCurrentDatabaseAllTables();
            Assert.IsNotNull(dt1);
            foreach (DataRow row in dt1.Rows)
            {
                var dt2 = dbContext.GetTableColumns(row["TableName"].ToString());
                Assert.IsNotNull(dt2);
            }
        }

        [TestMethod]
        public void TestGetDataTableListForPostgreSql()
        {
            BuildServiceForPostgreSql();
            var dbContext = AspectCoreContainer.Resolve<IDbContextCore>();
            var tables = dbContext.GetCurrentDatabaseTableList();
            Assert.IsNotNull(tables);
        }

        [TestMethod]
        public void TestGenerateEntitiesForPostgreSql()
        {
            BuildServiceForPostgreSql();
            CodeGenerator.GenerateAllCodesFromDatabase(true);
        }

        #endregion

        #region Test methods for SQL Server

        [TestMethod]
        public void TestGetDataTableForSqlServer()
        {
            BuildServiceForSqlServer();
            var dbContext = AspectCoreContainer.Resolve<IDbContextCore>();
            var dt1 = dbContext.GetCurrentDatabaseAllTables();
            Assert.IsNotNull(dt1);
            foreach (DataRow row in dt1.Rows)
            {
                var dt2 = dbContext.GetTableColumns(row["TableName"].ToString());
                Assert.IsNotNull(dt2);
            }
        }

        [TestMethod]
        public void TestGetDataTableListForSqlServer()
        {
            BuildServiceForSqlServer();
            var dbContext = AspectCoreContainer.Resolve<IDbContextCore>();
            var tables = dbContext.GetCurrentDatabaseTableList();
            Assert.IsNotNull(tables);
        }

        [TestMethod]
        public void TestGenerateEntitiesForSqlServer()
        {
            BuildServiceForSqlServer();
            CodeGenerator.GenerateAllCodesFromDatabase(true);
        }

        #endregion

        #region Test methods for MySQL

        [TestMethod]
        public void TestGetDataTableForMySql()
        {
            BuildServiceFoMySql();
            var dbContext = AspectCoreContainer.Resolve<IDbContextCore>();
            var dt1 = dbContext.GetCurrentDatabaseAllTables();
            Assert.IsNotNull(dt1);
            foreach (DataRow row in dt1.Rows)
            {
                var dt2 = dbContext.GetTableColumns(row["TableName"].ToString());
                Assert.IsNotNull(dt2);
            }
        }

        [TestMethod]
        public void TestGetDataTableListForMySql()
        {
            BuildServiceFoMySql();
            var dbContext = AspectCoreContainer.Resolve<IDbContextCore>();
            var tables = dbContext.GetCurrentDatabaseTableList();
            Assert.IsNotNull(tables);
        }

        [TestMethod]
        public void TestGenerateEntitiesForMySql()
        {
            BuildServiceFoMySql();
            CodeGenerator.GenerateAllCodesFromDatabase(true);
        }

        #endregion

        #region Test methods for Redis

        [TestMethod]
        public void TestCsRedisClient()
        {
            BuildServiceForSqlServer();
            var dbContext = AspectCoreContainer.Resolve<IDbContextCore>();
            RedisHelper.Set("test_cache_key", JsonConvertor.Serialize(dbContext.GetCurrentDatabaseTableList()),
                10 * 60);
            Thread.Sleep(2000);
            var content = DistributedCacheManager.Get("test_cache_key");
            Assert.IsNotNull(content);
        }
        
        #endregion

        [TestMethod]
        public void TestForMongoDb()
        {
            BuildServiceForMongoDB();
            var context = AspectCoreContainer.Resolve<IDbContextCore>();
            Assert.IsTrue(context.Add(new MongoModel()
            {
                Age = 28,
                Birthday = Convert.ToDateTime("1999-01-22"),
                IsBitch = false,
                UniqueId = Guid.NewGuid().ToString("N"),
                UserName = "����з",
                Wage = 100000000
            }) > 0);
            context.Dispose();
        }

        #region public methods

        public IServiceProvider BuildServiceForPostgreSql()
        {
            IServiceCollection services = new ServiceCollection();
            //������ע��EF������
            services = RegisterPostgreSqlContext(services);
            services.Configure<CodeGenerateOption>(options =>
            {
                options.OutputPath = "F:\\Test\\PostgreSQL";
                options.ModelsNamespace = "Zxw.Framework.Website.Models";
                options.IRepositoriesNamespace = "Zxw.Framework.Website.IRepositories";
                options.RepositoriesNamespace = "Zxw.Framework.Website.Repositories";
                options.ControllersNamespace = "Zxw.Framework.Website.Controllers";
            });
            services.AddOptions();
            
            return AspectCoreContainer.BuildServiceProvider(services); //����AspectCore.Injector
        }

        public IServiceProvider BuildServiceForSqlServer()
        {
            IServiceCollection services = new ServiceCollection();

            //������ע��EF������
            services = RegisterSqlServerContext(services);
            services.Configure<CodeGenerateOption>(options =>
            {
                options.OutputPath = "F:\\Test\\SqlServer";
                options.ModelsNamespace = "Zxw.Framework.Website.Models";
                options.IRepositoriesNamespace = "Zxw.Framework.Website.IRepositories";
                options.RepositoriesNamespace = "Zxw.Framework.Website.Repositories";
                options.ControllersNamespace = "Zxw.Framework.Website.Controllers";
            });
            services.UseCsRedisClient(
                "127.0.0.1:6379,abortConnect=false,connectRetry=3,connectTimeout=3000,defaultDatabase=1,syncTimeout=3000,version=3.2.100,responseTimeout=3000");
            services.AddOptions();
            return AspectCoreContainer.BuildServiceProvider(services); //����AspectCore.Injector
        }

        public IServiceProvider BuildServiceFoMySql()
        {
            IServiceCollection services = new ServiceCollection();

            //������ע��EF������
            services = RegisterMySqlContext(services);
            services.Configure<CodeGenerateOption>(options =>
            {
                options.OutputPath = "F:\\Test\\MySQL";
                options.ModelsNamespace = "Zxw.Framework.Website.Models";
                options.IRepositoriesNamespace = "Zxw.Framework.Website.IRepositories";
                options.RepositoriesNamespace = "Zxw.Framework.Website.Repositories";
                options.ControllersNamespace = "Zxw.Framework.Website.Controllers";
            });
            services.AddOptions();
            return AspectCoreContainer.BuildServiceProvider(services); //����AspectCore.Injector
        }

        public IServiceProvider BuildServiceForSqLite()
        {
            IServiceCollection services = new ServiceCollection();

            //������ע��EF������
            services = RegisterSqLiteContext(services);
            services.Configure<CodeGenerateOption>(options =>
            {
                options.OutputPath = "F:\\Test\\SQLite";
                options.ModelsNamespace = "Zxw.Framework.Website.Models";
                options.IRepositoriesNamespace = "Zxw.Framework.Website.IRepositories";
                options.RepositoriesNamespace = "Zxw.Framework.Website.Repositories";
                options.ControllersNamespace = "Zxw.Framework.Website.Controllers";
            });
            services.AddOptions();
            return AspectCoreContainer.BuildServiceProvider(services); //����AspectCore.Injector
        }
        public IServiceProvider BuildServiceForMongoDB()
        {
            IServiceCollection services = new ServiceCollection();

            //������ע��EF������
            services = RegisterMongoDbContext(services);
            services.AddOptions();
            return AspectCoreContainer.BuildServiceProvider(services); //����AspectCore.Injector
        }
        public IServiceProvider BuildServiceForOracle()
        {
            IServiceCollection services = new ServiceCollection();

            services.Configure<CodeGenerateOption>(options =>
            {
                options.OutputPath = "F:\\Test\\Oracle";
                options.ModelsNamespace = "Zxw.Framework.UnitTest.Models";
                options.IRepositoriesNamespace = "Zxw.Framework.UnitTest.IRepositories";
                options.RepositoriesNamespace = "Zxw.Framework.UnitTest.Repositories";
                options.ControllersNamespace = "Zxw.Framework.UnitTest.Controllers";
            });
            //������ע��EF������
            services = RegisterOracleDbContext(services);
            services.AddOptions();
            return AspectCoreContainer.BuildServiceProvider(services); //����AspectCore.Injector
        }
        /// <summary>
         /// ע��SQLServer������
         /// </summary>
         /// <param name="services"></param>
         /// <returns></returns>
        public IServiceCollection RegisterSqlServerContext(IServiceCollection services)
        {
            services.Configure<DbContextOption>(options =>
            {
                options.ConnectionString =
                    "initial catalog=NetCoreDemo;data source=192.168.42.103;password=xtyf;User id=xtyf;MultipleActiveResultSets=True;";
                //options.ModelAssemblyName = "Zxw.Framework.Website.Models";
            });
            services.AddScoped<IDbContextCore, SqlServerDbContext>(); //ע��EF������
            return services;
        }

        /// <summary>
        /// ע��MySQL������
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceCollection RegisterMySqlContext(IServiceCollection services)
        {
            services.Configure<DbContextOption>(options =>
            {
                options.ConnectionString =
                    "Server=127.0.0.1;Database=test; User ID=root;Password=123456;port=3306;CharSet=utf8;pooling=true;";
                //options.ModelAssemblyName = "Zxw.Framework.Website.Models";
            });
            services.AddScoped<IDbContextCore, MySqlDbContext>(); //ע��EF������
            return services;
        }

        /// <summary>
        /// ע��PostgreSQL������
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceCollection RegisterPostgreSqlContext(IServiceCollection services)
        {
            services.Configure<DbContextOption>(options =>
            {
                options.ConnectionString =
                    "User ID=postgres;Password=123456;Host=localhost;Port=5432;Database=ZxwPgDemo;Pooling=true;";
                //options.ModelAssemblyName = "Zxw.Framework.Website.Models";
            });

            var factory = DbContextFactory.Instance;
            factory.AddDbContext<PostgreSQLDbContext>("db1", new DbContextOption(){ConnectionString = "User ID=postgres;Password=123456;Host=localhost;Port=5432;Database=ZxwPgDemo;Pooling=true;" });
            factory.AddDbContext<SqlServerDbContext>("db2", new DbContextOption() { ConnectionString = "" });
            factory.AddDbContext<MongoDbContext>("db3", new DbContextOption() { ConnectionString = "" });
            services.AddSingleton(factory);

            services.AddScoped<IDbContextCore, PostgreSQLDbContext>(); //ע��EF������
            return services;
        }

        /// <summary>
        /// ע��SQLite������
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceCollection RegisterSqLiteContext(IServiceCollection services)
        {
            services.Configure<DbContextOption>(options =>
            {
                options.ConnectionString = "Data Source=F:\\EF6.db;Version=3;";
                //options.ModelAssemblyName = "Zxw.Framework.Website.Models";
            });
            services.AddScoped<IDbContextCore, SQLiteDbContext>(); //ע��EF������
            return services;
        }

        /// <summary>
        /// ע��SQLite������
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceCollection RegisterMongoDbContext(IServiceCollection services)
        {
            services.Configure<DbContextOption>(options =>
            {
                options.ConnectionString = "mongodb://localhost";
                options.ModelAssemblyName = "Zxw.Framework.UnitTest";
            });
            services.AddScoped<IDbContextCore, MongoDbContext>(); //ע��EF������
            return services;
        }

        /// <summary>
        /// ע��Oracle������
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceCollection RegisterOracleDbContext(IServiceCollection services)
        {
            services.Configure<DbContextOption>(options =>
            {
                options.ConnectionString = "DATA SOURCE=127.0.0.1:1234/testdb;USER ID=test;PASSWORD=123456;PERSIST SECURITY INFO=True;Pooling=True;Max Pool Size=100;Incr Pool Size=2;";
            });
            services.AddScoped<IDbContextCore, OracleDbContext>(); //ע��EF������
            return services;
        }
        #endregion
    }
}
