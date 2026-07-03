<template>
  <div style="height: 100vh">
    <PtdDesigner ref="designer" @exit="onExit" @save="onSave">
      <template v-slot:roy-designer-header-slot>
        <div class="head-slot">
          <template v-for="(tool, index) in headIconConfig">
            <i v-if="tool.name!='ImportTemplate'" :key="index" :class="tool.icon" :title="tool.title" class="roy-header-icon" @click="tool.event"></i>
            <el-upload v-if="tool.name=='ImportTemplate'" :key="index" style="display:inline" accept=".json" :multiple="false" :show-file-list="false" action="#" :before-upload="tool.event">
              <i :class="tool.icon" :title="tool.title" class="roy-header-icon"></i>
            </el-upload>
          </template>
        </div>
      </template>
    </PtdDesigner>
    <PtdViewer
      v-if="viewerVisible"
      :component-data="componentData"
      :data-set="dataSet"
      :data-source="dataSource"
      :page-config="pageConfig"
      :visible.sync="viewerVisible"
    />
  </div>
</template>

<script>
import PtdViewer from '@/components/print/Viewer/PtdViewer'
import { mapState } from 'vuex'

import {getDataInfo,addPrintTemplate,editPrintTemplate,templateInfo} from '@/api/report/printTemplate'
import {getApiSource} from '@/api/report/apisource'
import {chartApi} from '@/api/report/chartApi'

//模板打印开始
import PrintDesigner from '@/components/print/index.js'
import 'vxe-table/lib/style.css'; 
// import 'remixicon/fonts/remixicon.css'
// import 'xe-utils'
// import VXETable from 'vxe-table'
// import '@/components/print/css/vxevarible.scss'
// import XEUtils from 'xe-utils'
// import VXEUtils from 'vxe-utils'
import Vue from 'vue'
import store from '@/store'

// Vue.use(VXEUtils, XEUtils)
// Vue.use(VXETable)
// Vue.use(PrintDesigner, {
//     store
// })
// Vue.prototype.$VXETable = VXETable
// Vue.prototype.$XModal = VXETable.modal
//模板打印结束


