<template>
    <div class="databaseLeft">
        <div>
            <el-select class="selectChange" v-model="sourseId" placeholder="请选择数据源"
                @change="fetchChildrenData">
                <el-option v-for="item in sourseList" :key="item.Id" :label="item.DatabaseName" :value="item.Id" />
                <!-- <el-option label="SQLServer" value="sqlserver" /> -->
            </el-select>
            <el-tree :data="sourseTableList" :props="defaultProps" accordion @node-click="handleNodeClick"> 
                <span slot-scope="{ node, data }">
                    <span style="display: flex;align-items: center;">
                        <img v-if="data.TABLE_TYPE === 'TABLE'" src="@/assets/images/table-img.png" style="width: 16px; height: 16px; margin-right: 10px;" />
                        <img v-if="data.TABLE_TYPE === 'VIEW'" src="@/assets/images/look-img.png" style="width: 16px; height: 16px; margin-right: 10px;" />
                        <span>{{ node.label }} </span>
                    </span>
                </span>
            </el-tree>
        </div>
    </div>
</template>
<script>
import { listSourse, TableNames, TableStruct } from "@/api/report/sourse";
import addDataOrigin from "./addDataOrigin";
export default {
    components: {
        addDataOrigin
    },
    data() {
        return {
            // 查询参数
            queryParams: {
                pageNum: 1,
                pageSize: 9999,
                dataType: ''
            },
            sourseId: '',
            sourseList: [],
            sourseTableList: [
                { DatabaseName: '表', TABLE_TYPE: 'TABLE', children: [] },
                { DatabaseName: '视图', TABLE_TYPE: 'VIEW', children: [] }
            ],
            defaultProps: {
                children: 'children',
                label: 'DatabaseName'
            },
            dataOne: [], //tree记录第一层节点数据
        }
    },
    mounted() {
        this.getSourseList();
    },
    methods: {
        // 编辑时重置数据方法
        initCom(tmpoption) {
            const database = { ...tmpoption.database }
            this.sourseId =  database.sourseItem
            this.getSourseList(this.sourseId);
        },
        //获取数据源列表
        getSourseList(sourseId) {
            listSourse(this.queryParams).then((response) => {
                // for (const key in response.data.List) {
                //     response.data.List[key].children = []
                // }
                // console.log( response.data.List)
                this.sourseList = response.data.List;
                this.$emit('sourseDataList', this.sourseList);
                if (sourseId !== undefined) {
                    this.fetchChildrenData(sourseId)
                }
            });
        },
        // 获取数据库低下的表
        fetchChildrenData(id) {
            this.sourseTableList = [
                { DatabaseName: '表', TABLE_TYPE: 'TABLE', children: [] },
                { DatabaseName: '视图', TABLE_TYPE: 'VIEW', children: [] }
            ]
            this.$emit('sourseDataId', id);
            const data = this.sourseList.find(item => item.Id === id);
            let array = {
                type: data.DatabaseType,
                ipAdress: data.IpAddress,
                port: data.Port,
                baseName: data.DatabaseName,
                username: data.UserName,
                password: data.Password
            }
            TableNames(array).then(res => {
                for (const key in res.data) {
                    res.data[key].DatabaseName = res.data[key].TABLE_NAME
                    res.data[key].id = id
                    res.data[key].children = []
                    delete res.data[key].TABLE_NAME
                    if (res.data[key].TABLE_TYPE === 'TABLE') {
                        this.sourseTableList[0].children.push(res.data[key])
                    } else {
                        this.sourseTableList[1].children.push(res.data[key])
                    }
                }
            })
        },
        // 获取数据库低下的表的字段
        fetchChildrenTableStruct(list) {
            const data = this.sourseList.find(item => item.Id === list.id);
            let array = {
                type: data.DatabaseType,
                ipAdress: data.IpAddress,
                port: data.Port,
                baseName: data.DatabaseName,
                username: data.UserName,
                password: data.Password,
                tableName: list.DatabaseName
            }
            return new Promise((resolve, reject) => {
                TableStruct(array).then(res => {
                    for (const key in res.data) {
                        res.data[key].DatabaseName = res.data[key].COLUMN_NAME
                        delete res.data[key].COLUMN_NAME
                    }
                    const children = res.data;
                    resolve(children);
                })
            })
        },
        // 点击树节点
        handleNodeClick(data, node) {
            if (node.expanded) {
                return false
            }
            if (node.level === 2) {
                this.fetchChildrenTableStruct(data).then(childrenData => {
                    if (childrenData && childrenData.length) {
                        node.loading = false; // 取消加载动画
                        node.expanded = true; // 展开节点
                        data.children = childrenData; // 将子节点数据设置到树形控件的数据模型中
                    }
                }).catch(error => {
                    node.loading = false;
                });
            }
        }
    }
}
</script>
<style lang="scss" scoped>
.databaseLeft {
    width: 220px;
    background-color: #fff;
    padding: 15px;
    border-radius: 5px;
    box-shadow: 0 0 2px rgba(0, 0, 0, .1);
}
::v-deep{
    .el-tree-node{
        // max-height: 700px;
    }
    .selectChange{
        margin-top: 10px;
        height: 50px;
    }

    .selectChange .el-input--suffix,
    .el-input__inner {
        height: 30px;
    }

    .selectChange .el-input__icon {
        line-height: 30px;
    }
}
</style>