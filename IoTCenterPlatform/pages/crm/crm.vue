<template>
	<view  :class="[hide==true?'fixedActive':'fixedOn']">
		<view id="CRM" >
			<top leftIcon="" leftText="CRM" backgroundColor="#161A26"  leftFontSize="44rpx" title="" rightIcon="icon-a-tianjiabantouming" class="CRM-header">
				<template v-slot:top_right>
					<view class="right_top" @click="handAddPage">
						<custom-icons v-if="!hide" iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
						<custom-icons v-else iconsName="icon-crmtianjiaguanbi" iconsSize="36rpx"></custom-icons>
					</view>
				</template>
			</top>
			<view  class="crm-nav">
				<view class="crm-header-item" @click="handAuthorized" v-if="isCheckPermi(['/CRMService/Agent/AuthList'])">
					<view class="crm-header-logo iconfont icon-wodeshouquan" ></view>
					<view class="crm-header-text">Authorized</view>
				</view>
				<view class="crm-header-item" @click="handHighseas" v-if="isCheckPermi(['/CRMService/Customer/PubList'])">
					<view class="crm-header-logo iconfont icon-gonghaichi" ></view>
					<view class="crm-header-text">High seas</view>
				</view>
				<view class="crm-header-item" @click="handCustomers" v-if="isCheckPermi(['/CRMService/Customer/List'])">
					<view class="crm-header-logo iconfont icon-kehu" ></view>
					<view class="crm-header-text">Customers</view>
				</view>
				<view class="crm-header-item" @click="handCluePool" v-if="isCheckPermi(['/CRMService/Clue/PubList'])">
					<view class="crm-header-logo iconfont icon-xiansuochi" ></view>
					<view class="crm-header-text">Clue pool</view>
				</view>
				<view class="crm-header-item" @click="handXian" v-if="isCheckPermi(['/CRMService/Clue/PriList'])">
					<view class="crm-header-logo iconfont icon-xiansuo" ></view>
					<view class="crm-header-text">Clues</view>
				</view>
				<view class="crm-header-item" @click="handContact" v-if="isCheckPermi(['/CRMService/Contact/List'])">
					<view class="crm-header-logo iconfont icon-lianxiren" ></view>
					<view class="crm-header-text">Contacts</view>
				</view>
				<view class="crm-header-item" @click="handOppor" v-if="isCheckPermi(['/CRMService/Opportunity/List'])">
					<view class="crm-header-logo iconfont icon-shangji" ></view>
					<view class="crm-header-text">Opportunity</view>
				</view>
				<view class="crm-header-item" @click="handFollowPlan" v-if="isCheckPermi(['/CRMService/Plan/List'])">
					<view class="crm-header-logo iconfont icon-genjinjihua" ></view>
					<view class="crm-header-text">Follow up plan</view>
				</view>
				<view class="crm-header-item" @click="handFollowRecord" v-if="isCheckPermi(['/CRMService/Follow/List'])">
					<view class="crm-header-logo iconfont icon-genjinjilu" ></view>
					<view class="crm-header-text">Followed up</view>
				</view>
			</view>
			<view class="CRM-Sales-data">
				<view class="CRM-Sales-data-header">
					<view class="CRM-Sales-data-logo iconfont icon-xiaoshoushuju">
						<text class="text" >Sales data</text>
					</view>
					<view class="CRM-Sales-data-header-right" @click="handSalesData()">
						<text>{{saleTotDay}}</text>
						<text class="youjian iconfont icon-xialajiantou"></text>
					</view>
					<view class="pointAlert" v-if="salesDataHide">
						<view v-for="(item,index) in dayData" :key="index" class="pointAlert-item" @click="handSaleItem(item)">
							{{item.title}}
						</view>
					</view>
				</view>
				<view class="CRM-Sales-customers">
					<view class="CRM-Sales-customers-item">
						<view class="num">{{salesData.NewKfCount}}</view>
						<view class="parse-text">
							<view class="content">New</view>
							<view class="content">customers</view>
						</view>
					</view>
					
					<view class="CRM-Sales-customers-item">
						<view class="num">{{salesData.NewOpportCount}}</view>
						<view class="parse-text">
							<view class="content">New</view>
							<view class="content">opportunity</view>
						</view>
					</view>
					
					<view class="CRM-Sales-customers-item">
						<view class="num">{{salesData.NewFollowCount}}</view>
						<view class="parse-text">
							<view class="content">New</view>
							<view class="content">New follow-up</view>
						</view>
					</view>
					
					<view class="CRM-Sales-customers-item">
						<view class="num">{{salesData.FollowClueCount}}</view>
						<view class="parse-text">
							<view class="content">Clues</view>
							<view class="content">follow-up</view>
						</view>
					</view>
					
					<view class="CRM-Sales-customers-item">
						<view class="num">{{salesData.FollowCustomerCount}}</view>
						<view class="parse-text">
							<view class="content">Customer</view>
							<view class="content">follow-up</view>
						</view>
					</view>
					
					<view class="CRM-Sales-customers-item">
						<view class="num">{{salesData.FollowOpportCount}}</view>
						<view class="parse-text">
							<view class="content">Opportunity</view>
							<view class="content">follow-up</view>
						</view>
					</view>
				</view>
			</view>
			
			<view class="CRM-Sales-data">
				<view class="CRM-Sales-data-header">
					<view class="CRM-Sales-data-logo iconfont icon-xiaoshoushuju">
						<text class="text" >Sales funnel</text>
					</view>
					<view class="CRM-Sales-data-header-right" @click="handClickDays()">
						<text>{{totDay}}</text>
						<text class="youjian iconfont icon-xialajiantou"></text>
					</view>
					<view class="pointAlert" v-if="dayHide">
						<view v-for="(item,index) in dayData" :key="index" class="pointAlert-item" @click="handClickItem(item)">
							{{item.title}}
						</view>
					</view>
				</view>
				<view class="FunnelPlot">
					<funnel-plot :timeDate="timeDate"></funnel-plot>
				</view>
			</view>
		</view>
		<!--弹框创建-->
		<view class="alert" v-if="hide">
			<view class="alert-item" @click="handSeas">
				High seas
			</view>
			<view class="alert-item" @click="handCustomersRoute">
				Customers
			</view>
			<view class="alert-item" @click="handClueRoute">
				Clue pool
			</view>
			<view class="alert-item" @click="handRouteItem">
				Clues
			</view>
			<view class="alert-item" @click="handContactRoute">
				Contacts
			</view>
			<view class="alert-item" @click="handOpportRoute">
				Opportunity
			</view>
			<view class="alert-item" @click="handFollowRoute">
				Follow up plan
			</view>
			<view class="alert-item" @click="handFollowed">
				Followed up
			</view>
		</view>
		<view class="move" v-if="hide"></view>
		<my-tab-bar active="CRM" ref="mytab"></my-tab-bar>
		
	</view>
