<template>
  <el-dialog
    :close-on-click-modal="false"
    :title="formName"
    :visible.sync="open"
    width="1100px"
  >
    <div class="action-form">
      <div class="xx-form">
          <form-render ref="form" :optionInit="optionInit" :forms="formConf" v-model="formValues"  style="width:710px;" />
          <div class="xx-right">
            <div class="form-sign" v-if="needSign">
              <div class="sign-item" @click="handleSign">
                <template v-if="signBase != ''">
                  <el-image
                    :src="signBase"
                    style="width: 280px; height: 120px"
                    lazy
                  ></el-image>
                </template>
                <template v-else>
                  <i class="el-icon-edit" style="margin-right: 5px"></i>添加签名
                </template>
              </div>
            </div>
            <!--流程流转记录-->
            <div>
              <div class="clearfix" style="font-size: 16px; padding-top: 20px; padding-bottom: 20px;color:#333333;">
                <span>流程记录</span>
              </div>
              <div class="block">
                <el-empty v-if="node_list.length==0" description="暂无流程记录"></el-empty>
                <node-info-tree v-else :nodeList="node_list"></node-info-tree>
              </div>
            </div>
          </div>
      </div>
     
      <div class="form-footer">
        <el-button
        style="border-radius:20px"
          v-for="op in options"
          :key="op.action"
          icon="el-icon-edit-outline"
          :type="op.type"
          size="medium"
          :loading="formloading"
          @click="onHandle(op.action)"
          >{{ op.action }}</el-button
        >
      </div>

      <sign-picker ref="signRef" @ok="signOk"></sign-picker>
    </div>
  </el-dialog>
</template>

<script>
import NodeInfoTree from "./NodeInfoTree.vue"
import SignPicker from "../../common/SignPicker";
import FormRender from "../../common/form/FormRender";
import { flowRecord, excuteTask } from "@/api/flowable/process";
import { getQuery } from "@/api/flowable/process";
import "../../common/utlity.js";
export default {
  name: "RecordForm",
  components: {
    NodeInfoTree,
    FormRender,
    SignPicker,
  },
  props: {},
  data() {
    return {
      formName: "",
      formConf: [], // 默认表单数据
      formValues: {},
      formloading: false,
      flowId: null,
      nodeId: null,
      node_list: [],
      open: false,
      options: [],
      signBase: "",
      needSign: false,
      optionInit:[]
    };
  },
  methods: {
    handleSign() {
      this.$refs.signRef.show();
    },
    filterPerm(formItems,commitOperates){
      return formItems.filter((it) => {
        if (it.name === "SpanLayout") {
          it.items=this.filterPerm(it.props.items,commitOperates);
          it.props.disabled = false;
          return it.items.length>0;
        }
        else{
          let opval = commitOperates.get(it.id);
          if (opval != null) {
            if (opval.perm == "H") {
              return false;
            } else if (opval.perm == "R") {
              it.props.disabled = true;
              return true;
            }
          }
          return true;
        }
      });
    },
    async initForm() {
      this.signBase = "";
      this.needSign = false;
      // 初始化表单
      let rsp = await flowRecord(this.nodeId);
      this.options = rsp.data.Step.Options;
      let commitOperates;
      if(rsp.data.Step.FormPerms!=null){
        commitOperates = rsp.data.Step.FormPerms.toMap("id");
      }
      else{
        commitOperates = new Map();
      }

      this.optionInit = rsp.data.Step.optionInit
      let jsondata = rsp.data.NodeField;
      //初始化参数
      let queryrsp= await getQuery({FlowId:rsp.data.FlowId});
      let tmpqueryObj={};
      queryrsp.data.forEach(x=>{
        tmpqueryObj[x.Name]=x.Value;
      })
      this.formValues= Object.assign(rsp.data.Model, tmpqueryObj);
      this.formName = rsp.data.FormName;
      this.flowId = rsp.data.FlowId;

      //判断是否需要签名
      if (rsp.data.Step.Type == "FlowService.FlowNode.Builder.ApprovalTask") {
        this.needSign = rsp.data.Step.Sign;
      }

      this.node_list = rsp.data.NodeList;
      this.$set(this,"formConf",this.filterPerm(jsondata,commitOperates));
      console.info(this.formConf)
    },
    openDialog(nodeId) {
      this.nodeId = nodeId;
      this.open = true;
      this.initForm();
    },
    signOk(res) {
      this.signBase = res;
    },
    /** 执行动作 */
    onHandle(ac) {
      this.$refs.form.validate(valid => {
        if(valid){
          this.formloading = true;
          let subdatas = {};
          this.formConf.forEach((it) => {
            subdatas[it.id] = it.value;
          });
          if (this.needSign) {
            this.formValues["$ApprovalSign"] = this.signBase;
          }
      
          excuteTask({
            flowId: this.flowId,
            model: this.formValues,
            key: this.nodeId,
            action: ac,
          })
            .then((res) => {
              this.$modal.msgSuccess("操作成功");
              setTimeout(() => {
                this.open = false;
                this.$emit("finish");
              }, 2000);
            })
            .finally(() => {
              this.formloading = false;
            });
        }
      },ac)
    },
  },
};
</script>
<style lang="scss" scoped>
.action-form {
  min-height: 300px;
  display: flex;
  flex-direction: column;
  .xx-form{
    display: flex;
    justify-content: space-between;
    .xx-right{
      width:330px;
      padding-left: 20px;
      border-left: solid 1px #dadada;
    }
  }
}
.form-footer {
  padding-top: 10px;
  text-align: right;
}

.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}

.clearfix:after {
  clear: both;
}

.box-card {
  width: 100%;
  margin-bottom: 20px;
}

.el-tag + .el-tag {
  margin-left: 10px;
}
.form-sign {
  padding: 20px 0 20px 0;
  .sign-item {
    border: solid 1px #dadada;
    display: flex;
    justify-content: center;
    align-items: center;
    height: 120px;
    width: 100%;
    cursor: pointer;
  }
}
</style>
