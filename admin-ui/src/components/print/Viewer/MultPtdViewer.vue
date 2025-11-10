<!--
* @description 多个打印模板预览组件
* @filename MultPtdViewer.vue
* @author MZ
* @date 2024/10/11 15:31
!-->
<!--
* @description 打印模板预览组件
* @filename PtdViewer.vue
* @author ROYIANS
* @date 2022/11/11 15:31
!-->
<template>
  <RoyModal
    v-if="visibleIn"
    :show.sync="visibleIn"
    :title="directExport ? '打印导出' : '打印预览'"
    class="rptd-viewer"
    height="90%"
    width="90%"
    @close="closePrint"
  >
    <RoyLoading :loading="!initCompleted" :loading-text="loadingText">
      <div v-if="isBlankPage" class="roy-page-blank">
        <i class="ri-sticky-note-2-line"></i>
        <span>页面内容为空</span>
      </div>
      <div v-else id="roy-viewer" ref="viewer" :class="isExportPDF ? '' : 'is-show-border'" class="roy-viewer"></div>
      <div v-for="(item, index) in listPrintData" :key="'bb' + index" :ref="'tempHolder' + index" class="roy-temp-holder" :class="'roy-temp-holder' + index"></div>
      <div class="roy-viewer-right-conor">
        <div class="roy-viewer-btn" @click="exportPdf">
          <i class="ri-file-ppt-line"></i>
          <span>导出PDF</span>
        </div>
        <div class="roy-viewer-btn" v-print="printConfig">
          <i class="ri-printer-line"></i>
          <span>打印</span>
        </div>
        <div class="roy-viewer-btn" @click="btnClickPrint">
          <i class="ri-printer-line"></i>
          <span>直接打印</span>
        </div>
      </div>
    </RoyLoading>
  </RoyModal>
</template>
  
  <script>
import commonMixin from "@/mixins/print/commonMixin";
import RoyModal from "@/components/print/RoyModal/RoyModal";
import RoyLoading from "@/components/print/RoyLoading";
import html2canvas from "html2canvas";
import { jsPDF } from "jspdf";
import print from "vue-print-nb";
import toast from "../js/toast";
import { AutoRender } from "@/components/print/Viewer/automul-render";
// import { getLodop } from "@/components/print/js/LodopFuncs.js"; //导入模块
/**
 * 打印模板预览组件
 */
