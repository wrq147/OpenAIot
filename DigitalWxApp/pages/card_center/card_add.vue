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
		<view class="choose">
			<view class="title">
				选择样式
			</view>
			<scroll-view :scroll-x="true" class="content">
				<block>
					<view class="contwrap">
						<view v-for="(bg,idx) in bglist" class="item" @click="bgSelected(idx)">
							<view class="ac" v-if="(idx+1)==cardData.TemplateBk"></view>
							<fr-image class="ximg" :lazy-load="true" mode="widthFix" :src="bg"
								loading-ing-img="oblique-light" />
						</view>
					</view>
				</block>
			</scroll-view>
			<view class="border-bottom" style="padding-top: 40rpx;"></view>
		</view>

		<view class="xrow border-bottom" @click="changeCompany">
			<view class="xleft">企业</view>
			<view class="xright betwen">
				<view class="tip">{{cardData.OrgId>0?cardData.OrgName:'请选择名片所属企业'}}</view>
				<view class="iconwrap">
					<uni-icons type="forward" size="20" color="#666666"></uni-icons>
				</view>
			</view>
		</view>
		<view class="xrow border-bottom" v-if="cardData.OrgId>0">
			<view class="xleft">职位</view>
			<view class="xright ipt">
				<uni-easyinput trim="both" maxlength="50" v-model="cardData.PostName" @input="changeDraw"
					:inputBorder="false" placeholder="请输入所属职位">
				</uni-easyinput>
			</view>
		</view>

		<view class="xrow border-bottom">
			<view class="xleft">姓名</view>
			<view class="xright ipt">
				<!-- <uni-easyinput trim="both" maxlength="20" @input="changeDraw" v-model="cardData.RealName"
					:inputBorder="false" placeholder="请输入姓名">
				</uni-easyinput> -->
				<uni-easyinput class="weui-input" type="nickname" trim="both" maxlength="20" @input="changeDraw"
					v-model="cardData.RealName" :inputBorder="false" placeholder="请输入姓名">
				</uni-easyinput>
			</view>
		</view>

		<view class="xrow border-bottom">
			<view class="xleft">手机</view>
			<view class="xright ipt">
				<uni-easyinput trim="both" type="number" maxlength="11" @input="changeDraw" v-model="cardData.Mobile"
					:inputBorder="false" placeholder="请输入手机号码">
				</uni-easyinput>
			</view>
		</view>


		<view class="border-btn" style="border-top:none;">
			<button class="btn" @click="onSaveClick">
				创建
			</button>
		</view>
	</view>
</template>

