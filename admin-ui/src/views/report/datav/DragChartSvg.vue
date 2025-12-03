<template>
    <component :activeId="activeId"
      :is="ComponentName" :theme="theme" :isDraw="true"
      :chartOption="chartOption" :drawingList="drawingList" :customId="customId"
      :dragchartdata="dragchartdata"  @activated="onActivated">
    </component>
</template>

<script>
import VueEvent from './VueEvent'
import AllComponents from './ComponentsExport'

export default {
  components: {
    ...AllComponents
  },
  props: ["dragchartdata", "scale", "activeId", "drawingList", "theme"],
  data() {
    return {
      chartOption: this.dragchartdata.chartOption,
      chartType: this.dragchartdata.chartType,
      customId: this.dragchartdata.customId,
      selected: false
    };
  },
  computed: {
    ComponentName() {
      return this.chartType + "Chart";
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
    }
  },
  created() {
  },

  methods: {
    //点击控件
    onActivated() {
      this.$emit("actived", this.customId);
      //将当前选中的元素信息发给RightPanel组件
      VueEvent.$emit("to_activated_msg", this.dragchartdata);
      //将当前选中的元素信息发给LayerItems组件
      VueEvent.$emit("to_layer_msg", this.dragchartdata);
    },
  },
};
</script>

<style lang="scss">

</style>
