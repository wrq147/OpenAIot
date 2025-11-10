<template>
  <div :style="{'height':bodyConHei+'px','position': 'relative','padding-bottom':'70px'}">
    <div class="scale">
      <el-button
        icon="el-icon-plus"
        size="small"
        @click="scale += 10"
        :disabled="scale >= 150"
        circle
      ></el-button>
      <span>{{ scale }}%</span>
      <el-button
        icon="el-icon-minus"
        size="small"
        @click="scale -= 10"
        :disabled="scale <= 40"
        circle
      ></el-button>
      <!--      <el-button @click="validate">校验流程</el-button>-->
    </div>
    <!-- <div class="design" :style="'transform: scale('+ scale / 100 +');'">
      <process-tree ref="process-tree" @selectedNode="nodeSelected" />
    </div>-->
    <div style="width: 100%;height: 100%;box-sizing: border-box;position: relative;">
      <div style="display: flex;transform-origin: 50% 0px 0px;height: 100%;">
        <div v-dragscroll class="vue-drag-scroll-out-wrapper">
          <div class="vue-drag-scroll-wrapper" :style="zoomStye" style="box-sizing: border-box;">
            <div style="display: flex;">
              <process-tree ref="process-tree" @selectedNode="nodeSelected" />

              <data></data>
            </div>
          </div>
        </div>
      </div>
    </div>

    <el-drawer
      :title="selectedNode.name"
      :visible.sync="showConfig"
      :modal-append-to-body="false"
      :size="selectedNode.type === 'CONDITION' ? '600px':'500px'"
      direction="rtl"
      :modal="false"
      destroy-on-close
    >
      <div slot="title">
        <el-input
          v-model="selectedNode.name"
          size="medium"
          v-show="showInput"
          style="width: 300px"
          @blur="showInput = false"
        ></el-input>
        <el-link v-show="!showInput" @click="showInput = true" style="font-size: medium">
          <i class="el-icon-edit" style="margin-right: 10px"></i>
          {{selectedNode.name}}
        </el-link>
      </div>
      <div class="node-config-content">
        <node-config />
      </div>
    </el-drawer>
  </div>
</template>

<script>
import ProcessTree from "./process/ProcessTree.vue";
import NodeConfig from "../../common/process/config/NodeConfig";

export default {
  name: "ProcessDesign",
  components: { ProcessTree, NodeConfig },
  data() {
    return {
      scale: 100,
      selected: {},
      showInput: false,
      showConfig: false,
      bodyConHei: 0 //最外层高度
    };
  },
  computed: {
    selectedNode() {
      return this.$store.state.flowable.selectedNode;
    },
    zoomStye() {
      let INIT_WIDTH = document.body.offsetWidth;
      let INIT_HEIGHT = document.body.offsetHeight - 38;
      let width = INIT_WIDTH * (1 + (100 - this.scale) / 100);
      let height = INIT_HEIGHT * (1 + (100 - this.scale) / 100);

      return {
        width: `${width}px`,
        height: `${height}px`,
        transform: `scale(${this.scale / 100})`,
        "padding-top": `${(60 * this.scale) / 100}px`,
        "padding-bottom": `${(30 * this.scale) / 100}px`,
        "padding-left": `${(50 * this.scale) / 100}px`,
        "padding-right": `${(30 * this.scale) / 100}px`
      };
    }
  },
  mounted() {
    let div3 = document.getElementById("app-main");
    this.bodyConHei = div3.offsetHeight;
  },
  methods: {
    validate() {
      return this.$refs["process-tree"].validateProcess();
    },
    nodeSelected(node) {
      console.log("配置节点", node);
      this.showConfig = true;
    }
  },
  watch: {
    /*selectedNode:{
      deep: true,
      handler(node){
        console.log("更新")
        this.$refs["process-tree"].nodeDomUpdate(node)
      }
    }*/
  }
};
</script>

<style lang="less" scoped>
.vue-drag-scroll-out-wrapper {
  overflow-x: hidden;
  width: 100%;
  height: 100%;
  cursor: grab;
  // position: absolute;
  // top: 40px;
  // left: 0;
  &::-webkit-scrollbar {
    width: 0 !important;
  } // 隐藏垂直方向的滚动条
}
.design {
  margin-top: 100px;
  display: flex;
  transform-origin: 50% 0px 0px;
}

.scale {
  z-index: 999;
  position: fixed;
  top: 200px;
  right: 40px;

  span {
    margin: 0 10px;
    font-size: 15px;
    color: #7a7a7a;
    width: 50px;
  }
}

.node-config-content {
  padding: 0 20px 20px;
}

/deep/ .el-drawer__body {
  overflow-y: auto;
}
</style>
