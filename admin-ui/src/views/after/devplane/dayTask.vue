<template>
  <div>
    <el-table ref="tableRef" v-loading="loading" :data="tbList" border :header-cell-style="cellSty" style="width:100%" row-key="keyId" :span-method="arraySpanMethod">
      <el-table-column label="设备名称" align="center" prop="DeviceName" width="160" key="DeviceName"></el-table-column>
      <el-table-column label="所属车间" align="center" key="RoomNames">
          <template slot-scope="scope" v-if="scope.row.RoomNames&&scope.row.RoomNames.length>0">
              <span>{{ scope.row.RoomNames.join(',') }}</span>
          </template>
      </el-table-column>
      <el-table-column :label="item" v-for="(item,index) in daycolumn" :key="index+item" align="center" width="40">
          <template slot-scope="scope">
            <div class="block_div_con" v-if="hasShowblock(scope.row,item)">
              <el-tooltip class="item" effect="dark" :content="it.PlanName" placement="top-start" v-for="it in scope.row.TaskList2" :key="it.Id">
                <div class="block_div" :style="{'--bgcolor':setBgcolor(it.TaskStatus),'--width':setDivWidth(it,scope.row)+'px','--marginleft':setDivMargin(it,scope.row)+'px'}"></div>
              </el-tooltip>
              
            </div>
          </template>
          <!-- v-for="(it,ix) in scope.row.lenindex" :key="ix" -->
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
      daycolumn:[]
    };
  },

  mounted() {
    // this.getDayTask()
    this.setDaycolumn()
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
        let minTimemonth=dayjs(item.minTime).month()
        let minTimeyear=dayjs(item.minTime).year()
        let startDay=dayjs(row.StartOn).date()
        let startmonth=dayjs(row.StartOn).month()
        let startyear=dayjs(row.StartOn).year()
        let endtDay=dayjs(row.EndOn).date()
        let endtmonth=dayjs(row.EndOn).month()
        let endtyear=dayjs(row.EndOn).year()
        let num=0
        if(minTimemonth==startmonth&&minTimeyear==startyear){
          if(startmonth==endtmonth&&endtyear==startyear){
            num=endtDay-startDay+1
          }else{
            num=31-startDay+1
          }
          let wid=num*40-20
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
        let startDay=dayjs(row.StartOn).date()
        let startmonth=dayjs(row.StartOn).month()
        let startyear=dayjs(row.StartOn).year()
        let minTimetDay=dayjs(item.minTime).date()
        let minTimetmonth=dayjs(item.minTime).month()
        let minTimetyear=dayjs(item.minTime).year()
        let num=0
        if(startmonth==minTimetmonth&&minTimetyear==startyear){
          num=startDay-minTimetDay
          let marg=num*40
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
        let startDay=dayjs(row.TaskList2[0].StartOn).date()
        let startmonth=dayjs(row.TaskList2[0].StartOn).month()
        let startyear=dayjs(row.TaskList2[0].StartOn).year()
        let endtDay=dayjs(row.TaskList2[0].EndOn).date()
        let endtmonth=dayjs(row.TaskList2[0].EndOn).month()
        let endtyear=dayjs(row.TaskList2[0].EndOn).year()
        let num=0
        if(startmonth==endtmonth&&endtyear==startyear){
          num=endtDay-startDay+1
        }else{
          num=31-startDay+1
        }
        if(Number(index)>=startDay&&Number(index)<startDay+num){
          return true
        }else{
          return false
        }
      }else{
        return false
      }
    },
    arraySpanMethod({ row, column, rowIndex, columnIndex }){
      if(row.TaskList2){
        let rowTask=JSON.parse(JSON.stringify(row.TaskList2))
        if(rowTask&&rowTask.length>0){
          let startDay=dayjs(row.minTime).date()
          let startmonth=dayjs(row.c).month()
          let startyear=dayjs(row.minTime).year()
          let endtDay=dayjs(row.maxTime).date()
          let endtmonth=dayjs(row.maxTime).month()
          let endtyear=dayjs(row.maxTime).year()
          let num=0
          if(startmonth==endtmonth&&endtyear==startyear){
            num=endtDay-startDay+1
          }else{
            num=31-startDay+1
          }
          let startIndex=0
          startIndex=startDay+1
          if(columnIndex==startIndex){
            if(row.index==0){
              return[Number(row.lenindex.length),num]
            }else{
              return[0,0]
            }
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
    setDaycolumn(){
      for(let i=1;i<=31;i++){
        this.daycolumn.push(i.toString())
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