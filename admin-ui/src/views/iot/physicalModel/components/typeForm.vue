<template>
  <div>
    <el-form ref="typeForm" :rules="typeRules" :model="typeForm" label-width="100px">
      <el-form-item label="数据预处理" v-if="activeDefinition == 'attribute'">
        <el-switch v-model="expressEnable" @change="changeExpress"></el-switch>
        <div v-if="expressEnable">
          <el-input v-model="typeForm.express" placeholder="请输入预处理表达式" style="width:400px"></el-input>
          <el-link icon="el-icon-edit" @click="popExpressDlg()" style="margin-left: 10px;cursor: pointer;"></el-link>
        </div>
      </el-form-item>
      <el-form-item label="数据类型" prop="type">
        <el-select filterable v-model="typeForm.type" placeholder="请选择数据类型" style="width: 100%" @change="changeType">
          <el-option v-for="item in typeList" :key="item.value" :label="item.label" :value="item.value"
            v-show="!(activeDefinition == 'expands' && item.value == 'file')"></el-option>
        </el-select>
        <span style="font-size: 14px; color: #909399" v-if="(typeForm.type == 'int' || typeForm.type == 'float') &&
      activeDefinition != 'expands'
      ">
          <i class="zhongtaiiconfont zhongtai-icon-zhuyi" style="font-size:14px;margin-right:5px;"></i>间距 + 值 * 倍数
        </span>
      </el-form-item>
      <el-form-item label="数值范围" :prop="typeForm.min ? 'max' : 'min'"
        v-if="typeForm.type == 'int' || typeForm.type == 'float'">
        <el-input-number v-model="typeForm.min" controls-position="right" @change="maxNumChange"
          placeholder="最小值"></el-input-number>
        <span style="margin: 0 10px">~</span>
        <el-input-number v-model="typeForm.max" controls-position="right" @change="minNumChange"
          placeholder="最大值"></el-input-number>
      </el-form-item>
      <el-form-item label="精度" prop="decimals" v-if="typeForm.type == 'float'">
        <el-select v-model="typeForm.decimals" placeholder="请选择精度" style="width: 100%">
          <el-option :label="ite.label" :value="ite.value" v-for="ite in decimalsList" :key="ite.value"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="间距" prop="spacing" v-if="(typeForm.type == 'int' || typeForm.type == 'float') && activeDefinition == 'attribute'">
        <el-input type="number" v-model="typeForm.spacing" placeholder="请输入间距" @blur="typeForm.spacing=parseFloat(typeForm.spacing)"></el-input>
      </el-form-item>
      <el-form-item label="倍数" prop="multiple" v-if="(typeForm.type == 'int' || typeForm.type == 'float') && activeDefinition == 'attribute'">
        <el-input type="number" v-model="typeForm.multiple" placeholder="请输入倍数" @blur="typeForm.multiple=parseFloat(typeForm.multiple)"></el-input>
      </el-form-item>
      <el-form-item label="单位" prop="unit" v-if="typeForm.type == 'int' || typeForm.type == 'float'">
        <el-input v-model="typeForm.unit" placeholder="请输入单位" />
      </el-form-item>
      <el-form-item label="最大长度" prop="maxLen" v-if="typeForm.type == 'string'">
        <el-input-number v-model="typeForm.maxLen" controls-position="right" :max="255" :min="0"
          placeholder="0-255"></el-input-number>
        <span>字符</span>
      </el-form-item>
      <el-form-item label="时间格式" prop="format" v-if="typeForm.type == 'date'">
        <el-input v-model="typeForm.format" placeholder="请输入日期格式化字符串" />
      </el-form-item>
      <el-form-item label="坐标系转换" prop="format" v-if="typeForm.type == 'geo'&&activeDefinition == 'attribute'">
        <el-switch
          v-model="typeForm.usingGCJTo"
          active-color="#13ce66"
          inactive-color="#ff4949">
        </el-switch>
        <el-tooltip style="margin-left:10px;" effect="dark" content="是否开启大地坐标系转火星坐标系" placement="top">
          <i class="el-icon-question"></i>
        </el-tooltip>
      </el-form-item>
      <el-form-item label="为真描述" prop="trueText" v-if="typeForm.type == 'boolean'">
        <el-input v-model="typeForm.trueText" placeholder="请输入为真时的文字描述" />
      </el-form-item>
      <el-form-item label="为假描述" prop="falseText" v-if="typeForm.type == 'boolean'">
        <el-input v-model="typeForm.falseText" placeholder="请输入为假时的文字描述" />
      </el-form-item>
      <el-form-item label="文件类型" prop="bodyType" v-if="typeForm.type == 'file'">
        <el-select v-model="typeForm.bodyType" placeholder="请选择文件类型" style="width: 100%">
          <el-option v-for="item in bodyTypeList" :key="item" :label="item" :value="item"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="枚举值" prop="elementsLis" v-if="typeForm.type == 'enum'">
        <div class="enum_list_con">
          <div class="enum_list">
            <div class="enum_li" v-for="(it, inx) in typeForm.elementsLis" :key="it.key">
              <span v-if="activeDefinition == 'attribute'">{{ it.key }}:{{ it.value }}</span>
              <span v-else>{{ it.value }}</span>
              <i class="zhongtaiiconfont zhongtai-icon-a-guanbihui meijuclose" @click="delEnum(inx,it)"></i>
            </div>
            <div class="enum_con" @click="showEnumInput" v-if="!(showEnumKey || showEnumVal)">
              + 枚举值
            </div>
          </div>
          <div class="enum_btn" @click="exportEumVal(typeForm.elements)">导入</div>
        </div>

        <el-form ref="typeForm2" style="width: 250px; display: inline-block" :typeForm="typeForm" :rules="typeRules"
          label-width="65px" v-if="showEnumKey || showEnumVal">
          <el-form-item label="枚举key" prop="enumKey" class="enum_key" v-if="showEnumKey">
            <el-input v-model="typeForm.enumKey" ref="enumKeyAuto" placeholder="请输入枚举key" @blur="eumKeyBlur" />
          </el-form-item>
          <el-form-item label="枚举值" prop="enumValue" v-if="showEnumVal">
            <el-input v-model="typeForm.enumValue" ref="enumValAuto" placeholder="请输入枚举值" @blur="eumValBlur" />
          </el-form-item>
        </el-form>

        <div style="font-size: 14px; color: #909399" v-if="activeDefinition == 'attribute'">
          <i class="zhongtaiiconfont zhongtai-icon-zhuyi" style="font-size:14px;margin-right:5px;"></i>枚举的键名是原值，枚举的键值是转换的目标值
        </div>
      </el-form-item>
    </el-form>

    <el-dialog title="编辑表达式" append-to-body :close-on-click-modal="false" :visible.sync="expressOpen" width="670px">
      <div>
        <ExpressEditor :content="typeForm.express" :typeForm="typeForm" :attrTableData="attrTableData" ref="expEd" ></ExpressEditor>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="expressOpen = false;">取 消</el-button>
        <el-button type="primary" @click="confirmExpress">确 定</el-button>
      </div>
    </el-dialog>
    <el-dialog :close-on-click-modal="false" :title="upload.title" :visible.sync="upload.open" width="400px" append-to-body :destroy-on-close="true">
      <el-upload
        ref="uploadref"
        :limit="1"
        accept=".txt"
        :on-progress="handleFileUploadProgress"
        :on-change="handleImport"
        :auto-upload="false"
        action="#"
        drag
      >
        <i class="el-icon-upload"></i>
        <div class="el-upload__text">
          将文件拖到此处，或
          <em>点击上传</em>
        </div>
        <div class="el-upload__tip text-center" slot="tip">
          <span style="line-height: 33px;">仅允许导入txt格式文件。</span>
          <el-link
            type="primary"
            :underline="false"
            style="font-size:12px;vertical-align: baseline;"
            @click="onImportTemplate"
          >下载模板</el-link>
        </div>
      </el-upload>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitFileForm">确 定</el-button>
        <el-button @click="upload.open = false">取 消</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import ExpressEditor from "./PreprocessEditor.vue";
