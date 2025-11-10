<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="90px">
        <div style="background-color: #f5f5f5;padding: 10px 15px;border: solid 1px #dadada;font-size: 14px;margin-bottom: 15px;color: #999;">
          事件预处理
        </div>
        <chart-interact :chartOption="configData.chartOption" @changeData="changeInteractData"></chart-interact>
        <div style="background-color: #f5f5f5;padding: 10px 15px;border: solid 1px #dadada;font-size: 14px;margin-bottom: 15px;color: #999;">
          事件发生后更新数据源
        </div>
        <el-form-item label="变更事件">
          <el-select v-model="configData.chartOption.dataList" multiple placeholder="请选择" @change="bindDataList">
            <el-option v-for="item in dataList" :key="item.name" :label="item.name" :value="item.name">
            </el-option>
          </el-select>
        </el-form-item>
      </el-form>
    </el-scrollbar>
  </div>
</template>

<script>
import ChartInteract from "../interact/ChartInteract";

export default {
  props: ["costomData", "themeForm"],
  components: {
    ChartInteract,
  },
  data() {
    return {
      configData: this.costomData,
      dataList: [],
    };
  },
  watch: {
    configData: {
      deep: true,
      handler(newVal,oldVal) {
        this.$emit("costom-change", newVal);
      }
    },
    costomData: {
      deep: true,
      handler(newVal) {
        this.configData = newVal;
      }
    },
  },
  //页面加载完执行
  mounted() {
    this.dataList = this.themeForm.globalData;
  },
  computed: {},
  methods: {
    changeInteractData(val) {
      this.$emit("changeData", val);
    },
    bindDataList(val) {
      this.$set(this.configData.chartOption, "dataList", val);
    },
  },
};
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
  width: 33%;
  text-align: center;
}
.dataProduct {
  margin-bottom: 10px;
  line-height: 45px;
  padding: 0 15px;
  background-color: #f5f5f5;
  color: #666;
}
</style>
