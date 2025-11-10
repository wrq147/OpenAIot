<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
  >
    <div
      class="graph"
      :style="'height:' + graphHeight + 'px;' + 'width:' + graphWidth + 'px'"
    >
      <SeeksRelationGraph ref="seeksRelationGraph" />
    </div>
  </div>
</template>

<script>
import SeeksRelationGraph from "relation-graph";
import "../../animate/animate.css";
import dataChart from '../mixins/dataChart.js'

export default {
  mixins: [dataChart],
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
  components: { SeeksRelationGraph },
  data() {
    return {
      animate: this.className,
      graphWidth: Number(this.width.slice(0, -2)) * 0.8,
      graphHeight: Number(this.height.slice(0, -2)) * 0.8
    };
  },
  watch: {
    width() {
      this.$nextTick(() => {
        this.graphWidth = Number(this.width.slice(0, -2)) * 0.8;
      });
    },
    height() {
      this.$nextTick(() => {
        this.graphHeight = Number(this.height.slice(0, -2)) * 0.8;
      });
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
  methods: {
    setChartVal(result) {

      if (dataOption.animate != null) {
        //添加动画样式
        //animateUtil.addAnimate(dataOption.bindingDiv, dataOption.animate);
      }

      this.$refs.seeksRelationGraph.setOptions(
        dataOption.defaultGraphOptions,
        () => {}
      );
      this.$refs.seeksRelationGraph.setJsonData(
        dataOption.staticDataValue,
        seeksRGGraph => {}
      );
      //   addOption(dataOption.bindingDiv, option);
      /** 
      //开启图表联动
      if(dataOption.isLink == true){
          this.chart.off("click");
          this.chart.on('click', params => {
            //设置参数
            let arrObject = {             
              "legendName":params.name,
              "seriesName":params.seriesName,
              "data":params.value
            }

            let arrs = JSON.stringify(arrObject);
            
            //获取绑定的图表
            let bindList = this.chartOption.bindList;
                          
            if(bindList.length > 0){
              
              linkChart(dataOption.arrName, arrs, bindList,this.drawingList)

            }
            
          })
      }
      // else{
      //   //关闭图表联动，取消echart点击事件
      //   this.chart.off('click');
      // }

      //开启图表下钻
      else if(dataOption.isDrillDown == true){
          this.chart.off("click");
          this.chart.on('click', params => {
            
            //设置参数
            let arrObject = {             
              "legendName":params.name,
              "seriesName":params.seriesName,
              "data":params.value
            }

            let arrs = JSON.stringify(arrObject);
            
            //获取绑定的图表
            let drillDownChartOption = this.chartOption.drillDownChartOption;
                          
            if(drillDownChartOption != undefined && drillDownChartOption != null){
              
              this.$set(this.chartOption.drillDownChartOption.chartOption, 'requestParameters', "drillParam="+arrs);
              //发送下钻消息
              VueEvent.$emit("drill_down_msg", this.chartOption.drillDownChartOption);

            }
            
          })
      }
      else{
        //关闭图表联动，取消echart点击事件
        this.chart.off('click');
      }

      //开启远程图表控制
      if(dataOption.isRemote == true){
        if(dataOption.remote != undefined && dataOption.remote != null){
          this.chart.off("click");
          this.chart.on('click', params => {
            //设置参数
            let arrObject = {             
              "legendName":params.name,
              "seriesName":params.seriesName,
              "data":params.value
            }

            let remoteData = { ...dataOption.remote };
            remoteData.query = arrObject;
            //调用接口
            remoteChartApi(remoteData)
            
          })
        }
      }*/
    },
 
  }
};
</script>

<style scoped>
.graph >>> .rel-map {
  background-color: transparent;
}
</style>
