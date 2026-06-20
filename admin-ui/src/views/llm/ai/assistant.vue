<template>
  <div class="ai-chat-container">
    <!-- 顶部标题栏 -->
    <div class="chat-header"></div>

    <!-- 消息列表区域 -->
    <div class="chat-message-wrapper">
      <div class="chat-message-box" ref="messageBox" @scroll="onChatScroll">
        <!-- 空状态：居中显示标题 + 快捷提问按钮 -->
        <div v-if="ChatMessageList.length === 0" class="empty-state">
          <h1 class="empty-title">有什么我能帮你的吗？</h1>
          <div class="quick-questions">
            <span class="quick-btn" @click="handleQuickSend('你能干什么？')">你能干什么？</span>
            <span class="quick-btn" @click="handleQuickSend('我有哪些物联网设备？')">我有哪些物联网设备？</span>
            <span class="quick-btn" @click="handleQuickSend('如何开发新的物联网协议？')">如何开发新的物联网协议？</span>
            <span class="quick-btn" @click="handleQuickSend('如何使用规则引擎？')">如何使用规则引擎？</span>
          </div>
        </div>

        <!-- 用户消息 -->
        <div v-for="(item, index) in ChatMessageList" :key="index" class="message-item"
          :class="{ user: item.role === 'user', ai: item.role === 'assistant' }">
          <div class="avatar">
            <i v-if="item.role === 'user'" class="el-icon-user"></i>
            <i v-else class="el-icon-chat-dot-round"></i>
          </div>
          <div class="message-content">
            <div v-if="item.role === 'user'">{{ item.data }}</div>
            <div v-else class="ai-content-wrap">
              <!-- 思考折叠块 -->
              <div v-if="item.think != null && item.think != ''" class="think-block">
                <details open>
                  <summary>🤔 AI思考过程</summary>
                  <div class="think-body markdown-body" v-html="renderMarkdown(item.think)"></div>
                </details>
              </div>
              <!-- 正式回答 -->
              <div class="answer-body markdown-body" v-html="renderMarkdown(item.data)"></div>
            </div>
            <!-- AI 回复加载中动画 -->
            <div v-if="item.status === 1" class="loading-pulse-dots">
              <span class="dot"></span>
              <span class="dot"></span>
              <span class="dot"></span>
            </div>
          </div>
        </div>
      </div>
      <div class="scroll-fade-mask"></div>
      <div v-if="showScrollBottomBtn" class="scroll-bottom-btn" @click="scrollToBottom">
        <svg class="scroll-icon" width="24" height="24" viewBox="0 0 24 24" fill="currentColor" xmlns="http://www.w3.org/2000/svg"><path d="M18.7071 14.0313C19.0976 14.4218 19.0976 15.0548 18.7071 15.4453L13.4161 20.7363C12.6375 21.5147 11.3695 21.5217 10.586 20.7383L5.29301 15.4453C4.90265 15.0548 4.90265 14.4218 5.29301 14.0313C5.68349 13.6408 6.31654 13.6409 6.70707 14.0313L11.0001 18.3242L11.0001 3.23828C11.0001 2.68608 11.4479 2.23842 12.0001 2.23828C12.5523 2.23828 13.0001 2.686 13.0001 3.23828L13.0001 18.3242L17.293 14.0313C17.6835 13.6408 18.3166 13.6409 18.7071 14.0313Z" fill="currentColor"></path></svg>
      </div>
    </div>


    <div class="chat-input-box">
      <div class="input-wrapper">
        <el-input v-model="userInput" type="textarea" :autosize="{ minRows: 1, maxRows: 10 }" placeholder="请输入你的问题..."
          @keyup.enter.native="handleSend" class="chat-input" />
        <el-button type="primary" class="send-btn" @click="handleSend" :loading="isSending"><span
            v-if="!isSending">发送</span></el-button>
      </div>
      <!-- 底部工具栏 -->
      <div class="toolbar">
        <div class="toolbar-left">
          <div class="tool-btn" @click="handleQuickSend('检索下一页')">检索下一页</div>
          <span class="split">|</span>
          <div class="tool-btn" @click="handleQuickSend('检索上一页')">检索上一页</div>
          <span class="split">|</span>
          <div class="tool-btn" @click="handleQuickSend('检索首页')">检索首页</div>
        </div>
        <div class="mode-select-wrap" @click.stop="toggleModePopover">
          <!-- 底部显示当前选中按钮 -->
          <div class="current-mode-btn" :class="{ disabled: isSending }">
            <span class="mode-icon">{{ currentModeItem.icon }}</span>
            <span class="mode-name">{{ currentModeItem.label }}</span>
          </div>
          <!-- 下拉弹窗 -->
          <div v-if="showModePopover" class="mode-popover" @click.stop>
            <div v-for="item in modeList" :key="item.value" class="mode-option"
              :class="{ active: thinkMode === item.value }" @click="selectMode(item.value)">
              <div class="option-left">
                <span class="opt-icon">{{ item.icon }}</span>
                <div class="opt-text">
                  <div class="opt-title">
                    {{ item.label }}
                    <span v-if="item.tag" class="opt-tag">{{ item.tag }}</span>
                  </div>
                  <div class="opt-desc">{{ item.desc }}</div>
                </div>
              </div>
              <span v-if="thinkMode === item.value" class="check-mark">✓</span>
            </div>
          </div>
        </div>


      </div>
    </div>

  </div>
