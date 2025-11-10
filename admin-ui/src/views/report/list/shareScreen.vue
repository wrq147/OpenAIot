<template>
    <div>
      <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" width="700px" top="2vh" @close="cancel">
      <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="80px" class="addPeople">
        <el-form-item label="定时推送">
          <el-radio-group v-model="ruleForm.timerStatus" @input="changeTimerStatus">
            <el-radio :label="1">停用</el-radio>
            <el-radio :label="0">启用</el-radio>
          </el-radio-group>
        </el-form-item>
        
        <el-form-item prop="timerCron" label="定时时间" v-if="ruleForm.timerStatus==0">
          <el-input style="width: 460px" :readonly="true" v-model="ruleForm.cronName" placeholder="请选择定时时间">
            <template slot="append">
              <el-button type="primary" @click="handleShowCron()" style="margin-left:-20px">
                设置时间
                <i class="el-icon-time el-icon--right"></i>
              </el-button>
            </template>
          </el-input>
        </el-form-item>
        <el-form-item label="人员方式" v-if="ruleForm.timerStatus==0" prop="noticeUserType">
          <el-radio-group v-model="ruleForm.noticeUserType" @input="changeNoticeType">
            <el-radio :label="1">角色</el-radio>
            <el-radio :label="0">人员</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item :label="'通知'+userLabel" prop="noticeUsers" v-if="ruleForm.timerStatus==0">
            <el-select class="form_input_style" multiple v-model="ruleForm.noticeUsersName" ref="selectUsers" :placeholder="'请选择通知'+userLabel" @focus="getUsersFocus" style="width: 100%"></el-select>
            <org-picker :multiple="true" ref="userPicker" :selected="noticeUsers" @ok="selectUsersed"/>
        </el-form-item>
        <el-form-item label="通知方式" prop="noticeWay" v-if="ruleForm.timerStatus==0">
          <el-checkbox-group v-model="ruleForm.noticeWay">
            <el-checkbox label="APP">APP站内通知</el-checkbox>
            <el-checkbox label="EMAIL">EMAIL邮件通知</el-checkbox>
            <el-checkbox label="SMS">SMS短信通知</el-checkbox>
            <el-checkbox label="WX">WX微信通知</el-checkbox>
          </el-checkbox-group>
        </el-form-item>
        <el-form-item label="有效期" prop="EffectiveTime">
          <div class="flex_con">
            <el-select v-model="ruleForm.EffectiveTime" placeholder="请选择">
              <el-option label="1天" :value="1"></el-option>
              <el-option label="7天" :value="7"></el-option>
              <el-option label="30天" :value="30"></el-option>
              <el-option label="1年" :value="365"></el-option>
              <el-option label="永久" :value="0"></el-option>
            </el-select>
            
          </div>
        </el-form-item>
        <el-form-item label="链接" v-if="ruleForm.link">
            <div class="flex_con">
              <el-input v-model="ruleForm.link" class="inputColor" ></el-input>
              <el-button class="copy-button" type="primary" @click="copyText">复制链接</el-button>
            </div>
        </el-form-item>
        <el-form-item label="查看密码">
            <div class="flex_con">
              <el-input v-model="ruleForm.UsingPassword" class="inputColor" show-password></el-input>
            </div>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitForm('ruleForm')" v-if="!ruleForm.link||ruleForm.id">{{ruleForm.id?'确定':'生成分享'}}</el-button>
        <el-button @click="cancel">关闭</el-button>
      </span>
    </el-dialog>
    <cronTime ref="cronTime" @finishTimeChoice="finishTimeChoice"></cronTime>
  </div>
