<template>
  <div>
    <el-dialog :title="diaTitle" :close-on-click-modal="false" :visible.sync="open" width="800px" append-to-body
      class="add_dialog_border customer_add" @close="cancel" :destroy-on-close="true">
      <el-form class="add_clue_form" ref="customform" :model="customform" :rules="rules" label-width="100px"
        label-position="left">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="客户名称" prop="customerName">
              <el-input class="form_input_style" v-model="customform.customerName" placeholder="请输入客户名称" clearable
                size="small" style="width:100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="客户编号" prop="customerNumber">
              <el-input class="form_input_style" v-model="customform.customerNumber" placeholder="请输入客户编号" clearable
                size="small" style="width:100%" :disabled="true" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="客户类型" prop="customerType">
              <el-select class="form_input_style" v-model="customform.customerType" placeholder="请选择客户类型"
                style="width:100%" :disabled="customform.bindOrgId > 0">
                <el-option :key="0" label="代理" :value="0"></el-option>
                <el-option :key="1" label="直销" :value="1"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="负责人" prop="leaderId">
              <el-select class="form_input_style" filterable allow-create default-first-option
                v-model="customform.leaderName" ref="selectLeader" placeholder="请选择负责人" @focus="getLeaderFocus"
                style="width:100%" :disabled="customform.id?true:false"></el-select>
              <org-picker2 :multiple="false" ref="leaderPicker" :selected="customform.leaderInfo" @ok="selectLeadered" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="协作人" prop="helperName">
              <el-select class="form_input_style" multiple v-model="customform.helperName" ref="selectUsers"
                placeholder="请选择协作人" @focus="getUsersFocus" style="width:100%"></el-select>
              <org-picker :multiple="true" ref="userPicker" :selected="customform.userInfo" @ok="selectUsersed" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="客户来源" prop="fromType">
              <el-select class="form_input_style" v-model="customform.fromType" placeholder="请选择线索来源" style="width:100%">
                <el-option v-for="item in fromList" :key="item.value" :label="item.label" :value="item.value"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="公司网址" prop="companyUrl">
              <el-input class="form_input_style" v-model="customform.companyUrl" placeholder="请输入公司网址" clearable
                size="small" style="width:100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="公司电话" prop="companyTel">
              <el-input class="form_input_style" v-model="customform.companyTel" placeholder="请输入公司电话" clearable
                maxlength="11" size="small" style="width:100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="行业类型" prop="industry">
              <el-cascader v-model="customform.industryArr" placeholder="请选择行业类型"
                :props="{ value: 'Id', label: 'Name', children: 'children' }" :options="industryLis" @change="choiceSize"
                style="width:100%"></el-cascader>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item prop="addressName" label="地址">
              <el-input placeholder="请选择客户地址" v-model="customform.addressName" @focus="choiceMap"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="客户详情" prop="remark">
              <el-input type="textarea" v-model="customform.remark" placeholder="请输入线索详情"
                :autosize="{ minRows: 2, maxRows: 4 }"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item prop="addressDetail">
              <el-input placeholder="请输入地址详情" v-model="customform.addressDetail" type="textarea"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitForm">确 定</el-button>
        <el-button @click="cancel">取 消</el-button>
      </div>
    </el-dialog>
    
    <mapSelectCompt ref="mapSelectCompt" @returnMapInfo="getAddressInfo"></mapSelectCompt>
  </div>
</template>

