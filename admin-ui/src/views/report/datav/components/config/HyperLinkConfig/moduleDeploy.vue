<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="超链接" name="2">
          <el-form-item v-if="configData.chartOption.target!==undefined" label="新页面位置">
            <el-select v-model="configData.chartOption.target" placeholder="请选择">
              <el-option
                v-for="item in targets"
                :key="item.value"
                :label="item.label"
                :value="item.value">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.type!==undefined" label="超链接样式">
            <el-radio-group v-model="configData.chartOption.type">
              <el-radio label="label">文本</el-radio>
              <el-radio label="img">图片</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="文本类型" name="3">
              
          <el-form-item v-if="configData.chartOption.label.context!==undefined" label="显示文字">
            <el-input v-model="configData.chartOption.label.context" placeholder="请输入显示文字" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.label.fontSize!==undefined" label="字体大小">
            <el-slider v-model="configData.chartOption.label.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.label.fontFamily!==undefined" label="字体名称">
            <el-select v-model="configData.chartOption.label.fontFamily" placeholder="请选择">
              <el-option
                v-for="(item,index) in fontFamilys"
                :key="index"
                :label="item"
                :value="item">
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item label="文字粗细">
            <el-select v-model="configData.chartOption.fontWeight" placeholder="请选择">
              <el-option
                v-for="(item,index) in fontWeights"
                :key="index"
                :label="item"
                :value="item">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.decoration!==undefined" label="字体装饰线">
            <el-select v-model="configData.chartOption.label.decoration" placeholder="请选择">
              <el-option value="none" label="无"></el-option>
              <el-option value="underline" label="下划线"></el-option>
              <el-option value="line-through" label="中划线"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.linkColor!==undefined" label="未访问链接颜色">
            <el-color-picker v-model="configData.chartOption.label.linkColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.hoverColor!==undefined" label="鼠标悬停颜色">
            <el-color-picker v-model="configData.chartOption.label.hoverColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图片类型" name="4">
          <el-form-item v-if="configData.chartOption.img.src!==undefined" label="图片">
            <image-upload v-model="this.configData.chartOption.img.src" :limit="1"></image-upload>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.img.width!==undefined"  label="宽度">
            <el-input-number v-model="configData.chartOption.img.width" controls-position="right"  :min="1" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.img.height!==undefined"  label="高度">
            <el-input-number v-model="configData.chartOption.img.height" controls-position="right" :min="1" :step="1"></el-input-number>
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
import draggable from "vuedraggable";
export default {
  props: {
    configData: {
      type: Object,
      required: true
    }
  },
  components: {
    draggable
  },
  data() {
    return {
      targets:[
        { value:'_blank', label:'新窗口' },
        { value:'_self', label:'相同框架' },
        { value:'_parent', label:'父框架集' },
        { value:'_top', label:'整个窗口' }
      ],
      fontWeights:['normal', 'bold', 'bolder', 'lighter'],
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
  .el-slider{
    width: 95%;
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