<template>
	<view class="leo-tree">
		<treeNode v-for="item in data" :item="item" :key="item[defaultProps.id]" :defaultProps="defaultProps">
		</treeNode>
	</view>
</template>
<script>
	import treeNode from './tree-node/tree-node.vue';
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
			}
			
		},
		provide() {
			return {
				defaultProps: this.defaultProps,
				isInputSel: this.isInputSel,
				onClickItem: this.onClickItem,
				editItem: this.editItem,
			}
		},
		data() {
			return {

			}
		},
		methods: {
			editItem(item) {
				this.$emit('editItem', item)
			},
			onClickItem(e) {
				// console.log(e);
				this.$emit('node-click', e);
			}
		}
	}
</script>
<style scoped>

</style>