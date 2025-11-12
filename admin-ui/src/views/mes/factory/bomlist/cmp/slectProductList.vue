<template>
  <el-dialog v-if="deviceOpen" title="请选择所属物料" :visible.sync="deviceOpen" :close-on-click-modal="false" append-to-body
    width="980px" top="2vh" @close="cancel">
    <el-form :model="deviceQuery" ref="deviceForm" :inline="true" style="display: flex; justify-content: space-between">
      <div>
        <el-form-item label="查询关键字" prop="Key">
          <el-input v-model="deviceQuery.Key" placeholder="请输入物料关键字查询" clearable />
        </el-form-item>
      </div>
      <el-form-item>
        <el-button icon="el-icon-refresh" @click="resetDevice">重置</el-button>
        <el-button type="primary" icon="el-icon-search" @click="deviceQuery.pageNum = 1; loadDeviceList()">搜索</el-button>
      </el-form-item>
    </el-form>
    <el-table ref="devTable" :data="deviceList" tooltip-effect="dark" v-loading="loading" style="width: 100%"
      highlight-current-row @current-change="onDeviceChange" row-key="Id">
      <el-table-column prop="SkuNumber" label="物料编码" align="center" width="150" />
      <el-table-column label="物料类型" align="center">
        <template slot-scope="scope">
            <span>{{ scope.row.ProductLabel == "F" ? "成品" : (scope.row.ProductLabel == "M"?'原材料':"半成品") }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="ProductName" label="物料名称" />
      <el-table-column prop="ClassName" label="物料分类" />
      <el-table-column prop="Specs" label="规格型号" />
      <el-table-column prop="Unit" label="单位" />
      <el-table-column prop="Power" label="功率" />
    </el-table>
    <pagination v-show="total > 0" :total="total" :page.sync="deviceQuery.pageNum" :limit.sync="deviceQuery.pageSize"
      @pagination="loadDeviceList" />
  </el-dialog>
</template>
<script>
import { factoryProductListGet } from '@/api/factory/product'
export default {
  name: 'addDataOrigin',
  props: {
    dialogVisible: {
      type: Boolean
    },
    title: {
      type: String
    },
  },
  data() {
    return {
      loading: false,
      deviceOpen: false,
      isFather: false,
      deviceList: [],
      deviceQuery: {
        Key: '',
        pageNum: 1,
        pageSize: 10,
      },
      total: 0,

    }
  },
  watch: {
    dialogVisible(newValue) {
      this.deviceOpen = newValue
      this.loadDeviceList()
    }
  },
  methods: {
    loadDeviceList() {
      this.loading = true;
      if(this.isFather){
        this.deviceQuery.ProductLabel='U,F'
      }else{
        this.deviceQuery.ProductLabel='M'
      }
      factoryProductListGet(this.deviceQuery).then(response => {
        this.deviceList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      })
    },
    resetDevice() {
      this.devDateRange = [];
      this.resetForm("deviceForm");
      this.loadDeviceList();
    },
    onDeviceChange(val) {
      this.$emit('productSelect', val, this.isFather)
      this.cancel()
    },
    cancel() {
      this.$emit('cancelForm')
    }
  }
}
</script>

<style lang="scss" scoped>
::v-deep {
  .el-dialog__header {
    border-bottom: 1px solid #ccc;
  }

  .el-select,
  .el-cascader {
    width: 100%;
  }
}

.addPeople>.box {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
}

.addPeople>.btn {
  width: 100%;
  justify-content: flex-end;
  display: flex;
  align-items: center;
}
</style>