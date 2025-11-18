<template>
    <div class="elbiaoge_elform">
        <div style="color: #409eff;padding-bottom: 20px;padding-top:15px;">通用Mqtt通讯Topic，默认用户名hello，密码world，端口号2883，Mqtt的ClientId需要等于{通讯Id}</div>
        <el-table
        :data="tableData"
        :header-cell-style="headerFun"
        border
        style="width: 100%">
        <el-table-column
            prop="Name"
            label="功能"
            width="180">
        </el-table-column>
        <el-table-column
            prop="Topic"
            label="Topic类"
            width="380">
        </el-table-column>
        <el-table-column
            prop="Info"
            label="描述">
        </el-table-column>
        </el-table>
        <div style="color: #409eff;padding: 20px 0;">平台消息介绍</div>
        <el-table
        :data="msgData"
        :header-cell-style="headerFun"
        border
        style="width: 100%">
        <el-table-column
            prop="Type"
            label="消息类型"
            width="180">
        </el-table-column>
        <el-table-column
            prop="Content"
            label="消息格式"
            >
        </el-table-column>
        <el-table-column
            prop="Info"
            width="450"
            label="描述">
        </el-table-column>
        </el-table>
    </div>
  </template>
  <script>

  export default {
    data() {
      return {
        tableData: [{
            Name: '设备上报消息',
            Topic: 'wukong/up/{通讯Id}',
            Info: '设备向平台上报属性、事件等消息'
          },{
            Name: '向设备下发消息',
            Topic: 'wukong/down/{通讯Id}',
            Info: '平台向设备发送功能指定或回复消息'
          }],
          msgData:[
            {Type:'ReadPropertyReply',Content:'',Info:'设备属性上报'},
            {Type:'Bind',Content:'',Info:'协议发布时下发的固件升级消息'},
            {Type:'BindReply',Content:'',Info:'固件升级消息的回复'},
            {Type:'QueryICCID',Content:'',Info:'请求查询ICCID'},
            {Type:'QueryICCIDReply',Content:'',Info:'查询ICCID的回复消息（平台通过ICCID管理物联卡）'},
          ]
      };
    },
    methods: {
        headerFun(){
            return "color:#78829D;background: #F9FAFC !important;";
        }
    },
  };
  </script>
  
  <style lang="scss">
  
  </style>