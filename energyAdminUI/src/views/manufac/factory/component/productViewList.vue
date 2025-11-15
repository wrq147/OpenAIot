<template>
  <el-drawer title="分组列表" :visible.sync="drawer" direction="rtl" :before-close="handleClose" size="600px">
    <div class="drawer_cot_con">
      <el-row :gutter="10" style="margin-bottom:10px;">
        <el-button type="success" icon="el-icon-circle-plus-outline" plain @click="openAddView">添加分组</el-button>
        <span style="margin-left:10px;color:#666666;font-size:14px;">已创建{{tableData.length}}</span>
      </el-row>
      <el-table :data="tableData" border style="width: 100%" class="draggable-table" row-key="Id">
          <el-table-column prop="Name" label="名称">
              <template slot-scope="{ row }"> 
                <el-tooltip class="item" :content="row.Name" placement="top">
                  <div class="name_con">
                    <i class="el-icon-rank handle"></i>
                    <img class="type_icon" :src="row.PhotoUrl" alt="">
                    <span class="name_text">{{row.Name}}</span>
                  </div>
                </el-tooltip>
                  
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="140">
              <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)">修改</el-button>
                  <el-button type="text" icon="el-icon-edit" @click="handleDelete(scope.row)">删除</el-button>
              </template>
          </el-table-column>
      </el-table>
    </div>
  </el-drawer>
</template>

<script>
import Sortable from 'sortablejs';
import {
  factoryProductTypeListGet,
  factoryProductTypeRemove,
  editProductTypeSave
} from "@/api/factory/product";
export default {
  name: 'AdminUiProductViewList',

  data() {
    return {
      drawer:false,//rtl从右往左打开
      tableData:[],
    };
  },

  mounted() {
    
    
  },

  methods: {
    async loadProductTypeList(){
      try {
        let res=await factoryProductTypeListGet()
        console.log(res,'resres');
        this.tableData=JSON.parse(JSON.stringify(res.data))
        this.tableData.sort((a, b) => {
          if (a.Sort !== b.Sort) { // 首先按 sort 排序
            return a.Sort - b.Sort;
          }
        });
      } catch (error) {
        console.log(error,'error');
      }
    },
    async setDrawOpen(){
      await this.loadProductTypeList()
      this.drawer=true
      this.$nextTick(()=>{
        this.loadSortable()
      })
    },
    handleUpdate(row){
      this.$emit('openAddView',row.Id)
    },
    openAddView(){
      this.$emit('openAddView')
    },
    handleDelete(row){
      let that=this
      this.$modal.confirm('是否确认删除产品分组"' + row.Name + '"？').then(function () {
        return factoryProductTypeRemove({ id:row.Id });
      }).then(() => { 
        that.$modal.msgSuccess("移除成功");
        return that.loadProductTypeList();
       
      }).catch((err) => { 
        console.log("错误",err);
      });
    },
    handleClose(){
        this.drawer=false
    },
    loadSortable(){
      const tbody = document.querySelector('.draggable-table .el-table__body-wrapper tbody')
      console.log(tbody,'tbody');
      if(tbody){
        new Sortable(tbody, {
            handle: '.handle', // handle's class
            animation: 150,
            ghostClass: 'blue-background-class', // 拖动时元素的样式类
            // 需要在odEnd方法中处理原始eltable数据，使原始数据与显示数据保持顺序一致
            onEnd: async({ newIndex, oldIndex }) => {
                let targetRow = JSON.parse(JSON.stringify(this.tableData[oldIndex]))
                let targetNewRow = JSON.parse(JSON.stringify(this.tableData[newIndex]))
                console.log('新旧',targetNewRow,targetRow);
                targetRow.Sort=this.tableData[newIndex].Sort
                targetNewRow.Sort=this.tableData[oldIndex].Sort
                await this.setSaveSort(targetRow,false)
                await this.setSaveSort(targetNewRow,true)
                await this.loadProductTypeList()
                this.$emit('afterSave')
            },
        })
      }
    },
   async setSaveSort(row,showTips){
      try {
        let submitform={
          id:row.Id,
          sort:row.Sort
        }
        await editProductTypeSave(submitform)
        if(this.showTips){
          this.$modal.msgSuccess("重新排序成功");
        }
      } catch (error) {
        console.log("错误",error);
      }
    }
  },
};
</script>

<style lang="scss" scoped>
.blue-background-class{
  background: #F2F2F2;
}
.drawer_cot_con{
  padding: 0 20px;
}
.name_con{
    display: flex;
    align-items: center;
    .handle{
      cursor: pointer;
    }
    .type_icon{
      width: 20px;
      height: 20px;
      border-radius: 3px;
      margin-left: 10px;
    }
    .name_text{
      font-size: 14px;
      margin-left: 10px;
    }
}
</style>