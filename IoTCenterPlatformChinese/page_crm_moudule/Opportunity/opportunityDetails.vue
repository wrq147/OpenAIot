<template>
	<view id="CreateCustomer">
		<top :isRightSlot="true" leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff"
			title="详情" rightText="编辑" class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handEdit()">
					<view class="iconfont icon-bianji" style="font-size:32rpx;"></view>
				</view>
			</template>
		</top>
		<view class="CreateCustomer-header">
			<view class="Avatar-information">
				<view class="iconfont t-icon-shangjitouxiang1">

				</view>
				<view class="Avatar-information-title">
					{{arr1.OpportName}}
				</view>
				<view class="Avatar-information-content">
					NO.{{arr1.OpportNumber}}
				</view>

				<view class="Avatar-Contacts">
					<view class="Avatar-Contacts-left">
						<text>联系人: </text>
					</view>
					<view class="Avatar-Contacts-right">
						<view class="Avatar-Contacts-right-David">
							<!-- <text class="aviter"></text> -->
							<text class="content">{{contentName}}</text>
						</view>

					</view>
					<view class="iconfont t-icon-dianhua1" @click="TelPhone()">

					</view>
				</view>
			</view>
		</view>
		<!--需求阶段-->
		<view class="prodit">
			<!-- :class="[selected==item.PeriodName?'active':'on']" index==1||index==2||index==3?'saleBgImg':index==4?'two-saleImg':'on'-->
			<view :style="{'background-image':returnBgUrl(index)}"
				:class="[index==0?'one-saleImg':index==1||index==2||index==3?'saleBgImg':index==4?'two-saleImg':index==5?'singleOn':'on']"
				class="prodit-item" v-for="(item,index) in RequirementStage" :key="index"
				@click="handProditItem(item,index)">
				<view :style="{'background-image':returnBgUrl2(selected,PeriodId==item.Id)}" :class="[selected==0&&PeriodId==item.Id?'active':selected==1&&PeriodId==item.Id||selected==2&&PeriodId==item.Id||selected==3&&PeriodId==item.Id?'Rectangular':
				selected==4&&PeriodId==item.Id?'four-saleImg':
				selected==5&&PeriodId==item.Id||selected==6&&PeriodId==item.Id?'five-saleImg':'']">
					{{item.PeriodName}}
				</view>
			</view>
		</view>
		<view class="navigation-nav">
			<view v-for="(item,index) in arr" :key="index" class="navigation-nav-item" @click="handClickTo(index)"
				:class="[select==index?'active':'on']">
				<view>
					{{item.title}}
				</view>
				<view class="navigation-nav-border" v-if="select==index">

				</view>
			</view>
		</view>
		<view v-if="select==0">
			<view class="Information-item">
				<view class="Information">
					<view class="Information-title">
						销售阶段
					</view>
					<view class="Information-bottom">
						<text>{{ValidateText}}</text>
					</view>
				</view>
				<view class="Information">
					<view class="Information-title">
						成交几率
					</view>
					<view class="Information-bottom">
						<text>{{arr1.Probability}}</text>
					</view>
				</view>
				<view class="Information">
					<view class="Information-title">
						负责人
					</view>
					<view class="Information-bottom" v-if="arr1.LeaderUser">
						<img :src="arr1.LeaderUser.Avatar" class="aviter" alt="" />
						<text>{{arr1.LeaderUser.RealName}}</text>
					</view>
				</view>


				<view class="Information ">
					<view class="Information-title">
						协作人
					</view>
					<view class="Information-bottom" v-for="(ite,inx) in arr1.HelperUsers" :key="inx">
						<img class="aviter" :src="ite.Avatar" alt="">
						<text>{{ite.RealName}}</text>
					</view>
				</view>
				<view class="Information">
					<view class="Information-title">
						商机详情
					</view>
					<view class="Information-bottom">
						<text>{{arr1.Remark}}</text>
					</view>
				</view>
				<view class="Information InformBorder">
					<view class="Information-title">
						创建时间
					</view>
					<view class="Information-bottom">
						<text>{{arr1.createTime}}</text>
					</view>
				</view>
			</view>
		</view>
		<view v-if="select==1">

			<!-- <view class="productList" v-for="(item,index) in DetailList" :key="index" >
				<img class="productList-logo"  :src="item.PhotoUrl" alt="">
				<view class="productList-content">
					<view class="title">{{item.Name}}</view>
					<view class="content">{{item.DeviceNumber}}</view>
				</view>
				<view class="Mechines">
					<view v-if="item.ProductType==1">
						设备
					</view>
					<view v-else>
						耗材
					</view>
				</view>
			</view> -->
			<view class="product_ul" style="background:#ffffff;padding: 30rpx;margin-top: 30rpx;">
				<view class="product_li li_border" v-for="(row,inx) in DetailList"
					:style="{'margin-top':inx==0?'0':'20rpx'}">
					<view class="li_cont">
						<view class="cont_title">
							<view class="title_label" :class="{'normal':row.ProductLabel&&row.ProductLabel=='U'}">
								{{row.ProductLabel&&row.ProductLabel=='U'?'半成品':'成品'}}</view>
							<view class="title_name">
								<text>{{row.ProductName}}</text>
								<text v-if="row.TypeName" class="line">|</text>
								<text v-if="row.TypeName">{{row.TypeName}}</text>
							</view>
						</view>
						<view class="cont_number" v-if="row.SkuNumber">产品编码：{{row.SkuNumber}}</view>
					</view>
				</view>
			</view>


		</view>

		<view v-if="select==2">
			<view class="Follow-up-input" @click="handAddFloow">
				<text class="iconfont icon-tianxiegenjinhepinglun"></text>
				<input class="searchInput" type="text" placeholder="添加跟进动态">
			</view>

			<view class="Follow-up-item" v-for="(item,index) in FastTracking" :key="index">
				<view class="Follow-up-item-top">
					<img :src="item.FollowUserInfo.Avatar" class="aviter" alt="">
					<view class="Follow-up-item-top-content">
						<view>{{item.FollowUserInfo.RealName}}</view>
						<view class="time">{{item.createTime}}</view>
					</view>
				</view>
				<view class="Follow-up-item-center" v-html="item.Remark">

				</view>
				<view class="Follow-up-item-bottom">
					<text class="tit_text">跟进方式:</text>
					<text>{{item.FollowWay}}</text>
				</view>
			</view>
			<view style="height:50px;"></view>
		</view>
		<view v-if="select==3">
			<view class="Follow-up-input" @click="handComment">
				<text class="iconfont icon-tianxiegenjinhepinglun"></text>
				<input class="searchInput" type="text" placeholder="请输入评论">
			</view>
			<view class="Comments-item" v-for="(item,index) in payList" :key="index" @click="handPayListDetails(item)">
				<view class="Comments-item-leftparse">
					<view class="Comments-item-left" style="margin-top:15rpx;">
						<img class="aviter" :src="item.UserInfo.Avatar" alt="">
					</view>
					<view class="Comments-item-txt">
						<view class="Comments-item-txt-title">{{item.UserInfo.RealName}}</view>
						<view class="Comments-item-txt-content" v-html="item.Content">

						</view>
						<view class="Comments-item-parse">
							<view>{{item.CreateOn}}</view>
							<view class="reply">
								<text class="iconfont icon-huifu"></text>
								<text>回复</text>
							</view>
						</view>
					</view>
				</view>
				<view class="Comments-item" v-for="(row,index1) in item.children"
					style="padding-right:0rpx;margin-right:0rpx;">
					<view class="Comments-item-leftparse">
						<view class="Comments-item-left" style="margin-top:15rpx;">
							<img class="aviter" :src="row.UserInfo.Avatar" alt="">
						</view>
						<view class="Comments-item-txt">
							<view class="Comments-item-txt-title">{{row.UserInfo.RealName}}</view>
							<view class="Comments-item-txt-content" v-html="row.Content">

							</view>
							<view class="Comments-item-parse">
								<view>{{row.CreateOn}}</view>
								<view class="reply">
									<text class="iconfont icon-huifu"></text>
									<text>回复</text>
								</view>
							</view>
						</view>
					</view>
				</view>
			</view>
			<view style="height:50px;"></view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
		<msg-prompt ref="promptReceive" @confirm="confireCloseDrawer"></msg-prompt>
		<msg-prompt ref="promptDelete" @confirm="confireDelete"></msg-prompt>
		<view class="move" v-if="alertHide"></view>
		<!--添加弹窗-->
		<view class="AddPopUps" v-if="alertHide">
			<!--评论弹框-->
			<view v-if="alertName=='comment'">
				<view class="AddPopUps-title" style="margin-top:25rpx;">
					<view class="title">
						评论
					</view>
					<view class="iconfont icon-guanbidanchuang" @click="handClose">

					</view>
				</view>
				<view>
					<textarea v-model="content" placeholder="请输入评论" class="addPool-textarea" :rows="4" />
					<view class="addPool-button" @click="handCommentSubmit">
						确认
					</view>
				</view>
			</view>
		</view>
		<view class="BottomPositioning" @click="handDelete" v-if="select==0">
			删除
		</view>
	</view>
