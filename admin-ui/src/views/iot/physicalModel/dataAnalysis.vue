<template>
  <div :class="[isdialog ? '' : 'elbiaoge_elform']" :style="{ 'min-height': isdialog ? '' : 'calc(100vh - 194px' }">
    <el-tabs type="card" @tab-click="tagChange">
      <el-tab-pane label="脚本解释器" v-if="ChannelData.CanScript"></el-tab-pane>
      <el-tab-pane label="Modbus解释器" v-if="ChannelData.CanModbus"></el-tab-pane>
    </el-tabs>
    <div v-show="curtag == '脚本解释器'" class="js_con" :style="{ 'margin-top': '0px' }">
      <div class="button_con">
        <div>
          <el-button class="copyBtn" data-clipboard-action="copy" :data-clipboard-text="monacoContent" @click="copyCode"
            type="primary" plain v-if="!isdialog">一键复制</el-button>

          <el-button @click="insertUpCode" type="primary" plain>插入升级代码</el-button>
          <el-button @click="insertPrintCode" type="primary" plain>
            插入调试代码
          </el-button>

          <el-button @click="insertJsonPropCode" type="primary" plain>
            插入JSON属性解释
          </el-button>

          <el-button @click="insertFileUpload" type="primary" plain>
            插入文件上传
          </el-button>
          <el-button @click="choiceTemple" type="primary" plain v-if="!isdialog">
            导入模板
          </el-button>
        </div>
        <div>
          <el-button type="primary" @click="saveCode" v-if="!isdialog">保存代码</el-button>
          <el-button type="primary" @click="formatCode">格式化</el-button>
        </div>
      </div>
      <div class="js_func_con">
        <div class="code-container" ref="container"></div>
      </div>
    </div>
    <modbus v-show="curtag == 'Modbus解释器'" :productInfos="productInfo" :modbusData="modbusData"
      :attrTableData="attrTableData" @saveSetData="saveSetData"></modbus>
    <el-dialog title="选择脚本模板" :close-on-click-modal="false" :visible.sync="scriptTemVisible" width="1000px"
      append-to-body>
      <el-row :gutter="20">
        <el-col :span="24" :xs="24">
          <el-form class="biaodan" :model="scriptQueryParams" ref="scriptQueryForm" :inline="true"
            style="margin-bottom:0">
            <el-form-item label="搜索关键词" prop="SearchKey">
              <el-input class="set_radius" v-model="scriptQueryParams.SearchKey" placeholder="请输入关键字" clearable
                @keyup.enter.native="handleQuery" />
            </el-form-item>
            <el-form-item label="创建日期">
              <el-date-picker class="set_radius" v-model="scriptDateRange" style="width: 232px"
                value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                end-placeholder="结束日期"></el-date-picker>
            </el-form-item>
            <el-form-item class="submit_button_con">
              <el-button icon="el-icon-refresh" @click="resetScriptQuery">重置</el-button>
              <el-button type="primary" icon="el-icon-search" @click="handleScriptQuery">搜索</el-button>
            </el-form-item>
          </el-form>
          <el-row :gutter="10" justify="start">
            <el-col :span="8" v-for="item in iotScriptDataArr" :key="item.Id" style="margin-bottom: 20px">
              <div class="script_li" @click="choiceOneTemplete(item)">
                <div class="script_label">名称：{{ item.Name }}</div>
                <div class="tags_info" v-if="scriptSelected.Id && scriptSelected.Id == item.Id"></div>
                <div>
                  <el-input type="textarea" :autosize="{ minRows: 14, maxRows: 14 }" placeholder="请输入内容"
                    v-model="item.ScriptContent" disabled resize="none" style="background: #ffffff"></el-input>
                </div>
                <div class="remark_cot" v-if="item.Remark">
                  备注：{{ item.Remark }}
                </div>
              </div>
            </el-col>
          </el-row>

          <pagination v-show="scriptTotal > 0" :total="scriptTotal" :page.sync="scriptQueryParams.pageNum"
            :limit.sync="scriptQueryParams.pageSize" :pageSizes="scriptPageSizes" @pagination="loadIotScriptList" />
        </el-col>
      </el-row>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="finishImportScript">确 定</el-button>
        <el-button @click="scriptTemVisible = false">取 消</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import * as monaco from "monaco-editor";
import {
  iotScriptList
} from "@/api/scrtemp.js";

