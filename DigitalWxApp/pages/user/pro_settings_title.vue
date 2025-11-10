<template>
	<view>
		<view class="txt">
			<input ref="protitle" :focus="isFocus" class="input" type="text" v-model="title"
				placeholder="请输入标题" />
		</view>
		<view class="common_btn" @click="submit">
			确定
		</view>
	</view>
</template>

<script>
	import {
		addSetting
	} from '@/api/product.js'
	import {
		getOrg
	} from '@/api/org.js'
	export default {
		data() {
			return {
				title: '',
				orgInfo: null,
				userInfo: null,
				ProConfig: null, //产品设置信息
				config: {},
				list: [],
				isFocus: true
			}
		},
		onLoad() {
			this.isFocus = false;
			this.$nextTick(() => {
				this.isFocus = true;
			});
			this.reloadPages()
		},
		methods: {
			async reloadPages() {
				this.userInfo = await this.$store.dispatch("userInfo");
				this.orgInfo = await getOrg(this.userInfo.OrgId);
				this.ProConfig = this.orgInfo.data.ProConfig;
				if (this.ProConfig) {
					this.config = JSON.parse(this.ProConfig);
					let config2 = JSON.parse(this.ProConfig)
					if (config2[0]) {
						this.config = {
							oldInfo: config2
						}
					}

					// console.log("转化后产品设置信息this.config", this.config);
					if (this.config.title) {
						this.title = this.config.title
					} else if (this.config.oldInfo) {
						this.config.oldInfo.map((item, index) => {
							if (item.type == 'title') {
								this.title = item.data
							}
						})
					}
				}
			},
			submit() {
				this.config.title = this.title
				// console.log("提交产品部分标题时产品设置信息是",this.config.title);
				addSetting({
					OrgId: this.userInfo.OrgId,
					proConfig: JSON.stringify(this.config)
				}).then(re => {
					if (re.code == 0) {
						uni.showToast({
							icon: 'success',
							title: '提交成功'
						}, 200);
						setTimeout(() => {
							uni.navigateBack()
						}, 200)

					}

				})

			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #fff;
	}

	.txt {
		width: 100%;
		padding: 50rpx 30rpx;
		box-sizing: border-box;

		.input {
			background-color: #F1F2F3;
			width: 100%;
			height: 100rpx;
			border-radius: 8rpx;
			display: block;
			box-sizing: border-box;
			padding: 0 30rpx;
			font-size: 28rpx;
			color: #333;
			line-height: 100rpx;
		}

		input::placeholder {
			color: #999999;
		}

	}
</style>
