<template>
  <div style="padding:10px 10px 0 10px; height: 100%" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form :model="queryParams" class="biaodan" ref="queryForm" :inline="true">
          <el-form-item label="关键词" prop="Key">
            <el-input placeholder="请输入搜索的出库申请单号、物品名称" v-model="queryParams.Key" clearable></el-input>
          </el-form-item>

          <el-form-item label="申请时间">
            <el-date-picker v-model="dateRange" style="width: 232px" value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
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
            <el-button style="font-size: 12px" type="warning" plain icon="el-icon-close" @click="handleClose">关闭</el-button>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <!-- <el-tabs v-model="activeName" type="card" @tab-click="tabClick" style="margin-top:25px;">
            <el-tab-pane label="全部" name="-1"></el-tab-pane>
            <el-tab-pane label="待提交" name="0"></el-tab-pane>
            <el-tab-pane label="待审批" name="1"></el-tab-pane>
            <el-tab-pane label="申请成功" name="2"></el-tab-pane>
            <el-tab-pane label="申请失败" name="3"></el-tab-pane>
            <el-tab-pane label="已取消" name="4"></el-tab-pane>
          </el-tabs> -->
        <el-table border v-loading="loading" :data="tbList" class="data_table" :header-cell-style="cellSty" style="width: 100%" row-key="Id">
          <el-table-column label="出库申请单号" align="center" prop="ApplyNumber" width="160"></el-table-column>
          <el-table-column label="出库仓库" prop="House">
            <template slot-scope="scope">
              <span>{{scope.row.House ? scope.row.House.StoreName : ""}}</span>
            </template>
          </el-table-column>
          <el-table-column label="申请类型" align="center" width="100">
            <template slot-scope="scope">
              <span>{{ LeaveMethodName(scope.row.ApplyType) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="单据状态" align="center" width="100">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status == 0" type="warning">待提交</el-tag>
              <el-tag v-else-if="scope.row.Status == 1" type="warning">待审批</el-tag >
              <el-tag v-else-if="scope.row.Status == 2" type="success">申请成功</el-tag>
              <el-tag v-else-if="scope.row.Status == 3" type="danger">申请失败</el-tag>
              <el-tag v-else-if="scope.row.Status == 4" type="info">已取消</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="申请人" align="center" width="120">
            <template slot-scope="scope">
              <span>{{getApplyUserName(scope.row.ApplyUserId, scope.row)}}</span>
            </template>
          </el-table-column>
          <el-table-column label="申请时间" align="center" width="160">
            <template slot-scope="scope">
              {{ parseTime(scope.row.ApplyOn) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="160">
            <template slot-scope="scope">
              <el-link type="primary" v-if="scope.row.Status == 2 && scope.row.OutStatus == 0" @click="onSubmitLeave(scope.row)" style="margin-left: 15px">出库</el-link>
              <el-link type="primary" @click="onDetail(scope.row)" style="margin-left: 15px" v-if="scope.row.Status != 0">详情</el-link>
            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
      </div>
      <ManualPile ref="pieDlg" @confirm="getList"></ManualPile>
      <applyadd ref="applyDlg" @confirm="getList"></applyadd>
      <applyDetail ref="applyDetail"></applyDetail>
    </div>
  </div>
</template>
      
  <script>
import { ApplyList } from "@/api/storage/apply";
import applyadd from "./applyadd.vue";
import ManualPile from "./manualpile.vue";
import applyDetail from "./applyDetail.vue";
import { resizeTableCon } from "@/mixins/resizeTableCon";

export default {
  name: "adminApplylist",
  mixins: [resizeTableCon],
  dicts: ["apply_type"],
  components: { applyadd, applyDetail, ManualPile },
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
        Key: undefined,
        HouseId: undefined,
        Status: 2,
        OutStatus: 0,
      },
      // 总条数
      total: 0,
      houseOptions: [],
    };
  },
  async created() {
    const stockId = this.$route.query && this.$route.query.stockId;
    if (stockId) {
      this.queryParams.HouseId = stockId;
      await this.getList();
    }
  },
  methods: {
    handleClose() {
      this.$store.dispatch("tagsView/delView", this.$route);
      this.$router.push({ path: "/jxc/leave/list" });
    },
    getApplyUserName(userId, row) {
      //显示申请人名称
      if (userId == this.$store.state.user.uid) {
        return this.$store.state.user.name;
      } else {
        if (row.ApplyUserInfo) {
          return row.ApplyUserInfo.RealName;
        } else {
          return "";
        }
      }
    },
    LeaveMethodName(way) {
      let wayName = this.dict.getName("apply_type", way);
      return wayName;
    },
    tabClick(tab, event) {
      this.activeName = tab.name;
      this.getList();
    },
    /** 查询列表 */
    getList() {
      this.loading = true;
      ApplyList(this.addDateRange(this.queryParams, this.dateRange)).then(
        (response) => {
          this.tbList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.resetForm("queryForm");
      this.queryParams.pageNum = 1;
      this.dateRange = [];
      this.handleQuery();
    },
    onDetail(row) {
      this.$refs.applyDetail.openDialog(row.Id);
    },
    onSubmitLeave(row) {
      this.$refs.pieDlg.applyLeaveDialog(row);
    },
  },
};
</script>
  <style lang="scss"></style>