<template>
  <div class="config-item field-box">
    <el-form
      action=""
      label-width="90px"
      label-position="top"
      class="custom_form_item"
    >
      <el-form-item label="数据源">
        <el-select v-model="configData.globalType" placeholder="请选择" @change="changeSource">
          <el-option
            v-for="(item, index) in dataSourceOptions"
            :key="index"
            :label="item.name"
            :value="index"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="过滤器">
        <el-select v-model="configData.filtrationId" placeholder="请选择" @change="filtrationChange">
          <el-option
            v-for="(item, index) in filtrationData"
            :key="index"
            :label="item.title"
            :value="index"
          />
        </el-select>
      </el-form-item>
    </el-form>
    <div class="config-item-title">
      <div class="title">字段</div>
      <div class="edit-btn">
        <el-button size="mini" icon="el-icon-refresh-right" type="text"
          >刷新</el-button
        >
      </div>
    </div>
    <div class="field-items-box" style="height: calc(-331px + 100vh)">
      <el-scrollbar>
        <div
          class="field-class-item"
          v-for="(item, index) in keyList"
          :key="index"
        >
          <div class="field-class-name" @click="toggleContent(index)">
            <i v-if="item.isContentVisible" class="el-icon-caret-bottom" />
            <i v-else class="el-icon-caret-right" />
            <span>{{ item.name }}({{ item.data.length }})</span>
          </div>
          <div
            :class="['list-box', item.isContentVisible ? 'list-box-open' : '']"
          >
            <div class="list-items">
              <div
                v-for="(v, index) in item.data"
                :key="index"
                :class="['list-item', item.name==='字符串'? 'field-item-chart': item.name==='时间'? 'field-item-timer': item.name==='数字'? 'field-item-number': '']"
                draggable="true"
                @mousedown="startTimer(v)"
                @mousseup="clearTimer"
              >
                <div class="point"></div>
                <div class="name">{{ v.fieldName }}</div>
              </div>
            </div>
          </div>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import moment from 'moment'
