<template>
  <div style="padding: 20px 20px 0 20px; height: 100%" id="big_con">
    <div class="from_con" id="from_con" v-if="showSearch">
      <el-form
        class="biaodan"
        :model="queryParams"
        ref="queryForm"
        :inline="true"
      >
        <el-form-item label="计划日期">
          <el-date-picker
            class="form_input_style"
            v-model="dateRange"
            style="width: 232px"
            value-format="yyyy-MM-dd"
            type="daterange"
            range-separator="-"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
          ></el-date-picker>
        </el-form-item>
        <el-form-item label="关键字" prop="Key">
          <el-input class="set_radius" v-model="queryParams.Key" placeholder="请输入关键字" clearable/>
        </el-form-item>
        <!-- <el-col class="float_right" :span="24"> -->
        <el-form-item class="submit_button_con">
          <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
          <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
        </el-form-item>
        <!-- </el-col> -->
      </el-form>
    </div>
    <!--表格-->
    <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
      <el-row :gutter="10" class="mb8 button_row">
        <div>
          <el-col :span="1.5">
            <el-button type="primary" plain @click="handleAdd" v-hasPermi="['/CRMService/Plan/Add']">
              <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
              <span style="margin-left: 6px">添加</span>
            </el-button>
          </el-col>
        </div>
        <right-toolbar :showSearch.sync="showSearch" @queryTable="list"></right-toolbar>
      </el-row>
      <el-table
        ref="multipleTable"
        :data="tableData"
        tooltip-effect="dark"
        style="width: 100%"
        class="data_table"
        :header-cell-style="cellSty"
        @selection-change="handleSelectionChange"
      >
        <el-table-column label="跟进客户" align="center" width="220" prop="CustomerName"></el-table-column>
        <el-table-column prop="ExecutorName" align="center" label="计划执行人" width="220"></el-table-column>
        <el-table-column prop="PlanTime" align="center" label="计划时间" show-overflow-tooltip width="260"></el-table-column>
        <el-table-column prop="Status" align="center" label="状态" show-overflow-tooltip width="120">
          <template slot-scope="scope">
            <div>
              {{scope.row.Status=='A'?'待完成':'已完成'}}
            </div>
          </template>
        </el-table-column>
        <!-- <el-table-column prop="Remark" label="计划内容" show-overflow-tooltip>
          <template></template>
        </el-table-column> -->
        <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
          <template slot-scope="scope">
            <el-button type="text" v-hasPermi="['/CRMService/Plan/Remove']" @click="handClickDelete(scope.row.Id)">删除</el-button>
            <el-button type="text" v-hasPermi="['/CRMService/Plan/Finish']" @click="handClickCompleteFollow(scope.row.Id)">完成</el-button>
            <el-button type="text" v-hasPermi="['/CRMService/Plan/Edit']" @click="handEditTable(scope)">编辑</el-button>
          </template>
        </el-table-column>
      </el-table>
      <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="list" />
    </div>
    <!--添加操作-->
    <el-dialog :title="diaTitle" :close-on-click-modal="false" :visible.sync="open" width="700px" append-to-body class="add_dialog_border">
      <el-form class="add_clue_form" ref="planeform" :model="planeform" :rules="rules" label-width="100px" label-position="left">
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="跟进客户" prop="customerId">
              <el-select style="width:100%" filterable allow-create default-first-option v-model="planeform.customerName" @focus="openCustomDialog" placeholder="请选择跟进客户" ref="selectPlaneCustom">
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item label="计划执行人" prop="executor">
                <el-select multiple v-model="planeform.executorName"
                  ref="selectUsers"  placeholder="请选择计划执行人" @focus="getUsersFocus" style="width: 100%"></el-select>
                <org-picker :multiple="true" ref="userPicker" :selected="planeform.userInfo" @ok="selectUsersed"/>
              </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item label="计划时间" prop="planTime">
              <el-date-picker style="width:100%" v-model="planeform.planTime" type="datetime" value-format="yyyy-MM-dd HH:mm:ss" placeholder="选择日期时间">
              </el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item label="计划内容" prop="remark">
              <el-input type="textarea" v-model="planeform.remark" placeholder="请输入计划内容" :autosize="{ minRows: 5, maxRows: 5 }"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitForm">确 定</el-button>
        <el-button @click="cancel">取 消</el-button>
      </div>
    </el-dialog>
    <custom-choice ref="customOptions" @handleCustomChange="handleCustomChange"></custom-choice>
  </div>
