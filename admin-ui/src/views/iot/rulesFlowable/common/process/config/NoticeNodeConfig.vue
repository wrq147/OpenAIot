<template>
  <div>
    <el-form label-width="130px" :model="config" :rules="rules" ref="form">
      <el-form-item label="参考设备类型：" prop="NoticeWay">
        <el-select style="width:100%" v-model="config.NoticeWay" placeholder="请选择参考设备类型" @change="wayChange">
          <el-option label="APP站内通知" value="APP"></el-option>
          <el-option label="EMAIL邮件通知" value="EMAIL"></el-option>
          <el-option label="SMS短信通知" value="SMS"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="推送内容：" prop="Title">
        <el-input type="textarea" placeholder="请输入推送的内容" autosize v-model="config.Title"></el-input>
      </el-form-item>
      <el-form-item label="用户：" prop="TargetValue" v-if="config.NoticeWay==='APP'">
        <el-select clearable @clear="handleClear" class="form_input_style" v-model="config.userName" ref="selectUsers" placeholder="请选择通知的用户" @focus="getUsersFocus" style="width: 100%"></el-select>
        <org-picker :multiple="false" ref="userPicker" :selected="userInfo" @ok="selectUsersed"/>
      </el-form-item>
      <el-form-item label="邮件：" prop="TargetValue" v-if="config.NoticeWay==='EMAIL'">
        <el-input placeholder="请输入邮件地址" v-model="config.TargetValue"></el-input>
      </el-form-item>
      <el-form-item label="手机号：" prop="TargetValue" v-if="config.NoticeWay==='SMS'">
        <el-input placeholder="请输入手机号" v-model="config.TargetValue"></el-input>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import OrgPicker from "@/views/flowable/common/OrgPicker";
export default {
  name: "ConcurrentNodeConfig",
  components:{OrgPicker},
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      }
    }
  },
  computed: {
    selectedNode() {
      return this.$store.state.rulesFlowable.rulesSelectedNode;
    }
  },
  data() {
    return {
      userInfo: [],
      rules:{
        NoticeWay: [
          { required: true, trigger: "change", message: "请选择通知方式" }
        ],
        Title: [{ required: true, trigger: "blur", message: "请输入通知内容" }],
        TargetValue: [
          { required: true, trigger: "blur", message: "请输入通知目标" }
        ]
        
      }
    };
  },
  mounted() {
  },
  methods: {
    wayChange(){
      this.config.TargetValue = undefined;
      this.config.userName = undefined;
      this.config.userAvatar = undefined;
      this.userInfo =[]
    },
    selectUsersed(values) {
      //选择协作人
      this.userInfo = values;
      if (values.length > 0) {
        let li = values[0].id;
        let li2 = values[0].name;
        let li3 = values[0].avatar;
        this.config.TargetValue = li;
        this.config.userName = li2;
        this.config.userAvatar = li3;
      } else {
        this.config.TargetValue = undefined;
        this.config.userName = undefined;
        this.config.userAvatar = undefined;
      }
       //清除el-select多选时的清除按钮
      //  this.$nextTick(() => {
      //   const closeIcons = this.$refs.selectUsers.$el.querySelector(".el-select__tags").querySelectorAll(".el-icon-close");
      //   const arr = Array.from(closeIcons);
      //   arr.forEach((item) => {
      //     item.style.display = "none";
      //   });
      // });
      this.$forceUpdate();
    },
    handleClear() {
      //清除协作人
      this.config.TargetValue = undefined;
      this.config.userName = undefined;
      this.config.userAvatar = undefined;
      this.userInfo =[]
    },
    getUsersFocus() {
      //获取用户下拉列表的焦点
      this.$refs.selectUsers.blur();
      
      this.userInfo=[]
      if(this.config.NoticeWay==='APP'&&this.config.TargetValue&&this.config.userName){
        this.userInfo.push({id:this.config.TargetValue,name:this.config.userName,avatar:this.config.userAvatar,type: "user",})
        
      }
      this.$refs.userPicker.show(this.userInfo, "user");
    },
  }
};
</script>


<style lang="less" scoped>
.choose {
  border-radius: 5px;
  margin-top: 2px;
  background: #f4f4f4;
  border: 1px dashed #1890ff !important;
}

.drag-hover {
  color: #1890ff;
}

.drag-no-choose {
  cursor: move;
  background: #f8f8f8;
  border-radius: 5px;
  margin: 5px 0;
  height: 25px;
  line-height: 25px;
  padding: 5px 10px;
  border: 1px solid #ffffff;
  div {
    display: inline-block;
    font-size: small !important;
  }

  div:nth-child(2) {
    float: right !important;
  }
}
</style>
