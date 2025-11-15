<template>
      <div>
        <div class="jsfunc">
          <div class="button_con">
            <div>
              <el-button class="bt-item" size="small" @click="insertWait">只等待</el-button>
              <el-button class="bt-item" size="small" @click="insertPublicWait">下发等待</el-button>
              <el-button class="bt-item" size="small" @click="insertExeFun">执行功能</el-button>
              <el-button class="bt-item" size="small" @click="insertParamFun">下发参数</el-button>
              <el-button class="bt-item" size="small" @click="insertRefresh">刷新功能</el-button>
              <el-button class="bt-item" size="small" @click="insertJmp">跳转url</el-button>
              <el-button class="bt-item" size="small" @click="insertShow">展示信息</el-button>
              <el-button class="bt-item" type="primary"  size="small" @click="formatCode">格式化</el-button>
            </div>
          </div>
          <div class="js_func_con">
            <div class="code-container" ref="container"></div>
          </div>
        </div>
      </div>
  </template>
  <script>
  import * as monaco from "monaco-editor";
  
  export default {
    props: {
      value: String,
      language: { type: String, default: "javascript" },
      theme: { type: String, default: "vs" },
      options: {
        type: Object,
        default() {
          return {};
        },
      },
    },
    data() {
      return {
        // 编辑器对象
        monacoEditor: null,
        monacoContent: "",
      };
    },
    watch: {
      options: {
        deep: true,
        handler(options) {
          this.monacoEditor && this.monacoEditor.updateOptions(options);
        },
      },
    },
    mounted() {
      if (this.value == null || this.value == "") {
        this.monacoContent = `
  /**
   * 功能执行脚本
   * @param {FuncMessageContext} context
   */
  function exeFunc(context) {
    context.ConfirmSuccess({});
  }`;
      } else {
        this.monacoContent = this.value;
      }
      const fact = `
        declare class FastReader
        {
          ReadByte():Number;
          ReadBytes(count:Number):ArrayBuffer;
          ReadToEnd():ArrayBuffer;
          ToArray():ArrayBuffer;
          CopyTo(start:Number):FastReader;
          Reset():void;
          Skip(count:Number):void;
          ReadInt16LE():Number;
          ReadInt16BE():Number;
          ReadUInt16LE():Number;
          ReadUInt16BE():Number;
          ReadInt32LE():Number;
          ReadInt32BE():Number;
          ReadInt32CDAB():Number;
          ReadInt32BADC():Number;
          Peek(pos:Number):Number;
          Length:Number;
          Position:Number;
        }
        declare class FastWriter
        {
          WriteByte(value:Number):void;
          WriteBytes(values:ArrayBuffer):void;
          WriteUTF8String(value:String):void;
          WriteInt16LE(value:Number):void;
          WriteInt16BE(value:Number):void;
          WriteUInt16LE(value:Number):void;
          WriteUInt16BE(value:Number):void;
          WriteInt32LE(value:Number):void;
          WriteInt32BE(value:Number):void;
          Length:Number;
        }
        declare class BaseDeviceMessage
        {
          MsgType:String;
          ProductId:String;
          DeviceId:String;
        }
        declare class RequestMessage extends BaseDeviceMessage
        {
          MessageId:String;
        }
        declare class BaseUpDeviceMessage extends BaseDeviceMessage
        {
          Timestamp:Number;
        }
        declare class ReadPropertyMessageReply extends BaseUpDeviceMessage
        {
          MessageId:String;
          Properties:Object;
        }
        declare class CreateEmptyMessage extends BaseUpDeviceMessage
        {
        }
        declare class DeviceEventMessage extends BaseUpDeviceMessage
        {
          EventId:String;
          Outputs:Object;
        }
        declare class FunctionInvokeMessage extends RequestMessage
        {
            FunctionId:String;
            Inputs:Object;
        }
        declare class FunctionInvokeMessageReply extends BaseUpDeviceMessage
        {
          MessageId:String;
          Outputs:Object;
        }
        declare class QueryICCIDMessageReply extends BaseUpDeviceMessage
        {
          iccid:String;
        }
        declare class DeviceBindMessageReply extends BaseUpDeviceMessage
        {
          MessageId:String;
          IsSuccess:boolean;
          Reason:String;
        }

        declare class MessageContext
        {
          /**
           * 修改通道信息（需要通道允许修改）
           * @param {String} info - 通道信息
           */
          SetChannelInfo(info:String):void;
          /**
           * 缓存键值对
           * @param {String} key - 键
           * @param {String} value - 值
           */
          SetCache(key:String,value:String):void;
          /**
           * 获取缓存的值
           * @param {String} key - 键
           * @returns {String} 返回值
           */
          GetCache(key:String):String;
          /**
           * 获取当前设备的所有属性信息
           * @returns {Object} 设备属性信息
           */
          GetProps():Object;
          /**
           * 获取当前下发消息
           * @returns {RequestMessage} 返回请求消息
           */
          Message():RequestMessage;
          /**
           * 创建写用字节流
           * @returns {FastWriter} 返回写用字节流
           */
          Payload():FastWriter;
          /**
           * 对象转JSON字符串
           * @returns {String} 返回JSON字符串
           */
          ToJson(obj:Object):String;
          /**
           * 转设备属性消息
           * @returns {ReadPropertyMessage} 设备属性消息
           */
          ToPropertyMessage():ReadPropertyMessage;
          /**
           * 转设备绑定消息
           * @returns {DeviceBindMessage} 设备绑定消息
           */
          ToBindMessage():DeviceBindMessage;
          /**
           * 打印日志
           */
          Print(msg:Object):void;
          /**
           * 当前时间戳
           */
          Now():Number;
          /**
           * 创建属性回复包
           * @returns {ReadPropertyMessageReply} 返回属性回复包
           */
          CreatePropertyReply():ReadPropertyMessageReply;
          /**
           * 创建绑定回复包
           */
          CreateBindReply():DeviceBindMessageReply;
          /**
           * 创建功能回复包
           */
          CreateFuncReply():FunctionInvokeMessageReply;
          /**
           * 创建ICCID回复包
           */
          CreateICCIDReply():QueryICCIDMessageReply;
          /**
           * 创建设备事件包
           */
          CreateEventMessage():DeviceEventMessage;
          /**
           * 创建ModbusHex数组
           * @param {Boolean} space - 每个字节间是否空格
           * @returns {Array} 返回Hex数组
           */
          CreateModbusHex(space:Boolean):Array;
          /**
           * 获取最新固件路径
           * @param {String} tag - 固件类型
           * @returns {String} 返回固件的http路径
           */
          GetFirmwareNewest(tag:String):String;
          /**
           * 获取指定名称和标签的固件
           * @param {String} name - 固件版本
           * @param {String} tag - 固件类型
           * @returns {String} 返回固件的http路径
           */
          GetFirmware(name:String,tag:String):String;
          /**
           * 获取Modbus信息
           * @returns {ModbusInfo} 返回Modbus信息
           */
          GetModbusInfo():ModbusInfo;
          /**
           * 等待回复消息（不发送）
           * @param {String} msgId - 消息标识
           * @returns {String} 回复的数据
           */
           Wait(msgId:String):String;
           /**
           * 直接推送数据并返回复的消息
           * @param {String} msgId - 消息标识
           * @param {ArrayBuffer} bytes - 推送的字节数组
           * @returns {String} 推送数据返回
           */
          PublicWait(msgId:String,bytes:ArrayBuffer):String;
          /**
           * 直接推送数据
           * @param {ArrayBuffer} bytes - 推送的字节数组
           */
          Public(bytes:ArrayBuffer):void;
          /**
           * 推送字符串数据并返回复的消息
           * @param {String} msgId - 消息Id
           * @param {String} input - 输入字符串
           * @param {Boolean} hex - 是否为hex字符串
           */
           PublicStrWait(msgId:String,input:String,hex:Boolean):String;
          /**
           * 推送字符串数据
           * @param {String} input - 输入字符串
           * @param {Boolean} hex - 是否为hex字符串
           */
           PublicStr(input:String,hex:Boolean):void;
          /**
           * 发送确认回复包
           */
          ConfirmReply(msgId:String,msg:BaseUpDeviceMessage):void;
        }
        declare class FuncMessageContext extends MessageContext
        {
          /**
           * 获取请求的功能消息
           * @returns {FunctionInvokeMessage} 返回请求消息
           */
           FuncMessage():FunctionInvokeMessage;
           /**
           * 通知前端跳转到指定url
           * @param {String} url - 要跳转的url
           */
           GoUrl(url:String):void;
           /**
           * 通知前端展示信息
           * @param {String} msg - 要展示的信息
           */
           ShowMsg(msg:String):void;
           /**
           * 刷新前端功能列表
           */
           Refresh():void;
           /**
           * 变更所属产品
           * @param {String} targetId - 产品编号
           */
           ChangeProduct(targetId:String):void;
           /**
           * 功能执行成功
           * @param {Object} outputs - 功能的返回值
           */
           ConfirmSuccess(outputs:Object):void;
           /**
           * 功能执行失败
           * @param {String} error - 错误信息
           */
           ConfirmError(error:String):void;
           /**
           * 定时执行
           * @param {Function} ac - 定时执行的函数
           * @param {Number} millisecond - 定时时间,单位为毫秒
           */
           SetTimeout(ac:Function,millisecond:Number):void;
           /**
           * 执行设备的其它功能
           * @param {String} funId - 功能代码
           * @param {Object} inputs - 参数
           * @returns {Object} 返回执行结果
           */
           Execute(funId:String,inputs:Object):Object;
           /**
           * 执行其它设备的功能
           * @param {String} deviceId - 设备通讯Id
           * @param {String} funId - 功能代码
           * @param {Object} inputs - 参数
           * @returns {Object} 返回执行结果
           */
           ExecuteOther(deviceId:String,funId:String,inputs:Object):Object;
        }
        `;
  
      monaco.languages.typescript.javascriptDefaults.addExtraLib(
        fact,
        "customFileName"
      );
      this.monacoEditor = monaco.editor.create(this.$refs.container, {
        value: '',
        language: this.language,
        scrollbar:{
          handleMouseWheel: false,
          vertical:"hidden"
        },
        theme: this.theme,
        ...this.options,
      });
  
      this.monacoEditor.onDidChangeModelContent((event) => {
        this.monacoContent = this.monacoEditor.getValue();
        this.$emit("input", this.monacoContent);

        // 获取内容高度
        var lineHeight = this.monacoEditor.getOption(monaco.editor.EditorOption.lineHeight);
        var lineCount = this.monacoEditor.getModel().getLineCount();
        var contentHeight = lineHeight * lineCount + 50;
        // 设置编辑器高度
        this.monacoEditor.getDomNode().style.height = contentHeight + "px";
        this.monacoEditor.layout(); // 重新布局编辑器
      });
      this.$nextTick(()=>{
        this.monacoEditor.setValue(this.monacoContent);
        this.monacoEditor.trigger('', 'editor.action.formatDocument');
      })

    },
    methods: {
      //格式化代码
      formatCode() {
        this.monacoEditor.getAction("editor.action.formatDocument").run();
      },
      allInsertCode(txt){
        let position = this.monacoEditor.getPosition();
        let insertText = txt;
        this.monacoEditor.executeEdits('', [
            {
                range: {
                    startLineNumber: position.lineNumber,
                    startColumn: position.column,
                    endLineNumber: position.lineNumber,
                    endColumn: position.column
                },
                text: insertText
            }
        ]);
        this.monacoEditor.getAction("editor.action.formatDocument").run();
      },
      insertParamFun(){
        this.allInsertCode(`let pars = context.FuncMessage().Inputs;
    context.PublicStr(JSON.stringify(pars),false);`);
      },
      insertExeFun(){
        this.allInsertCode(`let frs = context.Execute("funcId",{});`);
      },
      insertWait(){
        this.allInsertCode(`let wrs = context.Wait("msgId");`);
      },
      insertPublicWait(){
        this.allInsertCode(`let prs = context.PublicStrWait("msgId","11233",false);`);
      },
      insertRefresh(){
        this.allInsertCode(`context.Refresh();`);
      },
      insertJmp(){
        this.allInsertCode(`context.GoUrl("跳转的url地址");`);
      },
      insertShow(){
        this.allInsertCode(`context.ShowMsg("要展示的信息");`);
      }
    },
  };
  </script>
  
  <style lang="less">
  .jsfunc {
    border: 1px solid #dddddd;
    padding: 0;
    .button_con {
      width: 100%;
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding-top: 5px;
      padding-bottom: 15px;
      padding-left: 10px;
      padding-right: 20px;
      background-color: #f7f8fa;
      .bt-item{
        margin-top: 10px;
        margin-left: 10px;
      }
    }
    .js_func_con {
      .code-container {
        border-top: 1px solid #dddddd;
        font-size: 16px;
      }
    }
  }
  .read_js_func_con {
    border: 1px solid #dddddd;
    border-left: none;
  }
  </style>