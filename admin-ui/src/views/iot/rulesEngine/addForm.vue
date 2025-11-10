<template>
  <div :style="{'width': isEdit?'68%':'100%'}">
    <el-form
      :model="setup"
      :rules="rules"
      ref="ruleForm"
      label-position="top"
      class="ruleForm"
      id="ruleForm"
    >
      <ul class="from_ul" id="from_ul">
        <li class="form_li">
          <div class="form_content">
            <div class="title_before"></div>
            <el-form-item label="名称" prop="Name">
              <el-input v-model="setup.Name" placeholder="请输入名称"></el-input>
            </el-form-item>
          </div>
        </li>
        <li class="form_li">
          <div class="form_content">
            <div class="title_before"></div>
            <el-form-item label="规则优先级" prop="Sort">
              <el-input v-model="setup.Sort" @input="setup.Sort = setup.Sort.replace(/[^0-9]/g, '')" style="width: 200px" placeholder="请输入规则优先级"></el-input>
              <span class="item-desc">
                <i class="zhongtaiiconfont zhongtai-icon-zhuyi" style="color: #e74032"></i>值越小优先级越高
              </span>
            </el-form-item>
          </div>
        </li>
        <li class="form_li">
          <div class="form_content">
            <div class="title_before"></div>
            <el-form-item label="规则分组" prop="GroupId" :class="{ 'is-not-require': true }">
              <treeselect class="groupSet" v-model="setup.GroupId" :options="groupTreeList" :show-count="true" :normalizer="normalizer" placeholder="请选择设备分组" :disabled="false"/>
            </el-form-item>
          </div>
        </li>
        <li class="form_li">
          <div class="form_content">
            <div class="title_before"></div>
            <el-form-item label="触发方式">
              <div class="fun_ul disabled">
                <el-button class="fun_li" :class="{ active: setup.TriggerWay == 0 }" :disabled="isEdit"  @click="choice(0, false)">
                  <div class="fun_left">
                    <div class="fun_top">设备触发</div>
                    <div class="fun_bottom">DEVICE TRIGGER</div>
                  </div>
                  <div class="fun_right">
                    <img src="../../../assets/images/device.png" alt />
                  </div>
                </el-button>
                <el-button class="fun_li" :class="{ active: setup.TriggerWay == 1 }" :disabled="isEdit" @click="choice(1, false)">
                  <!--choice传的第二个参数为false表示没有被禁用，true表示被禁用 fun_li  disabled表示禁用-->
                  <div class="fun_left">
                    <div class="fun_top">Http触发</div>
                    <div class="fun_bottom">HTTP TRIGGER</div>
                  </div>
                  <div class="fun_right">
                    <img src="../../../assets/images/manual.png" alt />
                  </div>
                </el-button>
                <el-button class="fun_li" :class="{ active: setup.TriggerWay == 2 }" :disabled="isEdit" @click="choice(2, false)">
                  <!--choice传的第二个参数为false表示没有被禁用，true表示被禁用 fun_li  disabled表示禁用-->
                  <div class="fun_left">
                    <div class="fun_top">定时触发</div>
                    <div class="fun_bottom">TIMING TRIGGER</div>
                  </div>
                  <div class="fun_right">
                    <img src="../../../assets/images/timing.png" alt />
                  </div>
                </el-button>
              </div>
            </el-form-item>
          </div>
        </li>
        <li class="form_li">
          <div class="form_content">
            <div class="title_before"></div>
            <div class="form_list" v-if="setup.TriggerWay == 0">
              <el-form-item label="触发设备" prop="selectDevic" style="width: 100%" v-if="isEdit">
                <div class="select_devic_list" v-if="setup.TriggerList.length > 0">
                  <span>{{ productmap.get(product) ? productmap.get(product).Name : "该产品不存在" }}</span>
                  <span class="devic_lis">
                    <span v-for="its in device" :key="its">{{ its == "-1" ? "全部设备" : (devicemap.get(its) ? (devicemap.get(its).Name ? devicemap.get(its).Name + "、" : "该设备不存在") : "该设备不存在") }}</span>
                  </span>
                  <span>
                    <i :class="setup.TopicMsg ? (setup.TopicMsg.icon ? setup.TopicMsg.icon : '') : ''"></i>
                    {{ setup.TopicMsg ? (setup.TopicMsg.label ? setup.TopicMsg.label : "") : ""}}
                  </span>
                </div>
              </el-form-item>
              <el-form-item label="触发设备" prop="selectDevic" style="width: 100%" v-if="!isEdit">
                <div class="select_devic_list" v-if="selProList.length > 0 && selDevList.length > 0 && selTopMsg.value" @click="openStepChoice">
                    <span>{{ selProList[0].Name }}</span>
                    <span class="devic_lis">
                       <span v-for="(its, ix) in selDevList" :key="its.Id">{{ix == selDevList.length - 1 ? its.Name : its.Name + "、"}}</span>
                    </span>
                    <span><i :class="selTopMsg.icon"></i>{{ selTopMsg.label }}</span>
                </div>
                <div v-else class="add_select_devic" @click="openStepChoice">
                    +
                </div>
              </el-form-item>
              <el-form-item v-if="selTopMsg.value == 'Event'" label="触发事件" prop="selectEvent" style="width: 100%">
                  <el-select v-model="evtCode" placeholder="请选择事件" @change="evtCodeChange">
                    <el-option v-for="item in proEvt" :key="item.code" :label="item.name" :value="item.code"></el-option>
                  </el-select>
              </el-form-item>
              <el-form-item prop="HttpParams" label="执行参数" :rules="rules.HttpParams" style="width: 60%">
                <div v-if="setup.HttpParams && setup.HttpParams.length > 0">
                  <div style="display: flex;flex-direction: row;justify-content: space-between;background-color: #f5f7fa;margin: 5px 0;" v-for="(items, inx) in setup.HttpParams" :key="inx">
                    <div style="padding-left: 15px">
                      <span>{{ items.name }}【{{ items.code }}】</span>
                      <span class="params_type">{{ typeListMap.get(items.type) ? typeListMap.get(items.type).label : "" }}</span>
                    </div>
                    <div>
                      <i class="el-icon-edit" @click="editParams(items, inx)" style="margin-right: 15px; cursor: pointer"></i>
                      <i class="el-icon-delete" @click="deleteParams(items, inx)" style="margin-right: 15px; cursor: pointer"></i>
                    </div>
                  </div>
                </div>
                <span style="color: #0055ff; cursor: pointer" @click="openParamsDrawer()">+输入参数</span>
              </el-form-item>
            </div>
            <div class="form_list" v-if="setup.TriggerWay == 1">
              <el-form-item prop="HttpParams" label="HTTP触发参数" :rules="rules.HttpParams">
                <div v-if="setup.HttpParams && setup.HttpParams.length > 0">
                  <div style="display: flex;flex-direction: row;justify-content: space-between;background-color: #f5f7fa;margin: 5px 0;" v-for="(items, inx) in setup.HttpParams" :key="inx">
                    <div style="padding-left: 15px">
                      <span>{{ items.name }}【{{ items.code }}】</span>
                      <span class="params_type">{{ typeListMap.get(items.type) ? typeListMap.get(items.type).label : "" }}</span>
                    </div>
                    <div>
                      <i class="el-icon-edit" @click="editParams(items, inx)" style="margin-right: 15px; cursor: pointer"></i>
                      <i class="el-icon-delete" @click="deleteParams(items, inx)" style="margin-right: 15px; cursor: pointer"></i>
                    </div>
                  </div>
                </div>
                <span style="color: #0055ff; cursor: pointer" @click="openParamsDrawer()">+输入参数</span>
              </el-form-item>
            </div>
            <div class="form_list" v-if="setup.TriggerWay == 2">
              <el-form-item style="width: 100%" label="定时产品" :class="{ 'is-not-require': true }">
                <el-select v-model="selProduct" filterable remote reserve-keyword placeholder="请选择定时产品"
                  :remote-method="remoteMethod" :loading="prodloading" :disabled="isEdit">
                  <el-option v-for="item in productLists" :key="item.Id" :label="item.Name" :value="item.Id">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item style="width: 100%" prop="TimerCron" label="定时时间" :rules="rules.TimerCron">
                <el-input style="width: 460px" :readonly="true" v-model="setup.CronName" placeholder="请选择定时时间">
                  <template slot="append">
                    <el-button type="primary" @click="handleShowCron">
                      设置时间
                      <i class="el-icon-time el-icon--right"></i>
                    </el-button>
                  </template>
                </el-input>
              </el-form-item>
              <el-form-item prop="HttpParams" label="执行参数">
                <div v-if="setup.HttpParams && setup.HttpParams.length > 0">
                  <div style="display: flex;flex-direction: row;justify-content: space-between;background-color: #f5f7fa;margin: 5px 0;" v-for="(items, inx) in setup.HttpParams" :key="inx">
                    <div style="padding-left: 15px">
                      <span>{{ items.name }}【{{ items.code }}】</span>
                      <span class="params_type">{{ typeListMap.get(items.type) ? typeListMap.get(items.type).label : "" }}</span>
                    </div>
                    <div v-if="inx > 1">
                      <i class="el-icon-edit" @click="editParams(items, inx)" style="margin-right: 15px; cursor: pointer"></i>
                      <i class="el-icon-delete" @click="deleteParams(items, inx)" style="margin-right: 15px; cursor: pointer"></i>
                    </div>
                  </div>
                </div>
                <span style="color: #0055ff; cursor: pointer" @click="openParamsDrawer()">+输入参数</span>
              </el-form-item>
            </div>
          </div>
        </li>
        <li class="form_li">
          <div class="form_content">
            <div class="title_before"></div>
            <el-form-item label="说明" prop="text" :class="{ 'is-not-require': true }">
              <el-input type="textarea" v-model="setup.Remark" placeholder="请输入说明" show-word-limit maxlength="200" :autosize="{ minRows: 2, maxRows: 5 }"></el-input>
            </el-form-item>
          </div>
        </li>
      </ul>
    </el-form>
    
  </div>
