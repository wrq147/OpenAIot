<template>
  <el-container>
    <el-header style="background: white;height: 50px;line-height: 50px;border-top: 1px solid #dadada;">
      <layout-header
        v-model="activeSelect"
        @publish="publishProcess"
        @preview="preview"
      ></layout-header>
    </el-header>
    <div class="layout-body">
      <form-base-setting
      :finishLoad="finishLoad"
        ref="baseSetting"
        v-show="activeSelect === 'baseSetting'"
      />
      <form-design ref="formSetting" v-show="activeSelect === 'formSetting'" />
      <process-design
        ref="processDesign"
        v-show="activeSelect === 'processDesign'"
      />
      <form-pro-setting
        ref="proSetting"
        v-show="activeSelect === 'proSetting'"
      />
    </div>
    <w-dialog :showFooter="false" v-model="validVisible" title="设置项检查">
      <el-steps align-center :active="validStep" finish-status="success">
        <el-step
          v-for="(step, i) in validOptions"
          :title="step.title"
          :key="i"
          :icon="step.icon"
          :status="step.status"
          :description="step.description"
        />
      </el-steps>
      <el-result
        :icon="validIcon"
        :title="errTitle"
        :subTitle="validResult.desc"
      >
        <i
          slot="icon"
          style="font-size: 30px"
          v-if="!validResult.finished"
          class="el-icon-loading"
        ></i>
        <div
          slot="subTitle"
          class="err-info"
          v-if="validResult.errs.length > 0"
        >
          <ellipsis
            hover-tip
            v-for="(err, i) in validResult.errs"
            :key="i + '_err'"
            :content="err"
          >
            <i slot="pre" class="el-icon-warning-outline"></i>
          </ellipsis>
        </div>
        <template slot="extra">
          <el-button
            type="primary"
            v-if="validResult.finished"
            size="medium"
            @click="doAfter"
          >
            {{ validResult.action }}
          </el-button>
        </template>
      </el-result>
    </w-dialog>
  </el-container>
</template>

<script>
import LayoutHeader from "./LayoutHeader";
import { getFormDetail, updateForm } from "@/api/flowable/design";
import WDialog from "../common/WDialog.vue";
import FormBaseSetting from "./layout/FormBaseSetting";
import FormDesign from "./layout/FormDesign";
import ProcessDesign from "./layout/ProcessDesign";
import FormProSetting from "./layout/FormProSetting";
import Ellipsis from "../common/Ellipsis.vue";

