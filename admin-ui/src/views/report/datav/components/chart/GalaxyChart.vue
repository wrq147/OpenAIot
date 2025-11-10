<template>
  <div :class="className" :style="{ height: height, width: width}" :id="chartOption.bindingDiv">

     <div :id="'galaxy'+chartOption.bindingDiv" :style="{ height: height, width: width}"></div>
      <!--<canvas :id="'c'+customId" ref="c" :style="{ height: height, width: width}"></canvas> -->
  </div>
</template>

<script>
import { GalaxyRun } from "./Galaxy"
import '../../animate/animate.css'
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
  },
  data() {
    return {
      opts:{
	  
        lineCount: 100,
        starCount: 30,
        
        radVel: .01,
        lineBaseVel: .1,
        lineAddedVel: .1,
        lineBaseLife: .4,
        lineAddedLife: .01,
        
        starBaseLife: 10,
        starAddedLife: 10,
        
        ellipseTilt: -.3,
        ellipseBaseRadius: .15,
        ellipseAddedRadius: .02,
        ellipseAxisMultiplierX: 2,
        ellipseAxisMultiplierY: 1,
        ellipseCX: this.width / 2,
        ellipseCY: this.height / 2,
        
        repaintAlpha: .015
      },
      tick:0,
    };
  },
  watch: {
    width() {
      this.setChartVal();
    },
    height() {
      this.setChartVal();
    },
  },
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
  },
  computed:{
  },
  methods: {
    setChartVal(result) {
      let dataOption = this.dataOption;
      dataOption.opts.ellipseCX = this.width / 2;
      dataOption.opts.ellipseCY = this.height / 2;
      var galaxy = document.getElementById('galaxy'+this.chartOption.bindingDiv);
      
      var box = document.getElementById('c'+this.chartOption.bindingDiv);     
      if (box != null) {
        box.remove();
      }

      //创建一个画布
      let can = document.createElement('canvas');
      //设置画布的长宽
      var width = this.width.replace("px","");
      var height = this.height.replace("px","");
      
      can.id = 'c'+this.chartOption.bindingDiv;
      can.width = width;
      can.height = height;

      galaxy.append(can);
      GalaxyRun(can, dataOption.opts,this.width,this.height, width,height);

    },
  }
};
</script>
