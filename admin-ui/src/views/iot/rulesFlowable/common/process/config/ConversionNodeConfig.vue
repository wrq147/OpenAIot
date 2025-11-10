<template>
  <div>
    <el-form class="jiaoben_con" label-position="top">
      <el-form-item label="脚本内容:">
        <div style="background-color: #f3f3f3;padding: 5px 15px;width:100%;border-top:solid 1px #dadada;border-left: solid 1px #dadada;border-right: solid 1px #dadada;">
          <el-button type="primary" size="small" @click="formatCode">格式化</el-button>
          <el-button type="primary" size="small" @click="openScriptEdit">弹窗编辑</el-button>
        </div>
        <div style="width:100%;">
          <div class="code-container" ref="container"></div>
        </div>
      </el-form-item>
    </el-form>
    <el-dialog title="编辑脚本内容" append-to-body :close-on-click-modal="false" :visible.sync="scriptEditorOpen" width="980px" top="2vh" :destroy-on-close="true" @close="scriptEditorOpen = false">
      <data-editor @submitData="submitData" @cancelData="scriptEditorOpen = false" :extLibs="extLibs" :customData="config.func"/>
    </el-dialog>
  </div>
</template>

<script>
import * as monaco from "monaco-editor";
import DataEditor from "../../runcode/DataEditor";
export default {
  name: "ConversionNodeConfig",
  components: {
    DataEditor
  },
  props: {
    language: { type: String, default: "javascript" },
    theme: { type: String, default: "vs" },
    options: {
      type: Object,
      default() {
        return {};
      }
    },
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
      // 编辑器对象
      monacoEditor: null,
      scriptEditorOpen:false,
      extLibs:`
      declare class MergeItem
      {
        /**
         * 合计方式：最大值:max，最小值：min，平均值：mean，合计：sum，期初值：first，期末值：last
         */
        MergeWay:string;
        /**
         * 时间
         */
        Time:Number;
        /**
         * 值
         */
        Val:Object;
        /**
         * 单位
         */
        Unit:String;
      }
      declare class StreamData
      {
        /**
         * 数据
         */
        Data:Object;
        /**
         * 时间
         */
        Time:Number;
      }
      declare class JsContext
      {
        /**
        * 清除事件沉默周期
        * @param {String} code - 事件标识
        * @return
        */
        ResetSilenceTime(code:String):void
        /**
        * 执行指定设备的功能
        * @param {String} devId - 为null表示当前设备
        * @param {String} functionId - 功能标识符
        * @param {Object} data - 功能参数
        * @return Object
        */
        ExeFunc(devId:String,functionId:String,data:Object):Object
        /**
        * 设置参数
        * @param {String} key - 键名
        * @param {Object} value - 值
        * @return
        */
        SetParam(key:String,value:Object):void
        /**
        * 获取参数
        * @param {String} key - 键名
        * @return Object
        */
        GetParam(key:String):Object
        /**
        * 获取节点的输入数据
        * @return Array
        */
        Input():Array
        /**
        * 打印输出到控制台
        * @param {Object} obj
        * @return
        */
        Print(msg:Object):void
        /**
        * 获取指定设备属性信息
        * @param {String} devId - 设备Id,为null则获取当前设备
        * @return Object
        */
        DeviceProp(devId:String):Object
        /**
        * 通过dtuid获取设备属性信息
        * @param {String} dtuId - 设备的dtuId,为null则获取当前设备
        * @return Object
        */
        DevicePropByDtuId(dtuId:String):Object
        /**
        * 修改设备属性值
        * @param {String} dtuId - 为null表示当前设备
        * @param {String} code - 属性标识符
        * @param {Object} val - 修改的值
        * @return
        */
        SetProp(dtuId:String,code:String,val:Object):void
        /**
        * 获取指定设备标签信息
        * @param {String} devId - 设备Id,为null则获取当前设备
        * @return Object
        */
        DeviceTag(devId:String):Object
        /**
        * 修改设备标签
        * @param {String} devId - 为null表示当前设备
        * @param {String} code - 标签标识符
        * @param {Object} val - 修改的值
        * @return
        */
        SetTag(devId:String,code:String,val:Object):void
        /**
        * 创建一条数据
        * @param {Object} data
        * @return StreamData
        */
        CreateData(data:Object):StreamData
        /**
        * 触发指定事件
        * @param {String} code
        * @param {Object} data
        * @return
        */
        TouchEvent(code:String,data:Object):void
        /**
        * 获取指定的聚合数据
        * @param {String} name - 聚合数据
        * @return Array
        */
        CountList(name:String):Array
        /**
        * 查询历史记录数据
        * @param {String} id - 设备Id，传null为当前设备
        * @param {String} merge - 合计方式，最大值:max，最小值：min，平均值：mean，合计：sum，期初值：first，期末值：last
        * @param {number} window - 合计窗口，0表示按日，1表示按月，2表示按时，3表示按分
        * @param {String} code - 属性标识
        * @param {Date} beginTime - 开始时间
        * @param {Date} endTime - 结束时间
        * @return Array<MergeItem>
        */
        QueryMergeList(id:String,merge:String,window:number,code:String,beginTime:Date,endTime:Date):Array<MergeItem>
        /**
        * 执行下一个节点，追加数据
        * @param {Object} obj
        * @return
        */
        NextObject(obj:Object):void
        /**
        * 执行下一个节点，追加数据流
        * @param {Array<StreamData>} obj
        * @return
        */
        Next(obj:Array<StreamData>):void
        /**
        * Http的Get请求
        * @param {String} url
        * @return String
        */
        HttpGet(url:String):String
        /**
        * Http的Post请求
        * @param {String} url
        * @param {Object} postParams
        * @return String
        */
        HttpPost(url:String,postParams:Object):String
        /**
        * Http的Post请求(Json格式)
        * @param {String} url
        * @param {Object} postParams
        * @return String
        */
        HttpPostJson(url:String,postParams:Object):String
      }`
    };
  },
  watch: {
    options: {
      deep: true,
      handler(options) {
        this.monacoEditor && this.monacoEditor.updateOptions(options);
      }
    }
  },
  mounted() {
    const fact = this.extLibs;

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
      ...this.options
    });

    this.monacoEditor.onDidChangeModelContent(event => {
      this.config.func = this.monacoEditor.getValue();
      this.$emit("change", this.config.func);


      // 获取内容高度
      var lineHeight = this.monacoEditor.getOption(monaco.editor.EditorOption.lineHeight);
      var lineCount = this.monacoEditor.getModel().getLineCount();
      var contentHeight = lineHeight * lineCount + 50;
      // 设置编辑器高度
      this.monacoEditor.getDomNode().style.height = contentHeight + "px";
      this.monacoEditor.layout(); // 重新布局编辑器
    });
    this.$nextTick(()=>{
      this.monacoEditor.setValue(this.config.func);
      this.monacoEditor.trigger('', 'editor.action.formatDocument');
    });
  },
  methods: {
    openScriptEdit(){
      this.scriptEditorOpen=true
    },
    submitData(data){
      //弹窗的代码编辑
      // console.log("编辑结果",data);
      this.config.func = data;
      this.$emit("change", this.config.func);
      this.monacoEditor.setValue(this.config.func);
      this.monacoEditor.trigger('', 'editor.action.formatDocument');
      this.scriptEditorOpen=false
    },
    //格式化代码
    formatCode() {
      this.monacoEditor.getAction("editor.action.formatDocument").run();
    }
  }
};
</script>

<style lang="less" scoped>
.code-container {
  border-top: 1px solid #dddddd;
  font-size: 16px;
  border: 1px solid #dddddd;
  padding-top: 15px;
}

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
