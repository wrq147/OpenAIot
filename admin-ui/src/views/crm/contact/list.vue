<template>
  <div style="padding: 20px 20px 0 20px; height: 100%" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="创建时间">
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
                  <el-button type="primary" plain @click="handleAdd" v-hasPermi="['/CRMService/Clue/Add']">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left: 6px">添加</span>
                  </el-button>
                </el-col>
              </div>
              <right-toolbar :showSearch.sync="showSearch" @queryTable="getContactList" :columns="columns"></right-toolbar>
            </el-row>

            <el-table v-loading="loading" :data="priList" :row-style="isRed" @selection-change="handleSelectionChange"
              class="data_table" :header-cell-style="cellSty" style="width: 100%">
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column label="企业ID" align="center" key="OrgId" prop="OrgId" v-if="columns[0].visible" width="160"/>
              <el-table-column label="客户名称" align="center" key="CustomerId" prop="CustomerName" v-if="columns[1].visible" :show-overflow-tooltip="true"/>
              <el-table-column label="联系人" align="center" key="RealName" prop="RealName" v-if="columns[2].visible" :show-overflow-tooltip="true"/>
              <el-table-column label="手机号" align="center" key="Mobile" prop="Mobile" v-if="columns[3].visible" :show-overflow-tooltip="true" width="150"/>
              <el-table-column label="邮箱" align="center" key="Email" prop="Email" v-if="columns[4].visible" :show-overflow-tooltip="true" width="160"/>
              <el-table-column label="微信号" align="center" key="WxNumber" prop="WxNumber" v-if="columns[5].visible" :show-overflow-tooltip="true"/>
              <el-table-column label="部门" align="center" key="DeptName" prop="DeptName" v-if="columns[6].visible"/>
              <el-table-column label="职务" align="center" key="PostName" prop="PostName" v-if="columns[7].visible"/>
              <el-table-column label="责任人" align="center" key="LeaderName" prop="LeaderName" v-if="columns[8].visible" :show-overflow-tooltip="true"/>
              <el-table-column label="协作人" align="center" key="HelperName" prop="HelperName" v-if="columns[9].visible" :show-overflow-tooltip="true"/>
              <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns[10].visible" width="240">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="200" fixed="right">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)" v-hasPermi="['/CRMService/Contact/Edit']">修改</el-button>
                  <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)" v-hasPermi="['/CRMService/Contact/Remove']">删除</el-button>
                </template>
              </el-table-column>
            </el-table>

            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getContactList"/>
          </div>
        </el-col>
      </el-row>
      <el-dialog :title="diaTitle" :close-on-click-modal="false" :visible.sync="open"
        width="900px" append-to-body class="add_dialog_border">
        <el-form class="add_clue_form" ref="contactforms" :model="contactform" :rules="rules" label-width="100px" label-position="left">
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="姓名" required prop="realName">
                <el-input class="form_input_style" v-model="contactform.realName" placeholder="请输入姓名"
                  clearable size="small" style="width: 100%"/>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="手机号" required prop="mobile">
                <el-input class="form_input_style" v-model="contactform.mobile" placeholder="请输入手机号"
                  clearable size="small" maxlength="11" style="width: 100%"/>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="性别" prop="sex">
                <el-radio-group v-model="contactform.sex">
                  <el-radio label="0">男</el-radio>
                  <el-radio label="1">女</el-radio>
                  <el-radio label="2">未知</el-radio>
                </el-radio-group>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="客户名称" required prop="customerId">
                <el-input class="form_input_style" v-model="contactform.customerName" placeholder="请选择客户" clearable
                  style="width:100%" @focus="openCustomDialog" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="部门" prop="deptName">
                <el-input class="form_input_style" v-model="contactform.deptName" placeholder="请输入部门"
                  clearable size="small" style="width: 100%"/>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="职务" prop="postName">
                <el-input class="form_input_style" v-model="contactform.postName" placeholder="请输入职务"
                  clearable size="small" style="width: 100%"/>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="微信号" prop="wxNumber">
                <el-input class="form_input_style" v-model="contactform.wxNumber"  placeholder="请输入微信号"
                  clearable size="small" style="width: 100%"/>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="邮箱" prop="email">
                <el-input class="form_input_style" v-model="contactform.email" placeholder="请输入邮箱"
                  clearable size="small" style="width: 100%"/>
              </el-form-item>
            </el-col>
            
            <el-col :span="12">
              <el-form-item label="负责人" prop="leaderId">
                <el-select class="form_input_style" filterable allow-create default-first-option
                  v-model="contactform.leaderName" ref="selectLeader" placeholder="请选择负责人"
                  @focus="getLeaderFocus" style="width: 100%" :disabled="contactform.id ? true : false"></el-select>
                <org-picker :multiple="false" ref="leaderPicker" :selected="contactform.leaderInfo" @ok="selectLeadered"/>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="协作人" prop="helperName">
                <el-select class="form_input_style" multiple v-model="contactform.helperName"
                  ref="selectUsers"  placeholder="请选择协作人" @focus="getUsersFocus" style="width: 100%"></el-select>
                <org-picker :multiple="true" ref="userPicker" :selected="contactform.userInfo" @ok="selectUsersed"/>
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="联系人详情" prop="remark">
                <div class="label_text">兴趣爱好 决策偏好等</div>
                <el-input type="textarea" v-model="contactform.remark" placeholder="请输入联系人详情"  :autosize="{ minRows: 2, maxRows: 4 }"></el-input>
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
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  contactList,
  addContact,
  contactInfo,
  editContact,
  delContact,
  clueReturn,
} from "@/api/crm/contact";
import { privateCustomer } from "@/api/crm/customer";
import OrgPicker from "@/views/flowable/common/OrgPicker";
import { listMember } from "@/api/system/Employee";
import CustomChoice from "@/views/crm/compontent/custom-choice.vue"
export default {
  name: "contact",
  components: { OrgPicker,CustomChoice },
  mixins: [resizeTableCon],
  data() {
    return {
      customerList: [], //客户列表
      customerMap: new Map(), //客户列表Map
      diaTitle: "添加联系人",
      open: false, //弹出层
      //表格样式设计
      // 选中数组
      ids: [],
      // 非单个禁用
      single: true,
      // 非多个禁用
      multiple: true,
      //表格样式设计
      // 日期范围
      dateRange: [],
      priList: [], //联系人列表
      // 显示搜索条件
      showSearch: true,
      // 列信息
      columns: [
        { key: 0, label: `企业ID`, visible: true },
        { key: 1, label: `客户名称`, visible: true },
        { key: 2, label: `联系人`, visible: true },
        { key: 3, label: `手机号`, visible: true },
        { key: 4, label: `邮箱`, visible: true },
        { key: 5, label: `微信号`, visible: true },
        { key: 6, label: `部门`, visible: true },
        { key: 7, label: `职务`, visible: true },
        { key: 8, label: `责任人`, visible: true },
        { key: 9, label: `协作人`, visible: true },
        { key: 10, label: `创建时间`, visible: true },
      ],
      // 遮罩层
      loading: true,
      // 导出遮罩层
      exportLoading: false,
      // 列表总条数
      total: 0,
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
      },
      rules: {
        realName: [
          {
            required: true,
            message: "联系人不能为空",
            trigger: ["blur", "change"],
          },
        ],
        mobile: [
          {
            required: true,
            message: "手机号码不能为空",
            trigger: ["blur", "change"],
          },
          {
            pattern: /^1[3|4|5|6|7|8|9][0-9]\d{8}$/,
            message: "请输入正确的手机号码",
            trigger: "blur",
          },
        ],
        customerId: [
          {
            required: true,
            message: "客户名称不能为空",
            trigger: ["blur", "change"],
          },
        ],
        leaderId: [
          {
            required: true,
            message: "请选择负责人",
            trigger: ["blur"],
          },
        ],
        sex: [
          {
            required: true,
            message: "请选择性别",
            trigger: ["blur", "change"],
          },
        ],
        email: [
          {
            type: "email",
            message: "请输入正确的邮箱",
            trigger: ["blur", "change"],
          },
        ],
      },
      contactform: {
        realName: "", //联系人
        mobile: "", //手机号
        customerId: "", //客户id
        customerName: "", //客户名称
        postName: "", //职位
        deptName: "", //部门
        wxNumber: "", //微信号
        email: "", //邮箱
        sex: "", //性别
        userInfo: [], //已经选择的协作人员工
        helperName: [], //协作人员名称
        helper: "",
        remark: "", //联系人详情
        leaderId: null, //负责人
        leaderInfo: [], //已经选择的负责人员工
        leaderName: [], //负责人员名称
      },
      fromMap: new Map(), //联系人来源map
      employeeMap: new Map(), //员工map
    };
  },

  mounted() {
    this.getContactList();
  },

  methods: {
    openCustomDialog() {
      //打开客户列表选择弹窗
      this.$refs.customOptions.openCustomDialog()
    },
    handleCustomChange(val) {
      //完成客户选择
      // console.log(val, 'val');
      if (val != null) {
        this.contactform.customerId = val.Id
        this.contactform.customerName = val.CustomerName
      }
    },
    async getCustomerList() {
      //获取私海客户列表
      // this.queryParams.orgId = this.$store.getters.orgId;
      let response = await privateCustomer({ showAll: true });
      if (response.data && response.data.List) {
        this.customerList = response.data.List;
        response.data.List.map((row) => {
          this.customerMap.set(row.Id, row);
        });
      }
    },

    handleUpdate(row) {
      //修改联系人信息
      if (row.Id) {
        contactInfo({ id: row.Id }).then((res) => {
          console.log(res,'res');
          if (res.data) {
            let data = res.data;
            let useList = [];
            let leaderList = [];
            let leaderName = "";
            data.HelperName=[]
            if (data.Helper) {
              useList=data.HelperUsers.map(row=>{
                let obj={
                  id:row.Id,
                  name:row.RealName,
                  avatar:row.Avatar,
                  type: "user",
                }
                return obj
              })
              data.HelperName=data.HelperUsers.map(row=>row.RealName)
              let noticeUsers=data.HelperUsers.map(row=>row.Id)
              data.Helper=noticeUsers.join(',')
            }
            if (data.LeaderId) {
              //获取责任人相关信息
              leaderName = data.LeaderUser.RealName;
              leaderList=[{
                id:data.LeaderUser.Id,
                name:data.LeaderUser.RealName,
                avatar:data.LeaderUser.Avatar,
                type: "user",
              }]
            }
            this.contactform = {
              realName: data.RealName, //联系人
              mobile: data.Mobile, //手机号
              customerId: data.CustomerId, //客户id
              customerName: data.CustomerName, //客户名称
              postName: data.PostName, //职位
              deptName: data.DeptName, //部门
              wxNumber: data.WxNumber, //微信号
              email: data.Email, //邮箱
              sex: data.Sex, //性别
              userInfo: useList, //已经选择的协作人员工
              helperName: data.HelperName, //协作人员名称
              helper: data.Helper,
              remark: data.Remark, //联系人详情
              leaderId: data.LeaderId, //负责人
              leaderInfo: leaderList, //已经选择的负责人员工
              leaderName: leaderName, //负责人员名称
              id: data.Id,
            };
            this.open = true;
            this.diaTitle = "编辑联系人";
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
    handleDelete(row) {
      //删除联系人
      this.$modal
        .confirm('是否确认删除名称为"' + row.RealName + '"的联系人？')
        .then(() => {
          this.loading = true;
          return delContact({ id: row.Id });
        })
        .then(() => {
          this.$modal.msgSuccess("删除成功");
          this.getContactList();
          this.loading = false;
        })
        .catch(() => {
          this.loading = false;
        });
    },
    async getMemberList() {
      try {
        let rsp = await listMember({ deptIdWithChildren: true, showAll: true,isPrimaryDept:true });
        if (rsp.data && rsp.data.List) {
          rsp.data.List.map((row) => {
            this.employeeMap.set(row.Id, row);
          });
        }
      } catch (err) {
        this.$message.error(err.data);
      }
    },
    submitForm() {
      //提交数据
      this.$refs.contactforms.validate((valid) => {
        if (valid) {
          if (this.contactform.leaderId) {
            let submitForm = {};
            submitForm = JSON.parse(JSON.stringify(this.contactform));
            if (submitForm.helperName) {
              submitForm.helperName = submitForm.helperName.join(",");
            }
            if (!submitForm.helper) {
              submitForm.helper = "";
            }
            delete submitForm.userInfo;
            delete submitForm.leaderName;
            delete submitForm.leaderInfo;
            this.loading = true;
            if (submitForm.id) {
              editContact(submitForm)
                .then((res) => {
                  if (res.code == 0) {
                    this.loading = false;
                    this.$modal.msgSuccess("修改成功");
                    this.open = false;
                    this.getContactList();
                  }
                })
                .catch((err) => {
                  console.log("err", err);
                  this.loading = false;
                });
            } else {
              addContact(submitForm)
                .then((res) => {
                  if (res.code == 0) {
                    this.loading = false;
                    this.$modal.msgSuccess("添加成功");
                    this.open = false;
                    this.getContactList();
                  }
                })
                .catch((err) => {
                  this.loading = false;
                });
            }
          } else {
            this.$modal.error("添加成功");
          }
        }
      });
    },
    getLeaderFocus() {
      //获取负责人选择下拉列表的焦点
      this.$refs.selectLeader.blur();
      this.$refs.leaderPicker.show(this.contactform.leaderInfo, "user");
    },
    getUsersFocus() {
      //获取协作人选择下拉列表的焦点
      this.$refs.selectUsers.blur();
      this.$refs.userPicker.show(this.contactform.userInfo, "user");
    },
    selectUsersed(values) {
      //选择协作人
      this.contactform.userInfo = values;
      if (values.length > 0) {
        let li = [];
        let li2 = [];
        let li3 = [];
        values.map((it, ix) => {
          li.push(parseInt(it.id));
          li2.push(it.name);
          li3.push(it.avatar);
          console.log("li", li);
        });
        this.contactform.helper = li.join(",");
        this.contactform.helperName = li2;
      } else {
        this.contactform.helper = undefined;
        this.contactform.helperName = undefined;
      }

      //清除el-select多选时的清除按钮
      this.$nextTick(() => {
        const closeIcons = this.$refs.selectUsers.$el.querySelector(".el-select__tags").querySelectorAll(".el-icon-close");
        const arr = Array.from(closeIcons);
        arr.forEach((item) => {
          item.style.display = "none";
        });
      });
      this.$forceUpdate();
    },
    selectLeadered(values) {
      //选择负责人
      this.contactform.leaderInfo = values;
      if (values.length > 0) {
        this.contactform.leaderId = values[0].id;
        this.contactform.leaderName = values[0].name;
      } else {
        this.contactform.leaderId = 0;
        this.contactform.leaderName = undefined;
      }
      this.$forceUpdate();
    },
    // 取消按钮
    cancel() {
      this.open = false;
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getContactList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map((item) => item.Id);
      // console.log("选中的",this.ids);

      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      //选中行的样式设置
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "#F6F9FF",
        };
      }
    },
    handleAdd() {
      //打开添加联系人
      this.diaTitle = "添加联系人";
      this.contactform = {
        realName: "", //联系人
        mobile: "", //手机号
        customerId: "", //客户id
        customerName: "", //客户名称
        postName: "", //职位
        deptName: "", //部门
        wxNumber: "", //微信号
        email: "", //邮箱
        sex: "", //性别
        userInfo: [], //已经选择的协作人员工
        helperName: [], //协作人员名称
        helper: "",
        remark: "", //联系人详情
        leaderInfo: [
          {
            id: parseInt(this.$store.state.user.uid),
            name: this.$store.state.user.name,
            avatar: this.$store.state.user.avatar,
            type:'user'
          },
        ], //已经选择的负责人
        leaderName: this.$store.state.user.name, //负责人名称默认设置为本人
        leaderId: parseInt(this.$store.state.user.uid), //负责人id
      };
      this.resetForm("contactforms");
      this.open = true;
    },
    getContactList() {
      //获取公海联系人列表
      this.loading = true;
      // this.queryParams.orgId = this.$store.getters.orgId;
      contactList(this.addDateRange(this.queryParams, this.dateRange)).then(
        (response) => {
          console.log("联系人",response);
          if (response.data && response.data.List) {
            response.data.List.map((row) => {
              if (row.Helper) {
                
                row.HelperName = (row.HelperUsers.map(row=>row.RealName)).join(',')
              }
              row.LeaderName = "";
              if (row.LeaderId) {
                //负责人姓名获取
                row.LeaderName = row.LeaderUser.RealName
              }
            });
            this.priList = response.data.List;
          }

          // for(let i=0;i<3;i++){
          //   this.agentList=[...this.agentList,...this.agentList]
          // }
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
  },
};
</script>

<style lang="scss" scoped>
.label_text {
  font-size: 12px;
  color: #999;
  line-height: 12px;
  margin-bottom: 5px;
}
.add_clue_form{
  ::v-deep .el-form-item{
    margin-bottom: 20px;
  }
  ::v-deep .el-form-item__label{
    font-size: 16px;
    text-align: right;
  }
  ::v-deep .el-radio{
    .el-radio__label{
      font-size: 16px;
    }
  }
  ::v-deep .el-input {
    height: 48px;
    input {
      height: 48px;
      color: #333333;
      font-size: 16px;
    }
  }
}
</style>