<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="基准" name="2">
          <el-form-item v-if="configData.chartOption.opts.lineCount!==undefined" label="线条数量">
            <el-slider v-model="configData.chartOption.opts.lineCount" :min="1" :step="1" :max="300" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.opts.radVel!==undefined" label="旋转速度">
            <el-slider v-model="configData.chartOption.opts.radVel" :min="0" :step="0.001" :max="0.3" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.opts.lineBaseVel!==undefined" label="线条基准速度">
            <el-slider v-model="configData.chartOption.opts.lineBaseVel" :min="0" :step="0.1" :max="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.opts.lineBaseLife!==undefined" label="线条基准周期">
            <el-slider v-model="configData.chartOption.opts.lineBaseLife" :min="0" :step="0.1" :max="1" show-input></el-slider>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="绘图" name="3">
          <el-form-item v-if="configData.chartOption.opts.ellipseTilt!==undefined" label="旋转角度">
            <el-slider v-model="configData.chartOption.opts.ellipseTilt" :min="-3" :step="0.1" :max="3" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.opts.ellipseBaseRadius!==undefined" label="椭圆最小半径">
            <el-slider v-model="configData.chartOption.opts.ellipseBaseRadius" :min="0" :step="0.01" :max="0.5" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.opts.ellipseAddedRadius!==undefined" label="线条半径间隔">
            <el-slider v-model="configData.chartOption.opts.ellipseAddedRadius" :min="0" :step="0.001" :max="0.5" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.opts.ellipseAxisMultiplierX!==undefined" label="椭圆X轴角度">
            <el-slider v-model="configData.chartOption.opts.ellipseAxisMultiplierX" :min="0" :step="0.1" :max="3" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.opts.ellipseAxisMultiplierY!==undefined" label="椭圆Y轴角度">
            <el-slider v-model="configData.chartOption.opts.ellipseAxisMultiplierY" :min="0" :step="0.1" :max="3" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.opts.repaintAlpha!==undefined" label="线条透明度">
            <el-slider v-model="configData.chartOption.opts.repaintAlpha" :min="0" :step="0.001" :max="1" show-input></el-slider>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="4">
          <el-form-item v-if="configData.chartOption.animate !== undefined" label="载入动画">
            <el-select v-model="configData.chartOption.animate" placeholder="请选择">
              <el-option v-for="item in animateOptions" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
        </el-collapse-item>
      </el-collapse>
    </el-form>
</template>
<script>
import { animateOptions } from "../../../animate/animate";
export default {
  props: {
    configData: {
      type: Object,
      required: true
    },
  },
  data() {
    return {
      activeNames: ["1"],
      animateOptions,
    }
  },
  methods: {}
}
</script>
<style lang="scss" scoped>
::v-deep {
  .el-input--medium .el-input__inner {
    height: 32px;
    width: 100%;
  }
  .inputFontSize{
    width: 100%;
  }
  .el-select{
    height: 32px;
    width: 100%;
  }
  .el-input--suffix, .el-input__inner{
    height: 32px;
  }
  .el-select .el-input__icon {
    line-height: 32px; //el-select 改了多高，这边多高
  }
  .lableText .el-form-item__label{
      float: none;
  }
}
</style>