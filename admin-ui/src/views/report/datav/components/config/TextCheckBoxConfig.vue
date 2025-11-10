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

          <el-collapse-item title="标签边距" name="2">

            <el-form-item v-if="configData.chartOption.paddingLeftRight!==undefined" label="左右内边距">
              <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.paddingLeftRight"></el-input-number>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.paddingTopBottom!==undefined" label="上下内边距">
              <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.paddingTopBottom"></el-input-number>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.marginLeftRight!==undefined" label="左右外边距">
              <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.marginLeftRight"></el-input-number>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.marginTopBottom!==undefined" label="上下外边距">
              <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.marginTopBottom"></el-input-number>
            </el-form-item>

          </el-collapse-item>

          <el-collapse-item title="默认样式" name="3">

            <el-form-item v-if="configData.chartOption.fontSize!==undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.fontSize" :min="1" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.letterSpacing!==undefined" label="字体间距">
              <el-slider v-model="configData.chartOption.letterSpacing" :min="0" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.fontColor!==undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.fontFamily!==undefined" label="字体名称">
              <el-select v-model="configData.chartOption.fontFamily" placeholder="请选择">
                <el-option
                  v-for="(item,index) in fontFamilys"
                  :key="index"
                  :label="item"
                  :value="item">
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.fontWeight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.fontWeight" placeholder="请选择">
                <el-option
                  v-for="(item,index) in fontWeight"
                  :key="index"
                  :label="item"
                  :value="item">
                </el-option>
              </el-select>
            </el-form-item>

            <!-- <el-form-item v-if="configData.chartOption.borderWidth!==undefined" label="边框宽度">
              <el-slider v-model="configData.chartOption.borderWidth" :min="0" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.borderColor!==undefined" label="边框颜色">
              <el-color-picker v-model="configData.chartOption.borderColor" show-alpha></el-color-picker>
            </el-form-item> -->

            <el-form-item label="背景设置">
              <el-radio-group v-model="configData.chartOption.normalBGFlag">
                <el-radio label="color">背景颜色</el-radio>
                <el-radio label="img">背景图片</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.backgroundColor!==undefined && configData.chartOption.normalBGFlag == 'color'" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.backgroundColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.normalBGFlag == 'img'" label="背景图片">
              <image-gallary @getImg="getNormalBg"></image-gallary>
            </el-form-item>

          </el-collapse-item>

          <el-collapse-item title="选中样式" name="4">

            <el-form-item v-if="configData.chartOption.selectedFontSize!==undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.selectedFontSize" :min="1" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedLetterSpacing!==undefined" label="字体间距">
              <el-slider v-model="configData.chartOption.selectedLetterSpacing" :min="0" :step="1" show-input></el-slider>
            </el-form-item>

             <el-form-item v-if="configData.chartOption.selectedFontColor!==undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.selectedFontColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedFontFamily!==undefined" label="字体名称">
              <el-select v-model="configData.chartOption.selectedFontFamily" placeholder="请选择">
                <el-option
                  v-for="(item, index) in fontFamilys"
                  :key="index"
                  :label="item"
                  :value="item">
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedFontWeight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.selectedFontWeight" placeholder="请选择">
                <el-option
                  v-for="(item,index) in fontWeight"
                  :key="index"
                  :label="item"
                  :value="item">
                </el-option>
              </el-select>
            </el-form-item>

            <!-- <el-form-item v-if="configData.chartOption.selectedBorderWidth!==undefined" label="边框宽度">
              <el-slider v-model="configData.chartOption.selectedBorderWidth" :min="0" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedBorderColor!==undefined" label="边框颜色">
              <el-color-picker v-model="configData.chartOption.selectedBorderColor" show-alpha></el-color-picker>
            </el-form-item> -->
            <el-form-item label="背景设置">
              <el-radio-group v-model="configData.chartOption.selectedBGFlag">
                <el-radio label="color">背景颜色</el-radio>
                <el-radio label="img">背景图片</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedBackgroundColor!==undefined && configData.chartOption.selectedBGFlag=='color'" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.selectedBackgroundColor" show-alpha></el-color-picker>
            </el-form-item>
            
            <el-form-item v-if="configData.chartOption.selectedBGFlag == 'img'" label="背景图片">
              <image-gallary @getImg="getSelectedBg"></image-gallary>
            </el-form-item>

          </el-collapse-item>

          <el-collapse-item title="选中方式" name="5">
            <el-form-item  label="选择方式">
                <el-radio-group v-model="configData.chartOption.checkType">
                  <el-radio label="single">单选</el-radio>
                  <el-radio label="multiple">多选</el-radio>
                </el-radio-group>
            </el-form-item>
          </el-collapse-item>


          <el-collapse-item title="绑定组件" name="6">

            <el-form-item v-if="configData.chartOption.name!==undefined" label="参数名称">
              <el-input v-model="configData.chartOption.name" placeholder="请输入参数名称" />
            </el-form-item>

            <el-form-item  label="绑定组件">
              <el-select v-model="configData.chartOption.selectedCharts" multiple placeholder="请选择" filterable @change="bindCharts">
                <el-option
                    v-for="item in chartList"
                    :key="item.customId"
                    :label="item.layerName"
                    :value="item.customId">
                  </el-option>
              </el-select>
            </el-form-item>

          </el-collapse-item>

          <el-collapse-item title="自动轮播" name="7">

          <el-form-item label="是否自动轮播">
            <el-switch v-model="configData.chartOption.isRotation" />
          </el-form-item>

          <el-form-item label="间隔时长">
            <el-input-number v-model="configData.chartOption.dur" controls-position="right"  :step="1000"></el-input-number>
          </el-form-item>

          </el-collapse-item>
          <el-collapse-item title="动画" name="8">
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

