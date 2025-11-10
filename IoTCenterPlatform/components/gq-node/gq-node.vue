<template>
	<view class="tree" v-if="isShow">
		<view class="tree-mask" :class="{'show':showTree}"></view>
		<view class="leo-tree tree-cnt" :class="{'show':showTree}">
			<view class="tree-bar">
				<view class="tree-bar-cancel" hover-class="hover-c" @tap="_hide">
					close
				</view>
				<view class="tree-bar-title">{{title}}</view>
				<view class="tree-bar-confirm" hover-class="hover-c" @tap="_confirm">confirm</view>
			</view>
			<view class="tree-view">
				<treeNode ref="treePop" v-for="item in data" :item="item" :key="item[defaultProps.id]+','+componentKey" :defaultProps="defaultProps">
				</treeNode>
			</view>
		</view>
	</view>
</template>
<script>
	import treeNode from './tree-compt.vue';
	export default {
		components: {
			treeNode
		},
		props: {
			data: {
				type: Array,
				default: () => {
					return []
				}
			},
			checkList:{
				type: Array,
				default: () => {
					return []
				}
			},
			checkParentList:{
				type: Array,
				default: () => {
					return []
				}
			},
			defaultProps: {
				type: Object,
				default: () => {
					return {
						id: 'id',
						children: 'children',
						label: 'label'
					}
				}
			},
			isInputSel: {
				type: Boolean,
				default: false
			},
			title: { //头
				type: String,
				default: ''
			},

		},
		provide() {
			return {
				defaultProps: this.defaultProps,
				isInputSel: this.isInputSel,
				onClickItem: this.onClickItem,
				editItem: this.editItem,
				checkParentList:this.checkParentList,
				checkList:this.checkList
				
			}
		},
		data() {
			return {
				showTree: false,
				choiceList:[],
				isShow:true,
				componentKey:''
			}
		},
		mounted() {
			this.componentKey = Math.random()
			this.choiceList=[]
			// console.log(this.componentKey,'this.componentKey');
		},
		methods: {
			reLoadKey(){
				this.componentKey = Math.random()
				this.choiceList=[]
			},
			_confirm(){
				this.$emit('_confirm', this.choiceList);
				this.showTree = false
			},
			_show() {
				this.showTree = true
			},
			_hide() {
				this.showTree = false
			},
			editItem(item) {
				this.$emit('editItem', item)
			},
			onClickItem(e) {
				// console.log(e);
				this.$emit('node-click', e);
				if(e.checked){
					this.choiceList.push(e)
				}else{
					this.choiceList=this.choiceList.filter(row=>row.checked)
				}
				// console.log(this.choiceList,'this.choiceListthis.choiceList');
			}
		}
	}
</script>
<style lang="less" scoped>
	.tree-mask {
		position: fixed;
		top: 0rpx;
		right: 0rpx;
		bottom: 0rpx;
		left: 0rpx;
		z-index: 9998;
		background-color: rgba(0, 0, 0, 0.6);
		opacity: 0;
		transition: all 0.3s ease;
		visibility: hidden;
	}

	.tree-mask.show {
		visibility: visible;
		opacity: 1;
	}

	.tree-cnt {
		position: fixed;
		top: 0rpx;
		right: 0rpx;
		bottom: 0rpx;
		left: 0rpx;
		z-index: 9999;
		top: 360rpx;
		transition: all 0.3s ease;
		transform: translateY(100%);
		background-color: rgba(22, 26, 38, 1);
		width: 100%;

		// overflow-y: scroll;
		.tree-view {
			padding: 0 30rpx;
			box-sizing: border-box;
		}
	}

	.tree-cnt.show {
		transform: translateY(0);
	}

	.tree-bar {
		background-color: rgba(22, 26, 38, 1);
		height: 100rpx;
		padding-left: 20rpx;
		padding-right: 20rpx;
		display: flex;
		justify-content: space-between;
		align-items: center;
		box-sizing: border-box;
		border-bottom-width: 1rpx !important;
		border-bottom-style: solid;
		border-bottom-color: rgba(255, 255, 255, 0.2);
		font-size: 32rpx;
		color: rgba(255, 255, 255, 1);
		line-height: 1;
	}

	.tree-view {
		position: absolute;
		top: 0rpx;
		right: 0rpx;
		bottom: 0rpx;
		left: 0rpx;
		top: 100rpx;
		background-color: rgba(22, 26, 38, 1);
		padding-top: 20rpx;
		padding-right: 20rpx;
		padding-bottom: 20rpx;
		padding-left: 20rpx;
		overflow-y: scroll;
	}
</style>