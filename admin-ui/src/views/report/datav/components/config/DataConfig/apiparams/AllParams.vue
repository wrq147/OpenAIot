<template>
    <div>
        <el-form ref="form" :model="form" label-width="80px"
            v-if="activeComponentParams.formKey && activeComponentParams.formKey.length > 0" :rules="rules">
            <template v-for="(item, inx) in activeComponentParams.formKey">
                <el-form-item :label="activeComponentParams[item].text" :key="'param' + item + inx"
                    v-if="activeComponentParams[item]" :prop="item">


                    <el-select
                        :multiple="activeComponentParams[item] && activeComponentParams[item].multiple ? true : false"
                        v-if="activeComponentParams[item].type == 'remoteSelect' && activeComponentParams[item].name == 'deviceOptions'"
                        @change="onDeviceChange($event, item)" v-model="form[item]" clearable filterable remote
                        reserve-keyword :placeholder="activeComponentParams[item].placeholder"
                        :remote-method="deviceRemoteMethod" @clear="deviceRemoteMethod('')"
                        @remove-tag="deviceRemoteMethod('')" :loading="loading">
                        <el-option v-for="optionItem in deviceOptions"
                            :key="optionItem[activeComponentParams[item].key]"
                            :label="optionItem[activeComponentParams[item].label]"
                            :value="optionItem[activeComponentParams[item].value]"></el-option>
                    </el-select>

                    <el-select
                        :multiple="activeComponentParams[item] && activeComponentParams[item].multiple ? true : false"
                        v-if="activeComponentParams[item].type == 'optionselect' && activeComponentParams[item].optionsArr && activeComponentParams[item].optionsArr.length > 0"
                        v-model="form[item]" :placeholder="activeComponentParams[item].placeholder">
                        <el-option v-for="optionItem in activeComponentParams[item].optionsArr"
                            :key="optionItem[activeComponentParams[item].key]"
                            :label="optionItem[activeComponentParams[item].label]"
                            :value="optionItem[activeComponentParams[item].value]">
                        </el-option>
                    </el-select>

                    <el-select
                        v-if="activeComponentParams[item].type == 'select' && activeComponentParams[item].name == 'propOptions'"
                        v-model="form[item]" :placeholder="activeComponentParams[item].placeholder">
                        <el-option v-for="optionItem in propOptions" :key="optionItem[activeComponentParams[item].key]"
                            :label="optionItem[activeComponentParams[item].label]"
                            :value="optionItem[activeComponentParams[item].value]">
                        </el-option>
                    </el-select>


                    
                    <el-select
                        v-if="activeComponentParams[item].type == 'remoteSelect' && activeComponentParams[item].name == 'rulesOptions'"
                        v-model="form[item]" filterable remote reserve-keyword clearable
                        :placeholder="activeComponentParams[item].placeholder" :remote-method="loadRulesList"
                        @clear="loadRulesList('')" @remove-tag="loadRulesList('')" :loading="loading">
                        <el-option v-for="optionItem in rulesOptions" :key="optionItem[activeComponentParams[item].key]"
                            :label="optionItem[activeComponentParams[item].label]"
                            :value="optionItem[activeComponentParams[item].value]">
                        </el-option>
                    </el-select>

                    <el-select
                        v-if="activeComponentParams[item].type == 'remoteSelect' && activeComponentParams[item].name == 'roomOptions'"
                        v-model="form[item]" filterable remote reserve-keyword clearable
                        :placeholder="activeComponentParams[item].placeholder" :remote-method="loadRoomList"
                        @clear="loadRoomList('')" @remove-tag="loadRoomList('')" :loading="loading">
                        <el-option v-for="optionItem in roomOptions" :key="optionItem[activeComponentParams[item].key]"
                            :label="optionItem[activeComponentParams[item].label]"
                            :value="optionItem[activeComponentParams[item].value]">
                        </el-option>
                    </el-select>

                    <el-select
                        v-if="activeComponentParams[item].type == 'remoteSelect' && activeComponentParams[item].name == 'planeOptions'"
                        v-model="form[item]" clearable filterable reserve-keyword
                        :placeholder="activeComponentParams[item].placeholder" :loading="loading">
                        <el-option v-for="optionItem in planeOptions" :key="optionItem[activeComponentParams[item].key]"
                            :label="optionItem[activeComponentParams[item].label]"
                            :value="optionItem[activeComponentParams[item].value]">
                        </el-option>
                    </el-select>

                    <el-date-picker style="width: 100%;" v-if="item == 'dateRange'" v-model="dateRange"
                        :type="activeComponentParams.dateParams.type" range-separator="至"
                        :start-placeholder="activeComponentParams.dateParams.timeText[0]"
                        :end-placeholder="activeComponentParams.dateParams.timeText[0]">
                    </el-date-picker>

                    <el-input v-if="activeComponentParams[item].type == 'inputText'" class="inputText"
                        v-model="form[item]" :placeholder="activeComponentParams[item].placeholder"></el-input>

                    <el-input-number v-if="activeComponentParams[item].type == 'number'" v-model="form[item]"
                        :min="activeComponentParams[item].min"
                        :step="activeComponentParams[item].step"></el-input-number>

                    <el-radio-group v-if="activeComponentParams[item].type == 'radio'" v-model="form[item]">
                        <template v-for="(it, inx) in activeComponentParams[item].list">
                            <el-radio :label="it.label" style="margin-top:5px" v-if="it.hasmargin"
                                :key="'radio' + inx">{{ it.text }}</el-radio>
                            <el-radio :key="'radio' + inx" v-else :label="it.label">{{ it.text }}</el-radio>
                        </template>
                    </el-radio-group>
                </el-form-item>
            </template>
            <div style="margin-top: 20px;display: flex;flex-direction: row;justify-content: right;">
                <el-button plain type="primary" @click="onImport('form')">导 入</el-button>
                <!-- <el-button plain @click="onCancel">取 消</el-button> -->
            </div>
        </el-form>
    </div>
