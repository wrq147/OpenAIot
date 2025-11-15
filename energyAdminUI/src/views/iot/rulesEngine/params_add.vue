<template>
  <div>
    <el-dialog
      :title="activeParamsLine > -1 ? '编辑参数' : '添加参数'"
      :visible.sync="paramsDrawer"
      width="50%"
      :destroy-on-close="true"
      :close-on-click-modal="false"
    >
      <el-form ref="typeForm" :rules="paramsRules" :model="typeForm" label-width="100px" class="params_form">
        <el-form-item label="字段名称" prop="name">
          <el-input v-model="typeForm.name" placeholder="请输入字段名称" />
        </el-form-item>
        <el-form-item label="标识符" prop="code">
          <el-input :disabled="activeParamsLine > -1" v-model="typeForm.code" placeholder="请输入标识符" @input="typeForm.code = typeForm.code.replace(/[^a-zA-Z0-9_]{1,20}$/g, '')"/>
          <span style="font-size: 14px; color: #909399">
            <i class="zhongtaiiconfont zhongtai-icon-zhuyi" style="margin-right: 5px; font-size: 14px"></i>1到20位字母，数字，下划线
          </span>
        </el-form-item>
        <el-form-item label="数据类型" prop="type">
          <el-select filterable v-model="typeForm.type" placeholder="请选择数据类型" style="width: 100%" @change="typeFormTypeChange">
            <el-option v-for="item in typeList" :key="item.value" :label="item.label" :value="item.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="是否多选" prop="multi" v-if="typeForm.type=='enum'">
          <el-switch v-model="typeForm.multi" active-color="#13ce66" inactive-color="#ff4949"></el-switch>
        </el-form-item>
        <enum-item v-model="typeForm" v-if="typeForm.type == 'enum'"></enum-item>
        <el-form-item :label="triggerWay == 2 ? '值' : '默认值'" prop="defval">
          <el-row>
            <el-col :span="14">
              <param-item ref="defParamVal" :Item="typeForm" :disabled="triggerWay == 2 ? false : typeForm.disabledDef"></param-item>
            </el-col>
            <el-col :span="8" :offset="1">
              <el-checkbox v-model="typeForm.disabledDef">无效值</el-checkbox>
              <el-checkbox v-model="typeForm.readOnly">只读</el-checkbox>
            </el-col>
          </el-row>
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input
            type="textarea"
            :autosize="{ minRows: 3}"
            placeholder="请输入内容"
            v-model="typeForm.remark">
          </el-input>
        </el-form-item>
      </el-form>
      <div class="demo-drawer__footer" style="text-align: center; margin-top: 40p; padding-bottom: 20px">
        <el-button @click="paramsDrawer = false">取 消</el-button>
        <el-button type="primary" @click="joinParams" :loading="paramsLoading">{{ paramsLoading ? "提交中 ..." : "保 存" }}</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import paramItem from "../funInput/paramItem.vue";
import enumItem from "../funInput/enumItem.vue";
export default {
  name: "rulesParamsAdd",
  components: {
    paramItem,
    enumItem,
  },
  props:{
    typeList:{
        type:Array,
        default:()=>{
            return []
        }
    },
    triggerWay:{
        type:[Number,String]
    },
    HttpParams:{
        type:Array,
        default:()=>{
            return []
        }
    }
  },
  data() {
    const noRepeat2 = (rule, value, callback) => {
      let lisArr = this.HttpParams.map((x) => x.code);
      if (lisArr.includes(this.typeForm.code) && this.activeParamsLine == -1) {
        // 未上传文件
        callback("该标识符已存在，请重新输入");
      }
      callback();
    };
    return {
        paramsLoading: false,
        paramsRules: {
            name: [{ required: true, trigger: "blur", message: "请输入字段名称" }],
            code: [
            { required: true, trigger: "blur", message: "请输入标识符" },
            {
                min: 1,
                max: 20,
                message: "长度在 1 到 20 个字符",
                trigger: "blur",
            },
            { validator: noRepeat2, trigger: "blur" },
            ],
            type: [
            { required: true, trigger: "change", message: "请选择数据类型" },
            ],
        },
        typeForm: {
            //属性定义根据数据类型判断
            name: "", //名称
            code: "", //标识符
            type: null, //数据类型
            readOnly: false,
            defval: null,
            multi:false,
            remark: "",
        },
        paramsDrawer: false,
        activeParamsLine: -1,
    };
  },

  mounted() {},

  methods: {
    typeFormTypeChange(){
        this.typeForm.defval=null
    },
    openParamsDrawer() {
      //打开填写参数的弹出层
      this.paramsDrawer = true;
      this.activeParamsLine = -1;
      this.typeForm = {
        //属性定义根据数据类型判断
        name: "", //名称
        code: "", //标识符
        type: null,
        defval: null,
        readOnly: false,
        disabledDef: true,
        multi:false,
        remark: "",
      };
      this.resetForm("typeForm");
    },
    editParams(row, rowIndex) {
      //编辑添加的参数
      this.openParamsDrawer();
      this.$set(this,"typeForm",row);
      this.typeForm.disabledDef = this.typeForm.defval == null;
      if(row.remark==null){
        this.typeForm.remark = "";
      }
      if(row.multi==null){
        this.typeForm.multi = false;
      }
      this.activeParamsLine = rowIndex;
    },
    joinParams() {
      //添加输入输出参数
      this.$refs["typeForm"].validate((val1) => {
        if (val1) {
          this.paramsLoading = true;
          this.typeForm.defval = this.$refs.defParamVal.getVal();
          if (this.activeParamsLine > -1) {
            this.HttpParams[this.activeParamsLine] = this.typeForm;
          } else {
            this.typeForm.disabledDef == true;
            this.HttpParams.push(this.typeForm);
          }
          this.$emit('joinParams',this.HttpParams)
          this.paramsDrawer = false; //关闭弹窗
          this.paramsLoading = false;
        } else {
          // console.log("err");
        }
      });
    },
  },
};
</script>
