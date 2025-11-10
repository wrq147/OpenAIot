<template>
  <div>
    <div>
      <div style="margin-bottom:5px"> sql语句: </div>
      <div>
        <div class="editor" style="border: 1px solid darkgrey">
          <div ref="container" style="height: 200px;" class="edit-area"></div>
        </div>
      </div>
    </div>
    <div>
    <div style="margin-bottom:5px;margin-top:10px">响应数据：</div>
      <div>
        <div class="editor" style="border: 1px solid darkgrey">
          <div style="height:230px;overflow-y:auto" contenteditable="true">
              <JsonView :data="data"></JsonView>
          </div>
        </div>
      </div>
    </div>
    <div slot="footer" class="dialog-footer" style="margin-top:20px;float:right">
      <el-button type="success" @click="refresh">刷新数据</el-button>
      <el-button type="primary" @click="apply">确认</el-button>
    </div>

  </div>
</template>

<script>
import * as monaco from "monaco-editor";
import JsonView from 'vue-json-views'
import { chartBIanalysis } from '@/api/report/sourse'

export default {
  props:["dataBase","staticValue"],
  components: { 
    JsonView
  },
   data() {
    
    return {
      monacoEditor: null,
      database: this.dataBase,
      data:[],
      validFlag:false
    }
  },
  mounted() {},
  methods: {
    // 编辑时重置数据方法
    initCom(tmpoption) {
      if(tmpoption.database.executeSql){
        this.database.executeSql = tmpoption.database.executeSql
      }
      this.data = tmpoption.rawData ? JSON.parse(tmpoption.rawData) : [];
      monaco.languages.typescript.javascriptDefaults.addExtraLib(
        '',
        "customFileName"
      );
      if(!this.$refs.container.textContent){
        this.monacoEditor = monaco.editor.create(this.$refs.container, {
        value: this.database.executeSql,
        language: "sql",
        theme: "vs",
        folding: true, // 是否折叠
        foldingHighlight: true, // 折叠等高线
        foldingStrategy: "auto", // 折叠方式
        showFoldingControls: "always", // 是否一直显示折叠
        disableLayerHinting: true, // 等宽优化
        emptySelectionClipboard: false, // 空选择剪切板
        selectionClipboard: false, // 选择剪切板
        automaticLayout: true, // 自动布局
        codeLens: true, // 代码镜头
        scrollBeyondLastLine: false, // 滚动完最后一行后再滚动一屏幕
        colorDecorators: true, // 颜色装饰器
        accessibilitySupport: "on", // 辅助功能支持"auto" | "off" | "on"
        lineNumbers: "on", // 行号 取值： "on" | "off" | "relative" | "interval" | function
        lineNumbersMinChars: 4, // 行号最小字符   number
        enableSplitViewResizing: false,
        readOnly: false, //是否只读  取值 true | false
        fontSize:18
      });
      }
      
      this.monacoEditor.onDidChangeModelContent(() => {
        this.$emit('change', this.monacoEditor.getValue());
      });
    },
    apply(){
      if(this.validFlag){
         this.$emit("changeDataBase", { database: JSON.parse(JSON.stringify( this.database )), data: this.data});
      }else{
        this.$message("请先刷新数据");
      }
    },
    refresh(){
      let val = this.monacoEditor.getValue();
      if(val == "" ){
        this.$message("请输入自定义sql语句");
      }
      else if(!this.checkSqlInj(val)){
        this.database.executeSql = val;
        this.database.sqlType = 'custom';
        try {
          chartBIanalysis(this.database).then(response => {
            this.data = response.data;
            this.validFlag = true;
          });
          
        } catch (error) {
          console.log(error)
        }
      }
    },
    checkSqlInj(testInput) {
        let sqls = ["update" ,"insert" ,"delete","drop"];
        let invalid = false;
        let chkInput = (testInput + "").toLowerCase();
        let pos = -1;
        for (let i = 0, n = sqls .length; i < n; i++) {
          pos = chkInput.indexOf(sqls [i]);
          if (pos != -1) {
            this.$message("输入错误：含有非法字符\"" + testInput.substr(pos, sqls [i].length) + "\"!");
            invalid = true;
            break;
          }
        }
        return invalid;

    }
  }
}
</script>

<style scoped>
@media screen and (min-width: 960px) {
  .editorSet {
    width: 100%;
  }
  .editorSet .editor {
    height: 230px;
    width: 48%;
    overflow-y: hidden;
    background-color: rgb(44, 52, 55);
    float: left;
  }
}

@media screen and (max-width: 959px) {
  .editorSet {
    width: 100%;
  }
  .editorSet .editor {
    height: 230px;
    width: 100%;
    overflow-y: hidden;
    background-color: rgb(44, 52, 55);
  }
}

.NightTheme .editorSet .editor {
  -webkit-box-shadow: inset 0 1px 3px rgb(22, 26, 27);
  -moz-box-shadow: inset 0 1px 3px rgb(22, 26, 27);
  box-shadow: inset 0 1px 3px rgb(22, 26, 27);
}
.label {
  background: rgba(230, 230, 230, 0.5);
  height: 20px;
  padding: 0 6px;
  line-height: 20px;
  z-index: 999;
  text-align: center;
  font-size: 12px;
  color: #bbb;
  border-radius: 3px;
}

.editor iframe {
  border: 0 !important;
  min-height: 100px;
  min-width: 100px;
  height: 100%;
  width: 100%;
}

.edit-area {
  height: 100%;
}
</style>