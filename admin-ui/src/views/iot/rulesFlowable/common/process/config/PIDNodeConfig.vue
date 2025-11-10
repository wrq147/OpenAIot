<template>
  <div>
    <el-form label-width="110px">
      <el-form-item label="参考设备类型：">
        <el-select style="width:100%" v-model="curType" placeholder="请选择参考设备类型" @change="choiceTargetType">
          <el-option label="当前设备" :value="0" v-if="enableCur"></el-option>
          <el-option label="选择设备" :value="1"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="参考设备：" v-if="curType == 1">
        <el-select style="width:100%" v-model="config.refdevid" clearable filterable remote reserve-keyword
          :remote-method="remoteMethod" @clear="remoteMethod('')" :loading="loading" placeholder="请选择参考设备"
          @change="devChange">
          <el-option v-for="item in deviceLists" :key="item.Id" :label="item.Name" :value="item.Id"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="PID类型：">
        <el-select style="width:100%" v-model="config.pidtype" placeholder="请选择PID类型">
          <el-option label="位置式" :value="0"></el-option>
          <!-- <el-option label="增量式" :value="1"></el-option> -->
        </el-select>
      </el-form-item>
      <el-form-item label="设备属性：" v-if="curType == 0 || curType == 1 && config.refdevid">
        <el-select v-model="config.refprop" placeholder="请选择设备属性">
          <el-option v-for="item in attributeList" :key="item.code" :label="item.name" :value="item.code"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="目标值参数：" v-if="curType == 0 || curType == 1 && config.refdevid">
        <el-select v-model="config.targetval" placeholder="请选择目标值参数">
          <template v-for="item in ParamList">
            <el-option :key="item.code" :label="item.name" :value="item.code" v-if="item.type == 'float'"></el-option>
          </template>
        </el-select>
      </el-form-item>
      <el-form-item label="比例参数：" v-if="curType == 0 || curType == 1 && config.refdevid">
        <el-select v-model="config.pname" placeholder="请选择比例参数">
          <template v-for="item in ParamList">
            <el-option :key="item.code" :label="item.name" :value="item.code" v-if="item.type == 'float'"></el-option>
          </template>
        </el-select>
      </el-form-item>
      <el-form-item label="积分时间：" v-if="curType == 0 || curType == 1 && config.refdevid">
        <el-select v-model="config.iname" placeholder="请选择积分时间参数">
          <template v-for="item in ParamList">
            <el-option :key="item.code" :label="item.name" :value="item.code" v-if="item.type == 'float'"></el-option>
          </template>
        </el-select>
      </el-form-item>
      <el-form-item label="微分时间：" v-if="curType == 0 || curType == 1 && config.refdevid">
        <el-select v-model="config.dname" placeholder="请选择微分时间参数">
          <template v-for="item in ParamList">
            <el-option :key="item.code" :label="item.name" :value="item.code" v-if="item.type == 'float'"></el-option>
          </template>
        </el-select>
      </el-form-item>
    </el-form>
    <div style="border:solid 1px #dadada;">
      <div class="sch-title">
        <span>控制的设备列表</span>
        <div class="addbtn" @click="addDevController">添加</div>
      </div>
      <div class="sch-row">
        <div style="flex:1;width:0px;" class="cc-col">设备名</div>
        <div style="width:120px;" class="cc-col">操作</div>
      </div>
      <div class="sch-row" v-for="(dv, idx) in config.DevControllerItem" :key="dv.id">
        <div style="flex:1;width:0px;" class="cc-col">{{ DeviceName(dv.id) }}</div>
        <div style="width:120px;" class="cc-col">
          <el-link type="primary" @click="onEdit(idx)">编辑</el-link>
          <el-link type="warning" @click="onDel(idx)" style="text-decoration: none;margin-left:5px">删除</el-link>
        </div>
      </div>
    </div>

    <el-dialog :title="sceTitle" append-to-body :close-on-click-modal="false" :visible.sync="scheOpen" width="740px">
      <el-form label-width="150px" v-if="editSet != null">
        <el-form-item label="控制设备类型：">
          <el-select style="width:100%" v-model="editSet.TargetType" placeholder="请选择控制设备类型"
            @change="choiceTargetType2">
            <el-option label="当前设备" :value="0" v-if="enableCur"></el-option>
            <el-option label="选择设备" :value="1"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="控制设备：" v-if="editSet.TargetType == 1">
          <el-select style="width:100%" v-model="editSet.id" clearable filterable remote reserve-keyword
            :remote-method="remoteMethod2" @clear="remoteMethod2('')" :loading="loading" placeholder="请选择控制设备"
            @change="devChange2">
            <el-option v-for="item in deviceLists2" :key="item.Id" :label="item.Name" :value="item.Id"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="最大控制量参数：" v-if="editSet.TargetType == 0 || editSet.TargetType == 1 && editSet.id">
          <el-select v-model="editSet.maxval" placeholder="请选择最大控制量参数">
            <template v-for="item in ParamList">
              <el-option :key="item.code" :label="item.name" :value="item.code" v-if="item.type == 'float'"></el-option>
            </template>
          </el-select>
        </el-form-item>
        <el-form-item label="最小控制量参数：" v-if="editSet.TargetType == 0 || editSet.TargetType == 1 && editSet.id">
          <el-select v-model="editSet.minval" placeholder="请选择最小控制量参数">
            <template v-for="item in ParamList">
              <el-option :key="item.code" :label="item.name" :value="item.code" v-if="item.type == 'float'"></el-option>
            </template>
          </el-select>
        </el-form-item>
        <el-form-item label="执行的功能：" v-if="editSet.TargetType == 0 || editSet.TargetType == 1 && editSet.id">
          <el-select v-model="editSet.code" placeholder="请选择执行的功能">
            <el-option v-for="item in funList" :key="item.code" :label="item.name" :value="item.code"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="功能的控制量参数：" v-if="editSet.TargetType == 0 || editSet.TargetType == 1 && editSet.id">
          <el-select v-model="editSet.codeval" placeholder="请选择功能的控制量参数">
            <el-option v-for="item in funParamList" :key="item.code" :label="item.name" :value="item.code"></el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="scheOpen = false">取 消</el-button>
        <el-button type="primary" @click="onOk">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { DeviceList } from "@/api/rules/device";
