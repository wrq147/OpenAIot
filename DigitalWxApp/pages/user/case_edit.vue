<template>
	<view>
		<view class="wrap">
			<view class="top">
				<!-- <navigator url="/pages/user/choice_img" class="photo"> -->
				<view class="photo" @click="openUpload">
					<view class="img" :key="imgKey">
						<template>
							<image v-if="exmImg" class="exmImg" @click.stop="preViewImg" :src="exmImg"
								mode="aspectFill">
							</image>
							<text v-else>案例封面</text>
						</template>
						<image class="image" src="../../static/user/photo.png"></image>
					</view>
				</view>
				<!-- </navigator> -->
			</view>
			<view class="title">
				<view class="txt">
					<text style="color: red;">*</text>案例标题
				</view>
				<view class="inp">
					<input v-model="exmTitle" type="text" placeholder="请输入案例标题" @input="onInput" :maxlength="15" />
					<view class="num">
						<text>{{maxlen}}</text>/<text>15</text>
					</view>
				</view>
			</view>
			<view class="title">
				<template v-if="exmIntro!=null&&exmIntro!=''">
					<view class="inp">
						<navigator url="" @click="addExmIntro" class="txt"><text style="color: red;">*</text>案例内容<uni-icons type="forward" size="20"
								color="#666666"></uni-icons>
						</navigator>
						<view class="jswrap1" @click="addExmIntro">
							<mz-editor-parser :datalist="exmIntro"></mz-editor-parser>
						</view>
					</view>
				</template>
				<template v-else>
					<view class="inp">
						<view class="txt">
							<text style="color: red;">*</text>案例内容
						</view>
						<view class="jswrap">
							<navigator url="" class="border-all" @click="addExmIntro">
								<view class="uptip">
									<uni-icons custom-prefix="my-icon" type="my-icon-icon_add" size="14"
										color="#50A6FA">
									</uni-icons>
									<text style="margin-left: 10rpx;">添加案例内容详情</text>
								</view>
								<text class="dtip">向客户更好的展示成功案例</text>
							</navigator>
						</view>
					</view>
				</template>

			</view>
		</view>
		<view class="bnt_con" v-if="canClick">
			<view class="btn" @click.stop="submit">
				保存
			</view>
		</view>
		<view class="bnt_con" v-if="!canClick">
			<view class="btn" style="opacity: 0.3;">
				保存
			</view>
		</view>
	</view>
</template>

