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
              
              <el-form-item v-if="configData.chartOption.fontWeight!==undefined" label="标题粗细">
                <el-select v-model="configData.chartOption.fontWeight" placeholder="请选择">
                  <el-option label="normal" value="normal"></el-option>
                  <el-option label="bold" value="bold"></el-option>
                  <el-option label="bolder" value="bolder"></el-option>
                  <el-option label="lighter" value="lighter"></el-option>
                </el-select>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.title.x!==undefined" label="标题位置">
                <el-select v-model="configData.chartOption.title.x" placeholder="请选择">
                  <el-option label="居左" value="left"></el-option>
                  <el-option label="居中" value="center"></el-option>
                  <el-option label="居右" value="right"></el-option>
                </el-select>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.title.textStyle.fontSize!==undefined" label="标题字号">
                <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.title.textStyle.fontSize"></el-input-number>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.title.textStyle.fontWeight!==undefined" label="标题粗细">
                <el-select v-model="configData.chartOption.title.textStyle.fontWeight" placeholder="请选择">
                  <el-option label="normal" value="normal"></el-option>
                  <el-option label="bold" value="bold"></el-option>
                  <el-option label="bolder" value="bolder"></el-option>
                  <el-option label="lighter" value="lighter"></el-option>
                </el-select>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.title.textStyle.fontFamily!==undefined" label="字体样式">
                <el-select v-model="configData.chartOption.title.textStyle.fontFamily" placeholder="请选择">
                  <el-option
                    v-for="(item,index) in fontFamilys"
                    :key="index"
                    :label="item"
                    :value="item">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-collapse-item>
            
            <el-collapse-item title="图例" name="3">
              <el-form-item v-if="configData.chartOption.legend.show!==undefined" label="显示图例">
                <el-switch v-model="configData.chartOption.legend.show" />
              </el-form-item>

              <el-form-item v-if="configData.chartOption.legend.show===true" label="图例垂直位置">
                <el-select v-model="configData.chartOption.legend.y" placeholder="请选择">
                  <el-option label="顶部" value="normal"></el-option>
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

            <el-collapse-item title="环柱" name="4">

              <el-form-item v-if="configData.chartOption.innerRadius!==undefined" label="内半径">
                <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.innerRadius"></el-input-number>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.outerRadius!==undefined" label="外半径">
                <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.outerRadius"></el-input-number>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.col!==undefined" label="列数">
                <el-input-number v-model="configData.chartOption.col" controls-position="right" :min="1"  :step="1"></el-input-number>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.centerX!==undefined" label="x轴起点位置(%)">
                <el-slider v-model="configData.chartOption.centerX" :min="1" :max="100" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.centerY!==undefined" label="y轴起点位置(%)">
                <el-slider v-model="configData.chartOption.centerY" :min="1" :max="100" :step="1" show-input></el-slider>
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
            @changeSource="changeSource"  @changeData="changeData" :baseType="'two'"></data-source-config>

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
              <el-switch v-model="configData.chartOption.isLink" @change="isLinkChange" />
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
      fontFamilys:this.fontFamilys,
      currentTab: 'field',
      activeNames: ['1'],
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
   

  }
}
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
    width: 25%;
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