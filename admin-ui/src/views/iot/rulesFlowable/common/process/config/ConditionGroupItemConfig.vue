<template>
  <div>
    <div v-for="(group, index) in selectedNode.props.groups" :key="index + '_g'" class="group">
      <div class="group-header">
        <span class="group-name">条件组 {{ groupNames[index] }}</span>
        <div class="group-cp">
          <span>组内条件关系：</span>
          <el-switch v-model="group.groupType" active-color="#409EFF" inactive-color="#c1c1c1" active-value="AND"
            inactive-value="OR" active-text="且" inactive-text="或" />
        </div>
        <div class="group-operation">
          <el-popover placement="bottom" title="请选择条件" width="520" trigger="click">
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
            <div
              style="display: flex;justify-content: space-between;max-height: 400px;padding-top:15px;overflow-y: auto;">
              <span style="width: 80px;color:#999;font-size: 14px;">过滤条件：</span>
              <div style="width: 0px;flex:1;">
                <el-checkbox-group v-model="group.cids" value-key="code" style="margin-top: 0;padding-top: 0;">
                  <el-checkbox :label="condition.code" v-for="(condition, cindex) in conditionList"
                    :key="condition.code" @change="conditionChange(cindex, group)">{{ condition.title }}</el-checkbox>
                </el-checkbox-group>
              </div>

            </div>


            <i class="el-icon-plus" slot="reference"></i>
          </el-popover>
          <i class="el-icon-delete" @click="delGroup(index)"></i>
        </div>
      </div>
      <div class="group-content">
        <p v-if="group.conditions.length === 0">点击右上角 + 为本条件组添加条件 ☝</p>
        <div v-else>
          <el-form ref="condition-form" label-width="100px">
            <!--构建表达式-->
            <el-form-item v-for="(condition, cindex) in group.conditions" :key="condition.code + '_' + cindex">
              <ellipsis slot="label" hover-tip
                :content="condition.gname != null ? (condition.gname + '-' + condition.title) : condition.title" />
              <span v-if="condition.valueType === ValueType2.string">
                <el-select size="small" placeholder="判断符" style="width: 120px" v-model="condition.compare"
                  @change="condition.value = []">
                  <el-option label="等于" value="="></el-option>
                  <el-option label="不等于" value="!="></el-option>
                  <el-option label="包含" value="IN"></el-option>
                </el-select>
                <span style="margin-left: 10px">
                  <el-select v-if="condition.valueFrom == 1" size="small" v-model="condition.value" placeholder="请选择参数"
                    style="width: 220px">
                    <template v-for="opx in conditionList">
                      <el-option :label="opx.title" :value="opx.code" v-if="opx.valueType == 'string'"
                        :key="'cc' + opx.code"></el-option>
                    </template>
                  </el-select>
                  <el-input v-else style="width: 220px" placeholder="输入比较值" size="small" v-model="condition.value" />
                </span>
              </span>
              <span v-else-if="condition.valueType === ValueType2.int || condition.valueType === ValueType2.float">
                <el-select size="small" placeholder="判断符" style="width: 120px" v-model="condition.compare">
                  <el-option :label="exp.label" :value="exp.value" :key="exp.value" v-for="exp in explains"></el-option>
                </el-select>
                <span style="margin-left: 10px">
                  <el-select v-if="condition.valueFrom == 1" size="small" v-model="condition.value" placeholder="请选择参数"
                    style="width: 220px">
                    <template v-for="opx in conditionList">
                      <el-option :label="opx.title" :value="opx.code"
                        v-if="(condition.valueType === ValueType2.int && opx.valueType == 'int') || (condition.valueType === ValueType2.float && opx.valueType == 'float')"
                        :key="'cc' + opx.code"></el-option>
                    </template>
                  </el-select>
                  <el-input v-else style="width: 220px" size="small" placeholder="输入比较值" type="number"
                    v-model="condition.value" />
                </span>
              </span>
              <span v-else-if="condition.valueType === ValueType2.enum || condition.valueType === ValueType2.boolean">
                <el-select size="small" placeholder="判断符" style="width: 120px" v-model="condition.compare"
                  @change="condition.value = ''">
                  <el-option :label="condition.codeIsArrary==true||condition.valueType === ValueType2.enum ?'包含':'等于'" value="="></el-option>
                  <el-option :label="condition.codeIsArrary==true||condition.valueType === ValueType2.enum ?'不包含':'不等于'" value="!="></el-option>
                </el-select>
                <span style="margin-left:10px">
                  <el-select v-if="condition.valueFrom == 1"  size="small" v-model="condition.value" placeholder="请选择参数"
                    style="width: 220px">
                    <template v-for="opx in conditionList">
                      <el-option :label="opx.title" :value="opx.code"
                        v-if="(condition.valueType === ValueType2.enum && opx.valueType == 'enum') || (condition.valueType === ValueType2.boolean && opx.valueType == 'boolean')"
                        :key="'cc' + opx.code"></el-option>
                    </template>
                  </el-select>
                  <el-select v-else style="width: 220px" clearable size="small" :multiple="condition.codeIsArrary==true" v-model="condition.value"
                    placeholder="选择值">
                    <el-option v-for="(option, oi) in compareItems[condition.code]" :key="oi" :label="option.key"
                      :value="option.value"></el-option>
                  </el-select>
                </span>
              </span>
              <span v-else-if="condition.valueType === ValueType2.date">
                <span>在</span>
                <el-select v-show="condition.valueFrom == 1" size="small" v-model="condition.value" placeholder="请选择参数"
                  style="margin-left: 10px">
                  <template v-for="opx in conditionList">
                    <el-option :label="opx.title" :value="opx.code" v-if="opx.valueType == 'date'"
                      :key="'cc' + opx.code"></el-option>
                  </template>
                </el-select>
                <el-date-picker v-show="condition.valueFrom != 1" style="margin-left: 10px" v-model="condition.value"
                  value-format="timestamp" type="datetime" size="small" placeholder="请选择日期和时间"></el-date-picker>
                <el-select size="small" placeholder="判断符" style="width: 120px;margin-left: 10px"
                  v-model="condition.compare">
                  <el-option label="之前" value="before"></el-option>
                  <el-option label="之后" value="after"></el-option>
                </el-select>
              </span>
              <span v-if="condition.valueFrom == 1" class="el-swit" @click="switchVal(condition, 0)"
                style="margin-left:20px;margin-right: 10px;">变</span>
              <span v-else class="el-swit" @click="switchVal(condition, 1)"
                style="margin-left:20px;margin-right: 10px;">值</span>
              <i class="el-icon-delete" @click="rmSubCondition(group, cindex)"></i>
            </el-form-item>
          </el-form>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ValueType2 } from "../../form/ComponentsConfigExport";
