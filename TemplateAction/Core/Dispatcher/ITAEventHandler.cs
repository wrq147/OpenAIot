using System.Threading.Tasks;

namespace TemplateAction.Core
{
    public interface ITAEventHandler
    {
        Task OnEventAsync<T>(T evt) where T : class;
    }
}
