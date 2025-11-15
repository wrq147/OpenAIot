<template>
  <div>
    <el-form ref="addForm" :model="addForm" :rules="addRules" label-width="80px" v-if="!isShowNumm">
        <el-row :gutter="10">
            <el-col :span="12">
                <el-form-item :label="dataType==1?'设施名称':'分组名称'" prop="FacilityName">
                  <el-input :disabled="isOnlyView" type="text" v-model="addForm.FacilityName" placeholder="请输入设施名称"></el-input>
                </el-form-item>
            </el-col>
            <el-col :span="12" v-if="dataType==1">
                <el-form-item label="设施编号" prop="FacilityCode">
                  <el-input :disabled="isOnlyView" type="text" v-model="addForm.FacilityCode" placeholder="请输入设施编号"></el-input>
                </el-form-item>
            </el-col>
            <el-col :span="12">
                <el-form-item :label="dataType==1?'分组':'上级分组'" prop="ParentId" v-if="!isOnlyView||!addForm.Id||addForm.Id&&addForm.ParentId&&addForm.ParentId!='-'">
                  <treeselect @input="treeSelectChange" :multiple="false" :flat="true" :disabled="isOnlyView" v-model="addForm.ParentId" :options="groupTreeListOption" :show-count="true" :normalizer="normalizer" placeholder="请选择分组" />
                </el-form-item>
            </el-col>
            <el-col :span="12" v-if="dataType==1&&isOnlyView&&addForm.Manager||dataType==1&&!isOnlyView">
                <el-form-item label="负责人" prop="Manager">
                  <el-input :disabled="isOnlyView" type="text" v-model="addForm.Manager" placeholder="请输入负责人"></el-input>
                </el-form-item>
            </el-col>
            <el-col :span="12" v-if="dataType==1&&isOnlyView&&addForm.Contact||dataType==1&&!isOnlyView">
                <el-form-item label="联系方式" prop="Contact">
                  <el-input maxlength="11" :disabled="isOnlyView" type="text" v-model="addForm.Contact" placeholder="请输入联系方式"></el-input>
                </el-form-item>
            </el-col>
        </el-row>
        <el-row :gutter="10">
            <el-col :span="24">
              <div class="submit_btn_con" v-if="!isOnlyView">
                <el-button @click="cancelAdd">取消</el-button>
                <el-button type="primary" @click="submitForm" v-loading="saveLoading">保存</el-button>
              </div>
            </el-col>
        </el-row>
    </el-form>
  </div>
</template>

<script>
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import {addFacility,updateFacility,facilityCode} from '@/api/energy/facility'
export default {
  name: 'EnergyAdminUIFacilityBase',
  components: { Treeselect },
  props:{
    groupTreeList:{
      type:Array,
      default:()=>{
        return []
      }
    }
  },
  data() {
    return {
      addForm:{
        FacilityName:'',
        ParentId:null,
        Manager:'',
        Contact:'',
        FacilityCode:''
      },
      isOnlyView:false,
      saveLoading:false,
      addRules:{
        FacilityName: [{ required: true, trigger: "blur", message: "请输入设施名称" }],
        FacilityCode: [{ required: true, trigger: "blur", message: "请输入设施编码" }],
        ParentId: [{ required: true, trigger: "blur", message: "请选择分组" }],
        // Manager: [{ required: true, trigger: "blur", message: "请输入负责人" }],
        // Contact: [{ required: true, trigger: "blur", message: "请输入联系方式" }],
      },
      dataType:1,//1为设施，2为分组
      isShowNumm:false,
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
    treeSelectChange(val){
      //
    },
    setShowNull(){
      this.isShowNumm=true
    },
    async handleAdd(row,parentId){
      this.isShowNumm=false
      this.isOnlyView=false
      this.dataType=1
      if(row){
        this.addForm={
          FacilityName:row.FacilityName,
          ParentId:row.ParentId,
          Manager:row.Manager,
          Contact:row.Contact,
          Id:row.Id,
          FacilityCode:row.FacilityCode,
          FacilityType:row.FacilityType
        }
      }else{
        this.addForm={
          FacilityName:'',
          ParentId:parentId,
          Manager:'',
          Contact:'',
          FacilityCode:await this.loadfacilityCode(),
          FacilityType:true
        }
      }
      this.$forceUpdate()
      this.groupTreeListOption=this.setOptionDis(this.groupTreeList)
    },
    async loadfacilityCode(){
      let res=await facilityCode()
      return res.data
    },
    setDefaultVal(val,type){
      //设置默认数据
      this.dataType=type
      this.addForm={
        FacilityName:val.FacilityName,
        ParentId:val.ParentId,
        Manager:val.Manager,
        Contact:val.Contact,
        FacilityCode:val.FacilityCode,
        FacilityType:val.FacilityType,
        Id:val.Id
      }
      this.isOnlyView=true
      this.isShowNumm=false
    },
    cancelAdd(){
      //取消添加
      this.$emit('cancelAdd')
    },
    submitForm(){
      //添加保存
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
  },
};
</script>

<style lang="scss" scoped>

</style>