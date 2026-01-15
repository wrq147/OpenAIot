<template>
    <div id="basic-cont-box" class="basic-cont-box">
        <div class="chart-view-toolbar" style="top: 0px;">
            <div class="design-info">
                <div class="design-name">{{ workbookName }}</div>
                <!-- <div class="line"></div>
                <div class="create-by">创建人: {{ creationUser.name }}</div> -->
            </div>
            <div class="design-action">
                <div class="action-item item" @click="downloadExcel">
                    <img src="@/assets/images/download.png" alt="">
                </div>
                <div class="action-item item" @click="enterFullScreen">
                    <img src="@/assets/images/fullScreen.png" alt="">
                </div>
                <div class="action-item item" @click="printExcel">
                    <img src="@/assets/images/print.png" alt="">
                </div>
            </div>
        </div>
        <div id="contianer" class="contianer">
            <div class="content-box">
                <div class="search-list-box" v-if="searchTableData&&searchTableData.length>0">
                    <div class="search-item" v-for="(item, index) in searchTableData" :key="index">
                        <div class="search-item-title">{{ item.fieldName }}: </div>
                        <div  v-if="item.searchType === 0" class="search-content-box">
                            <el-input class="searchInput" v-model="item.searchDefault" placeholder="请输入查询内容" />
                        </div>
                        <div v-if="item.searchType === 1" class="search-content-box">
                            <el-date-picker class="searchInput" v-model="item.searchDefault" :format="item.format" type="datetime" placeholder="请选择开始时间" />
                        </div>
                        <div v-if="item.searchType === 2" class="search-content-box">
                            <el-date-picker class="searchInput" v-model="item.searchDefault" :format="item.format" type="daterange" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期" />
                        </div>
                    </div>
                    <div class="search-button-box">
                        <el-button size="mini" @click="resetList">重置</el-button>
                        <el-button size="mini" type="primary" @click="searchList">查询</el-button>
                    </div>
                </div>
                <div ref="printId" class="lucky-sheet-box">
                    <div class="lucky-sheet-page">
                        <div id="luckysheet" class="lucky-sheet"></div>
                    </div>
                </div>
            </div>
        </div>
        <el-dialog v-if="codeFlag" :visible.sync="codeFlag" width="600px" append-to-body :close-on-click-modal="false">
            <div style="display:flex">
                <label style="width:80px;line-height:40px;color:#000;font-size:16px">查看密码</label>
                <el-input v-model="code" class="effectSpan" @keydown.enter.native="checkCode"></el-input>
            </div>

            <div slot="footer" class="dialog-footer">
                <el-button type="primary" @click="checkCode">确认</el-button>
            </div>
        </el-dialog>
    </div>
