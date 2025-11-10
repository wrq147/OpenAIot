<template>
    <div id="container" class="basic-cont-box">
        <div class="report-design-page">
            <div class="designer-header">
                <div class="design-name-box">
                    <div class="name-input-box">
                        <div class="info-name">
                            <div v-show="!isWorkbookName">
                                <span style="margin-right: 15px;">{{ workbookName }}</span> 
                                <el-button size="mini" type="primary" icon="el-icon-edit" circle @click="workbookNameClick" />
                            </div>
                            <el-input class="workbookNameInput" v-show="isWorkbookName" ref="inputRef" v-model="workbookName" @blur="isWorkbookName = !isWorkbookName" />
                        </div>
                    </div>
                </div>
                <div class="action-box">
                    <el-upload
                        :file-list="fileList"
                        accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        class="upload-demo"
                        :before-upload="beforeUpload"
                        action=""
                        :show-file-list="false"
                    >
                        <el-button size="mini" type="success">上传Excel</el-button>
                    </el-upload>
                    <el-button type="danger" size="mini" @click="previewSheet" style="margin-left: 10px;">预览</el-button>
                    <el-button type="primary" size="mini" @click="saveSheet">保存</el-button>
                </div>
            </div>
            <div class="report-design-content">
               <div class="design-page">
                    <div class="design-left">
                        <div class="lucky-sheet-page">
                            <div id="luckysheet" class="luckysheet-wrap"></div>
                        </div>
                    </div>
                    <div class="design-right">
                        <div class="com-setting-page">
                            <information ref="information" :themeForm="themeForm" :cellData="cellData" @renewalData="renewalData" :keyList="keyList" @getSearchData="getSearchData"/>
                            <dataGather :themeForm="themeForm" :drawingList="drawingList" @fieldData="fieldData" @getKeyList="getKeyList" />
                        </div>
                    </div>
               </div>
            </div>
        </div>
    </div>
