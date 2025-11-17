<template>
  <div>
    <el-dialog title="选择触发设备" :destroy-on-close="true" :visible.sync="stepChoiceDevice" :close-on-click-modal="false" width="60%" style="z-index: inherit;">
        <el-steps :active="active" finish-status="success" class="device_step">
          <el-step title="选择产品">
            <span slot="description" style="color: #1c9efe" v-if="selProLists && selProLists.length > 0 && active > 0">
              <div @click="reselectProduct">重新选择</div>
            </span>
          </el-step>
          <el-step title="选择设备">
            <span slot="description" style="color: #1c9efe" v-if="selDevList && selDevList.length > 0 && active > 1">
              <div @click="reselectDevice">重新选择</div>
            </span>
          </el-step>
          <el-step title="订阅消息类型"></el-step>
        </el-steps>

        <div class="device_select_con">
          <el-form ref="selectForm" :inline="true" :model="selectForm" label-width="80px" v-if="active == 0">
            <el-form-item>
              <el-select v-model="filterType" placeholder="请选择" @change="changeFilterType" style="width: 120px">
                <el-option class="device_select_li" v-show="ite.value == 'product'" v-for="ite in filterTypeList" :key="ite.value" :label="ite.label" :value="ite.value"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-select v-model="filterConditions" placeholder="请选择" style="width: 120px">
                <el-option class="device_select_li" v-for="item in conditionsList" :key="item.value" :label="item.label" :value="item.value" v-show="item.value == 'contain'"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-input v-model="filterProductParams.Name" placeholder="请输入产品名称" v-if="filterType == 'product'"></el-input>
            </el-form-item>
            <div class="device_select_btn">
              <el-button icon="el-icon-search" type="primary" @click="getDevProductList">搜索</el-button>
              <el-button icon="el-icon-refresh-right" @click="retProductLoad">重置</el-button>
            </div>
          </el-form>
          <el-form ref="selectForm" :inline="true" :model="selectForm" label-width="80px" v-if="active == 1">
            <el-form-item>
              <el-select v-model="filterType" placeholder="请选择" @change="changeFilterType" style="width: 120px">
                <el-option class="device_select_li" v-for="ite in filterTypeList" v-show="ite.value != 'product'" :key="ite.value" :label="ite.label" :value="ite.value"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-select v-model="filterConditions" placeholder="请选择" style="width: 120px">
                <el-option class="device_select_li" v-for="item in conditionsList" :key="item.value" :label="item.label" :value="item.value"
                  v-show="(item.value == 'equal' && filterType != 'name') || (filterType == 'name' && item.value == 'contain')"
                ></el-option>
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-input v-model="filterParams.Name" placeholder="请输入设备名称" v-if="filterType == 'name'"></el-input>
              <el-input v-model="filterParams.DeviceId" placeholder="请输入设备编码" v-if="filterType == 'deviceCode'"></el-input>
            </el-form-item>
            <div class="device_select_btn">
              <el-button icon="el-icon-search" type="primary" @click="getRulesDevList">搜索</el-button>
              <el-button icon="el-icon-refresh-right" @click="retLoad">重置</el-button>
            </div>
          </el-form>
          <div class="device_list" v-loading="configLoading" v-if="active == 0">
            <div class="device_li_con" v-for="(its, inx) in devProductLists" :key="its.Id">
              <div :class="{ device_li: true }" @click="selectProduct(its, inx)">
                <div class="device_lis_top">
                  <img src="../../../assets/images/wulian.png" alt />
                  <div class="lis_top_cot">
                    <el-tooltip class="item" :content="its.Name" placement="top">
                      <div class="device_name">{{ its.Name }}</div>
                    </el-tooltip>
                    <div class="device_group_name">
                      <div class="group_name_lft">
                        <div class="title">协议分类</div>
                        <el-tooltip class="item" :content="its.ClassifiedId" placement="top" v-if="its.ClassifiedId">
                          <div class="cont">
                            {{its.ClassifiedId ? (classmap.get(its.ClassifiedId) ? classmap.get(its.ClassifiedId).Name : "该协议分类不存在") : "该协议分类不存在"}}
                          </div>
                        </el-tooltip>
                      </div>
                      <div class="group_name_rht">
                        <div class="title">接入方式</div>
                        <el-tooltip class="item" :content="its.NetworkWay" placement="top" v-if="its.NetworkWay">
                          <div class="cont">{{ its.NetworkWay }}</div>
                        </el-tooltip>
                      </div>
                    </div>
                  </div>
                </div>
                <div id="div6" v-show="its.selected">
                  <span class="check"></span>
                </div>
              </div>
            </div>
          </div>
          <div class="device_list" v-loading="devloading" v-if="active == 1">
            <div class="device_li_con" v-for="(its, inx) in deviceNodeList" :key="its.Id">
              <div :class="{device_li: true,disable_li: selDevList.length > 20 && !its.selected,}" @click="selectDevice(its, inx)">
                <div class="device_lis_top">
                  <img src="../../../assets/images/shebei.png" alt />
                  <div class="lis_top_cot">
                    <el-tooltip class="item" :content="its.Name" placement="top">
                      <div class="device_name">{{ its.Name }}</div>
                    </el-tooltip>
                    <div class="device_group_name">
                      <div class="group_name_lft" v-if="its.GroupName">
                        <div class="title">设备分组</div>
                        <el-tooltip class="item" :content="its.GroupName" placement="top">
                          <div class="cont">{{ its.GroupName }}</div>
                        </el-tooltip>
                      </div>
                      <div class="group_name_rht" v-if="its.ProductId">
                        <div class="title">协议名称</div>
                        <el-tooltip class="item" :content="its.ProductId ? (productmap.get(its.ProductId) ? productmap.get(its.ProductId).Name : '该协议不存在') : '该协议不存在'" placement="top">
                          <div class="cont">
                            {{its.ProductId ? (productmap.get(its.ProductId) ? productmap.get(its.ProductId).Name : "") : ""}}
                          </div>
                        </el-tooltip>
                      </div>
                    </div>
                  </div>
                </div>
                <div id="div6" v-show="its.selected">
                  <span class="check"></span>
                </div>
              </div>
            </div>
          </div>
          <div v-if="active == 2">
            <!-- <div>触发类型</div> -->
            <div class="trigger_type_list" v-loading="configLoading">
              <!-- <div>触发类型</div> -->
              <div class="trigger_type_con" v-for="(its, inx) in topicMsgList" :key="its.Id">
                <div :class="{ trigger_type: true, disable_li: false }" @click="selectTopicMsg(its, inx)">
                  <div class="type_content">
                    <div class="img">
                      <i :class="its.icon"></i>
                    </div>
                    <div>{{ its.label }}</div>
                  </div>
                  <div id="div6" v-show="selTopMsg && selTopMsg.value && selTopMsg.value == its.value">
                    <span class="check"></span>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <pagination
            v-if="active == 0"
            :total="proTotal"
            :pageSizes="pageSizes"
            :page.sync="filterProductParams.pageNum"
            :limit.sync="filterProductParams.pageSize"
            @pagination="getDevProductList"
          />
          <div>
            <pagination
              v-if="active == 1"
              :total="deviceTotal"
              :pageSizes="pageSizes2"
              :page.sync="filterParams.pageNum"
              :limit.sync="filterParams.pageSize"
              @pagination="getRulesDevList"
            />
          </div>
        </div>
        <div slot="footer">
          <div>
            <div v-if="active == 2">
              <el-button @click="stepChoiceDevice = false">取 消</el-button>
              <el-button type="primary" @click="finishChoice">确 定</el-button>
            </div>
            <div v-else>
              <el-button type="primary" style="margin-top: 12px" @click="next">下一步</el-button>
            </div>
          </div>
        </div>
    </el-dialog>
  </div>
