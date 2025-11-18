<template>
  <el-dialog width="800px" title="请选择分配的产品" :visible.sync="productOpen" append-to-body>
    <el-form :model="productQuery2" ref="deviceForm" :inline="true"
    style="display: flex; justify-content: space-between">
    <div>
        <el-form-item label="查询产品名称" prop="Name">
        <el-input v-model="productQuery2.Name" placeholder="请输入产品名称" clearable></el-input>
        </el-form-item>
    </div>
    <el-form-item>
        <el-button icon="el-icon-refresh" @click="resetSelectDevProduct">重置</el-button>
        <el-button type="primary" icon="el-icon-search" @click="getproductList2()">搜索</el-button>
    </el-form-item>
    </el-form>
    <el-table ref="devproductTable" :data="devProductList" tooltip-effect="dark" v-loading="prodloading" style="width: 100%"
        @selection-change="onDevProductChange" @row-click="clickDevProRow" @select="devProductBoxSelect" :row-key="getRowKeys">
    <el-table-column type="selection" width="55" :reserve-selection="true"> </el-table-column>
    <el-table-column label="预览图片" align="center" width="150">
        <template slot-scope="scope">
        <div class="imgwrap" style="max-width: 60px;max-height:60px;">
            <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'" :preview-src-list="[scope.row.PhotoUrl]"></el-image>
        </div>
        </template>
    </el-table-column>
    <el-table-column prop="ProductName" label="产品名称" align="center"> </el-table-column>
    </el-table>
    <pagination v-show="devProductTotal > 0" :total="devProductTotal" :page.sync="productQuery2.pageNum" :limit.sync="productQuery2.pageSize" @pagination="getproductList2()" />
    <div slot="footer" class="dialog-footer">
        <el-button @click="productOpen = false">取 消</el-button>
        <el-button type="primary" @click="addPlaneDevProduct">确 定</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { myProductList } from "@/api/after/dev";
export default {
  name: 'AdminUiSelectDevProduct',

  data() {
    return {
        getRowKeys(row) {
            return row.Id;
        },
        productOpen:false,
        productQuery2: {
            pageNum: 1,
            pageSize: 10,
            Name: "",
        },
        prodloading:false,
        devProductList:[],
        devProductTotal:0,
        afterSelectProduct:[],//选择后的产品列表
    };
  },

  mounted() {
    
  },

  methods: {
    onDevProductChange(val){
      this.afterSelectProduct = JSON.parse(JSON.stringify(val))
    },
    devProductBoxSelect(arr, row) {
      //点击产品选择多选框
      const selected = this.afterSelectProduct.some(
        (item) => item.Id === row.Id
      );
      if (selected) {
        this.afterSelectProduct = this.afterSelectProduct.filter(
          (rw) => rw.Id !== row.Id
        );
      }
    },
    clickDevProRow(row) {
      const selected = this.afterSelectProduct.some(
        (item) => item.Id === row.Id
      );
      if (!selected) {
        // 选择
        this.$refs.devproductTable.toggleRowSelection(row, true);
      } else {
        // 取消
        this.$refs.devproductTable.toggleRowSelection(row, false);
        this.afterSelectProduct = this.afterSelectProduct.filter(
          (rw) => rw.Id !== row.Id
        );
      }
    },
    resetSelectDevProduct(){//重置产品选择搜索
      this.productQuery2.Key = ''
      this.productQuery2.pageNum = 1
      this.getproductList2();
    },
    openAddProduct(planeTargetData){
      this.productOpen=true
      this.$nextTick(()=>{
        let allselArr=JSON.parse(JSON.stringify(planeTargetData))
        let selList=allselArr.filter(row=>row.TargetType==1)
        this.$refs.devproductTable.clearSelection()
        this.afterSelectProduct=selList.map(row=>{
          let obj={
            Id:row.TargetId,
            Name:row.TargetName,
            PhotoUrl:row.PhotoUrl
          }
          if(obj){
            this.$refs.devproductTable.toggleRowSelection(obj, true);
          }
          return obj
        })
      })
      this.productQuery2.pageNum = 1
      this.getproductList2()
    },
    addPlaneDevProduct(){
      //添加计划的产品
      let sellist=this.afterSelectProduct.map(row=>{
        let obj={
          TargetId:row.Id,
          TargetType:1,
          PhotoUrl:row.PhotoUrl,
          TargetName:row.ProductName,
        }
        return obj
      })
      this.$emit('addPlaneDevProduct',sellist)
      this.productOpen = false
    },
    async getproductList2() {
      this.prodloading = true;
      let response = await myProductList(this.productQuery2)
      if (response.code == 0) {
        if (response.data && response.data.List) {
          this.devProductList = response.data.List;
          this.devProductTotal = response.data.Total;
        }
      }
      this.prodloading = false;
    },
  },
};
</script>