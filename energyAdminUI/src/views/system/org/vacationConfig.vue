<template>
  <div>
    <div class="calendar-container">
      <div class="calendar-info">
        <el-calendar>
          <template slot="dateCell" slot-scope="{date, data}">
            <div class="calendar-Box" :style="{ backgroundColor: returnColor(data.day) }" @click="selectDate(data.day)">
              <div>{{ data.day.split('-').slice(1).join('-') }}</div>
              <div class="name"> {{ returnStatus(data.day).HolidayTypeName }} </div>
            </div>
          </template>
        </el-calendar>
      </div>
      <div class="edit-info">
        <el-button type="primary" icon="el-icon-plus" plain @click="handleView('')">新增节日类型</el-button>
        <el-table id="exportTab" border :data="holidayList">
            <el-table-column label="假期名称" prop="HolidayName" align="center" />
            <el-table-column label="操作" width="150px" align="center">
                <template v-slot="scope">
                    <el-button size="mini" type="text" icon="el-icon-edit-outline" @click="handleView(scope.row)">编辑</el-button>
                    <el-button size="mini" type="text" icon="el-icon-delete" style="color:red;" @click="removeHoliday(scope.row.Id)">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
      </div>
    </div>
      <!-- 新增节日类型弹窗 -->
    <vacationAdd ref="vacationAdd" :title="holidayTitle" :dialog-visible="dialogFlag" @cancelForm="cancelForm" @getListHoliday="getList" />
  </div>
</template>
<script>
import { 
  calendarList,
  HolidayTypeList,
  HolidayTypeDel,
  calendarAdd,
  calendarRemove
} from "@/api/monitor/job";
import vacationAdd from "./vacationAdd";
import moment from 'moment'
export default {
  components: {
    vacationAdd
  },
  data() {
    return {
      dialogFlag: false,
      holidayList: [],
      holidayTitle: '新增节日类型',
      list: [],
      ruleForm: {
        data: ''
      }
    }
  },
  created() {
    this.getList();
  },
  methods: {
    /**
     * 获取数据
     */
    getList() {
      this.dialogFlag = false
      calendarList(this.queryParams).then(res => {
        this.list = res.data
        this.getListHoliday()
      })
    },
    getListHoliday() {
      HolidayTypeList().then(res => {
        this.holidayList = res.data
      })
    },
    handleView(data) {
      if (data === '') {
        this.title = '新增节日类型'
        this.$refs['vacationAdd'].InitForm({
          holidayName: '',
          calendarType: '',
          holidayStart: '',
          holidayEnd: ''
        });
      } else {
        this.title = '编辑节日类型'
        this.$refs['vacationAdd'].InitForm(data);
      }
      this.dialogFlag = true
    },
    // 数据时间对比
    returnStatus(d) {
      let newDate = this.list.filter(o => moment(o.Holiday).format('YYYY-MM-DD') === d);
      return newDate[0] !== undefined ? newDate[0] : ''
    },
    returnColor(d) {
      let newDate = this.list.filter(o => moment(o.Holiday).format('YYYY-MM-DD') === d);
      return newDate[0] !== undefined ? '#d8edf3' : ''
    },
    selectDate(date) {
      let newDate = this.list.filter(o => moment(o.Holiday).format('YYYY-MM-DD') === date);
      if (newDate.length > 0) {
         if(newDate[0].HolidayTypeName === '') {
          this.removeCalendar(newDate[0].Holiday);
         }
      } else {
       this.addCalendar(date);
      }
    },
    addCalendar(date) {
      this.$confirm('将时间' + date + '添加为假期,是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        calendarAdd({ data: moment(date).format('YYYY-MM-DD HH:mm:ss')}).then(res => {
          this.getList();
        })
      }).catch(() => {})
    },
    removeCalendar(id) {
      this.$confirm('此操作将永久删除该数据, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        calendarRemove({ data: id }).then(res => {
          this.$message.success('删除成功!')
          this.getList()
        })
      }).catch(() => {})
    },
    removeHoliday(id) {
      this.$confirm('此操作将永久删除该数据, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        HolidayTypeDel({ id: id }).then(res => {
          this.$message.success('删除成功!')
          this.getList()
        })
      }).catch(() => {})
    },
    cancelForm() {
      this.dialogFlag = false
    }
  }
}
</script>
<style lang="scss" scoped>
::v-deep {
  .el-dialog__header{
    border-bottom: 1px solid #ccc;
  }
  .el-date-editor.el-input{
    width: 100%;
  }
  .el-form-item{
    margin-bottom: 22px;
  }
  .el-calendar-table .el-calendar-day{
    padding: 0;
  }
}
.calendar-container{
  width: 100%;
  display: flex;
}
.calendar-info{
  width: 70%;
}
.edit-info{
  width: 30%;
}
.edit-info > button{
  margin-bottom: 10px;
}

.calendar-Box{
  height: 100%;
  padding: 8px;
}
.calendar-Box .name{
  margin-top: 10px;
  color: red;
}
</style>