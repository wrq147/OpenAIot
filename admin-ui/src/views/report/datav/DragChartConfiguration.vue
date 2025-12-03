<template>
  <vue-draggable-resizable :key="customId" :custom-id="customId" :w="width" :h="height" :x="x" :y="y"
    :z="dragchartdata.zindex" :parent="true" :debug="false" :min-width="20" :min-height="20" :isConflictCheck="false"
    :snap="true" :snapTolerance="20" :active="activeId == customId" :scaleRatio="scale" :selected="selected"
    :onCtrl="onCtrl" :ctrlSelectArr="ctrlSelectArr" :draggable="isUnlocked" :resizable="isUnlocked" :grid="[10, 10]"
    @dragging="onDrag" @resizing="onResize" @dragstop="onDragstop" @resizestop="onResizstop"
    @refLineParams="getRefLineParams" @activated="onActivated" @deactivated="onDeactivated" @rightClick="onContextmenu"
    class="test3" class-name-dragging="my-dragging-class" class-name-resizing="my-resizing-class"
    class-name-active="my-active-class" class-name-selected="ctrlSelected"
    :style="{ 'background-color': chartType == 'polyline' ? 'transparent' : '' }">
    <!-- 判断显示组件:handles="['tl','tr','bl','br']" -->
    <!--{{x}}-{{y}}-{{customId}}-->
    <component :activeId="activeId" @finishPointDraw="finishPointDraw" @clearSprite="clearSprite"
      :ref="'com' + customId" :scene="scene" :layer="layer" :x="x" :y="y" :z="dragchartdata.zindex + 1"
      :is="ComponentName" :theme="theme" :isDraw="true" :width="width + 'px'" :height="height + 'px'"
      :chartOption="chartOption" :className="chartOption.animate" :drawingList="drawingList" :customId="customId"
      :dragchartdata="dragchartdata">
    </component>
  </vue-draggable-resizable>

</template>

<script>
import VueDraggableResizable from "./components/drag/vue-draggable-resizable";
import "./components/drag/vue-draggable-resizable.css";

import VueEvent from './VueEvent'
import AllComponents from './ComponentsExport'

