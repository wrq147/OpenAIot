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
                <el-form v-if="currentTab==='field'" size="small" label-width="90px">
                    <el-collapse v-model="activeName" accordion>
                        <el-collapse-item title="图层" name="0">
                        <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
                            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
                        </el-form-item>

                        <el-form-item v-if="configData.chartOption.title.text!==undefined" label="标题">
                            <el-input v-model="configData.chartOption.title.text" placeholder="请输入标题" />
                        </el-form-item>

                        <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
                            <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
                        </el-form-item>              
                        </el-collapse-item>
                        <el-collapse-item title="x轴设置" name="1">
                            <el-form-item v-if="configData.chartOption.xAxis.name!==undefined" label="坐标轴名称">
                                <el-input v-model="configData.chartOption.xAxis.name" placeholder="请输入X轴名称" />
                            </el-form-item>

                            <el-form-item label="坐标轴名称位置">
                                <el-select v-model="configData.chartOption.xAxis.nameLocation" placeholder="请选择">
                                <el-option
                                    v-for="item in nameLocation"
                                    :key="item.key"
                                    :label="item.name"
                                    :value="item.key">
                                </el-option>
                                </el-select>
                            </el-form-item>
                            
                            <el-form-item label="坐标轴名称字号">
                                 <el-input-number v-model="configData.chartOption.xAxis.nameTextStyle.fontSize" controls-position="right"  :step="1"></el-input-number>
                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.xAxis.show!==undefined" label="显示X轴">
                                <el-switch v-model="configData.chartOption.xAxis.show" />
                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.xAxis.show==true" label="显示轴线">
                                <el-switch v-model="configData.chartOption.xAxis.axisLine.show"/>
                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.xAxis.axisLine.show==true && configData.chartOption.xAxis.show==true" label="轴线粗细">
                                <el-input-number v-model="configData.chartOption.xAxis.axisLine.lineStyle.width"/>
                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.xAxis.axisLine.show==true && configData.chartOption.xAxis.show==true" label="轴线颜色">
                                <el-color-picker v-model="configData.chartOption.xAxis.axisLine.lineStyle.color" show-alpha></el-color-picker>
                            </el-form-item>
                            
                            <el-form-item v-if="configData.chartOption.xAxis.show==true" label="显示标签">
                                <el-switch v-model="configData.chartOption.xAxis.axisLabel.show"/>
                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.xAxis.axisLabel.show && configData.chartOption.xAxis.show==true" label="标签字号">
                                <el-input-number v-model="configData.chartOption.xAxis.axisLabel.fontSize" controls-position="right"  :step="1"></el-input-number>

                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.xAxis.axisLabel.show && configData.chartOption.xAxis.show==true" label="标签颜色">
                                <el-color-picker v-model="configData.chartOption.xAxis.axisLabel.color" show-alpha></el-color-picker>
                            </el-form-item>
                        </el-collapse-item>
                        <el-collapse-item title="y轴设置" name="2">
                            <el-form-item v-if="configData.chartOption.yAxis.name!==undefined" label="坐标轴名称">
                                <el-input v-model="configData.chartOption.yAxis.name" placeholder="请输入y轴名称" />
                            </el-form-item>

                            <el-form-item label="坐标轴名称位置">
                                <el-select v-model="configData.chartOption.yAxis.nameLocation" placeholder="请选择">
                                <el-option
                                    v-for="item in nameLocation"
                                    :key="item.key"
                                    :label="item.name"
                                    :value="item.key">
                                </el-option>
                                </el-select>
                            </el-form-item>

                            <el-form-item label="坐标轴字号">
                                <el-input-number v-model="configData.chartOption.yAxis.nameTextStyle.fontSize" controls-position="right"  :step="1"></el-input-number>
                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.yAxis.show!==undefined" label="显示y轴">
                                <el-switch v-model="configData.chartOption.yAxis.show" />
                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.yAxis.show==true" label="显示轴线">
                                <el-switch v-model="configData.chartOption.yAxis.axisLine.show"/>
                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.yAxis.axisLine.show==true && configData.chartOption.yAxis.show==true" label="轴线粗细">
                                <el-input-number v-model="configData.chartOption.yAxis.axisLine.lineStyle.width"/>
                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.yAxis.axisLine.show==true && configData.chartOption.yAxis.show==true" label="轴线颜色">
                                <el-color-picker v-model="configData.chartOption.yAxis.axisLine.lineStyle.color" show-alpha></el-color-picker>
                            </el-form-item>
                            
                            <el-form-item v-if="configData.chartOption.yAxis.show==true" label="显示标签">
                                <el-switch v-model="configData.chartOption.yAxis.axisLabel.show"/>
                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.yAxis.axisLabel.show && configData.chartOption.yAxis.show==true" label="标签字号">
                                <el-input-number v-model="configData.chartOption.yAxis.axisLabel.fontSize" controls-position="right"  :step="1"></el-input-number>

                            </el-form-item>

                            <el-form-item v-if="configData.chartOption.yAxis.axisLabel.show && configData.chartOption.yAxis.show==true" label="标签颜色">
                                <el-color-picker v-model="configData.chartOption.yAxis.axisLabel.color" show-alpha></el-color-picker>
                            </el-form-item>
                        </el-collapse-item>
 
                        <el-collapse-item title="图形设置" name="4">
                            <el-form-item label="显示占比">
                                <el-switch v-model="configData.chartOption.showAll"/>
                            </el-form-item>
                            <el-form-item label="最大值">
                                <el-input-number v-model="configData.chartOption.max"/>
                            </el-form-item>
                            <el-form-item v-if="configData.chartOption.showAll" label="占比背景颜色">
                                <el-color-picker v-model="configData.chartOption.shadowColor" show-alpha/>
                            </el-form-item>

                            <el-form-item label="左边距">
                               
                                <el-input-number v-model="configData.chartOption.gridLeft" controls-position="right"  :step="1"></el-input-number>

                            </el-form-item>

                            <el-form-item label="图形宽度">
                                <el-input-number v-model="configData.chartOption.threeDBarWidth" controls-position="right"  :step="1"></el-input-number>

                            </el-form-item>
                            <el-form-item label="图形高度">
                                <el-input-number v-model="configData.chartOption.threeDBarHeight" controls-position="right"  :step="1"></el-input-number>

                            </el-form-item>
                            <el-form-item v-if="configData.chartOption.processColor!==undefined" label="柱体颜色">
                            </el-form-item>
                            <div style="margin-bottom:20px">
                                <div style="display:flex">
                                    <el-form-item label="颜色(始)"></el-form-item>
                                    <el-form-item label="颜色(终)" style="margin-left: 60px;"></el-form-item>
                                </div>
                                <div class="select-item">
                                    <el-color-picker v-model="configData.chartOption.processColor[0]" show-alpha style="margin-left:30px"></el-color-picker>
                                    <div :style="gradientColor(configData.chartOption.processColor)"></div>
                                    <el-color-picker v-model="configData.chartOption.processColor[1]" show-alpha style="margin-left:20px"></el-color-picker>
                                </div>
                            </div>

                        </el-collapse-item>
                        <el-collapse-item title="数值标签" name="5">

                            <el-form-item label="字体">
                                <el-select v-model="configData.chartOption.series[2].label.fontFamily" placeholder="请选择">
                                <el-option
                                    v-for="(item,index) in fontFamilys"
                                    :key="index"
                                    :label="item"
                                    :value="item">
                                </el-option>
                                </el-select>
                            </el-form-item>

                            <el-form-item label="字号">
                                <el-input-number v-model="configData.chartOption.series[2].label.fontSize" controls-position="right"  :step="1"></el-input-number>

                            </el-form-item>

                            <el-form-item label="文字颜色">
                                <el-color-picker v-model="configData.chartOption.series[2].label.color" show-alpha></el-color-picker>
                            </el-form-item>

                            <el-form-item label="水平位置">
                                <el-input-number v-model="configData.chartOption.series[2].label.position[0]"/>
                            </el-form-item>

                            <el-form-item label="垂直位置">
                              <el-input-number v-model="configData.chartOption.series[2].label.position[1]"/>
                            </el-form-item>

                            <el-form-item label="后缀">
                                <el-input v-model="configData.chartOption.labelText" placeholder="请输入后缀" />
                            </el-form-item>
                        </el-collapse-item>
                        <el-collapse-item title="动画设置" name="6">
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
                    </el-collapse >
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

                <!-- 下钻 -->
                <el-form-item  label="是否开启图表下钻">
                    <el-switch v-model="configData.chartOption.isDrillDown" @change="isDrillDownChange"/>
                </el-form-item>

                <el-form-item v-show="configData.chartOption.isDrillDown===true"  label="下钻后展示的图表类型">
                    <el-select v-model="configData.chartOption.drillDownChartType" clearable filterable placeholder="请选择" @change="drillDownChartChange">
                        <el-option
                        v-for="item in drillDownOptions"
                        :key="item.chartType"
                        :label="item.layerName"
                        :value="item.chartType">
                        </el-option>
                    </el-select>
                </el-form-item>

                <el-form-item v-show="configData.chartOption.isDrillDown===true" label="弹窗背景颜色">
                    <el-color-picker v-model="drillBgColor" show-alpha @change="drillDownColorChange"/>
                </el-form-item>

                <el-form-item  v-show="configData.chartOption.isDrillDown===true" label="*参数说明">
                    <div class="el-form-item__content">
                    <span style="word-wrap: break-word;"><i style="color:#F00;">drillParam=参数值</i></span>
                    </div>
                </el-form-item> 

                <drill-down-config :drillDownDialogFlag.sync="drillDownDialogFlag" :drillDownDialogData="configData.chartOption.drillDownChartOption"/>
                <el-form-item v-show="configData.chartOption.isDrillDown===true"  label="">
                    <el-button type="primary" @click="DrillDownClick">前去设置</el-button>
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
    DataSourceConfig
  },
  data() {
    return {
      currentTab: 'field',
      animateOptions,
      chartList: this.drawingList,
      activeName:['0'],
      fontFamilys:this.fontFamilys,
      labelPosition:['top','left','right','bottom','inside','insideLeft','insideRight','insideTop','insideBottom','insideTopLeft','insideBottomLeft','insideTopRight','insideBottomRight'],
      nameLocation:[{key:'start',name:"起点"},{key:'center',name:"中间"},{key:'end',name:"末尾"}],
      legendOrient:[{key:"horizontal",name:"水平"},{key:"vertical",name:"垂直"}],
      splitLineType:[{key:'solid', name:"实线"},{key:'dashed',name:"短横虚线"},{key:'dotted',name:'点状虚线'}],
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
    handleBarOrient(val){
        if(val == "纵向"){
            this.$set(this.configData.chartOption.xAxis,'type',"category")
            this.$set(this.configData.chartOption.yAxis,'type',"value")
            this.$set(this.configData.chartOption.yAxis,'inverse',false)
            this.$set(this.configData.chartOption.series[0].label,'position','top')
        }
        if(val == "横向"){
            this.$set(this.configData.chartOption.xAxis,'type',"value")
            this.$set(this.configData.chartOption.yAxis,'type',"category")
            this.$set(this.configData.chartOption.yAxis,'inverse',true)
            this.$set(this.configData.chartOption.series[0].label,'position','right')
        }
    },
    gradientColor(item){
        let style = {width:'100px',height:'30px',marginLeft:'20px',background:`-webkit-linear-gradient(left, ${item[0]},${item[1]})`}
        return style
    },
    getImgList(imageList){
      this.$set(this.configData.chartOption,"imageList",imageList)
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