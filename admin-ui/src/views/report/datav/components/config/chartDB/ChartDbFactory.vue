<template>
  <div>
    <el-dialog title="数据库连接" v-if="modalShow" :visible.sync="modalShow" width="1800px" hight="800px" :before-close="modalClose">
        <div class="container" v-loading="loading">
            <div class="left-board">
                <el-scrollbar class="left-scrollbar">
                    <div class="components-list">
                        <draggable
                            class="components-draggable"
                            :list="tableList"
                            :group="{ name: 'componentsGroup', pull: 'clone', put: false }"
                            :clone="cloneComponent"
                            draggable=".components-item"
                            :sort="false"
                            @end="onEnd"
                        >
                            <div
                            v-for="(element, index) in tableList" :key="index" class="components-item"
                            @click="addComponent(element)"
                            >
                            <div class="components-body">
                                {{ element.table_name }}
                            </div>
                            </div>
                        </draggable>
                    </div>
                </el-scrollbar>
            </div>
            <div class="center-board">
                
                
                    <div class="center-board-row">
                        <div style="height: 380px; overflow-y:scroll;border: 1px dashed #000">

                            <draggable class="drawing-board" :list="drawingList" :animation="340" group="componentsGroup">
                            
                                <block  v-for="(element, index) in drawingList" 
                                        :key="index" 
                                        :element="element" 
                                        :sqlMap = "sqlMap"
                                        :style="{ width: 100/blockNum + '%'}">
                                </block>
 
                            </draggable>

                            <div v-show="!drawingList.length" class="empty-info">
                            从左侧拖入或点选组件进行表单设计
                            </div>
                        </div>
                        <div style="height: 308px; border: 1px dashed #000">
                        
                        </div>
                    </div>
                
            </div>
          <el-scrollbar class="right-board">
          <div>
            <el-form size="small" label-width="90px">
              <el-form-item  label="主表">
                  <el-select v-model="masterTable" placeholder="请选择">
                    <el-option
                      v-for="(item,index) in drawingList"
                      :key="index"
                      :label="item.tableName"
                      :value="item.tableName">
                    </el-option>
                  </el-select>
              </el-form-item>

              <!-- <el-form-item  label="从表">
                  <el-select v-model="slaveTable" multiple placeholder="请选择">
                    <el-option
                      v-for="(item,index) in drawingList"
                      :key="index"
                      :label="item.tableName"
                      :value="item.tableName">
                    </el-option>
                  </el-select>
              </el-form-item> -->
            
                <draggable             
                  :animation="340"
                  group="selectItem"
                  handle=".option-drag"
                >  
                  <div style="display:flex">
                    <el-form-item label="从表表名"></el-form-item>
                    <el-form-item label="从表别名" style="margin-left: 90px;"></el-form-item>
                  </div>
                
                
                  <div v-for="(item, index) in slaveTable" :key="index" class="select-item">
                    
                      
                      <el-select v-model="item.name" placeholder="请选择" size="small" style="width:180px">
                        <el-option
                          v-for="(slave,slaveindex) in drawingList"
                          :key="slaveindex"
                          :label="slave.tableName"
                          :value="slave.tableName">
                        </el-option>
                      </el-select>
                      <el-input v-model="item.alias" placeholder="别名" size="small" style="width:100px"/>
                    
                    <div class="close-btn select-line-icon" @click="removeSelectItem(index)">
                      <i class="el-icon-remove-outline" />
                    </div>
                  </div>
                </draggable>
                <div style="margin-left: 20px;">
                  <el-button style="padding-bottom: 0" icon="el-icon-circle-plus-outline" type="text" @click="addSelectItem">
                    添加列
                  </el-button>
                </div> 

                <el-form-item  label="">
                </el-form-item>

                <el-form-item  label="表关联">
                    <el-select v-model="sqlJoin" placeholder="请选择">
                      <el-option label="内关联" value="INNER JOIN" ></el-option>
                      <el-option label="左关联" value="LEFT JOIN" ></el-option>
                      <el-option label="右关联" value="RIGHT JOIN" ></el-option>
                      <el-option label="全关联" value="FULL JOIN" ></el-option>
                    </el-select>
                </el-form-item>

              <el-form-item v-show=" sqlJoin != ''" label="关联片段" prop="desc">
                <el-input type="textarea" :rows="6" v-model="joinFragment" ></el-input>
              </el-form-item>

              <el-form-item  label="条件片段" prop="desc">
                <el-input type="textarea" :rows="6" v-model="conditionFragment" ></el-input>
              </el-form-item>

              <el-form-item  label="sql语句" prop="desc">
                <el-input type="textarea" :rows="6" v-model="sql" ></el-input>
              </el-form-item>

              <div style="float:right">
                <button type="button" class="el-button el-button--primary el-button--medium" @click="createSql">确 定</button> 
                <button type="button" class="el-button el-button--success el-button--medium">执行</button>
              </div>

            </el-form>
           
          </div>
          </el-scrollbar>
        </div>
    </el-dialog>
  </div>
