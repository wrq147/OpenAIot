<template>
	<view>
		<top :title="topTitle" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#ffffff" rightIcon="icon-tianjia" @clickRight="wareRecordsAdd">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入客户名称" inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>
		<view class="agent_con">
			<view class="agent_list">
				<view class="agent_li" v-for="item in tbList" @click="clickListLi(item)">
					<view class="li_left">
						<!-- <image class="image" :src="getSerVerUrl()+'/appimg/default_cus.png'" mode=""></image> -->
						<view class="image t-icon-kehumorentouxiang1"></view>
					</view>
					<view class="li_right">
						<view class="right_li first">
							{{item.CustomerName}}
						</view>
						<view class="right_li" v-if="item.LeaderName">
							负责人: {{item.LeaderName}}
						</view>
					</view>
					<view class="daili tips" v-if="item.CustomerType==1">代理</view>
					<view class="zhixiao tips" v-if="item.CustomerType==0">直销</view>
				</view>
			</view>
		</view>
		<uni-load-more iconType="circle" :status="status" v-if="status" />
		<msg-prompt ref="promptMsg" @confirm="confirmCancel"></msg-prompt>
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
				topTitle: '选择目标客户',
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