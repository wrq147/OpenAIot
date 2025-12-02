<template>
    <el-dialog width="900px" title="请选择分配的设备" :visible.sync="roomOpen" append-to-body>
      <el-table ref="devRoomTable" :data="devRoomList" tooltip-effect="dark" v-loading="devRoomLoading" style="width: 100%"
        @selection-change="onRoomChange" @row-click="clickRoomRow" @select="roomBoxSelect" :row-key="getRowKeys">
        <el-table-column type="selection" width="55" :reserve-selection="true"> </el-table-column>
        <el-table-column label="房间名称" align="center" key="Name" prop="Name"></el-table-column>
        <el-table-column label="所属分类" align="center" key="CategoryName" prop="CategoryName" :show-overflow-tooltip="true"></el-table-column>
        <el-table-column label="负责人" align="center" key="LeaderId" prop="LeaderId" :show-overflow-tooltip="true">
        <template slot-scope="scope">
            <div>{{scope.row.LeaderInfo.RealName}}</div>
        </template>
        </el-table-column>
      </el-table>
      <div slot="footer" class="dialog-footer">
        <el-button @click="roomOpen = false">取 消</el-button>
        <el-button type="primary" @click="addDevRoom">确 定</el-button>
      </div>
    </el-dialog>
  </template>
  
  <script>
  import {
  deviceRoomList,
} from "@/api/after/room";
  export default {
    name: "AdminUiSelectDevice",
  
    data() {
      return {
          getRowKeys(row) {
              return row.Id;
          },
          devRoomList: [],
          roomOpen: false,
          devDateRange: [],//时间日期
          afterSelectRoom: [],//选择后的设备列表
          devRoomLoading: false,
      };
    },
  
    mounted() {},
  
    methods: {
      openAddDevRoom(planeTargetData) {
        //选择设备
        this.roomOpen = true
        this.$nextTick(()=>{
          let allselArr=JSON.parse(JSON.stringify(planeTargetData))
          let selList=allselArr.filter(row=>row.TargetType==2)
          this.$refs.devRoomTable.clearSelection()
          this.afterSelectRoom=selList.map(row=>{
            let obj={
              Id:row.TargetId,
              Name:row.TargetName,
            }
            if(obj){
              this.$refs.devRoomTable.toggleRowSelection(obj, true);
            }
            return obj
          })
        })
        this.getDevRoomList()
      },
      onRoomChange(val) {
        this.afterSelectRoom = JSON.parse(JSON.stringify(val))
      },
      roomBoxSelect(arr, row) {
        //点击设备选择多选框
        const selected = this.afterSelectRoom.some(
          (item) => item.Id === row.Id
        );
        if (selected) {
          this.afterSelectRoom = this.afterSelectRoom.filter(
            (rw) => rw.Id !== row.Id
          );
        }
      },
      clickRoomRow(row) {
        // console.log("点击单行",row);
        const selected = this.afterSelectRoom.some(
          (item) => item.Id === row.Id
        );
        if (!selected) {
          // 选择
          this.$refs.devRoomTable.toggleRowSelection(row, true);
        } else {
          // 取消
          this.$refs.devRoomTable.toggleRowSelection(row, false);
          this.afterSelectRoom = this.afterSelectRoom.filter(
            (rw) => rw.Id !== row.Id
          );
        }
      },
      getDevRoomList() {
        //获取设备列表
        this.devRoomLoading = true;
        deviceRoomList().then((response) => {
          // console.log("房间列表",response);
            this.devRoomList = response.data;
            this.devRoomLoading = false;
          })
          .catch((err) => {
            this.devRoomLoading = false;
          });
      },
      addDevRoom() {
        //添加计划的设备
        let sellist=this.afterSelectRoom.map(row=>{
          let obj={
            TargetId:row.Id,
            TargetType:2,
            TargetName:row.Name,
          }
          return obj
        })
        this.$emit('addDevRoom',sellist)
        this.roomOpen = false
      },
    },
  };
  </script>
  