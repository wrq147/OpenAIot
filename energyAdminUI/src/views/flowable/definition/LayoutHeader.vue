<template>
  <div>
    <div class="header">
      <el-menu
        :default-active="value"
        active-text-color="#409eff"
        class="el-menu-demo shejiqi"
        mode="horizontal"
        @select="handleSelect"
      >
        <el-menu-item index="baseSetting" @click="to('baseSetting')">① 基础信息</el-menu-item>
        <el-menu-item index="formSetting" @click="to('formSetting')">② 表单设计</el-menu-item>
        <el-menu-item index="processDesign" @click="to('processDesign')">③ 流程设计</el-menu-item>
        <el-menu-item index="proSetting" @click="to('proSetting')">④ 扩展设置</el-menu-item>
      </el-menu>
      <div class="publish">
        <el-button size="mini" type="primary" @click="publish">
          <i class="el-icon-s-promotion"></i>发布
        </el-button>
      </div>
      <div class="back">
        <el-button @click="exit" size="small" style="margin-right:10px;" icon="el-icon-back" circle></el-button>
        <span>
          <i :class="setup.Icon" :style="'background:' + setup.Background"></i>
          <span>{{ setup.Name }}</span>
        </span>
      </div>
    </div>


  </div>
</template>

<script>
export default {
  name: "LayoutHeader",
  props: {
    value: {
      type: String,
      default: "baseSetup"
    }
  },
  data() {
    return {
    };
  },
  computed: {
    setup() {
      return this.$store.state.flowable.design;
    }
  },
  created() {
    this.check();
  },
  mounted() {
    if (document.body.offsetWidth <= 970) {
      this.$msgbox.alert(
        "本设计器未适配中小屏幕，建议您在PC电脑端浏览器进行操作"
      );
    }
    this.listener();
  },
  methods: {
    publish() {
      this.$emit("publish");
    },
    valid() {
      if (!this.$isNotEmpty(this.setup.group)) {
        this.$message.warning("请选择分组");
        this.$router.push("/layout/baseSetup");
        return false;
      }
      return true;
    },
    exit() {
      this.$confirm("未发布的内容将不会被保存，是否直接退出 ?", "提示", {
        confirmButtonText: "退出",
        cancelButtonText: "取消",
        type: "warning"
      }).then(() => {
        this.$store.dispatch('tagsView/delView', this.$route);
        this.$router.go(-1);
      });
    },
    to(path) {
      this.$emit("input", path);
    },
    handleSelect(key, keyPath) {
      console.log(key, keyPath);
    },
    listener() {
      window.onunload = this.closeBefore();
      window.onbeforeunload = this.closeBefore();
      //window.on('beforeunload',this.closeBefore())
    },
    closeBefore() {
      //alert("您将要离开本页")
      return false;
    },
    check() {
      if (this.$store.state.isEdit === null) {
        //this.$router.push("/workPanel");
      }
    }
  }
};
</script>
<style lang="less" scoped>
/deep/ .header {
  min-width: 980px;
  // width: 100%;
  box-sizing: border-box;
  position: relative;
  display: flex;
  align-items: center;
  // justify-content: space-between;
  line-height: 50px;
  height: 50px;
  .el-menu {
    top: 0;
    z-index: 1;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
  }
  .shejiqi {
    height: 38px;
    line-height: 38px;
    border: none;
  }
  .shejiqi .el-menu-item {
    padding: 0;
    margin: 0 25px;
    height: 38px;
    line-height: 38px;
  }
  .publish {
    position: absolute;
    top: 0;
    right: 0;
    z-index: 2;

    i {
      margin-right: 6px;
    }

    button {
      border-radius: 15px;
    }
  }

  .back {
    position: absolute;
    z-index: 2;
    top: 0;
    left: 0;
    font-size: small;

    span {
      i {
        border-radius: 10px;
        padding: 7.8px;
        font-size: 20px;
        color: #ffffff;
        margin: 0 10px;
      }
    }
  }
}
</style>
