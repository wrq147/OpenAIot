<template>
  <div>
    <div v-if="mode === 'DESIGN'">
      <el-input size="medium" v-model="defaultValue" disabled :placeholder="placeholder" />
    </div>
    <div v-else>
      <el-input size="medium" :disabled="disabled" clearable v-model="_value" :placeholder="placeholder" />
    </div>
  </div>
</template>

<script>
import componentMinxins from "../ComponentMinxins";
import { GenerateNumber } from "@/api/flowable/process.js"
export default {
  mixins: [componentMinxins],
  name: "TextInput",
  components: {},
  props: {
    value: {
      type: String,
      default: null,
    },
    placeholder: {
      type: String,
      default: "请输入内容",
    },
    defaultValue: {
      type: String,
      default: "",
    },
    disabled: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {};
  },
  async mounted() {
    if (this.mode != 'DESIGN') {
      if (this.value == null) {
        if (this.defaultValue == "@FlowNumber") {
          //生成工单号
          let rsp = await GenerateNumber();
          this.$emit('input', rsp.data);
          this.valueModel["@FlowNumber"] = rsp.data;
        }
        else {
          this.$emit('input', this.defaultValue);
        }

      }
    }

  },
  methods: {},
};
</script>

<style scoped></style>
