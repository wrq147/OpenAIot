<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
    ref="chartDiv"
  />
</template>

<script>
import echarts from "echarts";
require("echarts/theme/macarons"); // echarts theme
import resize from "@/views/dashboard/mixins/resize";

import "../../animate/animate.css";

import "echarts/map/js/world";
import "echarts/map/js/china";
import "echarts/map/js/china-contour";
import "echarts/map/js/province/anhui";
import "echarts/map/js/province/aomen";
import "echarts/map/js/province/hebei";
import "echarts/map/js/province/heilongjiang";
import "echarts/map/js/province/henan";
import "echarts/map/js/province/hubei";
import "echarts/map/js/province/hunan";
import "echarts/map/js/province/jiangsu";
import "echarts/map/js/province/jiangxi";
import "echarts/map/js/province/jilin";
import "echarts/map/js/province/liaoning";
import "echarts/map/js/province/neimenggu";
import "echarts/map/js/province/beijing";
import "echarts/map/js/province/ningxia";
import "echarts/map/js/province/qinghai";
import "echarts/map/js/province/shandong";
import "echarts/map/js/province/shanghai";
import "echarts/map/js/province/shanxi";
import "echarts/map/js/province/shanxi1";
import "echarts/map/js/province/sichuan";
import "echarts/map/js/province/taiwan";
import "echarts/map/js/province/tianjin";
import "echarts/map/js/province/xianggang";
import "echarts/map/js/province/chongqing";
import "echarts/map/js/province/xinjiang";
import "echarts/map/js/province/xizang";
import "echarts/map/js/province/yunnan";
import "echarts/map/js/province/zhejiang";
import "echarts/map/js/province/fujian";
import "echarts/map/js/province/gansu";
import "echarts/map/js/province/guangdong";
import "echarts/map/js/province/guangxi";
import "echarts/map/js/province/guizhou";
import "echarts/map/js/province/hainan";


