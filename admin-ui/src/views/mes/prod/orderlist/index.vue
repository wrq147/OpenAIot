<template>
  <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="状态" prop="status">
                <el-select v-model="queryParams.status" placeholder="请选择状态" clearable>
                  <el-option label="待排产" :value="0" />
                  <el-option label="待生产" :value="1" />
                  <el-option label="生产中" :value="2" />
                  <el-option label="已完成" :value="3" />
                  <el-option label="已取消" :value="4" />
                </el-select>
              </el-form-item>
              <el-form-item label="创建时间">
                <el-date-picker class="set_radius" v-model="dateRange" style="width:232px"
                  value-format="yyyy-MM-dd HH:mm:ss" type="datetimerange" range-separator="-" start-placeholder="开始时间"
                  end-placeholder="结束时间" />
              </el-form-item>
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
            <el-table v-loading="loading" :data="taskList" class="data_table" style="width:100%">
              <el-table-column label="计划名称" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.PlanInfo ? scope.row.PlanInfo.PlanName : '' }}</span>
                </template>
              </el-table-column>
              <el-table-column label="产品名称" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.ProdInfo ? scope.row.ProdInfo.ProductName : '' }}</span>
                </template>
              </el-table-column>
              <el-table-column label="工单编号" align="center" prop="WorkNumber" />
              <el-table-column label="父工单" align="center">
                <template slot-scope="scope">
                  <el-tooltip placement="top" effect="dark" :disabled="!scope.row.ParentWorkInfo">
                    <div slot="reference" style="cursor: pointer;">
                      {{ scope.row.ParentWorkInfo ? scope.row.ParentWorkInfo.WorkNumber : '无' }}
                    </div>
                    <div v-if="scope.row.ParentWorkInfo" style="max-width: 400px; text-align: left;">
                      <p><strong>父工单编号：</strong>{{ scope.row.ParentWorkInfo.WorkNumber }}</p>
                      <p><strong>计划名称：</strong>{{ scope.row.ParentWorkInfo.PlanName }}</p>
                      <p><strong>产品名称：</strong>{{ scope.row.ParentWorkInfo.ProductName }}</p>
                      <p><strong>优先级：</strong>
                        {{ scope.row.ParentWorkInfo.priority === 1 ? '优先安排' :
                          scope.row.ParentWorkInfo.priority === 2 ? '加急处理' : '正常排产' }}
                      </p>
                      <p><strong>状态：</strong>
                        {{ scope.row.ParentWorkInfo.Status === 0 ? '待生产' :
                          scope.row.ParentWorkInfo.Status === 1 ? '生产中' :
                            scope.row.ParentWorkInfo.Status === 2 ? '已完成' : '已取消' }}
                      </p>
                      <p><strong>计划时间：</strong>{{ scope.row.ParentWorkInfo.PlannedStartOn }} 至 {{
                        scope.row.ParentWorkInfo.PlannedEndOn }}</p>
                    </div>
                  </el-tooltip>
                </template>
              </el-table-column>
              <el-table-column label="优先级" align="center" prop="Priority">
                <template slot-scope="scope">
                  <span v-if="scope.row.Priority == 1">优先安排</span>
                  <span v-if="scope.row.Priority == 2">加急处理</span>
                  <span v-if="scope.row.Priority == 3">正常排产</span>
                </template>
              </el-table-column>
              <el-table-column label="状态" align="center">
                <template slot-scope="scope">
                  <el-tag v-if="scope.row.Status == 0" type="warning">待生产</el-tag>
                  <el-tag v-if="scope.row.Status == 1" type="warning">生产中</el-tag>
                  <el-tag v-if="scope.row.Status == 2" type="success">已完成</el-tag>
                  <el-tag v-if="scope.row.Status == 3" type="danger">已取消</el-tag>
                </template>
              </el-table-column>
              <el-table-column label="超期时间" align="center" prop="OverTime" />
              <el-table-column label="计划开始时间" align="center" prop="PlannedStartOn" />
              <el-table-column label="计划结束时间" align="center" prop="PlannedEndOn" />
              <el-table-column label="实际开始时间" align="center" prop="StartOn" />
              <el-table-column label="实际结束时间" align="center" prop="EndOn" />
              <el-table-column label="当前进度" align="center">
                <template slot-scope="scope">
                  <template v-if="scope.row.Status == 1">
                    <el-progress type="line" :percentage="Math.round((scope.row.BatchCount / scope.row.Quantity) * 100)"
                      :text-inside="true" :stroke-width="10" status="success" />
                    <div style="font-size: 12px; color: #666; margin-top: 4px;">
                      {{ scope.row.BatchCount }}/{{ scope.row.Quantity }}
                    </div>
                  </template>
                  <template v-else>
                    <span style="color: #999;">无数据</span>
                  </template>
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
import { mesOrderList } from "@/api/mes/report";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "OrderList",
  mixins: [resizeTableCon],
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
      dateRange: [],
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
      // if(this.dateRange.length > 0) {
      //   this.queryParams.beginTime = this.dateRange[0];
      //   this.queryParams.endTime = this.dateRange[1];
      // }
      if (this.queryParams.status != null && this.queryParams.status != undefined) { } else {
        delete this.queryParams.status
      }
      mesOrderList(this.addDateRange(this.queryParams, this.dateRange)).then(response => {
        console.log("生产工单", response);
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
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
  }
}
</script>