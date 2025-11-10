using Common.Share;


namespace StorageService.Model
{
    public class In_DetailRecordPage : BaseQueryParam
    {
        /// <summary>
        /// 0为今天、1为昨天、7为近7天、30为近30天
        /// </summary>
        public int? Day { get; set; }
    }
}
