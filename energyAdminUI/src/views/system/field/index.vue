<template>
  <div style="padding:20px 20px;">
    <div style="background-color: #fff;">
      <el-tabs v-model="customFiledType" tab-position="top" @tab-click="getFiledList" style="padding:0 20px;">
        <el-tab-pane name="产品">
          <span slot="label"><i class="el-icon-date"></i> 产品</span>
        </el-tab-pane>
        <el-tab-pane name="供应商">
          <span slot="label"><i class="el-icon-s-shop"></i>供应商</span>
        </el-tab-pane>
        <el-tab-pane name="工序" v-if="isCheckPermi(['/MES/'])">
          <span slot="label"><i class="el-icon-tickets"></i>工序</span>
        </el-tab-pane>
        <el-tab-pane name="报工" v-if="isCheckPermi(['/MES/'])">
          <span slot="label"><i class="el-icon-timer"></i>报工</span>
        </el-tab-pane>
        <el-tab-pane name="工艺路线" v-if="isCheckPermi(['/MES/'])">
          <span slot="label"><i class="zhongtaiiconfont zhongtai-icon-xiansuo"></i>工艺路线</span>
        </el-tab-pane>
      </el-tabs>
      <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-button type="primary" plain @click="handleAdd">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left: 6px">新增</span>
              </el-button>
            </el-col>
            <!-- <el-col :span="1.5">
              <el-upload style="display:inline" accept=".json" :multiple="false" :show-file-list="false" action="#" :before-upload="handleImport">
                <el-button  type="primary" plain>
                  <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                  <span style="margin-left:6px">导入</span>
                </el-button>
              </el-upload>
            </el-col>
            <el-col :span="1.5">
                <el-button  type="primary" plain @click="exportRow(filedTableList,true)">
                  <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                  <span style="margin-left:6px">导出</span>
                </el-button>
            </el-col> -->
            <el-col :span="1.5">
              <el-button v-if="!showsort" type="primary" plain @click="startSort">
                <span style="margin-left:6px">点这里开始拖动排序</span>
              </el-button>
              <el-button v-else type="warning" plain @click="showsort = false">
                <span style="margin-left:6px">点击这里关闭排序</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-input v-model="searchTxt" placeholder="请输入要搜索的名称" clearable>
                <i slot="suffix" class="el-input__icon el-icon-search"></i>
              </el-input>
            </el-col>
          </div>
          <!-- <right-toolbar @queryTable="getFiledList"></right-toolbar> -->
        </el-row>

        <el-table border v-loading="loading" :data="filedTableList" class="data_table" :header-cell-style="cellSty"
          style="width: 100%" row-key="mapid" :row-class-name="sortFilterClass">
          <!-- <el-table-column prop="mapid" label="字段" align="left" width="260"></el-table-column> -->
          <el-table-column type="index" label="排序" align="center" width="50"></el-table-column>
          <el-table-column prop="name" label="字段名称" align="center" width="160"></el-table-column>
          <el-table-column prop="type" label="字段类型" align="center"></el-table-column>
          <el-table-column prop="is_required" label="是否必填" align="center">
            <template slot-scope="scope">
              <el-switch v-model="scope.row.is_required" active-color="#13ce66" :disabled="true"></el-switch>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="300">
            <template slot-scope="scope">
              <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row, scope.$index)">编辑</el-button>
              <el-button v-if="scope.row.parentId != 0" type="text" icon="el-icon-delete"
                @click="handleDelete(scope.row, scope.$index)">删除</el-button>
              <!-- <el-button type="text" icon="el-icon-upload2" @click="exportRow(scope.row)">导出</el-button> -->
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>

    <filedAdd ref="filedAddForm" :filedTableList="filedTableList" :customFiledType="customFiledType"
      @getFiledList="getFiledList"></filedAdd>
  </div>
</template>

