<template>
  <g @contextmenu.prevent="onContextmenu($event)">
    <!-- 主路径 -->
    <path @mousedown.stop="itemSelected()" :id="'line' + chartOption.bindingDiv" :d="getPathData()"
      :stroke="chartOption.lineColor" :stroke-width="chartOption.lineWidth" fill="none"
      marker-end="url(#marker-end-849d8617-602c-404f-8b13-b8aa2b883ddd)" stroke-dashoffset="0"
      :stroke-dasharray="chartOption.animateType == 'eleCurrent' ? chartOption.dasharray : 0" stroke-linejoin="round">
      <animate v-if="chartOption.isReverseAnimation" attributeName="stroke-dashoffset" from="0" to="1000"
        :dur="chartOption.delayTime + 's'" repeatCount="indefinite"></animate>
      <animate v-else attributeName="stroke-dashoffset" from="1000" to="0" :dur="chartOption.delayTime + 's'"
        repeatCount="indefinite"></animate>
    </path>

    <!-- 流动路径 -->
    <path @mousedown.stop="itemSelected()" :id="'line2' + chartOption.bindingDiv" :d="getPathData()"
      :stroke="chartOption.flowColor" :stroke-width="chartOption.flowWidth" fill-opacity="0" fill="none"
      :stroke-dasharray="chartOption.dasharray" stroke-dashoffset="0" stroke-linecap="round"
      v-if="chartOption.animateType == 'droplet'&&chartOption.staticDataValue[0].enable" stroke-linejoin="round">
      <animate v-if="chartOption.isReverseAnimation" attributeName="stroke-dashoffset" from="0" to="1000"
        :dur="chartOption.delayTime + 's'" repeatCount="indefinite"></animate>
      <animate v-else attributeName="stroke-dashoffset" from="1000" to="0" :dur="chartOption.delayTime + 's'"
        repeatCount="indefinite"></animate>
    </path>

    <!-- 跟踪动画 -->
    <circle v-if="chartOption.animateType == 'track'&&chartOption.staticDataValue[0].enable" cx="0" cy="0" :r="chartOption.radius"
      :fill="chartOption.radiusFillColor">
      <animateMotion v-if="chartOption.isReverseAnimation" :path="getReversePathData()"
        :dur="chartOption.delayTime + 's'" repeatCount="indefinite"></animateMotion>
      <animateMotion v-else :path="getPathData()" :dur="chartOption.delayTime + 's'" repeatCount="indefinite">
      </animateMotion>
    </circle>

    <!-- 动态渲染控制点：用动态属性控制大小，替代CSS修改r -->
    <circle v-for="(point, index) in chartOption.points" :key="`point${index}-${chartOption.bindingDiv}`" v-if="isDraw"
      @mousedown.stop="startDrag(index, $event)" :id="`point${index}${chartOption.bindingDiv}`" :cx="point.cx"
      :cy="point.cy" :r="isHoverPoint(index) ? 9 : 8" fill="white" stroke="black"
      :stroke-width="isHoverPoint(index) ? 3 : 2" :class="{ 'circle': true, 'circle-selected': isSelected }"
      @mouseenter="setHoverIndex(index)" @mouseleave="clearHoverIndex()" />
  </g>
</template>

