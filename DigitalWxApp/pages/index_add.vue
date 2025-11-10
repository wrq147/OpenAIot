<template>
	<view>
		<view class="xrow border-bottom">
			<view class="xleft">填写信息创建</view>
		</view>
		<view class="xrow border-bottom">
			<view class="xleft"><text style="color: red;">*</text>所在企业</view>
			<view class="xright betwen" @click="showInput(1)">
				<uni-easyinput v-if="isInput==1" :focus="isInput==1" trim="both" maxlength="50" :clearable="false"
					v-model="cardData.companyName" @blur="hideInput" :inputBorder="false" placeholder="请输入所属企业">
				</uni-easyinput>
				<text v-else
					:class="[cardData.companyName?'':'huise']">{{cardData.companyName?cardData.companyName:'请输入所属企业'}}</text>
			</view>
		</view>
		<view class="xrow border-bottom">
			<view class="xleft"><text style="color: red;">*</text>姓名</view>
			<view class="xright betwen" @click="showInput(2)">
				<uni-easyinput v-if="isInput==2" :focus="isInput==2" trim="both" maxlength="20" :clearable="false"
					v-model="cardData.realName" @blur="hideInput" :inputBorder="false" placeholder="请输入姓名">
				</uni-easyinput>
				<text v-else
					:class="[cardData.realName?'':'huise']">{{cardData.realName?cardData.realName:'请输入姓名'}}</text>
			</view>
		</view>
		<view class="xrow border-bottom">
			<view class="xleft"><text style="color: red;">*</text>职位</view>
			<view class="xright betwen" @click="showInput(3)">
				<uni-easyinput v-if="isInput==3" :focus="isInput==3" trim="both" maxlength="50" :clearable="false"
					v-model="cardData.positionNames" @blur="hideInput" :inputBorder="false" placeholder="请输入所属职位">
				</uni-easyinput>
				<text v-else
					:class="[cardData.positionNames?'':'huise']">{{cardData.positionNames?cardData.positionNames:'请输入职位'}}</text>
			</view>
		</view>
		<view class="xrow border-bottom">
			<view class="xleft"><text style="color: red;">*</text>所在地址</view>
			<view class="xright betwen" @click="chooseLocation">
				<!-- <view class="right"> -->
				<text
					:class="[cardData.addressName?'':'huise']">{{cardData.addressName?cardData.addressName:'请选择企业地址'}}</text>
				<uni-icons v-if="cardData.addressName==''" type="forward" size="20" color="#666666"></uni-icons>
				<!-- </view> -->
			</view>
		</view>
		<view class="xrow border-bottom">
			<view class="xleft"><text style="color: red;">*</text>地址详情</view>
			<view class="xright betwen">
				<!-- <view class="right"> -->
				<!-- <text
					:class="[cardData.companyAddress?'':'huise']">{{cardData.companyAddress?cardData.companyAddress:'请选择企业地址'}}</text> -->
				<textarea placeholder-style="color:#999999;" maxlength="225" style="max-height: 119rpx;text-align: right;" auto-height
					v-model="cardData.companyAddress" placeholder="请输入地址详情" />

				<!-- <uni-icons v-if="cardData.companyAddress==''" type="forward" size="20" color="#666666"></uni-icons> -->
				<!-- </view> -->
			</view>
		</view>
		<view class="button_row" @click="onSaveClick">
			<image class="image" src="/static/card/button_bg.png" mode=""></image>
			<view class="open_button">
				{{cid?'加入企业':'创建新企业'}}
			</view>
		</view>

	</view>
</template>

