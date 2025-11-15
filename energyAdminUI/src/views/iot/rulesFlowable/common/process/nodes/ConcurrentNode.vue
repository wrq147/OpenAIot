<template>
  <div class="node">
    <div class="node-body">
      <div class="node-body-left" @click.stop="$emit('leftMove')" v-if="level > 1">
        <i class="el-icon-arrow-left"></i>
      </div>
      <div class="node-body-main">
        <div class="node-body-main-header">
          <span class="title">
            <i class="el-icon-s-operation"></i>
            <ellipsis class="name" hover-tip :content="config.name ? config.name : ('并行任务' + level)" />
          </span>
          <span class="option" style="margin-left: 10px;">
            <el-tooltip content="复制分支" placement="top">
              <i class="el-icon-copy-document" @click="$emit('copy')"></i>
            </el-tooltip>
            <i class="el-icon-close" @click.stop="$emit('delNode')"></i>
          </span>
        </div>
        <div class="node-body-main-content" @click="$emit('selected')">
          <span>并行任务（同时进行）</span>
        </div>
      </div>
      <div class="node-body-right" @click.stop="$emit('rightMove')" v-if="level < size">
        <i class="el-icon-arrow-right"></i>
      </div>
    </div>
    <div class="node-footer">
      <div class="btn">
        <!-- <insert-button @insertNode="type => $emit('insertNode', type)"></insert-button> -->
        <el-button icon="el-icon-plus" type="primary" size="small" @click="addNode" circle></el-button>
      </div>
    </div>
  </div>
</template>

<script>
import Ellipsis from '../../Ellipsis.vue'
// import InsertButton from '../../InsertButton.vue'

export default {
  name: "ConcurrentNode",
  components: { Ellipsis },
  props: {
    config: {
      type: Object,
      default: () => {
        return {}
      }
    },
    level: {
      type: Number,
      default: 1
    },
    //条件数
    size: {
      type: Number,
      default: 0
    }
  },
  computed: {
    selectedNode() {
      return this.$store.state.rulesFlowable.rulesSelectedNode;
    },
    setup() {
      return this.$store.state.rulesFlowable.rulesDesign;
    },
  },
  data() {
    return {
    }
  },
  mounted() {
  },
  methods: {
    addNode() {
      this.$store.commit(
        "rulesSelectedParentNode",
        this.config
      );
      this.$store.commit("setShowNodeList", true);
    }
  }
}
</script>

<style lang="scss" scoped>
@import "~@/assets/styles/element-variables.scss";

.node {
  padding: 30px 55px 0;
  width: 220px;

  .node-body {
    overflow: hidden;
    cursor: pointer;
    min-height: 80px;
    max-height: 120px;
    position: relative;
    border-radius: 5px;
    background-color: white;
    box-shadow: 0px 0px 5px 0px #d8d8d8;

    &:hover {

      .node-body-left,
      .node-body-right {
        i {
          display: block !important;
        }
      }

      .node-body-main {
        .option {
          display: inline-block !important;
        }
      }

      box-shadow: 0px 0px 3px 0px $--color-primary;
    }

    .node-body-left,
    .node-body-right {
      display: flex;
      align-items: center;
      position: absolute;
      height: 100%;

      i {
        display: none;
      }

      &:hover {
        background-color: #ececec;
      }
    }

    .node-body-left {
      left: 0;
    }

    .node-body-right {
      right: 0;
    }

    .node-body-main {
      position: absolute;
      width: 188px;
      left: 17px;
      display: inline-block;

      .node-body-main-header {
        padding: 10px 0px 5px;
        font-size: xx-small;
        display: flex;
        align-items: center;
        .el-checkbox__label{
          font-size: 12px !important;
        }
        .title {
          color: #718dff;
          display: flex;
          flex:1;
          width: 0;
          align-items: center;
          .name {
            display: inline-block;
            height: 14px;
            flex:1;
            width: 0;
            margin-left: 2px;
          }
        }

        .option {
          display: none;
          font-size: medium;

          i {
            color: #888888;
            padding: 0 3px;
          }
        }
      }

      .node-body-main-content {
        padding: 6px;
        color: #656363;
        font-size: 14px;
        padding-bottom: 15px;
        i {
          position: absolute;
          top: 55%;
          right: 10px;
          font-size: medium;
        }
      }
    }
  }

  .node-footer {
    position: relative;

    .btn {
      width: 100%;
      display: flex;
      height: 70px;
      padding: 20px 0 32px;
      justify-content: center;
    }

    ::v-deep .el-button {
      height: 32px;
    }

    &::before {
      content: "";
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      z-index: -1;
      margin: auto;
      width: 2px;
      height: 100%;
      background-color: #CACACA;
    }
  }
}</style>
