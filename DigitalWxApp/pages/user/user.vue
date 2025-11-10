<template>
	<view>
		<view class="wrap" v-if="!isload">
			<view class="top">
				<view class="left">
					<view class="photo">
						<image :src="userInfo.avatar"></image>
					</view>
					<view class="info">
						<navigator url="/pages/user/user_modify" class="name">
							<text>{{userInfo.name}}</text>
							<uni-icons custom-prefix="my-icon" type="my-icon-bianzu" color="#999999" size="15">
							</uni-icons>
						</navigator>
						<view class="company">
							{{companyInfo.OrgName}}
						</view>
					</view>
				</view>
				<view class="right">
					<view @click="open"><text>切换企业</text>
						<uni-icons type="bottom" size="13" color="#ffffff"></uni-icons>
					</view>

				</view>
			</view>
			<view class="option">
				<view class="item" v-if="isOrgMan">
					<navigator :url="`/pages/company/company_edit?id=${userInfo.OrgId}`">
						<view class="left">
							<image src="../../static/user/icon0.png"></image>
							<text>企业信息</text>
						</view>
						<view class="right">
							<uni-icons type="forward" size="23" color="#999999"></uni-icons>
						</view>
					</navigator>
				</view>
				<view class="item" v-if="isOrgMan">
					<navigator :url="'/pages/user/managers?orgId='+userInfo.OrgId">
						<view class="left">
							<image src="../../static/user/icon1.png"></image>
							<text>管理员</text>
						</view>
						<view class="right">
							<uni-icons type="forward" size="23" color="#999999"></uni-icons>
						</view>
					</navigator>
				</view>
				<view class="item" v-if="isOrgMan">
					<navigator url="/pages/depman/depman">
						<view class="left">
							<image src="../../static/user/icon2.png"></image>
							<text>成员管理</text>
						</view>
						<view class="right">
							<uni-icons type="forward" size="23" color="#999999"></uni-icons>
						</view>
					</navigator>
				</view>
				<view class="item" v-if="isOrgMan">
					<navigator :url="`/pages/user/pro_manage?orgId=`+userInfo.OrgId">
						<view class="left">
							<image src="../../static/user/icon3.png"></image>
							<text>产品管理</text>
						</view>
						<view class="right">
							<uni-icons type="forward" size="23" color="#999999"></uni-icons>
						</view>
					</navigator>
				</view>
				<view class="item" v-if="isOrgMan">
					<navigator :url="'/pages/user/case_manage?orgId='+userInfo.OrgId">
						<view class="left">
							<image src="../../static/user/icon4.png"></image>
							<text>案例管理</text>
						</view>
						<view class="right">
							<uni-icons type="forward" size="23" color="#999999"></uni-icons>
						</view>
					</navigator>
				</view>
				<view class="item">
					<navigator url="/pages/user/settings">
						<view class="left">
							<image src="../../static/user/icon5.png"></image>
							<text>设置</text>
						</view>
						<view class="right">
							<uni-icons type="forward" size="23" color="#999999"></uni-icons>
						</view>
					</navigator>
				</view>
			</view>
		</view>
		<view v-else style="padding-top: 30vh;">
			<uni-load-more iconType="circle" :showText="false" status="loading" />
		</view>
		<uni-popup ref="popup" type="bottom" backgroundColor="#fff">
			<view class="switchCom">
				<view class="title">
					切换企业
				</view>
				<view class="com-scroll">
					<view class="myCom">
						<view class="itemwrap" v-for="item in orgList" :key="item.Id" @click="switchClick(item.Id)">
							<view :class="{'item':true,'cur':item.Id==userInfo.OrgId}">
								<fr-image class="ximg" :lazy-load="true" mode="aspectFill" :src="logoImg(item.Logo)" />
								<view class="cont">
									<text class="txt">{{item.OrgName}}</text>
									<text class="xp">行业：{{item.IndustryName}}</text>
									<text class="p">规模：{{item.SizeName}}</text>
								</view>
							</view>
						</view>

					</view>
					<view class="add" @click="addNewCom">
						<uni-icons type="plusempty" size="18" color="#666666"></uni-icons>
						<text>添加新企业</text>
					</view>
				</view>

			</view>
		</uni-popup>

		<my-tab-bar :active="3"></my-tab-bar>
	</view>
</template>

