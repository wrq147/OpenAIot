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
      <div
        v-for="(item, index) in messageList"
        :key="index"
        class="message-item"
        :class="{ user: item.type === 'user', ai: item.type === 'ai' }"
      >
        <div class="avatar">
          <i v-if="item.type === 'user'" class="el-icon-user"></i>
          <i v-else class="el-icon-chat-dot-round"></i>
        </div>
        <div class="message-content">
          {{ item.content }}
          <!-- AI 回复加载中动画 -->
          <span v-if="item.loading" class="loading-dot">...</span>
        </div>
      </div>
    </div>

    <!-- 输入框区域 -->
    <div class="chat-input-box">
      <el-input
        v-model="userInput"
        type="textarea"
        :rows="3"
        placeholder="请输入你的问题..."
        @keyup.enter.native="handleSend"
        :disabled="loading"
      ></el-input>
      <div class="btn-box">
        <el-button type="primary" @click="handleSend" :loading="loading">
          发送
        </el-button>
      </div>
    </div>
  </div>
</template>

<script>

export default {
  name: 'AiChat',
  data() {
    return {
      // 用户输入内容
      userInput: '',
      // 消息列表
      messageList: [],
      // 加载状态
      loading: false,
      // MQTT客户端实例
      mqttClient: null,
      // 会话ID（唯一标识当前对话）
      sessionId: '',
      // MQTT连接配置（根据你的实际MQTT服务修改）
      mqttOptions: {
        protocol: 'ws', // websocket协议
        host: 'localhost', // MQTT服务器地址
        port: 8083, // MQTT ws端口
        username: '', // 用户名（如有）
        password: '', // 密码（如有）
        clientId: 'ai_chat_' + Math.random().toString(16).substr(2, 8)
      }
    }
  },
  created() {
    // 生成会话ID
    this.sessionId = this.generateSessionId()
    // 初始化MQTT连接
    this.initMqtt()
  },
  beforeDestroy() {
    // 页面销毁时断开MQTT连接
    if (this.mqttClient) {
      this.mqttClient.end()
    }
  },
  methods: {
    // 生成随机会话ID
    generateSessionId() {
      return Date.now().toString(16) + Math.random().toString(16).substr(2)
    },
    // 初始化MQTT连接
    initMqtt() {
      const { protocol, host, port, ...options } = this.mqttOptions
      const connectUrl = `${protocol}://${host}:${port}/mqtt`
      
      // 创建客户端
      this.mqttClient = mqtt.connect(connectUrl, options)
      
      // 连接成功
      this.mqttClient.on('connect', () => {
        console.log('MQTT连接成功')
        // 订阅AI回复主题：llmchat/会话ID
        const topic = `llmchat/${this.sessionId}`
        this.mqttClient.subscribe(topic, (err) => {
          if (!err) {
            console.log('订阅主题成功：', topic)
          }
        })
      })

      // 监听消息接收（服务端流式回复）
      this.mqttClient.on('message', (topic, message) => {
        const content = message.toString()
        this.handleAiStreamResponse(content)
      })

      // 连接失败
      this.mqttClient.on('error', (err) => {
        console.error('MQTT连接失败：', err)
        this.$message.error('消息连接失败，请刷新页面')
      })
    },
    // 发送用户提问
    async handleSend() {
      const input = this.userInput.trim()
      if (!input) {
        this.$message.warning('请输入内容')
        return
      }
      if (this.loading) return

      // 1. 添加用户消息到列表
      this.messageList.push({
        type: 'user',
        content: input
      })
      this.scrollToBottom()

      // 2. 清空输入框 + 开启加载
      this.userInput = ''
      this.loading = true

      // 3. 添加AI加载中消息
      this.messageList.push({
        type: 'ai',
        content: '',
        loading: true
      })
      this.scrollToBottom()

      try {
        // 4. 调用后端接口发送提问
        await axios({
          url: '/LLMService/Chat/Message',
          method: 'post',
          data: {
            userInput: input,
            sessionId: this.sessionId // 携带会话ID，后端用于匹配MQTT推送
          }
        })
      } catch (error) {
        console.error('发送失败：', error)
        this.$message.error('发送失败，请重试')
        // 移除加载中消息
        this.messageList.pop()
        this.loading = false
      }
    },
    // 处理AI流式回复
    handleAiStreamResponse(content) {
      if (!content) return
      
      // 关闭加载状态
      this.loading = false
      
      // 获取最后一条AI消息
      const lastMsg = this.messageList[this.messageList.length - 1]
      if (lastMsg && lastMsg.type === 'ai') {
        // 流式追加内容
        lastMsg.content += content
        lastMsg.loading = false
      }
      this.scrollToBottom()
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

/* 标题栏 */
.chat-header {
  height: 60px;
  background: #fff;
  line-height: 60px;
  padding: 0 20px;
  font-size: 16px;
  font-weight: bold;
  border-bottom: 1px solid #eee;
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
  0%, 100% { content: ''; }
  33% { content: '.'; }
  66% { content: '..'; }
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