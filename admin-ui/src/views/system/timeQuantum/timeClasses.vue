<template>
    <div class="big_con" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item prop="banCiName">
                  <el-input v-model="queryParams.banCiName" placeholder="请输入班次名称" clearable />
              </el-form-item>
              <el-form-item>
                <el-date-picker
                    v-model="time"
                    type="daterange"
                    range-separator="至"
                    value-format="yyyy-MM-dd"
                    start-placeholder="开始日期"
                    end-placeholder="结束日期"
                />
              </el-form-item>
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div class="elbiaoge_elform">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" icon="el-icon-plus" plain @click="handleAdd('')">新增班次</el-button>
                </el-col>
              </div>
            </el-row>

            <el-table v-loading="loading" :data="batchList" class="data_table"  style="width:100%">
              <el-table-column label="序号" align="center" type="index" width="50" />
              <el-table-column label="班次名称" align="center" prop="BanCiName" />
              <el-table-column label="开始时间" align="center" prop="StartDate" />
              <el-table-column label="结束时间" align="center" prop="EndTDate" />
              <el-table-column label="周期数" align="center" prop="ZhouQiShu" />
              <el-table-column label="周期单位" align="center" prop="ZhouQiDanWei" />
              <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="220">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-date" style="color:#E6A23C" @click="handlePeriod(scope.row)">设置周期</el-button>
                  <el-button type="text" icon="el-icon-edit" @click="handleAdd(scope.row)">编辑</el-button>
                  <el-button type="text" icon="el-icon-delete" style="color:red" @click="handleCancel(scope.row.Id)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>
            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="getList" />
          </div>
        </el-col>
      </el-row>
    </div>
    <!-- 新增/编辑产品批次弹窗 -->
    <add-Classes ref="addClasses" :title="title" :dialog-visible="open" @cancelForm="cancelForm" @getList="getList" />
    <!-- 设置周期弹窗 -->
    <add-Period ref="addPeriod" :dialog-visible="openPeriod" @cancelForm="cancelForm" @getList="getList" />
  </div>
</template>

<script>
import { classesList, delClasses } from "@/api/scheduling/timeClasses";
import addClasses from './cmp/addClasses.vue'
import addPeriod from './cmp/addPeriod.vue'
export default {
  components: {
      addClasses,
      addPeriod
  },
  name: "Index",
  data() {
    return {
      loading: false,
      open: false,
      openPeriod: false,
      title: '新增批次',
      time: [],
      // 查询参数
      queryParams: {
          pageNum: 1,
          pageSize: 20,
          beginTime: '',
          endTime: '',
          banCiName:'',
      },
      total: 0,
      batchList: []
    };
  },
  created() {
      this.getList();
  },
  methods: {
      getList() {
          this.open = false;
          this.openPeriod = false;
          this.loading = true;
          if (this.time === null) {
              this.queryParams.beginTime = ''
              this.queryParams.endTime = ''
          } else {
              this.queryParams.beginTime = this.time[0] ? this.time[0] : ''
              this.queryParams.endTime = this.time[1] ? this.time[1] : ''
          }
          classesList(this.queryParams).then(response => {
              this.batchList = response.data.List;
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
      /** 新增按钮操作 */
      handleAdd(data) {
          if (data === '') {
              this.title = '新增班次';
              this.$refs['addClasses'].ruleForm={
                  banCiName: '',
                  deptId: '',
                  startDate: '',
                  endTDate: '',
                  zhouQiShu: 1,
                  zhouQiDanWei: '周'
              };
          } else {
              this.title = '编辑班次';
              this.$refs['addClasses'].ruleForm={
                  banCiID: data.Id,
                  deptId: data.DeptId,
                  banCiName: data.BanCiName,
                  startDate: data.StartDate,
                  endTDate: data.EndTDate,
                  zhouQiShu: data.ZhouQiShu,
                  zhouQiDanWei: data.ZhouQiDanWei
              };
          }
          this.open = true;
      },
      /** 设置周期按钮操作 */
      handlePeriod(data) {
        this.$refs['addPeriod'].ruleForm={
          shiDuan: data.ShiDuan,
          banCiID: data.Id,
          zhouQiShu: data.ZhouQiShu,
          zhouQiDanWei: data.ZhouQiDanWei
        }
        this.openPeriod = true;
      },
      /** 删除按钮操作 */
      handleCancel(id) {
          this.$confirm('此操作将永久删除该时段, 是否继续?', '提示', {
              confirmButtonText: '确定',
              cancelButtonText: '取消',
              type: 'warning'
          }).then(() => {
            delClasses({ banCiId: id }).then(response => {
                  this.$message({
                      type: 'success',
                      message: '删除成功!'
                  });
                  this.getList();
              })
          })
      },
      cancelForm() {
          this.open = false;
          this.openPeriod = false;
      }
  }
};
</script>

<style lang="scss" scoped>
.big_con{
  background-color: #F0F2F5;
}
.from_con{
  margin-bottom: 5px;
}
</style>
