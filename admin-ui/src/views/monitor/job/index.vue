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
          <el-form-item label="任务名称" prop="jobName">
            <el-input
              v-model="queryParams.jobName"
              class="set_radius"
              placeholder="请输入任务名称"
              clearable
            
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>
          <el-form-item label="任务组名" prop="jobGroup">
            <el-select
              v-model="queryParams.jobGroup"
              class="set_radius"
              placeholder="请选择任务组名"
              clearable
            
            >
              <el-option
                v-for="dict in dict.type.sys_job_group"
                :key="dict.value"
                :label="dict.label"
                :value="dict.value"
              />
            </el-select>
          </el-form-item>
          <el-form-item label="任务状态" prop="status">
            <el-select
              class="set_radius"
              v-model="queryParams.status"
              placeholder="请选择任务状态"
              clearable
            
            >
              <el-option
                v-for="dict in dict.type.sys_job_status"
                :key="dict.value"
                :label="dict.label"
                :value="dict.value"
              />
            </el-select>
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
              
                @click="handleAdd"
                v-hasPermi="['/MonitorService/Job/Add']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">新增</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button
                type="success"
                plain
              
                :disabled="single"
                @click="handleUpdate"
                v-hasPermi="['/MonitorService/Job/Edit']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-xiugai"></i>
                <span style="margin-left:6px">修改</span>
              </el-button>
            </el-col>

            <el-col :span="1.5">
              <el-button
                type="primary"
                plain
              
                :loading="exportLoading"
                @click="handleExport"
                v-hasPermi="['/MonitorService/Job/Export']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                <span style="margin-left:6px">导出</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button
                type="info"
                plain
              
                @click="handleJobLog"
                v-hasPermi="['/MonitorService/Job/']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-rizhi"></i>
                <span style="margin-left:6px">日志</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button
                type="danger"
                plain
              
                :disabled="multiple"
                @click="handleDelete"
                v-hasPermi="['/MonitorService/Job/Remove']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                <span style="margin-left:6px">删除</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table
          v-loading="loading"
          border
          :data="jobList"
          class="data_table"
          :row-style="isRed"
          @selection-change="handleSelectionChange"
          :header-cell-style="cellSty"
          style="width:100%"
        >
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column label="任务编号" width="100" align="center" prop="job_id" />
          <el-table-column
            label="任务名称"
            align="center"
            prop="job_name"
            :show-overflow-tooltip="true"
          />
          <el-table-column label="任务组名" align="center" prop="job_group">
            <template slot-scope="scope">
              <dict-tag :options="dict.type.sys_job_group" :value="scope.row.job_group" />
            </template>
          </el-table-column>
          <el-table-column
            label="调用目标字符串"
            align="center"
            prop="invoke_target"
            :show-overflow-tooltip="true"
          />
          <el-table-column
            label="cron执行表达式"
            align="center"
            prop="cron_expression"
            :show-overflow-tooltip="true"
          />
          <el-table-column label="状态" align="center">
            <template slot-scope="scope">
              <el-switch
                v-model="scope.row.status"
                active-value="0"
                inactive-value="1"
                @change="handleStatusChange(scope.row)"
              ></el-switch>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button
              
                type="text"
                icon="el-icon-edit"
                @click="handleUpdate(scope.row)"
                v-hasPermi="['/MonitorService/Job/Edit']"
              >修改</el-button>
              <el-button
              
                type="text"
                icon="el-icon-delete"
                @click="handleDelete(scope.row)"
                v-hasPermi="['/MonitorService/Job/Remove']"
              >删除</el-button>
              <el-dropdown
              
                @command="(command) => handleCommand(command, scope.row)"
                v-hasPermi="['/MonitorService/Job/ChangeStatus', '/MonitorService/Job/']"
              >
                <span class="el-dropdown-link">
                  <i class="el-icon-d-arrow-right el-icon--right"></i>更多
                </span>
                <el-dropdown-menu slot="dropdown">
                  <el-dropdown-item
                    command="handleRun"
                    icon="el-icon-caret-right"
                    v-hasPermi="['/MonitorService/Job/ChangeStatus']"
                  >执行一次</el-dropdown-item>
                  <el-dropdown-item
                    command="handleView"
                    icon="el-icon-view"
                  >任务详细</el-dropdown-item>
                  <el-dropdown-item
                    command="handleJobLog"
                    icon="el-icon-s-operation"
                  >调度日志</el-dropdown-item>
                </el-dropdown-menu>
              </el-dropdown>
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
      <!-- 添加或修改定时任务对话框 -->
      <el-dialog
        :title="title"
        :close-on-click-modal="false"
        :visible.sync="open"
        width="800px"
        append-to-body
      >
        <el-form ref="form" :model="form" :rules="rules" label-width="120px">
          <el-row>
            <el-col :span="12">
              <el-form-item label="任务名称" prop="job_name">
                <el-input v-model="form.job_name" placeholder="请输入任务名称" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="任务分组" prop="job_group">
                <el-select v-model="form.job_group" placeholder="请选择">
                  <el-option
                    v-for="dict in dict.type.sys_job_group"
                    :key="dict.value"
                    :label="dict.label"
                    :value="dict.value"
                  ></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item prop="invoke_target">
                <span slot="label">
                  调用方法
                  <el-tooltip placement="top">
                    <div slot="content">
                      ioc类调用示例：TaskInstance.ExeParams('ry')
                      <br />参数说明：支持字符串，布尔类型，长整型，浮点型，整型
                    </div>
                    <i class="el-icon-question"></i>
                  </el-tooltip>
                </span>
                <el-input v-model="form.invoke_target" placeholder="请输入调用目标字符串" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="cron表达式" prop="cron_expression">
                <el-input v-model="form.cron_expression" placeholder="请输入cron执行表达式">
                  <template slot="append">
                    <el-button type="primary" @click="handleShowCron">
                      生成表达式
                      <i class="el-icon-time el-icon--right"></i>
                    </el-button>
                  </template>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="错误策略" prop="misfire_policy">
                <el-radio-group v-model="form.misfire_policy">
                  <el-radio-button label="1">立即执行</el-radio-button>
                  <el-radio-button label="2">执行一次</el-radio-button>
                  <el-radio-button label="3">放弃执行</el-radio-button>
                </el-radio-group>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="是否并发" prop="concurrent">
                <el-radio-group v-model="form.concurrent">
                  <el-radio-button label="0">允许</el-radio-button>
                  <el-radio-button label="1">禁止</el-radio-button>
                </el-radio-group>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="状态">
                <el-radio-group v-model="form.status">
                  <el-radio
                    v-for="dict in dict.type.sys_job_status"
                    :key="dict.value"
                    :label="dict.value"
                  >{{dict.label}}</el-radio>
                </el-radio-group>
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </el-dialog>

      <el-dialog
        title="Cron表达式生成器"
        :close-on-click-modal="false"
        :visible.sync="openCron"
        append-to-body
        destroy-on-close
        class="scrollbar"
      >
        <crontab @hide="openCron=false" @fill="crontabFill" :expression="expression"></crontab>
      </el-dialog>

      <!-- 任务日志详细 -->
      <el-dialog
        title="任务详细"
        :close-on-click-modal="false"
        :visible.sync="openView"
        width="700px"
        append-to-body
      >
        <el-form ref="form" :model="form" label-width="120px">
          <el-row>
            <el-col :span="12">
              <el-form-item label="任务编号：">{{ form.job_id }}</el-form-item>
              <el-form-item label="任务名称：">{{ form.job_name }}</el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="任务分组：">{{ jobGroupFormat(form) }}</el-form-item>
              <el-form-item label="创建时间：">{{ form.createTime }}</el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="cron表达式：">{{ form.cronExpression }}</el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="下次执行时间：">{{ parseTime(form.nextValidTime) }}</el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="调用目标方法：">{{ form.invoke_target }}</el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="任务状态：">
                <div v-if="form.status == 0">正常</div>
                <div v-else-if="form.status == 1">失败</div>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="是否并发：">
                <div v-if="form.concurrent == 0">允许</div>
                <div v-else-if="form.concurrent == 1">禁止</div>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="执行策略：">
                <div v-if="form.misfire_policy == 0">默认策略</div>
                <div v-else-if="form.misfire_policy == 1">立即执行</div>
                <div v-else-if="form.misfire_policy == 2">执行一次</div>
                <div v-else-if="form.misfire_policy == 3">放弃执行</div>
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button @click="openView = false">关 闭</el-button>
        </div>
      </el-dialog>
    </div>
  </div>
