<template>
	<view class="pages_con pages_bgcon">
		<!--#ifdef H5 -->
		<jump-down></jump-down>
		<!--#endif -->
		<top-weather :hasHandleIcon="false" ref="topWeather" :userName="userInfo.RealName" :statusBarHeight="statusBarHeight"
			:isShowFixedTitle="isShowFixedTitle" :defaultTop="defaultTop" @openLeftPopup="openLeftPopup">
			<template v-slot:default>
				{{userInfo.dept_name}}
			</template>
		</top-weather>
		<view class="word_con" id="home_liscon">
			<view class="work_order_con">
				<view class="creat_work" @click="toStartTask">
					<image v-if="getSerVerUrl()" class="image" :src="getSerVerUrl()+'/appimg/creadwork.png'" mode=""></image>
					<view class="text_title">新建工单</view>
					<view class="text_cont">计划任务，一键添加</view>
				</view>
				<view class="scan" @click="setScan">
					<image v-if="getSerVerUrl()" class="image" :src="getSerVerUrl()+'/appimg/workscan.png'" mode=""></image>
					<view class="text_title">扫一扫</view>
					<view class="text_cont">查看设备详情</view>
				</view>
			</view>
			<view class="serve_con" v-if="serveList&&serveList.length>0">
				<view class="serve_title">我发布的计划</view>
				<view class="serve_li" v-for="item in serveList" @click="jumpTaskList(item)">
					<view class="left_con">
						<view class="icon_con" :style="{'background':item.Background}">
							<custom-icons :iconsName="item.Icon?iconSubStr(item.Icon):''" iconsSize="40rpx"
								iconsColor="#ffffff"></custom-icons>
						</view>
						<view class="temple_name">
							<view class="name">{{item.FlowTemplateName}}</view>
							<view class="type">{{item.Name}}</view>
						</view>
					</view>
					<custom-icons iconsName="icon-a-youjiantouhong" iconsSize="20rpx"
						iconsColor="#999999"></custom-icons>
				</view>
			</view>
		</view>
		<my-tab-bar active="home" ref="mytab"></my-tab-bar>
		<uni-popup ref="bottomPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list_con">
				<view class="title_con">
					<view class="kong">取消</view>
					<view class="title">发起任务</view>
					<view class="cancel" @click.stop="closePopup">取消</view>
				</view>
				<view class="popup_list">
					<scroll-view :scroll-top="scrollTop" scroll-y="true" class="scroll-Y">
						<view class="popup_li borradio" v-for="item in serveList" @click.stop="startTask(item)">
							<view class="icon_con" :style="{'background':item.Background}">
								<custom-icons :iconsName="item.Icon?iconSubStr(item.Icon):''" iconsSize="40rpx"
									iconsColor="#ffffff"></custom-icons>
							</view>
							<view class="temple_name">
								<view class="name">{{item.FlowTemplateName}}</view>
								<view class="type">{{item.Name}}</view>
							</view>
						</view>
					</scroll-view>
				</view>
			</view>
		</uni-popup>
		<!-- 切换企业的弹窗 -->
		<org-change ref="notificationcmt" jumpUrl="/pages/index/index2" :orgList="orgList" :orgId="userInfo.OrgId" :userInfo="userInfo"></org-change>
	</view>
</template>

