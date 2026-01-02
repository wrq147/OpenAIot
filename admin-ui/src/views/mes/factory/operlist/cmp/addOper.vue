<template>
    <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body
        width="900px" top="2vh" @close="cancel">
        <el-tabs v-model="dialogName" tab-position="top" :stretch="true"
            v-if="filedTableList && filedTableList.length > 0">
            <el-tab-pane name="null1" :disabled="true"><span slot="label"></span></el-tab-pane>
            <el-tab-pane name="null2" :disabled="true"><span slot="label"></span></el-tab-pane>
            <el-tab-pane name="custominfonull" :disabled="true"
                v-if="!filedTableList || filedTableList && filedTableList.length == 0"><span
                    slot="label"></span></el-tab-pane>
            <el-tab-pane name="baseinfo"><span slot="label">基本信息</span></el-tab-pane>
            <el-tab-pane name="custominfo"><span slot="label"
                    v-if="filedTableList && filedTableList.length > 0">自定义信息</span></el-tab-pane>
            <el-tab-pane name="null3" :disabled="true"><span slot="label"></span></el-tab-pane>
            <el-tab-pane name="null4" :disabled="true"><span slot="label"></span></el-tab-pane>
        </el-tabs>
        <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="140px" class="addPeople">
            <el-row v-show="dialogName === 'baseinfo'">
                <el-col :span="12">
                    <el-form-item label="工序名称" prop="operName">
                        <el-input v-model="ruleForm.operName" placeholder="请输入工序名称" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="报工数配比" prop="propOf">
                        <el-input v-model="ruleForm.propOf" type="number" placeholder="请输入报工数配比" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="工序使用的设备">
                        <el-button type="primary" size="mini" plain @click="equipOpen = true">
                            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                            <span style="margin-left: 6px">选择设备</span>
                        </el-button>
                        <div v-if="equipment.length > 0">
                            <el-tag v-for="(item, index) in equipment" :key="index" style="margin-right: 10px" closable
                                @close="removeEmployee('equipment', index)">
                                {{ item.Name }}
                            </el-tag>
                        </div>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="允许提交的人员">
                        <el-select v-model="employees" @click.native="selectUser()" value-key="name" class="select-u"
                            placeholder="请选择可以发起提交的人员" size="medium" clearable multiple>
                            <el-option v-for="(wc, index) in employees" :label="wc.name" :key="index" :value="wc" />
                        </el-select>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row v-show="dialogName === 'baseinfo'">
                <el-col :span="12">
                    <el-form-item label="预计工时(分钟)" prop="workTime">
                        <el-input v-model="ruleForm.workTime" type="number" placeholder="请输入预计工时(分钟)" />
                    </el-form-item>
                </el-col>
                <el-col :span="12" prop="unitPrice">
                    <el-form-item label="工资单价(元)" prop="unitPrice">
                        <el-input v-model="ruleForm.unitPrice" type="number" placeholder="请输入工资单价" />
                    </el-form-item>
                </el-col>
                <el-col :span="12" prop="priceMethod">
                    <el-form-item label="计件、计时" prop="priceMethod">
                        <el-select v-model="ruleForm.priceMethod" filterable clearable placeholder="请选择计件、计时">
                            <el-option label="计件" value="计件" />
                            <el-option label="计时" value="计时" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <el-form-item label="不良品项列表">
                        <el-select style="width: 80%;" v-model="ruleForm.DefectObject" value-key="Id" multiple
                            filterable remote reserve-keyword placeholder="请输入不良品项" :remote-method="defectRemoteMethod"
                            :loading="defectLoading">
                            <el-option v-for="item in defectOptions" :key="item.Id" :label="item.DefectName"
                                :value="item">{{ item.DefectName }}</el-option>
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <el-form-item label="报工表单权限">
                        <el-table :data="reportFieldsData" max-height="200" style="width: 100%;" row-key="Id"
                            class="data_table" border>
                            <el-table-column prop="name" label="表单字段" align="center" />
                            <el-table-column label="权限设置" align="center">
                                <template slot-scope="scope">
                                    <el-radio-group v-model="scope.row.perm">
                                        <el-radio :label="'R'">只读</el-radio>
                                        <el-radio :label="'E'">可编辑</el-radio>
                                        <el-radio :label="'H'">隐藏</el-radio>
                                    </el-radio-group>
                                </template>
                            </el-table-column>
                        </el-table>
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <el-form-item label="报工表单初始化配置">
                        <div style="margin-bottom: 10px;">
                            <el-button type="primary" icon="el-icon-plus" plain @click="addFieldsInit">添加一行</el-button>
                        </div>
                        <el-table :data="fieldsInit" max-height="200" style="width: 100%;" row-key="Id"
                            class="data_table" border>
                            <el-table-column label="工序表单字段" align="center">
                                <template slot-scope="scope">
                                    <el-select v-model="scope.row.sid" filterable clearable placeholder="请选择"
                                        @change="handleSourceChange(scope.$index, scope.row)">
                                        <el-option v-for="(item, index) in filteredSourceOptions(scope.row.tid)"
                                            :key="index" :label="item.name" :value="item.mapid" />
                                    </el-select>
                                </template>
                            </el-table-column>
                            <el-table-column label="报工表单字段" align="center">
                                <template slot-scope="scope">
                                    <el-select v-model="scope.row.tid" filterable clearable placeholder="请选择"
                                        @change="handleTargetChange(scope.$index, scope.row)">
                                        <el-option v-for="(item, index) in filteredTargetOptions(scope.row.sid)"
                                            :key="index" :label="item.name" :value="item.mapid" />
                                    </el-select>
                                </template>
                            </el-table-column>
                            <el-table-column label="操作" align="center">
                                <template slot-scope="scope">
                                    <el-button type="text" icon="el-icon-delete" style="color:red"
                                        @click="removeEmployee('fieldsInit', scope.$index)">删除</el-button>
                                </template>
                            </el-table-column>
                        </el-table>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row v-show="dialogName == 'custominfo'">
                <template v-for="(item, ix) in filedTableList">
                    <el-col :span="12" :key="'custom_filed' + ix" v-if="!inSetFormItemHide(item, ruleForm.ExtVals)">
                        <el-form-item :label="item.name" :prop="item.mapid">
                            <el-select @change="customValChange" :disabled="item.is_readonly"
                                :allow-create="item.is_add" :multiple="item.type === '复选框'"
                                :clearable="!item.is_required" v-model="ruleForm.ExtVals[item.mapid]"
                                :placeholder="item.prompt_text ? item.prompt_text : '请选择'" style="width: 100%"
                                v-if="(item.type === '单选框' && item.show_way === '下拉') || (item.type == '复选框' && item.show_way === '下拉')">
                                <el-option v-for="it in item.optionals" :label="it" :value="it"
                                    :key="it + ix"></el-option>
                            </el-select>
                            <el-radio-group @change="customValChange" :disabled="item.is_readonly"
                                v-model="ruleForm.ExtVals[item.mapid]"
                                v-if="item.type === '单选框' && item.show_way === '平铺'">
                                <el-radio v-for="it in item.optionals" :label="it" :key="it + ix">{{ it }}</el-radio>
                            </el-radio-group>
                            <el-checkbox-group @change="customValChange" :disabled="item.is_readonly"
                                v-model="ruleForm.ExtVals[item.mapid]"
                                v-if="item.type === '复选框' && item.show_way === '平铺'">
                                <el-checkbox v-for="it in item.optionals" :label="it" :key="it + ix">{{ it
                                }}</el-checkbox>
                            </el-checkbox-group>
                            <el-date-picker @change="customValChange" :disabled="item.is_readonly"
                                v-if="item.type === '时间'" v-model="ruleForm.ExtVals[item.mapid]" type="datetime"
                                :placeholder="item.prompt_text ? item.prompt_text : '请选择'" style="width: 100%"
                                :value-format="item.format" :format="item.format"></el-date-picker>
                            <el-input @input="customValChange" :disabled="item.is_readonly" v-if="item.type === '文本'"
                                :placeholder="item.prompt_text ? item.prompt_text : '请输入'"
                                :type="item.is_multiple ? 'textarea' : 'text'"
                                v-model="ruleForm.ExtVals[item.mapid]"></el-input>
                            <el-input @input="customValChange" :disabled="item.is_readonly" v-if="item.type === '数字'"
                                :placeholder="item.prompt_text ? item.prompt_text : '请输入'" type="number"
                                v-model="ruleForm.ExtVals[item.mapid]" :precision="item.decimals"></el-input>
                            <el-link :disabled="item.is_readonly" v-if="item.type === '超链接'" href="#" target="_blank">{{
                                item.describe_text }}</el-link>

                            <div class="avatar_con" v-if="item.type == '图片'">
                                <image-upload @input="customValChange($event, item)"
                                    v-model="ruleForm.ExtVals[item.mapid]" :limit="1" :isShowLeft="true">
                                    <template #tip>
                                        <div class="label_tip">
                                            <div class="label_text">　　</div>
                                            <div class="tip_con">
                                                <span style="margin-left:6px">请上传</span>
                                            </div>
                                        </div>
                                    </template>
                                </image-upload>
                            </div>
                            <file-upload @input="customValChange($event, item)" v-model="ruleForm.ExtVals[item.mapid]"
                                :limit="1" v-if="item.type == '附件'" :isShowLeft="true">
                                <template #tip>
                                    <div class="label_tip">
                                        <div class="label_text">　　</div>
                                        <div class="tip_con">
                                            <span style="margin-left:6px">请上传</span>
                                        </div>
                                    </div>
                                </template>
                            </file-upload>
                            <el-select @focus="afterValSearch(ruleForm.ExtVals[item.mapid], item)" :clearable="true"
                                @change="customValChange2($event, item)" style="width: 100%"
                                v-model="ruleForm.ExtVals[item.mapid]" filterable remote reserve-keyword
                                :placeholder="item.prompt_text ? item.prompt_text : '请选择'"
                                :remote-method="(query) => associationMethod(query, item)" :loading="Supplierloading"
                                v-if="item.type === '关联对象'">
                                <el-option v-for="ite in associationObject[item.mapid]" :key="ite.Value"
                                    :label="ite.Name" :value="ite.Value + ',' + ite.ValueName">{{ ite.Name
                                    }}</el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>
                </template>
            </el-row>
        </el-form>
        <span slot="footer" class="dialog-footer">
            <el-button @click="cancel">取消</el-button>
            <el-button type="primary" @click="submitForm('ruleForm')">确定</el-button>
        </span>
        <!-- 新增人员弹窗 -->
        <add-People ref="addGroupPeople" multiple @ok="getSelectPeople" />
        <add-Equipment ref="addEquipment" :dialog-visible="equipOpen" :equipment="equipment" @cancelForm="cancelForm"
            @getSelectEquip="getSelectEquip" />
    </el-dialog>
