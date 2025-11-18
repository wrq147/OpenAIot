<template>
  <el-dialog :visible.sync="dialogVisible" width="800px" :show-close="false" top="3vh">
    <div slot="title" class="dialog_slot_title">
      <div class="title_text">添加产品</div>
      <el-tabs v-model="dialogName" tab-position="top" :stretch="true" v-if="filedTableList&&filedTableList.length>0">
        <el-tab-pane name="null1" :disabled="true"><span slot="label"></span></el-tab-pane>
        <el-tab-pane name="null2" :disabled="true"><span slot="label"></span></el-tab-pane>
        <el-tab-pane name="custominfonull" :disabled="true" v-if="!filedTableList||filedTableList&&filedTableList.length==0"><span slot="label"></span></el-tab-pane>
        <el-tab-pane name="baseinfo"><span slot="label">基本信息</span></el-tab-pane>
        <el-tab-pane name="custominfo" v-if="filedTableList&&filedTableList.length>0"><span slot="label">自定义信息</span></el-tab-pane>
        <el-tab-pane name="null3" :disabled="true"><span slot="label"></span></el-tab-pane>
        <el-tab-pane name="null4" :disabled="true"><span slot="label"></span></el-tab-pane>
      </el-tabs>
      <div @click="handleClose" class="icon_con"><i class="el-icon-close" style="color: #93969b"></i></div>
    </div>
    <el-form ref="form" :model="form" label-width="100px" :rules="rules" :key="'proform'+formKey">
      <el-row :gutter="10" v-show="dialogName=='baseinfo'">
        <el-col :span="12">
          <el-form-item label="产品编码" prop="SkuNumber">
            <el-input v-model="form.SkuNumber" placeholder="请输入产品编码" :disabled="true"></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="产品名称" prop="ProductName">
            <el-input v-model="form.ProductName" placeholder="请输入产品名称"></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="生产来源" prop="ProductFrom">
            <el-select v-model="form.ProductFrom" placeholder="请选择生产来源" style="width: 100%">
              <el-option label="自制" value="自制"></el-option>
              <el-option label="外购" value="外购"></el-option>
              <el-option label="委外" value="委外"></el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="产品标签" prop="ProductLabel">
            <el-select clearable v-model="form.ProductLabel" placeholder="请选择产品标签" style="width: 100%" :disabled="!!form.Id">
              <template v-for="it in labelList">
                <el-option :label="it.label" :value="it.value" :key="it.value"></el-option>
              </template>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="产品分组" prop="TypeId">
            <el-select @change="productTypeChange" clearable v-model="form.TypeId" placeholder="请选择产品分组" style="width: 100%">
              <template v-for="it in productTypeList">
                <el-option :label="it.Name" :value="it.Id" :key="it.Id"></el-option>
              </template>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="产品属性" prop="Prop">
            <el-select clearable v-model="form.Prop" placeholder="请选择产品属性" style="width: 100%">
              <template v-for="it in propertiesData">
                <el-option :label="it" :value="it" :key="it"></el-option>
              </template>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="产品图片" prop="PhotoUrl">
            <!-- <image-upload v-model="form.PhotoUrl" :limit="1"></image-upload> -->
            <div class="avatar_con">
              <image-upload v-model="form.PhotoUrl" :limit="1" :isShowLeft="true">
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
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="包装单位" prop="Unit">
            <el-select clearable v-model="form.Unit" placeholder="请选择包装单位" style="width: 100%">
              <template v-for="it in Unitoptions">
                <el-option :label="it.UnitName" :value="it.UnitName" :key="it.Id"></el-option>
              </template>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="总计量" prop="Total">
            <el-input type="number" v-model="form.Total" placeholder="请输入总计量"></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="计量单位" prop="MinUnit">
            <el-select clearable v-model="form.MinUnit" placeholder="请选择计量单位" style="width: 100%">
              <template v-for="it in Unitoptions">
                <el-option :label="it.UnitName" :value="it.UnitName" :key="it.Id"></el-option>
              </template>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12" v-if="isCheckPermi(['/IoTService/IotProduct/ListPage'])&&form.ProductLabel=='F'">
          <el-form-item label="绑定协议" prop="IOTProductId">
            <el-select clearable style="width: 100%" v-model="form.IOTProductId" filterable remote reserve-keyword
              placeholder="请输入需要关联的协议名称" :remote-method="IOTProductRemoteMethod" :loading="IOTProductloading">
              <el-option v-for="item in IOTProductoptions" :key="item.Id" :label="item.Name" :value="item.Id">{{item.Name}}</el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="产品规格" prop="Specs">
            <el-input v-model="form.Specs" placeholder="请输入产品规格"></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="成本单价" prop="Price">
            <el-input type="number" v-model="form.Price" placeholder="请输入成本单价"></el-input>
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="销售单价" prop="SalesPrice">
            <el-input type="number" v-model="form.SalesPrice" placeholder="请输入销售单价"></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="工艺路线" prop="Route">
            <!-- <el-select clearable v-model="form.Route" placeholder="请选择工艺路线" style="width: 100%">
              <template v-for="it in typeList">
                <el-option :label="it" :value="it" :key="it"></el-option>
              </template>
            </el-select> -->
            <el-select clearable style="width: 100%" v-model="form.Route" filterable remote reserve-keyword
              placeholder="请输入工艺路线" :remote-method="routeRemoteMethod" :loading="Routeloading">
              <el-option v-for="item in Routeoptions" :key="item.Id" :label="item.RouteName" :value="item.Id">{{item.RouteName}}</el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="供应商" prop="Supplier">
            <el-select clearable style="width: 100%" v-model="form.Supplier" filterable remote reserve-keyword
              placeholder="请输入关键词" :remote-method="supplierRemoteMethod" :loading="Supplierloading">
              <el-option v-for="item in Supplieroptions" :key="item.Id" :label="item.SupplierName" :value="item.Id">{{item.SupplierName}}</el-option>
            </el-select>
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
              <el-select  @change="customValChange($event,item)" :disabled="item.is_readonly" :filterable="item.is_add" :allow-create="item.is_add" :multiple="item.type == '复选框'"
                :clearable="!item.is_required" v-model="form[item.mapid]" :placeholder="item.prompt_text ? item.prompt_text : '请选择'"
                style="width: 100%" v-if=" (item.type == '单选框' && item.show_way == '下拉') || (item.type == '复选框' && item.show_way == '下拉') ">
                <template v-for="it in item.optionals">
                  <el-option :label="it" :value="it" :key="it + ix"></el-option>
                </template>
              </el-select>
              <el-radio-group @change="customValChange($event,item)" :disabled="item.is_readonly"
                v-model="form[item.mapid]" v-if="item.type == '单选框' && item.show_way == '平铺'">
                <template v-for="it in item.optionals">
                  <el-radio :label="it" :key="it + ix">{{ it }}</el-radio>
                </template>
              </el-radio-group>
              <el-checkbox-group @change="customValChange($event,item)" :disabled="item.is_readonly"
                v-model="form[item.mapid]" v-if="item.type == '复选框' && item.show_way == '平铺'">
                <template v-for="it in item.optionals">
                  <el-checkbox :label="it" :key="it + ix">{{ it }}</el-checkbox>
                </template>
              </el-checkbox-group>
              <el-date-picker @blur="customValChange($event,item)" @change="customValChange($event,item)" :disabled="item.is_readonly" v-if="item.type == '时间'" v-model="form[item.mapid]"
                type="datetime" :placeholder="item.prompt_text ? item.prompt_text : '请选择'" style="width: 100%" :value-format="item.format" :format="item.format"></el-date-picker>
              <el-input @input="customValChange($event,item)" :disabled="item.is_readonly" v-if="item.type == '文本'"
                :placeholder="item.prompt_text ? item.prompt_text : '请输入'" :type="item.is_multiple ? 'textarea' : 'text'" v-model="form[item.mapid]"></el-input>
              <el-input @input="customValChange($event,item)" :disabled="item.is_readonly" v-if="item.type == '数字'"
                :placeholder="item.prompt_text ? item.prompt_text : '请输入'" type="number" v-model="form[item.mapid]" :precision="item.decimals"></el-input>
              <el-link style="line-height:30px;height:35px" :disabled="item.is_readonly" v-if="item.type == '超链接'" href="#" target="_blank">{{ item.describe_text }}</el-link>
              <!-- <image-upload @input="customValChange($event,item)" v-model="form[item.mapid]" :limit="1" v-if="item.type == '图片'"></image-upload> -->
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
              <el-select @focus="afterValSearch(form[item.mapid],item)" :clearable="true" @change="customValChange($event,item)" style="width: 100%" v-model="form[item.mapid]" filterable remote reserve-keyword
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
</template>

