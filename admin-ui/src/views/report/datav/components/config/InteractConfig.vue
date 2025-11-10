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
            <el-collapse-item title="文字样式" name="2">

              <el-form-item v-if="configData.chartOption.textStyle.content!==undefined" label="文字内容">
                <el-input v-model="configData.chartOption.textStyle.content" />
              </el-form-item>
              
              <el-form-item v-if="configData.chartOption.textStyle.fontSize!==undefined" label="字体大小">
                  <el-slider v-model="configData.chartOption.textStyle.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.textStyle.fontFamily!==undefined" label="字体名称">
                <el-select v-model="configData.chartOption.textStyle.fontFamily" placeholder="请选择">
                  <el-option
                    v-for="(item,index) in fontFamilys"
                    :key="index"
                    :label="item"
                    :value="item">
                  </el-option>
                </el-select>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.textStyle.fontColor!==undefined" label="字体颜色">
                <el-color-picker v-model="configData.chartOption.textStyle.fontColor" show-alpha></el-color-picker>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.textStyle.letterSpacing!==undefined" label="字体间距">
                  <el-slider v-model="configData.chartOption.textStyle.letterSpacing" :min="0" :max="200" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.textStyle.lineHeight!==undefined" label="字体行高">
                <el-input-number v-model="configData.chartOption.textStyle.lineHeight" controls-position="right"  :step="1"></el-input-number>
              </el-form-item>                    

              <el-form-item v-if="configData.chartOption.textStyle.fontWeight!==undefined" label="文字粗细">
                <el-select v-model="configData.chartOption.textStyle.fontWeight" placeholder="请选择">
                  <el-option
                    v-for="(item,index) in fontWeights"
                    :key="index"
                    :label="item"
                    :value="item">
                  </el-option>
                </el-select>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.textStyle.textAlign!==undefined" label="对齐方式">
                <el-select v-model="configData.chartOption.textStyle.textAlign" placeholder="请选择">
                  <el-option label="居左" value="left"></el-option>
                  <el-option label="居中" value="center"></el-option>
                  <el-option label="居右" value="right"></el-option>
                </el-select>
              </el-form-item>
            </el-collapse-item>
            
            <el-collapse-item title="背景设置" name="3">
              
              <el-form-item label="背景">
                <el-radio-group v-model="configData.chartOption.background.type">
                  <el-radio label="color">背景颜色</el-radio>
                  <el-radio label="img">背景图片</el-radio>
                </el-radio-group>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.background.type == 'color'" label="背景颜色">
                <el-color-picker v-model="configData.chartOption.background.backgroundColor" show-alpha></el-color-picker>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.background.type == 'img'" label="背景图片">
                <image-gallary @getImg="getSelectedBg"></image-gallary>
              </el-form-item>
            </el-collapse-item>

            <el-collapse-item title="动画" name="4">

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
        <el-form v-if="currentTab === 'interaction'" size="small" label-width="90px">
            <el-collapse  v-model="activeInteract" accordion>
              <el-collapse-item>
                <template slot="title">
                  <div style="width: 100%;display:flex;justify-content: space-between;">
                      <div>自定义事件</div>
                      <div>
                        <i class="el-icon-circle-plus-outline active" style="font-size:16px" @click.stop="addEvent"></i>
                        <i class="el-icon-delete active" style="margin-left:3px;margin-right:15px;font-size:16px" @click.stop="delEvent"></i>
                      </div>
                  </div>
                </template>

                <div>

                  <el-tabs  @tab-click="handleClick" v-model="tabName">

                      <el-tab-pane  v-for="(item) in events" :key="item.key" :label="item.name" :name="item.name"></el-tab-pane>
                    
                    </el-tabs>
                </div>

                <div v-if="events.length > 0">


                 <el-form-item label="事件类型">
                    <el-select v-model="events[eventIndex].type" placeholder="请选择">
                      <el-option label="鼠标点击" value="click"></el-option>
                      <el-option label="当请求完成或数据变化时" value="change"></el-option>
                    </el-select>
                  </el-form-item>

                  <el-form-item label="条件" v-if="events[eventIndex].type == 'change'">

                    <el-button type="primary" @click="editCondition">条件配置</el-button>

                  </el-form-item>

                  <el-form-item label="组件">
                    <el-radio-group v-model="events[eventIndex].chartType">
                      <el-radio label="popup">弹窗</el-radio>
                      <el-radio label="chart">组件</el-radio>
                    </el-radio-group>
                  </el-form-item>
                  
                  <el-form-item label="绑定组件" v-if="events[eventIndex].chartType == 'chart'">
                    <el-select v-model="events[eventIndex].chart" placeholder="请选择">
                      <el-option
                          v-for="item in chartList"
                          :key="item.customId"
                          :label="item.layerName"
                          :value="item.customId">
                        </el-option>
                    </el-select>

                  </el-form-item>

                  
                  <el-form-item label="动作" v-if="events[eventIndex].chartType == 'chart'">
                    <el-select v-model="events[eventIndex].act" placeholder="请选择">
                      <el-option label="显示" value="show"></el-option>
                      <el-option label="隐藏" value="hide"></el-option>
                      <el-option label="更新组件配置" value="change"></el-option>
                    </el-select>
                  </el-form-item>

                  <el-form-item label="组件隐藏" v-if="events[eventIndex].chartType == 'chart'">

                   <el-checkbox v-model="events[eventIndex].chartHide">默认隐藏</el-checkbox>

                  </el-form-item>

                  <el-form-item label="组件配置" v-if="events[eventIndex].chart != undefined && events[eventIndex].act == 'change'">

                    <el-button type="primary" @click="editChartOption">编辑组件配置</el-button>

                  </el-form-item>

                </div>


              </el-collapse-item>

              <!-- <el-collapse-item title="远程控制">
                <el-form-item  label="是否开启图表远程控制">
                    <el-switch v-model="configData.chartOption.isRemote" />
                </el-form-item>

                <el-form-item v-if="configData.chartOption.isRemote === true" label="生成键值">
                  <el-button type="primary" @click="createKey()">生成键值</el-button>
                </el-form-item>

                <el-form-item v-if="configData.chartOption.isRemote === true" label="键值">
                  <span style="word-wrap: break-word;"> {{configData.chartOption.remoteKey}}</span>
                  <el-button type="primary" @click="docopy(configData.chartOption.remoteKey)" style="margin-left:10px" v-show="configData.chartOption.remoteKey !== undefined">复制键值</el-button>
                </el-form-item>

                <el-form-item  label="是否接受图表远程控制">
                    <el-switch v-model="configData.chartOption.isControl" />
                </el-form-item>

                <el-form-item v-if="configData.chartOption.isControl === true" label="绑定键值">
                    <el-input v-model="controlKey" placeholder="请输入绑定键值" @blur.prevent="changeControlKey()"/>
                </el-form-item>

              </el-collapse-item> -->
            </el-collapse>
            
          

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

    <el-dialog
      title="自定义条件"
      v-if="conditionOpen"
      :visible.sync="conditionOpen"
      width="1000px"
      hight="900px"
      append-to-body
      :close-on-click-modal="false"
    >
      <data-editor
        :key="aceKey"
        @submitData="submitCondition"
        @cancelData="cancelCondition"
        :customData="events[eventIndex].condition"
      ></data-editor>


    </el-dialog> 

    <el-dialog
      title="自定义组件配置"
      v-if="chartOpen"
      :visible.sync="chartOpen"
      width="1000px"
      hight="900px"
      append-to-body
      :close-on-click-modal="false"
    >
      <data-editor
        @submitData="submitChartOption"
        @cancelData="cancelChartOption"
        :customData="events[eventIndex].customConfig"
      ></data-editor>


    </el-dialog> 

  </div>
