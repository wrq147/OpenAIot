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
    padding: 20px;
    background-color: #fff;
  }
  ::v-deep .el-tabs__header{
    background-color: #fff;
  }
  </style>