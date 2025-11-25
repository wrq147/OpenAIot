<template>
  <el-dialog :visible.sync="dialogVisible" width="800px" :show-close="false" class="report_add_dialog">
    <div slot="title" class="dialog_slot_title">
      <div class="title_text">{{ isOnlyRead ? "编辑报工" : "添加报工" }}</div>
      <el-tabs v-model="dialogName" tab-position="top" :stretch="true"
        v-if="filedTableList && filedTableList.length > 0">
        <el-tab-pane name="null1" :disabled="true"><span slot="label"></span></el-tab-pane>
        <el-tab-pane name="null2" :disabled="true"><span slot="label"></span></el-tab-pane>
        <el-tab-pane name="custominfonull" :disabled="true"
          v-if="!filedTableList || filedTableList && filedTableList.length == 0"><span
            slot="label"></span></el-tab-pane>
        <el-tab-pane name="baseinfo"><span slot="label">基本信息</span></el-tab-pane>
        <el-tab-pane name="custominfo"><span slot="label"
            v-if="filedTableList && filedTableList.length > 0">自定义信息</span></el-tab-pane>
        <!-- <el-tab-pane name="approveinfo"><span slot="label" >报工审批</span></el-tab-pane> -->
        <el-tab-pane name="null3" :disabled="true"><span slot="label"></span></el-tab-pane>
        <el-tab-pane name="null4" :disabled="true"><span slot="label"></span></el-tab-pane>
      </el-tabs>
      <div @click="handleClose" class="icon_con"><i class="el-icon-close" style="color: #93969b"></i></div>
    </div>
    <el-form ref="form" :model="form" label-width="100px" :rules="rules" label-position="top">
      <el-row :gutter="10" v-show="dialogName == 'baseinfo'">
        <el-col :span="12">
          <el-form-item label="报工编码" prop="Number" v-if="form.Number">
            <el-input v-model="form.Number" placeholder="请输入报工编码" :disabled="true"></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="批次编号" prop="BatchNo">
            <el-input v-model="form.BatchNo" placeholder="请输入批次编号" :disabled="isOnlyRead"></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="生产任务" prop="WorkTaskId">
            <div style="display: flex; align-items: center">
              <el-input :disabled="isOnlyRead" class="houseipt" v-model="form.WorkTaskNumber" readonly
                placeholder="请选择生产任务" @focus="onOpenWorkTask">
                <i slot="suffix" @click="onTaskClear" v-if="form.WorkTaskId != ''" class="el-icon-circle-close"
                  style="font-size: 22px;cursor: pointer;vertical-align: middle;"></i>
              </el-input>
            </div>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="良品数" prop="GoodNum">
            <el-input-number :disabled="isOnlyRead" v-model="form.GoodNum" :min="0"></el-input-number>
          </el-form-item>
        </el-col>
        <el-col :span="24" v-if="form.DefectList != null && form.DefectList.length > 0">
          <div style="margin-bottom:20px;">
            <div style="padding-bottom: 10px;color: #333333;">不良品项列表</div>
            <el-table :data="form.DefectList" border style="width: 100%;">
              <el-table-column label="不良品名称" align="center" prop="DefectName" width="450"></el-table-column>
              <el-table-column label="数量" align="center" prop="DefectNum">
                <template slot-scope="scope">
                  <el-input-number :disabled="isOnlyRead" v-model="scope.row.DefectNum" :min="0">
                  </el-input-number>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </el-col>
        <el-col :span="12">
          <el-form-item label="开始时间" prop="StartWork">
            <el-date-picker :disabled="isOnlyRead" @change="workTimeChange" v-model="form.StartWork" type="datetime"
              placeholder="开始时间" style="width: 100%"></el-date-picker>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="报工时长" prop="WorkTime">
            <el-input-number :disabled="isOnlyRead" @change="workTimeChange" v-model="form.WorkTime"
              :min="0"></el-input-number><span>分钟</span>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="结束时间" prop="EndWork">
            <el-date-picker disabled v-model="form.EndWork" type="datetime" placeholder="结束时间"
              style="width: 100%"></el-date-picker>
          </el-form-item>
        </el-col>

        <el-col :span="24">
          <el-form-item label="超时原因" prop="OverReason">
            <el-alert v-if="NeedReason" title="报工时长超过预计时长，请填写超时原因" :closable="false" type="error" show-icon>
            </el-alert>
            <el-input :disabled="isOnlyRead" type="textarea" v-model="form.OverReason" placeholder="请输入备注"></el-input>
          </el-form-item>
        </el-col>

      </el-row>
      <el-row :gutter="10" v-show="dialogName == 'custominfo'">
        <template v-for="(item, ix) in filedTableList">
          <el-col :span="12" :key="'custom_filed' + ix" v-if="!setFormItemHide(item)">
            <el-form-item :label="item.name" :prop="item.mapid">
              <el-select @change="customValChange" :disabled="item.is_readonly && isOnlyRead"
                :allow-create="item.is_add" :multiple="item.type == '复选框'" :clearable="!item.is_required"
                v-model="form.RepBat[item.mapid]" :placeholder="item.prompt_text ? item.prompt_text : '请选择'"
                style="width: 100%"
                v-if="(item.type == '单选框' && item.show_way == '下拉') || (item.type == '复选框' && item.show_way == '下拉')">
                <template v-for="it in item.optionals">
                  <el-option :label="it" :value="it" :key="it + ix"></el-option>
                </template>
              </el-select>
              <el-radio-group @change="customValChange" :disabled="item.is_readonly && isOnlyRead"
                v-model="form.RepBat[item.mapid]" v-if="item.type == '单选框' && item.show_way == '平铺'">
                <template v-for="it in item.optionals">
                  <el-radio :label="it" :key="it + ix">{{ it }}</el-radio>
                </template>
              </el-radio-group>
              <el-checkbox-group @change="customValChange" :disabled="item.is_readonly && isOnlyRead"
                v-model="form.RepBat[item.mapid]" v-if="item.type == '复选框' && item.show_way == '平铺'">
                <template v-for="it in item.optionals">
                  <el-checkbox :label="it" :key="it + ix">{{ it }}</el-checkbox>
                </template>
              </el-checkbox-group>
              <el-date-picker @blur="customValChange" @change="customValChange"
                :disabled="item.is_readonly && isOnlyRead" v-if="item.type == '时间'" v-model="form.RepBat[item.mapid]"
                type="datetime" :placeholder="item.prompt_text ? item.prompt_text : '请选择'" style="width: 100%"
                :value-format="item.format" :format="item.format"></el-date-picker>
              <el-input @input="customValChange" :disabled="item.is_readonly && isOnlyRead" v-if="item.type == '文本'"
                :placeholder="item.prompt_text ? item.prompt_text : '请输入'"
                :type="item.is_multiple ? 'textarea' : 'text'" v-model="form.RepBat[item.mapid]"></el-input>
              <el-input @input="customValChange" :disabled="item.is_readonly && isOnlyRead" v-if="item.type == '数字'"
                :placeholder="item.prompt_text ? item.prompt_text : '请输入'" type="number"
                v-model="form.RepBat[item.mapid]" :precision="item.decimals"></el-input>
              <el-link :disabled="item.is_readonly && isOnlyRead" v-if="item.type == '超链接'" href="#" target="_blank">{{
                item.describe_text }}</el-link>
              <!-- <image-upload @input="customValChange" v-model="form[item.mapid]" :limit="1" v-if="item.type == '图片'"></image-upload> -->
              <div class="avatar_con" v-if="item.type == '图片'">
                <image-upload :disabled="isOnlyRead" @input="customValChange($event, item)"
                  v-model="form.RepBat[item.mapid]" :limit="1" :isShowLeft="true">
                  <template #tip>
                    <span></span>
                  </template>
                </image-upload>
              </div>
              <file-upload :disabled="isOnlyRead" @input="customValChange($event, item)"
                v-model="form.RepBat[item.mapid]" :limit="1" v-if="item.type == '附件'" :isShowLeft="true">
                <template #tip>
                  <span></span>
                </template>
              </file-upload>
              <el-select :disabled="isOnlyRead" @focus="afterValSearch(form.RepBat[item.mapid], item)" :clearable="true"
                @change="customValChange2($event, item)" style="width: 100%" v-model="form.RepBat[item.mapid]"
                filterable remote reserve-keyword :placeholder="item.prompt_text ? item.prompt_text : '请选择'"
                :remote-method="(query) => associationMethod(query, item)" :loading="objectLoading"
                v-if="item.type == '关联对象'">
                <el-option v-for="ite in associationObject[item.mapid]" :key="ite.Value" :label="ite.Name"
                  :value="ite.Value + ',' + ite.ValueName">{{ ite.Name }}</el-option>
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
      <el-button @click="submitFiledAdd(0)" v-if="!isOnlyRead">保 存</el-button>
      <el-button type="primary" @click="submitFiledAdd(2)" v-if="!isOnlyRead">提 交</el-button>
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
import { GeneratePlaneNumber, reportFormData, reportSubmitModel, reportAdd, reportEdit, reportInfo } from '@/api/mes/report'
import { operInfo, routeOperInfo } from "@/api/mes/oper";
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
        DefectList: [],
        StartWork: "",//开始时间
        EndWork: "",//结束时间
        WorkTime: 0, //报工时长
        OverReason: "", //超时原因改备注
        // FlowId: 0, //流程表单id
        RepBat: {},
        PhotoUrl: '',
        WorkOrderId: '',
        WorkTaskId: '',
        OperId: '',
      },
      rules: {
        GoodNum: [
          { required: true, trigger: "change", message: "请输入良品数" },
        ],
        DefectNum: [
          { required: true, trigger: "change", message: "请输入不良品数" },
        ],
        BatchNo: [
          { required: true, trigger: 'blur', message: '批次编号不能为空' }
        ],
        StartWork: [
          { required: true, trigger: "change", message: "请选择开始时间" },
        ],
        WorkTime: [
          { required: true, trigger: "change", message: "请输入报工时长" },
        ],
        WorkTaskId: [
          { required: true, trigger: "change", message: "请选择生产任务" },
        ],
      },
      filedTableList: [], //报工自定义列表
      defectLoading: false,//不良品项加载
      defectOptions: [],//不良品项列表
      mesform: {},
      associationObject: {},//所有关联对象对应的下拉的参数列表
      objectLoading: true,
      isOnlyRead: false,//是否只读
    };
  },
  computed: {
    FlowParams: function () {
      return {
        "@from": this.form.Number,
        "@fromtype": "生产报工",
      };
    },
    NeedReason() {
      if (this.form.RouteOper == null) {
        return false;
      }
      let needWorkTime = this.form.RouteOper.WorkTime * this.form.RouteOper.PropOf * (this.form.GoodNum + this.form.DefectNum);
      if (needWorkTime < this.form.WorkTime && this.form.OverReason == "") {
        return true;
      }
      return false;
    }
  },
  mounted() {

  },

  methods: {
    async setTaskSelect(val) {
      //完成生产任务的选择
      this.form.WorkTaskNumber = val.WorkNumber + "-" + val.OperName;
      this.form.WorkOrderId = val.WorkOrderId
      this.form.WorkTaskId = val.Id
      this.form.OperId = val.OperId
      let operRes = await operInfo({ "id": val.OperId });
      this.form.DefectList = [];
      if (operRes.data.DefectJson != '') {
        let tmpdflist = JSON.parse(operRes.data.DefectJson);
        for (let i = 0; i < tmpdflist.length; i++) {
          this.form.DefectList.push({ "DefectId": tmpdflist[i].Id, "DefectName": tmpdflist[i].DefectName, "DefectCategory": tmpdflist[i].DefectCategory, "DefectNum": 0 })
        }
      }
      let routeRes = await routeOperInfo({ "id": val.RouteOperId });
      this.form.RouteOper = routeRes.data;
      this.$forceUpdate()
    },
    onOpenWorkTask() {
      this.$emit('onOpenWorkTask')
    },
    onTaskClear() {
      this.form.WorkTaskNumber = ''
      this.form.WorkOrderId = ''
      this.form.WorkTaskId = ''
      this.form.OperId = ''
      this.$forceUpdate()
    },
    workTimeChange() {
      if (this.form.StartWork) {
        if (!this.form.WorkTime) {
          this.form.WorkTime = 0
        }
        let end = dayjs(this.form.StartWork).add(this.form.WorkTime, 'minute')
        this.$set(this.form, 'EndWork', dayjs(end).format('YYYY-MM-DD HH:mm:ss'))
      }
    },
    returnCompareResult(field, compare, val, type) {
      let result = true;
      switch (compare) {
        case "=":
          result = this.form.RepBat[field] == val;
          break;
        case "!=":
          result = this.form.RepBat[field] != val;
          break;
        case "IN":
          result = this.form.RepBat[field] && this.form.RepBat[field].indexOf(val) > -1;
          break;
        case "NOTIN":
          result =
            !this.form.RepBat[field] ||
            (this.form.RepBat[field] && this.form.RepBat[field].indexOf(val) == -1);
          break;
        case "ISNULL":
          result = this.form.RepBat[field] == "" || this.form.RepBat[field] == null;
          break;
        case "NOTNULL":
          result = this.form.RepBat[field] != "" && this.form.RepBat[field] != null;
          break;
        case ">":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form.RepBat[field]).valueOf() >
              dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form.RepBat[field] > val;
          }
          break;
        case "<":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form.RepBat[field]).valueOf() <
              dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form.RepBat[field] < val;
          }
          break;
        case "==":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form.RepBat[field]).valueOf() ==
              dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form.RepBat[field] == val;
          }
          break;
        case "><":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form.RepBat[field]).valueOf() !=
              dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form.RepBat[field] != val;
          }
          break;
        case ">=":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form.RepBat[field]).valueOf() >=
              dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form.RepBat[field] >= val;
          }
          break;
        case "<=":
          if (type && type == "时间") {
            result =
              val.timeValue &&
              dayjs(this.form.RepBat[field]).valueOf() <=
              dayjs(val.timeValue).valueOf();
          } else if (type && type == "数字") {
            result = this.form.RepBat[field] <= val;
          }
          break;
        case "INRANGE":
          if (type && type == "数字") {
            if (val.min && val.max) {
              if (this.form.RepBat[field] >= val.min && this.form.RepBat[field] <= val.max) {
                result = true;
              } else {
                result = false;
              }
            }
          }
          result = this.form.RepBat[field] != "" && this.form.RepBat[field] != null;
          break;
        case "NOTINRANGE":
          if (type && type == "数字") {
            if (val.min && val.max) {
              if (this.form.RepBat[field] < val.min && this.form.RepBat[field] > val.max) {
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
              if (this.form.RepBat[field] >= min && this.form.RepBat[field] <= max) {
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
              if (this.form.RepBat[field] >= min && this.form.RepBat[field] <= max) {
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
    afterValSearch(val, item) {//关联对象回显时获取列表
      if (val && val.indexOf(',') > -1) {
        let keyVal = val.split(',')
        this.associationMethod(keyVal[1], item)
      } else {
        this.associationMethod('', item)
      }
    },
    associationMethod(query, item) {//关联对象的远程搜索事件
      // console.log("关联对象",item);
      this.getFactorySearchObject(query, item.object_type, item.mapid)
    },
    async getFactorySearchObject(key, objtype, mapid) {
      //根据不同的关联对象获取对象的列表
      this.objectLoading = true
      let obj = {
        key: key,
        objtype: objtype,
        pageNum: 1,
        pageSize: 10
      }
      let res = await factorySearchObject(obj)
      if (res.data.List) {
        // console.log("res.data.List",res.data.List);
        this.associationObject[mapid] = JSON.parse(JSON.stringify(res.data.List))
      }
      this.$forceUpdate()
      this.objectLoading = false
      // console.log(res,'resres');
    },
    customValChange2(val, fidItem) {//数据发生变化后刷新，并验证表单
      // console.log("看看关联对象选择后有没有出现",fidItem);
      let form = JSON.parse(JSON.stringify(this.form));
      this.form = JSON.parse(JSON.stringify(form));
      if (fidItem && fidItem.type == '关联对象' && this.form[fidItem.mapid]) {
        if (fidItem.items && fidItem.items.length > 0) {
          let fieldMapidVal = this.form[fidItem.mapid].split(',')
          let findObj = this.associationObject[fidItem.mapid].find(row => row.Value == fieldMapidVal[0])
          for (let i = 0; i < fidItem.items.length; i++) {
            let item = fidItem.items[i]
            this.form[item.field] = findObj.Obj[item.source_obj]
          }
        }
      }
      let form2 = JSON.parse(JSON.stringify(this.form));
      this.form = JSON.parse(JSON.stringify(form2));
      this.$nextTick(() => {
        if (fidItem && fidItem.type == '关联对象' || fidItem.type == '图片') {
          this.$refs["form"].validate((valid) => { });
        }
        // this.$refs["form"].validate((valid) => {});
        this.$forceUpdate();
      })
    },
    customValChange() {
      let form = JSON.parse(JSON.stringify(this.form));
      // console.log('form',form);
      this.$nextTick(() => {
        this.form = JSON.parse(JSON.stringify(form));
        this.$refs["form"].validate((valid) => { });
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
            } else if (rw.type == "时间") {
              this.form[rw.mapid] = '';
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
      // console.log("this.rules", this.rules);
    },
    async getCustomFiled() {
      //获取自定义的字段
      this.filedTableList = [];
      let orgId = this.$store.state.user.orgId;
      let res = await orgField({ orgId: orgId, field: "报工" });
      if (res.data) {
        let filedList = JSON.parse(res.data.ExtValue);
        this.filedTableList = filedList;
      } else {
        this.filedTableList = [];
      }
    },
    async openDialog(id, isOnlyRead) {
      await this.getCustomFiled();
      if (isOnlyRead) {
        this.isOnlyRead = isOnlyRead
      } else {
        this.isOnlyRead = false
      }
      if (id != null && id != "") {
        let res = await reportInfo({ id: id });
        this.form = {
          Id: res.data.Id,
          Number: res.data.Number,//唯一编号
          BatchNo: res.data.BatchNo,//批次编号
          GoodNum: res.data.GoodNum, //良品数
          DefectNum: res.data.DefectNum,//不良品数
          DefectList: res.data.DefectList,//不良品项
          StartWork: res.data.StartWork,//开始时间
          EndWork: res.data.EndWork,//结束时间
          WorkTime: res.data.WorkTime, //报工时长
          OverReason: res.data.OverReason, //备注
          FlowId: res.data.FlowId, //流程表单id
          PhotoUrl: res.data.PhotoUrl,
          WorkTaskNumber: res.data.WorkOrder.WorkNumber + "-" + res.data.Oper.OperName,
          WorkOrderId: res.data.WorkOrderId,
          WorkTaskId: res.data.WorkTaskId,
          OperId: res.data.OperId,
          RouteOper: res.data.RouteOper,
          RepBat: res.data.RepBat
        };

        this.setCustomDefaultValue(res.data.RouteOper);
      } else {
        this.form = {
          Id: null,
          Number: "",//唯一编号
          BatchNo: "",//批次编号
          GoodNum: 1, //良品数
          DefectNum: "",//不良品数
          DefectList: [],//不良品项
          StartWork: "",//开始时间
          EndWork: "",//结束时间
          WorkTime: 0, //报工时长
          OverReason: "", //备注
          // FlowId: 0, //流程表单id
          PhotoUrl: '',
          WorkTaskNumber: '',
          WorkOrderId: '',
          WorkTaskId: '',
          OperId: '',
          RepBat: { "Id": "" }
        }

        let numres = await GeneratePlaneNumber()//获取报工编号
        this.form.Number = numres.data
        this.setCustomDefaultValue();
        let response = await factoryMesConfig();//获取生产报工相关配置
        this.mesform.ReportTemplateName = response.data.ReportTemplateName;
        this.mesform.ReportTemplateId = response.data.ReportTemplateId;
      }
      this.dialogVisible = true;
      if (this.mesform.ReportTemplateId > 0) {
        let fromInfo = await reportFormData({
          "Number": this.form.Number,
        })
        console.log(fromInfo, 'fromInfo', this.$refs.flowForm);
        this.$nextTick(async () => {
          await this.$refs.flowForm.InitData(
            this.mesform.ReportTemplateId,
            this.FlowParams,
            this.form.Id == null ? null : this.form.Number,
            fromInfo.data
          );
        })
      }
      let form = JSON.parse(JSON.stringify(this.form));
      this.form = JSON.parse(JSON.stringify(form));
    },
    submitFiledAdd(st) {
      //提交数据
      this.$refs["form"].validate(async (valid, validateResult) => {
        // console.log("检验");
        if (valid) {
          let submitForm = JSON.parse(JSON.stringify(this.form));
          for (let i = 0; i < this.filedTableList.length; i++) {
            let row = this.filedTableList[i];
            if (row.type == "数字") {
              if (submitForm.RepBat[row.mapid]) {
              } else {
                submitForm.RepBat[row.mapid] = Number(submitForm.RepBat[row.mapid]);
              }
            } else if (row.type == "时间") {
              submitForm.RepBat[row.mapid] = dayjs(submitForm.RepBat[row.mapid]).valueOf();
            } else if (row.type == "复选框") {
              if (submitForm.RepBat[row.mapid] && submitForm.RepBat[row.mapid].length > 0) {
                submitForm.RepBat[row.mapid] = submitForm.RepBat[row.mapid].join(",");
              } else {
                submitForm.RepBat[row.mapid] = "";
              }
            } else {
              if (submitForm.RepBat[row.mapid]) {
              } else {
                submitForm.RepBat[row.mapid] = "";
              }
            }
          }
          submitForm.Status = 0
          delete submitForm.FlowId
          let response;
          if (submitForm.Id) {
            response = await reportEdit(submitForm);
            response.data = submitForm.Id
          } else {
            response = await reportAdd(submitForm);
          }
          if (this.mesform.ReportTemplateId > 0) {
            if (st == 2) {
              let tmpmodel = this.$refs.flowForm.getModel();
              if (submitForm.Id) {
                tmpmodel["id"] = submitForm.Id;
              } else {
                tmpmodel["id"] = response.data;
              }

              await reportSubmitModel(tmpmodel);
            }
            else {
              await this.$refs.flowForm.submitForm(st);
            }
          }
          else {
            if (st == 2) {
              await reportSubmitModel({ id: response.data });
            }

          }
          this.dialogVisible = false;
          this.$emit("reloadData");
          this.$modal.msgSuccess("操作成功");
        } else {
          let errKey = Object.keys(validateResult)
          if (errKey && errKey[0]) {
            let findObj = this.filedTableList.find(row => row.mapid == errKey[0])
            if (findObj) {
              this.dialogName = 'custominfo'
            } else {
              this.dialogName = 'baseinfo'
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

.report_add_dialog {
  ::v-deep .el-form-item__label {
    line-height: 14px;
  }

  .flow_con {
    ::v-deep .el-form-item__label {
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

  .label_tip {
    height: 40px;
    text-align: left;

    .label_text {
      height: 8px;
    }
  }

  .tip_con {
    width: 182px;
    height: 30px;
    border: 1px solid rgba(223, 226, 234, 1);
    color: rgba(120, 130, 157, 1);
    line-height: 30px;
    margin-right: 10px;
    text-align: center;
    border-radius: 4px;

    .zhongtaiiconfont {
      font-size: 10px;
    }
  }

  .el-upload--picture-card {
    background-color: #202e57;
    border: none;
  }

  ::v-deep .el-upload--picture-card i {
    font-size: 16px;
  }

  ::v-deep .el-upload.el-upload--picture-card {
    width: 70px;
    height: 40px;
    line-height: 40px;
  }

  ::v-deep .component-upload-image {
    height: 40px;

    // margin-bottom: 20px;
    .el-upload__tip {
      margin-top: 0;
    }
  }

  ::v-deep .el-upload-list--picture-card .el-upload-list__item {
    width: 70px;
    height: 40px;
  }
}
</style>