<template>
    <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
      <div>
        <el-row :gutter="20">
          <!--用户数据-->
          <el-col :span="24" :xs="24">
            <div class="from_con" id="from_con">
              <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
                <el-form-item label="状态" prop="Status">
                    <el-select v-model="queryParams.Status" clearable placeholder="请选择状态">
                        <el-option label="进行中" :value="0" />
                        <el-option label="已完成" :value="1" />
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
                <el-table-column label="工单编号" align="center">
                  <template slot-scope="scope">
                    <span>{{ scope.row.WorkNumber }}</span>
                  </template>
                </el-table-column>
                <el-table-column label="产品编号" align="center">
                  <template slot-scope="scope">
                    <span>{{ scope.row.SkuNumber }}</span>
                  </template>
                </el-table-column>
                <el-table-column label="产品名称" align="center" :show-overflow-tooltip="true">
                  <template slot-scope="scope">
                    <span>{{ scope.row.ProductName }}</span>
                  </template>
                </el-table-column>
                <el-table-column label="工序名称" align="center" :show-overflow-tooltip="true">
                  <template slot-scope="scope">
                    <span>{{ scope.row.OperName }}</span>
                  </template>
                </el-table-column>
                <el-table-column label="可报工人员" align="center" prop="DefectNum" :show-overflow-tooltip="true"/>
                <el-table-column label="报工数配比" align="center" prop="WorkTime" />
                <el-table-column label="工时(分钟)" align="center" prop="WorkTime" />
                <el-table-column label="总工时(分钟)" align="center" prop="WorkTime" />
                <el-table-column label="计划数" align="center" prop="DefectStr" :show-overflow-tooltip="true"/>
                <el-table-column label="良品数" align="center" prop="WorkTime" />
                <el-table-column label="不良品数" align="center" prop="OverReason" :show-overflow-tooltip="true"/>
                <el-table-column label="状态" align="center">
                  <template slot-scope="scope">
                    <span v-if="scope.row.IsFinish == true">已完成</span>
                    <span v-else>进行中</span>
                  </template>
                </el-table-column>
                <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="150">
                  <template slot-scope="scope">
                    <el-button type="text" icon="el-icon-notebook-2" @click="handleDetail(scope.row)">详情</el-button>
                    <el-button type="text" icon="el-icon-document-add" @click="handleReport(scope.row)">报工</el-button>
                  </template>
                </el-table-column>
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
          Status: -1,
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
        else{
          this.queryParams.beginTime = '';
          this.queryParams.endTime = ''; 
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