import Ellipsis from "../../Ellipsis.vue";
import { DeviceList } from "@/api/rules/device";
import { productInfo } from "@/api/rules/productModel";
export default {
  name: "ConditionGroupItemConfig",
  components: { Ellipsis },
  data() {
    return {
      compareItems: {},
      ValueType2,
      showOrgSelect: false,
      //groupConditions: [],
      groupNames: ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"],
      explains: [
        { label: "等于", value: "=" },
        { label: "不等于", value: "!=" },
        { label: "大于", value: ">" },
        { label: "大于等于", value: ">=" },
        { label: "小于", value: "<" },
        { label: "小于等于", value: "<=" },
      ],
      filterId: "",
      filterDtuAttr: [],
      filterDtuTags:[],
      searchloading: false,
      filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
      },
      options: []
    };
  },
  computed: {
    selectedNode() {
      return this.$store.state.rulesFlowable.rulesSelectedNode;
    },
    formItems() {
      return this.$store.state.rulesFlowable.rulesProductAttr;
    },
    formTags() {
      return this.$store.state.rulesFlowable.rulesProductTags;
    },
    formMap() {
      const map = new Map();
      this.formItems.forEach(item => this.itemToMap(map, item));
      return map;
    },
    conditionList() {
      //构造条件
      const conditionItems = [];
      let dtuItems = this.options.filter(x => x.Id == this.filterId);
      if (dtuItems.length > 0) {
        this.filterDtuAttr.forEach(item => {
          let newitem = JSON.parse(JSON.stringify(item));
          newitem.name = "【属】" + item.name;
          newitem.gname = dtuItems[0].Name;
          newitem.code = "$devprop." + this.filterId + "." + item.code;
          this.filterCondition(newitem, conditionItems);
        });

        this.filterDtuTags.forEach(item => {
          let newitem = JSON.parse(JSON.stringify(item));
          newitem.name = "【签】" + item.name;
          newitem.gname = dtuItems[0].Name;
          newitem.code = "$devtag." + this.filterId + "." + item.code;
          this.filterCondition(newitem, conditionItems);
        });

        let xboolArr = [];
        let tmpcode = `$online.${this.filterId}`;
        xboolArr.push({ "key": '真', "value": "true" });
        xboolArr.push({ "key": '假', "value": "false" });
        this.compareItems[tmpcode] = xboolArr;
        conditionItems.push({
          gname: dtuItems[0].Name,
          title: "在线状态",
          code: tmpcode,
          valueType: "boolean"
        });


        conditionItems.push({
          gname: dtuItems[0].Name,
          title: "属性变更间隔（秒）",
          code: `$updelta.${this.filterId}`,
          valueType: "int"
        });
      }
      else {
        this.formItems.forEach(item =>{
          let newitem = JSON.parse(JSON.stringify(item));
          newitem.name = "【属】" + item.name;
          newitem.code = item.code;
          this.filterCondition(newitem, conditionItems)
        });

        this.formTags.forEach(item =>{
          let newitem = JSON.parse(JSON.stringify(item));
          newitem.name = "【签】" + item.name;
          newitem.code = "$devtag.." + item.code;
          this.filterCondition(newitem, conditionItems)
        });
      }

      //构造节点条件
      const excType = [
        "METRONOME",
        "ExceptNode",
        "TIMESCHEDULER"
      ];

      this.$store.state.rulesFlowable.rulesNodeMap.forEach((v) => {
        if (excType.indexOf(v.type) != -1) {
          let boolArr = [];
          let tmpcode = `$node.${v.id}.IsActive`;
          boolArr.push({ "key": '真', "value": "true" });
          boolArr.push({ "key": '假', "value": "false" });
          this.compareItems[tmpcode] = boolArr;
          conditionItems.push({
            code: tmpcode,
            title: `【${v.name}】的激活状态`,
            valueType: "boolean",
            compare: "=",
            value: "true"
          });
        }
      });
      //其它条件
      conditionItems.push({
        code: "$now",
        title: "执行时间",
        valueType: "date"
      });

      let boolArr = [];
      boolArr.push({ "key": "真", "value": "true" });
      boolArr.push({ "key": "假", "value": "false" });
      this.compareItems["$true"] = boolArr;
      conditionItems.push({
        code: "$true",
        title: "真值常量",
        valueType: "boolean",
        compare: "=",
        value: "true",
        valueFrom: 0
      });
      let enumArr = [];
      this.formItems.forEach(item => {
        enumArr.push({ "key": item.name, "value": item.code });
      });

      if (this.$store.state.rulesFlowable.rulesDesign.TriggerWay == 0&&this.$store.state.rulesFlowable.rulesDesign.TopicMsg.value=='ReadPropertyReply') {
        this.compareItems["$prop"] = enumArr;
        conditionItems.push({
          code: "$prop",
          codeIsArrary:true,
          title: "触发的属性",
          valueType: "enum",
          compare: "="
        });

        this.compareItems["$change"] = enumArr;
        conditionItems.push({
          code: "$change",
          codeIsArrary:true,
          title: "改变的属性",
          valueType: "enum",
          compare: "="
        });
      }

      //添加输入参数
      let httpss = this.$store.state.rulesFlowable.rulesDesign.HttpParams;
      if (httpss != null) {
        let iptparams = JSON.parse(JSON.stringify(httpss));
        iptparams.forEach(item => {
          item.code = "$param." + item.code;
          this.filterCondition(item, conditionItems)
        }
        );
      }
      return conditionItems;
    }
  },
  mounted() {
    this.remoteMethod("");
  },
  methods: {
    devChange(devId) {
      if (devId == "") {
        this.filterDtuAttr = [];
        this.filterDtuTags=[];
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
              if(jsonLis.tags){
                this.$set(this, "filterDtuTags", jsonLis.tags);
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
    itemToMap(map, item) {
      map.set(item.code, item);
      if (item.name === "SpanLayout") {
        item.props.items.forEach(sub => this.itemToMap(map, sub));
      }
    },


    filterCondition(item, list) {
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
              gname: item.gname,
              title: item.name,
              code: item.code,
              valueType: curtype
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
              gname: item.gname,
              title: item.name,
              code: item.code,
              valueType: curtype
            });
          }
        } else {
          list.push({
            gname: item.gname,
            title: item.name,
            code: item.code,
            valueType: curtype
          });
        }
      }
    },
    delGroup(index) {//删除条件
      this.selectedNode.props.groups.splice(index, 1);
    },
    rmSubCondition(group, index) {
      group.cids.splice(index, 1);
      group.conditions.splice(index, 1);
    },
    conditionChange(index, group) {
      //判断新增的
      group.cids.forEach(cid => {
        if (0 > group.conditions.findIndex(cd => cd.code === cid)) {
          //新增条件
          let condition = { ...this.conditionList[index] };
          condition.compare = "";
          condition.value = [];
          group.conditions.push(condition);
        }
      });
      for (let i = 0; i < group.conditions.length; i++) {
        //去除没有选中的
        if (group.cids.indexOf(group.conditions[i].code) < 0) {
          group.conditions.splice(i, 1);
        }
      }
    },
    switchVal(condition, valfrom) {
      condition.value = "";
      condition.valueFrom = valfrom;
      this.$forceUpdate();
    }
  }
};
</script>

<style lang="less" scoped>
.group {
  margin-bottom: 20px;
  color: #5e5e5e;
  overflow: hidden;
  border-radius: 6px;
  border: 1px solid #e3e3e3;

  .group-header {
    padding: 5px 10px;
    background: #e3e3e3;
    position: relative;

    div {
      display: inline-block;
    }

    .group-name {
      font-size: small;
    }

    .group-cp {
      font-size: small;
      position: absolute;
      left: 100px;
      display: flex;
      top: 5px;
      justify-content: center;
      align-items: center;
    }

    .group-operation {
      position: absolute;
      right: 10px;

      i {
        padding: 0 10px;

        &:hover {
          cursor: pointer;
        }
      }
    }
  }

  .group-content {
    padding: 10px 5px;

    p {
      text-align: center;
      font-size: small;
    }

    .el-swit {
      color: #409EFF;
    }

    .el-swit,
    .el-icon-delete {
      cursor: pointer;
    }
  }

  .condition-title {
    display: block;
    width: 100px;
  }
}
</style>
