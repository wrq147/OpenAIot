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
