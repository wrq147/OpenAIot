<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标题" name="2">
          <el-form-item v-if="configData.chartOption.title.text!==undefined" label="标题">
            <el-input v-model="configData.chartOption.title.text" placeholder="请输入标题" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="表盘" name="3">
          <el-form-item v-if="configData.chartOption.radius!==undefined" label="半径">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.radius" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.min!==undefined" label="起始值">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.min" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.max!==undefined" label="终止值">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.max" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.startAngle!==undefined" label="起始角度">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.startAngle" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.endAngle!==undefined" label="终止角度">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.endAngle" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="刻度" name="4">
          <el-form-item v-if="configData.chartOption.splitNumber!==undefined" label="刻度数">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.splitNumber" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.length!==undefined" label="刻度长度">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.length" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.width!==undefined" label="刻度宽度">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.width" />
          </el-form-item>
          <el-form-item label="应用主题">
            <el-switch v-model="configData.chartOption.isTopic" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isTopic===false" label="刻度颜色">
            <div style="display:flex">
              <el-form-item label="位置"></el-form-item>
              <el-form-item label="颜色" style="margin-left: 10px;"></el-form-item>
            </div>
            <div v-for="(item, index) in configData.chartOption.color" :key="index" class="select-item">
              <el-input-number class="inputFontSize" v-model="item[0]" placeholder="位置" size="small" :min="0" :max="1" :step="0.1"/>
              <el-color-picker v-model="item[1]" placeholder="颜色" size="small" style="margin-left: 15px;" show-alpha />
            </div>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.noColor!==undefined" label="剩余刻度颜色">
            <el-color-picker v-model="configData.chartOption.noColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="值" name="5">
          <el-form-item label="显示数值">
            <el-switch v-model="configData.chartOption.isdetail" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.detailFamily!==undefined" label="字体">
            <el-select v-model="configData.chartOption.detailFamily" placeholder="请选择">
              <el-option v-for="(item, index) in detailFamily" :key="index" :label="item" :value="item">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.detailSize!==undefined" label="数字大小">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.detailSize" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.detailWeight!==undefined" label="数字样式">
            <el-select v-model="configData.chartOption.detailWeight" placeholder="请选择">
              <el-option v-for="(item,index) in detailWeight" :key="index" :label="item" :value="item">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.detailColor!==undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.detailColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="6">
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
    costomData: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      activeNames: ["1"],
      animateOptions,
      chartList: this.drawingList,
      detailFamily: ['宋体', '黑体', '微软雅黑', 'Digital', 'Chunkfive', 'unidreamLED', 'Arial', 'Helvetica', 'sans-serif'],
      detailWeight: ['normal', 'bold', 'bolder', 'lighter'],
    }
  },
  methods: {
  }
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
  .delete-icon {
    line-height: 32px;
    font-size: 22px;
    padding: 0 4px;
    cursor: pointer;
    color: #f56c6c;
  }
</style>