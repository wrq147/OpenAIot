<template>
  <div style="padding: 20px 20px 0 20px" id="big_con">
    <div class="from_con" id="from_con" v-if="showSearch">
      <el-form :model="taskForm" ref="taskForm" :inline="true" class="biaodan">
        <el-form-item
          label="流程发起人"
          prop="UserName"
          v-if="recordType != 'myPlane'"
        >
          <!-- <el-input v-model="taskForm.UserId" placeholder="请选择流程发起人" /> -->
          <el-select
            filterable
            allow-create
            default-first-option
            v-model="taskForm.UserName"
            ref="selectFlowCreatedUser"
            placeholder="请选择发起人"
            @focus="getCreatedFocus"
            style="width: 130px"
            :disabled="false"
          ></el-select>
        </el-form-item>
        <el-form-item label="创建时间" style="margin-left: 30px">
          <el-date-picker
            class="set_radius"
            v-model="dateRange"
            style="width: 230px"
            value-format="yyyy-MM-dd"
            type="daterange"
            range-separator="-"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
          ></el-date-picker>
        </el-form-item>
        <el-form-item label="任务状态" prop="TaskStatus" style="margin-left: 15px">
          <el-select
            class="set_radius"
            v-model="taskForm.TaskStatus"
            placeholder="请选择任务状态"
            clearable
            style="width: 180px"
          >
            <el-option
              v-for="dict in statusList"
              :key="dict.val"
              :label="dict.text"
              :value="dict.val"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="计划类型" prop="PlanTypeId" style="margin-left: 15px">
          <el-select
            class="set_radius"
            v-model="taskForm.PlanTypeId"
            placeholder="请选择计划类型"
            clearable
            style="width: 130px"
          >
            <el-option
              v-for="dict in planetbList"
              :key="dict.Id"
              :label="dict.Name"
              :value="dict.Id"
            />
          </el-select>
        </el-form-item>
        <el-form-item class="submit_button_con">
          <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
          <el-button type="primary" icon="el-icon-search" @click="handleClick"
            >搜索</el-button
          >
        </el-form-item>
      </el-form>
    </div>
    <div
      class="elbiaoge_elform"
      :style="{ 'min-height': tableConHeight + 'px','padding-top':'10px' }"
    >
      <el-tabs
        tab-position="top"
        v-model="recordType"
        @tab-click="switchRecordType"
      >
        <el-tab-pane label="我发起的" name="myPlane" class="record_tab">
          <el-row :gutter="10" class="mb8 button_row">
            <div>
              <el-col :span="1.5">
                <el-button type="primary" plain @click="handleStartTask">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                  <span style="margin-left: 6px">发起任务</span>
                </el-button>
              </el-col>
            </div>
            <right-toolbar
              :showSearch.sync="showSearch"
              @queryTable="getTaskList"
            ></right-toolbar>
          </el-row>
          <taskList
            :loading="loading"
            :tbList="tbList"
            :cellSty="cellSty"
            @reloadList="handleClick"
          ></taskList>
        </el-tab-pane>
        <el-tab-pane label="计划记录" name="planeRecord">
          <div style="position:relative;z-index:999;">
             <right-toolbar
                :showSearch.sync="showSearch"
                @queryTable="getTaskList"
              ></right-toolbar>
            </div>
          <el-tabs
            style="margin-top:10px;"
            v-model="listActiveType"
            type="card"
            @tab-click="handleClick"
          >
            <div class="status_ul" v-if="listActiveType != 'first'">
              <div class="status_li" v-for="(item, ix) in statusList" :key="ix">
                <div
                  class="li_block"
                  :style="{ '--bgcolor': item.color }"
                ></div>
                <div class="li_text">{{ item.text }}</div>
              </div>
            </div>
            <el-tab-pane label="列表" name="first">
              <taskList ref="taskList" :loading="loading" :tbList="tbList" :cellSty="cellSty" @reloadList="handleClick"></taskList>
            </el-tab-pane>
            <el-tab-pane label="日视图" name="second">
              <dayTask
                :loading="loading"
                :tbList="tbList"
                :cellSty="cellSty"
                :statusList="statusList"
              ></dayTask>
            </el-tab-pane>
            <el-tab-pane label="月视图" name="third">
              <monthTask
                :loading="loading"
                :tbList="tbList"
                :cellSty="cellSty"
                :statusList="statusList"
              ></monthTask>
            </el-tab-pane>
          </el-tabs>
        </el-tab-pane>
      </el-tabs>

      <pagination
        v-show="total > 0"
        :total="total"
        :page.sync="taskForm.pageNum"
        :limit.sync="taskForm.pageSize"
        @pagination="loadTaskList"
      />
      <plane_dialog ref="planedialog" @openTaskAdd="openTaskAdd"></plane_dialog>
      <task_add ref="task_add" @reloadList="handleClick"></task_add>
      <org-picker
        :multiple="false"
        ref="flowCreatedUser"
        @ok="selectLeadered"
      />
    </div>
  </div>