export default {
  name: "AdminUiTypeform",
  components: {
    ExpressEditor
  },
  props: {
    activeDefinition: {
      //当前活动的物模型页面
      type: String,
      default: "",
    },
    paramsForm: {
      type: Object,
      default: () => {
        return {};
      },
    },
    enumKeyList: {
      //枚举列表
      type: Array,
      default: () => {
        return [];
      },
    },
    attrTableData: {
      type: Array
    }
  },
  data() {
    return {
      expressOpen: false,
      expressEnable: false,
      typeForm: {
        //属性定义根据数据类型判断
        type: null,
        max: 999999999, //最大值
        min: -9999999, //最小值
        spacing: 0, //间距
        decimals: 2, //浮点数精度
        multiple: 0, //倍数
        unit: "", //单位
        trueText: "", //为真时文字描述
        falseText: "", //为假时文字描述
        maxLen: undefined, //最大长度
        format: "yyyy-MM-dd", //时间格式
        bodyType: null,
        elements: {}, //添加的枚举列表,保存数据库的数据
        elementsLis: [], //添加的枚举列表
        enumKey: "", //枚举key
        enumValue: "", //枚举value
        lng: 0, //经度
        lat: 0, //纬度
      }, //添加参数表单
      decimalsList: [
        { label: "0.1", value: 1 },
        { label: "0.01", value: 2 },
        { label: "0.001", value: 3 },
        { label: "0.0001", value: 4 },
        { label: "0.00001", value: 5 },
        { label: "0.0000001", value: 6 },
        { label: "0.00000001", value: 7 },
        { label: "0.000000001", value: 8 },
      ], //精度列表
      bodyTypeList: ["url", "base64"], //文件类型
      showEnumKey: false, //是否显示枚举key输入框
      showEnumVal: false, //是否显示枚举值输入框
      typeRules: {
        //属性定义参数规则判定
        type: [
          { required: true, trigger: "change", message: "请选择数据类型" },
        ],
        decimals: [
          { required: true, trigger: "change", message: "请选择精度" },
        ],
        max: [{ required: true, trigger: "blur", message: "请输入最大值" }],
        min: [{ required: true, trigger: "blur", message: "请输入最小值" }],
        spacing: [{ required: true, trigger: "blur", message: "请输入间距" }],
        multiple: [
          { required: true, trigger: "change", message: "请输入倍数" },
        ],
        trueText: [
          { required: true, trigger: "blur", message: "请输入为真时文字描述" },
        ],
        falseText: [
          { required: true, trigger: "blur", message: "请输入为假时文字描述" },
        ],
        maxLen: [
          { required: true, trigger: "blur", message: "请输入最大长度" },
        ],
        bodyType: [
          { required: true, trigger: "change", message: "请选择文件保存类型" },
        ],
        elementsLis: [
          { required: true, trigger: "change", message: "请添加枚举值" },
        ],
      }, //添加参数验证
      typeList: [
        { alabel: "整型", label: "整型(Int)", value: "int" },
        { alabel: "浮点", label: "浮点型(Float)", value: "float" },
        { alabel: "字符", label: "字符型(String)", value: "string" },
        { alabel: "时间", label: "时间型(Date)", value: "date" },
        { alabel: "布尔", label: "布尔型(Boolean)", value: "boolean" },
        { alabel: "枚举", label: "枚举型(Enum)", value: "enum" },
        { alabel: "文件", label: "文件类型(File)", value: "file" },
        { alabel: "位置", label: "设备位置(Geo)", value: "geo" },
      ], //数据类型列表
      upload:{
        title:'',
        open:false,
        file:null,
        isUploading:false
      }
    };
  },
  watch: {
    "typeForm.elementsLis"() {
      if (this.$refs["typeForm"]) {
        this.$refs["typeForm"].validateField("elementsLis", (valid) => {
          // console.log("枚举验证", valid);

          if (!valid) {
          } else {
            console.log("error submit!!");
            return false;
          }
        });
      }
    },
  },
  mounted() {
    this.$nextTick(() => {
      this.expressEnable =
        this.typeForm.express != null && this.typeForm.express != "";
    });
  },
  methods: {
    popExpressDlg() {
      this.expressOpen = true;
    },
    confirmExpress() {
      let tmpexpress = this.$refs.expEd.getFormulaStr();
      if (this.$refs.expEd.isExpress(tmpexpress).success == false) {
        this.$message.error("表达式格式错误");
        return;
      }
      this.typeForm.express = tmpexpress;
      this.expressOpen = false;
    },
    changeExpress(newVal) {
      if (newVal == false) {
        this.typeForm.express = "";
      }
    },
    reloadTypeForm() {
      this.typeForm = {
        //属性定义根据数据类型判断
        type: null,
        max: 999999999, //最大值
        min: -9999999, //最小值
        spacing: 0, //间距
        decimals: 2, //浮点数精度
        multiple: 0, //倍数
        unit: "", //单位
        usingGCJTo:false,//是否开启火星坐标系转换
        trueText: "", //为真时文字描述
        falseText: "", //为假时文字描述
        maxLen: 0, //最大长度
        format: "yyyy-MM-dd", //时间格式
        bodyType: null,
        elements: {}, //添加的枚举列表,保存数据库的数据
        elementsLis: [], //添加的枚举列表
        enumKey: "", //枚举key
        enumValue: "", //枚举value
        lng: 0, //经度
        lat: 0, //纬度
      };
      this.resetForm("typeForm");
    },
    setOptionsData() {
      //根据不同的数据类型获取不同的option
      let option = {};
      if (
        this.activeDefinition == "attribute" ||
        this.activeDefinition == "expands"
      ) {
        if (this.activeDefinition == "expands") {
          if (this.typeForm.type == "int") {
            option = {
              type: this.typeForm.type,
              express: this.typeForm.express,
              max: this.typeForm.max,
              min: this.typeForm.min,
              unit: this.typeForm.unit,
            };
            return option;
          }
          if (this.typeForm.type == "float") {
            option = {
              type: this.typeForm.type,
              express: this.typeForm.express,
              decimals: this.typeForm.decimals,
              max: this.typeForm.max,
              min: this.typeForm.min,
              unit: this.typeForm.unit,
            };
            return option;
          }
        } else if (this.activeDefinition == "attribute") {
          if (this.typeForm.type == "int") {
            option = {
              type: this.typeForm.type,
              express: this.typeForm.express,
              max: this.typeForm.max,
              min: this.typeForm.min,
              unit: this.typeForm.unit,
              spacing: this.typeForm.spacing, //数据将加上间距进行传输，如上报值为1，间距设置为10，则转化后为1+10=11
              multiple: this.typeForm.multiple, //数据将乘以倍数转换进行传输，如上报值为1，倍数设置为10，则转化后数据为1*10=10
            };
            return option;
          }
          if (this.typeForm.type == "float") {
            option = {
              type: this.typeForm.type,
              express: this.typeForm.express,
              decimals: this.typeForm.decimals,
              max: this.typeForm.max,
              min: this.typeForm.min,
              unit: this.typeForm.unit,
              spacing: this.typeForm.spacing, //数据将加上间距进行传输，如上报值为1，间距设置为10，则转化后为1+10=11
              multiple: this.typeForm.multiple, //数据将乘以倍数转换进行传输，如上报值为1，倍数设置为10，则转化后数据为1*10=10
            };
            return option;
          }
        }

        if (this.typeForm.type == "boolean") {
          option = {
            type: this.typeForm.type,
            express: this.typeForm.express,
            trueText: this.typeForm.trueText, //为真时文字描述
            falseText: this.typeForm.falseText, //为假时文字描述
          };
          return option;
        }
        if (this.typeForm.type == "string") {
          option = {
            type: this.typeForm.type,
            express: this.typeForm.express,
            maxLen: this.typeForm.maxLen, //字符串最大长度
          };
          // console.log("添加标签",option);

          return option;
        }
        if (this.typeForm.type == "date") {
          option = {
            type: this.typeForm.type,
            express: this.typeForm.express,
            format: this.typeForm.format, //字符串最大长度
          };
          return option;
        }
        if (this.typeForm.type == "enum") {
          option = {
            type: this.typeForm.type,
            express: this.typeForm.express,
            elements: this.typeForm.elements, //字符串最大长度
          };
          return option;
        }
        if (this.typeForm.type == "file") {
          option = {
            type: this.typeForm.type,
            express: this.typeForm.express,
            bodyType: this.typeForm.bodyType, //字符串最大长度
          };
          return option;
        }
        if (this.typeForm.type == "geo") {
          console.log("保存类型为geo", this.typeForm);

          option = {
            type: this.typeForm.type,
            usingGCJTo:this.typeForm.usingGCJTo,
            express: this.typeForm.express,
          };
          return option;
        }
      } else if (
        this.activeDefinition == "function" ||
        this.activeDefinition == "event"
      ) {
        if (this.typeForm.type == "int") {
          option = {
            name: this.paramsForm.name,
            code: this.paramsForm.code,
            option: {
              type: this.typeForm.type,
              max: this.typeForm.max,
              min: this.typeForm.min,
              unit: this.typeForm.unit,
              spacing: this.typeForm.spacing, //数据将加上间距进行传输，如上报值为1，间距设置为10，则转化后为1+10=11
              multiple: this.typeForm.multiple, //数据将乘以倍数转换进行传输，如上报值为1，倍数设置为10，则转化后数据为1*10=10
            },
          };
          return option;
        }
        if (this.typeForm.type == "float") {
          option = {
            name: this.paramsForm.name,
            code: this.paramsForm.code,
            option: {
              type: this.typeForm.type,
              decimals: this.typeForm.decimals,
              max: this.typeForm.max,
              min: this.typeForm.min,
              unit: this.typeForm.unit,
              spacing: this.typeForm.spacing, //数据将加上间距进行传输，如上报值为1，间距设置为10，则转化后为1+10=11
              multiple: this.typeForm.multiple, //数据将乘以倍数转换进行传输，如上报值为1，倍数设置为10，则转化后数据为1*10=10
            },
          };
          return option;
        }
        if (this.typeForm.type == "boolean") {
          option = {
            name: this.paramsForm.name,
            code: this.paramsForm.code,
            option: {
              type: this.typeForm.type,
              trueText: this.typeForm.trueText, //为真时文字描述
              falseText: this.typeForm.falseText, //为假时文字描述
            },
          };
          return option;
        }
        if (this.typeForm.type == "string") {
          option = {
            name: this.paramsForm.name,
            code: this.paramsForm.code,
            option: {
              type: this.typeForm.type,
              maxLen: this.typeForm.maxLen, //字符串最大长度
            },
          };
          return option;
        }
        if (this.typeForm.type == "date") {
          option = {
            name: this.paramsForm.name,
            code: this.paramsForm.code,
            option: {
              type: this.typeForm.type,
              format: this.typeForm.format, //字符串最大长度
            },
          };
          return option;
        }
        if (this.typeForm.type == "enum") {
          option = {
            name: this.paramsForm.name,
            code: this.paramsForm.code,
            option: {
              type: this.typeForm.type,
              elements: this.typeForm.elements, //字符串最大长度
            },
          };
          return option;
        }
        if (this.typeForm.type == "file") {
          option = {
            name: this.paramsForm.name,
            code: this.paramsForm.code,
            option: {
              type: this.typeForm.type,
              bodyType: this.typeForm.bodyType, //字符串最大长度
            },
          };
          return option;
        }
        if (this.typeForm.type == "geo") {
          option = {
            name: this.paramsForm.name,
            code: this.paramsForm.code,
            option: {
              type: this.typeForm.type,
              lng: this.typeForm.lng,
              lat: this.typeForm.lat,
            },
          };
          return option;
        }
      }
    },
    setNOtInputsType(paramsForm) {
      //typeForm赋值
      this.typeForm = paramsForm.option;
      if (this.activeDefinition == "expands") {
        this.$emit("setCurType", this.typeForm.type);
      }
      if (this.typeForm.type == "enum") {
        this.typeForm.elementsLis = [];
        for (let key in this.typeForm.elements) {
          if (key && this.typeForm.elements[key]) {
            let obj = {
              key: key,
              value: this.typeForm.elements[key],
            };
            this.typeForm.elementsLis.push(obj);
          }
        }
      }
      else if(this.typeForm.type=="geo"){
        if(this.typeForm.usingGCJTo==null){
          this.$set(this.typeForm,"usingGCJTo",false);
        }
      }
    },
    setInputsType(paramsForm) {
      this.typeForm = { type: paramsForm.type };
    },
    setTypeVal(type) {
      this.typeForm.type = type;
    },
    delEnum(inx, item) {
      this.typeForm.elementsLis.splice(inx, 1);
      delete this.typeForm.elements[item.key];
      this.enumKeyList.splice(this.enumKeyList.findIndex((item) => item == item.key),1);
      this.$forceUpdate();
    },
    eumKeyBlur() {
      //失去枚举Key输入框的焦点
      console.log(this.typeForm.enumKey, "this.typeForm.enumKey");
      if (
        this.typeForm &&
        this.typeForm.enumKey &&
        this.typeForm.enumKey != undefined &&
        this.typeForm.enumKey != null &&
        this.typeForm.enumKey != ""
      ) {
        if (!this.enumKeyList.includes(this.typeForm.enumKey)) {
          this.showEnumKey = false;
          this.showEnumVal = true;
          let objs = {
            key: this.typeForm.enumKey,
            value: "",
          };
          this.typeForm.elementsLis.push(objs);
          this.typeForm.elements[this.typeForm.enumKey] = "";
          this.enumKeyList.push(this.typeForm.enumKey);
          // console.log(this.typeForm.elements, "枚举的值");
          let arr = JSON.parse(JSON.stringify(this.typeForm.elements));
          this.typeForm.elements = JSON.parse(JSON.stringify(arr));
          this.$nextTick(() => this.$refs.enumValAuto.focus());
        } else {
          this.$message.error("该枚举已存在");
          this.typeForm.enumKey = "";
          this.$nextTick(() => this.$refs.enumKeyAuto.focus());
        }
      } else {
        this.showEnumKey = false;
        this.showEnumVal = false;
      }
    },
    exportEumVal(){
      this.upload.title="导入枚举"
      this.upload.open=true
    },
    onImportTemplate() {
      let tmploading = this.$loading({
        lock: true,
        text: "导出中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      let tmname = new Date().getTime();;
      tmploading.close();
      const content = "键名  键值\r\n键名  键值"
      const blobData = new Blob([content], { type: 'text/plain;charset=utf-8' })
      const filename = `${tmname}.txt` //可以自定义后缀名

      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blobData, filename)
      } else {
        const anchor = document.createElement('a')
        anchor.href = window.URL.createObjectURL(blobData)
        anchor.download = filename
        anchor.click()
        window.URL.revokeObjectURL(blobData)
      }
    },
    // 文件上传中处理
    handleFileUploadProgress(event, file, fileList) {
      this.upload.isUploading = true;
    },
    
    handleImport(file, fileList) {
      console.log(file,'filefile');
      this.upload.file=file.raw
    },
    // 提交上传文件
    submitFileForm(){
      if(this.upload.file){
        let tmploading = this.$loading({
          lock: true,
          text: "导入中...",
          background: "rgba(0, 0, 0, 0.7)",
        });
        const reader = new FileReader()
        reader.readAsText(this.upload.file)
        reader.onload = (e)=> {
          const str = e.target.result
          // console.log("str导入的数据",str);
          let list1=str.split('\r\n')
          for(let i=0;i<list1.length;i++){
            let rowArr=list1[i].split(/\s+/)
            if(rowArr[0]&&rowArr[1]){
              this.enumKeyList.push(rowArr[0]);
              let obj = {
                key: rowArr[0],
                value: rowArr[1],
              };
              this.typeForm.elements[rowArr[0]]=rowArr[1]
              this.typeForm.elementsLis.push(obj);
            }
          }

          this.upload.open=false
          this.$forceUpdate()
          tmploading.close();
        }
      }
      
    },
    eumValBlur() {
      //失去枚举值输入框的焦点
      if (this.typeForm.enumValue) {
        this.showEnumVal = false;

        if (this.activeDefinition != 'attribute'){
          this.typeForm.enumKey=this.typeForm.enumValue;
          if (!this.enumKeyList.includes(this.typeForm.enumKey)) {
            let objs = {
              key: this.typeForm.enumKey,
              value: "",
            };
            this.typeForm.elementsLis.push(objs);
            this.enumKeyList.push(this.typeForm.enumKey);
          } else {
            this.$message.error("该枚举已存在");
            return;
          }
        }


        this.typeForm.elementsLis[this.typeForm.elementsLis.length - 1].value = this.typeForm.enumValue;
        this.typeForm.elements[this.typeForm.enumKey] = this.typeForm.enumValue;
        let arr = JSON.parse(JSON.stringify(this.typeForm.elements));
        this.typeForm.elements = JSON.parse(JSON.stringify(arr));
        this.typeForm.enumKey = "";
        this.typeForm.enumValue = "";
      }
    },
    showEnumInput() {
      //点击后显示枚举输入框
      if (this.activeDefinition == 'attribute') {
        this.showEnumKey = true;
        this.$nextTick(() => this.$refs.enumKeyAuto.focus());
      }
      else {
        this.showEnumVal = true;
      }
    },
    maxNumChange() {
      //最大值改变
    },
    minNumChange() {
      //最小值改变
    },
    changeType(value) {
      //数据类型发生改变的时候
      this.reloadTypeForm()
      this.typeForm.type = value;
      this.$emit("changeMapcode");
      if (value == "float" || value == "int") {
        this.typeForm.spacing = 0;
        this.typeForm.multiple = 0;
        if (value == "float") {
          this.typeForm.decimals = 2;
        }
      }
      if (value == "date") {
        this.typeForm.format = "yyy-MM-dd";
      }
      if (value == "geo") {
        this.typeForm.lng = 0;
        this.typeForm.lat = 0;
      }
      this.$emit("setCurType", this.typeForm.type);
    },
  },
};
</script>

