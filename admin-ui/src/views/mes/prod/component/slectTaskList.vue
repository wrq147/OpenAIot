<template>
  <el-dialog v-if="planOpen" title="请选择生产任务" :visible.sync="planOpen" :close-on-click-modal="false" append-to-body
    width="980px" top="2vh" @close="cancel">
    <el-form class="biaodan" :model="queryParams" ref="taskForm" :inline="true">
      <el-form-item label="创建日期">
        <el-date-picker class="set_radius" v-model="time" style="width:232px" value-format="yyyy-MM-dd" type="daterange"
          range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期" />
      </el-form-item>
      <el-form-item class="submit_button_con">
        <el-button icon="el-icon-refresh" @click="resetTask">重置</el-button>
        <el-button type="primary" icon="el-icon-search" @click="taskQuery.pageNum = 1; loadTaskList()">搜索</el-button>
      </el-form-item>
    </el-form>
    <el-table ref="devTable" v-loading="loading" :data="taskList" class="data_table" tooltip-effect="dark"
      style="width:100%" highlight-current-row @current-change="onTaskChange" row-key="Id">
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
      <el-table-column label="状态" align="center">
        <template slot-scope="scope">
          <span v-if="scope.row.IsFinish == true">已完成</span>
          <span v-else>进行中</span>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize"
      @pagination="loadTaskList" />
  </el-dialog>
</template>
<script>
import { taskList } from "@/api/mes/task";
export default {
  name: 'selectTaskList',
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
      // 查询参数
      queryParams: {
        Status: 0,
        pageNum: 1,
        pageSize: 20,
        beginTime: '',
        endTime: ''
      },
      time: [],
      total: 0,
      taskList: [],
      planOpen: false
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.planOpen = newValue
      if (newValue) {
        this.loadTaskList()
      }

    }
  },
  methods: {
    loadTaskList() {
      this.loading = true;
      if (this.time.length > 0) {
        this.queryParams.beginTime = this.time[0];
        this.queryParams.endTime = this.time[1];
      }
      else {
        this.queryParams.beginTime = '';
        this.queryParams.endTime = '';
      }
      taskList(this.queryParams).then(response => {
        this.taskList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      })
    },
    resetTask() {
      this.devDateRange = [];
      this.resetForm("taskForm");
      this.loadTaskList();
    },
    onTaskChange(val) {
      this.$emit('taskSelect', val)
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
  .el-dialog__header {
    border-bottom: 1px solid #ccc;
  }

  .el-select,
  .el-cascader {
    width: 100%;
  }
}

.addPeople>.box {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
}

.addPeople>.btn {
  width: 100%;
  justify-content: flex-end;
  display: flex;
  align-items: center;
}
</style>