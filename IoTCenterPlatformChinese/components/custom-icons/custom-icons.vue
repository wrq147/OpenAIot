<template>
	<view>
		<view class="iconfont #icon-icon" :class="iconsName" :style="{'color':iconsColor,'font-size':iconsSize}"></view>
		<!-- <icon custom-prefix="iconfont" type="success" :size="iconsSize" class="iconfont" :color="iconsColor"/> -->
		<!-- <uni-icons custom-prefix="iconfont" :color="iconsColor" :type="iconsName" :size="iconsSize"></uni-icons> -->
	</view>
</template>

<script>
	export default {
		name: "custom-icons",
		props: {
			iconsName: {
				type: String,
				default: 'icon-fanhui'
			},
			iconsColor: {
				type: String,
				default: 'rgba(51, 51, 51, 1)'
			},
			iconsSize: {
				type: String,
				default: '20rpx'
			},
			xuanzhuan:{
				type:Boolean,
				default:false
			}
		},
		data() {
			return {

			};
		},
		methods:{
			onScanWifiConfig() {//扫码配网
				uni.scanCode({
					success: function(res) {
						let tmpidx = res.result.indexOf("AirLink-");
						if (tmpidx > -1) {
							let id = res.result.substring(tmpidx);
							uni.navigateTo({
								url: '/pages_device/wifi-conf/wifi-conf?dtuId=' + id
							});
						}
			
					}
				});
			},
		}
	}
</script>

<style lang="scss" scoped>
.iconfont{
	display: block;
}
</style>