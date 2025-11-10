<template>
  <div>
    <el-dialog width="960px" title="请选择退货原单" :visible.sync="rukuOpen" highlight-current-row :close-on-click-modal="false" append-to-body>
      <div>
        <el-form :model="rukuQuery" ref="agentForm" :inline="true" style="display: flex; justify-content: space-between">
          <div>
            <el-form-item label="关键字" prop="Key">
              <el-input v-model="rukuQuery.Key" placeholder="请输入搜索的入库单号、物品名称" clearable></el-input>
            </el-form-item>
            <el-form-item label="入库时间">
              <el-date-picker class="form_input_style" v-model="dateRange" style="width: 232px" value-format="yyyy-MM-dd"
                type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
            </el-form-item>
          </div>

          <el-form-item>
            <el-button icon="el-icon-refresh" @click="resetAgent">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="loadrkList">搜索</el-button>
          </el-form-item>
        </el-form>
        <el-table ref="agentTable" :data="tbList" tooltip-effect="dark" v-loading="agentLoading" style="width: 100%" @current-change="onRKChange">
          <el-table-column label="入库单号" align="center" prop="StockNumber" width="160"></el-table-column>
          <el-table-column label="所出仓库">
            <template slot-scope="scope">
              <template v-if="scope.row.FromHouseName == ''">无</template>
              <template v-else>
                <el-tag>{{ scope.row.FromName }}</el-tag>
                <span style="margin-left:10px;">{{ scope.row.FromHouseName }}</span>
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
          <el-table-column label="入库时间-创建时间" align="center" width="160">
            <template slot-scope="scope">
              <el-tag>{{ parseTime(scope.row.InDate) }}</el-tag>
              <br>
              <el-tag type="success" style="margin-top:5px;">{{ parseTime(scope.row.createTime) }}</el-tag>
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
        </el-table>
        <pagination v-show="total > 0" :total="total" :page.sync="rukuQuery.pageNum" :limit.sync="rukuQuery.pageSize" @pagination="loadrkList"/>
      </div>
    </el-dialog>
  </div>
</template>
      
<script>
import {
  enterList,
} from "@/api/storage/stock";
export default {
  name: "enterlist",
  props:{
    enterMethod:{
      type:[String,Number],
      default:null
    }
  },
  data() {
    return {
      showAgent: false,
      showKf: false,
      activeName: "first",
      rukuOpen: false, //选择代理商
      agentLoading: false,
      dateRange: [],
      rukuQuery: {
        pageNum: 1,
        pageSize: 20,
        Key: "",
        // Status:2//过滤入库成功的入库单
        // Status :2
      },
      customDateRange: [],
      tbList: [],
      total: 0,
    };
  },
  mounted() {
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
    resetAgent() {
      this.dateRange = [];
      this.resetForm("agentForm");
      this.loadrkList();
    },
    onRKChange(val) {
      if (val != null) {
        this.$emit("ok", val);
        this.rukuOpen = false;
      }
    },
    async loadrkList() {
      if(this.enterMethod){
        this.rukuQuery.EnterMethod=this.enterMethod
      }
      let response = await enterList(
        this.addDateRange(this.rukuQuery, this.dateRange)
      );
      // console.log("入库记录列表", response);
      this.tbList = response.data.List;
      this.total = response.data.Total;
    },
    async openAgentDialog() {
      this.rukuOpen = true;
      this.agentLoading = true;
      await this.loadrkList();
      this.agentLoading = false;
    },
  },
};
</script>
<style lang="scss" scoped></style>
      