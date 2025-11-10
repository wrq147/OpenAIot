<template>
	<page-meta :page-style="`overflow:${pageScrollFlag?'hidden':'visible'}`"></page-meta>
	<view>
		<!-- <hx-navbar :config="navconfig" @clickBtn="onClickBtn" @searchClick="jmp2Search">
		</hx-navbar> -->
		<!-- 树状结构 -->
		<view class="nav-mb-wrap" v-if="classParId!=0">
			<view class="nav-mb">
				<template v-for="(navit,idx) in navlist">
					<!-- {{navit.id+navit.label}} -->
					<navigator url="" @click="backDep(idx+1)" v-if="(idx+1)!=navlist.length" class="xi ac">
						{{navit.label}}
					</navigator>
					<uni-icons v-if="(idx+1)!=navlist.length" type="forward" size="16" color="#999999"
						style="margin: 0 10rpx;" class="xi"></uni-icons>
					<text v-if="(idx+1)==navlist.length" class="xi">{{navit.label}}</text>
				</template>
			</view>
		</view>

		<!-- <view style="height: 70rpx;" v-if="curdep!=null&&curdep.ParentId!=0"></view> -->
		<view class="page-contain">
			<view v-if="curdep==null" style="padding-top: 60rpx;">
				<uni-load-more iconType="circle" :showText="false" status="loading" />
			</view>

			<view v-else-if="curdep.length==0" :class="[parseInt(navlist.length)==0?'a':'marginSet']">
				<empty imgsrc="/static/empty/data_empty.png" txt="暂无数据"></empty>
			</view>

			<view v-else-if="curdep.length>0">
				<view :class="[parseInt(navlist.length)==0?'a':'marginSet']">
					<drag-list ref="dragRf" :list="curdep" @change="sortChange" :itemHeight="110"
						@StartMove="this.pageScrollFlag=true" @EndMove="this.pageScrollFlag=false">
						<template slot-scope="{item}">
							<view class="item-field">
								<navigator class="item-nav" :url="'/pages/user/pro_class?id='+item.Id"
									animation-type="none">
									<view class="l">
										<image src="../../static/fenlei.png" mode="aspectFill"
											style="width: 68rpx;height: 68rpx;margin-right: 20rpx;"></image>
										<text>{{item.CategoryName}}</text>
									</view>
									<view class="r">
										<uni-icons type="forward" size="22" color="#999999"></uni-icons>
									</view>
								</navigator>
							</view>
							<view style="height: 20rpx;"></view>
						</template>
					</drag-list>
					<!-- <view :style="{'height':depTop}"></view> -->
				</view>
			</view>
			<view v-else style="padding-bottom: 10rpx;">
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			</view>
		</view>
		<view class="bottom-fix">
			<!-- <navigator class="addmem-ss border-right" :url="'/pages/depman/user-yq?dp='+curdep.id">邀请员工</navigator> -->
			<navigator v-if="navlist.length>=0&&navlist.length<=2" class="add-ss border-right" url="" @click="addDept">{{classParId==0?'添加分类':'添加子级分类'}}</navigator>
			<navigator v-if="classParId!=0" class="setdep-ss" url="" @click="onMore">更多管理</navigator>
			<!-- <navigator class="setdep-ss" url="" @click="onImort" v-else>移入部门</navigator> -->
		</view>
	</view>
</template>

