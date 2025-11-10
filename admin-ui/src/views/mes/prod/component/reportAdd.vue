<template>
  <el-dialog :visible.sync="dialogVisible" width="800px" :show-close="false" class="report_add_dialog">
      <div slot="title" class="dialog_slot_title" :class="{'nodata':filedTableList&&filedTableList.length==0}">
        <div class="title_text">添加报工</div>
        <el-tabs v-model="dialogName" tab-position="top" :stretch="true" v-if="filedTableList&&filedTableList.length>0">
          <el-tab-pane name="null1" :disabled="true"><span slot="label"></span></el-tab-pane>
          <el-tab-pane name="null2" :disabled="true"><span slot="label"></span></el-tab-pane>
          <el-tab-pane name="custominfonull" :disabled="true" v-if="!filedTableList||filedTableList&&filedTableList.length==0"><span slot="label"></span></el-tab-pane>
          <el-tab-pane name="baseinfo"><span slot="label">基本信息</span></el-tab-pane>
          <el-tab-pane name="custominfo"><span slot="label" v-if="filedTableList&&filedTableList.length>0">自定义信息</span></el-tab-pane>
          <!-- <el-tab-pane name="approveinfo"><span slot="label" >报工审批</span></el-tab-pane> -->
          <el-tab-pane name="null3" :disabled="true"><span slot="label"></span></el-tab-pane>
          <el-tab-pane name="null4" :disabled="true"><span slot="label"></span></el-tab-pane>
        </el-tabs>
        <div @click="handleClose" class="icon_con"><i class="el-icon-close" style="color: #93969b"></i></div>
      </div>
      <el-form ref="form" :model="form" label-width="100px" :rules="rules" label-position="top">
        <el-row :gutter="10" v-show="dialogName=='baseinfo'">
          <el-col :span="12">
            <el-form-item label="报工编码" prop="Number" v-if="form.Number">
              <el-input v-model="form.Number" placeholder="请输入报工编码" :disabled="true"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="批次编号" prop="BatchNo">
              <el-input v-model="form.BatchNo" placeholder="请输入批次编号"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="生产任务" prop="WorkTaskId">
              <div style="display: flex; align-items: center">
                <el-input class="houseipt" v-model="form.WorkTaskNumber" readonly placeholder="请选择生产任务" @focus="onOpenWorkTask">
                  <i slot="suffix" @click="onAgentClear" v-if="form.WorkTaskId != null" class="el-icon-circle-close" style="font-size: 22px;cursor: pointer;vertical-align: middle;"></i>
                </el-input>
              </div>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="良品数" prop="GoodNum">
              <el-input-number v-model="form.GoodNum" :min="0"></el-input-number>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="不良品数" prop="DefectNum">
              <el-input-number v-model="form.DefectNum" :min="0"></el-input-number>
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="form.DefectNum>0">
            <el-form-item label="不良品项" prop="DefectStr">
              <el-select @focus="defectSearch" clearable style="width: 100%" v-model="form.DefectStr" filterable remote reserve-keyword
                placeholder="请输入不良品项" :remote-method="defectRemoteMethod" :loading="defectLoading">
                <el-option v-for="item in defectOptions" :key="item.Id" :label="item.DefectName" :value="item.Id+','+item.DefectName">{{item.DefectName}}</el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="开始时间" prop="StartWork">
              <el-date-picker @change="workTimeChange" v-model="form.StartWork" type="datetime" placeholder="开始时间" style="width: 100%"></el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="报工时长" prop="WorkTime">
              <el-input-number @change="workTimeChange" v-model="form.WorkTime" :min="0"></el-input-number><span>分钟</span>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="结束时间" prop="EndWork">
              <el-date-picker disabled v-model="form.EndWork" type="datetime" placeholder="结束时间" style="width: 100%"></el-date-picker>
            </el-form-item>
          </el-col>
          <!-- <el-col :span="12">
            <el-form-item label="报工状态" prop="Status">
              <el-radio-group v-model="form.Status">
                <template v-for="it in labelList">
                  <el-radio :key="'status' + it.value" :label="it.value">{{ it.label }}</el-radio>
                </template>
              </el-radio-group>
            </el-form-item>
          </el-col> -->
          <el-col :span="24">
            <el-form-item label="超时原因" prop="OverReason">
              <el-input type="textarea" v-model="form.OverReason" placeholder="请输入超时原因"></el-input>
            </el-form-item>
          </el-col>
          
        </el-row>
        <el-row :gutter="10" v-show="dialogName=='custominfo'">
          <template v-for="(item, ix) in filedTableList">
            <el-col :span="12" :key="'custom_filed' + ix" v-if="!setFormItemHide(item)">
              <el-form-item :label="item.name" :prop="item.mapid">
                <el-select @change="customValChange" :disabled="item.is_readonly" :allow-create="item.is_add" :multiple="item.type == '复选框'" :clearable="!item.is_required"
                  v-model="form[item.mapid]" :placeholder="item.prompt_text ? item.prompt_text : '请选择'" style="width: 100%"
                  v-if=" (item.type == '单选框' && item.show_way == '下拉') || (item.type == '复选框' && item.show_way == '下拉') ">
                  <template v-for="it in item.optionals">
                    <el-option :label="it" :value="it" :key="it + ix"></el-option>
                  </template>
                </el-select>
                <el-radio-group @change="customValChange" :disabled="item.is_readonly" v-model="form[item.mapid]" v-if="item.type == '单选框' && item.show_way == '平铺'">
                  <template v-for="it in item.optionals">
                    <el-radio :label="it" :key="it + ix">{{ it }}</el-radio>
                  </template>
                </el-radio-group>
                <el-checkbox-group @change="customValChange" :disabled="item.is_readonly" v-model="form[item.mapid]" v-if="item.type == '复选框' && item.show_way == '平铺'">
                  <template v-for="it in item.optionals">
                    <el-checkbox :label="it" :key="it + ix">{{ it }}</el-checkbox>
                  </template>
                </el-checkbox-group>
                <el-date-picker @blur="customValChange" @change="customValChange" :disabled="item.is_readonly" v-if="item.type == '时间'" v-model="form[item.mapid]"
                  type="datetime" :placeholder="item.prompt_text ? item.prompt_text : '请选择'" style="width: 100%" :value-format="item.format" :format="item.format"></el-date-picker>
                <el-input @input="customValChange" :disabled="item.is_readonly" v-if="item.type == '文本'" :placeholder="item.prompt_text ? item.prompt_text : '请输入'"
                  :type="item.is_multiple ? 'textarea' : 'text'" v-model="form[item.mapid]"></el-input>
                <el-input @input="customValChange" :disabled="item.is_readonly" v-if="item.type == '数字'" :placeholder="item.prompt_text ? item.prompt_text : '请输入'"
                  type="number" v-model="form[item.mapid]" :precision="item.decimals"></el-input>
                <el-link :disabled="item.is_readonly" v-if="item.type == '超链接'" href="#" target="_blank">{{ item.describe_text }}</el-link>
                <!-- <image-upload @input="customValChange" v-model="form[item.mapid]" :limit="1" v-if="item.type == '图片'"></image-upload> -->
                <div class="avatar_con" v-if="item.type == '图片'">
                  <image-upload @input="customValChange($event,item)" v-model="form[item.mapid]" :limit="1" :isShowLeft="true">
                    <template #tip>
                      <div class="label_tip">
                        <div class="label_text">　　</div>
                        <div class="tip_con">
                          <span style="margin-left:6px">请上传</span>
                        </div>
                      </div>
                    </template>
                  </image-upload>
                </div>
                <file-upload @input="customValChange($event,item)" v-model="form[item.mapid]" :limit="1" v-if="item.type == '附件'" :isShowLeft="true">
                  <template #tip>
                    <div class="label_tip">
                      <div class="label_text">　　</div>
                      <div class="tip_con">
                        <span style="margin-left:6px">请上传</span>
                      </div>
                    </div>
                  </template>
                </file-upload>
                <el-select @focus="afterValSearch(form[item.mapid],item)" :clearable="true" @change="customValChange2($event,item)" style="width: 100%" v-model="form[item.mapid]" filterable remote reserve-keyword
                  :placeholder="item.prompt_text ? item.prompt_text : '请选择'" :remote-method="(query)=>associationMethod(query,item)" :loading="objectLoading" v-if="item.type == '关联对象'">
                  <el-option v-for="ite in associationObject[item.mapid]" :key="ite.Value" :label="ite.Name" :value="ite.Value+','+ite.ValueName">{{ite.Name}}</el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </template>
        </el-row>
        <div v-show="mesform.ReportTemplateId !== ''">
          <AddEmbed ref="flowForm" class="flow_con">
            <div class="flow-title" style="font-size: 16px; color: #333">
              审批信息
            </div>
          </AddEmbed>
        </div>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="submitFiledAdd">确 定</el-button>
      </span>
    </el-dialog>
