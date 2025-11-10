<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="90px">
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
                      <!-- <el-radio label="popup">弹窗</el-radio> -->
                      <el-radio label="chart">组件</el-radio>
                    </el-radio-group>
                  </el-form-item>
                  <el-form-item label="绑定组件" v-if="events[eventIndex].chartType == 'chart'">
                    <el-select clearable v-model="events[eventIndex].chart" placeholder="请选择">
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
            </el-collapse>
        </el-form>
    </el-scrollbar>
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
import { getLinkChart} from "../../../util/LinkageChart";
import {echartsCharts} from '../../../ComponentsConfig'
import DataEditor from "../runcode/DataEditor";
export default {
  props: ["costomData", "themeForm","drawingList"],
  components: {
    DataEditor
  },
  data() {
    return {
      configData: this.costomData,
      activeInteract:['1'],
      tabName:"事件1",
      tabNameList:["事件1"],
      echartsCharts,
      events: this.costomData.chartOption.events,
      eventIndex:0,
      controlKey: "",
      eventKey:this.costomData.chartOption.eventKey,
      conditionOpen:false,
      chartList:null,
      chartOpen:false,
    };
  },
  watch: {
    configData: {
      deep: true,
      handler(newVal, oldVal) {
        this.$emit("costom-change", newVal);
      },
    },
    costomData: {
      deep: true,
      handler(newVal) {
        this.configData = newVal;
      },
    },
    events:{
      deep: true,
      handler(newVal) {
         this.$set(this.configData.chartOption, 'events', newVal);
      }
    }
  },
  //页面加载完执行
  mounted() {
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;
  },
  computed: {
    aceKey(){
      return "a"+new Date()
    },
  },
  methods: {
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
  },
};
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
  width: 33%;
  text-align: center;
}
.dataProduct {
  margin-bottom: 10px;
  line-height: 45px;
  padding: 0 15px;
  background-color: #f5f5f5;
  color: #666;
}
</style>
