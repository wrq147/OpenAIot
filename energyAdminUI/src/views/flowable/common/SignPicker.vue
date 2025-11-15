<template>
  <el-dialog
    :title="formName"
    :visible.sync="dialogVisible"
    width="600px"
    v-loading="loading"
     :close-on-click-modal="false" 
         :append-to-body="true"
  >
    <vue-esign
      ref="esign"
      class="mySign"
      :width="560"
      :height="240"
      :lineWidth="6"
      lineColor="#000000"
    />
    <span slot="footer" class="dialog-footer">
      <el-button @click="handleGenerate" type="primary">确认</el-button>
      <el-button @click="handleReset">清空</el-button>
      <el-button @click="dialogVisible = false">取消</el-button>
    </span>
  </el-dialog>
</template>

<script>
import vueEsign from "vue-esign";
export default {
  name: "SignPicker",
  components: {
    vueEsign,
  },
  props: {},
  data() {
    return {
      loading: false,
      formName: "请在虚线内签名",
      dialogVisible: false,
    };
  },
  methods: {
    show() {
      this.dialogVisible = true;
    },
    // 清空画板
    handleReset() {
      this.$refs.esign.reset();
    },
    handleGenerate() {
      this.loading = true;
      this.$refs.esign
        .generate() // 使用生成器调用把签字的图片转换成为base64图片格式
        .then((res) => {
          this.$emit("ok", res);
          this.dialogVisible = false;
        })
        .catch((err) => {
          // 画布没有签字时会执行这里提示一下
          this.$message({
            type: "warning",
            message: "您还没有签名",
          });
        })
        .finally(() => {
          this.loading = false;
        });
    },
  },
};
</script>
<style lang="scss" scoped>
.mySign {
  border: 1px dashed #000;
}
</style>
