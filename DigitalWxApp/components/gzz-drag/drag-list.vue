<template>
	<view class="drag-box">
		<view v-for="(item,index) in dataList" :key="index" :style="{top: item.top +'px',
				height: (itemHeight - 1)+'rpx'}" class="drag-item" :class="{'drag-active': item.isActive}">
			<view :class="{'nmpev':item.isActive}" @longtap="longtap(item)" @touchstart="touchstart"
				@touchmove="touchmove" @touchend="touchend(item)">
				<slot :item="item"></slot>
			</view>
		</view>
	</view>
</template>

<script>
	export default {
		props: {
			list: {
				type: Array,
				default: () => ([])
			},
			itemHeight: {
				type: [Number],
				default: 70
			}
		},
		data() {
			return {
				activeItem: null,
				isDrag: false,
				dragTargetY: 0,
				dataList: [],
				sortIndexList: [],
			}
		},
		watch: {
			list: {
				immediate: true,
				deep: true,
				handler(list) {
					this.setList(list)
				}
			}
		},
		methods: {
			touchstart(e) {
				this.dragTargetY = e.touches[0].pageY;
			},
			longtap(item) {
				this.activeItem = item;
				this.isDrag = true;
				item.isActive = true;
				this.$emit('StartMove');
			},
			touchmove(e) {
				if (!this.isDrag) {
					return
				}
				let newY = e.touches[0].pageY;
				let d = newY - this.dragTargetY;
				this.activeItem.top += d;

				let prevIndex = this.sortIndexList[this.activeItem.index] - 1;
				let nextIndex = this.sortIndexList[this.activeItem.index] + 1;
				if (prevIndex >= 0 && d < 0) {
					let item = this.getItemByIndex(prevIndex);
					if (this.activeItem.top < item.top) {
						this.swapArray(item);
					}
				} else if (nextIndex < this.list.length && d > 0) {
					let item = this.getItemByIndex(nextIndex);
					if (this.activeItem.top > item.top) {
						this.swapArray(item);
					}
				}
				this.dragTargetY = newY;
			},
			touchend(item) {
				this.$emit('EndMove');
				if (!this.isDrag) {
					return
				}
				this.isDrag = false;
				item.isActive = false;
				this.activeItem.top = this.sortIndexList[this.activeItem.index] * this.rowHeight;
				let sortList = [];
				Array(this.dataList.length).fill(0).forEach((v, index) => {
					let tempObj = this.deepClone(this.getItemByIndex(index));
					delete tempObj.isActive;
					delete tempObj.top;
					delete tempObj.index;
					sortList.push(tempObj);
				});
				this.$emit('change', sortList);
			},
			getItemByIndex(index) {
				for (let i = 0; i < this.sortIndexList.length; i++) {
					if (this.sortIndexList[i] === index) {
						return this.dataList[i];
					}
				}
				return null;
			},
			swapArray(item) { //列表中两个元素交换位置
				let index = this.sortIndexList[this.activeItem.index];
				this.sortIndexList[this.activeItem.index] = this.sortIndexList[item.index];
				this.sortIndexList[item.index] = index;
				item.top = index * this.rowHeight;
				this.count = 0;
			},
			setList(list) {
				this.sortIndexList = [];
				this.dataList = list.map((item, index) => {
					this.sortIndexList.push(index);
					return {
						...item,
						isActive: false,
						top: index * this.rowHeight,
						index: index
					}
				})
			},
			deepClone(obj) {
				let newobj = null;
				if (typeof(obj) == 'object' && obj !== null) {
					newobj = obj instanceof Array ? [] : {};
					for (var i in obj) {
						newobj[i] = this.deepClone(obj[i])
					}
				} else {
					newobj = obj
				}
				return newobj;

			},
			isClass(o) {
				if (o === null) return "Null";
				if (o === undefined) return "Undefined";
				return Object.prototype.toString.call(o).slice(8, -1);
			}
		},
		mounted() {},
		computed: {
			rowHeight() {
				const res = uni.getSystemInfoSync();
				let screenWidth = res.screenWidth;
				if (this.itemHeight) {
					return this.itemHeight * screenWidth / 750;
				} else {
					return 0;
				}
			}
		}
	}
</script>

