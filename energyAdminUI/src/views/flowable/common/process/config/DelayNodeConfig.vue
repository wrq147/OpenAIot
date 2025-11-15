<template>
  <div>
    <div style="margin-bottom: 20px">
      <p class="item-desc">延时方式</p>
      <el-radio-group v-model="config.type" size="small">
        <el-radio-button label="FIXED">固定时长</el-radio-button>
        <el-radio-button label="AUTO">自动计算</el-radio-button>
      </el-radio-group>
    </div>
    <div v-if="config.type === 'FIXED'">
      <el-input
        style="width: 180px"
        placeholder="时间单位"
        size="small"
        type="number"
        v-model="config.time"
      >
        <el-select
          style="width: 75px"
          v-model="config.unit"
          slot="append"
          placeholder="请选择"
        >
          <el-option label="天" value="D"></el-option>
          <el-option label="小时" value="H"></el-option>
          <el-option label="分钟" value="M"></el-option>
        </el-select>
      </el-input>
      <span class="item-desc"> 后进入下一步</span>
    </div>
    <div class="item-desc" v-else>
      <el-date-picker
        v-model="config.dateTime"
        :picker-options="pickerOptions"
        value-format="yyyy-MM-dd HH:mm:ss"
        type="datetime"
        size="small"
        placeholder="选择执行的日期和时间"
      >
      </el-date-picker>
      <!-- <el-time-picker value-format="HH:mm:ss" style="width: 150px;" size="small" v-model="config.dateTime" placeholder="任意时间点"></el-time-picker> -->
      <span class="item-desc"> 后进入下一步</span>
    </div>
  </div>
</template>

<script>
import {parseTime} from '@/utils/common.js'
export default {
  name: "DelayNodeConfig",
  components: {},
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  data() {
    return {
      pickerOptions: {
        disabledDate(time) {
          return (
            time.getTime() < new Date(new Date().toLocaleDateString()).getTime()
          );
        }
      },
    };
  },
  methods: {},
};
</script>

<style scoped>
</style>
