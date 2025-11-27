<template>
  <div style="height: 100%">
    <!-- 自定义样式的页签切换 -->
    <div class="tab-container">
      <div class="tab-header">
        <div class="tab-item" :class="{ active: activeTab === 'system' }"
          @click="activeTab = 'system'; handleTabChange('system')">
          <i class="el-icon-setting"></i>
          <span>系统接口</span>
        </div>
        <div class="tab-item" :class="{ active: activeTab === 'custom' }"
          @click="activeTab = 'custom'; handleTabChange('custom')">
          <i class="el-icon-menu"></i>
          <span>自定义接口</span>
        </div>

      </div>
      <div class="tab-content">
        <!-- 自定义接口内容 -->
        <div v-if="activeTab === 'custom'" class="tab-panel">
          <div class="custom-table-container">
            <el-table ref="customTable" :data="customApiList" style="width: 100%" highlight-current-row>
              <el-table-column prop="label" label="接口名称" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.label }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" width="80">
                <template slot-scope="scope">
                  <el-button type="primary" size="mini" icon="el-icon-download"
                    @click="handleCustomRowClick(scope.row)">导入</el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>


          <!-- 分页 -->
          <pagination v-show="customTotal > 0" :total="customTotal" :page.sync="customPageNum"
            :limit.sync="customPageSize" @pagination="loadCustomApiList" layout="prev, pager, next" />
        </div>

        <!-- 系统接口内容 -->
        <div v-if="activeTab === 'system'" class="tab-panel">
          <!-- 添加固定高度和滚动条 -->
          <div class="system-table-container">
            <el-table ref="systemTable" :data="systemApiList" style="width: 100%" highlight-current-row
              :row-class-name="systemTableRowClassName" :cell-style="systemTableCellStyle" :height="tableHeight">
              <el-table-column prop="label" label="接口名称" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.label }}</span>
                  <el-tag v-if="!developerInfo" type="danger" size="mini" style="margin-left: 8px;">
                    未认证
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" width="80">
                <template slot-scope="scope">
                  <el-button type="primary" size="mini" icon="el-icon-download"
                    @click="handleSystemRowChange(scope.row)">导入</el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>

          <!-- 开发者认证提示 -->
          <div v-if="!developerInfo" class="auth-tip">
            <el-alert title="开发者接口未激活" type="warning" description="请先完成开发者认证，才能使用系统接口功能。" show-icon :closable="false" />
          </div>

          <!-- 系统接口导入弹窗 -->
          <el-dialog title="系统接口参数配置" :append-to-body="true" :visible.sync="systemApiDialogVisible" width="600px"
            @close="resetDeveloperForm">
            <smart-form v-if="developerConfig" :api-config="developerConfig" @ok="confirmImportDeveloperApi"
              ref="systemApiForm" />
          </el-dialog>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { listApiSource } from "@/api/report/apisource";
import { devProfile } from "@/api/dev";
import SmartForm from "./SmartForm.vue";
import { API_PARAMS, API_LIST } from "./apiConfigs";

