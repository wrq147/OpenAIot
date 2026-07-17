<template>
  <div>
    <el-dialog
      title="添加计划"
      :visible.sync="planeOpen"
      center
      width="900px"
      top="10px"
      :close-on-click-modal="false"
      :destroy-on-close="true"
      class="plan-dialog"
    >
      <el-form
        :model="devplaneFrom"
        ref="devplaneFrom"
        :rules="devplaneRules"
        label-position="left"
        class="plan-form"
        label-width="120px"
      >
        <!-- 基础信息区域 -->
        <div class="form-block">
          <div class="block-title">基础信息</div>
          <el-form-item label="计划名称" prop="name">
            <el-input
              v-model="devplaneFrom.name"
              placeholder="请输入计划名称"
              :disabled="isViewInfo"
            ></el-input>
          </el-form-item>

          <el-form-item label="任务触发方式" prop="taskType">
            <el-radio-group v-model="devplaneFrom.startWay" :disabled="isReadonly || isViewInfo">
              <el-radio label="0">手动发起</el-radio>
              <el-radio label="1">定时发起</el-radio>
              <el-radio label="2">设备事件触发</el-radio>
            </el-radio-group>
          </el-form-item>
        </div>

        <!-- 设备事件触发专属区块 -->
        <template v-if="devplaneFrom.startWay == 2">
          <div class="form-block">
            <div class="block-title">事件触发配置</div>
            <el-form-item label="目标产品">
              <el-select
                :disabled="isReadonly || isViewInfo"
                v-model="devplaneFrom.targetProductId"
                filterable
                remote
                reserve-keyword
                clearable
                placeholder="输入产品关键词搜索"
                :remote-method="remoteMethod"
                :loading="prodloading"
                @change="chgProd"
              >
                <el-option
                  v-for="item in productLists"
                  :key="item.Id"
                  :label="item.ProductName"
                  :value="item.Id"
                ></el-option>
              </el-select>
            </el-form-item>
            <el-form-item label="监听设备事件" prop="eventSelList">
              <el-select
                v-model="devplaneFrom.eventSelList"
                multiple
                placeholder="可多选事件"
                :disabled="isReadonly || isViewInfo"
              >
                <el-option
                  v-for="item in eventList"
                  :key="item.EventId"
                  :label="item.EventName"
                  :value="item.EventId"
                ></el-option>
              </el-select>
            </el-form-item>
          </div>
        </template>

        <!-- 手动/定时 目标设备/产品/房间表格区块 -->
        <template v-else>
          <div class="form-block">
            <div class="block-title">执行目标列表</div>
            <div class="target-btn-group" v-if="!isViewInfo">
              <el-button type="primary" plain size="small" @click="openAddDevice">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                加入设备
              </el-button>
              <el-button type="primary" plain size="small" @click="openAddProduct">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                加入产品
              </el-button>
              <el-button type="primary" plain size="small" @click="openAddRoom">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                加入房间
              </el-button>
            </div>
            <el-table
              border
              :data="planeTargetData"
              max-height="300"
              class="common-table"
            >
              <el-table-column
                label="名称"
                prop="TargetName"
                align="center"
                show-overflow-tooltip
              ></el-table-column>
              <el-table-column label="类型" prop="TargetType" align="center">
                <template slot-scope="scope">
                  <el-tag size="small" type="info">
                    {{ scope.row.TargetType == 0 ? '设备' : scope.row.TargetType == 1 ? '产品' : '房间' }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column
                label="操作"
                align="center"
                width="120"
                v-if="!isViewInfo"
              >
                <template slot-scope="scope">
                  <el-button text type="danger" size="mini" @click="handleTargetDelete(scope.row, scope.$index)">
                    删除
                  </el-button>
                </template>
              </el-table-column>
            </el-table>
            <div class="empty-tip" v-if="planeTargetData.length === 0">暂无执行目标，请添加设备/产品/房间</div>
          </div>
        </template>

        <!-- 定时配置 -->
        <template v-if="devplaneFrom.startWay == '1'">
          <div class="form-block">
            <div class="block-title">定时配置</div>
            <el-form-item label="定时时间" prop="timerCron">
              <el-input
                v-model="devplaneFrom.timerCronName"
                readonly
                placeholder="点击右侧按钮设置定时规则"
              >
                <template slot="append">
                  <el-button type="primary" size="small" @click="handleShowCron" :disabled="isViewInfo">
                    设置时间 <i class="el-icon-time el-icon--right"></i>
                  </el-button>
                </template>
              </el-input>
            </el-form-item>
            <el-form-item label="排除节假日" prop="excludeHoliday">
              <el-radio-group v-model="devplaneFrom.excludeHoliday" :disabled="isViewInfo">
                <el-radio :label="true">是</el-radio>
                <el-radio :label="false">否</el-radio>
              </el-radio-group>
            </el-form-item>
          </div>
        </template>

        <!-- 执行天数、发起人配置 -->
        <div class="form-block">
          <div class="block-title">执行规则</div>
          <el-form-item label="执行天数限制" prop="planeDays">
            <el-radio-group v-model="planeDaysType" :disabled="isViewInfo" @change="planeDaysTypeChange">
              <el-radio :label="0">不限制</el-radio>
              <el-radio :label="1">指定天数</el-radio>
            </el-radio-group>
            <el-input-number
              v-model="devplaneFrom.planeDays"
              :step="1"
              step-strictly
              :min="1"
              :disabled="isViewInfo || planeDaysType !== 1"
              class="ml-20"
            ></el-input-number>
            <span class="form-tip ml-10">天</span>
          </el-form-item>

          <el-form-item label="流程发起人" prop="flowCreatedUserId" v-if="devplaneFrom.startWay != '0'">
            <el-radio-group v-model="flowCreatedUserType" :disabled="isViewInfo">
              <el-radio :label="1">设备负责人</el-radio>
              <el-radio :label="2">指定人员</el-radio>
            </el-radio-group>
            <el-select
              filterable
              allow-create
              default-first-option
              v-model="devplaneFrom.flowCreatedUserName"
              ref="selectFlowCreatedUser"
              placeholder="点击选择发起人"
              @focus="getCreatedFocus"
              :disabled="isViewInfo || flowCreatedUserType !== 2"
              class="ml-20 w-260"
            ></el-select>
          </el-form-item>

          <el-form-item label="计划是否公开" prop="IsFilterLeader">
            <el-radio-group v-model="devplaneFrom.IsFilterLeader" :disabled="isViewInfo">
              <el-radio :label="false">公开</el-radio>
              <el-radio :label="true">仅负责人可见</el-radio>
            </el-radio-group>
          </el-form-item>
        </div>

        <!-- 超期提醒配置 -->
        <div class="form-block">
          <div class="block-title flex-between">
            <span>超期提醒规则</span>
            <el-button type="primary" plain size="small" @click="setAddExpireNotices" v-if="!isViewInfo">
              <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i> 新增提醒
            </el-button>
          </div>
          <el-table
            border
            :data="devplaneFrom.ExpireNotices"
            max-height="260"
            class="common-table"
          >
            <el-table-column label="提前天数提醒" prop="day" align="center" width="140">
              <template slot-scope="scope">
                <el-input-number
                  v-model="scope.row.day"
                  :step="1"
                  step-strictly
                  :min="0"
                  size="mini"
                  :disabled="isViewInfo"
                ></el-input-number>
              </template>
            </el-table-column>
            <el-table-column label="提醒对象类型" prop="way" align="center" width="140">
              <template slot-scope="scope">
                <el-select v-model="scope.row.way" size="mini" :disabled="isViewInfo">
                  <el-option label="指定人员" value="1"></el-option>
                  <el-option label="设备负责人" value="2"></el-option>
                </el-select>
              </template>
            </el-table-column>
            <el-table-column label="消息合并方式" prop="ccway" align="center" width="140">
              <template slot-scope="scope">
                <el-select v-model="scope.row.ccway" size="mini" :disabled="isViewInfo">
                  <el-option label="单独提醒" value="0"></el-option>
                  <el-option label="合并汇总" value="1"></el-option>
                </el-select>
              </template>
            </el-table-column>
            <el-table-column label="指定提醒人员" align="center">
              <template slot-scope="scope">
                <el-select
                  filterable
                  allow-create
                  multiple
                  v-model="scope.row.targetUser"
                  ref="selectExpireUser"
                  placeholder="选择人员"
                  @focus="getExpireFocus(scope.$index)"
                  @remove-tag="removeExpireUser"
                  :disabled="isViewInfo || scope.row.way !== '1'"
                >
                  <el-option
                    v-for="item in scope.row.target"
                    :key="item.userid"
                    :label="item.name"
                    :value="item.userid"
                  ></el-option>
                </el-select>
              </template>
            </el-table-column>
            <el-table-column label="操作" align="center" width="80" v-if="!isViewInfo">
              <template slot-scope="scope">
                <el-button text type="danger" size="mini" @click="handleExpireNoticesDelete(scope.row, scope.$index)">删除</el-button>
              </template>
            </el-table-column>
          </el-table>
          <div class="empty-tip" v-if="devplaneFrom.ExpireNotices.length === 0">未配置超期提醒</div>
        </div>

        <!-- 流程与表单初始化 -->
        <div class="form-block">
          <div class="block-title">流程配置</div>
          <el-form-item label="绑定计划流程" prop="flowTemplateId">
            <el-select
              filterable
              allow-create
              default-first-option
              v-model="devplaneFrom.flowTemplateName"
              ref="selectFlowTemp"
              placeholder="点击选择流程模板"
              @focus="selectFlowTemp"
              :disabled="isReadonly || isViewInfo"
            ></el-select>
          </el-form-item>

          <div class="form-sub-block" v-show="devplaneFrom.flowTemplateName !== ''">
            <div class="sub-title">流程表单初始化</div>
            <el-table
              border
              v-loading="tbloading"
              :data="formInit"
              row-key="id"
              class="common-table"
            >
              <el-table-column prop="title" label="表单字段" align="center" width="240">
                <template slot-scope="scope">
                  <span v-if="scope.row.way == -1 && devplaneFrom.startWay != 0 || scope.row.way != -1">
                    {{ scope.row.title }}
                  </span>
                </template>
              </el-table-column>
              <el-table-column label="值类型" align="center" width="130">
                <template slot-scope="scope">
                  <el-select
                    v-model="scope.row.way"
                    size="mini"
                    @change="changeLoadVal($event, scope.row, scope.$index)"
                    :disabled="isViewInfo"
                    v-if="scope.row.way != -1"
                  >
                    <el-option label="自定义" :value="0" v-if="scope.row.eltype != 'DevicPicker' && scope.row.eltype != 'UserPicker'"></el-option>
                    <el-option label="系统值" :value="1" v-if="scope.row.eltype != 'TableList' && scope.row.eltype != 'SelectInput'"></el-option>
                  </el-select>
                  <el-text type="danger" v-if="scope.row.way == -1 && devplaneFrom.startWay != 0">必填锁定</el-text>
                </template>
              </el-table-column>
              <el-table-column label="初始值" align="center">
                <template v-slot:default="scope">
                  <!-- 普通输入框 -->
                  <template v-if="scope.row.eltype != 'TableList' && scope.row.eltype != 'SelectInput' && scope.row.way != -1">
                    <el-input
                      v-if="scope.row.way == 0"
                      v-model="scope.row.val"
                      placeholder="自定义输入内容"
                      size="mini"
                      :disabled="isViewInfo"
                    ></el-input>
                    <el-select v-else v-model="scope.row.val" size="mini" :disabled="isViewInfo">
                      <el-option label="计划类型" value="计划类型" v-if="['TextInput','TextareaInput'].includes(scope.row.eltype)"></el-option>
                      <el-option label="任务备注" value="任务备注" v-if="['TextInput','TextareaInput'].includes(scope.row.eltype)"></el-option>
                      <el-option label="设备名称" value="设备名称" v-if="['TextInput','TextareaInput'].includes(scope.row.eltype)"></el-option>
                      <el-option label="设备编号" value="设备编号" v-if="['TextInput','TextareaInput'].includes(scope.row.eltype)"></el-option>
                      <el-option label="通讯编号" value="通讯编号" v-if="['TextInput','TextareaInput'].includes(scope.row.eltype)"></el-option>
                      <el-option label="同名参数" value="同名参数" v-if="['TextInput','TextareaInput'].includes(scope.row.eltype)"></el-option>
                      <el-option label="告警级别" value="告警级别" v-if="['TextInput','TextareaInput'].includes(scope.row.eltype)"></el-option>
                      <el-option label="目标设备" value="目标设备" v-if="scope.row.eltype == 'DevicPicker'"></el-option>
                      <el-option label="提交人" value="提交人" v-if="scope.row.eltype == 'UserPicker'"></el-option>
                      <el-option label="提交人部门" value="提交人部门" v-if="['TextInput','TextareaInput'].includes(scope.row.eltype)"></el-option>
                      <el-option label="提交人姓名" value="提交人姓名" v-if="['TextInput','TextareaInput'].includes(scope.row.eltype)"></el-option>
                      <el-option label="房间名称" value="房间名称" v-if="['TextInput','TextareaInput'].includes(scope.row.eltype)"></el-option>
                      <el-option label="房间分类名" value="房间分类名" v-if="['TextInput','TextareaInput'].includes(scope.row.eltype)"></el-option>
                      <el-option label="设备责任人" value="设备责任人" v-if="scope.row.eltype == 'UserPicker'"></el-option>
                      <el-option label="设备责任与协作人" value="设备责任与协作人" v-if="scope.row.eltype == 'UserPicker'"></el-option>
                    </el-select>
                  </template>
                  <!-- TableList组件 -->
                  <template v-else-if="scope.row.eltype == 'TableList' && scope.row.way != -1">
                    <TableList v-model="scope.row.val" mode="mode" v-bind="scope.row.props" :disabled="isViewInfo" />
                  </template>
                  <!-- 下拉选择组件 -->
                  <template v-else-if="scope.row.eltype == 'SelectInput' && scope.row.way == 0">
                    <el-select v-model="scope.row.val" size="mini" placeholder="请选择" :disabled="isViewInfo">
                      <template v-if="scope.row.props && scope.row.props.options">
                        <el-option
                          v-for="(its,ox) in scope.row.props.options"
                          :key="'sel'+ox"
                          :label="its"
                          :value="its"
                        ></el-option>
                      </template>
                    </el-select>
                  </template>
                  <!-- 不可自定义必填提示 -->
                  <template v-else-if="devplaneFrom.startWay != 0 && scope.row.way == -1">
                    <span class="text-red">*该字段表单必填，无法自定义初始化</span>
                  </template>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </div>

        <!-- 备注 -->
        <div class="form-block">
          <div class="block-title">补充备注</div>
          <el-form-item label="备注说明" prop="remark">
            <el-input
              type="textarea"
              v-model="devplaneFrom.remark"
              rows="3"
              placeholder="填写计划相关备注信息"
              :disabled="isViewInfo"
            ></el-input>
          </el-form-item>
        </div>
      </el-form>

      <!-- 底部按钮 -->
      <div slot="footer" class="dialog-footer" v-if="!isViewInfo">
        <el-button size="medium" @click="planeOpen = false">取消</el-button>
        <el-button type="primary" size="medium" @click="addDevicePlane" v-loading="submitLoading">
          确定保存
        </el-button>
      </div>
    </el-dialog>

    <!-- 弹窗子组件 -->
    <selectDevice ref="selectDevice" @addPlaneDevice="addPlaneDevice"></selectDevice>
    <selectDevRoom ref="selectDevRoom" @addDevRoom="addDevRoom"></selectDevRoom>
    <selectDevProduct ref="selectDevProduct" @addPlaneDevProduct="addPlaneDevProduct"></selectDevProduct>
    <FlowPicker ref="flowPicker" @selected="onSelected"></FlowPicker>
    <org-picker
      :multiple="activePensonType == 'expire'"
      ref="flowCreatedUser"
      @ok="selectLeadered"
      :isMulLimit="activePensonType == 'expire'"
      :limitNum="5"
    />
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
import { productInfo } from "@/api/rules/productModel";
import { getUserInfo } from '@/api/login'
import TableList from '@/views/flowable/common/form/components/TableList.vue'
import { checkPermi } from '@/utils/permission.js';

export default {
  name: 'AdminUiDevplaneAdd',
  components: { OrgPicker, FlowPicker, cronTime, TableList, selectDevice, selectDevProduct, selectDevRoom },
  data() {
    return {
      activeExpireNoticesUser: null,
      planeDaysType: 0,
      flowCreatedUserType: 1,
      isViewInfo: false,
      isReadonly: false,
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
        excludeHoliday: false,
        ExpireNotices: [],
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
        flowCreatedUserId: [{ required: true, trigger: "blur", message: "请选择发起人" }],
        flowTemplateId: [{ required: true, trigger: ["change"], message: "请选择计划流程" }],
        eventSelList: [{ required: true, trigger: "change", message: "请选择设备事件" }],
        planeDays: [{ required: true, trigger: "change", message: "请输入计划执行天数" }],
      },
      activePensonType: 'created',
      formInit: [],
      productLists: [],
      submitLoading: false,
      productQuery: {
        pageNum: 1,
        pageSize: 300,
        Name: "",
        Pids: []
      },
      prodloading: false,
      planeTargetData: [],
    };
  },
  computed: {
    IsFactory() {
      return checkPermi(['/AgentMan/'])
    }
  },
  mounted() { },
  methods: {
    handleExpireNoticesDelete(row, index) {
      this.devplaneFrom.ExpireNotices.splice(index, 1)
    },
    setAddExpireNotices() {
      this.devplaneFrom.ExpireNotices.push({ day: 1, way: '1', ccway: '0', target: [], targetUser: [] })
    },
    planeDaysTypeChange(val) {
      if (val == 1 && this.devplaneFrom.planeDays == 0) {
        this.devplaneFrom.planeDays = 1
      }
    },
    handleTargetDelete(row, inx) {
      this.planeTargetData.splice(inx, 1)
    },
    async remoteMethod(query) {
      this.productQuery.Name = query !== '' ? query : undefined;
      await this.getproductList();
    },
    async getproductList() {
      this.prodloading = true;
      const res = await myProductList(this.productQuery)
      if (res.code === 0 && res.data?.List) this.productLists = res.data.List;
      this.prodloading = false;
    },
    changeLoadVal(event, row, index) {
      this.formInit[index].val = event === 1 && row.eltype === "TableList" ? {} : ''
    },
    addPlaneDevice(sellist) {
      this.planeTargetData = [...this.planeTargetData.filter(r => r.TargetType !== 0), ...sellist]
    },
    addPlaneDevProduct(sellist) {
      this.planeTargetData = [...this.planeTargetData.filter(r => r.TargetType !== 1), ...sellist]
    },
    addDevRoom(sellist) {
      this.planeTargetData = [...this.planeTargetData.filter(r => r.TargetType !== 2), ...sellist]
    },
    async chgProd() {
      await this.loadEventList();
    },
    async loadEventList() {
      if (!this.devplaneFrom.targetProductId) return;
      const eventItem = await this.getProductEventsList(this.devplaneFrom.targetProductId)
      this.eventList = eventItem.map(rw => ({ EventId: rw.code, EventName: rw.name }))
    },
    async getProductEventsList(productId) {
      try {
        const rsp = await productInfo({ id: productId })
        if (rsp.code === 0) {
          const jsonLis = JSON.parse(rsp.data.ModelTSL);
          return jsonLis.events || []
        }
        return []
      } catch {
        return []
      }
    },
    openAddDevice() { this.$refs.selectDevice.openAddDevice(this.planeTargetData) },
    openAddProduct() { this.$refs.selectDevProduct.openAddProduct(this.planeTargetData) },
    openAddRoom() { this.$refs.selectDevRoom.openAddDevRoom(this.planeTargetData) },
    removeExpireUser(item) {
      this.devplaneFrom.expireNoticeUsers = this.devplaneFrom.expireNoticeUsers.filter(row => row.id != item)
    },
    selectFlowTemp() {
      this.$refs.selectFlowTemp.blur();
      this.$refs.flowPicker.OpenDialog();
    },
    getCreatedFocus() {
      this.activePensonType = 'created'
      this.$refs.selectFlowCreatedUser.blur();
      const list = this.devplaneFrom.flowCreatedUserId
        ? [{ id: this.devplaneFrom.flowCreatedUserId, name: this.devplaneFrom.flowCreatedUserName, avatar: this.devplaneFrom.Avatar, type: "user" }]
        : []
      this.$refs.flowCreatedUser.show(list, "user");
    },
    getExpireFocus(index) {
      this.activePensonType = 'expire'
      this.activeExpireNoticesUser = index
      this.$refs.selectExpireUser.blur();
      const row = this.devplaneFrom.ExpireNotices[index]
      const list = row?.target?.length
        ? row.target.map(r => ({ id: r.userid, name: r.name, avatar: r.img, type: "user" }))
        : []
      this.$refs.flowCreatedUser.show(list, "user");
    },
    finishTimeChoice(val) {
      this.devplaneFrom.timerCron = val
      toCronDes(val).then(x => this.devplaneFrom.timerCronName = x.data)
    },
    handleShowCron() {
      this.$refs.cronTime.handleShowCron(this.devplaneFrom.timerCron)
    },
    selectLeadered(val) {
      if (this.activePensonType == 'created') {
        this.devplaneFrom.flowCreatedUserId = val[0].id
        this.devplaneFrom.flowCreatedUserName = val[0].name
        this.devplaneFrom.Avatar = val[0].avatar
      } else {
        const targetList = val.map(row => ({ userid: row.id, name: row.name, img: row.avatar }))
        this.devplaneFrom.ExpireNotices[this.activeExpireNoticesUser].target = targetList
        this.devplaneFrom.ExpireNotices[this.activeExpireNoticesUser].targetUser = val.map(row => row.id)
      }
      this.$forceUpdate()
    },
    async onSelected(item, issetVal, setVal) {
      this.tbloading = true
      this.devplaneFrom.flowTemplateId = item.Id
      this.devplaneFrom.flowTemplateName = item.Name
      const rsp = await getFormDetail(item.Id);
      const FlowJson = JSON.parse(rsp.data.FlowJson);
      const tformItems = JSON.parse(rsp.data.Form.FormFields);
      const newformItems = getItems(tformItems);
      this.formInit = [];
      const FlowJsonlist = FlowJson.type === 'ROOT' ? FlowJson.props.formPerms : [];
      newformItems.forEach(element => {
        const selobj = FlowJsonlist.find(row => row.id == element.id)
        const isRequiredLock = element.props?.required && selobj?.perm == 'E'
        if (element.name == "TextInput" || element.name == "TextareaInput") {
          const obj = { id: element.id, title: element.title, eltype: element.name, way: 0, val: "" }
          if (issetVal && setVal) {
            const rowval = setVal.find(rw => rw.id == element.id)
            if (rowval) { obj.val = rowval.val; obj.way = rowval.way }
          }
          this.formInit.push(obj);
        } else if (element.name == "DevicPicker") {
          this.formInit.push({ id: element.id, title: element.title, eltype: element.name, way: 1, val: "目标设备" });
        } else if (element.name == "UserPicker") {
          this.formInit.push({ id: element.id, title: element.title, eltype: element.name, way: 1, val: "提交人" });
        } else if (element.name == "TableList") {
          const obj = { id: element.id, title: element.title, eltype: element.name, way: 0, val: [], props: element.props }
          if (issetVal && setVal) {
            const rowval = setVal.find(rw => rw.id == element.id)
            if (rowval) { obj.way = rowval.way; obj.val = rowval.val }
          }
          this.formInit.push(obj);
        } else if (element.name == "SelectInput") {
          if (isRequiredLock) {
            const obj = { id: element.id, title: element.title, eltype: element.name, way: 0, val: '', props: element.props }
            if (issetVal && setVal) {
              const rowval = setVal.find(rw => rw.id == element.id)
              if (rowval) { obj.val = rowval.val; obj.way = rowval.way }
            }
            this.formInit.push(obj);
          }
        } else {
          if (isRequiredLock) {
            this.formInit.push({ id: element.id, title: element.title, eltype: element.name, way: -1, val: "该字段不可设为必填，为必填时无法发起任务" });
          }
        }
      });
      this.tbloading = false;
    },
    async openDialog(val, isview) {
      let iserror = false
      this.resetForm("devplaneFrom");
      if (val) {
        try {
          const infores = await devPlaneInfo({ id: val, showTarget: true })
          const info = infores.data
          this.devplaneFrom = {
            id: info.Id,
            flowCreatedUserId: info.FlowCreatedUserId,
            flowCreatedUserName: info.FlowCreatedUser?.RealName || '',
            Avatar: info.FlowCreatedUser?.Avatar || '',
            planeDays: info.PlaneDays,
            flowTemplateId: info.FlowTemplateId,
            flowTemplateName: info.FlowTemplateName,
            flowInitJson: info.FlowInitJson ? JSON.parse(info.FlowInitJson) : {},
            startWay: info.StartWay.toString(),
            IsFilterLeader: info.IsFilterLeader,
            ExpireNotices: info.ExpireNotices ? JSON.parse(info.ExpireNotices) : [],
            name: info.Name,
            remark: info.Remark,
            timerCron: info.TimerCron,
            events: info.Events || [],
            eventSelList: info.Events || [],
            targetProductId: "",
            excludeHoliday: info.ExcludeHoliday,
          }
          this.devplaneFrom.ExpireNotices = this.devplaneFrom.ExpireNotices.map(rw => {
            rw.targetUser = rw.target.map(ro => ro.userid)
            return rw
          })
          const formInit = JSON.parse(info.FlowInitJson)
          await this.onSelected({ Id: info.FlowTemplateId, Name: info.FlowTemplateName }, true, formInit)
          this.finishTimeChoice(info.TimerCron)
          if (info.StartWay == 2) {
            this.devplaneFrom.targetProductId = info.Targets[0].TargetId;
            this.productQuery.Pids = [info.Targets[0].TargetId];
            await this.getproductList();
            await this.loadEventList();
          } else {
            this.planeTargetData = JSON.parse(JSON.stringify(info.Targets))
          }
          this.devplaneFrom.eventSelList = this.devplaneFrom.events.map(row => row.EventId)
          this.isReadonly = true;
          this.$nextTick(() => this.isViewInfo = !!isview)
        } catch (error) {
          console.error(error);
          iserror = true
        }
      } else {
        this.devplaneFrom = {
          flowCreatedUserId: 0,
          flowCreatedUserName: '',
          expireNoticeUsers: [],
          planeDays: 0,
          flowTemplateId: '',
          flowTemplateName: '',
          flowInitJson: '',
          startWay: '0',
          IsFilterLeader: false,
          ExpireNotices: [],
          name: '',
          remark: '',
          timerCron: '',
          timerCronName: '',
          events: [],
          eventSelList: [],
          targetProductId: "",
          excludeHoliday: false,
        }
        this.formInit = []
        this.planeTargetData = []
        this.isReadonly = false;
        this.isViewInfo = false;
        await this.getproductList();
      }
      this.eventList = []
      this.flowCreatedUserType = this.devplaneFrom.flowCreatedUserId == 0 ? 1 : 2
      this.planeDaysType = this.devplaneFrom.planeDays == 0 ? 0 : 1
      this.planeOpen = !iserror
    },
    addDevicePlane() {
      this.$refs.devplaneFrom.validate(async (valid) => {
        if (!valid) return;
        this.submitLoading = true
        try {
          const devplaneSubminFrom = JSON.parse(JSON.stringify(this.devplaneFrom))
          delete devplaneSubminFrom.eventSelList
          delete devplaneSubminFrom.Avatar
          devplaneSubminFrom.planeDays = this.planeDaysType == 0 ? 0 : devplaneSubminFrom.planeDays
          if (this.devplaneFrom.startWay == 2) {
            devplaneSubminFrom.targets = [{ TargetId: devplaneSubminFrom.targetProductId, TargetType: 1 }]
            devplaneSubminFrom.events = this.devplaneFrom.eventSelList.map(row => {
              const info = this.eventList.find(rw => rw.EventId == row)
              return { EventId: row, EventName: info?.EventName || '' }
            })
          } else {
            devplaneSubminFrom.targets = JSON.parse(JSON.stringify(this.planeTargetData))
            devplaneSubminFrom.events = []
            if (this.devplaneFrom.startWay == 1) {
              devplaneSubminFrom.flowCreatedUserId = this.flowCreatedUserType == 1 ? 0 : devplaneSubminFrom.flowCreatedUserId
              if (this.flowCreatedUserType == 1) devplaneSubminFrom.flowCreatedUserName = ''
            } else if (this.devplaneFrom.startWay == 0) {
              devplaneSubminFrom.flowCreatedUserId = ''
              devplaneSubminFrom.flowCreatedUserName = ''
            }
          }
          const initArr = this.formInit.filter(row => row.way != -1)
          if (this.devplaneFrom.startWay != 0) {
            const lockItem = this.formInit.find(i => i.way == -1)
            if (lockItem) {
              this.$modal.msgError('存在表单必填锁定字段，任务无法发起');
              this.submitLoading = false;
              return
            }
            for (const item of initArr) {
              if (item.way == 1 && !item.val) {
                this.$modal.msgError(`${item.title} 未选择系统初始值`);
                this.submitLoading = false;
                return;
              }
              if (item.eltype == 'SelectInput' && item.way == 0 && !item.val) {
                this.$modal.msgError(`${item.title} 未选择初始值`);
                this.submitLoading = false;
                return;
              }
            }
          }
          const subformInit = initArr.map(row => {
            const obj = { id: row.id, title: row.title, eltype: row.eltype, way: row.way, val: row.val }
            if (row.eltype === 'TableList' && row.way === 1) obj.val = JSON.stringify(row.val)
            return obj
          })
          devplaneSubminFrom.flowInitJson = JSON.stringify(subformInit);
          const expireSave = this.devplaneFrom.ExpireNotices.map(item => {
            const copy = { ...item }
            delete copy.targetUser
            return copy
          })
          devplaneSubminFrom.ExpireNotices = JSON.stringify(expireSave)
          if (devplaneSubminFrom.id) await this.editSubmitHandle(devplaneSubminFrom)
          else await this.submitHandle(devplaneSubminFrom)
        } catch (err) {
          console.error(err)
          this.submitLoading = false
        }
      })
    },
    submitHandle(form) {
      return devPlaneAdd(form).then(() => {
        this.$modal.msgSuccess("保存成功");
        this.$emit('reloadList')
        this.planeOpen = false
      }).finally(() => this.submitLoading = false)
    },
    editSubmitHandle(form) {
      return devPlaneEdit(form).then(() => {
        this.$modal.msgSuccess("修改成功");
        this.$emit('reloadList')
        this.planeOpen = false
      }).finally(() => this.submitLoading = false)
    }
  },
};
</script>

