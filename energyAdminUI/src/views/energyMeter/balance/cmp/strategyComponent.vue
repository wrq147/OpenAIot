<template>
  <div class="content_ai">
    <div class="content_con">
        <div class="content_text" ref="contentTextContainer" v-show="AIShow">
            <div class="title">用能策略推荐</div>
            <div class="text" v-html="currentContent" ref="contentContainer"></div>
            <div class="hide" @click.stop="setHide">隐藏</div>
        </div>
        <div class="ai_img">
            <img class="img" v-show="AIShow" src="@/assets/images/strategy_ai.png" alt="">
            <img class="img" v-show="!AIShow" src="@/assets/images/ai_static.png" alt="" @click.stop="setShow">
        </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'EnergyAdminUIStrategyComponent',
  props:{
    originalHtml:{
      type:String,
      default:''
    }
  },
  data() {
    return {
      AIShow:true,
      // 状态管理
      // 打字状态管理
      currentContent: "", // 当前已显示的内容
      segments: [], // 拆分后的HTML片段（标签/文本）
      segmentIndex: 0, // 当前处理的片段索引
      charIndex: 0, // 当前文本片段的字符索引
      isTyping: false, // 是否正在打字
      isPaused: false, // 是否暂停
      isComplete: false, // 是否完成
      typeTimer: null, // 打字定时器
      speed: 100, // 打字速度（毫秒/字符）
    //   originalHtml:'',
    };
  },
  watch:{
    originalHtml:{
      handler(newval){
        
        if(newval){
          this.$nextTick(()=>{
            this.segments = this.splitHtmlIntoSegments(newval);
            this.startTyping()
          })
        }
      },
      immediate:true
    },
    
  },
  mounted() {
    
  },
  beforeDestroy() {
    // 清理定时器，防止内存泄漏
    clearInterval(this.typeTimer);
  },
  methods: {
    setHide(){//设置ai隐藏
        this.AIShow=false
    },
    setShow(){//设置ai显示
        this.AIShow=true
    },
    /**
     * 拆分 HTML 为片段：标签段（如 <div>）和文本段（如 "Hello"）
     * 正则匹配规则：<...> 视为标签段，其他视为文本段
     */
    splitHtmlIntoSegments(html) {
      const segments = [];
      const regex = /(<\/?[^>]+>)/g; // 匹配 HTML 标签（包括闭合标签）
      let lastIndex = 0;

      html.replace(regex, (match, tag, index) => {
        // 提取标签前的文本段
        if (index > lastIndex) {
          segments.push({
            type: 'text',
            content: html.slice(lastIndex, index)
          });
        }
        // 提取标签段
        segments.push({
          type: 'tag',
          content: tag
        });
        lastIndex = index + tag.length;
      });

      // 提取剩余的文本段
      if (lastIndex < html.length) {
        segments.push({
          type: 'text',
          content: html.slice(lastIndex)
        });
      }

      return segments;
    },

    /**
     * 自动滚动到底部
     * 确保最新内容在可视区域内
     */
    autoScrollToBottom() {
      const container = this.$refs.contentContainer;
      const contentTextContainer=this.$refs.contentTextContainer
      if (!container) return;

      // 等待DOM更新后再计算滚动位置（关键）
      this.$nextTick(() => {
        // 内容总高度 > 容器可视高度时才滚动
        if (container.scrollHeight > contentTextContainer.clientHeight-56) {
          contentTextContainer.scrollTop = container.scrollHeight; // 滚动到底部
        }
      });
    },

    /** 开始打字 */
    startTyping() {
      if (this.isTyping || this.isComplete) return;

      this.isTyping = true;
      this.isPaused = false;
      this.runTyping();
    },

    /** 核心打字逻辑（含自动滚动） */
    runTyping() {
      this.typeTimer = setInterval(() => {
        // 所有片段处理完毕，结束打字
        if (this.segmentIndex >= this.segments.length) {
          this.isTyping = false;
          this.isComplete = true;
          clearInterval(this.typeTimer);
          this.autoScrollToBottom(); // 最后一次滚动
          return;
        }

        const currentSegment = this.segments[this.segmentIndex];

        if (currentSegment.type === 'tag') {
          // 标签段：整体添加（不拆分标签）
          this.currentContent += currentSegment.content;
          this.segmentIndex++;
          this.autoScrollToBottom(); // 标签添加后检查滚动
        } else {
          // 文本段：逐字添加
          if (this.charIndex < currentSegment.content.length) {
            this.currentContent += currentSegment.content[this.charIndex];
            this.charIndex++;
            // 优化：每3个字符滚动一次（减少性能消耗）
            if (this.charIndex % 3 === 0) {
              this.autoScrollToBottom();
            }
          } else {
            // 当前文本段结束，切换到下一段
            this.segmentIndex++;
            this.charIndex = 0;
            this.autoScrollToBottom(); // 文本段结束后检查滚动
          }
        }
      }, this.speed);
    },

    /** 暂停打字 */
    pauseTyping() {
      if (!this.isTyping || this.isPaused) return;

      this.isPaused = true;
      clearInterval(this.typeTimer);
    },

    /** 继续打字 */
    resumeTyping() {
      if (!this.isPaused) return;

      this.isPaused = false;
      this.runTyping();
    },
  },
};
</script>

