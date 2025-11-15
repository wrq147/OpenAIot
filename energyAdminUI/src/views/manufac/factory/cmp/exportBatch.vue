<template>
    <el-dialog v-if="deviceOpen" title="批量导入产品批次" :visible.sync="deviceOpen" :close-on-click-modal="false" append-to-body width="980px" top="2vh" @close="cancel">
        <el-form :model="deviceQuery" ref="deviceForm" :inline="true" style="display: flex; justify-content: space-between">
          <div>
            <el-form-item label="查询关键字" prop="Key">
              <el-input v-model="deviceQuery.Key" placeholder="请输入设备关键字查询" clearable />
            </el-form-item>
          </div>
          <el-form-item>
            <el-button icon="el-icon-refresh" @click="resetDevice">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="deviceQuery.pageNum = 1,loadDeviceList">搜索</el-button>
          </el-form-item>
        </el-form>
        <el-table
          ref="devTable"
          :data="deviceList"
          tooltip-effect="dark"
          v-loading="loading"
          style="width: 100%"
          @selection-change="handleSelectionChange"
          row-key="Id"
          :row-class-name="setRowClassName"
        >
          <el-table-column type="selection" width="55"></el-table-column>
          <el-table-column type="index" label="序号" align="center" width="50" />
          <el-table-column label="预览图片" align="center" width="150">
            <template slot-scope="scope">
              <div class="imgwrap">
                <el-image
                  fit="cover"
                  :src="scope.row.PhotoUrl + '?wh=500x500'"
                  :preview-src-list="[scope.row.PhotoUrl]"
                >
                </el-image>
              </div>
            </template>
          </el-table-column>
          <el-table-column prop="Name" label="设备名称" />
          <el-table-column prop="ProductId" label="物联产品编号" />
          <el-table-column prop="DeviceId" label="设备编号" />
        </el-table>
        <pagination
          v-show="total > 0"
          :total="total"
          :page.sync="deviceQuery.pageNum"
          :limit.sync="deviceQuery.pageSize"
          @pagination="loadDeviceList"
        />
        <slectProductList :dialog-visible="productOpen" @cancelForm="cancelForm" @productSelect="productSelect" />
        <span slot="footer" class="dialog-footer">
            <el-button @click="cancel">取消</el-button>
            <el-button type="primary" :disabled="list.length===0" @click="nextStep">下一步</el-button>
        </span>
    </el-dialog>
</template>
<script>
import { devicePageBatch, AddListBatch } from '@/api/manufac/batchList'
import slectProductList from './slectProductList.vue'
export default { 
  name: 'addDataOrigin',
  components: {
    slectProductList
  },
  props: {
    dialogVisible: {
      type: Boolean
    },
  },
  data() {
    return {
      loading: false,
      deviceOpen: false,
      productOpen: false,
      deviceList: [],
      list: [],
      selectedIds: [],
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
      this.loadDeviceList()
    }
  },
  methods: {
    // 获取设备列表
    loadDeviceList() {
        this.loading = true;
        devicePageBatch(this.deviceQuery).then(response => {
            this.deviceList = response.data.List;
            this.total = response.data.Total;
            this.loading = false;
        })
    },
    // 重置查询条件
    resetDevice() {
      this.devDateRange = [];
      this.resetForm("deviceForm");
      this.loadDeviceList();
    },
    // 选择设备列表
    handleSelectionChange(val) {
        this.list = val;
        this.selectedIds = val.map(row => row.Id);
    },
    // 设置行的类名
    setRowClassName({ row }) {
      return this.selectedIds.includes(row.Id) ? 'selected-row' : '';
    },
    // 下一步操作
    nextStep() {
        this.productOpen = true;
    },
    // 选择产品方法
    productSelect(data) {
        this.addListExport(data.Id);
    },
    // 批量导入方法
    addListExport(productId) {
        const ids = this.list.map(item => item.Id);
        AddListBatch({ ids: ids, productId: productId }).then(res => {
            this.$message.success('导入成功!');
            this.list = [];
            this.$emit('getList');
        })
    },
    cancelForm() {
        this.productOpen = false;
    },
    cancel() {
      this.list = [];
      this.$emit('cancelForm');
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
</style>