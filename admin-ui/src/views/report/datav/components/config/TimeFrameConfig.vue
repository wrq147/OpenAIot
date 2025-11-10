<template>
    <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="配置" name="field" />
      <el-tab-pane label="数据" name="data" />
      <el-tab-pane label="交互" name="interaction" />
      <el-tab-pane label="定位" name="location" />
    </el-tabs>
    <div class="field-box">
      
      <el-scrollbar class="right-scrollbar">
        <!-- 组件属性 -->
        <el-form v-if="currentTab==='field'" size="small" label-width="90px">

          <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.context!==undefined" label="label内容">
            <el-input v-model="configData.chartOption.context" placeholder="请输入label内容" />
          </el-form-item>
          
          <el-form-item v-if="configData.chartOption.fontSize!==undefined" label="label字号">
              <el-slider v-model="configData.chartOption.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.fontFamily!==undefined" label="label字体">
            <el-select v-model="configData.chartOption.fontFamily" placeholder="请选择">
              <el-option
                v-for="(item,index) in fontFamilys"
                :key="index"
                :label="item"
                :value="item">
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.fontColor!==undefined" label="label字体颜色">
            <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.fontWeight!==undefined" label="label文字粗细">
            <el-select v-model="configData.chartOption.fontWeight" placeholder="请选择">
              <el-option
                v-for="(item,index) in fontWeights"
                :key="index"
                :label="item"
                :value="item">
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.width" label="宽度">
            <el-input-number v-model="configData.chartOption.width" controls-position="right" :min="1" :step="1"></el-input-number>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.name!==undefined" label="name属性值">
            <el-input v-model="configData.chartOption.name" placeholder="请输入name属性值" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.type!==undefined" label="日期格式">
            <el-select v-model="configData.chartOption.type" placeholder="请选择">
              <el-option
                v-for="(item,index) in types"
                :key="index"
                :label="item.label"
                :value="item.value">
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item  label="绑定组件">
            <el-select v-model="configData.chartOption.bindList" multiple filterable placeholder="请选择" @change="bindCharts">
              <el-option
                  v-for="item in chartList"
                  :key="item.customId"
                  :label="item.layerName"
                  :value="item.customId">
                </el-option>
            </el-select>
          </el-form-item>

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

        </el-form>
        <!-- 表单属性 -->
        <el-form v-if="currentTab === 'data'" size="small" label-width="90px">
          <data-source-config :drawingList="drawingList" :themeForm="themeForm" :dataSourceType="configData.chartOption.dataSourceType" :customData="configData" :customId="configData.customId" 
            @changeSource="changeSource"  @changeData="changeData" :baseType="''"></data-source-config>

        </el-form>
         <!-- 组件交互 -->
         <el-form
          v-if="currentTab === 'interaction'"
          size="small"
          label-width="90px"
        >
          <div
            style="
              background-color: #f5f5f5;
              padding: 10px 15px;
              border: solid 1px #dadada;
              font-size: 14px;
              margin-bottom: 15px;
              color: #999;
            "
          >
            事件预处理
          </div>
          <chart-interact
            :chartOption="configData.chartOption"
            @changeData="changeInteractData"
          ></chart-interact>
          <div
            style="
              background-color: #f5f5f5;
              padding: 10px 15px;
              border: solid 1px #dadada;
              font-size: 14px;
              margin-bottom: 15px;
              color: #999;
            "
          >
            事件发生后更新数据源
          </div>
          <el-form-item label="变更事件">
            <el-select
              v-model="configData.chartOption.dataList"
              multiple
              placeholder="请选择"
              @change="bindDataList"
            >
              <el-option
                v-for="item in dataList"
                :key="item.name"
                :label="item.name"
                :value="item.name"
              >
              </el-option>
            </el-select>
          </el-form-item>
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
import ChartInteract from "./interact/ChartInteract";
import DataSourceConfig from './DataConfig/DataSourceConfig'
import sourceConfig from '../mixins/sourceConfig.js'
export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
    ChartInteract
  },
  data() {
    return {
      fontFamilys:this.fontFamilys,
      fontWeights:['normal','bold','bolder','lighter'],
      types: [{label:'月范围',value:'monthrange'},{label:'日期范围',value:'daterange'},{label:'日期时间范围',value:'datetimerange'}],
      currentTab: 'field',
      chartList: this.drawingList,
      animateOptions,
      dataList: [],
    }
  },
  //页面加载完执行
  mounted() {
    
    //获取可联动组件列表
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;
    this.dataList = this.themeForm.globalData;
  },
  computed: {
   
  },

  methods: {
    bindDataList(val) {
      this.$set(this.configData.chartOption, "dataList", val);
    },
    bindCharts(val){
       this.$set(this.configData.chartOption, 'bindList', val);
    },
  }
}
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
    width: 25%;
    text-align: center;
}
</style>