</template>

<script>
import {
  listJob,
  getJob,
  delJob,
  addJob,
  updateJob,
  exportJob,
  runJob,
  changeJobStatus
} from "@/api/monitor/job";
import Crontab from "@/components/Crontab";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  components: { Crontab },
  name: "Job",
  dicts: ["sys_job_group", "sys_job_status"],
  mixins: [resizeTableCon],
  data() {
    return {
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
      // 定时任务表格数据
      jobList: [],
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 是否显示详细弹出层
      openView: false,
      // 是否显示Cron表达式弹出层
      openCron: false,
      // 传入的表达式
      expression: "",
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        jobName: undefined,
        jobGroup: undefined,
        status: undefined
      },
      // 表单参数
      form: {},
      // 表单校验
      rules: {
        jobName: [
          { required: true, message: "任务名称不能为空", trigger: "blur" }
        ],
        invokeTarget: [
          { required: true, message: "调用目标字符串不能为空", trigger: "blur" }
        ],
        cronExpression: [
          { required: true, message: "cron执行表达式不能为空", trigger: "blur" }
        ]
      }
    };
  },
  created() {
    this.getList();
  },
  methods: {
    /** 查询定时任务列表 */
    getList() {
      this.loading = true;
      listJob(this.queryParams).then(response => {
        this.jobList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
    },
    // 任务组名字典翻译
    jobGroupFormat(row, column) {
      return this.selectDictLabel(this.dict.type.sys_job_group, row.job_group);
    },
    // 取消按钮
    cancel() {
      this.open = false;
      this.reset();
    },
    // 表单重置
    reset() {
      this.form = {
        job_id: undefined,
        job_name: undefined,
        job_group: undefined,
        invoke_target: undefined,
        cron_expression: undefined,
        misfire_policy: 1,
        concurrent: 1,
        status: "0"
      };
      this.resetForm("form");
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.job_id);
      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.job_id)) {
        return {
          backgroundColor: "#F6F9FF"
        };
      }
    },
    // 更多操作触发
    handleCommand(command, row) {
      switch (command) {
        case "handleRun":
          this.handleRun(row);
          break;
        case "handleView":
          this.handleView(row);
          break;
        case "handleJobLog":
          this.handleJobLog(row);
          break;
        default:
          break;
      }
    },
    // 任务状态修改
    handleStatusChange(row) {
      let text = row.status === "0" ? "启用" : "停用";
      this.$modal
        .confirm('确认要"' + text + '""' + row.job_name + '"任务吗？')
        .then(function() {
          return changeJobStatus(row.job_id, row.status);
        })
        .then(() => {
          this.$modal.msgSuccess(text + "成功");
        })
        .catch(function() {
          row.status = row.status === "0" ? "1" : "0";
        });
    },
    /* 立即执行一次 */
    handleRun(row) {
      this.$modal
        .confirm('确认要立即执行一次"' + row.job_name + '"任务吗？')
        .then(function() {
          return runJob(row.job_id);
        })
        .then(() => {
          this.$modal.msgSuccess("执行成功");
        })
        .catch(() => {});
    },
    /** 任务详细信息 */
    handleView(row) {
      getJob(row.job_id).then(response => {
        this.form = response.data;
        this.openView = true;
      });
    },
    /** cron表达式按钮操作 */
    handleShowCron() {
      this.expression = this.form.cron_expression;
      this.openCron = true;
    },
    /** 确定后回传值 */
    crontabFill(value) {
      this.form.cron_expression = value;
    },
    /** 任务日志列表查询 */
    handleJobLog(row) {
      const jobId = row.job_id || 0;
      this.$router.push({
        path: "/monitor/job-log/index",
        query: { jobId: jobId }
      });
    },
    /** 新增按钮操作 */
    handleAdd() {
      this.reset();
      this.open = true;
      this.title = "添加任务";
    },
    /** 修改按钮操作 */
    handleUpdate(row) {
      this.reset();
      const jobId = row.job_id || this.ids;
      getJob(jobId).then(response => {
        this.form = response.data;
        this.open = true;
        this.title = "修改任务";
      });
    },
    /** 提交按钮 */
    submitForm: function() {
      this.$refs["form"].validate(valid => {
        if (valid) {
          if (this.form.job_id != undefined) {
            updateJob(this.form).then(response => {
              this.$modal.msgSuccess("修改成功");
              this.open = false;
              this.getList();
            });
          } else {
            addJob(this.form).then(response => {
              this.$modal.msgSuccess("新增成功");
              this.open = false;
              this.getList();
            });
          }
        }
      });
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      const jobIds = row.job_id || this.ids;
      this.$modal
        .confirm('是否确认删除定时任务编号为"' + jobIds + '"的数据项？')
        .then(function() {
          return delJob(jobIds);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    /** 导出按钮操作 */
    handleExport() {
      const queryParams = this.queryParams;
      this.$modal
        .confirm("是否确认导出所有定时任务数据项？")
        .then(() => {
          this.exportLoading = true;
          return exportJob(queryParams);
        })
        .then(response => {
          this.exportLoading = false;
        })
        .catch(() => {});
    }
  }
};
</script>
