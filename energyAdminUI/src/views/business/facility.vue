<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div class="elbiaoge_elform tree_con" :style="{'min-height':tableConHeight+'px','margin-right':'10px'}">
      <el-row :gutter="8" class="mb8 button_row" style="padding:0 20px">
        <!-- <div> -->
          <el-col :span="1.5">
            <el-button style="width:96px" type="info" plain @click="handleGroup()" :disabled="treeList&&treeList.length>0&&!activeId||treeList&&treeList.length>0&&activeVal&&activeVal.FacilityType">
              <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
              <span style="margin-left:6px">添加分组</span>
            </el-button>
          </el-col>
          <el-col :span="1.5">
            <el-button style="width:96px" type="info" plain @click="handleFacilities()" :disabled="!treeList||treeList&&treeList.length==0||treeList&&treeList.length>0&&activeVal&&activeVal.FacilityType">
              <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
              <span style="margin-left:6px">添加设施</span>
            </el-button>
          </el-col>
        <!-- </div> -->
      </el-row>
      <el-row :gutter="10" style="padding:0 20px;margin-bottom:16px;">
        <el-col :span="24">
          <el-input @change="handleQuery" @clear="handleQuery" prefix-icon="el-icon-search" class="set_radius" style="width:200px" v-model="groupKey" placeholder="请输入分组" clearable @keyup.enter.native="handleQuery"></el-input>
        </el-col>
      </el-row>
      <treeLi :filterText="groupKey" @handleTreeEdit="handleTreeEdit" @handleTreeDel="handleTreeDel" :key="'treekey'+treekey+item.Id" @choice="finishChoice" ref="treeLi" :treeLi="item" v-for="item in treeList" :activeId="activeId"></treeLi>
    </div>
    <div class="elbiaoge_elform tab_con" :style="{'min-height':tableConHeight+'px'}">
      <div class="tabs_ul">
        <div class="tabs_li" :class="{'active':activeTabs=='baseInfo'}" @click="setActiveTabs('baseInfo')">
          <img src="@/assets/images/zs.png" alt="" v-if="activeTabs=='baseInfo'">
          <span>基础信息</span>
        </div>
        <div class="tabs_li" :class="{'active':activeTabs=='equip'}" @click="setActiveTabs('equip')" v-if="activeVal&&activeVal.FacilityType">
          <img src="@/assets/images/zs.png" alt="" v-if="activeTabs=='equip'">
          <span>关联设备</span>
        </div>
        <div class="tabs_li" :class="{'active':activeTabs=='alarm'}" @click="setActiveTabs('alarm')" v-if="activeVal&&activeVal.FacilityType">
          <img src="@/assets/images/zs.png" alt="" v-if="activeTabs=='alarm'">
          <span>告警配置</span>
        </div>
      </div>
      <div class="cont_con">
        <facilityBase ref="facilityBase" :groupTreeList="orginTreeList" @cancelAdd="cancelAdd" v-show="activeTabs=='baseInfo'" @loadList="cancelAdd(),loadTreeList()"></facilityBase>
        <equipTable ref="equipTable" v-show="activeTabs=='equip'" v-if="activeVal&&activeVal.FacilityType"></equipTable>
      </div>
    </div>
    <groupAdd ref="groupAdd" :parentId="activeId" @loadList="loadTreeList" :groupTreeList="orginTreeList"></groupAdd>

  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import treeLi from '@/views/business/compontent/tree_li'
