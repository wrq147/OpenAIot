<template>
	<view>
		<view class="wrap">
			<view class="mp">
				<!-- #ifdef MP-WEIXIN -->
				<canvas id="canvas" type="2d" style="width:690rpx; height:400rpx;" />
				<!-- #endif -->
				<!-- #ifndef MP-WEIXIN -->
				<canvas canvas-id="canvas" id="canvas" style="width:690rpx; height:400rpx;" />
				<!-- #endif -->
			</view>
		</view>
		<view class="common_wrap2" style="margin-top:0">
			<view @click="jumpToStyle" class="row navigator">
				<view class="mdtxt">名片模块</view>
				<view class="right">
					<view class="mbwrap">
						<image class="pbimg" mode="aspectFill" :src="curmbyx"></image>
						<text class="pbtip">更换名片模版</text>
					</view>
					<view class="iconwrap">
						<uni-icons type="forward" size="20" color="#666666"></uni-icons>
					</view>
				</view>
			</view>
			<navigator :url="'/pages/card_center/card_share?id='+cardId" class="row">
				<view class="mdtxt">分享标题</view>
				<view class="right">
					<view class="txt txt-tip">{{cardData.ShareTitle}}</view>
					<view class="iconwrap">
						<uni-icons type="forward" size="20" color="#666666"></uni-icons>
					</view>
				</view>
			</navigator>
		</view>
		<view class="common_wrap2">
			<view class="row" id="realNameId">
				<view class="mdtxt">姓名<text class="lbred">*</text></view>
				<view class="right ipt">
					<!-- <uni-easyinput trim="both" maxlength="20" @input="changeDraw" v-model="cardData.RealName"
						:inputBorder="false" placeholder="请输入姓名">
					</uni-easyinput> -->
					<uni-easyinput class="weui-input" placeholder-style="font-size:26rpx;color:rgba(203, 203, 203, 1);"
						type="nickname" trim="both" maxlength="20" @input="changeDraw" v-model="cardData.RealName"
						:inputBorder="false" placeholder="请输入姓名">
					</uni-easyinput>
				</view>
			</view>
			<view class="row">
				<view class="mdtxt">头像</view>
				<view class="right">
					<!-- <view class="txt" style="width: 68rpx !important;"> -->
					<image class="avatar" :src="cardData.Avatar?cardData.Avatar:'../../static/user/user_man.png'"
						mode="aspectFill" @tap.stop="seeImg(cardData.Avatar)" @click.stop="seeImg(cardData.Avatar)"
						style="width: 68rpx;height: 68rpx;">
					</image>
					<!-- </view> -->
					<button @click="onChooseAvatar" class="txt avatar-wrapper iconwrap" open-type="chooseAvatar"
						@chooseavatar="onChooseAvatar" style="background-color: #fff;height: 68rpx;">
						<!-- <view class="iconwrap"> -->
							<uni-icons type="forward" size="20" color="#666666"></uni-icons>
						<!-- </view> -->
					</button>

				</view>
			</view>
		</view>
		<view class="common_wrap2">
			<view class="title" id="mobileId">联系信息</view>
			<view class="row">
				<view class="mdtxt">手机</view>
				<view class="right ipt">
					<uni-easyinput class="iptTxt" placeholder-style="font-size:26rpx;color:rgba(203, 203, 203, 1);"
						type="number" @input="changeDraw" trim="both" maxlength="11" v-model.lazy="cardData.Mobile"
						:inputBorder="false" placeholder="请输入手机号码">
					</uni-easyinput>
				</view>
			</view>
			<view class="row">
				<view class="mdtxt">微信</view>
				<view class="right ipt">
					<uni-easyinput class="iptTxt" placeholder-style="font-size:26rpx;color:rgba(203, 203, 203, 1);"
						trim="both" maxlength="20" v-model="cardData.WxNumber" @input="changeDraw" :inputBorder="false"
						placeholder="请输入微信号">
					</uni-easyinput>
				</view>
			</view>
			<view class="row">
				<view class="mdtxt">邮箱</view>
				<view class="right ipt">
					<uni-easyinput class="iptTxt" placeholder-style="font-size:26rpx;color:rgba(203, 203, 203, 1);"
						trim="both" maxlength="50" v-model="cardData.Email" @input="changeDraw" :inputBorder="false"
						placeholder="请输入邮箱">
					</uni-easyinput>
				</view>
			</view>
			<view class="row">
				<view class="mdtxt">所在城市</view>
				<view :class="[cardData.AddressName=='请选择'||cardData.AddressName==''?'tishi':'ipt','right']"
					@click="chooseLocation" style="margin-left: 0;">
					{{cardData.AddressName==''?'请选择':cardData.AddressName}}
					<uni-icons type="forward" size="20" color="#666666"></uni-icons>
				</view>
			</view>
			<view class="row">
				<view class="mdtxt">详细地址</view>
				<view class="right ipt">
					<uni-easyinput class="iptTxt" placeholder-style="font-size:26rpx;color:rgba(203, 203, 203, 1);"
						trim="both" maxlength="225" v-model="cardData.AddressDetail" @input="changeDraw"
						:inputBorder="false" placeholder="请输入详细地址">
					</uni-easyinput>
				</view>
			</view>
			<view class="row">
				<view class="mdtxt">官网</view>
				<view class="right ipt">
					<uni-easyinput class="iptTxt" placeholder-style="font-size:26rpx;color:rgba(203, 203, 203, 1);"
						trim="both" maxlength="50" v-model="cardData.Website" @input="changeDraw" :inputBorder="false"
						placeholder="请输入官网">
					</uni-easyinput>
				</view>
			</view>
		</view>
		<view class="common_wrap2">
			<view class="title">企业信息</view>
			<navigator url="" class="row" @click="changeCompany">
				<view class="mdtxt">企业</view>
				<view class="right">
					<view :class="[cardData.OrgId>0?'':'txt-tip2','txt']">
						{{cardData.OrgId>0?cardData.OrgName:'请选择名片所属企业'}}
					</view>
					<view class="iconwrap">
						<uni-icons type="forward" size="20" color="#666666"></uni-icons>
					</view>
				</view>
			</navigator>
			<view class="row" v-if="cardData.OrgId>0">
				<view class="mdtxt">部门</view>
				<view class="right ipt">
					<uni-easyinput placeholder-style="font-size:26rpx;color:rgba(203, 203, 203, 1);" class="iptTxt"
						trim="both" maxlength="30" v-model="cardData.DeptName" @input="changeDraw" :inputBorder="false"
						placeholder="请输入所属部门">
					</uni-easyinput>
				</view>
			</view>
			<view class="row" v-if="cardData.OrgId>0">
				<view class="mdtxt">职位</view>
				<view class="right ipt">
					<uni-easyinput class="iptTxt" placeholder-style="font-size:26rpx;color:rgba(203, 203, 203, 1);"
						trim="both" maxlength="50" v-model="cardData.PostName" @input="changeDraw" :inputBorder="false"
						placeholder="请输入所属职位">
					</uni-easyinput>
				</view>
			</view>
		</view>
		<view class="common_wrap2">
			<template v-if="cardData.Intro!=null&&cardData.Intro!=''">
				<navigator url="" @click="nav2Intro" class="title">个人介绍<uni-icons type="forward" size="20"
						color="#666666"></uni-icons>
				</navigator>
				<view class="jswrap" @click="nav2Intro">
					<mz-editor-parser :datalist="cardData.Intro"></mz-editor-parser>
				</view>
			</template>
			<template v-else>
				<view class="title">个人介绍</view>
				<view class="jswrap">
					<navigator url="" class="border-all" @click="nav2Intro">
						<view class="uptip">
							<uni-icons custom-prefix="my-icon" type="my-icon-icon_add" size="14" color="#50A6FA">
							</uni-icons>
							<text style="margin-left: 10rpx;">添加个人介绍</text>
						</view>
						<text class="dtip">向客户更好的介绍您自己吧</text>
					</navigator>
				</view>
			</template>
		</view>
		<view style="height: 152rpx;"></view>
		<view class="edbtn">
			<button class="btn" @click="saveClick">保存</button>
		</view>

	</view>
