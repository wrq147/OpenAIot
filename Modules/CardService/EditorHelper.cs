using CardService.Model;
using Common.Share;
using System;

namespace CardService
{
    /// <summary>
    /// 编辑器专用
    /// </summary>
    public class EditorHelper
    {

        public static BusResponse<int> ValidateDetail(Tx_Pro_Item[] itemlist)
        {

            try
            {
                int videoCount = 0;
                int imgCount = 0;
                int txtLength = 0;
                foreach (Tx_Pro_Item it in itemlist)
                {
                    if (it.type == "image")
                    {
                        imgCount++;
                    }
                    else if (it.type == "video")
                    {
                        videoCount++;
                    }
                    else
                    {
                        txtLength += it.data.Length;
                    }
                }
                if (imgCount > 6)
                {
                    return BusResponse<int>.Error(115, "图片不能大于6张");
                }
                if (videoCount > 1)
                {
                    return BusResponse<int>.Error(116, "视频不能超过1个视频");
                }
                if (txtLength > 1000)
                {
                    return BusResponse<int>.Error(117, "文字不能超过800个字符");
                }
                return BusResponse<int>.Success();
            }
            catch
            {
                return BusResponse<int>.Error(113, "图文内容数据格式错误");
            }
        }
    }
}