<script>
import resize from "@/views/dashboard/mixins/resize";
import dataChart from "../mixins/dataChart.js";
import VueEvent from "../../VueEvent";
export default {
  mixins: [resize, dataChart],
  props: {
    className: {
      type: String,
      default: "chart",
    },
    drawingList: {
      type: Array,
    },
    dragchartdata: {
      type: Object
    }
  },
  data() {
    return {
      isSelected: false,
      svgPadding: 10,
      dragState: {
        index: -1,
        startX: 0,
        startY: 0
      },
      hoverPointIndex: -1, // 当前hover的点索引（用于动态控制大小）
    };
  },
  watch: {
    "chartOption.theme": {
      handler() { },
    },
  },
  mounted() {
    this.valUpdate = (result) => { };
    VueEvent.$on("SvgMove", this.drag);
    VueEvent.$on("SvgEndMove", this.endDrag);
    VueEvent.$on("SvgBgClick", this.bgClick);

  },
  beforeDestroy() {
    VueEvent.$off("SvgMove", this.drag);
    VueEvent.$off("SvgEndMove", this.endDrag);
    VueEvent.$off("SvgBgClick", this.bgClick);
  },
  methods: {

    //右键菜单
    onContextmenu(event) {
      this.$contextmenu({
        items: [
          {
            label: "删除组件",
            icon: "el-icon-delete",
            onClick: () => {
              VueEvent.$emit("delete_component", this.dragchartdata.customId);
            }
          },
          {
            label: "新增控制点",
            icon: "el-icon-arrow-up",
            onClick: () => {
              const lastidx = this.dragchartdata.chartOption.points.length - 1;
              const lastPoint = this.dragchartdata.chartOption.points[lastidx];

              // 新增点的默认位置
              const newPoint = {
                cx: lastPoint.cx - 20,
                cy: lastPoint.cy,
              };
              // 响应式添加到数组
              this.dragchartdata.chartOption.points.splice(lastidx, 0, newPoint);
            }
          },
          {
            label: "删除控制点",
            icon: "el-icon-arrow-down",
            onClick: () => {
              if (this.dragchartdata.chartOption.points.length > 2) {
                const lastidx = this.dragchartdata.chartOption.points.length - 2;
                this.dragchartdata.chartOption.points.splice(lastidx, 1);
              }
            }
          }
        ],
        event,
        customClass: "custom-class",
        zIndex: 3,
        minWidth: 230
      });
      return false;
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
    getMouseCoords(event, svgElem) {
      if (!svgElem) return { x: 0, y: 0 };

      const pt = svgElem.createSVGPoint();
      pt.x = event.clientX;
      pt.y = event.clientY;
      // 实时获取CTM矩阵（组件缩放后坐标正确）
      const ctm = svgElem.getScreenCTM();
      if (!ctm) return { x: 0, y: 0 };
      const svgCoords = pt.matrixTransform(ctm.inverse());
      return {
        x: svgCoords.x,
        y: svgCoords.y
      };
    },
    getParentSvg(el) {
      if (!el) return null;
      if (el.tagName.toLowerCase() === 'svg') return el;
      return this.getParentSvg(el.parentElement);
    },
    itemSelected() {
      this.$emit("activated");
      this.isSelected = true;
    },
    // 开始拖动：计算鼠标相对于点的偏移（解决点击位置不精准问题）
    startDrag(index, event) {
      let parentSvg = this.getParentSvg(event.target);
      const mouseCoords = this.getMouseCoords(event, parentSvg);
      this.dragState = {
        index,
        startX: mouseCoords.x,
        startY: mouseCoords.y
      };

      event.preventDefault();
      // 提升拖动优先级
      this.$el.style.pointerEvents = "auto";
      this.$emit("activated");
      this.isSelected = true;
    },

    // 拖动核心：实时更新，无延迟
    drag(event) {
      if (this.dragState == null) { return; }
      const { index, startX, startY } = this.dragState;
      if (index === -1) return;
      const points = this.chartOption.points;
      let parentSvg = this.getParentSvg(event.target);
      const mouseCoords = this.getMouseCoords(event, parentSvg);

      let newCx = mouseCoords.x;
      let newCy = mouseCoords.y;

      this.$set(points, index, {
        cx: newCx,
        cy: newCy
      });


    },

    // 结束拖动
    endDrag(event) {
      this.dragState = {
        index: -1,
        startX: 0,
        startY: 0
      };
    },
    bgClick() {
      this.isSelected = false;
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
  display: block;
  cursor: move;
  pointer-events: all;
  z-index: 1000;
  /* 确保控制点在最上层 */
  transition: fill 0.1s ease, stroke 0.1s ease;
  /* 只保留颜色过渡 */
}

.circle-selected {
  fill: #409eff;
  stroke: #1890ff;
}
</style>