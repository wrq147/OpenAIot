<template>
	<view>
		<top :title="topTitle" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#ffffff">
		</top>
		<uni-nav-bar :status-bar="false" :fixed="true" :border="false" :height="fixheight" :zIndex="995"
			backgroundColor="#ffffff">
			<template v-slot:allslot>
				<uni-forms ref="itemForm" :modelValue="item" :rules="rules">
					<uni-forms-item required name="DeviceNumber" id="DeviceNumber_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5" :showLabel="false" :isLastTop="true"
						:noBottomMar="true">
						<view class="search_con" id="DeviceNumber">
							<view class="search">
								<uni-easyinput prefixIcon="icon-sousuo"
									placeholder="请扫码添加或者输入物品编号"
									v-model="item.DeviceNumber" clearSize="18" :prefixIconSize="12"
									placeholderStyle="color:rgba(193, 193, 193, 1);font-size:28rpx"
									:styles="customstyles" inputHeight="64rpx"
									:isCustom="true" prefixIconFocusColor="#999999"
									prefixIconColor="rgba(193, 193, 193, 1)" @input="iptChange"
									@iconClick="iptChange">
								</uni-easyinput>
							</view>
							<view class="text" @click.stop="scanChecking">
								<custom-icons iconsName="icon-saoma" iconsSize="34rpx"
									iconsColor="#999999"></custom-icons>
							</view>
						</view>
					</uni-forms-item>
					<uni-forms-item required name="Num" id="Num_form" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" :showLabel="false" :isLastTop="true" :noBottomMar="true">
						<view class="search_con" id="Num">
							<view class="search btn_search">
								<uni-easyinput placeholder="请输入物品数量"
									v-model="item.Num" clearSize="18" :prefixIconSize="12"
									placeholderStyle="color:rgba(193, 193, 193, 1);font-size:28rpx"
									:styles="customstyles" inputHeight="64rpx"
									:isCustom="true" prefixIconFocusColor="#999999"
									prefixIconColor="rgba(193, 193, 193, 1)">
								</uni-easyinput>
							</view>
							<view class="btn" @click.stop="onSubmit">
								确定
							</view>
						</view>
					</uni-forms-item>
				</uni-forms>
				<view class="checking_item_con" id="checking_con" v-if="item.Id">
					<view class="checking_item">
						<view class="checking_li">
							<view class="label">物品名称:</view>
							<view class="value">{{item.Name}}</view>
						</view>
						<view class="checking_li hasmar">
							<view class="label">物品类型:</view>
							<view class="value">{{item.TargetType==1?'设备':'耗材'}}</view>
						</view>
					</view>
				</view>
			</template>
		</uni-nav-bar>

		<view class="detail_con">
			<view class="basic_info_list" v-if="tableData&&tableData.length>0">
				<view class="line_title">
					<view class="left_text">
						<text>盘点任务</text>
					</view>
					<view class="line"></view>
				</view>
				<view class="li_pro_con" v-for="(row,inx) in tableData" :class="{'first_con':inx==0}">
					<view class="li_pro">
						<view class="pro_left">
							<image class="image" :src="returnPhotoUrl(row.PhotoUrl)" mode=""></image>
						</view>
						<view class="pro_right">
							<view class="pro_title">{{row.Name}}</view>
							<view class="pro_num_price">
								<view class="text1">
									{{row.DeviceNumber}}
								</view>
								<view class="text2" style="text-align: right;">
									{{row.TargetType==1?'设备':'耗材'}}
								</view>
							</view>
						</view>
					</view>
				</view>
			</view>
		</view>
		<view class="botton_fix">
			<view class="label">
				盘点任务
			</view>
			<view class="progress_bar">
				<progress backgroundColor="rgba(248, 248, 248, 1)"
					:percent="parseInt((AllTotal - total) * 100.0 / AllTotal)" show-info stroke-width="10"
					activeColor="rgba(71, 226, 241, 1)" border-radius="8" />
			</view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		invItemList,
		invInfo,
		confirmItem,
		itemInfo
	} from "@/api/inventory";
	import {
		OutStockByKey,
	} from "@/api/stock";
	// #ifdef H5
	import serverUrl1 from '@/common/constVar.js'
	// #endif
	// #ifndef H5
	import serverUrl from '@/common/constVar.js'
	// #endif
	export default {
		data() {
			return {
				isShow: false,
				item: {
					Id: undefined,
					DeviceNumber: "",
					Num: null,
					Name: "",
					TargetType: 0,
				},
				topTitle: '盘点',
				taskId: '',
				customstyles: {
					color: '#333333',
					backgroundColor: 'rgba(248, 248, 248, 1)',
					disableColor: 'rgba(248, 248, 248, 0.5)',
					borderColor: 'rgba(248, 248, 248, 1)'
				},
				// 查询参数
				queryParams: {
					pageNum: 1,
					pageSize: 10,
				},
				// 表单校验
				rules: {
					DeviceNumber: {
						rules: [{
							required: true,
							errorMessage: "物品编号不能为空",
							// trigger: "change"
						}]
					},
					Num: {
						rules: [{
							required: true,
							errorMessage: "物品数量不能为空",
							// trigger: "change"
						}]
					}
				},
				form: {},
				tableData: [],
				// 总条数
				total: 0,
				AllTotal: 1,
				fixheight: '176rpx'
			};
		},
		onLoad(options) {
			if (options.id) {
				this.taskId = options.id
				this.loadInitialize(options.id)
			}
		},
		computed: {},
		methods: {
			scanChecking() {
				// 扫码盘点
				
				let that = this
				uni.scanCode({
					success: function(res) {
						let currenturl = res.result
						let targetUID = ''
						let tmpidx = res.result.lastIndexOf("iot");
						if (tmpidx > -1) {
							targetUID = res.result.substring(tmpidx);
				
						} else if (currenturl.indexOf("HD") == 0 || res.result.indexOf("UID") > -1 || res
							.result.indexOf("Codes") > -1) {
							if (currenturl.indexOf("HD") == 0) {
								//为一期设备码
								targetUID = currenturl;
							} else {
								let tmpidx1 = res.result.lastIndexOf("UID");
								targetUID = res.result.substring(tmpidx1);
								if (targetUID == null) {
									this.$refs.promptMsg.open('二维码无效', 2000)
									return;
								}
							}
							if (res.result.indexOf("Codes") > -1) {
								let paramCodes = res.result.lastIndexOf("Codes")
								targetUID = res.result.substring(paramCodes);
							}
				
						} else {
							this.$refs.promptMsg.open('二维码无效', 2000)
						}
						OutStockByKey(targetUID).then(res => {
							if (res.data) {
								this.item.DeviceNumber=res.data.DeviceNumber
								this.iptChange()
							}
						}).catch(err => {
							this.setMsgTop(err)
						})
					}
				});
			},
			returnPhotoUrl(url) {
				let ser=''
				// #ifdef H5
				ser=serverUrl1.getServerUrl()
				// #endif
				// #ifndef H5
				ser=serverUrl.getServerUrl()
				// #endif
				return ser + url
			},
			async loadInitialize(id) {
				this.isloading = true;
				this.resetItem();
				let invRsp = await invInfo(id);
				this.title = "'" + invRsp.data.Name + "'的盘点任务";
				this.form = invRsp.data;
				await this.getList();
				this.isloading = false;
			},
			async getList() {
				let allRsp = await invItemList({
					Id: this.form.Id
				});
				this.AllTotal = allRsp.data.Total;
				this.queryParams.Id = this.form.Id;
				if (this.form.Status == 2) {
					this.queryParams.UnFirst = true;
				} else if (this.form.Status == 3) {
					this.queryParams.UnCheck = true;
				} else {
					return;
				}
				let itemRsp = await invItemList(this.queryParams);
				this.tableData = itemRsp.data.List;
				this.total = itemRsp.data.Total;
			},
			async onSubmit() {
				let valiRsp = await this.$refs["itemForm"].validate();
				if (valiRsp) {
					let cc = parseInt(eval(this.item.Num));
					await confirmItem(this.form.Id, this.item.DeviceNumber, cc);
					this.$refs.promptMsg.open('提交成功！开始计数下一个物品', 2000)
					this.getList();
					this.resetItem();
				}
			},
			resetItem() {
				this.item = {
					Id: undefined,
					DeviceNumber: "",
					Num: null,
					Name: ""
				}
				this.fixheight = '176rpx'
			},
			async iptChange() {
				this.$nextTick(async () => {
					this.isShow = false
					let devN = this.item.DeviceNumber
					if (this.item.DeviceNumber != "") {
						try {
							let xx = await itemInfo(this.form.Id, this.item.DeviceNumber);
							if (xx.data == null) {
								this.resetItem();
								this.item.DeviceNumber = devN
								this.$refs.promptMsg.open('这个物品不存在', 2000)
								return;
							}
							this.item.Id = xx.data.Id;
							this.item.DeviceNumber = xx.data.DeviceNumber;
							this.item.Name = xx.data.Name;
							this.item.TargetType = xx.data.TargetType
							uni.showLoading({
								title: 'loading'
							})
							setTimeout(()=>{//这里修改绝对定位的距离
								let hei = 0
								const query = uni.createSelectorQuery().in(this);
								query.select('#checking_con').boundingClientRect(data => {
									hei = data.height
								}).exec();
								let hei2 = hei * 2 + 176
								this.fixheight = hei2 + 'rpx'
								this.$forceUpdate()
								uni.hideLoading()
							},300)
						} catch (e) {
							//TODO handle the exception
							console.log(e);
							this.setMsgTop(e)
						}
					}
				})

			}
		}
	}
