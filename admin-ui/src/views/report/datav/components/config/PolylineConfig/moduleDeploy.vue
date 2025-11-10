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
          <el-form-item v-if="configData.chartOption.start.cx!==undefined" label="第一个点的横坐标">
            <el-slider v-model="configData.chartOption.start.cx" :min="rangeMaxMin.x.min" :step="1" :max="rangeMaxMin.x.max" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.start.cy!==undefined" label="第一个点的纵坐标">
            <el-slider v-model="configData.chartOption.start.cy" :min="rangeMaxMin.y.min" :step="1" :max="rangeMaxMin.y.max" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.mid1.cx!==undefined" label="第二个点的横坐标">
            <el-slider v-model="configData.chartOption.mid1.cx" :min="rangeMaxMin.x.min" :step="1" :max="rangeMaxMin.x.max" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.mid1.cy!==undefined" label="第二个点的横坐标">
            <el-slider v-model="configData.chartOption.mid1.cy" :min="rangeMaxMin.y.min" :step="1" :max="rangeMaxMin.y.max" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.mid2.cx!==undefined" label="第三个点的横坐标">
            <el-slider v-model="configData.chartOption.mid2.cx" :min="rangeMaxMin.x.min" :step="1" :max="rangeMaxMin.x.max" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.mid2.cy!==undefined" label="第三个点的横坐标">
            <el-slider v-model="configData.chartOption.mid2.cy" :min="rangeMaxMin.y.min" :step="1" :max="rangeMaxMin.y.max" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.end.cx!==undefined" label="第四个点的横坐标">
            <el-slider v-model="configData.chartOption.end.cx" :min="rangeMaxMin.x.min" :step="1" :max="rangeMaxMin.x.max" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.end.cy!==undefined" label="第四个点的横坐标">
            <el-slider v-model="configData.chartOption.end.cy" :min="rangeMaxMin.y.min" :step="1" :max="rangeMaxMin.y.max" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isAnimation!==undefined" label="是否是动画">
            <el-radio-group v-model="configData.chartOption.isAnimation">
              <el-radio :label="false">否</el-radio>
              <el-radio :label="true">是</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item label="是否是反转动画">
            <el-radio-group v-model="configData.chartOption.isReverseAnimation">
              <el-radio :label="false">否</el-radio>
              <el-radio :label="true">是</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.animateType!==undefined" label="动画类型">
            <el-select v-model="configData.chartOption.animateType" placeholder="请选择">
              <el-option label="电流" value="eleCurrent"></el-option>
              <el-option label="水珠" value="droplet"></el-option>
              <el-option label="轨迹" value="track"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.delayTime!==undefined" label="延迟时间">
            <el-slider v-model="configData.chartOption.delayTime" :min="0" :step="1" :max="100" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.lineWidth!==undefined" label="线宽">
            <el-slider v-model="configData.chartOption.lineWidth" :min="0" :step="1" :max="100" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.animateType=='droplet'&&configData.chartOption.flowWidth!==undefined" label="动画宽">
            <el-slider v-model="configData.chartOption.flowWidth" :min="0" :step="1" :max="100" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.animateType!='track'" label="线缝隙间隔">
            <el-slider v-model="configData.chartOption.dasharray" :min="0" :step="1" :max="100" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.animateType=='track'&&configData.chartOption.radius!==undefined" label="轨迹半径">
            <el-slider v-model="configData.chartOption.radius" :min="0" :step="1" :max="100" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.lineColor!==undefined" label="线颜色">
            <el-color-picker v-model="configData.chartOption.lineColor" show-alpha style="width: 32px;"></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.animateType=='droplet'&&configData.chartOption.flowColor!==undefined" label="动画颜色">
            <el-color-picker v-model="configData.chartOption.flowColor" show-alpha style="width: 32px;"></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.animateType=='track'&&configData.chartOption.radiusFillColor!==undefined" label="轨迹颜色">
            <el-color-picker v-model="configData.chartOption.radiusFillColor" show-alpha style="width: 32px;"></el-color-picker>
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
        // console.log(newVal,'newVal');
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
  computed: {
    rangeMaxMin(){
      let obj={}
      obj.x={}
      obj.y={}
      obj.x.min=0
      obj.x.max=Number(this.configData.width)
      obj.y.min=0
      obj.y.max=Number(this.configData.height)
      return obj
    }
  },
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