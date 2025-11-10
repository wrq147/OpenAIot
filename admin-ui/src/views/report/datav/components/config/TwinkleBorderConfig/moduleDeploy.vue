<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="边框" name="2">
          <el-form-item label="边框类型">
            <el-select v-model="configData.chartOption.borderType" placeholder="请选择边框">
              <el-option
                v-for="(item,index) in borderTypeArr"
                :key="index"
                :label="item.label"
                :value="item.value">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.color1!==undefined" label="颜色1">
            <el-color-picker v-model="configData.chartOption.color1" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.color2!==undefined" label="颜色1">
            <el-color-picker v-model="configData.chartOption.color2" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.borderType!='dv-border-box-8'" label="背景色">
            <el-color-picker v-model="configData.chartOption.backgroundColor" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.borderType=='dv-border-box-8'" label="动画时长">
            <el-slider v-model="configData.chartOption.dur" :min="0" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.borderType=='dv-border-box-4'||configData.chartOption.borderType=='dv-border-box-5'||configData.chartOption.borderType=='dv-border-box-8'" label="是否反向">
            <el-switch v-model="configData.chartOption.reverse" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.borderType=='dv-border-box-11'" label="边框标题">
            <el-input v-model="configData.chartOption.title" placeholder="请输入边框标题" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.borderType=='dv-border-box-11'" label="标题宽度">
            <el-input-number class="inputNumber" v-model="configData.chartOption.titleWidth" :min="1" :step="1"></el-input-number>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="3">
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
      borderTypeArr:[
        {"label":"闪烁边框","value":"dv-border-box-1"},
        {"label":"游走边框","value":"dv-border-box-8"},
        {"label":"标题边框","value":"dv-border-box-11"},
        {"label":"简单边框1","value":"dv-border-box-2"},
        {"label":"简单边框2","value":"dv-border-box-3"},
        {"label":"简单边框3","value":"dv-border-box-4"},
        {"label":"简单边框4","value":"dv-border-box-5"},
        {"label":"简单边框5","value":"dv-border-box-6"},
        {"label":"简单边框6","value":"dv-border-box-7"},
        {"label":"简单边框7","value":"dv-border-box-9"},
        {"label":"简单边框8","value":"dv-border-box-10"},
        {"label":"简单边框9","value":"dv-border-box-11"},
        {"label":"简单边框10","value":"dv-border-box-12"},
        {"label":"简单边框11","value":"dv-border-box-13"}
      ]
    }
  },
  methods: {}
}
</script>
<style lang="scss" scoped>
::v-deep {
  .inputNumber , .inputNumber .el-input-number--small{
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