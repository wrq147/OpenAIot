<template>
  <div>
    <el-form label-width="80px">
      <el-form-item label="调度设备">
        <el-select :clearable="true" :multiple-limit="50" multiple style="width:300px" v-model="config.DeviceList"
          @change="chgDevice" @clear="reloadDevice" filterable remote reserve-keyword placeholder="请输入搜索关键词(未过滤则为触发设备)"
          :remote-method="remoteMethod" :loading="searchloading">
          <el-option v-for="item in deviceOptions" :key="item.Id" :label="item.Name" :value="item.Id">
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="调度数量">
        <el-input-number v-model="config.ScheAmount" :step="1" :min="1"
          :max="config.DeviceList.length"></el-input-number>
      </el-form-item>
    </el-form>
    <div style="border:solid 1px #dadada;">
      <div class="sch-title">设备调度配置</div>
      <div class="sch-row">
        <div style="width: 50px;" class="cc-col">序号</div>
        <div style="flex:1;width:0px;" class="cc-col">设备名</div>
        <div style="width:120px;" class="cc-col">操作</div>
      </div>
      <div class="sch-row" v-for="(dv, idx) in config.DeviceSets" :key="dv.id">
        <div style="width: 50px;" class="cc-col">{{ idx+1 }}</div>
        <div style="flex:1;width:0px;" class="cc-col">{{ DeviceName(dv.id) }}</div>
        <div style="width:120px;" class="cc-col">
          <el-link type="primary" @click="onEdit(dv.id, idx)">编辑</el-link>
        </div>
      </div>
    </div>

    <el-dialog :title="sceTitle" append-to-body :close-on-click-modal="false" :visible.sync="scheOpen" width="740px">
      <el-form label-width="80px" v-if="editSet != null">
        <el-form-item label="优先级">
          <el-select placeholder="请选择优先级参数" :value="editSet.level" style="width:220px;" @change="chgLevel">
            <el-option label="调度序号(默认)" value=""></el-option>
            <el-option :label="ppite.name" :value="ppite.code" v-for="ppite in IntParamList" :key="ppite.code"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="调度条件">
          <div>
            <el-button type="text" @click="onAddCondition">+ 添加</el-button>
          </div>
          <div class="dlg-param-bg" v-if="editSet.conditions.length > 0">
            <div class="paramrow" v-for="(ccitem, cidx) in editSet.conditions" :key="cidx">
              <span style="margin-right: 10px;">条件{{ cidx + 1 }}</span>
              <el-select v-model="ccitem.enablecode" placeholder="属性或参数" style="width: 160px;margin-right: 10px;"
                @change="condiChange($event, ccitem)">
                <el-option :label="'属性：' + opx.name" :value="opx.code" v-for="opx in propList"
                  :key="'pp' + opx.code"></el-option>
                <el-option :label="'标签' + opx.name" :value="'#' + opx.code" v-for="opx in tagList"
                  :key="'pp' + opx.code"></el-option>
                <el-option :label="'参数：' + opx.name" :value="'$' + opx.code" v-for="opx in ParamList"
                  :key="'cc' + opx.code"></el-option>
              </el-select>
              <el-select placeholder="判断符" v-model="ccitem.compare" style="width: 100px;margin-right: 10px;">
                <el-option label="等于" value="=" v-if="ccitem.valtype != 'Date'"></el-option>
                <el-option label="不等于" value="!=" v-if="ccitem.valtype != 'Date'"></el-option>
                <el-option label="大于" value=">"
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
                <el-option label="小于" value="<"
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
                <el-option label="大于等于" value=">="
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
                <el-option label="小于等于" value="<="
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
              </el-select>

              <el-select v-if="ccitem.valuefrom == 1" v-model="ccitem.val" placeholder="请选择参数" style="width: 200px">
                <template v-for="opx in ParamList">
                  <el-option :label="opx.name" :value="opx.code"
                    v-if="(ccitem.valtype === 'String' && opx.type == 'string') || (ccitem.valtype === 'Date' && opx.type == 'date') || (ccitem.valtype === 'Enum' && opx.type == 'enum') || (ccitem.valtype === 'Bool' && opx.type == 'boolean') || (ccitem.valtype === 'Long' && opx.type == 'int') || (ccitem.valtype === 'Double' && opx.type == 'float')"
                    :key="'cc' + opx.code"></el-option>
                </template>
              </el-select>

              <template v-else>
                <el-input v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long'" v-model="ccitem.val"
                  style="width: 200px" type="number" placeholder="输入比较值" />
                <el-input v-else-if="ccitem.valtype == 'String'" v-model="ccitem.val" type="text" placeholder="输入比较值"
                  style="width: 200px" />
                <el-select v-else-if="ccitem.valtype == 'Enum'" placeholder="请选择比较值" v-model="ccitem.val"
                  style="width: 200px;">
                  <el-option v-for="(option, oi) in enumArr" :key="oi" :label="option.key"
                    :value="option.value"></el-option>
                </el-select>
                <el-select v-else-if="ccitem.valtype == 'Bool'" placeholder="请选择布尔值" v-model="ccitem.val"
                  style="width: 200px;">
                  <el-option label="真" value="True"></el-option>
                  <el-option label="假" value="False"></el-option>
                </el-select>
                <el-date-picker v-else-if="ccitem.valtype == 'Date'" style="width: 200px;" v-model="ccitem.val"
                  value-format="timestamp" type="datetime" placeholder="请选择日期和时间">
                </el-date-picker>
              </template>
              <span v-if="ccitem.valuefrom == 1" class="el-swit" @click="switchVal(ccitem, 0)"
                style="margin-left:20px;margin-right: 10px;">参</span>
              <span v-else class="el-swit" @click="switchVal(ccitem, 1)"
                style="margin-left:20px;margin-right: 10px;">值</span>
              <i class="el-icon-delete" style="color: #ff0000;cursor: pointer;" @click="onDelCondition(cidx)"></i>
            </div>
          </div>
        </el-form-item>
        <el-form-item label="条件组合" v-if="editSet.conditions.length > 1">
          <div class="dlg-gg-row">
            <div style="margin-bottom: 10px;" v-for="(ggitem, ggidx) in LoopCount" :key="ggidx">
              <span style="margin-right: 10px;">条件{{ ggidx + 1 }}</span>
              <el-select style="width: 80px;margin-right: 10px;" v-model="editSet.groups[ggidx]"
                @change="chgGroup($event, ggidx)">
                <el-option label="且" value="&"></el-option>
                <el-option label="或" value="|"></el-option>
              </el-select>
              <span v-if="ggidx == (LoopCount.length - 1)" style="margin-right: 10px;">条件{{ editSet.conditions.length
                }}</span>
            </div>
          </div>
        </el-form-item>
        <el-form-item label="执行动作">
          <div>
            <el-button type="text" @click="onAddAction">+ 添加</el-button>
          </div>
          <div class="dlg-ac-bg" v-if="editSet.actions.length > 0">
            <div v-for="(acitem, idx) in editSet.actions" :key="idx"
              style="border-bottom: dashed 1px #dadada;margin-bottom: 10px;padding-bottom: 10px;">
              <div
                style="margin-bottom: 10px;display: flex;flex-direction: row;justify-content:space-between;align-items: center;">
                <div>
                  <span style="margin-right: 15px;">执行第{{ idx + 1 }}个</span>
                  <el-radio-group v-model="acitem.codetype" @change="acitem.code = '';acitem.express = '';acitem.targettype=0;acitem.targetid='';acitem.express='';acitem.errbreak=false;">
                    <el-radio :label="0">功能</el-radio>
                    <el-radio :label="1">参数</el-radio>
                    <el-radio :label="2">标签</el-radio>
                  </el-radio-group>
                </div>

                <i class="el-icon-delete" style="color: #ff0000;margin-right: 10px;cursor: pointer;"
                  @click="onDelAction(idx)"></i>
              </div>
              <div style="display: flex;flex-direction: row;margin-bottom: 15px;">
                  <el-select v-model="acitem.targettype" placeholder="请选择目标类型" @change="targetTypeChange(acitem,$event)" style="margin-right: 10px;">
                    <el-option label="当前设备" :value="0"></el-option>
                    <el-option label="选择设备" :value="1" v-if="acitem.codetype!=1"></el-option>
                  </el-select>
                  <el-select v-if="acitem.targettype==1" v-model="acitem.targetid" clearable filterable remote reserve-keyword :remote-method="(query)=>{itemRemoteMethod(acitem,query)}"
                    @clear="itemRemoteMethod(acitem,'')" :loading="acitem.loading" placeholder="请选择目标设备" @change="itemDevChange(acitem)">
                    <el-option v-for="item in acitem.deviceLists" :key="item.Id" :label="item.Name" :value="item.Id"></el-option>
                  </el-select>
              </div>
              <div v-if="acitem.codetype == 0" class="acrow">
                <el-select style="width:260px;" placeholder="选择功能" v-model="acitem.code"
                  @change="onChangeFun($event, acitem)">
                  <el-option :label="ffitt.name" :value="ffitt.code" v-for="ffitt in acitem.funList"
                    :key="ffitt.code"></el-option>
                </el-select>
                <div style="margin-left: 15px;cursor: pointer;color:#409EFF;text-decoration: underline;"
                  @click="onEditExpress(acitem)">参数<i class="el-icon-edit"></i>
                </div>
                <div style="margin-left: 15px;">
                  <span style="color:#999;margin-right: 5px;">（是否错误中断</span>
                  <el-switch v-model="acitem.errbreak"></el-switch>
                  <span style="color:#999;margin-left: 5px;">）</span>
                </div>
              </div>
              <div v-else-if="acitem.codetype == 1" class="acrow">
                <el-select placeholder="选择参数" v-model="acitem.code" style="width:160px;"
                  @change="acitem.express = 'data'">
                  <el-option :label="ppite.name" :value="ppite.code" v-for="ppite in ParamList"
                    :key="ppite.code"></el-option>
                </el-select>
                <span style="margin:0 10px;">=</span>
                <el-input placeholder="请输入表达式" v-model="acitem.express"
                  style="width:270px;margin-right: 10px;"></el-input>
                <el-link icon="el-icon-edit" @click="popExpressDlg(acitem)"></el-link>
              </div>
              <div v-else-if="acitem.codetype == 2" class="acrow">
                <el-select placeholder="选择标签" v-model="acitem.code" style="width:160px;"
                  @change="acitem.express = 'data'">
                  <el-option :label="taggite.name" :value="taggite.code" v-for="taggite in acitem.tagList"
                    :key="taggite.code"></el-option>
                </el-select>
                <span style="margin:0 10px;">=</span>
                <el-input placeholder="请输入表达式" v-model="acitem.express"
                  style="width:270px;margin-right: 10px;"></el-input>
                <el-link icon="el-icon-edit" @click="popExpressDlg(acitem)"></el-link>
              </div>

            </div>
          </div>
          <div style="color:#999;line-height: 22px;"></div>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="scheOpen = false">取 消</el-button>
        <el-button type="primary" @click="onOk">确 定</el-button>
      </div>
    </el-dialog>
    <el-dialog title="编辑功能参数" append-to-body :close-on-click-modal="false" :visible.sync="funOpen" width="650px">
      <div v-if="funParam != null">
        <vue-json-editor v-model="funParam" :showBtns="false" :mode="'code'" lang="zh" />
      </div>

      <div slot="footer" class="dialog-footer">
        <el-button @click="funOpen = false; funParam = null; curAction = null;">取 消</el-button>
        <el-button type="primary" @click="confirmFun">确 定</el-button>
      </div>
    </el-dialog>

    <el-dialog title="编辑表达式" append-to-body :close-on-click-modal="false" :visible.sync="expressOpen" width="670px">
      <div v-if="editAcItem != null">
        <ExpressEditor :content="editAcItem.express" ref="expEd"></ExpressEditor>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="expressOpen = false;">取 消</el-button>
        <el-button type="primary" @click="confirmExpress">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import vueJsonEditor from "vue-json-editor";
