<template>
    <el-dialog v-if="deviceOpen" title="添加人员" :visible.sync="deviceOpen" :close-on-click-modal="false" append-to-body width="980px" top="2vh" @close="cancel">
        <el-form :model="deviceQuery" ref="deviceForm" :inline="true" style="display: flex; justify-content: space-between">
          <div>
            <el-form-item label="所属部门" prop="deptId">
                <treeselect class="set_radius" v-model="deviceQuery.deptId" :options="deptOptions" :show-count="true"
                  placeholder="请选择所属部门" />
            </el-form-item>
            <el-form-item label="查询关键字" prop="Key">
              <el-input v-model="deviceQuery.Key" placeholder="请输入关键字查询" clearable />
            </el-form-item>
          </div>
          <el-form-item>
            <el-button icon="el-icon-refresh" @click="resetDevice">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="deviceQuery.pageNum = 1;loadDeviceList()">搜索</el-button>
          </el-form-item>
        </el-form>
        <el-table
          ref="devTable"
          :data="deviceList"
          tooltip-effect="dark"
          v-loading="loading"
          style="width: 100%"
          highlight-current-row
          @current-change="onDeviceChange"
          row-key="Id"
        >
          <el-table-column prop="Id" label="人员编号" align="center" width="150" />
          <el-table-column prop="RealName" label="人员名称" />
          <el-table-column prop="dept_name" label="所属部门" />
          <el-table-column prop="Mobile" label="手机号码" />
        </el-table>
        <pagination
          v-show="total > 0"
          :total="total"
          :page.sync="deviceQuery.pageNum"
          :limit.sync="deviceQuery.pageSize"
          @pagination="loadDeviceList"
        />
        <span slot="footer" class="dialog-footer">
            <el-button @click="cancel">取消</el-button>
            <el-button type="primary" @click="submitForm">确定</el-button>
        </span>
    </el-dialog>
</template>
<script>
import { listMembers } from "@/api/system/member";
import { treeselect } from "@/api/system/dept";
import { addGroupPeople } from "@/api/scheduling/timeGroup";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
export default { 
  name: 'addDataOrigin',
  components: { Treeselect },
  props: {
    dialogVisible: {
      type: Boolean
    },
    title: {
      type: String
    }
  },
  data() {
    return {
      loading: false,
      deviceOpen: false,
      deviceList: [],
      list: null,
      deptOptions: [],
      deviceQuery: {
        deptId: '',
        Key: '',
        pageNum: 1,
        pageSize: 10,
      },
      total: 0
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.deviceOpen = newValue
      if(newValue){
        this.loadDeviceList();
        this.getTreeselect();
      }
    }
  },
  methods: {
    loadDeviceList() {
        this.loading = true;
        listMembers(this.deviceQuery).then(response => {
            this.deviceList = response.data.List;
            this.total = response.data.Total;
            this.loading = false;
        })
    },
    /** 查询部门下拉树结构 */
    getTreeselect() {
      treeselect({ OrgId: this.$store.getters.orgId }).then(response => {
        this.deptOptions = response.data;
      });
    },
    resetDevice() {
      this.resetForm("deviceForm");
      this.loadDeviceList();
    },
    onDeviceChange(val) {
        this.list = val;
    },
    submitForm() {
        if (this.list) {
            this.$emit('getSelectPeople', this.list)
            this.cancel();
        } else {
            this.$message.error('请选择人员');
        }
    },
    cancel() {
      this.list = null;
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
  .el-select, .el-cascader{
    width: 100%;
  }
}
.addPeople>.box{
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
}
.addPeople>.btn{
  width:100%;
  justify-content: flex-end;
  display: flex;
  align-items: center;
}
.set_radius {
  height: 36px;
  line-height: 36px;
  border-radius: 4px;
  vertical-align: middle;
  // width: 232px;

  input {
    // width: 232px;
    border-radius: 4px;
  }
}
</style>