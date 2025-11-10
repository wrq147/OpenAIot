<template>
  <div :class="animate" :style="{ height: height, width: width}" :id="chartOption.bindingDiv" >
    
      <a v-if="this.chartOption.type=='label'" :href="href" :target="target" ref="hyper" @mouseenter="visible" @mouseleave="invisible" :style="normalTextStyle">{{context}}</a>
      <a v-if="this.chartOption.type=='img'" :href="href" :target="target" ref="hyper">
        <img :src="imgSrc" alt="" :style="imgStyle">
      </a>
      
  
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
      basePath: process.env.VUE_APP_BASE_API,
      imgSrc: this.chartOption.img.src,
      context:"",
      animate: this.className,
      href:""
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
    normalTextStyle(){
      const style = {color:this.chartOption.label.linkColor,fontSize:this.chartOption.label.fontSize + "px",fontFamily:this.chartOption.label.fontFamily,
        position:'absolute',cursor:'pointer',userSelect:'none',textDecoration:this.chartOption.label.decoration,"zIndex":this.zIndex,fontWeight:this.chartOption.fontWeight}
      return style
    },
    target(){
      return this.chartOption.target
    },
    imgStyle(){
      return {width:this.chartOption.img.width+'px',height:this.chartOption.img.height+'px'}
    }
  },
  methods: {
    setChartVal(result) {
       this.context=this.dataOption.label.context;
       this.imgSrc = this.dataOption.img.src,
       this.href=result;
    },
  
    visible:function(){
      let style = this.$refs.hyper.style;
      style.color = this.chartOption.label.hoverColor
    },
    invisible:function(){
      let style = this.$refs.hyper.style;
      style.color = this.chartOption.label.linkColor
    }
  }
};
</script>