export default {
  name: "PtdViewer",
  mixins: [commonMixin],
  components: {
    RoyLoading,
    RoyModal,
  },
  directives: {
    print,
  },
  props: {
    visible: {
      type: Boolean,
      default: true,
    },
    componentData: {
      type: Array,
      default: () => {
        return [];
      },
    },
    listPrintData: {
      type: Array,
      default: () => {
        return [];
      },
    },
    pageConfig: {
      type: Object,
      default: () => {
        return {};
      },
    },
    dataSource: {
      type: Array,
      default: () => {
        return [];
      },
    },
    dataSet: {
      type: Object,
      default: () => {
        return {};
      },
    },
    fileName: {
      type: String,
      default: "",
    },
    directExport: {
      type: Boolean,
      default: false,
    },
    needToast: {
      type: [Boolean, String],
      default: "建议导出PDF后再打印，更精准",
    },
  },
  computed: {
    loadingText() {
      if (this.isExportPDF) {
        return `正在导出PDF，正在处理第${this.curPage}页/共${this.totalPage}页`;
      } else {
        return "正在生成页面...";
      }
    },
  },
  data() {
    return {
      visibleIn: false,
      initCompleted: false,
      isBlankPage: false,
      isExportPDF: false,
      totalPage: 0,
      curPage: 0,
      printConfig: {
        id: "roy-viewer",
      },
    };
  },
  methods: {
    closePrint(){
      this.$emit('closePrint')
    },
    initMounted() {
      this.$nextTick(async () => {
        this.initCompleted = false;
        let renderPages = await this.reloadPageHt();
        if (renderPages.length) {
          const viewerElement = this.$refs.viewer;
          viewerElement.innerHTML = renderPages.join("");
          if (this.needToast) {
            toast(this.needToast, "info", 2000);
          }
        } else {
          this.isBlankPage = true;
        }
        for (let i = 0; i < this.listPrintData.length; i++) {
          this.$el.querySelector(".roy-temp-holder" + i).style.display = "none";
        }

        this.$nextTick(() => {
          if (this.directExport) {
            this.exportPdf();
          } else {
            this.initCompleted = true;
          }
        });
      });
    },
    async reloadPageHt() {
      let allpages = [];

      if (this.listPrintData && this.listPrintData.length > 0) {
        for (let i = 0; i < this.listPrintData.length; i++) {
          let row = this.listPrintData[i];
          const renderer = new AutoRender({
            renderElements: row.componentData,
            pagerConfig: row.globalConfig,
            dataSet: row.dataSet,
            dataSource: row.dataSource,
            tempHolder: this.$refs["tempHolder" + i][0],
          });
          let renderPage = await renderer.run();
          allpages = [...allpages, ...renderPage];
        }
      }
      return allpages;
    },
    exportPdf() {
      this.isExportPDF = true;
      this.initCompleted = false;
      this.$nextTick(async () => {
        const pages = this.$el.getElementsByClassName("roy-preview-page");
        if (!pages.length) {
          return;
        }

        // let doc = new jsPDF({
        //   orientation: this.pageConfig.pageDirection,
        //   format: [this.pageConfig.PaperWidth,this.pageConfig.PaperHeight],
        //   unit: 'mm'
        // })
        let doc = new jsPDF(this.pageConfig.pageDirection, "mm", [
          this.pageConfig.pageWidth,
          this.pageConfig.pageHeight,
        ]);
        this.totalPage = pages.length;
        for (let i = 0; i < pages.length; i++) {
          this.curPage = i + 1;
          const canvas = await html2canvas(pages[i], {
            scale: "5",
          });
          const isNormalPage = this.pageConfig.pageDirection === "p";
          doc.addImage({
            imageData: canvas.toDataURL("image/jpeg"),
            format: "JPEG",
            x: 0,
            y: 0,
            width: isNormalPage ? this.pageConfig.pageWidth : this.pageConfig.pageHeight,
            height: isNormalPage ? this.pageConfig.pageHeight : this.pageConfig.pageWidth,
          });
          if (i < pages.length - 1) {
            doc.addPage();
          }
        }
        doc.save(`${this.fileName || this.pageConfig.title || "预览"}.pdf`);
        this.initCompleted = true;
        this.isExportPDF = false;
        if (this.directExport) {
          this.$emit("update:visible", false);
        }
      });
    },
    async btnClickPrint() {
      setTimeout(async () => {
        // let printData = this.printForm.printers;
        let PageWidth = Number(this.pageConfig.pageWidth) * 5; //根据后端返回的打印配置信息设置纸张宽度
        let PageHeight = Number(this.pageConfig.pageHeight) * 5; //根据后端返回的打印配置信息设置纸张高度

        //初始化打印函数

        let ldop = await import("../js/LodopFuncs");
        let LODOP = ldop.getLodop();
        // 初始化打印
        LODOP.PRINT_INIT("lodop");
        // 设置纸张大小
        LODOP.SET_PRINT_PAGESIZE(
          this.pageConfig.pageDirection == "P" ? 1 : 2,
          PageWidth,
          PageHeight
        );
        //由于不好控制分页的时候，切纸的位置，于是换了一种方式，代码控制每一页的内容的地方使用LODOP.NewPageA();实现手动分页，这种方式可以保证切纸位置不会跑偏。
        const pages = this.$el.getElementsByClassName("roy-preview-page");
        for (let i = 0; i < pages.length; i++) {
          let item = pages[i];
          const canvas = await html2canvas(item, {
            scale: "5",
          });
          let canImg = canvas.toDataURL("image/jpeg");
          LODOP.ADD_PRINT_IMAGE(100, 100, "100%", "100%", canImg);
          LODOP.NewPageA();
        }
        //缩放比例 整页缩放打印
        LODOP.SET_PRINT_MODE("PRINT_PAGE_PERCENT", "Auto-Width");
        // //字体大小设置
        // //LODOP.SET_PRINT_STYLEA("FontSize", 12);
        LODOP.SET_PRINT_MODE("RESELECT_PRINTER", true); //允许重选打印机
        LODOP.SET_PRINT_MODE("RESELECT_ORIENT", true); //允许重选纸张方向
        LODOP.SET_PRINT_MODE("RESELECT_PAGESIZE", true); //允许重选纸张
        LODOP.SET_PRINT_MODE("RESELECT_COPIES", true); //允许重选份数
        //打印设计
        LODOP.PREVIEW();
        //设置设置完打印后 是否关闭预览窗口;
        LODOP.SET_PRINT_MODE("AUTO_CLOSE_PREWINDOW", 1);
        // LODOP.PRINT_DESIGN();
        LODOP.PRINT();
      }, 500);
    },
  },
  created() {
    this.visibleIn = this.visible;
  },
  mounted() {
    // this.initMounted()
  },
  watch: {
    visibleIn(newVal) {
      this.$emit("update:visible", newVal);
    },
    listPrintData: {
      async handler() {
        this.$forceUpdate();
        await this.initMounted();
      },
      deep: true,
      immediate: true,
    },
  },
};
</script>
  
  <style lang="scss">
.rptd-viewer {
  height: 100%;
  padding: 0;
  margin: 0;

  .vxe-modal--body {
    background: #efefef;
  }

  .roy-viewer {
    height: 100%;
    width: 100%;
    background: #efefef;
    display: block;
    overflow: auto;

    .roy-preview-page {
      margin: 20px auto;
      page-break-after: always;
    }
  }

  .is-show-border {
    .roy-preview-page {
      border: solid 1px #000;
    }
  }

  .roy-temp-holder {
    z-index: -1;
  }

  .roy-page-blank {
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    background: #fff;
  }

  .roy-viewer-right-conor {
    right: 25px;
    bottom: 25px;
    position: absolute;

    .roy-viewer-btn {
      width: 100px;
      height: 24px;
      font-size: 14px;
      line-height: 24px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: #4579e1;
      border-radius: 4px;
      user-select: none;
      cursor: pointer;
      color: #fff;
      box-shadow: rgba(69, 121, 225, 0.1) 0 4px 12px;

      & + .roy-viewer-btn {
        margin-top: 10px;
      }

      &:hover {
        box-shadow: none;
      }
    }
  }
}

@page {
  .roy-viewer {
    width: 0;
    height: 0;

    .roy-preview-page {
      margin: 0;
      padding: 0;
      border: none;
    }
  }
}

::-webkit-scrollbar {
  width: 12px;
  height: 12px;
}

::-webkit-scrollbar-track {
  background-color: #f4f4f4;
  border-radius: 8px;
}

::-webkit-scrollbar-track-piece {
  background-color: #f4f4f4;
  border-radius: 8px;
}

::-webkit-scrollbar-thumb {
  border-radius: 2px;
  background: #e2e2e2;
}

::-webkit-scrollbar-thumb:hover {
  background-color: #d9d9d9;
}

::selection {
  background: #3e6dcb;
  color: #fff;
}
</style>
  