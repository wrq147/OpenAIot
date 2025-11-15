<template>
  <div>
    <div class="tree_li" :class="{'open_li':treeLi.isopen,'active_li':activeId==treeLi[idFiled]}" @click="choiceHandle(treeLi,canChoice,activeInx)" :style="{'padding-left':paddingLeft+6+'px','align-items':alignItems,'background':hasActiveLiBg?'':'transparent'}">
      <div class="tree_li_left">
        <i class="zhongtaiiconfont zhongtai-icon-xiayiye" @click.stop="setOpenVal()" v-if="treeLi&&treeLi[childrenFiled]&&treeLi[childrenFiled].length>0"></i>
        <span v-else style="width:3px"></span>
        <span>{{treeLi[nameFiled]}}</span>
      </div>
      <div class="tree_li_right" v-if="isShowEdit||isShowDel">
        <i class="zhongtaiiconfont zhongtai-icon-gengduo" v-if="isShowEdit||isShowDel"></i>
        <div class="right_handle">
          <div class="handle_li" @click.stop="handleTreeEdit(treeLi)" v-if="isShowEdit">
            <i class="zhongtaiiconfont zhongtai-icon-xiugai1"></i>
            <span>编辑</span>
          </div>
          <div class="handle_li" @click.stop="handleTreeDel(treeLi)" v-if="isShowDel">
            <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
            <span>删除</span>
          </div>
        </div>
      </div>
    </div>
    <template v-if="treeLi&&treeLi.isopen&&treeLi[childrenFiled]&&treeLi[childrenFiled].length>0">
      <treeLi :activeInx="activeInx" :canChoice="!isAllChoice&&!item[childrenFiled]||!isAllChoice&&item[childrenFiled]&&item[childrenFiled].length==0||isAllChoice" :hasActiveLiBg="hasActiveLiBg" :alignItems="alignItems" ref="tree" :nameFiled="nameFiled" :isShowEdit="isShowEdit" :isShowDel="isShowDel" :childrenFiled="childrenFiled" :idFiled="idFiled" :filterText="filterText" @handleTreeEdit="handleTreeEdit" @handleTreeDel="handleTreeDel" :activeId="activeId" :treeLi="item" v-for="item in treeLi[childrenFiled]" :key="item.Id" @choice="choiceHandle" :paddingLeft="paddingLeft+6"></treeLi>
    </template>
  </div>
</template>

<script>
export default {
  name: 'TreeLi',
  props:{
    treeLi:{
      type:Object,
      default:()=>{
        return {}
      }
    },
    activeId:{
      type:String,
      default:''
    },
    paddingLeft:{
      type:Number,
      default:14
    },
    filterText:{
      type:String,
      default:''
    },
    isShowEdit:{
      type:Boolean,
      default:true
    },
    isShowDel:{
      type:Boolean,
      default:true
    },
    alignItems:{
      type:String,
      default:'center'
    },
    hasActiveLiBg:{
      type:Boolean,
      default:true
    },
    nameFiled:{
      type:String,
      default:'FacilityName'
    },
    childrenFiled:{
      type:String,
      default:'Children'
    },
    idFiled:{
      type:String,
      default:'Id'
    },
    canChoice:{
      type:Boolean,
      default:true
    },
    isAllChoice:{
      type:Boolean,
      default:true
    },
    activeInx:{
      type:Number,
      default:-1
    }
  },
  data() {
    return {
    };
  },
  watch: {
  },
  mounted() {
    this.$set(this.treeLi,'isopen',true)
  },

  methods: {
    handleTreeEdit(row){
      this.$emit('handleTreeEdit',row)
    },
    handleTreeDel(row){
      this.$emit('handleTreeDel',row)
    },
    choiceHandle(val,canChoice,activeInx){
      if(canChoice){
        this.$emit('choice',val,canChoice,activeInx)
      }
      
    },
    setOpenVal(){
      if(this.treeLi.isopen){
        // this.treeLi.isopen=false
        this.$set(this.treeLi,'isopen',false)
      }else{
        // this.treeLi.isopen=true
        this.$set(this.treeLi,'isopen',true)
      }
    }
  },
};
</script>

<style lang="scss" scoped>
.tree_li{
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 36px;
  padding: 0 20px;
  cursor: pointer;
  color: rgba(255, 255, 255, 1);
  &.open_li{
    .zhongtaiiconfont.zhongtai-icon-xiayiye{
      // transform-origin: top left;
      transform: rotate(90deg);
    }
  }
  &.active_li{
    background: rgba(61, 185, 143, 0.2);
    color: rgba(61, 185, 143, 1);
  }
  .tree_li_left{
    display: flex;
    align-items: center;
    cursor: pointer;
    .zhongtaiiconfont.zhongtai-icon-xiayiye{
      font-size: 10px;
    }
    span{
      font-size: 14px;
      
      margin-left: 8px;
    }
    
  }
  .tree_li_right{
    position: relative;
    .zhongtaiiconfont.zhongtai-icon-gengduo{
      font-size: 10px;
    }
    &:hover{
      .right_handle{
        display: block;
      }
    }
    .right_handle{
      position: absolute;
      z-index: 9;
      right: -22px;
      width: 62px;
      border-radius: 4px;
      background: rgba(34, 46, 64, 1);
      box-shadow: 0px 10px 20px 0px rgba(0,0,0,0.4);
      display: none;
      .handle_li{
        display: flex;
        align-items: center;
        font-size: 12px;
        color: rgba(255, 255, 255, 1);
        height: 30px;
        justify-content: center;
        i{
          font-size: 11px;
          color: rgba(255, 255, 255, 1);
          margin-right: 7px;
          cursor: pointer;
        }
      }
    }
  }
  // &:hover .tree_li_right{
  //   .right_handle{
  //     display: block;
  //   }
  // }
  i{
    color: rgba(255, 255, 255, 0.6);
  }
}
</style>