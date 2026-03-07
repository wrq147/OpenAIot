<template>
	<uni-popup ref="filedpopup" type="bottom" @change="popChange" :zIndex="999">
		<view class="filed_con">
			<view class="filed_title">
				<view class="cancel" @click="close">取消</view>
				<view class="title_text">筛选条件</view>
				<view class="filed_all">
					<view class="icon_con" @click="setAllChoice" v-if="allChoiceKey">
						<custom-icons v-if="isSelectAll" iconsName="icon-weixuanzhong" iconsSize="36rpx"
							iconsColor="rgba(234, 234, 234, 1)"></custom-icons>
						<view v-else class="icon_del t-icon-gouxuan1"></view>
					</view>
					<text style="margin-left: 12px;">全选</text>
				</view>
			</view>
			<view class="filed_ul">
				<view class="filed_li" v-for="(it,inx) in filedList" @click="setRowSelect(it,inx)">
					
					<view v-if="it.select" class="icon_sel t-icon-gouxuan1"></view>
					<custom-icons v-else iconsName="icon-weixuanzhong" iconsSize="36rpx" iconsColor="rgba(234, 234, 234, 1)"></custom-icons>
					<view class="filed_li_text">{{it.name}}</view>
				</view>
			</view>
			<view class="handle_zhanwei"></view>
			<view class="handle_con">
				<view class="handle_left">
					已选：{{selectLen}}项
				</view>
				<view class="filter_handle" @click="finishFiledChoice">确定</view>
			</view>
		</view>
	</uni-popup>
</template>

<script>
	var dayjs = require('@/common/day.js')

	export default {
		data() {
			return {
				filedList:[],
				allChoiceKey:true
			}
		},
		computed:{
			selectLen(){
				let filterAll=this.filedList.filter(row=>row.select)
				return filterAll.length
			},
			isSelectAll(){
				let obj=this.filedList.find(row=>row.select==undefined||!row.select)
				// console.log("isSelectAll",obj);
				if(obj){
					return true
				}else{
					return false
				}
			}
		},
		methods: {
			setAllChoice(){
				if(this.isSelectAll){
					this.filedList=this.filedList.map(row=>{
						row.select=true
						return row
					})
				}else{
					this.filedList=this.filedList.map(row=>{
						row.select=false
						return row
					})
				}
				this.$forceUpdate()
			},
			finishFiledChoice(){
				let filterSelectArr=this.filedList.filter(row=>row.select)
				this.close()
				this.$emit('finishFiledChoice',filterSelectArr)
				
			},
			setRowSelect(it,inx){
				if(it.select){
					this.filedList[inx].select=false
				}else{
					this.filedList[inx].select=true
				}
				let list=JSON.parse(JSON.stringify(this.filedList))
				this.filedList=JSON.parse(JSON.stringify(list))
				this.$forceUpdate()
			},
			close() {
				this.$refs.filedpopup.close();
			},
			popChange(){

			},
			openPopup(filedList,filterFiled) {
				if(filedList){
					this.filedList=JSON.parse(JSON.stringify(filedList))
					if(filterFiled){
						for(let i=0;i<filterFiled.length;i++){
							let item=filterFiled[i]
							let findObjIndex=this.filedList.findIndex(row=>row.mapid==item.field)
							if(findObjIndex!=undefined&&findObjIndex>-1){
								this.filedList[findObjIndex].select=true
							}
						}
					}
				}
				this.$refs.filedpopup.open('bottom');
			}
		}
	}
</script>

<style lang="less" scoped>
	.filed_con {
		// height: 70%;
		border-radius: 20rpx 20rpx 0 0;
		background-color: #ffffff;
		
		.filed_ul{
			max-height: calc(70vh - 128rpx);
			overflow-y: auto;
		}
		.handle_zhanwei{
			height: 128rpx;
			width: 100%;
		}
		.handle_con {
			position: fixed;
			bottom: 0;
			left: 0;
			display: flex;
			width: 100%;
			justify-content: space-between;
			align-items: center;
			padding: 20rpx 30rpx 30rpx;
			height: 128rpx;
			border-top: 1rpx solid rgba(234, 234, 234, 1);
			box-sizing: border-box;
			background: #ffffff;
			.handle_left {
				display: flex;
				justify-content: flex-start;
				align-items: center;
				font-size: 30rpx;
				color: rgba(102, 102, 102, 1);
		
				.clear_btn {
					padding: 24rpx 50rpx;
					border: 1rpx solid rgba(216, 216, 216, 1);
					border-radius: 44rpx;
				}
		
				.save_handle {
					padding: 24rpx 30rpx;
					border: 1rpx solid rgba(216, 216, 216, 1);
					border-radius: 44rpx;
					margin-left: 10rpx;
				}
			}
		
			.filter_handle {
				padding: 24rpx 50rpx;
				color: rgba(255, 255, 255, 1);
				background: rgba(35, 113, 255, 1);
				border-radius: 44rpx;
			}
		}
		.filed_title_zhanwei{
			width: 100%;
			padding: 38rpx 30rpx;
			height: 112rpx;
			box-sizing: border-box;
		}
		.filed_title{
			width: 100%;
			padding: 38rpx 30rpx;
			height: 112rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			font-size: 28rpx;
			color: rgba(153, 153, 153, 1);
			background: #ffffff;
			box-sizing: border-box;
			border-radius: 20rpx 20rpx 0 0;
			.title_text{
				font-size: 32rpx;
				color: rgba(51, 51, 51, 1);
				font-weight: bold;
			}
			.filed_all{
				display: flex;
				justify-content: center;
				align-items: center;
				color: rgba(102, 102, 102, 1);
				.icon_del{
					width: 36rpx;
					height: 36rpx;
				}
			}
		}
		.filed_li{
			width: 100%;
			padding: 0 30rpx;
			display: flex;
			justify-content: flex-start;
			align-items: center;
			height: 100rpx;
			box-sizing: border-box;
			.icon_sel{
				width: 36rpx;
				height: 36rpx;
			}
			.filed_li_text{
				margin-left: 20rpx;
				color: rgba(51, 51, 51, 1);
				font-size: 28rpx;
			}
		}
	}
</style>