<template>
    <div style="padding:20px 20px 0 20px" id="big_con">
        <div>
            <el-button type="primary" plain @click="handleAddProp">
                <i class="el-icon-coin"></i>
                <span style="margin-left:6px">生成规则属性数据</span>
            </el-button>

            <el-button type="primary" plain @click="handleCopyProp">
                <i class="el-icon-coin"></i>
                <span style="margin-left:6px">数据拷贝</span>
            </el-button>
        </div>


        <el-dialog title="选择生成时间" :visible.sync="propDialogVisible" width="400px" center>
            <div style="text-align: center;padding: 20px 0;">
                <el-date-picker v-model="propDateTime" type="datetime" placeholder="请选择日期时间"
                    value-format="yyyy-MM-dd HH:mm:ss" style="width: 100%;">
                </el-date-picker>
            </div>
            <div slot="footer" class="dialog-footer">
                <el-button @click="propDialogVisible = false">取 消</el-button>
                <el-button type="primary" @click="submitPropData">确 定</el-button>
            </div>
        </el-dialog>


        <el-dialog title="设备历史数据拷贝" :visible.sync="copyDialogVisible" width="680px">
            <el-form ref="copyForm" :model="copyForm" label-width="110px">
                <el-form-item label="源设备ID">
                    <el-input v-model="copyForm.SourceId" placeholder="请输入源设备唯一标识"
                        @change="loadSourceDeviceProduct"></el-input>
                    <div v-if="sourceProductTips" style="color:#666;font-size:12px;margin-top:4px">{{ sourceProductTips
                        }}
                    </div>
                </el-form-item>
                <el-form-item label="目标设备ID">
                    <el-input v-model="copyForm.TargetId" placeholder="请输入目标设备唯一标识"
                        @change="loadTargetDeviceProduct"></el-input>
                    <div v-if="targetProductTips" style="color:#666;font-size:12px;margin-top:4px">{{ targetProductTips
                        }}
                    </div>
                </el-form-item>

                <el-row :gutter="10">
                    <el-col :span="12">
                        <el-form-item label="拷贝开始时间">
                            <el-date-picker v-model="copyForm.StartTime" type="datetime"
                                value-format="yyyy-MM-dd HH:mm:ss" placeholder="拷贝起始时间"
                                style="width:100%"></el-date-picker>
                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item label="拷贝结束时间">
                            <el-date-picker v-model="copyForm.EndTime" type="datetime"
                                value-format="yyyy-MM-dd HH:mm:ss" placeholder="拷贝结束时间"
                                style="width:100%"></el-date-picker>
                        </el-form-item>
                    </el-col>
                </el-row>

                <!-- 属性映射 -->
                <el-form-item label="属性映射关系">
                    <el-button type="primary" @click="addIdentifier">+ 添加属性映射</el-button>
                    <div class="params_con">
                        <div class="params_li" v-for="(item, inx) in identifierList" :key="inx">
                            <el-select v-model="item.key" filterable placeholder="选择源属性" style="width: 190px">
                                <el-option v-for="attr in sourceAttrOptions" :key="attr.code"
                                    :label="attr.name + '(' + attr.code + ')'" :value="attr.code"></el-option>
                            </el-select>
                            <span class="marspan">-</span>
                            <el-select v-model="item.value" filterable placeholder="选择目标属性" style="width: 190px">
                                <el-option v-for="attr in targetAttrOptions" :key="attr.code"
                                    :label="attr.name + '(' + attr.code + ')'" :value="attr.code"></el-option>
                            </el-select>
                            <div class="delete_con" @click="deleteIdentifier(inx)">-</div>
                        </div>
                    </div>
                    <div style="color:#999;margin-top:8px;font-size:12px;">
                        说明：输入设备ID后回车/失焦自动加载对应产品属性；不配置映射属性不会被拷贝
                    </div>
                </el-form-item>
            </el-form>
            <div slot="footer" class="dialog-footer">
                <el-button @click="closeCopyDialog">取 消</el-button>
                <el-button type="primary" @click="submitCopyData">执行拷贝</el-button>
            </div>
        </el-dialog>

    </div>
</template>

