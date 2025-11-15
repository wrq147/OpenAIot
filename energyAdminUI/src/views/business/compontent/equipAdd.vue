<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="640px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>{{dialogTitle}}</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
      <div>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="设备编码" prop="EquipmentCode">
                <el-input :disabled="isOnlyView" type="text" v-model="addForm.EquipmentCode" placeholder="请输入设备编码"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="设备名称" prop="EquipmentName">
                <el-input :disabled="isOnlyView" type="text" v-model="addForm.EquipmentName" placeholder="请输入设备名称"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="第三方编码" prop="ThirdId">
                <el-select @focus="iotRemoteMethod()" @change="iotRemoteMethod" :disabled="isOnlyView" style="width:100%" v-model="addForm.ThirdId" clearable placeholder="请输入第三方编码" filterable remote reserve-keyword :remote-method="iotRemoteMethod" :loading="optionLoading">
                  <el-option :label="it.Name" :value="it.DeviceNumber" v-for="(it,ix) in iotOption" :key="'iot'+ix"/>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="关联计费标准" prop="PolicyId">
                <el-select @focus="FacilityRemoteMethod()" @change="FacilityRemoteMethod" :disabled="isOnlyView" style="width:100%" v-model="addForm.PolicyId" clearable placeholder="请输入计费标准名称" filterable remote reserve-keyword :remote-method="FacilityRemoteMethod" :loading="optionLoading">
                  <el-option :label="it.PolicyName" :value="it.Id" v-for="(it,ix) in FacilityOption" :key="'Facility'+ix"/>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="数据状态" prop="DataState">
                <el-select :disabled="isOnlyView" style="width:100%" v-model="addForm.DataState" clearable placeholder="请选择数据状态">
                  <el-option label="纳入能源计算" value="1" />
                  <el-option label="不纳入能源计算" value="2" />
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
        </div>
      </el-form>
    <div slot="footer" class="dialog-footer" v-if="!isOnlyView">
      <el-button @click="dialog = false">取消</el-button>
      <el-button type="primary" @click="submitForm" v-loading="saveLoading">保存</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { addEquipment, updateEquipment,equipmentCode } from '@/api/energy/equip'
// import {
//   DeviceList
// } from "@/api/rules/device";
import {myDeviceList} from "@/api/after/dev";
import {policyPageList} from '@/api/energy/Policy'
export default {
  name: 'EnergyAdminUIEquipAdd',

  data() {
    return {
      dialogTitle:'新增设备',
      dialog:false,
      isOnlyView:false,
      addForm:{
        EquipmentCode:'',
        EquipmentName:'',
        PolicyId:'',
        DataState:'1',
      },
      addRules:{
        // EquipmentCode: [{ required: true, trigger: "blur", message: "请输入设备编码" }],
        EquipmentName: [{ required: true, trigger: "blur", message: "请输入设备名称" }],
        ThirdId: [{ required: true, trigger: "blur", message: "请输入第三方编码" }],
      },
      saveLoading:false,
      FacilityForm:{
        pageNum:1,
        pageSize:10,
        orgId:this.$store.state.user.orgId,
      },//计费标准
      deviceForm:{
        pageNum: 1,
        pageSize: 10,
        Name: "",
      },
      FacilityOption:[],
      optionLoading:false,
      iotOption:[]
    };
  },

  mounted() {
    this.loadpolicyPageList()
    this.loadIotDevicePageList()
  },

  methods: {
    async loadIotDevicePageList(){
      let response=await myDeviceList(this.deviceForm);
      this.iotOption = JSON.parse(JSON.stringify(response.data.List));
      if(this.optionLoading){
        this.optionLoading=false
      }
    },
    async iotRemoteMethod(query){
      if (query !== "") {
        this.optionLoading = true;
        setTimeout(async () => {
          this.deviceForm.Key=query
          await this.loadIotDevicePageList()
          this.optionLoading = false;
        }, 200);
      } else {
        delete this.deviceForm.Key
        await this.loadIotDevicePageList()
      }
    },
    async loadpolicyPageList(){
      let response=await policyPageList(this.FacilityForm);
      this.FacilityOption = JSON.parse(JSON.stringify(response.data.List));
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
    async openDialog(info,isOnlyView){
      if(info){
        this.dialogTitle='修改设备'
        this.addForm={
          Id:info.Id,
          EquipmentCode:info.EquipmentCode,
          EquipmentName:info.EquipmentName,
          PolicyId:info.PolicyId,
          DataState:info.DataState,
          ThirdId:info.ThirdId//第三方编码
        }
      }else{
        this.dialogTitle='新增设备'
        this.addForm={
          EquipmentCode:await this.loadequipmentCode(),
          EquipmentName:'',
          PolicyId:'',
          DataState:'1',
          ThirdId:''
        }
      }
      if(isOnlyView){
        this.dialogTitle='设备详情'
        this.isOnlyView=isOnlyView
      }else{
        this.isOnlyView=false
      }
      this.dialog=true
    },
    async loadequipmentCode(){
      let res=await equipmentCode()
      return res.data
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
.adddialog{
  ::v-deep .el-dialog__header{
    padding: 0;
    color: #ffffff;
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