</script>

<style lang="less" scoped>
	.botton_fix {
		width: 100%;
		height: 98rpx;
		line-height: 98rpx;
		background-color: rgba(255, 255, 255, 1);
		position: fixed;
		bottom: 0;
		left: 0;
		padding: 0 20rpx;
		// display: flex;
		// justify-content: center;
		box-sizing: border-box;
		color: rgba(51, 51, 51, 1);
		box-shadow: 0px -6px 14px 0px rgba(0,0,0,0.02);
		display: flex;
		justify-content: flex-start;
		align-items: center;
		.label{
			width: 112rpx;
			margin-right: 22rpx;
			font-size: 28rpx;
			color: rgba(153, 153, 153, 1);
		}
		.progress_bar{
			width: calc(100% - 132rpx);
			
			
		}
	}
	::v-deep .uni-progress{
		.uni-progress-bar{
			border-radius: 8rpx !important;
			.uni-progress-inner-bar{
				border-radius: 8rpx !important;
			}
		}
	}

	.checking_item_con {
		width: 100%;
		background-color: rgba(255, 255, 255, 1);
		padding: 20rpx 20rpx 0 20rpx;
		box-sizing: border-box;
	}

	.checking_item {
		padding: 18rpx 30rpx;
		width: 100%;
		background-color: rgba(255, 255, 255, 1);
		box-sizing: border-box;
		// margin-top: 20rpx;
		border-radius: 10rpx;

		.checking_li {
			font-size: 28rpx;
			line-height: 40rpx;
			display: flex;
			justify-content: flex-start;
			align-items: center;
			flex-wrap: wrap;

			&.hasmar {
				margin-top: 8rpx;
			}

			.label {
				color: #999999;
			}

			.value {
				color: #333333;
				margin-left: 10rpx;
			}
		}
	}

	.search_con {
		width: 100%;
		padding: 12rpx 20rpx;
		box-sizing: border-box;
		background-color: #ffffff;
		height: 88rpx;
		display: flex;
		justify-content: space-between;
		align-items: center;


		.search {
			width: 644rpx;
			height: 64rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			background-color: #161A26;
			// padding: 0 6rpx 0 20rpx;
			box-sizing: border-box;
			border-radius: 10rpx;

			&.btn_search {
				width: 550rpx;
			}

			.input_text {
				width: 644rpx;
				font-size: 28rpx;
			}
		}

		.btn {
			width: 140rpx;
			height: 64rpx;
			background: rgba(35, 113, 255, 1);
			color: #FFFFFF;
			font-size: 28rpx;
			display: flex;
			justify-content: center;
			align-items: center;
			border-radius: 10rpx;
		}

		.text {
			font-size: 28rpx;
			color: #999999;
		}
	}
</style>