</template>

<script>
	import {
		BusineDataDetails, //商机详情
		flowDataRecord, //快速跟进详情
		commentList, //评论列表
		commentAdd, //评论添加
		contactData, //联系人
		periodList, //需求阶段
		phaseSwitching, //切换销售阶段
		oppartRemove //删除
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import {
		yuanAarngemnt //员工
	} from "@/api/personalCenter";
	export default {
		data() {
			return {
				DetailList: [],
				arr1: [],
				select: 0,
				arr: [{
						title: '基本信息'
					},
					{
						title: '商机明细'
					},
					{
						title: '快速跟进'
					},
					{
						title: '评论'
					}
				],
				FastTracking: [],
				ExecutionMode: new Map(),
				//yuanGong: new Map(),
				payList: [],
				alertHide: false,
				inviteForm: {
					regions: '',
					factoryId: '',
					bindCustomerId: ''
				},
				content: '',
				parentCommentId: '',
				parentCommentUserId: '',
				EditId: '',
				contentName: '',
				RequirementStage: [], //需求阶段
				selected: '',
				xiaoShou: new Map(), //销售阶段
				PeriodId: 0,
				ValidateText: '',
				SelecteInx: '',
				SelectePer: ''
			}
		},
		async onLoad(option) {
			if (option.id) {
				this.EditId = option.id
				this.list(option.id)
				this.CommentData(option.id)
				//this.stageData();//销售阶段
				let partUnit = await this.$store.dispatch("data/dictList", 'follow_way');
				partUnit.forEach((item) => {
					this.ExecutionMode.set(item.value, item.label);
				})
			}
		},
		methods: {
			returnBgUrl(index){
				//返回背景图片路径
				if(index==0){
					return `url(${this.getSerVerUrl()}/appimg/images/one-saleImg.png)`
				}else if(index==1||index==2||index==3){
					return `url(${this.getSerVerUrl()}/appimg/images/saleBgImg.png)`
				}else if(index==4){
					return `url(${this.getSerVerUrl()}/appimg/images/two-saleImg.png)`
				}
			},
			returnBgUrl2(selected,isac){
				if(isac){
					if(selected==0){
						return `url(${this.getSerVerUrl()}/appimg/images/saleBgZhong.png)`
					}else if(selected==1||selected==2||selected==3){
						return `url(${this.getSerVerUrl()}/appimg/images/Rectangular.png)`
					}else if(selected==4){
						return `url(${this.getSerVerUrl()}/appimg/images/four-saleImg.png)`
					}
				}else{
					return ''
				}
				
			},
			TelPhone() {
				const res = uni.getSystemInfoSync(); //获取当前的手机机型
				if (res.platform == 'ios') {
					uni.makePhoneCall({
						phoneNumber: this.arr.Mobile
					})
				} else {
					uni.makePhoneCall({
						phoneNumber: this.arr.Mobile,
					})
				}
			},
			confireDelete() {
				//删除
				oppartRemove({
					id: this.EditId
				}).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: '删除成功！',
							icon: 'none'
						})
						setTimeout(() => {
							setPagesParam('list')
						}, 700)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handDelete() {
				//删除
				this.$refs.promptDelete.noticeOpen(
					"你确定要删除商机吗?"
				)
			},
			confireCloseDrawer() {
				phaseSwitching({
					id: this.EditId,
					target: this.PeriodId
				}).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: '切换成功!',
							icon: 'none'
						})
						this.list(this.EditId);
					}
				})
			},
			handProditItem(item, inx) {
				if (this.ValidateText != item.PeriodName) {
					this.$refs.promptReceive.noticeOpen(
						"确认更改名为" + item.PeriodName + "的销售阶段！"
					)
					this.selected = inx;
					this.PeriodId = item.Id;

				}
				//console.log(this.selected,'this.selected')
			},

			handClose() {
				this.alertHide = false;
			},
			handPayListDetails(row) {
				this.content = ''
				this.alertHide = true
				this.alertName = 'comment'
				this.parentCommentId = row.Id
				this.parentCommentUserId = row.UserId
			},
			handComment() {
				this.alertHide = true
				this.alertName = 'comment'
				this.content = ''
				this.parentCommentId = ''
				this.parentCommentUserId = ''
			},
			//评论
			handCommentSubmit() {
				if (this.content == '') {
					uni.showToast({
						title: '请输入评论内容！',
						icon: 'none'
					})
					return
				}
				if (this.parentCommentId && this.parentCommentUserId) {
					var data = {
						content: this.content,
						TargetType: "商机",
						TargetId: this.EditId,
						parentCommentUserId: this.parentCommentUserId,
						parentCommentId: this.parentCommentId
					}
				} else {
					var data = {
						content: this.content,
						TargetType: "商机",
						TargetId: this.EditId
					}
				}
				commentAdd(data).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: '评论添加成功!',
							icon: 'none'
						})
						this.alertHide = false;
						this.CommentData(this.EditId);
					}
				})
			},
			//评论列表
			CommentData(id) {
				commentList({
					TargetType: "商机",
					TargetId: id,
					pageNum: 1,
					pageSize: 10,
				}).then((data) => {
					//console.log(data.data.List,'评论列表');
					//this.payList=data.data.List;
					var res1 = data.data.List;
					var listParse = []
					var listSon = []
					res1.map((row) => {
						if (row.ParentCommentUserId == 0) {
							listParse.push(row)
						} else {
							listSon.push(row)
						}
					})
					listParse.forEach((item, index) => {
						var str = []
						listSon.forEach((v) => {
							if (item.Id == v.ParentCommentId) {
								//console.log(item.value,v.ParentId,'v.ParentId')
								str.push(v)

							}
						})
						item['children'] = str;
					})
					this.payList = listParse
					//console.log(listParse,'评论列表分离')
				})
			},
			handAddFloow() {
				uni.navigateTo({
					url: '../Customer/AddFollow-up?id=' + this.arr1.CustomerId
				})
			},
			//快速跟进
			listData(id) {
				flowDataRecord({
					TargetId: id,
					pageNum: 1,
					pageSize: 10
				}).then((res) => {
					//console.log(res,'快速跟进')
					if (res.code == 0) {
						this.FastTracking = res.data.List;
						res.data.List.map((item, index) => {
							item.FollowWay = this.ExecutionMode.get(item.FollowWay)
						})
					}
				})
			},
			handEdit() {
				uni.navigateTo({
					url: './additionOpportunity?id=' + this.arr1.Id
				})
			},
			list(id) {
				BusineDataDetails({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						this.DetailList = []
						var infoData = res.data.DetailList
						this.DetailList = infoData.map((row, index) => {
							// console.log("商机明细",row);
							return row.ProductInfo
						})

						this.arr1 = res.data
						this.listData(res.data.CustomerId);

						//console.log(res,'res')
						this.contentName = res.data.ContactUser.RealName
						//销售阶段
						periodList().then((res1) => {
							//console.log(res1,'销售阶段');
							this.RequirementStage = res1.data;
							var data = res1.data;
							data.map((row, inx) => {
								this.xiaoShou.set(parseInt(row.Id), row);
								if (row.Id == res.data.Period) {
									//console.log(row,inx,'row1111')
									this.selected = inx
								}
							})
							if (this.xiaoShou.get(parseInt(res.data.Period))) {
								this.ValidateText = this.xiaoShou.get(parseInt(res.data.Period)).PeriodName
							}
							// this.selected=1
							this.PeriodId = res.data.Period
							//console.log(res.data.Period,this.PeriodId,res,'this.selected')
						})



					}
				})
			},
			handClickTo(inx) {
				this.select = inx
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
	.t-icon-dianhua1 {
		width: 50rpx;
		height: 50rpx;
		color: #E9F1FF;
		text-align: right;
		margin-left: auto;
		margin-right: 20rpx;
		font-size: 50rpx;
	}

	.productList {
		position: relative;
		margin: 0rpx 3%;
		background: #fff;
		height: 126rpx;
		border-radius: 8rpx;
		display: flex;
		align-items: center;
		color: #333;
		padding: 0rpx 25rpx;
		margin-top: 30rpx;

		.productList-content {
			.title {
				font-size: 28rpx;
				font-weight: bold;
			}

			.content {
				font-size: 24rpx;
				color: #999999;
			}
		}

		.Mechines {
			margin-left: auto;
			color: #999999;
			font-size: 24rpx;
		}

		.t-icon-yichu {
			position: absolute;
			right: -15rpx;
			top: -20rpx;
			width: 50rpx;
			height: 50rpx;
		}

		.productList-logo {
			width: 65rpx;
			height: 65rpx;
			margin-right: 30rpx;
		}
	}

	.prodit {
		color: #333;
		display: flex;
		flex-wrap: wrap;
		justify-content: left;
		padding: 0rpx 3%;
		text-align: center;
		background: #fff;
		padding-left: 45rpx;

		.four-saleImg {
			width: 91rpx;
			color: #fff;
			// background: url('../../static/images/four-saleImg.png') no-repeat;
			background-repeat: no-repeat;
			background-size: 100% 100%;
		}

		.Rectangular {
			width: 171rpx;
			color: #fff;
			// background: url('../../static/images/Rectangular.png') no-repeat;
			background-repeat: no-repeat;
			background-size: 100% 100%;
		}

		.two-saleImg {
			width: 91rpx;
			color: #fff;
			// background: url('../../static/images/two-saleImg.png') no-repeat;
			background-repeat: no-repeat;
			background-size: 100% 100%;
		}

		.saleBgImg {
			width: 171rpx;
			color: #999999;
			// background: url('../../static/images/saleBgImg.png') no-repeat;
			background-repeat: no-repeat;
			background-size: 100% 100%;

		}

		.one-saleImg {
			width: 141rpx;
			color: #fff;
			// background: url('../../static/images/one-saleImg.png') no-repeat;
			background-repeat: no-repeat;
			background-size: 100% 100%;
		}

		.prodit-item {
			height: 60rpx;
			line-height: 60rpx;
			border-radius: 10rpx;
			margin: 10rpx auto;
			font-size: 24rpx;
			color: #999999;
			margin-left: -16rpx;
		}

		.active {
			width: 141rpx;
			color: #fff;
			// background: url('../../static/images/saleBgZhong.png') no-repeat;
			background-size: 100% 100%;
		}

		.on {
			width: 140rpx;
			background: #f8f8f8;
			background-size: 100% 100%;
			color: #999999;
			margin-right: 194rpx;
		}

		.singleOn {
			width: 140rpx;
			color: rgba(255, 53, 53, 1);
			background: rgba(255, 245, 245, 1);
			margin-left: 194rpx;
		}

		.five-saleImg {
			width: 140rpx;
			background: #2371FF;
			background-size: 100% 100%;
			color: #fff;
			border-radius: 10rpx;
		}
	}

	.addPool-button {
		width: 200rpx;
		height: 70rpx;
		line-height: 70rpx;
		text-align: center;
		border-radius: 10rpx;
		color: #fff;
		background: linear-gradient(180deg, #ff3535, #ff613d);
	}

	.addPool-textarea {
		height: 290rpx;
		border: 1rpx solid rgba(255, 255, 255, .2);
		border-radius: 10rpx;
		margin: 20rpx auto;
		padding: 20rpx;
		background: #f8f8f8;
	}

	.copyInvite {
		.copyInvite-title {
			margin: 20rpx 0px;
		}

		.validityPeriod {
			border-radius: 10rpx;
			border: 1rpx solid rgba(255, 255, 255, .2);
			height: 90rpx;
			line-height: 90rpx;
			margin-top: 20rpx;
			padding-left: 20rpx;
		}

		.copyInvite-lianUrl {
			display: flex;
			align-items: center;

			.copyInvite-lianUrl-url {
				border: 1rpx solid rgba(255, 255, 255, .2);
				border-radius: 6rpx;
				padding: 10px 20px;
				line-height: 30rpx;
			}

			.copuUrl {
				border-radius: 6rpx;
				background: linear-gradient(180deg, #ff3535, #ff613d);
				padding: 10px 10px;
				margin-left: 25rpx;
				font-size: 24rpx;
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
		color: #333333;
		width: 90%;
		margin: 0px 5%;
		border-radius: 8rpx;
		top: 250rpx;

		.addPool-selected {
			::v-deep.uni-select__input-text {
				color: #fff !important;
			}

			::v-deep.uni-select {
				height: 88rpx !important;
				border: 1rpx solid rgba(255, 255, 255, .2);
			}

			::v-deep.uni-select__input-placeholder {
				font-size: 30rpx !important;
				color: rgba(255, 255, 255, .4) !important;
			}
		}

		.link {
			color: rgba(255, 255, 255, .4);
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
			margin-top: 20px;
			background: linear-gradient(180deg, #ff3535, #ff613d);
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
			}

			.icon-guanbidanchuang {
				position: absolute;
				margin-left: auto;
				width: 40rpx;
				height: 40rpx;
				right: 20rpx;
				color: #333;
				top: 5rpx;
			}
		}

		.AddPopUps-item {
			padding: 30rpx;

		}
	}


	#CreateCustomer {
		.Comments-item {
			display: flex;
			margin: 0rpx 3%;
			padding: 20rpx 30rpx;
			margin-bottom: 20rpx;
			background: #ffffff;
			border-radius: 14rpx;
			color: #333;

			.Comments-item-left {
				display: flex;

				.aviter {
					width: 60rpx;
					height: 60rpx;
					border-radius: 50%;
					margin-right: 20rpx;
				}
			}

			.Comments-item-txt {
				margin-left: auto;

				.Comments-item-parse {
					display: flex;
					font-size: 24rpx;
					color: #999;
					margin-top: 15rpx;

					.reply {
						margin-left: auto;
					}
				}

				.Comments-item-txt-content {
					font-size: 29rpx;
					color: #333;
				}

				.Comments-item-txt-title {
					color: #999;
					font-size: 28rpx;
				}
			}
		}

		.Follow-up-item {
			margin: 0rpx 3%;
			padding: 20rpx 30rpx;
			margin-bottom: 20rpx;
			background: #ffffff;
			border-radius: 14rpx;
			color: #333;

			.Follow-up-item-bottom {
				padding-top: 15rpx;
				font-size: 26rpx;
				color: #999999;

				.tit_text {
					margin-right: 10rpx;
				}
			}

			.Follow-up-item-center {
				font-size: 30rpx;
			}

			.Follow-up-item-top {
				display: flex;
				align-items: center;

				.Follow-up-item-top-content {
					.time {
						color: #999999;
						font-size: 24rpx;
					}
				}

				.aviter {
					float: left;
					width: 60rpx;
					height: 60rpx;
					border-radius: 50%;
					margin-right: 20rpx;
				}
			}
		}

		.Follow-up-input {
			display: flex;
			align-items: center;
			height: 88rpx;
			border: 1rpx solid rgba(255, 255, 255, .1);
			margin: 20rpx 20rpx;
			border-radius: 8rpx;
			background: #ffffff;

			.icon-tianxiegenjinhepinglun {
				color: #999;
				margin-left: 30rpx;
			}

			.searchInput {
				font-size: 32rpx;
				margin-left: 14rpx;
				color: #C1C1C1;
			}

			::-webkit-input-placeholder {}
		}

		.BottomPositioning {
			text-align: center;
			color: #999999;
			background: #ffffff;
			height: 100rpx;
			line-height: 100rpx;
			margin-top: 20rpx;
			font-size: 32rpx;
			margin: 40rpx 3%;
			border-radius: 7rpx;
		}

		.Information-item {
			margin: 0rpx 3%;
			background: #ffffff;
			color: #333;
			padding: 0rpx 20rpx;
			border-radius: 10rpx;

			.Information {
				display: flex;
				padding: 30rpx 0rpx;
				flex-direction: column;
				margin-top: 20rpx;
				border-bottom: 1rpx solid #f8f8f8;

				.Information-title {
					color: #999999;
				}

				.Information-bottom {
					display: flex;
					align-items: center;
					font-size: 32rpx;
					margin-top: 10rpx;
				}

				.aviter {
					float: left;
					width: 60rpx;
					height: 60rpx;
					border-radius: 50%;
					margin-right: 20rpx;
				}
			}

			.InformBorder {
				border: none;
			}
		}

		.navigation-nav {
			display: flex;
			color: #222;
			background: #fff;
			font-weight: bold;
			padding-top: 30rpx;

			.navigation-nav-border {
				width: 30rpx;
				height: 5rpx;
				border-radius: 14rpx;
				background: #2371ff;
				margin: 0 auto;
				margin-top: 10rpx;
			}

			.navigation-nav-item {
				width: 33%;
				text-align: center;
				font-size: 28rpx;
			}

			.active {}

			.on {
				font-weight: normal;
				color: #999999;
			}
		}

		.CreateCustomer-header {
			.Avatar-information {
				position: relative;
				display: flex;
				flex-direction: column;
				color: #333;
				background: #ffffff;
				text-align: center;
				padding: 30rpx 0rpx;
				// margin: 0rpx 3%;
				padding-bottom: 20rpx;

				// margin-top: 80rpx;
				.t-icon-shangjitouxiang1 {
					width: 140rpx;
					height: 140rpx;
					line-height: 140rpx;
					margin: 0 auto;
					// margin-top: -90rpx;
					color: #1C2232;
					border-radius: 10rpx;
				}

				.Avatar-Contacts {
					display: flex;
					align-items: center;
					width: 92%;
					// height: 160rpx;
					// line-height: 160rpx;
					margin: 10rpx 4%;
					padding-top: 2rpx;
					background: #F8F8F8;
					padding-bottom: 10rpx;
					border: 2rpx solid rgba(255, 255, 255, .2);
					border-radius: 8rpx;
					margin-top: 30rpx;

					.Avatar-Contacts-right {
						display: flex;
						justify-content: space-between;
						padding: 20rpx 20rpx;
						align-items: center;

						// margin:15rpx 0rpx;
						.t-icon-dianhua {
							width: 50rpx;
							height: 50rpx;
							line-height: 50rpx;
							text-align: right;
							font-size: 52rpx;
							margin-left: auto;
							color: red;
						}

						.Avatar-Contacts-right-David {
							.content {
								height: 60rpx;
								line-height: 60rpx;
								margin-left: 15rpx;
							}

							.aviter {
								float: left;
								width: 60rpx;
								height: 60rpx;
								border-radius: 50%;
							}
						}
					}

					.Avatar-Contacts-left {
						display: flex;
						justify-content: space-between;
						padding: 0rpx 20rpx;
						margin: 5rpx 0rpx;
						color: #999999;

						.Avatar-Contacts-iconfont {}

						.icon-a-youjiantoubai {
							font-size: 20rpx;
						}
					}
				}

				.Agent {
					position: absolute;
					right: 0rpx;
					top: 0rpx;
					// width:100rpx;
					padding: 0rpx 10rpx;
					text-align: center;
					font-size: 23rpx;
					border-top-right-radius: 8rpx;
					border-bottom-left-radius: 15rpx;
					background: linear-gradient(180deg, #EFA902, #F4BE3F);
				}

				.Avatar-information-title {
					margin-top: 35rpx;
					font-size: 32rpx;
				}

				.Avatar-information-content {
					color: #999999;
					font-size: 28rpx;
					margin-top: 5rpx;
				}


			}
		}

	}

	.Comments-item-leftparse {
		display: flex;
	}

	.Comments-item {
		display: flex;
		margin: 0rpx 3%;
		padding: 20rpx 30rpx;
		margin-bottom: 20rpx;
		background: #1C2232;
		border-radius: 14rpx;
		color: #fff;
		flex-direction: column;

		.Comments-item-left {
			display: flex;

			.aviter {
				width: 60rpx;
				height: 60rpx;
				border-radius: 50%;
				margin-right: 20rpx;
			}
		}

		.Comments-item-txt {
			// margin-left: auto;
			width: 100%;

			.Comments-item-parse {
				display: flex;
				font-size: 24rpx;
				color: rgba(255, 255, 255, .6);
				margin-top: 15rpx;
				width: 100%;

				.reply {
					margin-left: auto;
				}
			}

			.Comments-item-txt-content {
				font-size: 29rpx;
			}

			.Comments-item-txt-title {
				color: rgba(255, 255, 255, .6);
				font-size: 26rpx;
			}
		}
	}
</style>