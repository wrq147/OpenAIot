<template>
    <div class="big_con" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item prop="banZuName">
                  <el-input v-model="queryParams.banZuName" placeholder="请输入班组名称" clearable />
              </el-form-item>
              <!-- <el-form-item>
                <el-date-picker
                    v-model="time"
                    type="daterange"
                    range-separator="至"
                    value-format="yyyy-MM-dd"
                    start-placeholder="开始日期"
                    end-placeholder="结束日期"
                />
              </el-form-item> -->
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
                  <el-button type="primary" icon="el-icon-plus" plain @click="handleAdd('')">新增班组</el-button>
                </el-col>
              </div>
            </el-row>
            <div class="tableList">
                <div class="left">
                    <el-table ref="singleTable" v-loading="loading" border :data="batchList" style="width:100%" highlight-current-row @current-change="onDeviceChange">
                        <el-table-column label="序号" align="center" type="index" width="50" />
                        <el-table-column label="班组名称" align="center" prop="BanZuName" />
                        <el-table-column label="人数" align="center" prop="RenShu" width="100" />
                        <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="300">
                            <template slot-scope="scope">
                                <el-button type="text" icon="el-icon-plus" style="color:#E6A23C" @click="handleAddPeople(scope.row.Id)">添加人员</el-button>
                                <el-button type="text" icon="el-icon-edit" @click="handleAdd(scope.row)">编辑</el-button>
                                <el-button type="text" icon="el-icon-delete" style="color:red" @click="handleCancel(scope.row.Id)">删除</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                    <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                        :limit.sync="queryParams.pageSize" @pagination="getList" />
                </div>
                <div class="right">
                    <el-table v-loading="loading" border :data="peopleList" style="width:100%">
                        <el-table-column label="人员编号" align="center" prop="UserId" />
                        <el-table-column label="姓名" align="center" prop="UserName" />
                        <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="150">
                            <template slot-scope="scope">
                                <el-button type="text" icon="el-icon-delete" style="color:red" @click="handlePeopleCancel(scope.row.BanZuId,scope.row.UserId)">删除</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                </div>
            </div>
          </div>
        </el-col>
      </el-row>
    </div>
    <!-- 新增/编辑产品批次弹窗 -->
    <add-Group ref="addGroup" :title="title" :dialog-visible="open" @cancelForm="cancelForm" @getList="getList" />
    <!-- 新增人员弹窗 -->
    <add-People ref="addGroupPeople" :dialog-visible="peopleOpen" @cancelForm="cancelForm" @getSelectPeople="getSelectPeople" />
  </div>
</template>

<script>
import { groupList, delGroup, delGroupPeople, addGroupPeople } from "@/api/scheduling/timeGroup";
import addGroup from './cmp/addGroup.vue'
import addPeople from './cmp/addPeople.vue'
export default {
  components: {
      addGroup,
      addPeople
  },
  name: "Index",
  data() {
    return {
      loading: false,
      open: false,
      peopleOpen: false,
      title: '新增班组',
      // 查询参数
      queryParams: {
          pageNum: 1,
          pageSize: 20,
          banZuName:'',
      },
      total: 0,
      batchList: [],
      activeIndex: null,
      peopleList: [],
      banZuId: '',
    };
  },
  created() {
      this.getList();
  },
  methods: {
      getList() {
          this.open = false;
          this.peopleOpen = false;
          this.loading = true;
          groupList(this.queryParams).then(response => {
              this.batchList = response.data.List;
              this.total = response.data.Total;
              if (this.activeIndex !== null) {
                setTimeout(() => {
                  this.peopleList = this.batchList[this.activeIndex].ChengYuan;
                  this.$refs.singleTable.setCurrentRow(this.batchList[this.activeIndex]);
                }, 300)
              }
              this.loading = false;
          })
      },
      /** 点击操作 */
      onDeviceChange(data) {
        if (data === null){
          return false
        }
        const index = this.batchList.findIndex(item => item.Id === data.Id);
        this.activeIndex = index;
        this.peopleList = data.ChengYuan;
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
              this.title = '新增班组';
              this.$refs['addGroup'].ruleForm={
                banZuName: ''
              };
          } else {
              this.title = '编辑班组';
              this.$refs['addGroup'].ruleForm={
                  banZuID: data.Id,
                  banZuName: data.BanZuName
              };
          }
          this.open = true;
      },
      /** 新增人员按钮操作 */
      handleAddPeople(id) {
        this.banZuId = id;
        this.peopleOpen = true;
      },
      getSelectPeople(list) {
        const data = {
          banZuId: this.banZuId,
          userId: list.Id
        }
        addGroupPeople(data).then(response => {
            this.$message.success('添加成功');
            this.getList();
        })
      },
      /** 删除按钮操作 */
      handleCancel(id) {
          this.$confirm('此操作将永久删除该班组, 是否继续?', '提示', {
              confirmButtonText: '确定',
              cancelButtonText: '取消',
              type: 'warning'
          }).then(() => {
            delGroup({ banZuId: id }).then(response => {
                  this.$message({
                      type: 'success',
                      message: '删除成功!'
                  });
                  this.getList();
              })
          })
      },
      /** 删除人员按钮操作 */
      handlePeopleCancel(id, userId) {
          this.$confirm('此操作将永久删除该人员, 是否继续?', '提示', {
              confirmButtonText: '确定',
              cancelButtonText: '取消',
              type: 'warning'
          }).then(() => {
            delGroupPeople({ banZuId: id, userId }).then(response => {
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
          this.peopleOpen = false;
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
.tableList{
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
}
.tableList .left{
    width: 60% !important;
}
.tableList .right{
    width: 38% !important;
}
</style>