<style lang="less" scoped>
.enum_list_con{
  display: inline-flex;
  flex-direction: row;
  justify-content: flex-start;
  align-items: flex-start;
  justify-content: space-between;
}
.enum_btn{
  width: 60px;
  height: 26px;
  border-radius: 5px;
  background: #007ACC;
  color: #ffffff;
  font-size: 14px;
  display: flex;
  justify-content: center;
  align-items: center;
}
  .enum_list {
    display: inline-flex;
    flex-direction: row;
    justify-content: flex-start;
    align-items: flex-start;
    flex-wrap: wrap;
    margin-right: -5px;
    width: 410px;
    .enum_li {
      padding: 0 15px 0 8px;
      height: 30px;
      line-height: 30px;
      text-align: center;
      border-radius: 15px;
      border: 1px solid #dddddd;
      display: inline-block;
      margin: 0 5px 5px 0;
      position: relative;
      .zhongtaiiconfont.meijuclose{
        font-size: 6px;
        position: absolute;
        right: 6px;
        top: 0px;
        cursor: pointer;
      }

      // .svg-icon {
      //   font-size: 6px;
      //   position: absolute;
      //   right: 6px;
      //   top: 12px;
      //   cursor: pointer;
      // }
    }
  }


.enum_con {
  width: 100px;
  height: 30px;
  line-height: 30px;
  text-align: center;
  border-radius: 15px;
  border: 1px dashed #dddddd;
  display: inline-block;
  cursor: pointer;
  margin-left: 5px;
}
</style>