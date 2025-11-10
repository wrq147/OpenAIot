<template>
  <div style="padding: 20px 20px 0 20px" id="big_con">
    <el-row :gutter="20">
      <!--单位数据-->
      <el-col :span="24" :xs="24">
        <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
          <el-row :gutter="10" class="mb8 button_row">
            <el-col :span="1.5">
              <el-button type="primary" plain @click="createdProduct">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left: 6px">创建单位</span>
              </el-button>
            </el-col>
          </el-row>
          <el-table v-loading="loading" border :data="dateTableList" :row-style="isRed" @selection-change="handleSelectionChange" class="data_table"
            :header-cell-style="cellSty" style="width: 100%">
            <el-table-column label="单位名称" align="center" key="UnitName" prop="UnitName" :show-overflow-tooltip="true"/>
            <el-table-column label="备注" align="center" key="Remark" prop="Remark" :show-overflow-tooltip="true"/>
            <el-table-column label="更新时间" align="center" key="updateTime" prop="updateTime" width="140">
              <template slot-scope="scope">
                <span>{{ parseTime(scope.row.updateTime) }}</span>
              </template>
            </el-table-column>
            <el-table-column label="操作" align="center" width="248" class-name="small-padding fixed-width">
              <template slot-scope="scope">
                <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)">修改</el-button>
                <el-button type="text" icon="el-icon-edit" @click="handleDelete(scope.row)">删除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
        </div>
      </el-col>
    </el-row>
    <unitAdd ref="unitAdd" @reloadData="getList"></unitAdd>
    <!-- 添加或修改参数配置对话框 -->
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import unitAdd from "./component/unitAdd.vue";
import { factoryUnitListGet, factoryUnitRemove } from "@/api/factory/unit";
export default {
  name: "AdminUiProductlist",
  mixins: [resizeTableCon],
  components: { unitAdd },
  data() {
    return {
      total: 0,
      activeName: "all",
      queryParams: {
        Key: "",
      },
      dateRange: [],
      showSearch: true,
      dateTableList: [],
      // 列信息
      loading: false,
      ids: [], //选择的单位
    };
  },

  mounted() {
    this.getList();
  },

  methods: {
    createdProduct() {
      this.$refs.unitAdd.openDialog(); //打开添加单位的弹窗
    },
    handleClick() {
      //切换标签
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map((item) => item.Id);
      // console.log("选中的",this.ids);

      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "#F6F9FF",
        };
      }
    },
    onClear() {},
    getList() {
      factoryUnitListGet().then((res) => {
        console.log("查询到", res);
        this.dateTableList = res.data;
      });
    },
    resetQuery() {},
    handleQuery() {},
    handleUpdate(row) {
      //修改
      this.$refs.unitAdd.openDialog(row.Id); //打开添加单位的弹窗
    },
    handleDelete(row) {
      //删除
      this.$modal
        .confirm('是否确认删除单位"' + row.ProductName + '"？')
        .then(function () {
          return factoryUnitRemove({ id: row.Id });
        })
        .then(() => {
          that.getList();
          that.$modal.msgSuccess("移除成功");
        })
        .catch((err) => {
          console.log("错误", err);
        });
    },
  },
};
</script>
<style lang="less" scoped>
.product_type_title {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 100%;

  .right_icon {
    margin-left: 15px;
    transform: rotate(90deg); /* 旋转90度 */
    /* 可选：如果你想让元素保持其原始大小，可以同时应用transform-origin */
    transform-origin: center center; /* 旋转中心点在元素中心 */
  }
  .line {
    margin-left: 8px;
    width: 2px;
    height: 22px;
    background: #eeeeee;
  }
}
</style>