<template>
  <div>
    <el-form label-position="top" label-width="90px">
      <el-form-item label="选择触发的动作" prop="text" class="user-type">
        <el-radio-group v-model="config.type" @input="typeChange">
          <el-radio label="WEBHOOK">发送网络请求</el-radio>
          <el-radio label="EMAIL">发送邮件</el-radio>
          <el-radio label="NEWFLOW">发起子流程</el-radio>
        </el-radio-group>
      </el-form-item>
      <div v-if="config.type === 'WEBHOOK'">
        <el-form-item label="请求地址" prop="text">
          <el-input placeholder="请输入URL地址" size="medium" v-model="config.http.url">
            <el-select v-model="config.http.method" style="width: 85px" slot="prepend" placeholder="URL">
              <el-option label="GET" value="GET"></el-option>
              <el-option label="POST" value="POST"></el-option>
              <el-option label="PUT" value="PUT"></el-option>
              <el-option label="DELETE" value="DELETE"></el-option>
            </el-select>
          </el-input>
        </el-form-item>
        <el-form-item label="Header请求头" prop="text">
          <div slot="label">
            <span style="margin-right: 10px">请求头</span>
            <el-button type="text" @click="addItem(config.http.headers)"> + 添加</el-button>
          </div>
          <div v-for="(header, index) in config.http.headers" :key="header.name">
            -
            <el-input placeholder="参数名" size="small" style="width: 100px" v-model="header.name" />
            <el-radio-group size="small" style="margin: 0 5px" v-model="header.isField">
              <el-radio-button :label="true">表单</el-radio-button>
              <el-radio-button :label="false">固定</el-radio-button>
            </el-radio-group>
            <el-select v-if="header.isField" style="width: 180px" v-model="header.value" size="small"
              placeholder="请选择表单字段">
              <el-option v-for="form in forms" :key="form.id" :label="form.title" :value="form.id"></el-option>
            </el-select>
            <el-input v-else placeholder="请设置字段值" size="small" v-model="header.value" style="width: 180px" />
            <el-icon class="el-icon-delete" @click="delItem(config.http.headers, index)"
              style="margin-left: 5px; color: #c75450; cursor: pointer" />
          </div>
        </el-form-item>
        <el-form-item label="接口请求参数" prop="text">
          <div slot="label">
            <span style="margin-right: 10px">请求参数 </span>
            <el-button style="margin-right: 20px" type="text" @click="addItem(config.http.xparams)"> + 添加</el-button>
            <span>参数类型 - </span>
            <el-radio-group size="mini" style="margin: 0 5px" v-model="config.http.contentType">
              <el-radio-button label="JSON">json</el-radio-button>
              <el-radio-button label="FORM">form</el-radio-button>
            </el-radio-group>
          </div>
          <div v-for="(param, index) in config.http.xparams" :key="param.name">
            -
            <el-input placeholder="参数名" size="small" style="width: 100px" v-model="param.name" />
            <el-radio-group size="small" style="margin: 0 5px" v-model="param.isField">
              <el-radio-button :label="true">表单</el-radio-button>
              <el-radio-button :label="false">固定</el-radio-button>
            </el-radio-group>
            <el-select v-if="param.isField" style="width: 180px" v-model="param.value" size="small"
              placeholder="请选择表单字段">
              <el-option v-for="form in forms" :key="form.id" :label="form.title" :value="form.id"></el-option>
            </el-select>
            <el-input v-else placeholder="请设置字段值" size="small" v-model="param.value" style="width: 180px" />
            <el-icon class="el-icon-delete" @click="delItem(config.http.params, index)"
              style="margin-left: 5px; color: #c75450; cursor: pointer" />
          </div>
          <div></div>
        </el-form-item>
        <el-form-item label="请求结果处理" prop="text">
          <div slot="label">
            <span>请求结果处理</span>
            <span style="margin-left: 20px">自定义脚本: </span>
            <el-switch v-model="enableScript"></el-switch>
          </div>
          <span class="item-desc" v-if="config.http.handlerByScript">
            👉 返回值为 true 则流程通过，为 false 则流程将被驳回
            <div>
              支持函数<span style="color: dodgerblue">setFormByName(<span style="color: #939494">'表单字段名',
                  '表单字段值'</span>)</span>可改表单数据
            </div>
          </span>
          <span class="item-desc" v-else>👉 无论请求结果如何，均通过</span>
          <div v-show="config.http.handlerByScript">
            <div>
              <span>脚本处理😀：</span>
              <div ref="myCm" class="code-editor"></div>
            </div>
          </div>
        </el-form-item>
      </div>
      <div v-else-if="config.type === 'EMAIL'">
        <el-form-item label="邮件主题" prop="text">
          <el-input placeholder="请输入邮件主题" size="medium" v-model="config.email.subject" />
        </el-form-item>
        <el-form-item label="收件方" prop="text">
          <el-select size="small" style="width: 100%" v-model="config.email.to" filterable multiple allow-create
            default-first-option placeholder="请输入收件人">
            <el-option v-for="item in config.email.to" :key="item" :label="item" :value="item"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="邮件正文" prop="text">
          <el-input type="textarea" v-model="config.email.content" :rows="4"
            placeholder="邮件内容，支持变量提取表单数据 ${表单字段名} "></el-input>
        </el-form-item>
      </div>
      <div v-else-if="config.type === 'NEWFLOW'">
        <el-form-item label="流程模板" prop="text">
          <el-select v-model="config.flow.templateId" placeholder="请选择" filterable clearable @change="changeTemplate">
            <el-option v-for="item in processOptions" :key="item.Id" :label="item.Name" :value="item.Id">
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="目标流程发起人" prop="text" v-if="config.flow.templateId">
          <el-select v-model="config.flow.creator" placeholder="请选择">
            <el-option v-for="item in personSelOptions" :key="item.id" :label="item.title" :value="item.id">
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="目标流程自选人" prop="text"
          v-if="config.flow.templateId && config.flow.assign && config.flow.assign.length > 0">
          <div class="item_row" v-for="(rowIt, inx) in config.flow.assign" :key="'assign_user' + inx"
            :style="{ 'margin-top': inx > 0 ? '5px' : 0 }">
            <el-input v-model="config.flow.assign[inx].NodeName" placeholder="请输入内容" :disabled="true"
              style="width:180px"></el-input>
            <span>-</span>
            <el-select v-model="config.flow.assign[inx].fieldid" placeholder="请选择">
              <el-option v-for="item in personSelectForm" :key="item.id" :label="item.title" :value="item.id">
              </el-option>
            </el-select>
          </div>
        </el-form-item>
        <el-form-item label="目标流程表单初始化" prop="text"
          v-if="config.flow.templateId && config.flow.items && config.flow.items.length > 0">
          <div class="item_row" v-for="(rowIt, inx) in config.flow.items" :key="'form_user' + inx"
            :style="{ 'margin-top': inx > 0 ? '5px' : 0 }">
            <el-input v-model="config.flow.items[inx].title" placeholder="请输入内容" :disabled="true"
              style="width:180px"></el-input>
            <span>-</span>
            <el-select v-model="config.flow.items[inx].fieldid" placeholder="请选择" clearable>
              <el-option v-for="item in getMappingForm(config.flow.items[inx].eltype)" :key="item.id"
                :label="item.title" :value="item.id">
              </el-option>
            </el-select>
          </div>
        </el-form-item>
        <el-form-item label="返回目标流程编码" prop="text" v-if="config.flow.templateId">
          <el-select v-model="config.flow.flowas" placeholder="请选择" clearable>
            <el-option v-for="item in textForm" :key="item.id" :label="item.title" :value="item.id">
            </el-option>
          </el-select>
        </el-form-item>
      </div>
    </el-form>
  </div>
