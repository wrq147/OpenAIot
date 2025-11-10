<template>
  <div class="senior-setup">
    <el-form label-position="top" label-width="80px">
      <el-form-item label="审批同意时是否签字">
        <el-switch
          inactive-text="无需签字"
          active-text="需要签字"
          v-model="setup.sign"
        ></el-switch>
        <div class="pro-tip">
          如果此处设置为 <b>需要签字</b>，则所有审批人“同意时” <b>必须签字</b>
        </div>
      </el-form-item>
      <el-form-item label="发起提交次数限制">
        <el-switch
          inactive-text="不限制"
          active-text="限制"
          v-model="limitEnable"
        ></el-switch>
        <el-input-number
          style="margin-left: 30px"
          v-if="limitEnable"
          v-model="setup.sublimit"
          :min="1"
          :max="20"
          label="限制次数"
        ></el-input-number>
      </el-form-item>
      <el-form-item label="是否禁止直接发起">
        <el-switch
          inactive-text="不禁止"
          active-text="禁止"
          v-model="setup.startlimit"
        ></el-switch>
        <div class="pro-tip">
          禁止用户从 <b>我的流程</b> 发起
        </div>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
export default {
  name: "FormProSetting",
  computed: {
    setup() {
      return this.$store.state.flowable.design;
    },
    limitEnable: {
      get: function () {
        return this.setup.sublimit > 0;
      },
      set: function (val) {
        if (val) {
           this.setup.sublimit = 1;
        }
        else{
            this.setup.sublimit = 0;
        }
      },
    },
  },
  data() {
    return {

    };
  },
  methods: {
    validate() {
      return [];
    },
  },
};
</script>

<style lang="less" scoped>
.senior-setup {
  overflow: auto;
  margin: 0 auto;
  width: 600px;
  background: #ffffff;
  margin-top: 10px;
  padding: 15px 20px;

}
.pro-tip {
    color: #949495;
    font-size: small;
}
.el-form-item__label {
  margin-left: 0;
}
</style>
