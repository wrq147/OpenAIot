<template>
  <!-- AI配置中心弹窗 -->
  <el-dialog
    title="AI项目配置中心"
    :visible.sync="visible"
    width="90%"
    height="90vh"
    append-to-body
    :close-on-click-modal="false"
    :destroy-on-close="true"
    class="ai-config-dialog"
  >
    <div class="ai-config-container">
      <!-- 页面标题 -->
      <div class="page-header">
        <p class="sub-title">选择并配置需要启用的AI功能模块</p>
      </div>

      <!-- 主内容区 -->
      <el-card shadow="hover" class="main-card">
        <div class="config-layout">
          <!-- 左侧：可选项目列表 -->
          <div class="config-column">
            <!-- 列头：标题+搜索 -->
            <div class="column-header">
              <div class="header-left">
                <i class="el-icon-collection header-icon"></i>
                <span class="header-title">可选AI项目</span>
                <el-badge :value="optionalProjects.length" class="count-badge" />
              </div>
              <el-input
                v-model="optionalSearchText"
                placeholder="搜索项目名称..."
                size="small"
                class="search-input"
                prefix-icon="el-icon-search"
                @input="handleOptionalSearch"
              />
            </div>

            <!-- 空状态 -->
            <div v-if="filteredOptionalProjects.length === 0" class="empty-state">
              <el-empty :image-size="120">
                <template slot="description">
                  <span>暂无可选项目</span><br/>
                  <span>所有项目已添加至配置列表</span>
                </template>
                <el-button
                  type="text"
                  @click="clearAllConfigured"
                  v-if="configuredProjects.length > 0"
                >
                  清空已配置列表
                </el-button>
              </el-empty>
            </div>

            <!-- 可选项目表格 -->
            <el-table
              v-else
              :data="filteredOptionalProjects"
              border
              stripe
              style="width: 100%"
              v-loading="loading"
              @selection-change="handleSelectionChange"
              :row-class-name="tableRowClassName"
              class="project-table"
            >
              <el-table-column type="selection" width="55" />
              <el-table-column
                label="项目名称"
                prop="Name"
                width="180"
                align="center"
              />
              <el-table-column
                label="项目描述"
                prop="Remark"
                show-overflow-tooltip
                min-width="200"
              >
                <template slot-scope="scope">
                  <div class="remark-text">{{ scope.row.Remark }}</div>
                </template>
              </el-table-column>
            </el-table>

            <!-- 列尾：操作按钮 -->
            <div class="column-footer">
              <el-button
                type="primary"
                icon="el-icon-plus"
                @click="addSelectedProjects"
                :disabled="selectedProjects.length === 0"
                class="action-btn"
                :loading="addLoading"
              >
                添加选中项目 ({{ selectedProjects.length }})
              </el-button>
            </div>
          </div>

          <!-- 分割线 -->
          <div class="divider">
            <i class="el-icon-arrow-right divider-icon"></i>
          </div>

          <!-- 右侧：已配置项目列表 -->
          <div class="config-column">
            <!-- 列头：标题+操作 -->
            <div class="column-header">
              <div class="header-left">
                <i class="el-icon-setting header-icon"></i>
                <span class="header-title">已配置AI项目</span>
                <el-badge :value="configuredProjects.length" class="count-badge" type="primary" />
              </div>
              <el-button
                type="text"
                icon="el-icon-delete"
                @click="batchRemoveConfigured"
                :disabled="configuredProjects.length === 0"
                class="batch-remove-btn"
              >
                批量移除
              </el-button>
            </div>

            <!-- 空状态 -->
            <div v-if="configuredProjects.length === 0" class="empty-state">
              <el-empty :image-size="120">
                <template slot="description">
                  <span>暂无已配置项目</span><br/>
                  <span>从左侧选择项目添加</span>
                </template>
              </el-empty>
            </div>

            <!-- 已配置项目表格 -->
            <el-table
              v-else
              :data="configuredProjects"
              border
              stripe
              style="width: 100%"
              v-loading="loading"
              class="project-table"
              row-key="Name"
              :default-sort="{prop: 'Name', order: 'ascending'}"
              @sort-change="handleSortChange"
            >
              <el-table-column
                label="项目名称"
                prop="Name"
                width="180"
                sortable="custom"
                align="center"
              />
              <el-table-column
                label="项目描述"
                prop="Remark"
                show-overflow-tooltip
                min-width="200"
              >
                <template slot-scope="scope">
                  <div class="remark-text">{{ scope.row.Remark }}</div>
                </template>
              </el-table-column>
              <el-table-column
                label="是否启用绘制"
                width="140"
                align="center"
              >
                <template slot-scope="scope">
                  <el-switch
                    v-model="scope.row.enableDraw"
                    active-text="是"
                    inactive-text="否"
                    @change="handleDrawSwitchChange(scope.row)"
                    class="draw-switch"
                    active-color="#67c23a"
                    inactive-color="#909399"
                  />
                </template>
              </el-table-column>
              <el-table-column
                label="操作"
                width="200"
                fixed="right"
                align="center"
              >
                <template slot-scope="scope">
                  <el-button
                    type="primary"
                    icon="el-icon-setting"
                    size="mini"
                    @click="openParamConfig(scope.row)"
                    class="config-btn"
                  >
                    参数配置
                  </el-button>
                  <el-button
                    type="danger"
                    icon="el-icon-delete"
                    size="mini"
                    @click="removeConfiguredProject(scope.row)"
                    class="remove-btn"
                  >
                    移除
                  </el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </div>
      </el-card>

      <!-- 弹窗底部按钮 -->
      <div class="dialog-bottom-actions">
        <el-button @click="handleCancel">取消</el-button>
        <el-button
          type="primary"
          @click="handleConfirm"
          :loading="confirmLoading"
        >
          确认保存配置
        </el-button>
      </div>

      <!-- 参数配置弹窗 -->
      <el-dialog
        :title="dialogTitle"
        :visible.sync="paramConfigDialogVisible"
        width="70%"
        append-to-body
        :close-on-click-modal="false"
        class="config-dialog"
      >
        <!-- 弹窗头部提示 -->
        <div v-if="currentProject" class="dialog-tips">
          <el-tag size="small" :type="currentProject.enableDraw ? 'success' : 'info'">
            <i class="el-icon-paintbrush"></i>
            当前{{ currentProject.enableDraw ? '启用' : '禁用' }}绘制
          </el-tag>
        </div>

        <!-- 参数表单 -->
        <el-form
          ref="paramForm"
          :model="paramFormData"
          label-width="140px"
          v-if="currentProject"
          style="margin-top: 20px;"
          class="param-form"
        >
          <el-form-item
            v-for="(param, index) in currentProject.ParamList"
            :key="index"
            :label="param.name"
            class="param-form-item"
          >
            <!-- 参数类型：float -->
            <el-input-number
              v-if="param.type === 'float'"
              v-model="paramFormData[param.code]"
              :min="param.min"
              :max="param.max"
              :step="0.01"
              :precision="2"
              placeholder="请输入数值"
              class="param-input"
              size="default"
            />
            
            <!-- 参数类型：boolean -->
            <el-switch
              v-else-if="param.type === 'boolean'"
              v-model="paramFormData[param.code]"
              active-text="是"
              inactive-text="否"
              active-color="#67c23a"
              inactive-color="#909399"
              class="param-switch"
            />
            
            <!-- 参数类型：enum -->
            <el-select
              v-else-if="param.type === 'enum'"
              v-model="paramFormData[param.code]"
              placeholder="请选择"
              class="param-select"
              size="default"
            >
              <el-option
                v-for="option in param.options || []"
                :key="option.value"
                :label="option.label"
                :value="option.value"
              />
            </el-select>
            
            <!-- 参数类型：string -->
            <el-input
              v-else-if="param.type === 'string'"
              v-model="paramFormData[param.code]"
              placeholder="请输入文本"
              class="param-input"
              size="default"
            />

            <!-- 帮助提示 -->
            <el-tooltip
              effect="dark"
              :content="param.help"
              placement="top"
              enterable
              class="help-tooltip"
            >
              <i class="el-icon-question-circle"></i>
            </el-tooltip>
          </el-form-item>
        </el-form>

        <!-- 弹窗底部 -->
        <div slot="footer" class="dialog-footer">
          <el-button @click="paramConfigDialogVisible = false" class="dialog-btn">取消</el-button>
          <el-button
            type="primary"
            @click="saveParamConfig"
            :loading="saveLoading"
            class="dialog-btn primary-btn"
          >
            保存配置
          </el-button>
        </div>
      </el-dialog>
    </div>
  </el-dialog>
