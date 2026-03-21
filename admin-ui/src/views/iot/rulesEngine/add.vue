<template>
  <div :style="{height: activeSelect == 'formSetting' ? bodyConHei + 'px' : '',position: 'relative',}" v-loading="onLoading">
    <div style="position: absolute; top: 0; left: 0; width: 100%; z-index: 2">
      <div style="height: 2px;width:100%;background-color: #dadada;"></div>
      <div class="header">
        <el-menu
          :default-active="activeSelect"
          active-text-color="#409eff"
          class="el-menu-demo shejiqi"
          mode="horizontal"
          @select="handleSelect"
        >
          <el-menu-item index="baseSetting" @click="to('baseSetting')">基础信息</el-menu-item>
          <el-menu-item index="formSetting" @click="to('formSetting')">执行动作</el-menu-item>
        </el-menu>
        <div class="publish">
          <el-button type="primary" @click="preview" size="mini" v-if="activeSelect == 'formSetting'">
            <i class="el-icon-view"></i>调试
          </el-button>
          <el-button type="warning" v-if="setup.Id>0&&setup.CreatedFrom!='mobile'" @click="resetRule" size="mini">
            <i class="el-icon-refresh-right"></i>重置规则
          </el-button>
          <el-button type="primary" @click="saveRules" size="mini" style="border-radius: 20px" v-if="setup.CreatedFrom!='mobile'">
            <i class="el-icon-s-promotion"></i>{{ setup.Id ? "保存规则" : "创建规则" }}
          </el-button>
        </div>
        <div class="scale">
          <el-button icon="el-icon-plus" @click="addScale" size="mini" :disabled="scale >= 150" circle></el-button>
          <span>{{ scale }}%</span>
          <el-button icon="el-icon-minus" @click="reduceScale" size="mini" :disabled="scale <= 40" circle></el-button>
          <el-button type="primary" size="mini" class="putbutton">
            <i class="zhongtaiiconfont zhongtai-icon-daoru" style="font-size:12px"></i>导入规则<input type="file" @change="importProcess" id="putbuttonFile"/>
          </el-button>
          <el-button type="primary" @click="exportProcess" size="mini" class="putbutton">
            <i class="zhongtaiiconfont zhongtai-icon-daochu" style="font-size:12px"></i>导出规则
          </el-button>
        </div>
      </div>
    </div>
    <div style="background: #f5f6f6" id="con_div" class="con_div">
      <div v-show="activeSelect == 'baseSetting'" style="padding: 20px 20px 0 20px">
        <div class="basic elbiaoge_elform3" id="elbiaoge_elform3">
          <addForm ref="addForm" @editParams="editParams" @openParamsDrawer="openParamsDrawer" @handleShowCron="handleShowCron" @setAddForm="setAddForm" :groupTreeList="groupTreeList" :setup="setup" :productmap="productmap" :device="device" :product="product" :devicemap="devicemap"></addForm>
          <div class="outline" :style="{ height: scrollHei + 80 + 'px', 'margin-left': '12px' }">
            <div class="content_con">
              <h1>1.概述</h1>
            </div>
          </div>
        </div>
      </div>
      <div v-show="activeSelect == 'formSetting'" class="action_con">
        <div class="action_left" id="action_left">
          <!-- <el-main> -->
          <div class="action" :style="{ 'padding-left': showConsole ? '400px' : 0 }">
            <div v-dragscroll class="vue-drag-scroll-out-wrapper">
              <div class="vue-drag-scroll-wrapper" :style="zoomStye" style="box-sizing: border-box">
                <div style="display: flex">
                  <process-tree ref="process-tree" @selectedNode="nodeSelected"/>
                </div>
              </div>
            </div>
          </div>
          <el-drawer
            :title="selectedNode.name"
            :visible.sync="showConfig"
            :modal-append-to-body="false"
            :size="selectedNode.type === 'CONDITION' ? '600px' : '500px'"
            direction="rtl"
            :modal="true"
            :wrapperClosable="true"
            destroy-on-close
          >
            <div slot="title">
              <el-input v-model="selectedNode.name" v-show="showInput" style="width: 300px" @blur="showInput = false"></el-input>
              <el-link v-show="!showInput" @click="showInput = true" style="font-size: medium">
                <i class="el-icon-edit" style="margin-right: 10px"></i>
                {{ selectedNode.name }}
              </el-link>
            </div>
            <div class="node-config-content">
              <node-config />
            </div>
          </el-drawer>
          <el-drawer
            title="调试"
            :visible.sync="showConsole"
            :modal-append-to-body="false"
            size="400px"
            direction="ltr"
            :modal="false"
            :destroy-on-close="true"
            :wrapperClosable="false"
            class="console_con"
            @close="closeDebug"
          >
            <div slot="title">
              <div style="margin-bottom: 10px">
                <span>调试</span>
                <div style="padding-left: 30px; display: inline-block">
                  <el-button
                    type="primary"
                    @click="startConsole"
                    v-if="isConnent"
                    >启动调试</el-button
                  >
                  <el-button
                    type="danger"
                    @click="stopConsole"
                    v-if="!isConnent"
                    >停止调试</el-button
                  >
                  <el-button
                    type="primary"
                    @click="clearLog"
                    style="margin-left: 15px"
                    >清空</el-button
                  >
                </div>
              </div>
            </div>
            <div style="padding: 20px">
              <el-row style="min-height: 500px">
                <el-col>
                  <mq-console
                    ref="mq-console"
                    :xkey="'zrule' + rulesFrom.id"
                    :isSubs.sync="isConnent"
                  ></mq-console>
                </el-col>
              </el-row>
            </div>
          </el-drawer>
          <el-drawer
            title="添加规则节点"
            direction="rtl"
            size="350px"
            :modal-append-to-body="false"
            :modal="true"
            :destroy-on-close="true"
            :wrapperClosable="false"
            :visible="isShowNodeList"
            class="setRulesList"
            @close="$store.commit('setShowNodeList', false)"
          >
            <insert-button @insertNode="insertNode"></insert-button>
          </el-drawer>
        </div>
      </div>
    </div>
    <cronTime ref="cronTime" @finishTimeChoice="finishTimeChoice"></cronTime>
    <paramsAdd ref="paramsDialog" :typeList="typeList" :triggerWay="setup.TriggerWay" :HttpParams="setup.HttpParams" @joinParams="joinParams"></paramsAdd>
  </div>
