<template>
    <div class="com-setting-left">
        <el-scrollbar style="height: 100%;">
            <div class="com-setting-item">
                <div class="title">基本信息</div>
                <div class="set-item base-item">
                    <div class="name">坐标</div>
                    <div class="value-input dis">{{ cellData.r }}, {{ cellData.c }}</div>
                </div>
                <div class="set-item base-item">
                    <div class="name">值</div>
                    <div class="value-input">
                        <el-input v-model="cellData.value" @change="inputChange" />
                    </div>
                </div>
                <div class="set-item base-item">
                    <div class="name">列宽</div>
                    <div class="value-input">
                        <el-input v-model="cellData.columnWidth" @change="inputChange" />
                    </div>
                </div>
                <div class="set-item base-item">
                    <div class="name">列高</div>
                    <div class="value-input">
                        <el-input v-model="cellData.rowHeight" @change="inputChange" />
                    </div>
                </div>
            </div>
            <div v-if="cellData.isGroupSetting" class="com-setting-item">
                <div class="title">分组设置</div>
                <div class="set-item">
                    <div class="name">聚合方式</div>
                    <div class="value-input">
                        <el-select v-model="cellData.polymerization" @change="inputChange">
                            <el-option label="列表" value="list" />
                            <el-option label="分组" value="group" />
                        </el-select>
                    </div>
                </div>
                <div class="set-item">
                    <div class="name">排序方式</div>
                    <div class="value-input">
                        <el-select v-model="cellData.sort" @change="inputChange">
                            <el-option label="默认" value="default" />
                            <el-option label="正序" value="forward" />
                            <el-option label="倒序" value="reverse" />
                        </el-select>
                    </div>
                </div>
                <div class="set-item">
                    <div class="name">扩展方式</div>
                    <div class="value-input">
                        <el-select v-model="cellData.extend" @change="inputChange">
                            <el-option label="竖向" value="vertical" />
                            <el-option label="横向" value="horizontal" />
                        </el-select>
                    </div>
                </div>
            </div>
            <div class="com-setting-item">
                <div class="title">查询条件</div>
                <div class="search-button" @click="isDialogVisible = true">配置查询条件</div>
            </div>
        </el-scrollbar>
        <!-- 配置查询条件 -->
        <condition-search ref="conditionSearch" :tableData="tableData" :dialog-visible="isDialogVisible" @cancelForm="cancelForm" @submitForm="submitForm" />
    </div>
</template>
<script>
import conditionSearch from "./conditionSearch";
export default { 
  name: 'information',
  props: {
    cellData: {
        type: Object,
        required: true
    },
    themeForm: {
        type: Object,
        required: true
    },
    keyList: {
        type: Array,
        required: true
    }
  },
  components: {
    conditionSearch
  },
  watch: {
    keyList(newVal, oldVal) {
        if (this.isInitialized) {
            this.disposeKeyList()
        } else {
            this.isInitialized = true;
        }
    }
  },
  data() {
    return {
        isDialogVisible: false,
        tableData: [],
        isInitialized: false
    }
  },
  methods: {
    disposeKeyList() {
        let Array = JSON.parse(JSON.stringify(this.keyList))
        this.tableData = []
        Array.forEach(item => {
            item.data.forEach(v => {
                v.openSearch = false;
                v.searchDefault = '';
                v.searchType = v.fieldType === '时间' ? 1 : 0;
                this.tableData.push(v)
            })
        })
    },
    submitForm(data) {
        this.tableData = data
        this.$emit('getSearchData', this.tableData)
        this.cancelForm()
    },
    inputChange() {
        this.$emit('renewalData', this.cellData)
    },
    cancelForm() {
        this.isDialogVisible = false
    }
  }
}
</script>
<style lang="scss" scoped>
::v-deep{
    .el-input__inner{
        background-color: #f5f6f7;
        border: none !important;
    }
    .el-scrollbar__bar {
        display: none;
    }
}
.com-setting-left{
    height: 100%;
    width: 236px;
}
.com-setting-item{
    padding: 16px;
    border-bottom: 1px solid #eeeff0; 
}
.com-setting-item .title{
    font-size: 14px;
    font-weight: bold;
    color: #363b4c;
    font-family: SourceHanSansCN-Bold !important;
    margin-bottom: 8px;
}
.set-item{
    margin-top: 8px;
    display: flex;
    align-items: center;
}
.set-item .name{
    width: 56px;
    min-width: 56px;
    font-size: 14px;
    color: #363b4c;
    margin-right: 8px;
}
.base-item .name{
    width: 28px;
    min-width: 28px;
    font-size: 14px;
    color: #363b4c;
    margin-right: 8px;
}
.set-item .dis{
    color: #6f7588;
    padding: 0 8px;
}
.set-item .value-input{
    height: 36px;
    background: #f5f6f7;
    border-radius: 4px;
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: space-between;
}
.com-setting-item .search-button{
    display: flex;
    cursor: pointer;
    align-items: center;
    justify-content: center;
    height: 36px;
    background: #f5f6f7;
    border-radius: 4px;
    color: #363b4c;
    font-size: 14px;
}

</style>