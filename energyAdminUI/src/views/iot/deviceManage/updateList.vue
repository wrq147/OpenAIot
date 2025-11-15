<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--产品数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <!-- <el-form-item label="搜索关键词" prop="Name">
                <el-input
                  class="set_radius"
                  v-model="queryParams.Name"
                  placeholder="请输入产品名称"
                  clearable
                  @keyup.enter.native="handleQuery"
                />
              </el-form-item>-->
              <el-form-item label="过滤产品" prop="ProductId">
                <el-select class="set_radius" v-model="queryParams.ProductId" placeholder="请选择产品" clearable filterable>
                  <el-option v-for="item in productSelectList" :label="item.Name" :key="item.Id" :value="item.Id"/>
                </el-select>
              </el-form-item>
              <el-form-item label="创建日期">
                <el-date-picker
                  class="set_radius"
                  v-model="dateRange"
                  style="width:232px"
                  value-format="yyyy-MM-dd"
                  type="datetimerange"
                  range-separator="-"
                  start-placeholder="开始日期"
                  end-placeholder="结束日期"
                ></el-date-picker>
              </el-form-item>
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="onClear">
                    <!-- <svg-icon icon-class="qingkong"></svg-icon> -->
                    <i class="zhongtaiiconfont zhongtai-icon-qingkong"></i>
                    <span style="margin-left:6px">清除失败的升级</span>
                  </el-button>
                </el-col>
              </div>
            </el-row>
            <el-table v-loading="loading" border :data="updateTableList" :row-style="isRed" :cell-style="isRed"
              @selection-change="handleSelectionChange" class="data_table" :header-cell-style="cellSty" style="width:100%">
              <el-table-column label="设备名称" align="center" key="Name" prop="Name" v-if="columns[0].visible" :show-overflow-tooltip="true"/>
              <el-table-column label="产品名称" align="center" key="ProductName" prop="ProductName" v-if="columns[1].visible" :show-overflow-tooltip="true"/>
              <el-table-column label="尝试更新的次数" align="center" key="UpdateCount" prop="UpdateCount" width="150" v-if="columns[2].visible" :show-overflow-tooltip="true"/>
              <el-table-column label="更新的目标版本" align="center" key="Version" prop="Version" width="150" v-if="columns[3].visible" :show-overflow-tooltip="true"></el-table-column>
              <el-table-column label="更新失败的原因" align="center" key="UpdateErr" prop="UpdateErr" v-if="columns[4].visible" width="250"/>
              <el-table-column label="更新状态" align="center" key="Status" prop="Status" v-if="columns[5].visible" width="118"
                :filters="[{text: '待更新', value: '0'}, {text: '更新中', value: '1'}, {text: '更新失败', value: '2'}]"
                :filter-method="filterSatus">
                <template slot-scope="scope">{{scope.row.Status==0?'待更新':(scope.row.Status==1?'更新中':'更新失败')}}</template>
              </el-table-column>
              <el-table-column label="更新时间" align="center" key="UpdatedOn" prop="UpdatedOn" v-if="columns[6].visible" width="140">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.UpdatedOn) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" width="248" class-name="small-padding fixed-width">
                <template slot-scope="scope">
                  <div v-if="scope.row.Status==0||scope.row.Status==2">
                    <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)">手动升级</el-button>
                  </div>
                </template>
              </el-table-column>
            </el-table>

            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
          </div>
        </el-col>
      </el-row>

      <!-- 添加或修改参数配置对话框 -->
    </div>
  </div>
</template>