<script>
	import {
		dataList, //个人中心用户信息
	} from "@/api/personalCenter";
	// #ifdef H5
	import {
		scan
	} from '@/common/wxCommon.js'
	// #endif
	import {devPlaneList} from '@/api/devplane.js'
	export default {//工单类型的首页，宏港使用
		components: {
		},
		data() {
			return {
				scrollTop:0,
				serveList: ['巡检', '保养', '维修'],
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight,
				isShowFixedTitle: false,
				defaultTop: 0,
				userInfo: {},
				orgList: [],
				planeQuery:{
					pageNum:1,
					pageSize:0,
					CanStart:true,
					StartWay:0//过滤手动
				},
				status:'loading',
			};
		},
		async onLoad() {
			if (this.$store.state.user && this.$store.state.user.uid) {} else {
				await this.$store.dispatch('GetInfo')
			}
			this.dataInfo()
			this.$store.commit('orgLis/SET_ORG_LIST', null)
			this.orgList = await this.$store.dispatch("orgLis/setOrgList");
			this.$nextTick(() => {
				this.$refs.mytab.loadCheck()
				let query = uni.createSelectorQuery().in(this);
				query.select('#home_liscon').boundingClientRect(data => {
					if (data) {
						this.defaultTop = data.top
					}
				}).exec()
			})
			await this.getdevPlaneList()
		},
		onPageScroll() {
			this.getElementTop('')
		},
		methods: {
			toStartTask() {
				this.$refs.bottomPop.open()
			},
			closePopup(){
				this.$refs.bottomPop.close()
			},
			startTask(row){
				//开启任务
				this.$refs.bottomPop.close()
				if (row.StartWay == 0) {
					uni.navigateTo({
						url: '/pages_flow/device/task_add',
						success: (res) => {
							// 通过eventChannel向被打开页面传送数据
							let jumpform={
								id: row.Id,
							}
							res.eventChannel.emit('planeTaskForm',jumpform)
						}
					})
				}
			},
			jumpTaskList(item){
				uni.navigateTo({
					url:'/pages_device/device_info/plane_task',
					success: (res)=>{
						// 通过eventChannel向被打开页面传送数据
						res.eventChannel.emit('planeTaskForm', {
							id: item.Id,
							planeName:item.Name,
							startWay:item.StartWay
						})
					}
				})
			},
			async getdevPlaneList(){
				//获取设备计划类型
				if(this.planeQuery.pageNum==1){
					this.serveList=[]
				}
				try{
					let res=await devPlaneList(this.planeQuery)
					this.serveList=[...this.serveList,...res.data.List]
					if (res.data.List.length < this.planeQuery.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				}catch(e){
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			getElementTop() {
				//获取距离顶部的距离
				let minScrollhei = 10
				let query = uni.createSelectorQuery().in(this);
				query.select('#home_liscon').boundingClientRect(data => {
					if (data) {
						if (data.top <= this.statusBarHeight + minScrollhei) {
							this.isShowFixedTitle = true
						} else {
							this.isShowFixedTitle = false
						}
						this.$refs.topWeather.setDomOpac(data)
					}
				}).exec();
			},
			getTargetUrlParam(url, name) { //取链接上的值
				url = url.substr(url.indexOf("?"));
				var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
				var r = url.substr(1).match(reg);
				if (r != null) return unescape(r[2]);
				return null;
			},
			setScan() {
				console.log("扫一扫");
				// #ifndef H5
				uni.scanCode({
					success: (res) => {
						console.log("扫码结果",res);
						let id=this.getTargetUrlParam(res,'id')
						if(this.getTargetUrlParam(res,'Id')){
							id=this.getTargetUrlParam(res,'Id')
						}
						if(id){
							uni.navigateTo({
								url:'/pages_device/device_info/device_info?id='+id
							})
						}else{
							uni.showToast({
								title:'二维码无效'
							})
						}
						
					}
				});
				// #endif
				// #ifdef H5 
				scan('out', (currenturl) => {
					if (currenturl) {
						let id=this.getTargetUrlParam(currenturl,'id')
						if(this.getTargetUrlParam(currenturl,'Id')){
							id=this.getTargetUrlParam(currenturl,'Id')
						}
						if(id){
							uni.navigateTo({
								url:'/pages_device/device_info/device_info?id='+id
							})
						}else{
							uni.showToast({
								title:'二维码无效'
							})
						}
					}
			
				})
				// #endif  
			},
			openLeftPopup() {
				//打开左侧切换企业弹窗
				this.$refs.notificationcmt.openLeftPopup()
			},
			dataInfo() {
				//个人信息
				dataList().then((res) => {
					if (res.code == 0) {
						//console.log(res,'个人信息')
						this.userInfo = res.data.user
						this.userInfo.name = this.userInfo.RealName
						this.userInfo.avatar = this.userInfo.Avatar
						//console.log(res.data.user.OrgId,'res.data.user.OrgId')
					}
				})
			},
		}
	}
</script>

<style lang="less">
	.popup_list_con {
		background-color: #ffffff;
		padding-top: 30rpx;
		padding: 30rpx 30rpx 20rpx;
	
		.title_con {
			display: flex;
			justify-content: space-between;
			align-items: center;
			height: 54rpx;
	
			.title {
				font-size: 36rpx;
				color: #333333;
				font-weight: bold;
			}
	
			.kong {
				height: 100%;
				font-size: 28rpx;
				color: rgba(153, 153, 153, 0);
			}
	
			.cancel {
				height: 100%;
				font-size: 28rpx;
				color: rgba(153, 153, 153, 1);
			}
		}
	
		.popup_list {
			padding-top: 10rpx;
			max-height: calc(70vh - 114rpx);
			overflow-y: scroll;
			.popup_li {
				padding: 30rpx;
				background-color: rgba(248, 248, 248, 1);
				border-radius: 6rpx;
				display: flex;
				justify-content: flex-start;
				align-items: center;
				line-height: auto;
				margin-top: 20rpx;
	
				
			}
		}
	}
	.icon_con {
		width: 80rpx;
		height: 80rpx;
		border-radius: 10rpx;
		display: flex;
		justify-content: center;
		align-items: center;
	}
		
	.temple_name {
		margin-left: 30rpx;
		
		.name {
			line-height: 36rpx;
			box-sizing: border-box;
			text-align: left;
			font-size: 28rpx;
		}
		
		.type {
			margin-top: 12rpx;
			color: rgba(153, 153, 153, 1);
			font-size: 24rpx;
			line-height: 32rpx;
			text-align: left;
		}
	}
	.word_con{
		position: relative;
		width: 100%;
		padding: 0 30rpx;
		box-sizing: border-box;
	}
	.serve_con {
		margin-top: 70rpx;
		width: 100%;
		font-weight: bold;
		.serve_title{
			line-height: 50rpx;
			font-size: 30rpx;
			height: 50rpx;
		}
		.serve_li{
			height: 120rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			background-color: rgba(255, 255, 255, 1);
			border-radius: 20rpx;
			margin-top: 20rpx;
			width: 100%;
			box-sizing: border-box;
			padding: 0 30rpx;
			font-size: 30rpx;
			color: rgba(51, 51, 51, 1);
			.left_con{
				display: flex;
				justify-content: flex-start;
				align-items: center;
			}
		}
	}

	.work_order_con {
		width: 100%;
		display: flex;
		justify-content: space-between;
		margin-top: 10rpx;

		.creat_work {
			width: 430rpx;
			height: 310rpx;
			position: relative;

			.image {
				width: 430rpx;
				height: 310rpx;
			}
		}

		.scan {
			width: 230rpx;
			height: 310rpx;
			position: relative;

			.image {
				width: 230rpx;
				height: 310rpx;
			}
		}

		.text_title {
			position: absolute;
			top: 40rpx;
			left: 40rpx;
			font-size: 36rpx;
			color: #ffffff;
		}

		.text_cont {
			position: absolute;
			top: 92rpx;
			left: 40rpx;
			font-size: 24rpx;
			color: rgba(255, 255, 255, 0.6);
		}
	}
</style>