<style lang="less" scoped>
.content_ai{
    width: 300px;
    height: 520px;
    position: fixed;
    right: 27px;
    bottom: 10px;
    z-index: 99999;
    .content_con{
        position: relative;
        height: 520px;
        .content_text{
            // position: relative;
            padding: 56px 9px 20px 20px;
            width: 300px;
            height: 390px;
            overflow-y: auto;
            background: linear-gradient( 180deg, #3DB9B1 0%, #3DB98F 100%);
            box-shadow: 0px 0px 20px 0px #005E3E;
            border-radius: 16px 16px 0px 16px;
            font-size: 14px;
            color: rgba(255, 255, 255, 1);
            .strategy_table_con{
              width: 100%;
              border: 1px solid rgba(255, 255, 255, 0.50);
              margin-top: 12px;
              margin-bottom: 12px;
              .strategy_table_ul{
                display: flex;
                width: 100%;
                
                .strategy_table_li{
                  width: calc(100% / 3);
                  border: 1px solid rgba(255, 255, 255, 0.50);
                  padding-bottom: 9px;
                  .table_li_val{
                    text-align: center;
                    margin-top: 9px;
                    line-height: 14px;
                  }
                }
              }
            }
            &::-webkit-scrollbar {
                width: 6px;
                height: 6px;
            }
            /* 修改滚动条轨道 */
            &::-webkit-scrollbar-track {
                background: rgba(61, 185, 177, 0);
            }
            
            /* 修改滚动条滑块 */
            &::-webkit-scrollbar-thumb {
                background: linear-gradient( 180deg, rgba(61, 185, 177, 0) 0%, rgba(61, 185, 143, 0) 100%);
                border-radius: 5px;
            }
            
            /* 修改滑块在鼠标悬浮时的样式 */
            &::-webkit-scrollbar-thumb:hover {
                background: linear-gradient( 180deg, rgba(61, 185, 177, 0) 0%, rgba(61, 185, 143, 0) 100%);
            }
            .title{
                font-size: 16px;
                line-height: 24px;
                padding-bottom: 12px;
                background:rgba(61, 185, 177, 1);
                position: absolute;
                top:0;
                left:0;
                width:100%;
                padding:20px 20px 16px;
                border-radius:16px 16px 0 0;
            }
            .text{
                line-height: 22px;
                p{
                    margin: 0;
                }
            }
            p{
                margin: 0;
            }
            .hide{
                position: absolute;
                color: rgba(255, 255, 255, 0.8);
                right: 16px;
                top: 16px;
                line-height: 14px;
                cursor: pointer;
            }

        }
        .ai_img{
            position: absolute;
            bottom: 0;
            right: 0;
            width: 122px;
            height: 230px;
            .img{
                width: 122px;
                height: 230px;
            }
        }
    }
    
}
</style>