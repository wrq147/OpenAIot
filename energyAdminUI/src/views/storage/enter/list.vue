<template>
  <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form :model="queryParams" class="biaodan" ref="queryForm" :inline="true">

          <el-form-item label="入库方式" prop="EnterMethod">
            <el-select v-model="queryParams.EnterMethod" placeholder="请选择入库方式">
              <el-option label="出库" value="0"></el-option>
              <el-option label="退货" value="1"></el-option>
              <el-option label="调拨" value="2"></el-option>
              <el-option label="手动" value="3"></el-option>
            </el-select>
          </el-form-item>

          <el-form-item label="关键词" prop="Key">
            <el-input placeholder="请输入搜索的入库单号、物品名称" v-model="queryParams.Key" clearable>
            </el-input>
          </el-form-item>

          <el-form-item label="来源企业" prop="FromCompany">
            <el-input placeholder="请输入来源企业" v-model="queryParams.FromCompany" clearable>
            </el-input>
          </el-form-item>

          <el-form-item label="入库时间">
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
            <el-col :span="1.5">
              <span style="margin-right:15px;font-size: 14px;color: #1890ff;">当前仓库</span>
              <el-select v-model="queryParams.ToHouseId" placeholder="请选择仓库" clearable @change="changeHouse">
                <el-option v-for="hsitem in houseOptions" :key="hsitem.Id" :label="hsitem.StoreName"
                  :value="hsitem.Id"></el-option>
              </el-select>
            </el-col>
            <el-col :span="1.5" v-hasPermi="['/ProducerService/Parts/List','/IoTService/IotDevice/ListPage']">
              <el-button type="primary" plain @click="handleAdd">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">新增</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="primary" plain @click="handleScanPut">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                  <span style="margin-left: 6px">扫码入库</span>
                </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="primary" plain :loading="exportLoading" @click="handleExport">
                <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                <span style="margin-left:6px">导出</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-tabs v-model="activeName" type="card" @tab-click="tabClick" style="margin-top:25px;">
          <el-tab-pane label="全部" name="-1"></el-tab-pane>
          <el-tab-pane label="待提交" name="0"></el-tab-pane>
          <el-tab-pane label="待审批" name="1"></el-tab-pane>
          <el-tab-pane label="入库成功" name="2"></el-tab-pane>
          <el-tab-pane label="待退货" name="3"></el-tab-pane>
          <el-tab-pane label="已退货" name="4"></el-tab-pane>
        </el-tabs>
        <el-table border v-loading="loading" :data="tbList" class="data_table" :header-cell-style="cellSty"
          style="width:100%" row-key="Id">

          <el-table-column label="入库单号" align="center" prop="StockNumber" width="160"></el-table-column>
          <el-table-column label="所出仓库">
            <template slot-scope="scope">
              <template v-if="scope.row.FromHouseName == ''">无</template>
              <template v-else>
                【{{ scope.row.FromName }}】的{{ scope.row.FromHouseName }}
              </template>
            </template>
          </el-table-column>
          <el-table-column label="所入仓库" prop="ToHouseName"></el-table-column>
          <el-table-column label="入库方式" align="center" width="100">
            <template slot-scope="scope">
              <span>{{ EnterMethodName(scope.row.EnterMethod) }}</span>
            </template>
          </el-table-column>

          <el-table-column label="制单人" align="center" prop="Creater" width="120"></el-table-column>
          <el-table-column label="入库时间" align="center" width="160">
            <template slot-scope="scope">
              {{ parseTime(scope.row.InDate) }}
            </template>
          </el-table-column>
          <el-table-column label="单据状态" align="center" width="100">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status == 0" type="warning">待提交</el-tag>
              <el-tag v-else-if="scope.row.Status == 1" type="warning">待审批</el-tag>
              <el-tag v-else-if="scope.row.Status == 2" type="success">入库成功</el-tag>
              <el-tag v-else-if="scope.row.Status == 3" type="danger">待退货</el-tag>
              <el-tag v-else-if="scope.row.Status == 4" type="info">已退货</el-tag>

            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="150">
            <template slot-scope="scope">
              <el-link type="danger" v-if="scope.row.Status == 0 || scope.row.Status == 1"
                @click="onCancel(scope.row)">撤销</el-link>
              <el-link type="danger" v-if="scope.row.EnterMethod!=1&&scope.row.Status == 2 || scope.row.EnterMethod!=1&&scope.row.Status == 3"
                @click="onCancel(scope.row)">退货</el-link>
              <el-link type="primary" v-if="scope.row.Status == 0" @click="onSubmit(scope.row)"
                style="margin-left:15px;">入库</el-link>
              <el-link type="info" @click="onDetail(scope.row)" style="margin-left:15px;">详情</el-link>
            </template>
          </el-table-column>

        </el-table>
        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize"
          @pagination="getList" />
      </div>

      <!-- 扫码入库弹窗 -->
      <addScanPut ref="addScanPut" @confirm="getList" />
      <ManualPile ref="pieDlg" @confirm="getList"></ManualPile>
      <EnterReturn ref="returnDlg" @confirm="getList"></EnterReturn>
      <EnterDetail ref="detailDlg" @lk="$refs.leaveDlg.openDialog($event)"></EnterDetail>
      <LeaveDetail ref="leaveDlg" @lk="$refs.detailDlg.openDialog($event)"></LeaveDetail>
    </div>
  </div>
