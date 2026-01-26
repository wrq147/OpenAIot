<template>
  <!-- 模板部分保持不变 -->
  <el-dialog :title="isEdit ? '编辑录像计划' : '新增录像计划'" top="2vh" :visible.sync="visible" width="980px"
    :close-on-click-modal="false" @close="handleDialogClose">
    <!-- 核心内容卡片 -->
    <el-card shadow="never" class="dialog-card">
      <el-descriptions style="margin-bottom: 15px;" :column="3" border>
        <el-descriptions-item>
          <template slot="label">计划ID</template>
          {{ recordPlanForm.Id || '暂无' }}
        </el-descriptions-item>
        <el-descriptions-item>
          <template slot="label">创建人</template>
          {{ recordPlanForm.createName || '暂无' }}
        </el-descriptions-item>
        <el-descriptions-item>
          <template slot="label">创建时间</template>
          {{ recordPlanForm.createTime || '暂无' }}
        </el-descriptions-item>
      </el-descriptions>
      <el-tabs v-model="activeTab" type="card">
        <!-- 基础配置标签页（新增/编辑共用） -->
        <el-tab-pane label="基础配置" name="config">
          <el-form ref="recordPlanForm" :model="recordPlanForm" :rules="recordPlanRules" label-width="100px"
            class="form-container">
            <el-form-item label="计划名称" prop="PlanName">
              <el-input v-model="recordPlanForm.PlanName" placeholder="请输入计划名称" style="width:350px;"></el-input>
            </el-form-item>
            <!-- 视频源选择器 -->
            <el-form-item label="视频源" prop="VideoId">
              <el-select :disabled="isEdit" v-model="recordPlanForm.VideoId" filterable remote reserve-keyword
                placeholder="请输入关键词搜索视频源" :remote-method="remoteMethod" :loading="vdloading" clearable>
                <el-option v-for="item in videoSourceOptions" :key="item.value" :label="item.label"
                  :value="item.value"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item label="保存周期" prop="SaveCycle">
              <el-input-number v-model="recordPlanForm.SaveCycle" :min="1" :max="365" placeholder="录像文件保存天数"
                controls-position="right"></el-input-number>
              <span class="ml-2 text-gray">超出周期的录像文件将自动删除</span>
            </el-form-item>

            <!-- 录像时段配置 -->
            <el-form-item label="录像时段" prop="recordTimeConfig">
              <el-radio-group v-model="recordPlanForm.RecordTimeType" @change="handleTimeTypeChange">
                <el-radio label="time">按时段</el-radio>
                <el-radio label="week">按周配置</el-radio>
              </el-radio-group>

              <!-- 按周配置（可视化网格） -->
              <div v-if="recordPlanForm.RecordTimeType === 'week'" class="week-time-config mt-3">
                <!-- 小时表头 -->
                <div class="time-header">
                  <span class="empty-cell"></span>
                  <span class="time-cell" v-for="hour in 24" :key="hour">{{ hour - 1 }}时</span>
                  <span class="clear-icon-cell"></span> <!-- 清除图标列占位 -->
                </div>
                <!-- 每日时段行 -->
                <div class="day-row" v-for="(day, dayIdx) in weekList" :key="dayIdx">
                  <span class="day-label">{{ day.label }}</span>
                  <div class="time-block" v-for="hour in 24" :key="hour" :class="{
                    active: isTimeBlockActive(dayIdx, hour - 1),
                    preview: isPreviewBlock(dayIdx, hour - 1)
                  }" @click="toggleTimeBlock(dayIdx, hour - 1)"
                    @mouseenter="handleTimeBlockMouseEnter(dayIdx, hour - 1, $event)"
                    @mouseleave="handleTimeBlockMouseLeave" @mouseover="handleTimeBlockMouseOver(dayIdx, hour - 1)">
                  </div>
                  <!-- 每日清除图标 -->
                  <span class="clear-day-icon" @click="clearDayTime(dayIdx)" title="清空当天时段配置">
                    <i class="el-icon-delete"></i>
                  </span>
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

              <!-- 按时段配置（单日24小时可视化网格，和按周交互一致） -->
              <div v-if="recordPlanForm.RecordTimeType === 'time'" class="day-time-config mt-3">
                <!-- 小时表头 -->
                <div class="time-header">
                  <span class="empty-cell"></span>
                  <span class="time-cell" v-for="hour in 24" :key="hour">{{ hour - 1 }}时</span>
                  <span class="clear-icon-cell"></span> <!-- 清除图标列占位 -->
                </div>
                <!-- 单日时段行（无星期，仅24小时） -->
                <div class="day-row single-day-row">
                  <span class="day-label">全天</span>
                  <div class="time-block" v-for="hour in 24" :key="hour" :class="{
                    active: isDayTimeBlockActive(hour - 1),
                    preview: isDayPreviewBlock(hour - 1)
                  }" @click="toggleDayTimeBlock(hour - 1)" @mouseenter="handleDayTimeBlockMouseEnter(hour - 1, $event)"
                    @mouseleave="handleTimeBlockMouseLeave" @mouseover="handleDayTimeBlockMouseOver(hour - 1)"></div>
                  <!-- 单日清除图标 -->
                  <span class="clear-day-icon" @click="clearSingleDayTime()" title="清空所有时段配置">
                    <i class="el-icon-delete"></i>
                  </span>
                </div>
                <!-- 悬浮提示框 -->
                <div v-if="tooltipVisible" class="time-tooltip"
                  :style="{ left: tooltipPosition.x + 'px', top: tooltipPosition.y + 'px' }">
                  {{ tooltipText }}
                </div>
                <!-- 快捷操作 -->
                <div class="week-actions mt-3">
                  <el-button size="mini" type="text" @click="selectAllDayTime">全选</el-button>
                  <el-button size="mini" type="text" @click="clearAllDayTime">清空</el-button>
                </div>
              </div>
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <!-- 执行日志标签页 -->
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
            <el-table-column prop="id" label="日志ID" align="center" width="150"></el-table-column>
            <el-table-column prop="LogType" label="日志类型" align="center" width="120"
              :formatter="formatLogType"></el-table-column>
            <el-table-column prop="Content" label="日志内容" min-width="300" show-overflow-tooltip></el-table-column>
            <el-table-column prop="ExecTime" label="执行时间" align="center" width="180"></el-table-column>
          </el-table>
          <pagination v-show="logPagination.total > 0" :total="logPagination.total" :page.sync="logPagination.pageNum"
            :limit.sync="logPagination.pageSize" @pagination="handleLogCurrentChange" />

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
  name: 'RecordPlanDialog',
  data() {
    return {
      visible: false,
      activeTab: 'config',
      isEdit: false,
      submitLoading: false,
      vdloading: false,
      videoSourceOptions: [],
      // 星期列表
      weekList: [
        { label: '周一', value: 1 },
        { label: '周二', value: 2 },
        { label: '周三', value: 3 },
        { label: '周四', value: 4 },
        { label: '周五', value: 5 },
        { label: '周六', value: 6 },
        { label: '周日', value: 7 }
      ],
      // 表单数据
      recordPlanForm: {
        Id: '',
        PlanName: '',
        VideoId: '',
        SaveCycle: 7,
        RecordTimeType: 'time', // week:按周 | time:按时段（单日）
        WeekConfig: "",
        TimeConfig: "",
        // 新格式：按周时间配置 [{"week":1,"Time":8,"Op":"Start"},...]
        weekTimeRanges: [],
        // 新格式：按时段配置 [{"Time":8,"Op":"Start"},{"Time":18,"Op":"End"},...]
        dayTimeRanges: [],
        Status: 1,
        createName: '',
        createTime: ''
      },
      // 表单校验规则
      recordPlanRules: {
        PlanName: [{ required: true, message: '请输入计划名称', trigger: 'blur' }],
        VideoId: [{ required: true, message: '请选择视频源', trigger: 'change' }],
        SaveCycle: [{ required: true, message: '请输入保存周期', trigger: 'blur' }]
      },
      // 时间网格悬浮提示
      tooltipVisible: false,
      tooltipPosition: { x: 0, y: 0 },
      tooltipText: '',
      // 按周范围选择状态
      isSelecting: false,
      selectStart: { dayIdx: -1, hour: -1 },
      selectPreview: { dayIdx: -1, hour: -1 },
      // 按时段（单日）范围选择状态
      isDaySelecting: false,
      daySelectStart: { hour: -1 },
      daySelectPreview: { hour: -1 },
      // 日志相关
      logLoading: false,
      logSearchForm: { logType: '', timeRange: [] },
      planLogList: [],
      logPagination: { pageNum: 1, pageSize: 10, total: 0 },
      planId: ""
    }
  },
  methods: {
    // 打开对话框
    openDlg(data) {
      this.visible = true
      if (!data) {
        this.isEdit = false
        this.initForm()
      } else {
        this.isEdit = true
        this.planId = data.Id
        this.initForm()
      }
    },
    // 初始化表单
    async initForm() {
      // 重置所有选择状态
      this.resetAllSelectState()
      // 重置表单
      this.$nextTick(() => this.$refs.recordPlanForm?.resetFields())
      // 初始化时段数据
      this.recordPlanForm.weekTimeRanges = []
      this.recordPlanForm.dayTimeRanges = []
      this.isEdit = !!this.planId
      this.recordPlanForm.Id = this.planId

      // 编辑模式加载详情
      await this.remoteMethod('');
      if (this.isEdit) {
        try {
          const res = await recordInfo(this.recordPlanForm.Id)
          const data = res.data
          // 兼容后端返回的新格式数据
          if (data.RecordTimeType === 'time') {
            data.dayTimeRanges = this.$isNotEmpty(data.TimeConfig) ? JSON.parse(data.TimeConfig) : []
            data.weekTimeRanges = [];
          } else {
            data.dayTimeRanges = [];
            data.weekTimeRanges = this.$isNotEmpty(data.WeekConfig) ? JSON.parse(data.WeekConfig) : []
          }
          this.recordPlanForm = data
          if (!this.videoSourceOptions.some(el => el.value === data.VideoId)) {
            this.videoSourceOptions.push({ value: data.VideoId, label: data.Position });
          }
          this.fetchPlanLog()
        } catch (error) {
          this.$message.error('获取计划详情失败：' + (error.message || '网络异常'))
          this.handleCancel()
        }
      } else {
        this.recordPlanForm = {
          Id: '', PlanName: '', VideoId: '', SaveCycle: 7, RecordTimeType: 'time',
          weekTimeRanges: [], dayTimeRanges: [],
          Status: 1, createName: '', createTime: ''
        }
        this.activeTab = 'config'
      }
    },
    // 重置所有范围选择状态（按周+按时段）
    resetAllSelectState() {
      // 按周
      this.isSelecting = false
      this.selectStart = { dayIdx: -1, hour: -1 }
      this.selectPreview = { dayIdx: -1, hour: -1 }
      // 按时段
      this.isDaySelecting = false
      this.daySelectStart = { hour: -1 }
      this.daySelectPreview = { hour: -1 }
      // 悬浮提示
      this.tooltipVisible = false
    },
    // 视频源远程搜索
    async remoteMethod(query) {
      this.vdloading = true
      try {
        const res = await videoSourceList({ Key: query })
        this.videoSourceOptions = res.data.List.map(item => ({ value: item.Id, label: item.Position }))
      } catch (error) {
        this.$message.error('获取视频源列表失败：' + error.message)
        this.videoSourceOptions = []
      } finally { this.vdloading = false }
    },
    // 时段类型切换
    handleTimeTypeChange() {
      this.resetAllSelectState()
    },
    // ========== 工具方法：处理新格式数据 ==========
    // 获取指定星期+小时的操作类型
    getWeekTimeOp(week, hour) {
      const item = this.recordPlanForm.weekTimeRanges.find(i => i.week === week && i.Time === hour)
      return item ? item.Op : null
    },
    // 获取指定小时的操作类型（单日）
    getDayTimeOp(hour) {
      const item = this.recordPlanForm.dayTimeRanges.find(i => i.Time === hour)
      return item ? item.Op : null
    },
    // 检查小时是否处于激活状态（按周）
    isTimeBlockActive(dayIdx, hour) {
      const week = this.weekList[dayIdx].value
      const activeHours = {}
      let currentActive = false

      // 遍历所有时间点，标记激活区间
      for (let h = 0; h < 24; h++) {
        const op = this.getWeekTimeOp(week, h)
        if (op === 'Start') {
          currentActive = true
        } else if (op === 'End') {
          currentActive = false
        }
        activeHours[h] = currentActive
      }

      // 24点的End操作会影响23点的激活状态
      const end24 = this.getWeekTimeOp(week, 24) === 'End'
      if (hour === 23 && end24) {
        return true
      }

      // 默认未激活，除非被标记为激活
      return activeHours[hour] === true
    },
    // 检查小时是否处于预览状态（按周）
    isPreviewBlock(dayIdx, hour) {
      if (!this.isSelecting) return false
      const { dayIdx: sDay, hour: sHour } = this.selectStart
      const { dayIdx: pDay, hour: pHour } = this.selectPreview

      if (dayIdx === sDay && dayIdx === pDay) {
        // 移除跨天逻辑，只保留正向范围判断
        const minHour = Math.min(sHour, pHour)
        const maxHour = Math.max(sHour, pHour)
        return hour >= minHour && hour <= maxHour
      }
      return false
    },
    // 检查小时是否处于激活状态（单日）
    isDayTimeBlockActive(hour) {
      const activeHours = {}
      let currentActive = false

      // 遍历所有时间点，标记激活区间
      for (let h = 0; h < 24; h++) {
        const op = this.getDayTimeOp(h)
        if (op === 'Start') {
          currentActive = true
        } else if (op === 'End') {
          currentActive = false
        }
        activeHours[h] = currentActive
      }

      // 24点的End操作会影响23点的激活状态
      const end24 = this.getDayTimeOp(24) === 'End'
      if (hour === 23 && end24) {
        return true
      }

      // 默认未激活，除非被标记为激活
      return activeHours[hour] === true
    },
    // 检查小时是否处于预览状态（单日）
    isDayPreviewBlock(hour) {
      if (!this.isDaySelecting) return false
      const startHour = this.daySelectStart.hour
      const previewHour = this.daySelectPreview.hour

      // 移除跨天逻辑，只保留正向范围判断
      const minHour = Math.min(startHour, previewHour)
      const maxHour = Math.max(startHour, previewHour)
      return hour >= minHour && hour <= maxHour
    },
    // ========== 新增：全时段检测工具函数 ==========
    /**
     * 检测按周配置是否为全天（0-23点）
     * @param {Number} week 星期数
     * @returns {Boolean} 是否全天
     */
    isWeekAllDay(week) {
      // 检测现有节点是否覆盖全天
      const weekNodes = this.recordPlanForm.weekTimeRanges.filter(item => item.week === week)
      if (weekNodes.length === 2 &&
        weekNodes.some(n => n.Time === 0 && n.Op === 'Start') &&
        weekNodes.some(n => n.Time === 24 && n.Op === 'End')) {
        return true
      }

      // 检测通过多个区间拼接成全天的情况
      const activeHours = new Set()
      let currentActive = false

      for (let h = 0; h < 24; h++) {
        const op = this.getWeekTimeOp(week, h)
        if (op === 'Start') {
          currentActive = true
        } else if (op === 'End') {
          currentActive = false
        }

        if (currentActive) {
          activeHours.add(h)
        }
      }

      // 检查是否覆盖0-23所有小时
      return activeHours.size === 24
    },

    /**
     * 检测单日配置是否为全天（0-23点）
     * @returns {Boolean} 是否全天
     */
    isDayAllDay() {
      // 检测现有节点是否覆盖全天
      if (this.recordPlanForm.dayTimeRanges.length === 2 &&
        this.recordPlanForm.dayTimeRanges.some(n => n.Time === 0 && n.Op === 'Start') &&
        this.recordPlanForm.dayTimeRanges.some(n => n.Time === 24 && n.Op === 'End')) {
        return true
      }

      // 检测通过多个区间拼接成全天的情况
      const activeHours = new Set()
      let currentActive = false

      for (let h = 0; h < 24; h++) {
        const op = this.getDayTimeOp(h)
        if (op === 'Start') {
          currentActive = true
        } else if (op === 'End') {
          currentActive = false
        }

        if (currentActive) {
          activeHours.add(h)
        }
      }

      // 检查是否覆盖0-23所有小时
      return activeHours.size === 24
    },

    // ========== 新增：时间节点清理优化工具函数 ==========
    /**
     * 清理按周时间节点（去重、合并、排序、全时段标准化、连续同类型节点去重）
     * @param {Number} week 星期数
     */
    cleanWeekTimeNodes(week) {
      // 1. 筛选当前星期的节点
      let weekNodes = this.recordPlanForm.weekTimeRanges.filter(item => item.week === week)

      // 2. 去重：每个时间点只保留最后一个操作
      const uniqueMap = {}
      weekNodes.forEach(node => {
        uniqueMap[node.Time] = node.Op
      })

      // 3. 重新构建节点数组并按时间排序
      weekNodes = Object.keys(uniqueMap)
        .map(time => ({ week, Time: Number(time), Op: uniqueMap[time] }))
        .sort((a, b) => a.Time - b.Time)

      // 处理连续同类型的Start/End节点
      if (weekNodes.length > 1) {
        const cleanedNodes = [weekNodes[0]] // 保留第一个节点

        for (let i = 1; i < weekNodes.length; i++) {
          const lastNode = cleanedNodes[cleanedNodes.length - 1]
          const currentNode = weekNodes[i]

          // 跳过连续的同类型Start/End节点
          if (lastNode.Op === currentNode.Op) {
            // 如果是连续的Start，保留先出现的；连续的End，保留后出现的
            if (currentNode.Op === 'End') {
              cleanedNodes.pop() // 移除前一个End
              cleanedNodes.push(currentNode) // 添加当前End
            }
            // Start节点直接跳过当前节点
            continue
          }

          cleanedNodes.push(currentNode)
        }

        weekNodes = cleanedNodes
      }

      // 全时段检测并标准化为 0 Start + 24 End
      if (this.isWeekAllDay(week)) {
        // 替换为标准的全天节点
        weekNodes = [
          { week, Time: 0, Op: 'Start' },
          { week, Time: 24, Op: 'End' }
        ]
      }

      // 4. 替换原数组中的该星期节点
      this.recordPlanForm.weekTimeRanges = [
        ...this.recordPlanForm.weekTimeRanges.filter(item => item.week !== week),
        ...weekNodes
      ]
    },

    /**
     * 清理单日时间节点（去重、合并、排序、全时段标准化、连续同类型节点去重）
     */
    cleanDayTimeNodes() {
      // 1. 去重：每个时间点只保留最后一个操作
      const uniqueMap = {}
      this.recordPlanForm.dayTimeRanges.forEach(node => {
        uniqueMap[node.Time] = node.Op
      })

      // 2. 重新构建节点数组并按时间排序
      let dayNodes = Object.keys(uniqueMap)
        .map(time => ({ Time: Number(time), Op: uniqueMap[time] }))
        .sort((a, b) => a.Time - b.Time)

      // 处理连续同类型的Start/End节点
      if (dayNodes.length > 1) {
        const cleanedNodes = [dayNodes[0]] // 保留第一个节点

        for (let i = 1; i < dayNodes.length; i++) {
          const lastNode = cleanedNodes[cleanedNodes.length - 1]
          const currentNode = dayNodes[i]

          // 跳过连续的同类型Start/End节点
          if (lastNode.Op === currentNode.Op) {
            // 如果是连续的Start，保留先出现的；连续的End，保留后出现的
            if (currentNode.Op === 'End') {
              cleanedNodes.pop() // 移除前一个End
              cleanedNodes.push(currentNode) // 添加当前End
            }
            // Start节点直接跳过当前节点
            continue
          }

          cleanedNodes.push(currentNode)
        }

        dayNodes = cleanedNodes
      }

      // 全时段检测并标准化为 0 Start + 24 End
      if (this.isDayAllDay()) {
        // 替换为标准的全天节点
        dayNodes = [
          { Time: 0, Op: 'Start' },
          { Time: 24, Op: 'End' }
        ]
      }

      this.recordPlanForm.dayTimeRanges = dayNodes
    },

    /**
     * 计算时间范围的结束点（移除跨天逻辑，只返回endHour+1）
     * @param {Number} startHour 开始小时
     * @param {Number} endHour 结束小时
     * @returns {Number} 最终结束点
     */
    calculateEndTime(startHour, endHour) {
      // 只保留正向范围的结束点计算
      return endHour + 1
    },

    // ========== 新增：重叠范围清理函数 ==========
    /**
     * 清理按周时间范围内与指定范围重叠的节点
     * @param {Number} week 星期数
     * @param {Number} startHour 开始小时
     * @param {Number} endHour 结束小时
     */
    clearOverlappingWeekTime(week, startHour, endHour) {
      // 移除跨天逻辑，只清理正向范围
      const minHour = Math.min(startHour, endHour)
      const maxHour = Math.max(startHour, endHour)

      // 过滤掉与当前范围重叠的所有节点
      this.recordPlanForm.weekTimeRanges = this.recordPlanForm.weekTimeRanges.filter(item => {
        if (item.week !== week) return true;

        const time = item.Time;
        return !(time >= minHour && time <= maxHour + 1);
      });
    },

    /**
     * 清理单日时间范围内与指定范围重叠的节点
     * @param {Number} startHour 开始小时
     * @param {Number} endHour 结束小时
     */
    clearOverlappingDayTime(startHour, endHour) {
      // 移除跨天逻辑，只清理正向范围
      const minHour = Math.min(startHour, endHour)
      const maxHour = Math.max(startHour, endHour);

      // 过滤掉与当前范围重叠的所有节点
      this.recordPlanForm.dayTimeRanges = this.recordPlanForm.dayTimeRanges.filter(item => {
        const time = item.Time;
        return !(time >= minHour && time <= maxHour + 1);
      });
    },

    // ========== 按周配置相关方法 ==========
    handleTimeBlockMouseEnter(dayIdx, hour, event) {
      if (!event?.clientX) { this.tooltipVisible = false; return }
      const week = this.weekList[dayIdx].value
      const op = this.getWeekTimeOp(week, hour)

      if (op) {
        let opText = ''
        if (op === 'Start') opText = '开始录像'
        else if (op === 'End') opText = '结束录像'

        // 特殊处理24点的提示文本
        if (hour === 24) {
          this.tooltipText = `${this.weekList[dayIdx].label} 24:00:00 ${opText}`
        } else {
          this.tooltipText = `${this.weekList[dayIdx].label} ${hour.toString().padStart(2, '0')}:00:00 ${opText}`
        }
        this.tooltipPosition = { x: event.clientX + 10, y: event.clientY + 10 }
        this.tooltipVisible = true
      } else {
        this.tooltipVisible = false
      }
    },
    handleTimeBlockMouseLeave() { this.tooltipVisible = false },
    handleTimeBlockMouseOver(dayIdx, hour) {
      if (this.isSelecting) this.selectPreview = { dayIdx, hour }
    },
    toggleTimeBlock(dayIdx, hour) {
      const week = this.weekList[dayIdx].value

      if (!this.isSelecting) {
        // 开始范围选择
        this.isSelecting = true
        this.selectStart = { dayIdx, hour }
        this.selectPreview = { dayIdx, hour }
      } else {
        // 结束范围选择并生成操作点
        this.isSelecting = false
        const startHour = this.selectStart.hour
        const endHour = hour

        // 统一修正为正向范围（小→大）
        const minHour = Math.min(startHour, endHour)
        const maxHour = Math.max(startHour, endHour)

        // 第一步：清理重叠的旧时间节点
        this.clearOverlappingWeekTime(week, minHour, maxHour)

        // 第二步：添加新的操作点
        if (minHour === maxHour) {
          // 单个小时，添加Start和End（End在当前小时+1）
          this.recordPlanForm.weekTimeRanges.push({
            week,
            Time: minHour,
            Op: 'Start'
          })
          this.recordPlanForm.weekTimeRanges.push({
            week,
            Time: minHour + 1,
            Op: 'End'
          })
        } else {
          // 计算结束点（正向范围）
          const endTime = this.calculateEndTime(minHour, maxHour)

          // 时间范围，添加开始和结束点
          this.recordPlanForm.weekTimeRanges.push({
            week,
            Time: minHour,
            Op: 'Start'
          })

          this.recordPlanForm.weekTimeRanges.push({
            week,
            Time: endTime,
            Op: 'End'
          })
        }

        // 清理优化当前星期的时间节点（包含全时段标准化和连续节点去重）
        this.cleanWeekTimeNodes(week)
        this.selectPreview = { dayIdx: -1, hour: -1 }
        this.$forceUpdate()
      }
    },
    // 清空指定日期的时段配置
    clearDayTime(dayIdx) {
      this.resetAllSelectState()
      const week = this.weekList[dayIdx].value
      // 移除该星期的所有操作点
      this.recordPlanForm.weekTimeRanges = this.recordPlanForm.weekTimeRanges.filter(
        item => item.week !== week
      )
      this.$forceUpdate()
    },
    // ========== 按时段（单日）配置核心方法 ==========
    handleDayTimeBlockMouseEnter(hour, event) {
      if (!event?.clientX) { this.tooltipVisible = false; return }
      const op = this.getDayTimeOp(hour)

      if (op) {
        let opText = ''
        if (op === 'Start') opText = '开始录像'
        else if (op === 'End') opText = '结束录像'

        // 特殊处理24点的提示文本
        if (hour === 24) {
          this.tooltipText = `每日 24:00:00 ${opText}`
        } else {
          this.tooltipText = `每日 ${hour.toString().padStart(2, '0')}:00:00 ${opText}`
        }
        this.tooltipPosition = { x: event.clientX + 10, y: event.clientY + 10 }
        this.tooltipVisible = true
      } else {
        this.tooltipVisible = false
      }
    },
    handleDayTimeBlockMouseOver(hour) {
      if (this.isDaySelecting) this.daySelectPreview = { hour }
    },
    toggleDayTimeBlock(hour) {
      if (!this.isDaySelecting) {
        // 开始单日范围选择
        this.isDaySelecting = true
        this.daySelectStart = { hour }
        this.daySelectPreview = { hour }
      } else {
        // 结束单日范围选择并生成操作点
        this.isDaySelecting = false
        const startHour = this.daySelectStart.hour
        const endHour = hour

        // 统一修正为正向范围（小→大）
        const minHour = Math.min(startHour, endHour)
        const maxHour = Math.max(startHour, endHour)

        // 第一步：清理重叠的旧时间节点
        this.clearOverlappingDayTime(minHour, maxHour)

        // 第二步：添加新的操作点
        if (minHour === maxHour) {
          // 单个小时，添加Start和End（End在当前小时+1）
          this.recordPlanForm.dayTimeRanges.push({
            Time: minHour,
            Op: 'Start'
          })
          this.recordPlanForm.dayTimeRanges.push({
            Time: minHour + 1,
            Op: 'End'
          })
        } else {
          // 计算结束点（正向范围）
          const endTime = this.calculateEndTime(minHour, maxHour)

          // 时间范围，添加开始和结束点
          this.recordPlanForm.dayTimeRanges.push({
            Time: minHour,
            Op: 'Start'
          })

          this.recordPlanForm.dayTimeRanges.push({
            Time: endTime,
            Op: 'End'
          })
        }

        // 清理优化单日时间节点（包含全时段标准化和连续节点去重）
        this.cleanDayTimeNodes()

        this.daySelectPreview = { hour: -1 }
        this.$forceUpdate()
      }
    },
    // 清空单日的所有时段配置
    clearSingleDayTime() {
      this.resetAllSelectState()
      this.recordPlanForm.dayTimeRanges = []
      this.$forceUpdate()
    },
    // ========== 按周快捷操作 ==========
    selectAllTime() {
      this.resetAllSelectState()
      // 直接使用标准的全天节点格式
      this.recordPlanForm.weekTimeRanges = []
      // 为每一天添加标准的全天节点
      this.weekList.forEach(day => {
        this.recordPlanForm.weekTimeRanges.push(
          { week: day.value, Time: 0, Op: 'Start' },
          { week: day.value, Time: 24, Op: 'End' }
        )
        // 清理优化
        this.cleanWeekTimeNodes(day.value)
      })
    },
    clearAllTime() {
      this.resetAllSelectState()
      this.recordPlanForm.weekTimeRanges = []
    },
    selectWorkdayTime() {
      this.resetAllSelectState()
      // 直接使用标准的全天节点格式
      this.recordPlanForm.weekTimeRanges = []
      // 工作日（周一到周五）添加标准的全天节点
      for (let i = 0; i < 5; i++) {
        const week = this.weekList[i].value
        this.recordPlanForm.weekTimeRanges.push(
          { week, Time: 0, Op: 'Start' },
          { week, Time: 24, Op: 'End' }
        )
        // 清理优化
        this.cleanWeekTimeNodes(week)
      }
    },
    // ========== 按时段（单日）快捷操作 ==========
    selectAllDayTime() {
      this.resetAllSelectState()
      // 直接使用标准的全天节点格式
      this.recordPlanForm.dayTimeRanges = [
        { Time: 0, Op: 'Start' },
        { Time: 24, Op: 'End' }
      ]
      // 清理优化
      this.cleanDayTimeNodes()
    },
    clearAllDayTime() {
      this.resetAllSelectState()
      this.recordPlanForm.dayTimeRanges = []
    },

    // 提交表单
    async handleSubmit() {
      try {
        // 先校验表单
        await this.$refs.recordPlanForm.validate()
        // 校验时段是否选择
        const hasSelectTime = this.recordPlanForm.recordTimeType === 'week'
          ? this.recordPlanForm.weekTimeRanges.length > 0
          : this.recordPlanForm.dayTimeRanges.length > 0
        if (!hasSelectTime) {
          this.$message.error('请选择录像时段');
          return
        }

        this.submitLoading = true
        // 准备提交数据（转JSON字符串，适配后端）
        const submitData = {
          ...this.recordPlanForm,
          WeekConfig: JSON.stringify(this.recordPlanForm.weekTimeRanges),
          TimeConfig: JSON.stringify(this.recordPlanForm.dayTimeRanges)
        }

        // 调用接口
        const res = this.isEdit ? await editRecord(submitData) : await addRecord(submitData)
        this.$message.success(this.isEdit ? '更新成功！' : '新增成功！')
        this.$emit('success', this.recordPlanForm)
        this.handleCancel()
      } catch (error) {
        this.$message.error('提交失败：' + (error.message || '表单验证失败'))
      } finally {
        this.submitLoading = false
      }
    },
    // 取消/关闭对话框
    handleCancel() {
      this.resetAllSelectState()
      this.visible = false
      this.submitLoading = false
      this.activeTab = 'config'
      this.$emit('update:visible', false)
    },
    handleDialogClose() { this.handleCancel() },
    // ========== 日志相关方法 ==========
    formatLogType(row) {
      const logTypeMap = { start: '计划启动', stop: '计划停止', success: '录像成功', fail: '录像失败', clean: '文件清理' }
      return logTypeMap[row.logType] || '未知'
    },
    async fetchPlanLog() {
      this.logLoading = true
      try {
        const params = {
          planId: this.recordPlanForm.Id,
          pageNum: this.logPagination.pageNum,
          pageSize: this.logPagination.pageSize,
          logType: this.logSearchForm.logType,
          startTime: this.logSearchForm.timeRange[0] || '',
          endTime: this.logSearchForm.timeRange[1] || ''
        }
        const res = await recordLogList(params)
        if (res.code === 200) {
          this.planLogList = res.data.list || []
          this.logPagination.total = res.data.total || 0
        } else {
          this.planLogList = []
          this.logPagination.total = 0
        }
      } catch (error) {
        this.$message.error('获取执行日志失败：' + error.message)
        this.planLogList = []
        this.logPagination.total = 0
      } finally { this.logLoading = false }
    },
    resetLogSearch() {
      this.logSearchForm = { logType: '', timeRange: [] }
      this.logPagination.pageNum = 1
      this.fetchPlanLog()
    },
    handleLogCurrentChange() { this.fetchPlanLog() }
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

/* 按周/单日网格通用样式 */
.week-time-config,
.day-time-config {
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

.clear-icon-cell {
  width: 40px;
}

/* 清除图标列宽度 */
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
  border-color: #95de64;
}

.time-block.preview {
  background-color: #91d5ff;
  border-color: #409eff;
  opacity: 0.7;
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

/* 清除图标样式 */
.clear-day-icon {
  width: 40px;
  text-align: center;
  color: #999;
  cursor: pointer;
  font-size: 14px;
  transition: color 0.2s;
}

.clear-day-icon:hover {
  color: #f56c6c;
}

/* 单日网格专属样式（微调） */
.single-day-row {
  align-items: center;
}

/* 日志样式 */
.log-search-bar {
  padding-bottom: 8px;
  border-bottom: 1px solid #ebeef5;
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