export default {
  name: "fieldFormat",
  props: {
    configData: {
      type: Object,
      default: () => ({
        // 示例数据
      }),
    },
  },
  watch: {
    configData: {
      immediate: true,
      deep: true,
      handler() {
        let data = this.configData;
        this.dataSourceOptions = data.globalData;
        if (data.globalType!==undefined && data && data.globalData) {
          setTimeout(() => {
            this.changeSource(data.globalType, true)
          }, 100)
        }
      }
    },
    keyList(newVal, oldVal) {
        if (this.isInitialized) {
          this.$emit('getKeyList', this.keyList, this.configData.globalType, this.configData.filtrationId )
        } else {
            this.isInitialized = true;
        }
    }
  },
  data() {
    return {
      dataSourceOptions: [],
      timer: 0,
      filtrationData: [],
      data: [],
      keyList: [],
      formatTime: [
        { momentFormat: 'YYYY', useFormat: 'yyyy' },
        { momentFormat: 'YYYY-MM-DD', useFormat: 'yyyy-MM-dd' },
        { momentFormat: 'YYYY-MM-DD HH', useFormat: 'yyyy-MM-dd HH' },
        { momentFormat: 'YYYY-MM-DD HH:mm', useFormat: 'yyyy-MM-dd HH:mm' },
        { momentFormat: 'YYYY-MM-DD HH:mm:ss', useFormat: 'yyyy-MM-dd HH:mm:ss' },
      ],
      isInitialized: false
    };
  },
  methods: {
    disposeData(data) {
      for (const key in data) {
        let type = this.typeData(key);
        if (this.keyList.length > 0) {
          if (this.keyList.some((v) => v.name === type)) {
            this.keyList.forEach((i) => {
              if (i.name === type) {
                i.data.push(this.dataKey(key, type));
              }
            });
          } else {
            this.keyListPush(key, type)
          }
        } else {
          this.keyListPush(key, type)
        }
      }
    },
    typeData(val) {
      let type = typeof this.data[val];
      if (type === "string") {
        return this.isTime(this.data[val], type);
      } else {
        return type === "number" ? "数字" : type;
      }
    },
    isTime(data, type) {
      let asTime = !isNaN(Date.parse(data));
      return asTime ? "时间" : type === "string" ? "字符串" : type;
    },
    // keyList中push数据
    keyListPush(key, type){
      this.keyList.push({
        name: type,
        isContentVisible: true,
        data: [this.dataKey(key, type)]
      });
    },
    // keyListPush中子元素格式
    dataKey(key, type) {
      return {
          fieldName: key,
          fieldType: type,
          columnCount: key,
          formatDefault: '',
          format: type === '时间' ? this.detectionFormatTime(key) : ''
        }
    },
    // 检测时间格式
    detectionFormatTime (key) {
      const timeStr = this.data[key]
      for (let i = 0; i < this.formatTime.length; i++) {
        const isValidTime = moment(timeStr, this.formatTime[i].momentFormat, true).isValid();
        if (isValidTime) {
          return this.formatTime[i].useFormat;
        }
      }
      return ''
    },
    // 数据源改变
    changeSource(e, isFiltrationId) {
      this.configData.globalType = e;
      if(isFiltrationId === undefined) {
        this.configData.filtrationId = 0
      }
      this.filtrationData =
      this.dataSourceOptions[e].rawData != undefined
          ? JSON.parse(this.dataSourceOptions[e].rawData)
          : [];
      this.filtrationChange()
    },
    // 过滤器改变
    filtrationChange(e) {
      let index = e ? e : this.configData.filtrationId ? this.configData.filtrationId : 0;
      this.keyList = [];
      this.data = this.filtrationData[index].content[0];
      this.disposeData(this.filtrationData[index].content[0]);
    },
    toggleContent(index) {
      this.keyList[index].isContentVisible =
        !this.keyList[index].isContentVisible;
    },
    startTimer(item) {
      let that = this;
      this.timer = setTimeout(() => {
        that.$emit("fieldData", item);
      }, 500);
    },
    clearTimer() {
      clearTimeout(this.timer);
      this.timer = 0;
    },
  },
};
</script>
<style lang="scss" scoped>
::v-deep {
  .el-form--label-top .el-form-item__label {
    padding: 0;
  }
  .el-form-item {
    margin-bottom: 0;
  }
  .el-scrollbar{
    height: 100%;
    --el-scrollbar-opacity: .3;
    --el-scrollbar-bg-color: #909399;
    --el-scrollbar-hover-opacity: .5;
    --el-scrollbar-hover-bg-color: #909399;
  }
  .el-scrollbar__wrap{
    scrollbar-width: none;
  }
  .el-scrollbar__bar{
    display: none;
  }
}
.config-item {
  padding: 16px;
  box-sizing: border-box;
  border-bottom: 1px solid #eeeff0;
  position: relative;
}
.edit-btn {
  display: flex;
  align-items: center;
  right: 16px;
  position: absolute;
  cursor: pointer;
}
.field-box {
  padding: 0 16px;
  border-bottom: 0px solid #eeeff0;
}
.config-item-title {
  padding-top: 16px;
  -webkit-user-select: none;
  user-select: none;
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 8px;
}
.config-item-title .title {
  font-size: 14px;
  font-weight: bold;
  color: #363b4c;
  font-family: SourceHanSansCN-Bold !important;
}
.field-class-name {
  cursor: pointer;
  -webkit-user-select: none;
  user-select: none;
  margin-bottom: 10px;
}
.field-class-name span {
  color: #6f7588;
  font-weight: 500;
  font-size: 14px;
  font-family: SourceHanSansCN-Bold !important;
}

.field-class-item .list-box {
  display: grid;
  flex-direction: column;
  grid-template-rows: 0fr;
  overflow: hidden;
  transition: all 0.4s;
  margin-right: 12px;
}
.field-class-item .list-box-open {
  grid-template-rows: 1fr;
}
.list-box .list-items {
  overflow: hidden;
}
.list-box .list-items .list-item {
  display: flex;
  align-items: center;
  padding: 10px 0 10px 12px;
  height: 36px;
  line-height: 36px;
  cursor: pointer;
  -webkit-user-select: none;
  user-select: none;
  border-radius: 4px;
}
.list-box .list-items .list-item:hover {
  background-color: #f5f6f7;
}
.list-item .point {
  width: 10px;
  height: 10px;
  min-width: 10px;
  border-radius: 50%;
  margin-right: 8px;
}

.list-box .list-items .point{
  background-color: #f74fbf;
}
.list-box .field-item-chart .point {
  background-color: #1e6fff;
}
.list-box .field-item-number .point {
  background-color: #36b452;
}
.list-box .field-item-timer .point {
  background-color: #ff9736;
}
.list-items .list-item .name {
  color: #363b4c;
  font-size: 14px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
