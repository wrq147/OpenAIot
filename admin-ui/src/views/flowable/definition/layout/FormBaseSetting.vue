<template>
  <div class="base-setup">
    <el-form ref="baseSetting" :model="setup" label-position="top" label-width="80px">
      <el-form-item label="表单图标">
        <i
          :class="setup.Icon"
          :style="'background:' + setup.Background"
          style="width:36px;height:36px;line-height:16px;font-size:18px;"
        ></i>
        <span class="change-icon">
          <div class="choice_icon">
            <span class="text">选择背景色</span>
            <el-color-picker v-model="setup.Background" show-alpha size="small" :predefine="colors"></el-color-picker>
          </div>
          <div class="choice_icon" >
            <span class="text">选择图标</span>
            <el-popover placement="bottom-start" width="390"  trigger="click">
              <div class="icon-select">
                <i :class="i" v-for="(i, id) in icons" :key="id" @click="setup.Icon = i"></i>
              </div>
              <div slot="reference" style="cursor: pointer;">
                <i :class="setup.Icon"></i>
                <i class="el-icon-arrow-down ic_set"></i>
              </div>
            </el-popover>

          </div>
        </span>
      </el-form-item>
      <el-form-item label="表单名称" :rules="getRule('请输入表单名称')" prop="Name">
        <el-input v-model="setup.Name" size="medium"></el-input>
      </el-form-item>
      <el-form-item label="所在分组" :rules="getRule('请选择表单分组')" class="group" prop="GroupId">
        <el-select v-model="setup.GroupId" placeholder="请选择分组" size="medium">
          <el-option
            v-for="(op, index) in fromGroup"
            :key="index"
            v-show="op.Id > 0"
            :label="op.Name"
            :value="op.Id"
          ></el-option>
        </el-select>
        <el-popover
          placement="bottom-end"
          title="新建表单分组"
          width="300"
          trigger="manual"
          v-model="showGroupAdd"
        >
          <el-input size="medium" v-model="newGroup" placeholder="请输入新的分组名">
            <el-button slot="append" size="medium" type="primary" @click="addGroup">提交</el-button>
          </el-input>
          <el-button
            icon="el-icon-plus"
            slot="reference"
            size="medium"
            type="primary"
            @click="showGroupAdd = !showGroupAdd"
          >新建分组</el-button>
        </el-popover>
      </el-form-item>
      <el-form-item label="表单说明">
        <el-input
          placeholder="请输入表单说明"
          v-model="setup.remark"
          type="textarea"
          show-word-limit
          :autosize="{ minRows: 2, maxRows: 5}"
          maxlength="500"
        ></el-input>
      </el-form-item>
      <el-form-item label="消息通知方式" :rules="getNoticeRule()" prop="notify">
        <el-select
          v-model="setup.notify.types"
          value-key="name"
          placeholder="选择消息通知方式"
          style="width: 35%;"
          size="medium"
          clearable
          multiple
          collapse-tags
        >
          <el-option
            v-for="(wc, index) in notifyTypes"
            :label="wc.name"
            :key="index"
            :value="wc.type"
          ></el-option>
        </el-select>
        <el-input
          size="medium"
          v-model="setup.notify.title"
          style="width: 63%; float:right;"
          placeholder="消息通知标题"
        ></el-input>
        <div>
          <el-dropdown trigger="click" @command="onNewData">
              <div style=" color: #3572ff;cursor: pointer;text-align: left;">
                  <i class="el-icon-plus" style="font-size:16px;padding:0;color: #3572ff;"></i>
                  <span style="margin-left: 6px; margin-right: 6px">添加消息通知标题组成</span>
              </div>
              <el-dropdown-menu slot="dropdown">
                  <el-dropdown-item command="text">文本</el-dropdown-item>
                  <el-dropdown-item command="source">组件</el-dropdown-item>
              </el-dropdown-menu>
          </el-dropdown>
          <div class="data-bd">
              <el-row style="margin-bottom: 15px" v-for="(params, idx) in noticeTitleList" :key="idx">
                  <el-col :span="16">
                      <el-input style="width: 100%;" v-if="params.type=='text'" v-model="params.value" placeholder="请输入文本" type="text"></el-input>
                      <el-select style="width: 100%;" v-if="params.type=='source'" v-model="params.value" placeholder="请选择组件">
                          <el-option v-for="opt in forms" :key="opt.id" :label="opt.title" :value="opt.id"></el-option>
                      </el-select>
                  </el-col>
                  <el-col :span="8" class="op">
                      <el-button style="margin-left: 10px" size="mini" @click="delDataItem(idx)" type="danger" icon="el-icon-delete" circle/>
                  </el-col>
              </el-row>
          </div>
        </div>
      </el-form-item>
      <el-form-item label="谁可以发起提交">
        <el-select
          v-model="setup.process.props.assignedUser"
          @click.native="selectUser()"
          value-key="name"
          class="select-u"
          placeholder="请选择可以发起提交的人员"
          size="medium"
          clearable
          multiple
        >
          <el-option
            v-for="(wc, index) in setup.process.props.assignedUser"
            :label="wc.name"
            :key="index"
            :value="wc"
          ></el-option>
        </el-select>
      </el-form-item>
    </el-form>
    <org-picker title="请选择可以发起提交的人员" multiple ref="orgPicker" @ok="selected"></org-picker>
  </div>
