<template>
  <div>
    <div v-if="mode === 'DESIGN'">
      <el-button disabled icon="el-icon-setting" type="primary" size="mini" round>选择设备</el-button>
      <span class="placeholder">{{ placeholder }}</span>
    </div>
    <div v-else style="width:100%">
      <el-button :disabled="disabled" icon="el-icon-setting" type="primary" size="mini" round
        @click="showDialog" v-if="!disabled">选择设备</el-button>
      <dev-picker :limit="limit" ref="devicePicker" :limit_product="limit_product" :selected="_value" @ok="selected" />
      <span class="placeholder" v-if="_value && _value.length == 0">{{ placeholder }}</span>
      <div style="margin-top: 5px;display:inline-block" v-else>
        <el-tag size="mini" style="margin: 5px;" :style="{'cursor': disabled?'pointer':''}" :closable="!disabled" v-for="(dept, i) in _value"
          @close="delDept(i)" :key="dept.id+''+i" @click.stop="jumpToDevDetail(dept)">{{ dept.name }}</el-tag>
      </div>
    </div>
  </div>
</template>

<script>
import componentMinxins from "../ComponentMinxins";
import devPicker from "../../DevicePickers";
import { DeviceInfo, DeviceLiveInfo } from "@/api/rules/device";
export default {
  mixins: [componentMinxins],
  name: "DevicPicker",
  components: { devPicker },
  props: {
    value: {
      type: Array,
      default: () => {
        return [];
      }
    },
    limit_product: {
      type: Array,
      default: () => {
        return [];
      }
    },
    placeholder: {
      type: String,
      default: "请选择设备"
    },
    limit: {
      type: Number,
      default: 2
    },
    synclist: {
      type: Array,
      default: () => {
        return [];
      }
    },
    disabled: {
      default: false,
      type: Boolean
    }
  },
  data() {
    return {
      showOrgSelect: false
    };
  },
  mounted() {
  },
  methods: {
    jumpToDevDetail(devinfo){
      //跳转至详情
      if(this.disabled){
        const ke = new KeyboardEvent('keydown', {
            bubbles: true, cancelable: true, keyCode: 27
        });
        document.body.dispatchEvent(ke);//模拟键盘按键esc，执行关闭窗口事件
        this.$router.push({
          path: "/iot/deviceManage/deviceDetail",
          query: { id: devinfo.id, isCustom: true },
        });
      }
    },
    showDialog() {
      this.showOrgSelect = true;
      this.$refs.devicePicker.show(this._value, "device");
    },
    selected(values) {
      this.showOrgSelect = false;
      this._value = values;

      if (values.length > 0) {
        DeviceInfo({ id: values[0].id }).then(rsp => {
          if (this.$isNotEmpty(rsp.data.DeviceId)) {
            return DeviceLiveInfo({ id: rsp.data.DeviceId, needTag: true });
          }
          else {
            return Promise.reject();
          }
        }).then(rsp => {
          if (this.synclist == null) return;

          this.synclist.forEach(x => {
            let tmpval = rsp.data.filter(z => z.Code == x.code);
            if (tmpval.length > 0) {
              this.valueModel[x.field_id] = tmpval[0].Value;
            }

          });

        });
      }


      this.$refs.devicePicker.show([], "device");
    },
    delDept(i) {
      if(this.disabled==true){
        return;
      }
      this._value.splice(i, 1);
    }
  }
};
</script>

<style scoped>
.placeholder {
  margin-left: 10px;
  color: #adabab;
  font-size: smaller;
}
</style>