<script>
import {
  priAddCustomer,
  priEditCustomer,
  customerInfo,
  GenerateNumber
} from "@/api/crm/customer";
import OrgPicker from "@/views/flowable/common/OrgPicker";
import OrgPicker2 from "@/views/flowable/common/OrgPicker";
import mapSelectCompt from "@/views/iot/deviceManage/mapSelectCompt";
export default {
  name: "AdminUiCustomerDialog",
  components: { OrgPicker, OrgPicker2,mapSelectCompt },
  props: {
    // customform: {
    //   default: {},
    //   type: Object
    // },
    // open: {
    //   //打开弹窗
    //   default: false,
    //   type: Boolean
    // },
    // diaTitle: {
    //   //弹窗表单标题
    //   default: "添加客户"
    // },
  },
  data() {
    return {
      employeeMap: new Map(),
      customform: {
        customerNumber: null, //客户唯一编号
        customerType: null, //客户类型：0为代理，1为直销（新增必填）
        fromId: "", //表单id
        bindOrgId: 0, //客户对应的企业Id,未邀请则为0
        leaderId: null, //负责人
        leaderName: [], //负责人
        leaderInfo: [], //负责人
        customerName: "", //客户名称
        fromType: "", //线索来源
        userInfo: [], //已经选择的协作人员工
        helperName: [], //
        helper: "",
        remark: "", //线索详情
        lng: 0, //经度
        lat: 0, //纬度
        geo: "", //经纬度的geo编码
        addressCode: "", //省市区代码
        addressName: "", //地址名称
        addressDetail: "", //详情地址
        industry: null, //行业类型
        industryArr: null, //行业类型在选择后展示在组件内的结果
        companyTel: "", //公司电话
        companyUrl: "" //公司网址
      },
      diaTitle: '添加客户',
      open: false,
      fromList: [
        //线索来源列表
        {
          value: "weixin",
          label: "微信线索"
        },
        {
          value: "form",
          label: "流程表单"
        },
        {
          value: "other",
          label: "其他"
        }
      ],
      rules: {
        //表单验证规则
        customerName: [
          {
            required: true,
            message: "客户名称不能为空",
            trigger: ["blur"]
          }
        ],
        customerType: [
          {
            required: true,
            message: "请选择客户类型",
            trigger: ["blur"]
          }
        ],
        leaderId: [
          {
            required: true,
            message: "请选择负责人",
            trigger: ["blur"]
          }
        ]
      },
      //地址选择相关参数
      center: null,
      map: null,
      suggestionList: [],
      search: null,
      suggest: null,
      markers: null,
      infoWindowList: Array(1),
      choiceAddress: {},
      isFirstDraw: true,
      geocoder: null,
      //地址选择相关参数
      industryLis: [] //行业类型列表
    };
  },
  mounted() {
    this.getDatas();
  },
  watch: {
    open() {
      this.resetForm("customform");
    }
  },
  methods: {
    setEmployeeMap(val) {
      // 设置employeeMap的值
      // console.log(val,'设置employeeMap的值');
      this.employeeMap = val
    },
    async handleAdd() {
      //打开添加线索

      this.diaTitle = "添加客户";
      this.customform = {
        customerNumber: null, //客户唯一编号
        customerType: null, //客户类型：0为代理，1为直销（新增必填）
        fromId: "", //表单id
        bindOrgId: 0, //客户对应的企业Id,未邀请则为0
        leaderInfo: [{ id: parseInt(this.$store.state.user.uid), name: this.$store.state.user.name, avatar: this.$store.state.user.avatar }], //已经选择的负责人
        leaderName: this.$store.state.user.name, //负责人名称默认设置为本人
        leaderAvatar: this.$store.state.user.avatar,//负责人头像
        leaderId: parseInt(this.$store.state.user.uid),//负责人id
        customerName: "", //客户名称
        fromType: "", //线索来源
        userInfo: [], //已经选择的协作人员工
        helperName: [], //
        helperAvatar: [],//协作人头像
        helper: "",
        remark: "", //线索详情
        lng: 0, //经度
        lat: 0, //纬度
        geo: "", //经纬度的geo编码
        addressCode: "", //省市区代码
        addressName: "", //地址名称
        addressDetail: "", //详情地址
        industry: null, //行业类型
        industryArr: null, //行业类型在选择后展示在组件内的结果
        companyTel: "", //公司电话
        companyUrl: "" //公司网址
      };
      this.resetForm("customform");
      await this.getGenerateNumber();
      this.openDialog()
    },
    async getGenerateNumber() {
      //生成客户唯一编号
      try {
        let res = await GenerateNumber();
        this.customform.customerNumber = res.data;
      } catch (error) {
      }
    },
    handleUpdate(row) {
      //修改线索信息
      if (row.Id) {
        customerInfo({ id: row.Id }).then(async res => {
          // console.log(res,'客户详情');
          if (res.data) {
            let data = res.data;
            let useList = [];
            let helperAvatar = [];
            let leaderList = [];
            let leaderName = "";
            let leaderAvatar = ''
            let industryArr = [];
            if (data.Industry) {
              industryArr = await this.getIndustryArr(data.Industry);
            }

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
            if (data.LeaderId) {
              //获取责任人相关信息
              leaderList.push({
                id: data.LeaderId, name: data.LeaderName, avatar: data.LeaderAvatar, type: "user"
              });
              leaderName = data.LeaderName;
              // leaderAvatar = this.employeeMap.get(parseInt(data.LeaderId))
              //   .Avatar;
            }
            this.customform = {
              id: data.Id,
              customerNumber: data.CustomerNumber, //客户唯一编号
              customerType: data.CustomerType, //客户类型：0为代理，1为直销（新增必填）
              fromId: data.FromId, //表单id
              bindOrgId: data.BindOrgId, //客户对应的企业Id,未邀请则为0
              leaderId: data.LeaderId, //负责人
              // leaderAvatar: leaderAvatar,
              leaderName: leaderName, //负责人
              leaderInfo: leaderList, //负责人
              customerName: data.CustomerName, //客户名称
              fromType: data.FromType, //线索来源
              userInfo: useList, //已经选择的协作人员工
              helperName: data.HelperName ? data.HelperName.split(",") : [], //
              helperAvatar: helperAvatar,
              helper: data.Helper,
              remark: data.Remark, //线索详情
              lng: data.Lat, //经度
              lat: data.Lng, //纬度
              geo: data.Geo, //经纬度的geo编码
              addressCode: data.AddressCode, //省市区代码
              addressName: data.AddressName, //地址名称
              addressDetail: data.AddressDetail, //详情地址
              industry: data.Industry, //行业类型
              industryArr: industryArr, //行业类型在选择后展示在组件内的结果
              companyTel: data.CompanyTel, //公司电话
              companyUrl: data.CompanyUrl //公司网址
            };
            this.openDialog()
            this.diaTitle = "编辑客户";
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
    async getIndustryArr(Industry) {
      //获取行业规模数组
      let arr = await this.$store.dispatch("datas/industryTree");
      let rsArray = [];
      for (let index = 0; index < arr.length; index++) {
        if (arr[index].children && arr[index].children.length > 0) {
          for (let ix = 0; ix < arr[index].children.length; ix++) {
            if (arr[index].children[ix].Id == Industry) {
              rsArray = [];
              rsArray.push(arr[index].children[ix].ParentId);
              rsArray.push(arr[index].children[ix].Id);
              return rsArray;
              // this.org.industryArr = JSON.parse(JSON.stringify(rsArray));
            }
          }
        }
      }
    },
    openDialog() {
      this.open = true
      this.resetForm("customform");
    },
    // 取消按钮
    cancel() {
      // this.$emit('cancel');
      this.open = false
    },
    choiceSize() {
      //选择行业类型
      // console.log("行业类型",this.orgForm.industry);
      this.customform.industry = this.customform.industryArr[
        this.customform.industryArr.length - 1
      ];
    },
    getDatas() {
      //加载行业数据列表
      this.$store.dispatch("datas/industryTree").then(rt => {
        // console.log("行业", rt);
        this.industryLis = rt;
      });
    },
    //选择负责人和协作人
    getLeaderFocus() {
      //获取负责人选择下拉列表的焦点
      this.$refs.selectLeader.blur();
      if (this.customform.leaderId > 0) {
        this.customform.leaderInfo = [{ id: this.customform.leaderId, name: this.customform.leaderName, avatar: this.customform.leaderAvatar, type: "user" }];
      }
      else {
        this.customform.leaderInfo = [];
      }

      this.$refs.leaderPicker.show(this.customform.leaderInfo, "user");
    },
    getUsersFocus() {
      //获取协作人选择下拉列表的焦点
      this.$refs.selectUsers.blur();
      let helperList = []
      if (this.customform.helper) {
        helperList = this.customform.helper.split(",")
      }
      console.log(helperList && helperList.length > 0,helperList);
      if (helperList && helperList.length > 0) {
        let arr = []
        helperList.map((row, index) => {
          if (row > 0) {
            let obj = { id: parseInt(row), name: this.customform.helperName[index], avatar: this.customform.helperAvatar[index], type: "user" }
            arr.push(obj)
          }

        })
        this.customform.userInfo = JSON.parse(JSON.stringify(arr));
      }
      else {
        this.customform.userInfo = [];
      }
      this.$refs.userPicker.show(this.customform.userInfo, "user");
    },
    selectUsersed(values) {
      //选择协作人
      this.customform.userInfo = values;
      if (values.length > 0) {
        let li = [];
        let li2 = [];
        let li3 = [];
        values.map((it, ix) => {
          li.push(parseInt(it.id));
          li2.push(it.name);
          li3.push(it.avatar);
        });
        this.customform.helper = li.join(",");
        this.customform.helperName = li2;
        this.customform.helperAvatar = li3;
      }
      else {
        this.customform.helper = undefined;
        this.customform.helperName = undefined;
        this.customform.helperAvatar = undefined;
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
      this.customform.leaderInfo = values;
      if (values.length > 0) {
        this.customform.leaderId = values[0].id;
        this.customform.leaderName = values[0].name;
        this.customform.leaderAvatar = values[0].avatar
      }
      else {
        this.customform.leaderId = 0;
        this.customform.leaderName = undefined;
        this.customform.leaderAvatar = undefined;
      }
      this.$forceUpdate();
    },
    //选择负责人和协作人
    //客户新增编辑提交数据
    submitForm() {
      //提交数据
      this.$refs.customform.validate(valid => {
        if (valid) {
          let submitForms = {};
          submitForms = JSON.parse(JSON.stringify(this.customform));
          if(submitForms.helperName){
            submitForms.helperName = submitForms.helperName.join(",");
          }
          if(!submitForms.helper){
            submitForms.helper=''
          }
          delete submitForms.userInfo;
          delete submitForms.helperAvatar;
          delete submitForms.leaderName;
          delete submitForms.leaderInfo;
          delete submitForms.leaderAvatar;
          delete submitForms.industryArr;
          this.loading = true;
          if (submitForms.id) {
            priEditCustomer(submitForms)
              .then(res => {
                if (res.code == 0) {
                  this.$modal.msgSuccess("修改成功");
                  this.cancel()
                  this.$emit('finishLoading');
                }
              })
              .catch(err => {
                console.log("err", err);
                this.$emit('errLoading');
              });
          } else {
            priAddCustomer(submitForms)
              .then(res => {
                if (res.code == 0) {
                  this.$modal.msgSuccess("添加成功");
                  this.cancel()
                  this.$emit('finishLoading', res.data);
                }
              })
              .catch(err => {
                console.log("err", err);
                this.$emit('errLoading');
              });
          }
        }
      });
    },
    //客户新增编辑新增数据
    //地址选择相关方法
    getAddressInfo(choiceAddress) {//选择完地址
      if (choiceAddress) {
        this.customform.addressName = choiceAddress.AddressName;
        this.customform.addressDetail = choiceAddress.AddressDetail;
        this.customform.addressCode = choiceAddress.AddressCode;
        this.customform.lat = choiceAddress.Lat;
        this.customform.lng = choiceAddress.Lng;
      }
    },
    choiceMap() {
      //打开地址选择的弹窗
      //选择位置
      this.$refs.mapSelectCompt.choiceMap();
    },
    
  }
};
</script>

<style lang="scss" scoped>


.add_clue_form {
  ::v-deep .el-form-item {
    margin-bottom: 22px;
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