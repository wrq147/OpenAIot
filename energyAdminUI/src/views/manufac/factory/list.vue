<template>
  <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form :model="queryParams" class="biaodan" ref="queryForm" :inline="true">
          <el-form-item label="生产商名称" prop="FactoryName">
            <el-input class="set_radius" v-model="queryParams.deptName" placeholder="请输入生产商名称" clearable />
          </el-form-item>
          <el-form-item label="创建日期">
            <el-date-picker class="set_radius" v-model="dateRange" style="width: 250px" value-format="yyyy-MM-dd"
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
              <el-button type="primary" plain @click="handleAdd" v-hasPermi="['/ProducerService/Factory/Add']">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">新增</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table border v-loading="loading" :data="tbList" class="data_table" :header-cell-style="cellSty"
          style="width:100%" row-key="Id">
          <el-table-column prop="Id" label="企业Id" align="center"></el-table-column>
          <el-table-column prop="OrgName" label="生产商名称" align="left" width="260"></el-table-column>
          <el-table-column label="地址" align="center">
            <template slot-scope="scope">
              {{ scope.row.AddressName + scope.row.AddressDetail }}
            </template>
          </el-table-column>
          <el-table-column prop="PHNumPrefix" label="批次前缀" align="center"></el-table-column>
          <el-table-column prop="IndustryName" label="行业类型" align="center"></el-table-column>
          <el-table-column prop="SizeName" label="员工规模" align="center"></el-table-column>
          <el-table-column label="添加时间" align="center">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.createTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="258">
            <template slot-scope="scope">
              <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)"
                v-hasPermi="['/ProducerService/Factory/Remove']">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize"
          @pagination="getList" />
      </div>

      <!-- 添加生产商对话框 -->
      <el-dialog title="添加生产商" :close-on-click-modal="false" :visible.sync="open" width="500px" append-to-body>
        <el-form ref="form" :model="form" :rules="rules" label-width="120px">
          <el-form-item label="生产商名称" prop="Id">
            <el-select style="width:260px;" v-model="form.Id" filterable remote reserve-keyword
              placeholder="请输入要搜索的企业名称" :remote-method="searchOrg" :loading="uidloading">
              <el-option v-for="item in UserOrgList" :key="item.Id" :label="item.OrgName" :value="item.Id">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="批次前缀" prop="PHNumPrefix">
            <el-input style="width:260px" v-model="form.PHNumPrefix" placeholder="请输入自定义的批次前缀"></el-input>
          </el-form-item>
        </el-form>

        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </el-dialog>
    </div>
  </div>
</template>

<script>
import {
  factoryList,
  addFactory,
  delFactory
} from "@/api/manufac/factory";
import {
  searchOrg
} from "@/api/system/Employee";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "FactoryList",
  mixins: [resizeTableCon],
  data() {
    return {
      // 遮罩层
      loading: true,
      uidloading: false,
      // 显示搜索条件
      showSearch: true,
      // 表格树数据
      tbList: [],
      // 是否显示弹出层
      open: false,
      // 日期范围
      dateRange: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        FactoryName: undefined
      },
      // 总条数
      total: 0,
      // 表单参数
      form: {},
      // 表单校验
      rules: {
        region: [
          { required: true, message: '请选择生产商', trigger: 'change' }
        ]
      },
      UserOrgList: [],
    };
  },
  created() {
    this.getList();
  },
  methods: {
    /** 查询列表 */
    getList() {
      this.loading = true;
      factoryList(this.addDateRange(this.queryParams, this.dateRange)).then(response => {
        this.tbList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
    },
    searchOrg(query) {
      if (query !== '') {
        this.uidloading = true;
        searchOrg({ key: query }).then(res => {
          this.UserOrgList = res.data;
          this.uidloading = false;
        });

      } else {
        this.UserOrgList = [];
      }
    },
    // 取消按钮
    cancel() {
      this.open = false;
      this.reset();
    },
    // 表单重置
    reset() {
      this.form = {
        Id: undefined,
        GradeWay: "auto"
      };
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
    /** 新增按钮操作 */
    handleAdd(row) {
      this.reset();
      if (row != undefined) {
        this.form.parentId = row.deptId;
      }
      this.open = true;
    },
    /** 提交按钮 */
    submitForm: function () {
      this.form.GradeWay = "auto"
      addFactory(this.form).then(response => {
        this.$modal.msgSuccess("新增成功");
        this.open = false;
        this.getList();
      });
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      this.$modal
        .confirm('是否确认删除名称为"' + row.OrgName + '"的数据项？')
        .then(function () {
          return delFactory(row.Id);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => { });
    }
  }
};
</script>