</template>

<script>
	import {
		serverUrl
	} from '@/common/request.js'
	import {
		drawCard
	} from '@/common/card.js'
	import {
		getCard,
		editCard
	} from '@/api/userCard.js'

	import {
		userOrgList
	} from '@/api/org.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	import {
		reverseGeocoder
	} from '@/api/address.js'
	import request from '@/common/request.js'
	import {
		uploadFile
	} from '@/api/file.js'
	export default {
		data() {
			return {
				cardId: 0,
				cardData: {
					RealName: "",
					AddressName: "请选择",
					AddressCode: '',
					AddressDetail: '',
					Lng: -1,
					Lat: -1
				}
			}
		},
		computed: {
			curmbyx() {
				if (this.cardData.TemplateId) {
					if (this.cardData.TemplateId == null) {
						return "";
					}
				} else {
					this.cardData.TemplateId = 1
				}

				return serverUrl + "/yx/" + this.cardData.TemplateId + ".png";
			}
		},
		onLoad(options) {
			// console.info(this.cardData.AddressName == '' || this.cardData.AddressName == '请选择')
			this.cardId = parseInt(options.id || "0");
		},
		async onReady() {
			await this.reloadpage();
		},
		methods: {
			seeImg(img) { //预览头像
				if (img) {
					let list = []
					list.push(img)
					uni.previewImage({
						urls: list,
						longPressActions: {
							success: function(data) {

							},
							fail: function(err) {
								console.log(err.errMsg);
							}
						}
					});
				}
			},
			async onChooseAvatar(e) {
				if (this.$store.state.sysType == 'windows') {
					uni.navigateTo({
						url: '/pages/card_center/card_avator?id=' + this.cardId
					})
				} else {
					// console.log("头像的只", e);
					let respath = await uploadFile(e.detail.avatarUrl);
					// console.log("上传以后", respath);
					await editCard({
						Id: this.cardId,
						Avatar: respath
					});
					// this.cardData.disAvatarUrl = respath
					this.cardData.Avatar = request.config.baseURL + respath
					this.$store.commit('SET_ISNEWCARD_DATA', true); //名片样式改变，设置名片重新刷新
				}
				// console.log("上传以后名片", this.cardData);

			},
			jumpToStyle() {
				//跳转到名片设计处
				this.$store.state.submitMod.formArrary.push(this.cardData);
				uni.navigateTo({
					url: '/pages/card_center/cardStyle_setting'
				})
			},
			chooseLocation() {
				uni.chooseLocation({
					success: res => {
						// console.log("地址",res);
						// this.cardData.AddressName = res.name;
						// this.cardData.AddressDetail = res.address;
						this.cardData.Lng = res.longitude;
						this.cardData.Lat = res.latitude;
						reverseGeocoder(this.cardData.Lng, this.cardData.Lat).then(async (rsp) => {
							// console.log("地址结果", rsp);
							this.cardData.AddressCode = rsp.ad_info.adcode;
							
							let addressArea = ''
							if (rsp.address_component.province == rsp.address_component.city) {
								addressArea = rsp.address_component.province + rsp.address_component
									.district
							} else {
								addressArea = rsp.address_component.province + rsp.address_component
									.city + rsp.address_component.district
							}
							this.cardData.AddressName = addressArea
							
							this.cardData.AddressDetail = (rsp.address_reference.town.title ? rsp
								.address_reference.town.title : '') + rsp.address.substr(this
								.cardData.AddressName.length) + rsp.address_reference.landmark_l2.title
							if (res.address) {
								this.cardData.AddressDetail = res.address.substr(this.cardData.AddressName.length) + res
									.name
							}
							await drawCard('canvas', this.cardData);
							// formatted_addresses:
							// recommend: "南安市丰城小学"

						});

					}
				});
			},
			async changeCompany() {
				try {
					//获取用户的企业列表
					let rsp = await userOrgList();
					if (rsp.data.length == 0) {
						uni.showModal({
							title: '提示',
							content: '暂未加入任何企业，是否创建新的企业',
							success: function(res) {
								if (res.confirm) {
									uni.navigateTo({
										url: '/pages/company/company_add'
									});
								}
							}
						});
						return;
					}
					this.$store.state.submitMod.formArrary.push(this.cardData);
					uni.navigateTo({
						url: '/pages/company/company_sel'
					});

				} catch (err) {
					console.info('异常：', err);
				}
			},
			async reloadpage(name) {
				try {
					let rsp = await getCard(this.cardId);
					// console.log("名片编辑", rsp);
					if (name == "style") {
						// console.log("名片样式返回",this.cardData);
						this.cardData.TemplateId = rsp.data.TemplateId;
						this.cardData.TemplateBk = rsp.data.TemplateBk;
						await drawCard('canvas', this.cardData);
					} else if (name == "share") {
						this.cardData.ShareTitle = rsp.data.ShareTitle;
					} else if (name == "intro") {
						this.cardData.Intro = rsp.data.Intro;
					} else if (name == "company") {
						await drawCard('canvas', this.cardData);
					} else if (name == "avator") {
						this.cardData.Avatar = rsp.data.Avatar;
						await drawCard('canvas', this.cardData);
					} else if (name == "addressName") {
						this.cardData.AddressName = rsp.data.AddressName;
						await drawCard('canvas', this.cardData);
					} else if (name == "addressDetail") {
						this.cardData.AddressDetail = rsp.data.AddressDetail;
						await drawCard('canvas', this.cardData);
					} else {
						// console.log("else执行力");
						this.cardData = rsp.data;
						await drawCard('canvas', this.cardData);
					}

					// this.$forceUpdate();
				} catch (err) {
					console.info('异常：', err);
				}
			},
			async changeDraw(e) {
				await drawCard('canvas', this.cardData);
			},

			nav2Intro() {
				this.$store.state.submitMod.formArrary.push(this.cardData);
				uni.navigateTo({
					url: '/pages/card_center/intro_editor'
				});

			},
			async saveClick() {
				if (this.cardData.RealName == "") {
					uni.pageScrollTo({
						selector: "#realNameId",
						duration: 50
					});
					uni.showToast({
						title: "姓名不能为空",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.cardData.Mobile) {
					if (!(/^1[3456789]\d{9}$/.test(this.cardData.Mobile))) {
						uni.showToast({
							title: '手机号码格式错误',
							icon: "none"
						});
						return;
					}
				}
				if (this.cardData.Email) {
					if (!(/^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.[a-zA-Z0-9]{2,6}$/.test(this.cardData
							.Email))) {
						uni.showToast({
							title: '邮箱格式错误',
							icon: "none"
						});
						return;
					}
				}

				// if (this.cardData.Mobile == "") {
				// 	uni.pageScrollTo({
				// 		selector: "#mobileId",
				// 		duration: 50
				// 	});
				// 	uni.showToast({
				// 		title: "手机号码不能为空",
				// 		icon: "none",
				// 		duration: 2000
				// 	});
				// 	return;
				// }

				uni.showLoading({
					title: '加载中...'
				});
				try {

					await editCard({
						Id: this.cardId,
						RealName: this.cardData.RealName,
						Mobile: this.cardData.Mobile,
						WxNumber: this.cardData.WxNumber,
						Website: this.cardData.Website,
						Email: this.cardData.Email,
						OrgId: this.cardData.OrgId,
						DeptName: this.cardData.DeptName,
						PostName: this.cardData.PostName,
						AddressName: this.cardData.AddressName,
						AddressDetail: this.cardData.AddressDetail,
						AddressCode: this.cardData.AddressCode,
						Lng: this.cardData.Lng,
						Lat: this.cardData.Lat

					});
					this.$store.commit('SET_ISNEWCARD_DATA', true); //名片样式改变，设置名片重新刷新
					reloadPrePage(1, "card");
					uni.navigateBack();
				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}

			}
		}
	}
</script>

<style lang="scss">
	.canvas-hide {
		/* 1 */
		position: fixed;
		right: 100vw;
		bottom: 100vh;
		/* 2 */
		z-index: -9999;
		/* 3 */
		opacity: 0;
	}

	.wrap {
		width: 100%;
		box-sizing: border-box;
		padding: 20rpx 30rpx;
		background-color: #FFFFFF;

		.mp {
			width: 690rpx;
			height: 400rpx;
			overflow: hidden;
			border-radius: 10rpx;
			box-sizing: border-box;
			color: #FFFFFF;
			position: relative;
			background-color: #14395F;
			box-shadow: 0px 6px 15px 0px rgba(0, 0, 0, 0.2000);

		}

	}

	.common_wrap2 {
		width: 100%;
		background-color: #FFFFFF;
		margin-top: 20rpx;
		padding-top: 20rpx;
		padding-bottom: 20rpx;

		.title {
			display: flex;
			height: 90rpx;
			font-size: 30rpx;
			color: #333333;
			padding: 0 30rpx;
			align-items: center;
			justify-content: space-between;
		}

		.border-all:after {
			border-radius: 4rpx;
		}

		.jswrap {
			display: flex;
			flex-direction: column;
			padding: 0 30rpx;

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

		.row {
			width: 100%;
			min-height: 90rpx;
			display: flex;
			box-sizing: border-box;
			padding: 0 30rpx;
			height: 90rpx;
			align-items: center;

			.mdtxt {
				color: #666666;
				font-size: 30rpx;
				width: 190rpx;
				text-align: left;
				display: flex;
				align-items: center;

				.lbred {
					color: #ff0000;
					margin-left: 10rpx;
					margin-top: 10rpx;
				}
			}

			.right {
				display: flex;
				align-items: center;
				font-size: 28rpx;
				color: #333333;
				flex: 1;
				justify-content: space-between;

				&.ipt {
					margin-left: -10px;
					font-size: 26rpx;
				}

				&.tishi {
					color: #999999;
					margin-left: 0;
					font-size: 24rpx;
				}

				.mbwrap {
					display: flex;
					justify-content: space-between;
					align-items: center;
					flex: 1;

					.pbimg {
						width: 73rpx;
						height: 43rpx;
					}

					.pbtip {
						color: #50A6FA;
						font-size: 24rpx;
					}
				}

				.txt {
					flex: 1;
					display: flex;
					align-items: center;

					image {
						width: 68rpx;
						height: 68rpx;
						border-radius: 10rpx;
						display: block;
					}

					&.txt-tip {
						font-size: 24rpx;
					}

					&.txt-tip2 {
						font-size: 24rpx;
						color: #999999;
					}
				}


				.iconwrap {
					width: 34rpx;
					height: 90rpx;
					display: flex;
					justify-content: flex-end;
					align-items: center;
					padding-left: 20rpx;
				}

			}

		}

	}

	.edbtn {
		position: fixed;
		bottom: 0;
		background-color: #fff;
		width: 100vw;
		z-index: 15;

		.btn {
			margin: 20rpx 30rpx;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			border-radius: 8rpx;
			color: #ffffff;
		}
	}
</style>
