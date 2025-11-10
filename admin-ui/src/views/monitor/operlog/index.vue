<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form
          :model="queryParams"
          ref="queryForm"
          class="biaodan"
          :inline="true"
        >
          <el-form-item label="系统模块" prop="title">
            <el-input
              v-model="queryParams.title"
              class="set_radius"
              placeholder="请输入系统模块"
              clearable
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>
          <el-form-item label="操作人员" prop="operName">
            <el-input
              v-model="queryParams.operName"
              class="set_radius"
              placeholder="请输入操作人员"
              clearable
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>

          <el-form-item label="状态" prop="status">
            <el-select v-model="queryParams.status" class="set_radius" placeholder="操作状态" clearable>
              <el-option
                v-for="dict in dict.type.sys_common_status"
                :key="dict.value"
                :label="dict.label"
                :value="dict.value"
              />
            </el-select>
          </el-form-item>
          <el-form-item label="操作时间">
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
                v-hasPermi="['/MonitorService/OperLog/Export']"
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
                v-hasPermi="['/MonitorService/OperLog/Remove']"
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
                v-hasPermi="['/MonitorService/OperLog/Remove']"
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
          <el-table-column label="日志编号" align="center" prop="oper_id" />
          <el-table-column label="系统模块" align="center" prop="title" />
          <el-table-column label="请求方式" align="center" prop="request_method" />
          <el-table-column
            label="操作人员"
            align="center"
            prop="oper_name"
            width="100"
            :show-overflow-tooltip="true"
          />
          <el-table-column
            label="操作地址"
            align="center"
            prop="oper_ip"
            width="130"
            :show-overflow-tooltip="true"
          />
          <el-table-column
            label="操作地点"
            align="center"
            prop="oper_location"
            :show-overflow-tooltip="true"
          />
          <el-table-column label="操作状态" align="center" prop="status">
            <template slot-scope="scope">
              <dict-tag :options="dict.type.sys_common_status" :value="scope.row.status" />
            </template>
          </el-table-column>
          <el-table-column
            label="操作日期"
            align="center"
            prop="oper_time"
            sortable="custom"
            :sort-orders="['descending', 'ascending']"
            width="180"
          >
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.oper_time) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button
                type="text"
                icon="el-icon-view"
                @click="handleView(scope.row,scope.index)"
                v-hasPermi="['/MonitorService/OperLog']"
              >详细</el-button>
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
      <!-- 操作日志详细 -->
      <el-dialog
        title="操作日志详细"
        :close-on-click-modal="false"
        :visible.sync="open"
        width="700px"
        append-to-body
      >
        <el-form ref="form" :model="form" label-width="100px">
          <el-row>
            <el-col :span="12">
              <el-form-item label="操作模块：">{{ form.title }}</el-form-item>
              <el-form-item
                label="登录信息："
              >{{ form.oper_name }} / {{ form.oper_ip }} / {{ form.oper_location }}</el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="请求地址：">{{ form.oper_url }}</el-form-item>
              <el-form-item label="请求方式：">{{ form.request_method }}</el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="操作方法：">{{ form.method }}</el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="请求参数：">{{ form.oper_param }}</el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="返回参数：">{{ form.json_result }}</el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="操作状态：">
                <div v-if="form.status === 0">正常</div>
                <div v-else-if="form.status === 1">失败</div>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="操作时间：">{{ parseTime(form.oper_time) }}</el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="异常信息：" v-if="form.status === 1">{{ form.error_msg }}</el-form-item>
            </el-col>
          </el-row>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button @click="open = false">关 闭</el-button>
        </div>
      </el-dialog>
    </div>
  </div>
</template>

<script>
import {
  list,
  delOperlog,
  cleanOperlog,
  exportOperlog
} from "@/api/monitor/operlog";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "Operlog",
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
      // 是否显示弹出层
      open: false,
      // 日期范围
      dateRange: [],
      // 默认排序
      defaultSort: { prop: "oper_time", order: "descending" },
      // 表单参数
      form: {},
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        title: undefined,
        operName: undefined,
        status: undefined
      }
    };
  },
  created() {
    this.getList();
  },
  methods: {
    /** 查询登录日志 */
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
      // console.log("多选选项",selection);
      this.ids = selection.map(item => item.oper_id);
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.oper_id)) {
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
    /** 详细按钮操作 */
    handleView(row) {
      this.open = true;
      this.form = row;
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      const operIds = row.operId || this.ids;
      this.$modal
        .confirm('是否确认删除日志编号为"' + operIds + '"的数据项？')
        .then(function() {
          return delOperlog(operIds);
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
        .confirm("是否确认清空所有操作日志数据项？")
        .then(function() {
          return cleanOperlog();
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
          return exportOperlog(queryParams);
        })
        .then(response => {
          this.exportLoading = false;
        })
        .catch(() => {});
    }
  }
};
</script>

