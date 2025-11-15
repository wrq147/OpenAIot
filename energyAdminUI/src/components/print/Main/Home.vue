<!--
* @description 主页
* @filename Home.vue
* @author ROYIANS
* @date 2022/9/29 9:23
!-->
<template>
  <roy-container id="roy-print-template-designer" class="roy-designer-container" theme="day">
    <roy-header class="roy-designer-header" height="40px">
      <div id="tttt" class="roy-designer-header__text">
        <div @click="exit" class="roy-designer-back"><i class="ri-arrow-left-line"></i></div>
        <span>打印模板设计器 | {{ pageConfig.title }}</span>
      </div>
      <div class="roy-designer__right">
        <div style="margin-right: 25px;">
          <slot name="roy-designer-header-slot"></slot>
        </div>
        <div class="roy-night-mode">
          <i
            v-for="(tool, index) in headIconConfig"
            :key="index"
            :class="tool.icon"
            :title="tool.name"
            @click="tool.event"
          ></i>
          <i
            v-if="configIn.toolbarConfig.showNightMode && isNightMode"
            class="ri-haze-fill"
            title="切换到日间模式"
            @click="dayNightChange"
          ></i>
          <i
            v-if="configIn.toolbarConfig.showNightMode && !isNightMode"
            class="ri-moon-foggy-fill"
            title="切换到夜间模式"
            @click="dayNightChange"
          ></i>
        </div>
      </div>
    </roy-header>
    <roy-container style="height: calc(100% - 40px)">
      <roy-aside class="roy-designer-aside" width="auto">
        <DesignerAside :show-right.sync="defaultExpendAside" />
      </roy-aside>
      <roy-main class="roy-designer-main">
        <DesignerMain :show-right="defaultExpendAside">
          <template v-slot:roy-designer-toolbar-slot>
            <slot name="roy-designer-toolbar-slot"></slot>
          </template>
        </DesignerMain>
      </roy-main>
    </roy-container>
  </roy-container>
</template>

<script>
import config from '../../../../package.json'
import { mapActions, mapState } from 'vuex'
import DesignerAside from './DesignerAside.vue'
import DesignerMain from './DesignerMain.vue'
import shepherd from '@/components/print/RoyUserTour/userTour'
import commonMixin from '@/mixins/print/commonMixin'
import { renderers } from '@/components/print/config/renderers'

const VERSION = config.version

/**
 * 主页
 */