<script>
	import {
		getExampleInfo,
		editExample
	} from '@/api/example.js'
	import request from '@/common/request.js'
	import {
		uploadFile
	} from '@/api/file.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				maxlen: 0,
				exmTitle: '', //案例标题
				exmIntro: null, //案例内容
				infoStr: null, //用于传递到书写案例内容详情的页面
				exmImg: '', //案例封面
				userInfo: null, //用户信息
				orgId: null,
				exm_id: '',
				canClick: true //是否允许被点击
			}
		},
		onLoad(options) {
			this.exm_id = options.id;
			// console.log("传过来的案例id", options.id);
			this.reloadpage();
		},
		onShow() {
			this.imgKey = new Date().getTime(); //将时间戳设置为key
		},
		methods: {
			preViewImg(index) { //预览图片
				let imgList = [];
				imgList.push(this.exmImg)
				uni.previewImage({
					current: index,
					urls: imgList
				});
			},
			openUpload() {
				//上传图片
				uni.chooseImage({
					count: 1,
					sizeType: ['compressed'],
					success: async (res) => {
						// console.log("上传图片", res);
						if (res.tempFiles[0].size > 10 * 1024 * 1024) {
							uni.showToast({
								title: '上传图片不能超过10M',
								icon: "none",
								duration: 2000
							});
							return;
						}
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
						let paths = res.tempFilePaths[0];

						let files = await uploadFile(paths);
						this.exmImg = request.config.baseURL + files
					}
				})
			},
			//
			async reloadpage() {
				this.userInfo = await this.$store.dispatch("userInfo");
				// console.log("用户信息", this.userInfo);
				this.orgId = this.userInfo.OrgId
				uni.showLoading({
					title: '加载中'
				})
				try {
					getExampleInfo({
						id: this.exm_id
					}).then(res => {
						// console.log("产品信息", res);
						this.exmTitle = res.data.Title;
						this.exmIntro = res.data.Detail;
						this.exmImg = res.data.ImageUrl;
						this.maxlen = this.exmTitle.length;
					})
				} catch (error) {
					console.log(error);
				} finally {
					setTimeout(() => {
						uni.hideLoading()
					}, 1000)

				}


			},
			//用于接收上一个页面传递过来的案例详情
			getIntro(params) {
				// console.log("上个页面传递过来的参数", params);
				this.exmIntro = params;
			},
			onInput(e) {
				// 【不用v-model绑定表单,直接时间获取值】这种方式是uni-app官方的方式,测试结果正确！
				let str = e.detail.value;
				let len = String(str).length;
				this.maxlen = len;
			},
			//填写案例内容
			addExmIntro() {
				this.infoStr = {
					infoStr: this.exmIntro
				};
				this.$store.state.submitMod.formArrary.push(this.infoStr);
				uni.navigateTo({
					url: '/pages/user/content_editor?urlType=example'
				});

			},
			submit() {
				if (this.exmTitle == "") {
					uni.showToast({
						title: "案例标题不能为空",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.exmImg == "") {
					uni.showToast({
						title: "请先上传案例封面",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.exmIntro == null || this.exmIntro == '') {
					uni.showToast({
						title: "案例内容不能为空",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.exmTitle != "" && this.exmImg != "" && this.exmIntro != null && this.exmIntro != '' && this
					.orgId != null) {
					let data = {
						id: this.exm_id,
						orgId: this.orgId,
						title: this.exmTitle,
						imageUrl: this.exmImg,
						detail: this.exmIntro
					}
					// console.log("需要上传的参数", data);

					uni.showLoading({
						title: '加载中...'
					});
					try {
						editExample(data).then(res => {
							// console.log("编辑后的返回值",res);
							if (res.data > 0) {
								this.canClick = false
								uni.showToast({
									icon: 'success',
									title: '修改成功',
									duration: 1500
								});
								setTimeout(() => {
									// reloadPrePage();
									reloadPrePage(1, "edit");
									uni.navigateBack();
								}, 1000);
							} else {
								uni.showToast({
									icon: 'error',
									title: '修改失败',
									duration: 1500
								});
							}
						});

					} catch (err) {
						console.info('异常：', err);
					} finally {
						uni.hideLoading();
					}

				} else {
					uni.showToast({
						title: "企业id为空",
						icon: "none",
						duration: 2000
					})
				}
			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFFFFF;
	}

	.bnt_con {
		width: 750rpx;
		padding: 39rpx 0 30rpx 0;
		position: fixed;
		bottom: 0;
		left: 0;
		display: flex;
		justify-content: center;
		background-color: #FFFFFF;
		opacity: 1;
		z-index: 9;

		.btn {
			width: 690rpx;
			height: 90rpx;
			line-height: 90rpx;
			text-align: center;
			color: #FFFFFF;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			border-radius: 8rpx;
			font-size: 30rpx;
		}
	}

	.wrap {
		width: 100%;
		box-sizing: border-box;
		padding: 30rpx;
		background-color: #FFFFFF;
		padding-bottom: 136rpx;

		.top {
			width: 100%;
			display: flex;
			justify-content: center;
			margin-top: 25rpx;

			.photo {
				width: 160rpx;
				height: 160rpx;
				border-radius: 8rpx;
				display: flex;
				justify-content: center;
				align-items: center;
				text-align: center;
				color: #999999;
				font-size: 24rpx;
				line-height: 160rpx;
				position: relative;
				background-color: #F1F2F3;

				.img {
					width: 160rpx;
					height: 160rpx;

					image {
						width: 160rpx;
						height: 160rpx;
						border-radius: 8rpx;
					}

					image.image {
						position: absolute;
						bottom: 0;
						right: 0;
						width: 60rpx;
						height: 60rpx;

					}
				}


			}
		}

		.title {
			margin-top: 59rpx;
			margin-bottom: 30rpx;

			.txt {
				color: #666666;
				font-size: 28rpx;
				font-weight: bold;
				height: 60rpx;
				line-height: 60rpx;
				display: flex;
				align-items: center;
				justify-content: flex-start;
			}

			.inp {
				width: 100%;
				margin-top: 30rpx;
				margin-bottom: 40rpx;
				color: #666666;
				// height: 100rpx;

				.jswrap1 {
					background-color: #FFFFFF;
				}

				.jswrap {
					background-color: #F1F2F3;
				}

				.jswrap1,
				.jswrap {
					display: flex;
					flex-direction: column;
					border-radius: 8rpx;

					.border-all {
						margin: 60rpx;
					}

					.border-all:after {
						border-radius: 4rpx;

					}

					.uptip {
						color: #50A6FA;
						display: flex;
						justify-content: center;
						padding-top: 60rpx;
						padding-bottom: 10rpx;

					}

					.dtip {
						font-size: 22rpx;
						color: #666666;
						display: flex;
						justify-content: center;
						padding-bottom: 50rpx;
					}
				}

				.num {
					display: flex;
					font-size: 28rpx;
					color: #999999;
					position: absolute;
					right: 24rpx;
					top: 30rpx;
				}

				input::-ms-input-placeholder {
					color: #999999;
				}

				input {
					height: 100rpx;
					box-sizing: border-box;
					// padding: 30rpx;
					padding-left: 30rpx;
					padding-right: 98rpx;
					line-height: 100rpx;
					width: 100%;
					border-radius: 8rpx;
					background-color: #F1F2F3;
					font-size: 28rpx;
					color: #666666;
				}
			}
		}
	}
</style>