export default {
  components: {
    VueDraggableResizable,
    ...AllComponents
  },
  props: ["dragchartdata", "scale", "activeId", "drawingList", "onCtrl", "ctrlSelectArr", "theme", "layer", "scene"],
  data() {
    return {
      chartOption: this.dragchartdata.chartOption,
      chartType: this.dragchartdata.chartType,
      customId: this.dragchartdata.customId,
      width: this.dragchartdata.width,
      height: this.dragchartdata.height,
      x: this.dragchartdata.x,
      y: this.dragchartdata.y,
      selected: false,
      //是否没被锁定
      isUnlocked: this.dragchartdata.isUnlocked == undefined ? true : this.dragchartdata.isUnlocked,
    };
  },
  computed: {
    ComponentName() {
      console.log(this.chartType)
      if (this.chartType == "text") {
        return "NormalText";
      }
      else if (this.chartType == "lamp") {
        return "LampText";
      }
      else if (this.chartType == "date") {
        return "DateText";
      }
      else if (this.chartType == "textCheckBox") {
        return "TextCheckBox";
      }
      else {
        return this.chartType + "Chart";
      }
    }
  },
  watch: {
    "dragchartdata.chartOption": {
      deep: true,
      handler(newVal) {
        this.chartOption = newVal
      }
    },
    "dragchartdata.chartType": {
      deep: true,
      handler(newVal) {
        this.chartType = newVal
      }
    },
    "dragchartdata.customId": {
      deep: true,
      handler(newVal) {
        this.customId = newVal
      }
    },
    "dragchartdata.width": {
      deep: true,
      handler(newVal) {
        this.width = newVal
      }
    },
    "dragchartdata.height": {
      deep: true,
      handler(newVal) {
        this.height = newVal
      }
    },
    "dragchartdata.x": {
      deep: true,
      handler(newVal) {
        this.x = newVal
      }
    },
    "dragchartdata.y": {
      deep: true,
      handler(newVal) {
        this.y = newVal
      }
    },
    "dragchartdata.animate": {
      deep: true,
      handler(newVal) {
        this.animate = newVal
      }
    },
    "dragchartdata.isUnlocked": {
      deep: true,
      handler(newVal) {
        this.isUnlocked = newVal
      }
    },

    ctrlSelectArr: {
      deep: true,
      handler(newVal) {
        this.selected = newVal.indexOf(this.customId) > -1 ? true : false;
      }
    }
  },
  created() {
  },

  methods: {
    finishPointDraw() {
      this.$emit('finishPointDraw')
    },
    clearSprite() {
      this.$emit('clearSprite')
    },
    //右键菜单
    onContextmenu(event) {
      this.$contextmenu({
        items: [
          {
            label: "组合分组",
            icon: "el-icon-connection",
            onClick: () => {
              VueEvent.$emit("combine_component", this.ctrlSelectArr);
            }
          },
          {
            label: "复制图层",
            icon: "el-icon-document-copy",
            onClick: () => {
              VueEvent.$emit("copy_component", this.customId);
            }
          },
          {
            label: "删除图层",
            icon: "el-icon-delete",
            onClick: () => {
              let comid = 'com' + this.customId;
              if (this.$refs[comid].removeSprite) {
                this.$refs[comid].removeSprite();
              }

              if (this.ctrlSelectArr.length == 0) {
                VueEvent.$emit("delete_component", this.customId);
              }
              else {
                VueEvent.$emit("delete_component", this.activeId);
              }

            }
          },
          {
            label: "置顶图层",
            icon: "el-icon-arrow-up",
            onClick: () => {
              VueEvent.$emit("top_layer", this.customId);
            }
          },
          {
            label: "置底图层",
            icon: "el-icon-arrow-down",
            onClick: () => {
              VueEvent.$emit("bottom_layer", this.customId);
            }
          },
          {
            label: "上移一层",
            icon: "el-icon-top",
            onClick: () => {
              VueEvent.$emit("up_layer", this.customId);
            }
          },
          {
            label: "下移一层",
            icon: "el-icon-bottom",
            onClick: () => {
              VueEvent.$emit("down_layer", this.customId);
            }
          },
          {
            label: "收藏",
            icon: "el-icon-star-off",
            onClick: () => {
              VueEvent.$emit("collect", this.customId);
            }
          }
        ],
        event,
        //x: event.clientX,
        //y: event.clientY,
        customClass: "custom-class",
        zIndex: 3,
        minWidth: 230
      });
      return false;
    },
    onResize: function (x, y, width, height) {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
    },
    onDrag(x, y) {
      this.x = x;
      this.y = y;
      if (this.ctrlSelectArr.length > 0) {
        this.$emit("dragging", this.customId, x, y);
      }
      if (this.chartType == 'group') {
        VueEvent.$emit("combine_move", this.customId, x, y);
      }
    },
    onDragstop(x, y) {
      this.dragchartdata.x = x;
      this.dragchartdata.y = y;
      if (this.ctrlSelectArr.length > 0) {
        this.$emit("dragstop");
      }
      if (this.chartType == 'group') {
        VueEvent.$emit("combine_stop");
      }
    },
    onResizstop(x, y, width, height) {
      this.dragchartdata.x = x;
      this.dragchartdata.y = y;
      this.dragchartdata.width = width;
      this.dragchartdata.height = height;
    },
    handleSetLineChartData(type) {
      //this.lineChartData = lineChartData[type];
    },
    // 辅助线回调事件
    getRefLineParams(params) {

      this.$emit("getRefLineParams", params);

    },
    //点击控件
    onActivated(customId) {
      this.$emit("actived", customId);
      //将当前选中的元素信息发给RightPanel组件
      VueEvent.$emit("to_activated_msg", this.dragchartdata);
      //将当前选中的元素信息发给LayerItems组件
      VueEvent.$emit("to_layer_msg", this.dragchartdata);

    },
    //取消选中控件
    onDeactivated() {

    }
  },
};
</script>

<style lang="scss">
.test1 {
  background-color: rgb(239, 154, 154);
}

.test2 {
  background-color: rgb(129, 212, 250);
}

.test3 {
  background-color: rgba(255, 255, 255, 0);
}

.my-dragging-class {
  background-color: rgba(9, 97, 230, 0.89);
  border: 1px solid black;
}

.my-resizing-class {
  background-color: blue;
  border: 1px solid black;
  color: white;
}

.my-active-class {
  background-color: rgba(9, 97, 230, 0.89);
  border: 1px solid black;
}

.ctrlSelected {
  border: 1px solid rgba(9, 97, 230, 0.89);
}
</style>
