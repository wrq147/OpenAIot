<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.opacity!==undefined" label="透明度">
             <el-slider v-model="configData.chartOption.opacity" :max="1" :min="0" :step="0.1" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.rotation!==undefined" label="开启3D旋转">
            <el-switch v-model="configData.chartOption.rotation" />
          </el-form-item>

          <el-form-item  label="开启预览">
            <el-switch v-model="configData.chartOption.enlarge" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.fit!==undefined" label="适应方式">
            <el-select v-model="configData.chartOption.fit" placeholder="请选择">
              <el-option label="拉伸填充" value="fill" />
              <el-option label="原比例缩放" value="contain" />
              <el-option label="原比例剪切" value="cover" />
            </el-select>
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