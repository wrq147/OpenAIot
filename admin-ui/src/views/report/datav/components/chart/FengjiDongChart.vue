<template>
  <div :class="[animate,,'configuration'+chartOption.bindingDiv]" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="chartDiv"></div>
</template>

<script>
import resize from "@/views/dashboard/mixins/resize";
import "../../animate/animate.css";
import dataChart from "../mixins/dataChart.js";
import zutaiChart from "../mixins/zutaiChart.js";
import imgUrl1 from '@/views/report/datav/image/zutai/fengji2/1.png'
import imgUrl2 from '@/views/report/datav/image/zutai/fengji2/2.png'
import imgUrl3 from '@/views/report/datav/image/zutai/fengji2/3.png'
import imgUrl4 from '@/views/report/datav/image/zutai/fengji2/4.png'
import imgUrl5 from '@/views/report/datav/image/zutai/fengji2/5.png'
import imgUrl6 from '@/views/report/datav/image/zutai/fengji2/6.png'
export default {
  mixins: [resize, dataChart, zutaiChart],
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
    drawingList: {
      type: Array,
    },
  },
  data() {
    return {
      chart: null,
      value: "",
      animate: this.className,
      chartKey:0
    };
  },
  watch: {
    "chartOption.theme": {
      handler() {},
    },
    className: {
      handler(value) {
        this.animate = value;
      },
    },
  },
  mounted() {
    this.valUpdate = this.setChartVal;
    this.$nextTick(() => {
      let dom = document.getElementById(this.chartOption.bindingDiv);
      if (dom) {
        dom.addEventListener("mouseup", this.domOnMouseUp, true);
      }
      // const imgUrl=require(this.chartOption.svgUrl)
      // const imgUrl = "https://s5.ssl.qhres2.com/static/ec9f373a383d7664.svg";
      let imgUrl=[imgUrl1,imgUrl2,imgUrl3,imgUrl4,imgUrl5,imgUrl6]
      this.sprite(imgUrl);
    });
  },
  beforeDestroy() {},
  computed: {
    
  },
  methods: {
    setChartVal(result) {},
  },
};
</script>
