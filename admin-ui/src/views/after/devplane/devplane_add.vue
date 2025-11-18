<template>
  <div>
    <el-dialog title="添加计划" :visible.sync="planeOpen" center width="900px" top="10px" :close-on-click-modal="false"
      :destroy-on-close="true">
      <el-form :model="devplaneFrom" ref="devplaneFrom" :rules="devplaneRules" label-position="left" class="groupFrom"
        :inline="true" label-width="110px">
        <el-form-item label="计划名称" prop="name">
          <el-input type="text" v-model="devplaneFrom.name" placeholder="请输入计划名称" :disabled="isViewInfo"></el-input>
        </el-form-item>

        <el-form-item label="计划任务" prop="taskType">
          <el-radio-group v-model="devplaneFrom.startWay" :disabled="isReadonly || isViewInfo">
            <el-radio label="0">手动发起</el-radio>
            <el-radio label="1">定时发起</el-radio>
            <el-radio label="2">设备事件</el-radio>
          </el-radio-group>
        </el-form-item>
        <template v-if="devplaneFrom.startWay == 2">
          <el-form-item label="目标产品">
            <el-select :disabled="isReadonly || isViewInfo" v-model="devplaneFrom.targetProductId" filterable remote
              reserve-keyword :clearable="true" placeholder="请输入关键词" :remote-method="remoteMethod" :loading="prodloading"
              @change="chgProd">
              <el-option v-for="item in productLists" :key="item.Id" :label="item.ProductName" :value="item.Id">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="设备事件" prop="eventSelList">
            <el-select v-model="devplaneFrom.eventSelList" multiple placeholder="请选择"
              :disabled="isReadonly || isViewInfo">
              <el-option v-for="item in eventList" :key="item.EventId" :label="item.EventName" :value="item.EventId">
              </el-option>
            </el-select>
          </el-form-item>
        </template>
        <template v-else>
          <el-form-item label="计划目标">
            <div style="width: 100%;height:20px;"></div>
          </el-form-item>
          <div style="margin-bottom: 20px;">
            <el-row :gutter="10" class="mb8 button_row" style="margin-top:-25px;justify-content:flex-start;" v-if="!isViewInfo">
              <el-col :span="1.5">
                <el-button type="primary" plain @click="openAddDevice">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                  <span style="margin-left: 6px">加入设备</span>
                </el-button>
              </el-col>
              <el-col :span="1.5">
                <el-button type="primary" plain @click="openAddProduct">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                  <span style="margin-left: 6px">加入产品</span>
                </el-button>
              </el-col>
              <el-col :span="1.5">
                <el-button type="primary" plain @click="openAddRoom">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                  <span style="margin-left: 6px">加入车间</span>
                </el-button>
              </el-col>
            </el-row>
            <el-table border :data="planeTargetData" max-height="300" style="margin-top: 10px;width: 100%;"> 
              <!-- <el-table-column label="预览图片" align="center" width="150">
                <template slot-scope="scope">
                  <div class="imgwrap" style="max-width: 30px;max-height:30px;">
                    <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'" :preview-src-list="[scope.row.PhotoUrl]"></el-image>
                  </div>
                </template>
              </el-table-column> -->
              <el-table-column label="名称" prop="TargetName" align="center" :show-overflow-tooltip="true" ></el-table-column>
              <el-table-column label="类型" prop="TargetType" align="center" :show-overflow-tooltip="true" >
                <template slot-scope="scope">
                  <div>{{scope.row.TargetType==0?'设备':(scope.row.TargetType==1?'产品':(scope.row.TargetType==2?'车间':''))}}</div>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="200" v-if="!isViewInfo">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-delete" @click="handleTargetDelete(scope.row,scope.$index)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>
            
          </div>
        </template>

        <el-form-item label="定时时间" prop="timerCron" v-if="devplaneFrom.startWay == '1'">
          <el-input style="width: 460px" :readonly="true" v-model="devplaneFrom.timerCronName" placeholder="请选择定时时间">
            <template slot="append">
              <el-button type="primary" @click="handleShowCron" :disabled="isViewInfo">
                设置时间
                <i class="el-icon-time el-icon--right"></i>
              </el-button>
            </template>
          </el-input>
        </el-form-item>
        <el-form-item label="执行天数" prop="planeDays">
          <el-radio-group v-model="planeDaysType" :disabled="isViewInfo" @change="planeDaysTypeChange">
            <el-radio :label="0">不限制</el-radio>
            <el-radio :label="1">指定天数</el-radio>
          </el-radio-group>
          <el-input-number v-model="devplaneFrom.planeDays" :step="1" step-strictly :min="0"
            :disabled="isViewInfo" v-if="planeDaysType==1" style="margin-left:20px;"></el-input-number>
        </el-form-item>
        <el-form-item label="发起人" prop="flowCreatedUserId" v-if="devplaneFrom.startWay != '0'">
          <el-radio-group v-model="flowCreatedUserType" :disabled="isViewInfo">
            <el-radio :label="1">设备负责人</el-radio>
            <el-radio :label="2">指定人员</el-radio>
          </el-radio-group>
          <el-select filterable allow-create default-first-option v-model="devplaneFrom.flowCreatedUserName"
            ref="selectFlowCreatedUser" placeholder="请选择发起人" @focus="getCreatedFocus" style="width:262px;margin-left:20px;"
            :disabled="isViewInfo" v-if="flowCreatedUserType==2"></el-select>
        </el-form-item>
        <el-form-item label="是否公开" prop="IsFilterLeader">
          <el-radio-group v-model="devplaneFrom.IsFilterLeader" :disabled="isViewInfo" style="width: 262px;">
            <el-radio :label="false">是</el-radio>
            <el-radio :label="true">否</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="排除假期" prop="excludeHoliday" v-if="devplaneFrom.startWay == '1'">
          <el-radio-group v-model="devplaneFrom.excludeHoliday" :disabled="isViewInfo">
            <el-radio :label="true">是</el-radio>
            <el-radio :label="false">否</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="超期提醒">
          <el-row :gutter="10" v-if="!isViewInfo">
              <el-col :span="1.5">
                <el-button type="primary" plain @click="setAddExpireNotices">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                  <span style="margin-left: 6px">增加提醒</span>
                </el-button>
              </el-col>
            </el-row>
        </el-form-item>
        <el-table border :data="devplaneFrom.ExpireNotices" max-height="300" style="margin-top: -10px;width: 100%;margin-bottom:10px">
          <el-table-column label="多少天后提醒" prop="day" align="center" :show-overflow-tooltip="true">
            <template slot-scope="scope">
              <el-input-number v-model="scope.row.day" :step="1" step-strictly :min="0" :disabled="isViewInfo"></el-input-number>
            </template>
          </el-table-column>
          <el-table-column label="提醒人员" prop="way" align="center" width="135" :show-overflow-tooltip="true" >
            <template slot-scope="scope">
              <el-select v-model="scope.row.way" placeholder="请选择" :disabled="isViewInfo">
                <el-option key="1" label="指定人员" value="1"></el-option>
                <el-option key="2" label="设备负责人" value="2"></el-option>
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="提醒方式" prop="ccway" align="center" width="135" :show-overflow-tooltip="true" >
            <template slot-scope="scope">
              <el-select v-model="scope.row.ccway" placeholder="请选择" :disabled="isViewInfo">
                <el-option key="0" label="单独提醒" value="0"></el-option>
                <el-option key="1" label="合并提醒" value="1"></el-option>
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="指定提醒人员" align="center" class-name="small-padding fixed-width" width="300">
            <template slot-scope="scope">
              <el-select filterable allow-create multiple default-first-option v-model="scope.row.targetUser"
                ref="selectExpireUser" placeholder="请选择提醒人员" @focus="getExpireFocus(scope.$index)" style="width:100%"
                @remove-tag="removeExpireUser" :disabled="isViewInfo" v-if="scope.row.way=='1'">
                <el-option v-for="item in scope.row.target" :key="item.userid" :label="item.name"
                  :value="item.userid">
                </el-option>
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="60" v-if="!isViewInfo">
            <template slot-scope="scope">
              <el-button type="text" icon="el-icon-delete" @click="handleExpireNoticesDelete(scope.row,scope.$index)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-form-item label="计划流程" prop="flowTemplateId">
          <el-select :disabled="isReadonly || isViewInfo" filterable allow-create default-first-option
            v-model="devplaneFrom.flowTemplateName" ref="selectFlowTemp" placeholder="请选择计划流程" @focus="selectFlowTemp"
            style="width:100%"></el-select>
        </el-form-item>
        <div class="box-row" v-show="devplaneFrom.flowTemplateName!=''">
          <div class="bx-hd">流程表单初始化</div>
          <div class="bx-bd">
            <el-table border v-loading="tbloading" :data="formInit" row-key="id" style="width:100%">
              <el-table-column prop="title" label="表单字段" align="center" width="300">
                <template slot-scope="scope">
                  <div v-if="scope.row.way==-1&&devplaneFrom.startWay!=0||scope.row.way!=-1">{{scope.row.title}}</div>
                </template>
              </el-table-column>
              <el-table-column label="值类型" align="center" width="120">
                <template slot-scope="scope">
                  <el-select v-model="scope.row.way" placeholder="请选择"
                    @change="changeLoadVal($event, scope.row, scope.$index)" :disabled="isViewInfo" v-if="scope.row.way!=-1">
                    <el-option label="自定义" :value="0" v-if="scope.row.eltype != 'DevicPicker'&&scope.row.eltype != 'UserPicker'"></el-option>
                    <el-option label="系统值" :value="1" v-if="scope.row.eltype != 'TableList'&&scope.row.eltype != 'SelectInput'"></el-option>
                  </el-select>
                  <div v-if="scope.row.way==-1&&devplaneFrom.startWay!=0">无</div>
                </template>
              </el-table-column>
              <el-table-column label="初始值" align="center">
                <template v-slot:default="scope">
                  <template v-if="scope.row.eltype != 'TableList'&&scope.row.eltype != 'SelectInput'&&scope.row.way!=-1">
                    <el-input v-if="scope.row.way == 0" v-model="scope.row.val" placeholder="请输入内容"
                      :disabled="isViewInfo"></el-input>
                    <el-select v-else v-model="scope.row.val" placeholder="请选择" :disabled="isViewInfo">
                      <el-option label="计划类型" value="计划类型"
                      v-if="scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput'"></el-option>
                      <el-option label="任务备注" value="任务备注"
                      v-if="scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput'"></el-option>
                      <el-option label="设备名称" value="设备名称"
                        v-if="scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput'"></el-option>
                      <el-option label="设备编号" value="设备编号"
                        v-if="scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput'"></el-option>
                      <el-option label="通讯编号" value="通讯编号"
                        v-if="scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput'"></el-option>
                      <el-option label="同名参数" value="同名参数"
                        v-if="scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput'"></el-option>
                      <el-option label="告警级别" value="告警级别"
                        v-if="scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput'"></el-option>
                      <el-option label="目标设备" value="目标设备" v-if="scope.row.eltype == 'DevicPicker'"></el-option>
                      <el-option label="提交人" value="提交人" v-if="scope.row.eltype == 'UserPicker'"></el-option>
                      <el-option label="提交人部门" value="提交人部门"
                      v-if="scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput'"></el-option>
                      <el-option label="提交人姓名" value="提交人姓名"
                      v-if="scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput'"></el-option>
                      <el-option label="车间名称" value="车间名称"
                      v-if="scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput'"></el-option>
                      <el-option label="车间分类名" value="车间分类名"
                      v-if="scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput'"></el-option>
                      <el-option label="设备责任人" value="设备责任人" v-if="scope.row.eltype == 'UserPicker'"></el-option>
                      <el-option label="设备责任与协作人" value="设备责任与协作人" v-if="scope.row.eltype == 'UserPicker'"></el-option>
                    </el-select>
                  </template>
                  <template v-else-if="scope.row.eltype == 'TableList'&&scope.row.way!=-1">
                    <TableList :valueModel="{}" v-model="scope.row.val" mode="mode" v-bind="scope.row.props"
                      :disabled="isViewInfo"></TableList>
                    <!-- <component ref="form" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel" v-bind="config.props"/> -->
                  </template>
                  <template v-else-if="scope.row.eltype == 'SelectInput'&&scope.row.way==0">
                    <el-select v-model="scope.row.val" placeholder="请选择" :disabled="isViewInfo">
                      <template v-if="scope.row.props&&scope.row.props.options">
                        <el-option :label="its" :value="its" v-for="(its,ox) in scope.row.props.options" :key="'sel'+ox"></el-option>
                      </template>
                    </el-select>
                    <!-- <component ref="form" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel" v-bind="config.props"/> -->
                  </template>
                  <template v-else-if="devplaneFrom.startWay!=0&&scope.row.way==-1">
                    <div style="color:#E74032">{{'*'+scope.row.val}}</div>
                    <!-- <component ref="form" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel" v-bind="config.props"/> -->
                  </template>
                </template>

              </el-table-column>
            </el-table>
          </div>
        </div>
        <el-form-item label="备注" prop="remark">
          <el-input type="textarea" v-model="devplaneFrom.remark" placeholder="请输入计划类型备注"
            :disabled="isViewInfo"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer" v-if="!isViewInfo">
        <el-button @click="planeOpen = false">取 消</el-button>
        <el-button type="primary" @click="addDevicePlane" v-loading="submitLoading" :disabled="submitLoading">确
          定</el-button>
      </div>
    </el-dialog>
    <selectDevice ref="selectDevice" @addPlaneDevice="addPlaneDevice"></selectDevice>
    <selectDevRoom ref="selectDevRoom" @addDevRoom="addDevRoom"></selectDevRoom>
    <selectDevProduct ref="selectDevProduct" @addPlaneDevProduct="addPlaneDevProduct"></selectDevProduct>
    
    <FlowPicker ref="flowPicker" @selected="onSelected"></FlowPicker>
    <org-picker :multiple="activePensonType == 'expire'" ref="flowCreatedUser" @ok="selectLeadered"
      :isMulLimit="activePensonType == 'expire'" :limitNum="5" />
    <cronTime ref="cronTime" @finishTimeChoice="finishTimeChoice"></cronTime>
  </div>
