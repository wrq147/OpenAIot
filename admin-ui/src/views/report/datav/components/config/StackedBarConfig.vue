<template>
    <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="配置" name="field" />
      <el-tab-pane label="数据" name="data" />
      <el-tab-pane label="定位" name="location" />
    </el-tabs>
    <div class="field-box">
      
      <el-scrollbar class="right-scrollbar">
        <!-- 组件属性 -->
        <el-form v-if="currentTab==='field'" size="small" label-width="90px">
          <el-collapse  v-model="activeNames" accordion>
            <el-collapse-item title="图层" name="1">
              <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
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
            <el-collapse-item title="图例" name="3">
              <el-form-item v-if="configData.chartOption.legend.show!==undefined" label="显示图例">
                <el-switch v-model="configData.chartOption.legend.show" />
              </el-form-item>

              <el-form-item label="标记宽度">
                <el-input-number v-model="configData.chartOption.itemWidth"/>
              </el-form-item>

              <el-form-item label="标记高度">
                <el-input-number v-model="configData.chartOption.itemHeight"/>
              </el-form-item> 

              <el-form-item label="字号">
                <el-slider v-model="configData.chartOption.legendFontSize" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item label="字体颜色">
                <el-color-picker v-model="configData.chartOption.legendFontColor" show-alpha></el-color-picker>
              </el-form-item>

              <el-form-item label="布局朝向">
                <el-select v-model="configData.chartOption.legendOrient" placeholder="请选择">
                  <el-option
                    v-for="item in legendOrient"
                    :key="item.key"
                    :label="item.name"
                    :value="item.key">
                  </el-option>
                </el-select>
              </el-form-item>

              <el-form-item label="水平位置">
                <el-input-number :min="0" :max="100" v-model="configData.chartOption.legendX"/>
              </el-form-item>
              <el-form-item label="垂直位置">
                <el-input-number :min="0" :max="100" v-model="configData.chartOption.legendY"/>
              </el-form-item>

            </el-collapse-item>  
            <el-collapse-item title="柱体" name="4">
              <el-form-item v-if="configData.chartOption.barWidth!==undefined" label="柱体宽度">
                <el-input v-model="configData.chartOption.barWidth" placeholder="请输入柱体宽度" />
              </el-form-item>
              <el-form-item v-if="configData.chartOption.isVertical!==undefined" label="竖展示">
                <el-switch v-model="configData.chartOption.isVertical" />
              </el-form-item>

            </el-collapse-item>
            <el-collapse-item title="坐标轴" name="5">
                  
              <el-form-item v-if="configData.chartOption.xAxis.name!==undefined" label="X轴名称">
                <el-input v-model="configData.chartOption.xAxis.name" placeholder="请输入X轴名称" />
              </el-form-item>

              <el-form-item v-if="configData.chartOption.yAxis.name!==undefined" label="y轴名称">
                <el-input v-model="configData.chartOption.yAxis.name" placeholder="请输入y轴名称" />
              </el-form-item>

              <el-form-item v-if="configData.chartOption.xAxis.show!==undefined" label="是否显示X轴">
                <el-switch v-model="configData.chartOption.xAxis.show" />
              </el-form-item>

              <el-form-item v-if="configData.chartOption.yAxis.show!==undefined" label="是否显示y轴">
                <el-switch v-model="configData.chartOption.yAxis.show" />
              </el-form-item>

              <el-form-item v-if="configData.chartOption.yAxis.splitLine.show!==undefined" label="是否显示分隔线">
                <el-switch v-model="configData.chartOption.yAxis.splitLine.show" />
              </el-form-item>
            </el-collapse-item>

            <el-collapse-item title="动画" name="6">

              <el-form-item v-if="configData.chartOption.animate!==undefined" label="载入动画">
                <el-select v-model="configData.chartOption.animate" placeholder="请选择">
                  <el-option
                    v-for="item in animateOptions"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-collapse-item>
          </el-collapse>

        </el-form>
        <!-- 表单属性 -->
        <el-form v-if="currentTab === 'data'" size="small" label-width="90px">
          <data-source-config :drawingList="drawingList" :themeForm="themeForm" :dataSourceType="configData.chartOption.dataSourceType" :customData="configData" :customId="configData.customId" 
            @changeSource="changeSource"  @changeData="changeData" :baseType="'four'"></data-source-config>

        </el-form>
        <!-- 组件位置 -->
        <el-form v-if="currentTab === 'location'" size="small" label-width="90px">
          <el-form-item label="X位置">
            <el-input-number v-model="configData.x" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="y位置">
            <el-input-number v-model="configData.y" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="宽度">
            <el-input-number v-model="configData.width" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="高度">
            <el-input-number v-model="configData.height" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="zIndex">
            <el-input-number v-model="configData.zindex" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
        </el-form>
        
      </el-scrollbar>
    </div>

  </div>
</template>

<script>

import {animateOptions} from '../../animate/animate'
import { getLinkChart} from "../../util/LinkageChart";

import DataSourceConfig from './DataConfig/DataSourceConfig'
import sourceConfig from '../mixins/sourceConfig.js'
export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
  },
  data() {
    return {
      activeNames: ['1'],
      currentTab: 'field',
      animateOptions,
      arrName: this.costomData != null ? this.costomData.chartOption.arrName : '',
      legendOrient:[{key:"horizontal",name:"水平"},{key:"vertical",name:"垂直"}],
      chartList: this.drawingList,
    }
  },
  //页面加载完执行
  mounted() {
    //获取可联动组件列表
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;

  },
  computed: {
    
  },
  methods: {
    
    changeArrName(){
      this.$set(this.configData.chartOption, 'arrName', this.arrName);
    },
    bindCharts(val){
       this.$set(this.configData.chartOption, 'bindList', val);
    },
    isLinkChange() {
      if (this.configData.chartOption.isLink == true) {
        this.$set(this.configData.chartOption, 'isDrillDown', false);
        //this.configData.chartOption.isDrillDown == false;
      }
    },
  
    
  }
}
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
    width: 33%;
    text-align: center;
}
.select-item {
  display: flex;
  border: 1px dashed #fff;
  box-sizing: border-box;
  & .close-btn {
    cursor: pointer;
    color: #f56c6c;
  }
  & .el-input + .el-input {
    margin-left: 4px;
  }
}

.select-item + .select-item {
  margin-top: 4px;
}
.select-item.sortable-chosen {
  border: 1px dashed #409eff;
}
.select-line-icon {
  line-height: 32px;
  font-size: 22px;
  padding: 0 4px;
  color: #777;
}
.screen>:first-child{
  margin-top:37px
}

/**  滚动条凹槽的颜色，还可以设置边框属性  **/
::-webkit-scrollbar-track-piece { 

  background-color:#f8f8f8; 
  border-radius: 10px;
}
/** 滚动条的宽度  **/
::-webkit-scrollbar {

  width:9px;

  height:9px; 

}
/** 滚动条的设置  **/
::-webkit-scrollbar-thumb {

  background-color:#dddddd;

  background-clip:padding-box;

  min-height:28px; 
  border-radius: 10px;

}

::-webkit-scrollbar-thumb:hover {

  background-color:#bbb; 

}
::v-deep .el-upload-dragger {
    background-color: #fff;
    border: 1px dashed #d9d9d9;
    border-radius: 6px;
    box-sizing: border-box;
    width: 225px;
    height: 180px;
    text-align: center;
    cursor: pointer;
    position: relative;
    overflow: hidden;
}
</style>