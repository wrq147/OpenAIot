<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div class="elbiaoge_elform tree_con" :style="{'min-height':tableConHeight+'px','margin-right':'10px'}">
      <div class="elform_title">
        <div class="elform_title_left">
          <img src="@/assets/images/zs.png" alt="">
          <span>基础信息</span>
        </div>
        <div class="right_time">最近保存时间：2025/08/08 20:00:00</div>
      </div>
      <el-form ref="orgForm" :model="addForm" :rules="addRules" label-width="112px">
        <el-row :gutter="80" style="width:calc(100% - 184px)">
          <el-col :span="12">
            <el-form-item label="企业名称" prop="OrgName">
              <el-input :disabled="isOnlyView" type="text" v-model="addForm.OrgName" placeholder="请输入设施名称"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="行业类型" prop="Industry">
              <el-cascader v-model="addForm.industryArr" placeholder="请选择行业类型" :props="{value:'Id', label: 'Name', children: 'children'}" :options="industryLis" @change="choiceIndustry" style="width:100%"></el-cascader>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系人" prop="Manager">
              <el-input :disabled="isOnlyView" type="text" v-model="addForm.Manager" placeholder="请输入联系人"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系电话" prop="Contact">
              <el-input maxlength="11" :disabled="isOnlyView" type="text" v-model="addForm.Contact" placeholder="请输入联系方式"></el-input>
            </el-form-item>
          </el-col>
          
          <el-col :span="12">
            <el-form-item label="生产地址" prop="province">
              <el-row :gutter="8" style="display:flex;">
                  <el-col :span="8">
                    <el-select class="set_radius" style="width:100%" v-model="addForm.province" placeholder="省" clearable @change="provinceChange()">
                      <el-option :label="item.Name" :value="item.Id" v-for="item in areaTreeList" :key="item.Id"/>
                    </el-select>
                  </el-col>
                  <el-col :span="8">
                    <el-select :disabled="!addForm.province" class="set_radius" style="width:100%" v-model="addForm.city" placeholder="市" clearable @change="cityChange()">
                      <el-option :label="item.Name" :value="item.Id" v-for="item in cityList" :key="item.Id"/>
                    </el-select>
                  </el-col>
                  <el-col :span="8">
                    <el-select :disabled="!addForm.city" class="set_radius" style="width:100%" v-model="addForm.district" placeholder="区" clearable @change="districtChange()">
                      <el-option :label="item.Name" :value="item.Id" v-for="item in districtList" :key="item.Id"/>
                    </el-select>
                  </el-col>
              </el-row>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="详细地址" prop="AddressDetail">
              <el-input :disabled="isOnlyView" type="text" v-model="addForm.AddressDetail" placeholder="请输入详细地址"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="有功电能标识" prop="ElecCode">
              <el-input :disabled="isOnlyView" type="text" v-model="addForm.ElecCode" placeholder="请输入标识"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="有功功率标识" prop="ElecPowerCode">
              <el-input maxlength="11" :disabled="isOnlyView" type="text" v-model="addForm.ElecPowerCode" placeholder="请输入标识"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        
      </el-form>
      <div class="elform_title">
        <div class="elform_title_left">
          <img src="@/assets/images/zs.png" alt="">
          <span>企业生产能源</span>
          <div class="line"></div>
          <div class="choice_con">
            <div class="choice_label">已选</div>
            <template v-for="(ite,inx) in selectFactors">
              <div class="choice_li" :key="'sel'+ite.Id" v-if="ite&&ite.FactorName">
                <span>{{ite.FactorName}}</span>
                <i class="zhongtaiiconfont zhongtai-icon-guanbi" @click="setDelSelectFactor(inx,ite)"></i>
              </div>
            </template>
          </div>
        </div>
        <div class="right_tips" @click.stop="opencoefficient">
          <i class="zhongtaiiconfont zhongtai-icon-tishi"></i>
          <span>各种能源折标准煤系数（参考值）</span>
        </div>
      </div>
      <div class="energy_ul">
        <div class="energy_li" v-for="(ite,x) in FactorTreeList" :key="ite.Id">
          <div class="energy_li_label">
            <!-- <div class="line"></div>
            <span>{{ite.TypeName}}：</span> -->
            <template>
              <treeLi :isAllChoice="true" :activeInx="x" :canChoice="!ite.Children||ite.Children&&ite.Children.length==0" :hasActiveLiBg="false" :isShowEdit="false" :isShowDel="false" :key="'enerytreekey'+item.Id" @choice="finishChoice" ref="treeLi" nameFiled="TypeName" :treeLi="item" v-for="item in [ite]" :activeId="activeIds[x]"></treeLi>
            </template>
          </div>
          <div class="energy_list">
            <template v-if="activeIds[x]&&returnChildrenList(activeIds[x])&&returnChildrenList(activeIds[x]).Children&&returnChildrenList(activeIds[x]).Children.length>0">
              <div class="energy_list_li_con" v-for="(item,ix) in returnChildrenList(activeIds[x]).Children" :key="ite.Id+item.Id">
                <div class="energy_list_li" :class="{'alllong':item.Factors&&item.Factors.length>0,'choice':twoTypeIds.includes(item.Id),'none':!twoTypeIds.includes(item.Id)}">
                  <div class="icon_con" @click.stop="ChoiceTwoType(item,false,[],false)">
                    <i class="zhongtaiiconfont zhongtai-icon-gouxuan" v-if="twoTypeIds.includes(item.Id)"></i>
                  </div>
                  <div class="li_text">{{item.TypeName}}</div>
                </div>
                <div class="radio_list" v-if="item.Factors&&item.Factors.length>0&&twoTypeIds.includes(item.Id)||item.Factors&&item.Factors.length>0&&ix==0">
                  <template v-for="ite in item.Factors">
                    <div class="radio_li" :key="ite.Id" :class="{'choice':selectFactorIds.includes(ite.Id),'none':!selectFactorIds.includes(ite.Id)}">
                      <!-- <i class="zhongtaiiconfont zhongtai-icon-a-xuanzhongdanxuan" @click.stop="ChoiceSelectFactor(ite,true,item.Factors,false)" v-if="selectFactorIds.includes(ite.Id)"></i>
                      <i class="zhongtaiiconfont zhongtai-icon-a-daixuanzedanxuan" @click.stop="ChoiceSelectFactor(ite,true,item.Factors,false)" v-else></i> -->
                      <div class="icon_con" @click.stop="ChoiceSelectFactor(ite,false,item.Factors,false,item)">
                        <i class="zhongtaiiconfont zhongtai-icon-gouxuan" v-if="selectFactorIds.includes(ite.Id)"></i>
                      </div>
                      <span>{{ite.FactorName}}</span>
                    </div>
                  </template>
                </div>
              </div>
            </template>
            <template v-else>
              <div class="energy_list_li" v-for="item in ite.Factors" :key="ite.Id+item.Id" :class="{'choice':selectFactorIds.includes(item.Id),'none':!selectFactorIds.includes(item.Id)&&item.FactorName.indexOf('电力')>-1||!selectFactorIds.includes(item.Id)&&item.FactorName.indexOf('天然气')>-1,'disable':item.FactorName.indexOf('电力')==-1&&item.FactorName.indexOf('天然气')==-1}">
                <div class="icon_con" @click.stop="ChoiceSelectFactor(item,false,[],item.FactorName.indexOf('电力')==-1&&item.FactorName.indexOf('天然气')==-1)">
                  <i class="zhongtaiiconfont zhongtai-icon-gouxuan" v-if="selectFactorIds.includes(item.Id)"></i>
                  <i class="zhongtaiiconfont zhongtai-icon-bukexuan" v-if="item.FactorName.indexOf('电力')==-1&&item.FactorName.indexOf('天然气')==-1"></i>
                </div>
                <div class="li_text">{{item.FactorName}}</div>
              </div>
            </template>
          </div>
        </div>
      </div>
      <el-row :gutter="10">
          <el-col :span="24">
            <div class="submit_btn_con" v-if="!isOnlyView">
              <el-button @click="cancelAdd">取消</el-button>
              <el-button type="primary" @click="orgSubmit" v-loading="saveLoading">保存</el-button>
            </div>
          </el-col>
        </el-row>
    </div>
    <coefficient ref="coefficient"></coefficient>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { orgInfo, editCompany } from "@/api/system/company";
