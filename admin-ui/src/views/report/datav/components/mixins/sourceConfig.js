export default {
  props: ["costomData", "drawingList", "themeForm"],
  data() {
    return {
      configData: this.costomData,
      arrName: this.costomData != null ? this.costomData.chartOption.arrName : '',
      isUpdatingFromCostomData: false
    }
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
      }
    },
    costomData: {
      deep: true,
      handler(newVal) {
        this.isUpdatingFromCostomData = true;
        this.configData = newVal;
        this.arrName = newVal.chartOption.arrName;
      }
    },
  },
  methods: {
    changeSource(val) {
      this.$set(this.configData.chartOption, 'dataSourceType', val);
    },
    changeInteractData(val) {
      this.$set(this.configData.chartOption, 'interactData', val);
    },
    changeData(val) {
      this.$set(this.configData.chartOption, 'customData', val);
    }


  }
}
