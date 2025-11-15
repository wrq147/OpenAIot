<template>
  <div>
    
  </div>
</template>

<script>
export default {
  name: "ConcurrentNodeConfig",
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      }
    }
  },
  computed: {
    selectedNode() {
      return this.$store.state.rulesFlowable.rulesSelectedNode;
    }
  },
  data() {
    return {
    };
  },
  mounted() {
  },
  methods: {}
};
</script>

<style lang="less">
.huancun_con {
  .el-form-item {
    label.el-form-item__label {
      font-size: 16px;
      text-align-last: justify;
      text-align: justify;
      text-justify: distribute-all-lines;
    }
    .el-form-item__content {
      // width: 100%;
      margin-left: 120px !important;
      .el-input-number.el-input-number--medium.is-controls-right {
        width: 227px;
      }
      input.el-input__inner {
        width: 227px;
      }
    }
  }
  .item-desc {
    color: rgba(61, 185, 143, 1);
    display: block;
    width: 80%;
    height: 36px;
    line-height: 36px;
    background-color: rgba(34, 46, 64, 1);
    text-align: left;
    margin-bottom: 10px;
    font-size: 14px;
    border-radius: 5px;
    border: 1px solid rgba(255, 255, 255, 0.2);
    padding-left: 30px;
    box-sizing: border-box;
  }
}
</style>
<style lang="less" scoped>
.choose {
  border-radius: 5px;
  margin-top: 2px;
  background: #f4f4f4;
  border: 1px dashed #1890ff !important;
}

.drag-hover {
  color: #1890ff;
}

.drag-no-choose {
  cursor: move;
  background: #f8f8f8;
  border-radius: 5px;
  margin: 5px 0;
  height: 25px;
  line-height: 25px;
  padding: 5px 10px;
  border: 1px solid #ffffff;
  div {
    display: inline-block;
    font-size: small !important;
  }

  div:nth-child(2) {
    float: right !important;
  }
}
</style>