</template>

<script>
	import {
		crmData,//Sales data 数据信息
	} from "@/api/crmApi";
	import {
		FunnelPlot,//漏斗图
	} from "@/api/crmApi";
	import {
		dataList,
	} from "@/api/personalCenter";
	import {
		checkPermi
	} from '@/common/permission.js';
	export default {
		data() {
			return {
				saleTotDay:'Today',
				totDay:'Today',
				dayHide:false,
				salesDataHide:false,
				dayData:[
					{
						title:'today',
						value:0
					},
					{
						title:'yesterday',
						value:1
					},
					{
						title:'7 days',
						value:7
					},
					{
						title:'30 days',
						value:30
					},
				],
				timeDateSales:'0',
				timeDate:'0',
				arr1:[],
				hide:false,
				salesData:[],//Sales data 数据信息
				arr:[
					{
						icon:'icon-wodeshouquan',
						text:'Authorized'
					},{
						icon:'icon-gonghaichi',
						text:'High seas'
					},{
						icon:'icon-kehu',
						text:'Customers'
					},{
						icon:'icon-xiansuochi',
						text:'Clue pool'
					},
					{
						icon:'icon-xiansuo',
						text:'Clues'
					},
					{
						icon:'icon-lianxiren',
						text:'Contacts'
					},
					{
						icon:'icon-lianxiren',
						text:'Opportunity'
					},
					{
						icon:'icon-genjinjihua',
						text:'Follow up plan'
					},
					{
						icon:'icon-genjinjilu',
						text:'Followed up'
					},
				]
			}
		},
		async onLoad() {
			this.$nextTick(()=>{
				this.$refs.mytab.loadCheck()
			})
			if(this.$store.state.user.roles&&this.$store.state.user.roles.length>0){
			}else{
				let rsp = await dataList({
					id: 0
				})
				await this.$store.dispatch('GetInfo')
				this.$store.commit('SET_ROLES',rsp.data.user.roleIds)
				this.$refs.mytab.loadCheck()
			}
			this.list();//Sales data 数据信息
			//this.dataFullt();//漏斗图
		},
		methods: {
			handSalesData(){
				this.salesDataHide=!this.salesDataHide
			},
			handSaleItem(ite){
				this.timeDateSales=JSON.stringify(ite.value);
				this.saleTotDay=ite.title
				this.salesDataHide=false;
				this.list();
			},
			handClickItem(ite){
				this.timeDate=JSON.stringify(ite.value);
				this.dayHide=false;
				this.totDay=ite.title
				//console.log(this.timeDate,'this.timeDate');
			},
			handClickDays(){
				this.dayHide=!this.dayHide
			},
			isCheckPermi(val) {
				return checkPermi(val)
			},
			dataFullt(){
				
			},
			handFollowed(){
				uni.navigateTo({
					url:'../../page_crm_moudule/followRrecords/AddFollowRecord'
				})
			},
			handFollowRoute(){
				uni.navigateTo({
					url:'../../page_crm_moudule/FollowPlan/addFollowPlan'
				})
			},
			handOpportRoute(){
				uni.navigateTo({
					url:'../../page_crm_moudule/Opportunity/additionOpportunity'
				})
			},
			handContactRoute(){
				uni.navigateTo({
					url:'../../page_crm_moudule/contacts/ContactAddition'
				})
			},
			handRouteItem(){
				uni.navigateTo({
					url:'../../page_crm_moudule/clue/addClue'
				})
			},
			handClueRoute(){
				uni.navigateTo({
					url:'../../page_crm_moudule/CluePool/addCluePool'
				})
			},
			handCustomersRoute(){
				uni.navigateTo({
					url:'../../page_crm_moudule/Customer/CreateCustomer'
				})
			},
			handSeas(){
				uni.navigateTo({
					url:'../../page_crm_moudule/OpenPool/addPool'
				})
			},
			handAddPage(){
				this.hide=!this.hide;
			},
			//我的授权
			handAuthorized(){
				uni.navigateTo({
					url:'/page_crm_moudule/MyAuthorization/index'
				})
			},
			//跟进记录
			handFollowRecord(){
				uni.navigateTo({
					url:'/page_crm_moudule/followRrecords/followRrecords'
				})
			},
			//跟进计划
			handFollowPlan(){
				uni.navigateTo({
					url:'/page_crm_moudule/FollowPlan/FollowPlan'
				})
			},
			//商机列表
			handOppor(){
				uni.navigateTo({
					url:'/page_crm_moudule/Opportunity/Opportunity'
				})
			},
			handContact(){//联系人
				uni.navigateTo({
					url:'/page_crm_moudule/contacts/contacts'
				})
			},
			handXian(){//线索
				uni.navigateTo({
					url:'/page_crm_moudule/clue/clue'
				})
			},
			handCluePool(){//线索池
				uni.navigateTo({
					url:'/page_crm_moudule/CluePool/CluePool'
				})
			},
			handCustomers(){//私海客户
			uni.navigateTo({
				url:'/page_crm_moudule/Customer/Customer'
			})
				
			},
			//公海池
			handHighseas(){
				uni.navigateTo({
					url:'/page_crm_moudule/OpenPool/OpenPool'
				})
				
			},
			list(){
				crmData({
					day:this.timeDateSales
				}).then((res)=>{
					if(res.code==0){
						//console.log(res)
						this.salesData=res.data
					}
				}).catch((err)=>{
					//console.log(err.message);
					this.setMsgTop(err)
				})
			}
		}
	}
