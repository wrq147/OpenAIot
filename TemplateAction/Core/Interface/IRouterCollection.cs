
namespace TemplateAction.Core
{
    public interface IRouterCollection<T> : IRouter where T:IRouter
    {
        /// <summary>
        /// 在最后添加
        /// </summary>
        /// <param name="router"></param>
        void Add(T router);
    }
}
