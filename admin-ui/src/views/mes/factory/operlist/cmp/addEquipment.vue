<template>
    <el-dialog v-if="deviceOpen" title="添加设备" :visible.sync="deviceOpen" :close-on-click-modal="false" append-to-body width="980px" top="2vh" @close="cancel">
        <el-form :model="deviceQuery" ref="deviceForm" :inline="true" style="display: flex; justify-content: space-between">
          <div>
            
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
          ref="tableRef"
          :data="deviceList"
          tooltip-effect="dark"
          v-loading="loading"
          style="width: 100%"
          highlight-current-row
          @selection-change="onDeviceChange"
          row-key="Id"
        >
          <el-table-column type="selection" width="55" />
          <el-table-column prop="Id" label="设备编码" align="center" width="150" />
          <el-table-column prop="Name" label="出厂编号" />
          <el-table-column label="设备状态">
            <template slot-scope="scope">
              <span v-if="scope.row.Online === 1">在线</span>
              <span v-else-if="scope.row.Online === 0">离线</span>
              <span v-else>未初始化</span>
            </template>
          </el-table-column>
          <el-table-column prop="ProductName" label="物料名称" />
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
import { myDeviceList } from "@/api/after/dev";
export default { 
  name: 'addDataOrigin',
  props: {
    dialogVisible: {
      type: Boolean
    },
    title: {
      type: String
    },
    // 新增：接收已选中的设备数组用于回显
    equipment: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
      loading: false,
      deviceOpen: false,
      deviceList: [],
      list: null,
      allSelectedDevices: [], //存所有选中的设备（跨页）
      deviceQuery: {
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
        this.allSelectedDevices = this.formatSelectedDevices(this.equipment);
        this.loadDeviceList();
      }
    },
    // 监听传入的设备列表变化
    equipment(newVal) {
      if (this.deviceOpen) {
        this.allSelectedDevices = this.formatSelectedDevices(newVal);
        this.$nextTick(() => {
          this.restoreSelection();
        });
      }
    }
  },
  methods: {
    // 格式化选中设备（提取ID并去重）
    formatSelectedDevices(devices) {
      if (!Array.isArray(devices)) return [];
      // 提取ID并去重，确保与表格数据结构一致
      const idSet = new Set(devices.map(item => String(item.Id || item.id)));
      return devices.filter(item => idSet.has(String(item.Id || item.id)));
    },
    async loadDeviceList() {
        this.loading = true;
        try {
          const response = await myDeviceList(this.deviceQuery);
          this.deviceList = response.data.List || [];
          this.total = response.data.Total || 0;
        } catch (error) {
          console.error('加载设备列表失败', error);
        } finally {
          this.loading = false;
          this.$nextTick(() => {
            this.restoreSelection(); // 数据加载完成后恢复选中状态
          });
        }
    },
    resetDevice() {
      this.resetForm("deviceForm");
      this.loadDeviceList();
    },
    onDeviceChange(val) {
      // 更新当前页选中的设备（使用ID去重）
      const currentIds = new Set(this.deviceList.map(item => item.Id));
      const selectedIds = new Set(val.map(item => item.Id));
      
      // 新增：跨页选中逻辑（保留不在当前页的选中项）
      this.allSelectedDevices = this.allSelectedDevices.filter(item => {
        // 若设备在当前页，检查是否被选中
        if (currentIds.has(item.Id)) {
          return selectedIds.has(item.Id);
        }
        // 若设备不在当前页，保留选中状态
        return true;
      });
      
      // 添加当前页新选中的设备
      this.allSelectedDevices = [...this.allSelectedDevices, ...val].filter((item, index, arr) => {
        return arr.findIndex(i => i.Id === item.Id) === index;
      });
    },
    submitForm() {
      if (this.allSelectedDevices.length > 0) {
          // 传递选中的设备列表
          this.$emit('getSelectEquip', this.allSelectedDevices)
          this.cancel();
      } else {
          this.$message.error('请选择设备');
      }
    },
    cancel() {
      this.list = null;
      this.$emit('cancelForm')
    },
    // 恢复选中状态
    restoreSelection() {
      if (!this.$refs.tableRef) return;
      
      // 清除当前选择
      this.$refs.tableRef.clearSelection();
      
      // 重新勾选已选中的设备（使用ID精确匹配）
      const selectedIds = new Set(this.allSelectedDevices.map(item => item.Id));
      this.deviceList.forEach(row => {
        if (selectedIds.has(row.Id)) {
          this.$refs.tableRef.toggleRowSelection(row, true);
        }
      });
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
  width: 232px;

  input {
    width: 232px;
    border-radius: 4px;
  }
}
</style>