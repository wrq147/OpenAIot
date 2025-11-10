<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item  title="标题" name="2">
          <el-form-item v-if="configData.chartOption.title.text!==undefined" label="标题">
            <el-input v-model="configData.chartOption.title.text" placeholder="请输入标题" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.x!==undefined" label="标题位置">
            <el-select v-model="configData.chartOption.title.x" placeholder="请选择">
              <el-option label="居左" value="left"></el-option>
              <el-option label="居中" value="center"></el-option>
              <el-option label="居右" value="right"></el-option>
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图例" name="3">
          <el-form-item v-if="configData.chartOption.legend.show!==undefined" label="显示图例">
            <el-switch v-model="configData.chartOption.legend.show" />
          </el-form-item>
          <el-form-item v-show="configData.chartOption.legend.show===true" label="图例方向">
            <el-radio-group v-model="configData.chartOption.legend.orient">
              <el-radio label="horizontal">横向</el-radio>
              <el-radio label="vertical">纵向</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show===true" label="图例垂直位置">
            <el-select v-model="configData.chartOption.legend.top" placeholder="请选择">
              <el-option label="顶部" value="top"></el-option>
              <el-option label="中部" value="middle"></el-option>
              <el-option label="底部" value="bottom"></el-option>
            </el-select>
          </el-form-item> 
          <el-form-item v-if="configData.chartOption.legend.show===true" label="图例水平位置">
            <el-select v-model="configData.chartOption.legend.x" placeholder="请选择">
              <el-option label="居左" value="left"></el-option>
              <el-option label="居中" value="center"></el-option>
              <el-option label="居右" value="right"></el-option>
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="设置" name="4">
          <el-form-item v-if="configData.chartOption.fontSize!==undefined" label="标签字体大小">
            <el-slider  v-model="configData.chartOption.fontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.fontFamily!==undefined" label="标签字体样式">
            <el-select v-model="configData.chartOption.fontFamily" placeholder="请选择">
              <el-option
                v-for="(item,index) in fontFamilys"
                :key="index"
                :label="item"
                :value="item">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.fontWeight!==undefined" label="标签字体粗细">
            <el-select v-model="configData.chartOption.fontWeight" placeholder="请选择">
              <el-option label="normal" value="normal"></el-option>
              <el-option label="bold" value="bold"></el-option>
              <el-option label="bolder" value="bolder"></el-option>
              <el-option label="lighter" value="lighter"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.fontColor!==undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.symbolSize!==undefined" label="标签大小">
            <el-slider v-model="configData.chartOption.symbolSize" :min="0"  :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.repulsion!==undefined" label="斥力系数">
            <el-slider v-model="configData.chartOption.repulsion" :min="0"  :step="10" :max="5000" show-input></el-slider>
          </el-form-item>
          <el-form-item label="应用主题">
            <el-switch v-model="configData.chartOption.isTopic" />
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
  },
  data() {
    return {
      activeNames: ["1"],
      fontFamilys: [],
      animateOptions,
    }
  },
  methods: {}
}
</script>
<style lang="scss" scoped>
::v-deep {
  .el-input-numbe{
    width: 100% !important;
  }
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