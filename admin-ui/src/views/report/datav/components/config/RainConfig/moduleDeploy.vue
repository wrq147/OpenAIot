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
          <el-form-item v-if="configData.chartOption.legend.show!==undefined" label="显示图例">
            <el-switch v-model="configData.chartOption.legend.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.toolbox.show!==undefined" label="显示工具箱">
            <el-switch v-model="configData.chartOption.toolbox.show" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="x轴设置" name="3">
          <el-form-item v-if="configData.chartOption.xAxis[0].show!==undefined" label="显示X轴">
            <el-switch v-model="configData.chartOption.xAxis[0].show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis[0].show ==true" label="x轴留白">
            <el-switch v-model="configData.chartOption.xAxis[0].boundaryGap" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="y轴设置" name="4">
          <el-form-item v-if="configData.chartOption.yAxis[0].name!==undefined" label="左y轴名称">
            <el-input v-model="configData.chartOption.yAxis[0].name" placeholder="请输入左y轴名称" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis[1].name!==undefined" label="右y轴名称">
            <el-input v-model="configData.chartOption.yAxis[1].name" placeholder="请输入右y轴名称" />
          </el-form-item>
          <el-form-item label="左y轴max">
            <el-input-number v-model="configData.chartOption.yAxis[0].max" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="右y轴max">
            <el-input-number v-model="configData.chartOption.yAxis[1].max" controls-position="right" :step="1"></el-input-number>
          </el-form-item> 
          <el-form-item v-if="configData.chartOption.yAxis[0].show!==undefined" label="显示左侧y轴">
            <el-switch v-model="configData.chartOption.yAxis[0].show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis[1].show!==undefined" label="显示右侧y轴">
            <el-switch v-model="configData.chartOption.yAxis[1].show" />
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
      fontFamilys: [],
      activeNames: ["1"],
      animateOptions,
      fontFamilys:[],
      nameLocation:[{key:'start',name:"起点"},{key:'center',name:"中间"},{key:'end',name:"末尾"}],
      legendOrient:[{key:"horizontal",name:"水平"},{key:"vertical",name:"垂直"}],
      splitLineType:[{key:'solid', name:"实线"},{key:'dashed',name:"短横虚线"},{key:'dotted',name:'点状虚线'}],
      chartList: this.drawingList,
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