</template>

<script>
import Cookies from "js-cookie";
import { postMessage } from "@/api/llmchat";
import marked from 'marked'
import hljs from 'highlight.js'
import 'highlight.js/styles/github.css'
import 'github-markdown-css/github-markdown.css';
marked.setOptions({
  highlight: (code) => hljs.highlightAuto(code).value,
  gfm: true,
  breaks: true,
});

export default {
  name: 'AiChat',
  data() {
    return {
      showScrollBottomBtn: false,
      userInput: '',
      showModePopover: false,
      thinkMode: 'fast',
      modeList: [
        {
          value: 'fast',
          icon: '⚡',
          label: '快速',
          desc: '适用于大部分情况'
        },
        {
          value: 'expert',
          icon: '⦿',
          label: '专家',
          tag: '深度思考',
          desc: '研究级智能模型'
        }
      ]
    }
  },
  computed: {
    ChatMessageList: function () {
      this.$nextTick(() => {
        setTimeout(() => {
          this.onChatScroll()
        }, 80)
      })
      return this.$store.state.llm.messageList;
    },
    isSending: function () {
      let msglist = this.$store.state.llm.messageList;
      for (let i = msglist.length - 1; i >= 0; i--) {
        if (msglist[i].role == "assistant") {
          if (msglist[i].status == 1) {
            return true;
          }
          else {
            return false;
          }
        }
      }
      return false;
    },
    currentModeItem() {
      return this.modeList.find(item => item.value === this.thinkMode)
    }
  },
  mounted() {
    document.addEventListener('click', this.closePopoverByDoc)
    let tmpmode = Cookies.get("ThinkMode");
    if (tmpmode == null) {
      this.thinkMode = "fast";
    }
    else {
      this.thinkMode = tmpmode;
    }
    this.$nextTick(() => {
      setTimeout(() => {
        this.onChatScroll()
      }, 100)
    })
  },
  beforeDestroy() {
    document.removeEventListener('click', this.closePopoverByDoc)
  },
  methods: {
    onChatScroll() {
      const box = this.$refs.messageBox;
      // 距离底部小于20px视为已到底部，隐藏按钮
      const isBottom = box.scrollHeight - box.scrollTop - box.clientHeight < 20;
      this.showScrollBottomBtn = !isBottom;
    },
    closePopoverByDoc() {
      this.showModePopover = false
    },
    toggleModePopover() {
      if (this.isSending) return
      this.showModePopover = !this.showModePopover
    },
    selectMode(val) {
      this.thinkMode = val
      this.showModePopover = false
      Cookies.set("ThinkMode", val)
    },
    renderMarkdown(content) {
      if (!content) return '';
      return marked.parse(content);
    },
    handleQuickSend(text) {
      this.userInput = text;
      this.handleSend();
    },
    async handleSend() {
      const input = this.userInput.trim()
      if (!input) {
        this.$message.warning('请输入内容')
        return
      }
      if (this.isSending) return
      this.$store.commit("llm/pushUserInput", input);
      this.scrollToBottom()

      this.userInput = ''
      this.$store.commit("llm/pushmsg", { data: "", isthink: true });
      this.scrollToBottom()

      await postMessage(input, this.thinkMode);
    },
    scrollToBottom() {
      this.$nextTick(() => {
        const dom = this.$refs.messageBox
        dom.scrollTop = dom.scrollHeight
      })
    }
  }
}
</script>
<style scoped>
.ai-chat-container {
  width: 100%;
  height: 100vh;
  background: #fff;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* 顶部标题栏 */
.chat-header {
  height: 5px;
  border-bottom: 1px solid #eee;
  flex-shrink: 0;
}

.chat-message-wrapper {
  flex: 1;
  position: relative;
  overflow: hidden;
}

.chat-message-box {
  width: 100%;
  height: 100%;
  padding: 20px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  align-items: stretch;
  padding-bottom: 30px;
  box-sizing: border-box;
}


.scroll-fade-mask {
  position: absolute;
  left: 0;
  right: 0;
  bottom: 0;
  height: 50px;
  background: linear-gradient(to top, white, rgba(255, 255, 255, 0));
  z-index: 10;
  pointer-events: none;
}

.scroll-bottom-btn {
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
  bottom: 60px;
  width: 36px;
  height: 36px;
  background: #ffffff;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.18);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  z-index: 11;
  transition: opacity .1s ease;
}
.scroll-icon{
  color: #333;
  font-size: 18px;
  padding: 3px;
  stroke: currentColor;
  stroke-width: 1.8;
}


/* 空状态居中 */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100%;
  text-align: center;
  width: 100%;
}

