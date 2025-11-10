<template>
  <div>
    <div style="border: 1px solid #ccc;" v-if="editOpen">
      <Toolbar style="border-bottom: 1px solid #ccc" :editor="editor" :defaultConfig="toolbarConfig" :mode="mode" />
      <Editor :style="styles" class="wang-editor" :value="value" :defaultConfig="editorConfig" :mode="mode"
        @onCreated="onCreated" @input="handleInput" @onChange="onChange" />
      <mention-modal v-if="isShowModal" @hideMentionModal="hideMentionModal"
        @insertMention="insertMention"></mention-modal>
    </div>
    <!-- <div style="margin-top: 10px;">
      <textarea v-model="html" style="width: 100%; height: 500px;"></textarea>
    </div> -->
  </div>
</template>

<script>

import { getToken } from "@/utils/auth";
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import { DomEditor, Boot } from '@wangeditor/editor'
import mentionModule from '@wangeditor/plugin-mention'
import MentionModal from './MentionModal'

// 注册插件
Boot.registerModule(mentionModule)
export default {
  name: "textEditor",
  components: { Editor, Toolbar,MentionModal  },
  props: {
    /* 编辑器的内容 */
    value: {
      type: String,
      default: "",
    },
    /* 高度 */
    height: {
      type: Number,
      default: null,
    },
    /* 最小高度 */
    minHeight: {
      type: Number,
      default: null,
    },
    /* 只读 */
    readOnly: {
      type: Boolean,
      default: false,
    },
    // 上传文件大小限制(MB)
    fileSize: {
      type: Number,
      default: 5,
    },
    editOpen: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      editor: null,
      html: '<p>你好<span data-w-e-type="mention" data-w-e-is-void data-w-e-is-inline data-value="A张三" data-info="%7B%22id%22%3A%22a%22%7D">@A张三</span></p>',
      toolbarConfig: {
        // toolbarKeys: [
        //   'bold',
        //   'underline',
        //   'italic',
        //   'insertLink',
        //   'through',
        //   'clearStyle',
        //   'color',
        //   'bgColor',
        //   'fontSize',
        //   'justifyLeft',
        //   'justifyRight',
        //   'justifyCenter',
        //   'justifyJustify',
        //   'lineHeight',
        //   'header1',
        //   'header2',
        //   'header3',
        //   'header4',
        //   'header5',
        //   'bulletedList',
        //   'numberedList',
        // 'uploadImage',
        //   "sup",
        //   "sub",
        //   "uploadVideo"
        // ],
        // excludeKeys: [], // 隐藏指定的菜单项
        excludeKeys: [
          'insertVideo', // 删除视频
          'uploadVideo',
          'group-video',
          'insertImage',// 删除网络图片上传
          // 'insertLink',// 删除链接
          // 'insertTable',// 删除表格
          // 'codeBlock',// 删除代码块
        ]
      },
      editorConfig: {
        placeholder: '请输入内容...', MENU_CONF: {
          uploadImage: {
            server: process.env.VUE_APP_BASE_API == "/"
              ? "/AuthService/File/Upload?withDomain=true"
              : process.env.VUE_APP_BASE_API + "/AuthService/File/Upload?withDomain=true",
            maxFileSize: this.fileSize * 1024 * 1024,//乘以两个1024，转换为Mb 
            headers: {
              Authorization: getToken(),
            },
            // 自定义插入图片
            customInsert(res, insertFn) {
              insertFn(res.data, "", "")
            },
            base64LimitSize: 5 * 1024, // 5kb
            // 上传错误，或者触发 timeout 超时
            onError(file, err, res) {
              if (err && JSON.parse(JSON.stringify(err)).isRestriction) {
                this.$message.error({
                  message: '上传图片大小不能超过10MB',
                });
                return
              }
            }
          }
        },
        EXTEND_CONF: {
          mentionConfig: {
            showModal: this.showMentionModal,
            hideModal: this.hideMentionModal,
          },
        },
      },
      mode: 'default', // or 'simple'
      isShowModal: false
    };
  },
  computed: {
    styles() {
      let style = {};
      if (this.minHeight) {
        style.minHeight = `${this.minHeight}px`;
      }
      if (this.height) {
        style.height = `${this.height}px`;
      }
      return style;
    },
    comp() {
      return this.info.map(i => '0_' + i)
    }
  },
  watch: {
    readOnly(newV, oldV) {
      if (editor == null) return;
      if (newV) {
        editor.disable();
      }
      else {
        editor.enable();
      }
    },
    fileSize(newV, oldV) {
      this.editorConfig.MENU_CONF['uploadImage'].maxFileSize = newV;
    }
  },
  mounted() {
  },
  beforeDestroy() {
    const editor = this.editor
    if (editor == null) return
    editor.destroy() // 组件销毁时，及时销毁编辑器
  },
  methods: {
    showMentionModal() {
      this.isShowModal = true
    },
    hideMentionModal() {
      this.isShowModal = false
    },
    insertMention(id, name) {
      const mentionNode = {
        type: 'mention', // 必须是 'mention'
        value: name,
        info: id,
        children: [{ text: '' }], // 必须有一个空 text 作为 children
      }
      const editor = this.editor
      if (editor) {
        editor.restoreSelection() // 恢复选区
        editor.deleteBackward('character') // 删除 '@'
        editor.insertNode(mentionNode) // 插入 mention
        editor.move(1) // 移动光标
      }
    },
    onChange(editor) {
      // 点击一下富文本编辑页窗口就会执行
      const toolbar = DomEditor.getToolbar(editor)
      // console.log("工具栏配置", toolbar.getConfig().toolbarKeys); // 工具栏配置
      // this.toolbarConfig.toolbarKeys=toolbar.getConfig().toolbarKeys
      this.toolbarConfig = JSON.parse(JSON.stringify(toolbar.getConfig()))

      this.curHtml = editor.getHtml()
    },
    onCreated(editor) {
      this.editor = Object.seal(editor)
      if (this.readOnly) {
        editor.disable();
      }
      else {
        editor.enable();
      }
    },
    handleInput(value) {
      console.log(value)
      // 将文本内容传给父组件（引用这个组件的vue文件的）
      this.$emit('input', value)
    }

  },
};
</script>

<style src="@wangeditor/editor/dist/css/style.css"></style>
<style lang="less" scoped>
.wang-editor {
  overflow-y: hidden;
}

.w-e-full-screen-container {
  z-index: 99;
}

.w-e-for-vue {
  margin: 0;
  border: 1px solid #ccc;

  .w-e-for-vue-toolbar {
    border-bottom: 1px solid #ccc;
  }

  .w-e-for-vue-editor {
    height: auto;

    /deep/ .w-e-text-container {

      .w-e-text-placeholder {
        top: 6px;
        color: #666;
      }

      pre {

        code {
          text-shadow: unset;
        }
      }

      p {
        margin: 5px 0;
        font-size: 14px; // 设置编辑器的默认字体大小为14px
      }
    }
  }
}
</style>
