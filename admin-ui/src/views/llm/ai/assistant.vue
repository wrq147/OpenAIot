<template>
  <div class="ai-chat-container">
    <!-- 对话标题栏 -->
    <div class="chat-header">
      <span>AI 智能助手</span>
    </div>

    <!-- 消息列表区域 -->
    <div class="chat-message-box" ref="messageBox">
      <!-- 空状态 -->
      <div v-if="messageList.length === 0" class="empty-tip">
        有什么可以帮你的吗？
      </div>

      <!-- 用户消息 -->
      <div v-for="(item, index) in ChatMessageList" :key="index" class="message-item"
        :class="{ user: item.role === 'user', ai: item.role === 'assistant' }">
        <div class="avatar">
          <i v-if="item.type === 'user'" class="el-icon-user"></i>
          <i v-else class="el-icon-chat-dot-round"></i>
        </div>
        <div class="message-content">
          {{ item.data }}
          <!-- AI 回复加载中动画 -->
          <span v-if="item.status==1" class="loading-dot">...</span>
        </div>
      </div>
    </div>

    <!-- 输入框区域 -->
    <div class="chat-input-box">
      <el-input v-model="userInput" type="textarea" :rows="3" placeholder="请输入你的问题..." @keyup.enter.native="handleSend"
        :disabled="loading"></el-input>
      <div class="btn-box">
        <el-button type="primary" @click="handleSend" :loading="loading">
          发送
        </el-button>
      </div>
    </div>
  </div>
</template>

<script>
import { postMessage } from "@/api/llmchat";

export default {
  name: 'AiChat',
  data() {
    return {
      // 用户输入内容
      userInput: '',
      // 加载状态
      loading: false,
    }
  },
  created() {

  },
  beforeDestroy() {
  },
	computed: {
    //消息列表
		ChatMessageList: function () {
			return this.$store.state.llm.messageList;
		}
	},
  methods: {
    // 发送用户提问
    async handleSend() {
      const input = this.userInput.trim()
      if (!input) {
        this.$message.warning('请输入内容')
        return
      }
      if (this.loading) return
      // 1. 添加用户消息到列表
      this.$store.commit("llm/pushUserInput", input);
      this.scrollToBottom()

      // 2. 清空输入框 + 开启加载
      this.userInput = ''
      this.loading = true

      // 3. 添加AI加载中消息
      this.$store.commit("llm/pushmsg", "");
      this.scrollToBottom()

      await postMessage(input);
      this.loading = false
    },
    // 滚动到底部
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
  height: calc(100vh - 80px);
  background: #f5f7fa;
  display: flex;
  flex-direction: column;
  border-radius: 4px;
  overflow: hidden;
}

/* 标题栏 - 升级美化版 */
.chat-header {
  height: 64px;
  /* 渐变背景，更高级 */
  background: linear-gradient(135deg, #409eff, #69b1ff);
  line-height: 64px;
  padding: 0 24px;
  font-size: 18px;
  font-weight: 600;
  color: #ffffff;
  /* 柔和阴影 */
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  /* 居中+美观 */
  display: flex;
  align-items: center;
  border-bottom: none;
  position: relative;
  z-index: 10;
}

/* 消息列表 */
.chat-message-box {
  flex: 1;
  padding: 20px;
  overflow-y: auto;
}

.empty-tip {
  text-align: center;
  color: #909399;
  padding: 40px 0;
}

/* 消息项 */
.message-item {
  display: flex;
  margin-bottom: 20px;
  align-items: flex-start;
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
}

.message-item.user .avatar {
  background: #67c23a;
}

/* 消息内容 */
.message-content {
  max-width: 70%;
  padding: 10px 14px;
  border-radius: 6px;
  background: #fff;
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

/* 输入框 */
.chat-input-box {
  background: #fff;
  padding: 15px 20px;
  border-top: 1px solid #eee;
}

.btn-box {
  text-align: right;
  margin-top: 10px;
}
</style>