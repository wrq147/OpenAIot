<template>
    <el-form ref="form" :model="formData" label-width="80px" :rules="formRules">
        <!-- 自动渲染所有表单项 -->
        <el-form-item v-for="(config, key) in apiConfig.params" label-width="120px" :key="key" :label="config.label" :prop="key">

            <!-- 输入框 -->
            <el-input v-if="config.type === 'input' || config.type === 'inputText'" v-model="formData[key]"
                :placeholder="`请输入${config.label}`" style="width: 100%;"></el-input>

            <!-- 数字输入 -->
            <el-input-number v-else-if="config.type === 'number'" v-model="formData[key]" :min="config.min || 0"
                :step="config.step || 1"></el-input-number>

            <!-- 单选框 -->
            <el-radio-group v-else-if="config.type === 'radio'" v-model="formData[key]">
                <el-radio v-for="(option, idx) in config.options" :key="`radio-${key}-${idx}`" 
                    :label="option.label">
                    {{ option.text }}
                </el-radio>
            </el-radio-group>

            <!-- 普通下拉选择 -->
            <el-select v-else-if="config.type === 'select' || config.type === 'optionselect'" v-model="formData[key]"
                :multiple="config.multiple" :clearable="true" :placeholder="`请选择${config.label}`">
                <el-option v-for="(option, idx) in config.options" :key="`select-${key}-${idx}`"
                    :label="option.label || option.name" :value="option.value || option.label">
                </el-option>
            </el-select>

            <!-- 日期范围 -->
            <el-date-picker v-else-if="config.type === 'date-range'" v-model="dateRange[key]" type="datetimerange"
                range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期"
                style="width: 100%;"></el-date-picker>

            <!-- 远程搜索选择器 -->
            <el-select v-else v-model="formData[key]" :multiple="config.multiple" :filterable="true" :remote="true"
                :remote-method="(query) => loadRemoteData(key, query)" :loading="loading[key]" :clearable="true"
                :placeholder="`请选择${config.label}`">
                <el-option v-for="(item, idx) in options[key]" :key="`remote-${key}-${idx}`"
                    :label="item.Name || item.label || item.name"
                    :value="item.DeviceId || item.Id || item.value || item.label">
                </el-option>
            </el-select>
        </el-form-item>

        <div style="margin-top: 20px; text-align: right;">
            <el-button type="primary" @click="submitForm">确 定</el-button>
        </div>
    </el-form>
</template>

<script>
import { getServiceData } from './serviceFactory';
import { productInfo } from "@/api/rules/productModel"; // 提前导入

export default {
    name: 'SmartForm',
    props: {
        apiConfig: {
            type: Object,
            required: true
        }
    },
    data() {
        return {
            formData: {},
            options: {},      // 存储远程加载的选项
            loading: {},      // 加载状态
            dateRange: {},    // 存储多个日期范围
            formRules: {}     // 表单验证规则
        };
    },
    created() {
        // 初始化表单数据
        this.initFormData();
    },
    watch: {
        apiConfig: {
            handler() {
                this.initFormData();
            },
            deep: true
        }
    },
    methods: {
        // 初始化表单数据和验证规则
        initFormData() {
            // 使用 $set 确保响应式
            this.formData = {};
            this.options = {};
            this.loading = {};
            this.dateRange = {};
            this.formRules = {};

            // 设置默认值和验证规则
            Object.keys(this.apiConfig.params || {}).forEach(key => {
                const config = this.apiConfig.params[key];

                // 使用 $set 确保新添加的属性是响应式的
                // 设置默认值（确保单选框默认值与选项label类型匹配）
                let defaultValue;
                if (config.type === 'radio' && config.defaultValue === undefined) {
                    // 如果是单选框且没有默认值，使用第一个选项的值
                    defaultValue = (config.options && config.options.length > 0) ? config.options[0].label : '';
                } else {
                    defaultValue = config.defaultValue || '';
                }
                
                // 关键修复：使用 this.$set 确保属性是响应式的
                this.$set(this.formData, key, defaultValue);

                // 设置验证规则
                if (config.required) {
                    this.$set(this.formRules, key, [{
                        required: true,
                        message: config.requiredMessage || `请选择${config.label}`,
                        trigger: config.type === 'radio' ? 'change' : 'blur'
                    }]);
                }

                // 如果是远程选择器，预加载数据
                if (config.type && (config.type.includes('-select') ||
                    ['device-select', 'rules-select', 'plane-select', 'room-select'].includes(config.type))) {
                    this.$set(this.loading, key, false);
                    this.$set(this.options, key, []);
                    this.loadRemoteData(key);
                }

                // 初始化日期范围
                if (config.type === 'date-range') {
                    this.$set(this.dateRange, key, []);
                }
            });
        },

        // 加载远程数据
        async loadRemoteData(key, query = '') {
            const config = this.apiConfig.params[key];
            this.$set(this.loading, key, true);

            try {
                // 获取服务类型（直接使用type或从配置中提取）
                const serviceType = config.serviceType || config.type;
                const data = await getServiceData(serviceType, { Key: query });
                this.$set(this.options, key, data);

                // 如果是设备选择器，监听变化以加载属性
                if (serviceType === 'device-select' && config.changeFunc === 'properties') {
                    this.$watch(() => this.formData[key], (newVal) => {
                        if (newVal && this.apiConfig.params.Code) {
                            this.loadDeviceProperties(newVal);
                        }
                    }, { immediate: true });
                }
            } catch (error) {
                console.error(`加载${config.label}数据失败`, error);
                this.$set(this.options, key, []);
            } finally {
                this.$set(this.loading, key, false);
            }
        },

        // 加载设备属性（针对DeviceHistory等需要属性选择的接口）
        async loadDeviceProperties(deviceId) {
            if (!deviceId) return;

            try {
                // 找到选中的设备
                const device = this.options.id?.find(item =>
                    item.DeviceId === deviceId || item.Id === deviceId);

                if (device && device.ProductId) {
                    const rsp = await productInfo({ id: device.ProductId });
                    const msl = JSON.parse(rsp.data.ModelTSL || '{"properties":[]}');

                    // 更新属性选项
                    this.$set(this.options, 'Code', (msl.properties || []).map(prop => ({
                        Id: prop.code,
                        Name: prop.name,
                        code: prop.code,
                        name: prop.name
                    })));
                }
            } catch (error) {
                console.error('加载设备属性失败', error);
            }
        },

        // 提交表单
        async submitForm() {
            this.$refs.form.validate((valid) => {
                if (valid) {
                    // 处理日期范围
                    const submitData = { ...this.formData };

                    // 合并日期范围数据
                    Object.keys(this.dateRange).forEach(key => {
                        if (this.dateRange[key] && this.dateRange[key].length) {
                            // 根据配置获取时间参数名
                            const timeParams = this.apiConfig.params[key].timeParams ||
                                [`${key}Start`, `${key}End`];

                            submitData[timeParams[0]] = this.formatDate(this.dateRange[key][0]);
                            submitData[timeParams[1]] = this.formatDate(this.dateRange[key][1]);
                        }
                    });
                    this.$emit('ok', submitData);
                }
            });
        },

        // 格式化日期
        formatDate(date) {
            if (!date) return '';
            return new Date(date).toISOString();
        }
    }
};
</script>