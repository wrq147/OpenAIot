<template>
  <el-dialog title="提示" :visible.sync="dialogVisible" width="800px" :before-close="handleClose">
    <el-form ref="form" :model="form" label-width="100px" :rules="rules">
      <el-form-item label="字段名称" prop="name">
        <el-input v-model="form.name" placeholder="请输入字段名称"></el-input>
      </el-form-item>
      <el-form-item label="字段类型" prop="type">
        <el-select v-model="form.type" placeholder="请选择字段类型" :disabled="activeFiledIndex != -1">
          <template v-for="it in typeList">
            <el-option :label="it" :value="it" :key="it"></el-option>
          </template>
        </el-select>
      </el-form-item>
      <el-form-item label="隐藏条件">
        <el-button type="success" icon="el-icon-circle-plus-outline" plain @click="addConditionList">添加</el-button>
        <el-row :gutter="10" :key="'condition' + inx" v-for="(ite, inx) in form.conditions"
          style="margin-top:10px;margin-left:-60px">
          <el-col :span="2">
            <div style="font-size:12px">条件{{ inx + 1 }}:</div>
          </el-col>
          <el-col :span="4">
            <el-select v-model="form.conditions[inx].field" placeholder="请选择字段" @change="fieldChange($event, inx)">
              <el-option v-for="item in activeTypeFiledList" :key="item.mapid" :label="item.name"
                :value="item.mapid"></el-option>
            </el-select>
          </el-col>
          <el-col :span="3">
            <el-select class="time_mini" placeholder="请选择比较符" v-model="form.conditions[inx].compare"
              v-if="form.conditions[inx] && form.conditions[inx].valtype == '文本'">
              <el-option label="等于" value="="></el-option>
              <el-option label="不等于" value="!="></el-option>
              <el-option label="包含" value="IN"></el-option>
              <el-option label="不包含" value="NOTIN"></el-option>
              <el-option label="为空" value="ISNULL"></el-option>
              <el-option label="不为空" value="NOTNULL"></el-option>
            </el-select>
            <el-select class="time_mini" @change="numberCompareChange($event, inx)"
              v-model="form.conditions[inx].compare" placeholder="请选择比较符"
              v-if="form.conditions[inx] && form.conditions[inx].valtype == '数字'">
              <el-option v-for="item in compareList" :key="item.value" :label="item.label"
                :value="item.value"></el-option>
            </el-select>
            <el-select class="time_mini" @change="numberCompareChange($event, inx)"
              v-model="form.conditions[inx].compare" placeholder="请选择比较符"
              v-if="form.conditions[inx] && form.conditions[inx].valtype == '时间'">
              <el-option v-for="item in timeCompareList" :key="item.value" :label="item.label"
                :value="item.value"></el-option>
            </el-select>
          </el-col>
          <!-- <el-col :span="3">
            <el-select @change="conditionValtype($event,inx)" v-model="form.conditions[inx].valtype" placeholder="请选择值类型" :disabled="true">
              <el-option v-for="item in typeList" :key="item.value" :label="item.lable" :value="item.value"></el-option>
            </el-select>
          </el-col> -->
          <el-col :span="2" v-if="form.conditions[inx] && form.conditions[inx].valtype == '时间'" class="time_mini">
            <el-select @change="conditionValtype($event, inx)" v-model="form.conditions[inx].val.timeType"
              placeholder="请选择">
              <el-option label="日" value="日"></el-option>
              <el-option label="时" value="时"></el-option>
              <el-option label="分" value="分"></el-option>
              <el-option label="秒" value="秒"></el-option>
            </el-select>
          </el-col>
          <div
            v-if="form.conditions[inx].compare && form.conditions[inx].compare != 'ISNULL' && form.conditions[inx].compare && form.conditions[inx].compare != 'NOTNULL'">
            <el-col :span="9" v-if="form.conditions[inx] && form.conditions[inx].valtype == '数字'">
              <div class="number_val_con"
                v-if="form.conditions[inx].compare && form.conditions[inx].compare == 'INRANGE' || form.conditions[inx].compare && form.conditions[inx].compare == 'NOTINRANGE'">
                <el-input style="width:calc(50% - 10px)" type="number" v-model="form.conditions[inx].val.max"
                  placeholder="最小值"></el-input>
                <span>~</span>
                <el-input style="width:calc(50% - 10px)" type="number" v-model="form.conditions[inx].val.min"
                  placeholder="最大值"></el-input>
              </div>
              <div v-else>
                <el-input type="number" v-model="form.conditions[inx].val" placeholder="请输入值"></el-input>
              </div>
            </el-col>
            <el-col :span="5"
              v-if="form.conditions[inx] && form.conditions[inx].valtype == '时间' && form.conditions[inx].compare && form.conditions[inx].compare != 'SELECTRANGE' && form.conditions[inx].compare != 'DYNAMICS'">
              <!-- <el-input type="text" v-model="form.conditions[inx].val" placeholder="请输入值"></el-input> -->
              <el-date-picker style="width:100%" v-model="form.conditions[inx].val.timeValue" type="date"
                placeholder="请选择值"
                v-if="form.conditions[inx].val.timeType && form.conditions[inx].val.timeType == '日'"></el-date-picker>
              <el-date-picker style="width:100%" v-model="form.conditions[inx].val.timeValue" type="datetime"
                placeholder="请选择值" v-else :value-format="returnFormatDate(inx)"
                :format="returnFormatDate(inx)"></el-date-picker>
            </el-col>
            <el-col :span="11"
              v-if="form.conditions[inx] && form.conditions[inx].valtype == '时间' && form.conditions[inx].compare && form.conditions[inx].compare == 'SELECTRANGE'">
              <!-- <el-input type="text" v-model="form.conditions[inx].val" placeholder="请输入值"></el-input> -->
              <el-date-picker style="width:100%" v-model="form.conditions[inx].val.timeValue" type="daterange"
                placeholder="请选择值"
                v-if="form.conditions[inx].val.timeType && form.conditions[inx].val.timeType == '日'"></el-date-picker>
              <el-date-picker style="width:100%" v-model="form.conditions[inx].val.timeValue" type="datetimerange"
                placeholder="请选择值" v-else :value-format="returnFormatDate(inx)"
                :format="returnFormatDate(inx)"></el-date-picker>
            </el-col>
            <el-col :span="11"
              v-if="form.conditions[inx] && form.conditions[inx].valtype == '时间' && form.conditions[inx].compare && form.conditions[inx].compare == 'DYNAMICS'">
              <!-- <el-input type="text" v-model="form.conditions[inx].val" placeholder="请输入值"></el-input> -->
              <el-date-picker :key="timeComptKey" style="width:100%" v-model="form.conditions[inx].val.timeValue"
                type="datetimerange" align="right" unlink-panels range-separator="至" start-placeholder="开始时间"
                end-placeholder="结束时间" :picker-options="pickerOptions" :value-format="returnFormatDate(inx)"
                :format="returnFormatDate(inx)">
              </el-date-picker>
            </el-col>
            <el-col :span="5" v-if="form.conditions[inx] && form.conditions[inx].valtype == '文本'">
              <el-input type="text" v-model="form.conditions[inx].val" placeholder="请输入值"></el-input>
            </el-col>
          </div>
          <el-col :span="2">
            <el-button size="mini" @click="delConditionsItem(inx)" type="danger" icon="el-icon-delete"
              circle></el-button>
          </el-col>
        </el-row>
        <el-row :gutter="10" style="margin-top:10px;margin-left:-60px" v-if="form.conditions && form.conditions.length > 1">
          <template v-for="(item2, index) in form.conditions">
            <el-col :span="2" :key="'groups' + index" v-if="index == 0">条件{{ index + 1 }}:</el-col>
            <el-col :span="3" v-if="index > 0" :key="'groups2' + index">
              <el-select v-model="form.groups[index - 1]" placeholder="请选择值类型">
                <el-option label="与" value="and"></el-option>
                <el-option label="或" value="or"></el-option>
              </el-select>
            </el-col>
            <el-col :span="3" :key="'groups' + index" v-if="index > 0">条件{{ index + 1 }}:</el-col>
          </template>
        </el-row>
      </el-form-item>
      <el-form-item label="是否必填">
        <el-checkbox v-model="form.is_required"></el-checkbox><span style="margin-left:5px">是否必填</span>
      </el-form-item>
      <div class="CheckBoxField" v-if="form.type == '复选框' || form.type == '单选框'">
        <!-- 多选 -->
        <el-form-item label="添加选项" v-if="form.show_way == '下拉'">
          <el-checkbox v-model="form.is_add"></el-checkbox><span style="margin-left:5px">是否添加选项</span>
        </el-form-item>
        <el-form-item label="可选项">
          <div class="options_con">
            <template v-if="form.optionals && form.optionals.length > 0">
              <div v-for="(it, inx) in form.optionals" class="options_li" :key="'opts' + it + '' + inx">
                <span>{{ it }}</span>
                <i class="el-icon-circle-close meijuclose" @click="delOptional(inx)"></i>
              </div>
            </template>
            <div class="options_li" @click="showHideInput" v-if="!showInput">+可选项</div>
            <div class="option_input" v-if="showInput"><el-input v-model="activeSelectValue" ref="enumKeyAuto"
                placeholder="请输入选项" @blur="inputBlur" @change="inputBlur" /></div>
          </div>
        </el-form-item>
        <el-form-item label="默认选项" v-if="form.type == '单选'">
          <el-select v-model="form.show_way" placeholder="请选择默认选项" clearable filterable>
            <template v-if="form.optionals && form.optionals.length > 0">
              <el-option v-for="it in form.optionals" :label="it" :value="it" :key="'def' + it"></el-option>
            </template>
          </el-select>
        </el-form-item>
        <el-form-item label="显示方式" prop="show_way">
          <el-select v-model="form.show_way" placeholder="请选择显示方式">
            <el-option label="下拉" value="下拉"></el-option>
            <el-option label="平铺" value="平铺"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="引导文字">
          <el-input placeholder="请输入引导文字" type="textarea" v-model="form.prompt_text"></el-input>
        </el-form-item>
        <el-form-item label="描述文字">
          <el-input placeholder="请输入描述文字" type="textarea" v-model="form.describe_text"></el-input>
        </el-form-item>
      </div>
      <div class="numberFiled" v-if="form.type == '超链接'">
        <el-form-item label="是否只读">
          <el-checkbox v-model="form.is_readonly"></el-checkbox><span style="margin-left:5px">是否只读</span>
        </el-form-item>
        <el-form-item label="引导文字">
          <el-input placeholder="请输入引导文字" type="textarea" v-model="form.prompt_text"></el-input>
        </el-form-item>
        <el-form-item label="描述文字">
          <el-input placeholder="请输入描述文字" type="textarea" v-model="form.describe_text"></el-input>
        </el-form-item>
      </div>
      <div class="numberFiled" v-if="form.type == '数字'">
        <el-form-item label="是否只读">
          <el-checkbox v-model="form.is_readonly"></el-checkbox><span style="margin-left:5px">是否只读</span>
        </el-form-item>
        <el-form-item label="千分位">
          <el-checkbox v-model="form.is_thousandth"></el-checkbox><span style="margin-left:5px">是否显示千分位分割符</span>
        </el-form-item>
        <el-form-item label="小数点位数">
          <el-input-number v-model="form.decimals" :min="0" :precision="0" label="小数点位数" :step="1"></el-input-number>
        </el-form-item>
        <el-form-item label="默认值">
          <el-input placeholder="请输入默认值" type="number" v-model="form.defval"></el-input>
        </el-form-item>
        <el-form-item label="引导文字">
          <el-input placeholder="请输入引导文字" type="textarea" v-model="form.prompt_text"></el-input>
        </el-form-item>
        <el-form-item label="描述文字">
          <el-input placeholder="请输入描述文字" type="textarea" v-model="form.describe_text"></el-input>
        </el-form-item>
      </div>
      <div class="objectFiled" v-if="form.type == '关联对象'">
        <el-form-item label="对象类型">
          <el-select v-model="form.object_type" placeholder="请选择对象类型" @change="objectFiledTypeChnage">
            <el-option label="用户" value="用户"></el-option>
            <el-option label="部门" value="部门"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="填充规则">
          <el-card class="box-card">
            <div slot="header" class="clearfix">
              <el-button :disabled="!form.object_type" style="float: left; padding: 3px 0" type="text"
                @click="addFillRules" class="el-icon-circle-plus-outline">填充规则</el-button>
            </div>
            <table>
              <tr v-for="(it, ix) in form.items" :key="'rul' + ix">
                <td>
                  <i class="el-icon-remove-outline" style="color:#FF3B30;font-size:16px;" @click="delFillRules(ix)"></i>
                </td>
                <td>
                  <el-select v-model="form.items[ix].source_obj" :placeholder="'请选择' + form.object_type + '的字段'">
                    <template
                      v-if="objectFiledSelectListObj[form.object_type] && objectFiledSelectListObj[form.object_type].length > 0">
                      <el-option :label="its.name" :value="its.mapid"
                        v-for="its in objectFiledSelectListObj[form.object_type]"
                        :key="'sourcesel' + its.mapid"></el-option>
                    </template>
                  </el-select>
                </td>
                <td>
                  的值填充到
                </td>
                <td>
                  <el-select v-model="form.items[ix].field" :placeholder="'请选择' + customFiledType + '的字段'">
                    <template
                      v-if="objectFiledSelectListObj[customFiledType] && objectFiledSelectListObj[customFiledType].length > 0">
                      <el-option :label="its.name" :value="its.mapid"
                        v-for="its in objectFiledSelectListObj[customFiledType]"
                        :key="'sourcesel' + its.mapid"></el-option>
                    </template>
                  </el-select>
                </td>
              </tr>
            </table>
          </el-card>
        </el-form-item>
      </div>
      <div class="textFiled" v-if="form.type == '文本'">
        <el-form-item label="是否只读">
          <el-checkbox v-model="form.is_readonly"></el-checkbox><span style="margin-left:5px">是否只读</span>
        </el-form-item>
        <el-form-item label="是否多行">
          <el-checkbox v-model="form.is_multiple"></el-checkbox><span style="margin-left:5px">是否多行</span>
        </el-form-item>
        <el-form-item label="扫码输入">
          <el-checkbox v-model="form.is_scan"></el-checkbox><span style="margin-left:5px">是否允许扫码输入</span>
        </el-form-item>
        <el-form-item label="扫码结果">
          <el-checkbox v-model="form.is_update_scan"></el-checkbox><span style="margin-left:5px">是否可修改扫码结果</span>
        </el-form-item>
        <el-form-item label="引导文字">
          <el-input placeholder="请输入引导文字" type="textarea" v-model="form.prompt_text"></el-input>
        </el-form-item>
        <el-form-item label="描述文字">
          <el-input placeholder="请输入描述文字" type="textarea" v-model="form.describe_text"></el-input>
        </el-form-item>
      </div>
      <div class="timeFiled" v-if="form.type == '时间'">
        <el-form-item label="是否只读">
          <el-checkbox v-model="form.is_readonly"></el-checkbox><span style="margin-left:5px">是否只读</span>
        </el-form-item>
        <el-form-item label="格式化字符串">
          <!-- <el-input placeholder="请输入格式化字符串" type="textarea" v-model="form.format"></el-input> -->
          <el-select v-model="form.format" placeholder="请选择格式化字符串" @change="objectFiledTypeChnage">
            <el-option label="年-月-日" value="yyyy-MM-dd"></el-option>
            <el-option label="年-月-日 时" value="yyyy-MM-dd HH"></el-option>
            <el-option label="年-月-日 时:分" value="yyyy-MM-dd HH:mm"></el-option>
            <el-option label="年-月-日 时:分:秒" value="yyyy-MM-dd HH:mm:ss"></el-option>
            <el-option label="时:分:秒" value="HH:mm:ss"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="默认值" v-if="form.format">
          <el-date-picker v-model="form.defvalTime" type="datetime" placeholder="选择" :value-format="form.format"
            :format="form.format"></el-date-picker>
        </el-form-item>
      </div>
    </el-form>
    <span slot="footer" class="dialog-footer">
      <el-button @click="dialogVisible = false">取 消</el-button>
      <el-button type="primary" @click="submitFiledAdd">确 定</el-button>
    </span>
  </el-dialog>
