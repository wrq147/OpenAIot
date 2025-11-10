<template>
	<view>
		<top :title="topTitle" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#ffffff" rightIcon="icon-tianjia" @clickRight="consumableAdd">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入耗材名称" inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>
		<view class="device_list_con">
			<view class="device_list">
				<view class="list_li" @click="toPJDetailFun(row)" v-for="row in tbList">
					<view class="li_left">
						<image class="image" :src="row.PhotoUrl+'?wh=500x500'" mode="aspectFit"></image>
					</view>
					<view class="li_right">
						<view class="dev_name">
							{{row.Name}}
						</view>
						<view class="group space_flex">
							<view class="text">
								{{row.DeviceNumber}}
							</view>
							<view class="text">
								价格: ￥{{row.Price}}
							</view>
						</view>
						<view class="group" v-if="row.ClassName">
							分类:{{row.ClassName}}
						</view>
					</view>
					<view class="select_icon t-icon-xuanzhongshebeihaocai1"
						v-if="isSelect&&selectId&&selectId.includes(row.Id)"></view>
				</view>
				<uni-load-more iconType="circle" :status="status" v-if="status" />
			</view>
			<view v-if="isSelect&&isMulSelect">
				<view class="btn_zhanwei" style="width: 100%;height: 98rpx;"></view>
				<button class="submit_button" @click="confirmSelected">
					确定({{selectConsumable.length}})
				</button>
			</view>
			<msg-prompt ref="promptMsg"></msg-prompt>
		</view>
	</view>
</template>

<script>
	import {
		partsList,
		addParts,
		delParts,
		editParts,
		partsInfo,
		generatePartsNumber
	} from "@/api/parts";
	import {
		classTree
	} from "@/api/partscls"
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				topTitle: '耗材管理',
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
				selectConsumable: [],
				selectId: [],
				isSelect: false,
				isMulSelect: false,
				status: 'loading'
			};
		},
		onLoad(options) {
			this.getList()
			if (options.isSelect) {
				this.isSelect = !!options.isSelect
				if (options.selectId) {
					this.selectConsumable = JSON.parse(options.selectId)
					this.selectId = this.selectConsumable.map(item => {
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
			consumableAdd(){
				uni.navigateTo({
					url:'/pages_factory/consumable_add'
				})
			},
			confirmSelected() {
				//确定选择设备结果
				setPagesParam('selectConsumable', this.selectConsumable, 1)
			},
			selectPJFun(row) {
				if (this.selectId && this.selectId.includes(row.Id)) {
					let inxO = this.selectId.indexOf(row.Id)
					this.selectId.splice(inxO, 1)
					this.selectConsumable.splice(inxO, 1)
				} else {
					this.selectConsumable.push(row)
					this.selectId.push(row.Id)
				}
				if (this.isMulSelect) {} else {
					let arr = []
					arr.push(row)
					setPagesParam('selectConsumable', arr, 1)
				}

			},
			toPJDetailFun(row) {
				//跳转去耗材详情
				if (this.isSelect) {
					this.selectPJFun(row)
				}else{
					
					uni.navigateTo({
						url:'/pages_factory/consumable_detail?id='+row.Id
					})
				}
			},
			/** 查询列表 */
			getList() {
				this.status = 'loading'
				partsList(this.querydata).then(response => {
					this.tbList = response.data.List;
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

			}
		}
	}
</script>

<style lang="less">

</style>