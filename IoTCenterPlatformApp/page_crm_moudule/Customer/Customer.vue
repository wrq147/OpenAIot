<template>
	<view id="Customer">
		<top :isRightSlot="true" leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="客户"
			rightIcon="icon-a-tianjiabantouming" class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handAdd()">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>
		<search-compt ref="selectCompt" @openSelect="openSelect" @searching="searching" pal="请输入客户名称"
			inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="queryData"
			@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
		<view class="poolsList" v-for="(item,index) in customerData" :key="index" @click="handClickDetails(item)">
			<view class="poolsList-item">
				<view class="poolsList-top">
					<view class="poolsList-top-title">{{item.CustomerName}}</view>
					<view class="time">负责人: {{item.LeaderName}}</view>

				</view>

				<view class="poolsList-item-button">
					<view class="button" v-if="item.BindOrgId==0" @click.stop="handInvite(item.Id)">
						<text class="iconfont icon-yaoqing"></text>
						<text>邀请</text>
					</view>
					<view class="button" v-if="item.BindOrgId>0" @click.stop="handCancelInvite(item)">
						<text class="iconfont icon-quxiaoyaoqing"></text>
						<text>取消邀请</text>
					</view>
					<view class="button" @click.stop="handEdit(item.Id)">
						<text class="iconfont icon-bianji"></text>
						<text>编辑</text>
					</view>
					<view class="button btnRight" @click.stop="handDelete(item)">
						<text class="iconfont icon-shanchu"></text>
						<text>删除</text>
					</view>
					<!-- <view class="button btnRight">
						<text class="iconfont icon-gengduo"></text>
						<text>More</text>
					</view> -->
				</view>
				<view class="Agent" v-if="item.CustomerType==0">
					代理<!--Agent代理0 直销为1-->
				</view>
				<view class="DirectSales" v-if="item.CustomerType==1">
					直销
				</view>
			</view>
		</view>
		<uni-popup ref="clearAlarmPopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">取消邀请</view>
					<view class="conten_icon_con">
						<view style="color:#C1C1C1;margin-bottom: 20rpx;">
							是否要取消代理权限？
						</view>
						<view class="conten_icon t-icon-kaibeifen" v-if="clearForm.isFilterDevice"
							@click.stop="setDeviceFilter(false)"></view>
						<view class="conten_icon t-icon-guan" v-if="!clearForm.isFilterDevice"
							@click.stop="setDeviceFilter(true)"></view>
					</view>
					<button class="submit_button" @click.stop="getWarnMsg()">
						确认
					</button>
					<view class="close_icon" @click="noticeColse">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
							iconsColor="#333333"></custom-icons>
					</view>
				</view>
			</view>
		</uni-popup>

		<!--添加弹窗-->
		<view class="AddPopUps" v-if="alertHide">
			<view class="AddPopUps-item">
				<view class="AddPopUps-title">
					<view class="title">
						创建员工
					</view>
					<view class="iconfont icon-guanbidanchuang" @click="handClose">

					</view>
				</view>
				<view v-if="tipsValue==''">
					<view class="link">
						代理商的工厂
					</view>
					<view class="lianUrlInput">
						<uni-data-select v-model="inviteForm.factoryId" :localdata="AgentFactory" type="line"
							placeholder="请选择客户类型" class="addPool-selected"></uni-data-select>
					</view>
					<view class="link">
						选择代理所属的区域
					</view>
					<view class="lianUrlInput">
						<!-- <uni-data-picker class="addPool-selected" v-model="inviteForm.regions"
							style="color:rgba(255, 255, 255, .2);" placeholder="Please select Industry type"
							popup-title="Please select" :isDark="true" :clear-icon="false"
							placeholder-class="IndustryType" :localdata="comSizeItems">
						</uni-data-picker> -->
						<view class="view_input" @click="choiceWZ">
							<view class="pal_col" v-if="!wzValue||wzValue.length==0">
								请选择地区
							</view>
							<view class="view_li_con personnel_li_con" v-if="wzValue&&wzValue.length>0">
								<view class="view_li_cot personnel_li_cot">
									<view class="view_li" v-for="(item,inx) in wzValue">
										<view class="view_text">
											{{item}}
										</view>
									</view>
								</view>
							</view>
							<view class="view_mask"></view>
							<view class="form_sel_icon">
								<custom-icons iconsName="icon-xialajiantou" iconsSize="14rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							</view>
						</view>
					</view>
					<view class="GenerateLink" @click="createLink()">生成链接</view>
				</view>
				<view v-else class="copyInvite">
					<view class="copyInvite-title">
						通过链接邀请
					</view>
					<view class="copyInvite-lianUrl">
						<view class="copyInvite-lianUrl-url">
							{{tipsValue}}
						</view>
					</view>
					<view class="copuUrl" @click="copy(tipsValueCopy)">
						复制链接
					</view>
					<view class="copuUrl-period">
						链接有效期：
					</view>
					<view class="">
						邀请链接将在 <text style="color:rgba(35, 113, 255, 1);"> 7 天</text> 后过期
					</view>
				</view>

			</view>
		</view>
		<view class="move" v-if="alertHide"></view>
		<uni-load-more iconType="circle" :status="status" v-if="status" />
		<uni-popup ref="addressPopup" type="bottom" :z-index="1000">
			<hy-address-multiple ref="addressSelect" :address="wzItems" @confirm="addressConfirm"></hy-address-multiple>
		</uni-popup>
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<msg-prompt ref="promptMsgInvite" @confirm="confirmYao"></msg-prompt>
	</view>
