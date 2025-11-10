<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    <div>
      <div class="from_con" id="from_con">
        <el-form
          :model="queryParams"
          ref="queryForm"
          :inline="true"
          class="biaodan"
        >
          <el-form-item label="模板名称" prop="Name">
            <el-input
              v-model="queryParams.Name"
              placeholder="请输入模板名称"
              clearable
            />
          </el-form-item>

          <el-form-item label="创建日期">
            <el-date-picker
              class="set_radius"
              v-model="dateRange"
              style="width: 250px"
              value-format="yyyy-MM-dd"
              type="daterange"
              range-separator="-"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
            ></el-date-picker>
          </el-form-item>

          <el-form-item class="submit_button_con">
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
          </el-form-item>
        </el-form>
      </div>
      <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-button
                type="primary"
                plain
                icon="el-icon-plus"
                @click="handleAdd"
              >新增</el-button>
            </el-col>
          </div>
        </el-row>

        <el-table
          border
          v-loading="loading"
          :data="tbList"
          class="data_table"
          :header-cell-style="cellSty"
        >
          <el-table-column prop="Name" label="模板名称" align="left" width="260"></el-table-column>
          <el-table-column label="纸张方向" align="center">
            <template slot-scope="scope">
              <span v-if="scope.row.PaperDirection=='p'">纵向长</span>
              <span v-else-if="scope.row.PaperDirection=='l'">横向长</span>
            </template>
          </el-table-column>
          <el-table-column prop="PaperName" label="纸张类型" align="center" width="160"></el-table-column>
          <el-table-column label="纸张长宽" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.PaperWidth }}×{{ scope.row.PaperHeight }}</span>
            </template>
          </el-table-column>
          <el-table-column label="创建时间" align="center">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.createTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column
            label="操作"
            align="center"
            class-name="small-padding fixed-width"
            width="258"
          >
            <template slot-scope="scope">
              <el-button
                type="text"
                icon="el-icon-edit"
                @click="handleUpdate(scope.row)"
                v-hasPermi="['/ReportService/Print/Edit']"
              >修改</el-button>
              <el-button
                type="text"
                icon="el-icon-delete"
                @click="handleDelete(scope.row)"
                v-hasPermi="['/ReportService/Print/Remove']"
              >删除</el-button>
            </template>
          </el-table-column>
        </el-table>

        <pagination
          v-show="total>0"
          :total="total"
          :page.sync="queryParams.pageNum"
          :limit.sync="queryParams.pageSize"
          @pagination="getList"
        />
      </div>

  
    </div>


    <el-dialog
        title="请选择数据源"
        :close-on-click-modal="false"
        :visible.sync="open"
        width="600px"
      >
        <el-row :gutter="12">
          <el-col :span="8" class="source-item" v-for="item in dataList" :key="item.Id">
            <el-card class="card-item" shadow="hover" @click.native="newPrint(item.Id)">
              {{item.Name}}
            </el-card>
          </el-col>
        </el-row>
      </el-dialog>

  </div>
</template>

<script>
import {
  listPrintTemplate,
  deletePrintTemplate,
  dataList
} from "@/api/report/printTemplate";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "printList",
  mixins: [resizeTableCon],
  data() {
    return {
      // 遮罩层
      loading: true,
      tbList:[],
      // 总条数
      total: 0,
      // 日期范围
      dateRange: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        Name:'',
      },
      open:false,
      dataList:[]
    };
  },
  created() {
    this.getList();
  },

  methods: {
    getList() {
      this.loading=true;
      listPrintTemplate(this.addDateRange(this.queryParams, this.dateRange)).then(rsp=>{
        this.tbList=rsp.data.List;
        this.total = rsp.data.Total;
        this.loading=false;
      });
    },
    handleAdd() {
      this.open=true;
      //弹出api数据源选择
      dataList().then(rsp=>{
        this.dataList=rsp.data;
      })

    },
    newPrint(id){
      this.$router.push({
        path: "/report/print/design",
        query: {data: id}
      });
    },
    handleUpdate(item){
         this.$router.push({
                  path: "/report/print/design",
                  query: {id: item.Id}
                });
    },
    handleDelete(item){
      this.$modal
        .confirm('是否确认删除模板"' + item.Name + '"的数据项？')
        .then(function() {
          return deletePrintTemplate(item.Id);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.resetForm("queryForm");
      this.handleQuery();
    },
    submitForm(){
      
    }
  }
};
</script>

<style lang="scss" scoped>
.source-item{
  padding-bottom: 12px;
  .card-item{
    cursor: pointer;
    text-align: center;
  }
}

</style>