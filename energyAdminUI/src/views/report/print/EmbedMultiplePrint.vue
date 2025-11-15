<template>
    <div v-if="viewerVisible" style="height: 100vh">
        <PtdViewer @closePrint="closePrint" :listPrintData="listPrintData" :component-data="componentData" :data-set="dataSet" :data-source="dataSource" :page-config="globalConfig"
            :visible.sync="viewerVisible" :needToast="false"/>
    </div>
</template>
  
<script>
import PtdViewer from '@/components/print/Viewer/MultPtdViewer'
import { getDataInfo, templateInfo } from '@/api/report/printTemplate'
import { chartApi } from '@/api/report/chartApi'
import { getApiSource } from '@/api/report/apisource'
//模板打印开始
import PrintDesigner from '@/components/print/index.js'


//模板打印结束
import Vue from 'vue'
import store from '@/store'

export default {
    name: 'PrintDesign',
    components: {
        PtdViewer
    },
    data() {
        return {
            viewerVisible: false,
            dataSet: undefined,
            componentData: undefined,
            dataSource: undefined,
            globalConfig: undefined,
            listPrintData:[]
        }
    },
    mounted(){
        this.$nextTick(()=>{
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
        })
    },
    methods: {
        closePrint(){
            this.$emit('closePrint')
        },
        async startPrint(id, inputParamsList){
            let teminfo = (await templateInfo(id)).data;
            let res = await getDataInfo(teminfo.DataId);
            let apires = await getApiSource(res.data.ApiId);
            let reqobj = {};
            reqobj.interfaceURL = apires.data.Url;
            reqobj.requestMethod = apires.data.Method;
            reqobj.requestHeader = JSON.parse(apires.data.HeaderJson);
            reqobj.requestParamType = apires.data.ParamType;
            if(inputParamsList&&inputParamsList.length>0){
                this.listPrintData=await Promise.all((inputParamsList.map(async inputParams=>{
                    let rowRes=await this.showPrint(inputParams,reqobj,res,teminfo,apires)
                    return rowRes
                }))
                );
                this.viewerVisible=true;
                this.$forceUpdate()
            }else{
                this.viewerVisible=true;
            }
        },
        async showPrint(inputParams,reqobj,res,teminfo,apires) {
            let printObj={}
            reqobj.requestParameters = Object.assign(JSON.parse(apires.data.ParamJson), inputParams);
            let drs;
            try {
                drs = await chartApi(reqobj);
            }
            catch (e) { drs = e; }

            printObj.dataSet = drs.data;
            printObj.dataSource = JSON.parse(res.data.DataMap);
            printObj.componentData = JSON.parse(teminfo.Content);
            printObj.globalConfig = {
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
            this.globalConfig=printObj.globalConfig
            return printObj
        }
    }
}
</script>
  
<style lang="scss"></style>
  