</template>
<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  followPlanList,
  AddFlowPlan,
  DeletePlan,
  CompleteFollow,
  ObtainDetails,
  ObtainEdit,
} from "@/api/crm/plan";
import CustomChoice from "@/views/crm/compontent/custom-choice.vue"
import OrgPicker from "@/views/flowable/common/OrgPicker";
export default {
  name: "",
  components: {
    OrgPicker,
    CustomChoice
  },
  mixins: [resizeTableCon],
  props: {},
  data() {
    return {
      showSearch:true,
      rules: {
        planTime: [
          {
            required: true,
            message: "计划时间不能为空",
            trigger: ["blur", "change"],
          },
        ],
        customerId: [
          {
            required: true,
            message: "跟进客户不能为空",
            trigger: ["blur", "change"],
          },
        ],
        executor: [
          {
            required: true,
            message: "请选择计划执行人",
            trigger: ["blur"],
          },
        ],
        remark: [
          {
            required: true,
            message: "计划内容不能为空",
            trigger: ["blur", "change"],
          },
        ],
      },

      planeform: {
        customerId: '',
        customerName:'',
        executor: null,
        executorName:null,
        planTime: '',
        remark: '',
      },
      dateRange:[],
      open: false,
      diaTitle: "跟进计划",
      tableData: [],
      multipleSelection: [],
      multiple: true,
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        Key: undefined,
      },
      total:0
    };
  },
  computed: {},
  watch: {},
  created() {
    this.list(); //跟进计划列表
  },
  methods: {
    selectUsersed(values) {
      //选择协作人
      // console.log("选中的协作人111111111111", values);
      this.$refs.userPicker.show(this.planeform.userInfo, "user");
      this.planeform.userInfo = values;
      let li = [];
      let li2 = [];
      values.map((it, ix) => {
        li.push(parseInt(it.id));
        li2.push(it.name);
      });
      this.planeform.executor = li.join(",");
      this.planeform.executorName = li2;
      this.$nextTick(() => {
        const closeIcons = this.$refs.selectUsers.$el.querySelector(".el-select__tags").querySelectorAll(".el-icon-close");
        const arr = Array.from(closeIcons);
        arr.forEach((item) => {
          item.style.display = "none";
        });
      });
      this.$forceUpdate();
      //选择负责人
    },
    getUsersFocus() {
      //获取用户下拉列表的焦点
      // this.$refs.selectUsers.blur();
      // this.$refs.userPicker.show(this.planeform.userInfo, "user");
      //获取负责人选择下拉列表的焦点
      // console.log(this.planeform.userInfo,'iiiiiii');
      this.$refs.selectUsers.blur();

      this.$refs.userPicker.show(this.planeform.userInfo, "user");
      
    },
    handleCustomChange(val) {
      //完成客户选择
      // console.log(val, 'val');
      if (val != null) {
        this.planeform.customerId = val.Id
        this.planeform.customerName = val.CustomerName
      }
    },
    openCustomDialog() {
      //打开客户列表选择弹窗
      this.$refs.selectPlaneCustom.blur();
      this.$refs.customOptions.openCustomDialog()
    },
    submitForm() {
      //提交数据
      this.$refs.planeform.validate((valid) => {
        if(valid){
          let submitForm = {};
          submitForm = JSON.parse(JSON.stringify(this.planeform));
          delete submitForm.userInfo;
          let data = {
            customerId: submitForm.customerId,
            executor: submitForm.executor,
            planTime: submitForm.planTime,
            remark: submitForm.remark,
          };
          // console.log(valid,data);
          //   return
          this.loading = true;
          if (this.diaTitle == "添加跟进记录") {
            AddFlowPlan(data)
              .then((res) => {
                // console.log(res, "添加跟进计划成功");
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("添加成功");
                  this.open = false;
                  this.list();
                }
              })
              .catch((err) => {
                console.log("err", err);
                this.loading = false;
              });
          } else {
            data.id = this.planeform.editId;
            // console.log(data,'this.form.companyName')
            // return;
            ObtainEdit(data)
              .then((res) => {
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("编辑成功");
                  this.open = false;
                  this.list();
                }
              })
              .catch((err) => {
                console.log("err", err);
                this.loading = false;
              });
          }
        }
      });
    },
    cancel() {
      this.open = false;
    },

    handleAdd() {
      //打开添加跟进计划
      this.diaTitle = "添加跟进记录";
      this.planeform = {
        customerId: '',
        customerName:'',
        executor: null,
        executorName:null,
        planTime: '',
        remark: '',
        userInfo:[]
      };
      this.resetForm("planeform");
      this.open = true;
    },
    list() {
      // this.queryParams
      if(this.dateRange&&this.dateRange.length>0){
        this.queryParams.PlanStartTime=this.dateRange[0]
        this.queryParams.PlanEndTime=this.dateRange[1]
      }else{
        delete this.queryParams.PlanStartTime
        delete this.queryParams.PlanEndTime
      }
      followPlanList(this.queryParams).then((res) => {
        // console.log(res.data.List, "this.tableData");
        res.data.List.map((row) => {
           row.ExecutorName=(row.ExecutorUsers.map(rs=>rs.RealName)).join(',')
        });
        this.total=res.data.Total
        this.tableData = res.data.List;
      });
    },
    toggleSelection(rows) {
      if (rows) {
        rows.forEach((row) => {
          this.$refs.multipleTable.toggleRowSelection(row);
        });
      } else {
        this.$refs.multipleTable.clearSelection();
      }
    },
    //完成
    handClickCompleteFollow(id) {
      CompleteFollow({
        id: id,
      }).then((res) => {
        if (res.code == 0) {
          this.$modal.msgSuccess("计划完成");
          this.list();
        }
      });
    },
    //编辑
    handEditTable(scoped, title) {
      this.diaTitle = "编辑跟进记录";
      this.open = true;
      this.resetForm("planeform");
      if (scoped) {
        ObtainDetails({
          id: scoped.row.Id,
        }).then((res) => {
          if (res.code == 0) {
            // console.log(res, "跟进计划详情")
            let data=res.data
            //console.log(scoped.row.CustomerName)
            this.planeform.executorName =data.ExecutorUsers.map(row=>row.RealName)
            this.planeform.customerName =data.CustomerName
            this.planeform.customerId = data.CustomerId;
            this.planeform.executor = data.Executor;
            this.planeform.planTime = data.PlanTime;
            this.planeform.remark = data.Remark;
            this.planeform.editId = data.Id;
            //this.planeform.userInfo=[res.data.Executor]
            var userList = [];
            if(data.Executor&&data.ExecutorUsers){
              userList=data.ExecutorUsers.map(row=>{
                let obj={
                  id:row.Id,
                  name:row.RealName,
                  avatar:row.Avatar,
                  type:'user'
                }
                return obj
              })
            }
            this.planeform.userInfo = userList;
            this.$nextTick(() => {
              const closeIcons = this.$refs.selectUsers.$el.querySelector(".el-select__tags").querySelectorAll(".el-icon-close");
              const arr = Array.from(closeIcons);
              arr.forEach((item) => {
                item.style.display = "none";
              });
            });
          }
          
        });
      }
    },
    //删除
    handClickDelete(id) {
      DeletePlan({
        id: id,
      }).then((res) => {
        if (res.code == 0) {
          this.$modal.msgSuccess("删除成功");
          this.list();
        }
      });
    },
    handleSelectionChange(val) {
      this.multipleSelection = val;
      console.log(this.multipleSelection, "this.multipleSelection");
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.list();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
  },
};
</script>
<style lang="scss" scoped>
.add_clue_form {
  ::v-deep .el-form-item {
    margin-bottom: 18px;
  }
  ::v-deep .el-form-item__label {
    padding-bottom: 0;
    text-align: right;
  }
  ::v-deep .form_input_style.el-input {
    height: 48px;
    line-height: 48px;
    input {
      height: 48px;
      line-height: 48px;
    }
  }
  .form_input_style {
    ::v-deep .el-input {
      height: 48px;
      line-height: 48px;
      input {
        height: 48px;
        line-height: 48px;
      }
    }
  }
}
</style>
