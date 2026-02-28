<template>
  <!-- AI配置中心弹窗 -->
  <el-dialog v-if="visible" title="AI项目配置中心" :visible.sync="visible" width="90%" append-to-body
    :close-on-click-modal="false" :destroy-on-close="true" class="ai-config-dialog" top="2vh">
    <div class="ai-config-container" v-loading="allloading">
      <!-- 检测间隔 -->
      <div class="detection-config-card">
        <div class="config-card-header">
          <span class="config-card-title">检测间隔参数配置</span>
        </div>
        <div class="config-card-body">
          <el-form :inline="true" :model="configForm" class="detection-form">
            <el-form-item label="运动块占比阈值" prop="MotionRatio" class="form-item">
              <el-input-number v-model="configForm.MotionRatio" :step="0.01" :precision="2" :min="0" :max="1"
                placeholder="请输入0-1之间的数值" class="input-number">
                <template slot="append">%</template>
              </el-input-number>
              <el-tooltip style="margin-left:5px;" effect="dark" content="检测到的运动块占画面的比例阈值，超过该值触发AI分析" placement="top">
                <i class="el-icon-question"></i>
              </el-tooltip>
            </el-form-item>
            <el-form-item label="冷却时间" prop="CoolDownMs" class="form-item">
              <el-input-number v-model="configForm.CoolDownMs" :min="300" :max="99999" :step="100"
                placeholder="请输入100-99999之间的数值" class="input-number">
              </el-input-number>
              <span style="margin-left:5px;">毫秒</span>
              <el-tooltip style="margin-left:5px;" effect="dark" content="连续两次AI分析的最小间隔时间，避免频繁触发" placement="top">
                <i class="el-icon-question"></i>
              </el-tooltip>
            </el-form-item>
          </el-form>
        </div>
      </div>

      <!-- 主内容区 -->
      <div class="main-card">
        <div class="config-layout">
          <!-- 左侧：可选项目列表 -->
          <div class="config-column-left">
            <!-- 列头：标题+搜索 -->
            <div class="column-header">
              <div class="header-left">
                <i class="el-icon-collection header-icon"></i>
                <span class="header-title">可选AI项目</span>
                <el-badge :value="optionalProjects.length" class="count-badge" />
              </div>
              <el-input v-model="optionalSearchText" placeholder="搜索项目名称..." size="small" class="search-input"
                prefix-icon="el-icon-search" />
            </div>

            <!-- 空状态 -->
            <div v-if="optionalProjects.length === 0" class="empty-state">
              <el-empty :image-size="120">
                <template slot="description">
                  <span>暂无可选项目</span><br />
                  <span>所有项目已添加至配置列表</span>
                </template>
                <el-button type="text" @click="clearAllConfigured" v-if="configuredProjects.length > 0">
                  清空已配置列表
                </el-button>
              </el-empty>
            </div>

            <!-- 可选项目表格 -->
            <el-table v-else :data="optionalProjects" border stripe style="width: 100%" v-loading="loading"
              @selection-change="handleSelectionChange" :row-class-name="tableRowClassName" class="project-table">
              <el-table-column type="selection" width="55" />
              <el-table-column label="项目名称" prop="Name" width="120" align="center" />
              <el-table-column label="执行阶段" width="100" align="center">
                <template slot-scope="scope">
                  <span v-if="scope.row.Stage == 'Detect'">检测阶段</span>
                  <span v-else-if="scope.row.Stage == 'Infer'">推理阶段</span>
                </template>
              </el-table-column>
              <el-table-column label="项目描述" prop="Remark">
                <template slot-scope="scope">
                  <div class="remark-text">
                    <CollapseText :text="scope.row.Remark" :max-lines="3" :line-height="18" />
                  </div>
                </template>
              </el-table-column>
            </el-table>

            <!-- 列尾：操作按钮 -->
            <div class="column-footer">
              <el-button type="primary" icon="el-icon-plus" @click="addSelectedProjects"
                :disabled="selectedProjects.length === 0" class="action-btn" :loading="addLoading">
                添加选中项目 ({{ selectedProjects.length }})
              </el-button>
            </div>
          </div>

          <!-- 分割线 -->
          <div class="divider">
            <i class="el-icon-arrow-right divider-icon"></i>
          </div>

          <!-- 右侧：已配置项目列表 -->
          <div class="config-column-right">
            <!-- 原有代码保持不变 -->
            <div class="column-header">
              <div class="header-left">
                <i class="el-icon-setting header-icon"></i>
                <span class="header-title">已配置AI项目</span>
                <el-badge :value="configuredProjects.length" class="count-badge" type="primary" />
              </div>
              <el-button type="text" icon="el-icon-delete" @click="batchRemoveConfigured"
                :disabled="configuredProjects.length === 0" class="batch-remove-btn">
                批量移除
              </el-button>
            </div>

            <!-- 空状态 -->
            <div v-if="configuredProjects.length === 0" class="empty-state">
              <el-empty :image-size="120">
                <template slot="description">
                  <span>暂无已配置项目</span><br />
                  <span>从左侧选择项目添加</span>
                </template>
              </el-empty>
            </div>

            <!-- 已配置项目表格 -->
            <el-table v-else :data="configuredProjects" border stripe style="width: 100%" v-loading="loading"
              class="project-table" row-key="Code">
              <el-table-column label="项目名称" prop="Name" width="120" align="center" />
              <el-table-column label="执行阶段" width="100" align="center">
                <template slot-scope="scope">
                  <span v-if="scope.row.Stage == 'Detect'">检测阶段</span>
                  <span v-else-if="scope.row.Stage == 'Infer'">推理阶段</span>
                </template>
              </el-table-column>
              <el-table-column label="项目描述" prop="Remark" show-overflow-tooltip>
                <template slot-scope="scope">
                  <div class="remark-text">
                    <CollapseText :text="scope.row.Remark" :max-lines="3" :line-height="18" />
                  </div>
                </template>
              </el-table-column>
              <el-table-column label="操作" width="220" fixed="right" align="center">
                <template slot-scope="scope">
                  <el-link type="primary" icon="el-icon-setting" class="config-btn" @click="openParamConfigDialog(scope.row, scope.$index)"
                    :disabled="!scope.row.ParamList || scope.row.ParamList.length === 0">配置</el-link>
                  <el-link type="primary" v-if="scope.row.Stage == 'Detect'" icon="el-icon-document-checked" @click="TestConfiguredProject(scope.row)" class="config-btn">测试</el-link>
                  <el-link type="danger" class="remove-btn" icon="el-icon-delete" @click="removeConfiguredProject(scope.row)">移除</el-link>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </div>
      </div>

      <!-- 弹窗底部按钮 -->
      <div class="dialog-bottom-actions">
        <el-button @click="handleCancel">取消</el-button>
        <el-button type="primary" @click="handleConfirm" :loading="confirmLoading">
          确认保存配置
        </el-button>
      </div>
    </div>

    <el-dialog title="AI项目参数配置" :visible.sync="paramConfigDialog.visible" width="700px" append-to-body
      :close-on-click-modal="false" :destroy-on-close="true" top="10vh">


      <div class="param-config-content">
        <el-form label-width="120px" class="param-form">
          <el-form-item v-for="(param, index) in paramConfigDialog.currentRow.ParamList" :key="index"
            :label="param.name" class="param-form-item">
            <!-- 参数类型：float -->
            <el-input-number v-if="param.type === 'float'"
              v-model="paramConfigDialog.currentRow.paramValues[param.code]" :min="param.min" :max="param.max"
              :step="0.01" :precision="2" placeholder="请输入数值" class="param-input" size="small" />

            <!-- 参数类型：boolean -->
            <el-switch v-else-if="param.type === 'boolean'"
              v-model="paramConfigDialog.currentRow.paramValues[param.code]" active-text="是" inactive-text="否"
              active-color="#67c23a" inactive-color="#909399" class="param-switch" />

            <!-- 参数类型：enum -->
            <el-select v-else-if="param.type === 'enum'" v-model="paramConfigDialog.currentRow.paramValues[param.code]"
              placeholder="请选择" class="param-select" size="small">
              <el-option v-for="option in param.options || []" :key="option.value" :label="option.label"
                :value="option.value" />
            </el-select>

            <!-- 参数类型：string -->
            <el-input v-else-if="param.type === 'string'" v-model="paramConfigDialog.currentRow.paramValues[param.code]"
              placeholder="请输入文本" class="param-input" size="small" />

            <!-- 参数类型：clip -->
            <template v-else-if="param.type === 'clip'">
              <div style="width: 380px;">
                <!-- 检测目标配置 -->
                <div style="margin-bottom: 12px; padding-bottom: 12px; border-bottom: 1px dashed #ebeef5;">
                  <div style="font-size: 13px; color: #666;">检测目标<span style="color: #f56c6c;">*</span></div>
                  <div style="margin-bottom: 8px;">
                    <el-radio-group v-model="paramConfigDialog.clipmode">
                      <el-radio label="text">文本描述</el-radio>
                      <el-radio label="image">上传图片</el-radio>
                    </el-radio-group>
                  </div>

                  <!-- 文本输入模式 -->
                  <el-input v-if="paramConfigDialog.clipmode === 'text'" v-model="paramConfigDialog.clipText"
                    type="textarea" rows="2" placeholder="请输入检测目标描述（如：红色汽车、行人）" size="small" style="width: 100%;" />

                  <!-- 图片上传模式 -->
                  <el-upload v-else action="#" :show-file-list="false"
                    :before-upload="(file) => handleClipImageUpload(file, 'targetImage')"
                    :on-error="() => this.$message.error('目标图片上传失败')" accept="image/*">
                    <img v-if="paramConfigDialog.clipImg" :src="paramConfigDialog.clipImg" alt="目标图片"
                      style="width:128px;height:128px;display: block; object-fit: cover;">
                    <i v-else class="el-icon-plus"
                      style="font-size: 28px;width:128px;height:128px;line-height: 128px;text-align: center;border: 1px dashed #d9d9d9;"></i>
                  </el-upload>
                </div>


                <!-- 生成按钮 & 特征向量展示 -->
                <div style="display: flex; align-items: center;">
                  <el-button type="success" icon="el-icon-magic-stick"
                    @click="handleGenerateClipFeature(paramConfigDialog.currentRow, param.code)">
                    生成特征向量
                  </el-button>
                  <div
                    style="font-size: 12px; color: #666; flex: 1; text-align: right; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;"
                    :title="paramConfigDialog.currentRow.paramValues[param.code] || '未生成特征向量'">
                    {{ paramConfigDialog.currentRow.paramValues[param.code] ? '已生成特征向量（长度：' +
                      JSON.parse(paramConfigDialog.currentRow.paramValues[param.code]).length + '）' : '未生成特征向量' }}
                  </div>
                </div>
              </div>
            </template>

            <!-- 帮助提示 -->
            <el-tooltip effect="dark" :content="param.help" placement="top" enterable class="help-tooltip">
              <i class="el-icon-question-circle"></i>
            </el-tooltip>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="paramConfigDialog.visible = false">取消</el-button>
        <el-button type="primary" @click="confirmParamConfig">确认保存参数</el-button>
      </div>
    </el-dialog>


    <!-- AI项目测试弹窗 -->
    <el-dialog title="AI项目测试" :visible.sync="testDialog.visible" width="700px" append-to-body
      :close-on-click-modal="false" :destroy-on-close="true" top="10vh">
      <div class="test-config-content">
        <div class="test-project-info">
          <span class="info-label">测试项目：</span>
          <span class="info-value">{{ testDialog.currentRow.Name || '-' }}</span>
        </div>

        <!-- 上传测试图片 -->
        <div class="test-image-upload">
          <div class="upload-title">
            <span>上传测试图片</span>
            <span style="color: #f56c6c; font-size: 12px;">* 支持jpg/png，≤2MB</span>
          </div>
          <el-upload 
            action="#" 
            :show-file-list="false"
            :before-upload="handleTestImageUpload"
            :on-error="handleTestImageError" 
            accept="image/*">
            <img v-if="testDialog.testImage" :src="testDialog.testImage" 
                 style="width:220px;height:220px;object-fit:cover;">
            <i v-else class="el-icon-plus"
              style="font-size:36px;width:220px;height:220px;line-height:220px;text-align:center;border:1px dashed #d9d9d9;">
            </i>
          </el-upload>
        </div>

        <!-- 测试结果图片 -->
        <div v-if="testDialog.testResultImage" class="test-result-image-box">
          <div class="result-title">检测结果</div>
          <img :src="testDialog.testResultImage" class="test-result-img" />
        </div>
      </div>

      <div slot="footer" class="dialog-footer">
        <el-button @click="testDialog.visible = false">关闭</el-button>
        <el-button type="primary" @click="executeTest" 
                   :disabled="!testDialog.testImage" :loading="testDialog.testLoading">
          开始测试
        </el-button>
      </div>
    </el-dialog>

  </el-dialog>
