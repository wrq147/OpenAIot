<template>
	<view>
		<template v-if="curdep!=null&&curdep.parentId!=0">
			<view class="nav-mb-wrap">
				<view class="nav-mb">
					<template v-for="(navit,idx) in navlist">
						<navigator url="" @click="backDep(idx)" v-if="(idx+1)!=navlist.length" class="xi ac">
							{{navit.label}}
						</navigator>
						<uni-icons v-if="(idx+1)!=navlist.length" type="forward" size="16" color="#999999"
							style="margin: 0 10rpx;" class="xi"></uni-icons>
						<text v-if="(idx+1)==navlist.length" class="xi">{{navit.label}}</text>
					</template>
				</view>
			</view>
			<view class="border-bottom" style="height: 70rpx;"></view>
		</template>
		<view v-else class="border-top"></view>

		<view class="dept-cont" v-if="curdep!=null||userList.length>0">
			<view v-if="curdep.children!=null">
				<!-- @click="getJumpTimes" -->
				<navigator url="" v-for="(item, index) in curdep.children" :key="item.id">
					<view @click="jmp2Next(item)" class="dept-item border-bottom">
						<view @click.stop="radioClick(item)">
							<!-- <radio :disabled="isDis(item)" color="#50A6FA" style="transform:scale(0.7)" /> -->
						</view>
						<image src="../../static/mulu.png" mode="aspectFill" style="width: 68rpx;height: 68rpx;margin-right: 20rpx;"></image>
						<view class="ltxt">{{item.label}}</view>
						<uni-icons type="forward" size="22" color="#999999"></uni-icons>
					</view>
				</navigator>
			</view>
			<view v-if="userList!=null">
				<view v-if="selectMethod=='mulChoice'">
					<navigator url="" v-for="(item, index) in userList" :key="item.Id">
						<label class="user-item border-bottom" @click="radioClick(item)">
							<!-- <view> -->
							<radio :checked="ifIn(item)" color="#50A6FA" style="transform:scale(0.7)" />
							<image class="image" :src="item.Avatar" mode="aspectFill"></image>
							<!-- </view> -->
							<view class="ltxt">{{item.RealName}}</view>

						</label>

					</navigator>
				</view>
				<view v-else-if="selectMethod=='singleChoice'">
					<navigator url="" v-for="(item, index) in userList" :key="item.Id">
						<label class="user-item border-bottom" @click="singleClick(item)">
							<!-- <view> -->
							<!-- <radio :checked="ifIn(item)" color="#50A6FA" style="transform:scale(0.7)" /> -->
							<image class="image" :src="item.Avatar" mode="aspectFill"></image>
							<!-- </view> -->
							<view class="ltxt">{{item.RealName}}</view>
						</label>
					</navigator>
				</view>
			</view>

		</view>
		<empty v-if="(curdep.children.length==0||curdep.children==null)&&userList.length==0 && status!='loading'"
			imgsrc="/static/empty/data_empty.png" txt="没有可以选择的用户">
		</empty>
		<view v-if="(curdep.children.length==0||curdep.children==null)&&userList.length==0 && status=='loading'" style="padding-bottom: 10rpx;">
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
		</view>
		<view class="bottom-fix" v-if="selected.length>0">
			<view class="select-area">
				<view class="l">
					<view v-for="it in selected" class="tag" @click="radioClick(it)">
						{{it.RealName}}
						<uni-icons type="closeempty" size="12" color="#ffffff" style="margin-left: 10rpx;"></uni-icons>
					</view>
				</view>
				<button class="mini-btn" type="primary" size="mini" @click="confirmClick()">确定</button>
			</view>
		</view>
	</view>
</template>

