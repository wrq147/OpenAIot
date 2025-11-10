<template>
    <div class="fieldFormatting">
        <div class="left">
            <div class="field-item" v-for="(item, index) in keyList" :key="index">
                <div :class="['field-item-content', active === index ? 'field-item-active': '']" @click="fieldItemContent(item, index)">
                    <span :class="['select-option-type', item.type === '字符串' ? 'dataflow-string': item.type === '数字' ? 'dataflow-number' : 'dataflow-date']">{{ item.type }}</span>
                    <span class="field-name" :title="item.label">{{ item.label }}</span>
                </div>
            </div>
        </div>
        <div class="right">
            <div v-if="form.label !== undefined">
                <el-form ref="form" :model="form">
                    <el-form-item label="转换方式">
                        <el-select v-model="form.changeType" placeholder="请选择转换方式">
                            <el-option label="无" value="" />
                            <el-option label="替换值" value="字符串" />
                            <el-option label="时间格式化" value="时间" />
                            <!-- <el-option label="数字" value="数字" />
                            <el-option label="null" value="null" /> -->
                        </el-select>
                    </el-form-item>
                    <el-form-item v-if="form.changeType === '时间'" label="时间格式">
                        <el-select v-model="form.timeType" placeholder="请选择时间格式" @change="resetData">
                            <el-option label="YYYY年MM月DD日" value="YYYY年MM月DD日" />
                            <el-option label="YYYY年MM月DD日HH时mm分ss秒" value="YYYY年MM月DD日HH时mm分ss秒" />
                            <el-option label="YYYY年MM月DD日HH时mm分" value="YYYY年MM月DD日HH时mm分" />
                            <el-option label="YYYY-MM-DD HH:mm:ss" value="YYYY-MM-DD HH:mm:ss" />
                            <el-option label="YYYY-MM-DD HH:mm" value="YYYY-MM-DD HH:mm" />
                            <el-option label="YYYY-MM-DD" value="YYYY-MM-DD" />
                            <el-option label="YYYY/MM/DD" value="YYYY/MM/DD" />
                            <el-option label="YYYY年MM月" value="YYYY年MM月" />
                            <el-option label="YYYY-MM" value="YYYY-MM" />
                            <el-option label="YYYY/MM" value="YYYY/MM" />
                            <el-option label="MM-DD HH:mm:ss" value="MM-DD HH:mm:ss" />
                            <el-option label="MM月DD日HH时mm分ss秒" value="MM月DD日HH时mm分ss秒" />
                            <el-option label="MM-DD HH:mm" value="MM-DD HH:mm" />
                            <el-option label="MM月DD日HH时mm分" value="YYYY年MM月DD日HH时mm分" />
                        </el-select>
                    </el-form-item>
                    <el-form-item v-if="form.changeType === '字符串'" label="值替换">
                        <el-button type="text" @click="addItem">+ 新增替换</el-button>
                        <div class="addReplace" v-for="(item, index) in form.changeValue" :key="index">
                            <el-form-item label="替换值" class="formItem">
                                <el-input v-model="item.newValue" placeholder="替换值" />
                            </el-form-item>
                            <el-form-item label="原来值" class="formItem">
                                <el-input v-model="item.oldValue" placeholder="原来值" @change="resetData" />
                            </el-form-item>
                            <el-button style="margin-bottom: 6px;" size="mini" type="danger" round @click="remove(item)">删除</el-button>
                        </div>
                    </el-form-item>
                </el-form> 
                <!-- <div style="width: 100%;text-align: end;">
                    <el-button type="success" @click="resetData">刷新数据</el-button>
                </div> -->
            </div>
        </div>
    </div>