.empty-title {
  font-size: 28px;
  color: #333;
  margin-bottom: 30px;
}

/* 快捷提问按钮组 */
.quick-questions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  justify-content: center;
  max-width: 900px;
  margin: 0 auto;
}

.quick-btn {
  padding: 8px 16px;
  background: #f5f7fa;
  border: 1px solid #e4e7ed;
  border-radius: 20px;
  font-size: 14px;
  color: #606266;
  cursor: pointer;
  transition: all 0.2s;
}

.quick-btn:hover {
  background: #ecf5ff;
  border-color: #409eff;
  color: #409eff;
}

/* 消息项 */
.message-item {
  display: flex;
  align-items: flex-start;
  width: 100%;
  margin-bottom: 20px;
}

/* 用户消息：右对齐 */
.message-item.user {
  flex-direction: row-reverse;
}

/* 头像 */
.avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: #409eff;
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  margin: 0 10px;
  flex-shrink: 0;
}

.message-item.user .avatar {
  background: #67c23a;
}

.markdown-body>>>table {
  word-break: keep-all;
}

/* 消息内容 */
.message-content {
  max-width: 70%;
  padding: 10px 14px;
  border-radius: 6px;
  background: #fff;
  line-height: 1.5;
  word-break: break-all;
  box-shadow: 0 0 6px rgba(0, 0, 0, 0.12);
}

.message-item.user .message-content {
  background: #409eff;
  color: #fff;
}

.ai-content-wrap {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.think-block {
  width: 100%;
}

.think-block details {
  background: #f6f8fb;
  border-radius: 6px;
  border: 1px solid #e5e7eb;
  overflow: hidden;
}

.think-block summary {
  padding: 8px 12px;
  cursor: pointer;
  font-size: 13px;
  color: #6b7280;
  background: #f0f4f9;
  user-select: none;
}

.think-block summary:hover {
  background: #e6edf7;
}

.think-body {
  padding: 10px 12px;
  font-size: 13px;
  color: #4b5563;
  border-left: 3px solid #94a3b8;
}

.answer-body {
  width: 100%;
}


.loading-pulse-dots {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 4px 0;
}

.loading-pulse-dots .dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #c0c4cc;
  animation: pulseDot 0.9s ease-in-out infinite;
}

