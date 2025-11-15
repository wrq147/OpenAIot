<template>
    <el-dialog v-if="dialogFlag" title="设置周期" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body width="1300px" top="2vh" @close="cancel">
      <div style="color: red">提示：请先选择时段，然后点击时段设置方框可单独设置，点击某行序号可直接设置一行！</div>
      <div class="tableList">
        <div class="left">
          <div style="margin-bottom: 10px;">时段</div>
          <time-info @getTimeInfo="getTimeInfo" />
        </div>
        <div class="right">
          <div style="margin-bottom: 10px;">时段设置</div>
          <period-info :timeData="timeData" :ruleForm="ruleForm" @getSetting="getSetting" />
        </div>
      </div>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm">保存</el-button>
      </span>
    </el-dialog>
</template>
<script>
import { classesList, editClassesTime, editClassesTimeMore } from "@/api/scheduling/timeClasses";
import timeInfo from './timeInfo.vue'
import periodInfo from './periodInfo.vue'
export default { 
  name: 'addBatch',
  components: {
    timeInfo,
    periodInfo
  },
  props: {
    dialogVisible: {
      type: Boolean
    },
    title: {
      type: String
    }
  },
  data() {
    return {
      dialogFlag: false,
      timeData: {},
      // 表单
      ruleForm: {},
      batchList: [],
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue;
    }
  },
  methods: {
    // 获取点击时段的信息
    getTimeInfo(val) {
      this.timeData = val;
    },
    // 获取周期设置的信息
    getSetting(list) {
      this.batchList = list;
    },
    // 保存操作
    submitForm() {
      // 创建一个数组来存储所有请求的Promise
      const requests = [];
      // 遍历批次列表
      for (let i = 0; i < this.batchList.length; i++) {
        const shiDuans = [];
        
        // 遍历周期时段
        for (let j = 0; j < this.batchList[i].ZhouQiShiDuan.length; j++) {
          if (this.batchList[i].ZhouQiShiDuan[j].ShiDuanID) {
            shiDuans.push({
              zhouQiShu: this.batchList[i].ZhouQiShu,
              zhouQi: this.batchList[i].ZhouQiShiDuan[j].ZhouQi,
              shiDuanId: this.batchList[i].ZhouQiShiDuan[j].ShiDuanID
            });
          }
        }
        const data = {
          BanCiId: this.ruleForm.banCiID,
          shiDuans: shiDuans
        };
        // 将每个请求的Promise添加到requests数组中
        requests.push(editClassesTimeMore(data));
      }
      // 使用Promise.all并行处理所有请求
      Promise.all(requests).then(responses => {
          this.$message.success('设置成功!');
          // 刷新数据或执行其他操作
          this.$emit('getList');
        }).catch(error => {});
    },
    cancel() {
      this.timeData = {};
      this.$emit('getList');
    }
  }
}
</script>
  
<style lang="scss" scoped>
 ::v-deep {
  .el-dialog__header{
    border-bottom: 1px solid #ccc;
  }
  .el-date-editor.el-input {
    width: 100%;
  }
  .el-select, .el-cascader{
    width: 100%;
  }
}
.tableList{
    margin-top: 10px;
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
}
.tableList .left{
  width: 45% !important;
  padding: 10px;
  border: 1px solid #ccc;
}
.tableList .right{
  width: 53% !important;
  padding: 10px;
  border: 1px solid #ccc;
}
</style>