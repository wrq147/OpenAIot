<template>
    <div style="padding:20px 20px 0 20px;height:100%" id="big_con">
      <div>
        <el-row :gutter="20">
          <!--用户数据-->
          <el-col :span="24" :xs="24">
            <div class="from_con" id="from_con">
              <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
                <el-form-item label="状态" prop="status">
                    <el-select v-model="queryParams.status" placeholder="请选择状态">
                        <el-option label="待提交" :value="0" />
                        <el-option label="待审批" :value="1" />
                        <el-option label="待执行" :value="2" />
                        <el-option label="执行中" :value="3" />
                        <el-option label="已完成" :value="4" />
                        <el-option label="已取消" :value="5" />
                        <el-option label="已驳回" :value="6" />
                    </el-select>
                </el-form-item>
                <el-form-item label="创建日期">
                    <el-date-picker class="set_radius" v-model="time" style="width:232px" value-format="yyyy-MM-dd"
                    type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期" />
                </el-form-item>
                <el-form-item class="submit_button_con">
                  <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                  <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                </el-form-item>
              </el-form>
            </div>
            <div class="elbiaoge_elform">
              <el-table v-loading="loading" :data="taskList" class="data_table" style="width:100%">
                <el-table-column label="唯一编号" align="center" prop="Number" :show-overflow-tooltip="true" />
                <el-table-column label="良品数" align="center" prop="GoodNum" />
                <el-table-column label="不良品数" align="center" prop="DefectNum" />
                <el-table-column label="不良品项" align="center" prop="DefectStr" />
                <el-table-column label="状态" align="center">
                  <template slot-scope="scope">
                    <span v-if="scope.row.Status == 0">待提交</span>
                    <span v-if="scope.row.Status == 1">待审批</span>
                    <span v-if="scope.row.Status == 2">待执行</span>
                    <span v-if="scope.row.Status == 3">执行中</span>
                    <span v-if="scope.row.Status == 4">已完成</span>
                    <span v-if="scope.row.Status == 5">已取消</span>
                    <span v-if="scope.row.Status == 6">已驳回</span>
                  </template>
                </el-table-column>
                <el-table-column label="报工时长(分钟)" align="center" prop="WorkTime" />
                <el-table-column label="超时原因" align="center" prop="OverReason" />
                <el-table-column label="开始时间" align="center" prop="StartWork" />
                <el-table-column label="结束时间" align="center" prop="EndWork" />
                
                <!-- <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="150">
                  <template slot-scope="scope">
                    <el-button type="text" icon="el-icon-edit" @click="handleAdd(scope.row)">编辑</el-button>
                  </template>
                </el-table-column> -->
              </el-table>
              <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                :limit.sync="queryParams.pageSize" @pagination="getList" />
            </div>
          </el-col>
        </el-row>
      </div>
    </div>
</template>
  
<script>
  import { taskList } from "@/api/mes/task";
  export default {
    name: "taskList",
    data() {
      return {
        loading: false,
        // 查询参数
        queryParams: {
          status: '',
          pageNum: 1,
          pageSize: 20,
          beginTime: '',
          endTime: ''
        },
        time: [],
        total: 0,
        taskList: []
      }
    },
    created() {
      this.getList();
    },
    methods: {
      getList() {
        this.open = false;
        this.loading = true;
        if(this.time.length > 0) {
          this.queryParams.beginTime = this.time[0];
          this.queryParams.endTime = this.time[1];
        }
        taskList(this.queryParams).then(response => {
          this.taskList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        })
      },
      /** 搜索按钮操作 */
      handleQuery() {
        this.queryParams.pageNum = 1;
        this.getList();
      },
      /** 重置按钮操作 */
      resetQuery() {
        this.time= []; 
        this.resetForm("queryForm");
        this.handleQuery();
      },
    }
  }
</script>