import DataSourceConfig from './DataConfig/DataSourceConfig'
import sourceConfig from '../mixins/sourceConfig.js'
import ChartInteract from "./interact/ChartInteract";
export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
    ChartInteract
  },
  data() {
    return {
      fontFamilys:this.fontFamilys,
      fontWeight:['normal','bold','bolder','lighter'],
      activeNames: ['1'],
      chartList: this.drawingList,
      currentTab: 'field',
      animateOptions,
      controlKey: "",
      dataList: [],
    }
  },
  //页面加载完执行
  mounted() {
    //获取可联动组件列表

    this.dataList = this.themeForm.globalData;
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;
  },
  computed: {
  
  },

  methods: {
    bindDataList(val) {
      this.$set(this.configData.chartOption, "dataList", val);
    },
    bindCharts(val){
       this.$set(this.configData.chartOption, 'selectedCharts', val);
    },
    getRemote(val){
      // console.log(val)
      this.$set(this.configData.chartOption, 'remote', val);
    },
    //复制key的方法
    docopy(text){
      this.$copyText(text).then(msg => {
        this.msgSuccess('复制成功')
      }).catch(err =>{
        console.log("copy.err",err)
        this.msgError('复制失败')
      })
    },
    createKey(){
      function S4() {
        return (((1+Math.random())*0x10000)|0).toString(16).substring(1);
      }
      let key = (S4()+S4()+S4()+S4()+S4()+S4()+S4()+S4());
      this.$set(this.configData.chartOption, 'sameRemoteKey', key);
    },
    changeControlKey(){
      this.$set(this.configData.chartOption, 'controlKey', this.controlKey);
    },
    getNormalBg(val){
       this.$set(this.configData.chartOption, 'normalBGImage',  val);
    },
    getSelectedBg(val){
       this.$set(this.configData.chartOption, 'selectedBGImage', val);
    },
  }
}
</script>

<style lang="scss" scoped>
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
::v-deep .select-item .el-input--medium .el-input__inner {
    height: 32px;
    width: 90px;
}
.el-form-item__label{
  width:30px
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
::v-deep .center-tabs .el-tabs__item {
    width: 25%;
    text-align: center;
}
</style>