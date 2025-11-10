<template>
	<view>
		<uni-popup ref="climsgPopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">{{noticeTitle}}</view>
					<view class="content">
						{{noticeContent}}
					</view>
					<button class="submit_button" @click.stop="confirm">
						Confirm
					</button>
					<!-- <view class="close_icon" @click="noticeColse">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
							iconsColor="rgba(255, 255, 255, 0.2)"></custom-icons>
					</view> -->
				</view>
			</view>
		</uni-popup>
		<uni-nav-bar :status-bar="true" :fixed="true" v-if="cusMask" :border="false" :height="fixedHeight" :zIndex="998"
			backgroundColor="#161A26">
			<!-- 为自定义的弹出层 -->
			<template v-slot:allslot>
				<view class="cliPopupmask" :style="{'height':fixedHeight}">
				</view>
			</template>
		</uni-nav-bar>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				t: '',
				cusMask: false,
				noticeTitle: 'System prompt information',
				noticeContent: 'Login status has expired, please log in again',
				fixedHeight:'100vh',
				urlStr:''
			}
		},
		onLoad(options) {
			this.$nextTick(()=>{
				this.noticeOpen()
				if (options.t) {
					this.t = options.t
				}
				this.urlStr=""
				for(var key in options){
					if(key !='t'){
						this.urlStr+='&'+key+'='+options[key]
					}
				}
			})
		},
		methods: {
			confirm() {
				this.noticeColse()
				console.log("this.urlStr",this.urlStr);
				uni.reLaunch({
					url: '/pages/index/login?t='+this.t+this.urlStr
				})
			},
			noticeOpen(msg, title, hideMask) {
				if (msg) {
					this.noticeContent = msg
				}
				if (title) {
					this.noticeTitle = title
				}
				this.cusMask = true
				if (hideMask) {
					this.cusMask = false
				}
				this.$refs.climsgPopup.open()

			},
			noticeColse() {
				this.cusMask = false
				this.$refs.climsgPopup.close()
			},
		}
	}
</script>

<style lang="less" scoped>
.notice_con{
	.notice_cont{
		.content{
			padding: 0 100rpx;
		}
	}
}
</style>