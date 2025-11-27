<template>
  <div>
    <el-dialog
      title="编辑数据集"
      :close-on-click-modal="false"
      :visible.sync="sourceOpenDis"
      width="1200px"
      top="2vh"
      append-to-body
      @close="closesource"
      class="apidata_dialog_con"
    >
      <div class="api_request_box">
        <div class="api_box_left">
          <apidataLeft ref="apidataLeft" @import="importApi"></apidataLeft>
        </div>
        <div class="api_box_right">
          <consoleApi :tableType="tableType" ref="consoleapi" :drawingList="drawingList" :curIdx="curIdx" @changeconfirmValue="changeconfirmValue" :costomData="configData"></consoleApi>
        </div>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="closesource">取 消</el-button>
        <el-button type="primary" @click="comfirmSource">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import apidataLeft from "./apidataLeft";
import consoleApi from "./consoleApi";
export default {
  components: {
    apidataLeft,
    consoleApi
  },
  props: ["drawingList", "dialogVisible", "curIdx","tableType","costomData"],
  data() {
    return {
      sourceOpenDis: false,
      configData:this.costomData
    };
  },
  computed: {
  },
  watch: {
    dialogVisible(newValue) {
      // console.log("弹窗样式", newValue);
      this.sourceOpenDis = newValue;
    },
    costomData:{
      handler(newval){
        this.configData=newval
      },
      immediate:true,
      deep:true
    },
  },
  mounted() {},
  methods: {
    closesource() {
      //关闭接口源
      this.$emit("closesource");
    },
    async getVal() {
      return await this.$refs.consoleapi.getVal();
    },
    async comfirmSource(){
      this.$emit('changeconfirmValue',false)
    },
    initCom(option) {
      setTimeout(()=>{
        this.$refs.consoleapi.initCom(option);
      },100)
    },
    changeconfirmValue(val){
        this.$emit('changeconfirmValue',val)
    },
    importApi(data) {
        this.$refs.consoleapi.importApi(data);
    },
  },
};
</script>

<style lang="scss" scoped>
/* jsoneditor右上角默认有一个链接,加css去掉了 */
::v-deep {
  .el-dialog__header {
    border-bottom: 1px solid #ccc;
  }
  .el-input-number {
    width: 100%;
  }
  .el-dialog__body {
    background-color: #f1f2f4;
  }
}
.api_request_box {
  display: flex;
  justify-content: space-between;
  width: 100%;
  .api_box_left {
    width: 360px;
    box-sizing: border-box;
  }
  .api_box_right {
    width: calc(100% - 380px);
    border-radius: 10px;
    background: #fff;
    padding: 10px;
    padding-top: 20px;
    box-sizing: border-box;
  }
}
div.jsoneditor-menu a.jsoneditor-poweredBy {
  display: none;
}
::v-deep {
  .el-input__icon {
    line-height: 28px;
  }
  .el-tabs__new-tab {
    background-color: #1682e6;
    margin-right: 18px;
    line-height: 16px;
  }
}
</style>
