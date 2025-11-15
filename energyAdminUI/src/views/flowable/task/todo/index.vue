<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form
          :model="queryParams"
          class="biaodan"
          ref="queryForm"
          :inline="true"
        >
          <el-form-item label="名称" prop="key">
            <el-input
              v-model="queryParams.key"
              class="set_radius"
              placeholder="请输入搜索关键字"
              clearable
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>
          <el-form-item label="接收时间" prop="deployTime">
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
        <el-table
          v-loading="loading"
          :close-on-click-modal="false"
          :data="todoList"
          class="data_table"
          :row-style="isRed" :cell-style="isRed"
          border
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column
            label="任务编号"
            align="center"
            prop="ExecutionNodeId"
            :show-overflow-tooltip="true"
          />
          <el-table-column label="流程名称" align="center" prop="FlowName" />
          <el-table-column label="任务节点" align="center" prop="StepName" />
          <el-table-column label="流程发起人" align="center">
            <template slot-scope="scope">
              <label>
                <el-tag>{{scope.row.startDeptName}}</el-tag>
                {{scope.row.startRealName}}
              </label>
            </template>
          </el-table-column>
          <el-table-column label="接收时间" align="center" prop="StartTime" width="180" />
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button
                type="text"
                icon="el-icon-edit-outline"
                @click="handleProcess(scope.row)"
              >处理</el-button>
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

      <record-form ref="recordForm" @finish="getList"></record-form>
    </div>
  </div>
</template>

<script>
import { todoList } from "@/api/flowable/process.js";
import RecordForm from "../record/edit.vue";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "ToDoList",
  components: { RecordForm },
  mixins: [resizeTableCon],
  data() {
    return {
      // 遮罩层
      loading: true,
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
      // 流程待办任务表格数据
      todoList: [],
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      dateRange: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        key: null
      },
      // 表单参数
      form: {},
      // 表单校验
      rules: {}
    };
  },
  created() {
    this.getList();
  },
  methods: {
    /** 查询流程定义列表 */
    getList() {
      this.loading = true;
      todoList(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          this.todoList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    // 跳转到处理页面
    handleProcess(row) {
      this.$refs.recordForm.openDialog(row.ExecutionNodeId);
    },
    // 取消按钮
    cancel() {
      this.open = false;
      this.reset();
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
      this.ids = selection.map(item => item.id);
      this.single = selection.length !== 1;
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
    /** 新增按钮操作 */
    handleAdd() {
      this.reset();
      this.open = true;
      this.title = "添加流程定义";
    },
    /** 修改按钮操作 */
    handleUpdate(row) {
      this.reset();
      const id = row.id || this.ids;
      getDeployment(id).then(response => {
        this.form = response.data;
        this.open = true;
        this.title = "修改流程定义";
      });
    }
  }
};
</script>

