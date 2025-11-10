<template>
  <div>
    <el-dialog :title="dialogTitle" :visible.sync="taskOpenAdd" center width="900px" :close-on-click-modal="false" :destroy-on-close="true">
      <el-form :model="taskAddFrom" ref="taskAddFrom" :rules="taskRules" label-position="top" class="groupFrom" label-width="110px">
        <el-form-item label="计划名称" prop="planName">
          <el-input type="text" v-model="taskAddFrom.planName" placeholder="请输入计划名称" :disabled="isViewInfo"></el-input>
        </el-form-item>
        <el-form-item label="任务单号" prop="planeNumber">
          <el-input type="text" v-model="taskAddFrom.planeNumber" placeholder="请输入计划名称" :disabled="true"></el-input>
        </el-form-item>
        <el-form-item label="目标设备" style="position: relative;" prop="targetId">
          <div class="select_btn" @click="openAddDevice">
            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
            选择设备
          </div>
          <div v-if="selectDevice&&selectDevice.length>0">
            <div class="devText" v-for="its in selectDevice" :key="its.Id">
              <div class="devText-info">
                <span>{{ its.Name }}</span>
                <span>{{ its.DeviceNumber }}</span>
              </div>
            </div>
          </div>
        </el-form-item>
        <div v-if="selectDevice&&selectDevice.length>0" class="line"></div>
        <!-- <div class="airCompressors" > -->
            <!-- <div class="airCompressors-item" v-for="its in selectDevice" :key="its.Id">
                <div class="airCompressors-item-top">
                    <div class="airCompressors-item-top-left">
                        <el-image fit="cover" class="airCompressors-item-top-logo" :src="its.PhotoUrl + '?wh=500x500'">
                            <img class="airCompressors-item-top-logo" slot="error" src="../../../assets/images/shebei.png" alt/>
                        </el-image>
                        <div class="airCompressors-item-top-left-text">
                            <p class="title">{{ its.Name }}</p>
                            <p class="number">{{ its.DeviceNumber }}</p>
                            <p class="onLine">
                                <span v-if="its.Online != 2" class="yuan" :class="[its.Online == 0 ? 'on1' : 'active1']"></span>
                                <span :class="[its.Online == 0 ? 'on' : its.Online == 1 ? 'active' : 'on',]">{{ its.Online == 0 ? "离线" : its.Online == 1 ? "在线" : "未知" }}</span>
                            </p>
                         </div>
                     </div>
                 </div>
             </div> -->
        <!-- </div> -->
        <div>
          <AddEmbed ref="flowForm"></AddEmbed>
        </div>
      </el-form>
      <div slot="footer" style="text-align: end;" class="dialog-footer" v-if="!isViewInfo">
          <el-button @click="taskOpenAdd = false">取 消</el-button>
          <el-button type="primary" @click="addTask" v-loading="submitLoading" :disabled="submitLoading">确 定</el-button>
      </div>
    </el-dialog>
    <el-dialog width="960px" title="请选择目标设备" :visible.sync="deviceOpen" append-to-body>
      <el-form :model="deviceQuery" ref="deviceForm" :inline="true" style="display: flex; justify-content: space-between">
        <div>
          <el-form-item label="查询关键字" prop="Key">
            <el-input v-model="deviceQuery.Key" placeholder="请输入设备名称、通讯编码或编号" clearable></el-input>
          </el-form-item>
          <el-form-item label="创建时间">
            <el-date-picker class="form_input_style" v-model="devDateRange" style="width: 232px" value-format="yyyy-MM-dd" type="daterange"
              range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
          </el-form-item>
        </div>
        <el-form-item>
          <el-button icon="el-icon-refresh" @click="resetSelectDevice">重置</el-button>
          <el-button type="primary" icon="el-icon-search" @click="getplaneDevList()">搜索</el-button>
        </el-form-item>
      </el-form>
      <el-table ref="devTable" :data="deviceList" tooltip-effect="dark" v-loading="deviceLoading" style="width: 100%"
        @row-click="clickDevRow" :row-key="getRowKeys">
        <!-- <el-table-column type="selection" width="55" :reserve-selection="true"> </el-table-column> -->
        <el-table-column prop="DeviceNumber" label="设备编号" align="center" width="150"></el-table-column>
        <el-table-column prop="DeviceId" label="通讯编码" align="center" width="150"></el-table-column>
        <el-table-column label="预览图片" align="center" width="150">
          <template slot-scope="scope">
            <div class="imgwrap" style="max-width: 60px;max-height:60px;">
              <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'" :preview-src-list="[scope.row.PhotoUrl]"></el-image>
            </div>
          </template>
        </el-table-column>
        <el-table-column prop="Name" label="设备名称"> </el-table-column>
        <el-table-column prop="ProductName" label="所属产品"></el-table-column>
      </el-table>
      <pagination v-show="deviceTotal > 0" :total="deviceTotal" :page.sync="deviceQuery.pageNum" :limit.sync="deviceQuery.pageSize" @pagination="getplaneDevList"/>
    </el-dialog>
  </div>
