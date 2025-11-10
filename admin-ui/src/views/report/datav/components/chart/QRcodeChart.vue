<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="text">
    <vue-qr :logoSrc="imageUrl" :text="text" :size="this.chartOption.size"></vue-qr>
  </div>
</template>

<script>
import vueQr from "vue-qr";
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
  },
  components: {
    vueQr
  },
  data() {
    return {
      text: "",
      imageUrl: this.chartOption.imageUrl,
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
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
  },
  computed: {},
  methods: {
    setChartVal(result) {
      this.imageUrl = this.chartOption.imageUrl;
      this.text = typeof result === "string" ? result : result.String()
    },

  }
};
</script>