export default {
  name: 'RoyPrintDesigner',
  components: {
    DesignerAside,
    DesignerMain
  },
  mixins: [commonMixin],
  props: {
    preComponentData: {
      type: [Array, Boolean],
      default: false
    },
    prePageConfig: {
      type: [Object, Boolean],
      default: false
    },
    preDataSource: {
      type: [Array, Boolean],
      default: false
    },
    preDataSet: {
      type: [Object, Boolean],
      default: false
    },
    config: {
      type: Object,
      default: () => {
        return {}
      }
    }
  },
  data() {
    return {
      defaultExpendAside: true,
      configIn: {
        toolbarConfig: {
          buttons: ['guide', 'saveTemplate'],
          showNightMode: true
        }
      },
      headIcons: [
        {
          code: 'guide',
          name: '界面指引',
          icon: 'ri-question-line',
          event: this.showUserGuide
        },
        {
          code: 'saveTemplate',
          name: '保存模板',
          icon: 'ri-save-line',
          event: this.saveTemplate
        }
      ]
    }
  },
  computed: {
    ...mapState({
      isNightMode: (state) => state.printTemplateModule.nightMode.isNightMode,
      pageConfig: (state) => state.printTemplateModule.pageConfig,
      componentData: (state) => state.printTemplateModule.componentData,
      dataSource: (state) => state.printTemplateModule.dataSource
    }),
    headIconConfig() {
      return this.headIcons.filter((item) => {
        return (
          this.configIn.toolbarConfig.buttons &&
          this.configIn.toolbarConfig.buttons.includes(item.code)
        )
      })
    }
  },
  methods: {
    ...mapActions({
      initNightMode: 'printTemplateModule/nightMode/initNightMode',
      toggleNightMode: 'printTemplateModule/nightMode/toggleNightMode'
    }),
    async initMounted() {
      this.initConfig()
      await this.registerTableRender(renderers)
    },
    registerTableRender(renderers) {
      // 注册渲染器
      for (let i in renderers) {
        this.$VXETable.renderer.add(i, renderers[i])
      }
    },
    initConfig() {
      this.initNightMode()
      this.configIn = Object.assign({}, this.configIn, this.config)
      if (this.preComponentData) {
        this.$store.commit('printTemplateModule/setComponentData', this.preComponentData)
      }
      if (this.prePageConfig) {
        this.$store.commit('printTemplateModule/setPageConfig', this.prePageConfig)
      }
      if (this.preDataSource) {
        this.$store.commit('printTemplateModule/setDataSource', this.preDataSource)
      }
      if (this.preDataSet) {
        this.$store.commit('printTemplateModule/setDataSet', this.preDataSet)
      }
    },
    dayNightChange() {
      this.toggleNightMode(!this.isNightMode)
    },
    showUserGuide() {
      const driver = shepherd()
      driver.addSteps([
        {
          attachTo: {
            element: document.querySelector('#royians-guide'),
            on: 'auto'
          },
          title: '打印模板设计器界面指引',
          text: '欢迎使用！接下来介绍整个界面。',
          buttons: [
            {
              action() {
                return this.next()
              },
              text: '下一步'
            }
          ]
        },
        {
          attachTo: {
            element: this.$el.querySelector('.roy-designer-header'),
            on: 'auto'
          },
          title: '打印模板设计器界面指引',
          text: '这是标题栏，左侧显示”打印模板设计器”字样和当前模板名称，右侧是开发者自定义插槽和夜间模式切换按钮。',
          buttons: [
            {
              action() {
                return this.next()
              },
              text: '下一步'
            }
          ]
        },
        {
          attachTo: {
            element: this.$el.querySelector('.roy-designer-aside'),
            on: 'auto'
          },
          title: '打印模板设计器界面指引',
          text: '这是左侧面板，包含四个模块。',
          buttons: [
            {
              action() {
                return this.next()
              },
              text: '下一步'
            }
          ]
        },
        {
          attachTo: {
            element: this.$el.querySelector('.roy-designer-main'),
            on: 'bottom-start'
          },
          title: '打印模板设计器界面指引',
          text: '这是右侧面版，包含一个工具栏和一个主窗口。',
          buttons: [
            {
              action() {
                return this.next()
              },
              text: '下一步'
            }
          ]
        },
        {
          attachTo: {
            element: document.querySelector('#royians-guide'),
            on: 'auto'
          },
          title: '打印模板设计器界面指引',
          text: '欢迎使用打印模板设计器。',
          buttons: [
            {
              action() {
                return this.cancel()
              },
              text: '完成'
            }
          ]
        }
      ])
      driver.start()
    },
   
    saveTemplate() {
      this.$emit("save",{
        pageConfig: this.pageConfig,
        componentData: this.componentData
      });
    },
    loadTemplateData(resultParsed) {
      this.$store.commit('printTemplateModule/setComponentData', resultParsed.componentData)
      this.$store.commit('printTemplateModule/setPageConfig', resultParsed.pageConfig)
      this.$store.commit('printTemplateModule/setDataSource', resultParsed.dataSource)
    },
    loadTemplate(pageConfig,componentData){
      this.$store.commit('printTemplateModule/setComponentData', componentData)
      this.$store.commit('printTemplateModule/setPageConfig', pageConfig)
    },
    loadDataSource(tmpDataSource,tmpDataSet) {
      this.$store.commit('printTemplateModule/setDataSource', tmpDataSource)
      this.$store.commit('printTemplateModule/setDataSet', tmpDataSet)
    },
    getTemplateData() {
      return {
        type: 'rptd',
        pageConfig: this.pageConfig,
        componentData: this.componentData,
        dataSource: this.dataSource
      }
    },
    exit() {
      this.$emit("exit");
    },
  },
  created() {},
  mounted() {
    this.initMounted()
  },
  watch: {}
}
</script>

<style lang="scss" scoped>
.roy-designer-container {
  background: var(--prism-background);
  width: 100%;
  height: 100%;

  .roy-designer-header {
    background: var(--roy-menu-bar-background);
    width: 100%;
    display: flex;
    justify-content: space-between;

    .roy-designer-back{
      border-radius: 50%;padding: 5px;margin-right:10px;font-size: 16px;cursor: pointer;
    }
    .roy-designer-header__text {
      color: #fff;
      display: flex;
      height: 100%;
      align-items: center;
    }

    .roy-designer__right {
      float: right;
      color: #fff;
      width: 50%;
      display: flex;
      align-items: flex-end;
      justify-content: flex-end;
      height: 100%;
      line-height: 40px;
    }

    .roy-night-mode {
      color: #fff;
      line-height: 40px;
      height: 40px;
      overflow: hidden;
      cursor: pointer;

      i {
        float: left;
        color: #fff;
        display: flex;
        align-items: flex-end;
        justify-content: flex-end;
        height: 100%;
        line-height: 40px;
        padding: 0 8px;
        cursor: pointer;

        &:hover {
          background: var(--roy-color-primary-light-3);
        }
      }
    }
  }

  .roy-designer-aside {
    margin: 10px 5px 10px 10px;
    height: calc(100% - 20px);
  }

  .roy-designer-main {
    margin: 5px;
    border-radius: 2px;
    overflow: auto;
    padding: 0;
  }
}
</style>