<script>
	import {
		addCard
	} from '@/api/userCard.js'
	import {
		getOrg,
		createOrg,
		editPost,
		directJoinOrg
	} from '@/api/org.js'
	import {
		editUser
	} from "@/api/user.js"
	import {
		reloadPrePage
	} from '@/common/util.js'
	import {
		reverseGeocoder
	} from '@/api/address.js'
	export default {
		data() {
			return {
				isInput: false,
				cardId: 0,
				cardData: {
					addressCode: 0,
					addressName: "",
					companyAddress: "",
					companyName: "",
					lat: -1,
					lng: -1,
					positionNames: "",
					realName: ""
				},
				bglist: [],
				orgData: {}, //企业信息
				infoMess: "",
				cid: 0
			};
		},
		onLoad(options) {
			// this.cardId = parseInt(options.id || "0");
			// console.log("华达智云传过来的值",options);
			if (options.info) {
				this.infoMess = decodeURIComponent(options.info || {})
				// console.log("华达智云传过来的值翻译后",this.infoMess);
				this.cardData = JSON.parse(this.infoMess || '')
				// console.log("json转换后",this.cardData);
				if (this.cardData.companyAddress && (this.cardData.companyAddress.indexOf('省') || this.cardData
						.companyAddress.indexOf('市'))) {
					this.cardData.addressName = this.cardData.companyAddress.substr(0, 6)
					this.cardData.companyAddress = this.cardData.companyAddress.substr(6)
				}
			}
			if (options.cid) {
				this.cid = options.cid
				this.getOrgInfo(options.cid)
			}
		},
		async onReady() {
			try {

			} catch (err) {
				console.info('异常：', err);
			}
		},
		methods: {
			async getOrgInfo(id) {
				let rsp = await getOrg(id)
				// console.log("根据id获取企业信息",rsp);
				this.cardData.companyName = rsp.data.OrgName
				this.cardData.companyAddress = rsp.data.AddressDetail
				if (this.cardData.companyAddress && (this.cardData.companyAddress.indexOf('省') || this.cardData
						.companyAddress.indexOf('市'))) {
					this.cardData.addressName = this.cardData.companyAddress.substr(0, 6)
					this.cardData.companyAddress = this.cardData.companyAddress.substr(6)
				}
			},
			chooseLocation() {
				uni.chooseLocation({
					success: res => {
						this.showDetailPlace = true;
						this.cardData.lng = res.longitude;
						this.cardData.lat = res.latitude;
						reverseGeocoder(this.cardData.lng, this.cardData.lat).then(rsp => {
							this.cardData.addressCode = rsp.ad_info.adcode;
							this.cardData.showDetailPlace = true;

							let addressArea = ''
							if (rsp.address_component.province == rsp.address_component.city) {
								addressArea = rsp.address_component.province + rsp.address_component
									.district
							} else {
								addressArea = rsp.address_component.province + rsp.address_component
									.city + rsp.address_component.district
							}
							this.cardData.addressName = addressArea

							this.cardData.companyAddress = (rsp.address_reference.town.title ? rsp
									.address_reference.town.title : '') + rsp.address.substr(this
									.cardData.addressName.length) + rsp.address_reference.landmark_l2
								.title

							if (res.address) {
								this.cardData.companyAddress = res.address.substr(this.cardData
										.addressName.length) + res
									.name
							}
							// console.log("选择地址以后",this.cardData);

						});

						// this.addressName = res.name;
						// this.addressDetail = res.address;
						// this.lng = res.longitude;
						// this.lat = res.latitude;
						// reverseGeocoder(this.lng, this.lat).then(rsp => {
						// 	this.addressCode = rsp.ad_info.adcode;

						// 	if (this.addressName == "") {
						// 		this.addressName = rsp.formatted_addresses.recommend;
						// 		this.addressDetail = rsp.address;
						// 	}

						// });
					}
				});
			},
			//设置显示输入框隐藏text
			showInput(num) {
				this.isInput = num;
			},
			//设置因此输入框显示text
			hideInput() {
				this.isInput = false;
			},
			async onSaveClick() {
				if (this.cardData.companyName == '' || this.cardData.companyName == null) {
					uni.showToast({
						title: "请选择企业",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.cardData.positionNames == '' || this.cardData.positionNames == null) {
					uni.showToast({
						title: "请填写职位",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.cardData.realName == '' || this.cardData.realName == null) {
					uni.showToast({
						title: "请填写姓名",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.cid) {} else if (this.cardData.companyAddress == '' || this.cardData.companyAddress == null) {
					uni.showToast({
						title: "请选择公司地址",
						icon: "none",
						duration: 2000
					});
					return;
				}

				uni.showLoading({
					title: '加载中...'
				});
				try {
					if ((this.cid || this.cardData.companyAddress) && this.cardData.positionNames && this.cardData
						.companyName && this.cardData
						.realName) {
						// console.log("保存时打印cardData",this.cardData);

						let changeuser = {}
						let joinorg = {}
						let changeuser2 = {}
						if (this.cid) {
							//等待加入企业接口
							joinorg = await directJoinOrg({
								orgId: this.cid,
								postName: this.cardData.positionNames,
								realName: this.cardData.realName,
								depId: ""
							})
							this.$store.commit('SET_USER_INFO', null);
						} else {
							changeuser = await editUser({
								realName: this.cardData.realName
							})
							joinorg = await createOrg({
								orgName: this.cardData.companyName,
								lng: this.cardData.lng || -1,
								lat: this.cardData.lat || -1,
								addressCode: this.cardData.addressCode || "",
								addressName: this.cardData.addressName || "",
								addressDetail: this.cardData.companyAddress || "",
								Logo: "",
								Industry: 0,
								Size: 0,
								Intro: "",
								bindId: this.cardData.cid
							})
							console.log("传递的值", {
								orgName: this.cardData.companyName,
								lng: this.cardData.lng || -1,
								lat: this.cardData.lat || -1,
								addressCode: this.cardData.addressCode || "",
								addressName: this.cardData.addressName || "",
								addressDetail: this.cardData.companyAddress || "",
								Logo: "",
								Industry: 0,
								Size: 0,
								Intro: "",
								bindId: this.cardData.cid
							});
							changeuser2 = await editPost(joinorg.data, this.cardData.positionNames)
							// console.log("joinorg",joinorg);
							this.$store.commit('SET_USER_INFO', null);
							// console.log("创建",joinorg);
						}

						if (joinorg.code == 0) {
							uni.showToast({
								title: "创建名片成功",
								icon: "success",
								duration: 1000
							});
							setTimeout(() => {
								this.$store.commit('SET_USER_INFO', null);
								uni.switchTab({
									url: '/pages/card_center/card_center'
								});
							})
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

	.xrow {
		display: flex;
		justify-content: space-between;
		align-items: center;
		padding: 0 30rpx;
		height: 120rpx;
		font-size: 30rpx;


		.xleft {
			width: 200rpx;
		}

		.xright {
			flex: 1;
			align-items: center;
			height: 119rpx;
			font-size: 30rpx;

			&.ipt {
				margin-left: -10px;
			}

			&.betwen {
				display: flex;
				justify-content: flex-end;
				align-items: center;
			}

			.huise {
				color: #999999;
			}

			.right {
				display: flex;
				justify-content: flex-end;
				align-items: center;
				height: 119rpx;
			}

			input {
				display: block;
				outline: none;
				flex: 1;
				height: 119rpx;
				margin-right: 8rpx;
				border: none;
				// font-size: 30rpx;
			}

			input::-ms-input-placeholder {
				color: #999999;
				border: none;
				// font-size: 30rpx;
			}
		}


	}

	.button_row {

		box-shadow: 0 10rpx 15rpx rgba(44, 194, 251, 0.5);
		display: flex;
		align-items: center;
		justify-content: center;
		position: relative;
		width: 690rpx;
		margin: 0 auto;
		margin-top: 108rpx;

		.image {
			width: 690rpx;
			height: 90rpx;
		}

		.open_button {
			position: absolute;
			top: 0;
			left: 0;
			width: 690rpx;
			text-align: center;
			height: 90rpx;
			line-height: 90rpx;
			border-radius: 12rpx;
			font-size: 30rpx;
			color: #ffffff;

		}
	}
</style>
