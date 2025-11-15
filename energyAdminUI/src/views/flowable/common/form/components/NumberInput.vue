<template>
  <div>
    <div v-if="mode === 'DESIGN'">
      <el-input-number size="medium" disabled :placeholder="placeholder"></el-input-number>
    </div>
    <div v-else>
      <el-input-number style="width: 100%" :disabled="disabled" v-model="_value" size="medium" :placeholder="placeholder" :min="min" :max="max"></el-input-number>
    </div>
  </div>
</template>

<script>
import componentMinxins from '../ComponentMinxins'

export default {
  mixins: [componentMinxins],
  name: "NumberInput",
  components: {},
  props:{
    value:{
      type: Number,
      default: null
    },
    placeholder:{
      type: String,
      default: '请输入数值'
    },
    disabled: {
      default: false,
      type: Boolean,
    },
    min:{
      type: Number,
      default: 0
    },
    max:{
      type: Number,
      default: 100
    },
    precision:{
      type: Number,
      default: 0
    },
  },
  data() {
    return {}
  },
  methods: {}
}
</script>

<style scoped>

</style>
