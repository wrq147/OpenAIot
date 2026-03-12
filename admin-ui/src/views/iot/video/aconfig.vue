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
              <el-input-number v-model="configForm.MotionRatio" :step="0.001" :precision="3" :min="0" :max="1"
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
              <el-link type="danger" icon="el-icon-delete" @click="batchRemoveConfigured"
                :disabled="configuredProjects.length === 0">
                全部移除
              </el-link>

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
              <el-table-column label="操作" width="180" fixed="right" align="center">
                <template slot-scope="scope">
                  <el-link type="primary" icon="el-icon-setting" class="config-btn"
                    @click="openParamConfigDialog(scope.$index)"
                    :disabled="!scope.row.ParamList || scope.row.ParamList.length === 0">配置</el-link>
                  <el-link type="primary" v-if="scope.row.Stage == 'Detect'" icon="el-icon-document-checked"
                    @click="TestConfiguredProject(scope.$index)" class="config-btn">测试</el-link>
                  <el-link type="danger" class="remove-btn" icon="el-icon-delete"
                    @click="removeConfiguredProject(scope.$index)">移除</el-link>
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

    <el-dialog :title="paramConfigDialog.title" :visible.sync="paramConfigDialog.visible" width="730px" append-to-body
      :close-on-click-modal="false" :destroy-on-close="true" top="2vh">
      <div class="param-config-content">
        <el-form label-width="120px">
          <el-form-item label-width="150px" v-for="(param, index) in paramConfigDialog.currentRow.ParamList"
            :key="index" :label="param.name" class="param-form-item">
            <template slot="label">
              <!-- 帮助提示 -->
              <el-tooltip effect="dark" :content="param.help" placement="top" enterable class="help-tooltip">
                <i class="el-icon-question"></i>
              </el-tooltip>
              <span style="margin-right:10px;">{{ param.name }}</span>
            </template>
            <!-- 参数类型：float -->
            <el-input-number v-if="param.type === 'float'"
              v-model="paramConfigDialog.currentRow.paramValues[param.code]" :min="param.min" :max="param.max"
              :step="0.01" :precision="2" placeholder="请输入数值" class="param-input" size="small" />

            <!-- 参数类型：boolean -->
            <el-switch v-else-if="param.type === 'boolean'"
              v-model="paramConfigDialog.currentRow.paramValues[param.code]" active-text="是" inactive-text="否"
              active-color="#67c23a" inactive-color="#909399" class="param-switch" />

            <!-- 参数类型：enum -->
            <el-select v-else-if="param.type === 'enum'" :multiple="param.multi"
              v-model="paramConfigDialog.currentRow.paramValues[param.code]" placeholder="请选择" class="param-select"
              size="small">
              <el-option v-for="(value, label) in param.elements" :key="value" :label="label" :value="value" />
            </el-select>

            <!-- 参数类型：string -->
            <el-input v-else-if="param.type === 'string'" v-model="paramConfigDialog.currentRow.paramValues[param.code]"
              placeholder="请输入文本" class="param-input" size="small" />

            <!-- 参数类型：clip -->
            <template v-else-if="param.type === 'clip'">
              <div style="width: 380px;">
                <!-- 目标配置 -->
                <div style="margin-bottom: 8px;">
                  <el-radio-group v-model="paramConfigDialog.clipmode">
                    <el-radio label="text">文本描述</el-radio>
                    <el-radio label="image">上传图片</el-radio>
                  </el-radio-group>
                </div>

                <!-- 文本输入模式 -->
                <el-input v-if="paramConfigDialog.clipmode === 'text'" v-model="paramConfigDialog.clipText"
                  type="textarea" rows="2" placeholder="请输入目标描述（如：红色汽车,行人）多个需要用逗号分隔" size="small"
                  style="width: 100%;" />

                <!-- 图片上传模式 -->
                <el-upload v-else action="#" :show-file-list="false"
                  :before-upload="(file) => handleClipImageUpload(file)"
                  :on-error="() => this.$message.error('目标图片上传失败')" accept="image/*">
                  <img v-if="paramConfigDialog.clipImg" :src="paramConfigDialog.clipImg" alt="目标图片"
                    style="width:128px;height:128px;display: block; object-fit: cover;">
                  <i v-else class="el-icon-plus"
                    style="font-size: 28px;width:128px;height:128px;line-height: 128px;text-align: center;border: 1px dashed #d9d9d9;"></i>
                </el-upload>
              </div>
            </template>

            <!-- 参数类型：region（入侵区域配置）- 支持多区域 -->
            <template v-else-if="param.type === 'region'">
              <div style="width: 450px;">
                <!-- 区域类型选择 + 多区域操作 -->
                <div style="height: 40px; display: flex; align-items: center; justify-content: space-between;">
                  <el-radio-group v-model="paramConfigDialog.regionType" @change="changeRegionType">
                    <el-radio label="Rectangle">矩形区域</el-radio>
                    <el-radio label="Polygon">多边形区域</el-radio>
                  </el-radio-group>
                </div>

                <!-- 区域列表选择 -->
                <div style="margin-bottom: 10px;">
                  <el-select :disabled="paramConfigDialog.regions.length === 0"
                    v-model="paramConfigDialog.currentRegionIndex" @change="onRegionSelectChange" size="small"
                    empty-text="暂无区域" placeholder="选择要编辑的区域" style="width: 100%;">
                    <el-option v-for="(region, idx) in paramConfigDialog.regions" :key="idx"
                      :label="`${region.type}区域 ${idx + 1}`" :value="idx" />
                    <el-option v-if="paramConfigDialog.regions.length === 0" label="暂无区域" :value="-1" />
                  </el-select>
                </div>

                <!-- 参考图上传 -->
                <div style="margin-bottom: 10px;display: flex; align-items: center;">
                  <el-upload action="#" :show-file-list="false" :before-upload="handleRegionImageUpload"
                    accept="image/*">
                    <el-button size="small" type="default" icon="el-icon-picture">上传参考背景图</el-button>
                  </el-upload>
                  <el-button style="margin-left:10px;" size="small" type="primary" icon="el-icon-plus"
                    @click="addNewRegion">
                    添加区域
                  </el-button>
                  <el-button size="small" type="danger" icon="el-icon-delete" @click="deleteCurrentRegion"
                    :disabled="paramConfigDialog.regions.length === 0 || paramConfigDialog.currentRegionIndex === -1">
                    删除当前区域
                  </el-button>
                </div>

                <!-- 区域绘制画布 -->
                <div style="position: relative; border: 1px solid #d9d9d9; border-radius: 4px;">
                  <!-- 绘制的区域覆盖层 -->
                  <canvas v-if="paramConfigDialog.regionImage" ref="regionCanvas" @click="drawRegionPoint"
                    @dblclick="finishDrawRegion"></canvas>
                  <div v-else
                    style="width: 100%; height: 200px; display: flex; align-items: center; justify-content: center; color: #999;">
                    请上传参考背景图后绘制入侵区域
                  </div>
                </div>

                <!-- 操作提示和按钮 -->
                <div style="margin-top: 8px; display: flex; justify-content: space-between; align-items: center;">
                  <div style="font-size: 12px; color: #666;">
                    <span v-if="paramConfigDialog.isDrawing">
                      {{ paramConfigDialog.regionType === 'Rectangle' ? '点击画布左上角和右下角确定矩形区域' : '点击画布添加多边形顶点，双击完成绘制' }}
                      <span style="color: #f56c6c; margin-left: 10px;">
                        已选点：{{ paramConfigDialog.tempPoints.length }}
                        <span v-if="paramConfigDialog.regionType === 'Polygon'">（至少3个）</span>
                      </span>
                    </span>
                    <span v-else>
                      当前共 <span style="color: #409eff;">{{ paramConfigDialog.regions.length }}</span> 个区域
                      <span v-if="paramConfigDialog.currentRegionIndex !== -1">
                        | 正在编辑：区域 {{ paramConfigDialog.currentRegionIndex + 1 }}
                      </span>
                    </span>
                  </div>
                </div>

              </div>
            </template>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="paramConfigDialog.visible = false">取消</el-button>
        <el-button type="primary" @click="confirmParamConfig"
          :loading="paramConfigDialog.generateLoading">确认保存参数</el-button>
      </div>
      <div v-if="paramConfigDialog.generateLoading" class="loading-mask">
        <div class="loading-content">
          <el-icon class="loading-icon">
            <Loading />
          </el-icon>
          <p class="loading-text">{{ paramConfigDialog.generateTxt }}</p>
          <p class="loading-subtext">这个过程可能需要十几秒，请耐心等待</p>
        </div>
      </div>
    </el-dialog>

    <!-- AI项目测试弹窗 -->
    <el-dialog :title="testDialog.title" :visible.sync="testDialog.visible" width="900px" append-to-body
      :close-on-click-modal="false" :destroy-on-close="true" top="5vh">
      <div class="test-config-content">
        <div class="test-content-wrapper">
          <!-- 上传测试图片 -->
          <div class="test-image-upload">
            <div class="upload-title">
              <span>上传测试图片</span>
              <span style="color: #f56c6c; font-size: 12px;">* 支持jpg/png，≤2MB</span>
            </div>
            <el-upload action="#" :show-file-list="false" :before-upload="handleTestImageUpload"
              :on-error="handleTestImageError" accept="image/*">
              <img v-if="testDialog.testImage" :src="testDialog.testImage"
                style="width:220px;height:220px;object-fit:cover;">
              <i v-else class="el-icon-plus"
                style="font-size:36px;width:220px;height:220px;line-height:220px;text-align:center;border:1px dashed #d9d9d9;">
              </i>
            </el-upload>
          </div>

          <!-- 分割线 -->
          <div class="test-divider"></div>

          <!-- 测试结果图片 -->
          <div class="test-result-container">
            <div class="result-title">检测结果</div>
            <div :class="{ 'empty': !testDialog.testResultImage }">
              <img v-if="testDialog.testResultImage" :src="testDialog.testResultImage" class="test-result-img" />
              <div v-else class="result-empty">
                <el-empty :image-size="100" description="暂无检测结果，请上传图片并执行测试"></el-empty>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="testDialog.visible = false">关闭</el-button>
        <el-button type="primary" @click="executeTest" :disabled="!testDialog.testImage"
          :loading="testDialog.testLoading">
          开始测试
        </el-button>
      </div>
    </el-dialog>
  </el-dialog>
