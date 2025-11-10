using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 文本结果
    /// </summary>
    public class TextResult : IResult
    {
        private string mContentType;
        private string mContent;
        public string ContentType
        {
            get { return mContentType; }
            set { mContentType = value; }
        }


        public TextResult(string content)
        {
            mContentType = "text/plain;charset=UTF-8";
            mContent = content;
        }
        public override string ToString()
        {
            return mContent;
        }
        public async Task Output(TAAction ac)
        {
            ac.Context.Response.ContentType = mContentType;
            await ac.Context.Response.WriteAsync(mContent);
        }
    }
}
