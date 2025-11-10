<template>
     <div>
        <div style="margin-top: -10px">
            <el-button type="text" @click="addItem">+ 添加</el-button>
        </div>
        <el-tabs v-model="staticcurrentTab" class="center-tabs">
            <el-tab-pane label="y轴数据" name="yAxisData">
                <!-- <el-scrollbar class="right-scrollbar"> -->
                    <div style="max-height:300px;overflow-y:scroll;margin-top:10px">
                        <div v-for="(tmpitem, index) in configData.chartOption.staticDataValue[0].yAxisData" :key="'a' + index" style="margin-bottom: 10px">
                        <el-input
                        placeholder="请输入"
                        size="small"
                        v-model="configData.chartOption.staticDataValue[0].yAxisData[index]"
                        style="width: calc(100% - 38px)"
                        />
                        <el-button
                        style="margin-left: 10px"
                        size="mini"
                        @click="delItem(index)"
                        type="danger"
                        icon="el-icon-delete"
                        circle
                        ></el-button>
                        </div>
                    </div>
                <!-- </el-scrollbar> -->
            </el-tab-pane>
            <el-tab-pane label="x轴数据" name="xAxisData">
                <!-- <el-scrollbar class="right-scrollbar"> -->
                    <div style="max-height:500px;overflow-y:scroll;margin-top:10px">
                        <div v-for="(tmpitem, index) in configData.chartOption.staticDataValue[0].xAxisData" :key="'a' + index" style="margin-bottom: 10px">
                        <el-input
                        placeholder="请输入"
                        size="small"
                        v-model="configData.chartOption.staticDataValue[0].xAxisData[index]"
                        style="width: calc(100% - 38px)"
                        />
                        <el-button
                        style="margin-left: 10px"
                        size="mini"
                        @click="delItem(index)"
                        type="danger"
                        icon="el-icon-delete"
                        circle
                        ></el-button>
                        </div>
                    </div>
                <!-- </el-scrollbar> -->
            </el-tab-pane>
            <el-tab-pane label="数据" name="data">
                <!-- <el-scrollbar class="right-scrollbar"> -->
                    <el-table border :data="configData.chartOption.staticDataValue[0].data" max-height="800" style="margin-top: 10px;"> 
                        <el-table-column label="y轴序号" align="center" :show-overflow-tooltip="true">
                            <template slot-scope="scope">
                                <el-input v-model="scope.row[0]" placeholder="请输入" @input="inputYdata(scope.$index,0,$event)"/>
                            </template>
                        </el-table-column>
                        <el-table-column label="x轴序号" align="center" :show-overflow-tooltip="true">
                            <template slot-scope="scope">
                                <el-input v-model="scope.row[1]" placeholder="请输入" @input="inputYdata(scope.$index,1,$event)"/>
                            </template>
                        </el-table-column>
                        <el-table-column label="值" align="center" :show-overflow-tooltip="true">
                            <template slot-scope="scope">
                                <el-input v-model="scope.row[2]" placeholder="请输入" @input="inputYdata(scope.$index,2,$event)" />
                            </template>
                        </el-table-column>
                        <el-table-column fixed="right" label="操作" width="80px" align="center">
                            <template v-slot="scope">
                                <el-button size="mini" type="text" icon="el-icon-delete" style="color:red;" @click="remove(scope.row)">删除</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                <!-- </el-scrollbar> -->
            </el-tab-pane>
        </el-tabs>
        
     </div>
</template>
<script>
export default {
    props: {
        costomData: {
            type: Object
        }
    },
    data() {
        return {
            staticcurrentTab:'yAxisData',
            configData:this.costomData
        }
    },
    watch: {
        configData: {
            deep: true,
            handler(newVal,oldVal) {
                this.$emit("costom-change", newVal);
            }
        },
        costomData: {
            deep: true,
            handler(newVal) {
                this.configData = newVal;
            }
        },
    },
    mounted(){
    },
    methods: {
        inputYdata(inx,ix,val){
            this.configData.chartOption.staticDataValue[0].data[inx][ix]=Number(val)
        },
        remove(row) {
            if(this.staticcurrentTab=='yAxisData'){
                this.configData.chartOption.staticDataValue[0].yAxisData.splice(row,1)
            }else if(this.staticcurrentTab=='xAxisData'){
                this.configData.chartOption.staticDataValue[0].xAxisData.splice(row,1)
            }else  if(this.staticcurrentTab=='data'){
                this.configData.chartOption.staticDataValue[0].data.splice(this.configData.chartOption.staticDataValue.indexOf(row), 1);
            }
        },
        addItem() {
            if(this.staticcurrentTab=='yAxisData'){
                this.configData.chartOption.staticDataValue[0].yAxisData.push('')
            }else if(this.staticcurrentTab=='xAxisData'){
                this.configData.chartOption.staticDataValue[0].xAxisData.push('')
            }else  if(this.staticcurrentTab=='data'){
                this.configData.chartOption.staticDataValue[0].data.push([0,0,0]);
            }
        }
    }
}
</script>
<style lang="scss" scoped>
::v-deep .el-scrollbar__wrap{
  height: 100%;
}
/**  滚动条凹槽的颜色，还可以设置边框属性  **/
::-webkit-scrollbar-track-piece {
  background-color: #f8f8f8;
  border-radius: 10px;
}
/** 滚动条的宽度  **/
::-webkit-scrollbar {
  width: 9px;

  height: 9px;
}
/** 滚动条的设置  **/
::-webkit-scrollbar-thumb {
  background-color: #dddddd;

  background-clip: padding-box;

  min-height: 28px;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb:hover {
  background-color: #bbb;
}
</style>