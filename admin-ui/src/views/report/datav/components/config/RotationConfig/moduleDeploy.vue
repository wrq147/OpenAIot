<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="表格列" name="2">
          <el-form-item v-if="configData.chartOption.isEffect!==undefined" label="轮播效果">
            <el-select v-model="configData.chartOption.isEffect" placeholder="请选择">
              <el-option
                v-for="(item,index) in swipers"
                :key="index"
                :label="item.label"
                :value="item.value">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.playSpeed!==undefined" label="轮播速度">
            <el-select v-model="configData.chartOption.playSpeed" placeholder="请选择">
              <el-option
                v-for="(item,index) in speeds"
                :key="index"
                :label="item.label"
                :value="item.value">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.direction!==undefined" label="轮播方向">
            <el-radio-group v-model="configData.chartOption.direction">
              <el-radio label="horizontal">横向</el-radio>
              <el-radio label="vertical">纵向</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isPagination!==undefined" label="关闭分页器">
            <el-switch v-model="configData.chartOption.isPagination" active-text="是(点击图片关闭)" inactive-text="否"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.paginationType!==undefined" label="分页样式">
            <el-select v-model="configData.chartOption.paginationType" placeholder="请选择">
              <el-option label="圆点" value="bullets"></el-option>
              <el-option label="分式" value="fraction"></el-option>
              <el-option label="进度条" value="progressbar"></el-option>
            </el-select>
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
    costomData: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      activeNames: ["1"],
      animateOptions,
      swipers:[
        {label: '位移', value: 'slide'},
        {label: '淡入', value: 'fade'},
        {label: '轻弹', value: 'flip'},
        {label: '方块', value: 'cube'},
        {label: '卡片式', value: 'cards'},
        {label: '创意性', value: 'creative'},
        {label: '3d翻转', value: 'flip'},
        {label: '3d流', value: 'coverflow'}
      ],
      speeds:[
        {label: '2500', value: '2500'},
        {label: '5000', value: '5000'},
        {label: '7500', value: '7500'},
        {label: '10000', value: '10000'}
      ],
    }
  },
  methods: {
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