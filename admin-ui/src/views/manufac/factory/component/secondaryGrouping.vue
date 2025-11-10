<template>
  <div>
    <div style="padding: 10px 20px 0">
      <el-tabs v-model="activeName" @tab-click="handleClick" lazy class="type_tabs">
        <el-tab-pane label="全部" name="all" key="typeall">
          <div slot="label" class="product_type_title">
            <i class="icon el-icon-date"></i><span>全部</span>
            <!-- <i class="el-icon-more right_icon"></i> -->
            <div class="line"></div>
          </div>
        </el-tab-pane>
        <el-tab-pane :label="item.Name" :name="item.Id" v-for="item in groupViewList" :key="'type' + item.Id">
          <div slot="label" class="product_type_title">
            <img class="icon" :src="item.PhotoUrl" alt="" /><span>{{ item.Name }}</span>
            <el-dropdown class="product_type_dropdown" @command="handleMenuSelect($event, item)">
              <span class="el-dropdown-link"><i class="el-icon-more right_icon"></i></span>
              <el-dropdown-menu slot="dropdown">
                <el-dropdown-item command="edit">编辑</el-dropdown-item>
                <el-dropdown-item command="del">删除</el-dropdown-item>
              </el-dropdown-menu>
            </el-dropdown>
            <div class="line"></div>
          </div>
        </el-tab-pane>
        <el-tab-pane label="" name="add_icon" :disabled="true">
          <div slot="label" class="product_type_title" style="cursor: pointer" @click="openViewList">
            <i class="el-icon-plus" style="color: #333333"></i>
          </div>
        </el-tab-pane>
      </el-tabs>
    </div>
    <div class="prop_ul" v-if="activePropsList && activePropsList.length > 0">
      <div class="prop_li" @click="setFilterProp('')" :key="'typeprop'" :class="{ active_prop: activeProp == '' }">全部</div>
      <div class="prop_li" @click="setFilterProp(it)" v-for="it in activePropsList" :key="'typeprop' + it" :class="{ active_prop: it == activeProp }"> {{ it }} </div>
    </div>

    <publicGroupList ref="viewList" @openAddView="openAddView" @afterSave="loadGroupViewList"></publicGroupList>
    <publicGroupEdit ref="viewEdit" @afterSave="afterSave"></publicGroupEdit>
  </div>
</template>

