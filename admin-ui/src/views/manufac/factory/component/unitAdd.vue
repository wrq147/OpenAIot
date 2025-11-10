<template>
  <el-dialog title="添加单位" :visible.sync="dialogVisible" width="800px" :show-close="true" @before-close="handleClose">
    <el-form ref="form" :model="form" label-width="100px" :rules="rules">
      <el-row :gutter="10">
        <el-col :span="24">
          <el-form-item label="单位名称" prop="UnitName">
            <el-input v-model="form.UnitName" placeholder="请输入单位名称"></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="备注说明" prop="Remark">
            <el-input type="textarea" v-model="form.Remark" placeholder="请输入备注"></el-input>
          </el-form-item>
        </el-col>
      </el-row>
      
      
    </el-form>
    <span slot="footer" class="dialog-footer">
      <el-button @click="dialogVisible = false">取 消</el-button>
      <el-button type="primary" @click="submitFiledAdd">确 定</el-button>
    </span>
  </el-dialog>
</template>

<script>
import {addUnitSave,factoryUnitInfo,editUnitSave} from '@/api/factory/unit'
export default {
  name: 'AdminUiProductAdd',

  data() {
    return {
      dialogVisible:false,
      form:{
        UnitName:'',
        Remark:'',//备注说明
      },
      rules: {
        UnitName: [{ required: true, trigger: "blur", message: "单位名称不能为空" }],
      },
    };
  },

  mounted() {
    
  },

  methods: {
    
    async openDialog(id){
        this.form={
            UnitName:'',
            Remark:'',//备注说明
        }
      if(id){
        let res=await factoryUnitInfo({id:id})
        console.log('unitInfo,单位详情',res);
        let unitInfo=res.data
        this.form={
          Id:unitInfo.Id,
          UnitName:unitInfo.UnitName,
          Remark:unitInfo.Remark,//备注说明
        }
      }
      this.dialogVisible=true
    },
    submitFiledAdd(){
      //提交数据
      this.$refs["form"].validate(valid => {
        console.log("检验");
        if(valid){
          let submitForm=JSON.parse(JSON.stringify(this.form))
          console.log("提交的数据",submitForm);
          if(submitForm.Id){
            editUnitSave(submitForm).then(res=>{
              console.log("修改执行结果",res);
              this.$modal.msgSuccess("修改成功");
              this.dialogVisible=false
              this.$emit('reloadData')
            })
          }else{
            addUnitSave(submitForm).then(res=>{
              console.log("添加执行结果",res);
              this.$modal.msgSuccess("添加成功");
              this.dialogVisible=false
              this.$emit('reloadData')
            })
          }
          
        }
      })
    },
    handleClose(){
      this.dialogVisible=false
    },
  },
};
</script>

<style lang="less" scoped>
.dialog_slot_title{
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  width: 100%;
  position: relative;
  .el-tabs{
    width: 100%;
  }
  .title_text{
    position: absolute;
    left: 0px;
    z-index: 9;
  }
  .icon_con{
    position: absolute;
    right: 0px;
    cursor: pointer;
    z-index: 9;
  }
}
</style>