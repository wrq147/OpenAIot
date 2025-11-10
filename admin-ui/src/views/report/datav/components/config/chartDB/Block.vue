<template>
  <div>
      <p>{{element.tableName}}</p>
      <el-tree
        :data="element.tableData"
        show-checkbox
        default-expand-all
        node-key="id"
        ref="tree"
        highlight-current
        @check-change="getCheckedKeys">
      </el-tree>
  </div>
  
</template>


<script>
export default {
  props: ["element","sqlMap"],
  data() {
      console.log("block=====",this.element);
      return {
        
      }
  },
  methods: {
    getCheckedKeys(){
       let res = this.$refs.tree.getCheckedKeys();
       //console.log(res);
       this.sqlMap.set(this.element.tableName,res);
    }
  },
}
</script>


