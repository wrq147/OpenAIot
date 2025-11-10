<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Details" rightText="Edit"
			class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handEdit()">
					<view style="font-size:32rpx;">Edit</view>
				</view>
			</template>
			</top>
		<view class="CreateCustomer-header">
			<view class="Avatar-information">
				<view class="iconfont t-icon-shangjitouxiang">

				</view>
				<view class="Avatar-information-title">
					{{arr1.OpportName}}
				</view>
				<view class="Avatar-information-content">
					NO.{{arr1.OpportNumber}}
				</view>
				
				<view class="Avatar-Contacts" >
					<view class="Avatar-Contacts-left">
						<text>Contacts: </text>
					</view>
					<view class="Avatar-Contacts-right">
						<view class="Avatar-Contacts-right-David">
							<text class="aviter iconfont t-icon-morentouxiang"></text>
							<text class="content">{{contentName}}</text>
						</view>
						<!-- <view class="iconfont t-icon-dianhua" @click="TelPhone()">

						</view> -->
					</view>
				</view>
			</view>
		</view>
		<!--需求阶段-->
		<view  class="prodit">
			<!-- :class="[selected==item.PeriodName?'active':'on']" index==1||index==2||index==3?'saleBgImg':index==4?'two-saleImg':'on'-->
			<view :class="[index==0?'one-saleImg':index==1||index==2||index==3?'saleBgImg':index==4?'two-saleImg':index==5?'singleOn':'on']" class="prodit-item" v-for="(item,index) in RequirementStage" :key="index" @click="handProditItem(item,index)">
				<view  :class="[selected==0&&PeriodId==item.Id?'active':selected==1&&PeriodId==item.Id||selected==2&&PeriodId==item.Id||selected==3&&PeriodId==item.Id?'Rectangular':
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
				<view class="Information InformBorder">
					<view class="Information-title">
						Sale stage
					</view>
					<view class="Information-bottom">
						<text>{{ValidateText}}</text>
					</view>
				</view>
				<view class="Information">
					<view class="Information-title">
						Win rate
					</view>
					<view class="Information-bottom">
						<text>{{arr1.Probability}}</text>
					</view>
				</view>
				<view class="Information">
					<view class="Information-title">
						Manager
					</view>
					<view class="Information-bottom" v-if="arr1.LeaderUser">
						<img :src="arr1.LeaderUser.Avatar" class="aviter" alt=""/>
						<text>{{arr1.LeaderUser.RealName}}</text>
					</view>
				</view>


				<view class="Information ">
					<view class="Information-title">
						Collaborator
					</view>
					<view class="Information-bottom" v-for="(ite,inx) in arr1.HelperUsers" :key="inx">
						<img class="aviter" :src="ite.Avatar" alt="">
						<text>{{ite.RealName}}</text>
					</view>
				</view>
				<view class="Information">
					<view class="Information-title">
						Opportunity details
					</view>
					<view class="Information-bottom">
						<text>{{arr1.Remark}}</text>
					</view>
				</view>
				<view class="Information">
					<view class="Information-title">
						Creation date
					</view>
					<view class="Information-bottom">
						<text>{{arr1.createTime}}</text>
					</view>
				</view>
			</view>
		</view>
		<view v-if="select==1">
		<view class="productList" v-for="(item,index) in DetailList" :key="index" >
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
			<!-- <view class="iconfont t-icon-yichu">
				
			</view> -->
		</view>

		
		</view>

		<view v-if="select==2">
		<view class="Follow-up-input" @click="handAddFloow">
			<text class="iconfont icon-tianxiegenjinhepinglun"></text>
			<input class="searchInput" type="text" placeholder="Fill in follow-up updates">
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
					<text class="tit_text">Execution mode:</text>
					<text>{{item.FollowWay}}</text>
				</view>
			</view>
			<view style="height:50px;"></view>
		</view>
		<view v-if="select==3">
			<view class="Follow-up-input" @click="handComment">
				<text class="iconfont icon-tianxiegenjinhepinglun"></text>
				<input class="searchInput" type="text" placeholder="Fill in Comments">
			</view>
			<view class="Comments-item" v-for="(item,index) in payList" :key="index" @click="handPayListDetails(item)" style="margin-bottom: 20rpx;">
				<view class="Comments-item-leftparse">
					<view class="Comments-item-left" style="margin-top:15rpx;">
						<img class="aviter" :src="item.UserInfo.Avatar" alt="">
					</view>
					<view class="Comments-item-txt">
						<view class="Comments-item-txt-title" >{{item.UserInfo.RealName}}</view>
						<view class="Comments-item-txt-content" v-html="item.Content">
			
						</view>
						<view class="Comments-item-parse">
							<view>{{item.CreateOn}}</view>
							<view class="reply">
								<text class="iconfont icon-huifu"></text>
								<text>Reply</text>
							</view>
						</view>
					</view>
				</view>
				<view class="Comments-item" v-for="(row,index1) in item.children"
					style="padding-right:0rpx;margin-right:0rpx;padding-bottom: 0rpx;">
					<view class="Comments-item-leftparse">
						<view class="Comments-item-left" style="margin-top:15rpx;margin-left:32rpx;">
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
									<text>Reply</text>
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
						comment
					</view>
					<view class="iconfont t-icon-guanbidanchuang" @click="handClose">
		
					</view>
				</view>
				<view>
					<textarea v-model="content" placeholder="Please enter a comment" class="addPool-textarea"
						:rows="4" />
					<view class="addPool-button" @click="handCommentSubmit">
						confirm
					</view>
				</view>
			</view>
		</view>
		<view class="BottomPositioning" @click="handDelete" v-if="select==0">
			Delete
		</view>
	</view>
