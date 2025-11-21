<template>
  <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="状态" prop="Status">
                <el-select v-model="queryParams.Status" clearable placeholder="请选择状态">
                  <el-option label="进行中" :value="0" />
                  <el-option label="已完成" :value="1" />
                </el-select>
              </el-form-item>
              <el-form-item label="创建日期">
                <el-date-picker class="set_radius" v-model="time" style="width:232px" value-format="yyyy-MM-dd"
                  type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期" />
              </el-form-item>
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div class="elbiaoge_elform">
            <el-table v-loading="loading" :data="taskList" class="data_table" style="width:100%">
              <el-table-column label="工单编号" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.WorkNumber }}</span>
                </template>
              </el-table-column>
              <el-table-column label="产品编号" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.SkuNumber }}</span>
                </template>
              </el-table-column>
              <el-table-column label="产品名称" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.ProductName }}</span>
                </template>
              </el-table-column>
              <el-table-column label="工序名称" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.OperName }}</span>
                </template>
              </el-table-column>
              <el-table-column label="允许报工人员" width="180" align="center">
                <template slot-scope="scope">
                  <div class="assigned-users-container">
                    <div v-for="(user, index) in parseAssignedUsers(scope.row.AssignedUser)" :key="index"
                      :class="['assigned-user-tag', user.type === 'user' ? 'user-tag' : 'dept-tag']">
                      <i :class="['el-icon', user.type === 'user' ? 'el-icon-user' : 'el-icon-office-building']"></i>
                      <span class="tag-text">{{ user.name || (user.type === "user" ? "未知人员" : "未知部门") }}</span>
                    </div>
                    <span v-if="!scope.row.AssignedUser || parseAssignedUsers(scope.row.AssignedUser).length === 0"
                      class="no-user-text">
                      无指定人员
                    </span>
                  </div>
                </template>
              </el-table-column>
              <el-table-column label="报工数配比" align="center" prop="PropOf" />
              <el-table-column label="工时(分钟)" align="center" prop="WorkTime" />
              <el-table-column label="总工时(分钟)" align="center" prop="WorkTimeTotal" />
              <el-table-column label="计划数" align="center" prop="PlanNum" />
              <el-table-column label="良品数" align="center" prop="GoodNum" />
              <el-table-column label="不良品数" align="center" prop="DefectNum" />
              <el-table-column label="状态" align="center">
                <template slot-scope="scope">
                  <span v-if="scope.row.IsFinish == true">已完成</span>
                  <span v-else>进行中</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="150">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-notebook-2" @click="handleDetail(scope.row)">详情</el-button>
                  <el-button type="text" icon="el-icon-document-add" @click="handleReport(scope.row)">报工</el-button>
                </template>
              </el-table-column>
            </el-table>
            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="getList" />
          </div>
        </el-col>
      </el-row>
    </div>
  </div>
</template>

<script>
import { taskList } from "@/api/mes/task";
export default {
  name: "taskList",
  data() {
    return {
      loading: false,
      // 查询参数
      queryParams: {
        Status: -1,
        pageNum: 1,
        pageSize: 20,
        beginTime: '',
        endTime: ''
      },
      time: [],
      total: 0,
      taskList: []
    }
  },
  created() {
    this.getList();
  },
  methods: {
    parseAssignedUsers(assignedUsers) {
      if (!assignedUsers || assignedUsers === "") return [];
      try {
        return JSON.parse(assignedUsers);
      } catch (e) {
        console.error('解析分配用户数据失败:', e);
        return [];
      }
    },
    getList() {
      this.open = false;
      this.loading = true;
      if (this.time.length > 0) {
        this.queryParams.beginTime = this.time[0];
        this.queryParams.endTime = this.time[1];
      }
      else {
        this.queryParams.beginTime = '';
        this.queryParams.endTime = '';
      }
      taskList(this.queryParams).then(response => {
        this.taskList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      })
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.time = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
  }
}
</script>
<style scoped>
/* 允许报工人员容器样式 */
.assigned-users-container {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  padding: 2px 0;
  min-height: 26px;
  align-items: center;
}

/* 标签通用样式 */
.assigned-user-tag {
  display: inline-flex;
  align-items: center;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
  line-height: 1.4;
  white-space: nowrap;
}

/* 用户标签样式 */
.user-tag {
  background-color: #e6f7ff;
  color: #1890ff;
  border: 1px solid #91d5ff;
}

/* 部门标签样式 */
.dept-tag {
  background-color: #f6ffed;
  color: #52c41a;
  border: 1px solid #b7eb8f;
}

/* 图标样式 */
.assigned-user-tag .el-icon {
  margin-right: 4px;
  font-size: 12px;
}

/* 标签文本样式 */
.tag-text {
  max-width: 100px;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* 无人员提示文本 */
.no-user-text {
  color: #999;
  font-size: 12px;
  font-style: italic;
}
</style>