using System;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// Png图片结果
    /// </summary>
    public class PngResult : IResult
    {
        private byte[] mData;
        public byte[] Data
        {
            get { return mData; }
        }

        public PngResult(byte[] pngdata)
        {
            mData = pngdata;
        }

        public async Task Output(TAAction ac)
        {
            ac.Context.Response.ContentType = "image/png";
            if (mData != null)
            {
                await ac.Context.Response.BinaryWriteAsync(mData);
            }
        }
    }
}