<script>
	import {
		getDrawObj,
		drawCard
	} from '@/common/card.js'

	import {
		addCard
	} from '@/api/userCard.js'
	import {
		getOrg
	} from '@/api/org.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				cardId: 0,
				cardData: {
					RealName: "",
					Mobile: "",
					OrgName: "",
					PostName: "",
					Avatar: "",
					TemplateId: 1,
					TemplateBk: 1,
					OrgId: 0
				},
				bglist: [],
				orgData:{},//企业信息
			};
		},
		onLoad(options) {
			this.cardId = parseInt(options.cardId || "0");
			let bgls = getDrawObj().bglist;
			let li = getDrawObj().templatelist[this.cardData.TemplateId - 1].bglist
			let list = []
			bgls.map((item, index) => {
				li.map((it, idx) => {
					if (index == it) { //只展示与模板适配的背景
						list.push(item)
					} else {
						return
					}
				})
			})
			this.bglist = list
		},
		async onReady() {
			try {
				let usrInfo = await this.$store.dispatch("userInfo");
				this.cardData.Avatar = usrInfo.avatar;
				await drawCard('canvas', this.cardData);
			} catch (err) {
				console.info('异常：', err);
			}
		},
		onShow() {
			drawCard('canvas', this.cardData);
		},
		methods: {
			
			changeCompany() {
				this.$store.state.submitMod.formArrary.push(this.cardData);
				uni.navigateTo({
					url: '/pages/company/company_sel'
				});
			},
			bgSelected(idx) {
				this.cardData.TemplateBk = idx + 1;
				drawCard('canvas', this.cardData);
			},
			async changeDraw(e) {
				await drawCard('canvas', this.cardData);
			},
			async onSaveClick() {
				if(this.cardData.OrgId<=0){
					uni.showToast({
						title: "请选择企业",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if(this.cardData.PostName==''||this.cardData.PostName==null){
					uni.showToast({
						title: "请填写职位",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if(this.cardData.RealName==''||this.cardData.RealName==null){
					uni.showToast({
						title: "请填写姓名",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if(this.cardData.Mobile==''||this.cardData.Mobile==null){
					uni.showToast({
						title: "请填写联系方式",
						icon: "none",
						duration: 2000
					});
					return;
				}else if (!(/^1[3456789]\d{9}$/.test(this.cardData.Mobile))) {
					uni.showToast({
						title: '手机号码格式错误',
						icon: "none"
					});
					return;
				}
				
				uni.showLoading({
					title: '加载中...'
				});
				try {
					if(this.cardData.OrgId>0 && this.cardData.PostName && this.cardData.Mobile && this.cardData.RealName){
						let result = await getOrg(this.cardData.OrgId);
						// console.log("企业信息",result);
						this.orgData = result.data;
						
						let rsp = await addCard({
							RealName: this.cardData.RealName,
							Mobile: this.cardData.Mobile,
							WxNumber: '',
							Website: '',
							Email: '',
							Avatar: this.cardData.Avatar,
							TemplateId: 1,
							TemplateBk: this.cardData.TemplateBk,
							OrgId: this.cardData.OrgId,
							DeptName: this.cardData.DeptName,
							PostName: this.cardData.PostName,
							ShareTitle: '',
							Intro: '',
							AddressCode:this.orgData.AddressCode,
							AddressDetail:this.orgData.AddressDetail,
							AddressName:this.orgData.AddressName,
							Lat:this.orgData.Lat,
							Lng:this.orgData.Lng
						});
						if(this.cardId){
							uni.redirectTo({
								url:'/pages/card_center/card?id=' + this.cardId
							});
						}else{
							reloadPrePage(1, "card");
							uni.redirectTo({
								url: '/pages/card_center/card_success?id=' + rsp.data
							});
						}
						
					}
					// else{
					// 	let rsp = await addCard({
					// 		RealName: this.cardData.RealName,
					// 		Mobile: this.cardData.Mobile,
					// 		WxNumber: '',
					// 		Website: '',
					// 		Email: '',
					// 		Avatar: this.cardData.Avatar,
					// 		TemplateId: 1,
					// 		TemplateBk: this.cardData.TemplateBk,
					// 		OrgId: this.cardData.OrgId,
					// 		DeptName: this.cardData.DeptName,
					// 		PostName: this.cardData.PostName,
					// 		ShareTitle: '',
					// 		Intro: ''
					// 	});
					// }
					
					
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
	page {
		background-color: #FFFFFF;
	}

	.wrap {
		width: 100%;
		box-sizing: border-box;
		padding-left: 30rpx;
		padding-right: 30rpx;
		background-color: #FFFFFF;
		margin-bottom: 20rpx;

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

	.choose {
		margin-top: 61rpx;
		padding-left: 30rpx;
		padding-right: 30rpx;

		.title {
			box-sizing: border-box;
			width: 100%;
			font-size: 30rpx;
			color: #999999;
			margin-bottom: 30rpx;
		}

		.content {

			height: 87rpx;
			white-space: nowrap;
			width: 100%;

			.contwrap {


				.item {
					display: inline-block;
					margin-right: 22rpx;
					position: relative;

					.ac {
						position: absolute;
						top: 0;
						left: 0;
						width: 147rpx;
						height: 87rpx;
						background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
						background-image: url('/static/icon_choose_n.png');
						z-index: 9;
						background-size: 50rpx 50rpx;
						background-repeat: no-repeat;
						background-position: center;
					}

					.ximg {
						display: flex;
						width: 147rpx;
						height: 87rpx;
						overflow: hidden;
					}
				}
			}


		}

	}

	.xrow {
		display: flex;
		justify-content: space-between;
		align-items: center;
		padding: 0 30rpx;
		height: 90rpx;
		font-size: 30rpx;

		.xleft {
			width: 200rpx;
		}

		.xright {
			flex: 1;

			&.ipt {
				margin-left: -10px;
			}

			&.betwen {
				display: flex;
				justify-content: space-between;
			}
		}
	}
</style>