<script>
import publicGroupList from '@/views/manufac/factory/component/publicGroupList.vue'//分组列表
import publicGroupEdit from '@/views/manufac/factory/component/publicGroupEdit.vue'//分组编辑
import {
  groupViewListGet,
  groupViewRemove,
  groupViewInfo,
  editGroupViewSave
} from "@/api/factory/GroupView";
export default {
  name: "AdminUiSecondaryGrouping",
  components: { publicGroupList, publicGroupEdit },
  props:{
    table:{
      type: String,
      default: "",
    },
    filterFiledList:{
      type: Array,
      default: ()=>{
        return []
      },
    },
  },
  data() {
    return {
        activeName:'all',
        groupViewList:[],//分组列表
        activePropsList:[],//二级分组列表
        activeFiledList:[],//字段列表
        activeProp:'',
        activePropFiled:''
    };
  },

  mounted() {},

  methods: {
    setSaveFilterList(filterList) {
      //筛选另存为新分组
      this.$refs.viewEdit.setSaveFilter(filterList,this.table)
    },
    setFilterProp(val) {
      //设置进行过滤的类型属性
      this.activeProp=val
      if(val!=''){
        if(this.activePropFiled){
          let propObj={
            field:this.activePropFiled,
            compare:'等于',//比较符号：大于、小于、大于等于、小于等于、不等于、等于、包含、不包含
            val:val,//字符串
          }
          this.$emit('setFilterProp',propObj)
        }
      }else{
        this.$emit('setFilterProp','')
      }
      
      
    },
    async handleClick(){
      this.activeProp=''
      if (this.activeName !== 'all') {
        let res = await groupViewInfo({ id: this.activeName })
        let findTypeRow = res.data
        // console.log('findTypeRow',findTypeRow.ListFieldsJson);
        // this.activePropsList = findTypeRow.PropList ? findTypeRow.PropList.split(',') : []//属性
        if(res.data.LevelCode){
          this.activePropFiled=res.data.LevelCode
          let levelFied=this.filterFiledList.find(row=>row.mapid==res.data.LevelCode)
          // console.log("levelFied",levelFied);
          if(levelFied&&levelFied.optionals){
            this.activePropsList=levelFied.optionals
          }else{
            this.activePropsList=[]
          }
        }else{
          this.activePropFiled=''
          this.activePropsList=[]
        }
        
        this.activeFiledList = findTypeRow.ListFieldsJson ? JSON.parse(findTypeRow.ListFieldsJson) : []//属性
        if(findTypeRow){
          let ConditionJson=findTypeRow.ConditionJson?JSON.parse(findTypeRow.ConditionJson):[]
          if(ConditionJson&&ConditionJson.length>0){
            ConditionJson=ConditionJson.map(rw=>{
              if(rw.val_num||rw.val_num==0){}else{
                delete rw.val_num
              }
              if(rw.val_arr&&rw.val_arr.length>0){}else{
                delete rw.val_arr
              }
              return rw
            })
          }
          this.$emit('handleGroupClick',this.activeName,this.activeFiledList,ConditionJson)
          return
        }
      }else{
        this.activePropsList=[]
      }
      
      this.$emit('handleGroupClick',this.activeName)
    },
    async afterSave() {
      //类型保存完成
      await this.loadGroupViewList()
      await this.$refs.viewList.loadGroupViewList(this.table)//加载类型列表
    },
    async loadGroupViewList() {//公共分组
      try {
        let res = await groupViewListGet({table:this.table})
        this.groupViewList = JSON.parse(JSON.stringify(res.data))
      } catch (error) {
        console.log(error, 'error');
      }
    },
    handleMenuSelect(val, item) {//下拉菜单的点击对应菜单事件
      // console.log("点击下拉",val,item);
      if (val == 'edit') {
        this.openAddView(item.Id)
      } else if (val == 'del') {
        this.deleteType(item)
      }
    },
    deleteType(item) {
      let that = this
      this.$modal.confirm('是否确认删除分组"' + item.Name + '"？').then(function () {
        return groupViewRemove({ id: item.Id });
      }).then(() => {
        that.$modal.msgSuccess("移除成功");
        return that.loadGroupViewList();

      }).catch((err) => {
        console.log("错误", err);
      });
    },
    openAddView(id) {//打开视图编辑弹窗
      this.$refs.viewEdit.setDrawOpen(id,this.table)
    },
    async openViewList() {
      //打开视图弹窗
      await this.$refs.viewList.setDrawOpen(this.table)
    },
  },
};
</script>

<style lang="scss" scoped>
.type_tabs {
  ::v-deep .el-tabs__item {
    padding-right: 0;
  }

  ::v-deep .el-tabs__active-bar {
    margin-left: -10px;
  }
}

.prop_ul {
  display: flex;
  align-items: center;

  .prop_li {
    color: #666;
    cursor: pointer;
    background-color: #f7f8f8;
    border-radius: 16px;
    padding: 8px 16px;
    font-size: 14px;
    line-height: 14px;
    text-align: center;
    margin-left: 20px;

    &.active_prop {
      background-color: rgba(24, 144, 255, 0.3);
      color: #3572FF;
    }
  }
}

.product_type_title {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 100%;
  position: relative;

  .icon {
    width: 16px;
    height: 16px;
    margin-right: 5px;
    font-size: 16px;
  }

  .right_icon {
    width: 20px;
    height: 20px;
    margin-left: 10px;
    transform: rotate(90deg);
    /* 旋转90度 */
    /* 可选：如果你想让元素保持其原始大小，可以同时应用transform-origin */
    transform-origin: center center;
    /* 旋转中心点在元素中心 */
    display: flex;
    justify-content: center;
    align-items: center;
    border-radius: 2px;
  }

  .right_icon:hover {
    background: #EEEEEE;
    color: #333;
  }

  .line {
    margin-left: 8px;
    width: 2px;
    height: 22px;
    background: #EEEEEE;
  }
}
</style>