<script>
	import {
		getDeptTree,
		moveDept
	} from '@/api/dept'
	import {
		setPagesParam
	} from '@/common/util.js'
	import {
		getUserList
	} from '@/api/user.js'
	import {
		setManager
	} from '@/api/managers.js'
	export default {
		data() {
			return {
				curdep: null,
				dpmap: new Map(),
				navlist: [],
				depid: 0,
				parentId: 0,
				selected: [],
				userList: [],
				selectMethod: null,
				JumpTimes: 0, //跳转次数
				status:'loading'//加载数据的状态
			};
		},
		async onLoad(options) {
			this.status='loading'
			// uni.showLoading({
			// 	title:'加载中'
			// })
			//根据selectMethod判断选择方式;值为singleChoice则为单选，值为mulChoice则为多选
			if (options.selectMethod) {
				this.selectMethod = options.selectMethod;
			}

			// console.log("跳转传的值", options);
			this.parentId = parseInt(options.in || "0");
			if (this.parentId != 0) {
				this.selected = this.$store.state.submitMod.formArrary.pop();
			}
			let usrInfo = await this.$store.dispatch("userInfo");
			let rsp = await getDeptTree({
				OrgId: usrInfo.OrgId
			});
			this.dpmap = new Map();
			this.dpmap.set(rsp.data[0].id, rsp.data[0]);
			this.initDeptMap(rsp.data[0]);
			if (this.parentId == 0) {
				this.curdep = rsp.data[0]
			} else {
				this.curdep = this.dpmap.get(this.parentId);
			}
			this.navlist = [];
			this.levAdd(this.curdep.id);
			//获取用户列表
			let result = await getUserList({
				deptId: this.parentId == 0 ?rsp.data[0].id : this.parentId,
				showAll: true,
				deptIdWithChildren:false,//deptIdWithChildren为false的时候表示需要通过子级筛选
			})
			this.userList = result.data.List
			// console.log("打印用户列表", this.userList);
			//获取用户列表
			// console.log("打印部门", this.curdep);
			// uni.hideLoading()
			this.status='Nomore'
		},
		methods: {
			hdImgErr: function(item) {
				if (item.Sex == 1) {
					item.Headimg = '/static/user/user_girl.png';
				} else {
					item.Headimg = '/static/user/user_man.png';
				}
			}, //没有头像是默认展品
			backDep(idx) {
				console.info(idx)
				uni.navigateBack({
					delta: (idx)
				});
			},
			levAdd(dpid) {
				let xdd = this.dpmap.get(dpid);
				if (xdd.parentId != 0) {
					this.levAdd(xdd.parentId);
				}
				this.navlist.push(xdd);
			},
			initDeptMap(node) {
				for (let idx = 0; idx < node.children.length; idx++) {
					let curnode = node.children[idx];
					this.dpmap.set(curnode.id, curnode);
					if (curnode.hasOwnProperty("children")) {
						this.initDeptMap(curnode);
					}
				}
			},
			radioClick(item) {
				// console.log("这里获取选中的值", this.selected);
				let idx = this.selected.findIndex(it => it.Id == item.Id);
				if (idx > -1) {
					this.selected.splice(idx, 1);
				} else {
					this.selected.push(item);
				}
				this.$forceUpdate();
			},

			//单选时执行的函数
			singleClick(item) {
				// uni.$emit('updateData', item);
				//直接设置要返回数据的那一页的data中的值为选中的值
				setPagesParam('onSelect', item, parseInt(this.navlist.length), 1000)
				
				//直接设置要返回数据的那一页的data中的值为选中的值
			},
			ifIn(item) {
				return this.selected.findIndex(it => it.Id == item.Id) > -1;
			},
			jmp2Next(item) {
				this.$store.state.submitMod.formArrary.push(this.selected);
				uni.navigateTo({
					url: '/pages/user/managers_add?in=' + item.id + '&selectMethod=' + this.selectMethod
				});
			},
			isDis(item) {
				return item.parentId == this.parentId || item.id == this.parentId;
			},
			async confirmClick() {
				uni.showLoading({
					title: '加载中...'
				});
				try {
					let ids = this.selected.map(it => it.Id);
					//直接设置要返回数据的那一页的data中的值为选中的值
					setPagesParam('onSelect', this.selected, parseInt(this.navlist.length), 1000)

				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}
			}
		}
	}
</script>

<style lang="scss">
	.nav-mb-wrap {
		position: fixed;
		width: 100vw;
		z-index: 99;
		// padding-bottom: 10rpx;
		// border-bottom: 1rpx solid rgba(153,153,153, 0.1);

		.nav-mb {
			background-color: #FFFFFF;
			display: flex;
			flex-direction: row;
			flex-wrap: nowrap;
			overflow-x: scroll;
			height: 70rpx;
			padding: 0 30rpx;
			align-items: center;

			.xi {
				flex-shrink: 0;
			}

			.ac {
				color: #50A6FA;
			}
		}
	}

	.dept-cont {
		background-color: #FFFFFF;

		.user-item,
		.dept-item {
			height: 130rpx;
			display: flex;
			align-items: center;
			padding: 0 30rpx;

			.image {
				width: 68rpx;
				height: 68rpx;
				display: block;
				border-radius: 50%;
			}

			.ltxt {
				margin-left: 20rpx;
				flex: 1;
			}
		}
	}

	.bottom-fix {
		position: fixed;
		z-index: 10;
		bottom: 0;
		width: 100vw;
		background-color: #FFFFFF;

		.select-area {
			display: flex;
			flex-direction: row;
			padding: 14rpx 30rpx;

			.l {
				flex: 1;
				display: flex;
				flex-direction: row;
				flex-wrap: wrap;
				align-items: center;
				margin-left: -20rpx;

				.tag {
					border-radius: 8rpx;
					background-color: #999999;
					color: #FFFFFF;
					font-size: 24rpx;
					height: 50rpx;
					padding: 0 16rpx;
					cursor: pointer;
					height: 50rpx;
					display: flex;
					align-items: center;
					margin-left: 20rpx;
				}
			}

			.mini-btn {
				background-color: #50A6FA;
			}
		}
	}
</style>
