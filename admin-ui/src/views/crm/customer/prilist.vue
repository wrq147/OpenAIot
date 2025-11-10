<template>
  <div style="padding:20px 20px 0 20px;height:100%" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="创建日期">
                <el-date-picker class="form_input_style" v-model="dateRange" style="width:232px"
                  value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                  end-placeholder="结束日期"></el-date-picker>
              </el-form-item>
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="handleAdd" v-hasPermi="['/CRMService/Customer/Add']">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">添加</span>
                  </el-button>
                </el-col>
              </div>
              <right-toolbar :showSearch.sync="showSearch" @queryTable="getPriList" :columns="columns"></right-toolbar>
            </el-row>

            <el-table v-loading="loading" :data="priList" :row-style="isRed" @selection-change="handleSelectionChange"
              class="data_table" :header-cell-style="cellSty" style="width:100%">
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column label="客户编号" width="140" align="center" v-if="columns[0].visible">
                <template slot-scope="scope">
                  <el-link type="primary" @click="viewInfo(scope.row)">{{ scope.row.CustomerNumber }}</el-link>
                </template>
              </el-table-column>
              <el-table-column label="客户名称" width="140" v-if="columns[1].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <el-link type="primary" @click="viewInfo(scope.row)">
                    {{ scope.row.CustomerName }}<i class="el-icon-edit el-icon--right"></i>
                  </el-link>
                  <el-tag type="warning"
                    v-if="scope.row.ItemOverday != null && scope.row.ItemOverday <= 7">{{ scope.row.ItemOverday }}天后跟进超期</el-tag>
                  <el-tag type="danger" v-if="scope.row.ItemOverday != null && scope.row.ItemOverday <= 0">即将超期入公海</el-tag>
                </template>
              </el-table-column>
              <el-table-column label="客户类型" align="center" key="CustomerType" prop="CustomerType" width="100"
                v-if="columns[2].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.CustomerType == 0 ? '代理' : '直销' }}</span>
                </template>
              </el-table-column>
              <el-table-column label="行业类型" align="center" key="IndustryName" prop="IndustryName" width="100"
                v-if="columns[4].visible" :show-overflow-tooltip="true" />
              <el-table-column label="公司电话" align="center" key="CompanyTel" prop="CompanyTel" v-if="columns[5].visible"
                width="140" />

              <el-table-column label="公司地址" align="center" :show-overflow-tooltip="true" key="AddressName"
                prop="AddressName" v-if="columns[6].visible">
                <template slot-scope="scope">
                  <p>{{ scope.row.AddressName + scope.row.AddressDetail }}</p>
                </template>
              </el-table-column>
              <el-table-column label="负责人" align="center" key="LeaderName" prop="LeaderName" v-if="columns[7].visible"
                width="140" />
              <el-table-column label="协作人" align="center" key="HelperName" prop="HelperName" v-if="columns[8].visible"
                width="140">
                <template slot-scope="scope">
                  <p>{{ scope.row.HelperName }}</p>
                </template>
              </el-table-column>
              <el-table-column label="线索来源" align="center" key="FromType" prop="FromType" v-if="columns[9].visible"
                width="140">
                <template slot-scope="scope" v-if="scope.row.FromType">
                  <span>{{ fromMap.get(scope.row.FromType) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="是否绑定" align="center" key="BindOrgId" prop="BindOrgId" v-if="columns[10].visible"
                width="140">
                <template slot-scope="scope">
                  <span>{{ scope.row.BindOrgId?'是':'否' }}</span>
                </template>
              </el-table-column>
              <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns[11].visible" width="180">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" fixed="right" align="center" class-name="small-padding fixed-width"
                width="258">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-user" @click.stop="handleInvite(scope.row)"
                    v-if="scope.row.BindOrgId == 0">邀请</el-button>
                  <el-button type="text" icon="el-icon-user" @click.stop="handleUnInvite(scope.row)"
                    v-if="scope.row.BindOrgId > 0">取消邀请</el-button>
                  <el-button type="text" icon="el-icon-edit" @click.stop="handleUpdate(scope.row)"
                    v-hasPermi="['/CRMService/Customer/Edit']">修改</el-button>
                  <el-dropdown v-hasPermi="['/CRMService/Customer/Return', '/CRMService/Customer/Remove']">
                    <el-button type="text" class="el-dropdown-link" @click.stop="">
                      <svg-icon icon-class="gengduo" style="margin-right:2px"></svg-icon>更多
                    </el-button>
                    <el-dropdown-menu slot="dropdown">
                      <el-dropdown-item @click.native="handleReturn(scope.row)" icon="el-icon-back"
                        v-hasPermi="['/CRMService/Customer/Return']">退回</el-dropdown-item>
                      <el-dropdown-item @click.native="handleDelete(scope.row)" icon="el-icon-delete"
                        v-hasPermi="['/CRMService/Customer/Remove']">删除</el-dropdown-item>
                    </el-dropdown-menu>
                  </el-dropdown>
                </template>
              </el-table-column>
            </el-table>

            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="getPriList" />
          </div>
        </el-col>
      </el-row>
      <customer-dialog ref="customAdd" @errLoading="errLoading" @finishLoading="finishLoading"></customer-dialog>


      <el-dialog :title="inviteTitle" :close-on-click-modal="false" :visible.sync="inviteOpen" width="600px"
        append-to-body class="add_dialog_border" :destroy-on-close="true">
        <el-button v-if="tipsValue" slot="title" @click="returnExit" size="small" style="margin-right:10px;" icon="el-icon-back" circle></el-button>
        <div class="dialog_content_con">
          <template v-if="tipsValue == ''">
            <div>
              <span>代理的生产商</span>
            </div>
            <div style="line-height: 40px; margin-top: 18px;margin-bottom:18px">
              <el-select v-model="inviteForm.factoryId" placeholder="请选择" style="width: 100%;">
                <el-option v-for="item in authList" :key="item.FactoryId" :label="item.FactoryName"
                  :value="item.FactoryId">
                </el-option>
              </el-select>
            </div>
            <div v-if="inviteType == 0">
              <span>选择代理区域</span>
            </div>
            <div style="line-height: 40px; margin-top: 18px;margin-bottom:18px" v-if="inviteType == 0">
              <el-cascader v-model="alChooseArea" clearable placeholder="请选择代理区域"
                :props="{ value: 'Id', label: 'Name', children: 'children', multiple: true, checkStrictly: true }"
                :options="areaLis" @change="choiceArea" style="width:100%"></el-cascader>
            </div>
            <div>
              <span>联系人</span>
            </div>
            <div style="line-height: 40px; margin-top: 18px;margin-bottom:18px">
              <el-input class="form_input_style" v-model="contactName" placeholder="请选择联系人" clearable
                  style="width:100%" @focus="openContactDialog" />
            </div>
            <!-- <div>
              <el-button class="link_button" type="primary" :loading="yqloading" @click="createLink"
                style="width:24%;margin-left:0;margin-bottom:20px">生成链接</el-button>
            </div> -->
            <div class="active_dialog-footer">
              <el-button class="cancel_btton" :loading="yqloading" @click="createLink()"
                style="width:260px;margin-left:0;margin-top:20px;">生成链接</el-button>
              <el-button class="confrim_button" type="primary" :disabled="sending" :loading="yqloading" @click="reSendUrl"
              style="width:260px;margin-left:0;margin-top:20px;">发送短信</el-button>
            </div>
          </template>
          <template v-else>
            <div>
              <span>通过链接邀请</span>
            </div>
            <div style="height:40px;margin-top:18px">
              <el-input v-model="tipsValue" class="tipsSelect" placeholder="邀请链接"></el-input>
              <el-button class="link_button" type="primary" @click="onCopy">复制链接</el-button>
            </div>
            <div class="date_prompt">
              <span>
                链接有效期：
                <span class="blue_color" style="color:#3572FF">7天</span>后邀请链接过期
              </span>
            </div>
          </template>
        </div>
      </el-dialog>

    </div>
    <contact-choice ref="contactOptions" @handleContactChange="handleContactChange"></contact-choice>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  privateCustomer,
  privateCustomerDel,
  customerReturn,
  customerInvite,
  customerUnInvite
} from "@/api/crm/customer";

import { listDept } from "@/api/system/dept";
import CustomerDialog from "@/views/crm/compontent/customer_dialog";
import { getConfigKey } from "@/api/system/config.js";
import { getCrmConfig } from "@/api/crm/config";
import ContactChoice from "@/views/crm/compontent/contact-choice.vue"
import OrgPicker from "@/views/flowable/common/OrgPicker";
import TextEditor from '@/components/Editor/index.vue'//富文本编辑器
import {
  AuthorizationList,
} from "@/api/manufac/myauth";
import { agentSendYqSms } from "@/api/manufac/agentMansge";
export default {
  name: "customerPrilist",
  components: { ContactChoice,CustomerDialog, OrgPicker, TextEditor },
  mixins: [resizeTableCon],
  dicts: ["follow_way"],
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
      // 表格选中值非单个禁用
      single: true,
      // 表格选中非多个禁用
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
        { key: 10, label: `是否绑定`, visible: true },
        { key: 11, label: `创建时间`, visible: true }
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

      fromMap: new Map(), //线索来源map
      returnContent: "",

      infoVisible: false, //客户详情弹窗

      deptMap: new Map(), //部门设置为Map

      //邀请相关参数
      inviteOpen: false,
      inviteType: null,//邀请的客户类型
      tipsValue: '',
      inviteForm: {},
      inviteTitle: '',
      authList: [],//代理生产商列表
      yqloading: false,
      areaLis: [],
      alChooseArea: [],
      chooseAreaStr: "", //地址区域选择字符串
      //邀请相关参数
      isCancelAgent: false,//是否取消代理权限
      overday: 0,
      contactInfo:null,//邀请的联系人信息
      contactName:'',//邀请的联系人名称
      sending:false
    };
  },
  async mounted() {
    let tmpconfig = await getCrmConfig();
    this.overday = tmpconfig.data.FollowReturnDay;

    await this.getDeptList();
    this.setFromMap();
    this.getPriList();
    this.getDatas()//获取地址相关列表
  },
  methods: {
    reSendUrl(){
      this.sending=true
      this.createLink(code=>{
        this.yaoqingCode=code
        let urlStr=''
        if(process.env.VUE_APP_LANG=='CN'){
          urlStr = window.location.origin + "/jump.html?lang=CN";
        }else{
          urlStr = window.location.origin + "/jump.html";
        }
        agentSendYqSms({
          tel:this.inviteForm.tel,
          url:urlStr,
          code:code
        }).then(re=>{
          this.$message({
            message: "发送邀请成功",
            type: "success"
          });
          this.sending=false
        }).catch(err=>{
          this.sending=false
        })
      })

    },
    handleContactChange(val) {
      //完成联系人选择
      console.log(val, 'val');
      if (val != null) {
        this.contactInfo=val
        this.contactName=val.RealName
      }
    },
    openContactDialog() {
      //选择联系人
      this.$refs.contactOptions.openContactDialog(this.inviteForm.bindCustomerId)
    },
    returnExit(){
      this.tipsValue=''
    },
    viewInfo(val) {
      //跳转客户详情
      this.$router.push({ path: "/crm/customer/customerDetail", query: { id: val.Id, } });
    },
    choiceArea() {
      //选择地址后，//选择区域后获取区域代码
      this.chooseAreaStr = "";
      this.alChooseArea.map(row => {
        // console.log("已经选择", row);
        if (row[row.length - 1] == "-1") {
          this.chooseAreaStr = this.chooseAreaStr + row[row.length - 2] + ",";
        } else {
          this.chooseAreaStr = this.chooseAreaStr + row[row.length - 1] + ",";
        }
      });
      if (this.chooseAreaStr.length > 0) {
        this.chooseAreaStr = this.chooseAreaStr.slice(
          0,
          this.chooseAreaStr.length - 1
        );
      } else {
        this.chooseAreaStr = "";
      }
      // console.log("选择的区域字符串", this.chooseAreaStr);
    },
    getDatas() {
      //获取地址列表
      this.$store.dispatch("datas/areaTree").then(area => {
        // console.log("地址", area);

        this.areaLis = area;
      });
    },
    async getFactoryList() {
      //获取代理生产商列表
      let res = await AuthorizationList();
      this.authList = res.data;
    },
    onCopy() {
      //复制邀请链接
      if (this.tipsValue) {
        let oInput = document.createElement("input");
        oInput.value = this.tipsValue;
        document.body.appendChild(oInput);
        oInput.select(); // 选择对象;
        document.execCommand("Copy"); // 执行浏览器复制命令
        this.$message({
          message: "复制成功",
          type: "success"
        });
        oInput.remove();
      } else {
        Message({
          message: '邀请链接不能为空',
          type: 'error',
          duration: 2000
        })
      }
    },
    async handleInvite(row) {
      //邀请
      console.log("邀请的客户row",row);
      await this.getFactoryList();
      if (this.authList.length == 0) {
        this.$message.error("没有可邀请的生产商，请先成为生产商的代理商");
        return;
      }
      this.contactInfo=null
      this.contactName=''
      this.inviteForm = {
        bindCustomerId: row.Id,
        factoryId: this.authList[0].FactoryId,
      }
      
      this.inviteForm.industry=row.Industry
      this.inviteForm.orgName=row.CustomerName
      // this.inviteForm.size=row.size
      this.inviteForm.addressName = row.AddressName;
      this.inviteForm.addressDetail = row.AddressDetail;
      this.inviteForm.addressCode = row.AddressCode;
      this.inviteForm.lat = row.Lat;
      this.inviteForm.lng = row.Lng;
      // this.createLink()//生成邀请链接
      this.inviteType = row.CustomerType
      this.tipsValue = ''
      this.$forceUpdate()
      // if(this.authList&&this.authList.length==1){
        
      // }
      this.inviteOpen = true
      
    },
    choiceChange(event) {
      //
      // console.log("event选择", document.getElementById("returnContent").checked);
      this.isCancelAgent = document.getElementById("returnContent").checked
    },
    handleUnInvite(row) {
      //取消邀请
      console.log("当前行数据", row);
      if (row.CustomerType == 1) {
        this.$modal
          .confirm('是否确认删除取消邀请客户"' + row.CustomerName + '"？')
          .then(() => {
            this.loading = true;
            return customerUnInvite({ id: row.Id });
          })
          .then((res) => {
            this.$modal.msgSuccess("取消邀请成功");
            this.getPriList();
            this.loading = false;
          })
          .catch(() => {
            this.loading = false;
          });
      } else {
        var _this = this;
        const h = _this.$createElement;
        _this.$msgbox({
          title: '消息',
          message: h('p', null, [
            h('span', null, '是否确认删除取消邀请客户"' + row.CustomerName + '"？'),
            h('br'),
            h('div', {
              style: {
                width: '100%',
                height: '20px',
                margin: '3px 0 3px 10px'
              }
            }),
            h('div', {
              style: {
                'display': 'flex',
                'justify-content': 'flex-start',
                'align-items': 'center',
                'height': '29px',
                'line-height': '29px',
              }
            }, [
              h('input', {
                attrs: {
                  class: "el-checkbox",
                  type: "checkbox",
                  id: "returnContent",
                  name: 'choice',
                  label: '是否取消代理权限',
                  // checked: _this.isCancelAll == true,

                },
                on: { change: _this.choiceChange }
              },),
              h('span', {
                style: {
                  'margin': '0 0 0 5px'
                }
              }, '是否取消代理权限'),
            ]),

          ]),
          showCancelButton: true,
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          beforeClose: async (action, instance, done) => {
            if (action === 'confirm') {
              instance.confirmButtonLoading = true;
              instance.confirmButtonText = '执行中...';
              this.loading = true;
              try {
                let res = await customerUnInvite({ id: row.Id, cancelAgent: this.isCancelAgent });
                setTimeout(() => {
                  this.$modal.msgSuccess("取消邀请成功");
                  this.getPriList();
                  this.loading = false;
                  done();
                  setTimeout(() => {
                    instance.confirmButtonLoading = false;
                  }, 300);
                }, 1500);
              } catch (error) {
                this.loading = false;
                instance.confirmButtonLoading = false;
              }

            } else {
              done();
            }
          }
        }).then(action => {
          // this.$message({
          //   type: 'info',
          //   message: 'action: ' + action
          // });
        });
      }

    },
    /** 生成链接按钮操作 */
    createLink(cb) {
      //生成邀请链接
      if (this.inviteType == 0) {
        this.inviteForm.regions = this.chooseAreaStr
      } else {
        this.inviteForm.regions = ''
      }
      if(this.contactInfo){
        this.inviteForm.contactName=this.contactInfo.RealName
        this.inviteForm.tel=this.contactInfo.Mobile
      }else{
        if(cb){
          this.$modal.msgError("请先选择联系人");
          return
        }
      }
      // console.log(this.inviteForm,'生成链接参数');
      customerInvite(this.inviteForm).then(res => {
        // console.log("生成的邀请码", res);
        // //获取当前url
        // let baseUrl = window.location.href;
        // //当前路由
        // let baseR = this.$route.path;
        // //分割url
        // let yumAry = baseUrl.split(baseR);
        // //得到域名
        // let yuming = yumAry[0];
        if (res.code == 0) {
          if(cb){
            cb(res.data)
          }else{
            if (process.env.VUE_APP_LANG == 'CN') {
              this.tipsValue = window.location.origin + "/jump.html?lang=CN&yaoqingId=" + res.data;
            } else {
              this.tipsValue = window.location.origin + "/jump.html?yaoqingId=" + res.data;
            }
          }
          
        }
      });
    },


    async getDeptList() {
      //获取企业部门列表
      try {
        let res = await listDept();
        if (res.data) {
          res.data.map(row => {
            this.deptMap.set(row.deptId, row);
          });
        }
      } catch (error) {
        console.log("部门列表报错", error);
      }
    },



    // closeCustomDialog() {
    //   //关闭客户详情展示的弹窗
    //   this.infoVisible = false;
    // },
    errLoading() {
      //访问接口失败
      this.loading = false;
    },
    finishLoading() {
      this.loading = false;
      this.$refs.customAdd.cancel()//关闭弹窗
      this.getPriList();
    },
    async getIndustry() {
      //获取行业名称
      let name = await this.$store.dispatch("datas/industryName");
    },

    handleReturn(row) {
      //退回线索
      this.returnContent = ""; //将退回原因置空
      var _this = this;
      const h = _this.$createElement;
      _this
        .$msgbox({
          title: "是否确认退回客户",
          message: h(
            "div",
            {
              attrs: {
                class: "el-textarea"
              }
            },
            [
              h("textarea", {
                attrs: {
                  class: "el-textarea__inner",
                  autocomplete: "off",
                  rows: 4,
                  placeholder: "请输入退回原因",
                  id: "returnContent"
                },
                value: _this.returnContent,
                on: { input: _this.onCommentInputChange }
              })
            ]
          ),
          showCancelButton: true,
          confirmButtonText: "退回",
          cancelButtonText: "取消",
          beforeClose: (action, instance, done) => {
            if (action === "confirm") {
              if (this.returnContent) {
                instance.confirmButtonLoading = true;
                customerReturn({ id: row.Id, reason: this.returnContent })
                  .then(res => {
                    if (res.code == 0) {
                      instance.confirmButtonLoading = false;
                      this.$message({
                        type: "success",
                        message: "退回成功"
                      });
                      this.getPriList();
                      done();
                    }
                  })
                  .catch(err => {
                    console.log("err", err);
                    instance.confirmButtonLoading = false;
                  });
              } else {
                this.$message({
                  type: "error",
                  message: "退回原因不能为空"
                });
              }
            } else {
              done();
            }
          }
        })
        .then(action => {

        });
    },
    onCommentInputChange() {
      this.returnContent = document.getElementById("returnContent").value;
    },
    handleUpdate(row) {
      //修改客户信息
      if (row.Id) {
        this.$refs.customAdd.handleUpdate(row)
      }
    },
    handleDelete(row) {
      //删除线索
      this.$modal
        .confirm('是否确认删除客户"' + row.CustomerName + '"？')
        .then(() => {
          this.loading = true;
          return privateCustomerDel({ id: row.Id });
        })
        .then(() => {
          this.$modal.msgSuccess("删除成功");
          this.getPriList();
          this.loading = false;
        })
        .catch(() => {
          this.loading = false;
        });
    },

    setFromMap() {
      //设置线索来源map
      this.fromList.map(row => {
        this.fromMap.set(row.value, row.label);
      });
    },
    // 取消按钮
    // cancel() {
    //   this.open = false;
    // },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getPriList();
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
      console.log("选中的", this.ids);

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
      this.$refs.customAdd.handleAdd()
    },
    getPriList() {
      //获取公海线索列表
      this.loading = true;
      // this.queryParams.orgId = this.$store.getters.orgId;
      privateCustomer(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          // console.log("客户列表信息", response);
          if (response.data && response.data.List) {
            response.data.List.map(async row => {
              if (this.overday > 0) {
                let followTime = row.LastFollowDate.replace(new RegExp(/-/gm), '/').replace('T', ' ').replace(new RegExp(/\.[\d]{3}/gm), '');
                let deta = new Date() - new Date(followTime);
                let detaday = deta / (1 * 24 * 60 * 60 * 1000);
                row.ItemOverday = this.overday - detaday;
              }
              row.IndustryName = "";
              if (row.Industry) {
                //行业类型
                row.IndustryName = await this.$store.dispatch(
                  "datas/industryName",
                  row.Industry
                );
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
    }
  }
};
</script>
<style lang="scss" scoped>
.active_dialog-footer {
  display: flex;
  justify-content: flex-end;
  margin-top: 30px;
  .cancel_btton {
    height: 52px;
    width: calc(50% - 10px);
    border: 1px solid #dfe2ea;
    background: #f6f9ff;
    color: #3572ff;
    font-size: 20px;
    margin-right: 20px;
  }
  .confrim_button {
    height: 52px;
    font-size: 20px;
    width: calc(50% - 10px);
    background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
  }
}
</style>
<style lang="scss">
.follow_add_form {
  .el-form-item {
    margin-bottom: 12px;

    .el-form-item__content {
      width: 100%;
    }
  }
}

// .custom_info_dialog {
//   .el-dialog__body {
//     padding-top: 0;
//     position: relative;
//   }
// }





.add_dialog_border>.el-dialog {
  border: 1px solid #e4e4e5;

  // .el-dialog__body {
  //   border-top: 1px solid #e4e4e5;
  // }

  .tipsSelect {
    width: 73%;

    input {
      height: 40px;
      line-height: 40px;
    }
  }

  .link_button {
    background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
    width: 24%;
    height: 40px;
    margin-left: 3%;
    border: none;
  }

  .dialog_content_con {
    max-height: 500px;
    overflow-y: auto;

    // 滚动条的样式,宽高分别对应横竖滚动条的尺寸
    &::-webkit-scrollbar {
      width: 0;
    }

    // 滚动条里面默认的小方块,自定义样式
    &::-webkit-scrollbar-thumb {
      background: #8798af;
      border-radius: 2px;
    }

    // 滚动条里面的轨道
    &::-webkit-scrollbar-track {
      background: transparent;
    }

    .date_prompt {
      background-color: #f9fafb;
      border: 1px solid #e4e4e5;
      padding: 15px 26px 15px 16px;
      border-radius: 4px;
      margin-top: 18px;
    }

    .account_add {
      border: 1px solid #2878ff !important;
      border-radius: 4px;

      input.el-input__inner {
        border: none;
        border-right: 1px solid #e4e4e5;
        // border-radius: 10px 0 0 10px;
        border-radius: 4px;
      }

      // .el-input-group__append {
      //   background-color: #fff;
      //   border: none;
      //   border-radius: 0 10px 10px 0;
      // }
    }
  }
}

.add_invitation_border {
  .el-dialog__body {
    // border-top: 1px solid #e4e4e5;
    // border-bottom: 1px solid #e4e4e5;
    background-color: #f0f2f5;
  }

  .dialog-title {
    display: flex;
    justify-content: space-between;
  }

  .account_add {
    border: 1px solid #2878ff !important;
    border-radius: 4px;
    width: 100%;
    background-color: #ffffff;

    input.el-input__inner {
      border: none;
      border-radius: 4px 0 0 4px;
      // border-radius: 4px;
      width: 85%;
    }

    .el-input-group__append {
      background-color: #fff;
      border: none;
      border-radius: 0 4px 4px 0;
      width: 15%;

      .el-input {
        text-align: center;
      }

      input {
        padding: 0;
        width: 85%;
      }
    }
  }

  .invite_tips {
    height: 302px;
    // display: flex;
    // align-items: center;
    // justify-content: center;
    width: 100%;

    .tips_con {
      width: 100%;
      height: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .daiyaoqing {
      padding: 0;
      margin: 0;
      margin-top: 10px;

      li {
        list-style: none;
        padding-top: 30px;
        position: relative;
        font-size: 14px;
        background-color: #ffffff;
        border-radius: 10px;

        div.input_float_top {
          display: flex;
          align-items: center;
          flex-direction: column;
          justify-content: center;
          margin-bottom: 30px;
        }

        div.input_float_bottom {
          display: flex;
          align-items: center;
          justify-content: center;
          flex-direction: column;
          text-align: center;

          .el-input {
            margin-bottom: 30px;

            input {
              // border: none;
              // border-left: 1px solid #E4E4E5;
              border-radius: 4px;
              text-align: center;
            }

            input.el-input__inner {
              padding: 0 30px 0 10px;
            }
          }
        }
      }
    }
  }
}
</style>