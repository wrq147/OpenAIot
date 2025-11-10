<template>
  <div>
    <el-dialog :title="diaTitle" :close-on-click-modal="false" :visible.sync="open" width="800px" append-to-body class="add_dialog_border"
     @close="open = false" :destroy-on-close="true">
      <el-form class="add_clue_form" ref="form" :model="form" :rules="rules" label-width="80px" label-position="left">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="联系人" required prop="realName">
              <el-input class="form_input_style" v-model="form.realName" placeholder="请输入联系人" clearable size="small" style="width: 100%"/>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="客户名称" required prop="companyName">
              <el-input class="form_input_style" v-model="form.companyName" placeholder="请输入客户名称" clearable size="small" style="width: 100%"/>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="手机号" required prop="mobile">
              <el-input class="form_input_style" v-model="form.mobile" placeholder="请输入手机号" clearable maxlength="11" size="small" style="width: 100%"/>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="部门" prop="deptName">
              <el-input class="form_input_style" v-model="form.deptName" placeholder="请输入部门" clearable size="small" style="width: 100%"/>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="职务" prop="postName">
              <el-input class="form_input_style" v-model="form.postName" placeholder="请输入职务" clearable size="small" style="width: 100%"/>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="线索来源" prop="fromType">
              <el-select class="form_input_style" v-model="form.fromType" ref="selectUsers" placeholder="请选择线索来源" style="width: 100%">
                <el-option v-for="item in fromList" :key="item.value" :label="item.label" :value="item.value"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="协作人" prop="helperName">
              <el-select class="form_input_style" multiple v-model="form.helperName" ref="selectUsers" placeholder="请选择协作人" @focus="getUsersFocus" style="width: 100%"></el-select>
              <org-picker :multiple="true" ref="userPicker" :selected="form.userInfo" @ok="selectUsersed"/>
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item label="线索详情" prop="remark">
              <el-input type="textarea" v-model="form.remark" placeholder="请输入线索详情" :autosize="{ minRows: 2, maxRows: 4 }"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitForm">确 定</el-button>
        <el-button @click="cancel">取 消</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import {
  priAddClue,
  clueInfo,
  priEditClue,
  priTransformClue,
} from "@/api/crm/clue";
import OrgPicker from "@/views/flowable/common/OrgPicker";
export default {
  name: "AdminUiClueAdd",
  components: { OrgPicker },
  data() {
    return {
      open: false,
      diaTitle: "添加线索",
      fromList: [
        {
          value: "weixin",
          label: "微信线索",
        },
        {
          value: "form",
          label: "流程表单",
        },
        {
          value: "other",
          label: "其他",
        },
      ],
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
        companyName: [
          {
            required: true,
            message: "客户名称不能为空",
            trigger: ["blur", "change"],
          },
        ],
      },
      form: {
        realName: "", //联系人
        fromId: "", //表单id
        changeId: "", //转移的客户id
        mobile: "", //手机号
        companyName: "", //客户名称
        fromType: "", //线索来源
        postName: "", //职位
        deptName: "", //部门
        userInfo: [], //已经选择的协作人员工
        helperName: [], //
        helperAvatar: [],
        helper: "",
        remark: "", //线索详情
      },
      fromMap: new Map(), //线索来源map
      employeeMap: new Map(), //员工map
      open: false, //弹出层
    };
  },

  mounted() {},

  methods: {
    async getGenerateNumber() {
      //生成线索唯一编号
      try {
        let res = await GenerateNumber();
        // console.log("生成客户编码", res);

        this.customform.customerNumber = res.data;
        console.log("表单信息", this.customform);
      } catch (error) {
        console.log("生成编码报错", err);
      }
    },
    handleAdd() {
      //打开添加线索
      this.diaTitle = "添加线索";
      this.form = {
        realName: "",
        fromId: "", //表单id
        changeId: "", //转移的客户id
        mobile: "",
        companyName: "",
        fromType: "",
        postName: "",
        deptName: "",
        userInfo: [],
        helperName: [],
        helperAvatar: [],
        helper: "",
        leaderId: 0, //线索跟进人（为0则为公海线索）
      };
      this.resetForm("form");
      this.open = true;
    },
    handleUpdate(row) {
      //修改线索信息
      if (row.Id) {
        clueInfo({ id: row.Id }).then((res) => {
          if (res.data) {
            let data = res.data;
            let useList = [];
            let helperAvatar = [];
            if (data.Helper) {
              data.HelperUsers.map(it => {
                useList.push({ id: it.Id, name: it.UserName, avatar: it.Avatar, type: "user" });
                if(it.Avatar){
                  helperAvatar.push(it.Avatar)
                }else{
                  helperAvatar.push('')
                }
              });
              let noticeUsers=data.HelperUsers.map(row=>row.Id)
              data.Helper=noticeUsers.join(',')
            }
            this.form = {
              realName: data.RealName,
              fromId: data.FromId, //表单id
              changeId: data.ChangeId, //转移的客户id
              mobile: data.Mobile,
              companyName: data.CompanyName,
              fromType: data.FromType,
              postName: data.PostName,
              deptName: data.DeptName,
              userInfo: useList,
              helperName: data.HelperName ? data.HelperName.split(",") : [],
              helperAvatar: helperAvatar,
              helper: data.Helper ? data.Helper : "",
              id: data.Id,
              remark: data.Remark,
            };
            this.open = true;
            this.diaTitle = "编辑线索";
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
    setFromMap() {
      //设置线索来源map
      this.fromList.map((row) => {
        this.fromMap.set(row.value, row.label);
      });
    },

    setEmployeeMap(val) {
      // 设置employeeMap的值
      this.employeeMap = val;
    },
    getUsersFocus() {
      //获取用户下拉列表的焦点
      this.$refs.selectUsers.blur();
      let helperList =[]
      if(this.form.helper){
        helperList = this.form.helper.split(",");
      }
      
      if (helperList && helperList.length > 0) {
        let arr = [];
        helperList.map((row, index) => {
          if (row > 0) {
            let obj = {
              id: parseInt(row),
              name: this.form.helperName[index],
              avatar: this.form.helperAvatar[index],
              type: "user",
            };
            arr.push(obj);
          }
        });
        this.form.userInfo = JSON.parse(JSON.stringify(arr));
      } else {
        this.form.userInfo = [];
      }
      this.$refs.userPicker.show(this.form.userInfo, "user");
    },
    selectUsersed(values) {
      //选择协作人
      this.form.userInfo = values;
      if (values.length > 0) {
        let li = [];
        let li2 = [];
        let li3 = [];
        values.map((it, ix) => {
          console.log(it.name);
          li.push(parseInt(it.id));
          li2.push(it.name);
          li3.push(it.avatar);
          console.log("li", li);
        });
        this.form.helper = li.join(",");
        this.form.helperName = li2;
        this.form.helperAvatar = li3;
      } else {
        this.form.helper = undefined;
        this.form.helperName = undefined;
        this.form.helperAvatar = undefined;
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
    openDialog() {
      // 打开
      this.open = true;
      this.resetForm("customform");
    },
    // 取消按钮
    cancel() {
      // this.$emit('cancel');
      this.open = false;
    },
    submitForm() {
      //提交数据    保存线索
      this.$refs.form.validate((valid) => {
        if (valid) {
          let submitForm = {};
          submitForm = JSON.parse(JSON.stringify(this.form));
          if(submitForm.helperName){
            submitForm.helperName = submitForm.helperName.join(",");
          }
          if(!submitForm.helper){
            submitForm.helper=''
          }
          delete submitForm.helperAvatar;
          delete submitForm.userInfo;
          this.loading = true;
          if (submitForm.id) {
            priEditClue(submitForm)
              .then((res) => {
                // console.log(res, "修改线索成功");
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("修改成功");
                  this.open = false;
                  // this.getPriList();
                  // if (this.infoVisible) {
                  //     this.viewInfo(); //线索详情页的窗口是打开的，编辑线索后重新加载
                  // }
                  this.$emit("finishLoading");
                }
              })
              .catch((err) => {
                console.log("err", err);
                this.loading = false;
              });
          } else {
            priAddClue(submitForm)
              .then((res) => {
                // console.log(res, "添加线索成功");
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("添加成功");
                  this.open = false;
                  // this.getPriList();
                  this.$emit("finishLoading");
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