<script>
	import MyTabBar from '@/components/my-tab-bar.vue'
	import {
		getOrg
	} from '@/api/org.js'
	import {
		existManager
	} from '@/api/managers.js'
	import {
		userOrgList,
		switchOrg
	} from '@/api/org.js'
	import request from '@/common/request.js'
	import {
		getNoRead
	} from '@/api/record.js'
	export default {
		components: {
			MyTabBar
		},
		data() {
			return {
				options: [{
					text: '取消',
					style: {
						backgroundColor: '#007aff'
					}
				}, {
					text: '确认',
					style: {
						backgroundColor: '#dd524d'
					}
				}],
				userInfo: {},
				companyInfo: {
					OrgName: "请点右边切换企业"
				},
				orgList: [],
				isload: true,
				isOrgMan: false
			}
		},
		onLoad() {
			this.reloadpage();
		},
		async onShow() {
			let usrInfo = await this.$store.dispatch("userInfo");
			
			if(usrInfo.name=='微信用户'||usrInfo.name==''){
				uni.showModal({
					title: '提示',
					content: '请先完善您的用户信息',
					success: function (res) {
						if (res.confirm) {
							// console.log('用户点击确定');
							uni.navigateTo({
								url: '/pages/user/change_user_info'
							})
						} else if (res.cancel) {
							// console.log('用户点击取消');
							// uni.navigateBack()
						}
					}
				});
			}
			if(this.$store.state.shouleShow){
				try {
					this.$store.commit('SET_USER_INFO', null);
					this.userInfo = await this.$store.dispatch("userInfo");
					// console.log("用户信息", this.userInfo);
					if (this.userInfo.OrgId > 0) {
						let rsp = await getOrg(this.userInfo.OrgId);
						this.companyInfo = rsp.data;
						await this.initManAuth(this.userInfo.OrgId);
					}
					this.getOrgList() //获取企业列表
					this.$forceUpdate();
				} catch (err) {
					console.info('异常：', err);
				}
				this.$store.commit('CARRY_USER_SHOW', false);
			}
			
		},
		methods: {
			async reloadpage(name) {
				if (name == 'useravator') { //修改头像时
					// console.log("判断一下执行了吗？")
					this.$store.commit('SET_USER_INFO', null);
					this.userInfo = await this.$store.dispatch("userInfo");
				}
				if (name == 'changeName') {
					this.$store.commit('SET_USER_INFO', null);
					this.userInfo = await this.$store.dispatch("userInfo");
					// console.log(this.userInfo);
				}
				if (name == 'orgJoin') {
					this.getOrgList() //获取企业列表
				}
				if (name == 'company') {
					this.$store.commit('SET_USER_INFO', null);
					this.userInfo = await this.$store.dispatch("userInfo");
					// console.log("操作企业后", this.userInfo);
				}
				if (name == 'companyEdit') {
					this.$store.commit('SET_USER_INFO', null);
					this.userInfo = await this.$store.dispatch("userInfo");
					this.getOrgList() //获取企业列表
					// console.log("解散退出企业后", this.userInfo);
					// if(this.orgList.length>0){
					// 	this.companyInfo=this.orgList[0]
					// }
					// return
				}
				try {
					this.$store.commit('SET_USER_INFO', null);
					this.userInfo = await this.$store.dispatch("userInfo");
					// console.log("用户信息", this.userInfo);
					if (this.userInfo.OrgId > 0) {
						let rsp = await getOrg(this.userInfo.OrgId);
						this.companyInfo = rsp.data;
						await this.initManAuth(this.userInfo.OrgId);
					}
					this.getOrgList() //获取企业列表
					this.$forceUpdate();
				} catch (err) {
					console.info('异常：', err);
				}
				this.isload = false;
			},
			async getOrgList() {
				let rsp = await userOrgList();
				for (let xi = 0; xi < rsp.data.length; xi++) {
					let x = rsp.data[xi];
					x.IndustryName = "";
					x.SizeName = "";
					if (x.Logo == "") {
						x.Logo = "/static/user/company.png";
					}
					if (x.Size) {
						let rt = await this.$store.dispatch("dictName", {
							name: "org_size",
							value: x.Size
						});
						x.SizeName = rt;
					}
					if (x.Industry) {
						let rt = await this.$store.dispatch("industryName", x.Industry);
						x.IndustryName = rt;
					}
				
				}
				this.orgList = rsp.data;
				// console.log("企业列表",rsp);
			},
			open() {
				this.$refs.popup.open('bottom')
			},
			addNewCom() {
				this.$refs.popup.close();
				uni.showActionSheet({
					itemList: ['全新创建企业', '加入其他企业'],
					success: function(res) {
						if (res.tapIndex == 0) {
							uni.navigateTo({
								url: "/pages/company/company_add"
							});
						} else if (res.tapIndex == 1) {
							uni.navigateTo({
								url: "/pages/user/user_join"
							});
						}
					}
				});
			},
			logoImg(img) {
				if (img == "") {
					return "/static/user/company.png";
				} else {
					return img;
				}
			},
			//判断是否有管理员权限
			async initManAuth(orgId) {
				try {
					let rsp = await existManager(orgId);
					this.isOrgMan = rsp.data;
				} catch (err) {
					console.info('异常：', err);
				}
			},
			async switchClick(id) {
				this.$refs.popup.close();
				uni.showLoading({
					title: '加载中...'
				});
				try {
					if (this.userInfo.OrgId != id) {
						await switchOrg(id);
						this.userInfo.OrgId = id;
						let rsp = await getOrg(this.userInfo.OrgId);
						this.companyInfo = rsp.data;
						this.$store.commit('SET_USER_INFO', null);
						this.userInfo = await this.$store.dispatch("userInfo");
						await this.initManAuth(id);
						
					}
				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
					this.$refs.popup.close();
				}
			},
			editCard(id) {
				this.$refs.popup.close();
				uni.navigateTo({
					url: '/pages/card_center/card_edit?id=' + id
				});

			}
		}
	}
