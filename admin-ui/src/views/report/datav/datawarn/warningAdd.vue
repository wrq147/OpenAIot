<template>
  <div>
    <el-dialog :title="title" :visible.sync="addopen" width="700px" append-to-body :close-on-click-modal="false" :destroy-on-close="true">
        <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="left" :inline="true" label-width="80px">
            <el-form-item label="预警名称" prop="name" style="width: 100%">
                <el-input type="text" v-model="addForm.name" placeholder="请输入预警名称" :disabled="isReadonly" style="width: 100%"></el-input>
            </el-form-item>
            <el-form-item label="预警条件" prop="conditionJson" style="width: 100%">
              <el-row :gutter="10">
                <el-col :span="6">
                  <el-select v-model="addForm.conditionJson.DataSet" placeholder="请选择数据集" @change="conditionDataSetChnage" :disabled="isReadonly">
                    <el-option v-for="item in globalDatas" :key="item.name" :label="item.name" :value="item.name"></el-option>
                  </el-select>
                </el-col>
                <el-col :span="1.5" v-if="!isReadonly">
                  <el-button type="primary" plain @click="addConditionList">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left: 6px">添加预警条件</span>
                  </el-button>
                </el-col>
              </el-row>
              <el-row :gutter="10" :key="'condition'+inx" v-for="(item,inx) in addForm.conditionJson.Conditions" style="margin-top:10px">
                <el-col :span="2">
                  <div style="font-size:12px">条件{{inx+1}}:</div>
                </el-col>
                <el-col :span="5">
                  <el-select v-model="item.field" placeholder="请选择预警字段" :disabled="isReadonly">
                      <el-option v-for="item in fieldOptions" :key="item" :label="item" :value="item"></el-option>
                  </el-select>
                </el-col>
                <el-col :span="5">
                  <el-select v-model="item.compare" placeholder="请选择比较符" :disabled="isReadonly">
                      <el-option v-for="item in compareList" :key="item.value" :label="item.label" :value="item.value"></el-option>
                  </el-select>
                </el-col>
                <el-col :span="5">
                  <el-select v-model="item.valtype" placeholder="请选择值类型" :disabled="true">
                      <el-option v-for="item in valtypeList" :key="item.value" :label="item.label" :value="item.value"></el-option>
                  </el-select>
                </el-col>
                <el-col :span="5">
                  <el-input type="text" v-model="item.val" placeholder="请输入预警值" :disabled="isReadonly"></el-input>
                </el-col>
                <el-col :span="2" v-if="!isReadonly">
                  <el-button  size="mini" @click="delConditionsItem(inx)" type="danger" icon="el-icon-delete" circle></el-button>
                </el-col>
              </el-row>
              <el-row :gutter="10" style="margin-top:10px" v-if="addForm.conditionJson.Conditions&&addForm.conditionJson.Conditions.length>1">
                <template v-for="(item2,index) in addForm.conditionJson.Conditions">
                  <el-col :span="3" :key="'groups'+index" v-if="index==0">条件{{index+1}}:</el-col>
                  <el-col :span="3" v-if="index>0" :key="'groups2'+index">
                    <el-select v-model="addForm.conditionJson.Groups[index-1]" placeholder="请选择值类型" :disabled="isReadonly">
                      <el-option label="与" value="and"></el-option>
                      <el-option label="或" value="or"></el-option>
                    </el-select>
                  </el-col>
                  <el-col :span="3" :key="'groups'+index" v-if="index>0">条件{{index+1}}:</el-col>
                </template>
              </el-row>
            </el-form-item>
            <el-form-item style="width: 100%" prop="timerCron" label="定时时间" :rules="addRules.timerCron">
              <el-input style="width: 460px" :readonly="true" v-model="addForm.cronName" placeholder="请选择定时时间" :disabled="isReadonly">
                <template slot="append">
                  <el-button type="primary" @click="handleShowCron()">
                    设置时间
                    <i class="el-icon-time el-icon--right"></i>
                  </el-button>
                </template>
              </el-input>
            </el-form-item>
            <el-form-item label="沉默周期" prop="silenceTime" style="width: 100%">
              <el-input-number v-model="addForm.silenceTime" :min="60" :disabled="isReadonly"></el-input-number><span style="margin-left:5px">秒</span>
            </el-form-item>
            <el-form-item label="人员方式" prop="noticeUserType">
              <el-radio-group v-model="addForm.noticeUserType" @input="changeNoticeType">
                <el-radio :label="1">角色</el-radio>
                <el-radio :label="0">人员</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item :label="'通知'+userLabel" prop="noticeUsers" style="width: 100%">
                <el-select :disabled="isReadonly" class="form_input_style" multiple v-model="addForm.noticeUsersName" ref="selectUsers" :placeholder="'请选择通知'+userLabel" @focus="getUsersFocus" style="width: 100%"></el-select>
                <org-picker :multiple="true" ref="userPicker" :selected="noticeUsers" @ok="selectUsersed"/>
            </el-form-item>
            <el-form-item label="通知方式" prop="noticeWay" style="width: 100%">
              <el-checkbox-group v-model="addForm.noticeWay" :disabled="isReadonly">
                <el-checkbox label="APP">APP站内通知</el-checkbox>
                <el-checkbox label="EMAIL">EMAIL邮件通知</el-checkbox>
                <el-checkbox label="SMS">SMS短信通知</el-checkbox>
                <el-checkbox label="WX">WX微信通知</el-checkbox>
              </el-checkbox-group>
            </el-form-item>
            <el-form-item label="分享链接" prop="shareId" style="width: 100%">
              <el-select v-model="addForm.shareId" placeholder="请选择分享链接" :disabled="isReadonly" style="width: 100%" :clearable="true">
                <el-option v-for="item in shareListArr" :key="item.Id" :label="item.ReportName" :value="item.Id">
                  <div class="option_li_con">
                    <div class="option_li_left">
                      <span class="name">{{item.ReportName}}</span>
                      <div>
                        <el-tag type="info" v-if="item.TimerStatus==1">不推送</el-tag>
                        <div v-else>
                          <el-tag type="success">{{item.CronName}}</el-tag>
                        </div>
                      </div>
                    </div>
                    <div class="option_li_right">{{item.EffectiveTime === 0 ? '永久' : item.EffectiveTime + '天'}}</div>
                  </div>
                </el-option>
              </el-select>
            </el-form-item>
            
        </el-form>
      <div slot="footer" class="dialog-footer" v-if="!isReadonly">
        <el-button type="primary" @click="submitForm('click')" :disabled="saveLoading">确 定</el-button>
        <el-button @click="cancel" :disabled="saveLoading">取 消</el-button>
      </div>
    </el-dialog>
    <cronTime ref="cronTime" @finishTimeChoice="finishTimeChoice"></cronTime>
  </div>
