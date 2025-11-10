<template>
	<view class="tree-item">
		<view class="head" @click.stop="changeli" :class="{'input_sel':isInputSel}">
			<view class="left_con">
				<view class="left_icon_con" :class="{'xuanzhuan_top':isInputSel&&show}"
					v-if="isInputSel&&item[defaultProps.children] && item[defaultProps.children].length > 0" @click.stop="changeShow">
					<custom-icons iconsName="icon-xialajiantou" iconsSize="12rpx"></custom-icons>
				</view>
				<text class="txt">{{item[defaultProps.label]}}</text>
			</view>
			<!-- <view class="right_icon">
				<custom-icons iconsName="icon-zuneiyixuan" iconsSize="20rpx" iconsColor="#D81E06"></custom-icons>
			</view> -->
			<view class="right_icon" v-if="item.checked"></view>
		</view>
		<view class="content" v-if="item[defaultProps.children] && item[defaultProps.children].length > 0" v-show="show"
			:class="{'input_sel_content':isInputSel}">
			<tree-node v-for="sitem in item[defaultProps.children]" :item="sitem" :key="sitem[defaultProps.id]"
				:defaultProps="defaultProps"
				:parentInfo="{'id':[...parentInfo.id,item[defaultProps.id]],'label':[...parentInfo.label,item[defaultProps.label]]}"></tree-node>
		</view>
	</view>
</template>

<script>
	import TreeNode from './tree-compt.vue'
	export default {
		name: 'TreeNode',
		componentName: 'TreeNode',
		props: {
			item: {
				type: Object,
				default: () => {
					return {}
				}
			},
			parentInfo: {
				type: Object,
				default: () => {
					return {
						id:[],
						label:[]
					}
				}
			}
		},
		inject: ['defaultProps','checkList','checkParentList', 'onClickItem', 'editItem', 'isInputSel'],
		data() {
			return {
				show: false
			}
		},
		beforeMount() {
			if (!this.isInputSel) {
				this.show = true
			}
			// console.log(this.item.checked,'item');
			
		},
		mounted(){
			
			if(this.checkList.includes(this.item[this.defaultProps.id])){
				this.item.checked=true
			}else{
				this.item.checked=false
			}
			if(this.checkParentList.includes(this.item[this.defaultProps.id])){
				this.show=true
			}else{
				this.show=false
			}
			this.$forceUpdate()
		},
		methods: {
			editItemNode(item) {
				this.editItem(item)
			},
			changeShow() {
				if (this.item[this.defaultProps.children] && this.item[this.defaultProps.children].length > 0) {
					this.show = !this.show;
				}
			},
			changeli() {
				if(this.item.checked){
					this.item.checked=false
				}else{
					this.item.checked=true
				}
				this.item.parentIdList=this.parentInfo.id
				this.item.parentNameList=this.parentInfo.label
				this.$forceUpdate()
				this.onClickItem(this.item);
			},
		}
	}
</script>

<style scoped lang="scss">
	@mixin animate2 {
		-moz-transition: all .2s linear;
		-webkit-transition: all .2s linear;
		-o-transition: all .2s linear;
		-ms-transition: all .2s linear;
		transition: all .2s linear;
	}

	.tree-item {
		.head {
			display: flex;
			justify-content: space-between;
			align-items: center;
			line-height: 100rpx;

			&.input_sel {
				line-height: 80rpx;

				.txt {
					font-size: 28rpx;
					color: #FFF;
					margin-left: 20rpx;
				}
			}

			.left_con {
				display: flex;
				justify-content: flex-start;
				align-items: center;
			}

			.txt {
				font-size: 32rpx;
				color: #FFF;
			}

			.right_icon {
				margin-right: 5px;
				border: 2px solid #FF3535;
				border-left: 0;
				border-top: 0;
				height: 12px;
				width: 6px;
				transform-origin: center;
				/* #ifndef APP-NVUE */
				transition: all 0.3s;
				/* #endif */
				transform: rotate(45deg);
			}
		}

		.left_icon_con {
			display: flex;
			justify-content: flex-start;
			align-items: center;

			&.xuanzhuan_top {
				transform: rotate(180deg);
				-webkit-transform: rotate(180deg);
				transition: transform ease 0.5s;
			}
		}

		.left-icon {
			width: 30rpx;
			height: 30rpx;
			border: 1rpx solid rgba(255, 255, 255, 0.20);
			margin-right: 20rpx;
			@include animate2;
			transform: rotate(-90deg);
			-ms-transform: rotate(-90deg);
			-moz-transform: rotate(-90deg);
			-webkit-transform: rotate(-90deg);
			-o-transform: rotate(-90deg);

			&.rt45 {
				transform: rotate(0deg);
				-ms-transform: rotate(0deg);
				-moz-transform: rotate(0deg);
				-webkit-transform: rotate(0deg);
				-o-transform: rotate(0deg);
			}
		}

		.content {
			padding-left: 40rpx;

			&.input_sel_content {
				padding-left: 20rpx;
			}
		}
	}
</style>