<template>
  <div class="zlmediakit-record-plan-list">
    <!-- 页面标题与操作区：移除查看执行日志按钮 -->
    <div class="page-header">
      <div>
        <h2>ZLMediaKit 录像计划管理</h2>
        <p class="text-gray">管理录像计划的启用、编辑与删除</p>
      </div>
      <div>
        <el-button type="primary" icon="el-icon-plus" @click="handleGoAdd">新增录像计划</el-button>
      </div>
    </div>

    <!-- 搜索筛选区域 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="流ID">
          <el-input v-model="searchForm.streamId" placeholder="请输入流ID关键词" clearable></el-input>
        </el-form-item>
        <el-form-item label="时段类型">
          <el-select v-model="searchForm.recordTimeType" placeholder="全部" clearable>
            <el-option label="按周" value="week"></el-option>
            <el-option label="按时段" value="time"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="计划状态">
          <el-select v-model="searchForm.status" placeholder="全部" clearable>
            <el-option label="启用" value="1"></el-option>
            <el-option label="禁用" value="0"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" @click="fetchRecordPlans">查询</el-button>
          <el-button icon="el-icon-refresh" @click="resetSearchForm">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 录像计划列表 -->
    <el-card shadow="never">
      <el-table v-loading="loading" :data="recordPlanList" border stripe @selection-change="handleSelectionChange">
        <el-table-column prop="streamId" label="流ID" min-width="180"></el-table-column>
        <el-table-column prop="saveCycle" label="保存周期(天)" width="120"></el-table-column>
        <el-table-column prop="recordTimeType" label="时段类型" width="120" :formatter="formatTimeType"></el-table-column>
        <el-table-column prop="recordTimeDesc" label="录像时段" min-width="200" show-overflow-tooltip></el-table-column>
        <el-table-column prop="status" label="状态" width="100" :formatter="formatStatus"></el-table-column>
        <el-table-column prop="createTime" label="创建时间" width="180"></el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
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
      <el-pagination @size-change="handleSizeChange" @current-change="handleCurrentChange"
        :current-page="pagination.pageNum" :page-sizes="[10, 20, 50, 100]" :page-size="pagination.pageSize"
        :total="pagination.total">
      </el-pagination>
    </el-card>
  </div>
</template>

<script>

export default {
  data() {
    return {
      loading: false,
      // 搜索表单
      searchForm: {
        streamId: '',
        recordTimeType: '',
        status: ''
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
        const res = await getRecordPlanList({
          pageNum: this.pagination.pageNum,
          pageSize: this.pagination.pageSize,
          ...this.searchForm
        })
        if (res.code === 200) {
          this.recordPlanList = res.data.list
          this.pagination.total = res.data.total
        }
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
     * 分页大小改变
     */
    handleSizeChange(val) {
      this.pagination.pageSize = val
      this.fetchRecordPlans()
    },

    /**
     * 当前页改变
     */
    handleCurrentChange(val) {
      this.pagination.pageNum = val
      this.fetchRecordPlans()
    },

    /**
     * 选择行改变
     */
    handleSelectionChange(val) {
      this.selectedRows = val
    },

    /**
     * 跳转至新增录像计划（统一详情页-新增模式）
     */
    handleGoAdd() {
      this.$router.push('/zlmediakit/record-plan/detail')
    },

    /**
     * 编辑录像计划（跳转至统一详情页-编辑模式）
     */
    handleEdit(row) {
      this.$router.push({
        path: '/zlmediakit/record-plan/detail',
        query: { id: row.id }
      })
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

<style scoped>
.zlmediakit-record-plan-list {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
}

.text-gray {
  color: #999;
  font-size: 12px;
  margin-top: 4px;
}

.search-card {
  margin-bottom: 0;
}
</style>