import { getLinkChart } from "../../util/LinkageChart";
import dataChart from '../mixins/dataChart.js'
import VueEvent from "../../VueEvent";

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
    drawingList: {
      type: Array
    }
  },
  data() {
    return {
      chart: null,
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
  methods: {
    setChartVal(result) {
      
      if (this.chart == null) {
        echarts.registerTheme("customTheme", this.chartOption.theme);
        this.chart = echarts.init(this.$el, "customTheme");
      }
      let dataOption = JSON.parse(JSON.stringify(this.dataOption));

      let color = ["#26587D", "#3BB3C4"];
      if (dataOption.isTopic == true) {
        if (this.chartOption.theme != null) {
          color = this.chartOption.theme.color;
        } else {
          color = color;
        }
      } else {
        color = color;
      }
      let mapName = dataOption.mapName;
      let geoCoordMap = {};
      /*获取地图数据*/
      //myChart.showLoading();
      let mapFeatures = echarts.getMap(mapName).geoJson.features;
      //myChart.hideLoading();
      mapFeatures.forEach(function(v) {
        // 地区名称
        let name = v.properties.name;
        // 地区经纬度
        geoCoordMap[name] = v.properties.cp;
      });
      let max = 500, min = 10; // todo
      let maxSize4Pin = 50, minSize4Pin = 20;
      
      let convertData = function(data) {
        let res = [];
        let array = JSON.parse(JSON.stringify(data));
        for (let i = 0; i < array.length; i++) {
          let geoCoord = geoCoordMap[array[i].name];
          if (geoCoord) {
            res.push({
              name: array[i].name,
              value: geoCoord.concat(array[i].value)
            });
          } else if (array[i].lng != undefined && array[i].lat != undefined) {
            res.push({
              name: array[i].name,
              value: [array[i].lng,array[i].lat,array[i].value]
            });
          }
        }
        return res;
      };
      
     
      let seriesData = [
        {
          name: "map",
          type: "map",
          map: mapName,
          geoIndex: 0,
          aspectScale: 0.75, //长宽比
          selectMode: "single",
          showLegendSymbol: false, // 存在legend时显示
          label: {
            normal: {
              show: true
            },
            emphasis: {
              show: false,
              textStyle: {
                color: "#fff"
              }
            }
          }
        }
      ];
      /*
       * 判断是否需要添加散点图、气泡图和top5
       */
      
      if (dataOption.isScatter == true) {
        dataOption.scatterData.data = JSON.parse(JSON.stringify(convertData(result)));
        dataOption.scatterData.symbolSize = function(val) {
          return dataOption.scatterSize
            ? dataOption.scatterSize
            : val[2] / 10 > 100
            ? 100
            : val[2] / 10;
        };
        dataOption.scatterData.tooltip.formatter = function(val) {
          return val.name + ":" + val.value[2] + dataOption.suffix;
        };
        seriesData.push(dataOption.scatterData);
      }
      
      if (dataOption.isGeo == true) {
        dataOption.geoData.data = JSON.parse(JSON.stringify(convertData(result)));
        dataOption.geoData.symbolSize = function(val) {
          //console.log(val[2]);
          var a = (maxSize4Pin - minSize4Pin) / (max - min);
          var b = minSize4Pin - a * min;
          b = maxSize4Pin - a * max;
          return dataOption.popSize
            ? dataOption.popSize
            : a * val[2] + b > 200
            ? 200
            : a * val[2] + b;
        };

        dataOption.geoData.tooltip.formatter = function(val) {
          return val.name + ":" + val.value[2] + dataOption.suffix;
        };
        //选择气泡图形时，选择颜色
        if (dataOption.popORimg == "pop") {
          dataOption.geoData.symbol = "pin";
          dataOption.geoData.itemStyle.normal.color = dataOption.popColor
            ? dataOption.popColor
            : "rgba(216, 55, 55, 1)";
        }
        if (dataOption.popORimg == "img") {
          dataOption.geoData.symbol = "image://" + dataOption.popImg;
          dataOption.geoData.label.normal.show = false;
        }

        seriesData.push(dataOption.geoData);
      }

      
      let sortData = [...result]
      sortData.sort(function(a, b) {
        return b.value - a.value;
      })
    
      if (dataOption.isTop == true) {
        //console.log("top5Data:", dataOption.top5Data.data);
        let topNum = dataOption.topNum;
        if (topNum > result.length) {
          topNum = result.length;
        }
        dataOption.top5Data.data = convertData(sortData.slice(0, topNum));
        
        dataOption.top5Data.symbolSize = function(val) {
          return dataOption.scatterSize ? dataOption.scatterSize : 10;
        };
        dataOption.top5Data.tooltip.formatter = function(val) {
          return val.name + ":" + val.value[2] + dataOption.suffix;
        };
        seriesData.push(dataOption.top5Data);
      }
     
      var option = {
        title: {
          text: dataOption.title.text, //主标题
          textStyle: {
            color: "#fff", //颜色
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize, // 字号大小
            align: "center" //水平对齐
          },
          subtext: dataOption.title.subtext, //副标题
          subtextStyle: {
            //对应样式
            color: "#fff",
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize - 5,
            align: "center"
          },
          x: dataOption.title.x,
          itemGap: 7
        },
        //tooltip: dataOption.tooltip,
        visualMap: dataOption.visualMap,
        tooltip: {
          trigger: "item",
          formatter: function(params) {
            //console.log(params.data)
            if (params.data) {
              var toolTiphtml = params.data.name + ":<br>" + params.data;
              return toolTiphtml;
            }
            return;
          }
        },
        geo: {
          show: true,
          map: mapName,
          label: {
            normal: {
              show: false
            },
            emphasis: {
              show: false
            }
          },
          roam: dataOption.roam,
          itemStyle: {
            normal: {
              // areaColor: "#031525",   //区域颜色
              // borderColor: "#3B5077", //描边颜色
              areaColor:
                dataOption.isTopic == true ? color[0] : dataOption.areaColor, //区域颜色
              borderColor:
                dataOption.isTopic == true ? color[1] : dataOption.borderColor //描边颜色
            },
            emphasis: {
              areaColor:
                dataOption.isTopic == true
                  ? color[2]
                  : dataOption.emphasisColor
                  ? dataOption.emphasisColor
                  : "#2B91B7"
            }
          }
        },
        series: seriesData,
        bindingType: "map",
        suffix: dataOption.suffix
      };

      this.chart.setOption(option, true);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          if(params.data != undefined){
            //设置参数
            let arrObject = {
              seriesName: params.name,
              data: params.value[2]
            };
            let arrs = JSON.stringify(arrObject);
            //获取绑定的图表
            let bindList = this.chartOption.bindList;

            if (bindList.length > 0) {
              getLinkChart(dataOption.arrName, arrs, bindList, this.drawingList);
            }
          }
          
        });
      }
      // else{
      //   //关闭图表联动，取消echart点击事件
      //   this.chart.off('click');
      // }

      //开启图表下钻
      else if (dataOption.isDrillDown == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          if(params.data != undefined){
            //设置参数
            let arrObject = {
              seriesName: params.name,
              data: params.value[2]
            };
            
            let arrs = JSON.stringify(arrObject);

            //获取绑定的图表
            let drillDownChartOption = this.chartOption.drillDownChartOption;

            if (
              drillDownChartOption != undefined &&
              drillDownChartOption != null
            ) {
              this.$set(
                this.chartOption.drillDownChartOption.chartOption,
                "requestParameters",
                [{"name":"drillParam","value":arrs}]
              );
              //发送下钻消息
              VueEvent.$emit(
                "drill_down_msg",
                this.chartOption.drillDownChartOption
              );
            }
          }

        });
      } else {
        //关闭图表联动，取消echart点击事件
        this.chart.off("click");
      }

    },
  
  }
};
</script>
