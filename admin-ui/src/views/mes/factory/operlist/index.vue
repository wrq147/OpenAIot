<template>
  <div style="padding: 10px 10px 0 10px" id="big_con">
    <div style="background: #ffffff; border-radius: 10px">
      <secondaryGrouping ref="secondaryGrouping" @handleGroupClick="handleGroupClick" table="工序" :filterFiledList="supplierFiledList" @setFilterProp="setFilterProp"/>
      <div>
        <el-row :gutter="20">
          <!--用户数据-->
          <el-col :span="24" :xs="24">
            <div class="from_con" id="from_con">
              <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
                <el-form-item prop="Key" label="关键字">
                  <el-input v-model="queryParams.Key" placeholder="请输入搜索的关键字" clearable/>
                </el-form-item>
                <el-form-item label="创建日期">
                  <el-date-picker class="set_radius" v-model="time" style="width: 232px" value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"/>
                </el-form-item>
                <el-form-item class="submit_button_con">
                  <el-col :span="1.5">
                    <filterPopover :filterFiledList="supplierFiledList" :hasSaveButton="true" @finishSelect="finishSelect" @setSaveFilterList="setSaveFilterList"/>
                  </el-col>
                  <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                  <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                </el-form-item>
              </el-form>
            </div>
            <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
              <el-row :gutter="10" class="mb8 button_row">
                <div>
                  <el-col :span="1.5"><el-button type="primary" icon="el-icon-plus" plain @click="handleAdd('')">新增生产工序</el-button></el-col>
                </div>
              </el-row>
              <el-table v-loading="loading" :data="operList" class="data_table" style="width: 100%">
                <template v-for="ite in activeFiledList">
                  <el-table-column :fixed="ite.isFixed ? 'left' : false" v-if="ite.isShow" :key="ite.field" :label="ite.fieldName" align="center" :prop="ite.field" :show-overflow-tooltip="true">
                    <template slot-scope="scope">
                      <div v-html="ingetFieldShow(scope.row, ite)"></div>
                    </template>
                  </el-table-column>
                </template>
                <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="150">
                  <template slot-scope="scope">
                    <el-button type="text" icon="el-icon-edit" @click="handleAdd(scope.row)">编辑</el-button>
                    <el-button type="text" icon="el-icon-delete" style="color: red" @click="handleDelete(scope.row.Id)">删除</el-button>
                  </template>
                </el-table-column>
              </el-table>
              <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
            </div>
          </el-col>
        </el-row>
      </div>
    </div>
    <!-- 新增/编辑不良品项弹窗 -->
    <add-oper ref="addOper" :title="title" :supplierFiledList="supplierFiledList" :dialog-visible="open" @cancelForm="cancelForm" @getList="getList"/>
  </div>
</template>
  
