using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;

namespace MarketingAsync.Redis.Cache
{
    public class CacheEntityEvent :
        IEventHandler<EntityCreatedEventData<CacheEntity>>,
        IEventHandler<EntityDeletedEventData<CacheEntity>>,
        IEventHandler<EntityUpdatedEventData<CacheEntity>>,
        ISingletonDependency
    {

        private readonly IRedisModelStrategy _model;

        public CacheEntityEvent()
        {
            _model = IocManager.Instance.Resolve<IRedisModelStrategy>();
        }

        /// <summary>
        /// 触发设置
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(EntityCreatedEventData<CacheEntity> eventData)
        {
            _model.SetModel(eventData.Entity.GetType().Name, eventData.Entity.Id.ToString(), eventData.Entity);
        }

        /// <summary>
        /// 触发删除
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(EntityDeletedEventData<CacheEntity> eventData)
        {
            CacheExtension.RemoveCache(eventData.Entity.GetType().Name, eventData.Entity.Id.ToString());
        }

        /// <summary>
        /// 触发修改
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(EntityUpdatedEventData<CacheEntity> eventData)
        {
            _model.SetModel(eventData.Entity.GetType().Name, eventData.Entity.Id.ToString(), eventData.Entity);
        }
    }

    public class EntityEvent :
      IEventHandler<EntityChangedEventData<CacheEntity>>, ISingletonDependency
    {

        public void HandleEvent(EntityChangedEventData<CacheEntity> eventData)
        {
            //所有操作到增上改的缓存全部清除 
            //CacheExtension.RemoveCache(eventData.Entity.GetType().Name, eventData.Entity.Id.ToString());

        }
    }
}