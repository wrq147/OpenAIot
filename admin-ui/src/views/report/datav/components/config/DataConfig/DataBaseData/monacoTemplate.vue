<template>
  <div class="editor" style="border: 1px solid #ccc">
    <div v-show="sourseItem" id="code" style="height: 300px;" class="edit-area"></div>
    <div v-show="!sourseItem"
      style="height:300px;display: flex; align-items: center; justify-content: center;color: red; font-size: 18px;">
      请先选择数据源!!!</div>
  </div>
</template>
<script>
import * as monaco from 'monaco-editor/esm/vs/editor/edcore.main'
import 'monaco-editor/esm/vs/basic-languages/sql/sql.contribution'
import { format } from 'sql-formatter'
import { MySQL } from './sqlparser';
import SqlKeywords from './SqlKeywords.js'
const mysqlInstance = new MySQL();
export default {
  props: {
    database: {
      type: String
    },
    sourseItem: {
      type: String
    }
  },
  data() {
    return {
      editor: null,
      color: null,
      suggestion: null,
      formatProvider: null,
      databaseData: {
        a: ['group', 'area'],
        b: ['user', 'client']
      },
      tableData: {
        user: ['age', 'gender'],
        group: ['id', 'name']
      }
    }
  },
  watch: {
    databaseData: function (newVal, oldVal) {
      this.formatting()
    },
    tableData: function (newVal, oldVal) {
      this.formatting()
    }
  },
  mounted() {
    this.initEditor()
  },
  methods: {
    initEditor() {
      this.editor = monaco.editor.create(document.getElementById('code'), {
        //初始化配置
        value: '',
        theme: 'vs',
        autoIndex: true,
        language: 'sql', // 语言类型
        tabCompletion: 'on',
        cursorSmoothCaretAnimation: true,
        formatOnPaste: true,
        mouseWheelZoom: true,
        folding: true, //代码折叠
        autoClosingBrackets: 'always',
        autoClosingOvertype: 'always',
        autoClosingQuotes: 'always',
        automaticLayout: 'always'
      })
      this.information()
      this.initColor()
      this.formatting()
      this.editor.onDidChangeModelContent(() => {
        // console.log('value', this.editor.getValue())
      })
    },
    // 插入文本信息
    insertTextAtCursor(text) {
      const position = this.editor.getPosition();
      this.editor.executeEdits('', [{
        range: new monaco.Range(position.lineNumber, position.column, position.lineNumber, position.column),
        text: '${' + text + '}',
        forceMoveMarkers: true
      }]);
    },
    // 表名
    getTableSuggest(dbName) {
      const tableNames = this.databaseData[dbName]
      if (!tableNames) {
        return []
      }
      return tableNames.map((name) => ({
        label: name,
        kind: monaco.languages.CompletionItemKind.Constant,
        insertText: name,
        detail: dbName
      }))
    },
    // 字段名
    getParamSuggest(tableName) {
      const params = this.tableData[tableName]
      if (!params) {
        return []
      }
      return params.map((item) => ({
        label: item.Name,
        kind: monaco.languages.CompletionItemKind.Constant,
        insertText: item.Name,
        detail: "(" + item.DataType + ")" + item.Comment
      }))
    },
    // 数据库名
    getDBSuggest() {
      return Object.keys(this.databaseData).map((key) => ({
        label: key,
        kind: monaco.languages.CompletionItemKind.Enum,
        insertText: key,
        detail: 'database'
      }))
    },
    information() {
      // 自动补全提示
      this.suggestion = monaco.languages.registerCompletionItemProvider('sql', {
        // 触发条件
        triggerCharacters: ['.', ' '],
        provideCompletionItems: (model, position) => {
          let suggestions = []
          let tsql = this.editor.getValue().toLowerCase();
          let newpos = { column: position.column, lineNumber: position.lineNumber };
          let matches = tsql.match(/\s+top\s+\d+/);
          if (matches != null && matches.length > 0) {
            let endidx = matches.index + matches[0].length;
            if (newpos.column > endidx) {
              tsql = tsql.substr(0, matches.index) + tsql.substring(endidx)
              newpos.column = newpos.column - matches[0].length;
            }
          }
          const syntaxSuggestions = mysqlInstance.getSuggestionAtCaretPosition(tsql, newpos);
          if (syntaxSuggestions != null) {
            let sugarr = syntaxSuggestions.keywords.map((key) => ({
              label: key,
              kind: monaco.languages.CompletionItemKind.Keyword,
              insertText: key,
              detail: 'keyword'
            }));
            suggestions = [...sugarr];
            syntaxSuggestions.syntax.forEach(x => {
              if (x.syntaxContextType == "table") {
                suggestions = [...this.getTableSuggest(this.database), ...suggestions];
              }
              else if (x.syntaxContextType == "column") {
                let machass = "";
                if (tsql.endsWith('.')) {
                  tsql = tsql.substr(0, tsql.length - 1);
                  if (x.wordRanges.length > 0 && x.wordRanges[0].text != '.') {
                    machass = x.wordRanges[0].text;
                  }
                }
                let tmpaall = mysqlInstance.getAllEntities(tsql, newpos);
                let tmptb = tmpaall.filter(x => x.entityContextType == "table");
                for (let i = 0; i < tmptb.length; i++) {
                  let tbname = tmptb[i].text;
                  if (machass != "") {
                    let testreg = new RegExp(tbname + '\\s+as\\s+' + machass, 'g');
                    let testreg2 = new RegExp(tbname + '\\s+' + machass, 'g');
                    if (testreg.test(tsql) || testreg2.test(tsql)) {
                      suggestions = [...this.getParamSuggest(tbname)];
                      break;
                    }
                  }
                  else {
                    suggestions = [...this.getParamSuggest(tbname), ...suggestions];
                  }
                }
              }
            });
          }

          return {
            suggestions,
          }
        }
      })
    },
    initColor() {
      //自定义文本颜色,也可以不设置，自带也有颜色区分
      let reg = '/'
      let i=0;
      SqlKeywords.forEach((keyword) => {
        if(i==0){
          reg += `\\b${keyword}\\b`;
        }
        else{
          reg += `|\\b${keyword}\\b`;
        }
        ++i;
      })
      reg += '/g'
      this.color = monaco.languages.setMonarchTokensProvider('sql', {
        ignoreCase: true,
        tokenizer: {
          root: [
            [
              reg,
              { token: 'keyword' },
            ], //蓝色
            [
              /[+]|[-]|[*]|[/]|[%]|[>]|[<]|[=]|[!]|[:]|[&&]|[||]/,
              { token: 'string' },
            ], //红色
            [/'.*?'|".*?"/, { token: 'string.escape' }], //橙色
            [/#--.*?\--#/, { token: 'comment' }], //绿色
            [/\\bnull\\b/, { token: 'regexp' }], //粉色
            [/[{]|[}]/, { token: 'type' }], //青色
            [/[\u4e00-\u9fa5]/, { token: 'predefined' }],//亮粉色
            [/''/, { token: 'invalid' }],//红色
            [/[\u4e00-\u9fa5]/, { token: 'number.binary' }],//浅绿
            [/(?!.*[a-zA-Z])[0-9]/, { token: 'number.hex' }], //浅绿
            [/[(]|[)]/, { token: 'number.octal' }], //浅绿
            [/[\u4e00-\u9fa5]/, { token: 'number.float' }],//浅绿
          ]
        }
      })
    },
    // 父组件获取值
    getValue() {
      return this.editor.getValue()
    },
    // 父组件设置值
    setValue(content) {
      this.editor.setValue(content)
    },
    // 格式化
    formatting() {
      const self = this
      this.formatProvider = monaco.languages.registerDocumentFormattingEditProvider('sql', {
        provideDocumentFormattingEdits(model) {
          return [{
            text: self.formatSql(1),
            range: model.getFullModelRange()
          }]
        }
      })
    },
    // 格式化代码
    formatSql(needValue) {
      this.clearMistake()
      try {
        this.setValue(format((this.editor).getValue()))
      } catch (e) {
        const { message } = e
        const list = message.split(' ')
        const line = list.indexOf('line')
        const column = list.indexOf('column')
        this.markMistake({
          startLineNumber: Number(list[line + 1]),
          endLineNumber: Number(list[line + 1]),
          startColumn: Number(list[column + 1]),
          endColumn: Number(list[column + 1])
        }, 'Error', message)
      }
      if (needValue) {
        return this.editor.getValue()
      }
    },
    // 标记错误信息
    markMistake(range, type, message) {
      const { startLineNumber, endLineNumber, startColumn, endColumn } = range
      monaco.editor.setModelMarkers(
        this.editor.getModel(),
        'eslint',
        [{
          startLineNumber,
          endLineNumber,
          startColumn,
          endColumn,
          severity: monaco.MarkerSeverity[type], // type可以是Error,Warning,Info
          message
        }]
      )
    },
    // 清除错误信息
    clearMistake() {
      monaco.editor.setModelMarkers(
        this.editor.getModel(),
        'eslint',
        []
      )
    }
  },
  beforeDestroy() {
    if (this.editor) {
      this.clearMistake()
      this.editor.dispose()
      this.color.dispose()
      this.suggestion.dispose()
      this.formatProvider.dispose()
    }
  }
}
</script>