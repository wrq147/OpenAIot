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
          <el-form-item v-if="configData.chartOption.title.textStyle.fontSize!==undefined" label="字体大小">
            <el-slider v-model="configData.chartOption.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.color!==undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图例设置" name="3">
          <el-form-item v-if="configData.chartOption.legend.show!==undefined" label="显示图例">
            <el-switch v-model="configData.chartOption.legend.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show===true" label="图例样式">
            <el-radio v-model="configData.chartOption.legend.orient" label="level">水平</el-radio>
            <el-radio v-model="configData.chartOption.legend.orient" label="horizontal">垂直</el-radio>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show===true" label="图例水平位置">
            <el-select v-model="configData.chartOption.legend.x" placeholder="请选择">
              <el-option label="居左" value="left"></el-option>
              <el-option label="居中" value="center"></el-option>
              <el-option label="居右" value="right"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show===true" label="图例垂直位置">
            <el-select v-model="configData.chartOption.legend.y" placeholder="请选择">
              <el-option label="顶部" value="top"></el-option>
              <el-option label="中部" value="center"></el-option>
              <el-option label="底部" value="bottom"></el-option>
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="饼图" name="4">
          <el-form-item v-if="configData.chartOption.innerRadius!==undefined" label="内半径">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.innerRadius"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.outerRadius!==undefined" label="外半径">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.outerRadius"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.innerPieWidth!==undefined" label="内环线宽度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.innerPieWidth"></el-input-number>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="引导线设置" name="5">
          <el-form-item v-if="configData.chartOption.length!==undefined" label="一段引导线长度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.length"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.length2!==undefined" label="二段引导线长度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.length2"></el-input-number>
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
      fontFamilys: [],
      activeNames: ["1"],
      animateOptions,
      chartList: this.drawingList,
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
  .delete-icon {
    line-height: 32px;
    font-size: 22px;
    padding: 0 4px;
    cursor: pointer;
    color: #f56c6c;
  }
</style>