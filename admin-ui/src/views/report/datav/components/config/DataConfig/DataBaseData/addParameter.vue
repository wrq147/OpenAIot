<template>
  <div class="right">
    <div class="title">
      <span style="color:#ff0000;line-height: 38px;margin-right: 15px;">注意：目前不支持sql server的专有关键词自动提示</span>
      <el-button type="primary" icon="el-icon-plus" size="mini" @click="addParameter">添加参数</el-button>
    </div>
    <div class="parameter">
      <div v-for="(item, index) in variableList" :key="index">
        <el-form class="parameterBox" ref="ruleForm" :model="item" :rules="rules" label-width="80px">
          <div class="variableBox">
            <div class="title">
              <!-- <el-form-item label="字段名称" prop="lable">
                <el-input placeholder="请输入" v-model="item.lable" />
              </el-form-item> -->
              <el-button type="primary" icon="el-icon-paperclip" size="mini" @click="quoteData(index, item)">引用到变量</el-button>
            </div>
            <div class="variableData">
              <el-form-item label="字段key" prop="key">
                <el-input placeholder="请输入" v-model="item.key" />
              </el-form-item>
              <el-form-item label="字段类型">
                <el-select v-model="item.type" @change="item.value = ''" placeholder="请选择">
                  <el-option label="字符串" value="字符串" />
                  <el-option label="数字" value="数字" />
                  <el-option label="布尔" value="布尔" />
                </el-select>
              </el-form-item>
              <el-form-item label="参数值" prop="value">
                <el-select v-if="item.type === '布尔'" v-model="item.value" placeholder="请选择">
                  <el-option label="true" value="true" />
                  <el-option label="false" value="false" />
                </el-select>
                <el-autocomplete v-else size="small" v-model="item.value" :fetch-suggestions="querySearch" placeholder="请设置字段值"  @select="handleSelect($event, index)" />
              </el-form-item>
            </div>
          </div>
          <div class="close-btn select-line-icon" @click="removeSelectItem(index)">
            <i class="el-icon-remove-outline" />
          </div>
        </el-form>
      </div>
    </div>
  </div>
</template>
<script>
import { getLinkChart } from "../../../../util/LinkageChart";
export default {
  name: 'addParameter',
  props: {
    costomData: {
      type: Object,
      required: true,
    },
    drawingList: {
      type: Array,
      required: true,
    },
    tableType: {
      type: String,
      default: "default",
    }
  },
  data() {
    return {
      // 表单
      variableList: [],
      rules: {
        lable: [
          { required: true, message: "名称不能为空", trigger: "blur" },
        ],
        key: [
          { required: true, message: "key不能为空", trigger: "blur" },
        ],
        value: [
          { required: true, message: "参数值不能为空", trigger: "blur" },
          { required: true, message: "参数值不能为空", trigger: "change" },
        ]
      }
    }
  },
  watch: {
    variableList: {
      handler: function (val) {
        this.$emit("getVariableList", this.variableList)
      },
      deep: true
    }
  },
  methods: {
    querySearch(queryString, cb) {
      let newArr = []
      if (this.tableType === 'spreadSheet') {
        let searchTableData = JSON.parse(localStorage.getItem("searchTableData"))
        if (searchTableData === null) return
        newArr = searchTableData.map((x) => {
          return { value: x.fieldName, formatDefault: x.formatDefault };
        });
      } else {
        let drawArr = getLinkChart(this.drawingList);
        // console.log(drawArr)
        newArr = drawArr.filter(x => x.chartType !== 'timeFrame').map((x) => {
          return { value: x.layerName, id: x.customId };
        });
        this.drawingList.map(item => {
          if (item.chartOption.pageSize !==undefined) {
            newArr.push({ value: item.chartOption.pageSize, id: item.customId })
          }
        })
      }
      cb(newArr);
    },
    handleSelect(item, index) {
      if (this.tableType === 'spreadSheet') {
        this.variableList[index].value = item.formatDefault;
        this.variableList[index].key = item.value;
      } else {
        if (isNaN(Number(item.value))) {
          this.variableList[index].value = "@" + item.value;
        } else {
          this.variableList[index].value = item.value.toString();
        }
      }
    },
    addParameter() {
      this.variableList.push({ key: '', type: '字符串', value: ''})
    },
    removeSelectItem(index) {
      this.variableList.splice(index, 1)
    },
    quoteData(index, item) {
      this.$refs['ruleForm'][index].validate((valid) => {
        if (valid) {
          this.$emit('quoteData', item)
        } else {
          return false
        }
      })
    },
    parameterReplace(str) {
      let replaceData = this.extractTemplateExpressions(str)
      let replacements = {}
      if (replaceData.length > 0) {
        for (let i = 0; i < replaceData.length; i++){
          this.variableList.forEach(v => {
            if (v.key === replaceData[i]) {
              if (v.value.indexOf('@') === 0) { 
                replacements[replaceData[i]] = this.isLinkParam(v.value)
              } else {
                replacements[replaceData[i]] = v.value
              }
            }
          })
        }
        let newStr = this.replaceTemplateString(str, replacements)
        return newStr
      } else {
        return str
      }
    },

    // 识别参数的value值带@更新为条件组件的值
    isLinkParam(value) {
      let newname = value.substring(1);
      let prename = "";
      let newChartList = this.drawingList.filter(x => x.layerName == newname);
      if (newChartList.length > 0) {
        if (newChartList[0].chartOption.getVal != null) {
          if (newChartList[0].chartOption.preprocess != null && prename != "") {
            if (prename in option.preprocess) {
              let code = option.preprocess[prename];
              let val = newChartList[0].chartOption.getVal();
              return (eval(code)).call({parseTime}, val);
            }
          }
          return newChartList[0].chartOption.getVal();
        }
      }
      return value
    },

    // 识别字符串里有${ } 并替换内容
    replaceTemplateString(str, replacements) {
      return str.replace(/\$\{([\w\p{L}]+)\}/gu, function(match, key) {
        return replacements[key] || match;
      });
    },
    // 识别字符串里有${ } 并提取出来
    extractTemplateExpressions(str) {
      const regex = /\$\{([^}]+)\}/g;
      let match;
      const expressions = [];
    
      while ((match = regex.exec(str))) {
        expressions.push(match[1]);
      }
      return expressions;
    }
  }
}
</script>
<style lang="scss" scoped>
::v-deep{
  .el-input{
    width: 90px;
  }
  .el-input__inner{
    background-color:#F5F6F8;
    border: none;
  }
  .el-select{
    width: 100px !important;
    height: 28px;
  }
  .el-input--suffix, .el-input__inner{
    height: 28px;
  }
  .el-input__icon {
    line-height: 28px;
  }
  .el-form-item{
    margin-bottom: 0;
    // border-right: 1px solid #ccc;
  }
}
.containnerTop .right{
  width: 50%;
}
.title{
  height: 50px;
  font-weight: bold;
  display: flex;
  justify-content: flex-end;
  padding: 0 10px 10px 10px;
}
.parameter{
  height: 300px;
  overflow-y: auto;
  border: 1px solid #ccc;
}
.parameterBox{
  padding: 15px;
  display: flex;
  align-items: center;
}
.parameterBox i{
  width: 22px;
  height: 22px;
  color: red;
}
.variableBox{
  width: 98%;
  border-radius: 5px;
  background-color: #F5F6F8;
  padding: 10px;
}
.variableBox .title{
  height: 40px;
}
.variableData{
  padding-top: 10px;
  border-top: 1px solid #ccc;
  display: flex;
  justify-content: space-between;
}
</style>