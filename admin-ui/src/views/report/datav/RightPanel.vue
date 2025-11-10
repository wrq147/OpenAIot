<template>
  <component :is="ComponentName" :themeForm="themeForm" :costomData="costomData" :drawingList="drawingList" :key="costomData.customId"></component>
</template> 

<script>

import VueEvent from './VueEvent'
import configComponents from './ComponentsConfigExport'

export default {
  components: configComponents,
  props: ["activeData","drawingList","themeForm"],
  data() {
    return {
      costomData: this.activeData,
    }
  },
  //页面加载完执行
  mounted() {
    VueEvent.$on("to_activated_msg", data=>{
      this.costomData = data;
    })
  },
  computed: {
    GlobalForm(){
      console.info(this.themeForm)
      return this.themeForm;
    },
    ComponentName(){
      if(this.costomData.chartType == "themeForm" || this.costomData.chartType === "group"){
        return "ThemeConfig";
      }
      else if(this.costomData.chartType == "lamp"){
        return "LampTextConfig";
      }
      else{
        return this.costomData.chartType+"Config";
      }
    }
  },
  watch: {
    costomData: {
      deep: true,
      handler(newVal, oldValue) {
        //console.log("3=>", newVal, oldValue);
        if(newVal.customId != oldValue.customId) {
          this.$emit("activeChange", newVal);
        }
      }
    },
    activeData: {
      deep: true,
      handler(newVal) {
        this.costomData = newVal;
      }
    }
  },
  methods: {
    costomChange(value) {
      //console.log("2=>", value);
      this.costomData = value;
    }
  }
}
</script>

<style lang="scss">
.right-board {
  width: 350px;
  position: absolute;
  right: 0;
  top: 0;
  padding-top: 3px;
  .field-box {
    position: relative;
    height: calc(100vh - 42px);
    box-sizing: border-box;
    overflow: hidden;
  }
  .right-scrollbar {
    height: 100%;
  }
}
.select-item {
  display: flex;
  border: 1px dashed #fff;
  box-sizing: border-box;
  & .close-btn {
    cursor: pointer;
    color: #f56c6c;
  }
  & .el-input + .el-input {
    margin-left: 4px;
  }
}
.select-item + .select-item {
  margin-top: 4px;
}
.select-item.sortable-chosen {
  border: 1px dashed #409eff;
}
.select-line-icon {
  line-height: 32px;
  font-size: 22px;
  padding: 0 4px;
  color: #777;
}
.option-drag {
  cursor: move;
}
.time-range {
  .el-date-editor {
    width: 227px;
  }
  ::v-deep .el-icon-time {
    display: none;
  }
}
.document-link {
  position: absolute;
  display: block;
  width: 26px;
  height: 26px;
  top: 0;
  left: 0;
  cursor: pointer;
  background: #409eff;
  z-index: 1;
  border-radius: 0 0 6px 0;
  text-align: center;
  line-height: 26px;
  color: #fff;
  font-size: 18px;
}
.node-label{
  font-size: 14px;
}
.node-icon{
  color: #bebfc3;
}
</style>