</template>

<script>
import { DeviceList } from "@/api/rules/device";
import { productList,classTree } from "@/api/rules/productModel";
export default {
  name: 'rulesSelectDeviceWay',
  props:{
    productmap:{
        type:Map,
        default:()=>{
            return new Map()
        }
    },
    selProList:{
        type:Array,
        default:()=>{
            return []
        }
    }
  },
  data() {
    return {
        selProLists:this.selProList,
        stepChoiceDevice: false,
        //设备过滤条件
        proTotal: 0,
        devloading: false, //
        selDevList: [], //选中的设备
        selTopMsg: {}, //选中的触发类型
        active: 0,
        pageSizes: [6, 12, 18, 26],
        pageSizes2: [5, 10, 15, 25],
        selectForm: {},
        filterType: "product", //过滤类型
        filterConditions: "contain", //过滤条件
        filterValue: "", //过滤的值
        filterTypeList: [
            { label: "协议", value: "product" },
            { label: "设备名称", value: "name" },
            { label: "设备编码", value: "deviceCode" },
        ],
        conditionsList: [
            { label: "等于", value: "equal" },
            { label: "包含", value: "contain" },
        ],
        deviceTableData: [],
        configLoading: false,
        filterParams: {
            pageNum: 1,
            pageSize: 5,
            // showAll:true
            productId: null,
            Name: "",
            DeviceId: "",
        },
        deviceTotal: 0,
        select: [], //选中的设备
        nodes: [],
        DeviceNodes: [], //设备列表
        //设备过滤条件
        topicMsgList: [
            { value: "Offline", label: "设备离线", icon: "el-icon-close" },
            { value: "Online", label: "设备在线", icon: "el-icon-check" },
            // { value: "Upgrade", label: "更新固件" },
            {
            value: "ReadPropertyReply",
            label: "属性上报",
            icon: "el-icon-upload2",
            },
            {
            value: "Event",
            label: "设备事件",
            icon: "el-icon-data-line",
            }],
        devProductLists: [], //设备选择是产品列表
        filterProductParams: {
            pageNum: 1,
            pageSize: 6,
            IsNet: true,
            // showAll:true
            Name: "",
        },
        classmap: new Map(), //协议分类标记
    };
  },

  mounted() {
    this.getClassList();
  },
  computed: {
    deviceNodeList() {
      return this.DeviceNodes;
    },
  },
  methods: {
    initClassMap(node) {
      for (let idx = 0; idx < node.length; idx++) {
        let curnode = node[idx];
        this.classmap.set(node[idx].Id, curnode);
        if (
          curnode.hasOwnProperty("Children") &&
          curnode.Children &&
          curnode.Children.length > 0
        ) {
          this.initClassMap(curnode.Children);
        }
      }
    },
    getClassList() {
      //获取分类列表
      classTree().then((response) => {
        if (response.data.length > 0) {
          this.initClassMap(response.data);
        }
      });
    },
    openStepChoice() {
      //打开设备选择窗口
      this.stepChoiceDevice = true;
      this.getDevProductList();
    },
    finishChoice() {
      //完成设备选择
      this.triggerLs = []; //每次重新设置选中的设备时先将选中的设备清空
      if (this.selTopMsg && this.selTopMsg.value) {
      } else {
        this.$message.warning("请先选择消息订阅类型");
        return;
      }
      if (
        this.selProLists &&
        this.selProLists.length > 0 &&
        this.selDevList &&
        this.selDevList.length > 0 &&
        this.selTopMsg &&
        this.selTopMsg.value
      ) {
        for (let i = 0; i < this.selDevList.length; i++) {
          let this_device =
            "/" + this.selProLists[0].Id + "/" + this.selDevList[i].DeviceId;
          let obj = {
            topicDevice: this_device,
            topicMsg: this.selTopMsg.value,
          };
          this.triggerLs.push(obj);
        }
      } else {
        this.$message.warning("请先选择触发类型");
        return;
      }
      this.stepChoiceDevice = false;
      this.evtCode = "";
    //   this.initProEvts();
      this.$emit('selectTopicMsg',this.selTopMsg)
      this.$emit('selectTriggerLs',this.triggerLs)
      this.$emit('selectProduct',this.selProLists)
      this.$emit('selectDeviceList',this.selDevList)
      this.$emit('initProEvts')
    },
    reselectProduct() {
      //重新选择协议
      this.active = 0;
      this.filterType = "product";
    },
    reselectDevice() {
      //重新选择设备
      this.active = 1;
      this.filterType = "name";
    },
    changeFilterType(val) {
      //根据选中的筛选条件选择
      this.filterParams.DeviceId = "";
      this.filterParams.Name = "";
      this.filterProductParams.Name = "";
      if (val == "name") {
        this.filterConditions = "contain";
      } else {
        this.filterConditions = "equal";
      }
    },
    retLoad() {
      //重置搜索
      this.filterParams = {
        pageNum: 1,
        pageSize: 6,
        GroupId: null,
        productId: null,
        Name: "",
        DeviceId: "",
      };
      this.getRulesDevList();
    },
    retProductLoad() {
      this.filterProductParams = {
        pageNum: 1,
        pageSize: 6,
        Name: "",
      };
      this.getDevProductList();
    },
    next() {
      if (this.active == 0) {
        //第一步先选择协议
        if (this.selProLists.length == 1) {
          this.active = 1;
          this.filterType = "name";
          let that = this;
          this.$nextTick(() => {
            that.getRulesDevList();
          });
        } else {
          this.$message.warning("请先选择产品");
          return;
        }
      } else if (this.active == 1) {
        //第二步选择设备
        if (this.selDevList.length > 0) {
          this.active = 2;
        } else {
          this.$message.warning("请先选择设备");
          return;
        }
      }
    },
    getRulesDevList() {//获取所有设备列表
      this.devloading = true;
      DeviceList(this.filterParams)
        .then((rsp) => {
          // console.log("选择设备控件", rsp);
          this.devloading = false;
          let list = [];
          list = [
            ...[{ Id: "-1", DeviceId: "-1", Name: "全部设备" }],
            ...rsp.data.List,
          ];
          this.DeviceNodes = JSON.parse(JSON.stringify(list));
          this.deviceTotal = rsp.data.Total + 1;

          this.selectDevToLeft();
          this.$forceUpdate();
        })
        .catch((err) => {
          this.devloading = false;
          this.$message.error("接口异常");
        });
    },
    selectProToLeft() {//确定产品是否选中
      this.devProductLists.forEach((node) => {
        if (this.selProLists.length > 0) {
          for (let i = 0; i < this.selProLists.length; i++) {
            if (this.selProLists[i].Id === node.Id) {
              node.selected = true;
            } else {
              node.selected = false;
            }
          }
        } else {
          node.selected = false;
        }
      });
    },
    selectDevToLeft() {
      //设置每台设备的选中状态
      this.DeviceNodes.forEach((node) => {
        if (this.selDevList.length > 0) {
          for (let i = 0; i < this.selDevList.length; i++) {
            if (this.selDevList[i].Id === node.Id) {
              node.selected = true;
              // break;
            }
          }
        } else {
          node.selected = false;
        }
      });
    },
    selectProduct(node, inx) {
      //选择协议
      if (this.selProLists[0] && node.Id == this.selProLists[0].Id) {
        this.selProLists = [];
      } else {
        this.selProLists = [node];
      }
      this.selectProToLeft();
      this.$forceUpdate();
      if (this.selProLists.length > 0) {
        this.filterParams.productId = this.selProLists[0].Id;
      }
    },
    selectDevice(node, inx) {
      //选择设备
      if (node.Id == "-1") {
        //如果点击的是全部，
        if (node.selected) {
          this.selDevList = [];
        } else {
          this.DeviceNodes.map((node) => {
            node.selected = false;
          });
          node.selected = true;
          this.selDevList = [];
          this.selDevList.push(node);
        }
        this.selectDevToLeft();
        this.$forceUpdate();
      } else {
        if (this.selDevList[0] && this.selDevList[0].Id == "-1") {
          //如果点击的不是全部，选择其他设备之前需将选中的设备列表置空
          this.selDevList = [];
          this.DeviceNodes.map((node) => {
            node.selected = false;
          });
        }
        if (this.selDevList.length <= 20) {
          if (node.selected) {
            node.selected = false;
            for (let i = 0; i < this.selDevList.length; i++) {
              if (this.selDevList[i].Id == node.Id) {
                this.selDevList.splice(i, 1);
                break;
              }
            }
          } else {
            node.selected = true;
            this.selDevList.push(node);
          }
          this.selectDevToLeft();
          this.$forceUpdate();
        } else {
          if (node.selected) {
            node.selected = false;
            for (let i = 0; i < this.selDevList.length; i++) {
              if (this.selDevList[i].Id == node.Id) {
                this.selDevList.splice(i, 1);
                break;
              }
            }
          }
          this.selectDevToLeft();
          this.$forceUpdate();
        }
      }
    },
    selectTopicMsg(node, inx) {
      //选择消息订阅类型
      this.selTopMsg = JSON.parse(JSON.stringify(node));
      this.$forceUpdate();
    },
    getDevProductList() {//获取协议列表
      productList(this.filterProductParams).then(async (response) => {
        this.devProductLists = response.data.List;
        this.proTotal = response.data.Total;
        this.selectProToLeft();
      });
    },
  },
};
</script>