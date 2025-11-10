<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item  title="标题" name="2">
          <el-form-item v-if="configData.chartOption.title.text!==undefined" label="标题">
            <el-input v-model="configData.chartOption.title.text" placeholder="请输入标题" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.x!==undefined" label="标题位置">
            <el-select v-model="configData.chartOption.title.x" placeholder="请选择">
              <el-option label="居左" value="left"></el-option>
              <el-option label="居中" value="center"></el-option>
              <el-option label="居右" value="right"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.fontSize!==undefined" label="标题字号">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.title.textStyle.fontSize"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.fontWeight!==undefined" label="标题粗细">
            <el-select v-model="configData.chartOption.title.textStyle.fontWeight" placeholder="请选择">
              <el-option label="normal" value="normal"></el-option>
              <el-option label="bold" value="bold"></el-option>
              <el-option label="bolder" value="bolder"></el-option>
              <el-option label="lighter" value="lighter"></el-option>
            </el-select> 
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.fontFamily!==undefined" label="字体样式">
            <el-select v-model="configData.chartOption.title.textStyle.fontFamily" placeholder="请选择">
              <el-option
                v-for="(item,index) in fontFamilys"
                :key="index"
                :label="item"
                :value="item">
              </el-option>
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="轮廓" name="3">
          <el-form-item label="展示轮廓">
            <el-switch v-model="configData.chartOption.series[0].outline.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].outline.show===true" label="轮廓颜色">
            <el-color-picker v-model="configData.chartOption.series[0].outline.itemStyle.borderColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].outline.show===true" label="轮廓间距">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].outline.itemStyle.borderWidth"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].outline.show===true" label="间距颜色">
            <el-color-picker v-model="configData.chartOption.series[0].outline.itemStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].outline.show===true" label="轮廓粗细">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].outline.itemStyle.borderWidth"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].outline.show===true" label="阴影范围">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].outline.itemStyle.shadowBlur"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].outline.show===true" label="阴影颜色">
            <el-color-picker v-model="configData.chartOption.series[0].outline.itemStyle.shadowColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="水球" name="4">
          <el-form-item v-if="configData.chartOption.series[0].shape!==undefined" label="水球形状">
              <el-select v-model="configData.chartOption.series[0].shape" placeholder="请选择">
                <el-option label="圆形" value="circle"></el-option>
                <el-option label="正方形" value="rect"></el-option>
                <el-option label="方圆形" value="roundRect"></el-option>
                <el-option label="三角形" value="triangle"></el-option>
                <el-option label="钻石" value="diamond"></el-option>
                <el-option label="气泡" value="pin"></el-option>
                <el-option label="箭头" value="arrow"></el-option>
                <el-option label="海豚" value="path://M367.855,428.202c-3.674-1.385-7.452-1.966-11.146-1.794c0.659-2.922,0.844-5.85,0.58-8.719 c-0.937-10.407-7.663-19.864-18.063-23.834c-10.697-4.043-22.298-1.168-29.902,6.403c3.015,0.026,6.074,0.594,9.035,1.728 c13.626,5.151,20.465,20.379,15.32,34.004c-1.905,5.02-5.177,9.115-9.22,12.05c-6.951,4.992-16.19,6.536-24.777,3.271 c-13.625-5.137-20.471-20.371-15.32-34.004c0.673-1.768,1.523-3.423,2.526-4.992h-0.014c0,0,0,0,0,0.014 c4.386-6.853,8.145-14.279,11.146-22.187c23.294-61.505-7.689-130.278-69.215-153.579c-61.532-23.293-130.279,7.69-153.579,69.202 c-6.371,16.785-8.679,34.097-7.426,50.901c0.026,0.554,0.079,1.121,0.132,1.688c4.973,57.107,41.767,109.148,98.945,130.793 c58.162,22.008,121.303,6.529,162.839-34.465c7.103-6.893,17.826-9.444,27.679-5.719c11.858,4.491,18.565,16.6,16.719,28.643 c4.438-3.126,8.033-7.564,10.117-13.045C389.751,449.992,382.411,433.709,367.855,428.202z"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.series[0].backgroundStyle.borderColor!==undefined" label="边框颜色">
              <el-color-picker v-model="configData.chartOption.series[0].backgroundStyle.borderColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.series[0].backgroundStyle.borderWidth!==undefined" label="边框粗细">
              <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].backgroundStyle.borderWidth"></el-input-number>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.series[0].backgroundStyle.color!==undefined" label="水球底色">
              <el-color-picker v-model="configData.chartOption.series[0].backgroundStyle.color" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.series[0].backgroundStyle.shadowBlur!==undefined" label="阴影范围">
              <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].backgroundStyle.shadowBlur"></el-input-number>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.series[0].backgroundStyle.shadowColor!==undefined" label="阴影颜色">
              <el-color-picker v-model="configData.chartOption.series[0].backgroundStyle.shadowColor" show-alpha></el-color-picker>
            </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="水纹" name="5">
          <el-form-item label="是否滚动">
            <el-switch v-model="configData.chartOption.series[0].waveAnimation" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].waveLength!==undefined" label="水波数量">
            <el-slider v-model="configData.chartOption.series[0].waveLength" :min="50" :max="500" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-show="configData.chartOption.series[0].direction!==undefined" label="滚动方向">
            <el-radio-group v-model="configData.chartOption.series[0].direction">
              <el-radio label="left">左边</el-radio>
              <el-radio label="right">右边</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].emphasis.itemStyle.opacity!==undefined" label="触发焦点透明度">
            <el-slider v-model="configData.chartOption.series[0].emphasis.itemStyle.opacity" :min="0" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="数值" name="6">
          <el-form-item v-if="configData.chartOption.series[0].label.fontSize!==undefined" label="数值字号">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].label.fontSize"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].label.fontFamily!==undefined" label="数值样式">
            <el-select v-model="configData.chartOption.series[0].label.fontFamily" placeholder="请选择">
              <el-option
                v-for="(item,index) in fontFamilys"
                :key="index"
                :label="item"
                :value="item">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].label.fontWeight!==undefined" label="数值粗细">
            <el-select v-model="configData.chartOption.series[0].label.fontWeight" placeholder="请选择">
              <el-option label="normal" value="normal"></el-option>
              <el-option label="bold" value="bold"></el-option>
              <el-option label="bolder" value="bolder"></el-option>
              <el-option label="lighter" value="lighter"></el-option>
            </el-select> 
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].label.color!==undefined" label="数值颜色">
            <el-color-picker v-model="configData.chartOption.series[0].label.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].label.insideColor!==undefined" label="水内数值颜色">
            <el-color-picker v-model="configData.chartOption.series[0].label.insideColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="7">
          <el-form-item v-if="configData.chartOption.animate !== undefined" label="载入动画">
            <el-select v-model="configData.chartOption.animate" placeholder="请选择">
              <el-option v-for="item in animateOptions" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
        </el-collapse-item>
      </el-collapse>
    </el-form>
</template>
<script>
import { animateOptions } from "../../../animate/animate";
import draggable from "vuedraggable";
export default {
  props: {
    configData: {
      type: Object,
      required: true
    },
  },
  components: {
    draggable
  },
  data() {
    return {
      activeNames: ["1"],
      fontFamilys: [],
      animateOptions,
    }
  },
  methods: {}
}
</script>
<style lang="scss" scoped>
::v-deep {
  .el-input-numbe, .el-input-number--small{
    width: 100% !important;
  }
  .el-input--medium .el-input__inner {
    height: 32px;
    width: 100%;
  }
  .inputFontSize{
    width: 100%;
  }
  .el-select{
    height: 32px;
    width: 100%;
  }
  .el-input--suffix, .el-input__inner{
    height: 32px;
  }
  .el-select .el-input__icon {
    line-height: 32px; //el-select 改了多高，这边多高
  }
  .lableText .el-form-item__label{
      float: none;
  }
}
</style>