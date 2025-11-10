<template>
	<view>
		<view class="wrap">
			<view class="top">
				<!-- <navigator url="/pages/user/choice_img" class="photo"> -->
				<view class="photo" @click="openUpload">
					<view class="img" :key="imgKey">
						<template>
							<image v-if="proImg" class="proImg" @click.stop="preViewImg" :src="proImg" mode="aspectFill"></image>
							<text class="text" v-else>产品照片</text>
						</template>
						<image class="images" src="../../static/user/photo.png"></image>
					</view>
				</view>
				<!-- </navigator> -->
			</view>
			<view class="title">
				<view class="txt">
					<text style="color: red;">*</text>产品名称
				</view>
				<view class="inp">
					<input class="input" type="text" v-model="proName" placeholder="请输入产品名称" @input="onInput"
						:maxlength="15" />
					<view class="num">
						<text>{{maxlen}}</text>/<text>15</text>
					</view>
				</view>
			</view>
			<view class="title">
				<view class="txt">
					<text style="color: red;">*</text>产品归属部门
				</view>
				<uni-data-picker ref="bmpick" popup-title="请选择部门" :value="yqdeptId" :localdata="deptTree"
					@change="onChangeDep" :map="{text:'label',value:'id',children:'children'}">
					<view class="inp" @click.stop="onPopDept()">
						<view class="right color-dept" :style="[headerStyle]">
							{{yqdeptName}}
							<uni-icons type="forward" size="20" color="#999999"></uni-icons>
						</view>
					</view>
				</uni-data-picker>

			</view>
			<view class="title">
				<view class="txt">
					<text style="color: red;">*</text>产品分类
				</view>
				<uni-data-picker ref="flpick" popup-title="请选择产品分类" :value="flValue" :localdata="classTree"
					@change="onChangeFL" :map="{text:'label',value:'id',children:'children'}">
					<view class="inp" @click.stop="onFLClick()">
						<view class="right color-class" :style="[headerStyle2]">
							{{flName}}
							<uni-icons type="forward" size="20" color="#999999"></uni-icons>
						</view>
					</view>
				</uni-data-picker>
			</view>
			<view class="title">
				<view class="txt">
					<text style="color: red;">*</text>产品单价
				</view>
				<view class="inp">
					<input class="input" v-model="proUnitPrice" type="digit" placeholder="请输入产品单价" :maxlength="15" />
				</view>
			</view>
			<view class="title">
				<view class="txt">
					<text style="color: red;">*</text>产品批发价
				</view>
				<view class="inp">
					<input class="input" v-model="proTradePrice" type="digit" placeholder="请输入产品批发价" :maxlength="15" />
				</view>
			</view>
			<view class="title">
				<template v-if="proIntro!=null&&proIntro!=''">
					<view class="inp">
						<navigator url="" @click="addProIntro" class="txt"><text style="color: red;">*</text>产品详情<uni-icons type="forward" size="20"
								color="#666666"></uni-icons>
						</navigator>
						<view class="jswrap1" @click="addProIntro">
							<mz-editor-parser :datalist="proIntro"></mz-editor-parser>
						</view>
					</view>
				</template>
				<template v-else>
					<view class="inp">
						<view class="txt">
							<text style="color: red;">*</text>产品详情
						</view>
						<view class="jswrap">
							<navigator url="" class="border-all" @click="addProIntro">
								<view class="uptip">
									<uni-icons custom-prefix="my-icon" type="my-icon-icon_add" size="14"
										color="#50A6FA">
									</uni-icons>
									<text style="margin-left: 10rpx;">添加产品详情</text>
								</view>
								<text class="dtip">为客户更好的了解产品</text>
							</navigator>
						</view>
					</view>
				</template>


			</view>
		</view>
		<view class="bnt_con" v-if="canClick">
			<view class="btn" @click.stop="submit" style="cursor: pointer;">
				保存
			</view>
		</view>
		<view class="bnt_con" v-if="!canClick">
			<view class="btn" style="opacity: 0.3;cursor:not-allowed">
				保存
			</view>
		</view>


	</view>
</template>