<script>
import {
    noticeCalProp,
    copyData
} from "@/api/rules/data";
import { DeviceInfo } from "@/api/rules/device";
import { productInfo } from "@/api/rules/productModel";
export default {
    name: "IotDataIndex",
    data() {
        return {
            proClassTree: [],//协议分类列表
            propDialogVisible: false, // 弹窗显示隐藏
            propDateTime: "", // 选中的时间

            // 拷贝弹窗基础
            copyDialogVisible: false,
            copyForm: {
                SourceId: "",
                TargetId: "",
                StartTime: "",
                EndTime: "",
                Mapping: {}
            },

            // 源设备/产品相关
            sourceProductId: "",
            sourceAttrOptions: [],
            sourceProductTips: "",

            // 目标设备/产品相关
            targetProductId: "",
            targetAttrOptions: [],
            targetProductTips: "",

            // 映射标识符数组
            identifierList: [],
            isInitWatch: true
        };
    },
    watch: {
        // 监听映射列表变化自动组装Mapping字典
        identifierList: {
            handler() {
                if (this.isInitWatch) return;
                const mapObj = {};
                this.identifierList.forEach(item => {
                    if (item.key && item.value) {
                        mapObj[item.key] = item.value;
                    }
                });
                this.copyForm.Mapping = { ...mapObj };
            },
            deep: true,
            immediate: true
        }
    },
    mounted() {
    },
    methods: {
        handleAddProp() {
            this.propDateTime = this.parseTime(new Date(), '{y}-{m}-{d} {h}:{i}:{s}');
            this.propDialogVisible = true;

        },
        submitPropData() {
            if (!this.propDateTime) {
                this.$modal.msgWarning("请选择时间");
                return;
            }

            this.$modal.loading("正在生成属性数据...");
            noticeCalProp(this.propDateTime).then(() => {
                this.$modal.closeLoading();
                this.propDialogVisible = false;
                this.$modal.msgSuccess(`生成成功！时间：${this.propDateTime}`);
            })

        },
        async loadSourceDeviceProduct() {
            const devId = this.copyForm.SourceId.trim();
            this.sourceAttrOptions = [];
            this.sourceProductTips = "";
            this.sourceProductId = "";
            if (!devId) return;
            try {
                const res = await DeviceInfo({ id: devId });
                if (res.code !== 0 || !res.data || !res.data.ProductId) {
                    this.sourceProductTips = "未查询到该设备或设备无绑定产品";
                    return;
                }
                const pid = res.data.ProductId;
                await this.loadSourceAttrByProduct(pid);
            } catch (err) {
                this.sourceProductTips = "查询设备信息失败";
            }
        },
        async loadTargetDeviceProduct() {
            const devId = this.copyForm.TargetId.trim();
            this.targetAttrOptions = [];
            this.targetProductTips = "";
            this.targetProductId = "";
            if (!devId) return;

            try {
                const res = await DeviceInfo({ id: devId });
                if (res.code !== 0 || !res.data || !res.data.ProductId) {
                    this.targetProductTips = "未查询到该设备或设备无绑定产品";
                    return;
                }
                const pid = res.data.ProductId;
                await this.loadTargetAttrByProduct(pid);
            } catch (err) {
                this.targetProductTips = "查询设备信息失败";
            }
        },
        async loadSourceAttrByProduct(productId) {
            this.sourceProductId = productId;
            const rsp = await productInfo({ id: productId });
            const tsl = JSON.parse(rsp.data.ModelTSL || "{}");
            const props = tsl.properties || [];
            this.sourceAttrOptions = props;
            this.sourceProductTips = `已加载产品属性，产品ID：${productId}`;
        },
        async loadTargetAttrByProduct(productId) {
            this.targetProductId = productId;
            const rsp = await productInfo({ id: productId });
            const tsl = JSON.parse(rsp.data.ModelTSL || "{}");
            const props = tsl.properties || [];
            this.targetAttrOptions = props;
            this.targetProductTips = `已加载产品属性，产品ID：${productId}`;
        },
        addIdentifier() {
            this.identifierList.push({ key: "", value: "" });
        },
        deleteIdentifier(inx) {
            this.identifierList.splice(inx, 1);
        },
        handleCopyProp() {
            this.copyForm = { SourceId: "", TargetId: "", StartTime: "", EndTime: "", Mapping: {} };
            this.sourceAttrOptions = [];
            this.targetAttrOptions = [];
            this.sourceProductId = "";
            this.targetProductId = "";
            this.sourceProductTips = "";
            this.targetProductTips = "";
            this.identifierList = [];
            this.isInitWatch = true;

            // 默认时间：昨天到当前
            const now = new Date();
            const yesterday = new Date(now.getTime() - 24 * 60 * 60 * 1000);
            this.copyForm.StartTime = this.parseTime(yesterday, '{y}-{m}-{d} {h}:{i}:{s}');
            this.copyForm.EndTime = this.parseTime(now, '{y}-{m}-{d} {h}:{i}:{s}');

            this.$nextTick(() => this.isInitWatch = false);
            this.copyDialogVisible = true;
        },
        closeCopyDialog() {
            this.copyDialogVisible = false;
        },
        async submitCopyData() {
            if (!this.copyForm.SourceId) return this.$modal.msgWarning("请填写源设备ID");
            if (!this.copyForm.TargetId) return this.$modal.msgWarning("请填写目标设备ID");
            if (!this.copyForm.StartTime || !this.copyForm.EndTime) return this.$modal.msgWarning("请选择拷贝起止时间");
            if (new Date(this.copyForm.StartTime) >= new Date(this.copyForm.EndTime)) return this.$modal.msgWarning("开始时间不能大于等于结束时间");

            this.$modal.loading("正在拷贝设备历史数据...");
            try {
                const res = await copyData(this.copyForm);
                this.$modal.closeLoading();
                this.copyDialogVisible = false;
                this.$modal.msgSuccess(`数据拷贝完成，共拷贝${res.data}条记录`);
            } catch (err) {
                this.$modal.closeLoading();
            }
        },
    }
};
</script>

<style lang="scss" scoped>
.params_con {
    margin-top: 10px;

    .params_li {
        margin-top: 12px;
        display: flex;
        align-items: center;

        .marspan {
            margin: 0 10px;
            color: #dcdfe6;
            font-size: 18px;
        }

        .delete_con {
            width: 32px;
            height: 32px;
            display: flex;
            justify-content: center;
            align-items: center;
            border: 1px solid #23A8F2;
            border-radius: 5px;
            margin-left: 10px;
            color: #23A8F2;
            cursor: pointer;
        }
    }
}
</style>