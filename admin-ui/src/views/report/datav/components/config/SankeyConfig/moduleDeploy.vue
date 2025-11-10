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
          <el-form-item label="标题字号">
            <el-input-number class="inputFontSize" controls-position="right" :min="0" v-model="configData.chartOption.title.textStyle.fontSize" />
          </el-form-item>
          <el-form-item label="标题颜色">
            <el-color-picker v-model="configData.chartOption.title.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
          </el-form-item>
          <el-form-item label="副标题字号">
            <el-input-number class="inputFontSize" controls-position="right" :min="0" v-model="configData.chartOption.title.subtextStyle.fontSize" />
          </el-form-item>
          <el-form-item label="副标题颜色">
            <el-color-picker v-model="configData.chartOption.title.subtextStyle.color" show-alpha></el-color-picker>
          </el-form-item>       
        </el-collapse-item>
        <el-collapse-item title="图形设置" name="3">
          <el-form-item label="布局朝向">
            <el-select v-model="configData.chartOption.series[0].orient" placeholder="请选择">
              <el-option v-for="item in orient" :key="item.key" :label="item.name" :value="item.key" />
            </el-select>
          </el-form-item>
          <el-form-item label="图形上边距">
            <el-input-number class="inputFontSize" controls-position="right" :min="0" v-model="configData.chartOption.series[0].top" />
          </el-form-item>
          <el-form-item label="图形右边距">
            <el-input-number class="inputFontSize" controls-position="right" :min="0" v-model="configData.chartOption.series[0].right" />
          </el-form-item>
          <el-form-item label="图形下边距">
            <el-input-number class="inputFontSize" controls-position="right" :min="0" v-model="configData.chartOption.series[0].bottom" />
          </el-form-item>
          <el-form-item label="图形左边距">
            <el-input-number class="inputFontSize" controls-position="right" :min="0" v-model="configData.chartOption.series[0].left" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="节点设置" name="4">
          <el-form-item label="节点宽度">
            <el-input-number class="inputFontSize" controls-position="right" :min="0" v-model="configData.chartOption.series[0].nodeWidth" />
          </el-form-item>
          <el-form-item label="节点间距">
            <el-input-number class="inputFontSize" controls-position="right" :min="0" v-model="configData.chartOption.series[0].nodeGap" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标签设置" name="4">
          <el-form-item label="标签位置">
            <el-select v-model="configData.chartOption.series[0].label.position" placeholder="请选择">
                <el-option v-for="(item,index) in labelPosition" :key="index" :label="item.label" :value="item.key" />
            </el-select>
          </el-form-item>
          <el-form-item label="标签间距">
            <el-input-number class="inputFontSize" controls-position="right" :min="0" v-model="configData.chartOption.series[0].label.distance" />
          </el-form-item>
          <el-form-item label="标签字号">
            <el-input-number class="inputFontSize" controls-position="right" :min="0" v-model="configData.chartOption.series[0].label.fontSize" />
          </el-form-item>
          <el-form-item label="标签颜色">
            <el-color-picker v-model="configData.chartOption.series[0].label.color" show-alpha />
          </el-form-item>
          <el-form-item label="标签字体">
            <el-select v-model="configData.chartOption.series[0].label.fontFamily" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="桑基图边设置" name="5">
          <el-form-item label="颜色">
            <el-select v-model="configData.chartOption.series[0].lineStyle.colorSelect" placeholder="请选择">
              <el-option v-for="item in lineStyleColor" :key="item.key" :label="item.name" :value="item.key" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].lineStyle.colorSelect == 'customer'" >
            <el-color-picker v-model="configData.chartOption.series[0].lineStyle.color" show-alpha />
          </el-form-item>
          <el-form-item label="透明度">
            <el-input-number class="inputFontSize" :min="0" :max="1" :step="0.1" v-model="configData.chartOption.series[0].lineStyle.opacity" />
          </el-form-item>
          <el-form-item label="曲度">
            <el-input-number class="inputFontSize" :min="0" :max="1" :step="0.1" v-model="configData.chartOption.series[0].lineStyle.curveness" />
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
      fontFamilys: [],
      lineStyleColor:[{key:"source",name:"源节点颜色"},{key:"target",name:"目标节点颜色"},{key:"customer",name:"自定义颜色"}],
      labelPosition:[
        {key:"top",label:"上"},{key:"left",label:"左"},{key:"right",label:"右"},{key:"bottom",label:"下"},{key:"inside",label:"内部中间"},
        {key:"insideLeft",label:"中间左部"},{key:"insideRight",label:"中间右部"},{key:"insideTop",label:"中间上部"},{key:"insideBottom",label:"中间下部"},
        {key:"insideTopLeft",label:"内部左上"},{key:"insideBottomLeft",label:"内部左下"},{key:"insideTopRight",label:"内部右上"},{key:"insideBottomRight",label:"内部右下"}],
      nameLocation:[{key:'start',name:"起点"},{key:'center',name:"中间"},{key:'end',name:"末尾"}],
      orient:[{key:"horizontal",name:"水平"},{key:"vertical",name:"垂直"}],
      nodeAlign:[{key:"justify",name:"双端对齐"},{key:"left",name:"左对齐"},{key:"right",name:"右对齐"}],
      splitLineType:[{key:'solid', name:"实线"},{key:'dashed',name:"短横虚线"},{key:'dotted',name:'点状虚线'}],
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
    .el-slider{
      width: 95%;
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