</template>
  <script>
import OrgPicker from "@/views/flowable/common/OrgPicker";
import {
  devPlaneTaskList,
  dayTaskList,
  devPlaneList,
} from "@/api/after/devplane";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import taskList from "./taskList";
import dayTask from "./dayTask";
import monthTask from "./monthTask";
import plane_dialog from "./plane_dialog";
import task_add from "./task_add";
import dayjs from "dayjs";
import { devPlaneTaskInfo } from "@/api/after/devplane";
export default {
  mixins: [resizeTableCon],
  components: {
    taskList,
    dayTask,
    plane_dialog,
    task_add,
    monthTask,
    OrgPicker,
  },
  data() {
    return {
      recordType: "myPlane",
      showSearch: true,
      taskForm: {
        UserId: '',
        pageNum: 1,
        pageSize: 10,
        TaskStatus: "",
        UserName: '',
        PlanTypeId: "",
      },
      dateRange: [],
      tbList: [],
      loading: false,
      total: 0,
      listActiveType: "first",
      statusList: [], //状态列表
      planeForm: {
        pageNum: 1,
        pageSize: 0,
        CanStart: true,
      },
      planetbList: [], //计划类型
    };
  },
  mounted() {
    let pars = this.$route.query;
    if (pars.id) {
      this.recordType='planeRecord'
      if(pars.id!=0){
        this.$nextTick(()=>{
          this.getPlaneTaskInfo(pars.id)
          // this.$refs.task_add.openDialog(pars.id);
        })
      }
    }
    this.setstatusList();
    this.tbList = [];
    this.switchRecordType();
    this.$nextTick(() => {
      this.getDevPlaneList();
    });
  },
  methods: {
    getPlaneTaskInfo(Id){
      //获取计划任务详情
      devPlaneTaskInfo({id:Id}).then(res=>{
          let data=res.data
          // console.log("流程信息",data);
          this.$refs.taskList.viewInfo(data)
      })
    },
    getDevPlaneList() {
      devPlaneList(this.planeForm)
        .then((res) => {
          // console.log(res,'计划类型');
          this.planetbList = JSON.parse(JSON.stringify(res.data.List));
          // for(let i=0;i<5;i++){
          //     this.tbList=[...this.tbList,...this.tbList]
          // }
        })
        .catch((err) => {});
    },
    selectLeadered(val) {
      //选择人
      this.taskForm.UserId = val[0].id;
      this.taskForm.UserName = val[0].name;
      // this.devplaneFrom.Avatar=val[0].avatar
      this.$forceUpdate();
    },
    getCreatedFocus() {
      //发起人选择开始
      this.$refs.selectFlowCreatedUser.blur();
      let flowCreatedUser = [];
      this.$refs.flowCreatedUser.show(flowCreatedUser, "user");
    },
    setstatusList() {
      this.statusList = [
        {
          text: "进行中",
          color: "#F158D7",
          val: 0,
        },
        {
          text: "待执行",
          color: "#DEA11E",
          val: 1,
        },
        {
          text: "执行中",
          color: "#358AEF",
          val: 2,
        },
        {
          text: "已完成",
          color: "#78BF34",
          val: 3,
        },
        {
          text: "已过期",
          color: "#B5B5B5",
          val: 4,
        },
        {
          text: "已验收",
          color: "#47CFB6",
          val: 5,
        },
        {
          text: "验收失败",
          color: "#EF357E",
          val: 6,
        },
        {
          text: "已作废",
          color: "#666666",
          val: 7,
        },
      ];
    },
    openTaskAdd(item) {
      //打开添加
      this.$refs.task_add.openDialog(item.Id);
      // this.$refs.task_add.openAddDevice(item.Id)
    },
    handleClick() {
      if (this.listActiveType == "first") {
        this.taskForm.pageNum = 1;
        this.tbList = [];
        this.loading = true;
        this.getTaskList();
      } else if (
        this.listActiveType == "second" ||
        this.listActiveType == "third"
      ) {
        this.taskForm.pageNum = 1;
        this.tbList = [];
        this.loading = true;
        this.getDayTask();
      }
    },
    /** 重置按钮操作 */
    resetQuery() {
        this.dateRange = [];
        this.resetForm("taskForm");
        this.handleClick();
    },
    handleStartTask() {
      //手动开始任务
      this.$refs.planedialog.openDialog();
    },
    loadTaskList() {
      if (this.listActiveType == "first") {
        this.getTaskList();
      } else if (
        this.listActiveType == "second" ||
        this.listActiveType == "third"
      ) {
        this.getDayTask();
      }
    },
    switchRecordType() {
      this.taskForm.pageNum = 1;
      this.tbList = [];
      if (this.recordType == "myPlane") {
        this.taskForm.UserId = this.$store.getters.uid;
        this.getTaskList();
      } else {
        delete this.taskForm.UserId;
        this.loadTaskList();
      }
    },
    getTaskList() {
      this.loading = true;
      devPlaneTaskList(this.addDateRange(this.taskForm, this.dateRange))
        .then((res) => {
          // console.log("计划任务",res);
          this.tbList = res.data.List;
          this.total = res.data.Total;
          this.loading = false;
        })
        .catch((err) => {
          this.loading = false;
        });
    },
    getDayTask() {
      //获取日视图
      this.loading = true;
      dayTaskList(this.addDateRange(this.taskForm, this.dateRange))
        .then((res) => {
          // console.log('日视图',res);
          this.tbList = [];
          res.data.List.map((row, index) => {
            let rowBeginIndex = 0;
            row.TaskList.map((rw, ix) => {
              rowBeginIndex = rowBeginIndex + ix;
              let obj = {};
              obj.TaskList = [];
              obj.TaskList2 = JSON.parse(JSON.stringify(row.TaskList));
              obj.TaskList.push(JSON.parse(JSON.stringify(rw)));
              obj.lenindex = row.TaskList.map((rs) => rs.TaskStatus);
              obj.DeviceName = row.DeviceName
                ? row.DeviceName
                : row.TargetDevice.Name;
              obj.RoomNames = row.RoomNames;
              obj.TargetId = row.TargetId;
              obj.TaskIdList = row.TaskIdList;
              obj.TaskIds = row.TaskIds;
              obj.keyId = ix + "index" + index;
              obj.index = rowBeginIndex;
              let nowmonth = dayjs().month();
              let nowYear = dayjs().year();
              if (this.listActiveType == "second") {
                obj.TaskList2 = row.TaskList.filter(
                  (row2) =>
                    nowYear == dayjs(row2.StartOn).year() &&
                    nowmonth == dayjs(row2.StartOn).month()
                );
              } else if (this.listActiveType == "third") {
                obj.TaskList2 = row.TaskList.filter(
                  (row2) => nowYear == dayjs(row2.StartOn).year()
                );
              }
              let dates = obj.TaskList2.map((item) => new Date(item.StartOn));
              let dates2 = obj.TaskList2.map((item) => new Date(item.EndOn));
              // 使用reduce获取最小和最大时间
              // const minTime = dates.reduce((min, current) => min > current ? current : min, new Date());
              // const maxTime = dates.reduce((max, current) => max < current ? current : max, new Date());
              // 获取最小时间
              const minTime = new Date(Math.min.apply(null, dates));

              // 获取最大时间
              const maxTime = new Date(Math.max.apply(null, dates2));
              obj.minTime = dayjs(minTime).format("YYYY-MM-DD HH:mm:ss");
              obj.maxTime = dayjs(maxTime).format("YYYY-MM-DD HH:mm:ss");
              // obj.statusVal=rw.TaskStatus
              this.tbList.push(obj);
            });
          });
          // this.tbList=res.data.List
          this.total = res.data.Total;
          this.loading = false;
        })
        .catch((err) => {
          this.loading = false;
        });
    },
  },
};
</script>
  <style lang="less" scoped>
::v-deep #tab-myPlane.el-tabs__item {
  padding-left: 0;
}
::v-deep #tab-planeRecord.el-tabs__item {
  padding-left: 0;
}
.el-select {
  ::v-deep input {
    width: 100%;
  }
}
::v-deep .el-tabs {
  position: relative;
  .el-tabs__item{
    width:200px;text-align: center;
  }
  .el-tabs__content {
    position: static;
  }
}
.status_ul {
  display: flex;
  align-items: center;
  flex-wrap: wrap !important;
  z-index: 1;
  width: calc(100% - 252px) !important;
  margin: 12px;
}
.status_li {
  display: flex;
  align-items: center;
  font-size: 14px;
  color: #333333;
  margin-left: 10px;
  .li_block {
    width: 12px;
    height: 12px;
    margin-right: 5px;
    background: var(--bgcolor);
  }
}
</style>