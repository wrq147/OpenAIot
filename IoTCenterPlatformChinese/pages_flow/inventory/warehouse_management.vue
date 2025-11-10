<template>
	<view class="pages_bgcon">
		<top :title="topTitle" leftWidth="157rpx" leftIcon="icon-fanhui" backgroundColor="rgba(255, 255, 255, 1)"
			rightWidth="157rpx" rightIcon="icon-tianjia" :isleftBack="true" @clickRight="addWareHouse">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入仓库名称" backgroundColor="#fff" inputBg="#F8F8F8"></search-compt>
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
								{{item.IsSystem==1?'系统':''}}
							</view>
						</view>
						<view class="house_user" v-if="item.LeaderName">
							负责人: {{item.LeaderName}}
						</view>
					</view>
					<view class="select_icon t-icon-xuanzhongshebeihaocai1"
						v-if="isSelect&&selectId&&selectId==item.Id"></view>
					<view class="house_edit" @click.stop="editHouse(item)">
						<custom-icons iconsName="icon-bianji" iconsSize="28rpx"></custom-icons>
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
				topTitle: '仓库管理',
				querydata: {
					pageNum: 1,
					pageSize: 10,
				},
				status: 'loading',
				selectListParam: [{
					name: '仓库状态',
					params: 'Status',
					pal: '请选择仓库状态',
					value: null,
					localdata: [{
							text: "正常",
							value: 1
						},
						{
							text: "停用",
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
					url:'/pages_flow/inventory/add_warehouse?houseId='+row.Id
				})
			},
			addWareHouse(){
				//跳转去添加仓库
				uni.navigateTo({
					url:'/pages_flow/inventory/add_warehouse'
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
			background-color: rgba(255, 255, 255, 1);
			border-radius: 10rpx;
			width: 100%;
			padding: 25rpx 30rpx;
			box-sizing: border-box;
			color: rgba(51, 51, 51, 1);
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
					font-size: 30rpx;
					line-height: 42rpx;
					.name{
						font-weight: bold;
						.isdelect{
							color: rgba(153, 153, 153, 1);
							text-decoration: line-through;
							text-decoration-color: rgba(153, 153, 153, 1);
							text-decoration-style: solid;
						}
					}
					.type{
						padding: 5rpx 8rpx;
						border-radius: 5rpx;
						font-size: 20rpx;
						line-height: 24rpx;
						display: flex;
						align-items: center;
						justify-content: center;
						margin-left: 20rpx;
						color: rgba(153, 153, 153, 1);
						
						&.system{
							color: rgba(255, 53, 53, 1);
							background-color: rgba(255, 53, 53, 0.10);
						}
					}
				}
				.house_user{
					color: rgba(153, 153, 153, 1);
					font-size: 24rpx;
					line-height: 38rpx;
					margin-top: 6rpx;
				}
			}
		}
	}
}
</style>