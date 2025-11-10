<template>
  <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" width="700px"
    top="2vh" @close="cancel">
    <el-form ref="ruleForm" :model="ruleForm" label-width="140px" class="addPeople">
      <el-form-item label="假日名称" prop="holidayName">
        <el-input v-model="ruleForm.holidayName" placeholder="请输入假日名称" style="width: 320px;" />
      </el-form-item>
      <el-form-item label="轮询方式" prop="calendarType">
        <el-select v-model="ruleForm.calendarType" placeholder="请选择轮询方式" style="width:180px;" @change="chgType">
          <el-option label="每年公历" :value="0"></el-option>
          <el-option label="每年农历" :value="1"></el-option>
          <el-option label="每周" :value="2"></el-option>
        </el-select>
      </el-form-item>
      <template v-if="ruleForm.calendarType == 2">
        <el-form-item label="开始放假">
          <el-select v-model="startWeek" placeholder="请选择" style="width: 200px;">
            <el-option label="周一" :value="1"></el-option>
            <el-option label="周二" :value="2"></el-option>
            <el-option label="周三" :value="3"></el-option>
            <el-option label="周四" :value="4"></el-option>
            <el-option label="周五" :value="5"></el-option>
            <el-option label="周六" :value="6"></el-option>
            <el-option label="周日" :value="7"></el-option>
          </el-select>
          <el-time-picker v-model="startWeekTime" placeholder="请选择" value-format="HH:mm:ss"
            style="width: 200px;margin-left: 15px;">
          </el-time-picker>
        </el-form-item>
        <el-form-item label="结束放假">
          <el-select v-model="endWeek" placeholder="请选择" style="width: 200px;">
            <el-option label="周一" :value="1"></el-option>
            <el-option label="周二" :value="2"></el-option>
            <el-option label="周三" :value="3"></el-option>
            <el-option label="周四" :value="4"></el-option>
            <el-option label="周五" :value="5"></el-option>
            <el-option label="周六" :value="6"></el-option>
            <el-option label="周日" :value="7"></el-option>
          </el-select>
          <el-time-picker v-model="endWeekTime" placeholder="请选择" value-format="HH:mm:ss"
            style="width: 200px;margin-left: 15px;">
          </el-time-picker>
        </el-form-item>
      </template>
      <template v-else>
        <el-form-item label="开始放假" prop="holidayStart">
          <el-date-picker v-model="ruleForm.holidayStart" value-format="yyyy-MM-dd HH:mm:ss" type="datetime"
            placeholder="选择开始日期时间" style="width: 320px;" />
        </el-form-item>
        <el-form-item label="结束放假" prop="holidayEnd">
          <el-date-picker v-model="ruleForm.holidayEnd" value-format="yyyy-MM-dd HH:mm:ss" type="datetime"
            placeholder="选择结束日期时间" style="width: 320px;" />
        </el-form-item>
      </template>
    </el-form>
    <span slot="footer" class="dialog-footer">
      <el-button @click="cancel">取消</el-button>
      <el-button type="primary" @click="submitForm('ruleForm')">确定</el-button>
    </span>
  </el-dialog>
