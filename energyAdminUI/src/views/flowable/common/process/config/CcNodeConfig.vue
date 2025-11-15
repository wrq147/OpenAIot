<template>
  <div>
    <el-button size="mini" icon="el-icon-plus" type="primary" @click="selectOrg" round>选择抄送人</el-button>
    <div class="option">
      <el-checkbox label="允许发起人添加抄送人" v-model="config.shouldAdd"></el-checkbox>
    </div>
    <org-items v-model="select"/>
    <org-picker multiple ref="orgPicker" @ok="selected"/>
  </div>
</template>

<script>
import OrgPicker from "../../OrgPicker.vue";
import OrgItems from "../OrgItems";

export default {
  name: "CcNodeConfig.vue",
  components: {OrgPicker, OrgItems},
  props:{
    config:{
      type: Object,
      default: ()=>{
        return {}
      }
    }
  },
  computed:{
    select: {
      get(){
        return this.config.assignedUser || []
      },
      set(val){
        this.config.assignedUser = val
      }
    }
  },
  data() {
    return {}
  },
  methods: {
    selectOrg() {
      this.$refs.orgPicker.show(this.select,"user")
    },
    selected(select) {
      this.select = Object.assign([], select)
    }
  }
}
</script>

<style lang="less" scoped>
.option{
  color: #606266;
  margin-top: 20px;
  font-size: small;
}

.desc{
  font-size: small;
  color: #8c8c8c;
}
.org-item{
  margin: 5px;
}
</style>
