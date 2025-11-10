<template>
    <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="配置" name="field">
        <el-scrollbar class="right-scrollbar">
          <module-deploy ref="moduleDeploy" :configData="configData" />
        </el-scrollbar>
      </el-tab-pane>
      <el-tab-pane label="定位" name="location">
        <el-scrollbar class="right-scrollbar">
          <module-position :configData="configData" />
        </el-scrollbar>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>

import sourceConfig from '../../mixins/sourceConfig.js'
import modulePosition from "./modulePosition";
import moduleDeploy from "./moduleDeploy";
export default {
  mixins: [sourceConfig],
  components: {
    modulePosition,
    moduleDeploy,
  },
  data() {
    return {
      currentTab: 'field',
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

<style scoped>
::v-deep .center-tabs .el-tabs__item {
  width: 50%;
  text-align: center;
}
::v-deep .el-scrollbar__wrap{
  height: calc(100vh - 40px);
}
</style>