</template>
<script>
import { HolidayTypeAdd, HolidayTypeEdit } from "@/api/monitor/job";
export default {
  name: 'addDataOrigin',
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
      startWeek: null,
      startWeekTime: null,
      endWeek: null,
      endWeekTime: null,
      dialogFlag: false,
      // 表单
      ruleForm: {},
      list: [],
      // 校验
      rulesOther: {
        holidayName: [
          { required: true, message: "假日名称不能为空", trigger: "blur" },
        ],
        calendarType: [
          { required: true, message: "请选择日历类型", trigger: "change" },
        ],
        holidayStart: [
          { required: true, message: "请选择开始时间", trigger: "change" },
        ],
        holidayEnd: [
          { required: true, message: "请选择结束时间", trigger: "change" },
        ]
      }
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue
    }
  },
  methods: {
    InitForm(data) {
      if (data.Id != null) {
        var startString = data.HolidayStart.replace(/-/g, "/");
        let startdate = new Date(startString);
        this.startWeek = startdate.getDate();

        let hours = startdate.getHours().toString().padStart(2, "0");
        let minutes = startdate.getMinutes().toString().padStart(2, "0");
        let seconds = startdate.getSeconds().toString().padStart(2, "0");
        this.startWeekTime = `${hours}:${minutes}:${seconds}`;

        var endWeekTimeString = data.HolidayEnd.replace(/-/g, "/");
        let enddate = new Date(endWeekTimeString);

        if (enddate.getDate() > 7) {
          this.endWeek = enddate.getDate() - 7;
        }
        else {
          this.endWeek = enddate.getDate();
        }

        hours = enddate.getHours().toString().padStart(2, "0");
        minutes = enddate.getMinutes().toString().padStart(2, "0");
        seconds = enddate.getSeconds().toString().padStart(2, "0");
        this.endWeekTime = `${hours}:${minutes}:${seconds}`;
      }
      else {
        this.startWeek = null;
        this.startWeekTime = null;
        this.endWeek = null;
        this.endWeekTime = null;
      }
      this.ruleForm = {
        id: data.Id,
        holidayName: data.HolidayName,
        calendarType: data.CalendarType,
        holidayStart: data.HolidayStart,
        holidayEnd: data.HolidayEnd
      };
    },
    submitForm(formName) {
      if (this.ruleForm.holidayName == null || this.ruleForm.holidayName == "") {
        this.$message.error("假日名称不能为空");
        return;
      }
      if (this.ruleForm.calendarType == null) {
        this.$message.error("请选择日历类型");
        return;
      }
      if (this.ruleForm.calendarType == 2) {
        if (this.startWeek == null || this.startWeekTime == null) {
          this.$message.error("请选择开始时间");
          return;
        }
        if (this.endWeek == null || this.endWeekTime == null) {
          this.$message.error("请选择结束时间");
          return;
        }
        this.ruleForm.holidayStart = "2020-06-" + this.startWeek.toString().padStart(2, "0") + " " + this.startWeekTime;
        if (this.startWeek > this.endWeek) {
          this.ruleForm.holidayEnd = "2020-06-" + (7 + this.endWeek).toString().padStart(2, "0") + " " + this.endWeekTime;
        }
        else {
          this.ruleForm.holidayEnd = "2020-06-" + this.endWeek.toString().padStart(2, "0") + " " + this.endWeekTime;
        }

        if(this.startWeek==this.endWeek&&this.endWeekTime<this.startWeekTime){
          this.$message.error("开始时间不可小于结束时间");
          return;
        }
      }
      else {
        if (this.ruleForm.holidayStart == null || this.ruleForm.holidayStart == "") {
          this.$message.error("请选择开始时间");
          return;
        }
        if (this.ruleForm.holidayEnd == null || this.ruleForm.holidayEnd == "") {
          this.$message.error("请选择结束时间");
          return;
        }
      }
      if (this.title === '新增节日类型') {
        this.getHolidayTypeAdd()
      } else {
        this.getHolidayTypeEdit()
      }
      this.dialogFlag = false;
    },
    getHolidayTypeAdd() {
      HolidayTypeAdd(this.ruleForm).then(res => {
        this.$message.success('添加成功!')
        this.$emit('getListHoliday')
      })
    },
    getHolidayTypeEdit() {
      HolidayTypeEdit(this.ruleForm).then(res => {
        this.$message.success('修改成功!')
        this.$emit('getListHoliday')
      })
    },
    cancel() {
      this.$emit('cancelForm')
    },
    chgType(val) {
      if (val == 2) {
        this.startWeek = null;
        this.startWeekTime = null;
        this.endWeek = null;
        this.endWeekTime = null;
      }
    }
  }
}
</script>

<style lang="scss" scoped>
::v-deep {
  .el-dialog__header {
    border-bottom: 1px solid #ccc;
  }

  .el-select {
    width: 100%;
  }
}

.addPeople>.btn {
  width: 100%;
  justify-content: flex-end;
  display: flex;
  align-items: center;
}
</style>