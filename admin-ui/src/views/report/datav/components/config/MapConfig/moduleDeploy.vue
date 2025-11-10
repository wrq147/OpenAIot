<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标题" name="2">
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
        <el-collapse-item title="功能样式" name="3">
          <el-form-item v-if="configData.chartOption.mapName!==undefined" label="地图类型">
            <el-select v-model="configData.chartOption.mapName" placeholder="请选择">
              <el-option
                v-for="item in provinceOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.suffix!==undefined" label="单位">
            <el-input v-model="configData.chartOption.suffix" placeholder="请输入单位" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.visualMap.show!==undefined" label="显示工具条">
            <el-switch v-model="configData.chartOption.visualMap.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isScatter!==undefined" label="显示散点">
            <el-switch v-model="configData.chartOption.isScatter" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isScatter == true && configData.chartOption.scatterSize!==undefined" label="散点大小">
            <el-slider v-model="configData.chartOption.scatterSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isGeo!==undefined" label="显示气泡">
            <el-switch v-model="configData.chartOption.isGeo" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isGeo == true && configData.chartOption.popSize!==undefined" label="气泡大小">
            <el-slider v-model="configData.chartOption.popSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isGeo ==true" label="气泡设置">
            <el-radio-group v-model="configData.chartOption.popORimg">
              <el-radio :label="'pop'">气泡图案</el-radio>
              <el-radio :label="'img'">自定义图片</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isGeo ==true && configData.chartOption.popORimg == 'pop' && configData.chartOption.areaColor!==undefined" label="气泡颜色">
            <el-color-picker v-model="configData.chartOption.popColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isGeo ==true && configData.chartOption.popORimg == 'img'" label="气泡图片">
            <image-gallary @getImg="getGeoPopImg"></image-gallary>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isTop!==undefined" label="显示排名">
            <el-switch v-model="configData.chartOption.isTop" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isTop!==undefined && configData.chartOption.isTop == true" label="排名位数">
            <el-input-number v-model="configData.chartOption.topNum" controls-position="right" :min="1" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="缩放和平移">
            <el-select v-model="configData.chartOption.roam" placeholder="请选择">
              <el-option label="开启缩放和平移" :value="true"></el-option>
              <el-option label="只允许缩放" value="scale"></el-option>
              <el-option label="只允许平移" value="move"></el-option>
              <el-option label="禁止缩放和平移" :value="false"></el-option>
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="配色" name="4">
          <el-form-item  label="应用主题">
            <el-switch v-model="configData.chartOption.isTopic" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isTopic===false" label="区域颜色">
            <el-color-picker v-model="configData.chartOption.areaColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isTopic===false" label="描边颜色">
            <el-color-picker v-model="configData.chartOption.borderColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isTopic===false" label="阴影颜色">
            <el-color-picker v-model="configData.chartOption.emphasisColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="5">
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
  data() {
    return {
      activeNames: ["1"],
      animateOptions,
      provinceOptions,
      fontFamilys: [],
      chartList: this.drawingList,
    }
  },
  methods: {
    getGeoPopImg(val){
      this.$set(this.configData.chartOption, 'popImg',  val);
    },
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
  }
  .delete-icon {
    line-height: 32px;
    font-size: 22px;
    padding: 0 4px;
    cursor: pointer;
    color: #f56c6c;
  }
</style>