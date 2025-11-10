<template>
	<view>
		<top :title="topTitle" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" backgroundColor="#161A26"
			rightWidth="157rpx" rightIcon="icon-tianjia" :isleftBack="true" @clickRight="addWareHouse">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="Please enter the name"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>
		<view class="list_con">
			<view class="house_ul">
				<view class="house_li" v-for="item in tbList" @click="choiceHouse(item)">
					<view class="house_info">
						<view class="name_type">
							<view class="name">
								<text :class="{'isdelect':item.Status=='0'}">{{item.StoreName}}</text>
							</view>
							<view class="type" :class="{'system':item.IsSystem==1}">
								{{item.IsSystem==1?'Syetem':'Convention'}}
							</view>
						</view>
						<view class="house_user" v-if="item.LeaderName">
							Manager: {{item.LeaderName}}
						</view>
					</view>
					<view class="select_icon t-icon-xuanzhongshebeihaocai"
						v-if="isSelect&&selectId&&selectId==item.Id"></view>
					<view class="house_edit" @click.stop="editHouse(item)">
						<custom-icons iconsName="icon-bianji" iconsSize="36rpx"></custom-icons>
					</view>
				</view>
			</view>
		</view>	
		<uni-load-more iconType="circle" :status="status" v-if="status" />
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		houseList,
	} from "@/api/house.js";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				tbList:[],//仓库数据
				topTitle: 'Warehouse',
				querydata: {
					pageNum: 1,
					pageSize: 10,
				},
				status: 'loading',
				selectListParam: [{
					name: 'Status',
					params: 'Status',
					pal: 'Please select a status',
					value: null,
					localdata: [{
							text: "normal",
							value: 1
						},
						{
							text: "Deactivate",
							value: 0
						}
					]
				}],
				selectId:'',
				isSelect:false
			};
		},
		onLoad(options) {
			if(options.isSelect){
				this.isSelect=!!options.isSelect
				if(options.selectId){
					this.selectId=options.selectId
				}
			}
			this.getHouseList()
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = "loading";
				this.getHouseList();
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			choiceHouse(item){
				if(this.isSelect){
					setPagesParam('selectHouse', item, 1)
				}
			},
			editHouse(row){
				//编辑仓库
				uni.navigateTo({
					url:'/pages_Inventory/add_warehouse?houseId='+row.Id
				})
			},
			addWareHouse(){
				//跳转去添加仓库
				uni.navigateTo({
					url:'/pages_Inventory/add_warehouse'
				})
			},
			loadList(){
				//加载列表方法
				this.querydata.pageNum=1
				this.status='loading'
				this.getHouseList()
			},
			getHouseList(query) {
				//获取仓库列表
				if(query){
					this.querydata.pageNum=1
					this.tbList=[]
					this.status='loading'
				}
				houseList(this.querydata).then(response => {
					if(this.querydata.pageNum==1){
						this.tbList=[]
					}
					console.log("仓库信息",response);
					this.tbList=[...this.tbList,...response.data.List]
					if (response.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				}).catch(err=>{
					this.setMsgTop(err)
				});
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
				console.log("query", query);
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.tbList=[]
				this.status='loading'
				this.getHouseList()
				
			}
		}
	}
</script>

<style lang="less">
.list_con{
	width: 100%;
	padding: 0 20rpx;
	box-sizing: border-box;
	.house_ul{
		.house_li{
			margin-top: 20rpx;
			background-color: rgba(28, 34, 50, 1);
			border-radius: 10rpx;
			width: 100%;
			padding: 25rpx 30rpx;
			box-sizing: border-box;
			color: #fff;
			display: flex;
			justify-content: space-between;
			align-items: center;
			position: relative;
			.select_icon{
				width: 50rpx;
				height: 50rpx;
				position: absolute;
				right: 0;
				top: 0;
			}
			.house_info{
				.name_type{
					display: flex;
					justify-content: flex-start;
					align-items: center;
					font-size: 32rpx;
					line-height: 42rpx;
					.name{
						.isdelect{
							color: rgba(255, 255, 255, 0.5);
							text-decoration: line-through;
							text-decoration-color: rgba(255, 255, 255, 1);
							text-decoration-style: solid;
						}
					}
					.type{
						padding: 5rpx 8rpx;
						border-radius: 5rpx;
						font-size: 22rpx;
						line-height: 24rpx;
						display: flex;
						align-items: center;
						justify-content: center;
						margin-left: 20rpx;
						color: rgba(255, 255, 255, 0.5);
						border: 1rpx solid rgba(255, 255, 255, 0.5);
						&.system{
							color: rgba(255, 53, 53, 1);
							border: 1rpx solid rgba(255, 53, 53, 1);
						}
					}
				}
				.house_user{
					color: rgba(255, 255, 255, 0.5);
					font-size: 28rpx;
					line-height: 38rpx;
					margin-top: 6rpx;
				}
			}
		}
	}
}
</style>