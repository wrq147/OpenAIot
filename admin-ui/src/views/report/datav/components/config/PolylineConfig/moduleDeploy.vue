<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="120px" class="custom_form_item">
        <el-collapse v-model="activeNames" accordion>
          <!-- 图层配置 -->
          <el-collapse-item title="图层" name="1">
            <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
            </el-form-item>
          </el-collapse-item>

          <!-- 默认样式配置 -->
          <el-collapse-item title="默认样式" name="2">
            <!-- 动态控制点区域 -->
            <div class="points-control">
              <el-button type="primary" size="mini" icon="el-icon-plus" @click="addPoint"
                :disabled="configData.chartOption.points.length >= 10">
                添加控制点
              </el-button>
              <el-button type="text" size="mini" icon="el-icon-delete" @click="deleteLastPoint"
                :disabled="configData.chartOption.points.length <= 2" class="delete-btn">
                删除最后一个点
              </el-button>
            </div>
            <div class="tip-text">当前共 {{ configData.chartOption.points.length }} 个控制点（至少2个，最多10个）</div>

            <!-- 动态渲染每个控制点的坐标配置 -->
            <div v-for="(point, index) in configData.chartOption.points" :key="`point-${index}`" class="point-item">
              <el-divider content-position="left">第 {{ index + 1 }} 个控制点</el-divider>

              <!-- 横坐标配置 -->
              <el-form-item label="横坐标(cx)">
                <span>{{point.cx}}</span>
              </el-form-item>

              <!-- 纵坐标配置 -->
              <el-form-item label="纵坐标(cy)">
                <span>{{point.cy}}</span>
              </el-form-item>
            </div>


            <el-form-item label="是否是反转动画">
              <el-radio-group v-model="configData.chartOption.isReverseAnimation">
                <el-radio :label="false">否</el-radio>
                <el-radio :label="true">是</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.animateType !== undefined" label="动画类型">
              <el-select v-model="configData.chartOption.animateType" placeholder="请选择">
                <el-option label="电流" value="eleCurrent"></el-option>
                <el-option label="水珠" value="droplet"></el-option>
                <el-option label="轨迹" value="track"></el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.delayTime !== undefined" label="延迟时间">
              <el-slider v-model="configData.chartOption.delayTime" :min="0" :step="1" :max="100"
                show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.lineWidth !== undefined" label="线宽">
              <el-slider v-model="configData.chartOption.lineWidth" :min="0" :step="1" :max="100"
                show-input></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.animateType == 'droplet' && configData.chartOption.flowWidth !== undefined"
              label="动画宽">
              <el-slider v-model="configData.chartOption.flowWidth" :min="0" :step="1" :max="100"
                show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.animateType != 'track'" label="线缝隙间隔">
              <el-slider v-model="configData.chartOption.dasharray" :min="0" :step="1" :max="100"
                show-input></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.animateType == 'track' && configData.chartOption.radius !== undefined"
              label="轨迹半径">
              <el-slider v-model="configData.chartOption.radius" :min="0" :step="1" :max="100" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.lineColor !== undefined" label="线颜色">
              <el-color-picker v-model="configData.chartOption.lineColor" show-alpha
                style="width: 32px;"></el-color-picker>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.animateType == 'droplet' && configData.chartOption.flowColor !== undefined"
              label="动画颜色">
              <el-color-picker v-model="configData.chartOption.flowColor" show-alpha
                style="width: 32px;"></el-color-picker>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.animateType == 'track' && configData.chartOption.radiusFillColor !== undefined"
              label="轨迹颜色">
              <el-color-picker v-model="configData.chartOption.radiusFillColor" show-alpha
                style="width: 32px;"></el-color-picker>
            </el-form-item>
          </el-collapse-item>

          <!-- 动画配置 -->
          <el-collapse-item title="动画" name="6">
            <el-form-item v-if="configData.chartOption.animate !== undefined" label="载入动画">
              <el-select v-model="configData.chartOption.animate" placeholder="请选择">
                <el-option v-for="item in animateOptions" :key="item.value" :label="item.label"
                  :value="item.value"></el-option>
              </el-select>
            </el-form-item>
          </el-collapse-item>
        </el-collapse>
      </el-form>
    </el-scrollbar>
  </div>
</template>

<script>
import { animateOptions } from "../../../animate/animate";
import { getLinkChart } from "../../../util/LinkageChart";

export default {
  props: ["costomData", "drawingList"],
  data() {
    return {
      fontFamilys: this.fontFamilys,
      fontWeights: ["normal", "bold", "bolder", "lighter"],
      types: [
        { label: "月范围", value: "monthrange" },
        { label: "日期范围", value: "daterange" },
        { label: "日期时间范围", value: "datetimerange" },
      ],
      activeNames: ["1"],
      chartList: this.drawingList,
      animateOptions,
      configData: this.costomData,
      isUpdatingFromCostomData: false
    };
  },
  watch: {
    configData: {
      deep: true,
      handler(newVal) {
        if (this.isUpdatingFromCostomData) {
          this.isUpdatingFromCostomData = false;
          return;
        }
        this.$emit("costom-change", newVal);
      },
    },
    costomData: {
      deep: true,
      handler(newVal) {
        this.isUpdatingFromCostomData = true;
        this.configData = newVal;
      },
    },
  },
  mounted() {
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;
  },
  computed: {
    rangeMaxMin() {
      const width = Number(this.configData.width) || 800; // 兜底宽度
      const height = Number(this.configData.height) || 400; // 兜底高度
      return {
        x: { min: 0, max: width },
        y: { min: 0, max: height },
      };
    },
  },
  methods: {
    // 添加控制点：在数组中间新增一个点（位置默认在最后一个点右侧）
    addPoint() {
      const lastidx = this.configData.chartOption.points.length - 1;
      const lastPoint = this.configData.chartOption.points[lastidx];

      // 新增点的默认位置
      const newPoint = {
        cx: lastPoint.cx - 20,
        cy: lastPoint.cy,
      };
      // 响应式添加到数组
      this.configData.chartOption.points.splice(lastidx, 0, newPoint);
    },

    // 删除一个控制点（保留至少2个）
    deleteLastPoint() {
      if (this.configData.chartOption.points.length > 2) {
        const lastidx = this.configData.chartOption.points.length - 2;
        this.configData.chartOption.points.splice(lastidx, 1);
      }
    },


    bindCharts(val) {
      this.$set(this.configData.chartOption, "bindList", val);
    },
  },
};
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
  width: 33%;
  text-align: center;
}

.dataProduct {
  margin-bottom: 10px;
  line-height: 45px;
  padding: 0 15px;
  background-color: #f5f5f5;
  color: #666;
}

/* 控制点控制区域样式 */
.points-control {
  display: flex;
  align-items: center;
  margin: 10px 0 20px;
  gap: 16px;
}

.delete-btn {
  color: #f56c6c;
}

.tip-text {
  font-size: 12px;
  color: #666;
  margin-left: auto;
}

/* 单个控制点样式 */
.point-item {
  margin-bottom: 20px;
  padding: 10px;
  background-color: #f9f9f9;
  border-radius: 8px;
}

::v-deep .point-item .el-divider__content {
  font-size: 14px;
  font-weight: 500;
  color: #333;
}

/* 适配滑块宽度 */
::v-deep .el-slider {
  width: 100%;
  max-width: 500px;
}
</style>