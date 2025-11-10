<template>
    <div style="padding:20px 20px 0 20px;height:100%" id="big_con">
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
            <div class="elbiaoge_elform">
              <el-row :gutter="10" class="mb8 button_row">
                <div>
                  <el-col :span="1.5">
                    <el-button type="primary" icon="el-icon-plus" plain @click="handleAdd('')">新增不良品项</el-button>
                  </el-col>
                </div>
              </el-row>
  
              <el-table v-loading="loading" :data="defectList" class="data_table" style="width:100%">
                <el-table-column label="不良品项编号" align="center" prop="Id" :show-overflow-tooltip="true" />
                <el-table-column label="不良品项名称" align="center" prop="DefectName" />
                <el-table-column label="不良类别" align="center" prop="DefectCategory" />
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
      <!-- 新增/编辑不良品项弹窗 -->
      <add-defect ref="addDefect" :title="title" :dialog-visible="open" @cancelForm="cancelForm" @getList="getList" />
    </div>
</template>
  
<script>
  import { defectList, defectRemove } from "@/api/mes/defect";
  import addDefect from './cmp/addDefect.vue'
  export default {
    name: "BatchList",
    components: {
      addDefect
    },
    data() {
      return {
        loading: false,
        open: false,
        title: '新增不良品项',
        // 查询参数
        queryParams: {
          pageNum: 1,
          pageSize: 20,
          beginTime: '',
          endTime: ''
        },
        time: [],
        total: 0,
        defectList: []
      }
    },
    created() {
      this.getList();
    },
    methods: {
      getList() {
        this.open = false;
        this.loading = true;
        if(this.time.length > 0) {
          this.queryParams.beginTime = this.time[0];
          this.queryParams.endTime = this.time[1];
        }
        defectList(this.queryParams).then(response => {
          this.defectList = response.data.List;
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
        this.time= []; 
        this.resetForm("queryForm");
        this.handleQuery();
      },
      /** 新增按钮操作 */
      handleAdd(data) {
        if (data === '') {
          this.title = '新增不良品项';
          this.$refs['addDefect'].ruleForm = {
            id: '',
            defectName: '',
            defectCategory: '',
          };
        } else {
          this.title = '编辑不良品项';
          this.$refs['addDefect'].ruleForm = {
            id: data.Id,
            defectName: data.DefectName,
            defectCategory: data.DefectCategory
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
            defectRemove({ id: id }).then(res => {
              this.$message.success('删除成功!')
              this.getList()
            })
        }).catch(() => { })
      },
      cancelForm() {
        this.open = false;
      },
    }
  }
  </script>