using Common.Share;
using Microsoft.Extensions.Options;
using Milvus.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Business
{
    public class MilvusBLL
    {
        private readonly MilvusClient _client;

        public MilvusBLL(IOptions<IoTAIOption> option)
        {
            _client = new MilvusClient(new Uri(option.Value.MilvusUrl), database: option.Value.MilvusDatabase);
        }

        public virtual async Task<BusResponse<string>> CreateMemberCollection()
        {
            // 检查集合是否已存在
            var exists = await _client.HasCollectionAsync("MemCollect");
            if (exists)
            {
                return BusResponse<string>.Error(111, "成员向量表已存在");
            }

            // 定义集合结构 - 假设我们使用128维的向量
            var schema = new CollectionSchema();
            schema.Fields.Add(FieldSchema.Create<long>("vec_id", isPrimaryKey: true, autoId: true));
            schema.Fields.Add(FieldSchema.CreateVarchar("h_id", maxLength: 128));
            schema.Fields.Add(FieldSchema.Create<long>("mem_id"));
            schema.Fields.Add(FieldSchema.CreateFloatVector("mem_vector", dimension: 128));

            var collection = await _client.CreateCollectionAsync("MemCollect", schema);
            // 创建索引以提高搜索性能
            await collection.CreateIndexAsync("mem_vector", IndexType.AutoIndex, SimilarityMetricType.Cosine);
            await collection.CreateIndexAsync(fieldName: "h_id", indexType: IndexType.AutoIndex);
            await collection.CreateIndexAsync(fieldName: "mem_id", indexType: IndexType.AutoIndex);

            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<int>> DelFromIdsCollection(long[] ids)
        {
            MilvusCollection collection = _client.GetCollection("MemCollect");
            var res = await collection.DeleteAsync("vec_id in [" + string.Join(',', ids) + "]");
            if (res.DeleteCount > 0)
            {
                return BusResponse<int>.Success((int)res.DeleteCount);
            }
            else
            {
                return BusResponse<int>.Error(111, "删除成员向量失败");
            }
        }
        public virtual async Task<BusResponse<int>> DelFromMemberCollection(long uid)
        {
            MilvusCollection collection = _client.GetCollection("MemCollect");
            var res = await collection.DeleteAsync("mem_id==" + uid);
            if (res.DeleteCount > 0)
            {
                return BusResponse<int>.Success((int)res.DeleteCount);
            }
            else
            {
                return BusResponse<int>.Error(111, "删除成员向量失败");
            }
        }
        public virtual async Task<BusResponse<long>> InsertToMemberCollection(long uid, string houseid, float[] data)
        {
            List<string> houseIds = new List<string>();
            List<long> memIds = new List<long>();
            List<ReadOnlyMemory<float>> memVector = new();
            houseIds.Add(houseid);
            memIds.Add(uid);
            memVector.Add(data);

            MilvusCollection collection = _client.GetCollection("MemCollect");
            MutationResult result = await collection.InsertAsync(
            new FieldData[]
            {
                FieldData.CreateVarChar("h_id", houseIds),
                FieldData.Create<long>("mem_id", memIds),
                FieldData.CreateFloatVector("mem_vector", memVector),
            });
            if (result.InsertCount > 0)
            {

                return BusResponse<long>.Success(result.Ids.LongIds[0]);
            }
            else
            {
                return BusResponse<long>.Error(111, "添加成员向量失败");
            }
        }
        public virtual async Task<BusResponse<List<long>>> Search(float[] vectors, string houseId, float score = 0.8f)
        {
            SearchParameters searchParameters = new();
            searchParameters.OutputFields.Add("mem_id");
            if (!string.IsNullOrEmpty(houseId))
            {
                searchParameters.Expression = "h_id='" + houseId + "'";
            }

            MilvusCollection collection = _client.GetCollection("MemCollect");
            var results = await collection.SearchAsync(
                vectorFieldName: "mem_vector",
                vectors: new ReadOnlyMemory<float>[] { vectors },
                SimilarityMetricType.Cosine,
                limit: 10, searchParameters);
            List<long> tmplist = new List<long>();
            foreach (var item in results.FieldsData)
            {
                if (item.FieldName == "mem_id")
                {
                    tmplist.AddRange((item as FieldData<long>).Data);
                }
            }
            HashSet<long> newhs = new HashSet<long>();
            List<long> newlist = new List<long>();
            for (int i = 0; i < tmplist.Count; i++)
            {
                if (results.Scores[i] > score && !newhs.Contains(tmplist[i]))
                {
                    newhs.Add(tmplist[i]);
                }
            }
            return BusResponse<List<long>>.Success(newhs.ToList());
        }
        public virtual async Task<BusResponse<List<long>>> Search(float[] vectors, List<string> houseIds, float score = 0.8f)
        {
            SearchParameters searchParameters = new();
            searchParameters.OutputFields.Add("mem_id");
            if (houseIds != null && houseIds.Count > 0)
            {
                // 过滤掉空/空白的houseId，避免无效条件
                var validHouseIds = houseIds.Where(id => !string.IsNullOrWhiteSpace(id)).ToList();
                if (validHouseIds.Count > 0)
                {
                    // 拼接 OR 条件：h_id='id1' OR h_id='id2' OR ...
                    var idConditions = validHouseIds.Select(id => $"h_id='{id}'");
                    searchParameters.Expression = string.Join(" OR ", idConditions);
                }
            }


            MilvusCollection collection = _client.GetCollection("MemCollect");
            var results = await collection.SearchAsync(
                vectorFieldName: "mem_vector",
                vectors: new ReadOnlyMemory<float>[] { vectors },
                SimilarityMetricType.Cosine,
                limit: 10, searchParameters);
            List<long> tmplist = new List<long>();
            foreach (var item in results.FieldsData)
            {
                if (item.FieldName == "mem_id")
                {
                    tmplist.AddRange((item as FieldData<long>).Data);
                }
            }
            HashSet<long> newhs = new HashSet<long>();
            List<long> newlist = new List<long>();
            for (int i = 0; i < tmplist.Count; i++)
            {
                if (results.Scores[i] > score && !newhs.Contains(tmplist[i]))
                {
                    newhs.Add(tmplist[i]);
                }
            }
            return BusResponse<List<long>>.Success(newhs.ToList());
        }
    }
}
