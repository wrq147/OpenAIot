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

              <el-form-item v-if="configData.chartOption.isShowLegend!==undefined" label="是否显示图例">
                <el-switch v-model="configData.chartOption.isShowLegend" />
              </el-form-item>

            </el-collapse-item>

            <el-collapse-item title="柱体" name="3">
              <el-form-item v-if="configData.chartOption.maxHeight!==undefined" label="柱体最大高度">
               <el-slider v-model="configData.chartOption.maxHeight" :min="1" :max="500" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.barSpacing!==undefined" label="柱体距离">
               <el-slider v-model="configData.chartOption.barSpacing" :min="1" :max="500" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.bottomSize[0]!==undefined" label="柱体x轴长度">
               <el-slider v-model="configData.chartOption.bottomSize[0]" :min="1" :max="500" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.bottomSize[1]!==undefined" label="柱体y轴长度">
               <el-slider v-model="configData.chartOption.bottomSize[1]" :min="1" :max="500" :step="1" show-input></el-slider>
              </el-form-item>

            </el-collapse-item>

            <el-collapse-item title="夹角" name="4">
              
              <el-form-item v-if="configData.chartOption.projectAngle[0]!==undefined" label="投影夹角α">
                  <el-slider v-model="configData.chartOption.projectAngle[0]" :min="1" :max="90" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.projectAngle[1]!==undefined" label="投影夹角β">
               <el-slider v-model="configData.chartOption.projectAngle[1]" :min="1" :max="90" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.lightAngle[0]!==undefined" label="光线夹角α">
                  <el-slider v-model="configData.chartOption.lightAngle[0]" :min="1" :max="90" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.lightAngle[1]!==undefined" label="光线夹角β">
                  <el-slider v-model="configData.chartOption.lightAngle[1]" :min="1" :max="90" :step="1" show-input></el-slider>
              </el-form-item>

            </el-collapse-item>

           

            <el-collapse-item title="动画" name="5">

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
        <!-- 组件交互 -->
        <el-form v-if="currentTab === 'interaction'" size="small" label-width="90px">
          <el-form-item  label="是否开启图表联动">
              <el-switch v-model="configData.chartOption.isLink" @change="isLinkChange"/>
          </el-form-item>

          <el-form-item v-show="configData.chartOption.isLink===true"  label="参数名称">
            <el-input v-model="arrName" placeholder="请参数名称" @blur.prevent="changeArrName()"/>
          </el-form-item>

          <el-form-item  v-show="configData.chartOption.isLink===true" label="绑定组件">
            <el-select v-model="configData.chartOption.bindList" multiple placeholder="请选择" @change="bindCharts">
              <el-option
                  v-for="item in chartList"
                  :key="item.customId"
                  :label="item.layerName"
                  :value="item.customId">
                </el-option>
            </el-select>
          </el-form-item>
          
          <el-form-item  v-show="configData.chartOption.isLink===true" label="*参数说明">
            <div class="el-form-item__content">
              <span style="word-wrap: break-word;"><i style="color:#F00;">参数名={"legendName":"xx","seriesName":"xxx","data":xxx}</i></span>
            </div>
            <div class="el-form-item__content">
              <span >legendName:图例名称</span>
              <span style="display:block;">seriesName:系列名称</span>
              <span >data:选中值</span>
            </div>
            
          </el-form-item> 
       
        <!-- 是否开启图表远程控制 -->
        
          <el-form-item  label="是否开启图表远程控制">
                <el-switch v-model="configData.chartOption.isRemote" />
          </el-form-item>

          <el-form-item  v-show="configData.chartOption.isRemote === true" label="*参数说明">
            <div class="el-form-item__content">
              <span style="word-wrap: break-word;"><i style="color:#F00;">{"legendName":"xx","seriesName":"xxx","data":xxx}</i></span>
            </div>
            <div class="el-form-item__content">
              <span >legendName:图例名称</span>
              <span style="display:block;">seriesName:系列名称</span>
              <span >data:选中值</span>
            </div>
            
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
      fontFamilys:this.fontFamilys,
      fontWeights:['normal','bold','bolder','lighter'],
      animateOptions,
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
    
    getRemote(val){
      this.$set(this.configData.chartOption, 'remote', val);
    },
  }
}
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
    width: 25%;
    text-align: center;
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