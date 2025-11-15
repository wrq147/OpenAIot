<template>
  <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="代理商" prop="OrgName" v-show="activeName=='agentList'">
                <el-input class="set_radius" v-model="queryParams.OrgName" placeholder="请输入代理商名称" clearable />
              </el-form-item>
              <el-form-item label="创建日期">
                <el-date-picker class="set_radius" v-model="dateRange" style="width:232px" value-format="yyyy-MM-dd"
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
            <el-tabs v-model="activeName" @tab-click="handleClick">
              <el-tab-pane label="代理商列表" name="agentList"></el-tab-pane>
              <el-tab-pane label="邀请记录" name="inviteList"></el-tab-pane>
            </el-tabs>
            <el-row :gutter="10" class="mb8 button_row" v-show="activeName=='agentList'">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="handleAdd" v-hasPermi="['/AuthService/Member/YaoQing']">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">邀请代理</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="primary" plain :loading="exportLoading" @click="handleExport"
                    v-hasPermi="['/AuthService/User/Export']">
                    <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                    <span style="margin-left:6px">导出</span>
                  </el-button>
                </el-col>

              </div>
              <right-toolbar :showSearch.sync="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
            </el-row>

            
            <el-table v-loading="loading" :data="agentList" :row-style="isRed" :cell-style="isRed" @selection-change="handleSelectionChange"
              class="data_table" :header-cell-style="cellSty" style="width:100%" v-show="activeName=='agentList'">
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column label="代理商ID" align="center" key="Id" prop="Id" v-if="columns[0].visible" />
              <el-table-column label="代理商名称" align="center" key="OrgName" prop="OrgName" v-if="columns[1].visible"
                :show-overflow-tooltip="true" />
              <el-table-column label="代理级别" align="center" key="GradeName" prop="GradeName" v-if="columns[3].visible"
                :show-overflow-tooltip="true" />
              <el-table-column label="上级企业名称" align="center" key="ParentOrgName" prop="ParentOrgName"
                v-if="columns[2].visible" :show-overflow-tooltip="true" />
              <el-table-column label="代理区域名称" align="center" key="RegionsName" prop="RegionsName"
                v-if="columns[4].visible" />

              <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns[6].visible" width="240">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>

              <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="150">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-delete" @click="handleCancel(scope.row)">取消授权</el-button>
                </template>
              </el-table-column>
            </el-table>

            <pagination v-show="total > 0&&activeName=='agentList'" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="getList" />
              <inviteRecord ref="inviteRecord" v-show="activeName=='inviteList'" @reloadInvite="reloadInvite"></inviteRecord>
          </div>
          
        </el-col>
      </el-row>

      <!-- 添加或修改参数配置对话框 -->
      <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="open" width="880px" append-to-body
        class="add_dialog_border" :destroy-on-close="true">
        <el-button v-if="tipsValue" slot="title" @click="returnExit" size="small" style="margin-right:10px;" icon="el-icon-back" circle></el-button>
        <div class="dialog_content_con">
          <template v-if="tipsValue == ''">
            <div :span="6" style="margin-bottom:10px">
              <span>选择代理区域</span>
            </div>
            <div :span="18" style="line-height: 40px;">
              <el-cascader class="autoheight" v-model="alChooseArea" clearable placeholder="请选择代理区域"
                :props="{ value: 'Id', label: 'Name', children: 'children', multiple: true, checkStrictly: true }"
                :options="areaLis" @change="choiceArea" style="width:100%;height:auto"></el-cascader>
            </div>
            <el-row :gutter="10">
              <el-col :span="8" style="margin-top:30px">
                <div class="label_text">姓名</div>
                  <el-input class="form_input_style" v-model="contactRealName" placeholder="请输入姓名" clearable size="small" style="width: 100%"/>
              </el-col>
              <el-col :span="8" style="margin-top:30px">
                <div class="label_text">联系方式</div>
                  <el-input class="form_input_style" v-model="contactTel" :placeholder="haiEmailLogin?'请输入手机号或者邮箱':'请输入手机号'"
                      clearable size="small" style="width: 100%"/>
              </el-col>
              <el-col :span="8" style="margin-top:30px">
                
                <div class="avatar_con">
                  
                  <image-upload ref="logoupload" v-model="logo" :limit="1" :isShowLeft="true">
                    <template #tip>
                      <div class="label_tip">
                        <div class="label_text">企业logo</div>
                        <div class="tip_con">
                          <span style="margin-left:6px">请上传企业logo</span>
                        </div>
                      </div>
                    </template>
                  </image-upload>
                </div>
              </el-col>
              
              <el-col :span="8" style="margin-top:30px">
                <div class="label_text">企业名称</div>
                <el-input class="form_input_style" v-model="orgName" placeholder="请输入企业名称" clearable size="small" style="width: 100%"/>
              </el-col>
              <el-col :span="8" style="margin-top:30px">
                <div class="label_text">行业类型</div>
                <el-cascader v-model="industryArr" placeholder="请选择行业类型" :props="{ value: 'Id', label: 'Name', children: 'children' }"
              :options="industryLis" @change="choiceSize" style="width: 100%"></el-cascader>
              </el-col>
              <el-col :span="8" style="margin-top:30px">
                <div class="label_text">规模</div>
                <el-select v-model="size" placeholder="请选择规模" style="width: 100%">
                  <el-option v-for="item in scaleLis" :key="item.value" :label="item.label" :value="item.value"></el-option>
                </el-select>
              </el-col>
              <el-col :span="24" style="margin-top:30px">
                <div class="label_text">代理商地址</div>
                <el-row :gutter="10">
                    <el-col :span="8">
                      <el-input placeholder="请选择代理商地址" v-model="addressInfo.addressName" @focus="choiceMap"></el-input>
                    </el-col>
                    <el-col :span="16">
                      <el-input placeholder="请输入地址详情" v-model="addressInfo.addressDetail"></el-input>
                    </el-col>
                </el-row>
              </el-col>
            </el-row>
            <!-- <div>
              <el-button v-if="yqWay=='link'" class="link_button create_link" type="primary" :loading="yqloading" @click="createLink()"
                style="width:100%;margin-left:0;margin-top:20px;">生成链接</el-button>
              <el-button v-if="yqWay=='sms'" class="link_button create_link" type="primary" :disabled="sending" :loading="yqloading" @click="reSendUrl"
              style="width:100%;margin-left:0;margin-top:20px;">{{tipsValue?'重新邀请':'发送短信'}}</el-button>
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
            <!-- <span class="active_dialog-footer">
              <el-button class="cancel_btton" @click="reSendUrl">短信邀请</el-button>
              <el-button class="confrim_button" type="primary" @click="onCopy">复制链接</el-button>
            </span> -->
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
    <mapSelectCompt ref="mapSelectCompt" @returnMapInfo="getAddressInfo"></mapSelectCompt>
  </div>
