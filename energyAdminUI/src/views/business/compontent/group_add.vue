<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="400px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>新增分组</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
      <div>
          <el-row :gutter="20">
            <el-col :span="24">
              <el-form-item label="分组名称" prop="FacilityName">
                <el-input :disabled="isOnlyView" type="text" v-model="addForm.FacilityName" placeholder="请输入分组名称"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="24" v-if="addForm.Id&&addForm.ParentId!='-'">
              <el-form-item label="上级分组" prop="ParentId">
                <treeselect style="width:100%" @input="treeSelectChange" :multiple="false" :flat="true" v-model="addForm.ParentId" :options="groupTreeListOption" :show-count="true" :normalizer="normalizer" placeholder="请选择分组" />
              </el-form-item>
            </el-col>
          </el-row>
        </div>
      </el-form>
    <div slot="footer" class="dialog-footer">
      <el-button @click="dialog = false">取消</el-button>
      <el-button type="primary" @click="submitForm" v-loading="saveLoading">保存</el-button>
    </div>
  </el-dialog>
</template>

<script>
import {addFacility,updateFacility} from '@/api/energy/facility'
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
export default {
  name: 'EnergyAdminUIgroupAdd',
  components: { Treeselect },
  props:{
    parentId:{
        type:String,
        default:''
    },
    groupTreeList:{
      type:Array,
      default:()=>{
        return []
      }
    }
  },
  data() {
    return {
      dialog:false,
      isOnlyView:false,
      addForm:{
        FacilityName:'',
        Manager:'-'
      },
      addRules:{
        FacilityName: [{ required: true, trigger: "blur", message: "请输入分组名称" }],
      },
      saveLoading:false,
      groupTreeListOption:[]
    };
  },
  watch:{
    groupTreeList:{
      handler(val){
        let listOptions=JSON.parse(JSON.stringify(val))
        listOptions=this.setOptionDis(listOptions)
        this.groupTreeListOption=JSON.parse(JSON.stringify(listOptions))
        // console.log(this.groupTreeListOption,'this.groupTreeListOption');
      },
      immediate:true,
      deep:true
    }
  },
  mounted() {
  },

  methods: {
    setOptionDis(lis,isallDis){
      let list=JSON.parse(JSON.stringify(lis))
      list=list.map(row=>{
        if(isallDis){
          row.isDisabled=true
        }else{
          if(row.FacilityType){
            row.isDisabled=true
          }else{
            if(row.Id==this.addForm.Id){
              row.isDisabled=true
            }else{
              row.isDisabled=false
            }
          }
        }
        if(row.Children&&row.Children.length>0){
          if(row.Id==this.addForm.Id){
            row.Children=this.setOptionDis(row.Children,true)
          }else{
            row.Children=this.setOptionDis(row.Children,false)
          }
          
        }
        return row
      })
      return list
    },
    normalizer(node) {
      if (node.Children == null || !node.Children.length) {
        delete node.Children;
      }
      return {
        id: node.Id,
        label: node.FacilityName,
        isDisabled: node.isDisabled,
        children: node.Children
      };
    },
    treeSelectChange(val){
      //
    },
    openDialog(info,isOnlyView){
      if(info){
        this.addForm={
          Id:info.Id,
          FacilityName:info.FacilityName,
          Manager:info.Manager,
          Contact:info.Contact,
          ParentId:info.ParentId,
          FacilityType:info.FacilityType
        }
      }else{
        this.addForm={
          FacilityCode:'-',
          FacilityName:'',
          Manager:'-',
          Contact:'-',
          ParentId:this.parentId,
          FacilityType:false
        }
      }
      if(isOnlyView){
        this.isOnlyView=isOnlyView
      }else{
        this.isOnlyView=false
      }
      this.groupTreeListOption=this.setOptionDis(this.groupTreeList)
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
            updateFacility(submitForm).then(response => {

              if (response.code == 0) {
                this.$message.success("修改分组成功");
                this.dialog = false;
                this.$emit('loadList')
                this.saveLoading = false;
              }
            }).catch(err => {
              this.saveLoading = false;
            });
          } else {
            submitForm.orgId=this.$store.getters.orgId
            addFacility(submitForm).then(response => {

              if (response.code == 0) {
                this.$message.success("添加分组成功");
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