</template>
<script>
import { rptInfo, checkShare } from "@/api/report/report";
import { getToken, setShareToken, removeShareToken } from '@/utils/auth'
import {
  getUrlData,
  getbaseData,
  combinationTableColum,
  combinationConfirmValue
} from "./commonRuning";
import printJs from 'print-js'
import { exportExcel } from './export'
import { loadLuckysheet } from '@/utils/luckysheetLoader.js'
export default {
    name: 'viewDataReport',
   
    data() {
        return {
            globalData: [],
            tableGlobal: [], // 默认数组
            forwardGlobal: [], // 正序数组
            reverseGlobal: [], // 倒序数组
            cellDataChange: [], // 获取需要渲染的列
            cellDataChangeOld: [], // 不需要渲染的标题
            workbookName: '',
            loadingQuery: null, // 加载中
            searchTableData: [], // 查询列表
            creationUser: {}, // 创建者信息
            luckysheetData: [], // 表格数组
            codeFlag: false,
            code: '',
            screenId: null,
            tokenId: null
        }
    },
    async created() {
        await loadLuckysheet();
        this.loadingQuery= this.$loading({//进入页面设置加载中效果，方便完成页面保存数据的初始化
            lock: true,
            text: 'Loading',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 1)',
            target:document.getElementById('container')
        })
        this.screenId = this.$route.query.screenId;
        this.tokenId = this.$route.query.tokenId;
        if(this.$route.query.tokenid){
            this.tokenId =this.$route.query.tokenid;
        }
        this.getLogin(this.screenId, this.tokenId);
    },
    methods: {
        getLogin(screenId, tokenId) {
            if (tokenId != null) {
                removeShareToken();
                //参数有tokenid 必须输入验证码
                checkShare(tokenId, this.code).then(response => {
                    this.codeFlag = false;
                    let screenId = response.data.Share.ReportId;
                    let tokenStr = response.data.Share.TokenStr;
                    //放入token
                    if (!getToken()) {
                        setShareToken(tokenStr);
                    }
                    this.loadDetailData(0,response.data.Report);
                }).catch(err=>{
                    if(err.code === 1) {
                        this.codeFlag = true;
                    }
                });
            } else {
                if (screenId === undefined) {
                    localStorage.removeItem('searchTableData');
                    this.getList(0);
                } else {
                    this.getListDetail(0, screenId);
                }
            }
        },

        async loadDetailData(index,response){
            let themeForm = JSON.parse(response.ThemeOption);
            this.luckysheetData = JSON.parse(response.DrawOption);
            this.workbookName = response.Name;
            this.luckysheetData.showGridLines = 0,//是否显示网格线
            this.globalData = themeForm.globalData;
            this.globalIndex = themeForm.globalIndex;
            this.creationUser = themeForm.creationUser;
            this.searchTableData = themeForm.searchTableData.filter(item => item.openSearch === true );
            this.disposeCellData(this.luckysheetData[index].celldata);
            this.tableGlobal = await this.loadInitalData(this.globalData, '');
            this.initSheet(this.luckysheetData);
        },
        getListDetail(index, screenId) {
            rptInfo(screenId).then(async (response) => {
                let data = response.data;
                let themeForm = JSON.parse(data.ThemeOption);
                this.workbookName = data.Name;
                this.luckysheetData = JSON.parse(data.DrawOption);
                this.luckysheetData.showGridLines = 0,//是否显示网格线
                this.globalData = themeForm.globalData;
                this.globalIndex = themeForm.globalIndex;
                this.creationUser = themeForm.creationUser;
                this.searchTableData = themeForm.searchTableData.filter(item => item.openSearch === true );
                this.disposeCellData(this.luckysheetData[index].celldata);
                this.tableGlobal = await this.loadInitalData(this.globalData, '');
                this.initSheet(this.luckysheetData);
            });
        },
        async getList(index) {
            let viewdataSheet = JSON.parse(localStorage.getItem("viewdataSheet"));
            let luckysheetData = viewdataSheet.luckysheetData;
            luckysheetData.showGridLines = 0,//是否显示网格线
            this.luckysheetData = luckysheetData;
            this.globalData = viewdataSheet.globalData;
            this.globalIndex = viewdataSheet.globalIndex;
            this.workbookName = viewdataSheet.workbookName;
            this.creationUser = viewdataSheet.creationUser;
            this.searchTableData = viewdataSheet.searchTableData.filter(item => item.openSearch === true );
            this.disposeCellData(luckysheetData[index].celldata);
            this.tableGlobal = await this.loadInitalData(this.globalData, '');
            this.initSheet(luckysheetData);
        },
        checkCode() {
            this.getLogin(this.screenId, this.tokenId);
        },
        async disposeCellData(celldata) {
            this.cellDataChange = []
            this.cellDataChangeOld = []
            celldata.forEach(item => {
                this.extractTemplateExpressions(item);
            });
        },
        // 识别字符串里有${ } 并提取出来
        extractTemplateExpressions(str) {
            const regex = /\$\{([^}]+)\}/g;
            let match;
            const expressions = {};
            while ((match = regex.exec(str.v.v))) {
                expressions.c = str.c;
                expressions.r = str.r;
                expressions.extend = str.v.extend;
                expressions.polymerization = str.v.polymerization;
                expressions.sort = str.v.sort;
                expressions.key = match[1];
                this.cellDataChange.push(expressions);
            }
        },
        async loadInitalData(globalData, searchTableData) {
            let ddtype = globalData[this.globalIndex] ? globalData[this.globalIndex].dataSourceType : '';
            switch (ddtype) {
                case 'url':
                    return await getUrlData(globalData[this.globalIndex], searchTableData);
                case 'database':
                    return await getbaseData(globalData[this.globalIndex], searchTableData);
                case 'combination':
                    let tableColum = await combinationTableColum(globalData[this.globalIndex], globalData, []);
                    globalData[this.globalIndex].combinationTable = tableColum;
                    return await combinationConfirmValue(globalData[this.globalIndex]);
                default: []
            }
        },
        initSheet(luckysheetData) {
            let that = this;
            luckysheet.create({
                container: 'luckysheet', // DOM容器的ID
                lang: 'zh', // 设定表格的语言
                showtoolbar: false,
                showinfobar: false,
                allowEdit: false,
                data: luckysheetData,
                showsheetbarConfig: {
                    add: false, // 底部sheet页隐藏新增sheet按钮
                    menu: false, // 底部sheet页隐藏管理按钮
                },
                sheetRightClickConfig: {
                    hide: false, // 隐藏，取消隐藏
                    move: false, // 向左移，向右移
                },
                hook: {
                    workbookCreateAfter: function () {
                        that.dataRendSheet();
                    },
                    sheetActivate: function (sheetIndex) {
                        that.luckysheetData.forEach(async (item, index) => {
                            if (item.index === sheetIndex) {
                                await that.disposeCellData(item.celldata);
                                that.dataRendSheet();
                            }
                        })
                    },
                }
            })
        },
       dataRendSheet() {
            this.cellDataChange.forEach(async item => {
                let c = item.c;
                let r = item.r;
                let key = item.key;
                let dataGlobal = await this.sequenceTableGlobal(item)
                dataGlobal.forEach(v => {
                    if (v[key] !==undefined && v[key]!=='') {
                        luckysheet.setCellValue(r, c, v[key])
                        if (item.extend === 'vertical') {
                            r +=1;
                        } else {
                            c +=1;
                        }
                    }
                })
                if (item.polymerization === 'group') {
                    this.mergeCellsByCondition(item);
                }
            })
            this.loadingQuery.close();
        },
         // 获取正序和倒序数组
        async sequenceTableGlobal(item) {
            let data = [...this.tableGlobal]
            let dataGlobal = []
            if (item.sort === "default") {
                dataGlobal = data
            }else {
                dataGlobal = data.sort((a, b) => {
                    let comparisonResult;
                    if (item.sort === "forward") {
                        if (a[item.key] && b[item.key]) {
                            comparisonResult = a[item.key].localeCompare(b[item.key]);
                        }
                        return comparisonResult;
                    } else if (item.sort === "reverse") {
                        if (a[item.key] && b[item.key]) {
                            comparisonResult = b[item.key].localeCompare(a[item.key]);
                        }
                        return comparisonResult;
                    }
                });
            }
            return dataGlobal
        },
        // 是否合并列或行的单元格
        mergeCellsByCondition(item) {
            const sheet = luckysheet.getluckysheetfile()[0];
            const rows = sheet.data;
            const rowIndex = item.r;
            const columnIndex = item.c;
            let startCol = item.extend === 'vertical' ? 0 : columnIndex;
            let index = item.extend === 'vertical' ? rowIndex : columnIndex;
            let prevValue;
            const mergedRanges = [];
            for (let i = index; i < this.tableGlobal.length + (index + 1); i++) { 
                let currentValue = item.extend === 'vertical' ? 
                                    rows[i][columnIndex] ? rows[i][columnIndex].v : '': 
                                    rows[rowIndex][i] ? rows[rowIndex][i].v : '';
                if (prevValue === undefined || prevValue !== currentValue) {
                    if (startCol !== i) {
                        if (startCol < i - 1) {
                            if (item.extend === 'vertical') {
                                mergedRanges.push({
                                    row: [startCol, i-1],
                                    column: [columnIndex, columnIndex]
                                });
                            } else {
                                mergedRanges.push({
                                    row: [rowIndex, rowIndex],
                                    column: [startCol, i-1]
                                });
                            }
                        }
                    }
                    startCol = i;
                    prevValue = currentValue;
                }
            }
            luckysheet.setRangeMerge(item.extend, { range: mergedRanges });
        },
        // 重置搜索列表
        resetList() {
            this.searchTableData.forEach(item => {
                item.searchDefault = ''
            })
        },
        // 查询搜索列表
        searchList() {
            // this.tableGlobal = this.loadInitalData(this.globalData, this.searchTableData)
            localStorage.setItem("searchTableData", JSON.stringify(this.searchTableData));
        },
        // 全屏
        enterFullScreen() {
            this.$nextTick(() => {
                let element = document.getElementById('contianer');
                let requestMethod = element.requestFullscreen || element.webkitRequestFullscreen || element.mozRequestFullscreen || element.msRequestFullscreen;
                if (requestMethod) {
                    requestMethod.call(element);
                }
            })
        },
        // 获取表格中包含内容的row，column
        getExcelRowColumn() {
            const sheetData = luckysheet.getSheetData();
            let objRowColumn = {
                row: [null, null], //行
                column: [null, null], //列
            };
            sheetData.forEach((item, index) => {
                //行数
                item.forEach((it, itemIndex) => {
                    if (it !== null) {
                        if (objRowColumn.row[0] == null) objRowColumn.row[0] = index; // row第一位
                            objRowColumn.row[1] = index; //row第二位
                            if (objRowColumn.column[0] == null)
                                objRowColumn.column[0] = itemIndex; //column第一位
                                objRowColumn.column[1] = itemIndex; //column第二位
                    }
                });
            });
            return objRowColumn;
        },
        // 打印操作
        printExcel() {
            let RowColumn = this.getExcelRowColumn() // 获取有值的行和列
            RowColumn.column[0] = 0 //因需要打印左边的边框，需重新设置
            luckysheet.setRangeShow(RowColumn) // 进行选区操作
            let src = luckysheet.getScreenshot(); // 生成base64图片
            printJs({
                printable: src,
                type: 'image',
                documentTitle: '', // 标题
                properties: {
                    'margin': '0',  // 去除页边距
                    'width': '100%'  // 设置宽度为 100% 以尽量铺满
                },
                style: '@media print {body{margin:0 5px;display: inline-block;}}' // 去除页眉页脚
            })
        },
        // 下载到excel表格
        downloadExcel() {
            let json = luckysheet.getAllSheets();
            exportExcel(json, this.workbookName);
        }
    }
}
</script>
<style lang="scss" scoped>
::v-deep{
    .searchInput {
        height: 32px;
        position: relative;
        font-size: 14px;
        display: inline-flex;
        width: 190px !important;
        line-height: 32px;
        box-sizing: border-box;
        vertical-align: middle;
    }
    .searchInput .el-input__inner{
        height: 32px;
        border: none;
        background-color: #f5f6f7 !important;
        box-shadow: none !important;
    }
    .el-input__icon{
        line-height: 32px;
    }
}
.luckysheet_info_detail{
    display: none !important;
}
.basic-cont-box{
    position: relative;
    height: 100%;
    box-sizing: border-box;
}
.chart-view-toolbar {
    position: absolute;
    height: 62px;
    z-index: 1;
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: space-between;
    background-color: #fff;
    padding: 0 24px;
    box-sizing: border-box;
    transition: all .2s;
}
.chart-view-toolbar .design-info {
    display: flex;
    align-items: center;
}
.chart-view-toolbar .design-info .design-name {
    color: #363b4c;
    font-size: 16px;
    font-weight: bold;
}
.chart-view-toolbar .design-info .line {
    width: 1px;
    height: 20px;
    background-color: #d7d8db;
    margin: 0 8px;
}
.chart-view-toolbar .design-info .create-by {
    font-size: 14px;
    color: #6f7588;
}
.chart-view-toolbar .design-action {
    display: flex;
    align-items: center;
}
.chart-view-toolbar .design-action .action-item {
    width: 32px;
    height: 32px;
    background: #f5f6f7;
    border-radius: 4px;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    position: relative;
    margin-right: 15px;
}
.chart-view-toolbar .design-action .action-item>img{
    width: 100%;
    height: 100%;
}
.contianer {
    // height: calc(100% - 2px);
    // padding-top: 62px;
    width: 100%;
    position: absolute;
    top: 62px;  /* 内容从头部下方开始 */
    left: 0;
    right: 0;
    bottom: 0;
    // background-color: #e9ecef;  /* 内容背景颜色 */
    // background-color: gray;
}
.contianer .content-box{
    height: 100%;
}
.contianer .content-box .search-list-box {
    background: #fff;
    padding: 2px 24px 0;
    display: flex;
    flex-wrap: wrap;
}
.contianer .content-box .lucky-sheet-box{
    height: calc(100% - 2px);
    width: 100%;
}
.lucky-sheet-page{
    position: relative;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    overflow: hidden;
}
.lucky-sheet-page .lucky-sheet {
    padding: 0;
    position: absolute;
    width: 100%;
    height: calc(100% + -0px);
    left: 0;
    top: 0;
    bottom: 0;
}
::v-deep .luckysheet-grid-window{
    bottom: 40px !important;
}
.contianer .content-box .search-list-box .search-item{
    height: 30px;
    display: flex;
    margin-bottom: 10px;
    align-items: center;
    margin-right: 15px;
}
.contianer .content-box .search-list-box .search-item .search-item-title{
    font-size: 15px;
    font-weight: 600;
    padding-right: 5px;
}
.contianer .content-box .search-list-box .search-button-box{
    margin-bottom: 10px;
}
</style>