</template>

<script>
export default {
  name: 'AIConfigDialog',
  props: {
    // 控制弹窗显示/隐藏
    visible: {
      type: Boolean,
      default: false
    },
    // 传入初始配置数据
    initConfig: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
      // 加载状态
      loading: false,
      saveLoading: false,
      addLoading: false,
      confirmLoading: false,
      // 弹窗状态
      paramConfigDialogVisible: false,
      // 当前选中项目
      currentProject: null,
      // 弹窗标题
      dialogTitle: '',
      // 参数表单数据
      paramFormData: {},
      // 选中的可选项目
      selectedProjects: [],
      // 搜索文本
      optionalSearchText: '',
      // 过滤后的可选项目
      filteredOptionalProjects: [],
      
      // 所有AI项目数据源
      allProjects: [
        {
          Name: '人脸识别',
          Remark: '人脸检测与识别是基于人工智能的生物识别技术，通过设备采集人脸图像，先检测定位人脸区域，再提取人脸特征并进行比对，实现快速确认人员身份、精准核验等功能。',
          ParamList: [
            {
              name: '人脸阈值',
              code: 'threshold',
              type: 'float',
              min: 0,
              max: 1,
              help: '0~1的区间值,值越小对人脸的检测越模糊'
            },
            {
              name: '交并阈值',
              code: 'iou_threshold',
              type: 'float',
              min: 0,
              max: 1,
              help: '0~1的区间值,值越小越不会检测重合人脸'
            },
            {
              name: '启用人脸库',
              code: 'enable_house',
              type: 'boolean',
              help: '是否匹配人脸库，并触发相应事件'
            }
          ],
          enableDraw: true
        },
        {
          Name: '车辆识别',
          Remark: '基于AI的车辆特征识别技术，可识别车牌、车型、颜色等信息，应用于交通管控、停车场管理等场景。',
          ParamList: [
            {
              name: '识别精度',
              code: 'accuracy',
              type: 'float',
              min: 0.5,
              max: 1,
              help: '识别精度阈值，值越高识别越精准'
            },
            {
              name: '启用车牌识别',
              code: 'enable_plate',
              type: 'boolean',
              help: '是否开启车牌识别功能'
            }
          ],
          enableDraw: true
        },
        {
          Name: '行为分析',
          Remark: '基于视频流的人体行为分析技术，可识别跌倒、奔跑、聚集等异常行为，适用于安防监控场景。',
          ParamList: [
            {
              name: '检测灵敏度',
              code: 'sensitivity',
              type: 'enum',
              options: [
                { label: '低', value: 'low' },
                { label: '中', value: 'medium' },
                { label: '高', value: 'high' }
              ],
              help: '行为检测灵敏度，越高越容易触发告警'
            },
            {
              name: '告警推送地址',
              code: 'alert_url',
              type: 'string',
              help: '异常行为告警的推送接口地址'
            }
          ],
          enableDraw: true
        }
      ],
      
      // 可选项目列表
      optionalProjects: [],
      // 已配置项目列表
      configuredProjects: []
    }
  },
  watch: {
    // 监听弹窗显示状态，初始化数据
    visible(val) {
      if (val) {
        this.initData()
      }
    },
    // 监听初始配置变化
    initConfig: {
      handler(val) {
        if (this.visible) {
          this.configuredProjects = JSON.parse(JSON.stringify(val))
          this.initOptionalProjects()
          this.handleOptionalSearch()
        }
      },
      deep: true
    }
  },
  created() {
    // 初始化数据
    this.initData()
  },
  methods: {
    /**
     * 初始化弹窗数据
     */
    initData() {
      // 如果有初始配置，使用初始配置
      if (this.initConfig && this.initConfig.length > 0) {
        this.configuredProjects = JSON.parse(JSON.stringify(this.initConfig))
      } else {
        this.configuredProjects = []
      }
      // 初始化可选项目列表
      this.initOptionalProjects()
      this.filteredOptionalProjects = [].concat(this.optionalProjects)
      // 重置状态
      this.selectedProjects = []
      this.optionalSearchText = ''
    },

    /**
     * 初始化可选项目列表
     */
    initOptionalProjects() {
      this.optionalProjects = this.allProjects.filter(project => {
        return !this.configuredProjects.some(cp => cp.Name === project.Name)
      })
    },

    /**
     * 处理可选项目搜索
     */
    handleOptionalSearch() {
      if (!this.optionalSearchText) {
        this.filteredOptionalProjects = [].concat(this.optionalProjects)
        return
      }
      
      // 模糊搜索项目名称
      this.filteredOptionalProjects = this.optionalProjects.filter(project => {
        return project.Name.toLowerCase().indexOf(this.optionalSearchText.toLowerCase()) > -1
      })
    },

    /**
     * 处理表格行样式
     */
    tableRowClassName(obj) {
      return obj.rowIndex % 2 === 0 ? 'row-even' : 'row-odd'
    },

    /**
     * 处理表格排序
     */
    handleSortChange(obj) {
      const prop = obj.prop
      const order = obj.order
      if (order === 'ascending') {
        this.configuredProjects.sort((a, b) => a[prop].localeCompare(b[prop]))
      } else if (order === 'descending') {
        this.configuredProjects.sort((a, b) => b[prop].localeCompare(a[prop]))
      }
    },

    /**
     * 处理可选项目选择变更
     */
    handleSelectionChange(val) {
      this.selectedProjects = val
    },

    /**
     * 添加选中的项目到已配置列表
     */
    addSelectedProjects() {
      if (this.selectedProjects.length === 0) {
        this.$message.warning('请先选择要添加的项目')
        return
      }

      try {
        this.addLoading = true
        // 防抖处理
        setTimeout(() => {
          // 添加项目并去重
          this.configuredProjects.push.apply(this.configuredProjects, this.selectedProjects)
          this.configuredProjects = this.configuredProjects.filter((item, index, self) => {
            return self.findIndex(v => v.Name === item.Name) === index
          })
          
          // 更新可选列表
          this.initOptionalProjects()
          this.handleOptionalSearch() // 保持搜索状态
          this.selectedProjects = []
          
          this.$message.success(`成功添加 ${this.selectedProjects.length} 个项目`)
          this.addLoading = false
        }, 300)
        
      } catch (error) {
        this.$message.error('添加项目失败，请重试')
        this.addLoading = false
      }
    },

    /**
     * 移除单个已配置项目
     */
    removeConfiguredProject(project) {
      this.$confirm(
        '确定要移除【' + project.Name + '】项目吗？',
        '提示',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }
      ).then(() => {
        // 移除项目
        this.configuredProjects = this.configuredProjects.filter(p => p.Name !== project.Name)
        // 更新可选列表
        this.initOptionalProjects()
        this.handleOptionalSearch()
        
        this.$message.success('已移除【' + project.Name + '】项目')
      }).catch(() => {
        this.$message.info('已取消移除操作')
      })
    },

    /**
     * 批量移除已配置项目
     */
    batchRemoveConfigured() {
      this.$confirm(
        '确定要移除所有已配置的 ' + this.configuredProjects.length + ' 个项目吗？',
        '批量移除',
        {
          type: 'warning',
          confirmButtonText: '确认移除',
          cancelButtonText: '取消'
        }
      ).then(() => {
        this.configuredProjects = []
        this.initOptionalProjects()
        this.handleOptionalSearch()
        
        this.$message.success('已清空所有已配置项目')
      }).catch(() => {
        this.$message.info('已取消批量移除操作')
      })
    },

    /**
     * 清空所有已配置项目
     */
    clearAllConfigured() {
      this.batchRemoveConfigured()
    },

    /**
     * 切换绘制状态
     */
    handleDrawSwitchChange(row) {
      this.$message({
        type: 'info',
        message: row.Name + ' 的绘制状态已' + (row.enableDraw ? '启用' : '禁用'),
        duration: 1500
      })
    },

    /**
     * 打开参数配置弹窗
     */
    openParamConfig(row) {
      this.currentProject = JSON.parse(JSON.stringify(row)) // 深拷贝防止实时联动
      this.dialogTitle = this.currentProject.Name + ' - 参数配置'
      this.paramConfigDialogVisible = true
      
      // 初始化表单数据（优先使用已保存的值，无则用默认值）
      this.paramFormData = {}
      row.ParamList.forEach(param => {
        // 如果有已保存的值则使用，否则用默认值
        if (row.paramValues && row.paramValues[param.code] !== undefined) {
          this.paramFormData[param.code] = row.paramValues[param.code]
        } else {
          switch (param.type) {
            case 'float':
              this.paramFormData[param.code] = param.min + (param.max - param.min) / 2
              break
            case 'boolean':
              this.paramFormData[param.code] = true
              break
            case 'enum':
              this.paramFormData[param.code] = param.options && param.options[0] ? param.options[0].value : ''
              break
            case 'string':
              this.paramFormData[param.code] = ''
              break
            default:
              this.paramFormData[param.code] = ''
          }
        }
      })
    },

    /**
     * 保存参数配置
     */
    saveParamConfig() {
      try {
        this.saveLoading = true
        
        // 模拟接口请求
        setTimeout(() => {
          // 保存参数值到项目对象（实现配置记忆）
          const targetProject = this.configuredProjects.find(p => p.Name === this.currentProject.Name)
          if (targetProject) {
            targetProject.paramValues = JSON.parse(JSON.stringify(this.paramFormData))
          }
          
          this.$message.success({
            message: '参数配置保存成功！',
            duration: 1500
          })
          
          this.paramConfigDialogVisible = false
          this.saveLoading = false
        }, 800)
        
      } catch (error) {
        this.$message.error('参数配置保存失败，请重试！')
        console.error('保存失败：', error)
        this.saveLoading = false
      }
    },

    /**
     * 取消操作，关闭弹窗
     */
    handleCancel() {
      this.$emit('update:visible', false)
      this.$emit('cancel')
    },

    /**
     * 确认保存配置
     */
    handleConfirm() {
      try {
        this.confirmLoading = true
        
        // 模拟保存请求
        setTimeout(() => {
          // 发送配置数据给父组件
          this.$emit('confirm', JSON.parse(JSON.stringify(this.configuredProjects)))
          this.$emit('update:visible', false)
          this.$message.success('AI配置保存成功！')
          this.confirmLoading = false
        }, 500)
        
      } catch (error) {
        this.$message.error('保存配置失败，请重试！')
        console.error('确认保存失败：', error)
        this.confirmLoading = false
      }
    }
  }
}
</script>

