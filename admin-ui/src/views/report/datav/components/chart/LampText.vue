<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
    ref="text"
  >
    <div :style="normalTextStyle" ref="noticeInner">
      <div :style="textWidth">
        {{ lampMsg }}
      </div>
    </div>
  </div>
</template>

<script>
import "../../animate/animate.css";
import dataChart from '../mixins/dataChart.js'
import {stringArrayDataHandle} from '../../util/commonChartChange'
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
      textArr: [],
      lampMsg: "",
      loopTimer:null,
      sObj: null,
      animate: this.className,
      dataArr:[]
    };
  },
  watch: {
    width() {},
    height() {},
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
    if(this.loopTimer!=null){
      clearTimeout(this.loopTimer);
    }
  },
  computed: {
    normalTextStyle() {
      const style = {
        color: this.chartOption.fontColor,
        fontSize: this.chartOption.fontSize + "px",
        fontFamily: this.chartOption.fontFamily,
        lineHeight: this.chartOption.lineHeight + "px",
        backgroundColor: this.chartOption.backgroundColor,
        height: this.height,
        width: this.width,
        fontWeight: this.chartOption.fontWeight,
        letterSpacing: this.chartOption.letterSpacing + "px",
        textAlign: this.chartOption.textAlign,
        overflow: "hidden"
      };
      return style;
    },

  },
  methods: {
    textWidth() {
      let lampWidth =
        this.lampMsg.length * parseInt(this.chartOption.fontSize) + 5;

      const style = { position: "absolute", width: lampWidth + "px" };
      return style;
    },
    setChartVal(result,rowGlobal) {
      // this.dataArr = result;
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        this.dataArr =stringArrayDataHandle(rowGlobal,this.chartOption)?stringArrayDataHandle(rowGlobal,this.chartOption):[]
      }else{
        this.dataArr = this.chartOption.staticDataValue;
      }
      if(this.loopTimer!=null){
        clearTimeout(this.loopTimer);
      }
      let dataOption = this.dataOption;
      this.init(dataOption);
    },
    init(data) {
      let currentIndex = 0;
      let innerWidth = 0;
      let child = this.$refs.noticeInner.childNodes;
      let padding = 0;

      let animation = () => {
        padding -= 1;
        if(currentIndex>=this.dataArr.length){
          return;
        }
        let lampWidth =
          (this.dataArr[currentIndex].length + data.letterSpacing) *
            parseInt(data.fontSize) +
          5;
        this.lampMsg = this.dataArr[currentIndex];
        this.$refs.noticeInner.childNodes[0].style.cssText =
          "position:'absolute';width:" +
          lampWidth +
          "px;transform: translate3d(" +
          padding +
          "px, 0, 0)";

        if (
          padding ===
          innerWidth -
            parseInt(window.getComputedStyle(child[0]).width.split("px")[0])
        ) {
          padding = this.width.split("px")[0];
          this.$refs.noticeInner.childNodes[0].style.cssText =
            "position:'absolute';width:" +
            lampWidth +
            "px;transform: translate3d(" +
            padding +
            "px, 0, 0)";
          if (currentIndex < this.dataArr.length - 1) {
            currentIndex++;
          } else {
            currentIndex = 0;
          }
        }
        this.loopTimer = setTimeout(function() {
          animation();
        }, data.loopDelay);
      };

      animation();
    },
  
  }
};
</script>
