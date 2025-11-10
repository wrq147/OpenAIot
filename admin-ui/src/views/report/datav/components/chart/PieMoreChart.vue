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
import { getLinkChart } from "../../util/LinkageChart";
import { addOption } from "../../codegen/codegen";
import VueEvent from "../../VueEvent";
import dataChart from '../mixins/dataChart.js'
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

      if (result.length > 0) {
        let maxValue = 0;
        result.forEach(element => {
          maxValue += parseFloat(element.value);
        });

        let pieradius = [dataOption.innerRadius, dataOption.outerRadius]; // 半径
        const labelTop = {
          normal: {
            label: {
              show: true,
              position: "center",
              formatter: "{b}",
              textStyle: {
                baseline: "bottom"
              }
            },
            labelLine: {
              show: true
            }
          }
        };
        const labelBottom = {
          normal: {
            color: "#ccc",
            label: {
              show: true,
              position: "center"
            },
            labelLine: {
              show: false
            }
          },
          emphasis: {
            color: "rgba(255,255,255,0.7)"
          }
        };

        if (typeof dataOption.centerX == "undefined") {
          dataOption.centerX = 20;
        }
        if (typeof dataOption.centerY == "undefined") {
          dataOption.centerY = 30;
        }
        let col = 4; //列数
        if (dataOption.col != null) {
          col = dataOption.col;
        }

        let seriesDataArr = [];
        let xinterval = 100; //每个环宽度占比
        let yinterval = 100; //每个环高度占比

        if (result.length > 0) {
          xinterval = Math.round(100 / col);
          yinterval = Math.round(
            100 / Math.ceil(result.length / col)
          );
        }
        if (yinterval == 100) {
          yinterval = 0;
        }
        if (xinterval == 100) {
          xinterval = 0;
        }

        for (var i in result) {
          let seriesData = {
            type: "pie",
            center: [dataOption.centerX + "%", dataOption.centerY + "%"],
            radius: pieradius,
            x: (parseInt(i) % col) * xinterval + 5 + "%", // for funnel
            y: Math.floor(parseInt(i) / col) * yinterval + "%", // for funnel
            //itemStyle: labelFromatter,
            data: [
              {
                name: result[i].name,
                value: result[i].value,
                itemStyle: labelTop
              },
              {
                value: maxValue - result[i].value,

                name: "其他",
                itemStyle: labelBottom
              }
            ]
          };
          seriesDataArr.push(seriesData);
        }

        let option = {
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
          legend: dataOption.legend,
          grid: {
            left: 30,
            // right: "2%",
            //bottom: "20%",
            //top: "20%",
            containLabel: true
          },
          tooltip: {
            show: true,
            trigger: "item",
            formatter: "{b} : {c} ({d}%)"
          },
          series: seriesDataArr
        };

        this.chart.setOption(option, true);

        addOption(dataOption.bindingDiv, option);

        //开启图表联动
        if (dataOption.isLink == true) {
          this.chart.off("click");
          this.chart.on("click", params => {
            //设置参数
            let arrObject = {
              legendName: params.name,
              seriesName: params.seriesName,
              data: params.value
            };

            let arrs = JSON.stringify(arrObject);

            //获取绑定的图表
            let bindList = this.chartOption.bindList;

            if (bindList.length > 0) {
              getLinkChart(dataOption.arrName, arrs, bindList, this.drawingList);
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
            //设置参数
            let arrObject = {
              legendName: params.name,
              seriesName: params.seriesName,
              data: params.value
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
          });
        } else {
          //关闭图表联动，取消echart点击事件
          this.chart.off("click");
        }
      }

    },
   
  }
};
</script>
