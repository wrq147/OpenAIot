<template>
    <div class="tabsInfo">
        <el-tabs v-model="activeName">
            <el-tab-pane label="时段管理" name="timeQuantum">
                <time-quantum ref="timeQuantum" />
            </el-tab-pane>
            <el-tab-pane label="班组管理" name="timeGroup">
                <time-group ref="timeGroup" />
            </el-tab-pane>
            <el-tab-pane label="班次管理" name="timeClasses">
                <time-classes ref="timeClasses" />
            </el-tab-pane>
        </el-tabs>
    </div>
</template>
<script>
  import timeQuantum from './timeQuantum.vue'
  import timeGroup from './timeGroup.vue'
  import timeClasses from './timeClasses.vue'
  export default {
    name: "Index",
    components: {
        timeQuantum,
        timeGroup,
        timeClasses
    },
    data() {
      return {
        activeName: 'timeQuantum'
      };
    },
    methods: {}
  };
  </script>
  
  <style lang="scss" scoped>
  .tabsInfo{
    padding: 10px;
    background-color: transparent;
    ::v-deep .el-tabs__nav-wrap::after{
      background-color:rgba(255, 255, 255, 0.2);
    }
  }
  .tabsInfo .el-tabs{
    padding: 10px;
    background-color: rgba(25, 33, 45, 1);
    border-radius: 4px;
  }
  ::v-deep .el-tabs__header{
    // background-color: rgba(25, 33, 45, 1);
  }
  </style>