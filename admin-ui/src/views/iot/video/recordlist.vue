<template>
  <div class="zlmediakit-record-plan-list">

    <!-- 搜索筛选区域 -->
    <el-card class="search-card" shadow="never" style="margin-bottom: 20px;">
      <div style="display: flex;flex-direction: row;justify-content: space-between;">
        <el-form :inline="true" :model="searchForm" class="search-form">
          <el-form-item label="通讯编码">
            <el-input v-model="searchForm.VideoId" placeholder="请输入通讯编码" clearable></el-input>
          </el-form-item>
          <el-form-item label="时段类型">
            <el-select v-model="searchForm.RecordTimeType" placeholder="全部" clearable>
              <el-option label="按周" value="week"></el-option>
              <el-option label="按时段" value="time"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="计划状态">
            <el-select v-model="searchForm.Status" placeholder="全部" clearable>
              <el-option label="启用" value="1"></el-option>
              <el-option label="禁用" value="0"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" icon="el-icon-search" @click="fetchRecordPlans">查询</el-button>
            <el-button icon="el-icon-refresh" @click="resetSearchForm">重置</el-button>
          </el-form-item>
        </el-form>
        <div>
          <el-button type="primary" icon="el-icon-plus" @click="handleGoAdd">新增录像计划</el-button>
        </div>

      </div>
    </el-card>

    <!-- 录像计划列表 -->
    <el-card shadow="never">
      <el-table v-loading="loading" :data="recordPlanList" border stripe @selection-change="handleSelectionChange">
        <el-table-column prop="VideoId" label="通讯编码" align="center" min-width="180"></el-table-column>
        <el-table-column prop="SaveCycle" label="保存周期(天)" align="center" width="120"></el-table-column>
        <el-table-column prop="RecordTimeType" label="时段类型" align="center" width="120" :formatter="formatTimeType"></el-table-column>
        <el-table-column prop="RecordTimeDesc" label="录像时段" min-width="200" show-overflow-tooltip></el-table-column>
        <el-table-column prop="Status" label="状态" width="100" align="center" :formatter="formatStatus"></el-table-column>
        <el-table-column prop="createTime" label="创建时间" align="center" width="180"></el-table-column>
        <el-table-column label="操作" align="center" width="200">
          <template slot-scope="scope">
            <el-button type="text" icon="el-icon-edit" @click="handleEdit(scope.row)">编辑</el-button>
            <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)"
              v-if="scope.row.status === 0">删除</el-button>
            <el-button icon="el-icon-switch-button" @click="handleToggleStatus(scope.row)"
              :type="scope.row.status === 1 ? 'warning' : 'success'">
              {{ scope.row.status === 1 ? '禁用' : '启用' }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页控件 -->
      <pagination v-show="pagination.total > 0" :total="pagination.total" :page.sync="pagination.pageNum"
        :limit.sync="pagination.pageSize" @pagination="getList" />
    </el-card>

    <record ref="recDlg" />
  </div>
</template>

<script>
import { recordList } from "@/api/rules/record";
import record from './record.vue'
export default {
  components: {
    record
  },
  data() {
    return {
      loading: false,
      // 搜索表单
      searchForm: {
        VideoId: '',
        RecordTimeType: '',
        Status: ''
      },
      // 录像计划列表
      recordPlanList: [],
      // 分页参数
      pagination: {
        pageNum: 1,
        pageSize: 10,
        total: 0
      },
      selectedRows: []
    }
  },
  created() {
    this.fetchRecordPlans()
  },
  methods: {
    /**
     * 获取录像计划列表
     */
    async fetchRecordPlans() {
      this.loading = true
      try {
        const res = await recordList({
          pageNum: this.pagination.pageNum,
          pageSize: this.pagination.pageSize,
          ...this.searchForm
        })
        this.recordPlanList = res.data.List
        this.pagination.total = res.data.Total
      } catch (error) {
        this.$message.error('获取录像计划列表失败：' + error.message)
      } finally {
        this.loading = false
      }
    },

    /**
     * 格式化时段类型
     */
    formatTimeType(row) {
      return row.recordTimeType === 'week' ? '按周' : '按时段'
    },

    /**
     * 格式化状态显示
     */
    formatStatus(row) {
      return row.status === 1
        ? '<span style="color: green;">启用</span>'
        : '<span style="color: red;">禁用</span>'
    },

    /**
     * 重置搜索表单
     */
    resetSearchForm() {
      this.searchForm = { streamId: '', recordTimeType: '', status: '' }
      this.fetchRecordPlans()
    },


    /**
     * 当前页改变
     */
    getList() {
      this.fetchRecordPlans()
    },

    /**
     * 选择行改变
     */
    handleSelectionChange(val) {
      this.selectedRows = val
    },

    /**
     * 跳转至新增录像计划
     */
    handleGoAdd() {
      this.$refs.recDlg.openDlg();
    },

    /**
     * 编辑录像计划
     */
    handleEdit(row) {
    },

    /**
     * 删除录像计划
     */
    async handleDelete(row) {
      try {
        await this.$confirm('确定要删除该录像计划吗？', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        })
        const res = await deleteRecordPlan(row.id)
        if (res.code === 200) {
          this.$message.success('删除成功！')
          this.fetchRecordPlans()
        } else {
          this.$message.error('删除失败：' + res.msg)
        }
      } catch (error) {
        if (error !== 'cancel') {
          this.$message.error('删除操作失败：' + error.message)
        }
      }
    },

    /**
     * 切换录像计划状态
     */
    async handleToggleStatus(row) {
      const statusText = row.status === 1 ? '禁用' : '启用'
      try {
        await this.$confirm(`确定要${statusText}该录像计划吗？`, '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        })
        const res = await toggleRecordPlanStatus({
          id: row.id,
          status: row.status === 1 ? 0 : 1
        })
        if (res.code === 200) {
          this.$message.success(`${statusText}成功！`)
          this.fetchRecordPlans()
        } else {
          this.$message.error(`${statusText}失败：` + res.msg)
        }
      } catch (error) {
        if (error !== 'cancel') {
          this.$message.error(`${statusText}操作失败：` + error.message)
        }
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.zlmediakit-record-plan-list {
  padding: 20px;
}

.search-card {
  .el-form-item {
    margin-bottom: 0px;
  }
}
</style>