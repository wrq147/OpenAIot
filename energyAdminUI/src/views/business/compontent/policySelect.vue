<template>
  <el-dialog :visible.sync="dialog" class="policy_select" :show-close="false" width="400px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>关联计费标准</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" label-position="top" label-width="90px">
      <div>
          <el-row :gutter="20">
            <el-col :span="24">
              <el-form-item label="" prop="PolicyId">
                <el-select @change="FacilityRemoteMethod" style="width:100%" v-model="addForm.PolicyId" clearable placeholder="请输入计费标准名称" filterable remote reserve-keyword :remote-method="FacilityRemoteMethod" :loading="optionLoading">
                  <el-option :label="it.PolicyName" :value="it.Id" v-for="(it,ix) in PolicyOption" :key="'Policy'+ix"/>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
        </div>
      </el-form>
    <div slot="footer" class="dialog-footer">
      <el-button style="width:100%;height:48px;font-size:16px;" type="primary" @click="submitForm" v-loading="saveLoading">确定</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { addEquipment, updateEquipment,equipmentCode } from '@/api/energy/equip'
import {policyPageList} from '@/api/energy/Policy'
export default {
  name: 'EnergyAdminUIPolicySelect',
  props:{
    parentId:{
        type:String,
        default:''
    }
  },
  data() {
    return {
      dialog:false,
      optionLoading:false,
      addForm:{
        EquipmentCode:'',
        EquipmentName:'',
        PolicyId:'',
        DataState:'1',
      },
      saveLoading:false,
      PolicyOption:[],
      FacilityForm:{
        pageNum:1,
        pageSize:10,
        orgId:this.$store.state.user.orgId,
      },//计费标准
    };
  },

  mounted() {
    this.loadpolicyPageList()
  },

  methods: {
    async loadpolicyPageList(){
      let response=await policyPageList(this.FacilityForm);
      this.PolicyOption = JSON.parse(JSON.stringify(response.data.List));
      if(this.optionLoading){
        this.optionLoading=false
      }
    },
    async FacilityRemoteMethod(query){
      if (query !== "") {
        this.optionLoading = true;
        setTimeout(async () => {
          this.FacilityForm.policyName=query
          await this.loadpolicyPageList()
          this.optionLoading = false;
        }, 200);
      } else {
        delete this.FacilityForm.policyName
        await this.loadpolicyPageList()
      }
    },
    async openDialog(info){
      if(info){
        this.dialogTitle='关联计费标准'
        this.addForm={
          Id:info.Id,
          EquipmentCode:info.EquipmentCode,
          EquipmentName:info.EquipmentName,
          PolicyId:info.PolicyId,
          DataState:info.DataState,
        }
      }
      this.dialog=true
    },
    closeDialog(){
      this.dialog=false
    },
    submitForm(){
      this.$refs["addForm"].validate(valid => {
        if (valid) {
          // let submitTag=[]
          let submitForm = JSON.parse(JSON.stringify(this.addForm))
          // submitForm.MarketTime=this.addForm
          this.saveLoading = true;
          if (this.addForm.Id) {
            updateEquipment(submitForm).then(response => {

              if (response.code == 0) {
                this.$message.success("修改设备成功");
                this.dialog = false;
                this.$emit('loadList')
                this.saveLoading = false;
              }
            }).catch(err => {
              this.saveLoading = false;
            });
          } else {
            submitForm.orgId=this.$store.getters.orgId
            addEquipment(submitForm).then(response => {

              if (response.code == 0) {
                this.$message.success("添加设备成功");
                this.dialog = false;
                this.$emit('loadList')
                this.saveLoading = false;
              }
            }).catch(err => {
              this.saveLoading = false;
            });
          }
        }
      });
    }
  },
};
</script>

<style lang="less" scoped>
.policy_select{
  ::v-deep .el-dialog__header{
    padding: 0;
    color: #ffffff;
  }
  ::v-deep .el-dialog__body{
    padding: 24px 40px 0;
  }
  ::v-deep .el-dialog__footer{
    padding: 0 40px 40px;
  }
  ::v-deep .el-form-item{
    margin-bottom: 24px;
  }
  ::v-deep .el-select{
    .el-input{
        .el-input__inner{
            height: 48px;
            
        }
    }
  }
}

.dialog_title{
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  .dialog_title_left{
    font-size: 16px;
    color: #ffffff;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    padding-left: 20px;
    line-height: 16px;
    height: 56px;
    .img{
      width: 16px;
      height: 16px;
    }
    span{
      margin-left: 6px;
    }
  }
  .dialog_title_right{
    margin-right: 20px;
    cursor: pointer;
    i.zhongtaiiconfont{
      color: rgba(255, 255, 255, 0.60);
      font-size: 12px;
    }
  }
}
</style>