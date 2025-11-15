<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form
          :model="queryParams"
          class="biaodan"
          ref="queryForm"
          :inline="true"
        >
          <el-form-item label="字典名称" prop="dictType">
            <el-select class="set_radius" v-model="queryParams.dictType">
              <el-option
                v-for="item in typeOptions"
                :key="item.dictId"
                :label="item.dictName"
                :value="item.dictType"
              />
            </el-select>
          </el-form-item>
          <el-form-item label="字典标签" prop="dictLabel">
            <el-input
              class="set_radius"
              v-model="queryParams.dictLabel"
              placeholder="请输入字典标签"
              clearable
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>
          <el-form-item label="状态" prop="status">
            <el-select class="set_radius" v-model="queryParams.status" placeholder="数据状态" clearable>
              <el-option
                v-for="dict in dict.type.sys_normal_disable"
                :key="dict.value"
                :label="dict.label"
                :value="dict.value"
              />
            </el-select>
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
                @click="handleAdd"
                v-hasPermi="['/DictService/DictType/Add']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">新增</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button
                type="success"
                plain
                :disabled="single"
                @click="handleUpdate"
                v-hasPermi="['/DictService/DictType/Edit']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-xiugai"></i>
                <span style="margin-left:6px">修改</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button
                type="danger"
                plain
                :disabled="multiple"
                @click="handleDelete"
                v-hasPermi="['/DictService/DictType/Remove']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                <span style="margin-left:6px">删除</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button
                type="warning"
                plain
                :loading="exportLoading"
                @click="handleExport"
                v-hasPermi="['/DictService/DictType/Export']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                <span style="margin-left:6px">导出</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table
          v-loading="loading"
          :data="dataList"
          :row-style="isRed" :cell-style="isRed"
          class="data_table"
          @selection-change="handleSelectionChange"
          :header-cell-style="cellSty"
          style="width:100%"
        >
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column label="字典编码" align="center" prop="dictCode" />
          <el-table-column label="字典标签" align="center" prop="label">
            <template slot-scope="scope">
              <span
                v-if="scope.row.listClass == '' || scope.row.listClass == 'default'"
              >{{ scope.row.label }}</span>
              <el-tag
                v-else
                :type="scope.row.listClass == 'primary' ? '' : scope.row.listClass"
              >{{ scope.row.label }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="字典键值" align="center" prop="value" />
          <el-table-column label="字典排序" align="center" prop="dictSort" />
          <el-table-column label="状态" align="center" prop="status">
            <template slot-scope="scope">
              <dict-tag :options="dict.type.sys_normal_disable" :value="scope.row.status" />
            </template>
          </el-table-column>
          <el-table-column label="备注" align="center" prop="remark" :show-overflow-tooltip="true" />
          <el-table-column label="创建时间" align="center" prop="createTime" width="180">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.createTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column
            label="操作"
            align="center"
            width="248"
            class-name="small-padding fixed-width"
          >
            <template slot-scope="scope">
              <el-button
                type="text"
                icon="el-icon-setting"
                @click="handleDefault(scope.row)"
                v-if="scope.row.is_default == 'N'"
              >默认</el-button>

              <el-button
                type="text"
                icon="el-icon-edit"
                @click="handleUpdate(scope.row)"
                v-hasPermi="['/DictService/DictType/Edit']"
              >修改</el-button>
              <el-button
                type="text"
                icon="el-icon-delete"
                @click="handleDelete(scope.row)"
                v-hasPermi="['/DictService/DictType/Remove']"
              >删除</el-button>
            </template>
          </el-table-column>
        </el-table>

        <pagination
          v-show="total > 0"
          :total="total"
          :page.sync="queryParams.pageNum"
          :limit.sync="queryParams.pageSize"
          @pagination="getList"
        />
      </div>

      <!-- 添加或修改参数配置对话框 -->
      <el-dialog
        :title="title"
        :close-on-click-modal="false"
        :visible.sync="open"
        width="500px"
        append-to-body
      >
        <el-form ref="form" :model="form" :rules="rules" label-width="80px">
          <el-form-item label="字典类型">
            <el-input v-model="form.dictType" :disabled="true" />
          </el-form-item>
          <el-form-item label="数据标签" prop="label">
            <el-input v-model="form.label" placeholder="请输入数据标签" />
          </el-form-item>
          <el-form-item label="数据键值" prop="value">
            <el-input v-model="form.value" placeholder="请输入数据键值" />
          </el-form-item>
          <el-form-item label="样式属性" prop="cssClass">
            <el-input v-model="form.cssClass" placeholder="请输入样式属性" />
          </el-form-item>
          <el-form-item label="显示排序" prop="dictSort">
            <el-input-number v-model="form.dictSort" controls-position="right" :min="0" />
          </el-form-item>
          <el-form-item label="回显样式" prop="listClass">
            <el-select v-model="form.listClass">
              <el-option
                v-for="item in listClassOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              ></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="状态" prop="status">
            <el-radio-group v-model="form.status">
              <el-radio
                v-for="dict in dict.type.sys_normal_disable"
                :key="dict.value"
                :label="dict.value"
              >{{ dict.label }}</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item label="备注" prop="remark">
            <el-input v-model="form.remark" type="textarea" placeholder="请输入内容"></el-input>
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
  listData,
  getData,
  delData,
  addData,
  updateData,
  exportData,
  setDefault
} from "@/api/system/dict/data";
import { listType, getType } from "@/api/system/dict/type";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "Data",
  dicts: ["sys_normal_disable"],
  mixins: [resizeTableCon],
  data() {
    return {
      // 遮罩层
      loading: true,
      // 导出遮罩层
      exportLoading: false,
      // 选中数组
      ids: [],
      // 非单个禁用
      single: true,
      // 非多个禁用
      multiple: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      // 字典表格数据
      dataList: [],
      // 默认字典类型
      defaultDictType: "",
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 数据标签回显样式
      listClassOptions: [
        {
          value: "default",
          label: "默认"
        },
        {
          value: "primary",
          label: "主要"
        },
        {
          value: "success",
          label: "成功"
        },
        {
          value: "info",
          label: "信息"
        },
        {
          value: "warning",
          label: "警告"
        },
        {
          value: "danger",
          label: "危险"
        }
      ],
      // 类型数据字典
      typeOptions: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        dictName: undefined,
        dictType: undefined,
        status: undefined
      },
      // 表单参数
      form: {},
      // 表单校验
      rules: {
        dictLabel: [
          { required: true, message: "数据标签不能为空", trigger: "blur" }
        ],
        dictValue: [
          { required: true, message: "数据键值不能为空", trigger: "blur" }
        ],
        dictSort: [
          { required: true, message: "数据顺序不能为空", trigger: "blur" }
        ]
      }
    };
  },
  created() {
    const dictId = this.$route.params && this.$route.params.dictId;
    this.getType(dictId);
    this.getTypeList();
  },
  methods: {
    /** 查询字典类型详细 */
    getType(dictId) {
      getType(dictId).then(response => {
        this.queryParams.dictType = response.data.dictType;
        this.defaultDictType = response.data.dictType;
        this.getList();
      });
    },
    /** 查询字典类型列表 */
    getTypeList() {
      listType({ showAll: true }).then(response => {
        this.typeOptions = response.data.List;
      });
    },
    /** 查询字典数据列表 */
    getList() {
      this.loading = true;

      listData(this.queryParams).then(response => {
        this.dataList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
    },
    // 取消按钮
    cancel() {
      this.open = false;
      this.reset();
    },
    // 表单重置
    reset() {
      this.form = {
        dictCode: undefined,
        dictLabel: undefined,
        dictValue: undefined,
        cssClass: undefined,
        listClass: "default",
        dictSort: 0,
        status: "0",
        remark: undefined
      };
      this.resetForm("form");
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.resetForm("queryForm");
      this.queryParams.dictType = this.defaultDictType;
      this.handleQuery();
    },
    /** 新增按钮操作 */
    handleAdd() {
      this.reset();
      this.open = true;
      this.title = "添加字典数据";
      this.form.dictType = this.queryParams.dictType;
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.dictCode);
      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      //设置表格中选中行的背景颜色
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.dictCode)) {
        return {
          backgroundColor: "rgba(32, 63, 65, 1)"
        };
      }
    },
    /** 修改按钮操作 */
    handleUpdate(row) {
      this.reset();
      const dictCode = row.dictCode || this.ids;
      getData(dictCode).then(response => {
        this.form = response.data;
        this.open = true;
        this.title = "修改字典数据";
      });
    },
    handleDefault(row) {
      const dictCode = row.dictCode || this.ids;
      setDefault(dictCode).then(response => {
        this.$modal.msgSuccess("修改成功");
        this.getList();
      });
    },
    /** 提交按钮 */
    submitForm: function() {
      this.$refs["form"].validate(valid => {
        if (valid) {
          if (this.form.dictCode != undefined) {
            updateData(this.form).then(response => {
              this.$modal.msgSuccess("修改成功");
              this.open = false;
              this.getList();
            });
          } else {
            addData(this.form).then(response => {
              this.$modal.msgSuccess("新增成功");
              this.open = false;
              this.getList();
            });
          }
        }
      });
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      const dictCodes = row.dictCode || this.ids;
      this.$modal
        .confirm('是否确认删除字典编码为"' + dictCodes + '"的数据项？')
        .then(function() {
          return delData(dictCodes);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    /** 导出按钮操作 */
    handleExport() {
      const queryParams = this.queryParams;
      this.$modal
        .confirm("是否确认导出所有数据项？")
        .then(() => {
          this.exportLoading = true;
          return exportData(queryParams);
        })
        .then(response => {
          this.exportLoading = false;
        })
        .catch(() => {});
    }
  }
};
</script>