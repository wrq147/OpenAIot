<template>
    <el-dialog v-if="planOpen" title="请选择所属物料" :visible.sync="planOpen" :close-on-click-modal="false" append-to-body width="980px" top="2vh" @close="cancel">
        <el-form :model="planQuery" ref="planForm" :inline="true" style="display: flex; justify-content: space-between">
          <div>
            <el-form-item label="创建日期">
                <el-date-picker class="set_radius" v-model="devDateRange" style="width:232px" value-format="yyyy-MM-dd"
                type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期" />
            </el-form-item>
          </div>
          <el-form-item>
            <el-button icon="el-icon-refresh" @click="resetPlan">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="planQuery.pageNum = 1;loadPlanList()">搜索</el-button>
          </el-form-item>
        </el-form>
        <el-table
          ref="devTable"
          :data="planList"
          tooltip-effect="dark"
          v-loading="loading"
          style="width: 100%"
          highlight-current-row
          @current-change="onPlanChange"
          row-key="Id"
        >
          <el-table-column label="唯一编号" align="center" prop="Number" :show-overflow-tooltip="true" />
          <el-table-column label="计划名称" align="center" prop="PlanName" :show-overflow-tooltip="true"/>
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
          <el-table-column label="超期时间" align="center" prop="OverTime" :show-overflow-tooltip="true"/>
        </el-table>
        <pagination
          v-show="total > 0"
          :total="total"
          :page.sync="planQuery.pageNum"
          :limit.sync="planQuery.pageSize"
          @pagination="loadPlanList"
        />
    </el-dialog>
</template>
<script>
import { planList } from "@/api/mes/plan";
export default { 
  name: 'selectPlanList',
  props: {
    dialogVisible: {
      type: Boolean
    },
    title: {
      type: String
    }
  },
  data() {
    return {
      loading: false,
      planOpen: false,
      devDateRange: [],
      planList: [],
      planQuery: {
        Key: '',
        pageNum: 1,
        pageSize: 10,
      },
      total: 0
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.planOpen = newValue
      if(newValue){
        this.loadPlanList()
      }
      
    }
  },
  methods: {
    loadPlanList() {
        this.loading = true;
        if(this.devDateRange.length > 0) {
          this.queryParams.beginTime = this.devDateRange[0];
          this.queryParams.endTime = this.devDateRange[1];
        }
        planList(this.planQuery).then(response => {
            this.planList = response.data.List;
            this.total = response.data.Total;
            this.loading = false;
        })
    },
    resetPlan() {
      this.devDateRange = [];
      this.resetForm("planForm");
      this.loadPlanList();
    },
    onPlanChange(val) {
      console.log("选择后",val);
        this.$emit('planSelect', val)
        this.cancel()
    },
    cancel() {
      this.$emit('cancelForm')
    }
  }
}
</script>
  
<style lang="scss" scoped>
 ::v-deep {
  .el-dialog__header{
    border-bottom: 1px solid #ccc;
  }
  .el-select, .el-cascader{
    width: 100%;
  }
}
.addPeople>.box{
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
}
.addPeople>.btn{
  width:100%;
  justify-content: flex-end;
  display: flex;
  align-items: center;
}
</style>