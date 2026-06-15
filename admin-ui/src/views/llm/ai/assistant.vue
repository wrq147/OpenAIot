<template>
  <div class="ai-chat-container">
    <!-- 顶部标题栏 -->
    <div class="chat-header"></div>

    <!-- 消息列表区域 -->
    <div class="chat-message-wrapper">
      <div class="chat-message-box" ref="messageBox">
        <!-- 空状态：居中显示标题 + 快捷提问按钮 -->
        <div v-if="ChatMessageList.length === 0" class="empty-state">
          <h1 class="empty-title">有什么我能帮你的吗？</h1>
          <div class="quick-questions">
            <span class="quick-btn" @click="handleQuickSend('你能干什么？')">你能干什么？</span>
            <span class="quick-btn" @click="handleQuickSend('什么食物可以帮助缓解拉肚子？')">什么食物可以帮助缓解拉肚子？</span>
            <span class="quick-btn" @click="handleQuickSend('什么是 AGI 时代？')">什么是 AGI 时代？</span>
            <span class="quick-btn" @click="handleQuickSend('解释L0、L1、L2范数的区别')">解释L0、L1、L2范数的区别</span>
            <span class="quick-btn" @click="handleQuickSend('资讯：Meta 智能眼镜人脸识别系统曝光后被紧急移除')">资讯：Meta
              智能眼镜人脸识别系统曝光后被紧急移除</span>
            <span class="quick-btn"
              @click="handleQuickSend('Transformer如何利用多头注意力机制进行学习？')">Transformer如何利用多头注意力机制进行学习？</span>
            <span class="quick-btn" @click="handleQuickSend('B850M适合搭建什么级别的主机？')">B850M适合搭建什么级别的主机？</span>
            <span class="quick-btn" @click="handleQuickSend('AGI 和 AI 有什么区别？')">AGI 和 AI 有什么区别？</span>
            <span class="quick-btn" @click="handleQuickSend('资讯：新一代智能体 OpenClaw 推动 AI 原生组织深度变革')">资讯：新一代智能体 OpenClaw 推动
              AI
              原生组织深度变革</span>
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
            <div v-else v-html="renderMarkdown(item.data)" class="md-content"></div>
            <!-- AI 回复加载中动画 -->
            <span v-if="item.status === 1" class="loading-dot">...</span>
          </div>
        </div>
      </div>
      <div class="scroll-fade-mask"></div>
    </div>


    <div class="chat-input-box">
      <div class="input-wrapper">
        <el-input v-model="userInput" type="textarea" :autosize="{ minRows: 1, maxRows: 10}" placeholder="请输入你的问题..." @keyup.enter.native="handleSend"
          :disabled="loading" class="chat-input" />
        <el-button type="primary" class="send-btn" @click="handleSend" :loading="loading"><span
            v-if="!loading">发送</span></el-button>
      </div>
      <!-- 底部工具栏 -->
      <div class="toolbar">
        <div class="toolbar-left">
          <div class="tool-btn"><i class="el-icon-plus"></i></div>
          <span class="split">|</span>
          <div class="tool-btn">快速</div>
          <span class="split">|</span>
          <div class="tool-btn">PPT 生成</div>
          <span class="split">|</span>
          <div class="tool-btn">图像生成</div>
          <span class="split">|</span>
          <div class="tool-btn">更多</div>
        </div>
      </div>
    </div>

  </div>
</template>

<script>
import { postMessage } from "@/api/llmchat";
import marked from 'marked'
import hljs from 'highlight.js'
import 'highlight.js/styles/github.css'
marked.setOptions({
  highlight: (code) => hljs.highlightAuto(code).value,
  gfm: true,
  breaks: true,
});

export default {
  name: 'AiChat',
  data() {
    return {
      userInput: '',
      loading: false,
    }
  },
  computed: {
    ChatMessageList: function () {
      return this.$store.state.llm.messageList;
    }
  },
  methods: {
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
      if (this.loading) return
      this.$store.commit("llm/pushUserInput", input);
      this.scrollToBottom()

      this.userInput = ''
      this.loading = true

      this.$store.commit("llm/pushmsg", "");
      this.scrollToBottom()

      await postMessage(input);
      this.loading = false
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
  background: #ffffff;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* 顶部标题栏 */
.chat-header {
  height: 5px;
  background: #fff;
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

/* 消息内容 */
.message-content {
  max-width: 70%;
  padding: 10px 14px;
  border-radius: 6px;
  background: #f5f7fa;
  line-height: 1.5;
  word-break: break-all;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.message-item.user .message-content {
  background: #409eff;
  color: #fff;
}

/* 加载动画 */
.loading-dot {
  color: #909399;
  animation: dot 1s infinite step-start;
}

@keyframes dot {

  0%,
  100% {
    content: '';
  }

  33% {
    content: '.';
  }

  66% {
    content: '..';
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
</style>