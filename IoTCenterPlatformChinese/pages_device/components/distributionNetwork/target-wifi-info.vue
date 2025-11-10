<template>
	<view>
		<view class="page-subtitle">{{title}}</view>
		<view class="wifi-form">
			<view class="weui-cells">
				<view class="weui-cell weui-cell_select">
					<view class="weui-cell__hd weui-cell__hd_in-select-after">
						<view class="weui-label">WiFi</view>
					</view>
					<view class="weui-cell__bd">
						<view v-if="isIOS">
							<input v-model="selectedWifiSSID" class="weui-input" placeholder="请输入连接的wifi的SSID"
								placeholder-class="form-placeholder" placeholder-style="color:#D8D8D8"/>
						</view>
						<view class="can_select_wifi" v-else @click="openWifiSelectList"
							:class="{'selected_wifi':selectedWifiSSID}">
							<view class="text">{{selectedWifiSSID?selectedWifiSSID:'请选择wifi'}}</view>
							<uni-icons type="right" size="20" color=" #D8D8D8"></uni-icons>
						</view>
					</view>
				</view>
				<view class="weui-cell weui-cell_input">
					<view class="weui-cell__hd">
						<view class="weui-label">密码</view>
					</view>
					<view class="weui-cell__bd">
						<input class="weui-input" placeholder="请输入WiFi密码"
							placeholder-class="form-placeholder" placeholder-style="color:#D8D8D8"
							v-model="selectedWifiPassword" />
					</view>
				</view>
			</view>
		</view>

		<btn-group :buttons="[{ btnText: '下一步', type: 'primary', id: 'complete'}]"
			@onBottomButtonClick="onBottomButtonClick" :fixed-bottom="true" />

		<jp-select ref="wifipick" :checkAll="false" :list="wifiListList" :item="selectedWifiSSID" select="radio"
			@checked="onWifiPickerSelect" tite="请选择要链接的wifi"></jp-select>
	</view>
</template>

<script>
	import {
		isLocationPermissionGranted,
		requestLocationPermission
	} from './perm.js'
	import btnGroup from '@/pages_device/components/distributionNetwork/btn-group.vue'
	export default {
		name: "input-wifi-info",
		components: {
			btnGroup
		},
		props: {
			title: {
				type: String,
			}
		},
		data() {
			return {
				wifiListList: [],
				selectedWifiSSID: '',
				selectedWifiPassword: '', //输入的wifi密码
				hasGetWifi: false
			};
		},
		computed: {
			isIOS() {
				return uni.getSystemInfoSync().platform == 'ios';
			},
		},
		mounted() {
			this.wifiListList = [];
			this.selectedWifiPassword = '';
			//#ifdef MP-WEIXIN
			wx.stopWifi()
			wx.startWifi()
			wx.onGetWifiList(this.onLoadWifiList);
			//#endif
			//#ifndef MP-WEIXIN
			// #ifdef APP-PLUS
			uni.stopWifi({
				success: (res) => {
				}
			})
			uni.startWifi({
				success: (res) => {
				}
			})
			uni.onGetWifiList(this.onLoadWifiList);
			//#endif
			//#endif
		},
		unmounted() {
			//#ifdef MP-WEIXIN
			wx.stopWifi()
			wx.startWifi()
			wx.offGetWifiList(this.onLoadWifiList);
			//#endif
			//#ifndef MP-WEIXIN
			// #ifdef APP-PLUS
			uni.stopWifi({
				success: (res) => {
				}
			})
			uni.startWifi({
				success: (res) => {
				}
			})
			uni.offGetWifiList(this.onLoadWifiList);
			//#endif
			//#endif
		},
		methods: {
			onLoadWifiList(res) {
				// console.log("res这是WiFi列表拉",res);
				let wifilist = res.wifiList;
				let wifiiids = wifilist.map((x) => {
					return x.SSID;
				});
				this.wifiListList.push(...wifiiids);
				this.wifiListList = Array.from(new Set(this.wifiListList));
				this.hasGetWifi = true;
				uni.hideLoading();
			},

			async openWifiSelectList() {
				
				if (!await requestLocationPermission()) {
					return;
				}
				uni.showLoading({
					title: '加载中'
				});
				
				setTimeout(() => {
					if (!this.hasGetWifi) {
						uni.hideLoading();
						// uni.showToast({
						// 	type:'none',
						// 	title: '请确认是否开启位置定位功能',
						// 	duration: 2000
						// });
						uni.showModal({
							title: '系统提示',
							content: '请确认是否开启位置定位功能',
							showCancel: false,
							success: (res) => {
								this.$refs.wifipick.toCancel();
								if (res.confirm) {
									//#ifdef MP-WEIXIN
									wx.stopWifi()
									//#endif
									// #ifdef APP-PLUS
									uni.stopWifi({
										success: (res) => {
										}
									})
									//#endif
									//#ifdef MP-WEIXIN
									wx.startWifi()
									//#endif
									// #ifdef APP-PLUS
									uni.startWifi({
										success: (res) => {
										}
									})
									//#endif
								}
							}
						});
					}

				}, 4000);
				//#ifdef MP-WEIXIN
				wx.stopWifi()
				wx.startWifi()
				// wx.offGetWifiList(this.onLoadWifiList)
				wx.onGetWifiList(this.onLoadWifiList);
				wx.getWifiList();
				
				//#endif
				// #ifdef APP-PLUS
				uni.stopWifi({
					success: (res) => {
					}
				})
				uni.startWifi({
					success: (res) => {
					}
				})
				// uni.offGetWifiList(this.onLoadWifiList);
				uni.onGetWifiList(this.onLoadWifiList);
				uni.getWifiList({
					complete: res => {}
				});
				//#endif
				this.wifiListList = [];
				this.$refs.wifipick.toOpen();

			},
			onWifiPickerSelect(val) {
				this.selectedWifiSSID = val;
			},
			onBottomButtonClick(e) {
				//按钮事件
				if(this.selectedWifiSSID){
					switch (e.btn.id) {
						case 'complete':
							this.$emit("onTargetWifiInputComplete",{selectedWifiSSID:this.selectedWifiSSID,selectedWifiPassword:this.selectedWifiPassword})
							break;
					}
				}
				
			}
		
		}

	}
</script>


<style lang="less" scoped>
	.wifi-form {
		margin-top: 30rpx;
	}
	.wifi-form .weui-cells{
		padding-top: 30rpx;
	}
	.wifi-form .weui-cell {
		display: flex;
		justify-content: flex-start;
		width: 100%;
		height: 63rpx;
		// line-height: 63rpx;
		margin-top: 50rpx;
		border-bottom: 1rpx solid #EAEAEA;
		font-size: 32rpx;
		.weui-cell__hd {
			margin-right: 60rpx;
			width: 64rpx;
		}

		.weui-cell__bd {
			display: flex;
			justify-content: flex-start;
			align-items: flex-start;
			width: calc(100% - 64rpx - 60rpx);

			.weui-select {
				height: 63rpx;
			}

			.can_select_wifi {
				width: 100%;
				display: flex;
				justify-content: space-between;
				align-items: flex-start;
				color: #D8D8D8;

				&.selected_wifi {
					color: #333333;
				}
			}
		}
	}

	.wifi-form .weui-select {
		padding-left: 0;
	}

	.form-placeholder {
		color: #D8D8D8;
	}
</style>