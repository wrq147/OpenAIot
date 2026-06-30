using Common;
using FluentMigrator;
using System;

namespace IoTService.Migrations
{
    [Migration(20241008011)]
    public class IotMigrator6 : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_iot_script");

            Create.Table("mz_iot_script").WithDescription("物联DTU模板")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("模板Id")
.WithColumn("OrgId").AsInt64().Indexed("IotScriptOrgId").WithColumnDescription("创建者组织ID")
.WithColumn("IsSystem").AsFixedLengthAnsiString(1).WithColumnDescription("是否为系统模板")
.WithColumn("Name").AsString(50).WithColumnDescription("模板名称")
.WithColumn("Remark").AsString(5000).WithColumnDescription("模板介绍")
.WithColumn("KeyWords").AsString(2000).WithColumnDescription("模板搜索关键字")
.WithColumn("ScriptContent").AsString(20000).WithColumnDescription("脚本内容")
.WithColumn("InitModelTSL").AsString(20000).WithColumnDescription("物模型初始化")
       .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
       .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
       .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
       .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            if (Constants.General.sqltype != "Sqlite")
            {
                Execute.Sql("ALTER TABLE mz_iot_script ADD FULLTEXT INDEX ScriptKeywords (KeyWords);");
            }


            this.Execute.Sql("delete FROM mz_iot_script where Id='SS00001'");
            Insert.IntoTable("mz_iot_script").Row(new
            {
                Id = "SS00001",
                OrgId = 1,
                IsSystem = "1",
                Name = "自定义DTU模板",
                Remark = "默认的解释脚本，未进行任何处理",
                KeyWords = "自定义 默认",
                ScriptContent = @"
/**
 * 原数据转平台消息
 * @param {DataContext} context
 * @returns {BaseUpDeviceMessage} 返回平台消息
 */
function rawDataTo(context) {
  return context.ReplyMessage();
}
/**
 * 平台消息转原数据
 * @param {MessageContext} context
 * @returns {FastWriter} 返回写入流
 */
function toRawData(context) {
  return null;
}",
                InitModelTSL = "",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            this.Execute.Sql("delete FROM mz_iot_script where Id='SS00002'");
            Insert.IntoTable("mz_iot_script").Row(new
            {
                Id = "SS00002",
                OrgId = 1,
                IsSystem = "1",
                Name = "移讯DTU模板",
                Remark = "移讯",
                KeyWords = "移讯 DTU",
                ScriptContent = @"
/**
 * 原数据转平台消息
 * @param {DataContext} context
 * @return BaseUpDeviceMessage
 */
function rawDataTo(context) {
  let bytess=context.Payload().ToArray();
  if(context.ToHex(bytess,false)==""4F4B0D0A""){
    context.Payload().ReadBytes(4);
    context.ReplyMsgId(""OkMSG"",""ok"");
  }
  return null;
}
/**
 * 平台消息转原数据
 * @param {MessageContext} context
 * @return FastWriter
 */
function toRawData(context) {
  let curmsg = context.Message();
  if (curmsg.MsgType == ""Bind"") {
    let modbusInfo= context.GetModbusInfo();
    context.PublicStr(""AT*UART=""+modbusInfo.BaudRate+"",""+modbusInfo.DataBits+"",""+modbusInfo.Parity+"",""+modbusInfo.StopBits+"",0#"", false);
    context.PublicStr(""AT*QRYCMD=123456#"",false);
    let tarr = context.CreateModbusHex(false);
    if (tarr.length > 0) {
       context.PublicStr(""AT*QRYTIME=""+(modbusInfo.PollTime/1000)+""#"", false);
       context.PublicStr(""AT*QRYTIME=""+(modbusInfo.PollTime/1000)+""#"", false);
       for(var idx=0;idx<tarr.length;idx++){
           if(idx<20){
             context.PublicStr(""AT*QRYCMD"" + idx + ""="" + tarr[idx] + ""#"",false);
           }
       }
    }
    context.ConfirmReply(curmsg.MessageId,context.CreateBindReply())

    return context.Payload();
  }
  return null;
}",
                InitModelTSL = "",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            this.Execute.Sql("delete FROM mz_iot_script where Id='SS00003'");
            Insert.IntoTable("mz_iot_script").Row(new
            {
                Id = "SS00003",
                OrgId = 1,
                IsSystem = "1",
                Name = "银尔达DTU模板",
                Remark = "银尔达",
                KeyWords = "银尔达 DTU",
                ScriptContent = @"
/**
 * 原数据转平台消息
 * @param {DataContext} context
 * @returns {BaseUpDeviceMessage} 返回平台消息
 */
function rawDataTo(context) {
  let tmpmsg = context.ReplyMessage();
  if (tmpmsg == null) {
    let redn = context.Payload().ReadToEnd();
    let rednstr = context.ToUtf8(redn);
    if (rednstr.indexOf(""\r\nconfig"") == 0 || rednstr.indexOf(""config"") == 0) {
      let tmparr = rednstr.split("","");
      let tmpreturnval = tmparr[2];
      if (tmparr.length > 3) {
        for (let i = 3; i < tmparr.length; i++) {
          tmpreturnval = tmpreturnval + "","" + tmparr[i];
        }
      }
      context.ReplyMsgId(tmparr[1], tmpreturnval);
      return null;
    }
    else if (rednstr.indexOf(""_"") != -1) {
      //位置信息
      let posmsg = context.CreatePropertyMessage();
      posmsg.Properties = { ""BoxPosition"": rednstr };
      return posmsg;
    }
    else {
      return null;
    }
  }
  return tmpmsg;
}
/**
 * 平台消息转原数据
 * @param {MessageContext} context
 * @returns {FastWriter} 返回写入流
 */
function toRawData(context) {
  let curmsg = context.Message();
  if (curmsg.MsgType == ""Bind"") {
    cmdarr.length = 0;
    let modbusInfo = context.GetModbusInfo();
    cmdarr.push(""configset"");
    context.PublicStr(""config,set,rs485,"" + modbusInfo.BaudRate + "","" + modbusInfo.DataBits + "","" + modbusInfo.Parity + "","" + modbusInfo.StopBits + "",200,0\r\n"", false);

    let tarr = context.CreateModbusHex(false);
    if (tarr.length > 0) {
      let autopollcmd = ""config,set,autopoll,rs485,2000,"" + modbusInfo.PollTime + "",1"";
      for (var idx = 0; idx < tarr.length; idx++) {
        autopollcmd = autopollcmd + "","" + tarr[idx];
      }
      autopollcmd = autopollcmd + ""\r\n"";
      cmdarr.push(""autopoll"");
      context.PublicStr(autopollcmd, false);
    }

    cmdarr.push(""location"");
    context.PublicStr(""config,set,location,1,1,300,1,0,1\r\n"", false);

    cmdarr.push(""save"");
    context.PublicStr(""config,set,save\r\n"", false);

    context.ConfirmReply(curmsg.MessageId, context.CreateBindReply());
    return context.Payload();
  }
  else if (curmsg.MsgType == ""QueryICCID"") {
    context.PublicStrCallback((tmprsss)=>{
      if (tmprsss != null && tmprsss.indexOf(""ok"") == 0) {
        let aarr = tmprsss.split("","");
        let tmprs = aarr[aarr.length - 1].replace(/\r\n/g, """");
        let replyccid = context.CreateICCIDReply();
        replyccid.iccid = tmprs;
        context.ConfirmReply(null, replyccid);
      }
    },""iccid"", ""config,get,iccid\r\n"", false);
    return context.Payload();
  }
  return null;
}",
                InitModelTSL = "{\"tags\":[{\"name\":\"设备位置\",\"code\":\"position\",\"value\":{\"lng\":0,\"lat\":0},\"mapcode\":\"BoxPosition\",\"enable\":true,\"option\":{\"type\":\"geo\",\"usingGCJTo\":false},\"description\":\"\"}],\"properties\":[{\"name\":\"位置\",\"code\":\"BoxPosition\",\"prefixcode\":\"\",\"description\":\"\",\"option\":{\"type\":\"geo\",\"usingGCJTo\":false}}],\"functions\":[],\"events\":[],\"modbus\":{\"BaudRate\":9600,\"DataBits\":8,\"Parity\":\"0\",\"StopBits\":\"1\",\"Mode\":\"RTU\",\"PollTime\":5000,\"Matches\":[]}}",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            this.Execute.Sql("delete FROM mz_menu where menu_id=4400");

            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4400,
                menu_name = "DTU模板",
                parent_id = 4000,
                order_num = 10,
                path = "scrtemp/index",
                component = "iot/scrtemp/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTService/IotScript/ListPage",
                icon = "baojingliebiao",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });


        }
        public override void Down()
        {
        }
    }
}