<script>
import { operList, operRemove } from "@/api/mes/oper";
import { orgFormFields } from "@/api/factory/customFields";
import addOper from "./cmp/addOper.vue";
import secondaryGrouping from "@/views/manufac/factory/component/secondaryGrouping.vue";
import filterPopover from "@/views/manufac/factory/component/filterPopover.vue";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { getFieldShow } from '@/utils/field.js'
export default {
  name: "BatchList",
  mixins: [resizeTableCon],
  components: {
    addOper,
    secondaryGrouping,
    filterPopover,
  },
  data() {
    return {
      loading: false,
      open: false,
      title: "新增生产工序",
      // 查询参数
      queryParams: {
        Key: "",
        pageNum: 1,
        pageSize: 20,
        beginTime: "",
        endTime: "",
        items: [],
      },
      time: [],
      total: 0,
      operList: [],
      supplierFiledList: [], //工序字段
      activeFiledList: [], //字段列表
      activeFilter: [], //手动筛选过滤条件
      groupConditionJson: [], //分组过滤条件
    };
  },
  mounted() {
    this.activeFiledList = [
      {
        field: "Id",
        fieldName: "工序编号",
        type: "文本",
        isShow: true,
        isFixed: false,
      },
      {
        field: "OperName",
        fieldName: "工序名称",
        type: "文本",
        isShow: true,
        isFixed: false,
      },
      {
        field: "PropOf",
        fieldName: "报工数配比",
        type: "文本",
        isShow: true,
        isFixed: false,
      },
      {
        field: "WorkTime",
        fieldName: "预计工时(分钟)",
        type: "文本",
        isShow: true,
        isFixed: false,
      },
      {
        field: "PriceMethod",
        fieldName: "计件、计时",
        type: "文本",
        isShow: true,
        isFixed: false,
      },
      {
        field: "UnitPrice",
        fieldName: "工资单价",
        type: "文本",
        isShow: true,
        isFixed: false,
      },
      {
        field: "updateTime",
        fieldName: "更新时间",
        type: "时间",
        isShow: true,
        isFixed: false,
      },
    ];
    this.getList();
    this.loadOrgFormFields("工序", true);
    this.$nextTick(() => {
      this.$refs.secondaryGrouping.loadGroupViewList();
    });
  },
  methods: {
    ingetFieldShow(obj, field) {
      return getFieldShow(obj, field);
    },
    getList() {
      this.open = false;
      this.loading = true;
      if (this.time.length > 0) {
        this.queryParams.beginTime = this.time[0];
        this.queryParams.endTime = this.time[1];
      }
      operList(this.queryParams).then((response) => {
        this.operList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
    },
    returnObjectName(val) {
      //显示关联对象字段的名称
      if (val && val.indexOf(",") > -1) {
        let arr = val.split(",");
        return arr[1];
      } else {
        return "";
      }
    },
    setFilterProp(propFilter) {
      let itemsArr = JSON.parse(JSON.stringify(this.queryParams.items));
      if (this.beforefilterProp) {
        this.queryParams.items = itemsArr.filter(
          (row) => JSON.stringify(row) != JSON.stringify(this.beforefilterProp)
        );
      }

      if (propFilter) {
        this.beforefilterProp = JSON.parse(JSON.stringify(propFilter));
        if (this.queryParams.items && this.queryParams.items.length > 0) {
        } else {
          this.queryParams.items = [];
        }
        this.queryParams.items.push(propFilter);
      } else {
        this.beforefilterProp = null;
        if (
          !this.queryParams.items &&
          this.queryParams.items &&
          this.queryParams.items.length == 0
        ) {
          delete this.queryParams.items;
        }
      }
      this.queryParams.pageNum = 1;
      this.getList();
    },
    //获取工序字段列表
    async loadOrgFormFields(field, ext) {
      let res = await orgFormFields({ field: field, ext });
      // console.log("字段列表",res);
      this.supplierFiledList = JSON.parse(JSON.stringify(res.data));
    },
    handleGroupClick(activeName, activeFiledList, ConditionJson) {
      if (activeName != "all") {
        if (ConditionJson) {
          this.groupConditionJson = JSON.parse(JSON.stringify(ConditionJson));
        } else {
          this.groupConditionJson = [];
        }
        this.activeFiledList = JSON.parse(JSON.stringify(activeFiledList));
        this.queryParams.items = [
          ...this.activeFilter,
          ...this.groupConditionJson,
        ];
        this.queryParams.pageNum = 1;
        this.getList();
      } else {
        this.activeFiledList = [
          {
            field: "Id",
            fieldName: "工序编号",
            type: "文本",
            isShow: true,
            isFixed: false,
          },
          {
            field: "OperName",
            fieldName: "工序名称",
            type: "文本",
            isShow: true,
            isFixed: false,
          },
          {
            field: "PropOf",
            fieldName: "报工数配比",
            type: "文本",
            isShow: true,
            isFixed: false,
          },
          {
            field: "WorkTime",
            fieldName: "预计工时(分钟)",
            type: "文本",
            isShow: true,
            isFixed: false,
          },
          {
            field: "PriceMethod",
            fieldName: "计件、计时",
            type: "文本",
            isShow: true,
            isFixed: false,
          },
          {
            field: "UnitPrice",
            fieldName: "工资单价",
            type: "文本",
            isShow: true,
            isFixed: false,
          },
          {
            field: "updateTime",
            fieldName: "更新时间",
            type: "时间",
            isShow: true,
            isFixed: false,
          },
        ];
        this.groupConditionJson = [];
        // delete this.queryParams.typeId
        if (this.activeFilter && this.activeFilter.length > 0) {
          this.queryParams.items = this.activeFilter;
        } else {
          delete this.queryParams.items;
        }
        this.getList();
      }
    },
    finishSelect(items) {
      //完成搜索
      this.activeFilter = items ? JSON.parse(JSON.stringify(items)) : [];
      if (items && items.length > 0) {
        this.queryParams.items = [...items, ...this.groupConditionJson];
      } else {
        if (this.groupConditionJson && this.groupConditionJson.length > 0) {
          this.queryParams.items = [...this.groupConditionJson];
        } else {
          delete this.queryParams.items;
        }
      }
      this.queryParams.pageNum = 1;
      this.getList();
    },
    setSaveFilterList(filterList) {
      //筛选另存为新分组
      this.$refs.secondaryGrouping.setSaveFilterList(filterList);
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
      if (data === "") {
        this.title = "新增生产工序";
        this.$refs["addOper"].ruleForm = {
          id: "",
          deviceIds: "",
          operName: "",
          assignedUser: "",
          propOf: "",
          workTime: "",
          priceMethod: "",
          unitPrice: "",
          reportFields: "",
          fieldsInit: "",
          DefectObject:[]
        };
        this.$refs["addOper"].employees = [];
        this.$refs["addOper"].equipment = [];
        this.$refs["addOper"].fieldsInit = [{ sid: "", tid: "" }];
        this.$refs["addOper"].getProductCustomFiled();
      } else {
        this.title = "编辑生产工序";
        this.$refs["addOper"].ruleForm = {
          id: data.Id,
          deviceIds: data.DeviceIds,
          operName: data.OperName,
          assignedUser: data.AssignedUser,
          propOf: data.PropOf,
          workTime: data.WorkTime,
          priceMethod: data.PriceMethod,
          unitPrice: data.UnitPrice,
          reportFields: data.ReportFields,
          fieldsInit: data.FieldsInit,
          DefectObject:data.DefectJson==''?[]:JSON.parse(data.DefectJson)
        };

        this.$refs["addOper"].employees =
          data.AssignedUser === "" ? [] : JSON.parse(data.AssignedUser);
        this.$refs["addOper"].loadDeviceList(data.DeviceIds);
        this.$refs["addOper"].fieldsInit =
          data.FieldsInit === ""
            ? [{ sid: "", tid: "" }]
            : JSON.parse(data.FieldsInit);
        this.$refs["addOper"].getProductCustomFiled(data);
      }
      this.open = true;
    },
    /** 删除按钮操作 */
    handleDelete(id) {
      this.$confirm("此操作将永久删除该数据, 是否继续?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(() => {
          operRemove({ id: id }).then((res) => {
            this.$message.success("删除成功!");
            this.getList();
          });
        })
        .catch(() => {});
    },
    cancelForm() {
      this.open = false;
    },
  },
};
</script>