</template>

<script>
import OrgPicker from "@/views/flowable/common/OrgPicker";
import FlowPicker from "@/views/flowable/common/FlowPicker.vue";
import selectDevice from "@/views/after/devplane/selectcompt/selectDevice.vue";
import selectDevRoom from "@/views/after/devplane/selectcompt/selectDevRoom.vue";
import selectDevProduct from "@/views/after/devplane/selectcompt/selectDevProduct.vue";
import { getItems } from "@/views/flowable/common/utlity.js"
import cronTime from "@/views/iot/rulesEngine/cron_time";
import { toCronDes } from "@/api/monitor/job";
import { getFormDetail } from "@/api/flowable/design";
import { myProductList } from "@/api/after/dev";
import { devPlaneAdd, devPlaneInfo, devPlaneEdit } from "@/api/after/devplane";
import {
  productInfo
} from "@/api/rules/productModel";
import { getUserInfo } from '@/api/login'
import TableList from '@/views/flowable/common/form/components/TableList.vue'
import { checkPermi } from '@/utils/permission.js';

export default {
  name: 'AdminUiDevplaneAdd',
  components: { OrgPicker, FlowPicker, cronTime, TableList,selectDevice,selectDevProduct,selectDevRoom },
  data() {
    return {
      activeExpireNoticesUser:null,
      planeDaysType:0,//计划天数类型
      flowCreatedUserType:1,//计划发起人是指定人员还是设备负责人
      isViewInfo: false,//是否是查看详情
      isReadonly: false,//是否只读
      planeOpen: false,
      tbloading: false,
      devplaneFrom: {
        flowCreatedUserId: '',
        flowCreatedUserName: '',
        expireNoticeUsers: [],
        planeDays: 1,
        flowTemplateId: '',
        flowTemplateName: '',
        flowInitJson: '',
        startWay: '0',
        excludeHoliday:false,
        ExpireNotices:[],
        name: '',
        remark: '',
        timerCron: '',
        targetProductId: "",
        events: [],
        eventSelList: [],
      },
      devplaneRules: {
        timerCron: [{ required: true, trigger: "change", message: "请输入定时时间" }],
        name: [{ required: true, trigger: "blur", message: "请输入计划名称" }],
        sort: [{ required: true, trigger: "blur", message: "请输入计划序号" }],
        flowCreatedUserId: [{ required: true, trigger: "blur", message: "请选择发起人" }],
        expireNoticeUsers: [{ required: true, trigger: "change", message: "请选择提醒人员" }],
        flowTemplateId: [{ required: true, trigger: ["change"], message: "请选择计划流程" }],
        eventSelList: [{ required: true, trigger: "change", message: "请选择设备事件" }],
        planeDays: [{ required: true, trigger: "change", message: "请输入计划执行天数" }],
      },
      activePensonType: 'created',
      formInit: [],
      productLists: [], //产品列表
      
      selectDevice: [],//选择后的设备列表
      eventList: [],//选择设备后的总事件列表
      // eventSelList:[],//选择后的事件
      submitLoading: false,
      productQuery: {
        pageNum: 1,
        pageSize: 300,
        Name: "",
        Pids: []
      },
      prodloading: false,
      selectDevProduct:[],//选择后的产品列表
      planeTargetData:[],
      // flowCreatedUser:[]
    };
  },
  computed: {
    IsFactory() {
      if (checkPermi(['/AgentMan/'])) {
        return true;
      }
      else{
        return false;
      }
    }
  },
  mounted() {

  },
  methods: {
    handleExpireNoticesDelete(row,index){
      this.devplaneFrom.ExpireNotices.splice(index,1)
    },
    setAddExpireNotices(){
      //增加提醒,//提醒方式ccway： 0为单独提醒、1为汇总提醒
      this.devplaneFrom.ExpireNotices.push({'day':1,'way':'1','ccway':'0','target':[],targetUser:[]})
    },
    planeDaysTypeChange(val){
      // console.log('切换计划天数',val);
      if(val==1){
        if(this.devplaneFrom.planeDays==0){
          this.devplaneFrom.planeDays=1
        }
      }
    },
    handleTargetDelete(row,inx){
      //删除计划目标
      // console.log('删除计划目标',row,inx);
      this.planeTargetData.splice(inx,1)
    },
    async remoteMethod(query) {
      if (query !== '') {
        this.productQuery.Name = query;
        await this.getproductList();
      }else{
        delete this.productQuery.Name
        await this.getproductList();
      }
    },
    async getproductList() {
      this.prodloading = true;
      let response = await myProductList(this.productQuery)
      if (response.code == 0) {
        if (response.data && response.data.List) {
          this.productLists = response.data.List;
        }
      }
      this.prodloading = false;
    },
    
    changeLoadVal(event, row, index) {
      //切换流程初始化默认值设置
      if (event == 1 && row.eltype == "TableList") {
        this.formInit[index].val = {}
      } else {
        this.formInit[index].val = ''
      }
    },
    addPlaneDevice(sellist) {
      //添加计划的设备
      let beforeSel=JSON.parse(JSON.stringify(this.planeTargetData))
      this.planeTargetData=beforeSel.filter(row=>row.TargetType!=0)
      this.planeTargetData=[...this.planeTargetData,...sellist]
      this.deviceOpen = false
    },
    addPlaneDevProduct(sellist){
      //添加计划的产品
      let beforeSel=JSON.parse(JSON.stringify(this.planeTargetData))
      this.planeTargetData=beforeSel.filter(row=>row.TargetType!=1)
      this.planeTargetData=[...this.planeTargetData,...sellist]
      this.productOpen = false
    },
    addDevRoom(sellist){
      //添加计划的设备车间
      let beforeSel=JSON.parse(JSON.stringify(this.planeTargetData))
      this.planeTargetData=beforeSel.filter(row=>row.TargetType!=2)
      this.planeTargetData=[...this.planeTargetData,...sellist]
      this.deviceOpen = false
    },
    async chgProd() {
      await this.loadEventList();
    },
    async loadEventList() {
      //初始化事件列表
      if (this.devplaneFrom.targetProductId == "") {
        return;
      }
      let eventItem = await this.getProductEventsList(this.devplaneFrom.targetProductId)
      let eventItem2 = JSON.parse(JSON.stringify(eventItem))
      let eventItemList = eventItem2.map(rw => {
        rw.EventId = rw.code
        rw.EventName = rw.name
        return rw
      })
      this.$set(this, "eventList", eventItemList);
    },
    async getProductEventsList(productId) {
      //获取产品
      try {
        let rsp = await productInfo({ id: productId })
        if (rsp.code == 0) {
          let productInfos = rsp.data;
          let jsonLis = JSON.parse(productInfos.ModelTSL);
          // console.log("产品的ModelTSL", jsonLis);
          if (jsonLis.events) {
            let eventTableData = jsonLis.events;
            return eventTableData
          }
        }
      } catch (error) {
        return []
      }
    },
    openAddDevice() {
      //选择设备
      this.$refs.selectDevice.openAddDevice(this.planeTargetData)
    },
    openAddProduct(){
      this.$refs.selectDevProduct.openAddProduct(this.planeTargetData)
    },
    openAddRoom(){
      this.$refs.selectDevRoom.openAddDevRoom(this.planeTargetData)
    },
    removeExpireUser(item) {
      this.devplaneFrom.expireNoticeUsers = this.devplaneFrom.expireNoticeUsers.filter(row => row.id != item)
    },
    selectFlowTemp() {
      this.$refs.selectFlowTemp.blur();
      this.$refs.flowPicker.OpenDialog();
    },
    getCreatedFocus() {
      //发起人选择开始
      this.activePensonType = 'created'
      this.$refs.selectFlowCreatedUser.blur();
      let flowCreatedUser = []
      if (this.devplaneFrom.flowCreatedUserId) {
        flowCreatedUser = [{ id: this.devplaneFrom.flowCreatedUserId, name: this.devplaneFrom.flowCreatedUserName, avatar: this.devplaneFrom.Avatar, type: "user" }];
      }
      else {
        flowCreatedUser = [];
      }
      this.$refs.flowCreatedUser.show(flowCreatedUser, "user");
    },
    getExpireFocus(index) {
      //发起人选择开始
      this.activePensonType = 'expire'
      this.activeExpireNoticesUser=index
      this.$refs.selectExpireUser.blur();
      let flowCreatedUser = []
      if (this.devplaneFrom.ExpireNotices[this.activeExpireNoticesUser]&&this.devplaneFrom.ExpireNotices[this.activeExpireNoticesUser].target && this.devplaneFrom.ExpireNotices[this.activeExpireNoticesUser].target.length > 0) {
        flowCreatedUser = this.devplaneFrom.ExpireNotices[this.activeExpireNoticesUser].target.map(row=>{
          let obj={
            id:row.userid,
            name:row.name,
            avatar:row.img,
            type: "user"
          }
          return obj
        })
      }
      else {
        flowCreatedUser = [];
      }
      this.$refs.flowCreatedUser.show(flowCreatedUser, "user");
    },
    finishTimeChoice(val) {
      //生成的定时表达式结果
      this.devplaneFrom.timerCron = val
      toCronDes(val).then(x => {
        this.$set(this.devplaneFrom, "timerCronName", x.data);
      })
    },
    handleShowCron() {//定时表达式生成初始化值设置
      this.$refs.cronTime.handleShowCron(this.devplaneFrom.timerCron)
    },
    selectLeadered(val) {
      //选择人
      if (this.activePensonType == 'created') {
        this.devplaneFrom.flowCreatedUserId = val[0].id
        this.devplaneFrom.flowCreatedUserName = val[0].name
        this.devplaneFrom.Avatar = val[0].avatar
      } else if (this.activePensonType == 'expire') {
        this.devplaneFrom.ExpireNotices[this.activeExpireNoticesUser].target = val.map(row=>{
          let obj={
            userid:row.id,
            name:row.name,
            img:row.avatar
          }
          return obj
        })
        this.devplaneFrom.ExpireNotices[this.activeExpireNoticesUser].targetUser = val.map(row => row.id)
      }
      this.$forceUpdate()
    },
    async onSelected(item, issetVal, setVal) {
      //选择流程
      this.tbloading = true
      this.devplaneFrom.flowTemplateId = item.Id
      this.devplaneFrom.flowTemplateName = item.Name
      let rsp = await getFormDetail(item.Id);
      let FlowJson=JSON.parse(rsp.data.FlowJson);
      let tformItems = JSON.parse(rsp.data.Form.FormFields);
      let newformItems = getItems(tformItems);
      this.formInit.length = 0;
      // console.log(FlowJson,'rsprsprsp流程详情');
      let FlowJsonlist=[]
      if(FlowJson.type=='ROOT'){
        FlowJsonlist=FlowJson.props.formPerms
      }
      newformItems.map(element => {
        if (element.name == "TextInput" || element.name == "TextareaInput") {
          let obj = { "id": element.id, "title": element.title, "eltype": element.name, "way": 0, "val": "" }
          if (issetVal && setVal) {
            let rowval = setVal.find(rw => rw.id == element.id)
            if (rowval) {
              obj.val = rowval.val
              obj.way = rowval.way
            }
          }
          this.formInit.push(obj);
        }
        else if (element.name == "DevicPicker") {
          this.formInit.push({ "id": element.id, "title": element.title, "eltype": element.name, "way": 1, "val": "目标设备" });
        }
        else if (element.name == "UserPicker") {
          this.formInit.push({ "id": element.id, "title": element.title, "eltype": element.name, "way": 1, "val": "提交人" });
        }
        else if (element.name == "TableList") {
          let obj = { "id": element.id, "title": element.title, "eltype": element.name, "way": 0, "val": [], "props": element.props }
          if (issetVal && setVal) {
            let rowval = setVal.find(rw => rw.id == element.id)
            if (rowval) {
              obj.way = rowval.way
              obj.val = rowval.val
            }
          }
          this.formInit.push(obj);
        }else if (element.name == "SelectInput") {
          let selobj=FlowJsonlist.find(row=>row.id==element.id)
          if(element.props&&element.props.required&&selobj&&selobj.perm=='E'){//单选必填项设置为必须初始化
            let obj = { "id": element.id, "title": element.title, "eltype": element.name, "way": 0, "val": '', "props": element.props }
            if (issetVal && setVal) {
              let rowval = setVal.find(rw => rw.id == element.id)
              if (rowval) {
                obj.val = rowval.val
                obj.way = rowval.way
              }
            }
            this.formInit.push(obj);
          }
        }else{
          let selobj=FlowJsonlist.find(row=>row.id==element.id)
          if(element.props&&element.props.required&&selobj&&selobj.perm=='E'){
            let obj={"id": element.id, "title": element.title, "eltype": element.name, "way": -1, "val": "该字段不可设为必填，为必填时无法发起任务" }
            this.formInit.push(obj);
          }
        }
      });
      this.tbloading = false;
    },
    async getUserInfoFun(id) {
      //个人信息
      try {
        let userInfo = await getUserInfo({ id: id })
        return userInfo.data.user
      } catch (error) {
        return ''
      }
    },
    async openDialog(val, isview) {
      //打开弹窗
      let iserror=false
      
      if (val) {
        this.resetForm("devplaneFrom");
        try {
          let infores = await devPlaneInfo({ id: val,showTarget:true })
          let info = infores.data
          this.devplaneFrom = {
            id: info.Id,
            flowCreatedUserId: info.FlowCreatedUserId,
            flowCreatedUserName: info.FlowCreatedUser ? info.FlowCreatedUser.RealName : '',
            Avatar: info.FlowCreatedUser ? info.FlowCreatedUser.Avatar : '',
            planeDays: info.PlaneDays,
            flowTemplateId: info.FlowTemplateId,
            flowTemplateName: info.FlowTemplateName,
            flowInitJson: info.FlowInitJson ? JSON.parse(info.FlowInitJson) : {},
            startWay: info.StartWay.toString(),
            IsFilterLeader:info.IsFilterLeader,
            ExpireNotices:info.ExpireNotices?JSON.parse(info.ExpireNotices):[],
            name: info.Name,
            remark: info.Remark,
            timerCron: info.TimerCron,
            events: info.Events ? info.Events : [],
            eventSelList: info.Events ? info.Events : [],
            targetProductId: '',
            excludeHoliday:info.ExcludeHoliday,//否排除假期
          }
          this.devplaneFrom.ExpireNotices=this.devplaneFrom.ExpireNotices.map(rw=>{
            rw.targetUser=rw.target.map(ro=>ro.userid)
            return rw
          })
          
          let formInit = JSON.parse(info.FlowInitJson)
          this.onSelected({ Id: info.FlowTemplateId, Name: info.FlowTemplateName }, true, formInit)
          this.finishTimeChoice(info.TimerCron)
          if (info.StartWay == 2) {
            this.devplaneFrom.targetProductId = info.Targets[0].TargetId;
            this.productQuery.Pids = [info.Targets[0].TargetId];
            await this.getproductList();
            await this.loadEventList();
          }
          else {
            this.planeTargetData = JSON.parse(JSON.stringify(info.Targets))
          }

          this.devplaneFrom.eventSelList = this.devplaneFrom.events.map(row => row.EventId)
          this.isReadonly = true;
          this.$nextTick(() => {
            if (isview) {
              this.isViewInfo = true
            } else {
              this.isViewInfo = false
            }
          })
        } catch (error) {
          console.log("报错了", error);
          iserror = true
        }
      } else {
        this.resetForm("devplaneFrom");
        this.devplaneFrom = {
          flowCreatedUserId: 0,
          flowCreatedUserName: '',
          expireNoticeUsers: [],
          planeDays: 0,
          flowTemplateId: '',
          flowTemplateName: '',
          flowInitJson: '',
          startWay: '0',
          IsFilterLeader:false,
          ExpireNotices:[],
          name: '',
          remark: '',
          timerCron: '',
          timerCronName: '',
          events: [],
          eventSelList: [],
          targetProductId: "",
          excludeHoliday:false,
        }
        this.formInit = []
        this.planeTargetData = []
        this.isReadonly = false;
        this.isViewInfo = false;
        await this.getproductList();

      }
      this.eventList = []
      if(this.devplaneFrom.flowCreatedUserId==0){
        this.flowCreatedUserType=1
      }else{
        this.flowCreatedUserType=2
      }
      if(this.devplaneFrom.planeDays==0){
        this.planeDaysType=0
      }else{
        this.planeDaysType=1
      }
      this.planeOpen = true
      if(iserror){//报错关闭弹窗
        this.planeOpen = false
      }
    },
    addDevicePlane() {
      this.$refs.devplaneFrom.validate((valid) => {
        if (valid) {
          try {
            this.submitLoading = true
            let devplaneSubminFrom = JSON.parse(JSON.stringify(this.devplaneFrom))
            delete devplaneSubminFrom.eventSelList
            delete devplaneSubminFrom.expireNoticeUsersId
            delete devplaneSubminFrom.expireNoticeUsersName
            delete devplaneSubminFrom.Avatar
            if(this.planeDaysType==0){
              devplaneSubminFrom.planeDays=0
            }
            if (this.devplaneFrom.startWay == 2) {
              devplaneSubminFrom.targets = [{ TargetId: devplaneSubminFrom.targetProductId, TargetType: 1 }]
              devplaneSubminFrom.events = this.devplaneFrom.eventSelList.map(row => {
                let obj = {}
                obj.EventId = row
                let eventInfo = this.eventList.find(rw => rw.EventId == row)
                if (eventInfo) {
                  obj.EventName = eventInfo.EventName
                }
                return obj
              })
            } else if (this.devplaneFrom.startWay == 1) {
              devplaneSubminFrom.targets=JSON.parse(JSON.stringify(this.planeTargetData))
              devplaneSubminFrom.events = []
              if(this.flowCreatedUserType==1){//发起人员为负责人，传0
                devplaneSubminFrom.flowCreatedUserId = 0
                devplaneSubminFrom.flowCreatedUserName = ''
              }
            } else if (this.devplaneFrom.startWay == 0) {
              devplaneSubminFrom.targets=JSON.parse(JSON.stringify(this.planeTargetData))
              devplaneSubminFrom.events = []
              devplaneSubminFrom.flowCreatedUserId = ''
              devplaneSubminFrom.flowCreatedUserName = ''
            }
            let initArr=this.formInit.filter(row=>row.way!=-1)
            let subformInit = JSON.parse(JSON.stringify(initArr))
            if(this.devplaneFrom.startWay != 0){
              
              for (let i = 0; i < this.formInit.length; i++) {
                if (this.formInit[i].way == -1){
                  this.$modal.msgError('有不可设置为必填项设置为必填，任务无法发起');
                  this.submitLoading = false
                  return
                }
              }
            }
            
            subformInit = subformInit.map(row => {
              let obj = {}
              if (row.eltype != 'TableList') {
                if(row.way!=-1){
                  obj = { "id": row.id, "title": row.title, "eltype": row.eltype, "way": row.way, "val": row.val }
                }
              } else {
                obj = { "id": row.id, "title": row.title, "eltype": row.eltype, "way": row.way, "val": row.val }
                if (row.way == 1) {
                  obj.val = JSON.stringify(row.value)
                }
              }
              return obj
            })
            if(this.devplaneFrom.startWay != 0){
              for (let i = 0; i < subformInit.length; i++) {
                if (subformInit[i].way == 1 && subformInit[i].val == "") {
                  this.$modal.msgError(subformInit[i].title + "未选择初始值");
                  this.submitLoading = false
                  return;
                }else if (subformInit[i].eltype == 'SelectInput'&&subformInit[i].way == 0 && subformInit[i].val == "") {
                  this.$modal.msgError(subformInit[i].title + "未选择初始值");
                  this.submitLoading = false
                  return;
                }
              }
            }
            devplaneSubminFrom.flowInitJson = JSON.stringify(subformInit);
            // if (this.devplaneFrom.expireNoticeWay == 1) {
            //   let expireNoticeUsers = JSON.parse(JSON.stringify(this.devplaneFrom.expireNoticeUsersId))
            //   devplaneSubminFrom.expireNoticeUsers = expireNoticeUsers.join(',')

            // } else {
            //   devplaneSubminFrom.expireNoticeUsers = ''
            //   // devplaneSubminFrom.flowTemplateId=''
            //   // devplaneSubminFrom.flowTemplateName=''
            //   // devplaneSubminFrom.flowInitJson=''
            // }
            let ExpireNotices=this.devplaneFrom.ExpireNotices.map(rows=>{
              delete rows.targetUser
              return rows
            })
            devplaneSubminFrom.ExpireNotices=JSON.stringify(ExpireNotices)
            if (devplaneSubminFrom.id) {
              this.editSubmitHandle(devplaneSubminFrom)
            } else {
              this.submitHandle(devplaneSubminFrom)
            }

          } catch (error) {
            this.submitLoading = false
            
          }
        }

      })
    },
    submitHandle(form) {
      //保存提交
      devPlaneAdd(form).then(res => {
        this.$modal.msgSuccess("保存成功");
        this.$emit('reloadList')
        this.planeOpen = false
        this.submitLoading = false
      }).catch(err => {
        this.submitLoading = false
      })
    },
    editSubmitHandle(form) {
      //保存提交
      devPlaneEdit(form).then(res => {
        this.$modal.msgSuccess("保存成功");
        this.$emit('reloadList')
        this.planeOpen = false
        this.submitLoading = false
      }).catch(err => {
        this.submitLoading = false
      })
    }
  },
};
</script>
<style lang="less" scoped>
::v-deep .groupFrom {
  .el-form-item {
    width: 100%;

    .el-form-item__content {
      width: calc(100% - 126px);

      .el-select {
        width: 100%;
      }

      .el-input {
        width: 100%;
      }
    }
  }
}

