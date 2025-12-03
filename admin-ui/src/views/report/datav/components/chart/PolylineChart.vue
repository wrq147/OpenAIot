<template>
  <div :class="[animate, 'svg_container']" :style="{ width: width, height: height, position: 'relative' }">
    <svg v-if="chartOption.bindingDiv" ref="svgRef" @mousemove.passive="drag" @mouseup="endDrag"
      @mouseleave="endDrag" :id="chartOption.bindingDiv" :width="svgWidth" :height="svgHeight"
      preserveAspectRatio="none">
      <!-- 主路径 -->
      <path :id="'line' + chartOption.bindingDiv" :d="getPathData()" :stroke="chartOption.lineColor"
        :stroke-width="chartOption.lineWidth" fill="none"
        marker-end="url(#marker-end-849d8617-602c-404f-8b13-b8aa2b883ddd)" stroke-dashoffset="0"
        :stroke-dasharray="chartOption.animateType == 'eleCurrent' ? chartOption.dasharray : 0" stroke-linejoin="round">
        <animate v-if="chartOption.isReverseAnimation" attributeName="stroke-dashoffset" from="0" to="1000"
          :dur="chartOption.delayTime + 's'" repeatCount="indefinite"></animate>
        <animate v-else attributeName="stroke-dashoffset" from="1000" to="0" :dur="chartOption.delayTime + 's'"
          repeatCount="indefinite"></animate>
      </path>

      <!-- 流动路径 -->
      <path :id="'line2' + chartOption.bindingDiv" :d="getPathData()" :stroke="chartOption.flowColor"
        :stroke-width="chartOption.flowWidth" fill-opacity="0" fill="none" :stroke-dasharray="chartOption.dasharray"
        stroke-dashoffset="0" stroke-linecap="round" v-if="chartOption.animateType == 'droplet'"
        stroke-linejoin="round">
        <animate v-if="chartOption.isReverseAnimation" attributeName="stroke-dashoffset" from="0" to="1000"
          :dur="chartOption.delayTime + 's'" repeatCount="indefinite"></animate>
        <animate v-else attributeName="stroke-dashoffset" from="1000" to="0" :dur="chartOption.delayTime + 's'"
          repeatCount="indefinite"></animate>
      </path>

      <!-- 跟踪动画 -->
      <circle v-if="chartOption.animateType == 'track'" cx="0" cy="0" :r="chartOption.radius"
        :fill="chartOption.radiusFillColor">
        <animateMotion v-if="chartOption.isReverseAnimation" :path="getReversePathData()"
          :dur="chartOption.delayTime + 's'" repeatCount="indefinite"></animateMotion>
        <animateMotion v-else :path="getPathData()" :dur="chartOption.delayTime + 's'" repeatCount="indefinite">
        </animateMotion>
      </circle>

      <!-- 动态渲染控制点：用动态属性控制大小，替代CSS修改r -->
      <circle v-for="(point, index) in chartOption.points" :key="`point${index}-${chartOption.bindingDiv}`"
        v-if="isDraw" @mousedown.stop="startDrag(index, $event)" :id="`point${index}${chartOption.bindingDiv}`"
        :cx="point.cx" :cy="point.cy" :r="isHoverPoint(index) ? 9 : 8" fill="white" stroke="black"
        :stroke-width="isHoverPoint(index) ? 3 : 2" class="circle" @mouseenter="setHoverIndex(index)"
        @mouseleave="clearHoverIndex()" />
    </svg>
  </div>
</template>