</template>

<script>
	import {
		CustomerList, //客户列表（私海）
		AuthorizationData, //授权列表
		customerInvite, //生成链接
		customerDelete, //删除
		cancelInvitation //取消邀请
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'

	// #ifdef H5
	import serverUrl1 from '@/common/constVar.js'
	// #endif
	// #ifndef H5
	import serverUrl from '@/common/constVar.js'
	// #endif

	let ser = ''
	// #ifdef H5
	ser = serverUrl1.getServerUrl()
	// #endif
	// #ifndef H5
	ser = serverUrl.getServerUrl()
	// #endif
	export default {
		data() {
			return {
				addressPopup: false,
				wzCode: [],
				wzValue: [],
				wzItems: [],
				wzValPar: [],
				wzValPar2: [],
				clearForm: {
					isFilterFun: false,
					isFilterDevice: false,
					mark: ""
				},
				timeQuery: {
					name: '日期',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				invateHide: false,
				alertHide: false,
				key: '',
				status: 'loading',
				customerData: [],
				queryData: {
					// Belong:1,
					pageNum: 1,
					pageSize: 30,
				},
				CustomerAll: '',
				AgentFactory: [],
				comSizeItems: [],
				inviteForm: {
					regions: '',
					factoryId: '',
					bindCustomerId: ''
				},
				tipsValue: '',
				EditId: '',
				tipsValueCopy: '',
				isresetAddress: false
			}
		},
		async onLoad(option) {
			this.list(); //客户列表（私海）
			if (option.CustomerAll) {
				this.CustomerAll = true
			}
			setTimeout(async () => {
				let arr = await this.$store.dispatch("data/areaTree")
				this.wzItems = JSON.parse((JSON.stringify(arr)))
			}, 100)
		},
		methods: {
			addressConfirm(resArr, resNameArr) {
				this.wzValue = JSON.parse(JSON.stringify(resNameArr))
				this.wzCode = JSON.parse(JSON.stringify(resArr))
				this.inviteForm.regions = this.wzCode.join(',')
				this.$refs.addressPopup.close()
			},
			choiceWZ() {
				//选择位置
				//打开选择器
				this.$refs.addressPopup.open()
				if (this.isresetAddress) {
					this.$nextTick(async () => {
						let arr = await this.$store.dispatch("data/areaTree")
						// console.log(arr,'arr');
						this.wzItems = JSON.parse(JSON.stringify(arr))
						this.$refs.addressSelect.newAddress = JSON.parse(JSON.stringify(arr))
						this.isresetAddress = false
					})
				}
			},
			getWarnMsg() {
				cancelInvitation({
					id: this.EditId,
					cancelAgent: this.clearForm.isFilterDevice
				}).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: '取消成功!',
							icon: 'none'
						});
						this.$refs.clearAlarmPopup.close()
						this.list();
					}
				})
			},
			noticeColse() {
				this.$refs.clearAlarmPopup.close()
			},
			setDeviceFilter(val) {
				//是否过滤设备
				this.clearForm.isFilterDevice = val
			},
			openSelect() {
				this.$refs.selectCompt.openSelect()
			},
			confirmUnbind() {
				//删除
				customerDelete({
					id: this.EditId
				}).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: '删除成功！',
							icon: 'none'
						})
						this.list();
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handDelete(item) {
				//删除
				this.$refs.promptMsg.noticeOpen(
					"你确定要删除名称为 " + item.CustomerName + "的客户吗?"
				)
				this.EditId = item.Id
			},
			copy(context) { //context被复制的内容
				const that = this
				//复制链接
				// console.log("复制链接", context);
				uni.setClipboardData({
					data: context,
					success: () => {
						// uni.showToast({
						// 	title: '已自动复制网址，请在手机浏览器里粘贴该网址',
						// 	duration: 2000,
						// 	icon: 'none'
						// });
						// uni.hideToast()
					},
					fail: (err) => {
						uni.showToast({
							title: '复制失败！',
							duration: 2000,
							icon: 'none'
						});
					}
				});

			},
			handClose() {
				this.alertHide = false;
			},
			createLink() {
				//生成邀请链接
				// console.log(this.inviteForm,'this.inviteForm')
				// return
				if (this.inviteForm.factoryId == '') {
					uni.showToast({
						title: '请选择代理工厂',
						icon: 'none'
					})
					return
				}
				customerInvite(this.inviteForm).then(res => {

					let yuming = ser + '/jump.html';

					// let yuming='http://52.28.35.196/#/index'
					if (res.code == 0) {
						uni.showToast({
							title: '链接已生成！',
							icon: 'none'
						})
						this.tipsValueCopy = yuming + "?yaoqingId=" + res.data + '&lang=CN';
						//this.tipsValue = yuming + "?yaoqingId=" + res.data+'&isMobile=true&lang=CN';
						var lineurl = yuming + "?yaoqingId=" + res.data + '&lang=CN';
						this.tipsValue = lineurl.length >= 30 ? lineurl.slice(0, 33) + '...' : lineurl
						//console.log(this.tipsValue, 'tipsValue')
					}
				}).catch((err) => {
					this.setMsgTop(err)
				});
			},
			confirmYao() {

			},
			handCancelInvite(item) {
				this.$refs.clearAlarmPopup.open()
				this.EditId = item.Id
			},
			handInvite(id) {
				this.alertHide = true;
				if (this.wzValue && this.wzValue.length > 0) {
					this.wzValue = []
					this.wzCode = []
					this.isresetAddress = true
				}
				this.inviteForm = {
					regions: '',
					factoryId: '',
					bindCustomerId: ''
				}
				this.tipsValue = ''
				this.inviteForm.bindCustomerId = id

				AuthorizationData().then((res) => {
					//console.log(res,'AuthorizationData')
					this.AgentFactory = []
					res.data.forEach((ite) => {
						this.AgentFactory.push({
							value: ite.FactoryId,
							text: ite.FactoryName
						})
					})
				})
			},
			handAdd() {
				uni.navigateTo({
					url: './CreateCustomer'
				})
			},
			handEdit(id) {
				uni.navigateTo({
					url: './CreateCustomer?id=' + id
				})
			},
			handClickDetails(ite) {
				if (this.CustomerAll) {
					setPagesParam('customData', ite, 1);
				} else {
					uni.navigateTo({
						url: './CustomerDetails?id=' + ite.Id
					})
				}

			},
			selectFinsh(query) {
				//console.log(query,'111')
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.list();
			},
			searching(val) {
				this.key = val
				if (this.key) {
					this.queryData.Key = this.key
					this.queryData.pageNum = 1
					this.status = 'loading'
					this.customerData = []
					this.list()
				} else {
					delete this.queryData.Key
					this.queryData.pageNum = 1
					this.status = 'loading'
					this.customerData = []
					this.list()
				}
			},
			list() {
				//客户列表（私海）
				CustomerList(this.queryData).then((res) => {
					if (res.code == 0) {
						if (this.queryData.pageNum == 1) {
							this.customerData = []
						}
						this.customerData = [...this.customerData, ...res.data.List];
						if (res.data.List.length < this.queryData.pageSize) {
							this.status = 'noMore';
						} else {
							this.status = 'more';
						}
						//console.log(res,'res')
						//this.customerData=res.data.List;
					}
				})
			}
		}
	}
