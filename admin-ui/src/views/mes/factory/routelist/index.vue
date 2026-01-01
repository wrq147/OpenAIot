<template>
  <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
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
          <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" icon="el-icon-plus" plain @click="handleAdd('')">新增工艺路线</el-button>
                </el-col>
              </div>
            </el-row>

            <el-table v-loading="loading" :data="routeList" class="data_table" style="width:100%">
              <!-- <el-table-column label="工艺路线编号" align="center" prop="Id" :show-overflow-tooltip="true" /> -->
              <el-table-column label="工艺路线名称" align="center" prop="RouteName" />
              <el-table-column label="创建者" align="center" prop="createName" />
              <el-table-column label="更新者" align="center" prop="updateName" />
              <el-table-column label="创建时间" align="center" prop="createTime" />
              <el-table-column label="更新时间" align="center" prop="updateTime" />
              <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="150">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-edit" @click="handleAdd(scope.row)">编辑</el-button>
                  <el-button type="text" icon="el-icon-delete" style="color:red"
                    @click="handleDelete(scope.row.Id)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>
            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="getList" />
          </div>
        </el-col>
      </el-row>
    </div>
    <!-- 新增/编辑工艺路线弹窗 -->
    <add-route ref="addRoute" :title="title" :routeList="routeList" :filedTableList="filedTableList"
      :dialog-visible="open" @cancelForm="cancelForm" @getList="getList" />
  </div>
</template>

<script>
import { orgFormFields } from "@/api/factory/customFields";
import { routeList, routeRemove } from "@/api/mes/processRoute";
import addRoute from './cmp/addRoute.vue'
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "BatchList",
  components: {
    addRoute
  },
  mixins: [resizeTableCon],
  data() {
    return {
      loading: false,
      open: false,
      title: '新增工艺路线',
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 20,
        beginTime: '',
        endTime: ''
      },
      time: [],
      total: 0,
      routeList: [],
      filedTableList: []
    }
  },
  created() {
    this.getProductCustomFiled();
    this.getList();
  },
  methods: {
    getList() {
      this.open = false;
      this.loading = true;
      if (this.time.length > 0) {
        this.queryParams.beginTime = this.time[0];
        this.queryParams.endTime = this.time[1];
      }
      routeList(this.queryParams).then(response => {
        this.routeList = response.data.List;
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
        this.title = '新增工艺路线';
        this.$refs['addRoute'].ruleForm = {
          id: '',
          routeName: '',
          toHouseId: '',
          items: [
            {
              ExtVals:this.filedTableList.reduce((acc, item) => {
                acc[item.mapid] = ''; // 初始化为空值
                return acc;
              }, {}),
              id: "",
              OrgId: this.$store.state.user.orgId,
              OperId: '',
              RouteId: '',
              PropOf: "",
              WorkTime: '',
              Sequence: '',
            }
          ]
        };
      } else {
        this.title = '编辑工艺路线';
        this.$refs['addRoute'].ruleForm = {
          id: data.Id,
          routeName: data.RouteName,
          toHouseId: data.ToHouseId,
          items: data.Items
        };
      }
      this.open = true;
    },
    /** 删除按钮操作 */
    handleDelete(id) {
      this.$confirm('此操作将永久删除该数据, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        routeRemove({ id: id }).then(res => {
          this.$message.success('删除成功!')
          this.getList()
        })
      }).catch(() => { })
    },

    //获取自定义的字段
    async getProductCustomFiled() {
      let res = await orgFormFields({ field: "工序", ext: true, isfixed: false });
      this.filedTableList = res.data;
    },

    cancelForm() {
      this.open = false;
    },
  }
}
</script>