</template>

<script>

import { factoryInvite, factorygetAgent, CancelProxy,agentSendYqSms } from "@/api/manufac/agentMansge";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import mapSelectCompt from "@/views/iot/deviceManage/mapSelectCompt";
import inviteRecord from "@/views/manufac/factory/component/inviteRecord";
import { getConfigKey } from "@/api/system/config.js";
export default {
  name: "User",
  components: { mapSelectCompt,inviteRecord },
  dicts: ["org_size"],
  mixins: [resizeTableCon],
  data() {
    return {
      yqloading: false,
      tipsValue: "", //链接
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
      // 用户表格数据
      agentList: null,
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 日期范围
      dateRange: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
      },
      // 列信息
      columns: [
        { key: 0, label: `用户编号`, visible: true },
        { key: 1, label: `用户名称`, visible: true },
        { key: 2, label: `用户昵称`, visible: true },
        { key: 3, label: `部门`, visible: true },
        { key: 4, label: `手机号码`, visible: true },
        { key: 5, label: `状态`, visible: true },
        { key: 6, label: `创建时间`, visible: true }
      ],
      areaLis: [], //地址区域列表
      alChooseArea: [],
      contactRealName: '',
      contactTel: '',
      industryArr:[],
      industry:'',
      orgName:'',
      size:'',
      logo:'',
      yqWay:'link',//邀请方式
      addressInfo:{
        lng: 0, //经度
        lat: 0, //纬度
        geo: "", //经纬度的geo编码
        addressCode: "", //省市区代码
        addressName: "", //地址名称
        addressDetail: "", //详情地址
      },
      chooseAreaStr: "",//地址区域选择字符串
      isCancelAll: false,//是否清除改代理商下级代理的授权
      haiEmailLogin:false,
      industryLis: [],//行业
      scaleLis:[],//规模
      sending:false,
      yaoqingCode:'',
      activeName:'agentList'
    };
  },
  created() {
    this.getList();
    this.getDatas(); //获取地址列表
    
    this.canEmail()
    this.$nextTick(()=>{
      this.getIndustryDatas()
    })
  },
  mounted(){
    this.$nextTick(()=>{
      this.$refs.inviteRecord.firstLoad(1)
    })
  },
  methods: {
    async reloadInvite(row){
      //重新邀请，赋值邀请信息
      this.addressInfo.addressName = row.AddressName;
      this.addressInfo.addressDetail = row.AddressDetail;
      this.addressInfo.addressCode = row.AddressCode;
      this.addressInfo.lat = row.Lat;
      this.addressInfo.lng = row.Lng;
      this.contactRealName=row.ContactName
      this.contactTel= row.Tel;
      this.industryArr=await this.getIndustryArr(row.Industry);
      this.industry=row.Industry;
      this.orgName=row.OrgName;
      this.size=row.Size+'';
      this.logo=row.Logo;
      let Regions=row.Regions.split(',')
      this.alChooseArea=[]
      for(let i=0;i<Regions.length;i++){
        this.alChooseArea.push(this.codeGetAllPath(Regions[i],[Regions[i]]))
      }
      this.open = true;
    },
    codeGetAllPath(code,arr){
      let row=this.areaAllLis.find(row=>row.Id==code)
      if(row&&row.ParentId!='100000'){
        arr.unshift(row.ParentId)
        let rowRes=this.codeGetAllPath(row.ParentId,arr)
        arr=JSON.parse(JSON.stringify(rowRes))
      }
      return arr
    },
    async getIndustryArr(Industry) {
      //获取行业规模数组
      let arr = await this.$store.dispatch("datas/industryTree");
      let isFinish = false;
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
          if (isFinish) {
            return rsArray;
          }
        }
      }
    },
    handleClick(){

    },
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
          tel:this.contactTel,
          url:urlStr,
          code:code
        }).then(re=>{
          this.$message({
            message: "发送邀请成功",
            type: "success"
          });
          this.open=false;
          this.activeName='inviteList'
          this.sending=false
        }).catch(err=>{
          this.sending=false
        })
      })

    },
    choiceSize() {
      //选择行业类型
      this.industry =this.industryArr[this.industryArr.length - 1];
    },
    getIndustryDatas() {
      this.$store.dispatch("datas/industryTree").then((rt) => {
        // console.log("行业", rt);
        this.industryLis = rt;
      });
      this.scaleLis = this.dict.type.org_size;
    },
    async canEmail(){
      //判断是否可以通过邮箱获取
      let res=await getConfigKey("login.email")
      this.haiEmailLogin=res.data=='false'||res.data==''?true:false;
    },
    returnExit(){
      this.tipsValue=''
    },
    //地址选择相关方法
    getAddressInfo(choiceAddress) {//选择完地址
      if (choiceAddress) {
        this.addressInfo.addressName = choiceAddress.AddressName;
        this.addressInfo.addressDetail = choiceAddress.AddressDetail;
        this.addressInfo.addressCode = choiceAddress.AddressCode;
        this.addressInfo.lat = choiceAddress.Lat;
        this.addressInfo.lng = choiceAddress.Lng;
      }
      this.openSelectMap = false;
    },
    choiceMap() {
      //打开地址选择的弹窗
      //选择位置
      this.$refs.mapSelectCompt.choiceMap();
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
        this.areaAllLis=this.$store.state.datas.areaAllData
      });
    },
    onCopy() {
      //复制链接
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
        this.$message({
          message: '邀请链接不能为空',
          type: 'error',
          duration: 2000
        })
      }
    },
    //打开部门成员列表
    // getDeptMember() {
    //   this.$refs.orgPicker.show();
    // },
    /** 生产商查询代理商列表 */
    getList() {
      this.loading = true;
      this.queryParams.ParentOrgId = this.$store.getters.orgId;
      factorygetAgent(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          // console.log("生产商获取的代理商列表", response);

          this.agentList = response.data.List;
          // for(let i=0;i<3;i++){
          //   this.agentList=[...this.agentList,...this.agentList]
          // }
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    /** 搜索按钮操作 */
    handleQuery() {
      if(this.activeName=='inviteList'){
        this.$refs.inviteRecord.setParams(this.dateRange)
      }else{
        this.queryParams.pageNum = 1;
        this.getList();
      }
      
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
          backgroundColor: "rgba(32, 63, 65, 1)"
        };
      }
    },
    handleAdd() {
      this.open = true;
      this.title = "邀请代理";
      this.tipsValue = "";
    },
    /** 生成链接按钮操作 */
    createLink(cb) {
      let query = {
        customerType: 0, //邀请的客户类型：0为代理，1为直销
        bindCustomerId: "" //绑定的客户
      };
      if (this.chooseAreaStr) {
        query.regions = this.chooseAreaStr;
      }
      else {
        query.regions = "";
      }
      if(this.addressInfo.addressCode&&this.addressInfo.addressName){
        query={...this.addressInfo,...query}
      }
      if(this.contactRealName){
        query.contactName=this.contactRealName
      }else{
        if(cb){
          this.$modal.msgError("请填写联系人姓名");
          this.sending=false
          return
        }
      }
      if(this.contactTel||cb){
        if(this.haiEmailLogin&&!cb){
          if(this.validateContact(this.contactTel)=='Email'||this.validateContact(this.contactTel)=='Phone'){
            query.tel=this.contactTel
          }else{
            this.$modal.msgError("请填写正确的手机号或邮箱");
            return
          }
        }else{
          if(this.validateContact(this.contactTel)=='Phone'){
            query.tel=this.contactTel
          }else{
            this.$modal.msgError("请填写正确的手机号");
            this.sending=false
            return
          }
        }
      }
      if(this.industry){
        query.industry=this.industry
      }else{
        if(cb){
          this.$modal.msgError("请选择行业类型");
          this.sending=false
          return
        }
      }
      if(this.orgName){
        query.orgName=this.orgName
      }else{
        if(cb){
          this.$modal.msgError("请填写企业名称");
          this.sending=false
          return
        }
      }
      if(this.size){
        query.size=this.size
      }else{
        if(cb){
          this.$modal.msgError("请选择规模");
          this.sending=false
          return
        }
      }
      if(this.logo){
        query.logo=this.logo
      }else{
        if(cb){
          this.$modal.msgError("请上车企业logo");
          this.sending=false
          return
        }
      }
      this.yqloading = true;
      factoryInvite(query).then(res => {
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
          this.$refs.inviteRecord.firstLoad(1)
          if(cb){
            cb(res.data)
          }else{
            if(process.env.VUE_APP_LANG=='CN'){
              this.tipsValue = window.location.origin + "/jump.html?lang=CN&yaoqingId=" + res.data;
            }else{
              this.tipsValue = window.location.origin + "/jump.html?yaoqingId=" + res.data;
            }
          }
          
          
        }
        this.yqloading = false;
      });
    },
    validateContact(contact) {
      const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
      const phoneRegex = /^1[3-9]\d{9}$/;
      if (emailRegex.test(contact)) {
          return 'Email';
      } else if (phoneRegex.test(contact)) {
          return 'Phone';
      } else {
          return 'Invalid';
      }
    },
    /** 导出按钮操作 */
    handleExport() {
      const queryParams = this.queryParams;
      this.$modal
        .confirm("是否确认导出所有用户数据项？")
        .then(() => {
          this.exportLoading = true;
          return exportUser(queryParams);
        })
        .then(() => {
          this.exportLoading = false;
        })
        .catch(() => { });
    },
    choiceChange(event) {
      //
      console.log("event选择", document.getElementById("returnContent").checked);
      this.isCancelAll = document.getElementById("returnContent").checked
    },
    handleCancel(item) {
      var _this = this;
      const h = _this.$createElement;
      _this.$msgbox({
        title: '消息',
        message: h('p', null, [
          h('span', null, "是否确认取消'" + item.OrgName + "'的代理权限？"),
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
                label: '是否取消该代理及其所有下级代理的授权',
                // checked: _this.isCancelAll == true,

              },
              on: { change: _this.choiceChange }
            },),
            h('span', {
              style: {
                'margin': '0 0 0 5px'
              }
            }, '是否取消该代理及其所有下级代理的授权'),
          ]),

        ]),
        showCancelButton: true,
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        beforeClose: async (action, instance, done) => {
          if (action === 'confirm') {
            instance.confirmButtonLoading = true;
            instance.confirmButtonText = '执行中...';
            let res = await CancelProxy({ id: item.Id, cancelDown: _this.isCancelAll });
            setTimeout(() => {
              this.getList();
              this.$modal.msgSuccess("操作成功");
              done();
              setTimeout(() => {
                instance.confirmButtonLoading = false;
              }, 300);
            }, 1500);
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
  }
};
</script>
<style rel="stylesheet/scss" lang="scss" scoped>
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
::v-deep .el-dialog__body{
  padding-top: 20px;
  width: 100%;
  box-sizing: border-box;
}
.dialog_content_con{
  width: 100%;
  font-size: 14px;
  overflow-x: hidden;
  .label_text{
    margin-bottom: 14px;
  }
  ::v-deep .el-input{
    height: 40px;
    line-height: 40px;
    input{
      height: 40px;
      line-height: 40px;
      font-size: 14px;
    }
  }
  .autoheight{
   ::v-deep .el-input{
      height: auto !important;
    }
  }
  .create_link.el-button{
    height: 48px;
    font-size: 16px;
  }
}
.avatar_con {
    width: 100%;
    text-align: center;
    display: flex;
    align-items: flex-start;
    .label_tip{
      height: 68px;
      text-align: left;
    }
    .tip_con{
      width: 182px;
      height: 40px;
      border: 1px solid rgba(223, 226, 234, 1);
      color: rgba(120, 130, 157, 1);
      line-height: 40px;
      margin-right: 10px;
      text-align: center;
      border-radius: 4px;
      .zhongtaiiconfont{
        font-size: 10px;
      }
    }
    .el-upload--picture-card {
      background-color: #202e57;
      border: none;
    }
    ::v-deep .el-upload.el-upload--picture-card{
      width: 68px;
      height: 68px;
      line-height: 68px;
    }
    ::v-deep .component-upload-image{
      height: 68px;
      margin-bottom: 20px;
      .el-upload__tip{
        margin-top: 0;
      }
    }
    ::v-deep .el-upload-list--picture-card .el-upload-list__item{
      width: 68px;
      height: 68px;
    }
  }