</template>

<script>
import { saveOrgField, orgFormFields } from '@/api/factory/customFields'
import dayjs from 'dayjs';
import { pickerOptions } from './js/filed'
export default {
  name: "AdminUiFiledAdd",
  props: {
    filedTableList: {
      type: Array,
      default: () => {
        return []
      }
    },
    customFiledType: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      MaxFieldCount: 100,
      pickerOptions: pickerOptions,
      activeSelectValue: '',
      dialogVisible: false,
      form: {
        mapid: '',
        name: '',//字段名称
        type: '',//字段类型
        is_required: false,//是否必填
        conditions: [],
        groups: [],
        is_readonly: false,
        prompt_text: '',//引导
        describe_text: '',//描述
        defval: null,//默认值
        object_type: '',//关联对象类型：用户、部门
        items: [],//数据填充规则
        is_add: false,
        optionals: [],//单选多选的选项值
        show_way: '下拉',//单选多选的选项排列方式
        is_multiple: false,//文本是否多行
        is_scan: false,//文本是否允许扫码输入,
        is_update_scan: false,//文本是否允许修改扫码输入结果,
        format: '',//时间的格式
        decimals: 0,
        is_thousandth: false,
        defvalTime: null
      },
      typeList: ['文本', '数字', '时间', '附件', '图片', '超链接', '单选框', '复选框', '关联对象'],
      // valtypeList:[{lable:'字符串',value:'string'},{lable:'数字',value:'number'},{lable:'时间',value:'time'}],
      fieldOptions: [],//字段
      compareList: [
        { label: '大于', value: '>' },
        { label: '小于', value: '<' },
        { label: '等于', value: '==' },
        { label: '不等于', value: '><' },
        { label: '大于等于', value: '>=' },
        { label: '小于等于', value: '<=' },
        { label: '为空', value: 'ISNULL' },
        { label: '不为空', value: 'NOTNULL' },
        { label: '在范围内', value: 'INRANGE' },
        { label: '不在范围内', value: 'NOTINRANGE' },
      ],//数字比较符
      timeCompareList: [
        { label: '大于', value: '>' },
        { label: '小于', value: '<' },
        { label: '等于', value: '==' },
        { label: '不等于', value: '><' },
        { label: '大于等于', value: '>=' },
        { label: '小于等于', value: '<=' },
        { label: '为空', value: 'ISNULL' },
        { label: '不为空', value: 'NOTNULL' },
        { label: '选择范围', value: 'SELECTRANGE' },
        { label: '动态筛选', value: 'DYNAMICS' },
      ],//时间比较符
      rules: {
        name: [{ required: true, trigger: "blur", message: "请输入字段名称" }],
        type: [{ required: true, trigger: "change", message: "请选择字段类型" }],
        show_way: [{ required: true, trigger: "change", message: "请选择排列方式" }],
      },
      activeFiledIndex: -1,
      activeTypeFiledList: [],//当前添加字段类型对象的所有字段
      objectFiledSelectListObj: {},//当前添加字段类型对象的所有字段
      timeComptKey: 1,//时间组件的key
      showInput: false,//单选框或者多选框的选项输入是否显示
    };
  },

  mounted() { },

  methods: {
    showHideInput() {
      this.showInput = true
      this.$nextTick(() => {
        this.$refs.enumKeyAuto.focus()
      })
    },
    conditionValtype(val, inx) {
      this.timeComptKey = (new Date).getTime()
    },
    returnFormatDate(inx) {
      if (this.form.conditions[inx].val && this.form.conditions[inx].val.timeType) {
        switch (this.form.conditions[inx].val.timeType) {
          case '日':
            return 'yyyy-MM-dd';
          case '时':
            return 'yyyy-MM-dd HH';
          case '分':
            return 'yyyy-MM-dd HH:mm';
          case '秒':
            return 'yyyy-MM-dd HH:mm:ss';
        }
      }
    },
    fieldChange(val, inx) {//切换隐藏规则设置的字段
      if (this.form.conditions[inx].field) {
        let activeFieldInfo = this.activeTypeFiledList.find(ro => ro.mapid == this.form.conditions[inx].field)
        this.form.conditions[inx].valtype = activeFieldInfo.type
        if (this.form.conditions[inx].valtype == '时间') {
          this.form.conditions[inx].val = {
            timeType: '日',
            timeValue: null
          }
        }
      }

    },
    numberCompareChange(val, inx) {
      if (this.form.conditions[inx].compare && this.form.conditions[inx].compare == 'INRANGE' || this.form.conditions[inx].compare && this.form.conditions[inx].compare == 'NOTINRANGE') {
        this.form.conditions[inx].val = {
          min: null,
          max: null
        }
      } else {
        if (this.form.conditions[inx].valtype != '时间') {
          this.form.conditions[inx].val = null
        }
      }

    },
    async objectFiledTypeChnage() {
      if (this.form.object_type) {
        if (this.objectFiledSelectListObj[this.form.object_type] && this.objectFiledSelectListObj[this.form.object_type] != {}) { } else {
          this.objectFiledSelectListObj[this.form.object_type] = await this.loadOrgFormFields(this.form.object_type, true)
        }
      }
    },
    async openDialog(index) {
      this.dialogVisible = true
      this.resetForm('form')
      if (index == undefined) {
        this.activeFiledIndex = -1
        this.form = {
          mapid: '',
          name: '',//字段名称
          type: '',//字段类型
          is_required: false,//是否必填
          conditions: [],
          groups: [],
          is_readonly: false,
          prompt_text: '',//引导
          describe_text: '',//描述
          defval: null,//默认值
          object_type: '',//关联对象类型：用户、部门
          items: [],//数据填充规则
          is_add: false,
          optionals: [],//单选多选的选项值
          show_way: '下拉',//单选多选的选项排列方式
          is_multiple: false,//文本是否多行
          is_scan: false,//文本是否允许扫码输入,
          is_update_scan: false,//文本是否允许修改扫码输入结果,
          format: '',//时间的格式
          decimals: 0,
          is_thousandth: false,
          defvalTime: null
        }
      } else {
        this.activeFiledIndex = index
        this.form = JSON.parse(JSON.stringify(this.filedTableList[index]))
        if (this.form.type == '时间') {
          let newStr = this.form.format.replace(/y/g, "Y");
          newStr = newStr.replace(/d/g, "D")
          console.log("newStr", newStr);
          this.form.defvalTime = dayjs(this.form.defval).format(newStr)
        }
      }
      if (this.customFiledType) {

        let activeTypeFiledList = await this.loadOrgFormFields(this.customFiledType, true)
        this.activeTypeFiledList = activeTypeFiledList.filter(rw => rw.type == '文本' || rw.type == '数字' || rw.type == '时间')//隐藏规则设置sh
        this.objectFiledSelectListObj[this.customFiledType] = JSON.parse(JSON.stringify(activeTypeFiledList))
      }
      if (this.form.object_type) {
        this.objectFiledSelectListObj[this.form.object_type] = await this.loadOrgFormFields(this.form.object_type, true)
        // console.log(this.objectFiledSelectListObj,'this.objectFiledSelectListObj');
      }
      this.$forceUpdate()
    },
    delFillRules(ix) {
      this.form.items.splice(ix, 1)
    },
    addFillRules() {
      //添加填充规则
      if (this.form.items) {
        this.form.items.push({ source_obj: '', field: '' })
      } else {
        this.form.items = []
        this.form.items.push({ source_obj: '', field: '' })
      }
    },
    inputBlur() {
      if (this.activeSelectValue) {
        if (this.form.optionals) {
          this.form.optionals.push(this.activeSelectValue)
        } else {
          this.form.optionals = []
          this.form.optionals.push(this.activeSelectValue)
        }
        this.activeSelectValue = ''
      } else {
        this.showInput = false
      }

    },
    delOptional(inx) {
      this.form.optionals.splice(inx, 1)
    },
    addConditionList() {
      //添加隐藏条件
      if (this.form.conditions) {
        this.form.conditions.push({ field: "", compare: "", valtype: "数字", val: "" })
      } else {
        this.form.conditions = []
        this.form.conditions.push({ field: "", compare: "", valtype: "数字", val: "" })
      }
      if (this.form.conditions && this.form.conditions.length >= 2) {
        if (this.form.conditions.length == 2) {
          this.form.groups = []
        }
        this.form.groups.push('and')
      } else {
        this.form.groups = []
      }
    },
    delConditionsItem(inx) {
      this.form.conditions.splice(inx, 1);
      if (inx > 0) {

        this.form.groups.splice(inx - 1, 1)
      }

    },
    setMapid() {
      //获取字段的唯一id
      try {
        if (this.activeFiledIndex == -1) {
          let mapidArr = []
          let filedTableList = JSON.parse(JSON.stringify(this.filedTableList))
          if (filedTableList && filedTableList.length > 0) {
            if (this.form.type == '数字' || this.form.type == '时间') {
              let filterArr = filedTableList.filter(rw => rw.mapid.indexOf('NumExt') > -1)
              if (filterArr) {
                mapidArr = (filterArr.map(row => {
                  if (row.mapid) {
                    return Number(row.mapid.slice(6))
                  }
                })).filter(rw => rw != undefined)
              }
              let arr = []
              if (mapidArr && mapidArr.length > 0) {
                arr = this.findMissingNumbers(1, this.MaxFieldCount, mapidArr)
                // console.log(arr);
                if (arr && arr.length > 0) {
                  const min = Math.min(...arr);
                  this.form.mapid = 'NumExt' + min
                } else {
                  const max = Math.max(...mapidArr);
                  this.form.mapid = 'NumExt' + Number(max + 1)
                }
              } else {
                this.form.mapid = 'NumExt1'
              }
            } else {
              let filterArr = filedTableList.filter(rw => rw.mapid.indexOf('StrExt') > -1)
              if (filterArr) {
                mapidArr = (filterArr.map(row => {
                  if (row.mapid) {
                    return Number(row.mapid.slice(6))
                  }
                })).filter(rw => rw != undefined)
              }

              let arr = []
              if (mapidArr && mapidArr.length > 0) {
                arr = this.findMissingNumbers(1, this.MaxFieldCount, mapidArr)//获取中间间断的数据
                if (arr && arr.length > 0) {
                  const min = Math.min(...arr);
                  this.form.mapid = 'StrExt' + min
                } else {
                  const max = Math.max(...mapidArr);
                  this.form.mapid = 'StrExt' + Number(max + 1)
                }
              } else {
                this.form.mapid = 'StrExt1'
              }

            }
          } else {
            if (this.form.type == '数字' || this.form.type == '时间') {
              this.form.mapid = 'NumExt1'
            } else {
              this.form.mapid = 'StrExt1'
            }
          }
        }

      } catch (error) {
        console.log(error, 'error');
      }

    },
    async loadOrgFormFields(field, ext) {
      let res = await orgFormFields({ field: field, ext })
      // console.log("字段列表",res);
      return res.data
    },
    findMissingNumbers(start, end, numArr) {
      let numbers = new Set(); // 使用Set来存储所有可能的数字
      for (let i = start; i <= end; i++) {
        numbers.add(i);
      }
      // 假设我们还有一个实际的序列来找出缺失的数字
      const actualSequence = JSON.parse(JSON.stringify(numArr)); // 示例序列
      actualSequence.forEach(num => numbers.delete(num)); // 从Set中删除实际存在的数字
      return Array.from(numbers); // 将Set转换为数组并返回
    },
    submitFiledAdd() {
      //提交字段
      this.$refs["form"].validate(valid => {
        if (valid) {
          this.setMapid()
          let submitFiledList = JSON.parse(JSON.stringify(this.filedTableList))
          let subForm = JSON.parse(JSON.stringify(this.form))
          if (this.activeFiledIndex == -1) {
            if (subForm.type == '时间') {
              subForm.defval = dayjs(subForm.defvalTime).valueOf()
            }
            delete subForm.defvalTime
            submitFiledList.push(subForm)
          } else {
            submitFiledList[this.activeFiledIndex] = JSON.parse(JSON.stringify(subForm))
          }
          if (submitFiledList.length > this.MaxFieldCount) {
            this.$modal.msgError("自定义字段数量不能超过【" + this.MaxFieldCount + "】个");
            return;
          }
          saveOrgField({ field: this.customFiledType, val: JSON.stringify(submitFiledList) }).then(res => {
            // console.log("添加成功",res);
            if (this.activeFiledIndex != -1) {
              this.$modal.msgSuccess("修改成功");
            } else {
              this.$modal.msgSuccess("添加成功");
            }
            this.$emit('getFiledList')
            this.dialogVisible = false
          })
        }
      })
    },
    handleClose() {
      this.dialogVisible = false;
    },
  },
};
</script>
<style lang="less" scoped>
.options_con {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  margin-top: -5px;
  margin-right: -5px;

  .options_li {
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0 10px;
    border-radius: 40px;
    line-height: 35px;
    font-size: 14px;
    border: 1px dashed #333333;
    position: relative;
    margin-top: 5px;
    margin-right: 5px;

    .meijuclose {
      width: 16px;
      height: 16px;
      background: #ffffff;
      font-size: 16px;
      position: absolute;
      right: -5px;
      top: -5px;
    }
  }

  .option_input {
    width: 200px;
    margin-top: 5px;
  }
}

.time_mini {
  ::v-deep .el-input input {
    padding-left: 10px;
    padding-right: 25px;
  }
}

.number_val_con {
  display: flex;
  justify-content: space-between;
  align-content: center;
}
</style>