</template>
<script>
import { addShare,editShare } from "@/api/report/share";
import cronTime from "@/views/iot/rulesEngine/cron_time";
import OrgPicker from "@/views/system/component/OrgPicker";
import {toCronDes} from "@/api/monitor/job";
export default { 
  name: 'addDataOrigin',
  props: {
    dialogVisible: {
      type: Boolean
    },
    title: {
        type: String
    }
  },
  components:{
    cronTime,
    OrgPicker
  },
  data() {
    return {
      dialogFlag: false,
      // 表单
      ruleForm: {
        timerStatus:0,
        noticeWay:[]
      },
      noticeUsers:[],
      list: [],
      // 校验
      rules: {
        EffectiveTime: [
            { required: true, message: "有效期不能为空", trigger: "change" }
        ],
        timerCron: [
            { required: true, message: "定时时间不能为空", trigger: "change" }
        ],
        noticeUserType: [
            { required: true, message: "请选择人员方式", trigger: "change" }
        ],
        noticeUsers: [
            { required: true, message: "通知人员不能为空", trigger: "change" }
        ],
        noticeWay: [
            { required: true, message: "通知方式不能为空", trigger: "change" }
        ]
      },
      userLabel:'人员'
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue
    }
  },
  mounted(){
    // let clipboard = new this.clipboard(".copy-button", {
    //   text: () =>{  return this.ruleForm.link  }
    // });
    //  clipboard.on('success', ()=> {
    //   this.$message.success("复制成功")
    //  });
    //  clipboard.on('error', ()=> {
    //    this.$message.errorsuccess("复制失败")
    //  });
  },
  methods: {
    copyText() {
      // console.log("执行了");
      if(this.ruleForm.link){
        const input = document.createElement('input');
        input.value = this.ruleForm.link; // 设置要复制的文本
        document.body.appendChild(input); // 添加input元素到DOM
        input.select(); // 选中文本
        document.execCommand('copy'); // 执行复制操作
        document.body.removeChild(input); // 移除input元素
        this.$message.success("复制成功")
      }
    },
    changeTimerStatus(){
      this.$set(this.ruleForm, 'link', '');
    },
    changeNoticeType(val){
      if(val==0){
        this.userLabel='人员'
        this.rules['noticeUsers'][0].message='通知人员不能为空'
      }else if(val==1){
        this.userLabel='角色'
        this.rules['noticeUsers'][0].message='通知角色不能为空'
      }
      this.noticeUsers=[]
      this.ruleForm.noticeUsers = undefined;
      this.ruleForm.noticeUsersName = undefined;
      this.ruleForm.noticeUsersAvatar = undefined;
    },
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          this.getAddShare()
        } else {
          return false
        }
      })
    },
    getAddShare() {
      let submitForm=JSON.parse(JSON.stringify(this.ruleForm))
      submitForm.noticeWay=this.ruleForm.noticeWay.join(',')
      delete submitForm.noticeUsersAvatar
      delete submitForm.noticeUsersName
      if(submitForm.id){
        editShare(submitForm).then(res => {
          if (this.ruleForm.ReportType === 'table') {
              this.ruleForm.link =  "http://" + window.location.host+"/#/report/spreadSheet/viewDataReport?tokenId=" +submitForm.id
            } else {
              this.ruleForm.link =  "http://" + window.location.host+"/#/report/datav/datavRelease?tokenId=" + submitForm.id
            }
            this.$message.success('修改成功!')
            this.$emit('getList','close')
        })
      }else{
        addShare(submitForm).then(res => {
            if (this.ruleForm.ReportType === 'table') {
              this.ruleForm.link =  "http://" + window.location.host+"/#/report/spreadSheet/viewDataReport?tokenId=" + res.data.Id    
            } else {
              this.ruleForm.link =  "http://" + window.location.host+"/#/report/datav/datavRelease?tokenId=" + res.data.Id    
            }
            this.$message.success('添加成功!')
        })
      }
        
    },
    cancel() {
      this.$emit('cancelForm')
    },
    handleShowCron(){
        //调起时间设计器规则
        this.$refs.cronTime.handleShowCron(this.ruleForm.timerCron);
    },
    finishTimeChoice(val){
        //生成的定时表达式结果
        this.ruleForm.timerCron=val
        // console.log("结果表达式",this.ruleForm.timerCron,val);
        toCronDes(val).then(x=>{
          this.$set(this.ruleForm,"cronName",x.data);
        })
    },
    getUsersFocus() {
      //获取用户下拉列表的焦点
      this.$refs.selectUsers.blur();
      let noticeUsersList =[]
      if(this.ruleForm.noticeUsers){
        noticeUsersList = this.ruleForm.noticeUsers.split(",");
      }
      let orgPickerType='user'
      if(this.ruleForm.noticeUserType==1){
        orgPickerType='role'
      }
      if (noticeUsersList && noticeUsersList.length > 0) {
        let arr = [];
        noticeUsersList.map((row, index) => {
          if (row > 0) {
            let obj = {}
            if(this.ruleForm.noticeUserType==1){
              obj = {
                id: parseInt(row),
                name: this.ruleForm.noticeUsersName[index],
                type: "role",
              }
            }else{
              obj = {
                id: parseInt(row),
                name: this.ruleForm.noticeUsersName[index],
                avatar: this.ruleForm.noticeUsersAvatar[index],
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
      this.noticeUsers = values;
      if (values.length > 0) {
        let li = [];
        let li2 = [];
        let li3 = [];
        values.map((it, ix) => {
          li.push(parseInt(it.id));
          li2.push(it.name);
          li3.push(it.avatar);
        });
        let rss=li.join(",")
        this.$set(this.ruleForm, 'noticeUsers', rss);
        this.ruleForm.noticeUsersName = li2;
        this.ruleForm.noticeUsersAvatar = li3;

      } else {
        this.ruleForm.noticeUsers = undefined;
        this.ruleForm.noticeUsersName = undefined;
        this.ruleForm.noticeUsersAvatar = undefined;
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
  }
}
</script>
  
<style lang="scss" scoped>
 ::v-deep {
  .el-dialog__header{
    border-bottom: 1px solid #ccc;
  }
  .el-select{
    width: 100%;
  }
  // .el-form-item__content{
  //   display: flex;
  //   align-items: center;
  // }
  .el-form-item__content button{
    margin-left: 20px;
  }
}
.flex_con{
  display: flex;
    align-items: center;
}
.addPeople>.box{
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
}
.addPeople>.btn{
  width:100%;
  justify-content: flex-end;
  display: flex;
  align-items: center;
}
</style>