</style>
<style rel="stylesheet/scss" lang="scss">
@import "~@/assets/styles/element-variables.scss";

.app-container {
  padding-right: 30px;
}

.el-tree--highlight-current .el-tree-node.is-current>.el-tree-node__content {
  background-color: $--color-primary !important;
  color: #fff !important;
}

.set_radius {
  height: 36px;
  line-height: 36px;
  border-radius: 4px;
  vertical-align: middle;
  // width: 232px;

  input {
    // width: 232px;
    border-radius: 4px;
  }
}

// .set_radius.vue-treeselect--focused{
// border: 1px solid #1890FF;
// }
.set_radius .vue-treeselect__control {
  border-radius: 4px;
  height: 38px;
  line-height: 38px;
}

.vue-treeselect__menu {
  font-weight: normal !important;
}

::v-deep .el-form-item__content {
  line-height: normal;
}

::v-deep .set_radius {
  // width: 204px;
  line-height: 38px;

  .vue-treeselect__placeholder {
    line-height: 38px;
  }

  .vue-treeselect__control {
    height: 38px;
  }
}

.set_radius .vue-treeselect--single .vue-treeselect__input {
  height: 38px;
}

.set_radius .vue-treeselect__label-container .vue-treeselect__label {
  font-weight: normal;
  color: #606266;
}