</template>

<script>
import * as monaco from "monaco-editor";
import { getItems } from "../../utlity"
import { definitionList } from "@/api/flowable/process";
import { getFormDetail } from "@/api/flowable/design";
export default {
  name: "TriggerNodeConfig",
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  computed: {
    forms() {
      return getItems(this.$store.state.flowable.design.formItems) || [];
    },
    personSelectForm() {
      let list = this.forms.filter(row => row.name == "UserPicker")
      return list
    },
    textForm() {
      let list = this.forms.filter(row => row.name == "TextInput")
      return list
    },
    enableScript: {
      get() {
        return this.config.http.handlerByScript;
      },
      set(val) {
        this.config.http.handlerByScript = val;
        this.initScript();
      }
    },
  },
  data() {
    return {
      monacoEditor: null,
      isfirst: true,
      processOptions: [],
      personSelOptions: [],
      allFormFields: [],
    };
  },
  mounted() {
    this.initScript();
    this.listDefinition()
    if (this.config.flow.templateId) {
      this.loadTemplateInfo(this.config.flow.templateId, true)
    }
  },
  methods: {
    getMappingForm(type) {//过滤相同类型的表单进行映射
      return this.forms.filter(row => row.name == type)
    },
    async changeTemplate(val) {
      if (val) {
        await this.loadTemplateInfo(val)
      }
    },
    async loadTemplateInfo(id, isfirstload) {
      let rsp = await getFormDetail(id);
      // console.log("rsp选择的流程信息",rsp);
      // this.formName = rsp.data.Form.FormName;
      let rootNode = JSON.parse(rsp.data.FlowJson);
      let jsondata = JSON.parse(rsp.data.Form.FormFields);
      this.allFormFields = JSON.parse(JSON.stringify(jsondata))
      this.personSelOptions = jsondata.filter(row => row.name == 'UserPicker')
      if (isfirstload) { } else {
        if (this.config.flow && this.config.flow.assign) {
          this.config.flow.assign = []
        }
        if (this.config.flow && this.config.flow.items) {
          this.config.flow.items = []
        }
        for (let i = 0; i < this.allFormFields.length; i++) {
          let activeForm = this.allFormFields[i]
          let rowPush = {
            id: activeForm.id,
            title: activeForm.title,
            eltype: activeForm.name,
            fieldid: '',
          }
          this.config.flow.items.push(rowPush)
        }
        this.loadFlowTree(rootNode)
      }

    },
    loadFlowTree(row) {
      //加载各个流程节点
      if (row.props && row.props.assignedType && row.props.assignedType == 'SELF_SELECT') {//子流程自选人
        let rowPush = {
          NodeId: row.id,
          NodeName: row.name,
          fieldid: '',
        }
        this.config.flow.assign.push(rowPush)
      }
      if (row.branchs && row.branchs.length > 0) {//处理条件节点和分支节点问题
        for (let i = 0; i < row.branchs.length; i++) {
          this.loadFlowTree(row.branchs[i])
        }
      }
      if (row.children) {
        this.loadFlowTree(row.children)
      }
    },
    typeChange(value) {
      if (value == 'NEWFLOW') {
        if (this.config.flow) { } else {
          this.config.flow = {
            templateId: '',
            creator: '',
            assign: [{
              NodeId: '',
              NodeName: '',
              fieldid: '',
            }],
            items: [{
              id: '',
              title: '',
              eltype: '',
              fieldid: '',
            }],
            flowas: '',
          }
        }
      }
    },
    listDefinition() {
      definitionList({ IsEmbed: false }).then(response => {
        this.processOptions = []
        response.data.map(row => {
          let curtemplateId = this.$store.state.flowable.design.Id;
          let newItems = row.Items.filter(x => x.Id != curtemplateId);
          this.processOptions = [...this.processOptions, ...newItems]
        })
      });
    },
    initScript() {
      if (this.config.http.handlerByScript && this.isfirst) {
        this.isfirst = false;
        this.$nextTick(() => {
          monaco.languages.typescript.javascriptDefaults.addExtraLib('', "customFileName");
          this.monacoEditor = monaco.editor.create(this.$refs.myCm, {
            value: this.config.http.script,
            language: "javascript",
            theme: "vs"
          });
        })
      }
    },
    addItem(items) {
      if (items.length > 0 && (items[items.length - 1].name.trim() === "" || items[items.length - 1].value.trim() === "")) {
        this.$message.warning("请完善之前项后在添加");
        return;
      }
      items.push({ name: "", value: "", isField: true });
    },
    delItem(items, index) {
      items.splice(index, 1);
    }
  },
};
</script>

<style lang="less">
.item-desc {
  color: #939494;
}

.code-editor {
  padding-top: 15px;
  min-height: 200px;
  font-size: 16px;
  height: auto;
}
</style>