<script>
	import {
		uploadFile
	} from '@/api/file.js'
	import {
		getProductMessage,
		editProduct,
		getClassTree
	} from '@/api/product.js'
	import {
		getDeptTree
	} from '@/api/dept.js'
	import {
		totree,
		randomStr,
		reloadPrePage,
		isNullOrEmpty
	} from '@/common/util.js'
	import request from '@/common/request.js'
	export default {
		data() {
			return {
				maxlen: 0,
				pro_id: null, //产品id
				proName: '', //产品名称
				proUnitPrice: null, //产品单价
				proTradePrice: null, //产品批发价
				proImg: '', //照片地址
				imgKey: '',
				proIntro: null, //产品详情
				infoStr: null, //用于传递到书写产品详情的页面
				flName: '请选择产品分类',
				flValue: '', //产品分类id
				// flItems: [], //产品分类列表
				orgId: '', //企业id
				yqdeptName: "请选择部门",
				deptTree: [], //部门树状结果
				yqdeptId: 0, //部门id
				classTree: [], //分类树状结果
				productInfo: null, //产品信息
				headerStyle: {
					'--font--color': '#999999',
				},
				headerStyle2: {
					'--font--color2': '#999999',
				},
				canClick:true



			}
		},

		onLoad(options) {

			this.orgId = options.orgId;
			this.pro_id = options.id;
			// console.log("产品id", options.id);

			this.reloadpages();



		},
		onShow() {
			this.imgKey = new Date().getTime(); //将时间戳设置为key

		},
		methods: {
			preViewImg(index) {//预览图片
			let imgList=[];
			imgList.push(this.proImg)
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
						console.log("上传图片", res);
						if(res.tempFiles[0].size>10*1024*1024){
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
						this.proImg = request.config.baseURL + files
					}
				})
			},
			//用于接收上一个页面传递过来的产品详情
			getIntro(params) {
				// console.log("上个页面传递过来的参数",params);
				this.proIntro = params;
			},
			//部门下拉显示
			onPopDept() {
				this.$refs.bmpick.show();
			},
			//选择部门
			onChangeDep(e) {
				if (e.detail.value.length > 0) {
					this.yqdeptId = e.detail.value[e.detail.value.length - 1].value;
					if (this.yqdeptId == -1) {
						this.yqdeptId = e.detail.value[e.detail.value.length - 2].value;
						this.yqdeptName = e.detail.value[e.detail.value.length - 2].text;
					} else {
						this.yqdeptName = e.detail.value[e.detail.value.length - 1].text;
					}
					this.headerStyle = {
						'--font--color': '#666666'
					}

				}

			},

			//获取产品详情以及产品分类的分类列表
			async reloadpages() {
				uni.showLoading({
					title: '加载中'
				});
				try {
					let userIntro = await this.$store.dispatch("userInfo");
					// console.log("用户信息", userIntro);
					let productInfo = await getProductMessage({
						id: this.pro_id,
						OrgId: userIntro.OrgId
					})
					// console.log("产品信息", productInfo);
					this.productInfo = productInfo.data; //
					this.proName = this.productInfo.ProName; //产品名称
					this.proImg = this.productInfo.ImageUrl; //产品图片
					this.proUnitPrice = this.productInfo.Price; //产品单价
					this.proTradePrice = this.productInfo.WholePrice; //产品批发价
					this.proIntro = this.productInfo.Detail; //产品详情
					this.flValue = this.productInfo.CategoryId; //分类id
					this.yqdeptId = this.productInfo.DeptId; //部门id
					this.maxlen = this.proName.length

					//获取部门列表
					getDeptTree({
						OrgId: userIntro.OrgId
					}).then(rsp => {
						this.deptTree = rsp.data;
						// console.log("部门数据", this.deptTree);
						let it = this.findTreeName(rsp.data[0], rsp.data[0].id);
						this.deptInit(rsp.data[0]);
					})
					//获取产品信息

					//获取分类树状列表
					getClassTree({
						OrgId: userIntro.OrgId,
						// ParentId:0
					}).then(rsp => {
						// console.log("树状分类结果", rsp);
						this.classTree = rsp.data;
						// console.log("分类数据", this.classTree);
						for (let i = 0; i < rsp.data.length; i++) {
							let it = this.findClassTreeName(rsp.data[i], rsp.data[i].id);
							this.classInit(rsp.data[i]);
						}

					})
					//获取分类树状列表

					//根据id获取产品分类名称
					// console.log("用户产品信息", this.productInfo);
					this.$store.dispatch("flClassName", this.flValue).then(res2 => {
						// console.log("这里查询分类单个id对应的名称", res2);
						this.flName = res2;
					})
					this.headerStyle2 = {
						'--font--color': '#666666'
					}
					//根据id获取部门名称
					// console.log("用户产品信息", this.productInfo);
					this.$store.dispatch("deptName", this.yqdeptId).then(res3 => {
						// console.log("这里查询部门单个id对应的名称", res3);
						this.yqdeptName = res3;
					})
					this.headerStyle = {
						'--font--color': '#666666'
					}

				} catch (err) {
					console.log('异常', err);
				} finally {
					uni.hideLoading();
				}
			},

			//部门下的子集部门查询
			deptInit(item) {
				if (!item.hasOwnProperty("children") || item.children.length == 0) {
					return;
				}
				for (let i = 0; i < item.children.length; i++) {
					this.deptInit(item.children[i]);
				}
				item.children.splice(0, 0, {
					"label": "--",
					"id": -1
				});
			},
			findTreeName(item, id) {
				if (item.id == id) {
					return item;
				}
				if (!item.hasOwnProperty("children")) {
					return null;
				}
				for (let i = 0; i < item.children.length; i++) {
					let rt = this.findTreeName(item.children[i], id);
					if (rt != null) {
						return rt;
					}
				}
				return null;
			},
			//部门下的子集部门查询
			//产品分类下的子集部门查询
			classInit(item) {
				if (!item.hasOwnProperty("children") || item.children.length == 0) {
					return;
				}
				for (let i = 0; i < item.children.length; i++) {
					this.classInit(item.children[i]);
				}
				item.children.splice(0, 0, {
					"label": "--",
					"id": -1
				});
			},
			findClassTreeName(item, id) {
				if (item.id == id) {
					return item;
				}
				if (!item.hasOwnProperty("children")) {
					return null;
				}
				for (let i = 0; i < item.children.length; i++) {
					let rt = this.findClassTreeName(item.children[i], id);
					if (rt != null) {
						return rt;
					}
				}
				return null;
			},
			//产品分类下的子集部门查询
			onFLClick() {
				this.$refs.flpick.show();
			},
			onChangeFL(e) {
				// console.log("eeeeeee", e);
				// this.flName = e.detail.value[e.detail.value.length - 1].text;
				// this.flValue = e.detail.value[e.detail.value.length - 1].value;
				if (e.detail.value.length > 0) {
					this.flValue = e.detail.value[e.detail.value.length - 1].value;
					if (this.flValue == -1) {
						this.flValue = e.detail.value[e.detail.value.length - 2].value;
						this.flName = e.detail.value[e.detail.value.length - 2].text;
					} else {
						this.flName = e.detail.value[e.detail.value.length - 1].text;
					}
					this.headerStyle2 = {
						'--font--color': '#666666'
					}

				}
				// this.headerStyle2 = {
				// 	'--font--color2': '#666666'
				// }
			},
			//提交数据添加产品，并做不能为空判断
			submit() {

				if (this.proName == "") {
					uni.showToast({
						title: "产品名称不能为空",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.flValue == "") {
					uni.showToast({
						title: "请选择产品分类",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.proUnitPrice == null) {
					uni.showToast({
						title: "产品单价不能为空",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.proTradePrice == null) {
					uni.showToast({
						title: "产品批发价不能为空",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.yqdeptId == null) {
					uni.showToast({
						title: "部门不能为空",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.proIntro == null) {
					uni.showToast({
						title: "产品详情不能为空",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.proIntro != null && this.yqdeptId != null && this.proUnitPrice != null && this.proName != "" &&
					this.flValue != ""&&this.flValue != null && this.proTradePrice != null) {
					let data = {
						id: this.pro_id,
						orgId: this.orgId,
						categoryId: this.flValue,
						imageUrl: this.proImg,
						proName: this.proName,
						price: this.proUnitPrice,
						wholePrice: this.proTradePrice,
						detail: this.proIntro,
						deptId: this.yqdeptId
					};
					// console.log("添加的数据", data);
					uni.showLoading({
						title: '加载中...'
					});
					try {
						editProduct(data).then(res => {
							if (res.data > 0) {
								this.canClick=false
								uni.showToast({
									icon: 'success',
									title: '修改成功',
									duration: 1500
								});
								setTimeout(() => {
									// reloadPrePage();
									uni.navigateBack();
								}, 1500);
							}
						});

					} catch (err) {
						console.info('异常：', err);
					} finally {
						uni.hideLoading();
					}
				}
			},

			//获取输入的产品详情
			addProIntro() {
				this.infoStr = {
					infoStr: this.proIntro
				};
				this.$store.state.submitMod.formArrary.push(this.infoStr);
				uni.navigateTo({
					url: '/pages/user/content_editor'
				});

			},
			onInput(e) {
				// 【不用v-model绑定表单,直接时间获取值】这种方式是uni-app官方的方式,测试结果正确！
				let str = e.detail.value;
				let len = String(str).length;
				this.maxlen = len;



			},


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

					.proImg {
						width: 160rpx;
						height: 160rpx;
						border-radius: 8rpx;
					}

					.images {
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
			margin-top: 39rpx;
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
				position: relative;

				.right {
					background-color: #F1F2F3;
					height: 100rpx;
					line-height: 100rpx;
					width: 100%;
					box-sizing: border-box;
					border-radius: 8rpx;
					padding-left: 30rpx;
					position: relative;

					uni-icons {
						position: absolute;
						right: 20rpx;
						// height: 30rpx;
					}
				}

				.color-dept {
					color: var(--font--color);

				}

				.color-class {
					color: var(--font--color2);

				}

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

				.input::-ms-input-placeholder {
					color: #999999;
				}

				.input {
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

		.isNew {
			background-color: #F1F2F3;
			border-radius: 8rpx;
			padding: 0 33rpx;
			box-sizing: border-box;
			color: #666666;
			display: flex;
			justify-content: space-between;
			width: 100%;
			height: 100rpx;
			align-items: center;
			margin-bottom: 120rpx;
		}

		switch {
			transform: scale(0.7)
		}
	}
</style>
