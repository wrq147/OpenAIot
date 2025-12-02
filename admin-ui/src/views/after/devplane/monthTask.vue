<template>
  <div>
    <el-table ref="tableRef" v-loading="loading" :data="tbList" border :header-cell-style="cellSty" style="width:100%" row-key="keyId" :span-method="arraySpanMethod">
      <el-table-column label="设备名称" align="center" prop="DeviceName" width="160" key="DeviceName"></el-table-column>
      <el-table-column label="所属房间" align="center" key="RoomNames">
          <template slot-scope="scope" v-if="scope.row.RoomNames&&scope.row.RoomNames.length>0">
              <span>{{ scope.row.RoomNames.join(',') }}</span>
          </template>
      </el-table-column>
      <el-table-column :label="item" v-for="(item,index) in monthcolumn" :key="index+item" align="center" width="100">
          <template slot-scope="scope">
            <div class="block_div_con" v-if="hasShowblock(scope.row,item)">
              
              <el-tooltip class="item" effect="dark" :content="it.PlanName" placement="top-start" v-for="it in scope.row.TaskList2" :key="it.Id">
                <div class="block_div" :style="{'--bgcolor':setBgcolor(it.TaskStatus),'--width':setDivWidth(it,scope.row)+'px','--marginleft':setDivMargin(it,scope.row)+'px'}"></div>
              </el-tooltip>
            </div>
          </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
var dayjs = require('@/utils/day.js')
export default {
  name: 'AdminUiDayTask',
  props:['loading','tbList','cellSty','statusList'],
  data() {
    return {
      monthcolumn:[]
    };
  },

  mounted() {
    // this.getDayTask()
    this.setMonthcolumn()
    this.$nextTick(() => {
      this.$refs.tableRef.doLayout()
    })

  },

  methods: {
    setBgcolor(item){
      //设置状态背景颜色
      let obj=this.statusList.find(row=>row.val==item)
      if(obj){
        return obj.color
      }else{
        return '#ffffff'
      }
    },
    setDivWidth(row,item){
      if(row){
        let minTimeyear=dayjs(item.minTime).year()
        let startmonth=dayjs(row.StartOn).month()
        let startyear=dayjs(row.StartOn).year()
        let endtmonth=dayjs(row.EndOn).month()
        let endtyear=dayjs(row.EndOn).year()
        let num=0
        if(minTimeyear==startyear){
          if(endtyear==startyear){
            num=endtmonth-startmonth+1
          }else{
            num=31-startmonth+1
          }
          let wid=num*100-20
          return wid
        }else{//不在同一年同一月时距离取0
          return 0
        }
        
      }else{
        return 0
      }
    },
    setDivMargin(row,item){
      if(row){
        let startmonth=dayjs(row.StartOn).month()
        let startyear=dayjs(row.StartOn).year()
        let minTimetmonth=dayjs(item.minTime).month()
        let minTimetyear=dayjs(item.minTime).year()
        let num=0
        if(minTimetyear==startyear){
          num=startmonth-minTimetmonth
          let marg=num*100
          return marg
        }else{
          return 0
        }
        
      }else{
        return 0
      }
    },
    hasShowblock(row,index){
      //是否显示颜色块
      if(row.TaskList2&&row.TaskList2.length>0){
        let startmonth=dayjs(row.TaskList2[0].StartOn).month()
        let startyear=dayjs(row.TaskList2[0].StartOn).year()
        let endtmonth=dayjs(row.TaskList2[0].EndOn).month()
        let endtyear=dayjs(row.TaskList2[0].EndOn).year()
        let num=0
        if(endtyear==startyear){
          num=endtmonth-startmonth+1
        }else{
          num=11-startmonth+1
        }
        if(Number(index)-1>=startmonth&&Number(index)-1<startmonth+num){
          return true
        }else{
          return false
        }
      }else{
        return false
      }
    },
    arraySpanMethod({ row, column, rowIndex, columnIndex }){
      // console.log(row,column,rowIndex,columnIndex);
      if(row.TaskList2){
        let rowTask=JSON.parse(JSON.stringify(row.TaskList2))
        if(rowTask&&rowTask.length>0){
          let startmonth=dayjs(row.minTime).month()
          let startyear=dayjs(row.minTime).year()
          let endtmonth=dayjs(row.maxTime).month()
          let endtyear=dayjs(row.maxTime).year()
          let num=0
          if(endtyear==startyear){
            num=endtmonth-startmonth+1
          }else{
            num=11-startmonth+1
          }
          let startIndex=0
          startIndex=startmonth+2
          
          if(columnIndex==startIndex){
            console.log(columnIndex,startIndex,columnIndex==startIndex,num);
            if(row.index==0){
              return[Number(row.lenindex.length),num]
            }else{
              return[0,0]
            }
            // Number(row.lenindex.length)
          }else{
            if(columnIndex>startIndex&&columnIndex<startIndex+num){
              return[0,0]
            }else{
              if(columnIndex==0||columnIndex==1){
                if(row.index==0){
                  return[Number(row.lenindex.length),1]
                }else{
                  return[0,0]
                }
                
              }else{
                return[Number(row.lenindex.length),1]
              }
              
            }
          }
        }
      }
    },
    setMonthcolumn(){
      for(let i=1;i<=12;i++){
        this.monthcolumn.push(i.toString())
      }
    }
  },
};
</script>
<style lang="less" scoped>
.block_div_con{
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: flex-start;
  flex-direction: column;
}
.block_div{
  width: var(--width);
  height: 10px;
  background: var(--bgcolor);
  margin-bottom: 5px;
  box-sizing: border-box;
  margin-left: var(--marginleft);
}
.block_div:last-child{
  margin-bottom: 0;
}
// ::v-deep .el-table th{
//   display: table-cell !important;
// }
::v-deep .el-table th.gutter{
  display: table-cell !important;
}
::v-deep .el-table colgroup.gutter{
  display: table-cell !important;
}
</style>