</template>
<script>
import { orgFormFields } from "@/api/factory/customFields";
import { operAdd, operEdit } from "@/api/mes/oper";
import { myDeviceList } from "@/api/after/dev";
import { factorySearchObject } from "@/api/factory/product";
import addPeople from '@/views/flowable/common/OrgPicker.vue';
import addEquipment from './addEquipment.vue'
import { defectList } from "@/api/mes/defect";
import { setCustomDefaultValue, checkBeforeSave, setFormItemHide } from '@/utils/field.js'
export default {
    name: 'addDefect',
    components: {
        addPeople,
        addEquipment
    },
    props: {
        dialogVisible: {
            type: Boolean
        },
        title: {
            type: String
        },
        supplierFiledList: {
            type: Array,
            default: () => {
                return []
            }
        }
    },
    data() {
        return {
            dialogName: "baseinfo",
            dialogFlag: false,
            equipOpen: false,
            // 表单
            ruleForm: {},
            employees: [],
            equipment: [],
            // 校验
            rules: {
                operName: [
                    { required: true, message: "请输入工序名称", trigger: "blur" },
                ],
                propOf: [
                    { required: true, message: "请输入报工数配比", trigger: "blur" },
                ],
                workTime: [
                    { required: true, message: "请输入预计工时(分钟)", trigger: "blur" },
                ],
                unitPrice: [
                    { required: true, message: "请输入工资单价", trigger: "blur" },
                ],
                priceMethod: [
                    { required: true, message: "请选择计件、计时", trigger: "change" },
                ]
            },
            filedTableList: [],
            reportFieldsData: [],
            filedListData: [],
            fieldsInit: [],
            associationObject: {},//所有关联对象对应的下拉的参数列表
            defectLoading: false,
            defectOptions: []
        }
    },
    watch: {
        dialogVisible(newValue) {
            this.dialogFlag = newValue;
            this.reportFieldsData = [];
            this.getOrgFormFields('报工');
            if (this.dialogFlag == true) {
                this.defectRemoteMethod('');
            }
        }
    },
    methods: {
        inSetFormItemHide(item, extdata) {
            setFormItemHide(item, extdata);
        },
        //关联对象回显时获取列表
        afterValSearch(val, item) {
            if (val && val.indexOf(',') > -1) {
                let keyVal = val.split(',')
                this.associationMethod(keyVal[1], item)
            } else {
                this.associationMethod('', item)
            }
        },
        //关联对象的远程搜索事件
        associationMethod(query, item) {
            this.getFactorySearchObject(query, item.object_type, item.mapid);
        },
        async getFactorySearchObject(key, objtype, mapid) {
            //根据不同的关联对象获取对象的列表
            let obj = {
                key: key,
                objtype: objtype,
                pageNum: 1,
                pageSize: 10
            }
            let res = await factorySearchObject(obj)
            if (res.data.List) {
                // console.log("res.data.List",res.data.List);
                this.associationObject[mapid] = JSON.parse(JSON.stringify(res.data.List))
            }
            this.$forceUpdate()
            // console.log(res,'resres');
        },
        // 获取组织表单字段
        getOrgFormFields(type) {
            orgFormFields({ isfixed: true, ext: true, field: type }).then((res) => {
                this.filedListData = res.data;
                let filedList = res.data;
                // 处理现有字段数据
                let existingFields = this.ruleForm.reportFields !== ''
                    ? JSON.parse(this.ruleForm.reportFields)
                    : [];

                // 创建现有字段的ID映射表，便于快速查找
                const existingIds = new Set(existingFields.map(item => item.id));

                // 合并字段：添加现有字段中不存在的新字段
                const mergedFields = [
                    ...existingFields,
                    ...filedList
                        .filter(item => !existingIds.has(item.mapid)) // 过滤掉已存在的ID
                        .map(item => ({ id: item.mapid, name: item.name, perm: 'R' })) // 映射为目标格式
                ];

                this.reportFieldsData = mergedFields;
            })
        },

        async getProductCustomFiled(afterForm, type) {
            //获取自定义的字段
            this.filedTableList = [];
            let res = await orgFormFields({ field: "工序", ext: true, isfixed: false });
            this.filedTableList = res.data;
            setCustomDefaultValue(this.filedTableList, this.ruleForm, this.rules, afterForm);

        },

        submitForm(formName) {
            this.$refs[formName].validate((valid) => {
                if (valid) {
                    let submitForm = JSON.parse(JSON.stringify(this.ruleForm));
                    submitForm.DefectJson = JSON.stringify(submitForm.DefectObject);
                    checkBeforeSave(this.filedTableList, submitForm);
                    submitForm.reportFields = JSON.stringify(this.reportFieldsData);
                    submitForm.fieldsInit = JSON.stringify(this.fieldsInit);
                    if (this.title === '新增生产工序') {
                        this.getAddRpt(submitForm);
                    } else {
                        this.setEditRpt(submitForm);
                    }
                } else {
                    return false
                }
            })
        },

        customValChange() {
            let ruleForm = JSON.parse(JSON.stringify(this.ruleForm));
            this.ruleForm = JSON.parse(JSON.stringify(ruleForm));
            this.$refs["ruleForm"].validate((valid) => { });
            this.$forceUpdate();
        },

        //数据发生变化后刷新，并验证表单
        customValChange2(val, fidItem) {
            let ruleForm = JSON.parse(JSON.stringify(this.ruleForm));
            this.ruleForm = JSON.parse(JSON.stringify(ruleForm));
            if (fidItem && fidItem.type === '关联对象' && this.ruleForm[fidItem.mapid]) {
                if (fidItem.items && fidItem.items.length > 0) {
                    let fieldMapidVal = this.ruleForm[fidItem.mapid].split(',')
                    let findObj = this.associationObject[fidItem.mapid].find(row => row.Value == fieldMapidVal[0])
                    for (let i = 0; i < fidItem.items.length; i++) {
                        let item = fidItem.items[i]
                        this.ruleForm[item.field] = findObj.Obj[item.source_obj]
                    }
                }
            }
            let form2 = JSON.parse(JSON.stringify(this.ruleForm));
            this.ruleForm = JSON.parse(JSON.stringify(form2));
            this.$nextTick(() => {
                if (fidItem && fidItem.type === '关联对象' || fidItem.type === '图片') {
                    this.$refs["form"].validate((valid) => { });
                }
                // this.$refs["form"].validate((valid) => {});
                this.$forceUpdate();
            })
        },

        getAddRpt(submitForm) {
            operAdd(submitForm).then(res => {
                this.$message.success('添加成功!');
                this.$emit('getList');
            })
        },

        setEditRpt(submitForm) {
            operEdit(submitForm).then(res => {
                this.$message.success('修改成功!');
                this.$emit('getList');
            })
        },

        selectUser() {
            this.$refs.addGroupPeople.show(this.employees);
        },
        // 加载设备列表方法
        loadDeviceList(deviceIds) {
            // 处理设备ID参数（支持字符串和数组）
            const idList = deviceIds.split(',');

            // 确保ID列表非空
            if (idList.length === 0) {
                this.equipment = [];
                return;
            }

            // 加载全部设备列表
            myDeviceList({ pageNum: 1, pageSize: 9999 }).then(response => {
                if (!response.data || !response.data.List) {
                    this.equipment = [];
                    return;
                }

                // 将ID转换为集合用于快速查询
                const idSet = new Set(idList);
                const allDevices = response.data.List;

                // 过滤出匹配的设备
                this.equipment = allDevices.filter(device => idSet.has(String(device.Id)));
            }).catch(error => {
                console.error('加载设备列表失败', error);
                this.equipment = [];
            });
        },

        // 获取选择的人员方法
        getSelectPeople(list) {
            this.employees = [];
            list.forEach(item => {
                this.employees.push(item);
            });
            this.ruleForm.assignedUser = JSON.stringify(this.employees);
        },

        // 获取选择设备的方法
        getSelectEquip(list) {
            this.equipment = list;
            this.ruleForm.deviceIds = this.equipment.map(emp => emp.Id).join(',');
        },

        // 从数组中移除该员工
        removeEmployee(property, index) {
            if (property === 'equipment') {
                this.equipment.splice(index, 1);
                return;
            } else if (property === 'employees') {
                this.employees.splice(index, 1);
                return;
            } else {
                this.fieldsInit.splice(index, 1);
            }
        },

        // 过滤工序字段选项，仅显示与当前选择的报工字段类型一致的选项
        filteredSourceOptions(targetId) {
            if (!targetId) return this.supplierFiledList;

            const targetItem = this.filedListData.find(item => item.mapid === targetId);
            if (!targetItem) return this.supplierFiledList;

            return this.supplierFiledList.filter(item => item.type === targetItem.type);
        },

        // 过滤报工字段选项，仅显示与当前选择的工序字段类型一致的选项
        filteredTargetOptions(sourceId) {
            if (!sourceId) return this.filedListData;

            const sourceItem = this.supplierFiledList.find(item => item.mapid === sourceId);
            if (!sourceItem) return this.filedListData;

            return this.filedListData.filter(item => item.type === sourceItem.type);
        },

        // 处理工序字段选择变化
        handleSourceChange(index, row) {
            // 如果新选择的工序字段类型与当前报工字段类型不一致，清空报工字段
            if (row.tid) {
                const sourceItem = this.supplierFiledList.find(item => item.mapid === row.sid);
                const targetItem = this.filedListData.find(item => item.mapid === row.tid);

                if (sourceItem && targetItem && sourceItem.type !== targetItem.type) {
                    row.tid = null;
                }
            }
        },

        // 处理报工字段选择变化
        handleTargetChange(index, row) {
            // 如果新选择的报工字段类型与当前工序字段类型不一致，清空工序字段
            if (row.sid) {
                const sourceItem = this.supplierFiledList.find(item => item.mapid === row.sid);
                const targetItem = this.filedListData.find(item => item.mapid === row.tid);

                if (sourceItem && targetItem && sourceItem.type !== targetItem.type) {
                    row.sid = null;
                }
            }
        },

        // 从数组中添加一行报工表单初始化
        addFieldsInit() {
            this.fieldsInit.push({
                sid: '', // 工序表单字段ID
                tid: '', // 报工表单字段ID
            });
        },

        cancel() {
            this.$emit('cancelForm')
        },
        cancelForm() {
            this.peopleOpen = false;
            this.equipOpen = false;
        },
        async defectRemoteMethod(key) {
            this.defectLoading = true
            let obj = {
                Key: key,
                pageNum: 1,
                pageSize: 30
            }
            let res = await defectList(obj)
            if (res.data.List) {
                this.defectOptions = JSON.parse(JSON.stringify(res.data.List))
            }
            this.defectLoading = false
        },
    }
}
</script>

