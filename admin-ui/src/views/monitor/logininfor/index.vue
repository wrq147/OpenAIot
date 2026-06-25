<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form
          :model="queryParams"
          ref="queryForm"
          :inline="true"
          class="biaodan"
        >
          <el-form-item label="登录地址" prop="ipaddr">
            <el-input
              v-model="queryParams.ipaddr"
              class="set_radius"
              placeholder="请输入登录地址"
              clearable
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>
          <el-form-item label="用户名称" prop="userName">
            <el-input
              v-model="queryParams.userName"
              class="set_radius"
              placeholder="请输入用户名称"
              clearable
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>
          <el-form-item label="状态" prop="status">
            <el-select v-model="queryParams.status" class="set_radius" placeholder="登录状态" clearable>
              <el-option
                v-for="dict in dict.type.sys_common_status"
                :key="dict.value"
                :label="dict.label"
                :value="dict.value"
              />
            </el-select>
          </el-form-item>
          <el-form-item label="登录时间">
            <el-date-picker
              v-model="dateRange"
              class="set_radius"
              style="width: 250px"
              value-format="yyyy-MM-dd"
              type="daterange"
              range-separator="-"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
            ></el-date-picker>
          </el-form-item>
          <!-- <el-col class="float_right" :span="24"> -->
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
          </el-form-item>
          <!-- </el-col> -->
        </el-form>
      </div>
      <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-button
                type="primary"
                plain
                :loading="exportLoading"
                @click="handleExport"
                v-hasPermi="['/AuthService/LoginiLog/Export']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                <span style="margin-left:6px">导出</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button
                type="danger"
                plain
                :disabled="multiple"
                @click="handleDelete"
                v-hasPermi="['/AuthService/LoginiLog/Remove']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                <span style="margin-left:6px">删除</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button
                type="warning"
                plain
                @click="handleClean"
                v-hasPermi="['/AuthService/LoginiLog/Remove']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-qingkong"></i>
                <span style="margin-left:6px">清空</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table
          ref="tables"
          border
          v-loading="loading"
          class="data_table"
          :row-style="isRed"
          :data="list"
          @selection-change="handleSelectionChange"
          :default-sort="defaultSort"
          @sort-change="handleSortChange"
          :header-cell-style="cellSty"
          style="width:100%"
        >
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column label="访问编号" align="center" prop="SysLogID" />
          <el-table-column
            label="用户名称"
            align="center"
            prop="UserName"
            :show-overflow-tooltip="true"
          />
          <el-table-column
            label="登录地址"
            align="center"
            prop="IPAddress"
            width="130"
            :show-overflow-tooltip="true"
          />
          <el-table-column
            label="登录地点"
            align="center"
            prop="IPLocation"
            :show-overflow-tooltip="true"
          />
          <el-table-column label="浏览器" align="center" prop="Browser" :show-overflow-tooltip="true" />
          <el-table-column label="操作系统" align="center" prop="OS" />
          <el-table-column label="登录状态" align="center" prop="Status">
            <template slot-scope="scope">
              <dict-tag :options="dict.type.sys_common_status" :value="scope.row.Status" />
            </template>
          </el-table-column>
          <el-table-column label="操作信息" align="center" prop="Info" />
          <el-table-column
            label="登录日期"
            align="center"
            prop="CreateDate"
            sortable="custom"
            :sort-orders="['descending', 'ascending']"
            width="180"
          >
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.CreateDate) }}</span>
            </template>
          </el-table-column>
        </el-table>

        <pagination
          v-show="total>0"
          :total="total"
          :page.sync="queryParams.pageNum"
          :limit.sync="queryParams.pageSize"
          @pagination="getList"
        />
      </div>
    </div>
  </div>
</template>

<script>
import {
  list,
  delLogininfor,
  cleanLogininfor,
  exportLogininfor
} from "@/api/monitor/logininfor";
import { resizeTableCon } from "@/mixins/resizeTableCon";

export default {
  name: "Logininfor",
  dicts: ["sys_common_status"],
  mixins: [resizeTableCon],
  data() {
    return {
      // 遮罩层
      loading: true,
      // 导出遮罩层
      exportLoading: false,
      // 选中数组
      ids: [],
      // 非多个禁用
      multiple: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      // 表格数据
      list: [],
      // 日期范围
      dateRange: [],
      // 默认排序
      defaultSort: { prop: "CreateDate", order: "descending" },
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        ipaddr: undefined,
        userName: undefined,
        status: undefined
      }
    };
  },
  created() {
    this.getList();
  },
  methods: {
    /** 查询登录日志列表 */
    getList() {
      this.loading = true;
      list(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          this.list = response.data.List;
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
      this.$refs.tables.sort(this.defaultSort.prop, this.defaultSort.order);
      this.handleQuery();
    },
    /** 多选框选中数据 */
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.SysLogID);
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      console.log("选中的", checkIdList, this.ids, row);
      if (checkIdList.includes(row.SysLogID)) {
        return {
          backgroundColor: "#F6F9FF"
        };
      }
    },
    /** 排序触发事件 */
    handleSortChange(column, prop, order) {
      this.queryParams.orderByColumn = column.prop;
      this.queryParams.isAsc = column.order;
      this.getList();
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      const infoIds = row.infoId || this.ids;
      this.$modal
        .confirm('是否确认删除访问编号为"' + infoIds + '"的数据项？')
        .then(function() {
          return delLogininfor({id:infoIds});
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    /** 清空按钮操作 */
    handleClean() {
      this.$modal
        .confirm("是否确认清空所有登录日志数据项？")
        .then(function() {
          return cleanLogininfor();
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("清空成功");
        })
        .catch(() => {});
    },
    /** 导出按钮操作 */
    handleExport() {
      const queryParams = this.queryParams;
      this.$modal
        .confirm("是否确认导出所有操作日志数据项？")
        .then(() => {
          this.exportLoading = true;
          return exportLogininfor(queryParams);
        })
        .then(response => {
          this.exportLoading = false;
        })
        .catch(() => {});
    }
  }
};
</script>

