<template>
	<view>
		<view class="lunbo">
			<view @click="choiceImg(1)" class="item">
				<image class="image" v-if="lunbo.firstCaseImg" :src="lunbo.firstCaseImg" mode="aspectFill"></image>
				<view v-else class="view">
					<uni-icons type="image-filled" size="20"></uni-icons>
					<text class="text">上传轮播图</text>
				</view>
			</view>
			<view @click="choiceImg(2)" class="item">
				<image class="image" v-if="lunbo.secondCaseImg" :src="lunbo.secondCaseImg" mode="aspectFill"></image>
				<view v-else class="view">
					<uni-icons type="image-filled" size="20"></uni-icons>
					<text class="text">上传轮播图</text>
				</view>
			</view>
			<view @click="choiceImg(3)" class="item">
				<image class="image" v-if="lunbo.thirdCaseImg" :src="lunbo.thirdCaseImg" mode="aspectFill"></image>
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
	} from '@/api/example.js'
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
					firstCaseImg: '',
					secondCaseImg: '',
					thirdCaseImg: '',
				},
				imgPath: [],
				imgList: [],
				orgInfo: null,
				userInfo: null,
				CaseConfig: null, //案例设置信息
				config: {},
				typeList: [], //案例设置中包含的设置类型
			}
		},
		onLoad() {
			this.reloadPages()
		},
		methods: {
			async reloadPages() {
				this.userInfo = await this.$store.dispatch("userInfo");
				this.orgInfo = await getOrg(this.userInfo.OrgId);
				this.CaseConfig = this.orgInfo.data.CaseConfig;
				
				if (this.CaseConfig) {
					this.config = JSON.parse(this.CaseConfig);
					let config2 = JSON.parse(this.CaseConfig)
					if (config2[0]) {
						this.config = {
							oldInfo: config2
						}
					}

					if (this.config.lunbo) {
						this.lunbo.firstCaseImg = this.config.lunbo.firstCaseImg
						this.lunbo.secondCaseImg = this.config.lunbo.secondCaseImg
						this.lunbo.thirdCaseImg = this.config.lunbo.thirdCaseImg
					} else if (this.config.oldInfo) {//如果现在还没有存入图片但是已经有用原来的方式存入图片的则展示原来存入的图片
						this.config.oldInfo.map((item, index) => {
							if (item.type == 'lunbo') {
								let imgList = item.data
								imgList.map((it, ix) => {
									if (it.key == 'firstCaseImg') {
										this.lunbo.firstCaseImg = request.config.baseURL + it.url
									}
									if (it.key == 'secondCaseImg') {
										this.lunbo.secondCaseImg = request.config.baseURL + it.url
									}
									if (it.key == 'thirdCaseImg') {
										this.lunbo.thirdCaseImg = request.config.baseURL + it.url
									}
								})
							}
						})
					}
				}
				if(this.CaseConfig=='[]'){
					this.config={}
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
						let paths = res.tempFilePaths[0];
						let files = await uploadFile(paths);
						// console.log("上传后端后图片的地址",files);
						if (sort == 1) {
							this.lunbo.firstCaseImg = request.config.baseURL + files
							await this.save("第一张")


						}
						if (sort == 2) {
							this.lunbo.secondCaseImg = request.config.baseURL + files
							await this.save("第二张")

						}
						if (sort == 3) {
							this.lunbo.thirdCaseImg = request.config.baseURL + files
							await this.save("第三张")
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
					addSetting({
						OrgId: this.userInfo.OrgId,
						caseConfig: JSON.stringify(this.config)
					}).then(re => {
						if (re.code == 0) {
							uni.showToast({
								icon: 'success',
								title: text + '修改成功'
							}, 2000);
						}

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