</template>

<script>
	import {
		BusineDataDetails,//商机详情
		flowDataRecord, //快速跟进详情
		commentList, //评论列表
		commentAdd ,//评论添加
		contactData,//联系人
		periodList,//需求阶段
		phaseSwitching,//切换销售阶段
		oppartRemove//删除
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
				DetailList:[],
				arr1:[],
				select: 0,
				arr: [{
						title: 'Information'
					},
					{
							title: 'items'
						},
					{
						title: 'Follow-up'
					},
					{
						title: 'Comments'
					}
				],
				FastTracking: [],
				ExecutionMode: new Map(),
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
				EditId:'',
				contentName:'',
				RequirementStage:[],//需求阶段
				selected:'',
				xiaoShou:new Map(),//销售阶段
				PeriodId:0,
				ValidateText:''
			}
		},
	 async onLoad(option){
			if(option.id){
				this.EditId=option.id
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
			TelPhone(){
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
			confireDelete(){
				//删除
				oppartRemove({id:this.EditId}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'Delete successful！',
							icon:'none'
						})
						setTimeout(()=>{
							setPagesParam('list')
						},700)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handDelete(){
				//删除
				this.$refs.promptDelete.noticeOpen(
					"Are you sure to delete the customers?"
				)
			},
			confireCloseDrawer(){
				phaseSwitching({
					id:this.EditId,
					target:this.PeriodId
				}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'Switching successful!',
							icon:'none'
						})
						
						this.list(this.EditId);
					}
				})
			},
			handProditItem(item,inx){
		if(this.ValidateText!=item.PeriodName){
			this.$refs.promptReceive.noticeOpen(
				"确认更改名为"+item.PeriodName+"的销售阶段！"
			)
			this.selected=inx;
			this.PeriodId=item.Id
		}
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
						title: 'Please enter the comment content！',
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
							title: 'Comment added successfully!',
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
					// /console.log(res,'快速跟进')
					if (res.code == 0) {
						this.FastTracking = res.data.List;
						res.data.List.map((item,index)=>{
							item.FollowWay=this.ExecutionMode.get(item.FollowWay)
						})
					}
				})
			},
			handEdit(){
				uni.navigateTo({
					url:'./additionOpportunity?id='+this.arr1.Id
				})
			},
			list(id){
				BusineDataDetails({
				id:id
				}).then((res)=>{
					if(res.code==0){
						//console.log(res,'商机详情')
					this.DetailList=[]
					var infoData=res.data.DetailList
					infoData.forEach((row,index)=>{
						let obj3={}
						if(row.PartInfo){
							obj3={
								DeviceNumber: row.PartInfo.DeviceNumber,
								Name: row.PartInfo.Name,
								PhotoUrl: row.PartInfo.PhotoUrl,
								//  Quantity: 1,
								TargetId: row.PartInfo.Id,
								TargetType: 0,
								Unit: row.PartInfo.Unit,
							}
						}
						
						if(row.DeviceInfo){
							obj3={
								DeviceNumber: row.DeviceInfo.DeviceNumber,
								Name: row.DeviceInfo.Name,
								PhotoUrl: row.DeviceInfo.PhotoUrl,
								//  Quantity: 1,
								TargetId: row.DeviceInfo.Id,
								TargetType: 1,
								Unit: row.DeviceInfo.Unit,
							}
						}
						this.DetailList.push(obj3)
					})
					//	console.log(this.selected,'this.selected')
						this.arr1=res.data
						this.listData(res.data.CustomerId);
						this.contentName=res.data.ContactUser.RealName
					
						//销售阶段
							periodList().then((res1)=>{
								//console.log(res1,'销售阶段');
								this.RequirementStage=res1.data;
								var data=res1.data;
								data.map((row,inx)=>{
									this.xiaoShou.set(parseInt(row.Id), row);
									if(row.Id==res.data.Period){
										//console.log(row,inx,'row1111')
										this.selected=inx
									}
								})
								if(this.xiaoShou.get(parseInt(res.data.Period))){
									this.ValidateText=this.xiaoShou.get(parseInt(res.data.Period)).PeriodName
								}
								// this.selected=1
								this.PeriodId=res.data.Period
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

<style lang="less" scoped>
	page {
		background: #161A26;
		.productList{
			position: relative;
			margin:0rpx 3%;
			background: rgba(28, 34, 50, 1);
			height: 126rpx;
			border-radius:8rpx;
			display: flex;
			align-items: center;
			color:#fff;
			padding:0rpx 25rpx;
			margin-top:30rpx;
			.productList-content{
				.title{
					font-size:28rpx;
				}
				.content{
					font-size:24rpx;
					color:rgba(255, 255, 255, .3);
				}
			}
			.Mechines{
				margin-left:auto;
				color:rgba(255, 255, 255, .3);
				font-size:24rpx;
			}
			.t-icon-yichu{
				position: absolute;
				right:-15rpx;
				top:-20rpx;
				width: 50rpx;
				height:50rpx;
			}
			.productList-logo{
				width:65rpx;
				height:65rpx;
				margin-right:30rpx;
			}
		}
		.prodit{
			color:#fff;
			display: flex;
			margin:0rpx 3%;
			flex-wrap: wrap;
			justify-content: center;
			margin-top: 20rpx;
			text-align:center;
			margin-left:38rpx;
			.four-saleImg{
				width: 91rpx;
				color:#fff;
				background: url('../../static/images/four-saleImg.png') no-repeat;
				background-size:100% 100%;
			}
			.Rectangular{
				width: 171rpx;
				color:#fff;
				background: url('../../static/images/Rectangular.png') no-repeat;
				background-size:100% 100%;
			}
			.two-saleImg{
				width: 91rpx;
				color:#fff;
				background: url('../../static/images/two-saleImg.png') no-repeat;
				background-size:100% 100%;
			}
			.saleBgImg{
				width: 171rpx;
				color:#fff;
				background: url('../../static/images/saleBgImg.png') no-repeat;
				background-size:100% 100%;
				
			}
			.one-saleImg{
				width: 141rpx;
				color:#fff;
				background: url('../../static/images/one-saleImg.png') no-repeat;
				background-size:100% 100%;
			}
			.prodit-item{
				height:60rpx;
				line-height:60rpx;
				border-radius: 10rpx;
				margin:10rpx auto;
				font-size:24rpx;
				color:rgba(255, 255, 255, .3);
				margin-left:-16rpx;
			}
			.active{
				width: 141rpx;
				color:#fff;
				background: url('../../static/images/saleBgZhong.png') no-repeat;
				background-size:100% 100%;
			}
			
			.on{
				width: 140rpx;
				background: #1C2232;
				background-size:100% 100%;
				color:#999999;
				margin-right:194rpx;
			}
			.singleOn{
				width: 140rpx;
				color:rgba(255, 53, 53, 1);
				background:#1C2232;
				margin-left:210rpx;
			}
			.five-saleImg{
				width: 140rpx;
				background: linear-gradient(180deg, #ff3535, #ff613d);
				background-size:100% 100%;
				color:#fff;
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
			background: #161A26;
			z-index: 333;
			color: #fff;
			width: 90%;
			margin: 0px 5%;
			border-radius: 8rpx;
			top: 250rpx;
		
			.addPool-selected {
				/deep/.uni-select__input-text {
					color: #fff !important;
				}
		
				/deep/.uni-select {
					height: 88rpx !important;
					border: 1rpx solid rgba(255, 255, 255, .2);
				}
		
				/deep/.uni-select__input-placeholder {
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
		
				.t-icon-guanbidanchuang {
					position: absolute;
					margin-left: auto;
					width: 40rpx;
					height: 40rpx;
					right: 20rpx;
					top: 5rpx;
				}
			}
		
			.AddPopUps-item {
				padding: 30rpx;
		
			}
		}
	}

	#CreateCustomer {
		.Comments-item {
			display: flex;
			margin: 0rpx 3%;
			padding: 20rpx 30rpx;
			background: #1C2232;
			border-radius: 14rpx;
			color: #fff;

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
					color: rgba(255, 255, 255, .6);
					margin-top: 15rpx;

					.reply {
						margin-left: auto;
					}
				}

				.Comments-item-txt-content {
					font-size: 32rpx;
					margin-top:10rpx;
				}

				.Comments-item-txt-title {
					color: rgba(255, 255, 255, .6);
					font-size: 28rpx;
				}
			}
		}

		.Follow-up-item {
			margin: 0rpx 3%;
			padding: 20rpx 30rpx;
			margin-bottom: 20rpx;
			background: #1C2232;
			border-radius: 14rpx;
			color: #fff;

			.Follow-up-item-bottom {
				padding-top: 15rpx;
				font-size: 28rpx;

				.tit_text {
					color: rgba(255, 255, 255, .4);
					margin-right: 10rpx;
				}
			}

			.Follow-up-item-center {
				font-size:32rpx;
				margin-top:10rpx;
			}

			.Follow-up-item-top {
				display: flex;
				align-items: center;

				.Follow-up-item-top-content {
					font-size: 28rpx;
					.time {
						color: rgba(255, 255, 255, .5);
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

			.icon-tianxiegenjinhepinglun {
				color: rgba(255, 255, 255, .4);
				margin-left: 30rpx;
			}

			.searchInput {
				font-size: 28rpx;
				margin-left: 14rpx;
				color: #fff;
			}

			::-webkit-input-placeholder {}
		}

		.BottomPositioning {
			text-align: center;
			color:rgba(255, 255, 255, .5);
			background: #1C2232;
			height:100rpx;
			line-height: 100rpx;
			margin-top:20rpx;
			font-size:36rpx;
			margin:40rpx 3%;
			border-radius: 7rpx;
		}

		.Information-item {
			margin: 20rpx 3%;
			background: #1C2232;
			color: #fff;
			padding: 0rpx 20rpx;
			
			.Information {
				display: flex;
				padding: 30rpx 0rpx;
				flex-direction: column;
				border-top: 1rpx solid rgba(255, 255, 255, .1);

				.Information-title {
					color: rgba(255, 255, 255, .6);
					font-size:28rpx;
				}

				.Information-bottom {
					display: flex;
					align-items: center;
					margin-top: 10rpx;
					font-size:32rpx;
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
			color: #fff;
			margin-top: 30rpx;

			.navigation-nav-border {
				width: 30rpx;
				height: 5rpx;
				border-radius: 14rpx;
				background: #fff;
				margin: 0 auto;
				margin-top: 10rpx;
			}

			.navigation-nav-item {
				width: 33%;
				text-align: center;
				font-size:28rpx;
			}

			.active {}

			.on {
				color: rgba(255, 255, 255, .4);
			}
		}

		.CreateCustomer-header {
			.Avatar-information {
				position: relative;
				display: flex;
				flex-direction: column;
				color: #fff;
				background: #1C2232;
				text-align: center;
				padding: 30rpx 0rpx;
				margin: 0rpx 3%;
				padding-bottom: 20rpx;
				margin-top: 80rpx;
				.t-icon-shangjitouxiang {
					width: 140rpx;
					height: 140rpx;
					line-height: 140rpx;
					margin: 0 auto;
					margin-top: -90rpx;
					color: #1C2232;
					border-radius: 10rpx;
				}
				.Avatar-Contacts {
					display: flex;
					flex-direction: column;
					width: 92%;
					// height: 160rpx;
					// line-height: 160rpx;
					margin: 10rpx 4%;
					padding-top: 15rpx;
					padding-bottom: 10rpx;
					border: 2rpx solid rgba(255, 255, 255, .2);
					border-radius: 8rpx;

					.Avatar-Contacts-right {
						display: flex;
						justify-content: space-between;
						padding: 20rpx 20rpx;
						align-items: center;
						font-size:32rpx;
						// margin:15rpx 0rpx;
						.t-icon-dianhua {
							width: 50rpx;
							height: 50rpx;
							line-height: 50rpx;
							text-align: right;
							font-size: 52rpx;
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
						font-size:28rpx;
						color: rgba(255, 255, 255, .6);

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
					font-size: 36rpx;
				}

				.Avatar-information-content {
					color: rgba(255, 255, 255, .6);
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
					font-size: 32rpx;
					margin-top:10rpx;
				}

				.Comments-item-txt-title {
					color: rgba(255, 255, 255, .6);
					font-size: 28rpx;
				}
			}
		}

</style>


