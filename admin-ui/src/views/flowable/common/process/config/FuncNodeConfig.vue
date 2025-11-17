<template>
  <div>
    <el-form label-position="top" label-width="90px">
      <el-form-item label="执行的设备">
        <el-select v-model="config.FieldId" placeholder="请选择" @change="chgDev">
          <el-option v-for="item in DeviceForms" :key="item.id" :label="item.title" :value="item.id">
          </el-option>
        </el-select>
      </el-form-item>

      <el-form-item label="执行项目">
        <div>
          <div style="margin-bottom: 15px;">
            <el-button @click="onAddClick" type="primary" icon="el-icon-plus" size="small" plain>添加功能</el-button>
            <el-button style="margin-left:15px;" @click="onAddEvtClick" type="primary" icon="el-icon-plus" size="small" plain>添加事件</el-button>
          </div>
          <div style="border-bottom: solid 1px #dadada;">
            <el-row v-for="(item, idx) in config.Items" :key="idx" type="flex"
              style="border-top:solid 1px #dadada;padding:10px 0;" align="middle">
              <el-col :span="18">
                <div style="color:#333;">匹配<span style="margin:0 5px;">“{{ item.ProductName }}”</span>的{{ item.IsFunc==0?"功能":"事件" }}<span
                    style="margin:0 5px;">“{{ item.FunctionName }}”</span>并执行</div>
              </el-col>
              <el-col :span="6">
                <el-row type="flex" justify="end" align="middle">
                  <el-button @click="onFieldEditClick(idx)" type="primary" icon="el-icon-edit" size="small"
                    circle></el-button>
                  <el-button @click="onFieldDelClick(idx)" type="danger" icon="el-icon-delete" size="small"
                    circle></el-button>
                </el-row>
              </el-col>
            </el-row>
          </div>

        </div>
      </el-form-item>
    </el-form>
    <el-dialog title="执行项目" :visible.sync="dialogVisible" width="650px" :before-close="dlgclose" append-to-body>
      <el-form ref="form" :model="editform" label-width="100px">

        <el-form-item label="匹配协议" prop="ProductId">
          <el-select style="width:360px;" v-model="editform.ProductId" filterable remote reserve-keyword
            placeholder="请输入关键词" :remote-method="remoteMethod" :loading="prodloading" @change="chgProd"
            @visible-change="vschgProd">
            <el-option v-for="item in productLists" :key="item.Id" :label="item.Name" :value="item.Id">
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="执行功能" prop="FunctionId" v-if="editform.IsFunc==0">
          <el-select style="width:360px;" v-model="editform.FunctionId" placeholder="请选择功能标识" @change="choiceFuncId">
            <el-option v-for="item in funcItems" :key="item.code" :label="item.name" :value="item.code"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="执行事件" prop="FunctionId" v-else>
          <el-select style="width:360px;" v-model="editform.FunctionId" placeholder="请选择事件标识">
            <el-option v-for="item in evtItems" :key="item.code" :label="item.name" :value="item.code"></el-option>
          </el-select>
        </el-form-item>
        <div style="margin:-20px 20px 20px 20px;">
          <div>
            <el-table v-if="editform.IsFunc==0" :data="funcInputArr" class="data_table">
              <el-table-column property="name" label="参数名称" width="100"></el-table-column>
              <el-table-column property="type" label="输入类型" width="100"></el-table-column>
              <el-table-column label="值">
                <template slot-scope="scope">
                  <param-item :Item="scope.row" :disabled="scope.row.readOnly"
                    @change="chgFunParam(scope.row.code, $event)"></param-item>
                </template>
              </el-table-column>
            </el-table>
            <div v-else>
              <div style="padding-top:15px;"><el-button @click="onAddEvtItemClick" type="primary" icon="el-icon-plus" size="small" plain>添加参数</el-button></div>
              <el-table :data="evtOutputArr" class="data_table">
                <el-table-column property="name" label="参数名称" width="180"></el-table-column>
                <el-table-column label="对应表单" align="center">
                  <template slot-scope="scope">
                    <el-select v-model="scope.row.val" placeholder="请选择" @change="choiceEvtParam(scope.$index, $event)">
                      <el-option v-for="item in TextForms" :key="item.id" :label="item.title" :value="item.id">
                      </el-option>
                    </el-select>
                  </template>
                </el-table-column>
                <el-table-column label="操作" width="100" align="center">
                  <template slot-scope="scope">
                    <el-link type="danger" @click="delEvtParam(scope.$index)">删除</el-link>
                  </template>
                </el-table-column>
              </el-table>
            </div>

          </div>
        </div>
      </el-form>


      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="DlgOk">确 定</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { getItems } from "../../utlity"
