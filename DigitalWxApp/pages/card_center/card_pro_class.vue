<template>
	<view>
		<view class="marTop"></view>
		<view class="con">

			<view class="class_list">
				<scroll-view :scroll-y="true" class="class_scroll" :scroll-into-view="choiceAll"
					scroll-with-animation="true">
					<block v-for="item,index in classList" :key="item.id">
						<view class="caListCon">
							<view
								:class="['caList',choiceIndex==index?'active':'',choiceIndex+1==index?'activeNext':'']"
								@click="choiceClass(item.id,index,true,item.label)">
								{{item.label}}
							</view>
						</view>
					</block>
					<view class="caListCon">
						<view @click="choiceClass(0,classList.length,false,'')" class="all" id="allId"
							:class="['caList',choiceIndex==classList.length?'active':'',choiceIndex+1==classList.length?'activeNext':'']">
							全部
						</view>
					</view>
					<view class="caListCon">
						<view :class="['emptycon',choiceIndex==classList.length?'activeNext':'']"></view>
					</view>
				</scroll-view>
			</view>

			<view class="class_content">
				<view class="content_scroll">
					<!-- <view v-if="showAll" class="contentLi all" @tap="scroll"
						@click="choiceClass(0,classList.length,false)">
						<view class="allimg">
							<image class="image" style="width: 48rpx;height: 48rpx;" src="../../static/all.png"
								mode="aspectFill">
							</image>
						</view>
						<view class="text">
							全部
						</view>
					</view> -->
					<view class="content_scroll_con">
						<block v-if="thridLevelClass.length>0&&classLevel==3">
							<view class="content_con" v-for="(it,inx) in thridLevelClass" :key="it.parClass">
								<view class="class_con">
									<view class="row_line" @click="toProList(it.parClass,it.className)">
										<text>{{it.className}}</text>
										<uni-icons custom-prefix="my-icon" type="my-icon-jinruxiaji" size="16"
											color="#333333">
										</uni-icons>
									</view>
								</view>
								<view v-if="it.thirdClassLs.length>0" class="level3_con">
									<view class="leveL3" @click.stop="toProList(ite.id,ite.label)"
										v-for="ite in it.thirdClassLs" :key="ite.id">
										{{ite.label}}
									</view>
								</view>
							</view>
						</block>
						<block v-else-if="secondLevelClass.length>0&&classLevel==2">
							<view class="content_con" v-for="(it,inx) in secondLevelPro" :key="it.classId">
								<view class="class_con">
									<view class="row_line" @click="toProList(it.classId,it.className)">
										<text>{{it.className}}</text>
										<uni-icons custom-prefix="my-icon" type="my-icon-jinruxiaji" size="16"
											color="#333333">
										</uni-icons>
									</view>
								</view>
								<view v-if="it.prolis.length>0" class="proLis_con">
									<view class="contentLi" @click.stop="toDetail(ite)" v-for="ite in it.prolis"
										:key="ite.Id">
										<image class="image" v-if="ite.ImageUrl" :src="ite.ImageUrl" mode="aspectFill">
										</image>
										<view v-if="ite.ImageUrl==''" class="imgUrl">
											产品照片
										</view>
										<view class="text">
											{{ite.ProName}}
										</view>
									</view>
								</view>
							</view>
						</block>
						<block v-else-if="proList.length>0&&classLevel==1" v-for="item2 in proList" :key="item2.Id">
							<view class="contentLi" @click.stop="toDetail(item2)">
								<image class="image" v-if="item2.ImageUrl" :src="item2.ImageUrl" mode="aspectFill">
								</image>
								<view v-if="item2.ImageUrl==''" class="imgUrl">
									产品照片
								</view>
								<view class="text">
									{{item2.ProName}}
								</view>
							</view>
						</block>
						<view
							v-else-if="proList.length==0&&thridLevelClass.length==0&&secondLevelClass.length==0&&status!='loading'&&classLoading!='loading'"
							class="emptyClass">
							<empty imgsrc="/static/empty/content_empty.png" txt="暂无内容"></empty>
						</view>
						<view v-else style="padding-bottom: 10rpx;width: 100%;">
							<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
							<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
							<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
							<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
							<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
						</view>
					</view>

				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import {
		getProClassList,
		getProductList,
		getClassTree
	} from '@/api/product.js'
	export default {
		data() {
			return {
				userInfo: null, //登录用户信息
				classList: null, //产品分类列表
				firstClassId: null, //第一个类别id
				proList: [], //产品列表
				page: 1, //产品页码
				pageSize: 30,
				choiceIndex: 0,
				list: ['1', '2', '3', '4', '5', '1', '2', '3', '4', '5'],
				showAll: true, //是否要显示'全部图标'
				status: 'loading', //
				choiceId: 0,
				choiceAll: '', //描点
				orgid: 0, //企业id
				cardId: 0, //名片id
				uid: 0, //用户id
				dpmap: new Map(),
				classLevel: 1,
				firstLevelClass: [], //第一级的分类
				secondLevelClass: [], //第二级的分类
				thridLevelClass: [], //第三级的分类
				firstLevelPro: [], //第一级的分类的商品
				secondLevelPro: [], //第二级的分类的商品
				thridLevelPro: [], //第三级的分类的商品
				classLoading: 'loading'


			}
		},
		onLoad(options) {
			this.orgid = parseInt(options.orgid || 0);
			this.cardId = parseInt(options.cardId || 0); //
			this.uid = parseInt(options.uid || 0); //
			this.reloadPages()
		},
		onReachBottom() {
			if (this.status != 'noMore') {
				this.page++;
				this.status = "loading";
				this.load_data()
				// console.log("现在是第几页", this.page);
			}

		},
		methods: {
			toProList(classId, className) {
				//跳转到产品列表页
				this.$store.commit('SET_PROLISTTITLE_DATA', className);
				// console.log('产品分类名称', className);
				uni.navigateTo({
					url: '/pages/card_center/card_pro_list?id=' + classId +'&uid='+this.uid+'&cardId='+this.cardId
				})
			},
			initClasstMap(node) { //设置子级标志函数
				// console.log("initClasstMap", node);
				for (let idx = 0; idx < node.children.length; idx++) {
					let curnode = node.children[idx];
					this.dpmap.set(curnode.id, curnode);
					if (curnode.hasOwnProperty("children")) {
						this.initClasstMap(curnode);
					}
				}
			},
			toDetail(item) {
				//跳转到详情页
				// console.log("当前点击商品信息", item);
				uni.navigateTo({
					url: '/pages/card_center/card_pro_detail?id=' + item.Id +'&uid='+this.uid+'&cardId='+this.cardId
				})
			},
			scroll() {
				this.$nextTick(() => {
					this.choiceAll = 'allId' //设置如果点击了产品中的全部图标，那么就跳转到类别全部处
				});
				this.choiceAll = '' //不清空再次跳到锚点位置会不起作用
			},
			async reloadPages() {
				// this.userInfo = await this.$store.dispatch("userInfo");

				let res = await getClassTree({
					OrgId: this.orgid,
					CardId: this.cardId
				})
				this.dpmap = new Map();
				// console.log("产品分类列表", res);
				this.classList = res.data;
				this.classList.forEach((val, i) => {

					if (val.children) {
						this.initClasstMap(val); //将类别的子级都取出来，获取所有类别
					}
					this.dpmap.set(val.id, val); //分别设置每个id对应它的数据
				})
				this.firstClassId = this.classList[0].id
				this.page = 1;
				this.proList = []
				await this.choiceClass(this.firstClassId, 0, true, this.classList[0].label)
			},
			async load_data(classId) {
				let data = {};
				if (classId == 0) { //根据点击的id是不是0判断点击的是不是全部按钮
					data = {
						orgId: this.orgid,
						del_flag: 0,
						pageNum: this.page,
						pageSize: this.pageSize,
						showAll: false,
						CardId: this.cardId
					}
				} else {
					data = {
						CategoryId: classId,
						orgId: this.orgid,
						del_flag: 0,
						pageNum: this.page,
						pageSize: this.pageSize - 1,
						showAll: true,
						CardId: this.cardId

					}
				}
				let rsp = await getProductList(data)
				// console.log("查询类别列表结果", rsp);
				// this.proList=rsp.data.List;
				this.proList = [...this.proList, ...rsp.data.List]
				if (rsp.data.List.length < this.pageSize) {
					this.status = 'noMore';
				} else {
					this.status = 'more';
				}
			},
			async getProLis(proLis, classId, status) { //获取该分类所有商品列表
				// console.log("proLis查询产品列表", proLis);
				let data = {};

				data = {
					CategoryId: classId,
					orgId: this.orgid,
					del_flag: 0,
					// pageNum: page,
					// pageSize: this.pageSize,
					showAll: true,
					CardId: this.cardId

				}
				let rsp = await getProductList(data)
				// console.log("查询类别列表结果", rsp);
				proLis = [...proLis, ...rsp.data.List]

				return proLis
			},
			async choiceClass(id, index, show, label) { //index==0的时候表示点击的是"全部"按钮
				// uni.showLoading({
				// 	title: '加载中'
				// })
				this.classLoading = 'loading'
				this.secondLevelPro = [] //有二级分类的产品置空
				this.secondLevelClass = [] //二级分类的列表置空
				this.thridLevelClass = [] //三级分类的列表置空
				this.proList = [] //产品列表置空

				try {
					this.choiceId = id
					this.choiceIndex = index; //控制获取点击的是第几个元素
					this.showAll = show; //控制是否显示全部图标
					let classChild = this.dpmap.get(id)
					// console.log("点击的", id, "子级分类", classChild);
					// console.log("所有产品",allProLis);
					if (this.choiceId > 0 && classChild.children && classChild.children.length > 0) {

						let allProLis = await this.getProLis([], this.choiceId, 'loading') //查询所以产品
						let feileiPro = allProLis.reduce((prev, cur) => { //将查询到的产品进行分类
							// console.log("meicide1",prev,Object.keys(prev),cur['CategoryId']);
							let cateId = cur['CategoryId'];
							if (Object.keys(prev).includes(cateId + '')) {
								prev[cateId].push(cur)
							} else {
								prev[cateId] = []
								prev[cateId].push(cur)
							}
							return prev
						}, {})
						// console.log("分好类的产品打印", feileiPro);
						if (Object.keys(feileiPro).includes(this.choiceId + '')) { //获取一级分类的
							let ls = {
								className: label,
								classId: this.choiceId,
								prolis: feileiPro[this.choiceId]
							}
							this.secondLevelPro.push(ls)
						} else {
							let ls = {
								className: label,
								classId: this.choiceId,
								prolis: []
							}
							this.secondLevelPro.push(ls)
						}
						let classLs1 = {
							parClass: this.choiceId,
							className: label,
							thirdClassLs: []
						}
						this.thridLevelClass.push(classLs1)

						this.classLevel = 2
						this.secondLevelClass = classChild.children
						let isJThird = false
						this.secondLevelClass.forEach(async (val, ix) => {

							let classChild2 = this.dpmap.get(val.id)

							if (classChild2.children && classChild2.children.length > 0) {
								isJThird = true
								this.classLevel = 3
								let classLs = {
									parClass: val.id,
									className: val.label,
									thirdClassLs: classChild2.children
								}
								this.thridLevelClass.push(classLs)
							} else {
								let classLs = {
									parClass: val.id,
									className: val.label,
									thirdClassLs: []
								}
								this.thridLevelClass.push(classLs)
							}
							if (!isJThird) {
								if (Object.keys(feileiPro).includes(val.id + '')) {
									// console.log("Object.keys(feileiPro)",Object.keys(feileiPro));
									let ls2 = {
										className: val.label,
										classId: this.choiceId,
										prolis: feileiPro[val.id]
									}
									this.secondLevelPro.push(ls2)
								}
							}
						})

					} else {
						this.classLevel = 1
						this.page = 1;
						this.proList = []
						this.status = 'loading'
						this.load_data(this.choiceId)
					}
				} catch (e) {
					//TODO handle the exception
				} finally {
					// uni.hideLoading()
					this.classLoading = 'all'
				}
				// console.log("第二级分类的", this.secondLevelPro);
				// console.log("第三级分类的", this.thridLevelClass);

			},
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFFFFF;
	}

	.marTop {
		height: 30rpx;
		width: 100%;
		position: fixed;
		background-color: #F6F6F6;
		top: 0;
		left: 0;
		z-index: 99;
	}

	.con {

		width: 100vm;

		height: 100vh;
		box-sizing: border-box;
		padding-top: 30rpx;
		padding-left: 210rpx;
		display: flex;
		flex-direction: row;
		// align-items: center;
		justify-content: flex-end;
		flex: 1;
		position: relative;


		.class_list {
			// margin-top: 20rpx;
			width: 210rpx;
			// background-color: #FFFFFF;
			height: 100vh;
			position: fixed;
			top: 30rpx;
			left: 0;
			z-index: 999;
			background-color: #F5F5F5;

			.caListCon {
				box-sizing: border-box;
				width: 100%;
				height: 100rpx;
				line-height: 100rpx;
				background-color: #FFFFFF;
			}

			.emptycon {
				width: 100%;
				height: 186rpx;
				background-color: #F5F5F5;
			}

			.class_scroll {
				// height: 800rpx;
				height: 100vh;
				box-sizing: border-box;
				padding-bottom: 20rpx;

				.caList {
					padding: 0 30rpx;
					box-sizing: border-box;
					width: 100%;
					height: 100rpx;
					line-height: 100rpx;
					font-size: 30rpx;
					text-align: center;
					text-overflow: ellipsis;
					white-space: nowrap;
					overflow: hidden;
					font-size: 26rpx;
					background-color: #F5F5F5;
				}

				.all {
					// padding: 0 30rpx;
					height: 100rpx;
					line-height: 100rpx;
					font-size: 26rpx;
					text-align: center;
					background-color: #F5F5F5;
					// margin-bottom: 20rpx;
				}

				.activeNext {
					border-radius: 0 18rpx 0 0;
				}

				.active {
					background-color: #FFFFFF !important;
					color: #50A6FA;
					font-size: 30rpx;
					border-right: none;
				}

			}


		}

		.class_content {
			width: 540rpx;
			box-sizing: border-box;
			height: 100vh;
			background-color: #FFFFFF;

			.content_scroll {
				// height: 1090rpx;
				background-color: #FFFFFF;
				display: flex;

				flex: 1;

				// line-height: 180rpx;
				.content_scroll_con {
					display: flex;
					justify-content: flex-start;
					align-items: flex-start;
					flex-wrap: wrap;
					padding-bottom: 30rpx;
					flex: 1;
					.content_con {
						width: 540rpx;

						.level3_con {
							width: 540rpx;
							box-sizing: border-box;
							padding: 0 20rpx;
							display: flex;
							justify-content: space-between;
							align-items: flex-start;
							flex-wrap: wrap;
							margin-top: -20rpx;
							margin-bottom: 10rpx;

							.leveL3 {
								width: 234rpx;
								height: 60rpx;
								margin-top: 30rpx;
								background-color: #F5F5F5;
								text-align: center;
								line-height: 60rpx;
								border-radius: 8rpx;
								text-overflow: ellipsis;
								white-space: nowrap;
								overflow: hidden;

							}
						}

						.class_con {
							width: 100%;
							box-sizing: border-box;
							padding: 20rpx;

							.row_line {
								display: flex;
								justify-content: space-between;
								align-items: center;
								height: 60rpx;
								width: 100%;
								box-sizing: border-box;
								padding: 0 20rpx;
								background-color: #F5F5F5;
								border-radius: 10rpx;
							}
						}

						.proLis_con {
							width: 100%;
							display: flex;
							justify-content: flex-start;
							align-items: flex-start;
							flex-wrap: wrap;
						}
					}
				}



				.emptyClass {
					width: 540rpx;
				}

				.contentLi {
					width: 180rpx;
					box-sizing: border-box;
					padding: 0 40rpx;
					height: 180rpx;
					// margin-bottom: 100rpx;
					display: flex;
					align-items: center;
					justify-content: center;
					flex-wrap: wrap;

					.allimg {
						width: 100rpx;
						height: 74rpx;
						display: flex;
						align-items: center;
						justify-content: center;
						margin-bottom: 20rpx;
						margin-top: 26rpx;
					}

					.imgUrl {
						width: 100rpx;
						height: 74rpx;
						line-height: 74rpx;
						margin-bottom: 20rpx;
						margin-top: 26rpx;
						color: #999999;
						font-size: 24rpx;
						// box-sizing: border-box;
						text-align: center;
						padding: 6 20rpx;
						border: 1rpx solid #999999;
						// overflow: hidden;

					}

					.image {
						width: 100rpx;
						height: 74rpx;
						margin-top: 26rpx;
						margin-bottom: 20rpx;
					}

					.text {
						width: 105rpx;
						height: 60rpx;
						line-height: 30rpx;
						font-size: 22rpx;
						color: #666666;
						text-align: center;
						display: -webkit-box;
						/*弹性伸缩盒子模型显示*/
						-webkit-box-orient: vertical;
						/*排列方式*/
						-webkit-line-clamp: 2;
						/*显示文本行数*/
						text-overflow: ellipsis;
						overflow: hidden;
						/*溢出隐藏*/
					}
				}

				.all {
					image {
						width: 73rpx;
						height: 73rpx;
					}
				}

			}





		}
	}
</style>