</template>

<script>
import CollapseText from '@/components/CollapseText/index.vue';
import { getAIProjectList, getVideoDetail, editVideoSource } from "@/api/rules/video";
import { generateFeature } from "@/api/ai/clip";
import { drawBoxs } from "@/api/ai/proj";
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
      configForm: { "MotionRatio": 0.001, "CoolDownMs": 300, "Tasks": [] },
      projectId: null,
      paramConfigDialog: {
        clipmode: 'text',
        clipText: '',
        clipImg: '',
        clipTest: '',
        visible: false,       // 弹窗显隐
        currentIndex: -1,
        currentRow: {},       // 当前操作的项目行数据
        generateLoading: false,
        generateTxt: "正在保存参数中...",
        title: 'AI项目参数配置',

        // 多区域配置相关
        regionType: 'Rectangle',      // 区域类型：Rectangle/Polygon
        regionImage: null,            // 区域绘制参考图
        regions: [],                  // 所有区域数据 [{ type, points,isdraw }]
        currentRegionIndex: -1,       // 当前编辑的区域索引
        tempPoints: [],               // 绘制中的临时点
        isDrawing: false,             // 是否正在绘制
        canvasScale: { width: 1, height: 1 } // canvas缩放比例
      },
      testDialog: {
        visible: false,
        currentRow: {},      // 当前测试的项目数据
        testImage: '',       // 测试图片base64
        testResultImage: '', // 测试结果图片base64
        testLoading: false,
        title: 'AI项目测试'
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
      } else {
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
          x["paramValues"] = x.paramValues || this.initParamValues(x["ParamList"]);
          configarr.push(x);
        }
      }
      this.configuredProjects = configarr;
      // 重置状态
      this.selectedProjects = []
      this.optionalSearchText = ''
    },

    /**
     * 初始化参数默认值
     */
    initParamValues(paramList = []) {
      const paramValues = {};
      paramList.forEach(param => {
        if (param.defval != null) {
          paramValues[param.code] = param.defval;
        }
      });
      return paramValues;
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
          newitem["paramValues"] = this.initParamValues(newitem.ParamList || []);
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
    removeConfiguredProject(idx) {
      let project = this.configuredProjects[idx];
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

    /**
     * 打开参数配置弹窗
     */
    openParamConfigDialog(idx) {
      let row = this.configuredProjects[idx];
      // 赋值当前行数据，打开弹窗
      row.paramValues = row.paramValues || {};
      this.paramConfigDialog.currentIndex = idx;
      this.paramConfigDialog.currentRow = JSON.parse(JSON.stringify(row));

      // 初始化clip参数
      const clipParam = this.paramConfigDialog.currentRow.ParamList?.find(param => param.type === 'clip');
      if (clipParam) {
        this.paramConfigDialog.clipText = this.paramConfigDialog.currentRow.paramValues[clipParam.code + "-txt"];
        this.paramConfigDialog.clipImg = this.paramConfigDialog.currentRow.paramValues[clipParam.code + "-img"];
      }

      // 初始化多区域参数
      const regionParam = this.paramConfigDialog.currentRow.ParamList?.find(param => param.type === 'region');
      if (regionParam) {
        const regionData = this.paramConfigDialog.currentRow.paramValues[regionParam.code] || [];
        // 兼容旧数据格式（单区域）
        this.paramConfigDialog.regions = Array.isArray(regionData)
          ? regionData
          : (regionData.points ? [regionData] : []);
        this.paramConfigDialog.regionType = 'Rectangle';
        this.paramConfigDialog.currentRegionIndex = this.paramConfigDialog.regions.length > 0 ? 0 : -1;
        let base64img = this.paramConfigDialog.currentRow.paramValues[regionParam.code + "-img"];
        this.initRegionImg(base64img);
        this.paramConfigDialog.tempPoints = [];
        this.paramConfigDialog.isDrawing = false;
      }

      this.paramConfigDialog.visible = true;
      this.paramConfigDialog.generateLoading = false;
      this.paramConfigDialog.title = `项目【${row.Name}】的参数配置`;
    },

    /**
     * 确认保存参数配置
     */
    async confirmParamConfig() {
      try {
        // 检查是否有clip类型参数需要生成特征向量
        const clipParam = this.paramConfigDialog.currentRow.ParamList?.find(param => param.type === 'clip');
        // 如果有clip参数且未生成特征向量，则自动生成
        if (clipParam) {
          // 前置校验
          if (this.paramConfigDialog.clipmode === "text" && !this.paramConfigDialog.clipText.trim()) {
            this.$message.warning('请输入检测目标描述');
            return;
          }
          if (this.paramConfigDialog.clipmode === "image" && !this.paramConfigDialog.clipImg) {
            this.$message.warning('请上传检测目标图片');
            return;
          }

          // 显示加载状态和提示
          this.paramConfigDialog.generateLoading = true;
          this.paramConfigDialog.generateTxt = "正在生成特征向量，请稍候...";

          let res;
          if (this.paramConfigDialog.clipmode === "text") {
            // 过滤空值
            let tarr = this.paramConfigDialog.clipText.split(',').filter(item => item.trim());
            if (tarr.length === 0) {
              this.$message.warning('检测目标描述不能为空');
              this.paramConfigDialog.generateLoading = false;
              return;
            }
            this.paramConfigDialog.currentRow.paramValues[clipParam.code + "-txt"] = tarr.join(',');
            res = await generateFeature({ StrArr: tarr });
          } else if (this.paramConfigDialog.clipmode === "image") {
            res = await generateFeature({ ImgArr: [this.paramConfigDialog.clipImg] });
            this.paramConfigDialog.currentRow.paramValues[clipParam.code + "-img"] = this.paramConfigDialog.clipImg;
          }

          // 保存生成的特征向量
          this.paramConfigDialog.currentRow.paramValues[clipParam.code] = res.data;
          this.paramConfigDialog.generateTxt = "正在保存参数中...";
        }

        // 保存多区域参数
        const regionParam = this.paramConfigDialog.currentRow.ParamList?.find(param => param.type === 'region');
        if (regionParam) {
          // 验证所有区域的有效性
          const validRegions = this.paramConfigDialog.regions.filter(region => {
            if (region.type === 'Rectangle') {
              return region.points && region.points.length >= 2;
            } else if (region.type === 'Polygon') {
              return region.points && region.points.length >= 3;
            }
            return false;
          });

          if (validRegions.length === 0 && this.paramConfigDialog.regions.length > 0) {
            this.$message.error('请确保所有区域都绘制完成且顶点数符合要求');
            this.paramConfigDialog.generateLoading = false;
            return;
          }

          // 保存多区域数据
          this.paramConfigDialog.currentRow.paramValues[regionParam.code] = validRegions;
          this.paramConfigDialog.currentRow.paramValues[regionParam.code + "-img"] = this.paramConfigDialog.regionImage.src;
        }

        this.configuredProjects[this.paramConfigDialog.currentIndex] = this.paramConfigDialog.currentRow;
        this.paramConfigDialog.visible = false;
        this.paramConfigDialog.generateLoading = false;
        this.$message.success('参数配置已保存');
      } catch (ex) {
        this.paramConfigDialog.generateLoading = false;
        this.$message.error('保存参数失败：' + (ex.message || '系统异常'));
      }
    },

    /**
     * 处理clip图片上传（转换为Base64）
     */
    handleClipImageUpload(file) {
      // 1. 校验图片格式和大小
      const isImage = file.type.startsWith('image/');
      const isLt2M = file.size / 1024 / 1024 < 2; // 限制2M以内

      if (!isImage) {
        this.$message.error('请上传图片格式文件！');
        return false;
      }
      if (!isLt2M) {
        this.$message.error('图片大小不能超过2MB！');
        return false;
      }

      // 2. 使用FileReader读取文件，转换为Base64
      const reader = new FileReader();
      reader.onload = (e) => {
        this.paramConfigDialog.clipImg = e.target.result;
        this.$forceUpdate();
      };

      // 4. 开始读取文件为DataURL
      reader.readAsDataURL(file);

      // 5. 返回false，阻止el-upload的默认接口提交行为
      return false;
    },

    /**
     * 测试图片上传错误处理
     */
    handleTestImageError() {
      this.$message.error('测试图片上传失败');
    },

    /**
     * 打开测试弹窗
     */
    TestConfiguredProject(idx) {
      let row = this.configuredProjects[idx];
      this.testDialog = {
        visible: true,
        currentRow: JSON.parse(JSON.stringify(row)),
        testImage: "",
        testResultImage: "",
        title: `测试项目【${row.Name}】`,
        testLoading: false
      };
    },

    /**
     * 测试图片上传（转base64）
     */
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

    /**
     * 执行AI测试
     */
    async executeTest() {
      try {
        this.testDialog.testLoading = true;
        this.testDialog.testResultImage = "";

        // 构造传给后端的数据
        const params = {
          Code: this.testDialog.currentRow.Code,
          paramValues: JSON.stringify(this.testDialog.currentRow.paramValues) || "",
          Base64Img: this.testDialog.testImage
        };

        const res = await drawBoxs(params);
        const base64 = res.data;

        // 自动补全 base64 图片头部
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

    /**
     * 处理区域参考图上传
     */
    handleRegionImageUpload(file) {
      const isImage = file.type.startsWith('image/');
      const isLt2M = file.size / 1024 / 1024 < 2;

      if (!isImage) {
        this.$message.error('请上传图片格式文件！');
        return false;
      }
      if (!isLt2M) {
        this.$message.error('图片大小不能超过2MB！');
        return false;
      }

      const reader = new FileReader();
      reader.onload = (e) => {
        this.initRegionImg(e.target.result);
      };
      reader.readAsDataURL(file);
      return false;
    },
    initRegionImg(imgbase64) {
      // 重置绘制状态
      this.paramConfigDialog.tempPoints = [];

      // 图片加载完成后初始化canvas
      this.paramConfigDialog.regionImage = new Image();
      this.paramConfigDialog.regionImage.src = imgbase64;
      this.paramConfigDialog.regionImage.onload = () => {
        this.$nextTick(() => {
          this.initRegionCanvas();
        });
      };
    },
    /**
     * 初始化区域绘制画布
     */
    initRegionCanvas() {
      const canvas = this.$refs.regionCanvas;
      if (!canvas) return;

      const ctxCanvas = Array.isArray(canvas) ? canvas[0] : canvas;
      const container = ctxCanvas.parentElement;

      // 获取图片原始比例
      const img = this.paramConfigDialog.regionImage;
      const imgRatio = img.width / img.height;

      let canvasWidth = container.clientWidth;
      let canvasHeight = canvasWidth / imgRatio;

      // 设置canvas实际尺寸和显示尺寸
      ctxCanvas.width = img.width;
      ctxCanvas.height = img.height;
      ctxCanvas.style.width = `${canvasWidth}px`;
      ctxCanvas.style.height = `${canvasHeight}px`;
      ctxCanvas.style.display = 'block';
      ctxCanvas.style.margin = '0 auto';

      // 计算缩放比例
      this.paramConfigDialog.canvasScale = {
        width: img.width / canvasWidth,
        height: img.height / canvasHeight
      };

      // 绘制所有区域
      this.drawRegionCanvas();
    },

    /**
     * 绘制所有区域和临时点
     */
    drawRegionCanvas() {
      const canvas = this.$refs.regionCanvas;
      if (!canvas || !this.paramConfigDialog.regionImage) return;

      const ctxCanvas = Array.isArray(canvas) ? canvas[0] : canvas;
      const ctx = ctxCanvas.getContext('2d');
      const img = this.paramConfigDialog.regionImage;

      // 清空画布
      // ctx.clearRect(0, 0, ctxCanvas.width, ctxCanvas.height);
      ctx.drawImage(img, 0, 0, ctxCanvas.width, ctxCanvas.height);


      // 绘制所有已保存的区域
      this.paramConfigDialog.regions.forEach((region, regionIdx) => {
        const points = region.points || [];
        if (points.length === 0) return;

        // 转换相对坐标为画布绝对坐标
        const absPoints = points.map(p => ({
          x: p.x * ctxCanvas.width,
          y: p.y * ctxCanvas.height
        }));

        // 设置绘制样式
        ctx.beginPath();
        ctx.strokeStyle = regionIdx === this.paramConfigDialog.currentRegionIndex ? '#ff4757' : '#2ed573';
        ctx.lineWidth = regionIdx === this.paramConfigDialog.currentRegionIndex ? 3 : 2;
        ctx.fillStyle = 'rgba(255, 71, 87, 0.1)';

        // 绘制区域
        if (region.type === 'Rectangle' && absPoints.length >= 2) {
          const p1 = absPoints[0];
          const p2 = absPoints[1];
          const x = Math.min(p1.x, p2.x);
          const y = Math.min(p1.y, p2.y);
          const width = Math.abs(p2.x - p1.x);
          const height = Math.abs(p2.y - p1.y);
          ctx.rect(x, y, width, height);
          ctx.fill();
        } else if (region.type === 'Polygon' && absPoints.length >= 3) {
          ctx.moveTo(absPoints[0].x, absPoints[0].y);
          for (let i = 1; i < absPoints.length; i++) {
            ctx.lineTo(absPoints[i].x, absPoints[i].y);
          }
          ctx.closePath();
          ctx.fill();
        }
        ctx.stroke();

        // 绘制顶点
        absPoints.forEach((p, pointIdx) => {
          ctx.fillStyle = regionIdx === this.paramConfigDialog.currentRegionIndex ? '#ff4757' : '#2ed573';
          ctx.beginPath();
          ctx.arc(p.x, p.y, 4, 0, Math.PI * 2);
          ctx.fill();
        });
      });

      // 绘制临时绘制中的点和线
      if (this.paramConfigDialog.isDrawing && this.paramConfigDialog.tempPoints.length > 0) {
        const tempAbsPoints = this.paramConfigDialog.tempPoints.map(p => ({
          x: p.x * ctxCanvas.width,
          y: p.y * ctxCanvas.height
        }));

        ctx.beginPath();
        ctx.strokeStyle = '#ff4757';
        ctx.lineWidth = 2;
        ctx.moveTo(tempAbsPoints[0].x, tempAbsPoints[0].y);

        for (let i = 1; i < tempAbsPoints.length; i++) {
          ctx.lineTo(tempAbsPoints[i].x, tempAbsPoints[i].y);
        }

        // 矩形区域实时预览
        if (this.paramConfigDialog.regionType === 'Rectangle' && tempAbsPoints.length === 1) {
          const rect = ctxCanvas.getBoundingClientRect();
          const mouseX = (this.paramConfigDialog.mouseX - rect.left) * this.paramConfigDialog.canvasScale.width;
          const mouseY = (this.paramConfigDialog.mouseY - rect.top) * this.paramConfigDialog.canvasScale.height;

          ctx.rect(
            tempAbsPoints[0].x,
            tempAbsPoints[0].y,
            mouseX - tempAbsPoints[0].x,
            mouseY - tempAbsPoints[0].y
          );
        }

        ctx.stroke();

        // 绘制临时顶点
        tempAbsPoints.forEach((p, index) => {
          ctx.fillStyle = '#ff4757';
          ctx.beginPath();
          ctx.arc(p.x, p.y, 5, 0, Math.PI * 2);
          ctx.fill();
        });
      }
    },

    /**
     * 点击画布添加区域点
     */
    drawRegionPoint(e) {
      if (!this.paramConfigDialog.regionImage || !this.paramConfigDialog.isDrawing) return;

      // 记录鼠标位置用于实时预览
      this.paramConfigDialog.mouseX = e.clientX;
      this.paramConfigDialog.mouseY = e.clientY;

      let canvas = this.$refs.regionCanvas;
      const ctxCanvas = Array.isArray(canvas) ? canvas[0] : canvas;
      const rect = ctxCanvas.getBoundingClientRect();
      const scale = this.paramConfigDialog.canvasScale;

      // 计算相对坐标（0-1）
      const x = (e.clientX - rect.left) * scale.width / ctxCanvas.width;
      const y = (e.clientY - rect.top) * scale.height / ctxCanvas.height;

      // 边界处理
      const clamp = (val, min, max) => Math.max(min, Math.min(max, val));
      const relX = clamp(x, 0, 1);
      const relY = clamp(y, 0, 1);

      // 处理矩形区域（最多2个点）
      if (this.paramConfigDialog.regionType === 'Rectangle') {
        if (this.paramConfigDialog.tempPoints.length === 0) {
          // 第一个点（左上角）
          this.paramConfigDialog.tempPoints = [{ x: relX, y: relY }];
        } else if (this.paramConfigDialog.tempPoints.length === 1) {
          // 第二个点（右下角）- 直接完成绘制
          this.paramConfigDialog.tempPoints.push({ x: relX, y: relY });
          this.finishDrawRegion();
          return;
        }
      }
      // 处理多边形区域（双击完成）
      else if (this.paramConfigDialog.regionType === 'Polygon') {
        // 添加顶点（最多20个）
        if (this.paramConfigDialog.tempPoints.length < 20) {
          this.paramConfigDialog.tempPoints.push({ x: relX, y: relY });
        } else {
          this.$message.warning('多边形顶点数量不能超过20个');
        }
      }

      // 重绘画布
      this.drawRegionCanvas();
    },

    /**
     * 完成区域绘制
     */
    finishDrawRegion() {
      if (!this.paramConfigDialog.isDrawing || this.paramConfigDialog.tempPoints.length === 0) return;

      // 验证顶点数量
      if (this.paramConfigDialog.regionType === 'Rectangle' && this.paramConfigDialog.tempPoints.length < 2) {
        this.$message.warning('矩形区域需要至少2个顶点');
        return;
      }
      if (this.paramConfigDialog.regionType === 'Polygon' && this.paramConfigDialog.tempPoints.length < 3) {
        this.$message.warning('多边形区域需要至少3个顶点');
        return;
      }

      // 创建新区域
      const newRegion = {
        type: this.paramConfigDialog.regionType,
        points: JSON.parse(JSON.stringify(this.paramConfigDialog.tempPoints)),
        isdraw: false
      };

      // 更新当前编辑的区域
      this.paramConfigDialog.regions[this.paramConfigDialog.currentRegionIndex] = newRegion;

      // 重置绘制状态
      this.paramConfigDialog.tempPoints = [];
      this.paramConfigDialog.isDrawing = false;

      // 重绘画布
      this.drawRegionCanvas();
    },

    /**
     * 切换区域类型
     */
    changeRegionType() {
      this.paramConfigDialog.tempPoints = [];
      this.paramConfigDialog.isDrawing = true;
      this.paramConfigDialog.regions[this.paramConfigDialog.currentRegionIndex] = {
        type: this.paramConfigDialog.regionType,
        points: [],
        isdraw: true
      };
      this.drawRegionCanvas();
    },
    onRegionSelectChange(index) {
      // 校验选中的索引是否有效
      if (index === -1 || !this.paramConfigDialog.regions[index]) return;
      // 获取选中的区域
      const selectedRegion = this.paramConfigDialog.regions[index];
      this.paramConfigDialog.regionType = selectedRegion.type;
      this.paramConfigDialog.isDrawing = selectedRegion.isdraw;
      this.paramConfigDialog.tempPoints = [...(selectedRegion.points || [])];
      this.drawRegionCanvas();
    },

    /**
     * 添加新区域
     */
    addNewRegion() {
      const newRegion = {
        type: this.paramConfigDialog.regionType,
        points: [],
        isdraw: true
      };
      this.paramConfigDialog.isDrawing = true;
      this.paramConfigDialog.regions.push(newRegion);
      this.paramConfigDialog.currentRegionIndex = this.paramConfigDialog.regions.length - 1;

    },

    /**
     * 删除当前选中的区域
     */
    deleteCurrentRegion() {
      if (this.paramConfigDialog.currentRegionIndex === -1 || this.paramConfigDialog.regions.length === 0) {
        this.$message.warning('请先选择要删除的区域');
        return;
      }

      this.paramConfigDialog.regions.splice(this.paramConfigDialog.currentRegionIndex, 1);

      // 更新当前选中索引
      if (this.paramConfigDialog.regions.length > 0) {
        this.paramConfigDialog.currentRegionIndex = this.paramConfigDialog.regions.length - 1;
        this.onRegionSelectChange(this.paramConfigDialog.currentRegionIndex);
      } else {
        this.paramConfigDialog.currentRegionIndex = -1;
        this.drawRegionCanvas();
      }
      this.$message.success('区域已删除');
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
  margin-right: 8px;
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

.test-content-wrapper {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 20px;
  width: 100%;
}

.test-image-upload {
  flex: 0 0 auto;
}

.upload-title {
  margin-bottom: 8px;
  font-size: 14px;
}

.test-divider {
  width: 1px;
  height: 250px;
  background-color: #ebeef5;
  flex: 0 0 auto;
}

.test-result-container {
  flex: 1;
  min-width: 0;
}

.test-result-container .empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 220px;
  border: 1px dashed #ebeef5;
  border-radius: 4px;
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

.result-empty {
  width: 100%;
  height: 100%;
}

.loading-mask {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(255, 255, 255, 0.85);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 10;
  border-radius: 4px;
}

.loading-content {
  text-align: center;
  padding: 20px;
}

.loading-icon {
  font-size: 32px;
  color: #409eff;
  margin-bottom: 12px;
  animation: el-loading-rotate 1.5s linear infinite;
}

.loading-text {
  font-size: 16px;
  color: #333;
  margin-bottom: 4px;
  font-weight: 500;
}

.loading-subtext {
  font-size: 12px;
  color: #666;
}

.region-draw-area {
  position: relative;
}

.region-point {
  position: absolute;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: #ff4757;
  transform: translate(-50%, -50%);
  z-index: 2;
}

.region-line {
  position: absolute;
  background: #ff4757;
  height: 2px;
  transform-origin: left center;
  z-index: 1;
}
</style>