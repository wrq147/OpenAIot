<template>
  <div style="padding: 20px 20px 0 20px" id="big_con">
    <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 136px)','padding-top':'10px' }">
      <div class="header" style="border-top: none; padding: 0; border-bottom: 1px solid #dadada">
        <el-menu :default-active="activeRules" active-text-color="#409eff" class="el-menu-demo shejiqi" mode="horizontal">
          <el-menu-item index="rulesManage" @click="rulesSelect('rulesManage')">规则</el-menu-item>
          <el-menu-item index="rulesGroup" @click="rulesSelect('rulesGroup')">分组</el-menu-item>
        </el-menu>
      </div>
      <div class="from_con" id="from_con" v-show="showSearch" v-if="activeRules == 'rulesManage'">
        <el-form :model="queryParams" ref="queryForm" :inline="true" class="biaodan">
          <el-form-item label="规则分组" prop="status">
            <treeselect
              class="groupSet"
              style="width: 150px"
              v-model="queryParams.GroupId"
              :options="groupTreeList"
              :show-count="true"
              :normalizer="normalizer"
              placeholder="请选择规则分组"
              @select="selectGroupTree"
            />
          </el-form-item>
          <el-form-item label="关键字搜索" prop="key">
            <el-input v-model="queryParams.key" placeholder="请输入搜索关键字" clearable></el-input>
          </el-form-item>
          <el-form-item label="触发方式" prop="way">
            <el-select class="set_radius" v-model="queryParams.way" placeholder="请输入触发方式" clearable>
              <el-option v-for="dict in wayList" :key="dict.value" :label="dict.label" :value="dict.value"/>
            </el-select>
          </el-form-item>
          <el-form-item label="状态" prop="status">
            <el-select class="set_radius" v-model="queryParams.status" placeholder="状态" clearable>
              <el-option v-for="dict in statusList" :key="dict.value" :label="dict.label" :value="dict.value"/>
            </el-select>
          </el-form-item>

          <!-- <el-col class="float_right" :span="24"> -->
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
          </el-form-item>
          <!-- </el-col> -->
        </el-form>
      </div>
      <div style="padding: 20px; padding-top: 0" v-if="activeRules == 'rulesManage'">
        <el-row :gutter="10" class="mb8 button_row">
          <el-col :span="1.5">
            <el-button type="primary" plain @click="toAdd(null)" :disabled="multiple">
              <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
              <span style="margin-left: 6px">新增</span>
            </el-button>
          </el-col>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <div style="margin-top: 15px" class="wulian_list pro-table-card-items" v-loading="tableDataLoading">
          <el-row :gutter="20" style="margin-bottom: -25px">
            <el-col v-for="it in myRulesList" :key="it.Id" :span="colNum">
              <div class="iot-card">
                <div class="iot-content" @click="toEdit(it.Id)">
                  <div class="img_content">
                    <div class="img_con">
                      <img src="../../../assets/images/guize.png" alt />
                    </div>
                    <div class="iotcontent_con">
                      <div class="top_title">
                        <h2>{{ it.Name }}</h2>
                      </div>
                      <div class="bottom_detial">
                        <div class="left_content">
                          <div class="lab">触发方式</div>
                          <div class="ctn">
                            {{ it.TriggerWay == 0 ? "设备触发" : it.TriggerWay == 1 ? "HTTP触发" : "定时触发"}}
                          </div>
                        </div>
                        <div class="right_content">
                          <div class="lab">说明</div>
                          <div class="ctn">{{ it.Remark }}</div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="card-state error" v-if="it.Status == 1">
                    <div class="card-state-content">
                      <span class="ant-badge ant-badge-status">
                        <span class="ant-badge-status-dot ant-badge-status-error"></span>
                        <span class="ant-badge-status-text">暂停</span>
                      </span>
                    </div>
                  </div>
                  <div class="card-state success" v-if="it.Status == 0">
                    <div class="card-state-content">
                      <span class="ant-badge ant-badge-status">
                        <span class="ant-badge-status-dot ant-badge-status-success"></span>
                        <span class="ant-badge-status-text">正常</span>
                      </span>
                    </div>
                  </div>
                  <div class="card-mask">
                    <button>
                      <svg-icon icon-class="todetails" style="color: #fff"></svg-icon>
                    </button>
                  </div>
                </div>
                <div class="iot-fun" style="margin-top: 10px">
                  <el-row :gutter="10">
                    <el-col :span="10">
                      <div class="edit" @click="toEdit(it.Id)">
                        <el-button :disabled="false">
                          <i class="el-icon-edit" style="margin-right: 8px"></i>
                          <span>编辑</span>
                        </el-button>
                      </div>
                    </el-col>
                    <el-col :span="10">
                      <div class="open" @click="setStatus(it.Id, it.Status)">
                        <el-button v-if="it.Status == 1">
                          <i class="el-icon-video-play"></i>
                          <span>启用</span>
                        </el-button>
                        <el-button v-if="it.Status == 0">
                          <i class="el-icon-video-pause"></i>
                          <span>暂停</span>
                        </el-button>
                      </div>
                    </el-col>
                    <el-col :span="4">
                      <div class="del" @click="delrule(it.Id)">
                        <el-button style="font-size: 14px;display: flex;justify-content: center;width: 100%;">
                          <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                        </el-button>
                      </div>
                    </el-col>
                  </el-row>
                </div>
              </div>
            </el-col>
          </el-row>
        </div>

        <pagination
          v-if="myRulesList && myRulesList.length > 0"
          :pageSizes="rulesPageSizes"
          :total="total"
          :page.sync="queryParams.pageNum"
          :limit.sync="queryParams.pageSize"
          @pagination="getList"
        />
      </div>
      <div style="padding: 20px" v-if="activeRules == 'rulesGroup'">
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-button type="primary" plain @click="openAddGroup">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left: 6px">添加分组</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar
            :showSearch.sync="showSearch"
            @queryTable="getGroupList"
            :columns="columns"
          ></right-toolbar>
        </el-row>
        <el-table
          v-loading="tableDataLoading"
          :data="groupTableData"
          style="width: 100%"
          row-key="Id"
          class="data_table"
          :header-cell-style="cellSty"
          border
          :default-expand-all="isExpandAll"
          :tree-props="{ children: 'Children', hasChildren: 'hasChildren' }"
        >
          <el-table-column v-for="(item, inx) in groupParamsList" :key="inx" :prop="item.filed" :label="item.filedName" v-show="columns[inx].visible"
          ></el-table-column>
          <el-table-column label="操作" align="center" width="320" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button type="text" icon="el-icon-edit" @click="editRowData(scope.row, false)">编辑</el-button>
              <el-button type="text" icon="el-icon-delete" @click="deleteRowData(scope.row)">删除</el-button>
              <el-button type="text" icon="el-icon-tickets" @click="editRowData(scope.row, true)">查看详情</el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
      <el-dialog title="添加分组" :visible.sync="groupOpen" center width="600px" :close-on-click-modal="false">
        <el-form :model="groupFrom" ref="groupFrom" :rules="groupRules" label-position="left" class="groupFrom" :inline="true" label-width="80px">
          <el-form-item label="父级分组" prop="parentIdData">
            <treeselect class="groupSet" v-model="groupFrom.parentIdData" :options="groupTreeList" :show-count="true" :normalizer="normalizer" placeholder="请选择规则分组" @select="selectGroupTree" :disabled="groupFormDisable"/>
          </el-form-item>
          <el-form-item label="分组名称" prop="groupName">
            <el-input type="text" v-model="groupFrom.groupName" placeholder="请输入分组名称" :disabled="groupFormDisable"></el-input>
          </el-form-item>
          <el-form-item label="分组序号" prop="sort">
            <el-input type="number" v-model.number="groupFrom.sort" placeholder="请输入分组序号" :disabled="groupFormDisable"></el-input>
          </el-form-item>
          <el-form-item label="分组描述" prop="remark">
            <el-input type="text" v-model="groupFrom.remark" placeholder="请输入分组描述" :disabled="groupFormDisable"></el-input>
          </el-form-item>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button @click="groupOpen = false">取 消</el-button>
          <el-button type="primary" @click="addDeviceGroup">确 定</el-button>
        </div>
      </el-dialog>
      <el-dialog
        title="添加规则"
        :visible.sync="addRulesDialog"
        width="60%"
        @close="addRulesDialog = false"
        :destroy-on-close="true"
        :close-on-click-modal="false"
        top="6vh"
      >
        <div slot="title">
          <div class="dia_title_con">
            <div class="title_text">添加规则</div>
            <div style="padding-left: 30px; display: inline-block">
              <div class="rulesjson" v-if="rulesJsonName">{{ rulesJsonName }}</div>
              <el-button type="primary" class="putbutton">导入规则<input type="file" @change="importProcess" id="putbuttonFile"/></el-button>
            </div>
          </div>
        </div>
        <div class="add_rules_dialog">
          <addForm
            ref="addForm"
            @editParams="editParams"
            @openParamsDrawer="openParamsDrawer"
            @evtCodeChange="evtCodeChange"
            @handleShowCron="handleShowCron"
            @openStepChoice="openStepChoice"
            @setAddForm="setAddForm"
            @setSelProduct="setSelProduct"
            :groupTreeList="groupTreeList"
            :productmap="productmap"
            :productLists="productLists"
            :device="rulesFrom.device"
            :product="rulesFrom.product"
            :isEdit="false"
          ></addForm>
        </div>
        <div slot="footer">
          <el-button @click="addRulesDialog = false">取 消</el-button>
          <el-button type="primary" @click="saveRules">创 建</el-button>
        </div>
      </el-dialog>
      <cronTime ref="cronTime" @finishTimeChoice="finishTimeChoice"></cronTime>
      <paramsAdd
        ref="paramsDialog"
        :typeList="typeList"
        :triggerWay="rulesFrom.TriggerWay"
        :HttpParams="rulesFrom.HttpParams"
        @joinParams="joinParams"
      ></paramsAdd>
      <selectDeviceWay
        ref="selectDeviceWay"
        @selectTriggerLs="selectTriggerLs"
        @initProEvts="initProEvts"
        :productmap="productmap"
        :selProList="selProList"
        @selectProduct="selectProduct"
        @selectDeviceList="selectDeviceList"
        @selectTopicMsg="selectTopicMsg"
      ></selectDeviceWay>
    </div>
  </div>
