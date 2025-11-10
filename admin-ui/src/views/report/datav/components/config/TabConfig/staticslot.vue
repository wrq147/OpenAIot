<template>
  <div>
    <el-form-item>
      <div slot="label">
        <span style="margin-right: 10px">显示数据</span>
      </div>
      <div style="margin-top: -10px">
        <el-button type="text" @click="addItem()">+ 添加</el-button>
      </div>

      <div
        v-for="(tmpitem, index) in configData.chartOption.staticDataValue"
        :key="'a' + index"
        style="margin-bottom: 10px"
      >
        <el-input
          placeholder="bindid"
          size="small"
          v-model="configData.chartOption.staticDataValue[index].bindid"
          style="width: calc(50% - 24px)"
        />
        <span>-</span>
        <el-input
          placeholder="内容"
          size="small"
          v-model="configData.chartOption.staticDataValue[index].content"
          style="width: calc(50% - 24px)"
        />
        <el-button
          style="margin-left: 10px"
          size="mini"
          @click="delItem(index)"
          type="danger"
          icon="el-icon-delete"
          circle
        ></el-button>
      </div>
    </el-form-item>
  </div>
</template>
<script>
export default {
  props: {
    configData: {
      type: Object,
    },
  },
  data() {
    return {};
  },
  methods: {
    addItem() {
      this.configData.chartOption.staticDataValue.push({ bindid: "", content: '' });
    },
    delItem(idx) {
      this.configData.chartOption.staticDataValue.splice(idx, 1);
    },
  },
};
</script>
<style lang="scss" scoped>
</style>