.add_dialog_border>.el-dialog {
  border: 1px solid rgba(255, 255, 255, 0.2);

  .el-dialog__body {
    border-top: 1px solid rgba(255, 255, 255, 0.2);
  }

  .tipsSelect {
    width: 73%;

    input {
      height: 40px;
      line-height: 40px;
    }
  }

  .link_button {
    background: rgba(61, 185, 143, 1);
    width: 24%;
    height: 40px;
    margin-left: 3%;
    border: none;
    &.is-disabled{
      opacity: 0.6;
    }
  }

  .dialog_content_con {
    max-height: 600px;
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
      background-color: transparent;
      border: 1px solid rgba(255, 255, 255, 0.2);
      padding: 15px 26px 15px 16px;
      border-radius: 4px;
      margin-top: 18px;
    }
  }
}

.add_invitation_border {
  .el-dialog__body {
    border-top: 1px solid rgba(255, 255, 255, 0.2);
    border-bottom: 1px solid rgba(255, 255, 255, 0.2);
    background-color: #f0f2f5;
  }

  .dialog-title {
    display: flex;
    justify-content: space-between;
    color: #ffffff;
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
              // border-left: 1px solid rgba(255, 255, 255, 0.2);
              border-radius: 4px;
              text-align: center;
            }

            input.el-input__inner {
              padding: 0 30px 0 10px;
            }
          }

          .vue-treeselect {
            display: flex;
            align-items: center;
            justify-content: center;
          }

          .vue-treeselect__input {
            display: flex;
            align-items: center;
            text-align: center;
            height: 34px;
            line-height: 34px;
          }

          .vue-treeselect__single-value,
          .vue-treeselect__placeholder {
            display: flex;
            align-items: center;
            justify-content: center;
            height: 100%;
            line-height: 100%;
          }

          .vue-treeselect__placeholder.vue-treeselect-helper-hide {
            display: none;
          }

          .vue-treeselect__label-container .vue-treeselect__label {
            font-weight: normal;
            color: #606266;
          }
        }
      }
    }
  }
}
</style>