<style scoped>
/* 弹窗容器 */
.ai-config-dialog {
  display: flex;
  flex-direction: column;
}

/* 全局容器 */
.ai-config-container {
  padding: 10px;
  background-color: #f5f7fa;
  height: calc(90vh - 60px);
  display: flex;
  flex-direction: column;
}

/* 页面标题 */
.page-header {
  margin-bottom: 10px;
}

.page-header h2 {
  font-size: 20px;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 8px 0;
}

.page-header .sub-title {
  font-size: 14px;
  color: #666;
  margin: 0;
}

/* 主卡片 */
.main-card {
  border-radius: 12px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.05);
  overflow: hidden;
  flex: 1;
  display: flex;
  flex-direction: column;
}

/* 布局 */
.config-layout {
  display: flex;
  gap: 16px;
  padding: 20px;
  flex: 1;
  overflow: hidden;
}

.config-column {
  flex: 1;
  display: flex;
  flex-direction: column;
  background-color: #fff;
  border-radius: 8px;
  padding: 16px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  height: 100%;
}

/* 列头 */
.column-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  padding-bottom: 8px;
  border-bottom: 1px solid #f0f0f0;
}

.header-left {
  display: flex;
  align-items: center;
}

.header-icon {
  font-size: 18px;
  color: #409eff;
  margin-right: 8px;
}

