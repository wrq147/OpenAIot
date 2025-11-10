<template>
	<view>
		<top :title="topTitle" leftText="Back" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#161A26">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="Please enter the name"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>
		<view class="agent_con">
			<view class="agent_list">
				<view class="agent_li" v-for="item in tbList" @click="clickListLi(item)">
					<view class="li_left">
						<image class="image" src="../static/huada.png" mode=""></image>
					</view>
					<view class="li_right">
						<view class="right_li first">
							{{item.CustomerName}}
						</view>
						<view class="right_li" v-if="item.LeaderName">
							Manager: {{item.LeaderName}}
						</view>
					</view>
					<view class="daili tips" v-if="item.CustomerType==0">Agent</view>
					<view class="zhixiao tips" v-if="item.CustomerType==1">Direct Sales</view>
				</view>
			</view>
		</view>
		<uni-load-more iconType="circle" :status="status" v-if="status" />
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		privateCustomer
	} from "@/api/factory";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				topTitle: 'Select target custom',
				querydata: {
					pageNum: 1,
					pageSize: 10,
				},
				timeQuery: {
					name: 'Creation time',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				tbList: [],
				total: 0,
				selectAgent: [],
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
			}

		},
		methods: {
			confirmSelected() {
				//确定选择设备结果
				setPagesParam('finishSelectCustom', this.selectAgent, 1)
			},
			clickListLi(row){
				if(this.isSelect){
					this.selectPJFun(row)
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
					setPagesParam('finishSelectCustom', arr, 1)
				}

			},
			/** 查询列表 */
			getList() {
				this.status = 'loading'
				privateCustomer(this.querydata).then(
					response => {
						console.log("客户列表", response);
						if (response.data && response.data.List) {
							response.data.List.map(async row => {
								row.IndustryName = "";
								if (row.Industry) {
									//行业类型
									row.IndustryName = await this.$store.dispatch(
										"data/industryName",
										row.Industry
									);
								}
							});
							this.tbList = response.data.List;
						}
						this.total = response.data.Total;
						if (response.data.List.length < this.querydata.pageSize) {
							this.status = 'noMore';
						} else {
							this.status = 'more';
						}
					}
				).catch(err => {
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
				console.log("query", query);
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

</style>