</template>

<script>
import {
  factorySearchObject
} from "@/api/factory/product";
var dayjs = require("@/utils/day.js");
import AddEmbed from "@/views/flowable/task/record/AddEmbed";
import OrgPicker from "@/views/flowable/common/OrgPicker";
import {
  factoryMesConfig,
} from "@/api/mes/config";
import { orgField } from "@/api/factory/customFields";
import {GeneratePlaneNumber,reportFormData,reportSubmitModel,reportAdd,reportEdit} from '@/api/mes/report'
  import { defectList } from "@/api/mes/defect";
export default {
  name: 'AdminUiReportAdd',
components: { AddEmbed, OrgPicker },
  data() {
    return {
      dialogName: "baseinfo",
      labelList: [
        {
          label: "停用",
          value: 0,
        },
        {
          label: "正常",
          value: 1,
        },
      ],
      typeList: [],
      dialogVisible: false,
      form: {
        Number: "",//唯一编号
        BatchNo: "",//批次编号
        GoodNum: 1, //良品数
        DefectNum: "",//不良品数
        DefectStr: "",//不良品项
        StartWork: "",//开始时间
        EndWork: "",//结束时间
        WorkTime: "", //报工时长
        OverReason: "", //超时原因
        FlowId: "", //流程表单id
        RepBat:{}
      },
      rules: {
        GoodNum: [
          { required: true, trigger: "change", message: "请输入良品数" },
        ],
        DefectNum: [
          { required: true, trigger: "change", message: "请输入不良品数" },
        ],
        DefectStr:[
          { required: true, trigger: "change", message: "不良品项不能为空" },
        ],
        StartWork: [
          { required: true, trigger: "change", message: "请选择开始时间" },
        ],
        WorkTime: [
          { required: true, trigger: "change", message: "请输入报工时长" },
        ],
        OverReason: [
          { required: true, trigger: "blur", message: "请输入超时原因" },
        ],
      },
      filedTableList: [], //报工自定义列表
      defectLoading:false,//不良品项加载
      defectOptions:[],//不良品项列表
      mesform:{},
      associationObject:{},//所有关联对象对应的下拉的参数列表
      objectLoading:true,
    };
  },
  computed: {
    FlowParams: function () {
      return {
        "@from": this.form.Number,
        "@fromtype": "生产报工",
      };
    },
  },
  mounted() {
    
  },

  methods: {
    setPlanSelect(val){
      //完成生产计划的选择
    },
    onOpenWorkTask(){
      this.$emit('onOpenWorkTask')
    },
    defectSearch(){
      if(this.form.DefectStr&&this.form.DefectStr.indexOf(',')>-1){
        let keyVal=this.form.DefectStr.split(',')
        this.defectRemoteMethod(keyVal[1])
      }else{
        this.defectRemoteMethod('')
      }
    },
    async defectRemoteMethod(key){
      this.defectLoading=true
      let obj={
        Key:key,
        pageNum:1,
        pageSize:10
      }
      let res=await defectList(obj)
      if(res.data.List){
        // console.log("res.data.List",res.data.List);
        this.defectOptions=JSON.parse(JSON.stringify(res.data.List))
      }
      this.$forceUpdate()
      this.defectLoading=false
      // console.log(res,'resres');
    },
    workTimeChange(){
      if(this.form.StartWork){
        if(!this.form.WorkTime){
          this.form.WorkTime=0
        }
        let end=dayjs(this.form.StartWork).add(this.form.WorkTime,'minute')
        this.$set(this.form,'EndWork',dayjs(end).format('YYYY-MM-DD HH:mm:ss'))
      }
    },
    returnCompareResult(field, compare, val, type) {
      let result = true;
      switch (compare) {
        case "=":
          result = this.form[field] == val;
          break;
        case "!=":
          result = this.form[field] != val;
          break;
        case "IN":
          result = this.form[field] && this.form[field].indexOf(val) > -1;
          break;
        case "NOTIN":
          result =
            !this.form[field] ||
            (this.form[field] && this.form[field].indexOf(val) == -1);
          break;
        case "ISNULL":
          result = this.form[field] == "" || this.form[field] == null;
          break;
        case "NOTNULL":
          result = this.form[field] != "" && this.form[field] != null;
          break;
        case ">":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form[field]).valueOf() >
                dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form[field] > val;
          }
          break;
        case "<":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form[field]).valueOf() <
                dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form[field] < val;
          }
          break;
        case "==":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form[field]).valueOf() ==
                dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form[field] == val;
          }
          break;
        case "><":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form[field]).valueOf() !=
                dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form[field] != val;
          }
          break;
        case ">=":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form[field]).valueOf() >=
                dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form[field] >= val;
          }
          break;
        case "<=":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form[field]).valueOf() <=
                dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form[field] <= val;
          }
          break;
        case "INRANGE":
          if (type && type == "数字") {
            if (val.min && val.max) {
              if (this.form[field] >= val.min && this.form[field] <= val.max) {
                result = true;
              } else {
                result = false;
              }
            }
          }
          result = this.form[field] != "" && this.form[field] != null;
          break;
        case "NOTINRANGE":
          if (type && type == "数字") {
            if (val.min && val.max) {
              if (this.form[field] < val.min && this.form[field] > val.max) {
                result = true;
              } else {
                result = false;
              }
            }
          }
          break;
        case "SELECTRANGE":
          if (type && type == "时间") {
            if (val[0] && val[1]) {
              let max = Math.max(...val);
              let min = Math.min(...val);
              if (this.form[field] >= min && this.form[field] <= max) {
                result = true;
              } else {
                result = false;
              }
            }
          }
          break;
        case "DYNAMICS":
          if (type && type == "时间") {
            if (val[0] && val[1]) {
              let max = Math.max(...val);
              let min = Math.min(...val);
              if (this.form[field] >= min && this.form[field] <= max) {
                result = true;
              } else {
                result = false;
              }
            }
          }
          break;
      }
      return result;
    },
    setFormItemHide(item) {
      if (item.conditions && item.conditions.length > 0) {
        let result = false;
        let conditionsResArr = [];
        for (let i = 0; i < item.conditions.length; i++) {
          let row = item.conditions[i];
          conditionsResArr[i] = this.returnCompareResult(
            row.field,
            row.compare,
            row.val,
            row.type
          );
        }
        for (let i = 0; i < conditionsResArr.length; i++) {
          if (i == 0) {
            result = conditionsResArr[i];
          } else {
            if (item.groups && item.groups[i - 1]) {
              if (item.groups[i - 1] == "and") {
                result = result && conditionsResArr[i];
              } else if (item.groups[i - 1] == "or") {
                result = result || conditionsResArr[i];
              }
            } else {
              result = result || conditionsResArr[i];
            }
          }
        }
        return result;
      } else {
        return false;
      }
    },
    afterValSearch(val,item){//关联对象回显时获取列表
      if(val&&val.indexOf(',')>-1){
        let keyVal=val.split(',')
        this.associationMethod(keyVal[1],item)
      }else{
        this.associationMethod('',item)
      }
    },
    associationMethod(query,item){//关联对象的远程搜索事件
      // console.log("关联对象",item);
      this.getFactorySearchObject(query,item.object_type,item.mapid)
    },
    async getFactorySearchObject(key,objtype,mapid){
      //根据不同的关联对象获取对象的列表
      this.objectLoading=true
      let obj={
        key:key,
        objtype:objtype,
        pageNum:1,
        pageSize:10
      }
      let res=await factorySearchObject(obj)
      if(res.data.List){
        // console.log("res.data.List",res.data.List);
        this.associationObject[mapid]=JSON.parse(JSON.stringify(res.data.List))
      }
      this.$forceUpdate()
      this.objectLoading=false
      // console.log(res,'resres');
    },
    customValChange2(val,fidItem) {//数据发生变化后刷新，并验证表单
      console.log("看看关联对象选择后有没有出现",fidItem);
      let form = JSON.parse(JSON.stringify(this.form));
      this.form = JSON.parse(JSON.stringify(form));
      if(fidItem&&fidItem.type=='关联对象'&&this.form[fidItem.mapid]){
        if(fidItem.items&&fidItem.items.length>0){
          let fieldMapidVal=this.form[fidItem.mapid].split(',')
          let findObj=this.associationObject[fidItem.mapid].find(row=>row.Value==fieldMapidVal[0])
          for(let i=0;i<fidItem.items.length;i++){
            let item=fidItem.items[i]
            this.form[item.field]=findObj.Obj[item.source_obj]
          }
        }
      }
      let form2 = JSON.parse(JSON.stringify(this.form));
      this.form = JSON.parse(JSON.stringify(form2));
      this.$nextTick(()=>{
        if(fidItem&&fidItem.type=='关联对象'||fidItem.type == '图片'){
          this.$refs["form"].validate((valid) => {});
        }
        // this.$refs["form"].validate((valid) => {});
        this.$forceUpdate();
      })
    },
    customValChange() {
      let form = JSON.parse(JSON.stringify(this.form));
      console.log('form',form);
      this.$nextTick(()=>{
        this.form = JSON.parse(JSON.stringify(form));
        this.$refs["form"].validate((valid) => {});
        this.$forceUpdate();
      })
    },
    setCustomDefaultValue(afterForm) {
      //设置自定义的变量初始化
      this.filedTableList.map((rw) => {
        if (afterForm) {
          this.form[rw.mapid] = afterForm[rw.mapid];
          if (rw.type == "时间") {
            let newStr = rw.format.replace(/y/g, "Y");
            newStr = newStr.replace(/d/g, "D");
            this.form[rw.mapid] = dayjs(afterForm[rw.mapid]).format(newStr);
          } else {
            if (rw.type == "数字") {
              this.form[rw.mapid] = Number(afterForm[rw.mapid]);
            } else if (rw.type == "复选框") {
              this.form[rw.mapid] = afterForm[rw.mapid].split(",");
            } else {
              this.form[rw.mapid] = afterForm[rw.mapid];
            }
          }
        } else {
          this.form[rw.mapid] = null;
          if (rw.defval != "" && rw.defval != undefined && rw.defval != null) {
            if (rw.type == "时间") {
              let newStr = rw.format.replace(/y/g, "Y");
              newStr = newStr.replace(/d/g, "D");
              this.form[rw.mapid] = dayjs(rw.defval).format(newStr);
            } else {
              if (rw.type == "数字") {
                this.form[rw.mapid] = Number(rw.defval);
              } else {
                this.form[rw.mapid] = rw.defval;
              }
            }
          } else {
            if (rw.type == "复选框") {
              this.form[rw.mapid] = [];
            }else if (rw.type == "时间") {
              this.form[rw.mapid] ='';
            }
          }
        }
        if (rw.is_required) {
          if (rw.type == "单选框" || rw.type == "复选框" || rw.type == "时间") {
            let rowRules = [
              {
                required: true,
                trigger: "change",
                message: "请选择" + rw.name,
              },
            ];
            this.rules[rw.mapid] = rowRules;
          } else {
            let rowRules = [
              { required: true, trigger: "blur", message: "请输入" + rw.name },
            ];
            this.rules[rw.mapid] = rowRules;
          }
        }
      });
      console.log("this.rules", this.rules);
    },
    async getCustomFiled() {
      //获取自定义的字段
      this.filedTableList = [];
      let orgId = this.$store.state.user.orgId;
      let res = await orgField({ orgId: orgId, field: "报工" });
      if (res.data) {
        if (res.data.ExtValue) {
          let filedList = JSON.parse(res.data.ExtValue);
          this.filedTableList = filedList; //排序处理，并且数字字段排前面
          // console.log("自定义字段",this.filedTableList);
        }
      } else {
        this.filedTableList = [];
      }
    },
    async openDialog(item) {
      await this.getCustomFiled();
      if (item) {
        // let res = await factoryreportInfo({ id: id });
        console.log("item,报工详情", item, this.filedTableList);
        let reportInfo = item;
        this.form = {
          Id:reportInfo.Id,
          Number: reportInfo.Number,//唯一编号
          BatchNo: reportInfo.BatchNo,//批次编号
          GoodNum: reportInfo.GoodNum, //良品数
          DefectNum: reportInfo.DefectNum,//不良品数
          DefectStr: reportInfo.DefectStr,//不良品项
          StartWork: reportInfo.StartWork,//开始时间
          EndWork: reportInfo.EndWork,//结束时间
          WorkTime: reportInfo.WorkTime, //报工时长
          OverReason: reportInfo.OverReason, //超时原因
          FlowId: reportInfo.FlowId, //流程表单id
        };
        this.setCustomDefaultValue(reportInfo);
      } else {
        this.form = {
          Id:null,
          Number: "",//唯一编号
          BatchNo: "",//批次编号
          GoodNum: 1, //良品数
          DefectNum: "",//不良品数
          DefectStr: "",//不良品项
          StartWork: "",//开始时间
          EndWork: "",//结束时间
          WorkTime: 0, //报工时长
          OverReason: "", //超时原因
          FlowId: "", //流程表单id
        }
        let numres=await GeneratePlaneNumber()//获取报工编号
        this.form.Number=numres.data
        this.setCustomDefaultValue();
        let response = await factoryMesConfig();//获取生产报工相关配置
        // console.log("初始化配置信息",response);
        this.mesform.ReportTemplateName = response.data.ReportTemplateName;
        this.mesform.ReportTemplateId = response.data.ReportTemplateId;
        console.log("表单初始化",this.form);
      }
      this.dialogVisible = true;
      if (this.mesform.ReportTemplateId > 0) {
        let fromInfo=await reportFormData({
          "Number": this.form.Number,
        })
        console.log(fromInfo,'fromInfo',this.$refs.flowForm);
        this.$nextTick(async ()=>{
          await this.$refs.flowForm.InitData(
            this.mesform.ReportTemplateId,
            this.FlowParams,
            this.form.Id == null ? null : this.form.Number,
            fromInfo.data
          );
        })
      }
      
    },
    submitFiledAdd() {
      //提交数据
      this.$refs["form"].validate(async (valid,validateResult) => {
        console.log("检验");
        if (valid) {
          let submitForm = JSON.parse(JSON.stringify(this.form));
          if(this.form.DefectNum>0&&this.form.DefectStr&&this.form.DefectStr.length>0){//是否有不良品项
            submitForm.DefectStr=this.form.DefectStr.join(',')
          }else{
            submitForm.DefectStr=''
          }
          
          for (let i = 0; i < this.filedTableList.length; i++) {
            let row = this.filedTableList[i];
            if (row.type == "数字") {
              if (submitForm[row.mapid]) {
              } else {
                submitForm[row.mapid] = Number(submitForm[row.mapid]);
              }
            } else if (row.type == "时间") {
              submitForm[row.mapid] = dayjs(submitForm[row.mapid]).valueOf();
            } else if (row.type == "复选框") {
              if (submitForm[row.mapid] && submitForm[row.mapid].length > 0) {
                submitForm[row.mapid] = submitForm[row.mapid].join(",");
              } else {
                submitForm[row.mapid] = "";
              }
            } else {
              if (submitForm[row.mapid]) {
              } else {
                submitForm[row.mapid] = "";
              }
            }
            submitForm.RepBat[row.mapid]=submitForm[row.mapid]
            delete submitForm[row.mapid]
          }
          console.log("提交的数据", submitForm);
          let response;
          if (submitForm.Id) {
            response=await reportEdit(submitForm);
            console.log("修改执行结果", response);
            this.$modal.msgSuccess("修改成功");
            this.dialogVisible = false;
            this.$emit("reloadData");
          } else {
            response=await reportAdd(submitForm);
            console.log("添加执行结果", response);
            this.$modal.msgSuccess("添加成功");
            this.dialogVisible = false;
            this.$emit("reloadData");
          }
          if (this.mesform.ReportTemplateId > 0) {
            if (st == 2) {
              let tmpmodel = this.$refs.flowForm.getModel();
              if(submitForm.Id){
                tmpmodel["id"] = submitForm.Id;
              }else{
                tmpmodel["id"] = response.data;
              }
              
              await reportSubmitModel(tmpmodel);
            }
            else {
              await this.$refs.flowForm.submitForm(st);
            }
          }
          else{
            await reportSubmitModel({ id: response.data });
          }
        }else{
          let errKey=Object.keys(validateResult)
          if(errKey&&errKey[0]){
            let findObj=this.filedTableList.find(row=>row.mapid==errKey[0])
            if(findObj){
              this.dialogName='custominfo'
            }else{
              this.dialogName='baseinfo'
            }
          }
        }
      });
    },
    handleClose() {
      this.dialogVisible = false;
    },
  },
};
</script>

