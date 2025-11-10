<template>
	<view>
		<top :title="topTitle" leftWidth="157rpx" :isleftBack="true" leftIcon="icon-fanhui" rightWidth="157rpx">
		</top>
		<view class="fun_con">
			<view class="fun_li" @click="finishFunChoice(1)":style="{'opacity':isOnlyRead&&timeType!=1?0.5:1}">
				<view class="name">
					按时间
				</view>
				<view class="right">
					<custom-icons iconsName="icon-danxuan" iconsSize="32rpx" iconsColor="#2371FF" v-if="timeType==1"></custom-icons>
					<custom-icons iconsName="icon-weixuanzhong" iconsSize="32rpx" iconsColor="#EAEAEA" v-else-if="timeType"></custom-icons>
				</view>
			</view>
			<view class="fun_li" @click="finishFunChoice(2)":style="{'opacity':isOnlyRead&&timeType!=2?0.5:1}">
				<view class="name">
					按间隔
				</view>
				<view class="right">
					<custom-icons iconsName="icon-danxuan" iconsSize="32rpx" iconsColor="#2371FF" v-if="timeType==2"></custom-icons>
					<custom-icons iconsName="icon-weixuanzhong" iconsSize="32rpx" iconsColor="#EAEAEA" v-else-if="timeType"></custom-icons>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				topTitle: '定时触发',
				infoIndex: 0,
				allInx: 0,
				defaultCronStr: '',
				timeType:null,
				isOnlyRead:false,//是否只读
			}
		},
		onLoad(options) {
			if (options.inx) {
				this.infoIndex = options.inx
			}
			if (options.allInx) {
				this.allInx = options.allInx
			}
			if(options.isOnlyRead){
				this.isOnlyRead=true
			}
			if (options.cronStr) {
				this.defaultCronStr = options.cronStr
				if(this.defaultCronStr.indexOf('/')>-1){
					this.timeType=2
				}else{
					this.timeType=1
				}
			}
		},
		methods: {
			finishFunChoice(val) {
				if(this.isOnlyRead){}else{
					this.timeType=val
				}
				if(this.timeType!=val){
					return
				}
				if (this.defaultCronStr) {
					if(this.isOnlyRead){
						uni.navigateTo({
							url: '/pages_rules/time_select?type=' + val + '&cronStr=' + this.defaultCronStr + '&inx=' +
								Number(this.infoIndex) + '&allInx=' + Number(this.allInx)+'&isOnlyRead='+this.isOnlyRead
						})
					}else{
						uni.navigateTo({
							url: '/pages_rules/time_select?type=' + val + '&cronStr=' + this.defaultCronStr + '&inx=' +
								Number(this.infoIndex) + '&allInx=' + Number(this.allInx)
						})
					}
					
				} else {
					uni.navigateTo({
						url: '/pages_rules/time_select?type=' + val
					})
				}

			}
		}
	}
</script>

<style lang="less" scoped>
	.fun_con {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;

		.fun_li {
			margin-top: 20rpx;
			width: 100%;
			height: 126rpx;
			background-color: #ffffff;
			border-radius: 10rpx;
			padding: 0 30rpx;
			box-sizing: border-box;
			display: flex;
			justify-content: space-between;
			align-items: center;

			.name {
				font-size: 32rpx;
				color: #333333;
			}
		}
	}
</style>