<template>
  <el-dialog :title="dialogTitle" :visible.sync="dialogVisible" width="900px" top="2vh">
    <el-form ref="form" :model="form" label-width="100px" :rules="rules">
      <div>
        <el-row :gutter="10">
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
      </div>
      <div class="dialog_slot_title">
        <el-tabs v-model="dialogName" type="card"
          v-if="(ExtItems && ExtItems.length > 0) || (mesform.ReportTemplateId != null && mesform.ReportTemplateId !== 0)">
          <el-tab-pane name="custominfo"><span slot="label"
              v-if="ExtItems && ExtItems.length > 0">自定义信息</span></el-tab-pane>
          <el-tab-pane name="approveinfo"
            v-if="mesform.ReportTemplateId != null && mesform.ReportTemplateId !== 0"><span
              slot="label">报工审批</span></el-tab-pane>
        </el-tabs>
      </div>
      <el-row :gutter="10" v-show="dialogName == 'custominfo'">
        <template v-for="(item, ix) in ExtItems">
          <el-col :span="12" :key="'custom_filed' + ix">
            <el-form-item :label="item.name" :prop="'RepBat.' + item.mapid">
              <el-select @change="customValChange" :disabled="setFormItemReadOnly(item)" :allow-create="item.is_add"
                :multiple="item.type == '复选框'" :clearable="!item.is_required" v-model="form.RepBat[item.mapid]"
                :placeholder="item.prompt_text ? item.prompt_text : '请选择'" style="width: 100%"
                v-if="(item.type == '单选框' && item.show_way == '下拉') || (item.type == '复选框' && item.show_way == '下拉')">
                <template v-for="it in item.optionals">
                  <el-option :label="it" :value="it" :key="it + ix"></el-option>
                </template>
              </el-select>
              <el-radio-group @change="customValChange" :disabled="setFormItemReadOnly(item)"
                v-model="form.RepBat[item.mapid]" v-if="item.type == '单选框' && item.show_way == '平铺'">
                <template v-for="it in item.optionals">
                  <el-radio :label="it" :key="it + ix">{{ it }}</el-radio>
                </template>
              </el-radio-group>
              <el-checkbox-group @change="customValChange" :disabled="setFormItemReadOnly(item)"
                v-model="form.RepBat[item.mapid]" v-if="item.type == '复选框' && item.show_way == '平铺'">
                <template v-for="it in item.optionals">
                  <el-checkbox :label="it" :key="it + ix">{{ it }}</el-checkbox>
                </template>
              </el-checkbox-group>
              <el-date-picker @blur="customValChange" @change="customValChange" :disabled="setFormItemReadOnly(item)"
                v-if="item.type == '时间'" v-model="form.RepBat[item.mapid]" type="datetime"
                :placeholder="item.prompt_text ? item.prompt_text : '请选择'" style="width: 100%"
                :value-format="item.format" :format="item.format"></el-date-picker>
              <el-input @input="customValChange" :disabled="setFormItemReadOnly(item)" v-if="item.type == '文本'"
                :placeholder="item.prompt_text ? item.prompt_text : '请输入'"
                :type="item.is_multiple ? 'textarea' : 'text'" v-model="form.RepBat[item.mapid]"></el-input>
              <el-input @input="customValChange" :disabled="setFormItemReadOnly(item)" v-if="item.type == '数字'"
                :placeholder="item.prompt_text ? item.prompt_text : '请输入'" type="number"
                v-model="form.RepBat[item.mapid]" :precision="item.decimals"></el-input>
              <el-link :disabled="setFormItemReadOnly(item)" v-if="item.type == '超链接'" href="#" target="_blank">{{
                item.describe_text }}</el-link>
              <!-- <image-upload @input="customValChange" v-model="form[item.mapid]" :limit="1" v-if="item.type == '图片'"></image-upload> -->
              <div class="avatar_con" v-if="item.type == '图片'">
                <image-upload :disabled="setFormItemReadOnly(item)" @input="customValChange($event, item)"
                  v-model="form.RepBat[item.mapid]" :limit="1" :isShowLeft="true">
                  <template #tip>
                    <span></span>
                  </template>
                </image-upload>
              </div>
              <file-upload :disabled="setFormItemReadOnly(item)" @input="customValChange($event, item)"
                v-model="form.RepBat[item.mapid]" :limit="1" v-if="item.type == '附件'" :isShowLeft="true">
                <template #tip>
                  <span></span>
                </template>
              </file-upload>
              <el-select :disabled="setFormItemReadOnly(item)" @focus="afterValSearch(form.RepBat[item.mapid], item)"
                :clearable="true" @change="customValChange2($event, item)" style="width: 100%"
                v-model="form.RepBat[item.mapid]" filterable remote reserve-keyword
                :placeholder="item.prompt_text ? item.prompt_text : '请选择'"
                :remote-method="(query) => associationMethod(query, item)" :loading="objectLoading"
                v-if="item.type == '关联对象'">
                <el-option v-for="ite in associationObject[item.mapid]" :key="ite.Value" :label="ite.Name"
                  :value="ite.Value + ',' + ite.ValueName">{{ ite.Name }}</el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </template>
      </el-row>
      <div v-show="dialogName == 'approveinfo' && mesform.ReportTemplateId !== 0">
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
import { orgFormFields } from "@/api/factory/customFields";
import { GeneratePlaneNumber, reportFormData, reportSubmitModel, reportAdd, reportEdit, reportInfo } from '@/api/mes/report'
import { operInfo } from "@/api/mes/oper";
import { factoryMesConfig } from "@/api/mes/config";
import { setCustomDefaultValue, checkBeforeSave, setFormItemHide } from '@/utils/field.js'
export default {
  name: 'AdminUiReportAdd',
  components: { AddEmbed, OrgPicker },
  data() {
    return {
      dialogName: "custominfo",
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
      isOnlyRead: false,
      NeedReason: false,
      FieldsPerms: []
    };
  },
  computed: {
    FlowParams: function () {
      return {
        "@from": this.form.Number,
        "@fromtype": "生产报工",
      };
    },
    dialogTitle: function () {
      return this.isOnlyRead ? "编辑报工" : "添加报工";
    },
    ExtItems: function () {
      let newFields = this.filedTableList.filter(item => {
        let fperm = this.FieldsPerms.filter(x => x.id == item.mapid);
        if (fperm.length > 0) {
          if (fperm[0].perm == 'H') {
            return false;
          }
        }
        return !setFormItemHide(item, this.form.RepBat);
      });
      return newFields;
    },
  },
  mounted() {

  },

  methods: {
    resetNeedReason() {
      if (!this.form.TaskInfo || !this.form.GoodNum || this.form.WorkTime === undefined) {
        this.NeedReason = false;
        return;
      }
      let needWorkTime = this.form.TaskInfo.WorkTime * this.form.TaskInfo.PropOf * (this.form.GoodNum + this.form.DefectNum);
      if (needWorkTime < this.form.WorkTime && this.form.OverReason == "") {
        this.NeedReason = true;
      }
      else {
        this.NeedReason = false;
      }
    },
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
      this.form.TaskInfo = val;
      this.FieldsPerms = JSON.parse(operRes.data.ReportFields);
            console.info(this.FieldsPerms)
      setCustomDefaultValue(this.filedTableList, this.form.RepBat, this.rules, null, "RepBat.");
      this.$forceUpdate()
      this.resetNeedReason();
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
      this.$forceUpdate();
      this.resetNeedReason();
    },
    
    setFormItemReadOnly(item) {
      let fperm = this.FieldsPerms.filter(x => x.id == item.mapid);
      if (fperm.length > 0) {
        if (fperm[0].perm == 'R') {
          return true;
        }
      }
      return item.is_readonly && this.isOnlyRead;
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
        this.$forceUpdate();
      })
    },
    customValChange() {
      let form = JSON.parse(JSON.stringify(this.form));
      this.$nextTick(() => {
        this.form = JSON.parse(JSON.stringify(form));
        this.$refs["form"].validate((valid) => { });
        this.$forceUpdate();
      })
    },

    async getCustomFiled() {
      //获取自定义的字段
      this.filedTableList = [];
      let res = await orgFormFields({ field: "报工", ext: true, isfixed: true });
      if (res.data) {
        this.filedTableList = res.data;
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
          TaskInfo: res.data.TaskInfo,
          RepBat: res.data.RepBat
        };
        this.FieldsPerms = JSON.parse(res.data.Oper.ReportFields);
        setCustomDefaultValue(this.filedTableList, this.form.RepBat, this.rules, res.data.RepBat, "RepBat.");
        this.resetNeedReason();
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
        this.form.Number = numres.data;
      }
      let response = await factoryMesConfig();//获取生产报工相关配置
      this.mesform.ReportTemplateName = response.data.ReportTemplateName;
      this.mesform.ReportTemplateId = response.data.ReportTemplateId;
      this.dialogVisible = true;
      if (this.mesform.ReportTemplateId > 0) {
        let fromInfo = await reportFormData({
          "Number": this.form.Number,
        })
        this.$nextTick(async () => {
          await this.$refs.flowForm.InitData(
            this.mesform.ReportTemplateId,
            this.FlowParams,
            this.form.Id == null ? null : this.form.Number,
            fromInfo.data
          );
        })
      }
    },
    submitFiledAdd(st) {
      //提交数据
      this.$refs["form"].validate(async (valid, validateResult) => {
        if (valid) {
          let submitForm = JSON.parse(JSON.stringify(this.form));
          checkBeforeSave(this.filedTableList, submitForm.RepBat)
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
            }
          }
        }
      });
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


.dialog_slot_title {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  width: 100%;
  position: relative;

  .el-tabs {
    width: 100%;
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