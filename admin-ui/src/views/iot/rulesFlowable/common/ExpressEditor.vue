<template>
  <div id="formulaPage">
    <div ref="formulaView" class="formulaView" contentEditable="false">
      <span v-for="(item, idx) in backFormulaStrArray" :key="idx" :class="{ 'cursor': item.active }">{{ item.name
      }}</span>
    </div>
    <div style="display: flex;flex-direction: row;">
      <div class="info-left">
        <div class="infomationContent">
          <div class="tab" style="width: 180px;">
            <ul>
              <li v-for="(v, i) in Num" :key="i" :class="{ 'dis': v.disabled }"
                style="background-color: #409EFF;width:40px;"
                @click="addItem($event, 'num', v.value, v.value, v.disabled)">
                {{ v.value }}
              </li>
              <li style="background-color: #409EFF;width:40px;" :class="{ 'dis': dataDisabled }"
                @click="addItem($event, 'data', '真', 'true', dataDisabled)">真</li>
              <li style="background-color: #409EFF;width:40px;" :class="{ 'dis': dataDisabled }"
                @click="addItem($event, 'data', '假', 'false', dataDisabled)">假</li>
            </ul>
          </div>
          <div class="tab" style="width:270px;">
            <ul>
              <li v-for="(v, i) in Symbol" :key="i" :class="{ 'dis': v.disabled }" style="background-color: #409EFF;"
                @click="addItem($event, 'symbol', v.name != null ? v.name : v.value, v.value, v.disabled)">
                {{ v.name != null ? v.name : v.value }}
              </li>
              <li style="background-color: #409EFF;" :class="{ 'dis': dataDisabled }"
                @click="addItem($event, 'data', '原值', 'data', dataDisabled)">原值</li>
            </ul>
          </div>
        </div>
        <div class="footerComtent">
          <div><i class="el-icon-info" style="margin-right: 5px;"></i>表达式格式示例：</div>
          <div style="line-height: 22px;color: #999;margin-top: 10px;margin-left: 15px;">
            1、data+param("TriggerDelta")<br />
            2、data+3+(data==2?1:0)<br />
            3、(data*2)/(3+4)<br />
            注：x ? y : z 就是三目条件运算符,如果x为真，就返回y，如果x为假，就返回z。
          </div>
        </div>
      </div>

      <div class="info-right">
        <div style="display: flex;flex-direction: row;">
          <span class="del-btn" @click="deleteHumerText" style="margin-right: 15px;"><i class="el-icon-back"
              style="margin-right: 5px;"></i>退格</span>
          <span class="del-btn" @click="clearHumerText"><i class="el-icon-delete"
              style="margin-right: 5px;"></i>清除</span>
        </div>
        <div :class="{ 'othdis': customDisabled }">
          <div class="in-top">添加参数：</div>
          <div>
            <select size="3" class="in-list" :disabled="customDisabled">
              <option class="in-item" :value="getParamVal(opx.code)" v-for="opx in ParamList" :key="opx.code"
                @click="addItem($event, 'custom', opx.name, getParamVal(opx.code), customDisabled)">{{ opx.name }}
              </option>
            </select>
          </div>
          <div class="in-top">添加属性和标签：</div>
          <div>
            <el-select :disabled="customDisabled" :clearable="true" @clear="devChange('')" size="small"
              v-model="filterId" filterable remote reserve-keyword placeholder="请输入搜索关键词" :remote-method="remoteMethod"
              :loading="searchloading" @change="devChange">
              <el-option v-for="item in options" :key="item.Id" :label="item.Name" :value="item.Id">
              </el-option>
            </el-select>
          </div>
          <div>
            <select size="3" class="in-list" :disabled="customDisabled">
              <option class="in-item" :value="getPropVal(opx.code)" v-for="opx in DevPropList" :key="'#pp' + opx.code"
                @click="addItem($event, 'custom', '【' + filterDevice.Name + '-属】' + opx.name, getPropVal(opx.code), customDisabled)">
                {{ opx.name }}
              </option>
              <option class="in-item" :value="getTagVal(tpx.code)" v-for="tpx in DevTagList" :key="tpx.code"
                @click="addItem($event, 'custom', '【' + filterDevice.Name + '-签】' + tpx.name, getTagVal(tpx.code), customDisabled)">
                {{ tpx.name }}
              </option>
              <option class="in-item" v-show="filterId == ''" :value="getCurPropVal(opx.code)" v-for="opx in propItems"
                :key="'#xxpp' + opx.code"
                @click="addItem($event, 'custom', '【属】' + opx.name, getCurPropVal(opx.code), customDisabled)">
                {{ '【属】' + opx.name }}
              </option>
              <option class="in-item" v-show="filterId == ''" :value="getCurTagVal(tpx.code)" v-for="tpx in tagItems"
                :key="'#xx' + tpx.code"
                @click="addItem($event, 'custom', '【签】' + tpx.name, getCurTagVal(tpx.code), customDisabled)">
                {{ '【签】' + tpx.name }}
              </option>
            </select>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { DeviceList } from "@/api/rules/device";
