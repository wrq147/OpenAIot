<template>
	<view style="background-color: rgba(245, 248, 249, 1);min-height: 100vh;">
		<top :title="topTitle" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="#ffffff">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入产品关键字" :fixed="true"
			backgroundColor="#fff" inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :selectListParam="selectListParam"
			:querydata="querydata" @selectFinsh="selectFinsh"></select-compt>
		<view class="product_con">
			<view class="product_total">
				总计（{{total}}）
			</view>
			<view class="product_ul">
				<view class="product_li" v-for="(row,inx) in agentProductList" @click.stop="choiceProduct(row,inx)">
					<view class="li_cont">
						<view class="cont_title">
							<view class="title_label" :class="{'normal':row.ProductLabel&&row.ProductLabel=='U'}">{{row.ProductLabel&&row.ProductLabel=='U'?'半成品':'成品'}}</view>
							<view class="title_name">
								<text>{{row.ProductName}}</text>
								<text v-if="row.TypeName" class="line">|</text>
								<text v-if="row.TypeName">{{row.TypeName}}</text>
							</view>
						</view>
						<view class="cont_number" v-if="row.SkuNumber">产品编码：{{row.SkuNumber}}</view>
					</view>
					<view class="li_icon">
						<view class="select_icon t-icon-gouxuan1"
							v-if="selectId&&selectId.includes(row.Id)"></view>
						<custom-icons v-else iconsName="icon-weixuanzhong" iconsSize="36rpx" iconsColor="rgba(234, 234, 234, 1)"></custom-icons>
					</view>
				</view>
			</view>
			
			<uni-load-more iconType="circle" :status="status" v-if="status" />
			<view style="height: 128rpx;"></view>
		</view>
		<view class="mul_confirm">
			<view class="mul_choice">已选：{{select.length}}项</view>
			<view class="choice_confirm" @click="confirmSelect">确认</view>
		</view>
	</view>
</template>

<script>
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import {
		agentProductListGet
	} from '@/api/product.js'
	export default {
		data() {
			return {
				topTitle: '选择产品',
				select: [],//选择的产品
				selectId:[],//选择产品的所有
				isSelect: false, //是否是选择
				isMulSelect: false, //是否是多选
				querydata: {
					pageNum: 1,
					pageSize: 30,
				}, //过滤参数
				agentProductList: [], //代理商产品列表
				selectListParam: [],
				timeQuery: {//下拉筛选
					name: '创建时间',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				total: 0 ,//产品总数
				status:'loading'
			};
		},
		onLoad(options) {
			if (options.isSelect) {
				this.isSelect = !!options.isSelect
			}
			if (options.isMulSelect) {
				this.isMulSelect = !!options.isMulSelect
			}
			if (options.selectId) {
				this.select = JSON.parse(options.selectId)
				this.selectId=this.select.map(row=>row.Id)
			}
			this.loadList()
		},
		onReachBottom() {
			//上拉触底
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = 'loading';
				this.loadAgentProductListGet();
			}
		},
		methods: {
			confirmSelect(){
				setPagesParam('finishSelectProduct', this.select, 1);
			},
			choiceProduct(row,inxO){
				if (this.selectId && this.selectId.includes(row.Id)) {
					let inxO = this.selectId.indexOf(row.Id)
					this.selectId.splice(inxO, 1)
					this.select.splice(inxO, 1)
				} else {
					this.select.push(row)
					this.selectId.push(row.Id)
				
				}
				if (this.isMulSelect) {
				} else {
					let arr = [];
					arr.push(row);
					setPagesParam('finishSelectProduct', arr, 1);
				}
			},
			loadList() {
				//加载列表的方法
				this.querydata.pageNum = 1;
				this.status = 'loading';
				this.loadAgentProductListGet();
			},
			loadAgentProductListGet() {
				agentProductListGet(this.querydata).then(res => {
					// console.log("加载代理商产品", res);
					if(this.querydata.pageNum == 1){
						this.agentProductList=[]
					}
					this.agentProductList = [...this.agentProductList,...res.data.List]
					this.total=res.data.Total
					if (res.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				})
			},
			searching(val) {
				//搜索关键字函数
				this.key = val
				if (this.key) {
					this.querydata.Key = this.key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.agentProductList = []
					this.loadAgentProductListGet()
				} else {
					delete this.querydata.Key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.agentProductList = []
					this.loadAgentProductListGet()
				}
			},
			openSelect() {
				//打开筛选弹窗
				this.$refs.selectCompt.openSelect()
			},
			selectFinsh(query) {
				//筛选完毕结果，筛选参数返回结果函数
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.agentProductList = []
				this.loadAgentProductListGet()
			}
		}
	}
</script>

<style lang="scss" scoped>
	
</style>