<template>
  <div>
    <el-dialog :visible.sync="dialogVisible" width="800px" :show-close="false">
      <div slot="title" class="dialog_slot_title">
        <div class="title_text">添加供应商</div>
        <el-tabs v-model="dialogName" tab-position="top" :stretch="true" v-if="filedTableList&&filedTableList.length>0">
          <el-tab-pane name="null1" :disabled="true"><span slot="label"></span></el-tab-pane>
          <el-tab-pane name="null2" :disabled="true"><span slot="label"></span></el-tab-pane>
          <el-tab-pane name="custominfonull" :disabled="true" v-if="!filedTableList||filedTableList&&filedTableList.length==0"><span slot="label"></span></el-tab-pane>
          <el-tab-pane name="baseinfo"><span slot="label">基本信息</span></el-tab-pane>
          <el-tab-pane name="custominfo"><span slot="label" v-if="filedTableList&&filedTableList.length>0">自定义信息</span></el-tab-pane>
          <el-tab-pane name="null3" :disabled="true"><span slot="label"></span></el-tab-pane>
          <el-tab-pane name="null4" :disabled="true"><span slot="label"></span></el-tab-pane>
        </el-tabs>
        <div @click="handleClose" class="icon_con"><i class="el-icon-close" style="color: #93969b"></i></div>
      </div>
      <el-form ref="form" :model="form" label-width="100px" :rules="rules">
        <el-row :gutter="10" v-show="dialogName=='baseinfo'">
          <el-col :span="12">
            <el-form-item label="供应商编码" prop="Number" v-if="form.Number">
              <el-input v-model="form.Number" placeholder="请输入供应商编码" :disabled="true"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="供应商名称" prop="SupplierName">
              <el-input v-model="form.SupplierName" placeholder="请输入供应商名称"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="供应商全称" prop="FullName">
              <el-input v-model="form.FullName" placeholder="请输入供应商全称"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="供应商状态" prop="Status">
              <el-radio-group v-model="form.Status">
                <template v-for="it in labelList">
                  <el-radio :key="'status' + it.value" :label="it.value">{{ it.label }}</el-radio>
                </template>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系人" prop="ContactName">
              <el-input v-model="form.ContactName" placeholder="请输入联系人"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="手机号" prop="Tel">
              <el-input v-model="form.Tel" placeholder="请输入手机号"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item prop="AddressName" label="供应商地址">
              <el-input placeholder="请选择供应商地址" v-model="form.AddressName" @focus="choiceMap"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item label="详情地址" prop="AddressDetail">
              <el-input type="textarea" v-model="form.AddressDetail" placeholder="请输入备注"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item label="备注说明" prop="Remark">
              <el-input type="textarea" v-model="form.Remark" placeholder="请输入备注"></el-input>
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
                <el-date-picker @change="customValChange" :disabled="item.is_readonly" v-if="item.type == '时间'" v-model="form[item.mapid]"
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
                  :placeholder="item.prompt_text ? item.prompt_text : '请选择'" :remote-method="(query)=>associationMethod(query,item)" :loading="Supplierloading" v-if="item.type == '关联对象'">
                  <el-option v-for="ite in associationObject[item.mapid]" :key="ite.Value" :label="ite.Name" :value="ite.Value+','+ite.ValueName">{{ite.Name}}</el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </template>
        </el-row>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="submitFiledAdd">确 定</el-button>
      </span>
    </el-dialog>
    
  </div>
</template>

<script>
import { orgField } from "@/api/factory/customFields";
import {
  factorySearchObject
} from "@/api/factory/product";
import {
  addSupplierSave,
  factorySupplierInfo,
  editSupplierSave,
} from "@/api/factory/supplier";
import dayjs from "dayjs";

export default {
  name: "AdminUiProductAdd",
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
        SupplierName: "",
        FullName: "",
        Status: 1, //供应商
        ContactName: "",
        Tel: "",
        Lng: "",
        Lat: "",
        AddressCode: "", //区域代码
        AddressName: "", //地址名称
        AddressDetail: "", //地址详情
      },
      rules: {
        SupplierName: [
          { required: true, trigger: "blur", message: "供应商名称不能为空" },
        ],
        Status: [
          { required: true, trigger: "change", message: "供应商状态不能为空" },
        ],
        Tel:[
          { pattern: /^1(3|4|5|7|8)\d{9}$/,message: '请输入11位有效手机号！' },
        ]
      },
      filedTableList: [], //供应商自定义列表
      Supplierloading:false,
      associationObject:{},//所有关联对象对应的下拉的参数列表
      
    };
  },

  mounted() {},

  methods: {
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
      // console.log(res,'resres');
    },
    choiceMap() {
      //选择位置
      this.$emit('choiceMap')
    },
    returnMapInfo(info) {
      console.log("选择的地址信息", info);
      this.form.Lat = info.Lat;
      this.form.Lng = info.Lng;
      this.form.AddressName = info.AddressName;
      this.form.AddressDetail = info.AddressDetail;
      this.$forceUpdate();
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
    customValChange2(val,fidItem) {//数据发生变化后刷新，并验证表单
      // console.log("看看关联对象选择后有没有出现",fidItem);
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
      this.form = JSON.parse(JSON.stringify(form));
      this.$refs["form"].validate((valid) => {});
      this.$forceUpdate();
    },
    async openDialog(id) {
      await this.getCustomFiled();
      if (id) {
        let res = await factorySupplierInfo({ id: id });
        console.log("supplierInfo,供应商详情", res, this.filedTableList);
        let supplierInfo = res.data;
        this.form = {
          Id: supplierInfo.Id,
          Number: supplierInfo.Number,
          SupplierName: supplierInfo.SupplierName,
          FullName: supplierInfo.FullName,
          Status: Number(supplierInfo.Status), //供应商
          ContactName: supplierInfo.ContactName,
          Tel: supplierInfo.Tel,
          Lng: supplierInfo.Lng,
          Lat: supplierInfo.Lat,
          AddressCode: supplierInfo.AddressCode, //区域代码
          AddressName: supplierInfo.AddressName, //地址名称
          AddressDetail: supplierInfo.AddressDetail, //地址详情
        };
        this.setCustomDefaultValue(supplierInfo);
      } else {
        this.form = {
        SupplierName: "",
        FullName: "",
        Status: 1, //供应商
        ContactName: "",
        Tel: "",
        Lng: "",
        Lat: "",
        AddressCode: "", //区域代码
        AddressName: "", //地址名称
        AddressDetail: "", //地址详情
      }
        this.setCustomDefaultValue();
        // console.log("表单初始化",this.form);
      }
      this.dialogVisible = true;
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
      let res = await orgField({ orgId: orgId, field: "供应商" });
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
    submitFiledAdd() {
      //提交数据
      this.$refs["form"].validate((valid,validateResult) => {
        console.log("检验");
        if (valid) {
          let submitForm = JSON.parse(JSON.stringify(this.form));
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
          }
          console.log("提交的数据", submitForm);
          if (submitForm.Id) {
            editSupplierSave(submitForm).then((res) => {
              console.log("修改执行结果", res);
              this.$modal.msgSuccess("修改成功");
              this.dialogVisible = false;
              this.$emit("reloadData");
            });
          } else {
            addSupplierSave(submitForm).then((res) => {
              console.log("添加执行结果", res);
              this.$modal.msgSuccess("添加成功");
              this.dialogVisible = false;
              this.$emit("reloadData");
            });
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

<style lang="less" scoped>
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