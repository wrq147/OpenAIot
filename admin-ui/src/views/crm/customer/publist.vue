<template>
  <div style="padding:20px 20px 0 20px;height:100%" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="创建日期">
                <el-date-picker class="form_input_style" v-model="dateRange" style="width:232px" value-format="yyyy-MM-dd"
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
          <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="handleAdd" v-hasPermi="['/CRMService/Customer/Add']">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">添加</span>
                  </el-button>
                </el-col>
              </div>
              <right-toolbar :showSearch.sync="showSearch" @queryTable="getPubList" :columns="columns"></right-toolbar>
            </el-row>

            <el-table v-loading="loading" :data="priList" :row-style="isRed" @selection-change="handleSelectionChange"
              class="data_table" :header-cell-style="cellSty" style="width:100%">
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column label="客户编号" key="CustomerNumber" prop="CustomerNumber"  width="140" align="center" v-if="columns[0].visible"></el-table-column>
              <el-table-column label="客户名称" width="220" key="CustomerName" prop="CustomerName" v-if="columns[1].visible" :show-overflow-tooltip="true">
              </el-table-column>
              <el-table-column label="客户类型" align="center" width="140" key="CustomerType" prop="CustomerType" v-if="columns[2].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.CustomerType==0?'代理':'直销'}}</span>
                </template>
              </el-table-column>
              <el-table-column label="行业类型" align="center" width="140" key="IndustryName" prop="IndustryName" v-if="columns[3].visible" :show-overflow-tooltip="true" />
              <el-table-column label="公司电话" align="center" width="140" key="CompanyTel" prop="CompanyTel" v-if="columns[4].visible"/>
              <el-table-column label="公司地址" :show-overflow-tooltip="true" align="center" key="AddressName" prop="AddressName" v-if="columns[5].visible">
                <template slot-scope="scope">
                  <p>{{ scope.row.AddressName+scope.row.AddressDetail}}</p>
                </template>
              </el-table-column>
              <el-table-column label="协作人" align="center" width="140" key="HelperName" prop="HelperName" v-if="columns[6].visible">
               <template slot-scope="scope">
                  <p>{{ scope.row.HelperName }}</p>
                </template>
              </el-table-column>
              <el-table-column label="线索来源" align="center" width="140" key="FromType" prop="FromType" v-if="columns[7].visible">
                <template slot-scope="scope" v-if="scope.row.FromType">
                  <span>{{ fromMap.get(scope.row.FromType) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns[8].visible">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" fixed="right" align="center" class-name="small-padding fixed-width" width="258">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-folder-checked" @click="handleReceive(scope.row)" v-hasPermi="['/CRMService/Customer/Draw']">领取</el-button>
                  <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)" v-hasPermi="['/CRMService/Customer/Edit']">修改</el-button>
                  <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)" v-hasPermi="['/CRMService/Customer/Remove']">删除</el-button>
                </template>
              </el-table-column>
            </el-table>

            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getPubList"/>
          </div>
        </el-col>
      </el-row>
      <el-dialog :title="diaTitle" :close-on-click-modal="false" :visible.sync="open" width="800px" append-to-body class="add_dialog_border customer_add">
        <el-form class="add_clue_form" ref="customforms" :model="customform" :rules="rules" label-width="100px" label-position="left">
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="客户名称" required prop="customerName">
                <el-input class="form_input_style" v-model="customform.customerName" placeholder="请输入客户名称" clearable size="small" style="width:100%"/>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="客户编号" required prop="customerNumber">
                <el-input class="form_input_style" v-model="customform.customerNumber" placeholder="请输入客户编号" clearable size="small" style="width:100%" :disabled="true"/>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="客户类型" required prop="customerType">
                <el-select class="form_input_style" v-model="customform.customerType" placeholder="请选择客户类型" style="width:100%">
                  <el-option :key="0" label="代理" :value="0"></el-option>
                  <el-option :key="1" label="直销" :value="1"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="协作人" prop="helperName">
                <el-select class="form_input_style" multiple v-model="customform.helperName" ref="selectUsers" placeholder="请选择协作人" @focus="getUsersFocus" style="width:100%"></el-select>
                <org-picker :multiple="true" ref="userPicker" :selected="customform.userInfo" @ok="selectUsersed"/>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="客户来源" prop="fromType">
                <el-select class="form_input_style" v-model="customform.fromType" placeholder="请选择线索来源" style="width:100%">
                  <el-option v-for="item in fromList" :key="item.value" :label="item.label" :value="item.value"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="公司网址" prop="companyUrl">
                <el-input class="form_input_style" v-model="customform.companyUrl" placeholder="请输入公司网址" clearable size="small" style="width:100%"/>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="公司电话" prop="companyTel">
                <el-input class="form_input_style" v-model="customform.companyTel" placeholder="请输入公司电话" maxlength="11" clearable size="small" style="width:100%"/>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="行业类型" prop="industry">
                <el-cascader v-model="customform.industryArr" placeholder="请选择行业类型" :props="{value:'Id', label: 'Name', children: 'children'}" :options="industryLis"
                  @change="choiceSize" style="width:100%"></el-cascader>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item prop="addressName" label="地址">
                <el-input placeholder="请选择客户地址" v-model="customform.addressName" @focus="choiceMap"></el-input>
                <el-input placeholder="请输入地址详情" v-model="customform.addressDetail" type="textarea" style="margin-top:20px"></el-input>
                <!-- <el-input placeholder="请选择企业所在地址" v-model="orgForm.lng" style="display:none"></el-input>
                   <el-input placeholder="请选择企业所在地址" v-model="orgForm.lat" style="display:none"></el-input>
                <el-input placeholder="请选择企业所在地址" v-model="orgForm.addressCode" style="display:none"></el-input>-->
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="客户详情" prop="remark">
                <el-input type="textarea" v-model="customform.remark" placeholder="请输入线索详情" :autosize="{ minRows: 2, maxRows: 4}"></el-input>
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
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  publicCustomer,
  pubAddCustomer,
  customerInfo,
  pubEditCustomer,
  publicCustomerDel,
  customerDraw,
  GenerateNumber
} from "@/api/crm/customer";
import OrgPicker from "@/views/flowable/common/OrgPicker";
import mapSelectCompt from "@/views/iot/deviceManage/mapSelectCompt";
export default {
  name: "customerPrilist",
  components: { OrgPicker,mapSelectCompt},
  mixins: [resizeTableCon],
  data() {
    return {
      diaTitle: "添加客户",
      fromList: [
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
      priList: [], //线索列表
      // 显示搜索条件
      showSearch: true,
      // 列信息
      columns: [
        { key: 0, label: `企业ID`, visible: true },
        { key: 1, label: `客户名称`, visible: true },
        { key: 2, label: `客户类型`, visible: true },
        { key: 3, label: `行业类型`, visible: true },
        { key: 4, label: `公司电话`, visible: true },
        { key: 5, label: `公司网址`, visible: true },
        { key: 6, label: `公司地址`, visible: true },
        { key: 7, label: `负责人`, visible: true },
        { key: 8, label: `协作人`, visible: true },
        { key: 9, label: `线索来源`, visible: true },
        { key: 10, label: `创建时间`, visible: true }
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
        Belong: 2 //为1表示公海，为2表示私海，其它为全部（不用传）
      },
      rules: {
        customerName: [
          {
            required: true,
            message: "客户名称不能为空",
            trigger: ["blur", "change"]
          }
        ],
        customerType: [
          {
            required: true,
            message: "请选择客户类型",
            trigger: ["blur", "change"]
          }
        ],
      },
      customform: {
        customerNumber: null, //客户唯一编号
        customerType: null, //客户类型：0为代理，1为直销（新增必填）
        fromId: "", //表单id
        bindOrgId: 0, //客户对应的企业Id,未邀请则为0
        leaderId: 0, //负责人
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
      fromMap: new Map(), //线索来源map
      returnContent: "",
      industryLis: [], //行业类型列表
      //地址选择相关参数
      choiceAddress: {},
      //地址选择相关参数
    };
  },
  beforeCreate() {
  },
  async mounted() {
    this.getDatas();
    this.setFromMap();
    this.getPubList();
  },

  methods: {
    async getIndustry(){
      //获取行业名称
      let name = await this.$store.dispatch("datas/industryName");
    },
    getDatas() {
      //加载行业数据列表
      this.$store.dispatch("datas/industryTree").then(rt => {
        // console.log("行业", rt);

        this.industryLis = rt;
      });
    },
    async getGenerateNumber() {
      //生成客户唯一编号
      try {
        let res = await GenerateNumber();

        this.customform.customerNumber = res.data;
      } catch (error) {
        console.log("生成编码报错", err);
      }
    },
    choiceSize() {
      //选择行业类型
      // console.log("行业类型",this.orgForm.industry);
      this.customform.industry = this.customform.industryArr[
        this.customform.industryArr.length - 1
      ];
    },
    handleReceive(row){
      this.$modal
        .confirm('是否确认领取客户名称为"' + row.CustomerName + '"的客户？')
        .then(()=>{
          this.loading = true;
         return customerDraw({ id: row.Id })
        })
        .then(() => {
          this.loading = false;
          this.$modal.msgSuccess("领取成功");
          this.getPubList();
        })
        .catch(() => {
          this.loading = false;
        });
    },
    onCommentInputChange() {
      this.returnContent = document.getElementById("returnContent").value;
    },
    handleUpdate(row) {
      //修改线索信息
      if (row.Id) {
        customerInfo({ id: row.Id }).then(async res => {
          if (res.data) {
            let data = res.data;
            // console.log(res.data,'res.datares.datares.data');
            let useList = [];
            let leaderList = [];
            let leaderName=''
            let industryArr=[]
            if(data.Industry){
                 industryArr=await this.getIndustryArr(data.Industry)
            }
            
            if (data.Helper) {
              data.HelperUsers.map(it => {
                useList.push({ id: it.Id, name: it.UserName, avatar: it.Avatar, type: "user" });
              });
            }
            this.customform = {
              id: data.Id,
              customerNumber: data.CustomerNumber, //客户唯一编号
              customerType: data.CustomerType, //客户类型：0为代理，1为直销（新增必填）
              fromId: data.FromId, //表单id
              bindOrgId: data.BindOrgId, //客户对应的企业Id,未邀请则为0
              leaderId: data.LeaderId, //负责人
              customerName: data.CustomerName, //客户名称
              fromType: data.FromType, //线索来源
              userInfo: useList, //已经选择的协作人员工
              helperName: data.HelperName?data.HelperName.split(","):undefined,
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
    handleDelete(row) {
      //删除线索
      this.$modal
        .confirm('是否确认删除客户"' + row.CustomerName + '"？')
        .then(()=>{
          this.loading = true;
          return publicCustomerDel({ id: row.Id });
        })
        .then(() => {
          this.$modal.msgSuccess("删除成功");
          this.getPubList();
          this.loading = false;
        })
        .catch((err) => {
          console.log("错误",err);
          
          this.loading = false;
        });
    },
    setFromMap() {
      //设置线索来源map
      this.fromList.map(row => {
        this.fromMap.set(row.value, row.label);
      });
    },
    submitForm() {
      //提交数据
      this.$refs.customforms.validate(valid => {
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
          delete submitForms.leaderName;
          delete submitForms.leaderInfo;
          delete submitForms.industryArr
          this.loading = true;
          if (submitForms.id) {
            pubEditCustomer(submitForms)
              .then(res => {
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("修改成功");
                  this.open = false;
                  this.getPubList();
                }
              })
              .catch(err => {
                console.log("err", err);
                this.loading = false;
              });
          } else {
            pubAddCustomer(submitForms)
              .then(res => {
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("添加成功");
                  this.open = false;
                  this.getPubList();
                }
              })
              .catch(err => {
                console.log("err", err);
                this.loading = false;
              });
          }
        }
      });
    },
    getUsersFocus() {
      //获取协作人选择下拉列表的焦点
      this.$refs.selectUsers.blur();
      let helperList = []
      if (this.customform.helper) {
        helperList = this.customform.helper.split(",")
      }
      if (helperList && helperList.length > 0) {
        let arr = []
        helperList.map((row, index) => {
          if (row > 0) {
            let obj = { id: parseInt(row), name: this.customform.helperName[index], avatar:this.customform.userInfo[index].avatar, type: "user" }
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
      // console.log("选中的协作人", values);
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
      }
      else {
        this.customform.helper = undefined;
        this.customform.helperName = undefined;
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
    // 取消按钮
    cancel() {
      this.open = false;
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getPubList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.Id);
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
          backgroundColor: "#F6F9FF"
        };
      }
    },
    async handleAdd() {
      //打开添加线索

      this.diaTitle = "添加客户";
      this.customform = {
        customerNumber: null, //客户唯一编号
        customerType: null, //客户类型：0为代理，1为直销（新增必填）
        fromId: "", //表单id
        bindOrgId: 0, //客户对应的企业Id,未邀请则为0
        leaderId: 0, //负责人
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
      };
      this.resetForm("customforms");
      await this.getGenerateNumber();
      this.open = true;
    },
    getPubList() {
      //获取公海线索列表
      this.loading = true;
      // this.queryParams.orgId = this.$store.getters.orgId;
      publicCustomer(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          if (response.data && response.data.List) {
            response.data.List.map(async row => {
              row.IndustryName=''
              if(row.Industry){//行业类型
                row.IndustryName = await this.$store.dispatch("datas/industryName",row.Industry);
              }
            });
            this.priList = response.data.List;
          }
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    getAddressInfo(choiceAddress) {
      if (choiceAddress) {
        this.customform.addressName = choiceAddress.AddressName;
        this.customform.addressDetail =choiceAddress.AddressDetail;
        this.customform.addressCode = choiceAddress.AddressCode;
        this.customform.lat = choiceAddress.Lat;
        this.customform.lng = choiceAddress.Lng;
      }
      // console.log("选择地址后的信息", this.customform);
    },
    choiceMap() {
      //打开地址选择的弹窗
      this.$refs.mapSelectCompt.choiceMap();
    },
  }
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