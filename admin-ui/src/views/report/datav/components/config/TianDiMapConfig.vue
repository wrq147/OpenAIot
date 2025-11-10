<template>
    <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="配置" name="field" />
      <el-tab-pane label="数据" name="data" />
      <el-tab-pane label="定位" name="location" />
    </el-tabs>
    <div class="field-box">
      
      <el-scrollbar class="right-scrollbar">
        <!-- 组件属性 -->
        <el-form v-if="currentTab==='field'" size="small" label-width="90px">
          <el-collapse  v-model="activeNames" accordion>
            <el-collapse-item title="图层" name="1">
              <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
                <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
              </el-form-item>
            </el-collapse-item>

            <el-collapse-item title="功能" name="2">

              <el-form-item v-if="configData.chartOption.isBounds!==undefined" label="显示标注">
                <el-switch v-model="configData.chartOption.isBounds" />
              </el-form-item>

              <el-form-item v-if="configData.chartOption.isHawkEye!==undefined" label="显示鹰眼">
                <el-switch v-model="configData.chartOption.isHawkEye" />
              </el-form-item>

              <el-form-item v-if="configData.chartOption.isMapType!==undefined" label="显示地图类型控件">
                <el-switch v-model="configData.chartOption.isMapType" />
              </el-form-item>

            </el-collapse-item>

          <el-collapse-item title="动画" name="5">

            <el-form-item v-if="configData.chartOption.animate!==undefined" label="载入动画">
              <el-select v-model="configData.chartOption.animate" placeholder="请选择">
                <el-option
                  v-for="item in animateOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value">
                </el-option>
              </el-select>
            </el-form-item>
          </el-collapse-item>
        </el-collapse>
         
        </el-form>
        <!-- 表单属性 -->
        <el-form v-if="currentTab === 'data'" size="small" label-width="90px">
          <data-source-config :drawingList="drawingList" :themeForm="themeForm" :dataSourceType="configData.chartOption.dataSourceType" :customData="configData" :customId="configData.customId" 
            @changeSource="changeSource"  @changeData="changeData" :baseType="''"></data-source-config>

        </el-form>

        <!-- 组件位置 -->
        <el-form v-if="currentTab === 'location'" size="small" label-width="90px">
          <el-form-item label="X位置">
            <el-input-number v-model="configData.x" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="y位置">
            <el-input-number v-model="configData.y" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="宽度">
            <el-input-number v-model="configData.width" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="高度">
            <el-input-number v-model="configData.height" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="zIndex">
            <el-input-number v-model="configData.zindex" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
        </el-form>

      </el-scrollbar>
    </div>


  </div>
</template>

<script>

import {animateOptions} from '../../animate/animate'
import DataSourceConfig from './DataConfig/DataSourceConfig'
import sourceConfig from '../mixins/sourceConfig.js'

export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig
  },
  data() {
    return {
      activeNames: ['1'],
      currentTab: 'field',
      animateOptions
    }
  },
  //页面加载完执行
  mounted() {
    
  },
  computed: {
    
  },

  methods: {

  }
}
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
    width: 33%;
    text-align: center;
}
</style>