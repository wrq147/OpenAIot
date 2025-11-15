<template>
  <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="handOpen" width="900px" append-to-body @close="close">
    <div class="hand_con">
      <div class="hand_content" v-if="taskContent&&taskContent.length>0">
        <div class="hand_content_li" v-for="(item,inx) in taskContent" :key="'a'+inx">
          <div class="cont_ftitle">
            <div class="num">{{inx+1}}</div>
            <div class="ftitle_text">{{item.title}}</div>
          </div>
          <div class="stitle">>数据准备<div class="unico" v-if="item.isAllFinish"></div></div>
          <div class="content_li_con">
            <div class="content_li" v-for="(it,index) in item.arr" :key="'a'+inx+index">
              <div class="li_text">{{it.name}}</div>
              <i class="el-icon-loading" v-if="!it.isfinish" ></i>
              <div class="li_tips" v-if="it.isfinish&&!it.errMsg">成功</div>
              <el-tooltip placement="top" v-if="it.errMsg"><div slot="content">{{it.errMsg}}</div><div class="li_tips error">失败</div></el-tooltip>
            </div>
          </div>
        </div>
      </div>
    </div>
  </el-dialog>
</template>

<script>
import {corpTaskInfo} from '@/api/system/sync.js';
export default {
  name: 'AdminUiHandSync',

  data() {
    return {
      title:'同步动态',
      handOpen:false,
      taskContent:[],
      timer:null,
    };
  },

  mounted() {
    
  },

  methods: {
    handleInfo(info){
      this.handOpen=true
      this.setSyncInfo(info)
    },
    openDialog(id){
      this.loadSyncInfo(id)
      this.timer=setInterval(()=>{
        this.loadSyncInfo(id)
      },3000)
    },
    close(){
      clearInterval(this.timer)
    },
    loadSyncInfo(id){
      corpTaskInfo(id).then(res=>{
        let info=res.data
        this.handOpen=true
        this.setSyncInfo(info)
      })
      
    },
    setSyncInfo(info){
      this.taskContent=[]
      let deptInfoArr={
        arr:[
          {
            name:'更新部门',
            isfinish:info.IsUpdateDept,
            errMsg:info.UpdateDeptErr
          },
          {
            name:'新增部门',
            isfinish:info.IsAddDept,
            errMsg:info.AddDeptErr
          },
          {
            name:'移动部门',
            isfinish:info.IsMoveDept,
            errMsg:info.MoveDeptErr
          },
          {
            name:'删除部门',
            isfinish:info.IsDelDept,
            errMsg:info.DelDeptErr
          },
        ],
        title:'同步部门',
        isAllFinish:false
      }
      if(info.IsUpdateDept&&info.IsAddDept&&info.IsMoveDept&&info.IsDelDept){
        deptInfoArr.isAllFinish=true
      }else{
        deptInfoArr.isAllFinish=false
      }
      let menInfoArr={
        arr:[
          {
            name:'更新人员',
            isfinish:info.IsUpdateMem,
            errMsg:info.UpdateMemErr
          },
          {
            name:'新增人员',
            isfinish:info.IsAddMem,
            errMsg:info.AddMemErr
          },
          {
            name:'删除人员',
            isfinish:info.IsDelMem,
            errMsg:info.DelMemErr
          },
        ],
        title:'同步人员',
        isAllFinish:false
      }
      if(info.IsUpdateMem&&info.IsAddMem&&info.IsDelMem){
        menInfoArr.isAllFinish=true
      }else{
        menInfoArr.isAllFinish=false
      }
      if(info.IsUpdateDept&&info.IsAddDept&&info.IsMoveDept&&info.IsDelDept&&info.IsUpdateMem&&info.IsAddMem&&info.IsDelMem){
        clearInterval(this.timer)
      }
      this.taskContent.push(deptInfoArr)
      this.taskContent.push(menInfoArr)
    }
  },
};
</script>
<style lang="less" scoped>
.hand_con{
  width: 100%;
}
::v-deep .el-dialog__body{
  padding-top: 10px;
}
.hand_content{
  background: #EEEEEE;
  padding: 13px;
  box-sizing: border-box;
  border-radius: 5px;
  .hand_content_li{
    background: #ffffff;
    border-radius: 5px;
    padding: 7px 10px;
    width: 100%;
    box-sizing: border-box;
    margin-top: 13px;
    &.hand_content_li:first-child{
      margin-top: 0;
    }
    .cont_ftitle{
      display: flex;
      align-items: center;
      line-height: 20px;
      margin-bottom: 5px;
      .num{
        width: 20px;
        height: 20px;
        color: #ffffff;
        font-size: 16px;
        font-weight: bold;
        background: #2DB7F5;
        align-content: center;
        text-align: center;
        border-radius: 50%;
        margin-right: 5px;
      }
      .ftitle_text{
        font-size: 20px;
        color: #333333;
        font-weight: bold;
      }
    }
    .content_li_con{
      display: flex;
      justify-content: flex-start;
      align-items: center;
      flex-wrap: wrap;
      width: 100%;
      border-top: 0.5px solid #F1F1F1;
      .content_li{
        font-size: 12px;
        display: flex;
        align-items: center;
        width: 25%;
        line-height: 32px;
        
        .li_text{
          color: #666666;
          margin-right: 5px;
        }
        .li_tips{
          color: #2FB7F5;
          
          &.error{
            color: #FF2929;
          }
          span{
            color: #2FB7F5;
          }
        }
      }
    }
  }
}
.stitle{
  display: flex;
  justify-content: flex-start;
  align-items: center;
  color: #333333;
  .unico {
    width: 28px;
    height: 28px;
    line-height: 26px;
    border: 1px solid #F9FAFC;
    background-color: #F9FAFC;
    position: relative;
    cursor: pointer;
  }
  .unico::before {
    position: absolute;
    content: " ";
    width: 7px;
    height: 13px;
    transform: rotate(45deg);
    border-right: 3px solid #2DB7F5;
    border-bottom: 3px solid #2DB7F5;
    top: 4px;
    left: 9px;
  }
}

</style>