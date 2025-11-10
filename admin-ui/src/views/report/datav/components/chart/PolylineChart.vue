<template>
  <!-- <div :class="animate" :style="{ height: height, width: width,}" :id="chartOption.bindingDiv" ref="chartDiv" style="z-index:999 !important"></div> -->
  <svg v-if="chartOption.bindingDiv" :ref="'svg' + chartOption.bindingDiv" @mousemove.stop="drag" @mouseup.stop="endDrag"
    :class="[animate, 'svg' + chartOption.bindingDiv,'svg_con']" :id="chartOption.bindingDiv" :width="width" :height="height">
    <path
      :id="'line' + chartOption.bindingDiv"
      :d="'M ' +chartOption.start.cx +' ' +chartOption.start.cy +' L' +chartOption.mid1.cx +' ' +chartOption.mid1.cy +' L' +chartOption.mid2.cx +' ' +chartOption.mid2.cy +' L' +chartOption.end.cx +' ' +chartOption.end.cy"
      :stroke="chartOption.lineColor"
      :stroke-width="chartOption.lineWidth"
      fill="none"
      marker-end="url(#marker-end-849d8617-602c-404f-8b13-b8aa2b883ddd)"
      stroke-dashoffset="0"
      :stroke-dasharray="chartOption.animateType=='eleCurrent'?chartOption.dasharray:0"
      stroke-linejoin="round"
    >
    <!-- stroke-linejoin="round"设置路径变得圆滑 //stroke-dasharray控制线缝隙间隔-->
      <animate v-if="chartOption.isReverseAnimation" attributeName="stroke-dashoffset" from="0" to="1000" :dur="chartOption.delayTime+'s'" repeatCount="indefinite"></animate>
      <!-- 正序运行动画 -->
      <animate v-else attributeName="stroke-dashoffset" from="1000" to="0" :dur="chartOption.delayTime+'s'" repeatCount="indefinite"></animate>
      <!-- 逆序运行动画 -->
    </path>
    <path
      :id="'line2' + chartOption.bindingDiv"
      :d="'M ' +chartOption.start.cx +' ' +chartOption.start.cy +' L' +chartOption.mid1.cx +' ' +chartOption.mid1.cy +' L' +chartOption.mid2.cx +' ' +chartOption.mid2.cy +' L' +chartOption.end.cx +' ' +chartOption.end.cy"
      :stroke="chartOption.flowColor"
      :stroke-width="chartOption.flowWidth"
      fill-opacity="0"
      fill="none"
      :stroke-dasharray="chartOption.dasharray"
      stroke-dashoffset="0"
      stroke-linecap="round"
      v-if="chartOption.animateType=='droplet'"
      stroke-linejoin="round"
    >
      <animate v-if="chartOption.isReverseAnimation" attributeName="stroke-dashoffset" from="0" to="1000" :dur="chartOption.delayTime+'s'" repeatCount="indefinite"></animate>
      <animate v-else attributeName="stroke-dashoffset" from="1000" to="0" :dur="chartOption.delayTime+'s'" repeatCount="indefinite"></animate>
    </path>
    <circle v-if="chartOption.animateType=='track'" cx="0" cy="0" :r="chartOption.radius" :fill="chartOption.radiusFillColor">
      <animateMotion v-if="chartOption.isReverseAnimation" :path="'M ' +chartOption.end.cx +' ' +chartOption.end.cy +' L' +chartOption.mid2.cx +' ' +chartOption.mid2.cy +' L' +chartOption.mid1.cx +' ' +chartOption.mid1.cy +' L' +chartOption.start.cx +' ' +chartOption.start.cy" :dur="chartOption.delayTime+'s'" repeatCount="indefinite"></animateMotion>
      <animateMotion v-else :path="'M ' +chartOption.start.cx +' ' +chartOption.start.cy +' L' +chartOption.mid1.cx +' ' +chartOption.mid1.cy +' L' +chartOption.mid2.cx +' ' +chartOption.mid2.cy +' L' +chartOption.end.cx +' ' +chartOption.end.cy" :dur="chartOption.delayTime+'s'" repeatCount="indefinite"></animateMotion>
    </circle>
    <!-- 四个点，改变点的位置可以改变线的走向 -->
    <circle v-if="isDraw" @mousedown.stop="startDragStart" :id="'start' + chartOption.bindingDiv" :cx="chartOption.start.cx"
      :cy="chartOption.start.cy" r="8" fill="white" stroke="black" stroke-width="2" class="circle"/>
    <circle v-if="isDraw" @mousedown.stop="startDragMid1" :id="'mid1' + chartOption.bindingDiv" :cx="chartOption.mid1.cx" :cy="chartOption.mid1.cy"
      r="8" fill="white" stroke="black" stroke-width="2" class="circle"/>
    <circle v-if="isDraw" @mousedown.stop="startDragMid2" :id="'mid2' + chartOption.bindingDiv" :cx="chartOption.mid2.cx" :cy="chartOption.mid2.cy"
      r="8" fill="white" stroke="black" stroke-width="2" class="circle"/>
    <circle v-if="isDraw" @mousedown.stop="startDragEnd" :id="'end' + chartOption.bindingDiv" :cx="chartOption.end.cx"
      :cy="chartOption.end.cy" r="8" fill="white" stroke="black" stroke-width="2" class="circle"/>
  </svg>