</template>

<script>
import OrgPicker from "../../common/OrgPicker";
import { getFormGroups, addGroup } from "@/api/flowable/design";

export default {
  name: "FormBaseSetting",
  components: { OrgPicker },
  props:{
    finishLoad:{
      type:Boolean,
      default:false
    }
  },
  data() {
    return {
      noticeTitleList:[],
      dataSource:[],
      showGroupAdd: false,
      newGroup: "",
      fromGroup: [],
      notifyTypes: [
        { type: "APP", name: "站内通知" },
        { type: "EMAIL", name: "邮件通知" },
        { type: "SMS", name: "短信通知" },
        { type: "WX", name: "微信通知" }
      ],
      colors: [
        "#ff4500",
        "#ff8c00",
        "#ffd700",
        "#90ee90",
        "#00ced1",
        "#1e90ff",
        "#c71585",
        "rgba(255, 69, 0, 0.68)",
        "rgb(255, 120, 0)",
        "hsl(181, 100%, 37%)",
        "hsla(209, 100%, 56%, 0.73)",
        "#c7158577"
      ],
      icons: [
        "el-icon-eleme",
        "el-icon-delete-solid",
        "el-icon-s-tools",
        "el-icon-phone",
        "el-icon-s-goods",
        "el-icon-warning",
        "el-icon-circle-plus",
        "el-icon-s-help",
        "el-icon-camera-solid",
        "el-icon-s-promotion",
        "el-icon-s-cooperation",
        "el-icon-s-platform",
        "el-icon-s-custom",
        "el-icon-s-data",
        "el-icon-s-check",
        "el-icon-s-claim",
        "el-icon-location"
      ]
    };
  },
  computed: {
    setup() {
      return this.$store.state.flowable.design;
    },
    forms() {
      if(this.$store.state.flowable.design.formItems&&this.$store.state.flowable.design.formItems.length>0){
        let filterItem=this.$store.state.flowable.design.formItems.filter(element=>element.name == "DevicPicker"||element.name == "TextInput"||element.name == "TextareaInput")
        return filterItem
      }else{
        return []
      }
      
    },
  },
  mounted() {
    this.getGroups();
  },
  watch:{
    'finishLoad':{
      handler(to){
        if(to){
          this.setNotifyTitle(this.setup.notify.title)
        }
      },
    },
    noticeTitleList:{
      handler(to){
        this.setup.notify.title=''
        to.map(row=>{
          if(row.type=='text'){
            this.setup.notify.title=this.setup.notify.title+row.value
          }else if(row.type=='source'){
            this.setup.notify.title=this.setup.notify.title+'${'+row.value+'}'
          }
        })
      },
      deep:true,
      immediate:true
    }
  },
  methods: {
    setNotifyTitle(title){
      if(title){
        this.noticeTitleList=[]
        this.load(title,'\$\{\d+\}')
      }
    },
    load(input1,pattern){
      let matches1 = input1.match(/\$\{(\d+)\}/g);
      let splitArr = input1.split(/\$\{(\d+)\}/g);
      this.noticeTitleList=[]
      if(splitArr&&matches1){
        for(let i=0;i<splitArr.length;i++){
          let tex=splitArr[i]
          let findObj=matches1.find(row=>row.indexOf(tex)>-1)
          if(tex&&findObj){
            this.noticeTitleList.push({
                type:'source',
                value:tex
            })
          }else if(tex){
            this.noticeTitleList.push({
                type:'text',
                value:tex
            })
          }
        }
      }
    },
    delDataItem(index){
        this.noticeTitleList.splice(index,1)
    },
    onNewData(val){
        if(val&&val=='text'){
            this.noticeTitleList.push({
                type:'text',
                value:''
            })
        }else if(val&&val=='source'){
            this.noticeTitleList.push({
                type:'source',
                value:''
            })
        }
    },
    getRule(msg) {
      return [{ required: true, message: msg, trigger: "blur" }];
    },
    getNoticeRule() {
      return [
        {
          required: true,
          validator: (rule, value, callback) => {
            if (
              this.setup.notify.title == "" ||
              this.setup.notify.types.length == 0
            ) {
              callback(new Error(""));
            } else {
              callback();
            }
          },
          message: "请选择消息通知方式",
          trigger: "blur"
        }
      ];
    },
    getGroups() {
      getFormGroups().then(rsp => {
        this.fromGroup = rsp.data;
      });
    },
    addGroup() {
      if (this.newGroup.trim() !== "") {
        this.showGroupAdd = false;
        addGroup({ Name: this.newGroup.trim() }).then(rsp => {
          this.setup.GroupId = rsp.data;
          this.$message.success("操作成功");
          this.getGroups();
        });
      }
    },
    selected(select) {
      this.setup.process.props.assignedUser.length = 0;
      select.forEach(item => {
        this.setup.process.props.assignedUser.push(item);
      });
    },
    selectUser() {
      this.$refs.orgPicker.show(this.setup.process.props.assignedUser);
    },
    validate() {
      this.$refs.baseSetting.validate();
      let err = [];
      if (!this.$isNotEmpty(this.setup.Name)) {
        err.push("表单名称未设置");
      }
      if (!this.$isNotEmpty(this.setup.GroupId)) {
        err.push("表单分组未设置");
      }
      if (this.setup.notify.types.length === 0) {
        err.push("审批消息通知方式未设置");
      }
      return err;
    }
  }
};
</script>