</template>

<script>
import {
  rulesList,
  delRusel,
  editRuselServe,
  addRuselServe,
  addGroup,
  groupTree,
  removeGroup,
  editGroup,
} from "@/api/rules/ruselSevic";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { productList } from "@/api/rules/productModel";

import Crontab from "@/components/Crontab";
import paramItem from "../funInput/paramItem.vue";
import enumItem from "../funInput/enumItem.vue";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import addForm from "./addForm";
import cronTime from "./cron_time";
import paramsAdd from "./params_add";
import selectDeviceWay from "./selectDeviceWay";
export default {
  name: "ProcessList",
  dicts: ["sys_normal_disable"],
  mixins: [resizeTableCon],
  components: {
    Crontab,
    paramItem,
    enumItem,
    Treeselect,
    cronTime,
    paramsAdd,
    addForm,
    selectDeviceWay,
  },
  data() {
    return {
      //http触发相关参数
      paramsLoading: false,
      typeListMap: new Map(),
      typeList: [
        { alabel: "整型", label: "整型(Int)", value: "int" },
        { alabel: "浮点", label: "浮点型(Float)", value: "float" },
        { alabel: "字符", label: "字符型(String)", value: "string" },
        { alabel: "时间", label: "时间型(Date)", value: "date" },
        { alabel: "布尔", label: "布尔型(Boolean)", value: "boolean" },
        { alabel: "枚举", label: "枚举型(Enum)", value: "enum" },
      ], //数据类型列表
      //产品过滤条件
      triggerLs: [], //选择的设备

      // rulesFrom:{},
      statusList: [
        { label: "正常", value: 0 },
        { label: "暂停", value: 1 },
      ],
      wayList: [
        { label: "订阅触发", value: 0 },
        { label: "HTTP触发", value: 1 },
        { label: "定时触发", value: 2 },
      ],
      colNum: 6, //col的span的值,页面每一行排列的个数
      multiple: false, //没有权限禁用
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      myRulesList: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 12,
        way: null,
        status: null,
        GroupId: null,
      },
      rulesPageSizes: [6, 8, 12, 18, 24, 32, 36, 48],
      rulesFrom: {
        //新增的规则
        id: null, //规则唯一标识id
        Name: "", //名称
        TimerCron: "",
        Sort: 0,
        GroupId: "",
        TriggerWay: 0, //触发方式
        device: [], //选中的设备
        product: "", //订阅的设备 产品
        TopicMsg: "", //订阅的消息类型
        HttpParams: [],
        process: {
          id: "root",
          parentId: null,
          type: "ROOT",
          name: "发起人",
          desc: "任何人",
          children: {},
        },
        remark: "备注说明",
      },
      rules: {
        name: [{ required: true, trigger: "blur", message: "请输入名称" }],
        sort: [{ required: true, trigger: "blur", message: "请输入优先级" }],
        product: [{ required: true, trigger: "change", message: "请选择产品" }],
        device: [{ required: true, trigger: "change", message: "请选择设备" }],
        topicMsg: [
          {
            required: true,
            trigger: "change",
            message: "请选择订阅的消息类型",
          },
        ],
        timerCron: [
          { required: true, trigger: "change", message: "请输入表达式" },
        ],
      },
      addRulesDialog: false, //添加规则的弹出层
      productLists: [], //产品列表
      // device: [], //选中的设备
      productParams: {
        showAll: true,
      }, //产品列表查询form
      // 传入的表达式
      timerCron: "",
      proEvt: [],
      evtCode: "",//事件Code
      activeRules: "", //活动的页面是规则还是分组
      groupOpen: false, //添加分组弹窗
      groupFrom: {
        parentIdData: "",
        id: "",
        groupName: "",
        sort: "",
        remark: "",
      },
      groupRules: {
        parentIdData: [
          { required: true, trigger: "change", message: "请选择父级分组" },
        ],
        groupName: [
          { required: true, trigger: "blur", message: "请输入分组名称" },
        ],
        sort: [{ required: true, trigger: "blur", message: "请输入分组序号" }],
      },
      groupTreeList: [],
      groupParamsList: [
        { filed: "Id", filedName: "分组ID" },
        { filed: "GroupName", filedName: "分组名称" },
        { filed: "Remark", filedName: "分组描述" },
        // { filed: "Path", filedName: "分组路径" },
        { filed: "updateTime", filedName: "修改时间" },
      ],
      groupmap: new Map(),
      // 是否展开，默认全部折叠
      isExpandAll: false,
      tableDataLoading: false,
      columns: [
        { key: 0, label: `分组Id`, visible: true },
        { key: 1, label: `分组名称`, visible: true },
        { key: 2, label: `分组描述`, visible: true },
        // { key: 3, label: `分组路径`, visible: true },
        { key: 3, label: `修改时间`, visible: true },
      ],
      groupFormDisable: false, //是否是查看详情
      rulesJsonName: "",
      productmap: new Map(), //产品map类型数据
      triggerProduct:'',//产品
    };
  },
  created() {
    this.getList(); //获取规则列表
    // console.log("屏幕宽度", window.screen.width);
    let wid = window.screen.width;
    if (wid > 1024) {
      this.colNum = 6;
    } else if (785 < wid < 1024) {
      this.colNum = 8;
    } else if (500 < wid < 768) {
      this.colNum = 12;
    } else if (500 < wid) {
      this.colNum = 24;
    }
  },
  computed: {
    selProList() {
      if (this.$refs.addForm) {
        return this.$refs.addForm.selProList;
      } else {
        return [];
      }
    },
  },
  mounted() {
    // this.getDeviceList();
    this.getProductList();

    this.getGroupList();
    this.activeRules = "rulesManage";
  },
  methods: {
    selectTriggerLs(list) {
      this.triggerLs = JSON.parse(JSON.stringify(list));
    },
    editParams(row, rowIndex) {
      //编辑添加的参数
      this.$refs.paramsDialog.editParams(row, rowIndex);
    },
    openParamsDrawer() {
      //添加参数
      this.$refs.paramsDialog.openParamsDrawer();
    },
    evtCodeChange(val) {
      //选择事件
      this.evtCode = val;
    },
    handleShowCron() {
      //定时表达式生成初始化值设置
      this.$refs.cronTime.handleShowCron(this.rulesFrom.TimerCron);
    },
    openStepChoice() {
      //打开设备选择窗口
      this.$refs.selectDeviceWay.openStepChoice();
    },
    selectDeviceList(list) {
      this.$refs.addForm.selectDeviceList(list);
    },
    selectProduct(list) {
      //选择完产品后
      this.$refs.addForm.selectProduct(list);
    },
    selectTopicMsg(node) {
      //选择完消息订阅方式
      this.$refs.addForm.selectTopicMsg(node);
    },
    joinParams(paramList) {
      //加入参数列表后
      this.$refs.addForm.joinParams(paramList);
    },
    async initProEvts() {
      //获取产品的事件
      await this.$refs.addForm.initProEvts();
    },
    setAddForm(formVal) {
      let process=formVal.process
      if(this.rulesFrom.process){
        process=this.rulesFrom.process
      }
      this.rulesFrom = JSON.parse(JSON.stringify(formVal));
      this.rulesFrom.process =process
    },
    setSelProduct(selProduct){
      this.triggerProduct=selProduct
    },
    editRowData(row, isdisable) {
      //修改一行的数据
      if (this.activeRules == "rulesGroup") {
        this.openAddGroup();
        this.groupFrom = {
          parentIdData: row.ParentId == "" ? -1 : row.ParentId,
          id: row.Id,
          groupName: row.GroupName,
          sort: row.Sort,
          remark: row.Remark,
        };
        // console.log(isdisable, "是否禁用");
        if (isdisable) {
          this.groupFormDisable = true;
        } else {
          this.groupFormDisable = false;
        }
      }
    },
    addDeviceGroup() {
      //添加设备分组
      this.$refs["groupFrom"].validate((valid) => {
        if (valid) {
          if (this.groupFrom.parentIdData == -1) {
            this.groupFrom.parentId = "";
          } else {
            this.groupFrom.parentId = this.groupFrom.parentIdData;
          }
          if (this.groupFrom.id) {
            editGroup(this.groupFrom)
              .then((response) => {
                this.$message.success("修改分组成功");
                if (response.code == 0) {
                  this.getGroupList();
                }
                this.groupOpen = false;
              })
              .catch((err) => {
                this.$message.error(err.message);
              });
          } else {

            addGroup(this.groupFrom)
              .then((response) => {
                this.$message.success("创建分组成功");
                if (response.code == 0) {
                  this.getGroupList();
                }
                this.groupOpen = false;
              })
              .catch((err) => {
                this.$message.error(err.message);
              });
          }
        }
      });
    },
    openAddGroup() {
      //打开添加分组
      this.resetForm("groupFrom");
      this.groupFrom = {
        parentIdData: null,
        id: "",
        groupName: "",
        sort: "",
        remark: "",
      };
      if (!this.groupTreeList[0] || this.groupTreeList[0].Id != "-1") {
        this.groupTreeList.unshift({
          GroupName: "作为一级分组",
          Id: "-1",
          ParentId: 0,
        });
      }
      this.groupOpen = true;
      this.groupFormDisable = false;
    },
    deleteRowData(row) {
      //删除表格中的一行的数据
      if (this.activeRules == "rulesGroup") {
        this.$modal
          .confirm('是否确认移除名为"' + row.GroupName + '"的设备分组？')
          .then(function () {
            return removeGroup({ id: row.Id });
          })
          .then(() => {
            this.getGroupList();
            this.$modal.msgSuccess("移除成功");
          })
          .catch(() => {});
      }
    },
    getGroupList() {
      //获取规则分组列表
      this.tableDataLoading = true;
      groupTree().then((res) => {
        if (res.code == 0) {
          this.groupTableData = res.data;
          this.initgroupClassMap(this.groupTableData);
          let lists = [];
          lists = JSON.parse(JSON.stringify(res.data));
          this.groupTreeList = lists;
          this.tableDataLoading = false;
        }
      });
    },
    selectGroupTree() {},
    normalizer(node) {
      if (node.Children == null || !node.Children.length) {
        delete node.Children;
      }
      return {
        id: node.Id,
        label: node.GroupName,
        children: node.Children,
      };
    },
    initgroupClassMap(node) {
      this.groupmap.set("-1", {
        GroupName: "作为一级分组",
        Id: "-1",
        Children: [],
      });
      for (let idx = 0; idx < node.length; idx++) {
        let curnode = node[idx];
        this.groupmap.set(node[idx].Id, curnode);
        if (
          curnode.hasOwnProperty("Children") &&
          curnode.Children &&
          curnode.Children.length > 0
        ) {
          this.initgroupClassMap(curnode.Children);
        }
      }
    },
    rulesSelect(path) {
      //切换设备管理
      this.configLoading = true;
      this.activeRules = path;
      if (this.activeRules == "rulesManage") {
        if (this.groupTreeList[0] && this.groupTreeList[0].Id == "-1") {
          this.groupTreeList.splice(0, 1);
        }
      }
      this.configLoading = false;
    },
    processReadFile(file) {
      //读取导入参数的值
      const reader = new FileReader();
      reader.onload = (e) => {
        try {
          let jsonArr = JSON.parse(e.target.result);
          this.rulesFrom.process = jsonArr.ProcessStr;
          this.rulesFrom.HttpParams = jsonArr.HttpParamsStr;
        } catch (error) {
          if (this.rulesFrom.process) {
          } else {
            this.rulesFrom.process = {
              id: "root",
              parentId: null,
              type: "ROOT",
              name: "发起人",
              desc: "任何人",
              children: {},
            };
          }
          if (this.rulesFrom.HttpParams) {
          } else {
            this.rulesFrom.HttpParams = [];
          }
          // this.$store.commit("rulesloadForm", this.setup);
        }
      };
      reader.readAsText(file);
    },
    importProcess(event) {
      //导入参数
      const file = event.target.files[0];
      if (!file) {
        return;
      }
      if (file.name) {
        this.rulesJsonName = file.name;
      }
      this.processReadFile(file);
    },
    timeTypeChange() {
      if (this.timeType == 0) {
        this.timeValue = null;
      }
    },
    intervalTypeChange() {
      //间隔类型
      if (this.intervalType == 0) {
        this.intervalMax = 59;
      } else if (this.intervalType == 1) {
        this.intervalMax = 59;
      } else if (this.intervalType == 2) {
        this.intervalMax = 23;
        if (this.intervalNum >= 23) {
          this.intervalNum = 23;
        }
      }
    },
    repeatValChange(val) {
      if (val == 3) {
        this.isShowWeekLis = true;
      } else {
        this.isShowWeekLis = false;
      }
    },
    setWeekVal(item) {
      //设置星期值
      let val = item.val;
      if (this.weekVal.includes(val)) {
        let ix = this.weekVal.indexOf(val);
        this.weekVal.splice(ix, 1);
      } else {
        this.weekVal.push(val);
      }
    },
    finishTimeChoice(val) {
      //生成的定时表达式结果
      this.$refs.addForm.finishTimeChoice(val);
    },
    setTypeMap() {
      //设置数据类型的map
      this.typeList.forEach((item) => {
        this.typeListMap.set(item.value, item);
      });
      this.$forceUpdate();
    },
    openParamsDrawer() {
      this.$refs.paramsDialog.openParamsDrawer();
    },
    editParams(row, rowIndex) {
      //编辑添加的参数
      this.$refs.paramsDialog.editParams(row, rowIndex);
    },
    //添加http触发相关函数
    stepChoice() {},
    saveRules() {
      this.$refs.addForm.validateForm((valid) => {
        // console.log("规则效验", valid);
        if (valid) {
          let httpParams = "[]";
          let triggerLs=[]
          if (this.rulesFrom.TriggerWay == 0) {
            //选择的是设备触发，必须要选择设备
            if (this.triggerLs && this.triggerLs.length > 0) {
              if (this.evtCode != "") {
                this.triggerLs.forEach((x) => {
                  x.topicMsg = x.topicMsg + "#" + this.evtCode;
                });
              } else {
                for (let xi = 0; xi < this.triggerLs.length; xi++) {
                  if (this.triggerLs[xi].topicMsg == "Event") {
                    this.$message.warning("请选择触发事件");
                    return;
                  }
                }
              }
            } else {
              this.$message.warning("请选择触发设备");
              return;
            }
            triggerLs=this.triggerLs
          } else {
            //选择的不是设备触发，将设备触发参数置空
            triggerLs = []; //如果选择的触发方式不是设备触发，需要清除设备触发选择的参数
            if(this.rulesFrom.TriggerWay == 2){
              if(this.triggerProduct){
                triggerLs.push({
                  topicMsg:'Execute',
                  topicDevice:'/'+this.triggerProduct+'/-1'
                })
              }
            }
          }
          if (this.rulesFrom.TriggerWay != 2) {
            //选择的不是定时触发，corn表达式清空
            this.rulesFrom.TimerCron = "";
          }
          // console.log("规则分组", this.rulesFrom);
          httpParams = JSON.stringify(this.rulesFrom.HttpParams);
          let template = {};
          template = {
            name: this.rulesFrom.Name,
            CreatedFrom: "pc",
            sort: this.rulesFrom.Sort,
            groupId: this.rulesFrom.GroupId,
            triggerWay: this.rulesFrom.TriggerWay,
            triggerList: JSON.parse(JSON.stringify(triggerLs)),
            timerCron: this.rulesFrom.TimerCron,
            ruleJson: JSON.stringify(this.rulesFrom.process),
            httpParams: httpParams,
            remark: this.rulesFrom.remark,
            debug: 1,
          };
          addRuselServe(template)
            .then((rsp) => {
              this.$message.success("规则创建成功");
              this.addRulesDialog = false;
              this.getList(); //获取规则列表
              this.$store.commit("tagsView/DEL_CACHED_VIEW", {
                path: "/iot/rulesEngine/add",
              });
              this.$nextTick(() => {
                this.$router.push({
                  path: "/iot/rulesEngine/add",
                  query: { id: rsp.data },
                });
              });
            })
            .catch((err) => {
              this.$message.error(err);
            });
        }
      });
    },
    getProductList() {
      productList(this.productParams).then(async (response) => {
        this.productLists = response.data.List;
        this.initProductMap(response.data.List);
      });
    },
    initProductMap(node) {
      for (let idx = 0; idx < node.length; idx++) {
        let curnode = node[idx];
        this.productmap.set(node[idx].Id, curnode);
      }
    },

    setStatus(id, sta) {
      //设置规则状态
      if (sta == 0) {
        this.$confirm('是否确认暂停编号为"' + id + '"的规则?', "警告", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
        }).then(() => {
          let par = { id: id, status: 1 };
          editRuselServe(par)
            .then((rsp) => {
              if (rsp.code == 0) {
                this.getList();
                this.$modal.msgSuccess("该规则已暂停");
              }
            })
            .catch((err) => {
              this.$message.error(err);
            });
        });
      } else if (sta == 1) {
        this.$confirm('是否确认启用编号为"' + id + '"的规则?', "警告", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
        }).then(() => {
          let par = { id: id, status: 0 };
          editRuselServe(par)
            .then((rsp) => {
              if (rsp.code == 0) {
                this.getList();
                this.$modal.msgSuccess("该规则已启用");
              }
            })
            .catch((err) => {
              this.$message.error(err);
            });
        });
      }
    },
    delrule(id) {
      //删除规则
      this.$confirm('是否确认删除编号为"' + id + '"的规则?', "警告", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(function () {
          return delRusel(id);
        })
        .then(() => {
          this.queryParams.pageNum = 1;
          this.getList();
          this.$modal.msgSuccess("删除成功");
        });
    },
    toAdd() {
      this.addRulesDialog = true;
      this.setTypeMap();
      this.rulesFrom = {
        //新增的规则
        id: null, //规则唯一标识id
        Name: "", //名称
        TimerCron: "",
        Sort: 0,
        GroupId: "",
        TriggerWay: 0, //触发方式
        device: [], //选中的设备
        product: "", //订阅的设备 产品
        TopicMsg: "", //订阅的消息类型
        HttpParams: [],
        process: {
          id: "root",
          parentId: null,
          type: "ROOT",
          name: "发起人",
          desc: "任何人",
          children: {},
        },
        remark: "备注说明",
      };
    }, //增加规则
    toEdit(id) {
      //到规则编辑页
      if (id) {
        this.$store.commit("tagsView/DEL_CACHED_VIEW", {
          path: "/iot/rulesEngine/add",
        });
        this.$nextTick(() => {
          this.$router.push({
            path: "/iot/rulesEngine/add",
            query: { id: id },
          });
        });
      }
    },
    getList() {
      rulesList(this.queryParams).then((response) => {
        // console.log("规则引擎列表查询结果", response);
        if (response.code == 0) {
          this.myRulesList = response.data.List;
          this.total = response.data.Total;
        }
      });
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.resetForm("queryForm");
      this.handleQuery();
    },
  },
};
</script>
<style lang="less">
.groupFrom {
  .el-form-item {
    width: 100%;

    .el-form-item__content {
      width: calc(100% - 80px);

      .el-select {
        width: 100%;
      }

      .el-input {
        width: 100%;
      }
    }
  }
}