<script>
	import {
		getProClassList,
		sortProClass,
		addProClass,
		deleteProClass,
		getClassTree
	} from '@/api/product.js'
	import {
		getUserList
	} from '@/api/user'
	import {
		reloadPrePage
	} from '@/common/util.js'
	import dragList from '@/components/gzz-drag/drag-list.vue';
	export default {
		components: {
			dragList
		},
		data() {
			return {
				navconfig: {
					leftButton: [{
						key: 'btn1',
						txt: '关闭',
						position: null
					}],
					search: {
						value: '',
						placeholder: '请输入关键词',
						disabled: true
					}
				},
				curdep: null,
				navlist: [], //树状结构所需元素
				// depid: 0,
				classParId: 0,
				pageScrollFlag: false,
				usrInfo: {}, //个人信息
				dpmap: new Map(),
				xdd: null,
			};
		},
		onLoad: function(options) {
			// this.depid = parseInt(options.id || "0");
			this.classParId = parseInt(options.id || "0"); //取每次跳转到新的分类处的id作为父级类别id
			// console.log("父id是什么？", this.classParId);
			this.reloadpage();

			// console.log("分类列表信息", this.curdep);
		},
		computed: {

		},
		// onShow() {
		// 	this.reloadpage();
		// },
		methods: {

			levAdd(id) {
				this.xdd = this.dpmap.get(id);
				//判断当前的id是否为0，为0表示没有父级id，不为0则可以查询到父级
				if (this.xdd) {
					this.navlist.unshift(this.xdd); //在最前面加入父级类名
					if (this.xdd.parentId != 0) {
						this.xdd = this.levAdd(this.xdd.parentId);
					}

				}
				// console.log("分类树this.navlist", this.navlist);
			},
			initClasstMap(node) {
				for (let idx = 0; idx < node.children.length; idx++) {
					let curnode = node.children[idx];
					this.dpmap.set(curnode.id, curnode);
					if (curnode.hasOwnProperty("children")) {
						this.initClasstMap(curnode);
					}
				}
			},

			// jmp2Search() {
			// 	uni.navigateTo({
			// 		url: '/pages/depman/user-search',
			// 		animationType: "none"
			// 	});
			// },
			//产品分类排序
			async sortChange(list) {
				let usrInfo = await this.$store.dispatch("userInfo");
				let idlist = list.map(x => {
					return x.Id;
				});
				let data = {
					idList: idlist,
					orgId: usrInfo.OrgId
				}
				let res = await sortProClass(data);

			},
			async reloadpage(name) {
				if (name == 'del') { //点击删除后更新产品分类
					this.$forceUpdate();
				}
				if (name == 'edit') { //点击修改产品后根性产品分类
					this.$forceUpdate();
				}
				if (name == 'add') { //点击添加产品后更新产品分类
					this.$forceUpdate();
				}
				// uni.showLoading({
				// 	title: '加载中'
				// })
				try {
					let usrInfo = await this.$store.dispatch("userInfo");
					this.usrInfo = usrInfo;
					this.navlist = [];
					let queryId = {
						OrgId: usrInfo.OrgId,
						parentId: this.classParId
					};
					let rsp = await getProClassList(queryId);
					this.curdep = rsp.data;

					let treeRes = await getClassTree({
						OrgId: this.usrInfo.OrgId,
					});
					// console.log("产品分类列表",treeRes);
					for (let j = 0; j < treeRes.data.length; j++) {
						if (treeRes.data[j].children) {
							this.initClasstMap(treeRes.data[j]); //将类别的子级都取出来，获取所有类别
						}
						this.dpmap.set(treeRes.data[j].id, treeRes.data[j]); //分别设置每个id对应它的数据
					}
					this.levAdd(this.classParId); //
				} catch (err) {
					//TODO handle the exception
					console.log('异常', err);
				} finally {
					// setTimeout(() => {
					// 	uni.hideLoading();
					// }, 500)
				}
			},

			// hdImgErr: function(item) {
			// 	if (item.Sex == 1) {
			// 		item.Headimg = '/static/user/user_girl.png';
			// 	} else {
			// 		item.Headimg = '/static/user/user_man.png';
			// 	}
			// },
			backDep(idx) { //头部树状索引
				uni.navigateBack({
					delta: (idx + 1)
				});
			},
			addDept() {
				uni.navigateTo({
					url: '/pages/user/class_edit?classParId=' + this.classParId,
				});
			},
			onMore() {
				uni.showActionSheet({
					itemList: ['修改当前分类名称', '删除当前分类'],
					success: res => {
						if (res.tapIndex == 0) {
							// console.log("当前类名",this.navlist[this.navlist.length-1].label);;
							this.$store.state.submitMod.formArrary.push(this.navlist[this.navlist.length - 1])
							uni.navigateTo({
								url: '/pages/user/class_edit?id=' + this.classParId + '&className=' +
									encodeURIComponent(JSON.stringify(this.navlist[this.navlist
										.length - 1].label)),
							});
						} else if (res.tapIndex == 1) {
							uni.showLoading({
								title: '加载中...'
							});
							deleteProClass({
								Id: this.classParId,
								orgId: this.usrInfo.orgId
							}).then(rsp => {
								uni.hideLoading();
								reloadPrePage(1, "del");
								uni.showToast({
									icon: 'success',
									title: '删除成功',
									duration: 1500
								});
								setTimeout(() => {
									uni.navigateBack();
								}, 1500);
							}).catch(err => {
								uni.hideLoading();
							});
						}

					}
				});
			}
		}
	}
</script>

<style lang="scss">
	.page-contain {
		padding: 10rpx 30rpx 10rpx 30rpx;
		border-top: 1rpx solid rgba(153, 153, 153, 0.1);

		.marginSet {
			margin: 60rpx 0;
		}
	}

	page {
		background-color: #FFFFFF;
	}

	.nav-mb-wrap {
		position: fixed;
		width: 100vw;
		z-index: 99;
		border-bottom: 1rpx solid rgba(153, 153, 153, 0.1);

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


	.item-field {
		background: #FFFFFF;
		border-radius: 6rpx;
		border-bottom: 1rpx solid rgba(153, 153, 153, 0.1);

		.item-nav {
			display: flex;
			flex-direction: row;
			justify-content: space-between;
			align-items: center;
			height: 90rpx;
			padding: 0 30rpx;

			.l {
				font-size: 28rpx;
				font-weight: bold;
				color: #333;
				display: flex;
				align-items: center;
			}
		}

	}

	.u-item {
		height: 140rpx;
		background-color: #FFFFFF;
		display: flex;
		flex-direction: row;
		align-items: center;
		padding: 10rpx 30rpx;

		.ximg {
			display: flex;
			overflow: hidden;
			width: 112rpx;
			height: 112rpx;
			border-radius: 50%;
		}

		.c {
			flex: 1;
			padding-left: 20rpx;

			.tp {
				display: flex;
				flex-direction: row;
				align-items: center;

				.name {
					font-size: 30rpx;
					color: #333333;
					margin-right: 20rpx;
				}
			}

			.bm {
				display: flex;
				flex-direction: row;
				align-items: center;
				margin-top: 20rpx;
				color: #999;
				font-size: 24rpx;
			}

		}
	}


	.bottom-fix {
		position: fixed;
		// padding: 30rpx 0;
		z-index: 10;
		bottom: 0;
		width: 100vw;
		display: flex;
		flex-direction: row;
		background-color: #FFFFFF;
		box-shadow: 0px 0px 8px rgba(0, 0, 0, 0.1);


		.addmem-ss,
		.add-ss,
		.setdep-ss {
			display: flex;
			height: 100rpx;
			flex: 1;
			color: #50A6FA;
			font-size: 28rpx;
			background-color: #FFFFFF;
			justify-content: center;
			align-items: center;
		}
	}
</style>