import { productInfo, productList, tslList } from "@/api/rules/productModel";

export default {
  name: 'ExpressEditor',
  props: {
    content: { type: String, default: '' },
    domflag: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      dataDisabled: false,
      customDisabled: false,
      // 公式编辑器最后光标位置
      formulaLast: 0,
      Num: [],
      Symbol: [],
      customType: [],
      backFormulaStrArray: [], // {type:"num||symbol||custom||start||data",value:"",name:""}
      filterId: "",
      filterDevice: null,
      searchloading: false,
      filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
      },
      options: [],
      DevPropList: [],
      DevTagList: []
    }
  },
  computed: {
    ParamList() {
      let httpss = this.$store.state.rulesFlowable.rulesDesign.HttpParams;
      if (httpss != null) {
        return httpss;
      }
      else {
        return [];
      }
    },
    propItems() {
      return this.$store.state.rulesFlowable.rulesProductAttr;
    },
    tagItems() {
      return this.$store.state.rulesFlowable.rulesProductTags;
    }
  },
  created() {

  },
  watch: {
    content: {
      // 每个属性值发生变化就会调用这个函数
      handler(newVal, oldVal) {
        this.resetDisable();
        this.parsingFormula(this.content);
        this.formulaLast = this.backFormulaStrArray.length - 1;
        this.updateLast();
        this.remoteMethod("");
      },
      immediate: true
    },
  },
  methods: {
    devChange(devId) {
      if (devId == "") {
        this.filterId = "";
        this.filterDevice = null;
        this.DevPropList = [];
        this.DevTagList = [];
      }
      else {
        let dtuItems = this.options.filter(x => x.Id == devId);
        if (dtuItems.length > 0) {
          this.filterDevice = dtuItems[0];
          productInfo({ id: this.filterDevice.ProductId }).then(rsp => {
            if (rsp.code == 0) {
              let jsonLis = JSON.parse(rsp.data.ModelTSL);
              if (jsonLis.properties) {
                this.DevPropList = jsonLis.properties.filter(x => x.option.type != 'geo'&&x.option.type != 'file');
                this.DevTagList = jsonLis.tags.filter(x => x.option.type != 'geo'&&x.option.type != 'file');
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
    getParamVal(code) {
      return 'param("' + code + '")';
    },
    getPropVal(code) {
      return 'prop("' + this.filterId + '","' + code + '")';
    },
    getTagVal(code) {
      return 'tag("' + this.filterId + '","' + code + '")';
    },
    getCurPropVal(code) {
      return 'prop(null,"' + code + '")';
    },
    getCurTagVal(code) {
      return 'tag(null,"' + code + '")';
    },
    resetDisable() {
      this.Num = [{ "value": "1", "disabled": false }, { "value": "2", "disabled": false }, { "value": "3", "disabled": false }, { "value": "4", "disabled": false }, { "value": "5", "disabled": false }, { "value": "6", "disabled": false }, { "value": "7", "disabled": false }, { "value": "8", "disabled": false }, { "value": "9", "disabled": false }, { "value": "0", "disabled": false }, { "value": ".", "disabled": true }];
      this.Symbol = [{ "value": "+", "disabled": true }, { "value": "-", "disabled": true }, { "value": "*", "disabled": true }, { "value": "/", "disabled": true }, { "value": "(", "disabled": false }, { "value": ")", "disabled": true }, { "name": "小于", "value": "<", "disabled": true }, { "name": "大于", "value": ">", "disabled": true }, { "name": "等于", "value": "==", "disabled": true }, { "name": "不等于", "value": "!=", "disabled": true }, { "name": "成立则", "value": "?", "disabled": true }, { "name": "否则", "value": ":", "disabled": true }, { "name": "且", "value": "&&", "disabled": true }, { "name": "或", "value": "||", "disabled": true }, { "name": "大于等于", "value": ">=", "disabled": true }, { "name": "小于等于", "value": "<=", "disabled": true }];
      this.dataDisabled = false;
      this.customDisabled = false;
    },
    clearHumerText() {
      this.backFormulaStrArray.splice(1, this.backFormulaStrArray.length - 1);
      this.formulaLast = 0;
      this.resetDisable();
    },
    // 删除操作
    deleteHumerText() {
      if (this.backFormulaStrArray.length == 1) {
        return;
      }
      this.backFormulaStrArray.splice(this.formulaLast, 1);
      this.formulaLast = this.backFormulaStrArray.length - 1;
      this.updateLast();
    },
    //更新光标
    updateLast() {
      this.backFormulaStrArray.forEach(x => {
        x.active = false;
      });
      this.backFormulaStrArray[this.formulaLast].active = true;
      if (this.backFormulaStrArray.length == 1) {
        this.resetDisable();
        return;
      }

      //更新可激活添加项
      let lastcc = this.backFormulaStrArray[this.formulaLast];
      if (lastcc.value == "0" || lastcc.value == "1" || lastcc.value == "2" || lastcc.value == "3" || lastcc.value == "4" || lastcc.value == "5" || lastcc.value == "6" || lastcc.value == "7" || lastcc.value == "8" || lastcc.value == "9") {
        this.Num.forEach(x => {
          x.disabled = false;
        })
        this.Symbol.forEach(x => {
          if (x.value == "(") {
            x.disabled = true;
          }
          else {
            x.disabled = false;
          }
        })
        this.dataDisabled = true;
        this.customDisabled = true;

        let hasdot = false;
        let numaarr = this.Num.map(x => x.value);
        for (let ll = this.backFormulaStrArray.length - 2; ll > 0; ll--) {
          if (numaarr.indexOf(this.backFormulaStrArray[ll].value) < 0) {
            break;
          }
          if (this.backFormulaStrArray[ll].value)
            if (this.backFormulaStrArray[ll].value == ".") {
              hasdot = true;
            }
        }
        if (hasdot) {
          this.Num[10].disabled = true;
        }
      }
      else if (lastcc.value == ".") {
        this.Num.forEach(x => {
          if (x.value == ".") {
            x.disabled = true;
          }
          else {
            x.disabled = false;
          }
        })
        this.Symbol.forEach(x => {
          x.disabled = true;
        })
        this.dataDisabled = true;
        this.customDisabled = true;
      }
      else if (lastcc.value == "+" || lastcc.value == "-" || lastcc.value == "*" || lastcc.value == "/" || lastcc.value == "(" || lastcc.value == "?" || lastcc.value == ":" || lastcc.value == "<" || lastcc.value == ">" || lastcc.value == ">=" || lastcc.value == "<=" || lastcc.value == "==" || lastcc.value == "!=" || lastcc.value == "&&" || lastcc.value == "||") {
        this.Num.forEach(x => {
          if (x.value == ".") {
            x.disabled = true;
          }
          else {
            x.disabled = false;
          }
        })
        this.Symbol.forEach(x => {
          if (x.value == "(") {
            x.disabled = false;
          }
          else {
            x.disabled = true;
          }
        })
        this.dataDisabled = false;
        this.customDisabled = false;
      }
      else if (lastcc.value == ")") {
        this.Num.forEach(x => {
          x.disabled = true;
        })
        this.Symbol.forEach(x => {
          if (x.value == ")") {
            x.disabled = true;
          }
          else {
            x.disabled = false;
          }
        })
        this.dataDisabled = true;
        this.customDisabled = true;
      }
      else if (lastcc.type == "data" || lastcc.type == "custom") {
        this.Num.forEach(x => {
          x.disabled = true;
        })
        this.Symbol.forEach(x => {
          if (x.value == "(") {
            x.disabled = true;
          }
          else {
            x.disabled = false;
          }
        })
        this.dataDisabled = true;
        this.customDisabled = true;
      }




      let leftkhcc = 0;
      let rightkhcc = 0;
      let whcc = 0;
      let macc = 0;
      this.backFormulaStrArray.forEach(x => {
        if (x.value == "(") {
          leftkhcc++;
        }
        else if (x.value == ")") {
          rightkhcc++;
        }
        else if (x.value == "?") {
          whcc++;
        }
        else if (x.value == ":") {
          macc++;
        }
      })
      if (leftkhcc <= rightkhcc) {
        this.Symbol[5].disabled = true;
      }
      if (whcc <= macc) {
        this.Symbol[11].disabled = true;
      }
      else {
        if (leftkhcc <= rightkhcc) {
          this.Symbol.forEach(x => {
            if (x.value != ":" && x.value != "(") {
              x.disabled = true;
            }
          })
        }
      }

      let llkhcc = 0;
      let rrkhcc = 0;
      let startIdx = 1;
      let lastIsSym = this.backFormulaStrArray[this.backFormulaStrArray.length - 1].value == ")";
      let symttt = -1;
      for (let zz = this.backFormulaStrArray.length - 1; zz > 0; zz--) {
        let zzval = this.backFormulaStrArray[zz].value;
        if (zzval == "(") {
          llkhcc++;
        }
        else if (zzval == ")") {
          rrkhcc++;
        }
        if (symttt == -1) {
          if (zzval == ">" || zzval == "<" || zzval == ">=" || zzval == "<=" || zzval == "==" || zzval == "!=" || zzval == "&&" || zzval == "||") {
            symttt = 0;
          }
          else if (zzval == "+" || zzval == "-" || zzval == "*" || zzval == "/") {
            symttt = 1;
          }
        }
        if (lastIsSym) {
          if (rrkhcc == llkhcc && (zzval == "+" || zzval == "-" || zzval == "*" || zzval == "/" || zzval == "?" || zzval == ":")) {
            startIdx = zz + 1;
            break;
          }
        }
        else {
          if (rrkhcc == llkhcc) {
            if (symttt == 1 && (zzval == ">" || zzval == "<" || zzval == ">=" || zzval == "<=" || zzval == "==" || zzval == "!=" || zzval == "&&" || zzval == "||" || zzval == "?" || zzval == ":")) {
              startIdx = zz + 1;
              break;
            }
            else if (symttt == 0 && (zzval == "+" || zzval == "-" || zzval == "*" || zzval == "/" || zzval == "?" || zzval == ":")) {
              startIdx = zz + 1;
              break;
            }
          }
        }
      }

      let lfstr = "";
      for (let txxi = startIdx; txxi < this.backFormulaStrArray.length; txxi++) {
        lfstr += this.backFormulaStrArray[txxi].value;
      }
      let exers = this.isExpress(lfstr);
      if (exers.success) {
        if (typeof exers.result === 'boolean') {
          this.Symbol.forEach(x => {
            if (x.value == "+" || x.value == "-" || x.value == "*" || x.value == "/" || x.value == ">" || x.value == "<" || x.value == ">=" || x.value == "<=") {
              x.disabled = true;
            }
          })
        }
        else if (exers.result == null) {
          this.Symbol.forEach(x => {
            if (x.value == "+" || x.value == "-" || x.value == "*" || x.value == "/" || x.value == ">" || x.value == "<" || x.value == ">=" || x.value == "<=" || x.value == "?" || x.value == "&&" || x.value == "||") {
              x.disabled = true;
            }
          })
        }
        else {
          this.Symbol.forEach(x => {
            if (x.value == "?" || x.value == "&&" || x.value == "||") {
              x.disabled = true;
            }
          })
        }
      }
    },
    // 获取公式字符串
    getFormulaStr: function () {
      var str = ''
      for (let i = 1; i < this.backFormulaStrArray.length; i++) {
        str += this.backFormulaStrArray[i].value;
      }
      return str;
    },
    addItem: function (e, type, name, val, disabled) {
      if (disabled) return;
      this.backFormulaStrArray.push({ type: type, name: name, value: val });
      this.formulaLast = this.backFormulaStrArray.length - 1;
      this.updateLast();
    },
    //校验是否为表达式
    isExpress(str) {
      try {
        let callFunction = eval("(data,param,prop,tag)=>" + str);
        let rs = (callFunction)(1, function (k) { return 1; }, function (id, k) { return 1; }, function (id, k) { return 1; });
        return { success: true, result: rs };
      }
      catch (ex) {
        return { success: false, result: null };
      }

    },

    // 公式反向解析
    async parsingFormula(formulaStr) {
      this.backFormulaStrArray = [];
      this.backFormulaStrArray.push({ type: "start", name: "" });
      let funcStr = "";
      let symStr = "";
      let devIds = [];
      for (let i = 0; i < formulaStr.length; i++) {
        let cc = formulaStr[i];
        if (funcStr == "" && symStr == "") {
          let formitem = this.Num.find(x => x.value == cc);
          if (formitem != null) {
            let tmpformname = cc;
            if (formitem.name != null) {
              tmpformname = formitem.name;
            }
            this.backFormulaStrArray.push({ type: "num", name: tmpformname, value: cc });
          }
          else {
            formitem = this.Symbol.find(x => x.value == cc);
            if (formitem != null) {
              let tmpformname = cc;
              if (formitem.name != null) {
                tmpformname = formitem.name;
              }
              this.backFormulaStrArray.push({ type: "symbol", name: tmpformname, value: cc });
            }
            else {
              if (cc == "&" || cc == "|" || cc == "=" || cc == ">" || cc == "<" || cc == "!") {
                symStr += cc;
              }
              else {
                funcStr += cc;
              }
            }
          }
        }
        else if (symStr.length > 0) {
          symStr += cc;
          if (symStr == "&&") {
            this.backFormulaStrArray.push({ type: "symbol", name: "且", value: symStr });
            symStr = "";
          }
          else if (symStr == "||") {
            this.backFormulaStrArray.push({ type: "symbol", name: "或", value: symStr });
            symStr = "";
          }
          else if (symStr == "==") {
            this.backFormulaStrArray.push({ type: "symbol", name: "等于", value: symStr });
            symStr = "";
          }
          else if (symStr == ">=") {
            this.backFormulaStrArray.push({ type: "symbol", name: "大于等于", value: symStr });
            symStr = "";
          }
          else if (symStr == "<=") {
            this.backFormulaStrArray.push({ type: "symbol", name: "小于等于", value: symStr });
            symStr = "";
          }
          else if (symStr == "!=") {
            this.backFormulaStrArray.push({ type: "symbol", name: "不等于", value: symStr });
            symStr = "";
          }
        }
        else {
          funcStr += cc;
          if (funcStr == "data") {
            funcStr = "";
            this.backFormulaStrArray.push({ type: "data", name: "原值", value: "data" });
          }
          else if (funcStr == "true") {
            funcStr = "";
            this.backFormulaStrArray.push({ type: "bool", name: "真", value: "true" });
          }
          else if (funcStr == "false") {
            funcStr = "";
            this.backFormulaStrArray.push({ type: "bool", name: "假", value: "false" });
          }
          else if (cc == ")") {
            let newcc = funcStr;
            funcStr = "";
            let startIdx = newcc.indexOf('(');
            let funcname = newcc.substr(0, startIdx);
            let cont = newcc.substr(startIdx + 1);
            cont = cont.substr(0, cont.length - 1);
            let conparas = cont.split(",");
            let newconparas = conparas.map(x => {
              return x.replace(new RegExp("'", "gm"), "").replace(new RegExp("\"", "gm"), "");
            })

            let tmpname = "";
            let tmpcode = "";
            if (funcname == "param") {
              let paraaar = this.ParamList.filter(x => x.code == newconparas[0]);
              if (paraaar.length > 0) {
                tmpname = paraaar[0].name;
                tmpcode = 'param("' + paraaar[0].code + '")';
                this.backFormulaStrArray.push({ type: "custom", name: tmpname, value: tmpcode });
                continue;
              }
            }
            else if (funcname == "prop") {
              if (newconparas[0] != "null") {
                devIds.push(newconparas[0]);
              }
            }
            else if (funcname == "tag") {
              if (newconparas[0] != "null") {
                devIds.push(newconparas[0]);
              }

            }
            this.backFormulaStrArray.push({ type: "custom", name: newcc, value: newcc });
          }

        }
      }

      //获取设备信息
      let alldevs = []
      if (devIds.length > 0) {
        alldevs = (await DeviceList({ Ids: devIds, showAll: true })).data.List;
      }
      let productIds = alldevs.map(x => x.ProductId);
      let localProdId = null;
      if (this.$store.state.rulesFlowable.selectProductInfo != null) {
        localProdId = this.$store.state.rulesFlowable.selectProductInfo.Id;
        productIds.push(localProdId);
      }

      //获取产品信息
      let allprod = [];
      if (productIds.length > 0) {
        allprod = (await tslList(productIds)).data;
      }
      this.backFormulaStrArray.forEach(x => {
        if (x.type == "custom" && (x.value.indexOf("prop") == 0 || x.value.indexOf("tag") == 0)) {
          let startIdx = x.value.indexOf('(');
          let cont = x.value.substr(startIdx + 1);
          cont = cont.substr(0, cont.length - 1);
          let conparas = cont.split(",");
          let newconparas = conparas.map(x => {
            return x.replace(new RegExp("'", "gm"), "").replace(new RegExp("\"", "gm"), "");
          })
          let newname = "";
          if (newconparas[0] == "null") {
            let ppvvlist = allprod.filter(pp => pp.Id == localProdId);
            if (ppvvlist.length > 0) {
              let jsonLis = JSON.parse(ppvvlist[0].ModelTSL);
              if (x.value.indexOf("prop") == 0 && jsonLis.properties) {
                let allppp = jsonLis.properties;
                let ddpppll = allppp.filter(ppx => ppx.code == newconparas[1]);
                if (ddpppll.length > 0) {
                  newname = "【属】" + ddpppll[0].name;
                }
              }
              else if (x.value.indexOf("tag") == 0 && jsonLis.tags) {
                let allppp = jsonLis.tags;
                let ddpppll = allppp.filter(ppx => ppx.code == newconparas[1]);
                if (ddpppll.length > 0) {
                  newname = "【签】" + ddpppll[0].name;
                }
              }
            }
          }
          else {
            let ddvvlist = alldevs.filter(dd => dd.Id == newconparas[0]);
            if (ddvvlist.length > 0) {
              newname = "【" + ddvvlist[0].Name;
              let ppvvlist = allprod.filter(pp => pp.Id == ddvvlist[0].ProductId);
              if (ppvvlist.length > 0) {
                let jsonLis = JSON.parse(ppvvlist[0].ModelTSL);
                if (x.value.indexOf("prop") == 0 && jsonLis.properties) {
                  let allppp = jsonLis.properties;
                  let ddpppll = allppp.filter(ppx => ppx.code == newconparas[1]);
                  if (ddpppll.length > 0) {
                    newname = newname + "-属】" + ddpppll[0].name;
                  }
                }
                else if (x.value.indexOf("tag") == 0 && jsonLis.tags) {
                  let allppp = jsonLis.tags;
                  let ddpppll = allppp.filter(ppx => ppx.code == newconparas[1]);
                  if (ddpppll.length > 0) {
                    newname = newname + "-签】" + ddpppll[0].name;
                  }
                }
              }
            }
          }

          x.name = newname;
        }
      })


    },

  }
}
</script>

<style lang="scss">
@keyframes blink {
  0% {
    opacity: 1;
  }

  /* 初始状态为完全不透明 */
  50% {
    opacity: 0.2;
  }

  /* 中间状态为部分透明度 */
  100% {
    opacity: 1;
  }

  /* 结束状态为完全不透明 */
}

#formulaPage {
  width: 100%;
  height: 100%;

  >.formulaView {
    margin-bottom: 20px;
    min-height: 130px;
    width: 100%;
    padding: 5px;
    border: 5px solid rgb(198, 226, 255);
    resize: both;
    overflow: auto;
    line-height: 25px;
    font-size: 20px;
    resize: none;

    span {
      position: relative;
      user-select: none;
      display: inline-block;
      height: 20px;
      line-height: 20px;
      border-radius: 3px;
      white-space: nowrap;
      margin-right: 2px;

      &:first-child {
        margin-left: 0;
      }
    }

    .cursor::after {
      position: absolute;
      right: -5px;
      top: 0;
      content: "";
      width: 2px;
      height: 20px;
      background-color: #666;
      animation: blink 1s infinite;
    }
  }

  .footerComtent {
    margin-top: 20px;
  }

  .infomationContent {
    display: flex;

    >.tab {

      >ul {
        margin: 0;
        padding: 0;
        display: flex;
        flex-wrap: wrap;

        // justify-content: space-between;
        &:after {
          content: '';
          display: table;
          clear: both;
        }

        >li {
          // margin-right: 20px;
          margin: 5px 10px;
          width: 65px;
          text-align: center;
          float: left;
          padding: 0 10px;
          height: 30px;
          line-height: 30px;
          border-radius: 5px;
          font-weight: bold;
          border: 1px solid #fff;
          list-style-type: none;
          cursor: pointer;
          -moz-user-select: none;
          /* 火狐 */
          -webkit-user-select: none;
          /* 谷歌、Safari */
          -ms-user-select: none;
          /* IE10+ */
          user-select: none;
          user-drag: none;
          box-shadow: 0px 0px 2px 2px rgba(55, 114, 203, 0.2),
            /*下面深蓝色立体阴影*/
            0px 0px 6px 1px #4379d0,
            /*内部暗色阴影*/
            0 -15px 2px 2px rgba(55, 114, 203, 0.1) inset;
          color: #fff;
          overflow: hidden;
        }

        >li:hover {
          color: #333;
          border-color: #c6e2ff;
          background-color: #ecf5ff;
        }

        >li:active {
          color: #3a8ee6;
          border-color: #3a8ee6;
          outline: none;
        }

        >li.dis {
          background-color: #a8ceff !important;
        }

        .noclick {
          cursor: not-allowed !important;
          background: #bcc0c4 !important;
          color: #e4eaf1 !important;
          border: none !important;
        }
      }
    }

  }

  .del-btn {
    display: flex;
    align-items: center;
    color: #333;
    padding: 0 10px;
    padding: 10px 15px;
    cursor: pointer;
    border-radius: 5px;
    box-shadow: 0px 0px 2px 2px rgba(203, 55, 55, 0.2),
      /*下面深蓝色立体阴影*/
      0px 0px 6px 1px #c40001,
      /*内部暗色阴影*/
      0 -15px 2px 2px rgba(209, 57, 57, 0.1) inset;
  }

  .del-btn:hover {
    color: #ffafaf;
  }

  .info-left {
    width: 450px;
  }

  .info-right {
    flex: 1;
    width: 0;
    padding-top: 5px;

    .othdis {
      opacity: 0.5;
    }

    .in-top {
      background-color: #409EFF;
      margin-top: 10px;
      line-height: 35px;
      padding: 0 10px;
      font-size: 12px;
      color: #fff;
    }

    .in-list {
      width: 100%;
      height: 120px;
      border: solid 1px #dadada;
      outline: none;

      .in-item {
        display: flex;
        align-items: center;
        justify-content: center;
        height: 35px;
      }
    }
  }
}
</style>