.groupSet {
  .vue-treeselect__input-container {
    display: flex;
    align-items: center;
  }
}
</style>
<style lang="less" scope>
.device_select_con {
  margin-top: 20px;
}
.device_select_li {
  position: relative !important;
  height: 34px !important;
  border: none !important;
  width: 120px !important;
}
.device_select_btn {
  float: right;
}
.device_select_con::after {
  clear: both;
}
.trigger_type_list {
  margin-right: -3%;
  margin-top: 15px;
  display: flex;
  justify-content: flex-start;
  flex-wrap: wrap;
  .trigger_type_con {
    width: 18%;
    padding-right: 3%;
    box-sizing: border-box;
    .trigger_type {
      padding: 10px 0;
      position: relative;
      box-sizing: border-box;
      border: 1px solid #e0e4e8;
      border-radius: 2px;
      cursor: pointer;
      .type_content {
        // display: flex;
        // justify-content: center;
        // align-items: center;
        // background：linear-gradient(to right top,#1C3FE9,#93A3F4);
        // flex-wrap: wrap;
        text-align: center;
        .img {
          display: inline-block;
          width: 50px;
          height: 50px;
          line-height: 50px;
          font-size: 16px;
          color: #ffffff;
          background: linear-gradient(to right top, #1c3fe9, #93a3f4 70px);
          box-shadow: 0 6px 6px 0 #93a3f4;
          margin-bottom: 10px;
        }
      }
      &.disabled {
        cursor: not-allowed;
        opacity: 0.6;
      }
      &.active {
        border-color: #10239e;
        opacity: 1;
        color: #000;
      }
      /* 三角形 */
      #div6 {
        position: absolute;
        right: 0;
        bottom: 0;
        width: 0;
        height: 0;
        // border: 11px solid #cccccc;
        // border-left: 11px solid transparent;
        // border-top: 11px solid transparent;
        border: 11px solid #3888ff;
        border-left: 11px solid transparent;
        border-top: 11px solid transparent;
      }

      /* 对号 */

      .check {
        position: relative;
        display: inline-block;
        width: 25px;
        height: 25px;
        border-radius: 25px;
      }

      .check::after {
        content: "";
        position: absolute;
        left: -2px;
        top: 0px;
        width: 40%;
        height: 22%;
        border: 2px solid #fff;
        border-radius: 1px;
        border-top: none;
        border-right: none;
        background: transparent;
        transform: rotate(-45deg);
      }
      &.disable_li {
        opacity: 0.6;
        cursor: not-allowed;
      }
    }
  }
}
.device_list {
  margin-right: -1.5%;
  margin-top: 15px;
  display: flex;
  justify-content: flex-start;
  flex-wrap: wrap;

  .device_li_con {
    width: 33%;
    padding-right: 1.5%;
    box-sizing: border-box;

    .device_li {
      cursor: pointer;
      box-shadow: 0 0 6px 6px rgba(244, 245, 249, 1);
      margin: 0 0 25px 0;
      border-radius: 5px;
      padding: 20px 10px;
      position: relative;
      box-sizing: border-box;
      .device_lis_top {
        width: 100%;
        display: flex;
        justify-content: flex-start;
        img {
          width: 80px;
          height: 80px;
          margin-right: 20px;
        }
        .lis_top_cot {
          width: calc(100% - 100px);
          .device_name {
            margin-bottom: 5px;
            color: #000000;
            font-weight: 600;
            font-size: 16px;
            width: 100%;
            white-space: nowrap;
            text-overflow: ellipsis;
            overflow: hidden;
          }
          .device_group_name {
            color: #887e7b;
            display: flex;
            justify-content: flex-start;
            align-items: flex-start;
            width: 100%;
            .group_name_lft {
              width: 49%;
              margin-right: 2%;

              .title {
                font-size: 12px;
              }
              .cont {
                margin-top: 5px;
                font-size: 16px;
                color: #1890ff;
                width: 100%;
                white-space: nowrap;
                text-overflow: ellipsis;
                overflow: hidden;
                // display: -webkit-box;
                // -webkit-line-clamp: 2;
                // -webkit-box-orient: vertical;
              }
            }
            .group_name_rht {
              width: 49%;
              .title {
                font-size: 12px;
              }
              .cont {
                margin-top: 5px;
                font-size: 16px;
                color: #1890ff;
                width: 100%;
                white-space: nowrap;
                text-overflow: ellipsis;
                overflow: hidden;
                // display: -webkit-box;
                // -webkit-line-clamp: 2;
                // -webkit-box-orient: vertical;
              }
            }
          }
        }
      }
      /* 三角形 */

      #div6 {
        position: absolute;
        right: 0;
        bottom: 0;
        width: 0;
        height: 0;
        // border: 11px solid #cccccc;
        // border-left: 11px solid transparent;
        // border-top: 11px solid transparent;
        border: 11px solid #3888ff;
        border-left: 11px solid transparent;
        border-top: 11px solid transparent;
      }

      /* 对号 */

      .check {
        position: relative;
        display: inline-block;
        width: 25px;
        height: 25px;
        border-radius: 25px;
      }

      .check::after {
        content: "";
        position: absolute;
        left: -2px;
        top: 0px;
        width: 40%;
        height: 22%;
        border: 2px solid #fff;
        border-radius: 1px;
        border-top: none;
        border-right: none;
        background: transparent;
        transform: rotate(-45deg);
      }
      &.disable_li {
        opacity: 0.6;
      }
    }
  }
}
//选择设备的步骤条样式
.device_step {
  .el-step__head.is-success {
    color: #1c9efe;
    border-color: #1c9efe;
  }
  .el-step__title.is-success {
    color: #1c9efe;
  }
}
//选择设备的步骤条样式

