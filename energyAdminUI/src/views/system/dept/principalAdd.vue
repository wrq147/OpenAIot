<template>
    <div>
      <el-dialog v-if="dialogFlag" title="修改负责人" :visible.sync="dialogFlag" :close-on-click-modal="false" width="800px" top="10vh" @close="cancel">
        <el-table v-loading="loading" stripe :data="peopleList" @selection-change="handleSelectionChange">
          <el-table-column type="selection" align="center" width="50" />
          <el-table-column label="账号" prop="UserName" align="center" width="200" />
          <el-table-column label="员工昵称" prop="RealName" align="center" />
          <el-table-column label="部门" prop="dept_name" align="center" />
          <el-table-column label="是否负责人" prop="dept_name" align="center">
              <template v-slot="scope">
                <div>{{ scope.row.IsLeader? '是': '否' }}</div>
              </template>
          </el-table-column>
          <el-table-column label="设置负责人" width="200" align="center">
              <template v-slot="scope">
                <el-switch
                    :value="scope.row.IsLeader"
                    active-color="#13ce66"
                    inactive-color="#ff4949"
                    active-text="是"
                    inactive-text="否"
                    @change="switchChange(scope.row.Id, $event)"
                  >
                </el-switch>
              </template>
          </el-table-column>
        </el-table>
        <span slot="footer" class="dialog-footer">
          <el-select style="margin-right: 20px;" v-model="deptMoveId" placeholder="请选择部门" clearable>
              <el-option
                v-for="(o, index) in deptOriginalData"
                :key="index"
                :label="o.deptName"
                :value="o.deptId"
              />
            </el-select>
          <el-button type="success" @click="deptMove">移动至部门</el-button>
          <el-button @click="cancel">取消</el-button>
        </span>
      </el-dialog>
    </div>
  </template>
  <script>
  import { ObtainEmployee } from "@/api/crm/plan";
  import { setLeader, DeptMove } from "@/api/system/dept";
  export default {
    name: 'principalAdd',
    props: {
      dialogVisible: {
        type: Boolean
      },
      deptOriginalData: {
        type: Array
      }
    },
    data() {
      return {
        dialogFlag: false,
        // 表单
        peopleList: [],
        deptId: '',
        deptMoveId: '',
        loading: true,
        ids: [],
        uids: []
      }
    },
    watch: {
      dialogVisible(newValue) {
        this.dialogFlag = newValue
      }
    },
    methods: {
      getPeople(deptId) {
        this.deptId = deptId;
        this.getApiPeople()
      },
      getApiPeople() {
        ObtainEmployee({ deptId: this.deptId, deptIdWithChildren: false }).then(response => {
            this.peopleList = response.data.List;
            this.loading = false
        });
      },
      switchChange(id, event) {
        setLeader({ deptId: this.deptId, leaderId: id, isLeader: event }).then(response => {
            this.getApiPeople()
        });
      },
      handleSelectionChange(val) {
        this.ids = []
        this.uids = []
        this.uids = val.map(item => item.Id);
        this.uids = [...new Set(this.uids)]
        this.ids = val.map(item => item.dept_id);
        this.ids = [...new Set(this.ids)]
      },
      deptMove() {
        if (this.uids.length > 0) {
          this.deptMoveSave()
        } else {
          this.$message({
            type: 'error',
            message: '请选择需要移动的人员'
          });
        }
      },
      // 移动确定
      deptMoveSave() {
        if(this.deptMoveId !=='' ) {
          DeptMove({ uids: this.uids, ids: [],parentId: this.deptMoveId, memDeptId: this.deptId }).then(response => {
            this.getApiPeople()
          });
        } else {
          this.$message({
            type: 'error',
            message: '请选择需要移动的部门'
          });
          return;
        }
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
  .el-radio-group {
    display: block;
  }
}
.outer-dialog {
  z-index: 2000 !important;
}
.inner-dialog {
  z-index: 2050 !important;
}
</style>