.header-title {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.count-badge {
  margin-left: 8px;
}

.search-input {
  width: 200px;
}

.batch-remove-btn {
  color: #f56c6c;
}

/* 分割线 */
.divider {
  display: flex;
  align-items: center;
  justify-content: center;
  color: #ccc;
}

.divider-icon {
  font-size: 24px;
  padding: 0 8px;
}

/* 空状态 */
.empty-state {
  display: flex;
  align-items: center;
  justify-content: center;
  flex: 1;
}

/* 表格样式 */
.project-table {
  flex: 1;
  overflow: auto;
}

.remark-text {
  font-size: 13px;
  color: #666;
  line-height: 1.4;
}

/* 按钮样式 */
.column-footer {
  margin-top: 16px;
  text-align: right;
}

.action-btn {
  border-radius: 6px;
  padding: 8px 16px;
}

.config-btn {
  margin-right: 8px;
}

/* 弹窗样式 */
.config-dialog {
  border-radius: 8px;
}

.dialog-tips {
  padding: 8px 10px;
  background: #f5f7fa;
  border-radius: 4px;
}

.param-form {
  max-height: 500px;
  overflow-y: auto;
}

.param-form-item {
  margin-bottom: 16px;
}

.param-input, .param-select {
  width: 300px;
}

.help-tooltip {
  margin-left: 8px;
  color: #409eff;
  cursor: pointer;
  font-size: 16px;
}

.dialog-footer {
  text-align: right;
}

.dialog-btn {
  border-radius: 6px;
  padding: 8px 16px;
  margin-left: 8px;
}

.primary-btn {
  background-color: #409eff;
  border-color: #409eff;
}

/* 弹窗底部操作按钮 */
.dialog-bottom-actions {
  text-align: right;
  margin-top: 16px;
  padding: 10px 20px;
  background: #fff;
  border-top: 1px solid #f0f0f0;
}

/* 表格行样式 */
::v-deep .row-even {
  background-color: #f9f9f9;
}

::v-deep .row-odd {
  background-color: #fff;
}

/* 开关样式 */
.draw-switch {
  margin: 0 auto;
  display: block;
}
</style>