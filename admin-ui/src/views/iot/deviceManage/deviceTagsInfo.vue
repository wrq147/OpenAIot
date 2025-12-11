<template>
  <div>
    <div class="info_title" v-if="labelIfo.length > 0">
      <h3>标签信息</h3>
    </div>
    <div>
      <el-form ref="deviceInfos" :model="deviceInfos">
        <table border="1" class="configInfo" v-loading="configLoadingTag" v-if="labelIfo.length > 0">
          <tr style="width: 50%; display: inline-table" v-for="(item, inx) in labelIfo" :key="item.Code">
            <th colspan="1" style="border-left: none">
              <span>{{ item.Name }}</span>
            </th>
            <td class colspan="3" style="width: 75%">
              <div style="display: flex;justify-content: space-between;align-items: center;">
                <div v-if="!item.isEdit">{{ item.DisplayValue }}</div>
                <div v-if="item.isEdit">
                  <el-form-item style="margin-bottom: 0">
                    <el-switch v-if="item.Option.type == 'boolean'" v-model="item.Value" :active-text="item.Option.trueText" :inactive-text="item.Option.falseText">
                    </el-switch>
                    <el-input-number v-else-if="item.Option.type == 'float'" v-model="item.Value" :precision="item.Option.decimals" :step="0.1" :min="item.Option.min" :max="item.Option.max"></el-input-number>
                    <el-input-number v-else-if="item.Option.type == 'int'" v-model="item.Value" :min="item.Option.min" :max="item.Option.max"></el-input-number>
                    <el-select v-else-if="item.Option.type == 'enum'" v-model="item.Value" placeholder="请选择">
                      <el-option v-for="(elii, kii) in item.Option.elements" :key="kii" :label="elii" :value="kii"></el-option>
                    </el-select>
                    <el-date-picker v-else-if="item.Option.type == 'date'" v-model="item.Value" value-format="timestamp" type="datetime" placeholder="选择日期时间"></el-date-picker>
                    <el-input v-else v-model="item.Value" placeholder="请输入内容"></el-input>
                  </el-form-item>
                </div>
                <div v-if="canChangeDevice">
                  <div class="edit_class" v-if="!item.isEdit && canChangeDevice">
                    <el-button type="text" @click="startEditLable(inx)" style="margin-left: 20px; color: #bfbfbf">
                      <i class="el-icon-edit"></i>
                      <span>编辑</span>
                    </el-button>
                    <el-button type="text" @click="labelInfoVisible(item)" style="margin-left: 20px" v-if="deviceStorageConfig&&item.MapCode">
                      <i class="zhongtaiiconfont zhongtai-icon-a-caidanguanli"></i>
                    </el-button>
                  </div>
                  <div class="edit_class" v-if="item.isEdit && canChangeDevice">
                    <el-button type="text" @click="cancelEditLable(inx)">
                      <i class="el-icon-close"></i>取消
                    </el-button>
                    <el-button type="text" @click="saveLabelInfo(item, inx)" :loading="saveLoading">
                      <i class="el-icon-check"></i>{{ saveLoading ? "提交中 ..." : "保 存" }}
                    </el-button>
                  </div>
                </div>
                <div v-if="!canChangeDevice">
                  <div class="edit_class">
                    <el-button type="text" @click="labelInfoVisible(item)" style="margin-left: 20px" v-if="deviceStorageConfig&&item.MapCode">
                      <i class="zhongtaiiconfont zhongtai-icon-a-caidanguanli"></i>
                    </el-button>
                  </div>
                </div>
              </div>
            </td>
          </tr>
        </table>
      </el-form>
    </div>
  </div>
</template>

<script>
import { saveDeviceTag, DeviceTagList } from "@/api/rules/device";
import dayjs from 'dayjs';
export default {
  name: "deviceTagsInfo",
  props: {
    deviceInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
    canChangeDevice: {
      type: Boolean,
      default: true,
    },
    deviceStorageConfig:{
      type:Boolean,
      default:false
    }
  },
  data() {
    return {
        configLoadingTag:true,
        labelIfo: [],
        deviceInfosBasic: this.deviceInfos,
        saveLoading:false,
    };
  },

  mounted() {},
  watch: {
    deviceInfos(to, from) {
    //   console.log("变化的", to);
      this.deviceInfosBasic = to;
      this.getDeviceTagList();
    },
  },
  methods: {
    labelInfoVisible(item){
        this.$emit('labelInfoVisible',item)
    },
    getDeviceTagList() {
      //获取标签信息
      this.configLoadingTag=true
      DeviceTagList({ id: this.deviceInfos.Id }).then((res) => {
        // console.log("标签信息", res);
        let editObj = {
          isEdit: false,
        };
        res.data = res.data.map((item) => ({ ...item, ...editObj }));
        this.labelIfo = JSON.parse(JSON.stringify(res.data));
        this.configLoadingTag=false
      });
    },
    startEditLable(inx) {
      //开始编辑
      this.labelIfo[inx].isEdit = true;
    },
    cancelEditLable(inx) {
      // 取消编辑
      this.labelIfo[inx].isEdit = false;
    },
    saveLabelInfo(row, inx) {
      //保存标签信息
      let arr = [];
      let obj = {
        Code: row.Code,
        Value: row.Value,
      };
      arr.push(obj);
      let subQuery={ id: this.deviceInfosBasic.Id, list: arr }
      if(row.indate){
        subQuery.indate=dayjs(row.indate).format('YYYY-MM-DD HH:mm:ss')
      }
      saveDeviceTag(subQuery).then((res) => {
        // console.log("保存标签信息返回", res);
        if (res.code == 0) {
          this.$modal.msgSuccess("修改成功");
          this.cancelEditLable(inx);
          this.getDeviceTagList();
        }
      });
    },
  },
};
</script>
<style lang="less" scoped>
.edit_class {
  font-size: 16px;
  color: #bfbfbf;
  margin-left: 10px;
  cursor: pointer;

  span {
    margin-left: 5px;
  }

  button {
    font-size: 16px;
  }
}

.info_title {
  display: flex;
  justify-content: flex-start;
  align-items: center;
}

table.configInfo {
  border-spacing: 0px;
  border-collapse: collapse;
  width: 100%;
  border: none;
  // border-top: solid 1px #efefef;
  border-left: solid 1px #efefef;

  tr {
    height: 48px;
    line-height: 48px;
    color: rgba(0, 0, 0, 0.85);
    font-size: 14px;

    th {
      background-color: #fafafa;
      padding: 0 15px;
      border-bottom: solid 1px #efefef;
      border-right: solid 1px #efefef;
    }

    td {
      padding: 0 15px;
      border-bottom: solid 1px #efefef;
      border-right: solid 1px #efefef;

      div {
        display: flex;
        align-items: center;
        // justify-content: center;
      }
    }
  }
}
</style>
