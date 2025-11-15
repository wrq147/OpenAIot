<template>
  <div>
    <el-form label-position="top" label-width="90px">
      <el-form-item label="请求地址">
        <el-input placeholder="请输入URL地址" size="medium" v-model="config.url">
          <el-select v-model="config.method" style="width: 85px;" slot="prepend" placeholder="URL">
            <el-option label="GET" value="GET"></el-option>
            <el-option label="POST" value="POST"></el-option>
            <el-option label="PUT" value="PUT"></el-option>
            <el-option label="DELETE" value="DELETE"></el-option>
          </el-select>
        </el-input>
      </el-form-item>
      <el-form-item label="Header请求头">
        <div slot="label">
          <span style="margin-right: 10px">Header请求头</span>
          <el-button type="text" @click="addItem(0)">+ 添加</el-button>
        </div>
        <div v-for="(header, hdidx) in config.headers" :key="header.name">
          <label style="width: 80px;display: inline-block;text-align: center;color: #999;">{{ header.name }}</label>
          <el-radio-group size="small" style="margin: 0 5px;" v-model="header.isField">
            <el-radio-button :label="true">动态</el-radio-button>
            <el-radio-button :label="false">固定</el-radio-button>
          </el-radio-group>
          <el-input v-if="header.isField" size="small" @focus="showDlg(header)" placeholder="请点击选择动态字段"
            v-model="header.valueTitle" :readonly="true" style="width: 180px;cursor: pointer;" />
          <el-input v-else placeholder="请设置字段值" size="small" v-model="header.value" style="width: 180px;" />
          <el-button type="danger" icon="el-icon-delete" @click="delItem(0, hdidx)" size="mini"
            style="margin-left: 15px;" circle></el-button>
        </div>
      </el-form-item>
      <el-form-item label="Header请求参数">
        <div slot="label">
          <span style="margin-right: 10px">Body请求参数</span>
          <el-button style="margin-right: 20px" type="text" @click="addItem(1)">+ 添加</el-button>
          <span>参数类型 -</span>
          <el-radio-group size="mini" style="margin: 0 5px;" v-model="config.contentType" @input="chgContentType">
            <el-radio-button label="JSON">json</el-radio-button>
            <el-radio-button label="RAW">json字符串</el-radio-button>
            <el-radio-button label="FORM">form</el-radio-button>
          </el-radio-group>
        </div>
        <div v-if="config.contentType == 'RAW'">
          <el-input type="textarea" :rows="3" placeholder="请输入内容" v-model="config.rawString">
          </el-input>
        </div>
        <div v-else v-for="(param, xidx) in config.xparams" :key="param.name"
          style="display: flex;flex-direction: row;align-items: center;">
          <el-tooltip effect="dark" :content="param.name" placement="top">
            <label style="width: 80px;text-align: center;color: #999;text-overflow: ellipsis;overflow: hidden;">{{
              param.name }}</label>
          </el-tooltip>
          <el-radio-group size="small" style="margin: 0 5px;" v-model="param.isField">
            <el-radio-button :label="true">动态</el-radio-button>
            <el-radio-button :label="false">固定</el-radio-button>
          </el-radio-group>
          <el-input v-if="param.isField" size="small" @focus="showDlg(param)" placeholder="请点击选择动态字段"
            v-model="param.valueTitle" :readonly="true" style="width: 180px;cursor: pointer;" />
          <el-input v-else placeholder="请设置字段值" size="small" v-model="param.value" style="width: 180px;" />
          <el-button type="danger" icon="el-icon-delete" @click="delItem(1, xidx)" size="mini"
            style="margin-left: 15px;" circle></el-button>
        </div>
      </el-form-item>
      <el-form-item label="成功校验字符">
        <el-input v-model="config.okTxt" placeholder="请输入调用成功包含的字符"></el-input>
      </el-form-item>
    </el-form>


    <el-dialog title="请选择动态字段" append-to-body :close-on-click-modal="false" :visible.sync="sourceOpen" width="670px">
      <div v-if="paramItem != null">
        <div style="display: flex;align-items: center;">
          <span style="width: 80px;color:#999;font-size: 14px;">过滤设备：</span>
          <div style="width: 0px;flex:1;">
            <el-select :clearable="true" @clear="devChange('')" size="small" style="width:300px" v-model="filterId"
              filterable remote reserve-keyword placeholder="请输入搜索关键词(未过滤则为触发设备)" :remote-method="remoteMethod"
              :loading="searchloading" @change="devChange">
              <el-option v-for="item in options" :key="item.Id" :label="item.Name" :value="item.Id">
              </el-option>
            </el-select>
          </div>
        </div>
        <div style="display: flex;justify-content: space-between;max-height: 400px;padding-top:30px;overflow-y: auto;">
          <span style="width: 80px;color:#999;font-size: 14px;">可选字段：</span>
          <div style="width: 0px;flex:1;">
            <el-radio-group v-model="paramItem.value" style="margin-top: 0;padding-top: 0;">
              <el-radio :label="paramItem.code" v-for="paramItem in paramValueList" :key="paramItem.code"
                @change="paramChange(paramItem)" style="margin-bottom: 15px;">{{ paramItem.title }}</el-radio>
            </el-radio-group>
          </div>
        </div>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="closeDlg">取 消</el-button>
        <el-button type="primary" @click="confirmDlg">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { DeviceList } from "@/api/rules/device";
