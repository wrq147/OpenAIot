<template>
  <div>
    <el-dialog title="拷贝" :visible.sync="dialogVisible" width="650px" :rules="codeRules">
      <el-form ref="form" :model="form" label-width="80px">
        <el-form-item label="字段名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入字段名称" />
        </el-form-item>
        <el-form-item label="标识符">
          <el-input v-model="form.code" placeholder="请输入标识符"
            @input="form.code = form.code.replace(/[^a-zA-Z0-9_]{1,20}$/g, '')" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="onOk">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
export default {
  name: "ModelCopyItem",
  data() {
    return {
      dialogVisible: false,
      form: {
        name: "",
        code: ""
      },
      codeRules: {
        name: [{ required: true, trigger: "blur", message: "请输入字段名称" }],
        code: [
          { required: true, trigger: "blur", message: "请输入标识符" },
          {
            min: 1,
            max: 20,
            message: "长度在 1 到 20 个字符",
            trigger: "blur"
          }
        ],
      }
    };
  },
  methods: {
    openDlg(row) {
      this.form = JSON.parse(JSON.stringify(row));
      this.form.name = "";
      this.form.code = "";
      this.dialogVisible = true;
    },
    onOk() {
      this.$emit("ok", this.form)
      this.dialogVisible = false;
    }
  }
};
</script>

<style lang="less"></style>