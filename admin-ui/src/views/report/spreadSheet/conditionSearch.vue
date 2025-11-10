<template>
    <el-dialog v-if="dialogFlag" title="查询条件" :visible.sync="dialogFlag" :close-on-click-modal="false" width="900px" top="2vh" @close="cancel">
        <div class="custom-dialog-body data-search-box">
            <el-table :data="tableDataChange" max-height="500"> 
                <el-table-column label="字段">
                  <template v-slot="scope">
                    <el-tooltip class="item" effect="dark" :content="'原名：' + scope.row.columnCount" placement="bottom">
                      <el-input v-model="scope.row.fieldName" />
                    </el-tooltip>
                  </template>
                </el-table-column>
                <el-table-column label="类型" width="100">
                  <template v-slot="scope">
                    <el-input v-model="scope.row.fieldType" readonly />
                  </template>
                </el-table-column>
                <el-table-column label="查询模式">
                  <template v-slot="scope">
                    <el-select v-model="scope.row.searchType" placeholder="请选择">
                      <el-option v-if="scope.row.fieldType !== '时间'" label="输入框" :value="0" />
                      <el-option v-if="scope.row.fieldType === '时间'" label="下拉单选" :value="1" />
                      <el-option v-if="scope.row.fieldType === '时间'" label="范围查询" :value="2" />
                    </el-select>
                  </template>
                </el-table-column>
                <el-table-column label="查询默认值">
                  <template v-slot="scope">
                    <el-input v-model="scope.row.searchDefault" placeholder="请输入查询默认值" />
                  </template>
                </el-table-column>
                <el-table-column label="查询日期格式">
                  <template v-slot="scope">
                    <el-select v-if="scope.row.fieldType === '时间'" v-model="scope.row.format" placeholder="请选择">
                      <el-option label="yyyy" value="yyyy" />
                      <el-option label="yyyy-MM-dd" value="yyyy-MM-dd" />
                      <el-option label="yyyy-MM-dd HH" value="yyyy-MM-dd HH" />
                      <el-option label="yyyy-MM-dd HH:mm" value="yyyy-MM-dd HH:mm" />
                      <el-option label="yyyy-MM-dd HH:mm:ss" value="yyyy-MM-dd HH:mm:ss" />
                    </el-select>
                    <el-input v-else v-model="scope.row.fieldType" readonly />
                  </template>
                </el-table-column>
                <el-table-column label="是否查询" width="80">
                  <template v-slot="scope">
                    <el-switch v-model="scope.row.openSearch" active-color="#13ce66" inactive-color="#e3e6e8" />
                  </template>
                </el-table-column>
            </el-table>
        </div>
        <span slot="footer" class="dialog-footer">
          <el-button @click="cancel">取消</el-button>
          <el-button type="primary" @click="submitForm('ruleForm')">确定</el-button>
        </span>
    </el-dialog>
</template>
<script>
export default { 
  name: 'conditionSearch',
  props: {
    dialogVisible: {
      type: Boolean
    },
    tableData: {
      type: Array,
      default: () => ({})
    }
  },
  data() {
    return {
      dialogFlag: false,
      tableDataChange: []
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue
      this.tableDataChange = JSON.parse(JSON.stringify(this.tableData))
    }
  },
  methods: {
    submitForm() {
      this.$emit('submitForm', this.tableDataChange)
    },
    cancel() {
      this.tableDataChange = JSON.parse(JSON.stringify(this.tableData))
      this.$emit('cancelForm')
    }
  }
}
</script>
  
<style lang="scss" scoped>
 ::v-deep {
  .el-dialog__header{
    height: 48px;
    background: #f5f6f7;
    padding: 0 16px 0 24px;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  .el-dialog__title{
    font-family: SourceHanSansCN-Bold !important;
    font-size: 14px;
    color: #363b4c;
    margin-bottom: 2px;
  }
  .el-dialog__body{
    padding: 0;
  }
  .el-input__inner{
    background-color: #f5f6f7;
    border: none;
  }
  .el-table tbody tr:hover>td {
    background: none !important;
  }
}
.custom-dialog-body{
  padding: 12px;
}
</style>