export default {
  components: { SmartForm },
  data() {
    return {
      activeTab: 'system', // 默认显示自定义接口页签

      // 自定义接口相关
      customApiList: [],
      customTotal: 0,
      customPageNum: 1,
      customPageSize: 5,
      currentCustomRow: null,
      dataSourceApis: [],

      // 系统接口相关
      systemApiList: API_LIST, // 系统接口列表固定
      currentSystemRow: null,
      systemApiDialogVisible: false, // 系统接口导入弹窗
      tableHeight: 550, // 表格固定高度

      developerInfo: null,
      developerConfig: null
    };
  },
  async created() {
    // 获取开发者信息
    try {
      const rsp = await devProfile();
      this.developerInfo = rsp.data;
    } catch (error) {
      this.developerInfo = null;
    }

    // 加载自定义接口列表
    this.loadCustomApiList();

    // 根据接口数量动态调整表格高度（可选）
    this.calculateTableHeight();
  },
  methods: {
    // 计算表格高度
    calculateTableHeight() {
      // 如果接口数量超过10个，设置固定高度；否则自适应
      if (this.systemApiList.length > 10) {
        this.tableHeight = 550;
      } else {
        this.tableHeight = 'auto';
      }
    },

    // 页签切换处理
    handleTabChange(tab) {
      // 清除当前选择
      this.currentCustomRow = null;
      this.currentSystemRow = null;
      this.developerConfig = null;
      this.systemApiDialogVisible = false;

      // 如果切换到自定义接口页签，确保数据已加载
      if (tab === 'custom' && this.customApiList.length === 0) {
        this.loadCustomApiList();
      }
    },

    // 加载自定义接口列表
    async loadCustomApiList() {
      const rsp = await listApiSource({
        pageNum: this.customPageNum,
        pageSize: this.customPageSize,
        ApiType: "0"
      });

      this.customApiList = rsp.data.List.map(item => ({
        ...item,
        label: item.InterfaceName,
        typename: '自定义'
      }));

      this.customTotal = rsp.data.Total;
      this.dataSourceApis = rsp.data.List;
    },

    // 自定义接口行点击
    handleCustomRowClick(row) {
      this.currentCustomRow = row;
      if (row) {
        this.importDataSource();
      }
    },

    // 系统接口行样式
    systemTableRowClassName({ row }) {
      if (!this.developerInfo) {
        return 'disabled-row';
      }
      return '';
    },

    // 系统接口单元格样式
    systemTableCellStyle({ row }) {
      if (!this.developerInfo) {
        return {
          color: '#999',
          backgroundColor: '#f9f9f9',
          position: 'relative'
        };
      }
      return {};
    },

    // 系统接口行选择变化 - 直接弹窗
    handleSystemRowChange(row) {
      if (row && !this.developerInfo) {
        // 清除当前选择
        this.$nextTick(() => {
          this.$refs.systemTable?.clearSelection();
          this.$refs.systemTable?.setCurrentRow(null);
        });
        this.currentSystemRow = null;
        this.developerConfig = null;
        this.$message.warning('请先完成开发者认证，才能使用系统接口');
        return;
      }

      this.currentSystemRow = row;
      if (row == null) {
        this.systemApiDialogVisible = false;
        return;
      }

      this.developerConfig = API_PARAMS[row.value] || null;
      // 选择后直接打开弹窗
      if (this.developerConfig && this.developerInfo) {
        this.systemApiDialogVisible = true;
      }
    },

    // 重置开发者表单
    resetDeveloperForm() {
      this.$refs.systemApiForm?.resetForm();
    },

    // 确认导入开发者接口
    confirmImportDeveloperApi(data) {
      if (!this.developerInfo) {
        this.$message.warning('开发者信息未获取，请刷新重试');
        return;
      }

      if (!this.currentSystemRow || !this.developerConfig) {
        this.$message.warning('请先选择有效的系统接口');
        return;
      }

      // 获取表单数据
      const params = data || {};

      this.$emit('import', {
        type: 'system',
        name: this.currentSystemRow.label,
        Url: this.developerConfig.url,
        Method: this.developerConfig.method,
        Header: [{ name: "token", value: this.developerInfo.SecKey }],
        ParamType: "JSON",
        ParamData: params
      });

      this.systemApiDialogVisible = false;
    },

    // 导入数据源接口（自定义接口直接导入）
    importDataSource() {
      try {
        if (!this.currentCustomRow) return;

        const api = this.dataSourceApis.find(item => item.Id === this.currentCustomRow.Id);
        if (!api) {
          this.$message.warning('未找到对应的接口配置');
          return;
        }

        this.$emit('import', {
          type: 'dataSource',
          name: this.currentCustomRow.label,
          data: {
            Url: api.Url,
            Method: api.Method,
            Header: JSON.parse(api.HeaderJson || '[]'),
            ParamType: api.ParamType,
            ParamData: JSON.parse(api.ParamJson || '{}')
          }
        });

        this.$message.success('自定义接口导入成功');

      } catch (error) {
        console.error('导入数据源接口失败:', error);
        this.$message.error('导入失败，请检查接口配置');
      }
    }
  }
};
</script>

<style scoped>
.auth-tip {
  margin-top: 15px;
}

/* 自定义页签样式 */
.tab-container {
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  margin-bottom: 15px;
}

.tab-header {
  display: flex;
  background-color: #f8f9fa;
  border-bottom: 1px solid #e9ecef;
}

.tab-item {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 14px 20px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-weight: 500;
  color: #6c757d;
  position: relative;
}

.tab-item i {
  margin-right: 8px;
  font-size: 16px;
}

.tab-item.active {
  color: #409eff;
  background-color: #fff;
}

.tab-item.active::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 3px;
  background-color: #409eff;
  border-radius: 3px 3px 0 0;
}

.tab-item:hover:not(.active) {
  color: #409eff;
  background-color: #f0f7ff;
}

.tab-content {
  padding: 20px;
  background-color: #fff;
}

.tab-panel {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.custom-table-container {
  border: 1px solid #e9ecef;
  border-radius: 4px;
  margin-bottom: 15px;
}

/* 系统接口表格容器 - 添加滚动条 */
.system-table-container {
  max-height: 620px;
  overflow-y: auto;
  border: 1px solid #e9ecef;
  border-radius: 4px;
  margin-bottom: 15px;
}

/* 自定义滚动条样式 */
::v-deep .system-table-container::-webkit-scrollbar {
  width: 6px;
}

::v-deep .system-table-container::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

::v-deep .system-table-container::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

::v-deep .system-table-container::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

/* 禁用行的样式 */
::v-deep .disabled-row {
  cursor: not-allowed !important;
  position: relative;
}

/* 禁用行的遮罩效果 */
::v-deep .disabled-row::after {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(255, 255, 255, 0.5);
  z-index: 1;
  pointer-events: none;
}

/* 禁用行的hover效果 */
::v-deep .disabled-row:hover>td {
  background-color: #f9f9f9 !important;
}

/* 禁用当前行高亮 */
::v-deep .disabled-row.current-row>td {
  background-color: #f9f9f9 !important;
  color: #999 !important;
}

/* 表格样式优化 */
::v-deep .el-table {
  --el-table-header-text-color: #333;
  --el-table-row-hover-bg-color: #f5f5f5;
  --el-table-current-row-bg-color: #e8f4fc;
}

::v-deep .el-table th {
  background-color: #f8f9fa !important;
}
</style>