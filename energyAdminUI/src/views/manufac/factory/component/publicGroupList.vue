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
  groupViewListGet,
  groupViewRemove,
  editGroupViewSave,
  setGroupViewSort
} from "@/api/factory/GroupView";
export default {
  name: 'AdminUiProductViewList',

  data() {
    return {
      drawer:false,//rtl从右往左打开
      tableData:[],
      activeGroupType:''
    };
  },

  mounted() {
    
    
  },

  methods: {
    async loadGroupViewList(table){
      try {
        let res=await groupViewListGet({table:table})
        console.log(res,'resres');
        this.tableData=JSON.parse(JSON.stringify(res.data))
        this.$forceUpdate()
        // this.tableData.sort((a, b) => {
        //   if (a.Sort !== b.Sort) { // 首先按 sort 排序
        //     return a.Sort - b.Sort;
        //   }
        // });
      } catch (error) {
        console.log(error,'error');
      }
    },
    async setDrawOpen(table){
      this.activeGroupType=table
      await this.loadGroupViewList(table)
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
      this.$modal.confirm('是否确认删除分组"' + row.Name + '"？').then(function () {
        return groupViewRemove({ id:row.Id });
      }).then(() => { 
        that.$modal.msgSuccess("移除成功");
        return that.loadGroupViewList(this.activeGroupType);
       
      }).catch((err) => { 
        console.log("错误",err);
      });
    },
    handleClose(){
        this.drawer=false
    },
    loadSortable(){
      const tbody = document.querySelector('.draggable-table .el-table__body-wrapper tbody')
      // console.log(tbody,'tbody');
      if(tbody){
        new Sortable(tbody, {
            handle: '.handle', // handle's class
            animation: 150,
            ghostClass: 'blue-background-class', // 拖动时元素的样式类
            // 需要在odEnd方法中处理原始eltable数据，使原始数据与显示数据保持顺序一致
            onEnd: async({ newIndex, oldIndex }) => {
              let tmparr = JSON.parse(JSON.stringify(this.tableData));
              let targetRow = JSON.parse(JSON.stringify(tmparr[oldIndex]))
              tmparr.splice(oldIndex, 1);
              tmparr.splice(newIndex, 0, targetRow);
              let idArr=tmparr.map(row=>row.Id)
              await this.setSaveSort(idArr)
              await this.loadGroupViewList(this.activeGroupType)
              this.$emit('afterSave')
            },
        })
      }
    },
    async setSaveSort(idArr){
      try {
        await setGroupViewSort(idArr)
        this.$modal.msgSuccess("重新排序成功");
      } catch (error) {
        console.log("错误",error);
      }
    },
  //  async setSaveSort(row,showTips){
  //     try {
  //       let submitform={
  //         id:row.Id,
  //         sort:row.Sort
  //       }
  //       await editGroupViewSave(submitform)
  //       if(this.showTips){
  //         this.$modal.msgSuccess("重新排序成功");
  //       }
  //     } catch (error) {
  //       console.log("错误",error);
  //     }
  //   }
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