<template>
  <div>
    <el-form size="small" label-width="90px">
      <el-form-item label="接口地址">
        <el-input
          type="textarea"
          :rows="5"
          placeholder="请输入接口地址"
          v-model="apiInterfaceUrl"
          @blur.prevent="changeInterfaceUrl()"
        >
        </el-input>
      </el-form-item>

      <el-form-item label="请求方式">
        <el-radio-group v-model="apiRequestMethod" @change="changeMethod()">
          <el-radio label="GET"></el-radio>
          <el-radio label="POST"></el-radio>
        </el-radio-group>
      </el-form-item>

      <el-form-item label="请求参数">
        <el-input
          type="textarea"
          :rows="5"
          placeholder="请输入请求参数"
          v-model="apiRequestParameters"
          @blur.prevent="changeRequestParameters()"
        >
        </el-input>
      </el-form-item>

      <el-form-item label="token令牌">
        <el-input
          placeholder="请输入token"
          v-model="apiToken"
          @blur.prevent="changeToken()"
        >
        </el-input>
      </el-form-item>

      <el-form-item label="刷新时间">
        <el-input-number
          v-model="apiTimeout"
          controls-position="right"
          :step="1000"
          @change="changeTimeout"
        ></el-input-number>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
export default {
  props: [
    "interfaceUrl",
    "requestMethod",
    "requestParameters",
    "token",
    "timeout"
  ],
  data() {
    return {
      apiInterfaceUrl: this.interfaceUrl, //接口地址
      apiRequestMethod: this.requestMethod, //请求方式
      apiRequestParameters: this.requestParameters, //请求参数
      apiToken: this.token, //token令牌
      apiTimeout: this.timeout //刷新时间
    };
  },
  watch: {
    interfaceUrl: {
      deep: true,
      handler(newVal) {
        this.apiInterfaceUrl = newVal;
      }
    },
    requestMethod: {
      deep: true,
      handler(newVal) {
        this.apiRequestMethod = newVal;
      }
    },
    requestParameters: {
      deep: true,
      handler(newVal) {
        this.apiRequestParameters = newVal;
      }
    },
    token: {
      deep: true,
      handler(newVal) {
        this.apiToken = newVal;
      }
    },
    timeout: {
      deep: true,
      handler(newVal) {
        this.apiTimeout = newVal;
      }
    }
  },
  mounted() {},
  methods: {
    changeInterfaceUrl() {
      this.$emit("changeApi", {
        type: "interfaceUrl",
        arr: this.apiInterfaceUrl
      });
    },
    changeRequestParameters() {
      this.$emit("changeApi", {
        type: "requestParameters",
        arr: this.apiRequestParameters
      });
    },
    changeMethod() {
      this.$emit("changeApi", {
        type: "requestMethod",
        arr: this.apiRequestMethod
      });
    },
    changeToken() {
      this.$emit("changeApi", { type: "token", arr: this.apiToken });
    },
    changeTimeout(currentValue, oldValue) {
      this.$emit("changeApi", { type: "timeout", arr: this.apiTimeout });
    }
  }
};
</script>

<style scoped></style>