import { productInfo } from "@/api/rules/productModel";
export default {
  name: "TriggerNodeConfig",
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      }
    }
  },
  computed: {
    paramValueList() {
      //构造参数值列表
      const paramItems = [];
      let dtuItems = this.options.filter(x => x.Id == this.filterId);
      if (this.filterDtuAttr.length > 0 && dtuItems.length > 0) {
        this.filterDtuAttr.forEach(item => {
          let newitem = JSON.parse(JSON.stringify(item));
          newitem.name = item.name;
          newitem.gname = dtuItems[0].Name;
          newitem.code = "$devprop." + this.filterId + "." + item.code;
          this.filterItem(newitem, paramItems);
        });
      }
      else {
        this.$store.state.rulesFlowable.rulesProductAttr.forEach(item =>
          this.filterItem(item, paramItems)
        );
      }


      //其它参数
      paramItems.push({
        code: "$now",
        title: "执行时间"
      });

      let enumArr = [];
      this.$store.state.rulesFlowable.rulesProductAttr.forEach(item => {
        enumArr.push({ "key": item.name, "value": item.code });
      });

      if (this.$store.state.rulesFlowable.rulesDesign.TriggerWay == 0) {
        this.compareItems["$prop"] = enumArr;
        paramItems.push({
          code: "$prop",
          title: "触发属性"
        });

        paramItems.push({
          code: "$dtuId",
          title: "设备通讯编码"
        });

        paramItems.push({
          code: "$devnumber",
          title: "设备第三方编码"
        });
      }
      else {
        //http与定时器触发添加输入参数
        let httpss = this.$store.state.rulesFlowable.rulesDesign.HttpParams;
        if (httpss != null) {
          let iptparams = JSON.parse(JSON.stringify(httpss));
          iptparams.forEach(item => {
            item.code = "$param." + item.code;
            this.filterItem(item, paramItems)
          }
          );
        }

      }

      return paramItems;
    }
  },
  data() {
    return {
      compareItems: {},
      sourceOpen: false,
      rawItem: null,
      paramItem: null,
      filterId: "",
      filterDtuAttr: [],
      options: [],
      searchloading: false,
      filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
      },
    };
  },
  mounted() {
  },
  methods: {
    chgContentType(label) {
      if (label == "RAW") {
        if (this.config.rawString == null||this.config.rawString=="") {
          this.config.rawString = "";
          if(this.config.xparams!=null&&this.config.xparams.length>0){
            let tjbo={};
            this.config.xparams.forEach(x=>{
              tjbo[x.name]=x.value;
            })
            this.config.rawString = JSON.stringify(tjbo);
          }
        }
      }
    },
    paramChange(item) {
      this.paramItem.value = item.code;
      this.paramItem.valueTitle = item.title;
    },
    devChange(devId) {
      if (devId == "") {
        this.filterDtuAttr = [];
        this.filterId = "";
      }
      else {
        let dtuItems = this.options.filter(x => x.Id == devId);
        if (dtuItems.length > 0) {
          productInfo({ id: dtuItems[0].ProductId }).then(rsp => {
            if (rsp.code == 0) {
              let jsonLis = JSON.parse(rsp.data.ModelTSL);
              if (jsonLis.properties) {
                this.$set(this, "filterDtuAttr", jsonLis.properties);
              }
            }
          });

        }
      }

    },
    remoteMethod(query) {
      this.searchloading = true;
      this.filterParams.Key = query;
      DeviceList(this.filterParams)
        .then(rsp => {
          this.searchloading = false;
          this.options = rsp.data.List;
        });

    },
    showDlg(param) {
      this.sourceOpen = true;
      this.rawItem = param;
      this.paramItem = JSON.parse(JSON.stringify(param));
    },
    closeDlg() {
      this.sourceOpen = false;
      this.paramItem = null;
    },
    confirmDlg() {
      this.sourceOpen = false;
      this.rawItem.value = this.paramItem.value;
      this.rawItem.valueTitle = this.paramItem.valueTitle;
      this.paramItem = null;
      this.rawItem = null;
    },
    addItem(itemtype) {
      this.$prompt("请输入新的参数名", "添加新参数", {
        confirmButtonText: "提交",
        cancelButtonText: "取消",
        inputPattern: /^[A-Za-z0-9\\-\\_]{1,30}$/,
        inputErrorMessage: "参数名不能为空且长度小于30",
        inputPlaceholder: "请输入参数名",
        closeOnClickModal: false
      }).then(({ value }) => {
        if (itemtype == 0) {
          this.config.headers.push({ name: value, value: "", isField: true, valueTitle: "" });
        }
        else if (itemtype == 1) {
          this.config.xparams.push({ name: value, value: "", isField: true, valueTitle: "" });
        }

      });
    },
    delItem(xtype, index) {
      if (xtype == 0) {
        this.config.headers.splice(index, 1);
      }
      else if (xtype == 1) {
        this.config.xparams.splice(index, 1);
      }
    },
    filterItem(item, list) {
      if (this.compareItems == null) {
        this.compareItems = {};
      }
      let opobj = item.option !== undefined ? item.option : item;
      let curtype = opobj.type;
      if (
        item.code &&
        (curtype == "int" ||
          curtype == "float" ||
          curtype == "string" ||
          curtype == "date" ||
          curtype == "boolean" ||
          curtype == "enum")
      ) {
        if (curtype == "boolean" || curtype == "enum") {
          //如果是布尔型和枚举型，需携带比较参数
          if (curtype == "boolean") {
            let boolArr = [];
            boolArr.push({ "key": opobj.trueText, "value": "true" });
            boolArr.push({ "key": opobj.falseText, "value": "false" });
            this.compareItems[item.code] = boolArr;
            list.push({
              title: item.gname == null ? item.name : (item.gname + "-" + item.name),
              code: item.code,
            });
          }
          if (curtype == "enum") {
            let enumArr = [];
            for (let key in opobj.elements) {
              if (item.option !== undefined) {
                enumArr.push({ "key": opobj.elements[key], "value": opobj.elements[key] });
              }
              else {
                enumArr.push({ "key": opobj.elements[key], "value": key });
              }
            }
            this.compareItems[item.code] = enumArr;
            list.push({
              title: item.gname == null ? item.name : (item.gname + "-" + item.name),
              code: item.code
            });
          }
        } else {
          list.push({
            title: item.gname == null ? item.name : (item.gname + "-" + item.name),
            code: item.code
          });
        }
      }
    },
  }
};
</script>

<style lang="less" scoped>
.code-container {
  // overflow-y: auto !important;
  border-top: 1px solid #dddddd;
  min-height: 380px;
  font-size: 16px;
  border: 1px solid #dddddd;
  padding-top: 15px;
}

.item-desc {
  color: #939494;
}
</style>