</template>
<script>
import moment from 'moment'
export default {
  name: 'fieldFormatting',
  props: {
    data: {
        type: Array
    },
    fielForm: {
        type: Array
    }
  },
  data() {
    return {
        keyList: [],
        form: {
            changeValue: []
        },
        active: ''
    }
  },
  watch: {
    data: {
        immediate: true,
        deep: true,
        handler() {
            this.disposeData();
        },
    },
    fielForm: {
        immediate: true,
        deep: true,
        handler() {
            if (this.fielForm.length === 0) {
                return false
            } else {
                this.fielForm.forEach((item,i)=>{
                    this.resetDataOpen(item)
                })
            }
        },
    }
  },
  methods: {
    disposeData() {
        this.keyList = []
        for (const key in this.data[0]) {
            let type = this.typeData(key)
            let array = { label: key, type: type, timeType: '', changeType: ''}
            this.keyList.push(array) 
        }
    },
    typeData(val) {
      let type = this.data[0][val] === null ? null : typeof this.data[0][val];
      if (type === 'string') {
        return this.isTime(this.data[0][val], type);
      } else if (type === null) {
        return 'null'
      } else {
        return type === 'number' ? '数字' : type
      }
    },
    isTime(data, type) {
        let asTime = !isNaN(Date.parse(data));
        return asTime ? '时间' : type === 'string' ? '字符串' : type
    },
    addItem() {
        this.form.changeValue.push({
            oldValue: '',
            newValue: ''
        });
    },
    remove(row) {
        this.form.changeValue.splice(this.form.changeValue.indexOf(row), 1);
    },
    fieldItemContent(item, index) {
        this.active = index;
        this.form = {
            changeType: '',
            timeType: '',
            label: item.label,
            changeValue: []
        }
    },
    resetData() {
        this.data.forEach(v => {
            switch (this.form.changeType) {
                case '字符串':
                    v[this.form.label] = v[this.form.label].toString();
                    break;
                // case '数字':
                //     v[this.form.label] = Number(v[this.form.label]);
                //     break;
                case '时间':
                   v[this.form.label] = moment(v[this.form.label]).format(this.form.timeType);
                    break;
                // case 'null':
                //    v[this.form.label] = 'null';
                //     break;
            }
            if (this.form.changeValue.length > 0) {
                this.form.changeValue.forEach(i => {
                    if (v[this.form.label] == i.oldValue) {
                        v[this.form.label] = !isNaN(i.newValue) ? Number(i.newValue) : i.newValue;
                    }   
                })
            }
        })
        this.$emit("getFieldForm", this.form)
        this.$message.success('数据格式刷新成功!')
    },
    resetDataOpen(item) {
        this.data.forEach(v => {
            switch (item.changeType) {
                case '字符串':
                    v[item.label] = v[item.label].toString();
                    break;
                case '时间':
                   v[item.label] = moment(v[item.label]).format(item.timeType);
                    break;
            }
            if (item.changeValue.length > 0) {
                item.changeValue.forEach(i => {
                    if (v[item.label] == i.oldValue) {
                        v[item.label] = !isNaN(i.newValue) ? Number(i.newValue) : i.newValue;
                    }   
                })
            }
        })
        this.$message.success('数据格式刷新成功!')
    }
  }
}
</script>
  
<style lang="scss" scoped>
.fieldFormatting{
    display: flex;
    max-height: 300px;
    overflow-y: auto;
}
.field-item{
    background-color: rgb(255, 255, 255);
    height: 50px;
    display: flex;
    align-items: center;
}
.field-item-content{
    box-sizing: border-box;
    display: flex;
    align-items: center;
    width: 100%;
    height: 32px;
    padding: 0px 16px;
    background: rgb(245, 246, 247);
    border-radius: 4px;
}
.field-item-active{
    background-color: #f7dfdf;
}
.select-option-type{
    font-size: 14px;
    width: 56px;
    height: 24px;
    line-height: 24px;
    text-align: center;
    box-sizing: border-box;
    font-weight: 500;
    text-overflow: ellipsis;
    padding: 0px 5px;
    border-radius: 4px;
    overflow: hidden;
}
.field-name{
    margin-left: 16px;
    text-overflow: ellipsis;
    color: rgb(54, 59, 76);
    font-size: 14px;
    white-space: nowrap;
    overflow: hidden;
}
.dataflow-date {
    color: rgb(142, 36, 249);
    background: rgba(142, 36, 249, 0.15);
}
.dataflow-string {
    color: rgb(20, 133, 246);
    background: rgba(20, 133, 246, 0.15);
}
.dataflow-number {
    color: rgb(244, 183, 0);
    background: rgba(244, 183, 0, 0.15);
}
.right{
    margin-left: 30px;
}
.addReplace{
    display: flex;
    align-items: flex-end;
}
::v-deep .formItem{
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    justify-content: flex-start;
    margin-right: 15px;
}
</style>