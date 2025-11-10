using System;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class StreamResult : IResult
    {
        private byte[] mdata;
        private string mfilename;


        public StreamResult(string filename, byte[] data)
        {
            mfilename = filename;
            mdata = data;
        }

        public async Task Output(TAAction ac)
        {
            int idx = mfilename.LastIndexOf(".");
            string ext = mfilename.Substring(idx);
            ac.Context.Response.ContentType = FileContentType.GetMimeType(ext);
            ac.Context.Response.AppendHeader("Access-Control-Expose-Headers", "download-filename");
            ac.Context.Response.AppendHeader("download-filename", mfilename);
            ac.Context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + mfilename);
            if (mdata != null)
            {
                await ac.Context.Response.BinaryWriteAsync(mdata);
            }
        }
    }
}
