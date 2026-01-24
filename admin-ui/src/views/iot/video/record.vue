<template>
  <el-dialog :title="isEdit ? '编辑录像计划' : '新增录像计划'" top="2vh" :visible.sync="visible" width="980px"
    :close-on-click-modal="false" @close="handleDialogClose">
    <!-- 核心内容卡片 -->
    <el-card shadow="never" class="dialog-card">
      <el-descriptions style="margin-bottom: 15px;" :column="3" border>
        <el-descriptions-item>
          <template slot="label">
            计划ID
          </template>
          {{ recordPlanForm.id }}
        </el-descriptions-item>
        <el-descriptions-item>
          <template slot="label">
            <i class="el-icon-mobile-phone"></i>
            创建人
          </template>
          {{ recordPlanForm.creator || '暂无' }}
        </el-descriptions-item>
        <el-descriptions-item>
          <template slot="label">
            <i class="el-icon-location-outline"></i>
            创建时间
          </template>
          {{ recordPlanForm.createTime || '暂无' }}
        </el-descriptions-item>
      </el-descriptions>
      <el-tabs v-model="activeTab" type="card">
        <!-- 基础配置标签页（新增/编辑共用） -->
        <el-tab-pane label="基础配置" name="config">
          <el-form ref="recordPlanForm" :model="recordPlanForm" :rules="recordPlanRules" label-width="100px"
            class="form-container">
            <el-form-item label="计划名称" prop="planName">
              <el-input v-model="recordPlanForm.planName" placeholder="请输入计划名称"></el-input>
            </el-form-item>
            <!-- 视频源选择器 -->
            <el-form-item label="视频源" prop="streamId">
              <el-select 
                v-model="recordPlanForm.streamId" 
                filterable 
                remote 
                reserve-keyword 
                placeholder="请输入关键词搜索视频源"
                :remote-method="remoteMethod" 
                :loading="vdloading"
                clearable
              >
                <el-option 
                  v-for="item in videoSourceOptions" 
                  :key="item.value" 
                  :label="item.label" 
                  :value="item.value"
                >
                </el-option>
              </el-select>
            </el-form-item>
            <el-form-item label="保存周期" prop="saveCycle">
              <el-input-number v-model="recordPlanForm.saveCycle" :min="1" :max="365" placeholder="录像文件保存天数"
                controls-position="right"></el-input-number>
              <span class="ml-2 text-gray">超出周期的录像文件将自动删除</span>
            </el-form-item>

            <!-- 录像时段配置 -->
            <el-form-item label="录像时段" prop="recordTimeConfig">
              <el-radio-group v-model="recordPlanForm.recordTimeType" @change="handleTimeTypeChange">
                <el-radio label="week">按周配置</el-radio>
                <el-radio label="time">按时段</el-radio>
              </el-radio-group>

              <!-- 按周配置（可视化网格） -->
              <div v-if="recordPlanForm.recordTimeType === 'week'" class="week-time-config mt-3">
                <!-- 小时表头 -->
                <div class="time-header">
                  <span class="empty-cell"></span>
                  <span class="time-cell" v-for="hour in 24" :key="hour">{{ hour - 1 }}时</span>
                </div>

                <!-- 每日时段行 -->
                <div class="day-row" v-for="(day, dayIdx) in weekList" :key="dayIdx">
                  <span class="day-label">{{ day.label }}</span>
                  <div class="time-block" v-for="hour in 24" :key="hour"
                    :class="{ active: isTimeBlockActive(dayIdx, hour - 1) }" @click="toggleTimeBlock(dayIdx, hour - 1)"
                    @mouseenter="showTimeTooltip(dayIdx, hour - 1, $event)" @mouseleave="hideTimeTooltip"></div>
                </div>

                <!-- 悬浮提示框 -->
                <div v-if="tooltipVisible" class="time-tooltip"
                  :style="{ left: tooltipPosition.x + 'px', top: tooltipPosition.y + 'px' }">
                  {{ tooltipText }}
                </div>

                <!-- 快捷操作 -->
                <div class="week-actions mt-3">
                  <el-button size="mini" type="text" @click="selectAllTime">全选</el-button>
                  <el-button size="mini" type="text" @click="clearAllTime">清空</el-button>
                  <el-button size="mini" type="text" @click="selectWorkdayTime">工作日全选</el-button>
                </div>
              </div>

              <!-- 按时段配置 -->
              <div v-if="recordPlanForm.recordTimeType === 'time'" class="custom-time-config mt-3">
                <el-form-item label="录像时段" prop="customTimeRange" class="no-label">
                  <el-time-picker v-model="recordPlanForm.customStartTime" format="HH:mm:ss" value-format="HH:mm:ss"
                    placeholder="选择开始时间" class="w-100"></el-time-picker>
                  <el-time-picker v-model="recordPlanForm.customEndTime" format="HH:mm:ss" value-format="HH:mm:ss"
                    placeholder="选择结束时间" class="w-100 mt-2"></el-time-picker>
                </el-form-item>

                <div class="text-gray mt-2">
                  说明：将在每天的指定时段内执行录像
                </div>
              </div>
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <!-- 执行日志标签页（仅编辑模式显示） -->
        <el-tab-pane label="执行日志" name="log">
          <div class="log-search-bar mb-3">
            <el-form :inline="true" :model="logSearchForm" class="log-search-form">
              <el-form-item label="日志类型">
                <el-select v-model="logSearchForm.logType" placeholder="全部" clearable style="width:160px;">
                  <el-option label="计划启动" value="start"></el-option>
                  <el-option label="计划停止" value="stop"></el-option>
                  <el-option label="录像成功" value="success"></el-option>
                  <el-option label="录像失败" value="fail"></el-option>
                  <el-option label="文件清理" value="clean"></el-option>
                </el-select>
              </el-form-item>

              <el-form-item label="执行时间">
                <el-date-picker v-model="logSearchForm.timeRange" type="daterange" range-separator="至"
                  start-placeholder="开始日期" end-placeholder="结束日期" value-format="yyyy-MM-dd" clearable></el-date-picker>
              </el-form-item>

              <el-form-item>
                <el-button type="primary" icon="el-icon-search" @click="fetchPlanLog">查询</el-button>
                <el-button icon="el-icon-refresh" @click="resetLogSearch">重置</el-button>
              </el-form-item>
            </el-form>
          </div>

          <el-table v-loading="logLoading" :data="planLogList" border stripe size="small">
            <el-table-column prop="id" label="日志ID" width="80"></el-table-column>
            <el-table-column prop="logType" label="日志类型" width="100" :formatter="formatLogType"></el-table-column>
            <el-table-column prop="content" label="日志内容" min-width="300" show-overflow-tooltip></el-table-column>
            <el-table-column prop="execTime" label="执行时间" width="180"></el-table-column>
            <el-table-column prop="ip" label="操作IP" width="120"></el-table-column>
            <el-table-column prop="remark" label="备注" min-width="150" show-overflow-tooltip></el-table-column>
          </el-table>

          <el-pagination @size-change="handleLogSizeChange" @current-change="handleLogCurrentChange"
            :current-page="logPagination.pageNum" :page-sizes="[10, 20, 50]" :page-size="logPagination.pageSize"
            layout="total, sizes, prev, pager, next, jumper" :total="logPagination.total" class="mt-4" background>
          </el-pagination>
        </el-tab-pane>
      </el-tabs>
    </el-card>

    <!-- 对话框底部按钮 -->
    <div slot="footer" class="dialog-footer">
      <el-button @click="handleCancel">取消</el-button>
      <el-button type="primary" @click="handleSubmit" :loading="submitLoading">{{ isEdit ? '更新' : '提交' }}</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { recordLogList, recordInfo, addRecord, editRecord } from "@/api/rules/record";
