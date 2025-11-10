<template>
  <div>
    <el-select
      class="undergroundMap_top_select"
      v-model="filter.keyword"
      clearable
      placeholder="请选择"
      @clear="handleClear"
      @focus="blurInput"
      ref="treeSelect"
      filterable
      :filter-method="filterMethod"
    >
      <el-option
        hidden
        key="upResId"
        :value="filter.keyword"
        :label="filter.upResName"
      >
      </el-option>
      <el-tree
        :data="treeData"
        :node-key="nodeKey"
        :props="defaultProps"
        :expand-on-click-node="false"
        :check-on-click-node="true"
        @node-click="handleNodeClick"
        ref="selecteltree"
        :filter-node-method="filterNode"
      >
        <span
          class="custom-tree-node"
          :title="node.label"
          slot-scope="{ node, data }"
        >
          <span :class="filter.keyword == data[nodeKey] ? 'name_active' : ''">
            {{ data[defaultProps.label] }}
          </span>
        </span>
      </el-tree>
    </el-select>
  </div>
</template>
<script>
export default {
  props: {
    defaultProps: {
      type: Object,
      default: () => {
        return {
          children: "children",
          label: "nodeName",
        };
      },
    },
    nodeKey:{
        type:String,
        default:'nodeId'
    },
    treeData: {
      type: Array,
      default: () => {
        return [];
      },
    },
  },
  data() {
    return {
      filter: {
        keyword: "",
        upResName: "",
      },
    };
  },
  methods: {
    filterMethod(value) {
      this.$refs["selecteltree"].filter(value.trim());
    },
    blurInput() {
      this.$refs["selecteltree"].filter("");
    },
    filterNode(value, data, node) {
      if (!value) return true;
      let parentNode = node.parent,
        labels = [node.label],
        level = 1;
      while (level < node.level) {
        labels = [...labels, parentNode.label];
        parentNode = parentNode.parent;
        level++;
      }
      return labels.some((label) => label.indexOf(value) !== -1);
      // if (data.video_code) return data.catalog_name.indexOf(value) !== -1;
    },
    // 节点点击事件
    handleNodeClick(data) {
      this.filter.upResName = data[this.defaultProps.label];
      this.filter.keyword = data[this.nodeKey];
      this.$emit('select',data)
      this.$refs.treeSelect.blur(); // 失去焦点 隐藏下拉
    },
    // 选择器配置可以清空选项，用户点击清空按钮时触发
    handleClear() {
      this.filter.upResName = "";
      this.filter.keyword = "";
      this.$emit('select',{})
    },
  },
};
</script>
<style lang="scss" scoped>
.name_active {
  color: #358efe;
}
</style>
