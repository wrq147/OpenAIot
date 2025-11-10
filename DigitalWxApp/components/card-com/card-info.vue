<template>
	<view>
		<view class="cardwrap">
			<view class="mp">
				<image class="canvesImage" :src="localImg"  mode="aspectFill" style="width:690rpx; height:400rpx;"></image>
			</view>
			<view class="info-block">
				<scroll-view :scroll-x="true" class="info-scroll">
					<block>
						<view class="info-wrap">
							<view class="info-item">
								<navigator url="" class="info-item-fx" @click="onTel(cardData.Mobile)">
									<view class="tp">
										<uni-icons custom-prefix="my-icon" type="my-icon-icon_bphone" color="#50A6FA"
											size="20"></uni-icons>
										<text class="n">电话</text>
									</view>
									<text class="txt">{{cardData.Mobile==""?"暂无电话信息":cardData.Mobile}}</text>
								</navigator>
							</view>
							<view class="info-item" v-if="cardData.WxNumber!=''">
								<navigator url="" class="info-item-fx" @click="onCopy(cardData.WxNumber)">
									<view class="tp">
										<uni-icons custom-prefix="my-icon" type="my-icon-icon_wechat" color="#50A6FA"
											size="20"></uni-icons>
										<text class="n">微信</text>
									</view>
									<text class="txt">{{cardData.WxNumber}}</text>
								</navigator>
							</view>
							<view class="info-item" v-if="cardData.Email!=''">
								<navigator url="" class="info-item-fx" @click="onCopy(cardData.Email)">
									<view class="tp">
										<uni-icons custom-prefix="my-icon" type="my-icon-icon_news" color="#50A6FA"
											size="20"></uni-icons>
										<text class="n">邮箱</text>
									</view>
									<text class="txt">{{cardData.Email}}</text>
								</navigator>
							</view>
							<view class="info-item" v-if="cardData.Website!=''">
								<navigator url="" class="info-item-fx" @click="onCopy(cardData.Website)">
									<view class="tp">
										<uni-icons custom-prefix="my-icon" type="my-icon-icon_eart" color="#50A6FA"
											size="20"></uni-icons>
										<text class="n">官网</text>
									</view>
									<text class="txt">{{cardData.Website}}</text>
								</navigator>
							</view>
							<view style="width: 10rpx;display: inline-block;"></view>
						</view>

					</block>
				</scroll-view>
			</view>
			<view class="map-block" v-if="cardData.Lng!=-1&&cardData.Lat!=-1" style="position: relative;">
				<view style="position: absolute;top: 0;left: 0;width: 100%; height: 300rpx;z-index: 1000;" @click="onMapTap"></view>
				<map @tap="onMapTap" @callouttap="onMapTap" style="width: 100%; height: 300rpx;z-index:200;" :latitude="cardData.Lat"
					:longitude="cardData.Lng" :markers="covers" :enable-scroll="false" :enable-zoom="false">
				</map>
			</view>
			<view class="op-block">
				<view class="in-store" v-if="inHolder">
					<uni-icons custom-prefix="my-icon" type="my-icon-icon_mail" color="#aaaaaa" size="20">
					</uni-icons>
					<text class="txt dis">已存通讯录</text>
				</view>
				<view class="in-store" v-else @click="onStoreIn">
					<uni-icons custom-prefix="my-icon" type="my-icon-icon_mail" color="#50A6FA" size="20">
					</uni-icons>
					<text class="txt">存入通讯录</text>
				</view>
				<button class="midbt" open-type="share">分享TA的名片</button>
				<button class="rightbt" v-if="!exchanging" @click="onExchange">递名片</button>
				<view class="rightbt dis" v-else>已递</view>
			</view>
			<view class="vis-block" v-if="visitCount>0">
				<view class="ximglist">
					<template v-for="img in visitImgs">
						<fr-image class="ximg" :lazy-load="true" mode="widthFix" :src="img"
							loading-ing-img="oblique-light" />
					</template>
				</view>
				<text class="txt">
					{{visitCount}}人浏览过
				</text>
			</view>
		</view>

		<view class="introwrap" v-if="tagIdx>=0">
			<view class="intro-hd border-bottom">
				<view v-if="cardData.Intro != ''" :class="{'sel-item':true,'ac':tagIdx==0}" @click="onTag(0)">个人介绍
				</view>
				<view v-if="orgData.Intro != ''" :class="{'sel-item':true,'ac':tagIdx==1}" @click="onTag(1)">企业介绍
				</view>
				<view style="flex: 1;"></view>
			</view>
			<view class="intro-content">
				<mz-editor-parser :datalist="tagIntro"></mz-editor-parser>
			</view>
		</view>
		<view v-else style="height: 120rpx;"></view>

	</view>
