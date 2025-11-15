<template>
    <div>
        <el-row :gutter="10">
          <el-col :span="24" :xs="24">
            <div class="elbiaoge_elform">
              <el-table :data="batchList" class="data_table"  style="width:100%">
                <el-table-column label="序号" align="center" width="50">
                  <template slot-scope="scope">
                    <div style="cursor: pointer;" @click="batchClick(scope.row, scope.$index)">{{ scope.$index + 1 }}</div>
                  </template>
                </el-table-column>
                <el-table-column v-for="(item, index) in batchList[0].ZhouQiShiDuan" :key="index" :label="item.ZhouQi" align="center" prop="TimeName" width="120">
                    <template slot-scope="scope">
                        <div class="cellBtn" @click="cellClick(index, scope.$index)">
                          <div class="cellBtn-child" v-for="(v, innerIndex) in getArrayData(batchList[scope.$index].ZhouQiShiDuan[index].ShiDuanID)" :key="innerIndex">
                            <el-tag closable @close="removeTag(index, scope.$index, v)">{{ getArrayData(batchList[scope.$index].ZhouQiShiDuan[index].ShiDuan)[innerIndex] }}</el-tag>
                          </div>
                        </div>
                    </template>
                </el-table-column>
              </el-table>
            </div>
          </el-col>
        </el-row>
      </div>
</template>
<script>
import { classesList, editClassesTime, editClassesTimeMore } from "@/api/scheduling/timeClasses";
  export default {
    name: "Index",
    props: {
        timeData: {
            type: Object
        },
        ruleForm: {
            type: Object
        }
    },
    data() {
      return {
        batchList: JSON.parse(JSON.stringify(this.ruleForm.shiDuan)),
      };
    },
    methods: {
      // 字符串转数组
      getArrayData(data) {
        if (data) {
          return data.split(',')
        } else {
          return [];
        }
      },
      // 获取班次数组列表
      // getClassesList(id) {
      //   classesList().then(response => {
      //     const newDate = response.data.List.filter(item => item.Id === this.ruleForm.banCiID);
      //     this.batchList = newDate[0].ShiDuan;
      //   })
      // },
      // 点击序号设置一行
      batchClick(item) {
        if (Object.keys(this.timeData).length > 0) {
          const newId = this.timeData.Id;
          const newName = this.timeData.TimeName;
          let hasUpdates = false; // 标记是否有更新
          for (let i = 0; i < item.ZhouQiShiDuan.length; i++) {
            const currentCell = item.ZhouQiShiDuan[i];
            
            // 检查是否已存在相同的 ID
            if (currentCell.ShiDuanID && currentCell.ShiDuanID.split(',').includes(newId)) {
              continue; // 跳过已存在的单元格
            }
            
            // 合并 ID 和名称
            let shiDuanIdValue = newId;
            let shiDuanValue = newName;
            
            if (currentCell.ShiDuanID) {
              shiDuanIdValue = `${currentCell.ShiDuanID},${newId}`;
              shiDuanValue = `${currentCell.ShiDuan},${newName}`;
            }
            
            // 更新单元格数据
            currentCell.ShiDuan = shiDuanValue;
            currentCell.ShiDuanID = shiDuanIdValue;
            hasUpdates = true;
          }
          
          // 只有当有更新时才触发事件
          if (hasUpdates) {
            this.$emit('getSetting', this.batchList);
          } else {
            this.$message.warning('所选时段已全部存在');
          }
        } else {
          this.$message.error('请选择时段')
          return false;
        }
      },
      // 点击单元格单独设置
      cellClick(columnIndex, rowIndex) {
        if (Object.keys(this.timeData).length > 0) {
          const currentCell = this.batchList[rowIndex].ZhouQiShiDuan[columnIndex];
          const newId = this.timeData.Id;
          const newName = this.timeData.TimeName;
          // 检查是否已存在相同的 ID
          if (currentCell.ShiDuanID && currentCell.ShiDuanID.split(',').includes(newId)) {
            this.$message.warning('该时段已存在');
            return;
          }
          // 合并 ID 和名称
          let shiDuanIdValue = newId;
          let shiDuanValue = newName;
          
          if (currentCell.ShiDuanID) {
            shiDuanIdValue = `${currentCell.ShiDuanID},${newId}`;
            shiDuanValue = `${currentCell.ShiDuan},${newName}`;
          }
    
          // 更新单元格数据
          currentCell.ShiDuan = shiDuanValue;
          currentCell.ShiDuanID = shiDuanIdValue;
    
          // 通知父组件数据变更
          this.$emit('getSetting', this.batchList);
        } else {
          this.$message.error('请选择时段')
          return false;
        }
      },
      // 删除时段信息标签
      removeTag(index, rowIndex, idToRemove) {
        const cell = this.batchList[rowIndex].ZhouQiShiDuan[index];
        if (cell.ShiDuanID) {
          const shiDuanArray = cell.ShiDuan.split(',');
          const shiDuanIdArray = cell.ShiDuanID.split(',');
          
          // 找到要删除的ID在数组中的位置
          const idIndex = shiDuanIdArray.findIndex(id => id === idToRemove);
          
          if (idIndex !== -1) {
            // 同步删除名称和ID
            shiDuanArray.splice(idIndex, 1);
            shiDuanIdArray.splice(idIndex, 1);
            
            cell.ShiDuan = shiDuanArray.join(',');
            cell.ShiDuanID = shiDuanIdArray.join(',');
            
            this.$emit('getSetting', this.batchList);
          }
        }
      }
    }
  };
  </script>
  <style lang="scss" scoped>
   .cellBtn{
      padding: 5px 8px;
      border: 1px solid #dcdfe6;
      box-sizing: border-box; 
      cursor: pointer;
      border-radius: 4px;
    }
    .cellBtn-child{
      height: 100%;
      width: 100%;
      margin-bottom: 5px;
    }
    .cellBtn-child:last-child{
      margin-bottom: 0;
    }
  </style>