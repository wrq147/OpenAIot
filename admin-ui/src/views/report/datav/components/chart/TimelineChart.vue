<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
    ref="chartDiv"
  ></div>
</template>

<script>
import echarts from "echarts";
require("echarts/theme/macarons"); // echarts theme
import resize from "@/views/dashboard/mixins/resize";
import "../../animate/animate.css";
import dataChart from '../mixins/dataChart.js'
import {stringArrayDataHandle} from '../../util/commonChartChange'
export default {
  mixins: [resize,dataChart],
  props: {
    className: {
      type: String,
      default: "chart"
    },
    width: {
      type: String,
      default: "100%"
    },
    height: {
      type: String,
      default: "100%"
    },
    customId: {
      type: Number
    },
    drawingList: {
      type: Array
    }
  },
  data() {
    return {
      chart: null,
      value: "",
      animate: this.className
    };
  },
  watch: {
    width() {
      this.$nextTick(() => {
        if (this.chart != null) {
          this.chart.resize();
        }
      });
    },
    height() {
      this.$nextTick(() => {
        if (this.chart != null) {
          this.chart.resize();
        }
      });
    },
    "chartOption.theme": {
      handler() {
        if (this.chart != null) {
          this.chart.dispose();
          this.chart = null;
        }
      }
    },
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
    if (!this.chart) {
      return;
    }
    this.chart.dispose();
    this.chart = null;
  },
  computed: {},
  methods: {
    setChartVal(resData,rowGlobal) {
      let result=[]
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        result =stringArrayDataHandle(rowGlobal,this.chartOption)?stringArrayDataHandle(rowGlobal,this.chartOption):[]
      }else{
        result = this.chartOption.staticDataValue;
      }
      if (this.chart == null) {
        echarts.registerTheme("customTheme", this.chartOption.theme);
        this.chart = echarts.init(this.$el, "customTheme");
      }
      let dataOption = this.dataOption;

      let option = {
        baseOption: {
          timeline: {
            left: "10%",
            right: "10%",
            top: "40%",
            width: "80%",
            axisType: "category",
            // realtime: false,
            // loop: false,
            autoPlay: dataOption.autoPlay,
            // currentIndex: 2,
            playInterval: dataOption.playInterval,
            data: result,

            label: {
              normal: {
                textStyle: {
                  color: dataOption.fontColor,
                  fontSize: dataOption.fontSize
                },
                formatter: function(s) {
                  return s + dataOption.suffix;
                }
              },
              emphasis: {
                textStyle: {
                  color: dataOption.checkFontColor
                }
              }
            },
            symbolSize: dataOption.symbolSize, //圆点大小
            lineStyle: {
              color: dataOption.lineColor //线颜色
            },
            itemStyle: {
              emphasis: {
                color: dataOption.pointColor
              }
            },
            checkpointStyle: {
              color: dataOption.checkPointColor,
              borderColor: dataOption.borderColor, //选中圆点边框颜色
              borderWidth: dataOption.borderWidth //选中圆点边框宽度
            },
            controlStyle: {
              showNextBtn: dataOption.showNextBtn, //下一个按钮
              showPrevBtn: dataOption.showPrevBtn, //上一个按钮
              showPlayBtn: dataOption.showPlayBtn,
              normal: {
                color: dataOption.buttonColor,
                borderColor: dataOption.buttonColor
              },
              emphasis: {
                color: dataOption.checkButtonColor,
                borderColor: dataOption.checkButtonColor
              }
            }
          }
        }
      };
     
      this.chart.clear();
      this.chart.setOption(option, true);

      let drawingList = this.drawingList;

      if(this.chart != undefined){
        //监听切换数据
        let that = this;
        that.chart.on("timelinechanged", function(timeLineIndex) {
          let val = dataOption.staticDataValue[timeLineIndex.currentIndex];
          //获取绑定的图表，传递参数，重新渲染
          let bindList = dataOption.bindList;
          let name = dataOption.aggrName;
  
          if (bindList != null && bindList.length > 0) {
            let exclusion = [
              "input",
              "timeframe",
              "select",
              "cascade",
              "tab",
              "textCheckBox",
              "timeline",
              "group"
            ];
            let renderCharts = drawingList.filter(item => {
              return (
                bindList.indexOf(item.customId) > -1 &&
                exclusion.indexOf(item.type) == -1
              );
            });
            //遍历绑定组件
            renderCharts.forEach(item => {
              //获取组件参数
              let requestParameters = item.chartOption.requestParameters;
  
              //如果已经包含该名称的参数则替换
              if (
                requestParameters != "" &&
                requestParameters.indexOf(name) != -1
              ) {
                //拆分成数组
                let paramArr = requestParameters.split("&");
                if (paramArr.length > 1) {
                  paramArr.pop();
                }
                //获取到包含该名称的数组项
                for (let i in paramArr) {
                  if (paramArr[i].indexOf(name) != -1) {
                    //替换位=为
                    当前内容;
                    paramArr.splice(i, 1, name + "=" + val);
                  }
                }
                //将数组重新按照&符号拼接为字符串
                requestParameters = "&" + paramArr.join("&");
              } else {
                if (requestParameters != "") {
                  let paramArr = requestParameters.split("&");
                  if (paramArr.length > 1) {
                    paramArr.pop();
                  }
                  requestParameters = "&" + paramArr.join("&");
                }
                //如果为新名称参数直接拼在结尾
                requestParameters += "&" + name + "=" + val;
              }
              //判断参数是否已&符号开始，是则删除该符号
              if (requestParameters.indexOf("&") == 0) {
                item.chartOption.requestParameters =
                  requestParameters.substring(1, requestParameters.length) +
                  "&timestamp=" +
                  new Date().getTime();
              } else {
                //给绑定组件重新赋值参数渲染组件
                item.chartOption.requestParameters =
                  requestParameters + "&timestamp=" + new Date().getTime();
              }
            });
          }

          that.chart.off("timelinechanged");
        });
      }

    },
   
  }
};
</script>