import { myProductList } from "@/api/after/dev";
import { productInfo } from "@/api/rules/productModel";
import paramItem from "../../paramItem.vue";
export default {
  name: "FuncNodeConfig",
  components: { paramItem },
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  data() {
    return {
      dialogVisible: false,
      editIdx: -1,
      editform: {},
      prodloading: false,
      productLists: [],
      funcItems: [],
      evtItems:[],
      funcInputArr: [],
      evtOutputArr:[],
      queryform: {
        pageNum: 1,
        pageSize: 300,
        Name: "",
        Pids: []
      },
    };
  },
  computed: {
    DeviceForms() {
      return getItems(this.$store.state.flowable.design.formItems).filter(x => x.name == 'DevicPicker');
    },
    TextForms() {
      return getItems(this.$store.state.flowable.design.formItems).filter(x => x.name == 'TextInput'||x.name == 'TextareaInput');
    },
  },
  async mounted() {
  },
  methods: {
    chgFunParam(code, val) {
      if (this.editform.InputData == null) {
        this.editform.InputData = {};
      }
      this.editform.InputData[code] = val;
    },
    chgDev(val) {
      let itevls = this.DeviceForms.filter(x => x.id == val);
      if (itevls.length > 0) {
        this.config.FieldName = itevls[0].title;
      }
    },
    async vschgProd(val) {
      if (val == true) {
        this.queryform.Pids = [];
        await this.remoteMethod("");
        await this.choiceProduct();
      }
    },
    async chgProd() {
      this.editform.FunctionId = "";
      this.editform.FunctionName = "";
      await this.choiceProduct();
    },
    dlgclose() {
      this.dialogVisible = false;
    },
    onAddClick() {
      this.editIdx = -1;
      this.editform = { "ProductId": "", "ProductName": "", "FunctionId": "", "FunctionName": "","IsFunc":0, "InputData": {} };
      this.queryform.pageNum = 1;
      this.queryform.pageSize = 300;
      this.queryform.Name = "";
      this.queryform.Pids = [];
      this.dialogVisible = true;
      this.remoteMethod('');
    },
    onAddEvtClick() {
      this.editIdx = -1;
      this.editform = { "ProductId": "", "ProductName": "", "FunctionId": "", "FunctionName": "","IsFunc":1, "InputData": {} };
      this.queryform.pageNum = 1;
      this.queryform.pageSize = 300;
      this.queryform.Name = "";
      this.queryform.Pids = [];
      this.dialogVisible = true;
      this.remoteMethod('');
    },
    async onFieldEditClick(idx) {
      if (this.config.Items.length <= idx) {
        return;
      }
      this.editform = this.config.Items[idx];
      if(this.editform.IsFunc==null){
        this.editform.IsFunc=0;
      }
      this.editIdx = idx;
      this.queryform.pageNum = 1;
      this.queryform.pageSize = 300;
      this.queryform.Name = "";
      this.queryform.Pids = [this.editform.ProductId];
      this.dialogVisible = true;
      await this.remoteMethod('');
      await this.choiceProduct();
      if(this.editform.IsFunc==0){
        await this.choiceFuncId();
      }
      else{
        this.initItemParams();
      }
    },
    async onFieldDelClick(idx) {
      this.config.Items.splice(idx, 1);
    },
    async choiceFuncId() {
      let curitems = this.funcItems.filter(x => x.code == this.editform.FunctionId);
      let tmparr = [];
      if (curitems.length > 0) {
        tmparr = curitems[0].inputs;
      }
      if (tmparr == null) {
        tmparr = [];
      }
      tmparr.forEach(element => {
        if (this.editform.InputData.hasOwnProperty(element.code)) {
          element.defval = this.editform.InputData[element.code];
        }
      });
      this.$set(this, "funcInputArr", tmparr);
    },
    initItemParams(){
      let tmparr = [];
      for (let key in this.editform.InputData) {
        let tmpkvv=this.editform.InputData[key];
        let tmpitem=this.TextForms.filter(x=>x.id==tmpkvv);
        
        if(tmpitem.length>0){
          tmparr.push({"name": tmpitem[0].title,"val":tmpitem[0].id});
        }
      }

      this.$set(this, "evtOutputArr", tmparr);
    },
    onAddEvtItemClick(){
      this.evtOutputArr.push({"name": "","val":null})
    },
    choiceEvtParam(idx,val){
      let tmpitem=this.TextForms.filter(x=>x.id==val);
      if(tmpitem.length>0){
        this.evtOutputArr[idx].name=tmpitem[0].title;
        this.evtOutputArr[idx].val=tmpitem[0].id;
      }
    },
    delEvtParam(idx){
      this.evtOutputArr.splice(idx,1);
    },
    async choiceProduct() {
      if (this.editform.ProductId != "") {
        let rsp = await productInfo({ id: this.editform.ProductId });
        if (rsp.code == 0) {
          let jsonLis = JSON.parse(rsp.data.ModelTSL);
          if (jsonLis.functions) {
            this.$set(this, "funcItems", jsonLis.functions);
            this.$set(this, "evtItems", jsonLis.events);
          }
        }
      }

    },
    async remoteMethod(query) {
      this.prodloading = true;
      this.queryform.Name = query;
      try {
        let rsp = await myProductList(this.queryform);
        if (rsp.code == 0) {
          this.productLists = rsp.data.List;
        }
      } catch (error) {
        console.log("错误", error);
      }
      this.prodloading = false;
    },

    DlgOk() {
      this.dialogVisible = false;
      let itps = this.productLists.filter(x => x.Id == this.editform.ProductId);
      if (itps.length > 0) {
        this.editform.ProductName = itps[0].Name;
      }
      if(this.editform.IsFunc==0){
        itps = this.funcItems.filter(x => x.code == this.editform.FunctionId);
        if (itps.length > 0) {
          this.editform.FunctionName = itps[0].name;
        }
      }
      else{
        itps = this.evtItems.filter(x => x.code == this.editform.FunctionId);
        if (itps.length > 0) {
          this.editform.FunctionName = itps[0].name;
        }
      }

      if(this.editform.IsFunc==1){
        this.editform.InputData={};
        this.evtOutputArr.forEach(x=>{
          this.editform.InputData[x.name]=x.val;
        })
      }

      if (this.editIdx >= 0) {
        this.$set(this.config.Items, this.editIdx, this.editform);
      }
      else {
        this.config.Items.push(this.editform);
      }
    }
  },
};
</script>

<style scoped></style>