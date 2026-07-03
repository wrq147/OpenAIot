<template>
  <div>
    <div class="node-select">
      <div
        @click="addConditionsNode"
      >
        <i class="el-icon-share" style="color:rgb(21, 188, 131);"></i>
        <span>条件分支</span>
      </div>
      <div @click="addConcurrentsNode">
        <i class="el-icon-s-operation" style="color:#718dff;"></i>
        <span>并行分支</span>
      </div>
      <div @click="addDelayNode">
        <i class="el-icon-time" style="color:#f25643;"></i>
        <span>延迟等待</span>
      </div>
      <div @click="addClearDeltaNode">
        <i class="el-icon-delete" style="color:#940404af;"></i>
        <span>清除数据</span>
      </div>
      <div @click="addConversionNode">
        <i class="zhongtaiiconfont zhongtai-icon-shujuzhuanhuan" style="color:#FF943E;"></i>
        <span>数据转换</span>
      </div>
      <div @click="addDataWriteyNode">
        <i class="zhongtaiiconfont zhongtai-icon-shujuhuancun" style="color:#3296FA;"></i>
        <span>参数赋值</span>
      </div>
      <div @click="addTagNode">
        <i class="zhongtaiiconfont zhongtai-icon-shujuhuancun" style="color:#2d7577;"></i>
        <span>标签赋值</span>
      </div>
      <div @click="addSetPropNode" v-if="setup.TriggerWay==0">
        <i class="zhongtaiiconfont zhongtai-icon-shujuhuancun" style="color:#3d5520;"></i>
        <span>属性赋值</span>
      </div>
      <div @click="addMetronomeNode" v-if="setup.TriggerWay==0">
        <i class="zhongtaiiconfont zhongtai-icon-jiepaiqi" style="color:#FBC31F;"></i>
        <span>聚合数据</span>
      </div>
      <div @click="addMessageNode">
        <i class="zhongtaiiconfont zhongtai-icon-xiaoxijiedian" style="color:#E11013;"></i>
        <span>触发事件</span>
      </div>
      <div @click="addFuncNode">
        <i class="zhongtaiiconfont zhongtai-icon-shixucunchu" style="color:#182445;"></i>
        <span>功能节点</span>
      </div>
      <div @click="addTriggerNode">
        <i class="el-icon-set-up" style="color:#15BC83;"></i>
        <span>触发器</span>
      </div>
      <div @click="addExceptNode" v-if="setup.TriggerWay==0">
        <i class="el-icon-warning-outline" style="color:#E6A23C;"></i>
        <span>异常检测</span>
      </div>
      <div @click="addRedirectNode" v-if="setup.TriggerWay==0">
        <i class="zhongtaiiconfont zhongtai-icon-a-zhuanfajiedian" style="color:#C53EF3;"></i>
        <span>转发节点</span>
      </div>
      <div @click="addTimeSchedulerNode">
        <i class="el-icon-s-unfold" style="color:#6106f3;"></i>
        <span>设备调度</span>
      </div>
      <div @click="addPIDNode" v-if="setup.TriggerWay==2">
        <i class="zhongtaiiconfont zhongtai-icon-PIDjiedian" style="color:#2FCAEB;"></i>
        <span>PID控制</span>
      </div>
      <div @click="addNoticeNode">
        <i class="zhongtaiiconfont zhongtai-icon-chufatongzhi" style="color:#f25100;"></i>
        <span>触发通知</span>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "InsertButton",
  components: {},
  data() {
    return {
      isShowNodeList: false
    };
  },
  computed: {
    setup() {
      return this.$store.state.rulesFlowable.rulesDesign;
    }
  },
  mounted() {},
  methods: {
    addDelayNode() {
      this.$emit("insertNode", "DELAY");
    },
    addConditionsNode() {
      this.$emit("insertNode", "CONDITIONS");
    },
    addConcurrentsNode() {
      this.$emit("insertNode", "CONCURRENTS");
    },
    addTriggerNode() {
      this.$emit("insertNode", "TRIGGER");
    },
    addConversionNode() {
      this.$emit("insertNode", "CONVERSION");
    },
    addDataWriteyNode() {
      this.$emit("insertNode", "DATAWRITE");
    },
    addMetronomeNode() {
      this.$emit("insertNode", "METRONOME");
    },
    addMessageNode() {
      this.$emit("insertNode", "WARN");
    },
    addFuncNode() {
      this.$emit("insertNode", "FUNC");
    },
    addExceptNode() {
      this.$emit("insertNode", "EXCEPT");
    },
    addRedirectNode() {
      this.$emit("insertNode", "REDIRECT");
    },
    addClearDeltaNode(){
      this.$emit("insertNode", "CLEARDELTA");
    },
    addTimeSchedulerNode(){
      this.$emit("insertNode", "TIMESCHEDULER");
    },
    addPIDNode(){
      this.$emit("insertNode", "PID");
    },
    addNoticeNode(){
      this.$emit("insertNode", "NOTICE");
    },
    addTagNode(){
      this.$emit("insertNode", "TAG");
    },
    addSetPropNode(){
      this.$emit("insertNode", "SETPROP");
    }
  }
};
</script>

<style lang="less" scoped>
/deep/ .el-drawer__wrapper.setRulesList {
  position: fixed;
  // right: 0;
  left: 0;
  width: 400px;
  height: 100%;
  .el-drawer__container {
    width: 400px;
  }
  // .el-drawer__body {
  //   // padding: 20px;
  // }
}
.setRulesList {
  .el-drawer__container {
    position: fixed !important;
    top: 0;
    right: 0;
  }
  .el-drawer.rtl {
    position: fixed !important;
    top: 0;
    right: 0;
  }
}
.node-select {
  padding: 20px;
  div {
    display: inline-block;
    margin: 5px 10px;
    cursor: pointer;
    padding: 10px 15px;
    border: 1px solid #f8f9f9;
    background-color: #f8f9f9;
    border-radius: 10px;
    width: 130px;
    position: relative;
    vertical-align: top;
    font-size: 14px;
    box-sizing: border-box;
    span {
      position: absolute;
      left: 65px;
      top: 18px;
    }
    &:hover {
      background-color: #fff;
      box-shadow: 0 0 8px 2px #d6d6d6;
    }
    i {
      font-size: 25px;
      padding: 5px;
      border: 1px solid #dedfdf;
      border-radius: 14px;
      &.zhongtaiiconfont{
        font-style: normal;font-weight: 400;
        font-variant: normal;
        text-transform: none;
        line-height: 1;
        vertical-align: baseline;
        display: inline-block;
        -webkit-font-smoothing: antialiased;
      }
    }
    .svg-icon {
      width: 37px;
      height: 37px;
      box-sizing: border-box;
      font-size: 25px;
      padding: 5px;
      border: 1px solid #dedfdf;
      border-radius: 14px;
    }
  }
}
</style>
