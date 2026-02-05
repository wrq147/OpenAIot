<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form :model="queryParams" ref="queryForm" :inline="true" class="biaodan">
          <el-form-item label="任务名称" prop="jobName">
            <el-input v-model="queryParams.jobName" class="set_radius" placeholder="请输入任务名称" clearable
              style="width: 240px" @keyup.enter.native="handleQuery" />
          </el-form-item>
          <el-form-item label="任务组名" prop="jobGroup">
            <el-select v-model="queryParams.jobGroup" class="set_radius" placeholder="请任务组名" clearable
              style="width: 240px">
              <el-option v-for="dict in dict.type.sys_job_group" :key="dict.value" :label="dict.label"
                :value="dict.value" />
            </el-select>
          </el-form-item>
          <el-form-item label="执行状态" prop="status">
            <el-select v-model="queryParams.status" class="set_radius" placeholder="请选择执行状态" clearable
              style="width: 240px">
              <el-option v-for="dict in dict.type.sys_common_status" :key="dict.value" :label="dict.label"
                :value="dict.value" />
            </el-select>
          </el-form-item>
          <el-form-item label="执行时间">
            <el-date-picker v-model="dateRange" class="set_radius" style="width: 240px" value-format="yyyy-MM-dd"
              type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
          </el-form-item>
          <el-form-item class="submit_button_con">
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
          </el-form-item>
        </el-form>
      </div>
      <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-button type="danger" plain :disabled="multiple" @click="handleDelete"
                v-hasPermi="['/MonitorService/Job/Remove']">
                <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                <span style="margin-left:6px">删除</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="danger" plain @click="handleClean" v-hasPermi="['/MonitorService/Job/Remove']">
                <i class="zhongtaiiconfont zhongtai-icon-qingkong"></i>
                <span style="margin-left:6px">清空</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="warning" plain :loading="exportLoading" @click="handleExport"
                v-hasPermi="['/MonitorService/Job/Export']">
                <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                <span style="margin-left:6px">导出</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="warning" plain @click="handleClose">
                <i class="zhongtaiiconfont zhongtai-icon-a-guanbihui"></i>
                <span style="margin-left:6px">关闭</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table v-loading="loading" class="data_table" border :data="jobLogList"
          @selection-change="handleSelectionChange" :header-cell-style="cellSty" style="width:100%">
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column label="日志编号" width="80" align="center" prop="job_log_id" />
          <el-table-column label="任务名称" align="center" prop="job_name" :show-overflow-tooltip="true" />
          <el-table-column label="任务组名" align="center" prop="job_group" :show-overflow-tooltip="true">
            <template slot-scope="scope">
              <dict-tag :options="dict.type.sys_job_group" :value="scope.row.job_group" />
            </template>
          </el-table-column>
          <el-table-column label="调用目标字符串" align="center" prop="invoke_target" :show-overflow-tooltip="true" />
          <el-table-column label="日志信息" align="center" prop="job_message" :show-overflow-tooltip="true" />
          <el-table-column label="执行状态" align="center" prop="status">
            <template slot-scope="scope">
              <dict-tag :options="dict.type.sys_common_status" :value="scope.row.status" />
            </template>
          </el-table-column>
          <el-table-column label="执行时间" align="center" prop="create_time" width="180">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.create_time) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button type="text" icon="el-icon-view" @click="handleView(scope.row)">详细</el-button>
              <el-button v-if="scope.row.status==1" type="text" icon="el-icon-refresh-left" @click="handleRetry(scope.row)">重试</el-button>
            </template>
          </el-table-column>
        </el-table>

        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
          :limit.sync="queryParams.pageSize" @pagination="getList" />
      </div>
      <!-- 调度日志详细 -->
      <el-dialog title="调度日志详细" :close-on-click-modal="false" :visible.sync="open" width="700px" append-to-body>
        <el-form ref="form" :model="form" label-width="100px">
          <el-row>
            <el-col :span="12">
              <el-form-item label="日志序号：">{{ form.job_log_id }}</el-form-item>
              <el-form-item label="任务名称：">{{ form.job_name }}</el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="任务分组：">{{ form.job_group }}</el-form-item>
              <el-form-item label="执行时间：">{{ form.create_time }}</el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="调用方法：">{{ form.invoke_target }}</el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="日志信息：">{{ form.job_message }}</el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="执行状态：">
                <div v-if="form.status == 0">正常</div>
                <div v-else-if="form.status == 1">失败</div>
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="异常信息：" v-if="form.status == 1">{{ form.exception_info }}</el-form-item>
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
import { getJob } from "@/api/monitor/job";
import {
  listJobLog,
  delJobLog,
  exportJobLog,
  cleanJobLog,
  retryJobLog
} from "@/api/monitor/jobLog";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "JobLog",
  dicts: ["sys_common_status", "sys_job_group"],
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
      // 调度日志表格数据
      jobLogList: [],
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
        jobName: undefined,
        jobGroup: undefined,
        status: undefined
      }
    };
  },
  created() {
    const jobId = this.$route.query.jobId;
    if (jobId !== undefined && jobId != 0) {
      getJob(jobId).then(response => {
        this.queryParams.jobName = response.data.job_name;
        this.queryParams.jobGroup = response.data.job_group;
        this.getList();
      });
    } else {
      this.getList();
    }
  },
  methods: {
    /** 查询调度日志列表 */
    getList() {
      this.loading = true;
      listJobLog(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          this.jobLogList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    // 返回按钮
    handleClose() {
      this.$store.dispatch("tagsView/delView", this.$route);
      this.$router.push({ path: "/monitor/job" });
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
      this.ids = selection.map(item => item.job_log_id);
      this.multiple = !selection.length;
    },
    /** 详细按钮操作 */
    handleView(row) {
      this.open = true;
      this.form = row;
    },
    async handleRetry(row) {
      try {
        await retryJobLog(row.job_log_id);
        this.getList();
        this.$modal.msgSuccess("重试成功");
      }
      catch { }
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      const jobLogIds = this.ids;
      this.$modal
        .confirm('是否确认删除调度日志编号为"' + jobLogIds + '"的数据项？')
        .then(function () {
          return delJobLog(jobLogIds);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => { });
    },
    /** 清空按钮操作 */
    handleClean() {
      this.$modal
        .confirm("是否确认清空所有调度日志数据项？")
        .then(() => {
          return cleanJobLog(this.queryParams.jobName, this.queryParams.jobGroup);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("清空成功");
        })
        .catch(() => { });
    },
    /** 导出按钮操作 */
    handleExport() {
      const queryParams = this.queryParams;
      this.$modal
        .confirm("是否确认导出所有调度日志数据项？")
        .then(() => {
          this.exportLoading = true;
          return exportJobLog(queryParams);
        })
        .then(response => {
          this.exportLoading = false;
        })
        .catch(() => { });
    }
  }
};
</script>