import {selectFactorOrg,saveFactorOrg,SelectOrgAllFactorList,} from "@/api/energy/factorLibrary";
import {orgConfSave,orgConfInfo} from "@/api/energy/org";
import coefficient from './compontent/coefficient.vue'
import treeLi from '@/views/business/compontent/tree_li'
export default {
  name: 'EnergyAdminUIArchives',
  mixins: [resizeTableCon],
  components:{coefficient,treeLi},
  data() {
    return {
      activeIds:[],
      activeVal:[],
      twoTypeIds:[],
      selectFactorIds:[],
      selectFactors:[],
      addForm:{},
      isOnlyView:false,
      addRules:{
        OrgName: [{ required: true, trigger: "blur", message: "请输入企业名称" }],
        Industry: [{ required: true, trigger: "blur", message: "请选择行业" }],
        Manager: [{ required: true, trigger: "blur", message: "请输入联系人" }],
        Contact: [{ required: true, trigger: "blur", message: "请输入联系方式" }],
        ElecCode: [{ required: true, trigger: "blur", message: "请输入有功电能标识" }],
        ElecPowerCode: [{ required: true, trigger: "blur", message: "请输入有功功率标识" }],
        province: [{ required: true, trigger: "change", message: "请完善生产地址" }],
      },
      industryArr:[],
      industryLis:[],
      saveLoading:false,
      areaTreeList:[],//地域地址列表
      cityList:[],//市列表
      districtList:[],//区列表
      orgId:'',
      FactorTreeList:[],//企业所有的排放因子
      EnergyTypeParent:new Map(),
    };
  },

  mounted() {
    this.orgId = this.$store.state.user.orgId;
    this.getDatas()
    this.getOrgInfo()
    this.loadFactorTreeList()
  },

  methods: {
    returnChildrenList(id){
      let rowdata=this.EnergyTypeParent.get(id)
      // console.log(rowdata,'rowdatarowdata');
      return rowdata
    },
    finishChoice(val,can,inx){
      // console.log(val,can,inx,'val,can,inx');
      if(inx>-1){
        this.activeIds[inx]=val.Id
        this.activeVal[inx]=val
      }
      this.$forceUpdate()
    },
    opencoefficient(){
      this.$refs.coefficient.openDialog()
    },
    ChoiceSelectFactor(row,isSingle,list,isdis,energyType){
      //设置选择或者取消选择因子
      if(isdis){
        return
      }
      let findx=this.selectFactorIds.findIndex(rw=>rw==row.Id)
      // console.log("findx",findx);
      if(isSingle){
        if(findx>-1){}else{
          let isSingleChoice=null
          this.selectFactorIds.map(ro=>{
            let roFind=list.find(rw=>rw.Id==ro)
            if(roFind){
              isSingleChoice=ro
            }
          })
          if(isSingleChoice){
            let findSinglex=this.selectFactorIds.findIndex(rw=>rw==isSingleChoice)
            this.selectFactorIds.splice(findSinglex,1)
            this.selectFactors.splice(findSinglex,1)
          }
          this.selectFactorIds.push(row.Id)
          
          if(energyType){
            row.TypeName=energyType.TypeName
          }else{
            row.TypeName=''
          }
          this.selectFactors.push(row)
        }
        
      }else{
        if(findx>-1){
          this.selectFactorIds.splice(findx,1)
          this.selectFactors.splice(findx,1)
          let twoTypeFilter=this.selectFactors.filter(ro=>ro.TypeId==row.TypeId)
          if(twoTypeFilter&&twoTypeFilter.length==0){
            if(this.twoTypeIds.includes(row.TypeId)){
              this.twoTypeIds.splice(this.twoTypeIds.indexOf(row.TypeId),1)
            }
          }
          
        }else{
          this.selectFactorIds.push(row.Id)
          if(energyType){
            row.TypeName=energyType.TypeName
          }else{
            row.TypeName=''
          }
          this.selectFactors.push(row)
          if(this.twoTypeIds.includes(row.TypeId)){}else{
            this.twoTypeIds.push(row.TypeId)
          }
        }
      }
      
    },
    setDelSelectFactor(inx,row){
      // console.log(row,'rowrow');
      let twoTypeFilter=this.selectFactors.filter(ro=>this.twoTypeIds.includes(ro.TypeId))
      if(twoTypeFilter&&twoTypeFilter.length==1){
        if(this.twoTypeIds.includes(row.TypeId)){
          this.twoTypeIds.splice(this.twoTypeIds.indexOf(row.TypeId),1)
        }
      }
      
      this.selectFactorIds.splice(inx,1)
      this.selectFactors.splice(inx,1)
    },
    hasTwoTypeVal(row){
      if(row.Children&&row.Children.length>0){
        let findrow=row.Children.find(rw=>rw.Id==this.twoTypeActvie)
        if(findrow){
          return true
        }
      }
      return false
    },
    ChoiceTwoType(row){
      let findx=this.twoTypeIds.findIndex(rw=>rw==row.Id)
      if(findx>-1){
        this.twoTypeIds.splice(findx,1)
        let list=row.Factors
        if(list&&list.length>0){
          let isSingleChoice=[]
          this.selectFactorIds.map(ro=>{
            let roFind=list.find(rw=>rw.Id==ro)
            if(roFind){
              isSingleChoice.push(ro)
            }
          })
          if(isSingleChoice&&isSingleChoice.length>0){
            isSingleChoice.map(r=>{
              let findSinglex=this.selectFactorIds.findIndex(rw=>rw==r)
              this.selectFactorIds.splice(findSinglex,1)
              this.selectFactors.splice(findSinglex,1)
            })
            
          }
        }
        
      }else{
        this.twoTypeIds.push(row.Id)
        let list=row.Factors
        if(list&&list.length>0){
          this.selectFactorIds.push(list[0].Id)
          this.selectFactors.push(list[0])
        }
      }
    },
    loadFactorTreeList(){
      SelectOrgAllFactorList().then(res=>{
        // console.log("企业可选择的排放因子",res);
        // this.FactorTreeList=res.data
        this.EnergyTypeParent=new Map()
        this.activeIds=[]
        this.activeVal=[]
        this.FactorTreeList=this.processFactorTreeList(res.data)
      })
    },
    processFactorTreeList(list,parInx){
      //处理树级数据
      let filtelist=list.map((row,inx)=>{
        if(row.Children&&row.Children.length>0){
          row.FactorType=false
          let childHasChild=row.Children.find(ro=>ro.Children&&ro.Children.length>0)
          if(childHasChild){
            if(parInx==undefined){
              row.Children=this.processFactorTreeList(row.Children,inx)
            }else{
              row.Children=this.processFactorTreeList(row.Children,parInx)
            }
          }else{
            if(parInx==undefined){
              if(this.activeIds[inx]==undefined){
                this.activeIds[inx]=row.Id
                this.activeVal[inx]=row
              }
            }else{
              if(this.activeIds[parInx]==undefined){
                this.activeIds[parInx]=row.Id
                this.activeVal[parInx]=row
              }
            }
            
            row.Children=row.Children.filter(row=>row.FactorType)
            this.EnergyTypeParent.set(row.Id,JSON.parse(JSON.stringify(row)))
            row.Children=[]
          }
        }
        return row
      })
      filtelist=filtelist.filter(row=>!row.FactorType)
      return filtelist
    },
    loadSelectFactorOrg(alSave){
      selectFactorOrg({OrgId:this.orgId}).then(res=>{
        // console.log("企业选择的排放因子",res);
        this.selectFactors=res.data.map(row=>{
          let findr=alSave.find(rw=>row.Id==rw.FactorId)
          if(findr&&findr.Name){
            row.TypeName=findr.Name
            if(this.twoTypeIds.includes(findr.Id)){}else{
              this.twoTypeIds.push(findr.Id)
            }
          }else{
            row.TypeName=''
          }
          
          return row
        })
        this.selectFactorIds=this.selectFactors.map(row=>row.Id)
      })
    },
    async saveSelectFactorOrg(){//保存已选排放因子
     let factorIdStr=this.selectFactorIds.join(',')
     let res=await saveFactorOrg({OrgId:this.orgId,factorId:factorIdStr})
    },
    async handleorgConfSave(){//联系人和联系方式
      let emissionSourceJsonArr=this.selectFactors.map(row=>{
        let obj={"Id":row.TypeId,"Name":row.TypeName,"FactorId":row.Id}
        return obj
      })
     let res=await  orgConfSave({
      id:this.orgId,
      contectName:this.addForm.Manager,
      contectTel:this.addForm.Contact,
      ElecCode:this.addForm.ElecCode,
      ElecPowerCode:this.addForm.ElecPowerCode,
      emissionSourceJson:JSON.stringify(emissionSourceJsonArr)
     })
    //  console.log("保存企业配置信息",res);
    },
    async loadorgConfInfo(){
      //获取企业配置信息
      let res= await orgConfInfo()
      return res.data
    },
    provinceChange(isnotSet){
      //选择省
      let findrow=this.areaTreeList.find(row=>row.Id==this.addForm.province)
      if(findrow){
        this.addForm.provinceName=findrow.Name
        this.cityList=findrow.children?findrow.children:[]
      }else{
        this.addForm.provinceName=''
        this.cityList=[]
      }
      if(isnotSet){}else{
        this.addForm.city=''
        this.addForm.cityName=''
        this.addForm.district=''
        this.addForm.districtName=''
      }
    },
    cityChange(isnotSet){
      //选择市
      let findrow=this.cityList.find(row=>row.Id==this.addForm.city)
      if(findrow){
        this.addForm.cityName=findrow.Name
        this.districtList=findrow.children?findrow.children:[]
      }else{
        this.districtList=[]
      }
      if(isnotSet){}else{
        this.addForm.district=''
        this.addForm.districtName=''
      }
    },
    districtChange(){
      //区选择完成
      this.addForm.AddressCode=this.addForm.district
      let findrow=this.districtList.find(row=>row.Id==this.addForm.district)
      if(findrow){
        this.addForm.districtName=findrow.Name
      }
      let addForm=JSON.parse(JSON.stringify(this.addForm))
      this.addForm=JSON.parse(JSON.stringify(addForm))
    },
    cancelAdd(){

    },
    getDatas() {
      this.$store.dispatch("datas/industryTree").then(rt => {
        this.industryLis = rt;
      });
      
    },
    async choiceIndustry() {
      //选择行业类型
      this.$set(this.addForm,"Industry",this.addForm.industryArr[this.addForm.industryArr.length - 1]);
      this.$store.commit("datas/SET_INDUSTRY",this.addForm.industryArr[this.addForm.industryArr.length - 1]);
      let rs = await this.$store.dispatch("datas/industryName");
      this.$set(this.addForm, "IndustryName", rs);
    },
    orgSubmit() {
      this.$refs["orgForm"].validate(async valid => {
        if (valid) {
          try {
            // console.log(this.addForm,'提交的数据');
            let org={
              Id:this.addForm.Id,
              Industry:this.addForm.Industry,
              IndustryName:this.addForm.IndustryName,
              OrgName:this.addForm.OrgName,
              AddressCode:this.addForm.AddressCode,
              AddressDetail:this.addForm.AddressDetail,
              AddressName:this.addForm.provinceName+this.addForm.cityName+this.addForm.districtName
            }
            let response=await editCompany(org)
            await this.handleorgConfSave()
            await this.saveSelectFactorOrg()
            this.$modal.msgSuccess("修改成功");
            this.$emit("reLoadOrg");
          } catch (error) {
            console.log("报错了",error);
          }
        }
      });
    },
    getOrgInfo() {
      this.isLoadingOrg = true;
      orgInfo({ id: this.orgId })
        .then(async response => {
          if (response.code == 0 && response.data) {
            if (response.data.Industry) {
              this.$store.commit("datas/SET_INDUSTRY", response.data.Industry);
              response.data.IndustryName = await this.$store.dispatch(
                "datas/industryName"
              );
              response.data.industryArr = await this.getIndustryArr(
                response.data.Industry
              ); //获取行业类型的值
            } else {
              response.data.IndustryName = "";
              response.data.industryArr = [];
            }
            
            let addForm={
              Id:response.data.Id,
              Industry:response.data.Industry?response.data.Industry:'',
              industryArr:response.data.industryArr?response.data.industryArr:[],
              IndustryName:response.data.IndustryName?response.data.IndustryName:'',
              OrgName:response.data.OrgName?response.data.OrgName:'',
              AddressCode:response.data.AddressCode?response.data.AddressCode:'',
              AddressDetail:response.data.AddressDetail?response.data.AddressDetail:'',
              AddressName:response.data.AddressName?response.data.AddressName:'',
            }
            let rt=await this.$store.dispatch("datas/areaTree")
            this.areaTreeList = rt;
            let areaform=this.loadAllArea(response.data.AddressCode,3,{})
            addForm={...addForm,...areaform}
            let contactInfo=await this.loadorgConfInfo()
            let EmissionSourceJson=contactInfo.EmissionSourceJson?JSON.parse(contactInfo.EmissionSourceJson):[]
            this.loadSelectFactorOrg(EmissionSourceJson)
            addForm.Manager=contactInfo.ContectName?contactInfo.ContectName:''
            addForm.Contact=contactInfo.ContectTel?contactInfo.ContectTel:''
            addForm.ElecCode=contactInfo.ElecCode
            addForm.ElecPowerCode=contactInfo.ElecPowerCode
            this.addForm=JSON.parse(JSON.stringify(addForm))
            this.provinceChange(true)
            this.cityChange(true)
            // console.log('初始化的数据',this.addForm);
          }
          this.isLoadingOrg = false;
        })
        .catch(err => {
          console.log(err);
          this.isLoadingOrg = false;
        });
    },
    loadAllArea(val,inx,form){
      let areaAllData=this.$store.state.datas.areaAllData
      if(inx!=1){
        areaAllData=areaAllData.filter(row=>row.ParentId!="100000")
      }
      let areaFind=areaAllData.find(row=>row.Id==val)
      if(areaFind&&areaFind.ParentId=="100000"){
        form.provinceName=areaFind.Name
        form.province=areaFind.Id
        // console.log("有返回吗？",form);
        return form
      }else{
        if(inx==3){
          if(areaFind){
            form.district=areaFind.Id
            form.districtName=areaFind.Name
          }else{
            form.district=''
            form.districtName=''
          }
          
        }else if(inx==2){
          form.city=areaFind.Id
          form.cityName=areaFind.Name
        }
        inx--
        if(areaFind&&areaFind.ParentId){
          return this.loadAllArea(areaFind.ParentId,inx,form)
        }else{
          return form
        }
        
      }
    },
    async getIndustryArr(Industry) {
      //获取行业规模数组
      let arr = await this.$store.dispatch("datas/industryTree");
      let isFinish = false;
      let rsArray = [];
      for (let index = 0; index < arr.length; index++) {
        if (arr[index].children && arr[index].children.length > 0) {
          for (let ix = 0; ix < arr[index].children.length; ix++) {
            if (arr[index].children[ix].Id == Industry) {
              rsArray = [];
              rsArray.push(arr[index].children[ix].ParentId);
              rsArray.push(arr[index].children[ix].Id);
              return rsArray;
              // this.org.industryArr = JSON.parse(JSON.stringify(rsArray));
            }
          }
          if (isFinish) {
            return rsArray;
          }
        }
      }
    },
  },
};
</script>

