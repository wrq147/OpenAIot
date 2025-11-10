<template>
  <div :class="[animate,,'configuration'+chartOption.bindingDiv]" :style="{ height: height, width: width,}" :id="chartOption.bindingDiv" ref="chartDiv" style="z-index:999 !important"></div>
</template>

<script>
import resize from "@/views/dashboard/mixins/resize";
import "../../animate/animate.css";
import dataChart from "../mixins/dataChart.js";
import zutaiChart from "../mixins/zutaiChart.js";
import imgUrl from '@/views/report/datav/image/zutai/fengji1.svg'
// import { gsap } from "gsap";
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
      container:null
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
      this.sprite(imgUrl);
      // if(this.chartOption.isOnlyRotate&&this.spriteimg){
      //   this.animateSprite(this.spriteimg)
      // }
      
    });
  },
  beforeDestroy() {},
  computed: {
    
  },
  methods: {
    animateSprite(sprite) {
      // const animation = gsap.to(sprite.attr(),  { x: 200 });
      // console.log(animation,'animationanimation');
      // gsap.to(sprite, {
      //   rotation: 360, // 旋转角度为 360 度
      //   duration: 2, // 动画持续时间为 2 秒
      //   repeat: -1, // 无限重复
      //   ease: "linear" // 线性缓动效果
      // });
      const animationObj = { rotation: 0 };
      const animation = gsap.to(
        animationObj,
        {
          rotation: 360,
          duration: 2,
          repeat: -1,
          ease: 'linear',
          onUpdate: () => {
            sprite.attr('rotate', animationObj.rotation);
          }
        }
      );
      // 设置动画重复播放
      // animation.repeat(-1); // -1表示无限循环播放
    },
    setChartVal(result) {},
  },
};
</script>
