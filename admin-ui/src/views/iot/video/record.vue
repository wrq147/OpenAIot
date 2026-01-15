<template>
  <div class="zlmediakit-record-plan-detail">
    <!-- 页面标题与返回按钮 -->
    <div class="page-header">
      <el-button
        type="text"
        icon="el-icon-back"
        @click="handleGoBack"
        class="mr-4"
      >返回录像计划列表</el-button>
      <h2>{{ isEdit ? '编辑录像计划' : '新增录像计划' }}</h2>
    </div>

    <!-- 核心内容卡片 -->
    <el-card shadow="never" class="mt-4">
      <el-tabs v-model="activeTab" type="card">
        <!-- 基础配置标签页（新增/编辑共用） -->
        <el-tab-pane label="基础配置" name="config">
          <el-form
            ref="recordPlanForm"
            :model="recordPlanForm"
            :rules="recordPlanRules"
            label-width="100px"
            class="form-container"
          >
            <el-form-item label="流ID" prop="streamId">
              <el-input v-model="recordPlanForm.streamId" placeholder="例如：camera_01"></el-input>
            </el-form-item>
            <el-form-item label="保存周期" prop="saveCycle">
              <el-input-number
                v-model="recordPlanForm.saveCycle"
                :min="1"
                :max="365"
                placeholder="录像文件保存天数"
                controls-position="right"
              ></el-input-number>
              <span class="ml-2 text-gray">超出周期的录像文件将自动删除</span>
            </el-form-item>

            <!-- 录像时段配置 -->
            <el-form-item label="录像时段" prop="recordTimeConfig">
              <el-radio-group v-model="recordPlanForm.recordTimeType" @change="handleTimeTypeChange">
                <el-radio label="week">按周配置</el-radio>
                <el-radio label="custom">自定义日历标签</el-radio>
              </el-radio-group>

              <!-- 按周配置 -->
              <div v-if="recordPlanForm.recordTimeType === 'week'" class="week-time-config mt-3">
                <div class="week-header mb-2">
                  <span class="week-item" v-for="(item, idx) in weekList" :key="idx">{{ item.label }}</span>
                  <span class="time-item">开始时间</span>
                  <span class="time-item">结束时间</span>
                </div>
                <div
                  class="week-row mb-2"
                  v-for="(item, idx) in recordPlanForm.weekTimeList"
                  :key="idx"
                >
                  <el-checkbox v-model="item.checked" class="week-checkbox">
                    {{ weekList[idx].label }}
                  </el-checkbox>
                  <el-time-picker
                    v-model="item.startTime"
                    format="HH:mm:ss"
                    value-format="HH:mm:ss"
                    placeholder="选择开始时间"
                    class="time-picker"
                    :disabled="!item.checked"
                  ></el-time-picker>
                  <el-time-picker
                    v-model="item.endTime"
                    format="HH:mm:ss"
                    value-format="HH:mm:ss"
                    placeholder="选择结束时间"
                    class="time-picker"
                    :disabled="!item.checked"
                  ></el-time-picker>
                </div>
              </div>

              <!-- 自定义日历标签配置 -->
              <div v-if="recordPlanForm.recordTimeType === 'custom'" class="custom-time-config mt-3">
                <el-form-item label="选择日历标签" prop="customTag">
                  <el-select
                    v-model="recordPlanForm.customTag"
                    placeholder="请选择已维护的日历标签"
                    clearable
                  >
                    <el-option
                      v-for="tag in customTagList"
                      :key="tag.value"
                      :label="tag.label"
                      :value="tag.value"
                    ></el-option>
                  </el-select>
                </el-form-item>
                <el-form-item label="标签内录像时段">
                  <el-time-picker
                    v-model="recordPlanForm.customStartTime"
                    format="HH:mm:ss"
                    value-format="HH:mm:ss"
                    placeholder="选择开始时间"
                    class="w-100"
                  ></el-time-picker>
                  <el-time-picker
                    v-model="recordPlanForm.customEndTime"
                    format="HH:mm:ss"
                    value-format="HH:mm:ss"
                    placeholder="选择结束时间"
                    class="w-100 mt-2"
                  ></el-time-picker>
                </el-form-item>
                <div class="text-gray mt-2">
                  说明：选择标签后，将在标签对应的日期内按设置时段录像
                </div>
              </div>
            </el-form-item>

            <el-form-item label="备注" prop="remark">
              <el-input
                v-model="recordPlanForm.remark"
                type="textarea"
                placeholder="请输入备注信息"
                rows="3"
              ></el-input>
            </el-form-item>

            <el-form-item class="form-btn-group">
              <el-button type="default" @click="handleGoBack">取消</el-button>
              <el-button type="primary" @click="handleSubmit">{{ isEdit ? '更新' : '提交' }}</el-button>
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <!-- 执行日志标签页（仅编辑模式显示） -->
        <el-tab-pane label="执行日志" name="log" v-if="isEdit">
          <div class="log-search-bar mb-3">
            <el-form :inline="true" :model="logSearchForm" class="log-search-form">
              <el-form-item label="日志类型">
                <el-select v-model="logSearchForm.logType" placeholder="全部" clearable>
                  <el-option label="计划启动" value="start"></el-option>
                  <el-option label="计划停止" value="stop"></el-option>
                  <el-option label="录像成功" value="success"></el-option>
                  <el-option label="录像失败" value="fail"></el-option>
                  <el-option label="文件清理" value="clean"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item label="执行时间">
                <el-date-picker
                  v-model="logSearchForm.timeRange"
                  type="daterange"
                  range-separator="至"
                  start-placeholder="开始日期"
                  end-placeholder="结束日期"
                  value-format="yyyy-MM-dd"
                  clearable
                ></el-date-picker>
              </el-form-item>
              <el-form-item>
                <el-button type="primary" icon="el-icon-search" @click="fetchPlanLog">查询</el-button>
                <el-button icon="el-icon-refresh" @click="resetLogSearch">重置</el-button>
              </el-form-item>
            </el-form>
          </div>

          <el-table
            v-loading="logLoading"
            :data="planLogList"
            border
            stripe
            size="small"
          >
            <el-table-column prop="id" label="日志ID" width="80"></el-table-column>
            <el-table-column prop="logType" label="日志类型" width="100" :formatter="formatLogType"></el-table-column>
            <el-table-column prop="content" label="日志内容" min-width="300" show-overflow-tooltip></el-table-column>
            <el-table-column prop="execTime" label="执行时间" width="180"></el-table-column>
            <el-table-column prop="ip" label="操作IP" width="120"></el-table-column>
            <el-table-column prop="remark" label="备注" min-width="150" show-overflow-tooltip></el-table-column>
          </el-table>

          <el-pagination
            @size-change="handleLogSizeChange"
            @current-change="handleLogCurrentChange"
            :current-page="logPagination.pageNum"
            :page-sizes="[10, 20, 50]"
            :page-size="logPagination.pageSize"
            layout="total, sizes, prev, pager, next, jumper"
            :total="logPagination.total"
            class="mt-4"
            background
          >
          </el-pagination>
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script>

