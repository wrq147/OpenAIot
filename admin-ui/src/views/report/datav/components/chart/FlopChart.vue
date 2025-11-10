<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
    ref="text"
  >
    <count-to
      ref="flop"
      :style="classObject"
      :start-val="chartOption.startVal"
      :end-val="valueNum"
      :duration="chartOption.duration"
      :decimals="chartOption.decimals"
      :separator="chartOption.separator"
      :prefix="chartOption.prefix"
      :suffix="chartOption.suffix"
      :autoplay="true"
    ></count-to>
  </div>
</template>

<script>
import CountTo from "vue-count-to";
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
  components: {
    CountTo
  },
  data() {
    return {
      valueNum: 0,
      animate: this.className
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
  computed: {
    classObject() {
      return {
        "font-size": `${this.chartOption.fontSize}px`,
        "font-family": this.chartOption.fontFamily,
        "font-weight": this.chartOption.fontWeight,
        color: this.chartOption.fontColor,
        "background-color": this.chartOption.backgroundColor,
        "letter-spacing": this.chartOption.letterSpacing + "px",
        display: `block`,
        margin: `10px 0`,
        "text-align":
          this.chartOption.textAlign == undefined
            ? "center"
            : this.chartOption.textAlign
      };
    }
  },
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
  },
  methods: {
    setChartVal(result,rowGlobal) {
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        this.valueNum =stringDataHandle(rowGlobal,this.chartOption)?parseFloat(stringDataHandle(rowGlobal,this.chartOption)):0
      }else{
        this.valueNum = parseFloat(this.chartOption.staticDataValue);
      }
      // this.valueNum = parseFloat(result);
      this.$refs.flop.start();
    },
 
  }
};
</script>

<style scoped></style>