<style lang="less" scoped>
// 全局通用间距
.ml-20 { margin-left: 20px; }
.ml-10 { margin-left: 10px; }
.w-260 { width: 260px !important; }
.text-red { color: #f56c6c; }
.flex-between { display: flex; justify-content: space-between; align-items: center; }

// 弹窗外层
::v-deep .plan-dialog .el-dialog__body {
  padding: 20px 24px;
  max-height: 80vh;
  overflow-y: auto;
}

// 表单整体
.plan-form {
  .el-form-item {
    margin-bottom: 18px;
  }
}

// 模块区块
.form-block {
  background: #f9fafb;
  border-radius: 8px;
  padding: 16px 20px;
  margin-bottom: 16px;
  border: 1px solid #e5e7eb;
}
.block-title {
  font-size: 15px;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 14px;
  padding-bottom: 8px;
  border-bottom: 1px solid #e5e7eb;
}
.form-sub-block {
  margin-top: 12px;
  padding-top: 12px;
  border-top: 1px dashed #d1d5db;
}
.sub-title {
  font-size: 14px;
  font-weight: 500;
  color: #4b5563;
  margin-bottom: 10px;
}

// 按钮组
.target-btn-group {
  margin-bottom: 12px;
  display: flex;
  gap: 10px;
}

// 通用表格样式
.common-table {
  background: #fff;
  border-radius: 6px;
  ::v-deep .el-table__header {
    th {
      background: #f2f3f5;
      color: #333;
    }
  }
  ::v-deep .el-table__row:hover > td {
    background: #f7f8fa;
  }
  ::v-deep .el-input-number {
    width: 100%;
  }
}

// 空数据提示
.empty-tip {
  color: #9ca3af;
  font-size: 13px;
  padding: 10px 0;
  text-align: center;
}

// 底部按钮区
.dialog-footer {
  text-align: right;
  padding-top: 12px;
}

// 表单提示文字
.form-tip {
  color: #6b7280;
  font-size: 13px;
}
</style>