</template>

<script>
import { productInfo } from "@/api/rules/productModel";
import {toCronDes} from "@/api/monitor/job";
import { productList } from "@/api/rules/productModel";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
export default {
  name: "rulesAddForm",
  components: {
    Treeselect
  },
  props:{
    isEdit:{
        type:Boolean,
        default:true
    },
    groupTreeList:{
        type:Array,
        default:()=>{
            return []
        }
    },
    setup:{
        type:Object,
        default:()=>{
            return {
                id: null, //规则唯一标识id
                Name: "", //名称
                TimerCron: "",
                CronName:"",
                Sort: 0,
                TriggerWay: 0, //触发方式
                device: [], //选中的设备
                product: "", //订阅的设备 产品
                TopicMsg: "", //订阅的消息类型
                HttpParams: [],
                GroupId:null,
                process: {
                    id: "root",
                    parentId: null,
                    type: "ROOT",
                    name: "发起人",
                    desc: "任何人",
                    children: {},
                },
                remark: "备注说明",
            }
        }
    },
    productmap:{
        type:Map,
        default:()=>{
            return new Map()
        }
    },
    devicemap:{
        type:Map,
        default:()=>{
            return new Map()
        }
    },
    device:{
        type:[Array,Object]
    },
    product:{//订阅的设备 产品
        type:String
    },
  },
  data() {
    return {
      selProduct:'',
      productLists:[],
      prodloading:false,
      evtCode:'',
      proEvt: [],//事件属性列表
      selDevList:[],
      selProList: [], //选中的产品
      selTopMsg: {}, //选中的触发类型
      //http触发相关参数
      typeListMap: new Map(),
      typeList:[{'alabel': "整型", 'label': "整型(Int)", 'value': "int"},
        {'alabel': "浮点", 'label': "浮点型(Float)", 'value': "float"},
        {'alabel': "字符", 'label': "字符型(String)", 'value': "string"},
        {'alabel': "时间", 'label': "时间型(Date)", 'value': "date"},
        {'alabel': "布尔", 'label': "布尔型(Boolean)", 'value': "boolean"},
        {'alabel': "枚举", 'label': "枚举型(Enum)", 'value': "enum"}],//数据类型列表
      //http触发相关参数
      rules: {
        name: [{ required: true, trigger: "blur", message: "请输入名称" }],
        Name: [{ required: true, trigger: "blur", message: "请输入名称" }],
        sort: [{ required: true, trigger: "blur", message: "请输入优先级" }],
        Sort: [{ required: true, trigger: "blur", message: "请输入优先级" }],
        product: [{ required: true, trigger: "change", message: "请选择产品" }],
        device: [{ required: true, trigger: "change", message: "请选择设备" }],
        topicMsg: [
          {
            required: true,
            trigger: "change",
            message: "请选择订阅的消息类型",
          },
        ],
        TimerCron: [
          { required: true, trigger: "change", message: "请输入表达式" },
        ],
      },
      queryform:{
        pageNum: 1,
        pageSize: 300,
        Name: "",
      }
    };
  },
  watch:{
    setup:{
        deep:true,
        handler(val, oldVal) {
          if(this.isEdit&&this.setup&&this.setup.TriggerWay==2){
            this.selProduct=this.product
            this.$forceUpdate()
          }
          this.$emit('setAddForm',val)
      }
    },
    selProduct:{
        deep:true,
        handler(val, oldVal) {
            this.$emit('setSelProduct',val)
      }
    },
  },
  mounted() {
    this.getproductList();
    this.$nextTick(()=>{
        this.setTypeMap()
        
    })
    
  },

  methods: {
    async getproductList() {
      this.prodloading = true;
      let response = await productList(this.queryform)
      if (response.code == 0) {
        if (response.data && response.data.List) {
          this.productLists = response.data.List;
          this.productLists.push({Id:null,Name:'【取消产品选择】'})
          this.$forceUpdate()
        }
      }
      this.prodloading = false;
    },
    async remoteMethod(query) {
      if (query !== '') {
        this.queryform.Name = query;
        await this.getproductList();
      }else{
        delete this.queryform.Name;
        await this.getproductList();
      }
    },
    evtCodeChange(val){
        //选择事件
        this.$emit('evtCodeChange',val)
    },
    validateForm(cb){
        if (this.$refs["ruleForm"]) {
        this.$refs["ruleForm"].validate((valid)=>{
            cb(valid)
        });
      }
    },
    selectProduct(list){
        this.selProList=JSON.parse(JSON.stringify(list))
        this.setup.product=list[0].Id
        this.$forceUpdate()
    },
    selectDeviceList(list){
        this.selDevList=JSON.parse(JSON.stringify(list))
        this.setup.device=JSON.parse(JSON.stringify(list))
        this.$forceUpdate()
    },
    selectTopicMsg(node){
        this.selTopMsg = JSON.parse(JSON.stringify(node));
        this.setup.TopicMsg= JSON.parse(JSON.stringify(node))
    },
    openStepChoice() {
      //打开设备选择窗口
      this.$emit('openStepChoice')
    },
    choice(mode, isDis) {
      //选择触发方式
      this.$refs.ruleForm.clearValidate(); //重新选择触发方式时清除表单验证
      this.setup.product = null;
      this.$set(this.setup, "device", []);
      this.setup.TimerCron = "";
      if (mode == 2) {
        this.setup.HttpParams = [
          { name: "累计时间[毫秒]", code: "TimeDelta", type: "int", defval: 0,readOnly:true },
          {
            name: "触发间隔[毫秒]",
            code: "TriggerDelta",
            type: "int",
            defval: 0,
            readOnly:true
          },
        ];
      } else {
        this.setup.HttpParams = [];
      }
      this.$nextTick(() => {
        if (!isDis) {
          this.$set(this.setup, "TriggerWay", mode);
        }
      });
    },
    async initProEvts() {//过滤设备产品事件列表
      if (this.selProList == null || this.selProList.length == 0) {
        this.proEvt = [];
        return;
      }
      let pinfo = (await productInfo({ id: this.selProList[0].Id })).data;
      let tsl = JSON.parse(pinfo.ModelTSL);
      this.proEvt = tsl.events;
      // console.log(this.selProList,this.selDevList,this.selTopMsg,'this.selProList');
      this.$forceUpdate()
    },
    finishTimeChoice(val){
        //生成的定时表达式结果
        this.setup.TimerCron=val
        toCronDes(val).then(x=>{
          this.$set(this.setup,"CronName",x.data);
        })
    },
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
    handleShowCron(){
        this.$emit('handleShowCron')
    },
    setTypeMap() {
      //设置数据类型的map
      this.typeList.map((item) => {
        this.typeListMap.set(item.value, item);
      });
    },
    //添加http触发相关函数
    deleteParams(row, rowIndex) {
      //删除参数
      this.$modal
        .confirm('是否确认删除字段名称为"' + row.name + '"的数据项？')
        .then((rs) => {
          if (rs == "confirm") {
            this.setup.HttpParams.splice(rowIndex, 1);
          }
        });
    },
    joinParams(paramList){//加入参数列表后
        this.setup.HttpParams=JSON.parse(JSON.stringify(paramList))
    },
    openParamsDrawer(){
        this.$emit('openParamsDrawer')
    },
    editParams(row, rowIndex) {
      //编辑添加的参数
      this.$emit('editParams',row, rowIndex)
    },
    
  },
};
</script>
<style lang="less">
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
.ruleForm {
    width: 100%;
    // min-height: calc(100% - 38px);
  }
.from_ul {
    .form_li {
      width: 100%;
      position: relative;

      .form_content {
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
          .el-form-item {
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
        .el-form-item.is-not-require label.el-form-item__label::after {
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
</style>