export default {
  data() {
    return {
      activeTab: 'config', // 默认激活基础配置标签
      isEdit: false, // 是否为编辑模式
      // 星期列表
      weekList: [
        { label: '周一', value: 1 },
        { label: '周二', value: 2 },
        { label: '周三', value: 3 },
        { label: '周四', value: 4 },
        { label: '周五', value: 5 },
        { label: '周六', value: 6 },
        { label: '周日', value: 0 }
      ],
      // 自定义日历标签列表
      customTagList: [],
      // 表单数据
      recordPlanForm: {
        id: '',
        streamId: '',
        saveCycle: 1, // 默认保存周期1天
        recordTimeType: 'week', // 默认按周配置
        weekTimeList: [
          { checked: false, startTime: '00:00:00', endTime: '23:59:59' },
          { checked: false, startTime: '00:00:00', endTime: '23:59:59' },
          { checked: false, startTime: '00:00:00', endTime: '23:59:59' },
          { checked: false, startTime: '00:00:00', endTime: '23:59:59' },
          { checked: false, startTime: '00:00:00', endTime: '23:59:59' },
          { checked: false, startTime: '00:00:00', endTime: '23:59:59' },
          { checked: false, startTime: '00:00:00', endTime: '23:59:59' }
        ],
        customTag: '',
        customStartTime: '00:00:00',
        customEndTime: '23:59:59',
        status: 1,
        remark: ''
      },
      // 表单校验规则
      recordPlanRules: {
        streamId: [{ required: true, message: '请输入流ID', trigger: 'blur' }],
        saveCycle: [{ required: true, message: '请输入保存周期', trigger: 'blur' }],
        recordTimeType: [{ required: true, message: '请选择时段类型', trigger: 'change' }],
        customTag: [
          { required: true, message: '请选择日历标签', trigger: 'change' },
          { required: true, message: '请选择日历标签', trigger: 'blur' }
        ]
      },

      // 日志相关
      logLoading: false,
      logSearchForm: {
        logType: '',
        timeRange: []
      },
      planLogList: [],
      logPagination: {
        pageNum: 1,
        pageSize: 10,
        total: 0
      }
    }
  },
  created() {
    // 1. 判断是否为编辑模式（路由参数带id）
    const planId = this.$route.query.id
    if (planId) {
      this.isEdit = true
      this.recordPlanForm.id = planId
      this.fetchPlanDetail(planId) // 加载编辑数据
    }
    // 2. 获取自定义日历标签
    this.fetchCustomTagList()
  },
  methods: {
    /**
     * 获取录像计划详情（编辑模式）
     */
    async fetchPlanDetail(id) {
      try {
        const res = await getRecordPlanDetail(id)
        if (res.code === 200) {
          const data = res.data
          this.recordPlanForm = { ...data }
          // 适配按周配置数据（防止后端返回空）
          if (!this.recordPlanForm.weekTimeList) {
            this.recordPlanForm.weekTimeList = this.weekList.map(() => ({
              checked: false,
              startTime: '00:00:00',
              endTime: '23:59:59'
            }))
          }
          // 初始化日志数据
          this.fetchPlanLog()
        }
      } catch (error) {
        this.$message.error('获取计划详情失败：' + error.message)
        this.handleGoBack()
      }
    },

    /**
     * 获取自定义日历标签列表
     */
    async fetchCustomTagList() {
      try {
        const res = await getCustomTagList()
        if (res.code === 200) {
          this.customTagList = res.data
        }
      } catch (error) {
        this.$message.error('获取日历标签失败：' + error.message)
      }
    },

    /**
     * 时段类型切换
     */
    handleTimeTypeChange() {
      if (this.recordPlanForm.recordTimeType === 'week') {
        this.recordPlanForm.customTag = ''
      } else {
        this.recordPlanForm.weekTimeList.forEach(item => {
          item.checked = false
        })
      }
    },

    /**
     * 生成录像时段描述
     */
    generateTimeDesc(form) {
      if (form.recordTimeType === 'week') {
        const descList = []
        form.weekTimeList.forEach((item, idx) => {
          if (item.checked) {
            descList.push(`${this.weekList[idx].label} ${item.startTime}-${item.endTime}`)
          }
        })
        return descList.length > 0 ? descList.join('、') : '未选择时段'
      } else {
        const tagLabel = this.customTagList.find(tag => tag.value === form.customTag)?.label || form.customTag
        return `${tagLabel} ${form.customStartTime}-${form.customEndTime}`
      }
    },

    /**
     * 提交表单（新增/编辑）
     */
    async handleSubmit() {
      try {
        await this.$refs.recordPlanForm.validate()
        // 生成时段描述
        this.recordPlanForm.recordTimeDesc = this.generateTimeDesc(this.recordPlanForm)

        let res
        if (this.isEdit) {
          res = await editRecordPlan(this.recordPlanForm)
        } else {
          res = await addRecordPlan(this.recordPlanForm)
        }

        if (res.code === 200) {
          this.$message.success(this.isEdit ? '更新成功！' : '新增成功！')
          this.handleGoBack()
        } else {
          this.$message.error((this.isEdit ? '更新' : '新增') + '失败：' + res.msg)
        }
      } catch (error) {
        if (error !== 'cancel') {
          this.$message.error('提交失败：' + error.message)
        }
      }
    },

    /**
     * 返回列表页
     */
    handleGoBack() {
      this.$router.push('/zlmediakit/record-plan/list')
    },

    // ========== 日志相关方法 ==========
    /**
     * 格式化日志类型
     */
    formatLogType(row) {
      const logTypeMap = {
        start: '计划启动',
        stop: '计划停止',
        success: '录像成功',
        fail: '录像失败',
        clean: '文件清理'
      }
      return logTypeMap[row.logType] || '未知'
    },

    /**
     * 获取当前计划的执行日志
     */
    async fetchPlanLog() {
      this.logLoading = true
      try {
        const params = {
          planId: this.recordPlanForm.id,
          pageNum: this.logPagination.pageNum,
          pageSize: this.logPagination.pageSize,
          logType: this.logSearchForm.logType,
          startTime: this.logSearchForm.timeRange[0] || '',
          endTime: this.logSearchForm.timeRange[1] || ''
        }
        const res = await getRecordPlanLog(params)
        if (res.code === 200) {
          this.planLogList = res.data.list
          this.logPagination.total = res.data.total
        }
      } catch (error) {
        this.$message.error('获取执行日志失败：' + error.message)
      } finally {
        this.logLoading = false
      }
    },

    /**
     * 重置日志搜索条件
     */
    resetLogSearch() {
      this.logSearchForm = { logType: '', timeRange: [] }
      this.logPagination.pageNum = 1
      this.fetchPlanLog()
    },

    /**
     * 日志分页大小改变
     */
    handleLogSizeChange(val) {
      this.logPagination.pageSize = val
      this.fetchPlanLog()
    },

    /**
     * 日志当前页改变
     */
    handleLogCurrentChange(val) {
      this.logPagination.pageNum = val
      this.fetchPlanLog()
    }
  }
}
</script>