</template>

<script>

import { getAllTable, getAllField } from '@/api/report/chartDB'
import draggable from 'vuedraggable'
import Block from './Block'

export default {
    props: ["database"],
    components: {
        draggable,
        Block
    },
    data() {
        return {
            loading: true,
            modalShow: false,
            dbParam: this.database,
            tableList: [],
            drawingList: [],
            activeObj: null,
            blockNum: 0,
            masterTable: '',
            slaveTable: [],
            sqlJoin: '',
            joinFragment: '',
            conditionFragment: '',
            sql:'',
            sqlMap: new Map()
        }
    },
    methods: {
        init() {
            this.modalShow = true;
            console.log(this.dbParam);
            //连接数据库
            getAllTable(this.dbParam).then(response => {
                if (response.code == 200) {
                    //alert("连接数据库成功");
                    this.$message({
                        message: '连接数据库成功',
                        type: 'success'
                    });
                    console.log(response.data);
                    this.tableList = response.data;
                    this.loading = false;
                } else {
                    this.$message.error('连接数据库失败');
                }
            });
        },
        modalClose() {
            this.modalShow = false;
        },
        cloneComponent(origin) {
            //alert("cloneComponent");
            console.log(origin);
            const clone = JSON.parse(JSON.stringify(origin));
            this.dbParam.tableName = clone.table_name;
            console.log(this.dbParam);
            //获取数据库下所有字段
            getAllField(this.dbParam).then(response => {
                if (response.code == 200) {
                    //alert("连接数据库成功");
                    // this.$message({
                    //     message: '连接数据库成功',
                    //     type: 'success'
                    // });
                    //console.log(response.data);
                    let obj = new Object();
                    obj.tableName = clone.table_name;
                    let treeData = [];
                    for (const item of response.data) {
                      let o = new Object();
                      o.id = item.columnName;
                      o.label = item.columnName + "( " + item.comment + " )";
                      treeData.push(o);
                    }
                    obj.tableData = treeData;
                    this.activeObj = obj;
                    this.drawingList.push(obj);
                } else {
                    this.$message.error('获取数据库下所有字段失败');
                }
            });
        },
        addComponent(item) {
            //alert("addComponent");
            console.log(item);
            // this.$message({
            //     message: '连接数据库成功',
            //     type: 'success'
            // });
        },
        onEnd() {
            //alert("onEnd");
            console.log("onEnd");
            console.log(this.activeObj);
            //this.drawingList.push(this.activeObj);
            this.blockNum ++;
        },
        addSelectItem(){
          this.slaveTable.push({
            name: '',
            alias: ''
          })
        },
        removeSelectItem(index){
         
          this.slaveTable.splice(index, 1);
        
        },
        createSql(){
          console.log(this.sqlMap);
          let str = 'SELECT ';
          if(this.masterTable != ''){
            this.sqlMap.get(this.masterTable).forEach( (value,index) => {
               str = str + this.masterTable +'.'+ value + ","
            })
          }
          // if(this.slaveTable != ''){
          //   this.slaveTable.forEach((key) => {
          //     this.sqlMap.get(key).forEach( (value,index) => {
          //      str = str + key +'.'+ value + " ,"
          //     })
          //   })
          // }
          str = str.substring(0,str.length-1) + " FROM " + this.masterTable + " AS " + this.masterTable;

          
          str = str + " " + this.joinFragment + " " + this.conditionFragment;

          this.sql = str;
          
        }
    }
}
</script>

<style lang='scss' scoped>
body, html{
  margin: 0;
  padding: 0;
  background: #fff;
  -moz-osx-font-smoothing: grayscale;
  -webkit-font-smoothing: antialiased;
  text-rendering: optimizeLegibility;
  font-family: -apple-system,BlinkMacSystemFont,Segoe UI,Helvetica,Arial,sans-serif,Apple Color Emoji,Segoe UI Emoji;
}

