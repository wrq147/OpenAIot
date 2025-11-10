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
        <el-collapse-item title="图表" name="3">
          
          <el-form-item v-if="configData.chartOption.name!==undefined" label="标签名称">
            <el-input v-model="configData.chartOption.name" placeholder="请输入标签名称" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.top!==undefined" label="距顶部距离">
            <el-slider v-model="configData.chartOption.top" show-input></el-slider>
            
          </el-form-item>

          <el-form-item v-if="configData.chartOption.bottom!==undefined" label="距底部距离">
            <el-slider v-model="configData.chartOption.bottom" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.isThemeColor!==undefined" label="是否应用主题颜色">
            <el-switch v-model="configData.chartOption.isThemeColor" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.itemPositive!==undefined" label="正数柱体颜色">
            <el-color-picker v-model="configData.chartOption.itemPositive" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.itemNegative!==undefined" label="负数柱体颜色">
            <el-color-picker v-model="configData.chartOption.itemNegative" show-alpha></el-color-picker>
          </el-form-item>

        </el-collapse-item>
        <el-collapse-item title="坐标轴" name="4">
          
          <el-form-item v-if="configData.chartOption.position!==undefined" label="坐标轴位置">
            <el-select v-model="configData.chartOption.position" placeholder="请选择">
              <el-option  label="上部" value="top"></el-option>
              <el-option  label="下部" value="bottom"></el-option>
              <el-option  label="左部" value="left"></el-option>
              <el-option  label="右部" value="right"></el-option>
            </el-select>
          </el-form-item>              
          <el-form-item v-if="configData.chartOption.direction!==undefined" label="正负轴方向">
            <el-select v-model="configData.chartOption.direction" placeholder="请选择">
              <el-option  label="水平" value="level"></el-option>
              <el-option  label="垂直" value="vertical"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.lineStyle!==undefined" label="分割线样式">
            <el-select v-model="configData.chartOption.lineStyle" placeholder="请选择">
              <el-option  label="实线" value="solid"></el-option>
              <el-option  label="虚线" value="dashed"></el-option>
              <el-option  label="点线" value="dotted"></el-option>
            </el-select>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.xAxisShow!==undefined" label="是否显示坐标轴">
            <el-switch v-model="configData.chartOption.xAxisShow" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.showLabel!==undefined" label="是否显示标签">
            <el-switch v-model="configData.chartOption.showLabel" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.axisLine!==undefined" label="是否显示坐标轴横线">
            <el-switch v-model="configData.chartOption.axisLine" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.axisTick!==undefined" label="是否显示坐标轴刻度">
            <el-switch v-model="configData.chartOption.axisTick" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.axisLabel!==undefined" label="是否显示坐标轴标签">
            <el-switch v-model="configData.chartOption.axisLabel" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="5">
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