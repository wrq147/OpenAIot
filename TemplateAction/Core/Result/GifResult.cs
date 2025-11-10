using System;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class GifResult : IResult
    {
        private byte[] mData;
        public byte[] Data
        {
            get { return mData; }
        }

        public GifResult(byte[] pngdata)
        {
            mData = pngdata;
        }

        public async Task Output(TAAction ac)
        {
            ac.Context.Response.ContentType = "image/gif";
            if (mData != null)
            {
                await ac.Context.Response.BinaryWriteAsync(mData);
            }
        }
    }
}
