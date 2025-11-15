<template>
  <div style="width: 100%;min-height: calc(100vh - 194px)">
    <!-- -->
    <iframe style="width: 100%;min-height: calc(100vh - 194px)" :src="productInfos.MonitorReportToken+'&DeviceNumber='+deviceInfos.DeviceNumber+'&DeviceId='+deviceInfos.DeviceId" frameborder="0"></iframe>
  </div>
</template>

<script>
export default {
  name: 'deviceCustomFunc',

  data() {
    return {
      
    };
  },
  props: {
    deviceInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
    productInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  mounted() {
    
  },

  methods: {
    
  },
};
</script>