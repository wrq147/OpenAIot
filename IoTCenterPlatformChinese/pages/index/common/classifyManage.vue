<template>
    <view class="classifyManage">
        <top
            title="分类管理"
            leftWidth="120rpx"
            rightWidth="120rpx"
            :isleftBack="true"
            leftIcon="icon-fanhui"
            backgroundColor="#ffffff"
            rightIcon="icon-tianjia"
            @clickRight="wareRecordsAdd"></top>
        <view class="classifyManage-container">
            <view class="container-info">
                <next-tree
                    ref="qiantree"
                    :selectParent="true"
                    :labelKey="'Name'"
                    :idKey="'Id'"
                    :childrenKey="'Children'"
                    :multiple="true"
                    :treeData="rangeList"
                    @treeDelete="treeDelete"
                    @treeEdit="treeEdit" />
            </view>
        </view>
        <!-- 弹出层 -->
        <uni-popup ref="popup" type="dialog" :zIndex="60">
            <view class="popup-content">
                <view class="popup-container">
                    <view class="popup-title">{{ title }}分类</view>
                    <uni-forms
                        ref="valiForm"
                        :rules="rules"
                        :modelValue="valiFormData"
                        label-width="90"
                        label-position="top">
                        <uni-forms-item label="分类名称" required name="name">
                            <uni-easyinput
                                v-model="valiFormData.name"
                                placeholder="请输入分类名称" />
                        </uni-forms-item>
                        <uni-forms-item label="父级分类">
                            <uni-data-select
                                v-model="valiFormData.parentId"
                                :clear="false"
                                :localdata="range"
                                placeholder="请选择父级分类"></uni-data-select>
                        </uni-forms-item>
                        <uni-forms-item label="排序" required name="sort">
                            <uni-easyinput
                                type="number"
                                v-model="valiFormData.sort"
                                placeholder="请输入排序" />
                        </uni-forms-item>
                        <button class="button" @click="submit">保存</button>
                    </uni-forms>
                </view>
            </view>
        </uni-popup>
    </view>
</template>

<script>
import { typeListTree, typeListAdd, typeListEdit, typeListDelete } from '@/api/device.js';
export default {
    data() {
        return {
            rangeList: [],
            title: '添加',
            showPopup: true,
            valiFormData: {
                customerId: '',
                id: '',
                name: '',
                parentId: '',
                sort: '',
                targetOrgId: 0,
            },
            // 校验规则
            rules: {
                name: {
                    rules: [{ required: true, errorMessage: '分类名称不能为空' }],
                },
                sort: {
                    rules: [{ required: true, errorMessage: '排序不能为空' }],
                },
            },
            range: [],
        };
    },
    onLoad(options) {
        this.getList();
    },
    methods: {
        getList() {
            this.range = [{ value: '', text: '作为一级分类' }];
            typeListTree({ orgid: this.$store.state.user.orgId }).then((res) => {
                this.rangeList = res.data;
                this.rangeList.forEach((item) => {
                    this.range.push({ value: item.Id, text: item.Name });
                    item.Children.forEach((v) => {
                        this.range.push({ value: v.Id, text: v.Name });
                    });
                });
            });
        },
        wareRecordsAdd() {
            this.title = '添加';
            this.valiFormData.id = '';
            this.valiFormData.targetOrgId = this.$store.state.user.orgId;
            this.valiFormData.name = '';
            this.valiFormData.parentId = '';
            this.valiFormData.sort = '';
            this.valiFormData.customerId = '';
            this.$refs.popup.open();
        },
        treeEdit(item) {
            this.title = '编辑';
            this.valiFormData.id = item.id;
            this.valiFormData.name = item.name;
            this.valiFormData.parentId = item.parentId;
            this.valiFormData.sort = item.sort;
            this.valiFormData.targetOrgId = item.targetOrgId;
            this.valiFormData.customerId = item.customerId;
            this.$refs.popup.open();
        },
        treeDelete(id) {
            let that = this;
            uni.showModal({
                title: '提示',
                content: '你确定要删除吗',
                success: function (res) {
                    if (res.confirm) {
                        typeListDelete({ id: id }).then((res) => {
                            uni.showToast({
                                title: '删除成功',
                                icon: 'success',
                                duration: 2000,
                            });
                            that.getList();
                        });
                    }
                },
            });
        },
        getAdd() {
            typeListAdd(this.valiFormData).then((res) => {
                uni.showToast({
                    title: '添加成功',
                    icon: 'success',
                    duration: 2000,
                });
                this.$refs.popup.close();
                this.getList();
            });
        },
        getUpdata() {
            typeListEdit(this.valiFormData).then((res) => {
                uni.showToast({
                    title: '编辑成功',
                    icon: 'success',
                    duration: 2000,
                });
                this.$refs.popup.close();
                this.getList();
            });
        },
        submit() {
            this.$refs.valiForm.validate((err, valiFormData) => {
                if (!err) {
                    if (this.title === '编辑') {
                        this.getUpdata();
                    } else {
                        this.getAdd();
                    }
                }
            });
        },
    },
};
</script>

<style lang="less" scoped>
.classifyManage {
    width: 100%;
}
.classifyManage-container {
    padding: 0 20rpx;
    margin-top: 20rpx;
}
.classifyManage-container .container-info {
    width: 100%;
    position: relative;
    height: calc(100vh - 140rpx);
}
::v-deep {
    .next-tree-view {
        padding: 0;
    }
    .next-tree-item {
        padding: 0 20rpx !important;
    }
    .next-tree-view {
        top: 0;
    }
}
.popup-content {
    width: 650rpx;
    height: 762rpx;
    border-radius: 10rpx;
    background-color: #fff;
}
.popup-container {
    padding: 20rpx;
}
.popup-container .popup-title {
    font-size: 34rpx;
    color: #333333;
    font-weight: bold;
    text-align: center;
    margin-bottom: 40rpx;
}
.button {
    width: 100%;
    height: 88rpx;
    background: #2371ff;
    border-radius: 10rpx;
    font-size: 28rpx;
    color: #ffffff;
    line-height: 88rpx;
}
// ::v-deep {
// 	.is-input-borde{
// 		border: 0 !important;
// 		height: 88rpx;
// 	}
// 	.uni-easyinput{
// 		height: 88rpx;
// 	}
// 	.uni-easyinput__content-input{
// 		width: 100%;
// 		height: 88rpx !important;
// 		background: #F8F8F8;
// 		border: 0;
// 	}
// 	.uni-input-input{
// 		background: #F8F8F8;
// 		border: 0;
// 	}
// }
</style>