</template>
    
<script>
import {
  enterList,
  exportEnterExcel,
  cancelEnter
} from "@/api/storage/stock";

import { resizeTableCon } from "@/mixins/resizeTableCon";
import ManualPile from "./manualpile.vue";
import addScanPut from "./addScanPut.vue";
import EnterReturn from "./return.vue";
import EnterDetail from "./detail.vue";
import LeaveDetail from "../leave/detail.vue";
import { houseList } from "@/api/storage/house";
export default {
  name: "EnterList",
  mixins: [resizeTableCon],
  components: { ManualPile, EnterReturn, EnterDetail, LeaveDetail, addScanPut },
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
        ToHouseId: undefined
      },
      // 总条数
      total: 0,
      houseOptions: [],
    };
  },
  async created() {
    let rsp = await houseList({ showAll: true, Status: "1" });
    this.houseOptions = rsp.data.List;
    if (this.houseOptions.length > 0) {
      let hidd = this.$cache.local.get("curhouseId-" + this.$store.state.user.orgId + "-" + this.$store.state.user.uid);
      this.queryParams.ToHouseId = this.houseOptions[0].Id;
      if (hidd != null && hidd != '' && this.houseOptions.some(x => x.Id == hidd)) {
        this.queryParams.ToHouseId = hidd;
      }
    }
    await this.getList();
  },
  methods: {
    EnterMethodName(way) {
      switch (way) {
        case 0:
          return "出库";
        case 1:
          return "退货";
        case 2:
          return "调拨";
        case 3:
          return "手动";
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
      this.queryParams['Status'] = this.activeName;
      enterList(this.addDateRange(this.queryParams, this.dateRange)).then(response => {
        this.tbList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
    },
    changeHouse(val) {
      this.$cache.local.set("curhouseId-" + this.$store.state.user.orgId + "-" + this.$store.state.user.uid, val);
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
    handleAdd(row) {
      this.$refs.pieDlg.openDialog(null, this.queryParams.ToHouseId);
    },
     /** 扫码入库按钮操作 */
    handleScanPut() {
      this.$refs.addScanPut.openDialog()
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
    async onCancel(row) {
      try {
        if (row.Status == 0 || row.Status == 1) {
          await this.$modal.confirm("确定撤销入库单'" + row.StockNumber + "'？（此操作不可逆）");
          await cancelEnter({ Id: row.Id });
          this.getList();
          if (row.Status == 1 || row.EnterMethod != 3) {
            this.$refs.returnDlg.openDialog(row.Id);
          }
        }
        else {
          this.$refs.returnDlg.openDialog(row.Id);
        }
      }
      catch { }
    },
    onSubmit(row) {
      this.$refs.pieDlg.openDialog(row.Id);
    },
    onDetail(row) {
      this.$refs.detailDlg.openDialog(row.Id);
    }

  }
};
</script>
<style lang="scss"></style>