import facilityBase from '@/views/business/compontent/facility_base'
import equipTable from '@/views/business/compontent/equipTable'
import groupAdd from '@/views/business/compontent/group_add'
import {facilityTree,removeFacility} from '@/api/energy/facility'
export default {
  name: 'EnergyAdminUIfacility',
  mixins: [resizeTableCon],
  components:{treeLi,facilityBase,groupAdd,equipTable},
  data() {
    return {
      groupKey:'',
      treeList:[],
      orginTreeList:[],
      activeTabs:'baseInfo',
      activeId:'',
      activeVal:null,
      groupmap:new Map(),
      setDefaultActive:null,
      treekey:1
    };
  },

  mounted() {
    this.setDefaultActive=null
    this.loadTreeList()
  },

  methods: {
    handleQuery(){
      let treeData=JSON.parse(JSON.stringify(this.orginTreeList))
      // if(this.groupKey){
      //   this.treeList=treeData.filter((row)=>row.FacilityName.indexOf(val)>-1)
      // }else{
      //   this.treeList=JSON.parse(JSON.stringify(this.orginTreeList))
      // }
      this.treeList=this.filterNodes(treeData,this.groupKey)
    },
    filterNodes(nodes, keyword) {
      return nodes.map(node => {
          // 如果有子节点，递归过滤子节点
          if (node.Children && node.Children.length) {
            const filteredChildren = this.filterNodes(node.Children, keyword);
            // 保留有匹配子节点的父节点
            // console.log(filteredChildren,'filteredChildren');
            if (filteredChildren.length > 0) {
              return { ...node, Children: filteredChildren };
            }else{
              node.Children=[]
            }
          }
          // 检查当前节点是否匹配关键字
          // console.log(node.FacilityName.indexOf(keyword)>-1,'node.FacilityName.indexOf(keyword)>-1');
          if (node.FacilityName.indexOf(keyword)>-1) {
            return node;
          }
          // 不匹配的节点过滤掉
          return null;
        })
        .filter(Boolean); // 过滤掉null值
    },
    handleTreeEdit(row){
      if(row&&row.FacilityType){
        this.$refs.facilityBase.handleAdd(row)
      }else{
        this.handleGroup(row)
      }
    },
    handleTreeDel(row){
      let text='分组'
      if(row.FacilityType){
        text='设施'
      }
      this.$modal.confirm('是否确认删除"'+text + row.FacilityName + '"？')
        .then(function () {
          return removeFacility({ Id: row.Id });
        })
        .then(() => {
          this.loadTreeList();
          this.$modal.msgSuccess("移除成功");
        })
        .catch(() => { });
    },
    loadTreeList(){
      //
      facilityTree({OrgId:this.$store.getters.orgId}).then(res=>{
        // console.log(res,'facilityTree');
        // this.treeList = res.data; //选择分类时分类树
        this.treeList = this.setSelDis(res.data); //选择分类时分类树
        this.orginTreeList= this.setSelDis(res.data);
        // console.log(this.treeList,'this.treeList',this.orginTreeList);
        this.initClassMap(this.treeList);
        this.treekey++
        this.cancelAdd()
      })
    },
    setSelDis(list){
      for(let i=0;i<list.length;i++){
        if(list[i].FacilityType){
          list[i].isDisabled =true
          if(!this.setDefaultActive){
            this.setDefaultActive=list[i]
            this.activeId=list[i].Id
            this.activeVal=list[i]
          }
        }else{
          if(list[i].Children&&list[i].Children.length>0){
            let isSelDis=list[i].Children.filter(row=>row.FacilityType)
            if(isSelDis&&isSelDis.length>0){
              list[i].isDisabled =true
            }else{
              list[i].isDisabled =false
            }
            
          }else{
            list[i].isDisabled =false
          }
        }
        
        if(list[i].Children&&list[i].Children.length>0){
          list[i].Children=this.setSelDis(list[i].Children)
        }
      }
      return list
    },
    initClassMap(node) {
      for (let idx = 0; idx < node.length; idx++) {
        let curnode = node[idx];
        // console.log("组合时用户列表curnode",curnode);
        this.groupmap.set(node[idx].Id, curnode);
        
        if (curnode.hasOwnProperty("Children") && curnode.Children && curnode.Children.length > 0) {
          this.initClassMap(curnode.Children);
        }
      }
    },
    finishChoice(val){
      this.activeTabs='baseInfo'
      this.activeId=val.Id
      this.activeVal=val
      let type=1
      if(val&&val.FacilityType){
        type=1
      }else{
        type=2
      }
      this.$refs.facilityBase.setDefaultVal(val,type)
    },
    handleGroup(info){
      //添加分组
      this.$refs.groupAdd.openDialog(info)
    },
    handleFacilities(){
      //添加设施
      if(this.activeId&&this.activeVal&&!this.activeVal.FacilityType){
        this.$refs.facilityBase.handleAdd(null,this.activeId)
      }else{
        this.$refs.facilityBase.handleAdd()
      }
      
    },
    cancelAdd(){
      if(this.setDefaultActive){
        this.$refs.facilityBase.setDefaultVal(this.setDefaultActive,1)
      }else{
        this.$refs.facilityBase.setShowNull()
      }
    },
    setActiveTabs(val){
      this.activeTabs=val 
      if(val=='equip'){
        this.$nextTick(()=>{
          if(this.activeVal&&this.activeVal.FacilityType){
            if(this.activeId){
              this.$refs.equipTable.loadData(this.activeId)
            }else{
              this.$refs.equipTable.setShowNull()
            }
          }
          
        })
      }
    },
  },
};
</script>

<style lang="scss" scoped>
#big_con{
  display: flex;
}
.elbiaoge_elform.tree_con{
  width: 240px;
  padding-left: 0;
  padding-right: 0;
  .set_radius{
    margin-top: 16px;
    ::v-deep .el-input__inner{
      height: 32px;
      line-height: 32px;
    }
  }
}
.elbiaoge_elform.tab_con{
  padding: 0 20px;
  width: 100%;
  box-sizing: border-box;
  .tabs_ul{
    width: 100%;
    display: flex;
    align-items: center;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    height: 55px;
    
    .tabs_li{
      line-height: 16px;
      display: flex;
      align-items: center;
      margin-right: 32px;
      cursor: pointer;
      img{
        width: 16px;
        height: 16px;
        margin-right: 8px;
      }
      span{
        font-size: 16px;
        color: rgba(255, 255, 255, 0.6);
      }
      &.active{
        span{
          color: rgba(255, 255, 255, 1);
        }
      }
    }
  }
  .cont_con{
    // width: calc(100% - 170px);
    width: 100%;
    padding-top: 24px;
  }
}
</style>