<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
    ref="text"
  >
    <div :style="normalTextStyle">
      {{ this.value }}
    </div>
  </div>
</template>

<script>

import "../../animate/animate.css";
import dataChart from '../mixins/dataChart.js'
import {stringDataHandle} from '../../util/commonChartChange'
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
  },
  data() {
    return {
      value: "",
      animate: this.className
    };
  },
  watch: {
    width() { },
    height() { },
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  mounted() {
    // this.loadChartData()
    this.valUpdate = this.setChartVal
  },
  beforeDestroy() {
    clearTimeout(this.timer);
  },
  computed: {
    normalTextStyle() {
      let topPd=0
      let leftPd=0
      let justify=''
      let textbackground=''
      let borderStr=''
      if(this.chartOption.topPadding){
        topPd=this.chartOption.topPadding
      }
      if(this.chartOption.leftPadding){
        leftPd=this.chartOption.leftPadding
      }
      if(this.chartOption.textAlign){
        if(this.chartOption.textAlign=='left'){
          justify='flex-start'
        }
        if(this.chartOption.textAlign=='center'){
          justify='center'
        }
        if(this.chartOption.textAlign=='right'){
          justify='flex-end'
        }
      }
      if(this.chartOption.backgroundType&&this.chartOption.backgroundType=='img'){
        textbackground="url("+this.chartOption.backgroundImage+")"
      }else{
        textbackground=this.chartOption.backgroundColor
      }
      if(this.chartOption.border&&this.chartOption.border==1){
        borderStr=this.chartOption.borderWidth+'px solid '+this.chartOption.borderColor
      }
      let style = {
        color: this.chartOption.fontColor,
        fontSize: this.chartOption.fontSize + "px",
        fontFamily: this.chartOption.fontFamily,
        lineHeight: this.chartOption.lineHeight + "px",
        background: textbackground,
        border:borderStr,
        height: this.height,
        width: this.width,
        fontWeight: this.chartOption.fontWeight,
        letterSpacing: this.chartOption.letterSpacing + "px",
        textAlign: this.chartOption.textAlign,
        display:this.chartOption.display?this.chartOption.display:'',
        borderRadius:this.chartOption.conRadius?this.chartOption.conRadius + "px":0,
        padding:topPd+'% '+leftPd+'%',
        'align-items':this.chartOption.alignItems?this.chartOption.alignItems:'',
        'align-content':this.chartOption.alignItems?this.chartOption.alignItems:'',
        'flex-warp':'warp',
        'justify-content':justify
      };
      if(this.chartOption.backgroundType&&this.chartOption.backgroundType=='img'){
        style.backgroundSize='cover' /* 背景图片覆盖整个元素 */
        style.backgroundRepeat= 'no-repeat' /* 背景图片不重复 */
        style.backgroundPosition= 'center' /* 背景图片居中 */
      }
      return style;
    }
  },
  methods: {
    setChartVal(result,rowGlobal) {
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        this.value =stringDataHandle(rowGlobal,this.chartOption)?stringDataHandle(rowGlobal,this.chartOption):''
      }else{
        this.value = this.chartOption.staticDataValue;
      }
    },
  }
};
</script>