<style lang="less" scoped>
/deep/ .el-select-dropdown {
  display: none;
}

.icon-select {
  display: flex;
  flex-wrap: wrap;

  i {
    cursor: pointer;
    font-size: large;
    padding: 10px;
    max-width: 38px !important;

    &:hover {
      box-shadow: 0 0 10px 2px #c2c2c2;
    }
  }
}

/deep/ .select-u {
  width: 100%;
}

.base-setup {
  overflow: auto;
  margin: 0 auto;
  width: 600px;
  // height: calc(100vh - 105px);
  background: #ffffff;
  margin-top: 10px;
  padding: 20px 30px 90px 30px;
  border-radius: 10px;

  i:first-child {
    position: relative;
    // cursor: pointer;
    font-size: xx-large;
    color: #ffffff;
    border-radius: 10px;
    padding: 10px;
  }

  

  /deep/ .el-form-item__label {
    padding: 0;
    font-weight: bold;
  }

  /deep/ .el-form-item {
    margin-bottom: 5px;
    .el-form-item__content {
      line-height: 48px;
      vertical-align: middle;
      .el-tag{
        border-radius: 16px;
        background-color: rgba(246, 249, 255, 1);
        span,i{
          color: rgba(120, 130, 157, 1);
          background-color: rgba(246, 249, 255, 1);
        }
      }
    }
    input.el-input__inner {
      border-radius: 16px;
      border: 1px solid rgba(179,186,205,0.2);
      height: 40px !important;
      line-height: 40px !important;
      
    }
    .el-input__inner:focus{
          border: 1px solid #1890FF;
      }
    textarea.el-textarea__inner {
      border-radius: 8px;
      border: 1px solid rgba(179,186,205,0.2);
      min-height: 100px !important;
    }
    .el-textarea__inner:focus{
       border: 1px solid #1890FF;
    }
    .el-button:not(.el-button--danger){
      border-radius: 16px;
       background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
    }
  }
}

/deep/ .group {
  .el-select {
    width: calc(100% - 130px);
  }

  .el-button {
    margin-left: 10px;
    width: 120px;
  }
}

::-webkit-scrollbar {
  width: 4px;
  height: 4px;
  background-color: #f8f8f8;
}

::-webkit-scrollbar-thumb {
  border-radius: 16px;
  background-color: #e8e8e8;
}
</style>
<style lang="scss">
  .change-icon {
    margin-left: 20px;
    display: inline-block;
    vertical-align: top;
    height: 20px;
    // line-height:20px;

    span {
      font-size: small;
      color: #7a7a7a;
      // margin-right: 15px;
    }

    i {
      // cursor: pointer;
      color: #7a7a7a;
      font-size: x-large;
    }
    .ic_set{
        color: #BBC0CD !important;
        right: -15px !important;
        top: 12px !important;
        font-size: 16px;
        box-sizing: border-box;
        cursor: pointer;
      }
    .choice_icon {
      position: relative;
      display: inline-block;
      width: 178px;
      height: 40px;
      border-radius: 20px;
      line-height: 40px;
      border: 2px solid rgba(246, 247, 250, 1);
      margin-right: 20px;
      text-align: right;
      padding: 4px 36px;
      box-sizing: border-box;

      .el-color-picker__trigger{
        padding: 0 !important;
        border-radius: 5px !important;
        border: none !important;
        .el-color-picker__color.is-alpha{
          width: 28px;
          height: 28px;
          border-radius: 5px !important;
          border: none !important;
          .el-color-picker__color-inner{
            width: 28px;
            height: 28px;
            border-radius: 5px !important;
          }
        }
        .el-color-picker__icon.el-icon-arrow-down{
          font-size: 20px !important;
          color: #B3BACD;
          position: absolute;
          left: 150%;
        }
      }
      .el-popover__reference-wrapper i::before{
        color: rgba(120, 130, 157, 1);
        position: absolute;
        top: 0;
        right: 28px;;
        width: 28px;
        height: 28px;
      }
      .el-popover__reference-wrapper i{
        color: rgba(120, 130, 157, 1);
        position: absolute;
        top: 4px;
        right: 10px;
        // width: 28px;
        height: 10px;
      }
      span.text {
        position: absolute;
        left: 20px;
        top: 0;
      }
    }
  }
</style>