.add_select_devic {
  margin-top: 10px;
  width: 100px;
  height: 36px;
  text-align: center;
  line-height: 36px;
  color: #1f9de5;
  border: 1px solid #dddddd;
  margin-left: 20px;
  font-size: 16px;
  cursor: pointer;
}
.select_devic_list {
  margin-top: 10px;
  // width: 100px;
  padding: 5px 10px;
  text-align: left;
  line-height: 26px;
  border: 1px solid #dddddd;
  margin-left: 20px;
  font-size: 16px;
  cursor: pointer;
  color: #606266;
  border-radius: 10px;
  span.devic_lis {
    margin: 0 10px;
  }
}
ul {
  padding: 0;
  margin: 0 auto;
  li {
    list-style: none;
  }
}
.dia_title_con {
  display: flex;
  width: 100%;
  box-sizing: border-box;
  align-content: center;
  justify-content: space-between;
  padding-right: 60px;
  .rulesjson {
    display: inline-block;
    margin-right: 20px;
    color: rgba(50, 150, 250, 0.71);
  }
  .putbutton {
    background: #1890ff;
    color: #fff;
    position: relative;
    span {
      color: #ffffff;
    }
    i {
      margin-right: 5px;
    }
    #putbuttonFile {
      position: absolute;
      left: 0;
      top: 0;
      width: 100%;
      height: 100%;
      opacity: 0;
      filter: alpha(opacity=0);
    }
  }
}
.add_rules_dialog {
  //设置信息窗体自动
  // 必须有高度 overflow 为自动
  overflow: auto;
  height: 634px;
  border-top: 1px solid #eff1f4;
  border-bottom: 1px solid #eff1f4;
  padding: 0px 30px 11px 27px;

  // 滚动条的样式,宽高分别对应横竖滚动条的尺寸
  &::-webkit-scrollbar {
    width: 3px;
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
}
.ruleForm {
  padding: 0 20px;
}
.from_ul {
  .form_li {
    width: 100%;
    position: relative;

    .form_content {
      .groupSet {
        .vue-treeselect__input-container {
          display: flex;
          align-items: center;
        }
      }
      .el-button.saveRules {
        background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
        width: 200px;
        height: 40px;
        // line-height: 40px;
        color: #ffffff;
      }
      .item-desc {
        color: rgba(50, 150, 250, 0.71);
        display: inline-block;
        width: 200px;
        height: 36px;
        line-height: 36px;
        background-color: #f5f7fa;
        text-align: left;
        margin-bottom: 10px;
        // font-size: 14px;
        border-radius: 5px;
        // border: 1px solid #dcdfe6;
        padding-left: 15px;
        box-sizing: border-box;
        margin-left: 5px;
      }
      .form_list {
        display: flex;
        align-items: center;
        flex-wrap: wrap;
        justify-content: flex-start;
        margin-left: -20px;
        width: 100%;
        .import_span {
          display: inline-block;
          position: relative;
          #inputFile {
            position: absolute;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            opacity: 0;
            filter: alpha(opacity=0);
          }
        }
        .param_list {
          //http参数列表样式
          color: #272e3b;
          width: 100%;
          box-sizing: border-box;
          padding: 12px;
          height: 36px;
          background-color: #fafafa;
          display: flex;
          justify-content: space-between;
          align-items: center;
          .list_left {
            .params_type {
              padding: 0 6px;
              border: 1px solid #595959;
              border-radius: 3px;
              margin-left: 10px;
            }
          }
          .list_right {
            i {
              padding: 0 5px;
              cursor: pointer;
            }
          }
        } //http参数列表样式
        .el-form-item {
          // margin-top: 20px;
          margin-left: 20px;
          .device-with-select {
            border: 1px solid #dcdfe6;
            height: 36px;
            line-height: 36px;
            border-radius: 5px;
            .left_select {
              border-radius: 5px 0 0 5px;
            }
            .el-input-group__prepend {
              input.el-input__inner {
                border-radius: 5px 0 0 5px;
              }
            }
            .el-input-group__append {
              input.el-input__inner {
                border-radius: 0 5px 5px 0;
              }
            }
            // border-radius: 8px;
            .el-input-group__append,
            .el-input-group__prepend {
              border: none;
            }
            input.el-input__inner {
              border: none;
            }
            .el-select {
              width: 120px;
              input {
                background-color: #ffffff;
              }
              // background-color: #ffffff;
            }
          }
          .el-select {
            width: 198px;
            input.el-input__inner {
              width: 198px;
              height: 36px;
            }
          }

          .device-with-select > input.el-input__inner {
            border-left: 1px solid #dcdfe6;
            border-right: 1px solid #dcdfe6;
            width: 200px;
            height: 36px;
            line-height: 36px;
          }
          .device-with-select > input.el-input__inner:focus {
            border: 1px solid #1890ff;
          }
          .Product-with-select > input.el-input__inner {
            display: none;
          }
          .Product-with-select {
            .el-input-group__prepend {
              width: 30px;
            }
            .el-input-group__append {
              border-left: 1px solid #dcdfe6;
              width: 200px;
              // background-color: #ffffff;
              .el-select {
                width: 198px;
                input.el-input__inner {
                  width: 198px;
                  height: 34px;
                }
              }
            }
          }
          .device-with-select.device-with-all > input.el-input__inner {
            border-radius: 0 5px 5px 0;
            border-right: none;
          }
        }
      }
      .el-input textarea {
        height: 100px !important;
      }

      .title_before {
        position: absolute;
        top: 1px;
        left: 0;
        width: 4px;
        height: 24px;
        background-color: #1890ff;
        border-radius: 0 3px 3px 0;
      }
      .el-form-item label.el-form-item__label {
        height: 26px;
        line-height: 26px;
        padding: 0 0 0 10px;
        margin-bottom: 8px;
        font-weight: 700;
        // font-size: 16px;
      }
      .el-form-item label.el-form-item__label::before {
        content: "";
      }
      .el-form-item input.el-input__inner {
        height: 36px;
      }
      .el-form-item label.el-form-item__label::after {
        display: inline-block;
        margin-left: 2px;
        color: #ff4d4f;
        // font-size: 14px;
        font-family: SimSun, sans-serif;
        line-height: 1;
        content: "*";
        font-weight: normal;
      }
      &.remark_con .el-form-item label.el-form-item__label::after {
        content: " ";
      }
      .fun_ul {
        display: flex;
        flex-direction: row;
        justify-content: flex-start;
        flex-wrap: wrap;
        align-items: center;
        margin-bottom: 24px;
        // grid-gap: 24px;
        // gap: 24px;
        width: 100%;
        margin-left: -24px;
        margin-bottom: -24px;
      }
      .fun_li {
        display: flex;
        padding: 22px 16px;
        border: 1px solid #e0e4e8;
        border-radius: 2px;
        cursor: pointer;
        transition: all 0.3s;
        margin-left: 24px;
        margin-bottom: 24px;
        span {
          display: flex;
        }
        .fun_right {
          margin-left: 26px;
          display: flex;
          align-items: center;
        }
        .fun_top {
          margin-bottom: 28px;
          font-weight: 700;
          // font-size: 16px;
        }
        .fun_bottom {
          color: rgba(0, 0, 0, 0.24);
          // font-size: 12px;
        }
      }
      .fun_ul.disabled .fun_li {
        cursor: not-allowed;
        // opacity: 0.6;
      }
      .fun_li.disabled {
        cursor: not-allowed;
        opacity: 0.6;
      }
      .fun_li.active {
        border-color: #10239e;
        opacity: 1;
        color: #000;
      }
    }
  }
}
.wulian_list {
  //   display: grid;
  //   grid-gap: 26px;
  // padding-bottom: 38px;
  box-sizing: border-box;
  .el-col {
    margin-bottom: 15px;
  }
  .iot-card {
    width: 100%;
    // background-color: blue;
    height: 176px;
    .card-state {
      position: absolute;
      top: 5px;
      right: -12px;
      display: flex;
      justify-content: center;
      width: 100px;
      padding: 2px 0;
      background-color: rgba(89, 149, 245, 0.15);
      transform: skewX(45deg);
      .card-state-content {
        transform: skewX(-45deg);
        .ant-badge-status {
          line-height: inherit;
          vertical-align: baseline;
        }
        .ant-badge {
          box-sizing: border-box;
          margin: 0;
          padding: 0;
          color: rgba(0, 0, 0, 0.85);
          font-size: 14px;
          font-variant: tabular-nums;
          line-height: 1.5715;
          list-style: none;
          font-feature-settings: "tnum", "tnum";
          position: relative;
          display: inline-block;
          line-height: 1;
        }
        .ant-badge-status-dot {
          position: relative;
          top: -1px;
          display: inline-block;
          width: 6px;
          height: 6px;
          vertical-align: middle;
          border-radius: 50%;
        }
        .ant-badge-status-error {
          background-color: #ff4d4f;
        }
        .ant-badge-status-success {
          background-color: #52c41a;
        }
        .ant-badge-status-text {
          margin-left: 8px;
          color: rgba(0, 0, 0, 0.85);
          font-size: 14px;
        }
      }
    }
    .card-state.error {
      background-color: rgba(229, 0, 18, 0.1);
    }
    .card-state.success {
      background-color: #f6ffed;
    }
    .iot-content {
      width: 100%;
      position: relative;
      border: 1px solid #e6e6e6;
      height: 134px;
      padding: 30px 12px 16px 30px;
      overflow: hidden;
      box-sizing: border-box;
      border-radius: 5px;
      .img_content {
        display: flex;
        justify-content: flex-start;
        width: 100%;
        .img_con {
          width: 88px;
          height: 88px;
          margin-right: 16px;
          img {
            width: 88px;
            height: 88px;
          }
        }
        .iotcontent_con {
          width: calc(100% - 104px);
          display: flex;
          justify-content: space-between;
          flex-direction: column;
          .top_title {
            width: 100%;
            margin-bottom: 12px;
            height: 16px;
            h2 {
              font-weight: 700;
              font-size: 16px;
            }
          }
          .bottom_detial {
            width: 100%;
            height: 44px;
            display: flex;
            justify-content: flex-start;
            // flex-wrap: wrap;
            .left_content,
            .right_content {
              width: 50%;
              line-height: 22px;
              height: 44px;
              .lab {
                width: 100%;
                height: 22px;
                color: rgba(0, 0, 0, 0.75);
                font-size: 12px;
                white-space: nowrap; /*默认normal 自动换行*/
                overflow: hidden;
                text-align: left;
                text-overflow: ellipsis;
              }
              .ctn {
                width: 100%;
                height: 22px;
                color: rgba(0, 0, 0, 0.75);
                font-size: 14px;
                white-space: nowrap; /*默认normal 自动换行*/
                overflow: hidden;
                text-align: left;
                text-overflow: ellipsis;
                // -webkit-box-orient: vertical;
                // -webkit-line-clamp: 1;
              }
            }
            .right_content {
              margin-left: 12px;
              width: calc(50%-12px);
            }
          }
        }
      }
      .card-mask {
        position: absolute;
        top: 0;
        left: 0;
        z-index: 2;
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100%;
        height: 100%;
        color: #fff;
        background-color: transparent;
        visibility: hidden;
        cursor: pointer;
      }
    }
    .iot-content:hover {
      .card-mask {
        background-color: rgba(0, 0, 0, 0.5);
        visibility: visible;
        button {
          border: none;
          cursor: pointer;
          font-size: 24px;
          background: rgba(0, 0, 0, 0);
        }
      }
    }
    .iot-content::before {
      position: absolute;
      top: 0;
      left: 40px;
      display: block;
      width: 15%;
      min-width: 64px;
      height: 2px;
      background: #8da1f4;
      //   background-repeat: no-repeat;
      background-size: 100% 100%;
      content: " ";
    }
    .iot-fun {
      font-size: 14px;
      .el-button {
        width: 100%;
      }
    }
  }
}
.content_con {
  padding-top: 15px;
  .content_li_con:last-child {
    border-bottom: 1px solid #ebeef5;
  }
  .content_li_con {
    padding: 12px;
    border-top: 1px solid #ebeef5;

    .title {
      font-size: 16px;
      text-align: left;
    }
    .content_ul {
      width: 100%;
      display: flex;
      justify-content: flex-start;
      flex-wrap: wrap;
      margin: 0;
      padding: 0;
      margin-top: 20px;

      .li_con {
        padding-left: 20px;
        width: 20%;
        height: 70px;
        box-sizing: border-box;
        margin-top: 10px;
        display: flex;
        justify-content: flex-start;

        .content_li {
          cursor: pointer;
          width: 100%;
          height: 70px;
          padding: 0 5px 0 10px;
          display: flex;
          justify-content: space-between;
          align-items: center;
          border: 1px solid #ebeef5;
          box-sizing: border-box;
          border-radius: 5px;
          &.active_li {
            border: 1px solid #448ed7;
          }
          .left_cont {
            display: flex;
            align-items: center;
            .li-icons {
              width: 30px;
              height: 30px;
              border-radius: 5px;
              font-size: 20px;
              text-align: center;
              line-height: 30px;
              flex-shrink: 0; //由于子元素宽度之和超出了弹性盒子宽度，因此两个子元素被等比例压缩，解决方法
            }
            span {
              margin-left: 5px;
            }
          }
          .active_content {
            color: #38adff;
            font-size: 12px;
            min-width: 48px;
            // flex-shrink:0;
            text-align: right;
            margin-left: 3px;
          }
        }
      }
    }
  }
}
</style>