</template>

<script>
import { DeviceList } from "@/api/rules/device";
import {
    productInfo
} from "@/api/rules/productModel";
import {
    DeviceHistory,
    RuleLive,
    CommonPag,
    GetDeviceList,
    GetTimeData,
    GetDeviceTimeData,
    GetRuleList,
    GetRuleListDetail,
    GetDeviceTagList,
    PlaneTaskStatis,
    PlaneTaskStatisList,
    SelectMergeList
}
    from '@/views/report/datav/components/config/DataConfig/apiparams/AllParams.js'
import { rulesList } from "@/api/rules/ruselSevic";
import { devPlaneList } from "@/api/after/devplane";
import { deviceRoomList } from "@/api/after/room";
export default {
    name: "ApiImport",
    props: {
        devValue: {
            type: String,
            default: ''
        },
    },
    data() {
        return {
            loading: false,
            filterParams: {},//执行获取列表的相关函数时的过滤参数
            deviceOptions: [],//设备的下拉选项
            propOptions: [],//属性的下拉选项
            rulesOptions: [],//规则列表的下拉选项
            planeOptions: [],//计划列表的下拉选项
            mergeWayOptions: [],//计划列表的下拉选项
            windowWayOptions: [],//计划列表的下拉选项
            roomOptions: [],//房间下拉选项
            // 日期范围
            dateRange: [],
            form: {},//表单的参数
            activeComponentParams: {},//当前接口的相关参数
            rules: {},//提交接口参数的验证规则
        };
    },
    computed: {
    },
    watch: {
        devValue: {
            handler(val) {
                console.log("数据发生变化", val);
                if (val && val == 'Api01') {
                    this.setStartForm(RuleLive)
                }
                if (val && val == 'Api04') {
                    this.setStartForm(CommonPag)
                }
                if (val && val == 'Api05') {
                    this.setStartForm(GetDeviceList)
                }
                if (val && val == 'Api06') {
                    this.setStartForm(GetDeviceTimeData)
                }
                if (val && val == 'Api07') {
                    this.setStartForm(DeviceHistory)
                }
                if (val && val == 'Api08') {
                    this.setStartForm(GetRuleList)
                }
                if (val && val == 'Api09') {
                    this.setStartForm(GetRuleListDetail)
                }
                if (val && val == 'Api10') {
                    this.setStartForm(GetTimeData)
                }
                if (val && val == 'Api11') {
                    this.setStartForm(GetDeviceTagList)
                }
                if (val && val == 'Api12') {
                    this.setStartForm(GetDeviceTagList)
                }
                if (val && val == 'Api13') {
                    this.setStartForm(PlaneTaskStatis)
                }
                if (val && val == 'Api15') {
                    this.setStartForm(PlaneTaskStatisList)
                }
                if (val && val == 'Api16') {
                    this.setStartForm(SelectMergeList)
                }
            },
            immediate: true
        }
    },
    created() {
    },
    mounted() {
        // this.remoteMethod('');
    },
    methods: {
        setStartForm(params) {//接口参数初始化
            this.activeComponentParams = JSON.parse(JSON.stringify(params))
            this.form = JSON.parse(JSON.stringify(params.form))
            if (params.isshowfilterParams) {
                this.filterParams = params.filterParams
                if (params.filterType == 'deviceList') {
                    this.deviceRemoteMethod('')
                }
                if (params.filterType == 'rulesList') {
                    this.loadRulesList('')
                }
                if (params.filterType == 'roomList') {
                    this.loadRoomList('');
                }
                if (params.filterType == 'planeList') {
                    this.loadDevPlaneList('')
                }
            }
            if (params.rules) {
                this.rules = JSON.parse(JSON.stringify(params.rules))
            } else {
                this.rules = {}
            }
            if (params.dateParams) {

            } else {
                this.dateRange = []
            }
        },
        loadDevPlaneList(query) {
            this.loading = true;
            devPlaneList(this.filterParams).then(rsp => {
                console.log('计划类型', rsp);
                this.loading = false;
                this.planeOptions = rsp.data.List;
            }).catch(err => {
                this.loading = false;
                this.$message.error("接口异常");
            });
        },
        loadRoomList(query) {
            this.loading = true;
            if (query) {
                this.filterParams.Name = query;
            } else {
                delete this.filterParams.Name
            }
            deviceRoomList(this.filterParams).then(rsp => {
                this.loading = false;
                this.roomOptions = rsp.data.List;
            }).catch(err => {
                this.loading = false;
                this.$message.error("接口异常");
            });
        },
        deviceRemoteMethod(query) {//设备列表的远程查询
            this.loading = true;
            if (query) {
                this.filterParams.Key = query;
            } else {
                delete this.filterParams.Key
            }
            DeviceList(this.filterParams)
                .then(rsp => {
                    this.loading = false;
                    this.deviceOptions = rsp.data.List;
                })
                .catch(err => {
                    this.loading = false;
                    this.$message.error("接口异常");
                });
        },
        async onDeviceChange(val, itemParam) {//选择的设备发生了变化
            if (this.activeComponentParams[itemParam].changeFunc && this.activeComponentParams[itemParam].changeFunc == 'properties') {
                let dvInfo = this.deviceOptions.filter(x => x.DeviceNumber == val)[0];
                let rsp = await productInfo({ id: dvInfo.ProductId });
                let msl = JSON.parse(rsp.data.ModelTSL);
                this.propOptions = msl.properties;
            }

        },
        loadRulesList(query) {
            this.loading = true;
            if (query) {
                this.filterParams.key = query;
            } else {
                delete this.filterParams.key
            }
            rulesList(this.filterParams).then(rsp => {
                this.loading = false;
                this.rulesOptions = rsp.data.List;
            })
                .catch(err => {
                    this.loading = false;
                    this.$message.error("接口异常");
                });
        },

        onImport(form) {
            this.$refs[form].validate((valid) => {
                if (valid) {
                    if (this.dateRange.length > 0) {
                        this.form[this.activeComponentParams.dateParams.timeParams[0]] = this.parseTime(this.dateRange[0]);
                        this.form[this.activeComponentParams.dateParams.timeParams[1]] = this.parseTime(this.dateRange[1]);
                    } else {
                        if (this.activeComponentParams.dateParams) {
                            this.form[this.activeComponentParams.dateParams.timeParams[0]] = '';
                            this.form[this.activeComponentParams.dateParams.timeParams[1]] = '';
                        }

                    }
                    this.$emit("ok", this.form);
                } else {
                    return false;
                }
            });

        },
        onCancel() {
            this.$emit("cancel");
        }
    }
};
</script>
<style lang="scss"></style>