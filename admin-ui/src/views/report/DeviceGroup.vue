<template>
  <el-radio-group size="small" v-model="viewerDevice" :class="viewerDevice" @change="deviceSelect">
    <el-radio-button label="phone">Phone</el-radio-button>
    <el-radio-button label="pad">Pad</el-radio-button>
    <el-radio-button label="pc">PC</el-radio-button>
  </el-radio-group>
</template>
<script>
export default {
  name: 'DeviceGroup',
  props:{
    width:{
      type: Number,
      default: 375,
    },
    height:{
      type: Number,
      default: 817,
    }
  },
  mounted(){
  },
  computed: {
  },
  watch:{
  },
  data() {
    return {
    
    }
  },
  methods: {
  }
}
</script>
<style lang="scss">
.m-editor-workspace {
  * {
    user-select: none;
  }
  .el-slider {
    position: absolute;
    bottom: 40px;
    left: 20px;
    width: 250px;
    opacity: 0.5;
    transition: opacity 1s;
  }
  .el-slider:hover {
    opacity: 1;
  }
  .el-radio-group {
    position: absolute;
    top: 10px;
    right: 40px;
    z-index: 10;
  }
  .viewer-scrollbar > .el-scrollbar__bar {
    display: none;
  }
  .select-component {
    text-align: center;
    transform: translate3d(0, -70px, 0);
    p {
      margin-top: 8px;
    }
  }
  .close-pop-button {
    position: absolute;
    left: 50%;
    transform: translate(-50%);
  }
}
</style>