let modbus = () => import("./components/modbus")//modbus配置
export default {
  components: {
    modbus
  },
  props: {
    content: String,
    language: { type: String, default: "javascript" },
    theme: { type: String, default: "vs" },
    options: {
      type: Object,
      default() {
        return {};
      },
    },
    productInfo: {
      type: Object,
      default() {
        return null;
      },
    },
    ChannelData: {
      type: Object,
      default() {
        return {
          CanScript: true,
          CanModbus: false
        };
      },
    },
    isdialog: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      // 编辑器对象
      monacoEditor: null,
      monacoContent: "",
      modbusData: {}, //modbus数据
      attrTableData: [],
      //脚本模板相关参数
      scriptTemVisible: false,
      scriptPageSizes: [6, 12, 18, 24, 30],
      scriptQueryParams: {
        pageNum: 1,
        pageSize: 6,
        SearchKey: ''
      },
      // 日期范围
      scriptDateRange: [],
      scriptTotal: 0,
      iotScriptDataArr: [],
      scriptSelected: {},
      curtag: '脚本解释器'
    };
  },
  watch: {
    options: {
      deep: true,
      handler(options) {
        this.monacoEditor && this.monacoEditor.updateOptions(options);
      },
    },
    productInfo: {
      handler(newVal) {
        if (newVal != null) {
          let jsonLis = JSON.parse(newVal.ModelTSL);
          if (jsonLis.modbus) {
            this.modbusData = jsonLis.modbus;
            this.attrTableData = jsonLis.properties;
          }
        }
      },
      immediate: true
    }
  },
  created() {
    if (this.monacoEditor) {
      this.monacoEditor.dispose();
    }
  },
  mounted() {
    if (this.ChannelData.CanScript == true) {
      this.curtag = "脚本解释器";
    }
    else if (this.ChannelData.CanModbus == true) {
      this.curtag = "Modbus解释器";
    }

    if (this.content == null || this.content == "") {
      this.monacoContent = `
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
    }`;
    } else {
      this.monacoContent = this.content;
    }
    const fact = `
    declare class DataContext{
        /**
         * 上报的扩展信息
         * @returns {String} 获取上报的主题扩展信息
         */
        CodePrefix():String;
        /**
         * 返回当前的回复消息
         * @returns {ReadPropertyMessageReply} 回复消息
         */
        ReplyMessage():ReadPropertyMessageReply;
        /**
         * 字节数组转字符串
         * @returns {FastReader} 字符串
         */
        Payload():FastReader;
        /**
         * 字节数组转Utf8字符串
         * @param {ArrayBuffer} bytes - 字节数组
         * @returns {String} 字符串
         */
        ToUtf8(bytes:ArrayBuffer):String;
        /**
         * 字节数组转ASCII字符串
         * @param {ArrayBuffer} bytes - 字节数组
         * @returns {String} 字符串
         */
        ToASCII(bytes:ArrayBuffer):String;
        /**
         * 字节数组转Hex字符串
         * @param {ArrayBuffer} bytes - 字节数组
         * @param {Boolean} space - 每个字节间是否空格
         * @returns {String} Hex字符串
         */
        ToHex(bytes:ArrayBuffer,space:Boolean):String;
        ToObject(json:String):Object;
        /**
         * 转成IEEE标准的浮点数
         * @param {Object} input - 对象
         * @returns {Number} 数值
         */
        ToFloat(input:Object):Number;
        /**
         * 获取指定属性定义
         * @param {String} code - 属性标识
         * @returns {String} 返回值
         */
        GetTslProps(code:String):BaseProperty;
        /**
         * 获取当前设备的所有属性信息
         * @returns {Object} 设备属性信息
         */
        GetProps():Object;
        /**
         * 创建属性回复包
         */
        CreatePropertyMessage():ReadPropertyMessageReply;
        /**
         * 创建空回复
         */
        CreateEmptyMessage():CreateEmptyMessage;
        /**
          * 创建ICCID回复包
          */
        CreateICCIDReply():QueryICCIDMessageReply;
        /**
          * 创建设备事件包
          */
        CreateEventMessage():DeviceEventMessage;
        Print(msg:Object):void;
        Now():Number;
        /**
         * 回复消息Id
         * @param {String} msgId - 可为null,为null时自动从系统缓存里取
         * @param {String} value - 回复的值
         * @return
         */
        ReplyMsgId(msgId:String,value:String):void;
        /**
         * 直接推送数据
         * @param {ArrayBuffer} bytes - 推送的字节数组
         */
        Public(bytes:ArrayBuffer):void;
        /**
         * 推送字符串数据
         * @param {String} input - 输入字符串
         * @param {Boolean} hex - 是否为hex字符串
         */
        PublicStr(input:String,hex:Boolean):void;
        /**
         * 向指定站点上传文件并返回文件的url
         * @param {String} url - 上传的url地址
         * @param {String} filename - 文件名（带扩展名）
         * @param {ArrayBuffer} bytes - 文件数据
         * @param {Object} headers - 参数（可不传）
         */
        UploadFile(url:String,filename:String,bytes:ArrayBuffer,headers:Object):String;
    }
    declare class MessageContext
      {
        /**
         * 修改通道信息（需要通道允许修改）
         * @param {String} info - 通道信息
         */
        SetChannelInfo(info:String):void;
        /**
         * 获取指定属性定义
         * @param {String} code - 属性标识
         * @returns {String} 返回值
         */
        GetTslProps(code:String):BaseProperty;
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
         * 创建写用字节流
         * @returns {FastWriter} 返回写用字节流
         */
        Payload():FastWriter;
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
         * @returns {DeviceBindMessageReply} 返回绑定回复包
         */
        CreateBindReply():DeviceBindMessageReply;
        /**
         * 创建功能回复包
         * @returns {FunctionInvokeMessageReply} 返回功能回复包
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

      declare class FastReader
      {
        ReadByte():Number;
        ReadBytes(count:Number):ArrayBuffer;
        ReadToEnd():ArrayBuffer;
        ToArray():ArrayBuffer;
        CopyTo(start:Number):FastReader;
        Reset():void;
        /**
         * 标识为数据未完整
         */
        MergeRead():void;
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
      declare class ReadPropertyMessage extends RequestMessage
      {
        /**
         * 可读取多个属性
         */
         Properties:Array;
      }
      declare class ReadPropertyMessageReply extends BaseUpDeviceMessage
      {
        /**
         * 属性键值对
         */
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
      declare class QueryICCIDMessageReply extends BaseUpDeviceMessage
      {
        iccid:String;
      }
      declare class FunctionInvokeMessageReply extends BaseUpDeviceMessage
      {
        MessageId:String;
        Outputs:Object;
      }
      declare class DeviceBindMessageReply extends BaseUpDeviceMessage
      {
        MessageId:String;
        IsSuccess:boolean;
        Reason:String;
      }
      declare class BaseValueOption
      {
        /**
         * 数据类型（必填项）
         */
        type:String;
        /**
         * 预处理表达式（格式：data>0?1:0）
         */
        express:String;
      }
      declare class BaseProperty
      {
        /**
         * 功能名称（必填项）
         */
        name:String;
        /**
         * 标识符（必填项）
         */
        code:String;
        /**
         * 前缀标识符
         */
        prefixcode:String;
        /**
         * 备注
         */
        description:String;
        /**
         * 数据信息
         */
        option:BaseValueOption;
      }
      declare class ModbusInfo
      {
        /**
         * 波特率
         */
        BaudRate:Number;
        /**
         * 数据位
         */
        DataBits:Number;
        /**
         * 奇偶校验
         */
        Parity:String;
        /**
         * 停止位
         */
        StopBits:String;
        Mode:String;
        /**
         * 轮询周期时间(单位ms)
         */
        PollTime:Number;
      }
    `;

    monaco.languages.typescript.javascriptDefaults.addExtraLib(
      fact,
      "customFileName"
    );
    this.monacoEditor = monaco.editor.create(this.$refs.container, {
      value: this.monacoContent,
      language: this.language,
      theme: this.theme,
      ...this.options,
    });

    this.monacoEditor.onDidChangeModelContent((event) => {
      this.monacoContent = this.monacoEditor.getValue();
      this.$emit("change", this.monacoContent);
    });
  },
  beforeDestroy() {
    this.monacoEditor.dispose();
  },
  methods: {
    tagChange(targetName) {
      this.curtag = targetName.label;
    },
    setCodeValue(code) {
      this.monacoEditor.setValue(code)
    },
    returnCode() {
      //返回脚本
      this.monacoContent = this.monacoEditor.getValue();
      return this.monacoContent
    },
    saveCode() {
      this.monacoContent = this.monacoEditor.getValue();
      this.$emit("saveCode", this.monacoContent);
    },
    copyCode() {
      //复制代码
      let clipboard = new this.clipboard(".copyBtn");
      clipboard.on("success", (e) => {
        console.log("复制", e);

        this.$message({
          message: "复制成功！",
          type: "success",
        });
        // //清除选中
        // e.clearSelection();
        //释放内存，以防重复复制
        clipboard.destroy();
      });
      clipboard.on("error", () => {
        this.$message.error("复制失败，请手动选择复制！");
      });
    },
    //格式化代码
    formatCode() {
      this.monacoEditor.getAction("editor.action.formatDocument").run();
    },
    insertUpCode() {
      let position = this.monacoEditor.getPosition();
      let insertText = `let msg = context.Message();
      if (msg.MsgType == "Bind") {
        let bindMsg = context.ToBindMessage();
        let bindReply = context.CreateBindReply();
        context.ConfirmReply(bindMsg.MessageId, bindReply);
        return context.Payload();
      }`;
      this.monacoEditor.executeEdits("", [
        {
          range: {
            startLineNumber: position.lineNumber,
            startColumn: position.column,
            endLineNumber: position.lineNumber,
            endColumn: position.column,
          },
          text: insertText,
        },
      ]);
      this.monacoEditor.getAction("editor.action.formatDocument").run();
    },
    insertPrintCode() {
      let position = this.monacoEditor.getPosition();
      let insertText = `context.Print("请在此处输入打印的内容");`;
      this.monacoEditor.executeEdits("", [
        {
          range: {
            startLineNumber: position.lineNumber,
            startColumn: position.column,
            endLineNumber: position.lineNumber,
            endColumn: position.column,
          },
          text: insertText,
        },
      ]);
      this.monacoEditor.getAction("editor.action.formatDocument").run();
    },
    insertJsonPropCode() {
      let position = this.monacoEditor.getPosition();
      let insertText = `var str = context.ToUtf8(context.Payload().ReadToEnd());
  var strobj = JSON.parse(str);
  var msg = context.CreatePropertyMessage();
  msg.Properties = {
    "属性1": strobj.prop1,
    "属性2": strobj.prop2
  }
  return msg;
  `;
      this.monacoEditor.executeEdits("", [
        {
          range: {
            startLineNumber: position.lineNumber,
            startColumn: position.column,
            endLineNumber: position.lineNumber,
            endColumn: position.column,
          },
          text: insertText,
        },
      ]);
      this.monacoEditor.getAction("editor.action.formatDocument").run();
    },
    insertFileUpload() {
      let position = this.monacoEditor.getPosition();
      let insertText = `context.UploadFile("http://www.baidu.com/FileTemp/Upload","tmp.jpg",context.Payload().ReadToEnd(),{"FileKey":""});`;
      this.monacoEditor.executeEdits("", [
        {
          range: {
            startLineNumber: position.lineNumber,
            startColumn: position.column,
            endLineNumber: position.lineNumber,
            endColumn: position.column,
          },
          text: insertText,
        },
      ]);
      this.monacoEditor.getAction("editor.action.formatDocument").run();
    },
    saveSetData(params, editInfo) {
      this.$emit("saveSetData", params, editInfo);
    },
    //脚本模板相关方法
    choiceTemple() {
      //选择模板
      this.scriptTemVisible = true
      this.loadIotScriptList()
    },
    choiceOneTemplete(item) {
      //选择一个脚本
      this.scriptSelected = item
    },
    finishImportScript() {
      //完成导入
      this.setCodeValue(this.scriptSelected.ScriptContent)
      this.saveCode(this.scriptSelected.ScriptContent)
      this.scriptTemVisible = false
    },
    loadIotScriptList() {
      //获取脚本模板列表
      this.scriptLoading = true
      iotScriptList(this.addDateRange(this.scriptQueryParams, this.scriptDateRange)).then(
        (res) => {
          this.iotScriptDataArr = res.data.List;
          this.scriptTotal = res.data.Total;
          this.scriptLoading = false
        }
      ).catch(err => {
        this.scriptLoading = false
      });
    },
    handleScriptQuery() {
      this.scriptQueryParams.pageNum = 1;
      this.loadIotScriptList();
    },
    resetScriptQuery() {
      //重置搜索
      this.scriptDateRange = [];
      this.resetForm("scriptQueryForm");
      this.handleScriptQuery();
    },

  },
};
</script>

<style lang="less">
.js_con {
  margin-top: 20px;
  border: 1px solid #dddddd;
  padding: 0;

  .button_con {
    width: 100%;
    height: 50px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 30px;
    background-color: #f7f8fa;
  }

  .js_func_con {
    .code-container {
      // overflow-y: auto !important;
      border-top: 1px solid #dddddd;
      min-height: 480px;
      font-size: 16px;
    }
  }
}

.read_js_func_con {
  border: 1px solid #dddddd;
  border-left: none;
}
</style>