</template>

<script>
	import {
		getExchangeInfo,
		addExchange
	} from '@/api/userCard.js'
	import {
		getOrg
	} from '@/api/org.js'
	import {
		addHolder
	} from '@/api/userCard.js'
	import {
		getVisitedList
	} from '@/api/record.js'
	export default {
		name: "card-com",
		props: {
			cardData: {
				type: Object,
				default: {
					Mobile: "",
					WxNumber: "",
					Email: "",
					OrgId: 0
				}
			},
			localImg:''
		},
		data() {
			return {
				orgData: {
					Lng: -1,
					Lat: -1,
				},
				covers: [],
				visitImgs: [],
				visitCount: 2,
				tagIdx: -1,
				tagIntro: "",
				exchanging: true,
				inHolder: true
			};
		},
		async mounted() {
			try {

				let usrInfo = await this.$store.dispatch("userInfo");

				getVisitedList({
					VisitType: 0,
					TargetId: this.cardData.Id,
					LastVisited: true,
					showAll:true
				}).then(xrsp => {
					this.visitImgs = xrsp.data.List.map(x => x.Avatar);
					// console.info(xrsp.data.List)
					this.visitCount = xrsp.data.Total;
				});


				let exrsp = await getExchangeInfo(this.cardData.UserId, this.cardData.Id, usrInfo.extObj.CardId);
				this.exchanging = exrsp.data.Exchanging;
				this.inHolder = exrsp.data.InHolder;

				if (this.cardData.OrgId > 0) {
					let rsp = await getOrg(this.cardData.OrgId);
					this.orgData = rsp.data;

					//生成地图标记
					let tmpaddddrrr = "";
					let showAddress=this.cardData.AddressName+this.cardData.AddressDetail
					let tmpaddi = showAddress.length;
					let tmpaddstart = 0;
					while (tmpaddi > 24) {
						tmpaddddrrr = tmpaddddrrr + showAddress.substr(tmpaddstart, 24) + "\n";
						tmpaddi = tmpaddi - 24;
						tmpaddstart += 24;
					}
					tmpaddddrrr = tmpaddddrrr + showAddress.substr(tmpaddstart);
					this.covers = [{
						id: 1,
						latitude: this.cardData.Lat,
						longitude: this.cardData.Lng,
						height: 0,
						width: 0,
						alpha: 0.8,//标注的透明度		默认1，无透明，范围 0 ~ 1
						"callout": {
							"content": tmpaddddrrr, //文本
							"color": '#323232', //文本颜色
							"borderRadius": 6, //边框圆角
							"bgColor": '#ffffff', //背景色
							"padding": 10, //文本边缘留白
							"display": "ALWAYS",
							"textAlign": 'center' //文本对齐方式。有效值: left, right, center
						}
					}];


					if (this.orgData.Intro != "") {
						this.tagIdx = 1;
						this.tagIntro = this.orgData.Intro;
					}
				}



				if (this.cardData.Intro != "") {
					this.tagIdx = 0;
					this.tagIntro = this.cardData.Intro;
				}



			} catch (err) {
				console.info('异常：', err);
			}
		},
		methods: {
			onTel(tel) {
				if (tel == "") return;
				uni.makePhoneCall({
					phoneNumber: tel
				});
			},
			onCopy(txt) {
				uni.setClipboardData({
					data: txt,
					success: function() {
						uni.showToast({
							icon: 'success',
							title: '复制成功'
						});
					}
				});
			},
			onMapTap() {
				uni.openLocation({
					latitude: this.cardData.Lat,
					longitude: this.cardData.Lng,
					name: this.cardData.AddressName,
					address: this.cardData.AddressDetail,
					success: function() {}
				});
			},
			onTag(idx) {
				this.tagIdx = idx;
				if (idx == 0) {
					this.tagIntro = this.cardData.Intro;
				} else if (idx == 1) {
					this.tagIntro = this.orgData.Intro;
				}
			},
			async onStoreIn() {
				uni.showLoading({
					title: '加载中...'
				});
				try {
					let rsp = await addHolder(this.cardData.Id);
					this.inHolder = true;
				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}

			},
			async onExchange() {
				uni.showLoading({
					title: '加载中...'
				});
				try {
					let rsp = await addExchange(this.cardData.Id);
					this.exchanging = true;
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

	.cardwrap {
		padding-top: 20rpx;
		padding-left: 30rpx;
		padding-right: 30rpx;
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
			// .canvesImage{
			// 	position: absolute;
			// 	top: 0;
			// 	left: 0;
			// 	width:100%;
			// 	height:100%;
			// }
			image {
				image-rendering: -moz-crisp-edges;
				image-rendering: -o-crisp-edges;
				image-rendering: -webkit-optimize-contrast;
				image-rendering: crisp-edges;
				-ms-interpolation-mode: nearest-neighbor;
			}
			
		}

		.info-block {
			padding-top: 30rpx;
			margin-left: -30rpx;
			margin-right: -30rpx;

			.info-scroll {
				height: 170rpx;
				white-space: nowrap;
				width: 100%;
			}

			.info-wrap {
				padding-left: 30rpx;
			}

			.info-item {
				display: inline-block;
				margin-right: 20rpx;
				position: relative;

				.info-item-fx {
					display: flex;
					flex-direction: column;
					height: 140rpx;
					background-color: #FFFFFF;
					box-shadow: 0px 6px 10px 0px rgba(0, 0, 0, 0.1000), 0px -2px 10px 0px rgba(0, 0, 0, 0.1000);
					border-radius: 10rpx;
					box-sizing: border-box;
					padding: 27rpx 24rpx;

					.tp {
						display: flex;

						.n {
							margin-left: 20rpx;
							font-size: 30rpx;
							color: #333333;
						}
					}

					.txt {
						font-size: 22rpx;
						color: #999999;
						margin-top: 16rpx;
					}
				}
			}

		}

		.map-block {
			border-radius: 10rpx;
			overflow: hidden;
			margin-bottom: 30rpx;
			box-shadow: 0px -2px 10px 0px rgba(0, 0, 0, 0.1000), 0px 2px 10px 0px rgba(0, 0, 0, 0.1000);
		}

		.op-block {
			display: flex;
			padding-top: 20rpx;
			padding-bottom: 20rpx;

			.in-store {
				display: flex;
				flex-direction: column;
				align-items: center;
				width: 110rpx;

				.txt {
					font-size: 22rpx;
					color: #666666;
					margin-top: 10rpx;

					&.dis {
						color: #aaaaaa;
					}
				}
			}

			.midbt {
				flex: 1;
				color: #50A6FA;
				font-size: 30rpx;
				border-radius: 8px 0px 0px 8px;
				border: 1px solid #50A6FA;
				display: flex;
				justify-content: center;
				align-items: center;
				margin-left: 48rpx;
			}

			.rightbt {
				flex: 1;
				color: #FFFFFF;
				font-size: 30rpx;
				background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
				border-radius: 0px 8px 8px 0px;
				display: flex;
				align-items: center;
				justify-content: center;

				&.dis {
					background: #C1E1FF;
					color: #ffffff;
				}
			}

		}

		.vis-block {
			display: flex;
			height: 90rpx;
			align-items: center;
			justify-content: space-between;

			.ximglist {
				display: flex;

				.ximg {
					display: flex;
					width: 48rpx;
					height: 48rpx;
					border-radius: 50%;
					overflow: hidden;
					margin-left: -10rpx;
				}
			}

			.txt {
				font-size: 26rpx;
				color: #666666;
			}
		}
	}

	.introwrap {
		margin-top: 20rpx;
		background-color: #FFFFFF;
		padding-bottom: 120rpx;

		.intro-hd {
			display: flex;
			height: 90rpx;
			align-items: center;

			.sel-item {
				display: flex;
				justify-content: center;
				width: 250rpx;
				font-size: 30rpx;
				font-weight: bold;
				color: #666666;
				position: relative;
				height: 90rpx;
				line-height: 90rpx;

				&.ac {
					color: #333333 !important;
				}

				&.ac::after {
					position: absolute;
					bottom: 0;
					content: " ";
					background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
					border-radius: 4px;
					width: 60rpx;
					height: 4rpx;
				}
			}
		}

		.intro-content {
			padding: 0 30rpx;
		}
	}
</style>