<style lang="scss" scoped>
::v-deep {
    .data_table.el-table .el-table__header-wrapper thead th {
        padding: 5px 0px;
    }

    .el-dialog__body {
        padding-top: 10px;
    }

    .el-dialog__header {
        border-bottom: 1px solid #ccc;
    }

    .el-select,
    .el-cascader {
        width: 100%;
    }
}

.addPeople>.box {
    display: flex;
    align-items: center;
    justify-content: space-between;
    flex-wrap: wrap;
}

.addPeople>.btn {
    width: 100%;
    justify-content: flex-end;
    display: flex;
    align-items: center;
}

.avatar_con {
    width: 100%;
    text-align: center;
    display: flex;
    align-items: flex-start;

    .label_tip {
        height: 40px;
        text-align: left;

        .label_text {
            height: 8px;
        }
    }

    .tip_con {
        width: 182px;
        height: 30px;
        border: 1px solid rgba(223, 226, 234, 1);
        color: rgba(120, 130, 157, 1);
        line-height: 30px;
        margin-right: 10px;
        text-align: center;
        border-radius: 4px;

        .zhongtaiiconfont {
            font-size: 10px;
        }
    }

    .el-upload--picture-card {
        background-color: #202e57;
        border: none;
    }

    ::v-deep .el-upload--picture-card i {
        font-size: 16px;
    }

    ::v-deep .el-upload.el-upload--picture-card {
        width: 70px;
        height: 40px;
        line-height: 40px;
    }

    ::v-deep .component-upload-image {
        height: 40px;

        // margin-bottom: 20px;
        .el-upload__tip {
            margin-top: 0;
        }
    }

    ::v-deep .el-upload-list--picture-card .el-upload-list__item {
        width: 70px;
        height: 40px;
    }
}
</style>