using System.Reflection;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase; 
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using MarketingAsync.Dapper.Framework;
using MarketingAsync.Mongodb;
using MarketingAsync.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MarketingAsync.Tests
{
    [DependsOn(
        typeof(MarketingAsyncRedisModule),
        typeof(MarketingAsyncMongodbCoreModule),
        typeof(MarketingAsyncDapperModule),
        typeof(MarketingAsyncApplicationModule),
        typeof(AbpTestBaseModule)
        )]
    public class MarketingAsyncTestModule : AbpModule
    {
        public override void PreInitialize()
        {
           // Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
            SetupInMemoryDb();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MarketingAsyncTestModule).GetAssembly());
        }

        private void SetupInMemoryDb()
        {
            var services = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );

          
        }
    }
}