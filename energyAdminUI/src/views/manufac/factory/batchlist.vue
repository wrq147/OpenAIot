<template>
  <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item prop="Key">
                <el-input v-model="queryParams.Key" placeholder="请输入搜索的关键字" clearable />
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
                  <el-button type="primary" icon="el-icon-plus" plain @click="handleAdd('')">新增批次</el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="success" icon="el-icon-refresh" plain @click="handleSync">自动同步</el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="warning" icon="el-icon-upload" plain @click="handleExport">批量导入</el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="danger" :disabled="multiple" icon="el-icon-delete" plain
                    @click="handleDelete('')">批量删除</el-button>
                </el-col>
              </div>
            </el-row>

            <el-table v-loading="loading" :data="batchList" class="data_table" style="width:100%"
              @selection-change="handleSelectionChange">
              <el-table-column type="selection" align="center" width="50" />
              <el-table-column label="批次编号" align="center" prop="Number" :show-overflow-tooltip="true"/>
              <el-table-column label="通讯编号" align="center" prop="LNumber" :show-overflow-tooltip="true"/>
              <el-table-column label="批次名称" align="center" prop="BatchName" />
              <el-table-column label="产品标签" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.ProductLabel == "F" ? "成品" : "半成品" }}</span>
                </template>
              </el-table-column>
              <el-table-column label="产品名称" align="center" prop="ProductName" />
              <el-table-column label="图片" align="center" width="100">
                <template slot-scope="scope">
                  <el-image style="width: 80px;height:80px" fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'"
                    :preview-src-list="[scope.row.PhotoUrl]" />
                </template>
              </el-table-column>
              <el-table-column label="单位名称" align="center" prop="Unit" />
              <el-table-column label="成本单价" align="center" prop="Price" />
              <el-table-column label="供应商" align="center" prop="SupplierName" />
              <el-table-column label="备注说明" align="center" prop="Remark" />
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
    <!-- 新增/编辑产品批次弹窗 -->
    <add-batch ref="addBatch" :title="title" :dialog-visible="open" @cancelForm="cancelForm" @getList="getList" />
    <!-- 批量导入产品批次弹窗 -->
    <export-batch ref="exportBatch" :dialog-visible="openExport" @cancelForm="cancelForm" @getList="getList" />
  </div>
</template>

<script>
import { BatchList, delBatch, SyncDeviceToBatch, delListBatch } from "@/api/manufac/batchList";
import addBatch from './cmp/addBatch.vue'
import exportBatch from './cmp/exportBatch.vue'
export default {
  name: "BatchList",
  components: {
    addBatch,
    exportBatch
  },
  data() {
    return {
      loading: false,
      open: false,
      openExport: false,
      title: '新增批次',
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 20,
        Key: '',
      },
      total: 0,
      batchList: [],
      ids: [],
      // 非多个禁用
      multiple: true,
    }
  },
  created() {
    this.getList();
  },
  methods: {
    getList() {
      this.open = false;
      this.openExport = false;
      this.loading = true;
      BatchList(this.queryParams).then(response => {
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
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
    async handleSync() {
      const loading = this.$loading({
        lock: true,
        text: '同步中...',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });
      await SyncDeviceToBatch();
      this.getList();
      loading.close();
    },
    /** 新增按钮操作 */
    handleAdd(data) {
      if (data === '') {
        this.title = '新增批次';
        this.$refs['addBatch'].ruleForm = {
          Number: '',
          LNumber: '',
          ProductId: '',
          ProductName: '',
        };
      } else {
        this.title = '编辑批次';
        this.$refs['addBatch'].ruleForm = {
          Id: data.Id,
          Number: data.Number,
          LNumber: data.LNumber,
          ProductId: data.ProductId,
          ProductName: data.ProductName,
        };
      }
      this.open = true;
    },
    /** 删除按钮操作 */
    handleDelete(id) {
      if (id === '' && this.ids.length < 1) {
        this.$message.error('请选择要删除的数据')
        return
      }
      this.$confirm('此操作将永久删除该数据, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        if (id === '') {
          delListBatch({ ids: this.ids }).then(res => {
            this.$message.success('删除成功!')
            this.getList()
          })

        }
        else {
          delBatch({ id: id }).then(res => {
            this.$message.success('删除成功!')
            this.getList()
          })
        }

      }).catch(() => { })

    },

    /** 点击批量导入 */
    handleExport() {
      this.openExport = true;
    },
    cancelForm() {
      this.open = false;
      this.openExport = false;
    },
    handleSelectionChange(selection) {
      this.ids = []
      this.ids = selection.map(item => item.Id)
      this.multiple = !selection.length;
    }
  }
}
</script>