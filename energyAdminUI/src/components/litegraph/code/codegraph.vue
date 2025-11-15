<template>
  <div style="padding:10px 10px 0 10px" id="big_con" v-loading="loading">
    <div style="display: flex;flex-direction: row;">
      <div style="background-color: #000;">
        <el-row style="background-color: #333;padding:15px 15px;width:1024px;border-bottom: solid 1px #444;">
          <el-button @click="resetView">重置视窗</el-button>
          <el-button v-if="graphStatus==1" @click="debugRun" type="success">模拟运行</el-button>
          <el-button v-else-if="graphStatus==2" @click="debugStop" type="danger">终止模拟</el-button>
        </el-row>
        <canvas id='mycanvas' width='1024' height='520'></canvas>
      </div>
      <div class="litePanel">
        <div class="litePanel-header">节点面板</div>
        <div id="NodeOptionsPanel"></div>
      </div>
    </div>
  </div>
</template>
<script>
import { LGraph, LGraphCanvas, LiteGraph } from "../litegraph.core.js";

export default {
  name: "codegraph",
  data() {
    return {
      loading: true,
      graphCanvas: null,
      graphObj: null,
      graphStatus: 1,
    };
  },
  computed: {
  },
  async mounted() {
    this.loading = true;
    this.graphObj = new LGraph()
    this.graphCanvas = new LGraphCanvas('#mycanvas', this.graphObj)
    this.$emit("InitNodes", LiteGraph, this.graphObj);

    this.loading = false;
  },
  destroyed(){
    this.graphObj.stop();
  },
  methods: {
    resetView() {
      this.graphCanvas.ds.reset();
      this.graphObj.change();
    },
    debugRun() {
      this.graphObj.start(1000);
      this.graphStatus = this.graphObj.status;
    },
    debugStop() {
      this.graphObj.stop();
      this.graphObj.change();
      this.graphStatus = this.graphObj.status;
    }
  }
};
</script>
<style lang="scss">
@import '../litegraph.css';

.litePanel {
  background-color: #333;
  width: 300px;
  border-left: solid 1px #444;

  .litePanel-header {
    background-color: #000;
    height: 40px;
    display: flex;
    align-items: center;
    padding: 0 15px;
    color: #fff;
    font-size: 16px;
    justify-content: center;
  }
}

.litegraph {
  .dialog-header {
    display: flex;
    height: 30px;
    align-items: center;
    padding: 0 15px;
    color: #fff;
    box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2);
    transition: 0.3s;
    background-color: #000;
    justify-content: space-between;

    .dialog-title {
      padding: 6px 15px;
      position: relative;
      background-color: #333;

      &::before {
        content: "";
        width: 40px;
        border-bottom: solid 1px #409EFF;
        position: absolute;
        bottom: 2px;
        left: 50%;
        transform: translateX(-50%);
      }
    }

    .close {
      cursor: pointer;
    }
  }

  .dialog-content {
    color: #fff;
    font-size: 12px;
    padding: 15px 15px;
    display: flex;
    flex-direction: column;

    >div {
      display: flex;
      flex-direction: column;

      h3 {
        margin: 20px 0 10px 0;
      }
    }

    .node_type {
      font-size: 14px;
      padding-bottom: 15px;
    }

    .node_desc {
      background-color: #bbb;
      color: #333;
      padding: 5px 10px;
      white-space: pre-wrap;
      line-height: 22px;
    }

    .property {

      display: flex;
      flex-direction: row;
      height: 40px;
      align-items: center;
      border-bottom: solid 1px #666;

      .property_name {
        width: 120px;
      }

      .property_value {
        flex: 1;
      }
    }
  }

  .dialog-footer {
    padding: 15px 15px;

    .btn {
      border: none;
      padding: 6px 17px;
      text-align: center;
      text-decoration: none;
      display: inline-block;
      font-size: 16px;
      border-radius: 4px;
      cursor: pointer;
    }

    .delete {
      background-color: #666;
      color: #fff;
    }
  }
}
</style>