export default {
  name: "FormProcessDesign",
  components: {
    WDialog,
    LayoutHeader,
    FormBaseSetting,
    FormDesign,
    ProcessDesign,
    FormProSetting,
    Ellipsis,
  },
  data() {
    return {
      finishLoad:false,//流程数据是否完成加载
      isNew: true,
      validStep: 0,
      timer: null,
      activeSelect: "baseSetting",
      validVisible: false,
      validResult: {},
      validOptions: [
        { title: "基础信息", description: "", icon: "", status: "" },
        { title: "审批表单", description: "", icon: "", status: "" },
        { title: "审批流程", description: "", icon: "", status: "" },
        { title: "扩展设置", description: "", icon: "", status: "" },
      ],
      validComponents: [
        "baseSetting",
        "formSetting",
        "processDesign",
        "proSetting",
      ],
    };
  },
  computed: {
    setup() {
      return this.$store.state.flowable.design;
    },
    errTitle() {
      if (this.validResult.finished && !this.validResult.success) {
        return (
          this.validResult.title + ` (${this.validResult.errs.length}项错误) 😥`
        );
      }
      return this.validResult.title;
    },
    validIcon() {
      if (!this.validResult.finished) {
        return "el-icon-loading";
      } else if (this.validResult.success) {
        return "success";
      } else {
        return "warning";
      }
    },
  },
  created() {
    this.showValiding();
    let formId = this.$route.query.code;
    //判断传参，决定是新建还是加载原始数据
    this.loadInitFrom();
    if (this.$isNotEmpty(formId)) {
      this.isNew = false;
      this.loadFormInfo(formId);
    }
    let group = this.$route.query.group;
    this.setup.GroupId = this.$isNotEmpty(group) ? parseInt(group) : null;
  },
  beforeDestroy() {
    this.stopTimer();
  },
  methods: {
    loadFormInfo(formId) {
      getFormDetail(formId)
        .then((rsp) => {
          let form = rsp.data;
          form.formItems = JSON.parse(form.Form.FormFields);
          
          form.process = JSON.parse(form.FlowJson);
          form.notify = JSON.parse(form.notify);
          this.$store.commit("loadForm", form);
          this.finishLoad=true
        })
        .catch((err) => {
          this.$message.error(err);
        });
    },
    loadInitFrom() {
      this.$store.commit("loadForm", {
        Id: null,
        Name: "未命名流程",
        Icon: "el-icon-s-custom",
        Background: "#FF7800",
        sign: false,
        sublimit:0,
        startlimit:false,
        notify: {
          types: ["APP"],
          title: "消息通知标题",
        },
        GroupId: undefined,
        formItems: [],
        process: {
          id: "root",
          parentId: null,
          type: "ROOT",
          name: "发起人",
          desc: "任何人",
          props: {
            assignedUser: [],
            formPerms: [],
          },
          children: {},
        },
        remark: "备注说明",
      });
    },
    validateDesign() {
      this.validVisible = true;
      this.validStep = 0;
      this.showValiding();
      this.stopTimer();
      this.timer = setInterval(() => {
        this.validResult.errs =
          this.$refs[this.validComponents[this.validStep]].validate();
        if (
          Array.isArray(this.validResult.errs) &&
          this.validResult.errs.length === 0
        ) {
          this.validStep++;
          if (this.validStep >= this.validOptions.length) {
            this.stopTimer();
            this.showValidFinish(true);
          }
        } else {
          this.stopTimer();
          this.validOptions[this.validStep].status = "error";
          this.showValidFinish(false, this.getDefaultValidErr());
        }
      }, 300);
    },
    getDefaultValidErr() {
      switch (this.validStep) {
        case 0:
          return "请检查基础设置项";
        case 1:
          return "请检查审批表单相关设置";
        case 2:
          return "请检查审批流程，查看对应标注节点错误信息";
        case 3:
          return "请检查扩展设置";
        default:
          return "未知错误";
      }
    },
    showValidFinish(success, err) {
      this.validResult.success = success;
      this.validResult.finished = true;
      this.validResult.title = success ? "校验完成 😀" : "校验失败 ";
      this.validResult.desc = success ? "设置项校验成功，是否提交？" : err;
      this.validResult.action = success ? "提 交" : "去修改";
    },
    showValiding() {
      this.validResult = {
        errs: [],
        finished: false,
        success: false,
        title: "检查中...",
        action: "处理",
        desc: "正在检查设置项",
      };
      this.validStep = 0;
      this.validOptions.forEach((op) => {
        op.status = "";
        op.icon = "";
        op.description = "";
      });
    },
    doAfter() {
      if (this.validResult.success) {
        this.doPublish();
      } else {
        this.activeSelect = this.validComponents[this.validStep];
        this.validVisible = false;
      }
    },
    stopTimer() {
      if (this.timer) {
        clearInterval(this.timer);
      }
    },
    preview() {
      
    },
    publishProcess() {
      this.validateDesign();
    },
    doPublish() {
      this.$confirm(
        "如果您只想预览请选择预览，确认发布后流程立即生效，是否继续?",
        "提示",
        {
          confirmButtonText: "发布",
          cancelButtonText: "取消",
          type: "warning",
        }
      ).then(() => {
        let template = {
          Id: this.setup.Id,
          Name: this.setup.Name,
          Icon: this.setup.Icon,
          Background: this.setup.Background,
          sign: this.setup.sign,
          sublimit:this.setup.sublimit,
          startlimit:this.setup.startlimit,
          notify: JSON.stringify(this.setup.notify),
          GroupId: this.setup.GroupId,
          Form: {
            FormName: this.setup.Name,
            FormFields: JSON.stringify(this.setup.formItems),
          },
          FlowJson: JSON.stringify(this.setup.process),
          remark: this.setup.remark,
        };
        if (this.isNew || !this.$isNotEmpty(this.setup.Id)) {
          updateForm(template)
            .then((rsp) => {
              this.$message.success("创建表单成功");
              this.$store.dispatch("tagsView/delView", this.$route);
              this.$router.push("/flowable/FormsPanel");
            })
            .catch((err) => {
              this.$message.error(err);
            });
        } else {
          updateForm(template)
            .then((rsp) => {
              this.$message.success("更新表单成功");
              this.$store.dispatch("tagsView/delView", this.$route);
              this.$router.push("/flowable/FormsPanel");
            })
            .catch((err) => {
              this.$message.error(err);
            });
        }
      });
    },
  },
};
</script>

<style lang="less" scoped>
.layout-body {
  min-width: 980px;
  background-color: #f5f6f6;
  height: calc(100vh - 110px);
  overflow: auto;
}
.hasTagsView {
  .layout-body {
    height: calc(100vh - 156px);
  }
}

/deep/ .el-step {
  .is-success {
    color: #2a99ff;
    border-color: #2a99ff;
  }
}

.err-info {
  max-height: 180px;
  overflow-y: auto;

  & > div {
    padding: 5px;
    margin: 2px 0;
    width: 220px;
    text-align: left;
    border-radius: 3px;
    background: rgb(242 242 242);
  }

  i {
    margin: 0 5px;
  }
}

::-webkit-scrollbar {
  width: 2px;
  height: 2px;
  background-color: white;
}

::-webkit-scrollbar-thumb {
  border-radius: 16px;
  background-color: #e8e8e8;
}
</style>