<script>
import filedAdd from './filedAdd.vue'
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { orgField, saveOrgField } from '@/api/factory/customFields'
import Sortable from 'sortablejs';
import dayjs from 'dayjs';
import { checkPermi } from "@/utils/permission"; 
export default {
  name: "AdminUiFieldIndex",
  components: { filedAdd },
  mixins: [resizeTableCon],
  data() {
    return {
      refreshTable: true,
      filedTableList: [],//表格数据
      loading: false,
      customFiledType: '产品',
      showsort: false,
      searchTxt: '',
    };
  },

  mounted() {
    this.getFiledList()
  },

  methods: {
    isCheckPermi(val) {
      return checkPermi(val)
    },
    exportRow(row,isAll) {//字段导出功能
      let tmploading = this.$loading({
        lock: true,
        text: "导出中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      let msgitem = {
        t: dayjs().valueOf(),
        items: []
      };
      if(isAll){
        msgitem.items=JSON.parse(JSON.stringify(row))
      }else{
        msgitem.items.push(row);
      }
      let tmname = dayjs().valueOf();
      tmploading.close();
      const content = JSON.stringify(msgitem)
      const blobData = new Blob([content], { type: 'application/json' })
      const filename = `${tmname}.json` //可以自定义后缀名

      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blobData, filename)
      } else {
        const anchor = document.createElement('a')
        anchor.href = window.URL.createObjectURL(blobData)
        anchor.download = filename
        anchor.click()
        window.URL.revokeObjectURL(blobData)
      }
    },
    handleImport(file){//字段导入功能
      let that=this
      let tmploading = this.$loading({
        lock: true,
        text: "导入中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      const reader = new FileReader()
      reader.readAsText(file)
      reader.onload = async (e)=> {
        const str = e.target.result
        const jsonData = JSON.parse(str)
        console.log(jsonData,'jsonDatajsonData',that.filedTableList);
        let resData=[]
        let hasFiled=0
        let hasWrong=0
        if(jsonData.items&&Array.isArray(jsonData.items)&&jsonData.items[0]){
          console.log();
          for(let i=0;i<jsonData.items.length;i++){
            if(jsonData.items[i].mapid){
              let findObj=that.filedTableList.find(ro=>ro.mapid==jsonData.items[i].mapid)
              console.log(findObj,'findObj');
              if(findObj){
                hasFiled++
              }else{
                resData.push(jsonData.items[i])
              }
            }else{
              hasWrong++
              return;
            }
          }
        }else{
          that.$modal.msgError("导入类型错误");
          tmploading.close();
          return;
        }
        console.log("导入的数据",resData);
        let tmparr = [...resData,...that.filedTableList];
        if(hasFiled>0){
          that.$modal.msgError("有字段已经存在，导入失败");
          // tmploading.close();
          // return;
        }
        if(hasWrong>0){
          that.$modal.msgError("导入类型错误");
          tmploading.close();
          return;
        }
        saveOrgField({ field: that.customFiledType, val: JSON.stringify(tmparr) }).then(res => {
          that.$modal.msgSuccess("导入成功");
          console.log(res, 'resres');
          that.getFiledList()
        }).catch(er=>{
          tmploading.close();
          return
        });
        tmploading.close();
      }
    },
    sortFilterClass({ row, rowIndex }) {
      if (this.showsort) {
        return "item-sort" + (this.searchTxt.length > 0 && row.name.indexOf(this.searchTxt) == -1 ? " hidden-row" : "");
      }
      else {
        return "no-sort" + (this.searchTxt.length > 0 && row.name.indexOf(this.searchTxt) == -1 ? " hidden-row" : "");
      }
    },
    startSort() {
      this.showsort = true;
      //初始化排序
      let that = this
      const tbody = document.querySelector(".data_table .el-table__body-wrapper tbody");
      new Sortable(tbody, {
        animation: 150,
        filter: ".no-sort",
        // 需要在odEnd方法中处理原始eltable数据，使原始数据与显示数据保持顺序一致
        onEnd: ({ newIndex, oldIndex }) => {
          console.log('排序结果', newIndex, oldIndex);
          let tmparr = JSON.parse(JSON.stringify(that.filedTableList))
          const targetRow = tmparr[oldIndex];
          tmparr.splice(oldIndex, 1);
          tmparr.splice(newIndex, 0, targetRow);
          saveOrgField({ field: that.customFiledType, val: JSON.stringify(tmparr) }).then(res => {
            console.log(res, 'resres');
          });
        },
      });
    },
    sortChange() {

    },
    handleUpdate(row, index) {
      this.$refs.filedAddForm.openDialog(index)//打开添加的弹窗
    },
    handleDelete(row, index) {
      console.log(row, 'row')
      let that = this
      this.$modal.confirm('是否确认移除自定义字段"' + row.name + '"？').then(function () {
        let sumfiledList = JSON.parse(JSON.stringify(that.filedTableList))
        sumfiledList.splice(index, 1)
        return saveOrgField({ field: that.customFiledType, val: JSON.stringify(sumfiledList) });
      }).then(() => {
        that.getFiledList();
        that.$modal.msgSuccess("移除成功");
      }).catch((err) => {
        console.log("错误", err);
      });
    },
    getFiledList() {
      let orgId = this.$store.state.user.orgId
      orgField({ orgId: orgId, field: this.customFiledType }).then(res => {

        if (res.data) {
          if (res.data.ExtValue) {
            let filedList = JSON.parse(res.data.ExtValue)
            // console.log(filedList, 'filedListfiledList');

            this.filedTableList = filedList//排序处理，并且数字字段排前面

          }
        } else {
          this.filedTableList = []
        }
      })
    },
    async handleAdd() {
      await this.$refs.filedAddForm.openDialog()//打开添加的弹窗
    }
  },
};
</script>
<style lang="less" scoped>
.data_table {
  ::v-deep .item-sort {
    box-shadow: 5px 5px 5px -2px rgba(0, 0, 0, .3);
  }

  ::v-deep .hidden-row {
    display: none;
  }
}
</style>