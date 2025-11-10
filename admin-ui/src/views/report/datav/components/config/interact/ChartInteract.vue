<template>
  <div>
    <div v-for="item in interactData" :key="item.func">
      <div class="evt-title">
        <span style="font-size: 14px;color: #666;">{{ item.name }}</span>
        <el-button type="primary" icon="el-icon-edit" size="mini" @click="onOpenEdit(item)">编辑</el-button>
      </div>
      <div style="margin-bottom: 20px;">
        <el-input class="evtcls" type="textarea" :rows="4" placeholder="点击此处编辑" @focus="onScriptEdit($event, item)"
          :value="CodeStr(item)">
        </el-input>
      </div>
    </div>

    <el-dialog title="编辑事件脚本" :close-on-click-modal="false" :visible.sync="scriptEditorOpen" width="750px" top="2vh">
      <data-editor v-if="scriptEditorOpen" @submitData="submitData" @cancelData="scriptEditorOpen = false"
        :customData="CodeStr(editItem)"></data-editor>
    </el-dialog>
  </div>
</template>

<script>
import DataEditor from "../runcode/DataEditor";
export default {
  components: {
    DataEditor
  },
  props: {
    chartOption: {
      type: Object
    }
  },
  data() {
    return {
      scriptEditorOpen: false,
      editItem: null
    };
  },
  computed: {
    interactData() {
      if (this.chartOption == null) {
        return [];
      }
      else {
        return this.chartOption.interactData;
      }
    }
  },
  mounted() {
  },
  methods: {
    onOpenEdit(item) {
      this.editItem = item;
      this.scriptEditorOpen = true;
    },
    onScriptEdit(_this, item) {
      _this.target.blur();
      this.editItem = item;
      this.scriptEditorOpen = true;
    },
    CodeStr(item) {
      if (item == null||item.code == "") {
        return "function " + item.func + `(evt){
          return evt;
}`;
      }
      else {
        return item.code;
      }
    },
    submitData(data) {
      this.chartOption.interactData.forEach(element => {
        if(element.func == this.editItem.func){
          element.code = data;
        }
      });
      this.$emit("changeData",this.chartOption.interactData);
      this.scriptEditorOpen = false;
    },
  }
};
</script>

<style lang="scss">
.evtcls {
  textarea {
    cursor: pointer;
  }
}

.evt-title {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  height: 45px;
}
</style>
