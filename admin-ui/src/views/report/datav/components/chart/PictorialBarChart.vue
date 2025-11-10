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
  mixins: [resize, dataChart],
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
      let seriesData = [];
      let axisData = new Set();
      let maxList = [];
      let legendData = new Set();
      result.forEach(element => {
        axisData.add(element.name)
        legendData.add(element.series)
      });
      axisData = [...axisData]
      legendData = [...legendData]
      if (dataOption.barOrient == "纵向") {
        dataOption.xAxis.data = axisData;
      }
      if (dataOption.barOrient == "横向") {
        dataOption.yAxis.data = axisData;
      }
      for (let index in legendData) {
        let data = { ...dataOption.series[0] };
        data.name = legendData[index];
        let valueArr = [];
        for (let v in result) {
          if (result[v].series === legendData[index]) {
            let array = {}
            array.value = result[v].value;
            array.name = result[v].name;
            valueArr.push(array);
          }
        }
        data.data = [...valueArr];
        for (const dataIndex in data.data) {
          if (dataOption.imageList.length > 0) {
            if (dataIndex < dataOption.imageList.length) {
              data.data[dataIndex].symbol =
                "image://" +
                process.env.VUE_APP_BASE_API +
                dataOption.imageList[index].imagePath;
            } else {
              data.data[dataIndex].symbol =
                "image://" +
                process.env.VUE_APP_BASE_API +
                dataOption.imageList[dataOption.imageList.length - 1].imagePath;
            }
          } else {
            data.data[dataIndex].symbol = data.symbol;
          }
        }
        data.symbolMargin = dataOption.symbolMargin + "%";
        data.label.show = false;
        seriesData.push(data);
      }
      //比例背景
      if (dataOption.showAll == true) {
        let dataLength = seriesData.length;
        //扩大series数组
        for (let index = 0; index < dataLength * 2; index += 2) {
          //为占比添加标签
          let newSeriesTemp = JSON.parse(JSON.stringify(seriesData[index]));
          //计算百分比
          let labelFormatter = e => {
            return (
              (
                (`${e.value}` / `${newSeriesTemp.symbolBoundingData}`) *
                100
              ).toFixed(0) + " %"
            );
          };

          let label = newSeriesTemp.label;
          label.formatter = labelFormatter;
          label.show = dataOption.series[0].label.show;
          label.offset = [dataOption.labelOffset0, dataOption.labelOffset1];
          newSeriesTemp.label = label;
          newSeriesTemp.symbolClip = false;
          newSeriesTemp.itemStyle = { opacity: 0.3 };
          // newSeriesTemp.label.show = false
          if (dataOption.barOrient == "横向") {
            newSeriesTemp.symbolOffset = [0, dataOption.symbolOffset[1]];

            dataOption.xAxis.max =
            result.axisMax || this.getMax(maxList);
          } else if (dataOption.barOrient == "纵向") {
            newSeriesTemp.symbolOffset = [dataOption.symbolOffset[0], 0];
            dataOption.yAxis.max =
            result.axisMax || this.getMax(maxList);
          }

          newSeriesTemp.z = 1;
          //在每一条series数据后，插入占比背景
          seriesData.splice(index + 1, 0, newSeriesTemp);
        }
      }

      let option = {
        title: dataOption.title,
        tooltip: dataOption.tooltip,
        legend: dataOption.legend,
        grid: {
          top: "15%",
          left: "0%",
          right: "15%",
          bottom: "10%",
          containLabel: true
        },
        xAxis: dataOption.xAxis,
        yAxis: dataOption.yAxis,
        series: seriesData
      };
      this.chart.setOption(option, true);
      let setting = {
        bindingType: "pictorialBar",
        option: option,
        symbolList: dataOption.imageList
      };
      addOption(dataOption.bindingDiv, setting);

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
      // this.chart.getZr().off('click');
      // this.chart.getZr().on('click',(params) => {
      //   let point=[params.offsetX,params.offsetY];
      //   if(this.chart.containPixel('grid',point)){
      //       let xIndex=this.chart.convertFromPixel({seriesIndex:0}, point)[0];

      //       let op=this.chart.getOption();
      //       let name=op.xAxis[0].data[xIndex];

      //   }

      // })

    },
    getMax(array) {
      let max = array[0];
      for (let index = 0; index < array.length; index++) {
        if (max < array[index]) {
          max = array[index];
        }
      }
      return max;
    }
  }
};
</script>