.box-row {
  .bx-hd {
    font-size: 16px;
    color: #333;
    background-color: rgb(249, 250, 252);
    padding: 0 15px;
    height: 48px;
    display: flex;
    justify-content: space-between;
    flex-direction: row;
    align-items: center;
  }

  .bx-bd {
    padding: 15px 0px;

    .el-input__suffix {
      display: flex;
      align-items: center;
    }
  }
}

.airCompressors {
  display: flex;
  justify-content: left;
  width: 100%;
  flex-wrap: wrap;
  align-items: center;
  margin-right: -15px;
  margin-top: 10px;
  //margin-top: -10px;
  .airCompressors-item:nth-child(3n) {
    margin-right: 0;
  }

  .airCompressors-item {
    background: rgba(249, 250, 252, 1);
    width: calc(33% - 10px);
    // height:180px;
    border-radius: 10px;
    margin-bottom: 15px;
    display: flex;
    margin-right: 14px;
    flex-direction: column;
    padding: 15px 20px;
    box-sizing: border-box;
    position: relative;

    .airCompressors-item-bottom {
      color: rgba(153, 153, 153, 1);
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-top: 15px;
      font-size: 13px;

      .airCompressors-item-bottom-left {
        width: 48%;
        border-right: 1px solid rgba(234, 234, 234, 1);
      }

      .airCompressors-item-bottom-right {
        width: 48%;
        text-align: center;
      }
    }

    .airCompressors-item-top {
      display: flex;
      //border-bottom: 1px solid rgba(234, 234, 234, 1);
      // padding-bottom: 15px;
      justify-content: space-between;

      .airCompressors-item-top-see {
        // margin-left: auto;
        position: absolute;
        bottom: 10px;
        right: 20px;
        align-self: flex-end;
        color: rgba(153, 153, 153, 1);
        font-size: 13px;
        display: flex;
        align-items: center;
        cursor: pointer;

        .airCompressors-item-top-see-txt {
          display: inline-block;
          margin-left: 6px;
        }
      }

      .airCompressors-item-top-see:hover {
        color: rgba(53, 114, 255, 1);
      }

      .airCompressors-item-top-left {
        display: flex;
        align-items: center;

        .airCompressors-item-top-left-text {
          line-height: 12px;
          margin-left: 20px;

          .number {
            color: rgba(153, 153, 153, 1);
            font-size: 14px;
          }

          .title {
            font-weight: bold;
            line-height: 25px;
          }

          .onLine {
            display: flex;
            font-size: 13px;
            align-items: center;

            .yuan {
              display: inline-block;
              width: 8px;
              height: 8px;
              border-radius: 50%;

              margin-right: 6px;
            }
          }

          .active1 {
            background: rgba(13, 179, 166, 1);
          }

          .on1 {
            background: rgba(186, 186, 186, 1);
          }

          .active {
            color: rgba(13, 179, 166, 1);
          }

          .on {
            color: rgba(102, 102, 102, 1);
          }
        }

        .airCompressors-item-top-logo {
          width: 75px;
          height: 75px;
          border-radius: 10px;
        }
      }
    }
  }
}
</style>