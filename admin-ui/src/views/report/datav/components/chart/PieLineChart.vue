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

      if (this.chart != null) {
        this.chart.on("updateAxisPointer", event => {
          let xAxisInfo = event.axesInfo[0];
          if (xAxisInfo) {
            let dimension = xAxisInfo.value + 1;
            this.chart.setOption({
              series: {
                id: "pie",
                label: {
                  formatter: "{b}: {@[" + dimension + "]} ({d}%)"
                },
                encode: {
                  value: dimension,
                  tooltip: dimension
                }
              }
            });
          }
        });


        let seriesData = []; //需要拼接折线图和饼图数据
        //let color = dataOption.color;
        dataOption.dataset.source = result;

        for (let index in result) {
          if (parseInt(index) != result.length - 1) {
            let line = {};
            line.type = "line";
            if (dataOption.isSmooth == true) {
              line.smooth = true;
            } else if (dataOption.isSmooth == false) {
              line.smooth = false;
            }

            //是否显示平均线
            if (dataOption.isMarkLine == true) {
              line.markLine = {
                data: [{ type: "average", name: "平均值" }]
              };
            } else if (
              dataOption.isMarkLine == false ||
              dataOption.isMarkLine == undefined
            ) {
              line.markLine = {};
            }

            //是否显示最大值和最小值
            if (dataOption.isMarkPoint == true) {
              line.markPoint = {
                data: [
                  { type: "max", name: "最大值" },
                  { type: "min", name: "最小值" }
                ]
              };
            } else if (
              dataOption.isMarkPoint == false ||
              dataOption.isMarkPoint == undefined
            ) {
              line.markPoint = {};
            }
            //line.smooth = true,
            line.seriesLayoutBy = "row";
            line.lineStyle = {
              width: 3,
              shadowColor: "rgba(231,234,237,0.4)",
              shadowBlur: 10,
              shadowOffsetY: 10
            };
            seriesData.push(line);
          }

          if (parseInt(index) == result.length - 1) {
            let pie = {};
            pie.type = "pie";
            pie.id = "pie";
            //pie.radius = '30%';//饼图半径(是否是环饼)
            if (dataOption.isRing == true) {
              pie.radius = [dataOption.innerRadius, dataOption.outerRadius];
            } else {
              pie.radius = "30%";
            }
            pie.center = ["50%", "30%"]; //饼图位置
            pie.roseType = false; //南丁格尔玫瑰
            if (dataOption.isRoseType == true) {
              pie.roseType = true;
            } else {
              pie.roseType = false;
            }

            let first = result[0][1];
            pie.label = {
              formatter: "{b}: {@"+first+"} ({d}%)"
            };
            pie.encode = {
              itemName: "product",
              value: first,
              tooltip: first
            };
            seriesData.push(pie);
          }

          //data.color = color[index];
        }

        let option = {
          title: dataOption.title,
          tooltip: dataOption.tooltip,
          legend: dataOption.legend,
          dataset: dataOption.dataset,
          grid: dataOption.grid,
          xAxis: dataOption.xAxis,
          yAxis: dataOption.yAxis,
          series: seriesData
          //,color: dataOption.color
        };


        this.chart.setOption(option, true);

        //开启图表联动
        if (dataOption.isLink == true) {
          this.chart.off("click");
          this.chart.on("click", params => {
            //设置参数
            let arrObject = {
              legendName: params.name,
              seriesName: params.dimensionNames[params.dataIndex + 1],
              data: params.value[params.dataIndex + 1]
            };

            let arrs = JSON.stringify(arrObject);

            //获取绑定的图表
            let bindList = this.chartOption.bindList;

            if (bindList.length > 0) {
              getLinkChart(dataOption.arrName, arrs, bindList, this.drawingList);
            }
          });
        }
        //开启图表下钻
        else if (dataOption.isDrillDown == true) {
          this.chart.off("click");
          this.chart.on("click", params => {
            //设置参数
            let arrObject = {
              legendName: params.name,
              seriesName: params.dimensionNames[params.dataIndex + 1],
              data: params.value[params.dataIndex + 1]
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