</template>

<script>
import resize from "@/views/dashboard/mixins/resize";
import "../../animate/animate.css";
import dataChart from "../mixins/dataChart.js";
// import { gsap } from "gsap";
export default {
  mixins: [resize, dataChart],
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
    x: {
      type: Number,
    },
    y: {
      type: Number,
    },
  },
  data() {
    return {
      chart: null,
      value: "",
      animate: this.className,
      start: "",
      end: "",
      mid1: "",
      mid2: "",
      line: "",
      svg: "",
      // svgRect:'',
      svgPadding: 10,
      startCoords: {
        x: 0,
        y: 0,
      },
      endCoords: {
        x: 100,
        y: 0,
      },
      mid1Coords: {
        x: 200,
        y: 0,
      },
      mid2Coords: {
        x: 300,
        y: 0,
      },
      isDraggingStart: false,
      isDraggingEnd: false,
      isDraggingMid1: false,
      isDraggingMid2: false,
      createSVGPoint: null,
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
  },
  beforeDestroy() {},
  computed: {
    widNum() {
      if (this.width.indexOf("px") > -1) {
        return Number(this.width.substring(0, this.width.length - 2));
      } else if (this.width.indexOf("%") > -1) {
        return Number(this.width.substring(0, this.width.length - 1));
      } else {
        return Number(this.width.substring(0, this.width.length));
      }
    },
    heiNum() {
      if (this.height.indexOf("px") > -1) {
        return Number(this.height.substring(0, this.height.length - 2));
      } else if (this.height.indexOf("%") > -1) {
        return Number(this.height.substring(0, this.height.length - 1));
      } else {
        return Number(this.height.substring(0, this.height.length));
      }
    },
    svgRect() {
      //范围
      let obj = {
        left: 0,
        right: 0 + this.widNum,
        top: 0,
        bottom: 0 + this.heiNum,
      };
      return obj;
    },
  },
  methods: {
    removeSprite() {},
    upActiveId() {},
    setChartVal(result) {},
    getMouseCoords(event) {
      const svg = document.querySelector(".svg" + this.chartOption.bindingDiv);
      const pt = svg.createSVGPoint();//用于创建一个空的SVGPoint对象，该对象表示二维平面上的一个点，可以用于进行坐标变换和计算‌‌
      pt.x = event.clientX;
      pt.y = event.clientY;
      return pt.matrixTransform(svg.getScreenCTM().inverse());
    },
    startDragStart(event) {//获取鼠标放下开始点的位置
      this.isDraggingStart = true;
      this.startCoords = this.getMouseCoords(event);
    },
    startDragEnd(event) {//获取鼠标放下结束点的位置
      this.isDraggingEnd = true;
      this.endCoords = this.getMouseCoords(event);
    },
    startDragMid1(event) {//获取鼠标放下中间点1的位置
      this.isDraggingMid1 = true;
      this.mid1Coords = this.getMouseCoords(event);
    },
    startDragMid2(event) {//获取鼠标放下中间点2的位置
      this.isDraggingMid2 = true;
      this.mid2Coords = this.getMouseCoords(event);
    },
    drag(event) {//鼠标拖动点的方法
      if (this.isDraggingStart) {//判断拖动的点是哪一个
        const coords = this.getMouseCoords(event);
        const dx = coords.x - this.startCoords.x;
        const dy = coords.y - this.startCoords.y;
        this.startCoords = coords;
        let x = parseFloat(this.chartOption.start.cx) + dx;
        let y = parseFloat(this.chartOption.start.cy) + dy;
        x = Math.min(
          Math.max(x, this.svgRect.left + this.svgPadding),
          this.svgRect.right - this.svgPadding
        );
        y = Math.min(
          Math.max(y, this.svgRect.top + this.svgPadding),
          this.svgRect.bottom - this.svgPadding
        );
        this.chartOption.start.cx = x;
        this.chartOption.start.cy = y;
      } else if (this.isDraggingEnd) {
        const coords = this.getMouseCoords(event);
        const dx = coords.x - this.endCoords.x;
        const dy = coords.y - this.endCoords.y;
        this.endCoords = coords;
        let x = parseFloat(this.chartOption.end.cx) + dx;
        let y = parseFloat(this.chartOption.end.cy) + dy;
        x = Math.min(
          Math.max(x, this.svgRect.left + this.svgPadding),
          this.svgRect.right - this.svgPadding
        );
        y = Math.min(
          Math.max(y, this.svgRect.top + this.svgPadding),
          this.svgRect.bottom - this.svgPadding
        );
        this.chartOption.end.cx = x;
        this.chartOption.end.cy = y;
      } else if (this.isDraggingMid1) {
        const coords = this.getMouseCoords(event);
        const dx = coords.x - this.mid1Coords.x;
        const dy = coords.y - this.mid1Coords.y;
        this.mid1Coords = coords;
        let x = parseFloat(this.chartOption.mid1.cx) + dx;
        let y = parseFloat(this.chartOption.mid1.cy) + dy;
        x = Math.min(
          Math.max(x, this.svgRect.left + this.svgPadding),
          this.svgRect.right - this.svgPadding
        );
        y = Math.min(
          Math.max(y, this.svgRect.top + this.svgPadding),
          this.svgRect.bottom - this.svgPadding
        );
        this.chartOption.mid1.cx = x;
        this.chartOption.mid1.cy = y;
      } else if (this.isDraggingMid2) {
        const coords = this.getMouseCoords(event);
        const dx = coords.x - this.mid2Coords.x;
        const dy = coords.y - this.mid2Coords.y;
        this.mid2Coords = coords;
        let x = parseFloat(this.chartOption.mid2.cx) + dx;
        let y = parseFloat(this.chartOption.mid2.cy) + dy;
        x = Math.min(
          Math.max(x, this.svgRect.left + this.svgPadding),
          this.svgRect.right - this.svgPadding
        );
        y = Math.min(
          Math.max(y, this.svgRect.top + this.svgPadding),
          this.svgRect.bottom - this.svgPadding
        );
        this.chartOption.mid2.cx = x;
        this.chartOption.mid2.cy = y;
      }
    },
    endDrag() {
      this.isDraggingStart = false;
      this.isDraggingEnd = false;
      this.isDraggingMid1 = false;
      this.isDraggingMid2 = false;
    },
  },
};
</script>
<style lang="less" scoped>
path {
  fill: none !important; //
}
.svg_con .circle{
  display:none;
}
.svg_con:hover .circle{
  display: block;
}
</style>