import { DeviceList } from "@/api/rules/device";
import { productInfo } from "@/api/rules/productModel";
import ExpressEditor from "../../ExpressEditor.vue";

export default {
  name: "TimeSchedulerNodeConfig",
  components: {
    vueJsonEditor, ExpressEditor
  },
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      }
    }
  },
  computed: {
    setup() {
      return this.$store.state.rulesFlowable.rulesDesign;
    },
    ParamList() {
      let httpss = this.$store.state.rulesFlowable.rulesDesign.HttpParams;
      if (httpss != null) {
        return httpss;
      }
      else {
        return [];
      }
    },
    IntParamList(){
      let httpss = this.$store.state.rulesFlowable.rulesDesign.HttpParams;
      if (httpss != null) {
        return httpss.filter(x=>x.type=="int");
      }
      else {
        return [];
      }
    },
    LoopCount() {
      if (this.editSet == null) return [];
      if (this.editSet.conditions.length > 1) {
        if (this.editSet.groups == null) {
          this.editSet.groups = [];
        }

        let ll = this.editSet.conditions.length - 1;
        for (let i = 0; i < ll; i++) {
          if (i >= this.editSet.groups.length) {
            this.editSet.groups[i] = "&";
          }
        }
        if (this.editSet.groups.length > ll) {
          this.editSet.groups.splice(ll);
        }
        return this.editSet.groups;
      }
      else {
        return [];
      }
    }
  },
  data() {
    return {
      searchloading: false,
      filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 50,
      },
      deviceOptions: [],
      scheOpen: false,
      sceTitle: "",
      editDevId: "",
      editSet: null,
      propList: [],
      tagList: [],
      funList: [],
      funOpen: false,
      funParam: null,
      curAction: null,
      enumArr: [],
      expressOpen: false,
      editAcItem: null,
    };
  },
  mounted() {
    this.filterParams.Ids = this.config.DeviceList;
    this.remoteMethod("");
  },
  methods: {
    async initProduct() {
      let tmplist = this.deviceOptions.filter(x => x.Id == this.editDevId);
      if (tmplist.length > 0) {
        let rsp = await productInfo({ id: tmplist[0].ProductId });
        let jsonLis = JSON.parse(rsp.data.ModelTSL);
        this.propList = jsonLis.properties.filter(x=>x.option.type!='geo');
        this.tagList = jsonLis.tags.filter(x=>x.option.type!='geo');
        this.funList = jsonLis.functions;
      }
    },
    reloadDevice() {
      this.filterParams.Ids = [];
      this.remoteMethod("");
    },
    chgDevice() {
      this.config.DeviceList.forEach(element => {
        if (!this.config.DeviceSets.some(x => x.id == element)) {
          this.config.DeviceSets.push({ id: element, conditions: [], actions: [] });
        }
      });

      for (let i = this.config.DeviceSets.length - 1; i >= 0; i--) {
        let ds = this.config.DeviceSets[i];
        if (this.config.DeviceList.indexOf(ds.id) < 0) {
          this.config.DeviceSets.splice(i, 1);
        }
      }

    },
    remoteMethod(query) {
      this.searchloading = true;
      this.filterParams.Key = query;
      DeviceList(this.filterParams)
        .then(rsp => {
          this.searchloading = false;
          this.deviceOptions = rsp.data.List;
        });

    },
    DeviceName(did) {
      let tmplist = this.deviceOptions.filter(x => x.Id == did);
      if (tmplist.length > 0) {
        return tmplist[0].Name;
      }
      else {
        return did;
      }
    },
    onAddCondition() {
      this.editSet.conditions.push({ enabletype: 1, enablecode: "", valtype: "String", compare: "=", val: "" });
    },
    onAddAction() {
      this.editSet.actions.push({targettype:1,targetid:"", codetype: 0, code: "", express: "",errbreak:false,tagList:[],deviceLists:[],funList:[],loading:false});
    },
    onDelCondition(idx) {
      this.$delete(this.editSet.conditions, idx);
    },
    onDelAction(idx) {
      this.$delete(this.editSet.actions, idx);
    },
    condiChange(code, item) {
      let newttt;
      let isProp = false;
      if (code.indexOf("$") == 0) {
        let newcc = code.substring(1);
        let newlist = this.ParamList.filter(x => x.code == newcc);
        if (newlist.length > 0) {
          newttt = newlist[0];
        }
      }
      else if (code.indexOf("#") == 0) {
        let newcc = code.substring(1);
        let newlist = this.tagList.filter(x => x.code == newcc);
        if (newlist.length > 0) {
          newttt = newlist[0].option;
        }
      }
      else {
        let newlist = this.propList.filter(x => x.code == code);
        isProp = true;
        if (newlist.length > 0) {
          newttt = newlist[0].option;
        }
      }

      if (newttt.type == "string") {
        item.valtype = "String";
        item.compare = "=";
        item.val = "";
      }
      else if (newttt.type == "boolean") {
        item.valtype = "Bool";
        item.compare = "=";
        item.val = "";
      }
      else if (newttt.type == "enum") {
        item.val = "";
        item.compare = "=";
        item.valtype = "Enum";
        this.enumArr = [];
        for (let key in newttt.elements) {
          if (isProp) {
            this.enumArr.push({ "key": newttt.elements[key], "value": newttt.elements[key] });
          }
          else {
            this.enumArr.push({ "key": newttt.elements[key], "value": key });
          }
        }
      }
      else if (newttt.type == "date") {
        item.val = new Date().getTime();
        item.compare = ">";
        item.valtype = "Date";
      }
      else if (newttt.type == "int") {
        item.val = 0;
        item.valtype = "Long";
      }
      else {
        item.val = 0;
        item.valtype = "Double";
      }
    },
    onChangeFun(code, item) {
      let newlist = this.funList.filter(x => x.code == code);
      if (newlist.length > 0) {
        if (newlist[0].inputs == null) {
          item.express = "";
        }
        else {
          let tmpobj = {};
          newlist[0].inputs.forEach(xx => {
            tmpobj[xx.code] = xx.defval;
          });
          item.express = JSON.stringify(tmpobj);
        }
      }
    },
    onEditExpress(item) {
      this.curAction = item;
      if (item.express == "") {
        this.funParam = {};
      }
      else {
        this.funParam = JSON.parse(item.express);
      }
      this.funOpen = true;
    },
    confirmFun() {
      this.curAction.express = JSON.stringify(this.funParam);
      this.curAction = null;
      this.funOpen = false;
    },
    async onEdit(did, idx) {
      this.editDevId = did;
      await this.initProduct();
      this.editSet = JSON.parse(JSON.stringify(this.config.DeviceSets[idx]));
      if(this.editSet.level==null){
        this.editSet.level="";
      }
      if(this.editSet.actions!=null){
        for(let i=0;i<this.editSet.actions.length;i++){
          let x=this.editSet.actions[i];
          if(x.errbreak==null){
            x.errbreak=false;
          }
          this.$set(x,"deviceLists",[]);
          if(x.targettype==null){
            this.$set(x,"targettype",0);
            this.$set(x,"targetid","");
          }
          if(x.targettype==0){
            this.$set(x,"funList",this.funList);
            this.$set(x,"tagList",this.tagList);
          }
          else{
            this.$set(x,"funList",[]);
            this.$set(x,"tagList",[]);
            await this.itemRemoteMethod(x,"");
            await this.itemDevChange(x);
          }
          x.loading=false;
        }
      }
   
      this.sceTitle = "编辑设备" + this.DeviceName(did) + "调度配置";
      this.scheOpen = true;
      this.$forceUpdate();
    },
    onOk() {
      this.funList = [];
      this.propList = [];
      this.tagList = [];
      for (let ii = 0; ii < this.config.DeviceSets.length; ii++) {
        if (this.config.DeviceSets[ii].id == this.editDevId) {
          if(this.editSet.actions!=null){
            this.editSet.actions.forEach(x=>{
              delete x.deviceLists;
              delete x.funList;
              delete x.tagList;
              delete x.loading;
            });
          }
    
          this.config.DeviceSets[ii] = this.editSet;
          break;
        }
      }

      this.editSet = null;
      this.editDevId = "";
      this.scheOpen = false;
    },
    popExpressDlg(item) {
      this.editAcItem = item;
      this.expressOpen = true;
    },
    confirmExpress() {
      let tmpexpress = this.$refs.expEd.getFormulaStr();
      if (this.$refs.expEd.isExpress(tmpexpress).success == false) {
        this.$message.error("表达式格式错误");
        return;
      }
      this.editAcItem.express = tmpexpress;
      this.editAcItem = null;
      this.expressOpen = false;
    },
    switchVal(condition, valfrom) {
      condition.val = "";
      condition.valuefrom = valfrom;
      this.$forceUpdate();
    },
    chgGroup(val, idx) {
      this.editSet.groups[idx] = val;
      this.$forceUpdate();
    },
    chgLevel(val){
      this.$set(this.editSet,"level",val);
      this.$forceUpdate();
    },
    async getNewProductFun(tmpitem,pid) {
      let rsp=await productInfo({ id: pid });
      if (rsp.code == 0) {
          let jsonLis = JSON.parse(rsp.data.ModelTSL);
          if (jsonLis.functions) {
            this.$set(tmpitem,"funList",jsonLis.functions);
          }
          if(jsonLis.tags){
            this.$set(tmpitem,"tagList",jsonLis.tags);
          }
      }
    },
    targetTypeChange(tmpitem,newval){
      tmpitem.targetid='';
      if(newval==1){
        this.itemRemoteMethod(tmpitem,'');
      }
      else if(newval==0){
        this.$set(tmpitem,"funList",this.funList);
        this.$set(tmpitem,"tagList",this.tagList);
      }
    },
    async itemDevChange(tmpitem) {
      let devlist = tmpitem.deviceLists.filter(x => x.Id == tmpitem.targetid);
      if (devlist.length == 0) {
        tmpitem.funList=[];
        tmpitem.tagList=[];
      }
      else {
        await this.getNewProductFun(tmpitem,devlist[0].ProductId);
      }
    },
    async itemRemoteMethod(tmpitem,query){
      tmpitem.loading = true;
      if (tmpitem.targetid != "") {
        this.filterParams.Ids = [tmpitem.targetid];
      }
      else {
        this.filterParams.Ids = undefined;
      }
      this.filterParams.Key = query;
      try{
        let rsp= await DeviceList(this.filterParams);
        tmpitem.loading = false;
        this.$set(tmpitem,"deviceLists",rsp.data.List);
      }
      catch(err){
        tmpitem.loading = false;
        this.$message.error("接口异常");
      }
    }
  }
};
</script>

<style lang="scss">
.sch-title {
  display: flex;
  height: 45px;
  background-color: #f5f5f5;
  align-items: center;
  padding: 0px 15px;
  font-size: 14px;
  color: #666;
}

.sch-row {
  display: flex;
  flex-direction: row;
  font-size: 14px;
  border-top: solid 1px #dadada;

  .cc-col {
    display: flex;
    padding: 12px 0px;
    align-items: center;
    justify-content: center;
  }
}

.dlg-param-bg {
  border-radius: 5px;
  background-color: #F5F7FA;
  border: solid 1px #dadada;
  padding: 15px 10px;

  .paramrow {
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: 10px;
  }
}

.dlg-gg-row {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  align-items: center;
}

.el-swit {
  color: #409EFF;
  cursor: pointer;
}

.dlg-ac-bg {
  background-color: #F5F7FA;
  border: solid 1px #dadada;
  padding: 15px 10px;

  .acrow {
    display: flex;
    flex-direction: row;
  }
}
</style>