import { videoSourceList } from "@/api/rules/video";

export default {
  data() {
    return {
      visible: false, // 弹窗显示状态
      activeTab: 'config', // 默认激活基础配置标签
      isEdit: false, // 是否为编辑模式
      submitLoading: false, // 提交按钮加载状态
      vdloading: false, // 视频源远程搜索加载状态
      videoSourceOptions: [], // 视频源下拉选项列表
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
      // 表单数据
      recordPlanForm: {
        id: '',
        planName: '', // 计划名称
        streamId: '', // 视频源ID
        saveCycle: 1, // 默认保存周期1天
        recordTimeType: 'week', // 默认按周配置
        // 按周时段配置（存储选中的时段区间：[[[start, end], ...], ...]）
        weekTimeRanges: Array(7).fill().map(() => []),
        customStartTime: '00:00:00',
        customEndTime: '23:59:59',
        status: 1,
        remark: '',
        creator: '', // 创建人
        createTime: '' // 创建时间
      },
      // 表单校验规则
      recordPlanRules: {
        planName: [{ required: true, message: '请输入计划名称', trigger: 'blur' }],
        streamId: [{ required: true, message: '请选择视频源', trigger: 'change' }],
        saveCycle: [{ required: true, message: '请输入保存周期', trigger: 'blur' }],
      },
      // 时间网格悬浮提示
      tooltipVisible: false,
      tooltipPosition: { x: 0, y: 0 },
      tooltipText: '',
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
      },
      planId: ""
    }
  },
  methods: {
    /**
     * 打开对话框方法
     * @param {Object|null} data - 编辑时传递的计划数据，新增时传null
     */
    openDlg(data) {
      this.visible = true
      if (data == null) {
        this.isEdit = false
        this.initForm()
      } else {
        this.isEdit = true
        this.planId = data.id
        this.initForm()
      }
    },

    /**
     * 初始化表单数据
     */
    async initForm() {
      // 重置表单
      this.$nextTick(() => {
        if (this.$refs.recordPlanForm) {
          this.$refs.recordPlanForm.resetFields()
        }
      })

      // 初始化按周时段配置
      this.recordPlanForm.weekTimeRanges = Array(7).fill().map(() => [])
      this.isEdit = !!this.planId
      this.recordPlanForm.id = this.planId || ''

      // 编辑模式加载详情
      if (this.isEdit) {
        await this.fetchPlanDetail(this.planId)
      } else {
        // 新增模式重置表单默认值
        this.recordPlanForm = {
          id: '',
          planName: '',
          streamId: '',
          saveCycle: 1,
          recordTimeType: 'week',
          weekTimeRanges: Array(7).fill().map(() => []),
          customStartTime: '00:00:00',
          customEndTime: '23:59:59',
          status: 1,
          remark: '',
          creator: '',
          createTime: ''
        }
        this.activeTab = 'config'
      }
    },

    /**
     * 获取录像计划详情（编辑模式）- 使用真实接口
     */
    async fetchPlanDetail(id) {
      try {
        // 调用真实的recordInfo接口
        const res = await recordInfo(id)
        if (res.code === 200) {
          this.recordPlanForm = { ...res.data }
          // 初始化日志数据
          this.fetchPlanLog()
        } else {
          this.$message.error('获取计划详情失败：' + (res.msg || '接口返回异常'))
          this.handleCancel()
        }
      } catch (error) {
        this.$message.error('获取计划详情失败：' + (error.message || '网络异常'))
        this.handleCancel()
      }
    },

    /**
     * 视频源远程搜索方法 - 使用真实接口
     */
    async remoteMethod(query) {
      if (!query) {
        this.videoSourceOptions = []
        return
      }
      this.vdloading = true
      try {
        // 调用真实的videoSourceList接口，传递搜索关键词
        const res = await videoSourceList({ keyword: query })
        if (res.code === 200) {
          // 适配接口返回格式，确保value/label字段正确
          this.videoSourceOptions = res.data.map(item => ({
            value: item.streamId || item.value, // 兼容不同的字段命名
            label: item.name || item.label || item.streamName
          }))
        } else {
          this.$message.error('获取视频源列表失败：' + (res.msg || '接口返回异常'))
          this.videoSourceOptions = []
        }
      } catch (error) {
        this.$message.error('获取视频源列表失败：' + (error.message || '网络异常'))
        this.videoSourceOptions = []
      } finally {
        this.vdloading = false
      }
    },

    /**
     * 时段类型切换
     */
    handleTimeTypeChange() {
      if (this.recordPlanForm.recordTimeType === 'week') {
        if (this.$refs.recordPlanForm) {
          this.$refs.recordPlanForm.clearValidate(['customStartTime', 'customEndTime'])
        }
      } else {
        this.recordPlanForm.weekTimeRanges = Array(7).fill().map(() => [])
        if (this.$refs.recordPlanForm) {
          this.$refs.recordPlanForm.validate(['customStartTime', 'customEndTime'])
        }
      }
    },

    /**
     * 判断时间块是否激活
     */
    isTimeBlockActive(dayIdx, hour) {
      const ranges = this.recordPlanForm.weekTimeRanges[dayIdx]
      return ranges.some(([start, end]) => hour >= start && hour < end)
    },

    /**
     * 切换时间块选中状态
     */
    toggleTimeBlock(dayIdx, hour) {
      const ranges = this.recordPlanForm.weekTimeRanges[dayIdx]
      const isActive = this.isTimeBlockActive(dayIdx, hour)

      if (isActive) {
        // 取消选中：移除包含该小时的区间
        this.recordPlanForm.weekTimeRanges[dayIdx] = ranges.filter(([start, end]) => {
          if (start <= hour && end > hour) {
            if (start < hour) {
              this.recordPlanForm.weekTimeRanges[dayIdx].push([start, hour])
            }
            if (end > hour + 1) {
              this.recordPlanForm.weekTimeRanges[dayIdx].push([hour + 1, end])
            }
            return false
          }
          return true
        })
      } else {
        // 选中：添加单小时区间
        this.recordPlanForm.weekTimeRanges[dayIdx].push([hour, hour + 1])
        // 合并相邻区间（优化显示）
        this.mergeTimeRanges(dayIdx)
      }
    },

    /**
     * 合并相邻的时间区间
     */
    mergeTimeRanges(dayIdx) {
      const ranges = this.recordPlanForm.weekTimeRanges[dayIdx]
      if (ranges.length <= 1) return

      // 按开始时间排序
      ranges.sort((a, b) => a[0] - b[0])

      const merged = [ranges[0]]
      for (let i = 1; i < ranges.length; i++) {
        const last = merged[merged.length - 1]
        const current = ranges[i]

        // 如果当前区间的开始等于上一个区间的结束，合并
        if (current[0] === last[1]) {
          last[1] = current[1]
        } else {
          merged.push(current)
        }
      }

      this.recordPlanForm.weekTimeRanges[dayIdx] = merged
    },

    /**
     * 显示时间块悬浮提示
     */
    showTimeTooltip(dayIdx, hour, event) {
      const ranges = this.recordPlanForm.weekTimeRanges[dayIdx]
      const range = ranges.find(([start, end]) => hour >= start && hour < end)

      if (range) {
        const startHour = range[0].toString().padStart(2, '0')
        const endHour = range[1].toString().padStart(2, '0')
        this.tooltipText = `${startHour}:00:00-${endHour}:00:00 周期执行`
        this.tooltipPosition = {
          x: event.clientX + 10,
          y: event.clientY + 10
        }
        this.tooltipVisible = true
      }
    },

    /**
     * 隐藏悬浮提示
     */
    hideTimeTooltip() {
      this.tooltipVisible = false
    },

    /**
     * 全选所有时间块
     */
    selectAllTime() {
      this.recordPlanForm.weekTimeRanges = Array(7).fill().map(() => [[0, 24]])
    },

    /**
     * 清空所有时间块
     */
    clearAllTime() {
      this.recordPlanForm.weekTimeRanges = Array(7).fill().map(() => [])
    },

    /**
     * 选中工作日时间块（周一到周五 0-24点）
     */
    selectWorkdayTime() {
      this.recordPlanForm.weekTimeRanges = [
        [[0, 24]], [[0, 24]], [[0, 24]], [[0, 24]], [[0, 24]], [], []
      ]
    },

    /**
     * 生成录像时段描述
     */
    generateTimeDesc(form) {
      if (form.recordTimeType === 'week') {
        const descList = []
        form.weekTimeRanges.forEach((ranges, idx) => {
          if (ranges.length > 0) {
            const timeDesc = ranges.map(([start, end]) => `${start}:00-${end}:00`).join('、')
            descList.push(`${this.weekList[idx].label} ${timeDesc}`)
          }
        })
        return descList.length > 0 ? descList.join('；') : '未选择时段'
      } else {
        return `每日 ${form.customStartTime}-${form.customEndTime}`
      }
    },

    /**
     * 提交表单（新增/编辑）- 使用真实接口
     */
    async handleSubmit() {
      try {
        await this.$refs.recordPlanForm.validate()
        this.submitLoading = true

        // 生成时段描述
        this.recordPlanForm.recordTimeDesc = this.generateTimeDesc(this.recordPlanForm)

        let res
        if (this.isEdit) {
          // 调用编辑接口
          res = await editRecord(this.recordPlanForm)
        } else {
          // 调用新增接口
          res = await addRecord(this.recordPlanForm)
        }

        if (res.code === 200) {
          this.$message.success(this.isEdit ? '更新成功！' : '新增成功！')
          this.$emit('success', this.recordPlanForm)
          this.handleCancel()
        } else {
          this.$message.error((this.isEdit ? '更新' : '新增') + '失败：' + (res.msg || '操作失败'))
        }
      } catch (error) {
        if (error !== 'cancel') {
          this.$message.error('提交失败：' + (error.message || '表单验证失败'))
        }
      } finally {
        this.submitLoading = false
      }
    },

    /**
     * 取消/关闭对话框
     */
    handleCancel() {
      this.visible = false
      this.submitLoading = false
      this.activeTab = 'config'
      this.$emit('update:visible', false)
    },

    /**
     * 对话框关闭时的处理（重置状态）
     */
    handleDialogClose() {
      this.handleCancel()
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
     * 获取当前计划的执行日志 - 使用真实接口
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

        // 调用真实的recordLogList接口
        const res = await recordLogList(params)
        if (res.code === 200) {
          this.planLogList = res.data.list || []
          this.logPagination.total = res.data.total || 0
        } else {
          this.$message.error('获取执行日志失败：' + (res.msg || '接口返回异常'))
          this.planLogList = []
          this.logPagination.total = 0
        }
      } catch (error) {
        this.$message.error('获取执行日志失败：' + (error.message || '网络异常'))
        this.planLogList = []
        this.logPagination.total = 0
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
.dialog-card {
  border: none;
  box-shadow: none;
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
  padding: 10px 0;
}

/* 隐藏重复的label */
.no-label {
  margin-left: -100px;
  padding-left: 100px;
}

/* 按周配置网格样式 */
.week-time-config {
  position: relative;
  padding: 10px;
  border: 1px solid #ebeef5;
  border-radius: 4px;
}

.time-header {
  display: flex;
  align-items: center;
  margin-bottom: 8px;
}

.empty-cell {
  width: 60px;
}

.time-cell {
  width: 40px;
  text-align: center;
  font-size: 12px;
  color: #666;
}

.day-row {
  display: flex;
  align-items: center;
  margin-bottom: 4px;
}

.day-label {
  width: 60px;
  font-size: 12px;
  color: #666;
  text-align: center;
}

.time-block {
  width: 40px;
  height: 24px;
  border: 1px solid #ebeef5;
  border-radius: 2px;
  cursor: pointer;
  transition: all 0.2s;
  background-color: #f9f9f9;
}

.time-block.active {
  background-color: #b7eb8f;
  /* 设计图中的绿色 */
  border-color: #95de64;
}

.time-block:hover {
  border-color: #409eff;
}

.time-tooltip {
  position: fixed;
  background: rgba(0, 0, 0, 0.7);
  color: #fff;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  pointer-events: none;
  z-index: 9999;
}

.week-actions {
  display: flex;
  gap: 16px;
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

::v-deep .el-dialog__body {
  padding: 0px;
}

.dialog-footer {
  text-align: right;
}
</style>