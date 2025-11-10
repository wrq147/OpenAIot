<template>
	<view>
		<top :title="topTitle" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="#ffffff" :rightIcon="isSelect?'':'icon-tianjia'" @clickRight="openAgentAdd">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入名称" inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>
		<view class="agent_con">
			<view class="agent_list">
				<view class="agent_li" v-for="item in tbList" @click="clickListLi(item)">
					<view class="li_left">
						<image class="image" :src="returnImgUrl(item.Logo)+'?wh=500x500'" mode=""></image>
					</view>
					<view class="li_right">
						<view class="right_li first">
							{{item.OrgName}}
						</view>
						<view class="right_li" v-if="item.RegionsName">
							地区: {{item.RegionsName}}
						</view>
					</view>
				</view>
			</view>
		</view>
		<uni-load-more iconType="circle" :status="status" v-if="status" />
		<msg-prompt ref="promptMsg"></msg-prompt>
		<uni-popup ref="invitePopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">邀请代理</view>
					<view class="form_content content">

						<view class="label">
							选择代理区域
						</view>
						<view class="form_li">
							<view class="view_input" @click="choiceWZ">
								<view class="pal_col" v-if="!wzValue||wzValue.length==0">
									请选择区域
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
										iconsColor="#c1c1c1"></custom-icons>
								</view>
							</view>
						</view>
					</view>
					<button class="submit_button" @click.stop="generateLink">
						生成链接
					</button>
					<view class="close_icon" @click="noticeColse">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
							iconsColor="rgba(193, 193, 193, 1)"></custom-icons>
					</view>
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="linkPopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">邀请代理</view>
					<view class="form_content content">
						<view class="label">
							邀请链接
						</view>
						<view class="form_li">
							<view class="view_input">
								<view class="tips_input">
									{{tipsValue}}
								</view>
							</view>
						</view>
					</view>
					<button class="submit_button" @click.stop="copyLink">
						复制链接
					</button>
					<view class="text_label">
						链接有效期:<view class="text_text">
							<text style="color: rgba(35, 113, 255, 1);margin-left: 10rpx;">7天</text>后邀请链接失效
						</view>
					</view>

					<view class="close_icon" @click="noticeColse2">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
							iconsColor="rgba(193, 193, 193, 1)"></custom-icons>
					</view>
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="addressPopup" type="bottom" @change="popChange" :z-index="1000">
		<hy-address-multiple ref="addressSelect" :address="wzItems" @confirm="addressConfirm"></hy-address-multiple>
		</uni-popup>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		factorygetAgent,
		factoryInvite
	} from "@/api/factory";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	// #ifdef H5
	import serverUrl1 from '@/common/constVar.js'
	// #endif
	// #ifndef H5
	import serverUrl from '@/common/constVar.js'
	// #endif
	export default {
		data() {
			return {
				addressPopup:false,
				parIsFocus: false,
				wzCode: [],
				wzValue: [],
				wzItems: [],
				wzValPar: [],
				wzValPar2: [],
				topTitle: '代理',
				querydata: {
					pageNum: 1,
					pageSize: 10,
				},
				timeQuery: {
					name: '创建时间',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				tbList: [],
				total: 0,
				selectAgent: [],
				selectId: [],
				isSelect: false,
				isMulSelect: false,
				status: 'loading',
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				isYaoqingLoad: false,
				tipsValue: '' ,//邀请链接的值
				isresetAddress:false
				
			};
		},
		async onLoad(options) {
			uni.showLoading({
				title: '加载中'
			})
			try {
				this.getList()
				if (options.isSelect) {
					this.isSelect = !!options.isSelect
					if (options.selectId) {
						this.selectAgent = JSON.parse(options.selectId)
						this.selectId = this.selectAgent.map(item => {
							if (item.id) {
								return item.id
							} else if (item.TargetId) {
								return item.TargetId
							}
						})
					}
					if (options.isMulSelect) {
						this.isMulSelect = !!options.isMulSelect
					}
				}else{
					let arr=await this.$store.dispatch("data/areaTree")
					this.wzItems=JSON.parse((JSON.stringify(arr)))
				}

				// console.log("地域信息", this.wzItems);
				uni.hideLoading()
			} catch (e) {
				//TODO handle the exception
				uni.hideLoading()
			}

		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = "loading";
				this.getList();
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			popChange(){
				
				//弹窗变化
			},
			returnImgUrl(url) {
				let ser = ''
				// #ifdef H5
				ser = serverUrl1.getServerUrl()
				// #endif
				// #ifndef H5
				ser = serverUrl.getServerUrl()
				// #endif
				// console.log("url", ser + url);
				if (url.indexOf('http') > -1) {
					return url
				} else {
					return ser + url
				}

			},
			choiceWZ() {
				//选择位置
				//打开选择器
				// this.addressPopup=true
				this.$refs.addressPopup.open()
				if(this.isresetAddress){
					this.$nextTick(async ()=>{
						let arr=await this.$store.dispatch("data/areaTree")
						// console.log(arr,'arr');
						this.wzItems=JSON.parse(JSON.stringify(arr))
						this.$refs.addressSelect.newAddress=JSON.parse(JSON.stringify(arr))
						this.isresetAddress=false
					})
				}
			},
			copyLink() {
				//复制邀请链接
				uni.setClipboardData({
					data: this.tipsValue,
					success: (res) => {
						this.$refs.linkPopup.close()

						// this.$refs.promptMsg.open('Replicating Success', 1500)
					}
				});
			},
			generateLink() {
				//生成链接
				this.$refs.invitePopup.close()
				this.$refs.promptMsg.loadingOpen('生成中...')
				let query = {
					customerType: 0, //邀请的客户类型：0为代理，1为直销
					bindCustomerId: "" //绑定的客户
				};
				if (this.wzCode && this.wzCode.length > 0) {
					query.regions = this.wzCode.join(',');
				} else {
					query.regions = "";
				}

				factoryInvite(query).then(async res => {
					if (res.code == 0) {
						let ser = ''
						// #ifdef H5
						ser = serverUrl1.getServerUrl()
						// #endif
						// #ifndef H5
						ser = serverUrl.getServerUrl()
						// #endif
						this.tipsValue = ser + '/jump.html?lang=CN' + "&yaoqingId=" + res.data;
					}
					if (this.wzValue && this.wzValue.length > 0) {
						this.wzValue = []
						this.wzCode = []
						this.isresetAddress=true
					}
					this.$refs.linkPopup.open()
					this.$refs.promptMsg.loadingColse()
				}).catch(err => {
					this.setMsgTop(err)
					this.$refs.promptMsg.loadingColse()
				});

			},
			noticeColse2() {
				this.$refs.linkPopup.close()
			},
			noticeColse() {
				this.$refs.invitePopup.close()
			},
			openAgentAdd() {
				//代理商邀请
				if (this.isSelect) {} else {
					if (this.wzValue && this.wzValue.length > 0) {
						this.wzValue = []
						this.wzCode = []
						this.isresetAddress=true
					}
					this.$refs.invitePopup.open()
				}
			},
			addressConfirm(resArr, resNameArr) {
				this.wzValue=JSON.parse(JSON.stringify(resNameArr))
				this.wzCode=JSON.parse(JSON.stringify(resArr))
				this.$refs.addressPopup.close()
			},
			confirmSelected() {
				//确定选择设备结果
				setPagesParam('finishSelectAgents', this.selectAgent, 1)
			},
			clickListLi(row) {
				if (this.isSelect) {
					this.selectPJFun(row)
				} else {
					uni.navigateTo({
						url: '/pages_factory/agents_detail?id=' + row.Id
					})
				}
			},
			selectPJFun(row) {
				if (this.selectId && this.selectId.includes(row.Id)) {
					let inxO = this.selectId.indexOf(row.Id)
					this.selectId.splice(inxO, 1)
					this.selectAgent.splice(inxO, 1)
				} else {
					this.selectAgent.push(row)
					this.selectId.push(row.Id)
				}
				if (this.isMulSelect) {} else {
					let arr = []
					arr.push(row)
					setPagesParam('finishSelectAgents', arr, 1)
				}

			},
			/** 查询列表 */
			getList() {
				this.status = 'loading'
				if (this.isYaoqingLoad) {
					//清除邀请区域信息
					this.wzValue = []
					this.wzCode = []
				}
				factorygetAgent(this.querydata).then(response => {
					// console.log(response, 'response代理商');
					if (this.querydata.pageNum == 1) {
						this.tbList = []
					}
					this.tbList = [...this.tbList, ...response.data.List];
					this.total = response.data.Total;
					if (response.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				}).catch(err => {
					this.status = 'noMore';
					this.setMsgTop(err)
				});
			},
			loadList() {
				//加载列表方法
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.getList()
			},
			searching(val) {
				//搜索
				if (val) {
					this.querydata.Name = val

				} else {
					delete this.querydata.Name
				}
				this.loadList()
			},
			openSelect() {
				this.$refs.selectCompt.openSelect()
			},
			selectFinsh(query) {
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.tbList = []
				this.status = 'loading'
				this.getList()

			},
		}
	}
</script>

<style lang="less" scoped>
	.form_content {
		display: flex;
		flex-direction: column;
		justify-content: flex-start;
		align-items: flex-start;
		width: 100%;

		&.content {
			margin-bottom: 20rpx;
		}

		.form_li {
			width: 100%;
			margin-top: 20rpx;
		}
	}

	.notice_cont {
		.text_label {
			margin-top: 34rpx;
			font-size: 28rpx;
			color: rgba(153, 153, 153, 1);
			line-height: 42rpx;
			display: flex;
			justify-content: flex-start;
			align-items: center;
		}

		.text_text {
			font-size: 28rpx;
			color: #333333;
			line-height: 42rpx;
			// margin-top: 12rpx;
		}
	}

	.view_input {
		width: 100%;
		height: 88rpx;
		line-height: 88rpx;
		background-color: rgba(248, 248, 248, 1);
		border-radius: 10rpx;
		// border: 1rpx solid rgba(255, 255, 255, 0.20);
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
			font-size: 32rpx;
		}

		.view_mask {
			position: absolute;
			right: 0;
			top: 0;
			width: 92rpx;
			height: 88rpx;
			background-color: rgba(248, 248, 248, 1);
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
			color: #c1c1c1;
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
			background-color: #ffffff;
			line-height: 28rpx;
			margin-left: 10rpx;
			white-space: nowrap;

			.view_icon {
				margin-left: 10rpx;
			}

			&.disabled_text {
				color: #c1c1c1;
				border: 1rpx solid #c1c1c1;
			}
		}

		.form_sel_icon {
			top: 0;
			width: 92rpx;
			z-index: 1;
		}
	}
</style>