<style scoped>
.zlmediakit-record-plan-detail {
  padding: 20px;
}

.page-header {
  display: flex;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
}

.mt-2 {
  margin-top: 12px;
}

.mt-3 {
  margin-top: 16px;
}

.mb-2 {
  margin-bottom: 12px;
}

.mb-3 {
  margin-bottom: 16px;
}

.ml-2 {
  margin-left: 8px;
}

.text-gray {
  color: #999;
  font-size: 12px;
}

.w-100 {
  width: 100%;
}

.form-container {
  max-width: 700px;
  padding: 10px 0;
}

.form-btn-group {
  margin-top: 20px;
}

/* 按周配置样式 */
.week-header {
  display: flex;
  align-items: center;
  font-weight: 600;
  color: #666;
}

.week-item {
  width: 80px;
  text-align: left;
}

.time-item {
  width: 140px;
  margin-left: 20px;
  text-align: left;
}

.week-row {
  display: flex;
  align-items: center;
  margin-top: 8px;
}

.week-checkbox {
  width: 80px;
}

.time-picker {
  width: 140px;
  margin-left: 20px;
}

/* 日志样式 */
.log-search-bar {
  padding-bottom: 8px;
  border-bottom: 1px solid #ebeef5;
}

.mt-4 {
  margin-top: 16px;
}

::v-deep .el-tabs__content {
  padding: 10px 0;
}
</style>