.loading-pulse-dots .dot:nth-child(1) {
  animation-delay: 0s;
}

.loading-pulse-dots .dot:nth-child(2) {
  animation-delay: 0.15s;
}

.loading-pulse-dots .dot:nth-child(3) {
  animation-delay: 0.3s;
}

@keyframes pulseDot {

  0%,
  100% {
    transform: scale(0.7);
    opacity: 0.5;
  }

  50% {
    transform: scale(1.1);
    opacity: 1;
  }
}


/* 底部输入框 固定底部 */
.chat-input-box {
  background: #fff;
  border-top: 1px solid #eee;
  border-radius: 15px;
  padding: 15px 20px;
  width: 100%;
  max-width: 800px;
  box-sizing: border-box;
  margin: 0 auto;
  box-shadow: 0 -4px 12px rgba(0, 0, 0, 0.08);
  flex-shrink: 0;
  position: relative;
  z-index: 20;
}

/* 输入框 + 发送按钮 */
.input-wrapper {
  display: flex;
  align-items: center;
  background: #fff;
  border: 1px solid #E5E7EB;
  border-radius: 24px;
  padding: 8px 16px;
}

.input-wrapper:focus-within {
  border-color: #409eff;
}

.chat-input {
  flex: 1;
  background: transparent;
  margin-right: 10px;
}

.chat-input>>>.el-textarea__inner {
  background: transparent;
  border: none;
  box-shadow: none;
  resize: none;
  font-size: 14px;
}

.send-btn {
  border-radius: 50%;
  width: 45px;
  height: 45px;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* 底部工具栏 */
.toolbar {
  display: flex;
  justify-content: center;
  margin-top: 8px;
}

.toolbar-left {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-wrap: wrap;
}

.tool-btn {
  font-size: 14px;
  color: #333;
  padding: 4px 10px;
  cursor: pointer;
  line-height: 1.3;
  user-select: none;
  transition: all 0.15s ease;
}

.tool-btn:hover {
  color: #409eff;
}

.tool-btn:active {
  transform: scale(0.92);
  color: #3388ff;
}

.split {
  color: #e0e3e9;
  font-size: 12px;
  user-select: none;
}



.mode-select-wrap {
  position: relative;
}

/* 底部当前模式按钮 */
.current-mode-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 12px;
  background: #f3f4f6;
  border-radius: 16px;
  font-size: 14px;
  cursor: pointer;
  transition: background 0.2s;
}

.current-mode-btn:hover:not(.disabled) {
  background: #e5e7eb;
}

.current-mode-btn.disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.mode-icon {
  font-size: 16px;
}

/* 下拉弹窗 */
.mode-popover {
  position: absolute;
  bottom: calc(100% + 8px);
  right: 0;
  width: 280px;
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  padding: 8px 0;
  z-index: 99;
}

/* 下拉选项 */
.mode-option {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 10px 16px;
  cursor: pointer;
  transition: background 0.18s;
}

.mode-option:hover {
  background: #f3f4f6;
}

.mode-option.active {
  background: #f0f4f9;
}

.option-left {
  display: flex;
  align-items: flex-start;
  gap: 12px;
}

.opt-icon {
  font-size: 20px;
  margin-top: 2px;
}

.opt-text {
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.opt-title {
  font-size: 15px;
  color: #111;
  display: flex;
  align-items: center;
  gap: 8px;
}

.opt-tag {
  font-size: 11px;
  padding: 2px 6px;
  background: #e5e7eb;
  border-radius: 6px;
  color: #555;
}

.opt-desc {
  font-size: 13px;
  color: #999;
}

.check-mark {
  font-size: 16px;
  color: #222;
  font-weight: 500;
}
</style>