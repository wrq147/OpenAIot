<template>
    <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
      <div>
        <el-row :gutter="20">
          <!--用户数据-->
          <el-col :span="24" :xs="24">
            <div class="from_con" id="from_con">
              <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
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
              <el-row :gutter="10" class="mb8 button_row">
                <div>
                  <el-col :span="1.5">
                    <el-button type="primary" icon="el-icon-plus" plain @click="handleAdd('')">新增生产计划</el-button>
                  </el-col>
                </div>
              </el-row>
  
              <el-table v-loading="loading" :data="planList" class="data_table" style="width:100%">
                <el-table-column label="唯一编号" align="center" prop="Number" :show-overflow-tooltip="true" />
                <el-table-column label="计划名称" align="center" prop="PlanName" />
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
                <el-table-column label="优先级" align="center">
                  <template slot-scope="scope">
                    <span v-if="scope.row.Priority == 1">优先安排</span>
                    <span v-if="scope.row.Priority == 2">加急处理</span>
                    <span v-if="scope.row.Priority == 3">正常排产</span>
                  </template>
                </el-table-column>
                <el-table-column label="超期时间" align="center" prop="OverTime" />
                <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="200">
                  <template slot-scope="scope">
                    <el-button v-if="scope.row.Status == 0" type="text" icon="el-icon-edit" @click="handleAdd(scope.row)">编辑</el-button>
                    <el-button v-else type="text" icon="el-icon-notebook-2" @click="handleDetail(scope.row)">详情</el-button>
                    <el-button v-if="scope.row.Status === 0 || scope.row.Status === 5" type="text" icon="el-icon-delete" style="color:red"
                      @click="handleDelete(scope.row.Id)">删除</el-button>
                    <el-button v-if="scope.row.Status === 1" type="text" style="color:rgb(230, 162, 60)" icon="el-icon-close" @click="handleCancel(scope.row.Id)">取消</el-button>
                  </template>
                </el-table-column>
              </el-table>
              <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                :limit.sync="queryParams.pageSize" @pagination="getList" />
            </div>
          </el-col>
        </el-row>
      </div>
      <!-- 新增/编辑生产计划弹窗 -->
      <add-plan ref="addPlan" :title="title" :dialog-visible="open" @cancelForm="cancelForm" @getList="getList" />
      <!-- 详情生产计划弹窗 -->
      <detail-plan ref="detailPlan" :dialog-visible="openDetail" @cancelForm="cancelForm" />
    </div>
</template>
  
<script>
  import { planList, operRemove, planeNumber, planeCancel } from "@/api/mes/plan";
  import addPlan from './cmp/addPlan.vue';
  import detailPlan from './cmp/detailPlan.vue';
  export default {
    name: "BatchList",
    components: {
      addPlan,
      detailPlan
    },
    data() {
      return {
        loading: false,
        open: false,
        openDetail: false,
        title: '新增生产计划',
        // 查询参数
        queryParams: {
          pageNum: 1,
          pageSize: 20,
          beginTime: '',
          endTime: ''
        },
        time: [],
        total: 0,
        planList: []
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
        planList(this.queryParams).then(response => {
          this.planList = response.data.List;
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
      /** 新增按钮操作 */
      async handleAdd(data) {
        if (data === '') {
          this.title = '新增生产计划';
          const number = await planeNumber();
          this.$refs['addPlan'].ruleForm = {
            id: '',
            number: number.data,
            planName: '',
            status: 0,
            priority: '',
            flowId: '',
            overTime: '',
            items: []
          };
        } else {
          this.title = '编辑生产计划';
          this.$refs['addPlan'].ruleForm = {
            id: data.Id,
            number: data.Number,
            planName: data.PlanName,
            status: data.Status,
            priority: data.Priority,
            flowId: data.FlowId,
            overTime: data.OverTime,
            items: []
          };
          this.$refs['addPlan'].getDataInfo(data.Id);
        }
        this.open = true;
      },
      // 详情按钮操作
      handleDetail(data) {
        this.$refs['detailPlan'].ruleForm = {
            id: data.Id,
            number: data.Number,
            planName: data.PlanName,
            status: data.Status,
            priority: data.Priority,
            flowId: data.FlowId,
            overTime: data.OverTime,
            items: []
          };
          this.$refs['detailPlan'].getDataInfo(data.Id);
          this.openDetail = true;
      },
      /** 删除按钮操作 */
      handleDelete(id) {
        this.$confirm('此操作将永久删除该数据, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
            operRemove({ id: id }).then(res => {
              this.$message.success('删除成功!')
              this.getList()
            })
        }).catch(() => { })
      },
      handleCancel(id) {
        this.$confirm('此操作将取消该计划, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
            planeCancel({ id: id }).then(res => {
              this.$message.success('取消成功!')
              this.getList()
            })
        }).catch(() => { })
      },
      cancelForm() {
        this.open = false;
        this.openDetail = false;
      },
    }
  }
  </script>