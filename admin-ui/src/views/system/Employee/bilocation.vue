<template>
    <el-dialog v-if="dialogFlag" title="设置分身" :visible.sync="dialogFlag" :close-on-click-modal="false" width="800px" top="10vh" @close="cancel">
        <el-table v-loading="loading" stripe :data="peopleList">
            <el-table-column label="部门" prop="deptName" align="center" />
            <el-table-column label="标识" prop="UserName" align="center">
                <template slot-scope="scope">
                    <div>{{ scope.row.IsPrimary ? '主要' : '分身' }}</div>
                </template>
            </el-table-column>
            <el-table-column label="操作" align="center" width="248" class-name="small-padding fixed-width">
                <template slot-scope="scope">
                    <!-- <el-button v-if="scope.row.IsPrimary" size="mini" type="text" icon="el-icon-plus" @click="bilocationAdd(scope.row)">添加分身</el-button> -->
                    <el-button v-if="!scope.row.IsPrimary" size="mini" type="text" icon="el-icon-delete" style="color:red;" @click="handleDelete(scope.row)">删除</el-button>
                </template>
              </el-table-column>
        </el-table>
        <span slot="footer" class="dialog-footer">
            <el-button type="primary" @click="bilocationAdd">添加分身</el-button>
            <el-button @click="cancel">取消</el-button>
        </span>
    </el-dialog>
</template>
<script>
import { userDepts, addClone, delClone } from "@/api/system/Employee";
export default {
  name: 'bilocation',
  props: {
    dialogVisible: {
      type: Boolean
    }
  },
  data() {
    return {
      dialogFlag: false,
      loading: true,
      deptId: '',
      uid: '',
      orgId: '',
      peopleList: [],
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue
    }
  },
  methods: {
    getPeople(row) {
        this.deptId = row.dept_id;
        this.uid = row.Id;
        this.orgId = row.OrgId;
        this.getUserDepts()
    },
    getUserDepts() {
        userDepts({ userId: this.uid, orgId: this.orgId }).then(response => {
            this.peopleList = response.data;
            this.loading = false
        });
    },
    bilocationAdd() {
        addClone({ uid: this.uid, deptId: this.deptId }).then(res => {
            this.getUserDepts()
        })
    },
    handleDelete(row){
        this.$confirm('是否确认删除分身为"' + this.uid + '"的数据项?', "警告", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
        }).then(() => {
            delClone({ uid: this.uid, deptId: row.deptId }).then(res => {
                this.$message.success('删除成功!')
                this.getUserDepts()
            })
        }).catch(() => {})
    },
    cancel() {
      this.$emit('cancelForm')
    }
  }
}
</script>
<style lang="scss" scoped>
::v-deep {
  .el-dialog__header{
    border-bottom: 1px solid #ccc;
  }
}
</style>