using System;
using System.IO;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    public interface IRequestFile
    {
        long ContentLength { get; }
        string ContentType { get; }
        string FileName { get; }
        Stream OpenReadStream();
        void SaveAs(string filename);
        Task SaveAsAsync(string filename);
    }
}