import { productInfo } from "@/api/rules/productModel";

export default {
  name: "TimeSchedulerNodeConfig",
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      }
    }
  },
  computed: {
    funParamList() {
      if (this.editSet.code) {
        let activeFuncode = this.funList.find(row => row.code == this.editSet.code)
        if (activeFuncode && activeFuncode.inputs) {
          let list = activeFuncode.inputs.filter(rw => rw.type == 'float')
          return list
        } else {
          return []
        }
      } else {
        return []
      }
    },
    setup() {
      return this.$store.state.rulesFlowable.rulesDesign;
    },
    ParamList() {
      let httpss = this.$store.state.rulesFlowable.rulesDesign.HttpParams;
      if (httpss != null) {
        return httpss;
      }
      else {
        return [];
      }
    },
    defaultProductId() {
      let TriggerList = this.$store.state.rulesFlowable.rulesDesign.TriggerList
      if (TriggerList && TriggerList.length > 0) {
        let prodectId = TriggerList[0].TopicDevice.split("/")[1]
        return prodectId
      } else {
        return ''
      }
    },
    LoopCount() {
      if (this.editSet == null) return [];
      if (this.editSet.conditions.length > 1) {
        if (this.editSet.groups == null) {
          this.editSet.groups = [];
        }

        let ll = this.editSet.conditions.length - 1;
        for (let i = 0; i < ll; i++) {
          if (i >= this.editSet.groups.length) {
            this.editSet.groups[i] = "&";
          }
        }
        if (this.editSet.groups.length > ll) {
          this.editSet.groups.splice(ll);
        }
        return this.editSet.groups;
      }
      else {
        return [];
      }
    },
    enableCur() {
      if (this.$store.state.rulesFlowable.selectProductInfo != null || this.$store.state.rulesFlowable.rulesDesign.TriggerWay == 0) {
        return true;
      }
      else {
        return false;
      }
    }
  },
  data() {
    return {
      curType: 1,
      scheOpen: false,
      sceTitle: "",
      editDevId: "",
      editSet: null,
      funList: [],//功能列表
      deviceLists: [],//显示设备列表
      deviceLists2: [],//显示设置控制的设备列表
      filterParams: {//设备列表过滤参数
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
        Key: ""
      },
      attributeList: [],//属性数组
      loading: false,
      tableDevList: [],
      activeDevIdx: null,
      isfirstLoad: true,
      isopenLoad: false,
    };
  },
  async mounted() {
    this.isfirstLoad = true
    if (this.config.refdevid == null) {
      this.curType = 0;
    }
    else {
      this.curType = 1;
    }
    await this.remoteMethod("");
    await this.loadListDev()

  },
  methods: {
    async onOk() {
      this.config.DevControllerItem[this.activeDevIdx] = JSON.parse(JSON.stringify(this.editSet))
      this.activeDevIdx = null
      this.editSet = null;
      await this.loadListDev()
      this.scheOpen = false;
    },
    async onEdit(idx) {
      this.isopenLoad = true
      this.activeDevIdx = idx
      this.editSet = JSON.parse(JSON.stringify(this.config.DevControllerItem[idx]));
      if (this.editSet.id == null) {
        this.editSet.TargetType = 0;
      }
      else {
        this.editSet.TargetType = 1;
      }
      let num = idx + 1
      this.sceTitle = "编辑第" + num + "条控制的设备";
      this.scheOpen = true;
      this.$forceUpdate();
      await this.remoteMethod2('');
    },
    onDel(ix) {
      this.config.DevControllerItem.splice(ix, 1);
    },
    addDevController() {
      this.config.DevControllerItem.push({ TargetType: 1, id: "", maxval: '', minval: '', code: '', codeval: '' });
    },
    async loadListDev() {
      //用于表格显示设备名称使用
      let queryObj = {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
        Ids: [],
      }
      if (this.config.DevControllerItem && this.config.DevControllerItem.length > 0) {
        let ids = this.config.DevControllerItem.map(row => row.id)
        queryObj.Ids = ids
        try {
          let rsp = await DeviceList(queryObj);
          this.tableDevList = rsp.data.List;
        }
        catch (err) {
          this.$message.error("接口异常");
        }
      }
    },
    async choiceTargetType2() {
      if (this.editSet.TargetType == 0) {
        this.editSet.id = null;
      }
      else {
        this.editSet.id = "";
      }
      await this.remoteMethod2('');
    },
    async remoteMethod2(query) {
      this.loading = true;
      if (this.editSet.TargetType == 1) {
        if (this.editSet.id != "") {
          this.filterParams.Ids = [this.editSet.id];
        }
        else {
          this.filterParams.Ids = undefined;
        }
        this.filterParams.Key = query;

        try {
          let rsp = await DeviceList(this.filterParams);
          this.deviceLists2 = rsp.data.List;
        }
        catch (err) {
          this.$message.error("接口异常");
        }
      }
      this.devChange2();
      this.isopenLoad = false
      this.loading = false;
    },
    devChange2() {
      if (!this.isopenLoad) {
        this.editSet.code = ''
      }
      if (this.editSet.TargetType !== null && this.editSet.TargetType !== '' && this.editSet.TargetType !== undefined) {
        let devlist = this.deviceLists2.filter(x => x.Id == this.editSet.id);
        if (devlist.length == 0) {
          if (this.editSet.TargetType == 0) {
            this.getNewProductFun(this.defaultProductId, true);
          } else {
            this.$set(this, "funList", []);
          }
        } else {
          this.getNewProductFun(devlist[0].ProductId, true);
        }
      }
    },
    async choiceTargetType() {
      if (this.curType == 0) {
        this.config.refdevid = null;
      }
      else {
        this.config.refdevid = "";
      }

      await this.remoteMethod('');
    },
    async remoteMethod(query) {
      this.loading = true;
      if (this.curType == 1) {
        if (this.config.refdevid != "") {
          this.filterParams.Ids = [this.config.refdevid];
        }
        else {
          this.filterParams.Ids = undefined;
        }
        this.filterParams.Key = query;

        try {
          let rsp = await DeviceList(this.filterParams);
          this.deviceLists = rsp.data.List;
          console.log("加载的设备列表", this.deviceLists);
        }
        catch (err) {
          this.$message.error("接口异常");
        }
      }
      this.devChange();
      this.isfirstLoad = false
      this.loading = false;
    },
    devChange() {
      if (!this.isfirstLoad) {
        this.config.refprop = ''
      }
      let devlist = this.deviceLists.filter(x => x.Id == this.config.refdevid);
      if (devlist.length == 0) {
        if (this.curType == 0) {
          this.getNewProductFun(this.defaultProductId);
        } else {
          this.$set(this, "attributeList", []);
        }
      } else {
        this.getNewProductFun(devlist[0].ProductId);
      }
    },
    getNewProductFun(pid, isController) {
      productInfo({ id: pid }).then(rsp => {
        if (rsp.code == 0) {
          let jsonLis = JSON.parse(rsp.data.ModelTSL);
          console.log(jsonLis, 'jsonLisjsonLisjsonLis');
          if (isController) {
            if (jsonLis.functions) {
              let filterFunList = jsonLis.functions
              this.$set(this, "funList", filterFunList);
            }
          } else {
            if (jsonLis.properties) {
              let filterProperties = jsonLis.properties.filter(row => row.option.type == 'float')
              this.$set(this, "attributeList", filterProperties);
            }
          }

        }
      });
    },
    reloadDevice() {
      this.filterParams.Ids = [];
      this.remoteMethod("");
    },
    DeviceName(did) {
      let tmplist = this.tableDevList.filter(x => x.Id == did);
      if (tmplist.length > 0) {
        return tmplist[0].Name;
      }
      else {
        return did;
      }
    },
  }
};
</script>

<style lang="scss">
.sch-title {
  display: flex;
  height: 45px;
  background-color: #f5f5f5;
  align-items: center;
  padding: 0px 15px;
  font-size: 14px;
  color: #666;
  justify-content: space-between;

  .addbtn {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 80px;
    height: 40px;
    border: 1px solid #DADADA;
    border-radius: 6px;
  }
}

.sch-row {
  display: flex;
  flex-direction: row;
  font-size: 14px;
  border-top: solid 1px #dadada;

  .cc-col {
    display: flex;
    padding: 12px 0px;
    align-items: center;
    justify-content: center;
  }
}

.dlg-param-bg {
  border-radius: 5px;
  background-color: #F5F7FA;
  border: solid 1px #dadada;
  padding: 15px 10px;

  .paramrow {
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: 10px;
  }
}

.dlg-gg-row {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  align-items: center;
}

.el-swit {
  color: #409EFF;
  cursor: pointer;
}

.dlg-ac-bg {
  background-color: #F5F7FA;
  border: solid 1px #dadada;
  padding: 15px 10px;

  .acrow {
    display: flex;
    flex-direction: row;
  }
}
</style>