</template>

<script>
import CollapseText from '@/components/CollapseText/index.vue';
import { getAIProjectList, getVideoDetail, editVideoSource } from "@/api/rules/video";
export default {
  name: 'AIConfigDialog',
  components: {
    CollapseText
  },
  data() {
    return {
      visible: false,
      allloading: false,
      // 加载状态
      loading: false,
      addLoading: false,
      confirmLoading: false,
      // 选中的可选项目
      selectedProjects: [],
      // 搜索文本
      optionalSearchText: '',
      // 已配置项目列表
      configuredProjects: [],
      allProjects: [],
      configForm: { "MotionRatio": 0.08, "CoolDownMs": 300, "Tasks": [] },
      projectId: null,
      paramConfigDialog: {
        clipmode: 'text',
        clipText: '',
        clipImg: '',
        clipTest: '',
        visible: false,       // 弹窗显隐
        currentIndex: -1,
        currentRow: {},     // 当前操作的项目行数据
        loading: false        // 弹窗内加载状态
      },
      testDialog: {
        visible: false,
        currentRow: {},      // 当前测试的项目数据
        testImage: '',       // 测试图片base64
        testResult: null,    // 测试结果
        testLoading: false   // 测试加载状态
      }
    }
  },
  computed: {
    optionalProjects: function () {
      let tmparr;
      if (!this.optionalSearchText) {
        tmparr = this.allProjects.filter(tmpProj => {
          return !this.configuredProjects.some(cp => cp.Code === tmpProj.Code);
        });
      }
      else {
        tmparr = this.allProjects.filter(tmpProj => {
          return !this.configuredProjects.some(cp => cp.Code === tmpProj.Code) && tmpProj.Name.toLowerCase().indexOf(this.optionalSearchText.toLowerCase()) > -1;
        });
      }

      return tmparr;
    }
  },
  methods: {
    async showDlg(id) {
      this.projectId = id;
      this.visible = true;
      this.allloading = true;
      await this.initData(id);
      this.allloading = false;
    },
    /**
     * 初始化弹窗数据
     */
    async initData(id) {
      this.configuredProjects = [];
      // 初始化可选项目列表
      let res = await getAIProjectList();
      let tmparr = [];
      for (let i = 0; i < res.data.length; i++) {
        let tmpProj = JSON.parse(res.data[i]);
        tmparr.push(tmpProj);
      }
      this.allProjects = tmparr;
      // 初始化已配置项
      let cres = await getVideoDetail({ id: id });
      let configarr = [];
      if (cres.data.AITasks != null && cres.data.AITasks != "") {
        this.configForm = JSON.parse(cres.data.AITasks);
      }
      if (this.configForm.Tasks.length > 0) {
        for (let j = 0; j < this.configForm.Tasks.length; j++) {
          let x = this.configForm.Tasks[j];
          let prj = this.allProjects.find(item => item.Code == x.Code);
          if (prj == null) {
            continue;
          }
          x["Name"] = prj.Name;
          x["Remark"] = prj.Remark;
          x["ParamList"] = prj.ParamList || [];
          // 初始化参数值
          x["paramValues"] = x.paramValues || {};
          configarr.push(x);
        }
      }

      this.configuredProjects = configarr;
      // 重置状态
      this.selectedProjects = []
      this.optionalSearchText = ''
    },

    /**
     * 处理表格行样式
     */
    tableRowClassName(obj) {
      return obj.rowIndex % 2 === 0 ? 'row-even' : 'row-odd'
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
        for (let i = 0; i < this.selectedProjects.length; i++) {
          let newitem = JSON.parse(JSON.stringify(this.selectedProjects[i]));
          // 初始化参数值
          newitem["paramValues"] = {};
          this.configuredProjects.push(newitem)
        }

        this.selectedProjects = []
        this.$message.success(`成功添加 ${this.configuredProjects.length} 个项目`)
        this.addLoading = false
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
        this.configuredProjects = this.configuredProjects.filter(p => p.Code !== project.Code)
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
     * 取消操作，关闭弹窗
     */
    handleCancel() {
      this.visible = false;
    },

    /**
     * 保存配置
     */
    async handleConfirm() {
      try {
        this.confirmLoading = true;
        // 准备提交数据（只保留必要字段）
        const submitData = this.configuredProjects.map(item => {
          const { Name, Remark, ParamList, ...rest } = item;
          return rest;
        });
        this.configForm.Tasks = submitData;
        await editVideoSource({ "Id": this.projectId, "AITasks": JSON.stringify(this.configForm) })
        this.$message.success({
          message: '参数配置保存成功！',
          duration: 1500
        })
        this.visible = false
        this.confirmLoading = false

      } catch (error) {
        this.$message.error('参数配置保存失败，请重试！')
        this.confirmLoading = false
      }
    },
    openParamConfigDialog(row, idx) {
      // 赋值当前行数据，打开弹窗
      row.paramValues = row.paramValues || {};
      console.info(row)
      this.paramConfigDialog.currentIndex = idx;
      this.paramConfigDialog.currentRow = JSON.parse(JSON.stringify(row));
      this.paramConfigDialog.visible = true;
    },
    confirmParamConfig() {
      this.configuredProjects[this.paramConfigDialog.currentIndex] = this.paramConfigDialog.currentRow;
      this.paramConfigDialog.visible = false;
      this.$message.success('参数配置已保存');
    },
    // 处理clip图片上传（转换为Base64）
    handleClipImageUpload(file, imageType) {
      // 1. 校验图片格式和大小（可选，优化体验）
      const isImage = file.type.startsWith('image/');
      const isLt2M = file.size / 1024 / 1024 < 2; // 限制2M以内

      if (!isImage) {
        this.$message.error('请上传图片格式文件！');
        return false; // 阻止上传
      }
      if (!isLt2M) {
        this.$message.error('图片大小不能超过2MB！');
        return false; // 阻止上传
      }

      // 2. 使用FileReader读取文件，转换为Base64
      const reader = new FileReader();
      // 读取完成后的回调
      reader.onload = (e) => {
        const base64Str = e.target.result; // 获取Base64编码（包含data:image/xxx;base64,前缀）

        // 3. 存储Base64数据和预览链接
        if (imageType === 'targetImage') {
          this.paramConfigDialog.clipImg = base64Str;
        } else if (imageType === 'testImage') {
          this.paramConfigDialog.clipTest = base64Str;
        }

        // 强制更新视图（避免数据更新后视图不刷新）
        this.$forceUpdate();
      };

      // 4. 开始读取文件为DataURL（即Base64格式）
      reader.readAsDataURL(file);

      // 5. 返回false，阻止el-upload的默认接口提交行为
      return false;
    },
    // 测试图片上传错误处理
    handleTestImageError() {
      this.$message.error('测试图片上传失败');
    },
    // 打开测试弹窗
    TestConfiguredProject(row) {
      this.testDialog = {
        visible: true,
        currentRow: JSON.parse(JSON.stringify(row)),
        testImage: "",
        testResultImage: "",
        testLoading: false
      };
    },

    // 测试图片上传（转base64）
    handleTestImageUpload(file) {
      const isImage = file.type.startsWith('image/');
      const isLt2M = file.size / 1024 / 1024 < 2;
      if (!isImage) {
        this.$message.error('只能上传图片！');
        return false;
      }
      if (!isLt2M) {
        this.$message.error('图片不能超过2MB！');
        return false;
      }

      const reader = new FileReader();
      reader.onload = e => {
        this.testDialog.testImage = e.target.result;
        this.testDialog.testResultImage = "";
      };
      reader.readAsDataURL(file);
      return false;
    },

    // 执行AI测试（后端返回 base64 图片）
    async executeTest() {
      try {
        this.testDialog.testLoading = true;
        this.testDialog.testResultImage = "";

        // 构造传给后端的数据
        const params = {
          projectCode: this.testDialog.currentRow.Code,
          paramValues: this.testDialog.currentRow.paramValues || {},
          imageBase64: this.testDialog.testImage, // 带头部的base64
          videoId: this.projectId
        };

        // ========== 这里调用你的真实接口 ==========
        // const res = await testAIDetect(params);
        // const base64 = res.data; 

        // 模拟后端返回 base64 图片（正式使用删掉这段）
        await new Promise(r => setTimeout(r, 1200));
        const base64 = this.testDialog.testImage; // 模拟用原图当结果图

        // 自动补全 base64 图片头部（后端如果没带，前端自动加上）
        let resultBase64 = base64;
        if (!base64.startsWith('data:image/')) {
          resultBase64 = 'data:image/jpeg;base64,' + base64;
        }

        this.testDialog.testResultImage = resultBase64;
        this.$message.success('测试完成');

      } catch (err) {
        this.$message.error('测试失败：' + (err.message || '接口异常'));
      } finally {
        this.testDialog.testLoading = false;
      }
    },

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
  background-color: #f5f7fa;
  height: calc(85vh - 60px);
  display: flex;
  flex-direction: column;
}


/* 检测参数配置 */
.detection-config-card {
  background: #fff;
  margin: 20px 20px;
  overflow: hidden;
}

.config-card-header {
  padding: 12px 16px;
  border-bottom: 1px solid #f0f0f0;
  display: flex;
  align-items: center;
}


.config-card-title {
  font-size: 15px;
  font-weight: 600;
  color: #333;
}

.config-card-body {
  padding-left: 16px;
  padding-right: 16px;
  padding-top: 16px;
}

.detection-form {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 24px;
}

/* 主卡片 */
.main-card {
  overflow: hidden;
  flex: 1;
  display: flex;
  flex-direction: column;
}

/* 布局 */
.config-layout {
  display: flex;
  gap: 16px;
  padding-left: 20px;
  padding-right: 20px;
  padding-bottom: 10px;
  flex: 1;
  overflow: hidden;
}

.config-column-left,
.config-column-right {
  display: flex;
  flex-direction: column;
  background-color: #fff;
  padding: 16px;
  height: 100%;
}

.config-column-left {
  width: 400px;
}

.config-column-right {
  flex: 1;
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
  padding: 16px 8px;
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

/* Popover参数配置样式 - 关键修复 */
::v-deep .param-popover {
  padding: 0 !important;
  border-radius: 8px !important;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15) !important;
  z-index: 9999 !important;
}

::v-deep .el-popover__reference {
  position: relative;
  z-index: 10000;
}

.param-config-content {
  width: 100%;
  padding: 0;
}

.param-form {
  padding: 16px;
}

.param-form-item {
  margin-bottom: 12px;
}

.param-input,
.param-select {
  width: 220px;
}

.param-switch {
  margin-right: 8px;
}

.help-tooltip {
  margin-left: 8px;
  color: #409eff;
  cursor: pointer;
  font-size: 14px;
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

/* 修复层级问题 */
::v-deep .el-dialog {
  z-index: 9000 !important;
}

::v-deep .el-table {
  position: relative;
  z-index: 1;
}

.test-config-content {
  padding: 10px;
}
.test-project-info {
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px dashed #ebeef5;
}
.info-label {
  font-weight: 600;
  color: #666;
}
.test-image-upload {
  margin-bottom: 20px;
}
.upload-title {
  margin-bottom: 8px;
  font-size: 14px;
}
.result-title {
  margin-bottom: 10px;
  font-size: 14px;
  font-weight: 600;
}
.test-result-img {
  max-width: 100%;
  max-height: 400px;
  border: 1px solid #eee;
  border-radius: 4px;
}
</style>