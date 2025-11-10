<template>
	<view>
		<view class="lunbo">
			<view @click="choiceImg(1)" class="item">
				<image class="image" v-if="lunbo.firstProImg" :src="lunbo.firstProImg" mode="aspectFill"></image>
				<view v-else class="view">
					<uni-icons type="image-filled" size="20"></uni-icons>
					<text class="text">上传轮播图</text>
				</view>
			</view>
			<view @click="choiceImg(2)" class="item">
				<image class="image" v-if="lunbo.secondProImg" :src="lunbo.secondProImg" mode="aspectFill"></image>
				<view v-else class="view">
					<uni-icons type="image-filled" size="20"></uni-icons>
					<text class="text">上传轮播图</text>
				</view>
			</view>
			<view @click="choiceImg(3)" class="item">
				<image class="image" v-if="lunbo.thirdProImg" :src="lunbo.thirdProImg" mode="aspectFill"></image>
				<view v-else class="view">
					<uni-icons type="image-filled" size="20"></uni-icons>
					<text class="text">上传轮播图</text>
				</view>
			</view>
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
	import {
		uploadFile
	} from '@/api/file.js'
	import request from '@/common/request.js'
	export default {
		data() {
			return {
				lunbo: {
					firstProImg: '',
					secondProImg: '',
					thirdProImg: '',
				},
				orgInfo: null,
				userInfo: null,
				ProConfig: null, //产品设置信息
				config: {},
			}
		},
		onLoad() {
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
					// console.log("uuuu", config2[0]);
					if(config2[0]){
						this.config={
							oldInfo:config2
						}
					}
					
					// console.log("产品设置轮播图页面设置信息", this.config);
					if (this.config.lunbo) {
						this.lunbo.firstProImg = this.config.lunbo.firstProImg
						this.lunbo.secondProImg = this.config.lunbo.secondProImg
						this.lunbo.thirdProImg = this.config.lunbo.thirdProImg
					}else if(this.config.oldInfo){
						this.config.oldInfo.map((item, index) => {
							// console.log("子级数据", item);
							if (item.type == 'lunbo') {
								// console.log("lunbotu",item.data);
								let imgList = item.data
								imgList.map((it, ix) => {
									if (it.key == 'firstProImg') {
										this.lunbo.firstProImg = request.config.baseURL + it.url
									}
									if (it.key == 'secondProImg') {
										this.lunbo.secondProImg = request.config.baseURL + it.url
									}
									if (it.key == 'thirdProImg') {
										this.lunbo.thirdProImg = request.config.baseURL + it.url
									}
								})
							}
						})
					}

				}

			},
			async choiceImg(sort) {
				let files = null;
				uni.chooseImage({
					count: 1,
					sizeType: ['compressed'],
					success: async (res) => {
						if (res.tempFilePaths.length == 0) {
							return;
						}
						if (res.tempFilePaths.length > 1) {
							uni.showToast({
								title: '只能上传一张图片',
								icon: 'none',
								duration: 1000
							})
						}
						// console.log("上传图片信息", res);
						let paths = res.tempFilePaths[0];

						let files = await uploadFile(paths);
						// console.log("上传后端后图片的地址");
						if (sort == 1) {
							this.lunbo.firstProImg = request.config.baseURL + files
							await this.save('第一张')


						}
						if (sort == 2) {
							this.lunbo.secondProImg = request.config.baseURL + files
							await this.save('第二张')

						}
						if (sort == 3) {
							this.lunbo.thirdProImg = request.config.baseURL + files
							await this.save('第三张')
						}
					}
				});


			},
			save(text) {
				uni.showLoading({
					title: "上传中"
				})
				try {
					this.config.lunbo = this.lunbo
					// console.log("修改轮播图片是产品设置信息", this.config);
					addSetting({
						OrgId: this.userInfo.OrgId,
						proConfig: JSON.stringify(this.config)
					}).then(re => {
						if (re.code == 0) {
							uni.showToast({
								icon: 'success',
								title: text + '修改成功'
							}, 2000);
						}
						// uni.navigateBack()

					})
				} catch (e) {
					//TODO handle the exception
				} finally {
					uni.hideLoading()
				}

			},
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #fff;
	}

	.lunbo {
		width: 100%;
		height: auto;
		padding: 30rpx;
		padding-top: 28rpx;
		box-sizing: border-box;
		display: block;

		.item {
			width: 100%;
			height: 256rpx;
			border-radius: 8rpx;
			background: #F1F2F3;
			display: flex;
			margin-bottom: 30rpx;
			justify-content: center;
			align-items: center;

			.image {
				width: 100%;
				height: 256rpx;
				border-radius: 8rpx;
			}

			.text {
				margin-left: 10rpx;
			}
		}
	}
</style>
