<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="地图设置" name="2">
          <el-form-item v-if="configData.chartOption.mapName!==undefined" label="地图类型">
            <el-select v-model="configData.chartOption.mapName" placeholder="请选择">
              <el-option v-for="item in provinceOptions" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.show!==undefined" label="开启板块名称">
            <el-switch v-model="configData.chartOption.label.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.textStyle.fontSize!==undefined" label="标签字号">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.label.textStyle.fontSize" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.textStyle.color!==undefined" label="标签颜色">
            <el-color-picker v-model="configData.chartOption.label.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.itemStyle.color!==undefined" label="板块颜色">
            <el-color-picker v-model="configData.chartOption.itemStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.emphasis.itemStyle.color!==undefined" label="高亮颜色">
            <el-color-picker v-model="configData.chartOption.emphasis.itemStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.itemStyle.borderWidth!==undefined" label="描边宽度">
            <el-slider v-model="configData.chartOption.itemStyle.borderWidth" :min="0" :step="1" :max="20" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.itemStyle.borderColor!==undefined" label="描边颜色">
            <el-color-picker v-model="configData.chartOption.itemStyle.borderColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="上下旋转角度">
            <el-slider v-model="configData.chartOption.viewControl.alpha" :step="1" :max="90" show-input></el-slider>
          </el-form-item>
          <el-form-item label="左右旋转角度">
            <el-slider v-model="configData.chartOption.viewControl.beta" :min="-90" :step="1" :max="90" show-input></el-slider>
          </el-form-item>
          <el-form-item label="手动旋转">
            <el-switch v-model="configData.chartOption.viewControl.isRotate"></el-switch>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="柱体设置" name="3">
          <el-form-item v-if="configData.chartOption.barSize!==undefined" label="柱体宽度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.barSize" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.bevelSize!==undefined" label="倒角尺寸">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.bevelSize" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.bevelSmoothness!==undefined" label="倒角光滑度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.bevelSmoothness" />
          </el-form-item>
          <el-form-item label="后缀">
            <el-input v-model="configData.chartOption.suffix" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标签" name="4">
          <el-form-item label="开启柱体标签">
            <el-switch v-model="configData.chartOption.labelShow" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.textStyle.fontSize!==undefined" label="标签字号">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.textStyle.fontSize" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.textStyle.color!==undefined" label="标签颜色">
            <el-color-picker v-model="configData.chartOption.textStyle.color" show-alpha />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.textStyle.backgroundColor!==undefined" label="背景颜色">
            <el-color-picker v-model="configData.chartOption.textStyle.backgroundColor" show-alpha />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.emphasis.textStyle.fontSize!==undefined" label="高亮字号">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.emphasis.textStyle.fontSize" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.emphasis.textStyle.color!==undefined" label="高亮颜色">
            <el-color-picker v-model="configData.chartOption.emphasis.textStyle.color" show-alpha />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.emphasis.textStyle.backgroundColor!==undefined" label="高亮背景">
            <el-color-picker v-model="configData.chartOption.emphasis.textStyle.backgroundColor" show-alpha />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="排序" name="5">
          <el-form-item label="开启排序标签">
            <el-switch v-model="configData.chartOption.sortShow" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.textStyle.sortFontSize!==undefined" label="标签字号">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.textStyle.sortFontSize"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.textStyle.sortColor!==undefined" label="标签颜色">
            <el-color-picker v-model="configData.chartOption.textStyle.sortColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.textStyle.borderColor!==undefined" label="边框颜色">
            <el-color-picker v-model="configData.chartOption.textStyle.borderColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.emphasis.textStyle.sortFontSize!==undefined" label="高亮字号">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.emphasis.textStyle.sortFontSize"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.emphasis.textStyle.sortColor!==undefined" label="高亮颜色">
            <el-color-picker v-model="configData.chartOption.emphasis.textStyle.sortColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.emphasis.textStyle.borderColor!==undefined" label="高亮边框">
            <el-color-picker v-model="configData.chartOption.emphasis.textStyle.borderColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="视觉映射" name="6">
          <el-form-item label="视觉映射">
            <el-switch v-model="configData.chartOption.visualMap.show"  active-text="开启" inactive-text="关闭"/>
          </el-form-item>
          <el-form-item label="图例宽度">
            <el-input-number v-model="configData.chartOption.visualMap.itemWidth" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="图例高度">
            <el-input-number v-model="configData.chartOption.visualMap.itemHeight" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="图例左边距">
            <el-input-number v-model="configData.chartOption.visualMap.left" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="图例下边距">
            <el-input-number v-model="configData.chartOption.visualMap.bottom" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="标签大小">
            <el-input-number v-model="configData.chartOption.visualMap.fontSize" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="图例颜色">
            <draggable :animation="340" group="selectItem" handle=".option-drag">  
              <div style="display:flex;flex-wrap: wrap;" >
                <div v-for="(item, index) in configData.chartOption.visualMap.color" :key="index" class="select-item" style="display:flex">
                  <el-color-picker v-model="configData.chartOption.visualMap.color[index]" show-alpha style="margin-left:25px"></el-color-picker>
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