</template>

<script>
import cronTime from "@/views/iot/rulesEngine/cron_time";
import {toCronDes} from "@/api/monitor/job";
import OrgPicker from "@/views/system/component/OrgPicker";
import {addReportWarn,editReportWarn,reportWarnInfo} from '@/api/report/warning';
import {shareList} from '@/api/report/share';
export default {
  name: 'AdminUiWarningAdd',
  components:{
    cronTime,
    OrgPicker
  },
  props:{
    globalData:{
        type:Array,
        default:()=>{
            return []
        }
    }
  },
  watch:{
    globalData:{
        handler(to){
          // console.log("数据集",to);
            this.globalDatas=to.filter(row=>row.dataSourceType=="database")
            this.$forceUpdate()
        },
        immediate:true,
        deep:true
    }
  },
  data() {
    return {
      userLabel:'人员',
      globalDatas:[],//数据源数据
      isReadonly:false,
      addopen:false,
      title:'添加预警',
      saveLoading:false,
      addForm:{
        name:'',
        conditionJson:{
          DataSet:'',
          Conditions:[],
          Groups:[]
        },
        silenceTime:86400,
        noticeWay:[],
        noticeUsers:'',
        noticeUsersName:[],
        timerCron:'',
        cronName:''
      },
      addRules:{
        timerCron: [{ required: true, trigger: "change", message: "请输入定时时间" }],
        name: [{ required: true, trigger: "blur", message: "请输入预警名称" }],
        noticeUsers: [{ required: true, trigger: "change", message: "请选择通知人员" }],
        noticeUserType: [{ required: true, message: "请选择人员方式", trigger: "change" }],
        noticeWay:[{ required: true, trigger: "change", message: "请输入预警通知方式" }],
      },
      noticeUsers:[],
      reportId:'',//报表Id
      fieldOptions:[],
      activedata:[],
      compareList:[
        {label:'大于',value: '>'},
        {label:'小于',value:'<'},
        {label:'等于',value:'=='},
        {label:'不等于',value: '><'},
        {label:'大于等于',value:'>='},
        {label:'小于等于',value:'<='},
      ],
      valtypeList:[
        {label:'数字',value:'number'}
      ],
      shareListArr:[],//分享链接列表
      shareQuery:{
        pageNum:1,
        pageSize:1000,
      }
    };
  },

  mounted() {
    
  },

  methods: {
    getShareList(){
      //获取分享链接列表
      shareList(this.shareQuery).then(res=>{
        this.shareListArr=res.data.List
      })
    },
    changeNoticeType(val){
      if(val==0){
        this.userLabel='人员'
        this.addRules['noticeUsers'][0].message='通知人员不能为空'
      }else if(val==1){
        this.userLabel='角色'
        this.addRules['noticeUsers'][0].message='通知角色不能为空'
      }
      this.noticeUsers=[]
      this.addForm.noticeUsers = undefined;
      this.addForm.noticeUsersName = undefined;
      this.addForm.noticeUsersAvatar = undefined;
    },
    conditionDataSetChnage(val){
      //数据集
      this.fieldOptions=[]
      let dataobj=this.globalDatas.find(row=>row.name==val)
      
      if(dataobj){
        if(dataobj.rawData){
          let rawData={}
          if(typeof dataobj.rawData=='string'){
            rawData=JSON.parse(dataobj.rawData)[0]
          }else{
            rawData=dataobj.rawData[0]
          }
          if(rawData.content&&rawData.content[0]){
            for(let keys in rawData.content[0]){
              if(typeof rawData.content[0][keys]=='number'){
                this.fieldOptions.push(keys)
              }
            }
          }
        }
      }
    },
    addConditionList(){
      //添加预警条件
      
      if(this.addForm.conditionJson.Conditions){
        this.addForm.conditionJson.Conditions.push({"field":"","compare":"","valtype":"number","val":""})
      }else{
        this.addForm.conditionJson.Conditions=[]
        this.addForm.conditionJson.Conditions.push({"field":"","compare":"","valtype":"number","val":""})
      }
      if(this.addForm.conditionJson.Conditions&&this.addForm.conditionJson.Conditions.length>=2){
        if(this.addForm.conditionJson.Conditions.length==2){
          this.addForm.conditionJson.Groups=[]
        }
        this.addForm.conditionJson.Groups.push('and')
      }else{
        this.addForm.conditionJson.Groups=[]
      }
    },
    delConditionsItem(inx){
      this.addForm.conditionJson.Conditions.splice(inx, 1);
      if(inx>0){
        this.addForm.conditionJson.Groups.splice(inx-1, 1)
      }
      
    },
    openAddDialog(reportId,warnId,isview){
        //打开添加弹窗
        this.saveLoading=false
        this.reportId=reportId
        
        if(isview){
          this.isReadonly=true
        }else{
          this.isReadonly=false
        }
        if(warnId){
          this.title='编辑预警'
          if(this.isReadonly){
            this.title='预警详情'
          }
          reportWarnInfo(warnId).then(res=>{
            // console.log("告警信息",res);
            let data=res.data
            this.addForm={
              reportId:data.ReportId,
              name:data.Name,
              conditionJson:data.ConditionJson?JSON.parse(data.ConditionJson):{DataSet:'',Conditions:[],Groups:[]},
              silenceTime:data.SilenceTime,
              noticeWay:data.NoticeWay?data.NoticeWay.split(','):[],
              noticeUsers:data.NoticeUsers,
              noticeUsersName:[],
              noticeUserType:data.NoticeUserType,
              timerCron:data.TimerCron,
              cronName:'',
              status:data.Status,
              id:data.Id,
              shareId:data.ShareId
            }
            let noticeUsers=data.NoticeUserList.map(row=>row.Id)
            this.addForm.noticeUsers=noticeUsers.join(',')
            this.noticeUsers=JSON.parse(JSON.stringify(data.NoticeUserList))
            this.addForm.noticeUsersName=data.NoticeUserList.map(row=>row.RealName)
            this.addForm.noticeUsersAvatar=data.NoticeUserList.map(row=>row.Avatar)
            this.$nextTick(() => {
              const closeIcons = this.$refs.selectUsers.$el.querySelector(".el-select__tags").querySelectorAll(".el-icon-close");
              const arr = Array.from(closeIcons);
              arr.forEach((item) => {
                item.style.display = "none";
              });
            });
            if(this.addForm.conditionJson.DataSet){
              this.conditionDataSetChnage(this.addForm.conditionJson.DataSet)//选择字段初始化操作
            }
            if(this.addForm.timerCron){
              toCronDes(this.addForm.timerCron).then(x=>{
                  this.$set(this.addForm,"cronName",x.data);
                })
            }
          })
          

        }else{
          this.title='添加预警'
          this.addForm={
            reportId:reportId,
            name:'',
            conditionJson:{
              DataSet:'',
              Conditions:[],
              Groups:[]
            },
            silenceTime:86400,
            noticeWay:[],
            noticeUsers:'',
            noticeUsersName:[],
            timerCron:'',
            cronName:'',
            noticeUserType:0,
            status:0,
            shareId:''
          }
        }
        this.shareQuery.reportId=reportId
        this.getShareList()
        this.addopen=true
        this.resetForm("addForm");
    },
    submitForm(){
        //数据提交保存
        // console.log("提交保存的数据",this.addForm);
        this.$refs.addForm.validate((valid) => {
          if(valid){
            this.saveLoading=true
            let submitForm={}
            submitForm={
              reportId:this.addForm.reportId,
              name:this.addForm.name,
              conditionJson:JSON.stringify(this.addForm.conditionJson),
              noticeUserType:this.addForm.noticeUserType,
              noticeUsers:this.addForm.noticeUsers,
              noticeUsersName:[],
              cronName:this.addForm.cronName,
              timerCron:this.addForm.timerCron,
              silenceTime:this.addForm.silenceTime,
              noticeWay:this.addForm.noticeWay.join(','),
              status:0,
              shareId:this.addForm.shareId
            }
            if(this.addForm.id){
              submitForm.id=this.addForm.id
              editReportWarn(submitForm).then(res=>{
                this.$modal.msgSuccess("修改成功");
                this.$emit('getReportWarnList')
                this.addopen=false
              }).catch(err=>{
                this.saveLoading=false
              })
            }else{
              addReportWarn(submitForm).then(res=>{
                this.$modal.msgSuccess("添加成功");
                this.$emit('getReportWarnList')
                this.addopen=false
              }).catch(err=>{
                this.saveLoading=false
              })
            }
            
          }
        })
    },
    cancel(){
        //取消保存操作
        this.addopen=false
    },
    handleShowCron(){
        //调起时间设计器规则
        if(this.isReadonly){
          return
        }
        this.$refs.cronTime.handleShowCron(this.addForm.timerCron);
    },
    finishTimeChoice(val){
        //生成的定时表达式结果
        this.addForm.timerCron=val
        // console.log("结果表达式",this.addForm.timerCron,val);
        toCronDes(val).then(x=>{
          this.$set(this.addForm,"cronName",x.data);
        })
    },
    //设置数据类型
    disposeData(activedata) {
        let keyList = []
        for (const key in activedata[0]) {
            let type = this.typeData(key)
            let array = { label: key, type: type, timeType: '', changeType: ''}
            keyList.push(array) 
        }
        return keyList
    },
    typeData(val,activedata) {
      let type = activedata[0][val] === null ? null : typeof activedata[0][val];
      if (type === 'string') {
        return this.isTime(activedata[0][val], type);
      } else if (type === null) {
        return 'null'
      } else {
        return type === 'number' ? '数字' : type
      }
    },
    isTime(data, type) {
        let asTime = !isNaN(Date.parse(data));
        return asTime ? '时间' : type === 'string' ? '字符串' : type
    },
    //设置数据类型
    getUsersFocus() {
      //获取用户下拉列表的焦点
      if(this.isReadonly){
        return
      }
      this.$refs.selectUsers.blur();
      let noticeUsersList =[]
      if(this.addForm.noticeUsers){
        noticeUsersList = this.addForm.noticeUsers.split(",");
      }
      let orgPickerType='user'
      if(this.addForm.noticeUserType==1){
        orgPickerType='role'
      }
      if (noticeUsersList && noticeUsersList.length > 0) {
        let arr = [];
        noticeUsersList.map((row, index) => {
          if (row > 0) {
            let obj = {}
            if(this.addForm.noticeUserType==1){
              obj = {
                id: parseInt(row),
                name: this.addForm.noticeUsersName[index],
                type: "role",
              }
            }else{
              obj={
                id: parseInt(row),
                name: this.addForm.noticeUsersName[index],
                avatar: this.addForm.noticeUsersAvatar[index],
                type: "user",
              }
            }
            arr.push(obj);
          }
        });
        this.noticeUsers = JSON.parse(JSON.stringify(arr));
      } else {
        this.noticeUsers = [];
      }
      
      this.$refs.userPicker.show(this.noticeUsers, orgPickerType);
    },
    selectUsersed(values) {
      //选择协作人
      // console.log(values,'values');
      this.noticeUsers = JSON.parse(JSON.stringify(values));
      if (values.length > 0) {
        let li = [];
        let li2 = [];
        let li3 = [];
        for(let ix=0;ix<values.length;ix++){
          let it=values[ix]
          li.push(parseInt(it.id));
          li2.push(it.name);
          li3.push(it.avatar);
        }
        this.$set(this.addForm, 'noticeUsers', li.join(","));
        this.addForm.noticeUsersName = JSON.parse(JSON.stringify(li2));
        this.addForm.noticeUsersAvatar = JSON.parse(JSON.stringify(li3));
      } else {
        this.addForm.noticeUsers = undefined;
        this.addForm.noticeUsersName = undefined;
        this.addForm.noticeUsersAvatar = undefined;
      }
       //清除el-select多选时的清除按钮
       this.$nextTick(() => {
          const closeIcons = this.$refs.selectUsers.$el.querySelector(".el-select__tags").querySelectorAll(".el-icon-close");
          const arr = Array.from(closeIcons);
          arr.forEach((item) => {
            item.style.display = "none";
          });
        });
      this.$forceUpdate();
    },
  },
};
</script>
<style lang="less" scoped>
::v-deep .el-form-item__content{
    width: calc(100% - 80px);
}
.option_li_con{
  display: flex;
  justify-content: space-between;
  align-items: center;
  .option_li_left{
    display: flex;
    justify-content: flex-start;
    align-items: center;
    .name{
      margin-right: 10px;
    }
  }
  .option_li_right{
    color: #8492a6; 
    font-size: 13px;
  }
}
</style>