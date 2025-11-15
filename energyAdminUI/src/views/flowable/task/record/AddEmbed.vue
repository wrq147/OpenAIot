<template>
  <div v-if="templateId != null">
    <slot></slot>
    <div class="test-form">
      <form-render ref="form" :forms="formConf" v-model="formValues" style="flex:1;" />

      <div class="xx-right">


        <div class="userblock">
          <div class="bk-title">审批流程</div>
          <div class="block">
            <el-empty v-if="nodelist.length == 0" description="暂无流程记录"></el-empty>
            <el-timeline v-else style="padding: 0">
              <el-timeline-item v-for="item in nodelist" :key="item.Id" :icon="item.Type == 'Approval' ? 'el-icon-user' : 'el-icon-position'
                " size="large">
                <div class="user-title">
                  <span>{{ item.Name }}</span>
                  <el-tag v-if="item.Tip != ''" size="small">{{
                    item.Tip
                  }}</el-tag>
                </div>
                <div class="user-cont">
                  <div class="user-item" v-for="itemv in item.Value" :key="itemv.Id">
                    <el-avatar shape="square" :size="53" fit="cover" :src="itemv.Avatar"></el-avatar>
                    <span style="line-height: 28px; font-size: 12px">{{
                      itemv.RealName
                    }}</span>
                  </div>
                  <div class="user-add" v-if="item.CanAdd" @click="selectUser(item)">
                    <i class="el-icon-plus"></i>
                  </div>
                </div>
              </el-timeline-item>
            </el-timeline>
          </div>
        </div>
      </div>
    </div>


    <org-picker multiple ref="orgPicker" @ok="selected" />
  </div>
</template>

<script>
import FormRender from "../../common/form/FormRender";
import { getFormDetail } from "@/api/flowable/design";
import { flowRootRecord, deployStart, getQuery } from "@/api/flowable/process";
import OrgPicker from "../../common/OrgPicker.vue";
import "../../common/utlity.js";
export default {
  name: "AddEmbed",
  components: {
    OrgPicker,
    FormRender,
  },
  data() {
    return {
      formName: "",
      formConf: [], // 默认表单数据
      formValues: {},
      templateId: null,
      nodelist: [],
      assign: {},
      flowId: 0,
      refreshNode: true,
      curassignId: null
    };
  },
  watch: {
    formValues: {
      handler: function () {
        this.refreshNode = true;
      },
      deep: true,
    },
  },

  mounted() { },
  methods: {
    async InitData(procDefId, fromParams, fromVal,defaultFromval) {
      // 初始化表单
      this.templateId = procDefId;
      if (fromVal != null) {
        let sdddx = await getQuery({ Name: "@from", Value: fromVal });
        if (sdddx.data.length > 0) {
          this.flowId = sdddx.data[0].FlowId;
        }
      }
      if (this.templateId != null) {
        let rsp = await getFormDetail(this.templateId);
        this.formName = rsp.data.Form.FormName;
        let rootNode = JSON.parse(rsp.data.FlowJson);
        let commitOperates = rootNode.props.formPerms.toMap("id");
        let jsondata = JSON.parse(rsp.data.Form.FormFields);

        let valuesModel = Object.assign({}, fromParams);
        this.formConf = jsondata.filter((it) => {
          let opval = commitOperates.get(it.id);
          if (opval != null) {
            if (opval.perm == "H") {
              return false;
            } else if (opval.perm == "R") {
              it.props.disabled = true;
              return true;
            }
          }
          valuesModel[it.id.toString()] = null;
          return true;
        });
        this.formValues = valuesModel;
        if(defaultFromval){
          this.formValues={...valuesModel,...defaultFromval}
        }
        if (this.flowId != null && this.flowId > 0) {
          // 初始化表单
          let xrsp = await flowRootRecord(this.flowId);
          this.formValues = xrsp.data.Model;
          this.assign = xrsp.data.Assign;
        }

        this.prebuild();
      }
    },
    prebuild() {
      if (this.refreshNode) {
        this.refreshNode = false;
        deployStart({
          templateId: this.templateId,
          model: this.formValues,
          state: 0,
          assign: this.assign,
          isEmbed: true
        }).then((res) => {
          res.data.forEach((element) => {
            if (this.assign[element.Id] != null) {
              element.Value.concat(this.assign[element.Id]);
            }
          });
          this.nodelist = res.data;
        });
      }
      setTimeout(this.prebuild, 2000);
    },
    getModel() {
      return { "model": this.formValues, "assign": this.assign, "flowId": this.flowId };
    },
    /** 申请流程表单数据提交 */
    async submitForm(st) {
      if (this.templateId == null) return 0;
      let rs = await deployStart({
        templateId: this.templateId,
        model: this.formValues,
        assign: this.assign,
        state: st,
        flowId: this.flowId,
        isEmbed: true
      });
      return rs.data;
    },
    selectUser(item) {
      this.curassignId = item.Id;
      let uitems = item.Value.filter((x) => x.IsNew != null);
      let slect = uitems.map((x) => {
        return {
          avatar: x.Avatar,
          id: x.Id,
          name: x.RealName,
          selected: false,
          type: "user",
        };
      });
      this.$refs.orgPicker.show(slect, "user");
    },
    selected(select) {
      let curnode = this.nodelist.find((x) => x.Id == this.curassignId);
      let oldarr = curnode.Value.filter((x) => x.IsNew == null);
      let newarr = [];
      select.forEach((x) => {
        if (oldarr.some((i) => i.Id == x.id)) {
          return;
        }
        newarr.push({
          Id: x.id,
          RealName: x.name,
          Avatar: x.avatar,
          IsNew: true,
        });
      });
      curnode.Value = oldarr.concat(newarr);
      this.assign[this.curassignId] = newarr;
    },
  },
};
</script>
<style lang="scss" scoped>
@import "~@/assets/styles/element-variables.scss";


.test-form {
  padding: 15px 15px 10px 15px;
  display: flex;
  justify-content: space-between;

  .xx-right {
    margin-left: 30px;
    width: 330px;
    padding-left: 20px;
    border-left: solid 1px #dadada;
  }
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

.el-timeline-item {
  padding-bottom: 0px !important;
}

.userblock {
  padding: 0 25px;

  .bk-title {
    padding-bottom: 20px;
    padding-top: 20px;
  }

  .user-title {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    height: 24px;
  }

  .user-cont {
    padding-top: 10px;
    display: flex;

    .user-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      margin-right: 15px;
    }

    .user-add {
      display: flex;
      height: 53px;
      width: 53px;
      justify-content: center;
      align-items: center;
      border: dashed 1px $--color-primary;
      color: $--color-primary;
      font-size: 35px;
      border-radius: 5px;
      cursor: pointer;
    }
  }
}
</style>