input, textarea{
  font-family: -apple-system,BlinkMacSystemFont,Segoe UI,Helvetica,Arial,sans-serif,Apple Color Emoji,Segoe UI Emoji;
}

.editor-tabs{
  background: #121315;
  .el-tabs__header{
    margin: 0;
    border-bottom-color: #121315;
    .el-tabs__nav{
      border-color: #121315;
    }
  }
  .el-tabs__item{
    height: 32px;
    line-height: 32px;
    color: #888a8e;
    border-left: 1px solid #121315 !important;
    background: #363636;
    margin-right: 5px;
    user-select: none;
  }
  .el-tabs__item.is-active{
    background: #1e1e1e;
    border-bottom-color: #1e1e1e!important;
    color: #fff;
  }
  .el-icon-edit{
    color: #f1fa8c;
  }
  .el-icon-document{
    color: #a95812;
  }
}

// home
.right-scrollbar {
  .el-scrollbar__view {
    padding: 12px 18px 15px 15px;
  }
}
.left-scrollbar .el-scrollbar__wrap {
  box-sizing: border-box;
  overflow-x: hidden !important;
  margin-bottom: 0 !important;
}
.center-tabs{
  .el-tabs__header{
    margin-bottom: 0!important;
  }
  .el-tabs__item{
    width: 50%;
    text-align: center;
  }
  .el-tabs__nav{
    width: 100%;
  }
}
.reg-item{
  padding: 12px 6px;
  background: #f8f8f8;
  position: relative;
  border-radius: 4px;
  .close-btn{
    position: absolute;
    right: -6px;
    top: -6px;
    display: block;
    width: 16px;
    height: 16px;
    line-height: 16px;
    background: rgba(0, 0, 0, 0.2);
    border-radius: 50%;
    color: #fff;
    text-align: center;
    z-index: 1;
    cursor: pointer;
    font-size: 12px;
    &:hover{
      background: rgba(210, 23, 23, 0.5)
    }
  }
  & + .reg-item{
    margin-top: 18px;
  }
}
.action-bar{
  & .el-button+.el-button {
    margin-left: 15px;
  }
  & i {
    font-size: 20px;
    vertical-align: middle;
    position: relative;
    top: -1px;
  }
}

.custom-tree-node{
  width: 100%;
  font-size: 14px;
  .node-operation{
    float: right;
  }
  i[class*="el-icon"] + i[class*="el-icon"]{
    margin-left: 6px;
  }
  .el-icon-plus{
    color: #409EFF;
  }
  .el-icon-delete{
    color: #157a0c;
  }
}

.left-scrollbar .el-scrollbar__view{
  overflow-x: hidden;
}

.el-rate{
  display: inline-block;
  vertical-align: text-top;
}
.el-upload__tip{
  line-height: 1.2;
}

$selectedColor: #f6f7ff;
$lighterBlue: #409EFF;

.container {
  position: relative;
  width: 100%;
  height: 700px;
}

.components-list {
  padding: 8px;
  box-sizing: border-box;
  height: 100%;
  .components-item {
    display: inline-block;
    width: 48%;
    margin: 1%;
    transition: transform 0ms !important;
  }
}
.components-draggable{
  padding-bottom: 20px;
}
.components-title{
  font-size: 14px;
  color: #222;
  margin: 6px 2px;
  .svg-icon{
    color: #666;
    font-size: 18px;
  }
}

.components-body {
  padding: 8px 10px;
  background: $selectedColor;
  font-size: 12px;
  cursor: move;
  border: 1px dashed $selectedColor;
  border-radius: 3px;
  .svg-icon{
    color: #777;
    font-size: 15px;
  }
  &:hover {
    border: 1px dashed #787be8;
    color: #787be8;
    .svg-icon {
      color: #787be8;
    }
  }
}