</script>

<style lang="less">
	.alert{
		position: absolute;
		right:25rpx;
		/* #ifdef H5*/
		top:115rpx;
		/*#endif*/
		/* #ifndef H5*/
		top:180rpx;
		/*#endif*/
		width: 240rpx;
		// height:634rpx;
		background: rgba(39, 45, 60, 1);
		color:#fff;
		z-index:229;
		border-radius: 9rpx;
		padding:15rpx 30rpx;
		.alert-item{
			height: 80rpx;
			line-height: 80rpx;
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
	.fixedActive{
		position: fixed;
	}
	.fixedOn{
		position: relative;
	}
#CRM{
	background: #161A26;
	height:100%;
	// height:100vh;
	color:#fff;

	.CRM-Sales-data{
		width:94%;
		margin:20rpx 3%;
		background: #1C2232;
		.FunnelPlot{
			margin-top:-100rpx;
			padding-bottom: 40rpx;
			/deep/canvas{
				height:850rpx;
			}
		}
		.CRM-Sales-customers{
			display: flex;
			flex-wrap: wrap;
			 align-items: center;
			.CRM-Sales-customers-item{
				display: flex;
				flex-direction: column;
				 align-items: center;
				width: 33%;
				text-align: center;
				margin-bottom:25rpx;
				margin-top:20rpx;
				.parse-text{
					line-height: 26rpx;
					margin-top:10rpx;
				}
				.content{
					color:rgba(255,255,255,.6);
					font-size:24rpx;
				}
				.num{
					font-size:32rpx;
				}
			}
		}
		.CRM-Sales-data-header{
			position: relative;
			display: flex;
			padding:30rpx;
			align-items: center;
			.pointAlert{
				position: absolute;
				width: 200rpx;
				background: rgba(39, 45, 60, 1);
				right:0rpx;
				top:80rpx;
				border-radius: 12rpx;
				z-index:100;
				.pointAlert-item{
					text-align: center;
					height:80rpx;
					line-height: 80rpx;
				}
			}
			.CRM-Sales-data-header-right{
				margin-left:auto;
				color:rgba(255,255,255,.6);
				font-size:24rpx;
				z-index:100;
				.youjian{
					font-size:12rpx;
					margin-left:10rpx;
					
				}
			}
			.CRM-Sales-data-logo{
				.text{
					padding-left:20rpx;
					font-size:36rpx;
				}
			}
		}
		
	}
	.crm-nav{
		display: flex;
		flex-wrap: wrap;
		text-align: center;
		margin-top:20rpx;
		font-size:24rpx;
		.crm-header-item{
			width: 33%;
			padding:25rpx 0rpx;
			.crm-header-text{
				margin-top:5rpx;
				 color:rgba(255,255,255,.6);
				 font-size:24rpx;
			}
			.crm-header-logo{
				width: 44rpx;
				height:44rpx;
				margin:0 auto;
				font-size:44rpx;
			}
		}
		
	}
	.CRM-header{
		font-size:44rpx;
		font-weight: bold;
	}
	/deep/.uni-navbar__header{
		padding-left:0rpx;
	}
	
}
</style>



