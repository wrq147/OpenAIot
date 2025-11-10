<template>
  <div>
    <div style="display: flex; flex-direction: column">
      <div id="main_container">
        <div id="core" class="core core_margin1">
          <div class="editorSet clearfix">
            <div>
              <div class="editor">
                <div class="c-tt">
                  <span class="label">javascript</span>
                  <el-button size="small" type="success" @click="format"
                    >格式化</el-button
                  >
                </div>

                <div ref="container" class="edit-area"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div style="margin-top: 30px" v-if="showBtn">
        <el-button type="primary" @click="editSubmit" style="float: right"
          >确 定</el-button
        >
        <el-button @click="cancel" style="float: right; margin-right: 20px"
          >取 消</el-button
        >
      </div>
    </div>
  </div>
</template>
<script>
import * as monaco from "monaco-editor";
export default {
  props: {
    showBtn: {
      type: Boolean,
      default: true,
    },
    customData: {
      type: String,
    },
  },
  components: {},
  data() {
    return {
      monacoEditor: null,
      data: '',
    };
  },
  watch: {
    customData: {
      immediate: true,
      deep: true,
      handler() {
          this.data = this.customData;
      },
    }
  },
  created() {},
  mounted() {
    monaco.languages.typescript.javascriptDefaults.addExtraLib(
      "",
      "customFileName"
    );

    this.monacoEditor = monaco.editor.create(this.$refs.container, {
      value: this.data != undefined ? this.data : "",
      language: "javascript",
      theme: "vs",
    });
  },
  methods: {
    setVal(val){
      //设置编辑器的值
      this.monacoEditor.setValue(val)
    },
    getVal() {
      return JSON.parse(JSON.stringify(this.monacoEditor.getValue()));
    },
    editSubmit() {
      this.$emit(
        "submitData",
        JSON.parse(JSON.stringify(this.monacoEditor.getValue()))
      );
    },
    cancel() {
      this.$emit("cancelData", "");
    },
    format() {
      this.monacoEditor.getAction("editor.action.formatDocument").run();
    },
  },
};
</script>
<style scoped>
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

.c-tt {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 10px;
  border-bottom: solid 1px #dadada;
}
.edit-area {
  min-height: 250px;
}
</style>
