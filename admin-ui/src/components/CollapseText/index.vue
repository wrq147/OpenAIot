<template>
  <div class="collapse-text-wrapper">
    <!-- 用于计算真实高度的隐藏容器 -->
    <div 
      ref="measureContainer"
      class="collapse-text-measure"
      :style="{
        fontSize: '13px',
        lineHeight: lineHeight + 'px',
        width: '100%',
        position: 'absolute',
        visibility: 'hidden',
        whiteSpace: 'normal',
        wordBreak: 'break-all'
      }"
    >
      {{ text }}
    </div>
    
    <!-- 实际显示的容器 -->
    <div 
      ref="displayContainer"
      class="collapse-text-container"
      :class="{ 'collapsed': isCollapsed && isOverflow }"
      :style="{
        fontSize: '13px',
        lineHeight: lineHeight + 'px',
        maxHeight: isCollapsed && isOverflow ? (maxLines * lineHeight) + 'px' : 'none',
        position: 'relative'
      }"
    >
      <span class="text-content">{{ text }}</span>
      
      <!-- 更多/收起按钮 - 修复显示问题 -->
      <span 
        v-if="isOverflow" 
        class="toggle-btn" 
        @click="toggleCollapse"
        :style="{
          position: isCollapsed ? 'absolute' : 'static',
          bottom: isCollapsed ? '0' : 'auto',
          right: isCollapsed ? '0' : 'auto',
          backgroundColor: '#fff',
          paddingLeft: '8px',
          marginTop: isCollapsed ? '0' : '4px'
        }"
      >
        {{ isCollapsed ? '更多' : '收起' }}
      </span>
    </div>
  </div>
</template>

<script>
export default {
  name: 'CollapseText',
  props: {
    // 需要显示的文本内容
    text: {
      type: String,
      default: ''
    },
    // 最大显示行数
    maxLines: {
      type: Number,
      default: 3
    },
    // 行高（px）
    lineHeight: {
      type: Number,
      default: 18
    }
  },
  data() {
    return {
      isCollapsed: true,  // 是否折叠
      isOverflow: false   // 是否溢出
    };
  },
  watch: {
    text: {
      immediate: true,
      handler() {
        this.$nextTick(() => {
          this.checkOverflow();
        });
      }
    },
    maxLines: {
      immediate: true,
      handler() {
        this.checkOverflow();
      }
    }
  },
  mounted() {
    // 确保DOM渲染完成后检测溢出
    this.$nextTick(() => {
      this.checkOverflow();
    });
  },
  methods: {
    /**
     * 精准检测文本是否溢出指定行数
     */
    checkOverflow() {
      if (!this.text || this.text.trim() === '') {
        this.isOverflow = false;
        return;
      }

      const measureContainer = this.$refs.measureContainer;
      const displayContainer = this.$refs.displayContainer;
      
      if (!measureContainer || !displayContainer) {
        setTimeout(() => this.checkOverflow(), 100); // 延迟重试
        return;
      }

      // 计算容器宽度（去除padding/margin）
      const containerWidth = displayContainer.offsetWidth;
      measureContainer.style.width = containerWidth + 'px';

      // 计算实际高度和最大允许高度
      const actualHeight = measureContainer.offsetHeight;
      const maxAllowedHeight = this.maxLines * this.lineHeight;

      // 判断是否溢出
      this.isOverflow = actualHeight > maxAllowedHeight;
      
      // 调试用：可以打开查看计算结果
      // console.log('文本溢出检测:', {
      //   text: this.text,
      //   actualHeight,
      //   maxAllowedHeight,
      //   isOverflow: this.isOverflow
      // });
    },
    
    /**
     * 切换折叠/展开状态
     */
    toggleCollapse() {
      this.isCollapsed = !this.isCollapsed;
      this.$emit('toggle', this.isCollapsed);
      
      // 切换后强制重绘
      this.$nextTick(() => {
        this.checkOverflow();
      });
    }
  }
}
</script>

<style scoped>
.collapse-text-wrapper {
  position: relative;
  width: 100%;
  font-size: 13px;
  color: #666;
}

.collapse-text-container {
  width: 100%;
  overflow: hidden;
  word-break: break-all;
  white-space: normal;
}

/* 折叠状态样式 - 改用JS控制高度，兼容更多浏览器 */
.collapse-text-container.collapsed {
  display: block;
  overflow: hidden;
  position: relative;
}

.text-content {
  display: inline;
}

/* 修复按钮显示问题 */
.toggle-btn {
  color: #409eff;
  cursor: pointer;
  font-size: 12px;
  user-select: none;
  display: inline-block;
}

/* 隐藏的测量容器 */
.collapse-text-measure {
  z-index: -1;
  height: auto;
}
</style>