<template>
  <div :class="animate" :style="{ height: height, width: width}" :id="chartOption.bindingDiv">
    <div v-if="isDraw" style="display: flex;height: 100%;width:100%;align-items: center;justify-content: center;">
      iframe连接地址：{{src }}
    </div>
    <div v-else @contextmenu.prevent="" ref="text">
      <iframe :src="src" frameborder="0" allowtransparency="true" :style="iframeStyle">您的浏览器不支持嵌入式框架，或者当前配置为不显示嵌入式框架。</iframe>
    </div>
  </div>
</template>

<script>
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
    zIndex:{
      type:Number
    }
  },
  data() {
    return {
      src:"",
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
    }
  },
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
  },
  computed:{
   iframeStyle(){
     return {backgroundColor:this.chartOption.backgroundColor,height:'100%',width:'100%',position:'absolute',display:'block',"zIndex":this.zIndex}
   }
  },
  methods: {
    setChartVal(result) {
      this.src = result;
    },
 
  }
};
</script>
