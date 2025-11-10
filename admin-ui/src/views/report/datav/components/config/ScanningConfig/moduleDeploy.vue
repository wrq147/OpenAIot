<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.color1!==undefined" label="扫描颜色">
            <el-color-picker v-model="configData.chartOption.color1" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.color2!==undefined" label="底盘颜色">
            <el-color-picker v-model="configData.chartOption.color2" show-alpha></el-color-picker>
          </el-form-item>
          
          <el-form-item v-if="configData.chartOption.scanDur!==undefined" label="扫描时长">
            <el-input-number v-model="configData.chartOption.scanDur" placeholder="扫描动画时长" :step="1"/>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.haloDur!==undefined" label="光晕时长">
            <el-input-number v-model="configData.chartOption.haloDur" placeholder="光晕动画时长" :step="1"/>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.opacity!==undefined" label="透明度">
             <el-slider v-model="configData.chartOption.opacity" :max="1" :min="0" :step="0.1"></el-slider>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="2">
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
export default {
  props: {
    configData: {
      type: Object,
      required: true
    },
  },
  data() {
    return {
      activeNames: ["1"],
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