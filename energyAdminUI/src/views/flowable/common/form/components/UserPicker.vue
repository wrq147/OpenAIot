<template>
  <div>
    <div v-if="mode === 'DESIGN'">
      <el-button disabled icon="el-icon-user" type="primary" size="mini" round>选择人员</el-button>
      <span class="placeholder"> {{placeholder}}</span>
    </div>
    <div v-else style="width:100%">
      <el-button :disabled="disabled" icon="el-icon-user" type="primary" size="mini" round @click="showDialog">选择人员</el-button>
      <org-picker :multiple="multiple" ref="orgPicker" :selected="_value" @ok="selected"/>
      <span class="placeholder" v-if="_value && _value.length == 0"> {{placeholder}}</span>
      <div style="margin-top: 5px;display:inline-block">
        <el-tag size="mini" style="margin: 5px" :style="{'cursor': disabled?'pointer':''}" :closable="!disabled" v-for="(dept, i) in _value" :key="i" @close="delDept(i)">{{dept.name}}</el-tag>
      </div>
    </div>
  </div>
</template>

<script>
import componentMinxins from '../ComponentMinxins'
import OrgPicker from "../../OrgPicker";

export default {
  mixins: [componentMinxins],
  name: "UserPicker",
  components: {OrgPicker},
  props: {
    value:{
      type: Array,
      default: () => {
        return []
      }
    },
    placeholder: {
      type: String,
      default: '请选择人员'
    },
    multiple:{
      type: Boolean,
      default: false
    },
    disabled: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
      showOrgSelect: false
    }
  },
  methods: {
    showDialog() {
      this.showOrgSelect = true;
      this.$refs.orgPicker.show(this._value, "user");
    },
    selected(values){
      this.showOrgSelect = false
      this._value = values
      this.$refs.orgPicker.show([], "user");
    },
    delDept(i){
      if(this.disabled==true){
        return;
      }
      this._value.splice(i, 1)
    }
  }
}
</script>

<style scoped>
.placeholder{
  margin-left: 10px;
  color: #adabab;
  font-size: smaller;
}
</style>
