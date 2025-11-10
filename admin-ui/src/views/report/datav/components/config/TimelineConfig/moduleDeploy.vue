<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="90px" class="custom_form_item">

        <el-collapse  v-model="activeNames" accordion>
          
          <el-collapse-item title="图层" name="1">
            <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
            </el-form-item>
          </el-collapse-item>

        <el-collapse-item title="默认样式" name="2">

          <el-form-item v-if="configData.chartOption.suffix!==undefined" label="单位">
            <el-input v-model="configData.chartOption.suffix" placeholder="请输入单位" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.fontSize!==undefined" label="标签字体大小">
            <el-slider v-model="configData.chartOption.fontSize" :min="1" :max="50" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.fontColor!==undefined" label="标签字体颜色">
            <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.symbolSize!==undefined" label="圆点大小">
            <el-slider v-model="configData.chartOption.symbolSize" :min="1" :max="50" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.pointColor!==undefined" label="圆点颜色">
            <el-color-picker v-model="configData.chartOption.pointColor" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.lineColor!==undefined" label="时间轴颜色">
            <el-color-picker v-model="configData.chartOption.lineColor" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.buttonColor!==undefined" label="按钮颜色">
            <el-color-picker v-model="configData.chartOption.buttonColor" show-alpha></el-color-picker>
          </el-form-item>


        </el-collapse-item>

        <el-collapse-item title="选中样式" name="3">

          <el-form-item v-if="configData.chartOption.checkFontColor!==undefined" label="标签字体颜色">
            <el-color-picker v-model="configData.chartOption.checkFontColor" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.checkPointColor!==undefined" label="圆点颜色">
            <el-color-picker v-model="configData.chartOption.checkPointColor" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.borderColor!==undefined" label="边框颜色">
            <el-color-picker v-model="configData.chartOption.borderColor" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.borderWidth!==undefined" label="边框大小">
            <el-slider v-model="configData.chartOption.borderWidth" :min="1" :max="50" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.checkButtonColor!==undefined" label="按钮颜色">
            <el-color-picker v-model="configData.chartOption.checkButtonColor" show-alpha></el-color-picker>
          </el-form-item>


        </el-collapse-item>

        <el-collapse-item title="控制" name="4">

          <el-form-item v-if="configData.chartOption.autoPlay!==undefined" label="自动播放">
            <el-switch v-model="configData.chartOption.autoPlay" />
          </el-form-item>

          <el-form-item label="播放间隔">
            <el-input-number v-model="configData.chartOption.playInterval" controls-position="right"  :step="500"></el-input-number>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.showPlayBtn!==undefined" label="显示播放">
            <el-switch v-model="configData.chartOption.showPlayBtn" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.showNextBtn!==undefined" label="显示下一个">
            <el-switch v-model="configData.chartOption.showNextBtn" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.showPrevBtn!==undefined" label="显示上一个">
            <el-switch v-model="configData.chartOption.showPrevBtn" />
          </el-form-item>

        </el-collapse-item>

        <el-collapse-item title="绑定组件" name="5">
          
          <el-form-item v-if="configData.chartOption.aggrName!==undefined" label="参数名称">
            <el-input v-model="configData.chartOption.aggrName" placeholder="请输入参数名称" />
          </el-form-item>


          <el-form-item  label="绑定组件">
            <el-select v-model="configData.chartOption.bindList" multiple filterable placeholder="请选择" @change="bindCharts">
              <el-option
                  v-for="item in chartList"
                  :key="item.customId"
                  :label="item.layerName"
                  :value="item.customId">
                </el-option>
            </el-select>
          </el-form-item>

        </el-collapse-item>


        <el-collapse-item title="动画" name="6">
          
          <el-form-item v-if="configData.chartOption.animate!==undefined" label="载入动画">
            <el-select v-model="configData.chartOption.animate" placeholder="请选择">
              <el-option
                v-for="item in animateOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value">
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
import { getLinkChart} from "../../../util/LinkageChart";
export default {
  props: ["costomData","drawingList"],
  data() {
    return {
      fontFamilys:this.fontFamilys,
      fontWeights:['normal','bold','bolder','lighter'],
      types: [{label:'月范围',value:'monthrange'},{label:'日期范围',value:'daterange'},{label:'日期时间范围',value:'datetimerange'}],
      activeNames: ['1'],
      chartList: this.drawingList,
      animateOptions,
      configData: this.costomData,
    };
  },
  watch: {
    configData: {
      deep: true,
      handler(newVal, oldVal) {
        this.$emit("costom-change", newVal);
      },
    },
    costomData: {
      deep: true,
      handler(newVal) {
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
    bindCharts(val){
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