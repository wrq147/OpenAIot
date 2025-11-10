<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标签" name="2">
          <el-form-item v-if="configData.chartOption.textStyle.fontSize!==undefined" label="标签字号">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.textStyle.fontSize"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.textStyle.color!==undefined" label="标签颜色">
            <el-color-picker v-model="configData.chartOption.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.textStyle.opacity!==undefined" label="标签透明度">
            <el-slider v-model="configData.chartOption.textStyle.opacity" :max="1" :min="0" :step="0.1"></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.textStyle.backgroundColor!==undefined" label="背景颜色">
            <el-color-picker v-model="configData.chartOption.textStyle.backgroundColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="提示框" name="3">
          <el-form-item label="后缀">
            <el-input v-model="configData.chartOption.suffix" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.tooltip.textStyle.color!==undefined" label="文字颜色">
            <el-color-picker v-model="configData.chartOption.tooltip.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.tooltip.borderWidth!==undefined" label="边框宽度">
            <el-slider v-model="configData.chartOption.tooltip.borderWidth" :min="0" :step="1" :max="20" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.tooltip.borderRadius!==undefined" label="边框圆角">
            <el-slider v-model="configData.chartOption.tooltip.borderRadius" :min="0" :step="1" :max="100" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.tooltip.borderColor!==undefined" label="边框颜色">
            <el-color-picker v-model="configData.chartOption.tooltip.borderColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.tooltip.backgroundColor!==undefined" label="背景颜色">
            <el-color-picker v-model="configData.chartOption.tooltip.backgroundColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图形" name="4">
          <el-form-item v-if="configData.chartOption.itemStyle.color!==undefined" label="地图板块颜色">
            <el-color-picker v-model="configData.chartOption.itemStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.itemStyle.opacity!==undefined" label="图形透明度">
            <el-slider v-model="configData.chartOption.itemStyle.opacity" :max="1" :min="0" :step="0.1"></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.itemStyle.borderWidth!==undefined" label="描边宽度">
            <el-slider v-model="configData.chartOption.itemStyle.borderWidth" :min="0" :step="1" :max="20" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.itemStyle.borderColor!==undefined" label="描边颜色">
            <el-color-picker v-model="configData.chartOption.itemStyle.borderColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="手动旋转">
            <el-switch v-model="configData.chartOption.isRotate"></el-switch>
          </el-form-item>
          <el-form-item label="上下旋转角度">
            <el-slider v-model="configData.chartOption.alpha" :step="1" :max="90" show-input></el-slider>
          </el-form-item>
          <el-form-item label="左右旋转角度">
            <el-slider v-model="configData.chartOption.beta" :min="-90" :step="1" :max="90" show-input></el-slider>
          </el-form-item>
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
        </el-collapse-item>
        <el-collapse-item title="视觉映射" name="5">
          <el-form-item label="视觉映射">
            <el-switch v-model="configData.chartOption.visualShow"  active-text="开启" inactive-text="关闭"/>
          </el-form-item>
          <el-form-item label="图例宽度">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.itemWidth" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="图例高度">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.itemHeight" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="图例左边距">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.visualX" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="图例下边距">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.visualY" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="标签大小">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.visualFontSize" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="图例颜色">
            <draggable :animation="340" group="selectItem" handle=".option-drag">  
              <div style="display:flex;flex-wrap: wrap;" >
                <div v-for="(item, index) in colors" :key="index" class="select-item" style="display:flex">
                  <el-color-picker v-model="colors[index]" show-alpha style="margin-left:25px"></el-color-picker>
                    <div class="close-btn select-line-icon" @click="removeSelectItem(index)" style="margin-left:10px">
                      <i class="el-icon-remove-outline" />
                    </div>
                </div>
              </div>
            </draggable>
            <div style="margin-left: 20px;">
              <el-button style="padding-bottom: 0" icon="el-icon-circle-plus-outline" type="text" @click="addSelectItem">
                添加颜色
              </el-button>
            </div> 
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="6">
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
    addSelectItem(){
      this.colors.push( '#244779')
      this.$set(this.configData.chartOption, 'colors', this.colors);
    },
    removeSelectItem(index){
      this.colors.splice(index, 1);
      this.$set(this.configData.chartOption, 'colors', this.colors);
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