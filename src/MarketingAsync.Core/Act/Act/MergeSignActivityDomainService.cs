using System.Collections.Generic;
using Abp.Domain.Services;
using MarketingAsync.Act.Sqlserver;
using MarketingAsync.Act.User;

namespace MarketingAsync.Act.Act
{
    public class MergeSignActivityDomainService : IDomainService
    {
        private readonly ISignActivityRepository _signActRepositoryMongo;
        private readonly IUserSignActivityRepository _userSignActRepositoryMongo;
        public MergeSignActivityDomainService(ISignActivityRepository signActRepositoryMongo, IUserSignActivityRepository userSignActRepositoryMongo)
        {
            _signActRepositoryMongo = signActRepositoryMongo;
            _userSignActRepositoryMongo = userSignActRepositoryMongo;
        }


        /// <summary>
        /// 合并活动数据
        /// </summary>
        /// <param name="activity">活动数据</param>
        /// <param name="setEntity"></param>
        /// <returns></returns>
        public SignActivity MergeActData(SignPointActivity activity, List<SignPointSetEntity> setEntityList)
        {
            //SignActivity signAct=activity.MapTo
            return new SignActivity();
        }

    }
}