<style lang="scss" scoped>
.elform_title{
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 56px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  width: 100%;
  margin-bottom: 24px;
  font-size: 14px;
  .elform_title_left{
    width: calc(100% - 241px);
    font-size: 16px;
    display: flex;
    align-items: center;
    line-height: 16px;
    color: rgba(255, 255, 255, 1);
    .line{
      width: 1px;
      height: 14px;
      background: rgba(255, 255, 255, 0.2);
      margin: 0 16px 0 20px;
    }
    img{
      width: 16px;
      height: 16px;
      margin-right: 8px;
    }
    .choice_con{
      // display: flex;
      // align-items: center;
      width: 82.2%;
      white-space: nowrap; /* 防止文本换行 */
      overflow: hidden; /* 隐藏溢出的文本 */
      text-overflow: ellipsis; /* 显示省略号 */
      color: rgba(255, 255, 255, 0.65);
      .choice_label{
        color: rgba(255, 255, 255, 0.65);
        margin-right: 16px;
        display: inline-flex;
      }
      .choice_li{
        display: inline-flex;
        align-items: center;
        background: rgba(61, 185, 143, 1);
        border-radius: 2px;
        color: rgba(255, 255, 255, 1);
        font-size: 14px;
        padding: 6px 10px;
        margin-left: 4px;
        i{
          font-size: 8px;
          margin-left: 10px;
          cursor: pointer;
        }
      }
    }
  }
  .right_time{
    color: rgba(255, 255, 255, 0.6);
  }
  .right_tips{
    color: rgba(61, 185, 143, 1);
    cursor: pointer;
    i{
      margin-right: 4px;
    }
  }
}
.energy_ul{
  .energy_li{
    display: flex;
    flex-wrap: wrap;
    font-size: 14px;
    color: rgba(255, 255, 255, 1);
    margin-bottom: 24px;
    .energy_li_label{
      width: 184px;
      line-height: 14px;;
      display: flex;
      margin-top: -11px;
      margin-left: -16px;
      .line{
        width: 4px;
        height: 14px;
        background: rgba(61, 185, 143, 1);
        border-radius: 2px;
        margin-right: 8px;
      }
    }
    .energy_list{
      display: flex;
      flex-wrap: wrap;
      width: calc(100% - 168px);
      border-bottom:1px solid rgba(255, 255, 255, 0.1);
      .energy_list_li_con{
        // width: 146px;
        width: 100%;
      }
      .energy_list_li{
        display: flex;
        flex-wrap: wrap;
        // align-items: center;
        font-size: 14px;
        width: 146px;
        line-height: 14px;
        // height: 14px;
        padding-bottom: 16px;
        &.alllong{
          width: 100%;
        }
        
        &.none{
          .icon_con{
            border: 1px solid rgba(255, 255, 255, 0.6);
            background:transparent;
            
          }
          .li_text{
            color: rgba(255, 255, 255,1);
          }
        }
        &.disable{
          .icon_con{
            border: none;
            background:transparent;
            cursor: not-allowed;
            i{
              color: rgba(255, 255, 255,0.2);
            }
          }
          .li_text{
            color: rgba(255, 255, 255,0.3);
          }
        }
        .icon_con{
          margin-right: 10px;
          width: 14px;
          height: 14px;
          border: none;
          border-radius: 4px;
          cursor: pointer;
        }
        &.choice{
          // align-items: center;
          padding-bottom: 2px;
          .icon_con{
            background: rgba(61, 185, 143, 1);
            width: 28px;
            height: 28px;
            display: flex;
            align-items: center;
            justify-content: center;
            transform:translateY(-7px) translateX(-7px) scale(0.5); /* 缩小到原始大小的80% */
            margin-right: -4px;
            
            i{
              color: rgba(255, 255, 255,1);
            }
          }
        }
        
        .li_text{
          color: rgba(255, 255, 255,1);
          line-height: 14px;
        }
      }
      
    }
    .radio_list{
      display: flex;
      flex-wrap: wrap;
      width: 100%;
      margin-bottom: 24px;
      background: rgba(19, 25, 34, 1);
      padding-left: 18px;
      padding-top: 8px;
      box-sizing: border-box;
      border-radius: 4px;
      // width: calc(100% - 158px);
      // margin-left: 158px;
      .radio_li{
        font-size: 14px;
        line-height: 14px;
        color: rgba(255, 255, 255, 1);
        width: calc(100% / 6);
        padding-bottom: 16px;
        padding-top: 8px;
        display: flex;
        flex-wrap: wrap;
        .icon_con{
          margin-right: 10px;
          width: 14px;
          height: 14px;
          border: 1px solid rgba(255, 255, 255, 0.6);
          background:transparent;
          border-radius: 4px;
          cursor: pointer;
        }
        &.choice{
          padding-bottom: 2px;
          .icon_con{
            background: rgba(61, 185, 143, 1);
            width: 28px;
            height: 28px;
            display: flex;
            align-items: center;
            justify-content: center;
            transform:translateY(-7px) translateX(-7px) scale(0.5); /* 缩小到原始大小的80% */
            margin-right: -4px;
            
            i{
              color: rgba(255, 255, 255,1);
            }
          }
          // i{
          //   color: rgba(61, 185, 143, 1);
          // }
        }
        // i{
        //   color: rgba(255, 255, 255, 0.6);
        //   margin-right: 10px;
        //   cursor: pointer;
        // }
      }
      // .radio_li:last-child{
      //   padding-bottom: 0;
      // }
    }
  }
  .energy_li:last-child{
    margin-bottom: 40px;
    .energy_list{
      border-bottom: none;
    }
  }
}
</style>