.left-board {
  width: 300px;
  position: absolute;
  left: 0;
  top: 0;
  height: 100%;
}
.left-scrollbar{
  height: 100%;
  overflow: hidden;
}
.center-scrollbar {
  height: 680px;
  overflow: hidden;
  border-left: 1px solid #f1e8e8;
  border-right: 1px solid #f1e8e8;
  box-sizing: border-box;
}
.center-board {
  height: 680px;
  width: auto;
  margin: 0 350px 0 300px;
  box-sizing: border-box;
}
.empty-info{
  position: absolute;
  top: 25%;
  left: 0;
  right: 0;
  text-align: center;
  font-size: 18px;
  color: #ccb1ea;
  letter-spacing: 4px;
}
.action-bar{
  position: relative;
  height: 42px;
  text-align: right;
  padding: 0 15px;
  box-sizing: border-box;;
  border: 1px solid #f1e8e8;
  border-top: none;
  border-left: none;
  .delete-btn{
    color: #F56C6C;
  }
}
.logo-wrapper{
  position: relative;
  height: 42px;
  background: #fff;
  border-bottom: 1px solid #f1e8e8;
  box-sizing: border-box;
}
.logo{
  position: absolute;
  left: 12px;
  top: 6px;
  line-height: 30px;
  color: #00afff;
  font-weight: 600;
  font-size: 17px;
  white-space: nowrap;
  > img{
    width: 30px;
    height: 30px;
    vertical-align: top;
  }
  .github{
    display: inline-block;
    vertical-align: sub;
    margin-left: 15px;
    > img{
      height: 22px;
    }
  }
}

.center-board-row {
  padding: 12px 12px 15px 12px;
  box-sizing: border-box;
  & > .el-form {
    // 69 = 12+15+42
    height: calc(100vh - 69px);
  }
}
.drawing-board {
  display: flex;
  height: 100%;
  position: relative;
  .components-body {
    padding: 0;
    margin: 0;
    font-size: 0;
  }
  .sortable-ghost {
    position: relative;
    display: block;
    overflow: hidden;
    &::before {
      content: " ";
      position: absolute;
      left: 0;
      right: 0;
      top: 0;
      height: 3px;
      background: rgb(89, 89, 223);
      z-index: 2;
    }
  }
  .components-item.sortable-ghost {
    width: 100%;
    height: 60px;
    background-color: $selectedColor;
  }
  .active-from-item {
    & > .el-form-item{
      background: $selectedColor;
      border-radius: 6px;
    }
    & > .drawing-item-copy, & > .drawing-item-delete{
      display: initial;
    }
    & > .component-name{
      color: $lighterBlue;
    }
  }
  .el-form-item{
    margin-bottom: 15px;
  }
}
.drawing-item{
  position: relative;
  cursor: move;
  &.unfocus-bordered:not(.activeFromItem) > div:first-child  {
    border: 1px dashed #ccc;
  }
  .el-form-item{
    padding: 12px 10px;
  }
}
.drawing-row-item{
  position: relative;
  cursor: move;
  box-sizing: border-box;
  border: 1px dashed #ccc;
  border-radius: 3px;
  padding: 0 2px;
  margin-bottom: 15px;
  .drawing-row-item {
    margin-bottom: 2px;
  }
  .el-col{
    margin-top: 22px;
  }
  .el-form-item{
    margin-bottom: 0;
  }
  .drag-wrapper{
    min-height: 80px;
  }
  &.active-from-item{
    border: 1px dashed $lighterBlue;
  }
  .component-name{
    position: absolute;
    top: 0;
    left: 0;
    font-size: 12px;
    color: #bbb;
    display: inline-block;
    padding: 0 6px;
  }
}
.drawing-item, .drawing-row-item{
  &:hover {
    & > .el-form-item{
      background: $selectedColor;
      border-radius: 6px;
    }
    & > .drawing-item-copy, & > .drawing-item-delete{
      display: initial;
    }
  }
  & > .drawing-item-copy, & > .drawing-item-delete{
    display: none;
    position: absolute;
    top: -10px;
    width: 22px;
    height: 22px;
    line-height: 22px;
    text-align: center;
    border-radius: 50%;
    font-size: 12px;
    border: 1px solid;
    cursor: pointer;
    z-index: 1;
  }
  & > .drawing-item-copy{
    right: 56px;
    border-color: $lighterBlue;
    color: $lighterBlue;
    background: #fff;
    &:hover{
      background: $lighterBlue;
      color: #fff;
    }
  }
  & > .drawing-item-delete{
    right: 24px;
    border-color: #F56C6C;
    color: #F56C6C;
    background: #fff;
    &:hover{
      background: #F56C6C;
      color: #fff;
    }
  }
}
.right-board {
  width: 300px;
  height: 100%;
  position: absolute;
    right: 0;
    top: 0;
}
.select-item {
  display: flex;
  border: 1px dashed #fff;
  box-sizing: border-box;
  & .close-btn {
    cursor: pointer;
    color: #f56c6c;
    margin-top: 8px;
  }
  & .el-input + .el-input {
    margin-left: 4px;
  }
}
</style>