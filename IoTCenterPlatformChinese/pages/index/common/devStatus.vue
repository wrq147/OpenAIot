<template>
    <view class="devStatus">
        <view class="devStatus-box">
            <view class="network" @click.stop="isNetwork = !isNetwork">
                {{ netTitle }}
                <custom-icons
                    style="margin-left: 16rpx"
                    iconsName="icon-xialasanjiao"
                    iconsSize="10rpx"
                    iconsColor="#333"></custom-icons>
                <ul v-if="isNetwork" class="networkPop">
                    <li
                        v-for="(item, index) in network"
                        :key="index"
                        @click.stop="networkClick(item)">
                        {{ item.value }}
                    </li>
                </ul>
            </view>
            <view
                v-if="operationShow === 'true'"
                class="operation"
                @click.stop="isOpera = !isOpera">
                {{ operaTitle }}
                <custom-icons
                    style="margin-left: 16rpx"
                    iconsName="icon-xialasanjiao"
                    iconsSize="10rpx"
                    iconsColor="#333"></custom-icons>
                <ul v-if="isOpera" class="operationPop">
                    <!-- <li class="inputBox">
						<uni-easyinput trim="all" v-model="value" @clear="clearClick" placeholder="请输入运行状态" />
						<button class="btn" @click.stop="operationClick()">确定</button>
					</li> -->
                    <li @click.stop="operationClick('')">全部</li>
                    <li
                        v-for="(item, index) in operation"
                        :key="index"
                        @click.stop="operationClick(item.label, item.value)">
                        {{ item.label }}
                    </li>
                </ul>
            </view>
        </view>
        <view v-if="current !== 0" @click="onLongPress" class="equipmentMove">
            {{ current === 1 ? '移动设备' : '移除设备' }}
        </view>
    </view>
</template>

<script>
import { getConfigKey } from '@/api/config.js';
export default {
    props: {
        current: {
            type: Number,
            default: () => 0,
        },
        defaultNetTitle: {
            type: [String, Number],
            default: '联网状态',
        },
    },
    data() {
        return {
            network: [
                { id: '', value: '联网状态' },
                { id: 1, value: '在线' },
                { id: 0, value: '离线' },
                { id: 2, value: '未知' },
            ],
            operation: [],
            netTitle: '联网状态',
            operaTitle: '运行状态',
            isNetwork: false,
            isOpera: false,
            operationShow: '',
        };
    },
    watch: {
        current: {
            handler() {
                this.netTitle = '联网状态';
                this.operaTitle = '运行状态';
            },
        },
        defaultNetTitle: {
            handler() {
                let act = this.network.find((row) => row.id === this.defaultNetTitle);
                if (act) {
                    this.netTitle = act.value;
                }
                this.$emit('getOnline', this.defaultNetTitle);
            },
        },
    },
    mounted() {
        getConfigKey('device.runstate').then((res) => {
            this.operationShow = res.data;
        });
        this.$store.dispatch('data/dictList', 'device_run').then((res) => {
            this.operation = res;
        });
        // document.addEventListener('click', this.handleDocumentClick);
    },
    beforeDestroy() {
        // document.removeEventListener('click', this.handleDocumentClick);
    },
    methods: {
        handleDocumentClick() {
            // 这里处理点击页面空白处的逻辑
            this.isNetwork = false;
            this.isOpera = false;
        },
        networkClick(item) {
            this.netTitle = item.value;
            this.isNetwork = false;
            this.$emit('getOnline', item.id);
        },
        operationClick(label, value) {
            this.isOpera = false;
            if (label) {
                this.operaTitle = label;
            } else {
                this.operaTitle = '全部';
            }

            this.$emit('getDState', value);
        },
        onLongPress() {
            this.$emit('onLongPress', this.current);
        },
    },
};
</script>

<style lang="less" scoped>
.devStatus {
    width: 100%;
    // margin-top: 10rpx;
    display: flex;
    align-items: center;
    justify-content: space-between;
}
.devStatus-box {
    display: flex;
    align-items: center;
}
.network,
.operation {
    width: 160rpx;
    height: 60rpx;
    background: #ffffff;
    border-radius: 35rpx;
    font-weight: 400;
    font-size: 24rpx;
    color: #666666;
    display: flex;
    align-items: center;
    justify-content: center;
    position: relative;
    margin-right: 10rpx;
}
.networkPop {
    position: absolute;
    top: 114%;
    width: 78%;
    left: 0;
    padding: 10rpx 0 10rpx 30rpx;
    margin: 0;
    background: #ffffff;
    box-shadow: 0rpx 4rpx 16rpx 0rpx rgba(0, 0, 0, 0.06);
    border-radius: 10rpx;
}
.networkPop > li {
    width: 100%;
    list-style: none;
    padding: 0;
    margin: 0;
    background-color: #fff;
    height: 78rpx;
    line-height: 78rpx;
    font-weight: 400;
    font-size: 28rpx;
    color: #333333;
}
.operationPop {
    position: absolute;
    top: 114%;
    width: 254%;
    left: 0;
    padding: 20rpx 26rpx 10rpx 20rpx;
    margin: 0;
    background: #ffffff;
    box-shadow: 0rpx 4rpx 16rpx 0rpx rgba(0, 0, 0, 0.06);
    border-radius: 10rpx;
}
.operationPop > li {
    width: 100%;
    list-style: none;
    padding: 0;
    margin: 0;
    background-color: #fff;
    height: 78rpx;
    line-height: 78rpx;
    font-weight: 400;
    font-size: 28rpx;
    color: #333333;
}
.inputBox {
    background: #f8f8f8 !important;
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 64rpx;
}
::v-deep .uni-easyinput {
    width: 308rpx !important;
}
::v-deep .uni-easyinput__content-input {
    height: 60rpx;
    border: 0;
    background-color: #f8f8f8;
}
::v-deep .is-input-border {
    height: 60rpx;
    border: 0;
}
.btn {
    width: 120rpx;
    height: 60rpx;
    background: #ffffff;
    border-radius: 10rpx;
    font-weight: 400;
    font-size: 24rpx;
    color: #333333;
    text-align: center;
    line-height: 60rpx;
    margin-right: 6rpx;
}
.equipmentMove {
    width: 160rpx;
    height: 68rpx;
    background: #ffffff;
    border-radius: 35rpx;
    font-weight: 400;
    font-size: 24rpx;
    color: #333333;
    text-align: center;
    line-height: 68rpx;
}
</style>
