<template>
  <div style="padding:20px 20px 0 20px;height:100%" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form :model="queryParams" class="biaodan" ref="queryForm" :inline="true">
          <el-form-item label="盘点名称" prop="Name">
            <el-input v-model="queryParams.Name" placeholder="请输入盘点名称"></el-input>
          </el-form-item>
          <el-form-item label="盘点状态" prop="Status">
            <el-select v-model="queryParams.Status" placeholder="请选择仓库状态">
              <el-option label="待提交" value="0"></el-option>
              <el-option label="待开始" value="1"></el-option>
              <el-option label="初盘中" value="2"></el-option>
              <el-option label="复盘中" value="3"></el-option>
              <el-option label="已结束" value="4"></el-option>
              <el-option label="已修正" value="5"></el-option>
              <el-option label="已取消" value="6"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="初盘时间">
            <el-date-picker class="form_input_style" v-model="dateRange" style="width:232px" value-format="yyyy-MM-dd"
              type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
          </el-form-item>
          <!-- <el-col class="float_right" :span="24"> -->
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
          </el-form-item>
          <!-- </el-col> -->
        </el-form>
      </div>
      <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-button type="primary" plain @click="handleAdd">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">新增</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button :disabled="!AllCanDel" type="danger" plain @click="handleDelete()">
                <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                <span style="margin-left:6px">删除</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button :disabled="!AllCanStart" type="primary" plain @click="onStartInv()">
                <i class="zhongtaiiconfont zhongtai-icon-xiugai"></i>
                <span style="margin-left:6px">开始初盘</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button :disabled="!AllCheckStart" type="primary" plain @click="onCheckInv()">
                <i class="zhongtaiiconfont zhongtai-icon-xiugai"></i>
                <span style="margin-left:6px">开始复盘</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button :disabled="!AllFinish" icon="el-icon-circle-check" type="primary" plain @click="onFinishInv()">
                <span style="margin-left:6px">完成盘点</span>
              </el-button>
            </el-col>

          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table border v-loading="loading" :data="tbList" class="data_table" :header-cell-style="cellSty"
          style="width:100%" row-key="Id" @selection-change="handleSelectionChange">
          <el-table-column type="index" align="center" width="50">
          </el-table-column>
          <el-table-column type="selection" width="55">
          </el-table-column>
          <el-table-column label="盘点状态" align="center" width="120">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status == 0" type="warning">待提交</el-tag>
              <el-tag v-if="scope.row.Status == 1" type="warning">待开始</el-tag>
              <el-tag v-else-if="scope.row.Status == 2">初盘中</el-tag>
              <el-tag v-else-if="scope.row.Status == 3">复盘中</el-tag>
              <el-tag v-else-if="scope.row.Status == 4" type="danger">已结束</el-tag>
              <el-tag v-else-if="scope.row.Status == 5" type="success">已修正</el-tag>
              <el-tag v-else-if="scope.row.Status == 6" type="info">已取消</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="盘点名称" prop="Name" align="left"></el-table-column>
          <el-table-column label="盘点仓库" align="center" width="180">
            <template slot-scope="scope">
              <span v-if="scope.row.House != null">{{ scope.row.House.StoreName }}</span>
            </template>
          </el-table-column>
          <el-table-column label="创建时间" align="center" width="160">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.createTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="初盘人员" align="center" width="350">
            <template slot-scope="scope">
              <span>{{ getInventoryUsers(scope.row, 0) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="初盘时间" align="center" width="160">
            <template slot-scope="scope">
              <span v-if="scope.row.StartOn != null">{{ parseTime(scope.row.StartOn) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="复盘人员" align="center" width="350">
            <template slot-scope="scope">
              <span>{{ getInventoryUsers(scope.row, 1) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="复盘时间" align="center" width="160">
            <template slot-scope="scope">
              <span v-if="scope.row.CheckOn != null">{{ parseTime(scope.row.CheckOn) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="结束时间" align="center" width="160">
            <template slot-scope="scope">
              <span v-if="scope.row.EndOn != null">{{ parseTime(scope.row.EndOn) }}</span>
            </template>
          </el-table-column>

          <el-table-column label="操作" fixed="right" align="center" class-name="small-padding fixed-width" width="280">
            <template slot-scope="scope">
              <el-link icon="el-icon-edit" v-if="scope.row.Status == 0" type="primary"
                @click="handleUpdate(scope.row)">提交</el-link>
              <el-link icon="el-icon-delete" v-if="scope.row.Status == 0" type="danger" @click="handleDelete(scope.row)"
                style="margin-left:15px;">删除</el-link>
              <el-link icon="el-icon-video-play" type="primary" v-if="scope.row.Status == 1"
                @click="onStartInv(scope.row)" style="margin-left:15px;">初盘</el-link>
              <el-link icon="el-icon-video-play" type="primary" v-if="scope.row.Status == 2 && hasChecker(scope.row)"
                @click="onCheckInv(scope.row)" style="margin-left:15px;">复盘</el-link>

              <el-link icon="el-icon-circle-check" type="primary" v-if="scope.row.Status == 2 || scope.row.Status == 3"
                @click="onFinishInv(scope.row)" style="margin-left:15px;">完成</el-link>

              <el-link icon="el-icon-s-tools" type="primary" v-if="scope.row.Status == 4" @click="onRepair(scope.row)"
                style="margin-left:15px;">修正</el-link>

              <el-link icon="el-icon-switch-button" type="danger"
                v-if="scope.row.Status == 1 || scope.row.Status == 2 || scope.row.Status == 3"
                @click="onCancel(scope.row)" style="margin-left:15px;">取消</el-link>


              <el-link icon="el-icon-info" type="primary" v-if="scope.row.Status >0" @click="onItem(scope.row)"
                style="margin-left:15px;">明细</el-link>

            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize"
          @pagination="getList" />
      </div>

      <CheckAdd ref="addDlg" @reload="getList()"></CheckAdd>
      <CheckRevision ref="revisionDlg" @reload="getList()"></CheckRevision>
    </div>
  </div>
</template>
  
<script>
import {
  invList, delInv, startInv, cancelInv, checkInv, finishInv
} from "@/api/storage/inventory";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { checkPermi } from '@/utils/permission.js'

import CheckAdd from "./add.vue";
import CheckRevision from "./revision.vue"
export default {
  name: "HouseList",
  mixins: [resizeTableCon],
  components: { CheckAdd, CheckRevision },
  data() {
    return {
      // 遮罩层
      loading: true,
      // 显示搜索条件
      showSearch: true,
      // 表格树数据
      tbList: [],
      // 日期范围
      dateRange: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        Status: undefined,
        Name: undefined
      },
      // 总条数
      total: 0,
      multipleSelection: []
    };
  },
  computed: {
    AllCanDel: function () {
      if (this.multipleSelection.length == 0) return false;
      for (let i = 0; i < this.multipleSelection.length; i++) {
        if (this.multipleSelection[i].Status != 0) {
          return false;
        }
      }
      return true;
    },
    AllCanStart: function () {
      if (this.multipleSelection.length == 0) return false;
      for (let i = 0; i < this.multipleSelection.length; i++) {
        if (this.multipleSelection[i].Status != 1) {
          return false;
        }
      }
      return true;
    },
    AllCheckStart: function () {
      if (this.multipleSelection.length == 0) return false;
      for (let i = 0; i < this.multipleSelection.length; i++) {
        if (this.multipleSelection[i].Status != 2 || !this.multipleSelection[i].UserList.some(x => x.TimeIn == 1)) {
          return false;
        }
      }
      return true;
    },
    AllFinish: function () {
      if (this.multipleSelection.length == 0) return false;
      for (let i = 0; i < this.multipleSelection.length; i++) {
        if (this.multipleSelection[i].Status != 2 && this.multipleSelection[i].Status != 3) {
          return false;
        }
      }
      return true;
    },
  },
  created() {
    this.getList();
  },
  methods: {
    hasChecker(row) {
      return row.UserList.some(x => x.TimeIn == 1);
    },
    handleSelectionChange(val) {
      this.multipleSelection = val;
    },
    getInventoryUsers(row, ti) {
      if (row.UserList == null) {
        return "";
      }
      var tmpusers = "";
      row.UserList.filter(x => x.TimeIn == ti).forEach(element => {
        tmpusers += "," + element.UserInfo.RealName;
      });
      if (tmpusers != "") {
        tmpusers = tmpusers.substring(1);
      }
      return tmpusers;
    },

    checkItemPermi(value) {
      return checkPermi(value);
    },
    /** 查询列表 */
    getList() {
      this.loading = true;
      invList(this.addDateRange(this.queryParams, this.dateRange)).then(response => {
        this.tbList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
    },

    /** 搜索按钮操作 */
    handleQuery() {
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
    /** 新增按钮操作 */
    handleAdd() {
      this.$refs.addDlg.openDlg("新增盘点");
    },
    onRepair(row) {
      this.$refs.revisionDlg.openDlg(row.Id);
    },
    async handleUpdate(row) {
      await this.$refs.addDlg.openDlg("编辑盘点", row.Id);
    },
    onCancelInv(row) {
      this.$modal
        .confirm('是否确认取消盘点"' + row.Name + '"？')
        .then(() => {
          return cancelInv([row.Id]);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("操作成功");
        })
        .catch(() => { });
    },
    onStartInv(row) {
      if (row == null) {
        this.$modal
          .confirm('是否确认批量开始初盘')
          .then(() => {
            return startInv(this.multipleSelection.map(x => x.Id));
          })
          .then(() => {
            this.getList();
            this.$modal.msgSuccess("操作成功");
          })
          .catch(() => { });
      }
      else {
        this.$modal
          .confirm('是否确认"' + row.Name + '"开始初盘？')
          .then(() => {
            return startInv([row.Id]);
          })
          .then(() => {
            this.getList();
            this.$modal.msgSuccess("操作成功");
          })
          .catch(() => { });
      }

    },
    onCheckInv(row) {
      if (row == null) {
        this.$modal
          .confirm('是否确认批量开始复盘')
          .then(() => {
            return checkInv(this.multipleSelection.map(x => x.Id));
          })
          .then(() => {
            this.getList();
            this.$modal.msgSuccess("操作成功");
          })
          .catch(() => { });
      }
      else {
        this.$modal
          .confirm('是否确认"' + row.Name + '"开始复盘？')
          .then(() => {
            return checkInv([row.Id]);
          })
          .then(() => {
            this.getList();
            this.$modal.msgSuccess("操作成功");
          })
          .catch(() => { });
      }
    },
    onFinishInv(row) {
      if (row == null) {
        this.$modal
          .confirm('是否确认批量完成盘点单')
          .then(() => {
            return finishInv(this.multipleSelection.map(x => x.Id));
          })
          .then(() => {
            this.getList();
            this.$modal.msgSuccess("操作成功");
          })
          .catch(() => { });
      }
      else {
        this.$modal
          .confirm('是否确认完成盘点"' + row.Name + '"？')
          .then(() => {
            return finishInv([row.Id]);
          })
          .then(() => {
            this.getList();
            this.$modal.msgSuccess("操作成功");
          })
          .catch(() => { });
      }
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      if (row == null) {
        this.$modal
          .confirm('是否确认批量删除盘点单')
          .then(() => {
            return delInv(this.multipleSelection.map(x => x.Id));
          })
          .then(() => {
            this.getList();
            this.$modal.msgSuccess("删除成功");
          })
          .catch(() => { });
      }
      else {
        this.$modal
          .confirm('是否确认删除名称为"' + row.Name + '"的数据项？')
          .then(() => {
            return delInv([row.Id]);
          })
          .then(() => {
            this.getList();
            this.$modal.msgSuccess("删除成功");
          })
          .catch(() => { });
      }

    },
    onItem(row){

    }
  }
};
</script>
<style lang="scss"></style>