</template>

<script>
import MqConsole from "../debug/console.vue";
import ProcessTree from "@/views/iot/rulesFlowable/definition/layout/process/ProcessTree.vue";
import NodeConfig from "@/views/iot/rulesFlowable/common/process/config/NodeConfig";
import InsertButton from "../rulesFlowable/common/InsertButton.vue";
import { editRuselServe, getRuselDetail,groupTree,resetRusel,debugOn,debugOff } from "@/api/rules/ruselSevic";
import { productInfo } from "@/api/rules/productModel";
import { DeviceList } from "@/api/rules/device";
import addForm from './addForm'
import cronTime from './cron_time'
import paramsAdd from './params_add'
export default {
  name: "RulesEngineAdd",
  components: {
    ProcessTree,
    NodeConfig,
    MqConsole,
    InsertButton,
    addForm,
    cronTime,
    paramsAdd
  },
  props: {
    value: {
      type: String,
      default: "baseSetup",
    },
  },
  data() {
    return {
      typeList:[{'alabel': "整型", 'label': "整型(Int)", 'value': "int"},
        {'alabel': "浮点", 'label': "浮点型(Float)", 'value': "float"},
        {'alabel': "字符", 'label': "字符型(String)", 'value': "string"},
        {'alabel': "时间", 'label': "时间型(Date)", 'value': "date"},
        {'alabel': "布尔", 'label': "布尔型(Boolean)", 'value': "boolean"},
        {'alabel': "枚举", 'label': "枚举型(Enum)", 'value': "enum"}],//数据类型列表
      onLoading: false, //是否正在加载数据
      devicemap: new Map(), //设备
      productmap: new Map(),
      isConnent: false,
      // isShowNodeList: false,
      showConsole: false, //是否显示调试框
      scale: 100, //缩放
      activeSelect: "baseSetting", //切换页面
      selected: {},
      showInput: false,
      showConfig: false,
      device: [], //选中的设备
      allDevice: "全部设备", //全部设备
      product: "", 
      productLabel: "product",
      // 是否显示Cron表达式弹出层
      openCron: false,
      // 传入的表达式
      timerCron: "",
      topicMsgList: [
        { value: "Offline", label: "设备离线", icon: "el-icon-close" },
        { value: "Online", label: "设备在线", icon: "el-icon-check" },
        // { value: "Upgrade", label: "更新固件" },
        {
          value: "PropReply",
          label: "属性上报",
          icon: "el-icon-upload2",
        },
        {
          value: "Event",
          label: "设备事件",
          icon: "el-icon-data-line",
        },
      ],
      rulesFrom: {
        id: null, //规则唯一标识id
      },
      scrollHei: 0, //右侧滚动的高度
      isShowNode: false,
      zoomStye: {
        width: `${document.body.offsetWidth}px`,
        height: `${document.body.offsetHeight - 38}px`,
        transform: `scale(${100 / 100})`,
        "padding-top": `60px`,
        "padding-bottom": `30px`,
      },
      bodyConHei: 0,
      deviceForm: {
        ProductId: 0,
        showAll: true,
      }, //设备查询form
      groupTreeList:[]//规则分组列表
    };
  },
  computed: {
    isShowNodeList() {
      return this.$store.state.rulesFlowable.isShowNodeList;
    },
    setup() {
      console.log(this.$store.state.rulesFlowable.rulesDesign,'this.$store.state.rulesFlowable.rulesDesign');
      return this.$store.state.rulesFlowable.rulesDesign; //规则流程设计
    },
    selectedNode() {
      return this.$store.state.rulesFlowable.rulesSelectedNode; //选中的规则节点
    },
    SelectedParentNode() {
      return this.$store.state.rulesFlowable.rulesSelectedParentNode; //添加节点时父级节点
    },
  },
  created() {
    this.check();
    // console.log("建立了");
  },
  beforeDestroy() {
    // 销毁
    window.removeEventListener("resizeRules",()=>{});
    window.removeEventListener("resizeRules",()=>{});
    this.closeDebug();
  },
  mounted() {
    window.addEventListener("resizeRules", () => {
      this.Refresh(); //浏览器宽高发生变化后重新设置页面的样式
    });
    window.addEventListener("resizeRules", () => {
      this.setRulesFormHei();
    });
    //第一次创建页面时执行
    this.getGroupList()
    if (this.$route.query.id) {
      //进入添加页面获取链接参数，判断是添加还是修改
      this.rulesFrom.id = this.$route.query.id;
      this.loadFormInfo(this.rulesFrom.id);
    } else {
      this.loadInitFrom();
    }
    //第一次创建页面时执行

    if (document.body.offsetWidth <= 970) {
      this.$msgbox.alert(
        "本设计器未适配中小屏幕，建议您在PC电脑端浏览器进行操作"
      );
    }
    this.listener();
    this.setRulesFormHei();
  },

  methods: {
    editParams(row, rowIndex) {
      //编辑添加的参数
      this.$refs.paramsDialog.editParams(row, rowIndex)
    },
    openParamsDrawer(){//添加参数
        this.$refs.paramsDialog.openParamsDrawer()
    },
    handleShowCron(){//定时表达式生成初始化值设置
        this.$refs.cronTime.handleShowCron(this.setup.TimerCron)
    },
    finishTimeChoice(val){
        //生成的定时表达式结果
        this.$refs.addForm.finishTimeChoice(val)
    },
    joinParams(paramList){
        this.setup.HttpParams=JSON.parse(JSON.stringify(paramList))
    },
    setAddForm(val){
      this.$store.commit("rulesloadForm", val);
    },
    getGroupList() {
      //获取规则分组列表
      groupTree().then((res) => {
        // console.log("分组列表", res);
        if (res.code == 0) {
          let lists = [];
          lists = JSON.parse(JSON.stringify(res.data));
          this.groupTreeList = lists;
        }
      });
    },
    processReadFile(file) {
      //读取导入参数的值
      const reader = new FileReader();
      reader.onload = (e) => {
        try {
          let jsonArr = JSON.parse(e.target.result);
          this.setup.Process = jsonArr.ProcessStr;
          this.setup.HttpParams = jsonArr.HttpParamsStr;
          // console.log(jsonArr, "导入的json", this.setup.Process);
        } catch (error) {
          console.error("Error parsing JSON", error);
          if (this.setup.Process) {
          } else {
            this.setup.Process = {
              id: "root",
              parentId: null,
              type: "ROOT",
              name: "发起人",
              desc: "任何人",
              children: {},
            };
          }
          if (this.setup.HttpParams) {
          } else {
            this.setup.HttpParams = [];
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
      this.processReadFile(file);
    },
    exportProcess() {
      //导出执行参数
      // let onlynum = new Date().getTime();
      let onlynum = this.setup.Name;
      // console.log("时间", onlynum, this.setup.Process);
      let jsonStr = {};
      jsonStr.ProcessStr = this.setup.Process;
      jsonStr.HttpParamsStr = this.setup.HttpParams;
      const json = JSON.stringify(jsonStr);
      const blob = new Blob([json], { type: "application/json" });
      const url = URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = onlynum;
      link.click();
      URL.revokeObjectURL(url);
    },
    clearLog() {
      this.$refs["mq-console"].clear();
    },
    initDeviceMap() {
      if (!this.device.includes("-1")) {
        DeviceList({ DtuIds: this.device, showAll: true }).then((rsp) => {
          for (let idx = 0; idx < rsp.data.List.length; idx++) {
            let curnode = rsp.data.List[idx];
            this.devicemap.set(curnode.DeviceId, curnode);
          }
        });
      }
    },
    setRulesFormHei() {
      //设置规则设计中表单部分的样式
      if (this.activeSelect == "baseSetting") {
        let div = document.getElementById("from_ul");
        // let div2 = document.getElementById("elbiaoge_elform3");
        let div3 = document.getElementById("app-main");
        this.bodyConHei = div3.offsetHeight - 120;
        this.scrollHei = div.offsetHeight + 30;
        // console.log(div2.offsetHeight,div.offsetHeight,this.scrollHei,'div.offsetHeight');
        this.$forceUpdate();
      }
    },
    insertNode(type) {
      //执行子组件processTree的插入组件事件
      this.$refs["process-tree"].insertNode(type, this.SelectedParentNode); //使用ref调用子组件process-tree的事件
      this.$store.commit('setShowNodeList', false);
    },
    stopConsole() {
      //停止调试
      return this.$refs["mq-console"].disconnect();
    },
    startConsole() {
      //启用调试
      return this.$refs["mq-console"].connect();
    },
    closeDebug(){
      if(this.rulesFrom.id!=null){
        debugOff(this.rulesFrom.id);
      }
    },
    preview() {
      if(this.rulesFrom.id!=null){
        debugOn(this.rulesFrom.id);
      }
      //开启调试控制台
      this.showConsole = !this.showConsole;
      this.Refresh();
    },
    addScale() {
      //放大
      this.scale += 10;
      this.Refresh();
    },
    reduceScale() {
      //缩小
      this.scale -= 10;
      this.Refresh();
    },
    Refresh() {
      //设置规则流程的宽高
      if (this.activeSelect == "formSetting" && this.isShowNode) {
        // let div = document.getElementById("action_left");
        let INIT_WIDTH = document.body.offsetWidth;
        let INIT_HEIGHT = document.body.offsetHeight - 38;
        let width = INIT_WIDTH * (1 + (100 - this.scale) / 100);
        let height = INIT_HEIGHT * (1 + (100 - this.scale) / 100);
        this.zoomStye = {
          width: `${width}px`,
          height: `${height}px`,
          transform: `scale(${this.scale / 100})`,
          "padding-top": `${(60 * this.scale) / 100}px`,
          "padding-bottom": `${(30 * this.scale) / 100}px`,
          "padding-left": `${(50 * this.scale) / 100}px`,
          "padding-right": `${(30 * this.scale) / 100}px`,
        };
      }
    },
    resetRule(){
      this.$modal
        .confirm('是否确认重置参数与定时器？')
        .then(()=> {
          resetRusel(this.setup.Id).then(() => {
            this.$message.success("操作成功");
          })
          .catch(() => {});
        });
        
    },
    saveRules() {
      const loading = this.$loading({
        lock: true,
        text: "验证规则中...",
        spinner: "el-icon-loading",
        background: "rgba(0, 0, 0, 0.7)",
      });

      let erss = this.validate();
      loading.close();
      if (erss.length > 0) {
        this.$alert(erss.join(), "规则错误", {
          confirmButtonText: "确定",
          type: "error",
        });
        return;
      }
      if (this.$isNotEmpty(this.setup.Id)) {
        this.$refs.addForm.validateForm((valid) => {
          if (valid) {
            let template = {
              Id: this.setup.Id,
              Name: this.setup.Name,
              Sort: this.setup.Sort ? this.setup.Sort : 0,
              RuleJson: JSON.stringify(this.setup.Process),
              Remark: this.setup.Remark,
              TimerCron: this.setup.TimerCron,
              GroupId:this.setup.GroupId,
              CreatedFrom:'pc',
            };
            template.HttpParams = JSON.stringify(this.setup.HttpParams);
            editRuselServe(template)
              .then((rsp) => {
                this.$message.success("规则更新成功");
              })
              .catch((err) => {
                this.$message.error(err);
              });
          }
        })
        
      }
    },
    async getProductAttr(productId) {
      let rsp = await productInfo({ id: productId });
      if (rsp.code == 0) {
        this.productmap.set(rsp.data.Id, rsp.data);
        let jsonLis = JSON.parse(rsp.data.ModelTSL);
        if (jsonLis.properties) {
          this.$store.commit("setProductAttrList", jsonLis.properties);
        }
        if (jsonLis.functions) {
          this.$store.commit("setProductFuncList", jsonLis.functions);
        }
        if (jsonLis.events) {
          this.$store.commit("setProductEventList", jsonLis.events);
        }
        if (jsonLis.tags) {
          this.$store.commit("setProductTagList", jsonLis.tags);
        }
        if (productId) {
          this.$store.commit("setSelectProductInfo", rsp.data);
        }
        this.$forceUpdate();
        return jsonLis;
      }
    },
    async loadFormInfo(rulseId) {
      //如果是编辑规则流程，则显示已有规则
      this.onLoading = true;
      let rsp = await getRuselDetail({ id: rulseId });
      let rulse = {};
      rulse.Id = rsp.data.Id;
      rulse.Process = JSON.parse(rsp.data.RuleJson);
      rulse.Name = rsp.data.Name;
      rulse.Sort = rsp.data.Sort;
      rulse.Remark = rsp.data.Remark;
      rulse.TriggerWay = rsp.data.TriggerWay;
      rulse.GroupId=rsp.data.GroupId
      rulse.TriggerList = rsp.data.TriggerList;
      rulse.CreatedFrom=rsp.data.CreatedFrom
      let tmptsl = null;
      if (rulse.TriggerList.length > 0) {
        this.product = rulse.TriggerList[0].TopicDevice.split("/")[1];
        this.deviceForm.ProductId = this.product;
        for (let j = 0; j < rulse.TriggerList.length; j++) {
          let spLs = rulse.TriggerList[j].TopicDevice.split("/");
          if(spLs[2]){
            this.device.push(spLs[2]);
          }
          
        }
        this.initDeviceMap();

        if(this.deviceForm.ProductId&&this.deviceForm.ProductId!='-1'){
          tmptsl = await this.getProductAttr(this.deviceForm.ProductId);
        }
        
      } else {
        this.$store.commit("setProductAttrList", []);
        this.$store.commit("setProductFuncList", []);
        this.$store.commit("setProductEventList", []);
        this.$store.commit("setProductTagList", []);
        this.$store.commit("setSelectProductInfo",null);
      }

      if (rulse.TriggerWay == 0) {
        this.topicMsgList.map((its) => {
          if (rulse.TriggerList[0].TopicMsg == its.value) {
            rulse.TopicMsg = its;
          } else {
            if (
              rulse.TriggerList[0].TopicMsg.indexOf("Event") == 0 &&
              tmptsl != null
            ) {
              let tmpccc = rulse.TriggerList[0].TopicMsg.substr(6);
              let tmpeeee = tmptsl.events.filter((x) => x.code == tmpccc);
              if (tmpeeee.length > 0) {
                rulse.TopicMsg = its;
                rulse.TopicMsg.code=tmpccc;
                rulse.TopicMsg.label =
                  its.label + "【" + tmpeeee[0].name + "】";
              }
            }
          }
        });
      }
      rulse.TimerCron = rsp.data.TimerCron;
      rulse.CronName = rsp.data.CronName;
      if (rsp.data.HttpParams == null || rsp.data.HttpParams == "") {
        rulse.HttpParams = [];
      } else {
        rulse.HttpParams = JSON.parse(rsp.data.HttpParams);
      }
      this.$store.commit("rulesloadForm", rulse);
      this.$forceUpdate();
      this.onLoading = false;
    },
    loadInitFrom() {
      this.$store.commit("rulesloadForm", {
        Id: null,
        Name: null,
        TriggerWay: 0,
        TopicDevice: "All",
        TopicMsg: null,
        TimerCron: "",
        Sort: 0,
        Process: {
          id: "root",
          parentId: null,
          type: "ROOT",
          name: "发起人",
          desc: "任何人",
          children: {},
        },
        Remark: "",
      });
    },
    //流程图
    nodeSelected(node) {
      // console.log("配置节点", node);
      this.showConfig = true;
    },
    validate() {
      return this.$refs["process-tree"].validateProcess();
    },
    //流程图
    publish() {
      this.$emit("publish");
    },
    exit() {
      this.$confirm("未发布的内容将不会被保存，是否直接退出 ?", "提示", {
        confirmButtonText: "退出",
        cancelButtonText: "取消",
        type: "warning",
      }).then(() => {
        //window.location.reload()
        //this.$store.commit('clearTemplate')
        this.$router.push("/iot/rulesEngine/index");
        this.$store.dispatch("tagsView/delView", this.$route);
      });
    },
    to(path) {
      this.activeSelect = path;
      if (path == "formSetting") {
        this.$nextTick(() => {
          this.isShowNode = true;
        });
      } else {
        this.$nextTick(() => {
          this.isShowNode = false;
        });
      }
    },
    handleSelect(key, keyPath) {
      // console.log(key, keyPath);
    },
    listener() {
      window.onunload = this.closeBefore();
      window.onbeforeunload = this.closeBefore();
      //window.on('beforeunload',this.closeBefore())
    },
    closeBefore() {
      //alert("您将要离开本页")
      return false;
    },
    check() {
      if (this.$store.state.rulesFlowable.rulesIsEdit === null) {
        //this.$router.push("/workPanel");
      }
    },
  },
};
</script>
<style lang="less" scoped>

/deep/ .el-drawer__wrapper.console_con {
  // right: 0;
  position: absolute;
  top: 0;
  left: 0;
  width: 400px;
  height: calc(100vh - 163px);
  border-radius: 0 4px 0 0;
  //   .el-drawer.rtl{
  //     overflow: scroll;
  // }
  border-top: 1px solid #bbc0cd;
  border-right: 1px solid #bbc0cd;
  .el-drawer__container {
    width: 400px;
    .el-drawer__header {
      align-items: flex-start;
      margin-bottom: 0;
    }
  }
  .el-select {
    width: 260px;
    input.el-input__inner {
      width: 260px;
      height: 36px;
      line-height: 36px;
    }
  }
  .el-form-item label.el-form-item__label {
    height: 36px;
    line-height: 36px;
  }
}

.vue-drag-scroll-out-wrapper {
  overflow-x: hidden;
  width: 100%;
  height: 100%;
  cursor: grab;
  // position: absolute;
  // top: 40px;
  // left: 0;
  &::-webkit-scrollbar {
    width: 0 !important;
  } // 隐藏垂直方向的滚动条
}
.app-main {
  position: absolute;
  top: 110px;
  left: 0;
}
/deep/ .header {
  //   min-width: 980px;
  background-color: #ffffff;
  border-top: 1px solid #dadada;
  width: 100%;
  box-sizing: border-box;
  display: flex;
  align-items: center;
  height: 50px;

  position: relative;
  .el-menu {
    top: 0;
    // z-index: 999;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
  }
  .shejiqi {
    height: 38px;
    line-height: 38px;
    border: none;
  }
  .shejiqi .el-menu-item {
    padding: 0;
    margin: 0 25px;
    height: 38px;
    line-height: 38px;
  }
  .publish {
    position: absolute;
    top: 12px;
    right: 50px;
    // z-index: 1000;

    i {
      margin-right: 6px;
    }

    button {
      border-radius: 15px;
      margin-right: 10px;
    }
  }

  .back {
    position: absolute;
    // z-index: 1000;
    top: 15px;
    left: 20px;
    // font-size: small;
    .return_button {
      margin-right: 6px;
      background: #ffffff;
      color: #78829d;
      width: 18px;
      height: 18px;
      line-height: 18px;
      // font-size: 16px;
      padding: 0;
      text-align: center;
      border: 1px solid #78829d;
    }

    span {
      i {
        border-radius: 10px;
        padding: 7.8px;
        // font-size: 20px;
        color: #ffffff;
        margin: 0 10px;
      }
    }
  }
  .scale {
    // z-index: 999;
    position: absolute;
    left: 22px;
    top: 10px;

    span {
      margin: 0 10px;
      // font-size: 15px;
      color: #7a7a7a;
      width: 50px;
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
}
.con_div {
  height: 100%;
  box-sizing: border-box;
  padding-top: 49px;
}
.elbiaoge_elform3 {
  background-color: #ffffff;
  border-radius: 4px 4px 0 0;
  width: 100%;
  min-height: calc(100vh - 188px); //116px+20px+49px
  padding: 20px;
  box-sizing: border-box;
}
</style>
<style lang="less">
.basic {
  width: 100%;
  padding: 20px;
  box-sizing: border-box;
  display: flex;
  justify-content: space-between;
  ul {
    padding: 0;
    margin: 0 auto;
    li {
      list-style: none;
    }
  }
  
  .outline {
    width: 30%;
    height: 100%;

    padding: 24px;
    box-sizing: border-box;
    color: rgba(0, 0, 0, 0.8);
    // font-size: 14px;
    border: 1px solid #e0e0e0;
    .content_con {
      width: 100%;
      height: 100%;
      overflow: scroll;
      overflow-x: hidden;
      padding-top: 0;
    }
  }
  
}
.action_con {
  // display: flex;
  // justify-content: flex-start;
  width: 100%;
  height: 100%;
  position: relative;
  .action_left {
    width: 100%;
    height: 100%;
    box-sizing: border-box;
    // overflow-y: scroll;
    position: relative;
    // ::-webkit-scrollbar {
    //   width: 4px;
    //   height: 2px;
    //   background-color: white;
    // }

    // ::-webkit-scrollbar-thumb {
    //   border-radius: 16px;
    //   background-color: #e8e8e8;
    // }
  }

  .action {
    // margin-top: 30px;
    display: flex;
    transform-origin: 50% 0px 0px;
    height: 100%;
  }

  .node-config-content {
    padding: 0 20px 20px;
  }
  /deep/ .el-drawer__body {
    overflow-y: auto;
  }
  // ::-webkit-scrollbar {
  //   width: 4px;
  //   height: 2px;
  //   background-color: white;
  // }

  // ::-webkit-scrollbar-thumb {
  //   border-radius: 16px;
  //   background-color: #e8e8e8;
  // }
}
</style>