</template>
<script>
import { mapGetters } from 'vuex'
import { rptInfo, editRpt} from "@/api/report/report";
import information from "./information";
import dataGather from "./dataGather";
import {
  confirmValue,
  getbaseData,
  combinationTableColum,
  combinationConfirmValue,
} from "@/views/report/datav/LayerItems/commonRuning";
import LuckyExcel from 'luckyexcel'
export default {
    name: 'spreadSheet',
    components: {
        information,
        dataGather
    },
    computed: {
        ...mapGetters(['name'])
    },
    data() {
        return {
            defaultForm: {
                chartType: "themeForm",
                customId: "0",
                isSelfAdaption: true,
                adaptionType: "0", //全自适应
                globalData: [],
                chartOption: {
                    database: {
                        type: 'MySQL',
                        ipAdress: '127.0.0.1',
                        port: '3306',
                        username: 'root',
                        password: '123',
                        baseName: 'test',
                        tableName: 'test1',
                        dataSourceId: '',
                        sqlType: 'MySQL',
                        executeSql: ''
                    },
                    timeout: 200
                },
            },
            luckysheetOldData: { "name": "Sheet1", color: "", "status": "1", "order": "0", "data": [], "config": {}, "index":0,  "defaultRowHeight": 30, "defaultColWidth": 120, 'row': 80 },
            cellData: {
                c: 0,
                r: 0,
                value: '',
                rowHeight: '',
                columnWidth: '',
                polymerization: 'list',
                sort: 'forward',
                extend: 'vertical',
                isGroupSetting: false
            },
            workbookName: '未命名报表',
            isWorkbookName: false,
            fieldValue: '',
            form: [],
            drawingList: [],
            themeForm: {},
            activeId: '',
            activeData: {},
            loadingQuery: null, // 加载中
            keyList: [], // 节点数据
            globalIndex: '', // global index
            searchTableData: [], // 配置的搜索信息
            creationUser: {}, // 创建者信息
            fileList: [] // 上传文件
        }
    },
    created() {
        this.loadingQuery= this.$loading({//进入页面设置加载中效果，方便完成页面保存数据的初始化
            lock: true,
            text: 'Loading',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 1)',
            target:document.getElementById('container')
        });
        this.creationUser = {
            name: this.name
        }
        this.initDataDraw();
    },
    mounted(){
       
    },
    methods: {
        async initDataDraw() {
            //获取路由传来的参数
            let sId = this.$route.query.screenId;
            rptInfo(sId).then((response) => {
                this.form = response.data;
                this.workbookName = response.data.Name;
                this.drawingList = [];
                this.themeForm = this.form.ThemeOption === "" ? this.defaultForm : JSON.parse(this.form.ThemeOption);
                this.searchTableData = this.themeForm.searchTableData !== undefined ? this.themeForm.searchTableData : [];
                this.activeId = this.themeForm.customId;
                this.activeData = this.themeForm;
                this.loadInitalData();
            });
        },
        async loadInitalData() {
            for (let i = 0; i < this.themeForm.globalData.length; i++) {
                let tmpoption = this.themeForm.globalData[i];
                let ddtype = tmpoption.dataSourceType;
                if (ddtype === "url") {
                    let rawData = await confirmValue(tmpoption, this.drawingList,this.themeForm.globalData);
                    tmpoption.rawData = JSON.stringify(rawData);
                    this.$set(this.themeForm.globalData, i, tmpoption);
                } else if (ddtype === "database") {
                    let rowGlobal = await getbaseData(tmpoption, this.themeForm.globalData);
                    rowGlobal.rawData = JSON.stringify(rowGlobal.rawData);
                    this.$set(this.themeForm.globalData, i, rowGlobal);
                } else if (ddtype === "combination") {
                    let tableColum = await combinationTableColum(
                        tmpoption,
                        this.themeForm.globalData,
                        this.drawingList
                    );
                    tmpoption.combinationTable = tableColum;
                    let rawData = combinationConfirmValue(tmpoption,this.themeForm.globalData);
                    tmpoption.rawData = JSON.stringify(rawData);
                    this.$set(this.themeForm.globalData, i, tmpoption);
                }
            }
            this.initSheet()
            this.$refs['information'].tableData = [...this.searchTableData];
            this.loadingQuery.close();
        },
        initSheet(toLead) {
            let that = this;
            let luckysheetData = toLead ? toLead : this.form.DrawOption !== '[]' ? JSON.parse(this.form.DrawOption) : [this.luckysheetOldData]
            luckysheet.create({
                container: 'luckysheet', // DOM容器的ID
                lang: 'zh', // 设定表格的语言
                data: luckysheetData,
                showtoolbarConfig: {
                    print: false,  // 工具栏隐藏打印按钮
                },
                showsheetbarConfig: {
                    add: true, // 底部sheet页隐藏新增sheet按钮
                    menu: false, // 底部sheet页隐藏管理按钮
                },
                sheetRightClickConfig: {
                    hide: false, // 隐藏，取消隐藏
                    move: false, // 向左移，向右移
                },
                hook: {
                    cellMousedown: function (cell, postion, sheetFile, ctx) {
                        that.cellMousedown(postion, cell)
                        
                    },
                    cellDragStop:  function (cell, postion, sheet, ctx, event) {
                        that.cellDragStop(postion)
                    },
                    sheetCreateAfter: function(sheet) {
                        sheet.sheet.defaultRowHeight = 30
                        sheet.sheet.defaultColWidth = 120
                        sheet.sheet.row = 80
                    }
                }
            })
            this.cellData.rowHeight = luckysheet.getRowHeight([this.cellData.r])[0]
            this.cellData.columnWidth = luckysheet.getColumnWidth([this.cellData.c])[0]
        },
        // 点击工作表名称修改
        workbookNameClick() {
            this.isWorkbookName = !this.isWorkbookName;
            this.$nextTick((_) => {
                this.$refs.inputRef.focus();
            })
        },
        // 监听单元格点击事件
        cellMousedown(postion, cell) {
            this.cellData.c = postion.c;
            this.cellData.r = postion.r;
            this.cellData.value = cell ? cell.v : '';
            this.extractWithoutPlaceholders(cell)
        },
        // 识别字符串里有${ } 单元格并做设置
        extractWithoutPlaceholders(cell) {
            if (cell === null) {
                this.cellData.isGroupSetting = false
                return false
            }
            if (cell.v.includes('${')) {
                this.cellData.polymerization = cell.polymerization;
                this.cellData.sort = cell.sort;
                this.cellData.extend = cell.extend;
                this.cellData.isGroupSetting = true
            } else {
                this.cellData.isGroupSetting = false
            }
        },
        // 基本信息回显数据
        renewalData(cellData) {
            let cellAttr = {
                'v': cellData.value,
                'polymerization': cellData.polymerization,
                'sort': cellData.sort,
                'extend': cellData.extend
            }
            luckysheet.setCellValue(cellData.r, cellData.c, cellAttr);
            let Height = {}, Width = {}
            if (cellData.rowHeight !=='') {
                Height[cellData.r] = cellData.rowHeight
                luckysheet.setRowHeight(Height);
            }
            if (cellData.columnWidth !=='') {
                Width[cellData.c] = cellData.columnWidth
                luckysheet.setColumnWidth(Width);
            }
        },
        // 数据集字段回显数据
        fieldData(data) {
            this.fieldValue = '${' + data.fieldName + '}';
        },
        // 单元格����结束后回显数据
        cellDragStop(postion) {
            let cellAttr = {
                'v': this.fieldValue,
                'polymerization': 'list',
                'sort': 'default',
                'extend': 'vertical'
            }
            luckysheet.setCellValue(postion.r, postion.c, cellAttr);
        },
        getKeyList(data, type, filtrationId) {
            this.keyList = data
            this.globalIndex = type
            this.themeForm.globalData[type].filtrationId = filtrationId
        },
        // 获取配置搜索信息
        getSearchData(data) {
            this.searchTableData = data
        },
        // 保存表格数据
        saveSheet() {
            let luckysheetData = luckysheet.getAllSheets();
            this.form.Name = this.workbookName;
            this.form.DrawOption = JSON.stringify(luckysheetData);
            this.themeForm.searchTableData = this.searchTableData;
            this.themeForm.globalIndex = this.globalIndex;
            this.themeForm.creationUser = this.creationUser;
            if (this.themeForm.globalData) {
                for (let i = 0; i < this.themeForm.globalData.length; i++) {
                    let rawData = JSON.parse(this.themeForm.globalData[i].rawData);
                    for (let j = 0; j < rawData.length; j++) {
                        rawData[j].content = [];
                    }
                    this.themeForm.globalData[i].rawData = JSON.stringify(rawData);
                }
            }
            console.log(this.themeForm)
            this.form.ThemeOption = JSON.stringify(this.themeForm);
            editRpt(this.form).then((response) => {
              this.$message({ message: "修改成功", type: "success" });
              this.$store.dispatch("tagsView/delView", this.$route);
              this.$router.go(-1);
            }).catch(err=>{});
        },
        // 表格预览
        previewSheet() {
            localStorage.removeItem('viewdataSheet');
            let luckysheetData = luckysheet.getAllSheets();
            let viewData = {
                globalData: this.themeForm.globalData,
                globalIndex: this.globalIndex,
                luckysheetData: luckysheetData,
                creationUser: this.creationUser,
                searchTableData: this.searchTableData,
                workbookName: this.workbookName
            };
            localStorage.setItem("viewdataSheet", JSON.stringify(viewData));
            const viewRuter = this.$router.resolve({
                path: "/report/spreadSheet/viewDataReport",
            });
            window.open(viewRuter.href, "_blank");
        },
        // 上传excel
        beforeUpload(file) {
            let that = this;
            const types = file.name.split(".")[1];
            const fileType = ["xlsx"].some(item => item === types);
            if (!fileType) {
                that.$message({ message: "请上传.xlsx格式的表格", type: "error" });
                return false;
            }
            LuckyExcel.transformExcelToLucky(
                file,
                function(exportJson, luckysheetfile){
                    that.$nextTick(() => {
                        window.luckysheet.destroy();
                        exportJson.sheets[0].defaultRowHeight = 30,
                        exportJson.sheets[0].defaultColWidth = 120,
                        that.initSheet(exportJson.sheets)
                    })
                }
            )
        }
    }
}
</script>
<style lang="scss" scoped>
::v-deep{
    .workbookNameInput{
        background-color: #f5f6f7;
        border: 1px solid #1e6fff;
    }
}
.basic-cont-box{
    width: 100%;
    position: relative;
    height: 100%;
    box-sizing: border-box;
}
.report-design-page {
    width: 100%;
    height: 100%;
}
.designer-header{
    min-width: 1250px;
    height: 56px;
    padding: 0 24px;
    box-sizing: border-box;
    display: flex;
    align-items: center;
    justify-content: space-between;
    border-bottom: 1px solid #eeeff0;
}
.design-name-box{
    display: flex;
    flex: 1;
    align-items: center;
}
.designer-header .design-name-box .name-input-box{
    border: 1px solid transparent;
    box-sizing: border-box;
    border-radius: 4px;
    overflow: hidden;
    margin-right: 8px;
    display: flex;
    align-items: center;
}
.designer-header .design-name-box .name-input-box .info-name{
    color: #363b4c;
    font-size: 16px;
    cursor: pointer;
}
.designer-header .action-box{
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: flex-end;
}
.report-design-content{
    height: calc(100% - 57px);
    background-color: #f5f6f7;
}
.design-page {
    display: flex;
    width: 100%;
    height: 100%;
}
.design-page .design-left{
    width: calc(100% - 472px);
    height: 100%;
    background-color: #f5f6f7;
}
.lucky-sheet-page{
    position: relative;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    overflow: hidden;
}
.luckysheet-wrap {
    padding: 0;
    position: absolute;
    width: 100%;
    height: calc(100% + -0px);
    left: 0;
    top: 0;
    bottom: 0;
}
.design-right{
    width: 472px;
    max-width: 472px;
    height: 100%;
    background-color: #fff;
}
.com-setting-page{
    width: 100%;
    height: calc(100% + -0px);
    display: flex;
}
</style>