</template>

<script>

import {animateOptions} from '../../animate/animate'
import { getLinkChart} from "../../util/LinkageChart";
import DataEditor from "./runcode/DataEditor";
import DataSourceConfig from './DataConfig/DataSourceConfig'
import {echartsCharts} from '../../ComponentsConfig'
import sourceConfig from '../mixins/sourceConfig.js'

export default {
  mixins: [sourceConfig],
  components: {
    DataEditor,
    DataSourceConfig
  },
  data() {
    return {
      activeInteract:['1'],
      activeNames: ['1'],
      tabName:"事件1",
      tabNameList:["事件1"],
      fontFamilys:this.fontFamilys,
      fontWeights:['normal','bold','bolder','lighter'],
      currentTab: 'field',
      animateOptions,
      echartsCharts,
      events: this.costomData.chartOption.events,
      eventIndex:0,
      controlKey: "",
      eventKey:this.costomData.chartOption.eventKey,
      conditionOpen:false,
      chartList:null,
      chartOpen:false,
    }
  },
  //页面加载完执行
  mounted() {
    //获取可联动组件列表
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;
  },
  computed: {
    aceKey(){
      return "a"+new Date()
    },
  },
  watch: {
    events:{
      deep: true,
      handler(newVal) {
         this.$set(this.configData.chartOption, 'events', newVal);
      }
    }
  },
  methods: {

    getSelectedBg(val){
       this.$set(this.configData.chartOption.background, 'backgroundImg', val);
    },
    handleClick(tab) {
        this.eventIndex = tab.index
    },
    addEvent(){
      this.eventKey++;
      let index = this.eventKey;
      this.$set(this.configData.chartOption, 'eventKey',index);

      this.events.push({name:"事件"+index,key:index,type:"",chartType:"",chart:null,act:"",condition:'(data) => { \n'+
                        '  return true\n'+
                    '}',customConfig:'',chartHide:false})

      this.tabName = "事件"+index;
      this.tabNameList.push(this.tabName)

      this.eventIndex = this.events.length -1;
    },
    delEvent(){
      let newCols = this.events;
      newCols.splice(this.eventIndex, 1);
      this.$set(this.configData.chartOption, 'events',JSON.parse(JSON.stringify( newCols)));
      this.tabNameList.splice(this.eventIndex, 1);
      
     
      //删除第一个事件
      if(this.eventIndex == 0){
          this.tabName =  this.tabNameList[0];
          this.eventIndex = 0;
      }else{      
          this.tabName =  this.tabNameList[this.eventIndex-1];
          this.eventIndex = this.eventIndex - 1;
      }
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
      this.$set(this.configData.chartOption, 'remoteKey', key);
    },
    changeControlKey(){
      this.$set(this.configData.chartOption, 'controlKey', this.controlKey);
    },
    editCondition(){
      this.conditionOpen = true;
    },
    cancelCondition(){
      this.conditionOpen = false;
    },
    submitCondition(data){
      this.events[this.eventIndex].condition = data;
      this.$set(this.configData.chartOption, 'events', this.events);
      this.conditionOpen = false;
    },
    editChartOption(){

      let chart = this.chartList.find(item => item.customId == this.events[this.eventIndex].chart);

      if(chart != null && this.events[this.eventIndex].customConfig == ''){

        //echarts图执行代码
        if(this.echartsCharts.indexOf(chart.chartType) > -1){
           this.events[this.eventIndex].customConfig = '(option,result,chartDiv)=>{ \n'+
              '   return option; \n'+
          '}';
        }else{
          this.events[this.eventIndex].customConfig = '(result,chartDiv)=>{ \n'+
              '   return result; \n'+
          '}'
        }

      }
      this.chartOpen = true;

    },
    cancelChartOption(){
      this.chartOpen = false;
    },
    submitChartOption(data){
 
      this.events[this.eventIndex].customConfig = data;
      this.$set(this.configData.chartOption, 'events', this.events);
      this.chartOpen = false;
    },

  }
}
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
    width: 25%;
    text-align: center;
}

.active:hover{
  color: deepskyblue;
}
</style>