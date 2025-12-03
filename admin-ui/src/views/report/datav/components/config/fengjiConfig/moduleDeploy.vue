<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="90px" class="custom_form_item">

        <el-collapse v-model="activeNames" accordion>

          <el-collapse-item title="图层" name="1">
            <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="默认样式" name="2">

          </el-collapse-item>


          <el-collapse-item title="动画" name="6">

            <el-form-item v-if="configData.chartOption.animate !== undefined" label="载入动画">
              <el-select v-model="configData.chartOption.animate" placeholder="请选择">
                <el-option v-for="item in animateOptions" :key="item.value" :label="item.label" :value="item.value">
                </el-option>
              </el-select>
            </el-form-item>

          </el-collapse-item>

        </el-collapse>

      </el-form>
    </el-scrollbar>
  </div>
</template>

<script>
import { animateOptions } from "../../../animate/animate";
import { getLinkChart } from "../../../util/LinkageChart";
export default {
  props: ["costomData", "drawingList"],
  data() {
    return {
      fontFamilys: this.fontFamilys,
      fontWeights: ['normal', 'bold', 'bolder', 'lighter'],
      types: [{ label: '月范围', value: 'monthrange' }, { label: '日期范围', value: 'daterange' }, { label: '日期时间范围', value: 'datetimerange' }],
      activeNames: ['1'],
      chartList: this.drawingList,
      animateOptions,
      configData: this.costomData,
      isUpdatingFromCostomData: false
    };
  },
  watch: {
    configData: {
      deep: true,
      handler(newVal, oldVal) {
        if (this.isUpdatingFromCostomData) {
          this.isUpdatingFromCostomData = false;
          return;
        }
        this.$emit("costom-change", newVal);
      },
    },
    costomData: {
      deep: true,
      handler(newVal) {
        this.isUpdatingFromCostomData = true;
        this.configData = newVal;
      },
    },
  },
  //页面加载完执行
  mounted() {
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;
  },
  computed: {},
  methods: {
    bindCharts(val) {
      this.$set(this.configData.chartOption, 'bindList', val);
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