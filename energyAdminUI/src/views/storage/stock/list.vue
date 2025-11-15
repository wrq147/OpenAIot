<template>
  <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form :model="queryParams" class="biaodan" ref="queryForm" :inline="true">

          <el-form-item label="存储类型" prop="TargetType">
            <el-select v-model="queryParams.TargetType" clearable placeholder="请选择存储类型">
              <el-option label="半成品" value="0"></el-option>
              <el-option label="成品" value="1"></el-option>
            </el-select>
          </el-form-item>


          <el-form-item label="关键词" prop="Key">
            <el-input placeholder="请输入搜索的设备、耗材名称或唯一编号" style="width: 300px;" v-model="queryParams.Key" clearable>
            </el-input>
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
              <el-select v-model="queryParams.HouseId" filterable clearable placeholder="请输入仓库" @change="changeHouse">
                <el-option v-for="item in houseOptions" :key="item.Id" :label="item.StoreName" :value="item.Id">
                </el-option>
              </el-select>
            </el-col>
            <el-col :span="1.5">
              <el-button type="primary" plain :loading="exportLoading" @click="handleExport">
                <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                <span style="margin-left:6px">导出</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="primary" plain @click="handleWarning">
                <i class="zhongtaiiconfont zhongtai-icon-a-guizeyinqing"></i>
                <span style="margin-left:6px">设置预警</span>
              </el-button>
            </el-col>
          </div>

        </el-row>

        <el-table border v-loading="loading" :data="tbList" class="data_table" :header-cell-style="cellSty"
          style="width:100%" row-key="Id" @selection-change="handleSelectionChange">
          <el-table-column type="selection" width="55"></el-table-column>
          <el-table-column label="物品编号" align="center" prop="DeviceNumber" width="180"></el-table-column>
          <el-table-column label="物品名称" align="left" prop="Name"></el-table-column>
          <el-table-column label="存储类型" align="center" width="100">
            <template slot-scope="scope">
              <span>{{ scope.row.TargetType == 0 ? "半成品" : "成品" }}</span>
            </template>
          </el-table-column>
          <el-table-column label="所在仓库" align="center" prop="StoreName" width="120"></el-table-column>

          <el-table-column label="预览图片" align="center" width="220">
            <template slot-scope="scope">
              <div class="imgwrap">
                <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'" :preview-src-list="[scope.row.PhotoUrl]">
                </el-image>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="库存数量" align="center" prop="Quantity" width="120"></el-table-column>
          <el-table-column label="锁定数量" align="center" prop="LockQuantity" width="120"></el-table-column>
          <el-table-column label="平均价格（元）" align="center" prop="Price" width="120"></el-table-column>
          <el-table-column label="单位" align="center" prop="Unit" width="120"></el-table-column>
          <el-table-column label="操作" fixed="right" align="center" class-name="small-padding fixed-width" width="140">
            <template slot-scope="scope">
              <el-link icon="el-icon-info" type="primary" @click="onRecordItem(scope.row)">变更记录</el-link>
            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize"
          @pagination="getList" />
      </div>

      <StockRecord ref="recordDlg"></StockRecord>
      <Stockwarning ref="stockwarning" :allStock="tbList" :selectStock="multipleSelection"></Stockwarning>
    </div>
  </div>
</template>
  
<script>
import {
  stockList,
  exportExcel
} from "@/api/storage/stock";
import {
  houseList
} from "@/api/storage/house";
import StockRecord from "./record.vue";
import Stockwarning from "./setwarning.vue";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "StockList",
  mixins: [resizeTableCon],
  components: { StockRecord,Stockwarning },
  data() {
    return {
      // 导出遮罩层
      exportLoading: false,
      houseOptions: [],
      // 遮罩层
      loading: true,
      // 显示搜索条件
      showSearch: true,
      // 表格树数据
      tbList: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        TargetType: undefined,
        HouseId: undefined,
        Key: undefined
      },
      // 总条数
      total: 0,
      multipleSelection:[]
    };
  },
  async created() {
    let rsp = await houseList({ showAll: true });
    this.houseOptions = rsp.data.List;
    if (this.houseOptions.length > 0) {
      let hidd = this.$cache.local.get("curhouseId-" + this.$store.state.user.orgId + "-" + this.$store.state.user.uid);
      this.queryParams.HouseId = this.houseOptions[0].Id;
      if (hidd != null && hidd != '' && this.houseOptions.some(x => x.Id == hidd)) {
        this.queryParams.HouseId = hidd;
      }
    }
    this.getList();
  },
  methods: {
    handleWarning(){
      this.$refs.stockwarning.setWarnOption(this.multipleSelection)
    },
    handleSelectionChange(val) {
      console.log("val选中的",val);
      this.multipleSelection = val;
    },
    /** 查询列表 */
    getList() {
      this.loading = true;
      stockList(this.queryParams).then(response => {
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

    /** 导出按钮操作 */
    handleExport() {
      const queryParams = this.queryParams;
      this.$modal
        .confirm("是否将数据导出为Excel文件？")
        .then(() => {
          this.exportLoading = true;
          return exportExcel(queryParams);
        })
        .then(response => {
          this.exportLoading = false;
        })
        .catch(() => { });
    },
    async onRecordItem(row) {
      this.$refs.recordDlg.openDlg(row);
    }
  }
};
</script>
<style lang="scss">
.imgwrap {
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;

  .el-image {
    display: flex;
    width: 80px;
    height: 80px;
    justify-content: center;
    align-items: center;
  }
}
</style>