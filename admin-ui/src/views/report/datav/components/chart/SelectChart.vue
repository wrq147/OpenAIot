<template>
  <div :class="animate"  :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="text">
    <!-- <div class="demo-input-suffix"> -->
      <label class="el-form-item__label" for="" :style="normalTextStyle">{{this.chartOption.context}}</label>
      <div class="el-form-item__content">
        <el-select v-model="select" placeholder="请选择" clearable filterable :style="selectStyle" :id="selectId" :name="this.chartOption.name" @change="renderChart($event)">
          <el-option
            v-for="item in options"
            :key="item.value"
            :label="item.tab"
            :value="item.value"
          >
          </el-option>
        </el-select>
      </div>
    <!-- </div> -->
  </div>
</template>

<script>
import '../../animate/animate.css'
// import { bindChart} from "../../util/LinkageChart";
import dataChart from '../mixins/dataChart.js'
import {objectArrayDataHandle} from '../../util/commonChartChange'
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
    drawingList: {
      type: Array,
    },
  },
  data() {
    return {
      options: this.chartOption.staticDataValue,
      select: '',
      animate: this.className
    };
  },
  watch: {
    width() {},
    height() {},
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  mounted() {
    this.valUpdate = this.setChartVal;
    if (this.chartOption.getVal == null) {
      this.chartOption.getVal = () => {
        return this.select;
      };
    }
  },
  beforeDestroy() {
  },
  computed: {
    normalTextStyle() {
      const style = {
        color: this.chartOption.fontColor,
        fontSize: this.chartOption.fontSize + "px",
        fontFamily: this.chartOption.fontFamily,
        fontWeight: this.chartOption.fontWeight,
      };
      return style;
    },
    selectId() {
      return "select" + this.chartOption.bindingDiv;
    },
    selectStyle() {
      const style = {
        color: "#fff",
        borderRadius: "5px",
        borderColor: "rgba(255, 255, 255, 0.8)",
        backgroundColor: "rgba(0, 0, 0, 0)",
        fontWeight: this.chartOption.fontWeight,
        fontSize: this.chartOption.fontSize + "px",
        fontFamily: this.chartOption.fontFamily,
        width:this.chartOption.width+"px"
      };
      return style;
    },
  },
  methods: {
    renderChart(val) {
      // //alert("OvO");
      // //获取与input绑定的图表，传递参数，重新渲染
      // let bindList = this.chartOption.selectedCharts;
      // let name = this.chartOption.name;
      // if(bindList.length > 0){

      //   bindChart(name, this.select, bindList,this.drawingList)

      //   // let exclusion = ["input","timeframe", "select", "cascade","tab","textCheckBox","timeline"];
      //   // //console.log("O-O", this.drawingList);
      //   // let renderCharts = this.drawingList.filter(item => { 
      //   //   return bindList.indexOf(item.chartOption.bindingDiv) > -1 && exclusion.indexOf(item.type) == -1
      //   // })
      //   // //遍历绑定组件
      //   // renderCharts.forEach(item => {
          
      //   //   //获取组件参数
      //   //   let requestParameters = item.chartOption.requestParameters;
          
      //   //   //如果已经包含该名称的参数则替换
      //   //   if(requestParameters != "" && requestParameters.indexOf(name) != -1) {
      //   //     //拆分成数组
      //   //     let paramArr = requestParameters.split('&');
      //   //     //获取到包含该名称的数组项
      //   //     for(let i in paramArr) {
      //   //        if(paramArr[i].indexOf(name) != -1) {
      //   //          //替换位=为当前内容
      //   //         paramArr.splice(i, 1, name + "=" + this.select)
               
      //   //       }
      //   //     }
      //   //      //将数组重新按照&符号拼接为字符串
      //   //     requestParameters = '&' + paramArr.join("&")
            
      //   //   } else {
      //   //     //如果为新名称参数直接拼在结尾
      //   //     requestParameters += "&" + this.chartOption.name + "=" + this.select;
      //   //   }
      //   //   //判断参数是否已&符号开始，是则删除该符号
      //   //   if(requestParameters.indexOf('&') == 0) {
      //   //     item.chartOption.requestParameters = requestParameters.substring(1, requestParameters.length);
      //   //   } else {
      //   //     //给绑定组件重新赋值参数渲染组件
      //   //     item.chartOption.requestParameters = requestParameters
      //   //   }
         
      //   // });

      // }
      // //远程控制组件
      // if(this.chartOption.isRemote == true){

      //     if(this.chartOption.remote != undefined && this.chartOption.remote != null){
      //       let remoteData = { ...this.chartOption.remote };
      //       let queryObj = {};
      //       queryObj[this.chartOption.name] = this.select;
      //       remoteData.query = queryObj;
      //       //调用接口
      //       remoteChartApi(remoteData)
      //     }
      // }
      this.$emit("onChange", this.options[val]);
      //更新数据源
      if (this.chartOption.dataList != null) {
        this.chartOption.dataList.forEach(element => {
          this.refreshData(element)
        });
      }
     
    },
    setChartVal(result,rowGlobal) {
      // console.log(result, '=============result')
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        this.options =objectArrayDataHandle(rowGlobal,this.chartOption)?objectArrayDataHandle(rowGlobal,this.chartOption):[]
      }else{
        this.options = this.chartOption.staticDataValue;
      }
      // this.options = result;
    },
 
  },
};
</script>

<style lang="scss" scoped>
// .demo-input-suffix {
//   display: flex;
//   float: left;
// }
</style>