<script>
import { productList } from "@/api/rules/productModel";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { updateList, manualUpdate,clearError } from "@/api/rules/device";
export default {
  name: "ProductList",
  mixins: [resizeTableCon],
  data() {
    return {
      classmap: new Map(),
      // 遮罩层
      loading: true,
      // 导出遮罩层
      exportLoading: false,
      // 选中数组
      ids: [],
      // 非单个禁用
      single: true,
      // 非多个禁用
      multiple: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      // 升级列表表格数据
      updateTableList: null,
      //选择的产品列表
      productSelectList: [],
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 日期范围
      dateRange: [],
      // 表单参数
      form: {},
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        ProductId: null
      },
      // 列信息
      columns: [
        { key: 0, label: `设备名称`, visible: true },
        { key: 1, label: `产品名称`, visible: true },
        { key: 2, label: `尝试更新的次数`, visible: true },
        { key: 3, label: `更新的目标版本`, visible: true },
        { key: 4, label: `更新失败的原因`, visible: true },
        { key: 5, label: `更新状态`, visible: true },
        { key: 6, label: `更新时间`, visible: true }
      ]
    };
  },
  mounted() {
    this.getProductList();
    this.getList();
  },
  methods: {
    // changeStatus() {
    //   //改变查询的状态的值
    //   if (this.queryParams.Status == "") {
    //     this.queryParams.Status = null;
    //   }
    // },
    filterSatus(value, row) {
      return row.Status === value;
    },
    /** 查询产品列表 */
    getProductList() {
      console.log("查询", this.queryParams);
      productList({ showAll: true }).then(async response => {
        this.productSelectList = response.data.List;
      });
    },
    /** 查询升级列表 */
    getList() {
      this.loading = true;
      console.log("查询", this.queryParams);
      updateList(this.addDateRange(this.queryParams, this.dateRange)).then(
        async response => {
          this.updateTableList = response.data.List;
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
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.Id);
      // console.log("选中的",this.ids);

      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "rgba(32, 63, 65, 1)"
        };
      }
    },
    /** 手动升级操作 */
    handleUpdate(row) {
      const updloading =this.$loading({
          lock: true,
          text: 'Loading',
          spinner: 'el-icon-loading',
          background: 'rgba(0, 0, 0, 0.7)'
        });
      manualUpdate({ id: row.Id }).then(res => {
        updloading.close();
        if (res.code == 0) {
          this.getList();
          this.$modal.msgSuccess("升级成功");
        }
      }).catch((ex)=>{
        updloading.close();
      });
    },
    onClear(){
      this.$modal
        .confirm('是否确认清除失败的升级')
        .then(function () {
          return clearError();
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("清除成功");
        })
        .catch(() => { });
    }
  }
};
</script>
<style rel="stylesheet/scss" lang="scss">
@import "~@/assets/styles/element-variables.scss";
// @import "~@/assets/icons/iconfont.css";
// .app-container {
//   padding-right: 30px;
// }
.data_table {
  th div.cell .el-table__column-filter-trigger .el-icon-arrow-down::before {
    font-family: "zhongtaiiconfont" !important;
    // font-size: 24px;
    font-size: var(--shaixuan);
    font-style: normal;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    content: "\e6d2";
  }
  .col_con {
    width: 320px;
    display: flex;
    justify-content: flex-start;
    .col_left {
      display: flex;
      align-items: center;
      img {
        width: 54px;
        height: 54px;
      }
    }
    .col_right {
      display: flex;
      flex-direction: column;
      align-items: center;
      margin-left: 12px;
      .col_right_top {
        line-height: 22px;
        text-align: left;
        width: 100%;
        color: #0054fc;
      }
      .col_right_bottom {
        line-height: 22px;
        .right_bottom1 {
          display: flex;
          align-items: flex-start;
          justify-content: flex-start;
          flex-direction: column;
          .proName {
            color: #0054fc;
          }

        
        }
        .right_bottom2 {
          display: flex;
          align-items: center;
          justify-content: flex-start;
          .data_icon {
            display: block;
            width: 44px;
            height: 16px;
            color: #0054fc;
            .el-tooltip__popper .popper__arrow {
              border-width: 6px;
              border-color: rgb(0, 0, 0, 1);
            }
            i {
              display: none;
              margin-left: 8px;
            }
          }
        }
      }
    }
  }
}
.data_table.el-table table tr.el-table__row:hover {
  .col_con .col_right .col_right_bottom .right_bottom2 .data_icon i {
    display: inline;
    cursor: pointer;
  }
}
</style>