</script>
<style>
	page {
		background: #F5F8F9;
	}
</style>
<style lang="less" scoped>
	::v-deep.tree-bar {
		background: #fff;
		color: #333;
	}

	::v-deep.tree-view {
		background: #fff;
		color: #333;
	}

	::v-deep.tree-item .head.input_sel .txt {
		color: #333 !important;
	}

	.view_input {
		width: 100%;
		height: 88rpx;
		line-height: 88rpx;
		background-color: rgb(248, 248, 248);
		border-radius: 10rpx;
		border: 1rpx solid rgba(255, 255, 255, 0.20);
		color: #333333;
		font-size: 28rpx;
		padding: 10rpx 0;
		box-sizing: border-box;
		position: relative;
		display: flex;
		justify-content: flex-start;
		align-items: center;
		overflow: hidden;

		.tips_input {
			display: flex;
			justify-content: flex-start;
			align-items: center;
			overflow-x: scroll;
			white-space: nowrap;
			width: 100%;
			padding: 0 20rpx;
			box-sizing: border-box;
		}

		.view_mask {
			position: absolute;
			right: 0;
			top: 0;
			width: 92rpx;
			height: 88rpx;
			background-color: rgb(248, 248, 248);
			// z-index: 1;
		}

		.view_li_con {
			width: calc(100% - 92rpx);
			height: 68rpx;
			overflow: hidden;

			&.personnel_li_con {
				overflow-x: scroll;
				line-height: 68rpx;
			}
		}

		.view_li_cot {
			display: flex;
			justify-content: flex-start;
			align-items: center;
			width: auto;
			position: absolute;

			// position: relative;
			&.personnel_li_cot {
				overflow-x: scroll;
				position: relative;
			}
		}

		.shenglue_li::before {
			content: "...";
			position: absolute;
			bottom: 0;
			right: 63rpx;
			padding-left: 10px;
			z-index: 2;
		}

		.pal_col {
			color: #C1C1C1;
			font-size: 32rpx;
			padding-left: 20rpx;
			box-sizing: border-box;
			width: 100%;
			height: 100%;
			display: flex;
			align-items: center;
		}

		.view_li {
			display: flex;
			justify-content: flex-start;
			padding: 20rpx 30rpx 20rpx 20rpx;
			background-color: #fff;
			line-height: 28rpx;
			margin-left: 10rpx;
			white-space: nowrap;

			.view_icon {
				margin-left: 10rpx;
			}

			&.disabled_text {
				color: rgba(255, 255, 255, 0.50);
			}
		}

		.form_sel_icon {
			top: 0;
			width: 92rpx;
			z-index: 1;
		}
	}

	.copyInvite {
		.copyInvite-title {
			margin: 20rpx 0px;
			color: #999;
		}

		.validityPeriod {
			border-radius: 10rpx;
			border: 1rpx solid rgba(255, 255, 255, .2);
			height: 90rpx;
			line-height: 90rpx;
			margin-top: 20rpx;
			padding-left: 20rpx;
		}

		.copuUrl-period {
			color: #333333;
			padding: 15rpx 0rpx;
		}

		.copuUrl {
			border-radius: 6rpx;
			background: #2371FF;
			padding: 10px 10px;
			font-size: 24rpx;
			text-align: center;
			margin-top: 22rpx;
			color: #fff;
			font-size: 32rpx;
			margin-bottom: 25rpx;
		}

		.copyInvite-lianUrl {
			.copyInvite-lianUrl-url {
				align-items: center;
				border: 1rpx solid #EAEAEA;
				border-radius: 6rpx;
				padding: 0rpx 20rpx;
				line-height: 30rpx;
				word-break: break-all;
				height: 88rpx;
				line-height: 88rpx;
			}

		}
	}

	.move {
		position: fixed;
		height: 100%;
		width: 100%;
		background: #000;
		opacity: .7;
		top: 0px;
		left: 0px;
		z-index: 222;
	}

	.AddPopUps {
		position: fixed;
		background: #ffffff;
		z-index: 333;
		color: #333;
		width: 90%;
		margin: 0px 5%;
		border-radius: 8rpx;
		top: 250rpx;

		.addPool-selected {
			::v-deep.uni-select__input-text {
				color: #333 !important;
			}

			::v-deep.uni-select {
				height: 88rpx !important;
				border: 1rpx solid #f8f8f8;
				background: rgb(248, 248, 248);
			}

			::v-deep.uni-select__input-placeholder {
				font-size: 30rpx !important;
				color: #C1C1C1 !important;
			}
		}

		.link {
			color: rgb(153, 153, 153);
			margin: 0rpx 3%;
			margin-top: 30rpx;
		}

		.GenerateLink {
			text-align: center;
			height: 90rpx;
			border-radius: 6px;
			line-height: 90rpx;
			width: 94%;
			margin: 0 auto;
			color: #fff;
			margin-top: 20px;
			background: #2371FF;
		}

		.lianUrlInput {
			height: 88rpx;
			line-height: 88rpx;
			padding: 0rpx 20rpx;
			// border:2rpx solid rgba(255, 255, 255, .2);
			border-radius: 4rpx;
			margin-top: 15rpx;
			display: flex;
			align-items: center;
			border-radius: 10rpx;

			.lianUrl-input {
				width: 100%;
			}
		}

		.AddPopUps-title {
			position: relative;
			// margin:20rpx 30rpx;
			align-items: center;

			.title {
				text-align: center;
				font-size: 34rpx;
				font-weight: bold;
			}

			.icon-guanbidanchuang {
				position: absolute;
				margin-left: auto;
				width: 40rpx;
				height: 40rpx;
				right: 0rpx;
				top: 5rpx;
				color: #333333;
			}
		}

		.AddPopUps-item {
			padding: 30rpx;

		}
	}
</style>