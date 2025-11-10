<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标签" name="2">
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
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.title.textStyle.fontSize" />
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
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图例" name="3">
          <el-form-item  label="显示图例">
            <el-switch v-model="configData.chartOption.visualMap.show" />
          </el-form-item>
          <el-form-item  label="左侧距离">
            <el-slider v-model="configData.chartOption.visualMap.left" :min="0"  :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item  label="底部距离">
            <el-slider v-model="configData.chartOption.visualMap.bottom" :min="0"  :step="1" show-input></el-slider>
          </el-form-item>        
        </el-collapse-item>
        <el-collapse-item title="地图设置" name="4">
          <el-form-item v-if="configData.chartOption.mapName!==undefined" label="地图类型">
            <el-select v-model="configData.chartOption.mapName" placeholder="请选择">
              <el-option v-for="item in provinceOptions" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
          <el-form-item label="缩放和平移">
            <el-select v-model="configData.chartOption.roam" placeholder="请选择">
              <el-option label="开启缩放和平移" :value="true"></el-option>
              <el-option label="只允许缩放" value="scale"></el-option>
              <el-option label="只允许平移" value="move"></el-option>
              <el-option label="禁止缩放和平移" :value="false"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="区域颜色">
            <el-color-picker v-model="configData.chartOption.itemStyle.normal.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item  label="描边颜色">
            <el-color-picker v-model="configData.chartOption.itemStyle.normal.borderColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item  label="描边宽度">
            <el-slider v-model="configData.chartOption.itemStyle.normal.borderWidth" :min="0"  :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item  label="阴影颜色">
            <el-color-picker v-model="configData.chartOption.itemStyle.emphasis.color" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="散点设置" name="5">
          <el-form-item v-if="configData.chartOption.suffix!==undefined" label="单位">
            <el-input v-model="configData.chartOption.suffix" placeholder="请输入单位" />
          </el-form-item>
          <el-form-item label="标签位置">
            <el-select v-model="configData.chartOption.label.position" placeholder="请选择">
              <el-option label="上" value="top"></el-option>
              <el-option label="下" value="bottom"></el-option>
              <el-option label="左" value="left"></el-option>
              <el-option label="右" value="right"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item  label="字体大小">
            <el-slider v-model="configData.chartOption.label.fontSize" :min="0"  :step="1" :max="200" show-input></el-slider>
          </el-form-item>
          <el-form-item  label="标签颜色">
            <el-color-picker v-model="configData.chartOption.label.color" show-alpha/>
          </el-form-item>
          <el-form-item  label="提示框字体大小">
            <el-slider v-model="configData.chartOption.tooltip.fontSize" :min="0"  :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item  label="波纹时长">
            <el-slider v-model="configData.chartOption.rippleEffect.period" :min="0"  :step="1" :max="30" show-input></el-slider>
          </el-form-item>
          <el-form-item  label="波纹半径">
            <el-slider v-model="configData.chartOption.rippleEffect.scale" :min="0"  :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item  label="自动设置散点大小">
            <el-switch v-model="configData.chartOption.symbol.auto" />
          </el-form-item>
          <el-form-item  v-show="configData.chartOption.symbol.auto===true" label="大小系数">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.symbol.coefficient" controls-position="right" :step="0.1" />
          </el-form-item>
          <el-form-item  v-show="configData.chartOption.symbol.auto===false" label="散点大小">
            <el-slider v-model="configData.chartOption.symbol.size" :min="0"  :step="1" :max="200" show-input></el-slider>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="轨迹设置" name="6">
          <el-form-item label="图标样式">
            <el-select v-model="configData.chartOption.lines.effect.symbol" placeholder="请选择">
                <el-option label="箭头" value="arrow"></el-option>
                <el-option label="圆形" value="circle"></el-option>
                <el-option label="方形" value="rect"></el-option>
                <el-option label="三角形" value="triangle"></el-option>
                <el-option label="菱形" value="diamond"></el-option>
                <el-option label="椭圆形" value="pin"></el-option>
                <el-option label="无" value="none"></el-option>
              </el-select>
          </el-form-item>
          <el-form-item  label="图标大小">
            <el-slider v-model="configData.chartOption.lines.effect.symbolSize" :min="0"  :step="1" show-input />
          </el-form-item>
          <el-form-item  label="指向速度">
            <el-slider v-model="configData.chartOption.lines.effect.period" :min="0"  :step="1" show-input />
          </el-form-item>
          <el-form-item  label="尾迹长度">
            <el-slider v-model="configData.chartOption.lines.effect.trailLength" :min="0"  :step="0.1" :max="1" show-input />
          </el-form-item>
          <el-form-item  label="尾迹线条宽度">
            <el-slider v-model="configData.chartOption.lines.lineStyle.normal.width" :min="0"  :step="1" show-input />
          </el-form-item>
          <el-form-item  label="尾迹线条透明度">
            <el-slider v-model="configData.chartOption.lines.lineStyle.normal.opacity" :min="0"  :step="0.1" :max="1" show-input />
          </el-form-item>
          <el-form-item  label="尾迹线条曲直度">
            <el-slider v-model="configData.chartOption.lines.lineStyle.normal.curveness" :min="0"  :step="0.1" :max="1" show-input />
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
import draggable from 'vuedraggable'
import { animateOptions } from "../../../animate/animate";
import { provinceOptions } from '../../../util/map'
export default {
  props: {
    configData: {
      type: Object,
      required: true
    },
    costomData: {
      type: Object,
      required: true
    }
  },
  components: {
    draggable
  },
  data() {
    return {
      activeNames: ["1"],
      animateOptions,
      provinceOptions,
      fontFamilys: [],
      colors:this.costomData.chartOption.colors != undefined ?this.costomData.chartOption.colors:['#2884db', '#244779'],
      chartList: this.drawingList,
    }
  },
  methods: {
    getGeoPopImg(val){
      this.$set(this.configData.chartOption, 'popImg',  val);
    }
  }
}
</script>
<style lang="scss" scoped>
::v-deep {
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
    .el-slider{
      width: 95%;
    }
  }
  .delete-icon {
    line-height: 32px;
    font-size: 22px;
    padding: 0 4px;
    cursor: pointer;
    color: #f56c6c;
  }
</style>