<style lang="scss" scoped>
.flow-title {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  height: 48px;
  background-color: rgb(249, 250, 252);
  padding: 0 15px;
  margin-bottom: 5px;
}
.report_add_dialog{
  ::v-deep .el-dialog__body{
    padding-top: 0;
  }
  ::v-deep .el-form-item__label{
    line-height: 14px;
  }
  .flow_con{
    ::v-deep .el-form-item__label{
      margin-bottom: 10px;
    }
  }
}
.dialog_slot_title {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  width: 100%;
  position: relative;
  &.nodata{
    margin-bottom: 20px;
  }
  .el-tabs {
    width: 100%;
  }
  .title_text {
    position: absolute;
    left: 0px;
    z-index: 9;
  }
  .icon_con {
    position: absolute;
    right: 0px;
    cursor: pointer;
    z-index: 9;
  }
}
.houseipt {
  ::v-deep .el-input__inner {
    cursor: pointer;
  }
}
.avatar_con {
  width: 100%;
  text-align: center;
  display: flex;
  align-items: flex-start;
  .label_tip{
    height: 40px;
    text-align: left;
    .label_text{
      height: 8px;
    }
  }
  .tip_con{
    width: 182px;
    height: 30px;
    border: 1px solid rgba(223, 226, 234, 1);
    color: rgba(120, 130, 157, 1);
    line-height: 30px;
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
  ::v-deep .el-upload--picture-card i{
    font-size: 16px;
  }
  ::v-deep .el-upload.el-upload--picture-card{
    width: 70px;
    height: 40px;
    line-height: 40px;
  }
  ::v-deep .component-upload-image{
    height: 40px;
    // margin-bottom: 20px;
    .el-upload__tip{
      margin-top: 0;
    }
  }
  ::v-deep .el-upload-list--picture-card .el-upload-list__item{
    width: 70px;
    height: 40px;
  }
}
</style>