</template>

<script>
import { devPlaneInfo,devPlaneTaskNumber,taskFormData,devPlaneTaskAdd } from "@/api/after/devplane";
import TableList from '@/views/flowable/common/form/components/TableList.vue'
import AddEmbed from "@/views/flowable/task/record/AddEmbed";
import { planeDevList } from "@/api/after/dev";
export default {
  name: 'AdminUiTaskAdd',
  components: { TableList,AddEmbed },
  data() {
    return {
      deviceQuery: {
          pageNum: 1,
          pageSize: 10,
          Key: "",
      },
      deviceTotal:0,
      devDateRange:[],
      isViewInfo:false,
      getRowKeys(row) {
        return row.Id;
      },
      dialogTitle:'',
      taskOpenAdd:false,
      deviceList:[],
      deviceOpen:false,
      deviceLoading:false,
      afterSelectDevice:[],
      selectDevice:[],
      taskAddFrom:{
        planeNumber:'',
        planName:'',
        planTypeId:'',
        targetId:''
      },
      taskRules:{
        planName: [{ required: true, trigger: "blur", message: "请输入计划名称" }],
        targetId:[{ required: true, trigger: "change", message: "请选择设备" }],
      },
      submitLoading:false,
      formInit:[],
      tbloading:false,
      activePlaneId:'',
      taskFlowMode:{}
    };
  },

  mounted() {
    
  },

  methods: {
    async openDialog(id){
      this.activePlaneId=id
      this.selectDevice=[]
      this.taskAddFrom={
        planeNumber:'',
        planName:'',
        planTypeId:'',
        targetId:''
      }
      this.resetForm("taskAddFrom");
      await this.getPlaneInfo(id)
      this.taskOpenAdd=true
    },
    openAddDevice(){
        this.deviceOpen=true
    },
    async getTaskNumber(){
        try {
            let res=await devPlaneTaskNumber()
            return res.data
        } catch (error) {
            
        }
    },
    async addPlaneDevice(){
        this.selectDevice=JSON.parse(JSON.stringify(this.afterSelectDevice))
        if(this.selectDevice&&this.selectDevice.length>0){
          this.taskAddFrom.targetId=this.selectDevice[0].Id
          await this.loadtaskFormData(this.selectDevice[0].Id,this.activePlaneId)
        }
        
    },
    async getPlaneInfo(val){
        try {
            let infores=await devPlaneInfo({id:val})
            let info=infores.data
            this.dialogTitle='当前计划类型：'+info.Name
            this.taskAddFrom.planeNumber=await this.getTaskNumber()
            this.taskAddFrom.planName=info.Name
            this.taskAddFrom.flowId=info.FlowTemplateId
            this.taskAddFrom.planTypeId=info.Id
            this.deviceQuery.Id=val
            // this.deviceList=infores.data.DeviceList?JSON.parse(JSON.stringify(infores.data.DeviceList)):[]
            await this.getplaneDevList();
        } catch (error) {
            
        }
    },
    async resetSelectDevice(){
      this.deviceQuery.Key = ''
      this.devDateRange = []
      this.deviceQuery.pageNum = 1
      await this.getplaneDevList();
    },
    async getplaneDevList(){
      let res=await planeDevList(this.deviceQuery)
      this.deviceList=res.data.List
      this.deviceTotal=res.data.Total
    },
    addTask(){
      this.$refs.taskAddFrom.validate((valid) => {
        if(valid){
          this.submitLoading=true
          let submitForm={}
          let tmpmodel = this.$refs.flowForm.getModel();
          submitForm.task=JSON.parse(JSON.stringify(this.taskAddFrom))
          submitForm.model=JSON.parse(JSON.stringify(tmpmodel.model))
          submitForm.assign=JSON.parse(JSON.stringify(tmpmodel.assign))
          devPlaneTaskAdd(submitForm).then(res=>{
            //console.log("添加成功",res);
            this.$modal.msgSuccess("保存成功");
            this.$emit('reloadList')
            this.taskOpenAdd=false
            if(this.submitLoading){
              this.submitLoading=false
            }
          }).catch(err=>{
            if(this.submitLoading){
              this.submitLoading=false
            }
          })
        }
      })
    },
    async loadtaskFormData(devId,planeId){
      try {
        let res=await taskFormData({devId:devId,planeId:planeId})
        this.taskFlowMode=JSON.parse(JSON.stringify(res.data))
        if(this.taskAddFrom.flowId){
          await this.$refs.flowForm.InitData(
            this.taskAddFrom.flowId,
            {},null,res.data
          );
        }
      } catch (error) {
        
      }
    },
    // async onDeviceChange(val) {
    //     this.afterSelectDevice=JSON.parse(JSON.stringify(val))
        
    // },
    // async devBoxSelect(arr, row) {
    //   //点击多选框
    //   this.afterSelectDevice=[row]
    //   await this.addPlaneDevice()
      
    // },
    async clickDevRow(row) {
      this.afterSelectDevice=[row]
      this.deviceOpen=false
      await this.addPlaneDevice()
      
    },
  },
};
</script>
<style lang="less" scoped>
::v-deep .groupFrom {
  .el-form-item {
    width: 100%;
    .el-form-item__content {
      .el-select {
        width: 100%;
      }

      .el-input {
        width: 100%;
      }
    }
  }
}
.select_btn{
  position: absolute;
  right: 0;
  top: -54px;
  display: flex;
  align-items: center;
  font-weight: 400;
  font-size: 14px;
  color: #3572FF;
  cursor: pointer;
}
.select_btn>i{
  margin-right: 6px;
}
.devText{
  width: 100%;
  height: 48px;
  background: #E8F4FF;
  border-radius: 4px;
  border: 1px solid #D1E9FF;
  font-weight: 400;
  font-size: 16px;
  color: #3572FF;
  line-height: 48px;
  margin-bottom: 10px;
}
.devText-info{
  padding: 0 20px;
}
.devText-info>span{
  margin-right: 60px;
}
.line{
  width: 100%;
  height: 10px;
  background: #F8F8F8;
  margin: 20px 0;
}
.box-row {
    .bx-hd {
        font-size: 16px;
        color: #333;
        background-color: rgb(249, 250, 252);
        padding: 0 15px;
        height: 48px;
        display: flex;
        justify-content: space-between;
        flex-direction: row;
        align-items: center;
    }

    .bx-bd {
        padding: 15px 0px;

        .el-input__suffix {
            display: flex;
            align-items: center;
        }
    }
}
.airCompressors {
  display: flex;
  justify-content: left;
  width: 100%;
  flex-wrap: wrap;
  align-items: center;
  margin-right: -15px;
  margin-bottom: -15px;
  //margin-top: -10px;
  .airCompressors-item:nth-child(3n) {
    margin-right: 0;
  }
  .airCompressors-item {
    background: rgba(249, 250, 252, 1);
    width: calc(33% - 10px);
    // height:180px;
    border-radius: 10px;
    margin-bottom: 15px;
    display: flex;
    margin-right: 14px;
    flex-direction: column;
    padding: 15px 20px;
    box-sizing: border-box;
    .airCompressors-item-bottom {
      color: rgba(153, 153, 153, 1);
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-top: 15px;
      font-size: 13px;
      .airCompressors-item-bottom-left {
        width: 48%;
        border-right: 1px solid rgba(234, 234, 234, 1);
      }
      .airCompressors-item-bottom-right {
        width: 48%;
        text-align: center;
      }
    }
    .airCompressors-item-top {
      display: flex;
      //border-bottom: 1px solid rgba(234, 234, 234, 1);
      // padding-bottom: 15px;
      justify-content: space-between;
      .airCompressors-item-top-see {
        // margin-left: auto;
        align-self: flex-end;
        color: rgba(153, 153, 153, 1);
        font-size: 13px;
        display: flex;
        align-items: center;
        cursor: pointer;
        .airCompressors-item-top-see-txt {
          display: inline-block;
          margin-left: 6px;
        }
      }
      .airCompressors-item-top-see:hover {
        color: rgba(53, 114, 255, 1);
      }
      .airCompressors-item-top-left {
        display: flex;
        align-items: center;

        .airCompressors-item-top-left-text {
          line-height: 12px;
          margin-left: 20px;
          .number {
            color: rgba(153, 153, 153, 1);
            font-size: 14px;
          }
          .title {
            font-weight: bold;
            line-height: 25px;
          }
          .onLine {
            display: flex;
            font-size: 13px;
            align-items: center;
            .yuan {
              display: inline-block;
              width: 8px;
              height: 8px;
              border-radius: 50%;

              margin-right: 6px;
            }
          }
          .active1 {
            background: rgba(13, 179, 166, 1);
          }
          .on1 {
            background: rgba(186, 186, 186, 1);
          }
          .active {
            color: rgba(13, 179, 166, 1);
          }
          .on {
            color: rgba(102, 102, 102, 1);
          }
        }
        .airCompressors-item-top-logo {
          width: 75px;
          height: 75px;
          border-radius: 10px;
        }
      }
    }
  }
}
</style>