<style lang="scss">
	.drag-box {
		width: 100%;
		height: 100%;
		position: relative;

		.drag-item {
			width: 100%;
			text-align: center;
			transition: all 0.5s;
			// background-color: #F6F6F6;
			z-index: 1;
			position: absolute;
			.nmpev {
				pointer-events: none;
			}
		}
	}

	.drag-active {
		box-shadow: 0 8px 20px 0 #e6e6e6;
		transform: scale(1.1);
		z-index: 9 !important;
		transition: box-shadow .5s, transform .5s, top 0s !important;
	}

	@font-face {
		font-family: "HM-DS-font";
		src: url('data:font/ttf;charset=utf-8;base64,AAEAAAANAIAAAwBQRkZUTZMIf78AAAbkAAAAHEdERUYAKQAKAAAGxAAAAB5PUy8yPThJ0gAAAVgAAABgY21hcAAP6q4AAAHIAAABQmdhc3D//wADAAAGvAAAAAhnbHlmwVHingAAAxgAAADoaGVhZCA5nLMAAADcAAAANmhoZWEG1QOFAAABFAAAACRobXR4DAAAwAAAAbgAAAAQbG9jYQB0AAAAAAMMAAAACm1heHABEgBZAAABOAAAACBuYW1lXoIBAgAABAAAAAKCcG9zdE5kUbQAAAaEAAAAOAABAAAAAQAAyh5W8F8PPPUACwQAAAAAAN6yLFsAAAAA3rIsWwDAAJAC9wJwAAAACAACAAAAAAAAAAEAAAOA/4AAXAQAAAAAAAL3AAEAAAAAAAAAAAAAAAAAAAAEAAEAAAAEAE0ABAAAAAAAAgAAAAoACgAAAP8AAAAAAAAABAQAAZAABQAAAokCzAAAAI8CiQLMAAAB6wAyAQgAAAIABQMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUGZFZADA5wPnAwOA/4AAAAPcAIAAAAABAAAAAAAAAAAAAAAgAAEEAAAAAAAAAAQAAAAEAADAAAAAAwAAAAMAAAAcAAEAAAAAADwAAwABAAAAHAAEACAAAAAEAAQAAQAA5wP//wAA5wP//xkAAAEAAAAAAAABBgAAAQAAAAAAAAABAgAAAAIAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAdAAAAAQAwACQAvcCcAAnADQAQABMAAABFjI+AS8BJiIPAQYeATY/AREnLgEOAR8EMz8CNjQmBg8BERclISIuATYzITIWFAYjFyEiJjQ2MyEyFhQGByEiJjQ2MyEyFhQGAXEGEgwBBlUGEwZWBgENEgYwMAYSDQEGVwQFBQUEB1UGDRIGMDABbf7yCw4BDwsBDgoPDwsB/vIKDw8KAQ4KDw8K/vIKDw8KAQ4KDw8B8AcLEgZdBwddBhIMAQY1/rg1BgEMEQdeAwIBAgVdBxEMAQY1AUg1FA8UDw8UD5sPFA8PFA+bDxQPDxQPAAAAABIA3gABAAAAAAAAABMAKAABAAAAAAABAAgATgABAAAAAAACAAcAZwABAAAAAAADAAgAgQABAAAAAAAEAAgAnAABAAAAAAAFAAsAvQABAAAAAAAGAAgA2wABAAAAAAAKACsBPAABAAAAAAALABMBkAADAAEECQAAACYAAAADAAEECQABABAAPAADAAEECQACAA4AVwADAAEECQADABAAbwADAAEECQAEABAAigADAAEECQAFABYApQADAAEECQAGABAAyQADAAEECQAKAFYA5AADAAEECQALACYBaABDAHIAZQBhAHQAZQBkACAAYgB5ACAAaQBjAG8AbgBmAG8AbgB0AABDcmVhdGVkIGJ5IGljb25mb250AABpAGMAbwBuAGYAbwBuAHQAAGljb25mb250AABSAGUAZwB1AGwAYQByAABSZWd1bGFyAABpAGMAbwBuAGYAbwBuAHQAAGljb25mb250AABpAGMAbwBuAGYAbwBuAHQAAGljb25mb250AABWAGUAcgBzAGkAbwBuACAAMQAuADAAAFZlcnNpb24gMS4wAABpAGMAbwBuAGYAbwBuAHQAAGljb25mb250AABHAGUAbgBlAHIAYQB0AGUAZAAgAGIAeQAgAHMAdgBnADIAdAB0AGYAIABmAHIAbwBtACAARgBvAG4AdABlAGwAbABvACAAcAByAG8AagBlAGMAdAAuAABHZW5lcmF0ZWQgYnkgc3ZnMnR0ZiBmcm9tIEZvbnRlbGxvIHByb2plY3QuAABoAHQAdABwADoALwAvAGYAbwBuAHQAZQBsAGwAbwAuAGMAbwBtAABodHRwOi8vZm9udGVsbG8uY29tAAAAAAIAAAAAAAAACgAAAAAAAQAAAAAAAAAAAAAAAAAAAAAABAAAAAEAAgECDXR1b2Rvbmd3ZWl6aGkAAAAB//8AAgABAAAADAAAABYAAAACAAEAAwADAAEABAAAAAIAAAAAAAAAAQAAAADVpCcIAAAAAN6yLFsAAAAA3rIsWw==') format('truetype');
	}

	.iconfont {
		font-family: "HM-DS-font" !important;
		font-style: normal;

		&.icon-drag {
			&:before {
				content: "\e703";
			}
		}

	}
</style>