<script>
import { orgField } from "@/api/factory/customFields";
import {
  factoryProductNumber,
  addProductSave,
  factoryProductInfo,
  factoryProductTypeInfo,
  editProductSave,
  factorySearchObject
} from "@/api/factory/product";
import {factorySupplierListGet} from '@/api/factory/supplier'
import { factoryUnitListGet } from "@/api/factory/unit";
import { routeList } from "@/api/mes/processRoute";
import {
  productList
} from "@/api/rules/productModel";
import { checkPermi } from "@/utils/permission"; 
import dayjs from 'dayjs';
export default {
  name: "AdminUiProductAdd",
  props:{
    productTypeList:{
      type:Array,
      default:()=>{
        return []
      }
    }
  },
  data() {
    return {
      formKey:1,
      RouteoptionsList:[],
      Routeoptions:[],//供应商列表
      Routeloading:false,//供应商加载
      SupplieroptionsList:[],
      Supplieroptions:[],//供应商列表
      Supplierloading:false,//供应商加载
      IOTProductoptionsList:[],
      IOTProductoptions:[],//物联网产品列表
      IOTProductloading:false,//物联网产品加载
      Unitoptions:[],//单位列表
      dialogName: "baseinfo",
      labelList: [{label: "半成品",value: "U"},{label: "成品",value: "F"}],
      typeList: [],
      dialogVisible: false,
      form: {
        ProductName: "",
        TypeId: "",
        SkuNumber: "", //产品编码
        IOTProductId: "", //物联网编码
        ProductLabel: "", //产品标签
        Prop: "", //产品属性
        PhotoUrl: "", //产品图片
        Unit: "", //单位
        Specs: "", //产品规格
        Price: 0, //成本单价
        Total: 1, //总计量
        SalesPrice: 0, //销售单价
        Route: "", //工艺路线，
        Supplier: "", //供应商，
        Remark: "", //备注说明
      },
      rules: {
        SkuNumber: [{ required: true, trigger: "blur", message: "产品编码不能为空" },],
        ProductName: [{ required: true, trigger: "blur", message: "产品名称不能为空" },],
        ProductLabel: [{ required: true, trigger: "change", message: "产品标签不能为空" },],
        ProductFrom: [{ required: true, trigger: "change", message: "生产来源不能为空" },],
        Unit: [{ required: true, trigger: "change", message: "包装单位不能为空" },],
      },
      filedTableList: [], //产品自定义列表
      supplierQueryParams:{
        key:'',
        pageNum: 1,
        pageSize: 2,
      },
      routeQueryParams:{
        key:'',
        pageNum: 1,
        pageSize: 10,
      },
      IOTProductQueryParams:{
        pageNum: 1,
        pageSize: 10,
        Name: null,
      },
      propertiesData:[],//产品分组属性
      associationObject:{},//所有关联对象对应的下拉的参数列表
      isWatch:false
    };
  },
  watch: {
    $route(to, from) {
      this.isWatch=true
      this.$nextTick(async ()=>{
        await this.getUnitList()//单位
      })
    }
  },
  computed:{

  },
  async mounted() {
    if(!this.isWatch){
      await this.getSupplierList()//供应商
      if(this.isCheckPermi(['/IoTService/IotProduct/ListPage'])){
        await this.getIOTProductList()//物联网产品
      }
      await this.getUnitList()//单位
    }
    
  },

  methods: {
    isCheckPermi(val) {
      return checkPermi(val)
    },
    routeSearch(){
      let keyVal=this.form.RouteName
        // console.log(keyVal[1],'keyVal[1]');
        this.routeRemoteMethod(keyVal)
    },
    async routeRemoteMethod(query){//工艺路线动态加载
      if (query !== ""&&query) {
        console.log(query,'query');
        this.Routeloading = true;
        setTimeout(async () => {
          this.routeQueryParams.key=query
          await this.getRouteList()
          this.Routeloading = false;
          this.Routeoptions = this.RouteoptionsList.filter((item) => {
            return item.RouteName.toLowerCase().indexOf(query.toLowerCase()) > -1;
          });
          console.log(this.Routeoptions,'this.Routeoptions',this.RouteoptionsList);
          this.$forceUpdate()
        }, 200);
      } else {
        delete this.routeQueryParams.key
        await this.getRouteList()
        this.Routeoptions = JSON.parse(JSON.stringify(this.RouteoptionsList));
        
        this.$forceUpdate()
      }
    },
    async getRouteList(){//获取供应商列表接口
      let res=await routeList(this.routeQueryParams)
      //  console.log("查询到供应商",res);
      if(!this.routeQueryParams.key){
        this.Routeoptions=JSON.parse(JSON.stringify(res.data.List))
          
      }
      this.RouteoptionsList = JSON.parse(JSON.stringify(res.data.List));
    },
    supplierSearch(){
      let keyVal=this.form.SupplierName
        // console.log(keyVal[1],'keyVal[1]');
        this.supplierRemoteMethod(keyVal)
    },
    iOTProductSearch(){
      let keyVal=this.form.IOTProductName
      this.IOTProductRemoteMethod(keyVal)
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
    async getUnitList() {//获取单位列表
      let res=await factoryUnitListGet()
      // console.log("查询到单位", res);
      this.Unitoptions = res.data;
    },
    async supplierRemoteMethod(query){//供应商动态加载
      if (query !== ""&&query) {
        console.log(query,'query');
        this.Supplierloading = true;
        setTimeout(async () => {
          this.supplierQueryParams.key=query
          await this.getSupplierList()
          this.Supplierloading = false;
          this.Supplieroptions = this.SupplieroptionsList.filter((item) => {
            return item.SupplierName.toLowerCase().indexOf(query.toLowerCase()) > -1;
          });
          console.log(this.Supplieroptions,'this.Supplieroptions',this.SupplieroptionsList);
          this.$forceUpdate()
        }, 200);
      } else {
        delete this.supplierQueryParams.key
        await this.getSupplierList()
        this.Supplieroptions = JSON.parse(JSON.stringify(this.SupplieroptionsList));
        
        this.$forceUpdate()
      }
    },
   async getSupplierList(){//获取供应商列表接口
     let res=await factorySupplierListGet(this.supplierQueryParams)
    //  console.log("查询到供应商",res);
     if(!this.supplierQueryParams.key){
      this.Supplieroptions=JSON.parse(JSON.stringify(res.data.List))
        
    }
     this.SupplieroptionsList = JSON.parse(JSON.stringify(res.data.List));
    },
    async IOTProductRemoteMethod(query){//物联网产品动态加载
      
      if (query !== ""&&query !== null&&query) {
        // console.log(query,'query物联产品');
        this.IOTProductloading = true;
        setTimeout(async () => {
          this.IOTProductQueryParams.Name=query
          await this.getIOTProductList()
          this.IOTProductloading = false;
          this.IOTProductoptions = this.IOTProductoptionsList.filter((item) => {
            return item.Name.toLowerCase().indexOf(query.toLowerCase()) > -1;
          });
        }, 200);
      } else {
        delete this.IOTProductQueryParams.Name
        await this.getIOTProductList()
        this.IOTProductoptions = JSON.parse(JSON.stringify(this.IOTProductoptionsList));
      }
    },
   async getIOTProductList(){//获取物联网产品列表接口
     let res=await productList(this.IOTProductQueryParams)
    //  console.log("查询到物联网产品",res);
     if(!this.IOTProductQueryParams.Name){
        this.IOTProductoptions=res.data.List
      }
      this.IOTProductoptionsList = res.data.List;
     
    },
    productTypeChange(val){
      if(val){//当属于手动切换产品分组时，属性要清空
        this.form.Prop=''
      }
      
      if(this.form.TypeId){//产品分组属性
        factoryProductTypeInfo({id:this.form.TypeId}).then(res=>{
          // console.log('产品分组属性',res);
          if (res.data.PropList) {
            this.propertiesData = res.data.PropList.split(',');
          }
        })
      }else{
        this.propertiesData=[]
      }
      
    },
    setFormItemHide(item) {//判断字段是否隐藏
      if (item.conditions && item.conditions.length > 0) {
        let result = false;
        let conditionsResArr = [];
        for (let i = 0; i < item.conditions.length; i++) {
          let row = item.conditions[i];
          conditionsResArr[i] = this.returnCompareResult(
            row.field,
            row.compare,
            row.val,
            row.valtype
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
    returnCompareResult(field, compare, val, type) {//隐藏规则设置方法
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
            result =val.timeValue &&dayjs(this.form[field]).valueOf()>dayjs(val.timeValue).valueOf();
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
    customValChange(val,fidItem) {//数据发生变化后刷新，并验证表单
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
    async openDialog(id) {//添加弹窗打开
      try {
        this.propertiesData=[]
        // this.associationObject={}
        
        await this.getProductCustomFiled();
        if (id) {
          let res = await factoryProductInfo({ id: id });
          console.log("productinfo,产品详情", res,);
          let productinfo = res.data;
          this.form = {
            Id: productinfo.Id,
            ProductName: productinfo.ProductName,
            ProductFrom:productinfo.ProductFrom,
            TypeId: productinfo.TypeId,
            SkuNumber: productinfo.SkuNumber, //产品编码
            IOTProductId: productinfo.IOTProductId, //物联网编码
            IOTProductName:productinfo.IOTProductName,
            ProductLabel: productinfo.ProductLabel, //产品标签
            Prop: productinfo.Prop, //产品属性
            PhotoUrl: productinfo.PhotoUrl, //产品图片
            Unit: productinfo.Unit, //单位
            MinUnit:productinfo.MinUnit,
            Specs: productinfo.Specs, //产品规格
            Price: productinfo.Price, //成本单价
            Total: productinfo.Total, //总计量
            SalesPrice: productinfo.SalesPrice, //销售单价
            Route: productinfo.Route, //工艺路线，
            RouteName:productinfo.RouteName,
            Supplier: productinfo.Supplier, //供应商，
            SupplierName:productinfo.SupplierName,
            Remark: productinfo.Remark, //备注说明
          };
          this.resetForm("form");
          this.productTypeChange()
          this.setCustomDefaultValue(productinfo);
          this.supplierSearch()//供应商
          this.routeSearch()//工艺路线
          this.iOTProductSearch()
        } else {
          this.form={
            ProductName: "",
            ProductFrom:'',
            TypeId: "",
            SkuNumber: "", //产品编码
            IOTProductId: "", //物联网编码
            IOTProductName: '',
            ProductLabel: "", //产品标签
            Prop: "", //产品属性
            PhotoUrl: "", //产品图片
            Unit: "", //单位
            Specs: "", //产品规格
            Price: 0, //成本单价
            Total: 1, //总计量
            SalesPrice: 0, //销售单价
            Route: "", //工艺路线，
            RouteName:"",
            Supplier: "", //供应商，
            SupplierName:"",
            Remark: "", //备注说明
          }
          this.resetForm("form");
          this.setCustomDefaultValue();
          // console.log("表单初始化",this.form);
          await this.getFactoryProductNumber();
        }
        let form=JSON.parse(JSON.stringify(this.form))
        this.form=JSON.parse(JSON.stringify(form))
        this.dialogVisible = true;
        
      } catch (error) {
        console.log("出错",error);
      }
    },
    setCustomDefaultValue(afterForm) {
      //设置自定义的变量初始化
     let res =this.filedTableList.map((rw) => {
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
              if(rw.type=='关联对象'){
                this.afterValSearch(afterForm[rw.mapid],rw)//用于处理编辑时关联对象的回显
              }
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
        return rw
      });
      // console.log("this.rules", this.rules,res);
    },
    async getFactoryProductNumber() {//生成产品编码
      let res = await factoryProductNumber();
      this.form.SkuNumber = res.data;
      // console.log("产品编码",res);
    },
    async getProductCustomFiled() {
      //获取自定义的字段
      this.filedTableList = [];
      let orgId = this.$store.state.user.orgId;
      let res = await orgField({ orgId: orgId, field: "产品" });
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
      this.formKey++
      this.$nextTick(()=>{
        this.$refs["form"].validate((valid,validateResult) => {
          // console.log("检验",valid);
          if (valid) {
            let submitForm = JSON.parse(JSON.stringify(this.form));
            delete submitForm.IOTProductName
            delete submitForm.RouteName
            delete submitForm.SupplierName
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
            // console.log("提交的数据", submitForm);
            if (submitForm.Id) {
              editProductSave(submitForm).then((res) => {
                // console.log("修改执行结果", res);
                this.$modal.msgSuccess("修改成功");
                this.dialogVisible = false;
                this.$emit("reloadData");
              });
            } else {
              addProductSave(submitForm).then((res) => {
                // console.log("添加执行结果", res);
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
      })
      
    },
    handleClose() {//关闭弹窗的方法
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