<script>
import resize from "@/views/dashboard/mixins/resize";
import "../../animate/animate.css";
import dataChart from "../mixins/dataChart.js";

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
    dragchartdata: {
      type: Object
    }
  },
  data() {
    return {
      chart: null,
      value: "",
      animate: this.className,
      svgPadding: 10,
      svgWidth: 0, // SVG实际宽度（实时更新）
      svgHeight: 0, // SVG实际高度（实时更新）
      dragState: {
        index: -1,
        startX: 0,
        startY: 0
      },
      widNum: this.WHToNumber(this.width),
      heiNum: this.WHToNumber(this.height),
      svgElement: null, // 缓存SVG元素
      hoverPointIndex: -1, // 当前hover的点索引（用于动态控制大小）
    };
  },
  watch: {
    "chartOption.theme": {
      handler() { },
    },
    className: {
      handler(value) {
        this.animate = value;
      },
    },
    width: {
      handler(newvalue) {
        this.widNum = this.WHToNumber(newvalue);
        this.updateSvgSize();
        this.fixPoints();
      },
      immediate: true,
    },
    height: {
      handler(newvalue) {
        this.heiNum = this.WHToNumber(newvalue);
        this.updateSvgSize();
        this.fixPoints();
      },
      immediate: true,
    },

  },
  mounted() {
    this.valUpdate = (result) => { };
    this.svgElement = this.$refs.svgRef;
    this.updateSvgSize(); // 初始化SVG尺寸
    this.fixPoints();
  },
  methods: {
    WHToNumber(str) {
      if (str.indexOf("px") > -1) {
        return Number(str.substring(0, str.length - 2));
      } else if (str.indexOf("%") > -1) {
        return Number(str.substring(0, str.length - 1));
      } else {
        return Number(str.substring(0, str.length));
      }
    },
    fixPoints() {
      if (!this.svgWidth || !this.svgHeight) return;
      const points = this.chartOption.points;
      if (!Array.isArray(points) || points.length < 2) return;


      // 边界限制
      for (let i = 0; i < points.length; i++) {
        let newCx = points[i].cx;
        let newCy = points[i].cy;
        newCx = Math.min(
          Math.max(newCx, this.svgPadding),
          this.svgWidth - this.svgPadding
        );

        newCy = Math.min(
          Math.max(newCy, this.svgPadding),
          this.svgHeight - this.svgPadding
        );
        this.$set(points, i, {
          cx: newCx,
          cy: newCy
        });
      }



    },
    // 设置当前hover的点索引
    setHoverIndex(index) {
      this.hoverPointIndex = index;
    },

    // 清除hover状态
    clearHoverIndex() {
      this.hoverPointIndex = -1;
    },

    // 判断是否是当前hover的点
    isHoverPoint(index) {
      return this.hoverPointIndex === index;
    },

    // 实时更新SVG尺寸（解决组件缩放问题）
    updateSvgSize() {
      if (!this.$el) return;

      this.svgWidth = this.widNum;
      this.svgHeight = this.heiNum;

      // 同步更新SVG元素的width/height属性（确保SVG坐标系与计算值一致）
      if (this.svgElement) {
        this.svgElement.setAttribute("width", this.svgWidth);
        this.svgElement.setAttribute("height", this.svgHeight);
      }
    },
    // 精准获取SVG坐标系下的鼠标坐标（无延迟）
    getMouseCoords(event) {
      if (!this.svgElement) return { x: 0, y: 0 };

      const pt = this.svgElement.createSVGPoint();
      pt.x = event.clientX;
      pt.y = event.clientY;
      // 实时获取CTM矩阵（组件缩放后坐标正确）
      const ctm = this.svgElement.getScreenCTM();
      if (!ctm) return { x: 0, y: 0 };
      const svgCoords = pt.matrixTransform(ctm.inverse());
      return {
        x: svgCoords.x,
        y: svgCoords.y
      };
    },

    // 开始拖动：计算鼠标相对于点的偏移（解决点击位置不精准问题）
    startDrag(index, event) {
      const mouseCoords = this.getMouseCoords(event);
      this.dragState = {
        index,
        startX: mouseCoords.x,
        startY: mouseCoords.y
      };

      event.preventDefault();
      // 提升拖动优先级
      this.$el.style.pointerEvents = "auto";
    },

    // 拖动核心：实时更新，无延迟
    drag(event) {
      const { index, startX, startY } = this.dragState;
      if (index === -1) return;
      const points = this.chartOption.points;
      const mouseCoords = this.getMouseCoords(event);


      let newCx = mouseCoords.x;
      let newCy = mouseCoords.y;

      if (newCx > (this.svgWidth - this.svgPadding)) {
        this.dragchartdata.width = this.dragchartdata.width + 20;
      }
      else if (newCx < this.svgPadding) {
        this.dragchartdata.x = this.dragchartdata.x - 20;
        this.dragchartdata.width = this.dragchartdata.width + 20;
      }

      if (newCy > (this.svgHeight - this.svgPadding)) {
        this.dragchartdata.height = this.dragchartdata.height + 20;
      }
      else if (newCy < this.svgPadding) {
        this.dragchartdata.y = this.dragchartdata.y - 20;
        this.dragchartdata.height = this.dragchartdata.height + 20;
      }



      // 边界限制
      newCx = Math.min(
        Math.max(newCx, this.svgPadding),
        this.svgWidth - this.svgPadding
      );

      newCy = Math.min(
        Math.max(newCy, this.svgPadding),
        this.svgHeight - this.svgPadding
      );

      this.$set(points, index, {
        cx: newCx,
        cy: newCy
      });


    },

    // 结束拖动
    endDrag() {
      this.dragState = {
        index: -1,
        startX: 0,
        startY: 0
      };
    },

    // 构建路径数据（实时更新，无延迟）
    getPathData() {
      const { points } = this.chartOption;
      if (!points || points.length < 2) return "";

      // 不做精度限制，优先保证实时性
      const pathSegments = points.map((point, i) =>
        i === 0 ? `M ${point.cx} ${point.cy}` : `L ${point.cx} ${point.cy}`
      );
      return pathSegments.join(" ");
    },

    // 构建反向路径数据
    getReversePathData() {
      const { points } = this.chartOption;
      if (!points || points.length < 2) return "";

      const reversedPoints = [...points].reverse();
      const pathSegments = reversedPoints.map((point, i) =>
        i === 0 ? `M ${point.cx} ${point.cy}` : `L ${point.cx} ${point.cy}`
      );
      return pathSegments.join(" ");
    },
  },
};
</script>
<style lang="less" scoped>
.svg_container {
  user-select: none;
  -webkit-user-select: none;
  overflow: hidden;
  /* 避免SVG超出容器 */
}

svg {
  display: block;
}

path {
  fill: none !important;
  transition: none;
  /* 禁用路径过渡，提升实时性 */
}

.circle {
  display: none;
  cursor: move;
  pointer-events: all;
  z-index: 1000;
  /* 确保控制点在最上层 */
  transition: fill 0.1s ease, stroke 0.1s ease;
  /* 只保留颜色过渡 */
}

/* 鼠标悬浮在SVG上时显示所有控制点 */
svg:hover .circle {
  display: block;
}

/* hover状态的颜色变化（通过JS控制大小和边框宽度） */
.circle:hover {
  fill: #409eff;
  stroke: #1890ff;
}
</style>