<template>
  <div style="padding:20px 20px 0 20px;height:100%" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form :model="queryParams" class="biaodan" ref="queryForm" :inline="true">
          <el-form-item label="出库方式" prop="LeaveMethod">
            <el-select v-model="queryParams.LeaveMethod" placeholder="请选择出库方式">
              <el-option label="出库" value="0"></el-option>
              <el-option label="退货" value="1"></el-option>
              <el-option label="调拨" value="2"></el-option>
              <el-option label="领料" value="3"></el-option>
            </el-select>
          </el-form-item>

          <el-form-item label="关键词" prop="Key">
            <el-input placeholder="请输入搜索的出库单号、物品名称" v-model="queryParams.Key" clearable></el-input>
          </el-form-item>

          <el-form-item label="目标企业" prop="ToCompany">
            <el-input placeholder="请输入目标企业" v-model="queryParams.ToCompany" clearable></el-input>
          </el-form-item>

          <el-form-item label="出库时间">
            <el-date-picker v-model="dateRange" style="width:232px" value-format="yyyy-MM-dd" type="daterange"
              range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
          </el-form-item>
          <!-- <el-col class="float_right" :span="24"> -->
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
          </el-form-item>
          <!-- </el-col> -->
        </el-form>
      </div>
      <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="7.5" style="margin-top:10px">
              <span style="margin-right:15px;font-size: 14px;color: #1890ff;">当前仓库</span>
              <el-select v-model="queryParams.FromHouseId" placeholder="请选择仓库" clearable @change="changeHouse">
                <el-option v-for="hsitem in houseOptions" :key="hsitem.Id" :label="hsitem.StoreName"
                  :value="hsitem.Id"></el-option>
              </el-select>
            </el-col>
            <el-col :span="3.5" style="margin-top:10px">
              <el-button type="primary" plain @click="handleAdd">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">新增出库单</span>
              </el-button>
            </el-col>
            <el-col :span="3.5" style="margin-top:10px">
              <el-button type="primary" plain @click="handleAddTransfer">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">新增调拔单</span>
              </el-button>
            </el-col>
            <el-col :span="3.5" style="margin-top:10px">
              <el-button type="primary" plain @click="handleAddReturnDlg">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">新增退货单</span>
              </el-button>
            </el-col>
            <el-col :span="2.5" style="margin-top:10px">
              <el-button type="primary" plain :loading="exportLoading" @click="handleExport">
                <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                <span style="margin-left:6px">导出</span>
              </el-button>
            </el-col>
            <el-col :span="4.5" style="margin-top:10px">
              <div class="button_con">
                <el-button type="warning" plain @click="jumpApplist" :disabled="!queryParams.FromHouseId">
                  <i class="el-icon-tickets"></i>
                  <span style="margin-left:6px">待出库申请单</span>
                </el-button>
                <div class="tips" v-if="applyWait>0">{{applyWait}}</div>
              </div>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-tabs v-model="activeName" type="card" @tab-click="tabClick" style="margin-top:25px;">
          <el-tab-pane label="全部" name="-1"></el-tab-pane>
          <el-tab-pane label="待提交" name="0"></el-tab-pane>
          <el-tab-pane label="待审批" name="1"></el-tab-pane>
          <el-tab-pane label="出库成功" name="2"></el-tab-pane>
          <el-tab-pane label="出库失败" name="3"></el-tab-pane>
          <el-tab-pane label="已取消" name="4"></el-tab-pane>
        </el-tabs>
        <el-table border v-loading="loading" :data="tbList" class="data_table" :header-cell-style="cellSty"
          style="width:100%" row-key="Id">
          <el-table-column label="出库单号" align="center" prop="StockNumber" width="160"></el-table-column>
          <el-table-column label="所出仓库" prop="FromHouseName">
          </el-table-column>
          <el-table-column label="所入仓库" prop="ToHouseName">
            <template slot-scope="scope">
              <template v-if="scope.row.ToHouseId == ''">无</template>
              <template v-else>
                【{{ scope.row.ToName }}】{{ scope.row.ToHouseName!=""?("的"+scope.row.ToHouseName ):""}}
              </template>
            </template>
          </el-table-column>
          <el-table-column label="出库方式" align="center" width="100">
            <template slot-scope="scope">
              <span>{{ LeaveMethodName(scope.row.LeaveMethod) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="单据状态" align="center" width="100">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status == 0" type="warning">待提交</el-tag>
              <el-tag v-else-if="scope.row.Status == 1" type="warning">待审批</el-tag>
              <el-tag v-else-if="scope.row.Status == 2" type="success">出库成功</el-tag>
              <el-tag v-else-if="scope.row.Status == 3" type="danger">出库失败</el-tag>
              <el-tag v-else-if="scope.row.Status == 4" type="info">已取消</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="制单人" align="center" prop="Creater" width="120"></el-table-column>
          <el-table-column label="出库时间" align="center" width="160">
            <template slot-scope="scope">
              {{ parseTime(scope.row.OutDate) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="160">
            <template slot-scope="scope">
              <el-link type="danger" v-if="scope.row.Status == 0 || scope.row.Status == 1"
                @click="onCancel(scope.row)">撤销</el-link>
                <el-link type="primary" v-if="scope.row.Status == 0" @click="onSubmit(scope.row)"
                style="margin-left:15px;">出库</el-link>
              <el-link type="primary" @click="onDetail(scope.row)" style="margin-left:15px;">详情</el-link>
            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize"
          @pagination="getList" />
      </div>

      <ManualPile ref="pieDlg" @confirm="getList"></ManualPile>
      <Transfer ref="transDlg" @confirm="getList"></Transfer>
      <Returns ref="returnDlg" @confirm="getList"></Returns>
      <LeaveDetail ref="detailDlg" @lk="$refs.enterDlg.openDialog($event)"></LeaveDetail>
      <EnterDetail ref="enterDlg" @lk="$refs.detailDlg.openDialog($event)"></EnterDetail>
    </div>
  </div>
</template>
    
<script>
import { leaveList, exportEnterExcel, cancelLeave } from "@/api/storage/stock";
import {applyWaitCount} from "@/api/storage/apply";
import ManualPile from "./manualpile.vue";
import Transfer from "./transfer.vue";
import Returns from "./return.vue";
import LeaveDetail from "./detail.vue";
import EnterDetail from "../enter/detail.vue";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { houseList } from "@/api/storage/house";
export default {
  name: "LeaveList",
  mixins: [resizeTableCon],
  components: { ManualPile,Transfer, LeaveDetail,EnterDetail,Returns },
  data() {
    return {
      activeName: "-1",
      // 导出遮罩层
      exportLoading: false,
      // 遮罩层
      loading: true,
      // 显示搜索条件
      showSearch: true,
      // 入库日期范围
      dateRange: [],
      // 表格树数据
      tbList: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        EnterMethod: undefined,
        Key: undefined,
        FromCompany: undefined,
        FromHouseId: undefined
      },
      // 总条数
      total: 0,
      houseOptions: [],
      applyWait:0
    };
  },
  async created() {
    let rsp = await houseList({showAll:true, Status: "1" });
    this.houseOptions = rsp.data.List;
    if (this.houseOptions.length > 0) {
      let hidd = this.$cache.local.get("curhouseId-" + this.$store.state.user.orgId + "-" + this.$store.state.user.uid);
      this.queryParams.FromHouseId = this.houseOptions[0].Id;
      if (hidd != null && hidd != '' && this.houseOptions.some(x => x.Id == hidd)) {
        this.queryParams.FromHouseId = hidd;
      }

    }
    await this.getList();
    if(this.queryParams.FromHouseId){
      this.getapplyWaitCount()
    }
    
  },
  methods: {
    getapplyWaitCount(){
      //获取当前仓库出库成功，待申请的数量
      applyWaitCount({houseId:this.queryParams.FromHouseId}).then(res=>{
        this.applyWait=res.data
      })
    },
    jumpApplist(){
      const stockId = this.queryParams.FromHouseId;
      this.$router.push("/jxc/leave/stockApplylist?stockId=" + stockId);
    },
    LeaveMethodName(way) {
      switch (way) {
        case 0:
          return "出库";
        case 1:
          return "退货";
        case 2:
          return "调拨";
        case 3:
          return "领料";
      }
      return "";
    },
    tabClick(tab, event) {
      this.activeName = tab.name;
      this.getList();
    },
    /** 查询列表 */
    getList() {
      this.loading = true;
      this.queryParams["Status"] = this.activeName;
      leaveList(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          this.tbList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    changeHouse(val) {
      this.$cache.local.set("curhouseId-" + this.$store.state.user.orgId + "-" + this.$store.state.user.uid, val);
      if(this.queryParams.FromHouseId){
        this.getapplyWaitCount()
      }
      this.getList();
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.resetForm("queryForm");
      this.handleQuery();
    },
    /** 新增按钮操作 */
    handleAdd() {
      this.$refs.pieDlg.openDialog(null, this.queryParams.FromHouseId);
    },
    handleAddTransfer(){//调拨
      this.$refs.transDlg.openDialog(null, this.queryParams.FromHouseId);
    },
    handleAddReturnDlg(){//退货
      this.$refs.returnDlg.openDialog(null);
    },
    /** 导出按钮操作 */
    handleExport() {
      const queryParams = this.queryParams;
      this.$modal
        .confirm("是否将数据导出为Excel文件？")
        .then(() => {
          this.exportLoading = true;
          return exportEnterExcel(queryParams);
        })
        .then(response => {
          this.exportLoading = false;
        })
        .catch(() => { });
    },
    onDetail(row) {
      this.$refs.detailDlg.openDialog(row.Id);
    },
    onCancel(row) {
      this.$modal.confirm("确定撤销出库单'" + row.StockNumber + "'？（此操作不可逆）").then(async rs => {
        if (rs == "confirm") {
          await cancelLeave({ id: row.Id });
          this.getList();
        }
      });

    },
    onSubmit(row) {
      if(row.LeaveMethod&&row.LeaveMethod!=3){
        this.$refs.returnDlg.openDialog(row.Id);
      }else{
        if(row.LeaveMethod==3){
          this.$refs.pieDlg.openDialog(row.Id);
        }else{
          this.$refs.pieDlg.openDialog(row.Id);
        }
        
      }
    },
  }
};
</script>
<style lang="scss">
.button_con{
  position: relative;
  .tips{
    background-color: red;
    width: 16px;
    height: 16px;
    text-align: center;
    line-height: 16px;
    border-radius: 10px;
    color: #ffffff;
    position: absolute;
    top: -8px;
    left: 7px;
    font-size: 12px;
  }
}
</style>