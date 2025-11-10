<template>
  <div style="width:100%; height:100%;"></div>
</template>
<script>

import echarts from "echarts";
require("echarts/theme/macarons"); // echarts theme
import resize from '@/views/dashboard/mixins/resize'

import 'echarts-gl'
import 'echarts-liquidfill'
import 'echarts-wordcloud'

import 'echarts/map/js/china'
import 'echarts/map/js/world'
import 'echarts/map/js/province/anhui'
import 'echarts/map/js/province/aomen'
import 'echarts/map/js/province/hebei'
import 'echarts/map/js/province/heilongjiang'
import 'echarts/map/js/province/henan'
import 'echarts/map/js/province/hubei'
import 'echarts/map/js/province/hunan'
import 'echarts/map/js/province/jiangsu'
import 'echarts/map/js/province/jiangxi'
import 'echarts/map/js/province/jilin'
import 'echarts/map/js/province/liaoning'
import 'echarts/map/js/province/neimenggu'
import 'echarts/map/js/province/beijing'
import 'echarts/map/js/province/ningxia'
import 'echarts/map/js/province/qinghai'
import 'echarts/map/js/province/shandong'
import 'echarts/map/js/province/shanghai'
import 'echarts/map/js/province/shanxi'
import 'echarts/map/js/province/shanxi1'
import 'echarts/map/js/province/sichuan'
import 'echarts/map/js/province/taiwan'
import 'echarts/map/js/province/tianjin'
import 'echarts/map/js/province/xianggang'
import 'echarts/map/js/province/chongqing'
import 'echarts/map/js/province/xinjiang'
import 'echarts/map/js/province/xizang'
import 'echarts/map/js/province/yunnan'
import 'echarts/map/js/province/zhejiang'
import 'echarts/map/js/province/fujian'
import 'echarts/map/js/province/gansu'
import 'echarts/map/js/province/guangdong'
import 'echarts/map/js/province/guangxi'
import 'echarts/map/js/province/guizhou'
import 'echarts/map/js/province/hainan'

export default {
  props: ["customOption"],
  data() {
      return {
      chart: null,
      //dataOption: this.chartOption,
      
    };
  },
  watch: {
    width() {
      this.chart.resize();
    },
    height() {
      this.chart.resize();
    },
    customOption: {
      deep: true,
      handler(newVal) {
        this.$nextTick(() => {
          this.chart.dispose();
          this.chart = null;
          this.initChart(JSON.parse(JSON.stringify( newVal )));
        });
      }
    },
    
  },
  beforeDestroy() {
    if (!this.chart) {
      return;
    }
    this.chart.dispose();
    this.chart = null;
  },
  mounted() {
    this.$nextTick(() => {
      this.initChart(JSON.parse(JSON.stringify( this.customOption )));
    });
  },
  methods: {
      initChart(dataOption) {
          this.chart = echarts.init(this.$el, 'macarons');
          var option;
          //eval(dataOption);
          (function(echarts, myChart, dataValue,requestParameters){
            //里面为要处理的代码块
            eval(dataOption)
          })(echarts, this.chart, null,"你所传的参数")
          this.chart.setOption(option, true);
      }
  },
};
</script>
<style scoped>
* {
  margin: 0;
  padding: 0;
}

@media screen and (min-width: 960px) {
  .editorSet {
    width: 100%;
  }
  .editorSet .editor {
    height: 450px;
    width: 48%;
    overflow-y: hidden;
    background-color: rgb(44, 52, 55);
    float: left;
  }
}

@media screen and (max-width: 959px) {
  .editorSet {
    width: 100%;
  }
  .editorSet .editor {
    height: 450px;
    width: 100%;
    overflow-y: hidden;
    background-color: rgb(44, 52, 55);
  }
}

.NightTheme .editorSet .editor {
  -webkit-box-shadow: inset 0 1px 3px rgb(22, 26, 27);
  -moz-box-shadow: inset 0 1px 3px rgb(22, 26, 27);
  box-shadow: inset 0 1px 3px rgb(22, 26, 27);
}

.label {
  background: rgba(230, 230, 230, 0.5);
  height: 20px;
  padding: 0 6px;
  line-height: 20px;
  z-index: 999;
  text-align: center;
  font-size: 12px;
  color: #bbb;
  border-radius: 3px;
}

.editor iframe {
  border: 0 !important;
  min-height: 100px;
  min-width: 100px;
  height: 100%;
  width: 100%;
}

.submit-btn {
  color: #fff;
  background-color: #67c23a;
  display: inline-block;
  line-height: 1;
  white-space: nowrap;
  cursor: pointer;
  border: 1px solid #67c23a;
  -webkit-appearance: none;
  text-align: center;
  box-sizing: border-box;
  outline: none;
  margin: 0;
  transition: 0.1s;
  -moz-user-select: none;
  -webkit-user-select: none;
  -ms-user-select: none;
  padding: 12px 20px;
  font-size: 14px;
  border-radius: 4px;
  margin-bottom: 2px;
}

.edit-area {
  height: 100%;
}
</style>