export default {
  name: 'PrintDesign',
  beforeCreate(){
    require('remixicon/fonts/remixicon.css')
    require('xe-utils')
    const VXETable=require('vxe-table')
    require('@/components/print/css/vxevarible.scss')
    const XEUtils=require('xe-utils')
    const VXEUtils=require('vxe-utils')
    Vue.use(VXEUtils, XEUtils)
    Vue.use(VXETable)
    Vue.use(PrintDesigner, {
        store
    })
    Vue.prototype.$VXETable = VXETable
    Vue.prototype.$XModal = VXETable.modal
  },
  async created() {
    this.id=this.$route.query.id;
    let teminfo=null;
    if(this.id==null){
      this.dataId=this.$route.query.data;
    }
    else{
      let rsssp=await templateInfo(this.id);
      teminfo=rsssp.data;
      this.dataId=teminfo.DataId;
    }
    let res=await getDataInfo(this.dataId);
    let tmpDataSource=JSON.parse(res.data.DataMap);
    let apires=await getApiSource(res.data.ApiId);
    let reqobj={};
    reqobj.interfaceURL=apires.data.Url;
    reqobj.requestMethod=apires.data.Method;
    reqobj.requestHeader=JSON.parse(apires.data.HeaderJson);
    reqobj.requestParamType=apires.data.ParamType;
    reqobj.requestParameters=JSON.parse(apires.data.ParamJson);
    let drs;
    try{
      drs=await chartApi(reqobj);
    }
    catch(e){
      drs=e;
    }
    
    let tmpDataSet=drs.data;
    // console.info(tmpDataSet)
    this.$refs.designer.loadDataSource(tmpDataSource,tmpDataSet);
    // console.log("打印模板信息",teminfo);
    if(teminfo!=null){
      let globalconfig = {
          // 页面大小
          pageSize: teminfo.PaperName,
          // 页面方向 l 横向长 p 纵向长
          pageDirection: teminfo.PaperDirection,
          // 页面长度：mm
          pageWidth: teminfo.PaperWidth,
          // 页面高度：mm
          pageHeight: teminfo.PaperHeight,
          // 页面下边距 mm
          pageMarginBottom: teminfo.PaperMarginBottom,
          // 页面上边距 mm
          pageMarginTop: teminfo.PaperMarginTop,
          // 页面标题/模板标题
          title: teminfo.Name,
          // 默认缩放比例：100%
          scale: 1,
          // 页面背景
          background: teminfo.Background,
          // 默认字体颜色
          color: '#212121',
          // 默认字号
          fontSize: 12,
          // 默认字体
          fontFamily: teminfo.FontFamily,
          // 默认行高
          lineHeight: teminfo.LineHeight
      }
      let componentData=JSON.parse(teminfo.Content);
      this.$refs.designer.loadTemplate(globalconfig,componentData);
    }
  },
  components: {
    PtdViewer
  },
  computed: {
    ...mapState({
      pageConfig: (state) => state.printTemplateModule.pageConfig,
      componentData: (state) => state.printTemplateModule.componentData,
      dataSource: (state) => state.printTemplateModule.dataSource,
      dataSet: (state) => state.printTemplateModule.dataSet
    })
  },
  data() {
    return {
      id:"",
      dataId:"",
      headIconConfig: [
        {
          name: 'Github',
          icon: 'ri-file-word-2-line',
          title: '预设模板',
          event: () => {
            this.$modal.msgError("暂不支持预设模板");
            this.templateVisible = true
          }
        },
        {
          name: 'ShowViewer',
          icon: 'ri-eye-line',
          title: '预览设计模板',
          event: () => {
            this.showViewer()
          }
        },
        {
          name: 'ExportTemplate',
          icon: 'el-icon-upload2',
          title: '导出模板',
          event: () => {
            this.ExportTemplate()
          }
        },
        {
          name: 'ImportTemplate',
          icon: 'el-icon-download',
          title: '导入模板',
          event: (file) => {
            this.ImportTemplate(file)
          }
        }
      ],
      viewerVisible: false,
      templateVisible: false
    }
  },
  methods: {
    ExportTemplate(){
      //导出模板
      let tmploading = this.$loading({
        lock: true,
        text: "导出中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      let msgitem = JSON.parse(JSON.stringify(this.componentData))
      let tmname = this.pageConfig.title;
      tmploading.close();
      const content = JSON.stringify(msgitem)
      const blobData = new Blob([content], { type: 'application/json' })
      const filename = `${tmname}.json` //可以自定义后缀名

      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blobData, filename)
      } else {
        const anchor = document.createElement('a')
        anchor.href = window.URL.createObjectURL(blobData)
        anchor.download = filename
        anchor.click()
        window.URL.revokeObjectURL(blobData)
      }
    },
    ImportTemplate(file){
      //导入模板
      let tmploading = this.$loading({
        lock: true,
        text: "导入中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      const reader = new FileReader()
      reader.readAsText(file)
      reader.onload = (e)=> {
       
        try {
          const str = e.target.result
          const jsonData = JSON.parse(str)
          if(jsonData.length&&jsonData.length>0){
            let objkeys=Object.keys(jsonData[0]);
            if(objkeys.includes('code')&&objkeys.includes('component')&&objkeys.includes('label')&&objkeys.includes('name')){
              this.$refs.designer.loadTemplate(this.pageConfig,jsonData);
            }else{
              this.$modal.msgError("导入的模板格式错误");
            }
          }
          
        } catch (error) {
          console.log(error,'报错');
        }
        tmploading.close();
      }
    },
    onExit(){
      this.$confirm("未发布的内容将不会被保存，是否直接退出 ?", "提示", {
        confirmButtonText: "退出",
        cancelButtonText: "取消",
        type: "warning"
      }).then(() => {
        this.$store.dispatch('tagsView/delView', this.$route);
        this.$router.go(-1);
      });
    },
    onSave(data){
      this.$modal
        .confirm('是否确认保存"' + data.pageConfig.title + '"吗？')
        .then(()=> {
          let saveData = {
            Name:data.pageConfig.title,
            PaperDirection:data.pageConfig.pageDirection,
            PaperMarginTop:data.pageConfig.pageMarginTop,
            PaperMarginBottom:data.pageConfig.pageMarginBottom,
            Background:data.pageConfig.background,
            FontFamily:data.pageConfig.fontFamily,
            LineHeight:data.pageConfig.lineHeight,
            PaperName:data.pageConfig.pageSize,
            PaperWidth:data.pageConfig.pageWidth,
            PaperHeight:data.pageConfig.pageHeight
          }
          saveData.Content=JSON.stringify(data.componentData);
          
          if(this.id==null){
            saveData.DataId=this.dataId;
            return addPrintTemplate(saveData);
          }
          else{
            saveData.Id=this.id;
            return editPrintTemplate(saveData);
          }
        })
        .then((rsp) => {
          if(this.id==null){
            this.id=rsp.data;
            console.info(this.id);
          }
 
          this.$modal.msgSuccess("保存成功");
        })
        .catch(() => {});
    

    },
    showViewer() {
      this.viewerVisible = true
    },
  }
}
</script>

<style lang="scss">
html,
body,
.height-all {
  height: 100%;
}

.head-slot {
  display: flex;
  height: 40px;
  align-items: center;

  .roy-header-icon {
    padding: 0 8px;
    font-size: 14px;
    cursor: pointer;

    &:hover {
      background: var(--roy-color-primary-light-3);
    }
  }
}

.toolbar-slot-item {
  cursor: pointer;
  background: var(--roy-bg-color);
  display: grid;
  width: 18px;
  height: 18px;
  font-size: 10px;
  line-height: 18px;
  box-shadow: rgba(99, 99, 99, 0.2) 0 2px 8px 0;
  padding: 4px;
  text-align: center;
  border-radius: 4px;

  & + .roy-designer-main__toolbar__item {
    margin-left: 5px;
  }

  &:hover {
    background: var(--roy-bg-color-page);
  }

  i {
    padding: 0;
    margin: 0;
    font-size: 14px;
  }
}

</style>