</script>

<style lang="scss">
	.wrap {
		.top {
			width: 100%;
			min-height: 198rpx;
			background-color: #FFFFFF;
			display: flex;
			justify-content: space-between;
			align-items: center;
			box-sizing: border-box;
			padding: 44rpx 30rpx;

			.left {
				width: 565rpx;
				display: flex;

				.photo image {
					width: 110rpx;
					height: 110rpx;
					border-radius: 10rpx;
					margin-right: 32rpx;
				}

				.info {
					flex: 1;
				}

				.info .name {
					width: 100%;
					color: #999999;
					font-size: 30rpx;
					margin-bottom: 8rpx;
					margin-top: 13rpx;
				}

				.info .name text {
					margin-right: 10rpx;
				}

				.info .name image {
					width: 28rpx;
					height: 28rpx;
				}

				.info .name icon {
					font-size: 28rpx;
				}

				.info .company {
					width: 100%;
					color: #999999;
					font-size: 22rpx;
				}

			}

			.right view {
				display: block;
				background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
				color: #FFFFFF;
				height: 45rpx;
				display: flex;
				justify-content: center;
				align-items: center;
				font-size: 22rpx;
				border-radius: 22rpx;
				width: 140rpx;
				padding-left: 8rpx;
				text-align: center;

				text {
					margin-right: 5rpx;
				}

				icon {
					display: block;
				}
			}


		}

		.option {
			width: 100%;
			margin-top: 30rpx;

			.item {
				width: 100%;
				height: 120rpx;
				font-size: 30rpx;
				margin-bottom: 1rpx;
				background-color: #FFFFFF;

				navigator {
					background-color: #FFFFFF;
					width: 100%;
					height: 120rpx;
					display: flex;
					justify-content: space-between;
					color: #666666;
					box-sizing: border-box;
					padding: 0 40rpx;

					.left {
						display: flex;
						align-items: center;

						image {
							width: 44rpx;
							height: 44rpx;
							display: block;
							margin-right: 40rpx;
						}
					}

					.right {
						display: flex;
						align-items: center;

					}
				}

			}
		}
	}

	.switchCom {
		background-color: #FFFFFF;
		min-height: 60%;
		width: 100%;
		box-sizing: border-box;
		padding: 0 30rpx;
		bottom: 0;
		font-family: pfcu;

		.title {
			width: 100%;
			height: 102rpx;
			line-height: 102rpx;
			text-align: left;
			font-size: 30rpx;
		}

		.com-scroll {
			overflow-y: scroll;
			max-height: 640rpx;
			white-space: nowrap;
			width: 100%;
		}

		.myCom {
			width: 100%;

			.item {
				width: 100%;
				height: 200rpx;
				display: flex;
				border: 1px solid #D7D7D7;
				border-radius: 10rpx;
				align-items: center;

				box-sizing: border-box;
				z-index: 9999;
				overflow: hidden;
				background-color: #FFFFFF;

				.ximg {
					margin-left: 14rpx;
					width: 163rpx;
					height: 174rpx;
					overflow: hidden;
					border-radius: 8rpx;
				}

				.cont {
					margin-left: 50rpx;
					display: flex;
					flex-direction: column;

					.txt {
						margin-top: 10rpx;
						font-size: 30rpx;
						font-weight: bold;
						color: #333333;
					}

					.xp {
						margin-top: 20rpx;
						margin-bottom: 10rpx;
					}

					.p,
					.xp {
						font-size: 22rpx;
						color: #999999;
					}
				}


			}

			.cur {
				border: 1px solid #4EA8FA;
				background-color: #E9F6FE;
				z-index: 999;
			}

		}

		.add {
			display: flex;
			height: 200rpx;
			width: 100%;
			align-items: center;
			justify-content: center;
			color: #666666;
			font-size: 30rpx;
			box-sizing: border-box;
			border: 1px solid #D7D7D7;
			border-radius: 10rpx;
			margin-bottom: 30rpx;

			text {
				margin-left: 10rpx;
			}
		}
		::-webkit-scrollbar {
			width: 4px;
			height: 2px;
			background-color: white;
		}
		
		::-webkit-scrollbar-thumb {
			border-radius: 16px;
			background-color: #e8e8e8;
		}
	}


	.itemwrap {
		background-color: #CBCCCD;
		height: 200rpx;
		margin-bottom: 30rpx;
		border-radius: 11rpx;
	}
</style>
