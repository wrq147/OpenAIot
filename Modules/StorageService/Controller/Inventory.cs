using AuthService.Controller;
using Common;
using Common.Share;
using StorageService.Business;
using StorageService.Model;
using MonitorService;
using TemplateAction.Core;
using TemplateAction.Route;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StorageService.Controller
{
    /// <summary>
    /// 盘点API
    /// </summary>
    public class Inventory : AbstractLoginedController
    {
        private InventoryBLL _inventoryBLL;
        public Inventory(InventoryBLL inventoryBLL)
        {
            _inventoryBLL = inventoryBLL;
        }

        /// <summary>
        /// 盘点列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Inventory>>> List(In_InventoryList query)
        {
            return this.Success(await _inventoryBLL.SelectList(query, false));
        }
        /// <summary>
        /// 盘点项列表
        /// </summary>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<Out_InventoryItem>>> ItemList(In_InventoryItem data)
        {
            return this.Success(await _inventoryBLL.SelectItemList(data));
        }
        /// <summary>
        /// 盘点任务
        /// </summary>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Inventory>>> Task(In_InventoryList query)
        {
            return this.Success(await _inventoryBLL.SelectList(query, true));
        }

        /// <summary>
        /// 记录盘点数据
        /// </summary>
        /// <param name="id">盘点Id</param>
        /// <param name="number">物品编号</param>
        /// <param name="count"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/Task")]
        [HttpPost]
        [Des("记录一条盘点数据")]
        public async Task<DefaultAjaxResult<int>> ConfirmItem(string id, string number, int count)
        {
            return (await _inventoryBLL.ConfirmItem(id, number, count)).ToAjaxResult();
        }
        /// <summary>
        /// 获取盘点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Inventory>> Info(string id)
        {
            return (await _inventoryBLL.Info(id)).ToAjaxResult();
        }

        /// <summary>
        /// 获取盘点项信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Out_InventoryItem>> ItemInfo(string id, string number)
        {
            return (await _inventoryBLL.SelectItemInfo(id, number)).ToAjaxResult();
        }

        /// <summary>
        /// 添加盘点
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("添加盘点单")]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Inventory data)
        {
            return (await _inventoryBLL.Add(data)).ToAjaxResult();
        }

        /// <summary>
        /// 修改盘点
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("修改盘点单")]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_Inventory data)
        {
            return (await _inventoryBLL.Edit(data)).ToAjaxResult();
        }

        /// <summary>
        /// 提交盘点单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("提交盘点单")]
        public async Task<DefaultAjaxResult<int>> Submit(string id)
        {
            return (await _inventoryBLL.Submit(id)).ToAjaxResult();
        }

        /// <summary>
        /// 开始初盘
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("开始初盘")]
        public async Task<DefaultAjaxResult<int>> Start(string[] ids)
        {
            return (await _inventoryBLL.Start(ids)).ToAjaxResult();
        }

        /// <summary>
        /// 开始复盘
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("开始复盘")]
        public async Task<DefaultAjaxResult<int>> Check(string[] ids)
        {
            return (await _inventoryBLL.Check(ids)).ToAjaxResult();
        }

        /// <summary>
        /// 取消盘点单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("取消盘点单")]
        public async Task<DefaultAjaxResult<int>> Cancel(string[] ids)
        {
            return (await _inventoryBLL.Cancel(ids)).ToAjaxResult();
        }

        /// <summary>
        /// 完成盘点单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("完成盘点单")]
        public async Task<DefaultAjaxResult<int>> Finish(string[] ids)
        {
            return (await _inventoryBLL.Finish(ids)).ToAjaxResult();
        }

        /// <summary>
        /// 修正盘点单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("修正盘点单")]
        public async Task<DefaultAjaxResult<int>> Repair(string id, List<MZ_InventoryItem> items)
        {
            return (await _inventoryBLL.Repair(id, items)).ToAjaxResult();
        }
        /// <summary>
        /// 删除盘点
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Remove(string[] ids)
        {
            var res = (await _inventoryBLL.Remove(ids)).ToAjaxResult();
            this.ServiceProvider.GetService<OperLogThread>().PushLog($"删除{ids.Length}条盘点单", res.ToString());
            return res;
        }

        /// <summary>
        /// 删除指定盘点项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("删除盘点项")]
        public async Task<DefaultAjaxResult<int>> DelItems(string id, List<string> ids)
        {
            return (await _inventoryBLL.DelItems(id, ids)).ToAjaxResult();
        }

        /// <summary>
        /// 添加盘点项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("添加盘点项")]
        public async Task<DefaultAjaxResult<int>> AddItems(string id, List<MZ_InventoryItem> list)
        {
            return (await _inventoryBLL.AddItems(id, list)).ToAjaxResult();
        }


        /// <summary>
        /// 添加全部
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("添加盘点项")]
        public async Task<DefaultAjaxResult<int>> AddItemsAll(In_AllItem data)
        {
            return (await _inventoryBLL.AddItemsAll(data)).ToAjaxResult();
        }


        /// <summary>
        /// 移除全部
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/StorageService/Inventory/List")]
        [HttpPost]
        [Des("删除盘点项")]
        public async Task<DefaultAjaxResult<int>> DelItemsAll(string id)
        {
            return (await _inventoryBLL.DelItemsAll(id)).ToAjaxResult();
        }
    }
}
