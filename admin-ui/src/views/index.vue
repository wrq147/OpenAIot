<template>
  <div class="home_com" v-loading="isLoading">
    <device-info v-if="isCheckPermi(['/AfterService/Dev/List'])"></device-info>
    <schedule-reminder v-if="checkNum % 2 > 0"></schedule-reminder>
    <!-- v-if="checkNum % 2 > 0" -->
    <custom-info v-if="isCheckPermi(['/CRMMan/'])"></custom-info>
    <staging v-if="isCheckPermi(['/FlowService/Flow'])"></staging>
    <!-- 流程管理 -->
    <stock-info v-if="isCheckPermi(['/Stock/'])"></stock-info>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import DeviceInfo from "@/views/homeModules/deviceInfo.vue";
import StockInfo from "@/views/homeModules/stockInfo.vue";
import ScheduleReminder from "@/views/homeModules/scheduleReminder.vue";
import staging from "@/views/homeModules/staging.vue";
import CustomInfo from "@/views/homeModules/customInfo.vue";
import Calendar from 'vue-calendar-component';
import { checkPermi } from '@/utils/permission.js';
export default {
  name: "Index",
  components: {
    Calendar,
    DeviceInfo,
    ScheduleReminder,
    staging,
    StockInfo,
    CustomInfo
  },
  mixins: [resizeTableCon],
  data() {
    return {
      isLoading: false,
      checkNum: 0
    };
  },
  mounted() {
    this.getCheckNum()

  },
  methods: {
    isCheckPermi(val) {
      return checkPermi(val)
    },
    getCheckNum() {
      //获取权限数量
      if (checkPermi(['/AfterService/Dev/List'])) {
        this.checkNum = this.checkNum + 1
      }
      if (checkPermi(['/CRMMan/'])) {
        this.checkNum = this.checkNum + 1
      }
      if (checkPermi(['/FlowService/Flow'])) {
        this.checkNum = this.checkNum + 1
      }
      if (checkPermi(['/Stock/'])) {
        this.checkNum = this.checkNum + 1
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.home_com {
  width: 100%;
  padding: 20px;
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;
  align-items: flex-start;
  background: #F4F5F9;
  margin-bottom: -30px;
}
</style>
