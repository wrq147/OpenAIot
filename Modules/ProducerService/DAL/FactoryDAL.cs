using ProducerService.Model;
using Common;
using Common.Share;
using MyAccess.DB;
using System.Threading.Tasks;
using JiebaNet.Segmenter;

namespace ProducerService.DAL
{
    public class FactoryDAL : BaseRepository<MZ_Factory>
    {
        public virtual async Task<PageObject<Out_FactoryItem>> SelectByPage(In_FactoryList query)
        {

            return await new SqlBuilder(help).Query<Out_FactoryItem>().Append("select f.*,g.OrgName,g.AddressName,g.AddressDetail,g.Industry,g.Size from mz_factory f left join mz_org g on f.Id=g.Id where 1=1")
                .Then(!string.IsNullOrEmpty(query.FactoryName), sq =>
                {
                    var keys = new JiebaSegmenter().CutForSearch(query.FactoryName);
                    sq.Append(" and ").FullSearch("g.KeyWords", keys);
                })
                .Then(query.beginTime != null, sq => sq.Append(" and f.create_time >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and f.create_time <= ").AppendParam(query.endTime))
            .GeneratePageObjectAsync(query, "f.create_time desc");
        }
    }
}
