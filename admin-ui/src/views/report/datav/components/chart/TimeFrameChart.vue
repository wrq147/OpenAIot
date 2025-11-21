<template>
  <div :class="animate" :style="{ height: height, width: width}" :id="chartOption.bindingDiv">
    <div class="demo-input-suffix" ref="text">
      <label class="el-form-item__label" for="" :style="normalTextStyle">{{this.chartOption.context}}</label>
      <div class="el-form-item__content">

        <el-date-picker v-model="value" :style="timeStyle" :type="this.chartOption.type" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期"  value-format="yyyy-MM-dd HH:mm:ss"  
         @change="renderChart($event)"> </el-date-picker>
      </div>
      
    </div>
  </div>
</template>

<script>
import '../../animate/animate.css'
import { bindChart} from "../../util/LinkageChart";
import dataChart from '../mixins/dataChart.js'
export default {
  mixins: [dataChart],
  props: {
    className: {
      type: String,
      default: "chart",
    },
    width: {
      type: String,
      default: "100%",
    },
    height: {
      type: String,
      default: "100%",
    },
    customId:{
      type: Number 
    },
    drawingList:{
      type: Array
    }
  },
  data() {
    return {
      value: [],
      animate: this.className
    };
  },
  watch: {
    width() {
    },
    height() {    
    },
    className: {
      handler(value) {
        this.animate = value;
      }
    },
    $route: {
      handler: function (route) {
        // console.log(route,);
        if(route.query){
          let pars = route.query
          if(this.chartOption.params&&pars[this.chartOption.params]){
            this.input=pars[this.chartOption.params]
          }
        }
       
      },
      immediate: true,
    },
  },
  mounted() {
    // this.valUpdate = this.setChartVal;
    if(this.chartOption.params){
      let pars = this.$route.query;
      if(pars[this.chartOption.params]){
        this.value=pars[this.chartOption.params]
      }
      
    }
    this.valUpdate = this.setChartVal;
    if (this.chartOption.getVal == null) {
      this.chartOption.getVal = () => {
        return this.value;
      };
    }
  },
  beforeDestroy() {
  },
  computed:{
    normalTextStyle(){
      const style = {color:this.chartOption.fontColor,fontSize:this.chartOption.fontSize + "px",fontFamily:this.chartOption.fontFamily,fontWeight:this.chartOption.fontWeight}
      return style
    } ,
    timeStyle(){
      const style = {width:this.chartOption.width+"px"}
      return style
    } 
  },
  methods: {
    setChartVal(result) {
      if(this.chartOption.params){
        let pars = this.$route.query;
        if(pars[this.chartOption.params]){
          this.value=pars[this.chartOption.params]
        }
      }else{
        this.value=result
      }
    },
    renderChart(val){
      this.$emit("onChange", val);
      //更新数据源
      if (this.chartOption.dataList != null) {
        this.chartOption.dataList.forEach(element => {
          this.refreshData(element)
        });
      }
      //获取与input绑定的图表，传递参数，重新渲染
      // let bindList = this.chartOption.bindList;
      // let name = this.chartOption.name;
      
      // if(bindList.length > 0){

      //   bindChart(name, this.value, bindList,this.drawingList)
        
        // let exclusion = ["input","timeframe", "select", "cascade","tab","textCheckBox","timeline"];
        // let renderCharts = this.drawingList.filter(item => { 
        //   return bindList.indexOf(item.customId) > -1 && exclusion.indexOf(item.type) == -1
        // })
        // //遍历绑定组件
        // renderCharts.forEach(item => {

        //   //获取组件参数
        //   let requestParameters = item.chartOption.requestParameters;

        //   //如果已经包含该名称的参数则替换
        //   if(requestParameters != "" && requestParameters.indexOf(name) != -1){
        //     //拆分成数组
        //     let paramArr = requestParameters.split('&');
        //     //获取到包含该名称的数组项
        //     for(let i in paramArr){
        //        if(paramArr[i].indexOf(name) != -1){
        //         //替换位=为当前内容
        //         paramArr.splice(i,1,name + "=" + this.value)
               
        //       }
        //     }
        //     //将数组重新按照&符号拼接为字符串
        //     requestParameters = '&' + paramArr.join("&")
            
        //   }else{
        //     //如果为新名称参数直接拼在结尾
        //     requestParameters += "&"+this.chartOption.name + "=" + this.value;
        //   }
        //   //判断参数是否已&符号开始，是则删除该符号
        //   if(requestParameters.indexOf('&') == 0){
        //     item.chartOption.requestParameters = requestParameters.substring(1, requestParameters.length);
        //   }else{
        //     //给绑定组件重新赋值参数渲染组件
        //     item.chartOption.requestParameters = requestParameters
        //   }
          
        // });

      // }
 
    },
   
  }
};
</script>

<style lang="scss" scoped>
.demo-input-suffix{
  display: flex;
  float: left;
}
</style>
